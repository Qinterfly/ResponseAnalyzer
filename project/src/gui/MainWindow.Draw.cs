using System;
using System.Globalization;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL4;

namespace ResponseAnalyzer
{
    public partial class ResponseAnalyzer
    {
        private void glWindow_Load(object sender, EventArgs e)
        {
            glWindow.MakeCurrent();
            modelRenderer_.setControl(glWindow);

            // -- Debug only --
            testRender();
            testExcel();
            // ----------------
        }

        private void glWindow_Paint(object sender, PaintEventArgs e)
        {
            modelRenderer_.draw();
        }

        private void glWindow_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            float scale = 1.0f + MouseWeights.scaling * e.Delta;
            modelRenderer_.setScale(scale);
            modelRenderer_.draw();
        }

        private void glWindow_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var keyboard = Keyboard.GetState();
            switch (e.Button)
            {
                case MouseButtons.Middle:
                    lastMousePosition_[0] = e.X;
                    lastMousePosition_[1] = e.Y;
                    if (keyboard.IsKeyDown(Key.ControlLeft))
                        isTranslation_ = true;
                    else
                        isRotation_ = true;
                    break;
                case MouseButtons.Left:
                    bool isNewSelection = true;
                    if (keyboard.IsKeyDown(Key.ShiftLeft) || isEditSelection_)
                        isNewSelection = false;
                    Tuple <Vector3d, string, string> info = modelRenderer_.select(e.X, e.Y, isNewSelection);
                    modelRenderer_.draw();
                    // Show the information about the selection
                    const string format = "G6";
                    setStatus("Coordinates of " + info.Item2 + selectionDelimiter_ + info.Item3 + " = ("
                              + info.Item1.X.ToString(format, CultureInfo.InvariantCulture) + "; "
                              + info.Item1.Y.ToString(format, CultureInfo.InvariantCulture) + "; "
                              + info.Item1.Z.ToString(format, CultureInfo.InvariantCulture) + ")");
                    break;
                case MouseButtons.Right:
                    glContextMenu.Show(Cursor.Position.X, Cursor.Position.Y);
                    break;
            }
        }

        private void glWindow_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var keyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();
            if (isTranslation_)
            {
                float dX = (e.X - lastMousePosition_[0]) * MouseWeights.translation;
                float dY = (e.Y - lastMousePosition_[1]) * MouseWeights.translation;
                modelRenderer_.setTranslation(dX, -dY);
                modelRenderer_.draw();
                lastMousePosition_[0] = e.X;
                lastMousePosition_[1] = e.Y;
            }
            if (isRotation_)
            {
                float dRotX = (e.Y - lastMousePosition_[1]);
                float dRotY = (e.X - lastMousePosition_[0]);
                dRotX = MathHelper.DegreesToRadians(dRotX) * MouseWeights.rotation;
                dRotY = MathHelper.DegreesToRadians(dRotY) * MouseWeights.rotation;
                modelRenderer_.setRotationXY(dRotX, dRotY);
                modelRenderer_.draw();
                lastMousePosition_[0] = e.X;
                lastMousePosition_[1] = e.Y;
            }
        }

        private void glWindow_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var keyboard = Keyboard.GetState();
            if (e.Button == MouseButtons.Middle)
            {
                isTranslation_ = false;
                isRotation_ = false;
            }
            if (keyboard.IsKeyUp(Key.ControlLeft))
                isTranslation_ = false;
        }

        private void glWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Control | Keys.F:
                    modelRenderer_.setView(LMSModel.Views.UP);
                    modelRenderer_.draw();
                    break;
                case Keys.Escape:
                    modelRenderer_.clearSelection();
                    modelRenderer_.draw();
                    clearStatus();
                    break;
            }
        }

        private void glWindow_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glWindow.Width, glWindow.Height);
            modelRenderer_.resize();
        }

        private void stripMode_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text.ToUpper())
            {
                case "LINE":
                    modelRenderer_.setPolygonMode(PolygonMode.Line);
                    break;
                case "FILL":
                    modelRenderer_.setPolygonMode(PolygonMode.Fill);
                    break;
            }
            modelRenderer_.draw();
        }

        private void stripView_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text.ToUpper())
            {
                case "FRONT":
                    modelRenderer_.setView(LMSModel.Views.FRONT);
                    break;
                case "BACK":
                    modelRenderer_.setView(LMSModel.Views.BACK);
                    break;
                case "UP":
                    modelRenderer_.setView(LMSModel.Views.UP);
                    break;
                case "DOWN":
                    modelRenderer_.setView(LMSModel.Views.DOWN);
                    break;
                case "LEFT":
                    modelRenderer_.setView(LMSModel.Views.LEFT);
                    break;
                case "RIGHT":
                    modelRenderer_.setView(LMSModel.Views.RIGHT);
                    break;
                case "ISOMETRIC":
                    modelRenderer_.setView(LMSModel.Views.ISOMETRIC);
                    break;
            }
            modelRenderer_.draw();
        }

        // Show node names
        private void stripNodeNames_CheckedChanged(object sender, EventArgs e)
        {
            modelRenderer_.isShowNodeNames = stripNodeNames.Checked;
            modelRenderer_.draw();
        }

        // Select components to draw
        private void showComponents(object sender, System.EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            bool isShow = item.Checked;
            string keyName = item.Text;
            if (item.Name.Equals("All"))
            {
                keyName = string.Empty;
                isShow = true;
            }
            else if (item.Name.Equals("Nothing"))
            {
                keyName = string.Empty;
                isShow = false;
            }
            if (!string.IsNullOrEmpty(keyName)) 
            { 
                modelRenderer_.setShowComponent(keyName, isShow);
            }
            else
            {
                ToolStripItemCollection objects = stripComponentVisualisation.DropDownItems;
                int nObjects = objects.Count;
                for (int i = 3; i != nObjects; ++i)
                {
                    ToolStripMenuItem it = (ToolStripMenuItem) objects[i];
                    modelRenderer_.setShowComponent(it.Text, isShow);
                    it.CheckedChanged -= showComponents;
                    it.Checked = isShow;
                    it.CheckedChanged += showComponents;
                }
            }
            modelRenderer_.draw();
        }

    }

    static class MouseWeights
    {
        public const float scaling = 0.001f;
        public const float translation = 1.0f;
        public const float rotation = 0.6f;
    }
}
