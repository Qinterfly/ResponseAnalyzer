﻿using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL4;

namespace ResponseAnalyzer
{
    public partial class ResponseAnalyzer : Form
    {
        public ResponseAnalyzer()
        {
            InitializeComponent();
            comboBoxTemplateType.SelectedIndex = 0;
            modelRenderer_ = new LMSModel();
            lastMousePosition_ = new int[2] { 0, 0 };
        }

        // Opening project
        private void buttonOpenProject_Click(object sender, EventArgs e)
        {
            clearStatus();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "LMS Test.Lab 15A Project (*.lms)|*.lms";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                project = new LMSProject(filePath);
                clearProjectControls();
                if (!project.isProjectOpened()) {
                    textBoxProjectPath.Text = filePath;
                    setStatus("The project was successfully opened");
                }
                modelRenderer_.setGeometry(project.geometry_);
            }
            else if (dialogResult != DialogResult.Cancel)
            {
                setStatus("An error occured while choosing a project file");
                return;
            }
            else
            {
                return;
            }
        }

        // Set the status label
        private void setStatus(string status)
        {
            statusStripLabel.Text = status;
        }

        // Clear the status label
        private void clearStatus()
        {
            statusStripLabel.Text = null;
        }

        // Clear project controls
        private void clearProjectControls()
        {
            textBoxProjectPath.Clear();
        }


        private void buttonAddSelection_Click(object sender, EventArgs e)
        {
            var selection = modelRenderer_.getSelection();
            int nSelection = selection.Count;
            if (nSelection == 0)
                return;
            if (nSelection > 1)
            {
                TreeNode line = treeSelection.Nodes.Add("Line" + indLine_.ToString());
                foreach (string item in selection)
                    line.Nodes.Add(item);
                ++indLine_;
            }
            if (nSelection == 1)
                treeSelection.Nodes.Add(selection[0]);
        }

        private void buttonEditSelection_Click(object sender, EventArgs e)
        {
            // If not a line was selected
            if (treeSelection.SelectedNode == null || treeSelection.SelectedNode.Nodes.Count == 0)
                return;
            if (isEditSelection)
            {
                TreeNode line = treeSelection.Nodes[iSelectedSet_];
                line.Nodes.Clear();
                var selection = modelRenderer_.getSelection();
                if (selection.Count < 2)
                {
                    line.Remove();
                }
                else
                {
                    foreach (string item in selection)
                        line.Nodes.Add(item);
                }
                iSelectedSet_ = -1;
            }
            else
            { 
                iSelectedSet_ = treeSelection.SelectedNode.Index;
                modelRenderer_.clearSelection();
                // Select all the nodes in the set
                foreach (TreeNode item in treeSelection.SelectedNode.Nodes)
                {
                    string[] selectionInfo = item.Text.Split(selectionDelimiter_);
                    modelRenderer_.select(selectionInfo[0], selectionInfo[1], false);
                }
                modelRenderer_.draw();
            }
            // Invert the states of the controls
            buttonAddSelection.Enabled = isEditSelection;
            buttonRemoveSelection.Enabled = isEditSelection;
            isEditSelection = !isEditSelection;
        }

        private void buttonRemoveSelection_Click(object sender, EventArgs e)
        {
            if (treeSelection.Nodes.Count == 0 || treeSelection.SelectedNode == null)
                return;
            int iSelected = treeSelection.SelectedNode.Index;
            // If a child node is selected
            if (treeSelection.SelectedNode.Parent != null)
                return;
            // Line
            if (treeSelection.SelectedNode.Nodes.Count != 0)
            {
                foreach (TreeNode node in treeSelection.SelectedNode.Nodes)
                    removeNodeFromTree(node);
            }
            // Single node
            else 
            {
                removeNodeFromTree(treeSelection.SelectedNode);
            }
            treeSelection.Nodes.RemoveAt(iSelected);
            modelRenderer_.draw();
        }

        private void removeNodeFromTree(TreeNode node)
        {
            string selectedNode = node.Text;
            string[] selectionInfo = selectedNode.Split(selectionDelimiter_);
            modelRenderer_.removeSelection(selectionInfo[0], selectionInfo[1]);
        }

        private void glWindow_Load(object sender, EventArgs e)
        {
            glWindow.MakeCurrent();
            modelRenderer_.setControl(glWindow);

            // -- Debug only --
            testRender();
            testExcel();
            // ----------------
        }

        private void setEnabled()
        {
            bool flag = project != null;
            // Project
            buttonAddSelection.Enabled = flag;
            buttonRemoveSelection.Enabled = flag;
            buttonEditSelection.Enabled = flag;
            // Excel template
            flag = excelTemplate_ != null;
            buttonAddTemplateObject.Enabled = flag;
            buttonRemoveTemplateObject.Enabled = flag;
            comboBoxTemplateType.Enabled = flag;
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
                    if (keyboard.IsKeyDown(Key.ShiftLeft) || isEditSelection)
                        isNewSelection = false;
                    modelRenderer_.select(e.X, e.Y, isNewSelection);
                    modelRenderer_.draw();
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
            if (e.Button == MouseButtons.Middle) {
                isTranslation_ = false;
                isRotation_ = false;
            }
            if (keyboard.IsKeyUp(Key.ControlLeft)) 
                isTranslation_ = false;
        }

        private void testRender()
        {
            //string path = Path.GetFullPath(@"..\..\..\examples\Plate.lms");
            //string path = Path.GetFullPath(@"..\..\..\examples\Rib.lms");
            //string path = Path.GetFullPath(@"..\..\..\examples\Airplane.lms");
            string path = Path.GetFullPath(@"..\..\..\examples\Yak130.lms");
            project = new LMSProject(path);
            textBoxProjectPath.Text = path;
            modelRenderer_.setGeometry(project.geometry_);
            modelRenderer_.setView(LMSModel.Views.ISOMETRIC);
            setEnabled();
        }

        private void testExcel()
        {
            string path = Path.GetFullPath(@"..\..\..\templates\Base.xlsx");
            excelTemplate_ = new ExcelObject(path);
            textBoxExcelTemplatePath.Text = path;
            updateExcelTemplateList();
            setEnabled();
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
                    break;
            }
        }
        private void glWindow_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glWindow.Width, glWindow.Height);
        }

        private void stripMode_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text.ToUpper()) { 
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
            switch (e.ClickedItem.Text.ToUpper()) {
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

        private void buttonOpenExcel_Click(object sender, EventArgs e)
        {
            clearStatus();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                excelTemplate_ = new ExcelObject(filePath);
                if (excelTemplate_.isOpened())
                {
                    textBoxExcelTemplatePath.Text = filePath;
                    updateExcelTemplateList();
                    setStatus("The Excel template file was successfully opened");
                }
                
            }
            else if (dialogResult != DialogResult.Cancel)
            {
                setStatus("An error occured while choosing the template Excel file");
                return;
            }
            else
            {
                return;
            }
        }
        
        private void updateExcelTemplateList()
        {
            listBoxTemplateCharts.Items.Clear();
            List<string> charts = excelTemplate_.getChartNames();
            chartTypes_ = new Dictionary<string, ChartTypes>();
            chartUnits_ = new Dictionary<string, SignalUnits>();
            chartNodes_ = new Dictionary<string, List<string>>();
            foreach (string chart in charts) { 
                listBoxTemplateCharts.Items.Add(chart);
                chartTypes_.Add(chart, ChartTypes.UNKNOWN);
                chartUnits_.Add(chart, SignalUnits.UNKNOWN);
                chartNodes_.Add(chart, new List<string>());
            }
            listBoxTemplateCharts.SelectedIndex = 0;
        }

        static class MouseWeights
        {
            public const float scaling = 0.001f;
            public const float translation = 1.0f;
            public const float rotation = 1.0f;
        }

        private void comboBoxTemplateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            ChartTypes iSelected = (ChartTypes)comboBoxTemplateType.SelectedIndex;
            chartTypes_[listBoxTemplateCharts.SelectedItem.ToString()] = iSelected;
        }

        private void listBoxTemplateCharts_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxTemplateType.SelectedIndex = (int)chartTypes_[listBoxTemplateCharts.SelectedItem.ToString()];
            comboBoxTemplateUnits.SelectedIndex = (int)chartUnits_[listBoxTemplateCharts.SelectedItem.ToString()];
            listBoxTemplateObjects.Items.Clear();
            List<string> tNodes = chartNodes_[listBoxTemplateCharts.SelectedItem.ToString()];
            if (tNodes.Count != 0)
            {
                foreach (string node in tNodes)
                    listBoxTemplateObjects.Items.Add(node);
                listBoxTemplateObjects.SelectedIndex = 0;
            }
        }

        private void comboBoxTemplateUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            SignalUnits iSelected = (SignalUnits)comboBoxTemplateUnits.SelectedIndex;
            chartUnits_[listBoxTemplateCharts.SelectedItem.ToString()] = iSelected;
        }

        private void buttonAddTemplateObject_Click(object sender, EventArgs e)
        {
            ChartTypes templateType = (ChartTypes)comboBoxTemplateType.SelectedIndex;
            if (treeSelection.SelectedNode == null 
                || templateType == ChartTypes.UNKNOWN)
                return;
            // Check if an appopriate object was chosen
            bool isLineSelected = treeSelection.SelectedNode.Nodes.Count != 0;
            bool isNeedInsertion = false;
            if (isLineSelected)
                isNeedInsertion = templateType == ChartTypes.MODESHAPE;
            else
                isNeedInsertion = templateType == ChartTypes.REALFRF || templateType == ChartTypes.IMAGFRF;
            if (isNeedInsertion) 
            {
                string node = treeSelection.SelectedNode.Text;
                string chart = listBoxTemplateCharts.SelectedItem.ToString();
                listBoxTemplateObjects.Items.Add(node);
                chartNodes_[chart].Add(node);
                listBoxTemplateObjects.SelectedIndex = 0;
            }
        }

        private void buttonRemoveTemplateObject_Click(object sender, EventArgs e)
        {
            int iSelected = listBoxTemplateObjects.SelectedIndex;
            if (iSelected < 0)
                return;
            listBoxTemplateObjects.Items.RemoveAt(iSelected);
            if (listBoxTemplateObjects.Items.Count != 0)
                listBoxTemplateObjects.SelectedIndex = 0;
        }

        private void buttonSelectTestLab_Click(object sender, EventArgs e)
        {
            // TODO: a lot of checks are need here (project, selection, excel template)
            int nSelected = project.selectSignals();
            labelSelectionInfo.Text = "Selected signals:" + nSelected.ToString();
        }
    }
}