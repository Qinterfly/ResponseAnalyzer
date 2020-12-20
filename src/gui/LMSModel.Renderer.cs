using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace ResponseAnalyzer
{
    public partial class LMSModel
    {
        public void initializeGL()
        {
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            // Blending
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            // Z-buffer
            GL.ClearDepth(1.0f);
            GL.DepthMask(true);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.DepthTest);
            // Smoothing
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);
            // Sizes
            GL.PointSize(DrawOptions.pointSize);
            GL.LineWidth(DrawOptions.lineWidth);
            // Project
            projection_ = Matrix4.CreateOrthographic(glControl_.Width, glControl_.Height, DrawOptions.zNear, DrawOptions.zFar);
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
        }

        // Draw all the elements
        public void draw()
        {
            if (!isCongruent())
                return;
            // Preparing the window
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.PolygonMode(MaterialFace.FrontAndBack, polygonMode_);
            // Drawing the components
            shader_.Use();
            Matrix4 model = modelRotation_ * modelScale_ * modelTranslation_;
            shader_.SetMatrix4("model", model);
            shader_.SetMatrix4("view", Matrix4.Identity);
            shader_.SetMatrix4("projection", projection_);
            int colorLocation = shader_.GetUniformLocation("definedColor");
            int VAO, EBO;
            int sizeElement;
            int nVertices = 0;
            foreach (string component in componentNames_)
            {
                VAO = componentBuffers_.vertexBufferObject[component];
                GL.BindBuffer(BufferTarget.ArrayBuffer, VAO);
                int attrib = shader_.GetAttribLocation("inPosition");
                GL.VertexAttribPointer(attrib, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
                GL.Uniform4(colorLocation, componentSet_.colors[component]);
                nVertices = componentSet_.vertices[component].Length / 3;
                GL.DrawArrays(PrimitiveType.Points, 0, nVertices);
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
            }
            glControl_.SwapBuffers();
        }
       

        public static class DrawOptions
        {
            public const float pointSize = 8.0f;
            public const float lineWidth = 1.25f;
            public const float defaultScale = 200.0f;
            public const float zNear = -10.0f;
            public const float zFar = 10.0f;
        }

        public enum Views
        {
            FRONT, BACK,
            UP, DOWN, LEFT, RIGHT,
            ISOMETRIC
        }
    }
}
