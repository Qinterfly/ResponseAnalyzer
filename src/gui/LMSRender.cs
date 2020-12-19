﻿using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using LMSTestLabAutomation;


namespace ResponseAnalyzer
{

    // Defining types
    using StringDictionary = Dictionary<string, Dictionary<string, uint>>;
    using ArrayDictionary = Dictionary<string, Array>;
    using ElementDictionary = Dictionary<ElementType, Dictionary<string, Array>>;
    using ColorDictionary = Dictionary<string, Color4>;
    public enum ElementType { QUADS, TRIAS, LINES}

    public class LMSRender
    {
        // Constructor
        public LMSRender()
        {
            elementTypes_ = Enum.GetValues(typeof(ElementType));
            // Binding the element types with the rendering ones
            mapElements_ = new Dictionary<ElementType, PrimitiveType>();
            mapElements_[ElementType.LINES] = PrimitiveType.Lines;
            mapElements_[ElementType.TRIAS] = PrimitiveType.Triangles;
            mapElements_[ElementType.QUADS] = PrimitiveType.Quads;
            // Defining all the colors for further selection
            availableColors_ = new List<Color4>()
            {
                Color4.Blue, Color4.Green, Color4.Yellow,
                Color4.Red, Color4.Orange, Color4.Purple,
                Color4.Aquamarine, Color4.Coral, Color4.Chocolate
            };
            // Transformations
            position_ = Vector3.Zero;
            modelScale_ = Matrix4.CreateScale(DrawParameters.defaultScale, DrawParameters.defaultScale, 0.0f);
            modelTranslation_ = Matrix4.Identity;
            modelRotation_ = Matrix4.Identity;
        }

        // Check if it's all the data is correct
        public bool isCongruent()
        {
            return componentNames_.Count != 0 && glControl_ != null;
        }

        // Retreiving drawing objects from the user specified geometry
        public void setGeometry(IGeometry geometry)
        {
            if (geometry == null)
                return;
            Array X, Y, Z, rotXY, rotXZ, rotYZ;
            Array nodeNamesA, nodeNamesB, nodeNamesC, nodeNamesD;
            componentNames_ = new List<string>();
            componentSet_ = new ComponentGeometry(elementTypes_);
            componentBuffers_ = new ComponentBufferPointers();
            Array componentNames = geometry.ComponentNames;
            int nColors = availableColors_.Count;
            int indexCurrentColor = 0;
            Vector4d globalNodeLocation = Vector4d.UnitW;
            Matrix4d transform = Matrix4d.Identity;
            // Limits of the coordinates
            float[,] limits = new float[3, 2];
            float tempVal = float.MaxValue;
            for (int i = 0; i != 3; ++i) {
                limits[i, 0] = tempVal;
                limits[i, 1] = -tempVal;
            }
            foreach (string component in componentNames)
            {
                componentNames_.Add(component);
                Array nodeNames = geometry.ComponentNodeNames[component];
                int nNodes = nodeNames.Length;
                // Component node names
                Dictionary<string, uint> mapNodes = new Dictionary<string, uint>();
                for (uint iNode = 0; iNode != nNodes; ++iNode)
                    mapNodes.Add((string)nodeNames.GetValue(iNode), iNode);
                componentSet_.nodeNames.Add(component, mapNodes);
                // Component positions
                double tX, tY, tZ, tRotXY, tRotXZ, tRotYZ;
                geometry.ComponentValues(component, out tX, out tY, out tZ, out tRotXY, out tRotXZ, out tRotYZ);
                double[] componentPosition = new double[6] { tX, tY, tZ, tRotXY, -tRotXZ, tRotYZ }; // (!) -rotY
                componentSet_.positions_.Add(component, componentPosition);
                // Nodal positions and angles of the local coordinate systems
                geometry.ComponentNodesValues(component, nodeNames, out X, out Y, out Z, out rotXY, out rotXZ, out rotYZ);
                double[,] angles = new double[nNodes, 3];
                float[] vertices = new float[nNodes * 3];
                int insertInd = 0;
                for (int iNode = 0; iNode != nNodes; ++iNode)
                {
                    globalNodeLocation[0] = (double)X.GetValue(iNode);
                    globalNodeLocation[1] = (double)Y.GetValue(iNode);
                    globalNodeLocation[2] = (double)Z.GetValue(iNode);
                    globalNodeLocation[3] = 1.0;
                    // Conversion from the component's local to the global coordinate system
                    transform = Matrix4d.CreateRotationX(componentPosition[5])
                              * Matrix4d.CreateRotationY(componentPosition[4])
                              * Matrix4d.CreateRotationZ(componentPosition[3]);
                    transform *= Matrix4d.CreateTranslation(componentPosition[0], componentPosition[1], componentPosition[2]);
                    globalNodeLocation = Vector4d.Transform(globalNodeLocation, transform);
                    for (int k = 0; k != 3; ++k)
                        vertices[insertInd + k] = (float)globalNodeLocation[k];
                    // Saving the nodal angles
                    angles[iNode, 0] = (double)rotXY.GetValue(iNode);
                    angles[iNode, 1] = -(double)rotXZ.GetValue(iNode); // (!) -rotY
                    angles[iNode, 2] = (double)rotYZ.GetValue(iNode);
                    insertInd = insertInd + 3;
                }
                // Finding the maximum and minimum of the nodal coordinates { (:, 1) -- minimum, (:, 2) -- maximum }
                int lenVertices = vertices.Length;
                for (int i = 0; i != lenVertices; i = i + 3)
                {
                    for (int j = 0; j != 3; ++j)
                    {
                        tempVal = vertices[i + j];
                        if (tempVal < limits[j, 0]) limits[j, 0] = tempVal;
                        if (tempVal > limits[j, 1]) limits[j, 1] = tempVal;
                    }
                }
                componentSet_.vertices.Add(component, vertices);
                componentSet_.nodeAngles.Add(component, angles);
                // Lines 
                geometry.ComponentLines(component, out nodeNamesA, out nodeNamesB);
                int nLines = nodeNamesA.Length;
                uint[] linesInd = new uint[nLines * 2];
                insertInd = 0;
                for (int i = 0; i != nLines; ++i)
                {
                    linesInd[insertInd] = mapNodes[(string)nodeNamesA.GetValue(i)];
                    linesInd[insertInd + 1] = mapNodes[(string)nodeNamesB.GetValue(i)];
                    insertInd = insertInd + 2;
                }
                componentSet_.elementData[ElementType.LINES].Add(component, linesInd);
                // Triangles
                geometry.ComponentTrias(component, out nodeNamesA, out nodeNamesB, out nodeNamesC);
                int nTrias = nodeNamesA.Length;
                uint[] triasInd = new uint[nTrias * 3];
                insertInd = 0;
                for (int i = 0; i != nTrias; ++i)
                {
                    triasInd[insertInd] = mapNodes[(string)nodeNamesA.GetValue(i)];
                    triasInd[insertInd + 1] = mapNodes[(string)nodeNamesB.GetValue(i)];
                    triasInd[insertInd + 2] = mapNodes[(string)nodeNamesC.GetValue(i)];
                    insertInd = insertInd + 3;
                }
                componentSet_.elementData[ElementType.TRIAS].Add(component, triasInd);
                // Quads
                geometry.ComponentQuads(component, out nodeNamesA, out nodeNamesB, out nodeNamesC, out nodeNamesD);
                int nQuads = nodeNamesA.Length;
                uint[] quadsInd = new uint[nQuads * 4];
                insertInd = 0;
                for (int i = 0; i != nQuads; ++i)
                {
                    quadsInd[insertInd] = mapNodes[(string)nodeNamesA.GetValue(i)];
                    quadsInd[insertInd + 1] = mapNodes[(string)nodeNamesB.GetValue(i)];
                    quadsInd[insertInd + 2] = mapNodes[(string)nodeNamesC.GetValue(i)];
                    quadsInd[insertInd + 3] = mapNodes[(string)nodeNamesD.GetValue(i)];
                    insertInd = insertInd + 4;
                }
                componentSet_.elementData[ElementType.QUADS].Add(component, quadsInd);
                // Generate buffers for rendering
                generateBuffers(component);
                // Specifying colors
                indexCurrentColor = indexCurrentColor >= nColors - 1 ? 0 : indexCurrentColor + 1;
                componentSet_.colors.Add(component, availableColors_[indexCurrentColor]);
            }
            // Calculating the range of the coordinates { Min -- (:, 0), Delta -- (:, 1) }
            float[] shift = new float[3];
            for (int i = 0; i != 3; ++i)
            {
                limits[i, 1] = limits[i, 1] - limits[i, 0];
                shift[i] = -1.0f;
                if (Math.Abs(limits[i, 1]) <= float.Epsilon)
                {
                    limits[i, 1] = 1.0f;
                    shift[i] = 0.0f;
                }
            }
            // Normalizing the coordinates
            foreach (string component in componentNames)
                normalizeVertices((float[])componentSet_.vertices[component], limits, shift);
        }

        public void generateBuffers(string componentName)
        {
            // Creating a vertex buffer object
            int vertexBufferObject = GL.GenBuffer();
            float[] vertices = (float[])componentSet_.vertices[componentName];
            int lenVertices = vertices.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, lenVertices * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            // Compiling a shader
            shader_ = new Shader("../../shaders/shader.vert", "../../shaders/shader.frag");
            shader_.Use();
            componentBuffers_.vertexBufferObject.Add(componentName, vertexBufferObject);
            Array elementTypes = Enum.GetValues(typeof(ElementType));
            int nElements = elementTypes.Length;
            int[] elementBufferPointers = new int[nElements];
            int elementBufferObject;
            foreach (ElementType type in elementTypes)
            {
                uint[] indices = (uint[])componentSet_.elementData[type][componentName];
                elementBufferObject = 0;
                if (indices.Length != 0)
                {
                    elementBufferObject = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
                }
                elementBufferPointers[(int)type] = elementBufferObject;
            }
            componentBuffers_.elements.Add(componentName, elementBufferPointers);
            // Rendering options
            GL.PointSize(DrawParameters.pointSize);
            GL.LineWidth(DrawParameters.lineWidth);
            projection_ = Matrix4.CreateOrthographic(glControl_.Width, glControl_.Height, -100.0f, 100.0f);
        }

        public void setControl(OpenTK.GLControl glControl)
        {
            glControl_ = glControl;
            initGL();
        }

        public void initGL()
        {
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            // Z-buffer
            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
            // Smoothing
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);
            // Offsets
            //GL.PolygonOffset(0.01f, 0.01f);
        }
        public void setScale(float scale)
        {
            modelScale_ *= Matrix4.CreateScale(scale, scale, 0.0f);
        }

        public void setTranslation(Vector3 displacement)
        {
            position_ += displacement;
            modelTranslation_ = Matrix4.CreateTranslation(position_);
        }

        public void setRotation(Vector3 diffRotation)
        {
            modelRotation_ *= Matrix4.CreateRotationX(diffRotation[0])
                           * Matrix4.CreateRotationY(diffRotation[1])
                           * Matrix4.CreateRotationZ(diffRotation[2]);
        }

        public void setView(Views view)
        {
            position_ = Vector3.Zero;
            modelTranslation_ = Matrix4.Identity;
            modelScale_ = Matrix4.CreateScale(DrawParameters.defaultScale, DrawParameters.defaultScale, 0.0f);
            modelRotation_ = Matrix4.Identity;
            switch (view)
            {
                case Views.FRONT:
                    setRotation(new Vector3(0.0f, 0.0f, 0.0f));
                    break;
                case Views.BACK:
                    setRotation(new Vector3(MathHelper.TwoPi, 0.0f, 0.0f));
                    break;
                case Views.UP:
                    setRotation(new Vector3(MathHelper.PiOver2, 0.0f, 0.0f));
                    break; 
                case Views.DOWN:
                    setRotation(new Vector3(-MathHelper.PiOver2, 0.0f, 0.0f));
                    break;
                case Views.LEFT:
                    setRotation(new Vector3(0.0f, MathHelper.PiOver2, 0.0f));
                    break;
                case Views.RIGHT:
                    setRotation(new Vector3(0.0f, -MathHelper.PiOver2, 0.0f));
                    break;
                case Views.ISOMETRIC:
                    setRotation(new Vector3(2.0f * MathHelper.PiOver3, 0.0f, MathHelper.PiOver4));
                    break;
            }
        }

        private void normalizeVertices(float[] vertices, float[,] limits, float[] shift)
        {
            int nVertices = vertices.Length;
            for (int i = 0; i != nVertices; i = i + 3)
            {
                for (int j = 0; j != 3; ++j)
                    vertices[i + j] = 2.0f * (vertices[i + j] - limits[j, 0]) / limits[j, 1] + shift[j];
            }
        }

        // Draw all the elements
        public void draw() {
            if (!isCongruent())
                return;
            // Preparing the window
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            // Drawing the components
            shader_.Use();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            Matrix4 model = modelRotation_ * modelScale_ * modelTranslation_;
            Matrix4 transform = model * projection_;
            shader_.SetMatrix4("transform", model);
            int colorLocation = shader_.GetUniformLocation("definedColor");
            int VAO, EBO;
            int sizeElement;
            int nVertices = 0;
            Color4 color;
            foreach (string component in componentNames_)
            {
                VAO = componentBuffers_.vertexBufferObject[component];
                GL.BindBuffer(BufferTarget.ArrayBuffer, VAO);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
                color = componentSet_.colors[component];
                GL.Uniform4(colorLocation, color);
                foreach (ElementType type in elementTypes_)
                {
                    EBO = componentBuffers_.elements[component][(int)type];
                    if (EBO != 0)
                    {
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
                        sizeElement = componentSet_.elementData[type][component].Length;
                        GL.DrawElements(mapElements_[type], sizeElement, DrawElementsType.UnsignedInt, 0);
                    }
                }
                nVertices = componentSet_.vertices[component].Length / 3;
                GL.DrawArrays(PrimitiveType.Points, 0, nVertices);
            }
            glControl_.SwapBuffers();
        }
        // Control
        private OpenTK.GLControl glControl_ = null;
        // Components data
        private List<string> componentNames_;
        private ComponentGeometry componentSet_;
        private Shader shader_;
        private ComponentBufferPointers componentBuffers_;
        private Array elementTypes_;
        private Dictionary<ElementType, PrimitiveType> mapElements_;
        // Rendering
        private List<Color4> availableColors_;
        private Matrix4 modelScale_;
        private Matrix4 modelTranslation_;
        private Matrix4 modelRotation_;
        private Matrix4 projection_;
        private Vector3 position_;
    }

    public class ComponentGeometry
    {
        public ComponentGeometry(Array elementTypes)
        {
            positions_ = new ArrayDictionary();
            nodeNames = new StringDictionary();
            nodeAngles = new ArrayDictionary();
            elementData = new ElementDictionary();
            vertices = new ArrayDictionary();
            foreach (ElementType type in elementTypes)
                elementData.Add(type, new Dictionary<string, Array>());
            colors = new ColorDictionary();
        }
        public ArrayDictionary positions_;
        public StringDictionary nodeNames;
        public ArrayDictionary nodeAngles;
        public ElementDictionary elementData;
        public ArrayDictionary vertices;
        public ColorDictionary colors;
    }

    public class ComponentBufferPointers
    {
        public ComponentBufferPointers()
        {
            vertexBufferObject = new Dictionary<string, int>();
            elements = new Dictionary<string, int[]>();
        }
        public Dictionary<string, int> vertexBufferObject;
        public Dictionary<string, int[]> elements;
    }

    public static class DrawParameters 
    {
        public const float pointSize = 8.0f;
        public const float lineWidth = 1.0f;
        public const float defaultScale = 0.8f;
    }

    public enum Views 
    { 
        FRONT, BACK,
        UP, DOWN, LEFT, RIGHT, 
        ISOMETRIC
    }
}
