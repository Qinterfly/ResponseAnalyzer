using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using QuickFont;

namespace ResponseAnalyzer
{
    public class CoordinateSystem
    {
        public CoordinateSystem()
        {
            double triangleHeight = tetraSide * Math.Sqrt(3.0 / 2.0);
            float halfSideTetra = (float) (tetraSide / 2.0);
            float heightTriangle1 = (float) (triangleHeight * 1.0 / 3.0);
            float heightTriangle2 = (float) (triangleHeight * 2.0 / 3.0);
            // Vertices
            float[] vertices =
            {
                0.0f, 0.0f, 0.0f,                             // Origin
                // Axes
                lengthAxis, 0.0f, 0.0f,                       // X-end
                0.0f, lengthAxis, 0.0f,                       // Y-end
                0.0f, 0.0f, lengthAxis,                       // Z-end
                // Tetrahedron at the end of X axis
                lengthAxis + heightTetra, 0.0f, 0.0f,         // Peak
                lengthAxis, -heightTriangle1, -halfSideTetra, // Left
                lengthAxis, -heightTriangle1, halfSideTetra,  // Right
                lengthAxis, heightTriangle2, 0.0f,            // Up
                // Tetrahedron at the end of Y axis
                0.0f, lengthAxis + heightTetra, 0.0f,         // Peak
                -heightTriangle1, lengthAxis, -halfSideTetra, // Left
                -heightTriangle1, lengthAxis, halfSideTetra,  // Right
                heightTriangle2, lengthAxis, 0.0f,            // Up
                // Tetrahedron at the end of Z axis
                0.0f, 0.0f, lengthAxis + heightTetra,         // Peak
                -heightTriangle1, -halfSideTetra, lengthAxis, // Left
                -heightTriangle1, halfSideTetra, lengthAxis,  // Right
                heightTriangle2, 0.0f, lengthAxis,            // Up
                // Box
                    // XZ
                0.0f, sideBox, 0.0f,                            
                sideBox, sideBox, 0.0f,
                sideBox, sideBox, sideBox,
                0.0f, sideBox, sideBox,
                    // XY
                0.0f, 0.0f, sideBox,
                sideBox, 0.0f, sideBox,
                    // YZ
                sideBox, 0.0f, 0.0f
            };
            vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            // Axes buffer
            uint[] axesIndices =
            {
                0, 1, // X
                0, 2, // Y
                0, 3  // Z
            };
            axesBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, axesBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, axesIndices.Length * sizeof(uint), axesIndices, BufferUsageHint.StaticDraw);
            // Tetrahedra
            tetrahedronBuffers = new int[3];
            // Base indices for X axis
            uint[] tetraIndices = 
            {
                4, 5, 7,
                4, 6, 7,
                4, 5, 6,
                5, 6, 7
            }; 
            int nInd = tetraIndices.Length;
            for (uint iAxis = 0; iAxis != 3; ++iAxis)
            {
                tetrahedronBuffers[iAxis] = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, tetrahedronBuffers[iAxis]);
                GL.BufferData(BufferTarget.ElementArrayBuffer, nInd * sizeof(uint), tetraIndices, BufferUsageHint.StaticDraw);
                for (int i = 0; i != nInd; ++i)
                    tetraIndices[i] += 4;
            }
            // Box
            uint[] boxIndices =
            {
                16, 17, 18, 19,
                18, 19, 20, 21,
                17, 18, 21, 22,
            };
            boxBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, boxBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, boxIndices.Length * sizeof(uint), boxIndices, BufferUsageHint.StaticDraw);
            // Fonts
            fontRenderOptions = new QFontRenderOptions();
            fontDrawing = new QFontDrawing();
        }

        public void draw(Matrix4 model, Matrix4 view, Matrix4 projection)
        {
            // Shader
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            // Fonts
            fontDrawing.DrawingPrimitives.Clear();
            fontDrawing.ProjectionMatrix = projection;
            // Vertex buffer
            int colorLocation = shader.GetUniformLocation("definedColor");
            int attrib = shader.GetAttribLocation("inPosition");
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.VertexAttribPointer(attrib, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            // Drawing options
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            // Axes
            int ptrDraw = 0;
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, axesBuffer);
            for (int iAxis = 0; iAxis != 3; ++iAxis)
            {
                GL.Uniform4(colorLocation, axesColors[iAxis]);
                GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, ptrDraw * sizeof(uint));
                ptrDraw += 2;
            }
            // Box
            GL.Uniform4(colorLocation, boxColor);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, boxBuffer);
            GL.DrawElements(PrimitiveType.Quads, 12, DrawElementsType.UnsignedInt, 0);
            // Tetra
            for (int iAxis = 0; iAxis != 3; ++iAxis)
            {
                Color4 color = axesColors[iAxis];
                GL.Uniform4(colorLocation, color);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, tetrahedronBuffers[iAxis]);
                GL.DrawElements(PrimitiveType.Triangles, 12, DrawElementsType.UnsignedInt, 0);
                // Label
                Vector4 position = Vector4.Zero;
                position[iAxis] = lengthAxis + heightTetra + shiftLabel;
                position.W = 1.0f;
                position *= model * view;
                fontRenderOptions.Colour = (Color) color;
                fontDrawing.Print(font, axesNames[iAxis], position.Xyz, QFontAlignment.Justify, fontRenderOptions);
            }
            fontDrawing.RefreshBuffers();
            fontDrawing.Draw();
        }

        // Shader
        public Shader shader { get; set; }
        // Labels
        public QFont font { get; set; }
        private QFontDrawing fontDrawing;
        private float shiftLabel = 0.03f;
        // Dimensions
        private float lengthAxis = 0.15f;
        private double tetraSide = 0.03;
        private float heightTetra = 0.05f;
        private float sideBox = 0.08f;
        // Vertices
        private int vertexBuffer;
        // Axes
        private Color4[] axesColors = { Color4.Green, Color4.Blue, Color4.Red };
        private int axesBuffer;
        private string[] axesNames = { "X", "Y", "Z" };
        private QFontRenderOptions fontRenderOptions;
        // Tetrahedra
        private int[] tetrahedronBuffers;
        // Box
        private int boxBuffer;
        private Color4 boxColor = Color4.DimGray;
    }
}
