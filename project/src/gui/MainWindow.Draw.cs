using System;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Keyboard = System.Windows.Input.Keyboard;
using Key = System.Windows.Input.Key;

namespace ResponseAnalyzer
{
    public partial class ResponseAnalyzer
    {
        private void glWindow_Load(object sender, EventArgs e)
        {
            glWindow.MakeCurrent();
            modelRenderer_.setControl(glWindow);
            #if DEBUG
                testRender();
                testExcel();
            #endif
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
            switch (e.Button)
            {
                case MouseButtons.Middle:
                    lastMousePosition_[0] = e.X;
                    lastMousePosition_[1] = e.Y;
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        isTranslation_ = true;
                    else
                        isRotation_ = true;
                    break;
                case MouseButtons.Left:
                    bool isNewSelection = true;
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) || isEditSelection_)
                        isNewSelection = false;
                    Tuple <Vector3d, string, string> info = modelRenderer_.select(e.X, e.Y, isNewSelection);
                    if (info == null)
                        return;
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
            if (e.Button == MouseButtons.Middle)
            {
                isTranslation_ = false;
                isRotation_ = false;
            }
            if (Keyboard.IsKeyUp(Key.LeftCtrl) || Keyboard.IsKeyUp(Key.RightCtrl))
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

        // Nodes
        // Show node names
        private void glNodeNames_CheckedChanged(object sender, EventArgs e)
        {
            modelRenderer_.isShowNodeNames = glNodeNames.Checked;
            modelRenderer_.draw();
        }

        // Show node markers
        private void glNodeMarkers_CheckedChanged(object sender, EventArgs e)
        {
            modelRenderer_.isShowNodeMarkers = glNodeMarkers.Checked;
            modelRenderer_.draw();
        }

        // Enable lighting
        private void stripLighting_CheckedChanged(object sender, EventArgs e)
        {
            modelRenderer_.isLighting = stripLighting.Checked;
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

        // Retrieve the selection from the tree
        private void treeTemplateObjects_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = treeTemplateObjects.SelectedNode;
            if (selectedNode == null)
                return;
            bool isLine = selectedNode.Nodes.Count != 0;
            string[] selectionInfo;
            modelRenderer_.clearSelection();
            if (isLine)
            {
                foreach (TreeNode node in selectedNode.Nodes)
                {
                    selectionInfo = node.Text.Split(selectionDelimiter_);
                    modelRenderer_.select(selectionInfo[0], selectionInfo[1], false);
                }
            }
            else
            {
                selectionInfo = selectedNode.Text.Split(selectionDelimiter_);
                modelRenderer_.select(selectionInfo[0], selectionInfo[1], false);
            }
            clearStatus();
            modelRenderer_.draw();
        }

        // Create components to select
        private void createComponentStrips()
        {
            List<string> componentNames = modelRenderer_.getComponentNames();
            ToolStripItemCollection items = stripComponentVisualisation.DropDownItems;
            items.Clear();
            // Show all the components
            ToolStripMenuItem item = (ToolStripMenuItem)items.Add("Show all");
            item.Name = "All";
            item.Click += new EventHandler(showComponents);
            // Show nothing
            item = (ToolStripMenuItem)items.Add("Show none");
            item.Name = "Nothing";
            item.Click += new EventHandler(showComponents);
            items.Add(new ToolStripSeparator());
            // Component names
            foreach (string component in componentNames)
            {
                item = (ToolStripMenuItem)items.Add(component);
                item.Checked = true;
                item.CheckOnClick = true;
                item.CheckedChanged += showComponents;
            }
        }
    }

    static class MouseWeights
    {
        public const float scaling = 0.001f;
        public const float translation = 1.0f;
        public const float rotation = 0.6f;
    }
}
