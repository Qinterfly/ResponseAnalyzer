using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using QuickFont;
using QuickFont.Configuration;

namespace ResponseAnalyzer
{
    public partial class LMSModel
    {
        public void initializeGL()
        {
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            // Blending
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.PolygonSmooth);
            GL.Enable(EnableCap.LineSmooth);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            // Z-buffer
            GL.ClearDepth(1.0f);
            GL.DepthMask(true);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.DepthTest);
            // Stencil
            GL.ClearStencil(0);
            GL.Enable(EnableCap.StencilTest);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
            // Smoothing
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);
            // Sizes
            GL.PointSize(DrawOptions.pointSize);
            GL.LineWidth(DrawOptions.lineWidth);
            // Defining all the colors for further selection
            availableColors_ = new List<Color4>()
            {
                Color4.Blue, Color4.Green, Color4.DarkOrange,
                Color4.Red, Color4.SaddleBrown, Color4.Purple,
                Color4.DarkBlue, Color4.DarkCyan, Color4.Chocolate
            };
            selection_ = new Dictionary<string, List<uint>>();
            selectionColor_ = Color4.Yellow;
            // Transformations
            location_ = Vector3.Zero;
            modelTranslation_ = Matrix4.Identity;
            modelScale_ = Matrix4.CreateScale(DrawOptions.defaultScale, DrawOptions.defaultScale, 1.0f);
            modelRotation_ = Matrix4.Identity;
            view_ = Matrix4.Identity;
            projection_ = Matrix4.CreateOrthographic(glControl_.Width, glControl_.Height, DrawOptions.zNear, DrawOptions.zFar);
            // Fonts
            isShowNodeNames = false;
            fontDrawing_ = new QFontDrawing();
            var builderConfig = new QFontBuilderConfiguration(true)
            {
                TextGenerationRenderHint = TextGenerationRenderHint.ClearTypeGridFit,
                Characters = CharacterSet.General | CharacterSet.Japanese | CharacterSet.Thai | CharacterSet.Cyrillic
            };
            font_ = new QFont("Optima", 8, builderConfig);
            fontRenderOptions_ = new QFontRenderOptions()
            {
                Colour = Color.Black,
                DropShadowActive = false,
                CharacterSpacing = 0.1f
            };
            // Compiling a shader
            shader_ = new Shader("../../shaders/shader.vert", "../../shaders/shader.frag");
            // Coordinate system
            coordinateSystem_ = new CoordinateSystem();
            coordinateSystem_.font = font_;
            coordinateSystem_.shader = shader_;
            coordinateSystemOrigin_ = new Vector3(DrawOptions.originSystemX, DrawOptions.originSystemY, DrawOptions.originSystemZ);
            coordinateSystemScaleTranslation_ = Matrix4.CreateScale(DrawOptions.defaultScale, DrawOptions.defaultScale, 1.0f) * Matrix4.CreateTranslation(coordinateSystemOrigin_);
        }

        public void resize()
        {
            if (isCongruent())
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
            componentBuffers_.vertexBufferObject.Add(componentName, vertexBufferObject);
            // Buffers for each element
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
            // Selection buffer
            elementBufferObject = GL.GenBuffer();
            componentBuffers_.selection.Add(componentName, elementBufferObject);
        }

        // Draw all the elements
        public void draw()
        {
            if (!isCongruent())
                return;
            // Preparing the window
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.PolygonMode(MaterialFace.FrontAndBack, polygonMode_);
            // Drawing the components
            shader_.Use();
            Matrix4 model = modelRotation_ * modelScale_ * modelTranslation_;
            shader_.SetMatrix4("model", model);
            shader_.SetMatrix4("view", view_);
            shader_.SetMatrix4("projection", projection_);
            int colorLocation = shader_.GetUniformLocation("definedColor");
            int VAO, EBO;
            int sizeElement;
            int nVertices = 0;
            foreach (string component in componentNames_)
            {
                if (!componentShowMask_[component])
                    continue;
                VAO = componentBuffers_.vertexBufferObject[component];
                GL.BindBuffer(BufferTarget.ArrayBuffer, VAO);
                int attrib = shader_.GetAttribLocation("inPosition");
                GL.VertexAttribPointer(attrib, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
                Color4 componentColor = componentSet_.colors[component];
                // Points
                GL.StencilMask(0xFF);
                GL.StencilFunc(StencilFunction.Always, 1, 0xFF); // Drawing all the points
                GL.Uniform4(colorLocation, componentColor);
                nVertices = componentSet_.vertices[component].Length / 3;
                GL.DrawArrays(PrimitiveType.Points, 0, nVertices);
                // Elements
                GL.Uniform4(colorLocation, componentColor);
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
                // Selected points
                if (selection_.Count != 0 && selection_.ContainsKey(component))
                {
                    GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF); // Discarding previously drawn points
                    GL.StencilMask(0x00);
                    GL.Disable(EnableCap.DepthTest);
                    GL.Uniform4(colorLocation, selectionColor_);
                    uint[] indices = selection_[component].ToArray();
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, componentBuffers_.selection[component]);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);
                    GL.DrawElements(PrimitiveType.Points, indices.Length, DrawElementsType.UnsignedInt, 0);
                    GL.StencilMask(0xFF);
                    GL.Enable(EnableCap.DepthTest);
                }
            }
            // Node names
            if (isShowNodeNames)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                fontDrawing_.DrawingPrimitives.Clear();
                fontDrawing_.ProjectionMatrix = projection_;
                int nNodes = 0;
                string resName;
                int iVert = 0;
                Vector4 position = Vector4.Zero;
                foreach (string component in componentNames_)
                {
                    if (!componentShowMask_[component])
                        continue;
                    iVert = 0;
                    string[] nodeNames = (string[])componentSet_.nodeNames[component];
                    float[] vertices = (float[])componentSet_.vertices[component];
                    nNodes = nodeNames.Length;
                    for (int iNode = 0; iNode != nNodes; ++iNode)
                    {
                        resName = component + ":" + nodeNames[iNode];
                        position.X = vertices[iVert + 0];
                        position.Y = vertices[iVert + 1] + DrawOptions.shiftLabelY;
                        position.Z = vertices[iVert + 2];
                        position.W = 1.0f;
                        position *= model * view_;
                        fontDrawing_.Print(font_, resName, position.Xyz, QFontAlignment.Justify, fontRenderOptions_);
                        iVert += 3;
                    }
                }
                fontDrawing_.RefreshBuffers();
                fontDrawing_.Draw();
                GL.PolygonMode(MaterialFace.FrontAndBack, polygonMode_);
            }
            // Coordinate system
            coordinateSystem_.draw(modelRotation_ * coordinateSystemScaleTranslation_, view_, projection_);
            glControl_.SwapBuffers();
        }

        // Colors
        private List<Color4> availableColors_;
        private Color4 selectionColor_;
        // Orientation
        private Matrix4 modelTranslation_;
        private Matrix4 modelScale_;
        private Matrix4 modelRotation_;
        private Matrix4 view_;
        private Matrix4 projection_;
        private Vector3 location_;
        // Options
        private PolygonMode polygonMode_ = PolygonMode.Fill;
        private Vector3 isoVector_ = new Vector3(0.4607291f, -0.8350012f, -0.3008356f);
        private float isoAngle_ = 0.910852849f;
        // Fonts
        private QFont font_;
        private QFontDrawing fontDrawing_;
        private QFontRenderOptions fontRenderOptions_;
        // Show mode
        public bool isShowNodeNames { get; set; }
        private Dictionary<string, bool> componentShowMask_;
        // Cordinate system
        private CoordinateSystem coordinateSystem_;
        private Vector3 coordinateSystemOrigin_;
        private Matrix4 coordinateSystemScaleTranslation_;

        public static class DrawOptions
        {
            public const float pointSize = 8.0f;
            public const float lineWidth = 1.25f;
            public const float defaultScale = 300.0f;
            public const float defaultY = 100.0f;
            public const float zNear = -10.0f;
            public const float zFar = 10.0f;
            // Fonts
            public const float shiftLabelY = 0.04f;
            // Coordinate system
            public const float originSystemX = 200.0f;
            public const float originSystemY = 250.0f;
            public const float originSystemZ = 0.0f;
        }

        public enum Views
        {
            FRONT, BACK,
            UP, DOWN, LEFT, RIGHT, ISOMETRIC
        }
    }
}
