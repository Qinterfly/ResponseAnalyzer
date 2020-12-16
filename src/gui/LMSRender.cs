using System;
using System.Collections.Generic;
using LMSTestLabAutomation;
using OpenTK.Graphics.OpenGL4;

namespace ResponseAnalyzer
{
    
    // Defining types
    using StringDictionary = Dictionary<string, Dictionary<string, uint>>;
    using ArrayDictionary = Dictionary<string, Array>;
    using ElementDictionary = Dictionary<ElementType, Dictionary<string, Array>>;
    public enum ElementType { LINES, TRIAS, QUADS }

    public class LMSRender
    {
        // Constructor
        public LMSRender()
        {
            elementTypes_ = Enum.GetValues(typeof(ElementType));
            mapElements = new Dictionary<ElementType, PrimitiveType>();
            mapElements[ElementType.LINES] = PrimitiveType.Lines;
            mapElements[ElementType.TRIAS] = PrimitiveType.Triangles;
            mapElements[ElementType.QUADS] = PrimitiveType.Quads;
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
            Array X, Y, Z, rotX, rotY, rotZ;
            Array nodeNamesA, nodeNamesB, nodeNamesC, nodeNamesD;
            componentNames_ = new List<string>();
            componentSet_ = new ComponentGeometry(elementTypes_);
            componentBuffers_ = new ComponentBufferPointers();
            Array componentNames = geometry.ComponentNames;
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
                // Nodal angles
                geometry.ComponentNodesValues(component, nodeNames, out X, out Y, out Z, out rotX, out rotY, out rotZ);
                double[,] angles = new double[nNodes, 3];
                float[] vertices = new float[nNodes * 3];
                int insertInd = 0;
                double tX, tY, tZ;
                for (int iNode = 0; iNode != nNodes; ++iNode)
                {
                    tX = (double)X.GetValue(iNode);
                    tY = (double)Y.GetValue(iNode);
                    tZ = (double)Z.GetValue(iNode);
                    //vertices[insertInd] = (float)tX;
                    //vertices[insertInd + 1] = (float)tY;
                    //vertices[insertInd + 2] = (float)tZ;
                    vertices[insertInd] = (float)tX;
                    vertices[insertInd + 1] = (float)tZ;
                    vertices[insertInd + 2] = (float)tY;
                    angles[iNode, 0] = (double)rotX.GetValue(iNode);
                    angles[iNode, 1] = (double)rotY.GetValue(iNode);
                    angles[iNode, 2] = (double)rotZ.GetValue(iNode);
                    insertInd = insertInd + 3;
                }
                normalizeVertices(vertices);
                componentSet_.vertices.Add(component, vertices);
                componentSet_.nodeAngles.Add(component, angles);
                // Lines 
                geometry.ComponentLines(component, out nodeNamesA, out nodeNamesB);
                int nLines = nodeNamesA.Length;
                uint[] linesInd = new uint[nLines * 2];
                insertInd = 0;
                for (int i = 0; i != nLines; ++i)
                {
                    linesInd[insertInd    ] = mapNodes[(string)nodeNamesA.GetValue(i)];
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
                    triasInd[insertInd    ] = mapNodes[(string)nodeNamesA.GetValue(i)];
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
                    quadsInd[insertInd    ] = mapNodes[(string)nodeNamesA.GetValue(i)];
                    quadsInd[insertInd + 1] = mapNodes[(string)nodeNamesB.GetValue(i)];
                    quadsInd[insertInd + 2] = mapNodes[(string)nodeNamesC.GetValue(i)];
                    quadsInd[insertInd + 3] = mapNodes[(string)nodeNamesD.GetValue(i)];
                    insertInd = insertInd + 4;
                }
                componentSet_.elementData[ElementType.QUADS].Add(component, quadsInd);
                // Generate buffers for rendering
                generateBuffers(component);
            }
        }

        public void generateBuffers(string componentName)
        {
            // Creating a vertex buffer object
            int vertexBufferObject = GL.GenBuffer();
            float[] vertices = (float[])componentSet_.vertices[componentName];
            int nVertices = vertices.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, nVertices * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            // Compiling a shader
            shader_ = new Shader("../../shaders/shader.vert", "../../shaders/shader.frag");
            shader_.Use();
            componentBuffers_.vertexBufferObject.Add(componentName, vertexBufferObject);
             //indices = null;
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
            // Vertex attributes pointers
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        }

        public void setControl(OpenTK.GLControl glControl)
        {
            glControl_ = glControl;
        }

        private void normalizeVertices(float[] vertices)
        {
            // Finding the maximum and minimum of the nodal coordinates
            // (:, 1) -- minimum, (:, 2) -- maximum
            float[,] limits = new float[3, 2];
            float tempVal;
            for (int i = 0; i != 3; ++i) {
                tempVal = vertices[0];
                for (int j = 0; j != 2; ++j)
                    limits[i, j] = tempVal;
            }            
            int nVertices = vertices.Length;
            for (int i = 3; i != nVertices; i = i + 3)
            {
                for (int j = 0; j != 3; ++j)
                {
                    tempVal = vertices[i + j];
                    if (tempVal < limits[j, 0]) limits[j, 0] = tempVal;
                    if (tempVal > limits[j, 1]) limits[j, 1] = tempVal;
                }
            }
            // Calculating the range 
            // Min -- (:, 0), Delta -- (:, 1) 
            float[] shift = new float[3];
            for (int i = 0; i != 3; ++i) 
            { 
                limits[i, 1] = limits[i, 1] - limits[i, 0];
                shift[i] = -1.0f;
                if (Math.Abs(limits[i, 1]) <= float.Epsilon) {
                    limits[i, 1] = 1.0f;
                    shift[i] = 0.0f;
                }
            }
            // Normalizing coordinates
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
            GL.Clear(ClearBufferMask.ColorBufferBit); // Clear screen by using the onLoad color set
            // Drawing components
            shader_.Use();
            foreach (string component in componentNames_)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, componentBuffers_.vertexBufferObject[component]);
                foreach (ElementType type in elementTypes_)
                {
                    int EBO = componentBuffers_.elements[component][(int)type];
                    if (EBO != 0)
                    {
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
                        int nElements = componentSet_.elementData[type][component].Length;
                        GL.DrawElements(mapElements[type], nElements, DrawElementsType.UnsignedInt, 0);
                    }
                }
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
        private Dictionary<ElementType, PrimitiveType> mapElements;
    }

    public class ComponentGeometry
    {
        public ComponentGeometry(Array elementTypes)
        {
            nodeNames = new StringDictionary();
            nodeAngles = new ArrayDictionary();
            elementData = new ElementDictionary();
            vertices = new ArrayDictionary();
            foreach (ElementType type in elementTypes)
                elementData.Add(type, new Dictionary<string, Array>());
        }
        public StringDictionary nodeNames;
        public ArrayDictionary nodeAngles;
        public ElementDictionary elementData;
        public ArrayDictionary vertices;
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
}
