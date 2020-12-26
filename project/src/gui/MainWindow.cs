using System;
using System.IO;
using System.Globalization;
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
                modelRenderer_.setView(LMSModel.Views.ISOMETRIC);
                setEnabled();
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
            int iRemoved = treeSelection.SelectedNode.Index;
            // If a child node is selected
            if (treeSelection.SelectedNode.Parent != null)
                return;
            // Delete the bindings
            int tInd;
            Dictionary<string, List<int>> updatedChartNodeIndices = new Dictionary<string, List<int>>();
            foreach (string chart in chartNodeIndices_.Keys)
            {
                List<int> indices = chartNodeIndices_[chart];
                List<int> updatedIndices = new List<int>();
                int nSelected = indices.Count;
                for (int i = 0; i != nSelected; ++i)
                {
                    tInd = indices[i];
                    if (tInd > iRemoved)
                        updatedIndices.Add(tInd - 1);
                    else if (tInd < iRemoved)
                        updatedIndices.Add(tInd);
                }
                updatedChartNodeIndices.Add(chart, updatedIndices);
            }
            chartNodeIndices_ = updatedChartNodeIndices;
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
            treeSelection.Nodes.RemoveAt(iRemoved);
            modelRenderer_.draw();
            listBoxTemplateCharts_SelectedIndexChanged();
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
            textBoxDirectoryExcel.Text = "C:\\Users\\qinterfly\\Desktop";
            textBoxNameExcel.Text = "TestMe";
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

        private void buttonOpenExcelTemplate_Click(object sender, EventArgs e)
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
                    setEnabled();
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
            chartNodeIndices_ = new Dictionary<string, List<int>>();
            chartDirection_ = new Dictionary<string, ChartDirection>();
            chartNormalization_ = new Dictionary<string, double>();
            chartAxis_ = new Dictionary<string, ChartDirection>();
            foreach (string chart in charts) { 
                listBoxTemplateCharts.Items.Add(chart);
                chartTypes_.Add(chart, ChartTypes.UNKNOWN);
                chartUnits_.Add(chart, SignalUnits.UNKNOWN);
                chartDirection_.Add(chart, ChartDirection.UNKNOWN);
                chartNodeIndices_.Add(chart, new List<int>());
                chartNormalization_.Add(chart, 1.0);
                chartAxis_.Add(chart, ChartDirection.UNKNOWN);
            }
            listBoxTemplateCharts.SelectedIndex = 0;
        }

        private void comboBoxTemplateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            ChartTypes iSelected = (ChartTypes)comboBoxTemplateType.SelectedIndex;
            chartTypes_[listBoxTemplateCharts.SelectedItem.ToString()] = iSelected;
        }

        private void numericNormalization_ValueChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            chartNormalization_[listBoxTemplateCharts.SelectedItem.ToString()] = (double) numericTemplateNormalization.Value;
        }

        private void listBoxTemplateCharts_SelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            comboBoxTemplateType.SelectedIndex = (int)chartTypes_[listBoxTemplateCharts.SelectedItem.ToString()];
            comboBoxTemplateUnits.SelectedIndex = (int)chartUnits_[listBoxTemplateCharts.SelectedItem.ToString()];
            comboBoxTemplateDirection.SelectedIndex = (int)chartDirection_[listBoxTemplateCharts.SelectedItem.ToString()];
            comboBoxTemplateAxis.SelectedIndex = (int)chartAxis_[listBoxTemplateCharts.SelectedItem.ToString()];
            numericTemplateNormalization.Value = (decimal)chartNormalization_[listBoxTemplateCharts.SelectedItem.ToString()];
            listBoxTemplateObjects.Items.Clear();
            List<int> tNodesIndices = chartNodeIndices_[listBoxTemplateCharts.SelectedItem.ToString()];
            if (tNodesIndices.Count != 0)
            {
                foreach (int nodeIndex in tNodesIndices)
                    listBoxTemplateObjects.Items.Add(treeSelection.Nodes[nodeIndex].Text);
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

        private void comboBoxTemplateDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            ChartDirection iSelected = (ChartDirection)comboBoxTemplateDirection.SelectedIndex;
            chartDirection_[listBoxTemplateCharts.SelectedItem.ToString()] = iSelected;
        }

        private void comboBoxTemplateAxis_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            ChartDirection iSelected = (ChartDirection)comboBoxTemplateAxis.SelectedIndex;
            chartAxis_[listBoxTemplateCharts.SelectedItem.ToString()] = iSelected;
        }

        private void buttonAddTemplateObject_Click(object sender, EventArgs e)
        {
            ChartTypes templateType = (ChartTypes)comboBoxTemplateType.SelectedIndex;
            if (treeSelection.SelectedNode == null)
                return;
            // Check if an appopriate object was chosen
            TreeNode node = treeSelection.SelectedNode;
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            listBoxTemplateObjects.Items.Add(node.Text);
            chartNodeIndices_[chart].Add(node.Index);
            listBoxTemplateObjects.SelectedIndex = 0;
        }

        private void buttonRemoveTemplateObject_Click(object sender, EventArgs e)
        {
            int iSelected = listBoxTemplateObjects.SelectedIndex;
            if (iSelected < 0)
                return;
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            chartNodeIndices_[chart].RemoveAt(iSelected);
            listBoxTemplateObjects.Items.RemoveAt(iSelected);
            if (listBoxTemplateObjects.Items.Count != 0)
                listBoxTemplateObjects.SelectedIndex = 0;
        }

        private void buttonSelectTestLab_Click(object sender, EventArgs e)
        {
            // Check if all the fields are correct
            if (!project.isProjectOpened() || !excelTemplate_.isOpened())
                return;
            int nSelected = project.selectSignals(modelRenderer_.componentSet_);
            labelSelectionInfo.Text = "Selected signals: " + nSelected.ToString();
            listBoxFoundSignals.Items.Clear();
            listBoxFrequencies.Items.Clear();
            if (nSelected < 1)
                return;
            ResponseHolder response = null; 
            foreach (string nodeName in project.signals_.Keys)
            {
                foreach (ChartDirection dir in project.signals_[nodeName].Keys)
                {
                    response = project.signals_[nodeName][dir];
                    listBoxFoundSignals.Items.Add(response.signalName);
                }
            }
            int k = 0;
            foreach (double freq in response.frequency) 
            { 
                listBoxFrequencies.Items.Add(freq.ToString("G4", CultureInfo.InvariantCulture));
                listBoxFrequencies.SetSelected(k++, true);
            }
            listBoxFrequencies.TopIndex = 0;
            // Resonance frequency
            textBoxResonanceFrequency.Clear();
            textBoxResonanceFrequency.Tag = -1;
        }

        private void listBoxFrequencies_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                int nItems = listBoxFrequencies.Items.Count;
                for (int i = 0; i != nItems; ++i)
                    listBoxFrequencies.SetSelected(i, true);
                listBoxFrequencies.TopIndex = 0;
            }
        }

        private void buttonSelectDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
                textBoxDirectoryExcel.Text = openFolderDialog.SelectedPath;
        }

        private void buttonProcess_Click(object sender = null, EventArgs e = null)
        {
            ExcelObject excelResult = new ExcelObject(excelTemplate_, textBoxDirectoryExcel.Text, textBoxNameExcel.Text);
            // Checking the project, template and selected signals
            if (!project.isProjectOpened() || !excelTemplate_.isOpened() || listBoxFoundSignals.Items.Count == 0)
                return;
            // Retrieve selected frequencies
            List<int> selectedIndicies = new List<int>();
            foreach (int index in listBoxFrequencies.SelectedIndices)
                selectedIndicies.Add(index);
            int nSelectedFrequency = selectedIndicies.Count;
            ChartPosition.lastRow = 0;
            // Creating series
            foreach (string chart in listBoxTemplateCharts.Items) // Charts
            {
                // Nodes selection
                List<int> selectedIndices = chartNodeIndices_[chart];
                if (selectedIndices.Count == 0)
                    continue;
                // Type and direction
                ChartTypes type = chartTypes_[chart];
                ChartDirection direction = chartDirection_[chart];
                SignalUnits units = chartUnits_[chart];
                if (type == ChartTypes.UNKNOWN || direction == ChartDirection.UNKNOWN || units == SignalUnits.UNKNOWN)
                    continue;
                // Norm
                double norm = chartNormalization_[chart];
                ChartDirection axis = chartAxis_[chart];
                // Frequency response function: real and imaginary parts
                if (type == ChartTypes.REALFRF || type == ChartTypes.IMAGFRF)
                {
                    List<string> chartNodes = retrieveNodes(selectedIndices);
                    foreach (string node in chartNodes) // Node
                    {
                        // Check if there is an appropriate signal
                        if (!project.signals_.ContainsKey(node) || !project.signals_[node].ContainsKey(direction))
                            continue;
                        ResponseHolder response = project.signals_[node][direction];
                        // Slice data by the selected index
                        double[,] refFullData = response.data[units];
                        double[,] data = new double[nSelectedFrequency, 2];
                        int iSelected;
                        int iType = (int)type - 1; 
                        for (int i = 0; i != nSelectedFrequency; ++i)
                        {
                            iSelected = selectedIndicies[i];
                            data[i, 0] = response.frequency[iSelected];
                            data[i, 1] = refFullData[iSelected, iType];
                        }
                        string ptrNode = "т. " + node.Split(selectionDelimiter_)[1];
                        excelResult.addSeries(chart, data, ptrNode);
                    }
                }
                // Modeshape
                if (type == ChartTypes.MODESHAPE)
                {
                    int indResonance = (int) textBoxResonanceFrequency.Tag;
                    if (axis == ChartDirection.UNKNOWN || indResonance < 0)
                        continue;
                    // For each line
                    foreach (int iSelected in selectedIndices)
                    {
                        // Check if a single node was selected
                        if (treeSelection.Nodes[iSelected].Nodes.Count == 0)
                            continue;
                        List<string> lineNodes = retrieveNodesFromLine(iSelected);
                        List<double> coordinates = new List<double>();
                        List<double> values = new List<double>();
                        foreach (string node in lineNodes)
                        {
                            // Check if there is an appropriate signal
                            if (!project.signals_.ContainsKey(node) || !project.signals_[node].ContainsKey(direction))
                                continue;
                            // Retreiving the coordinate along the choosen axis
                            string[] selectionInfo = node.Split(selectionDelimiter_);
                            uint indNode = modelRenderer_.componentSet_.mapNodeNames[selectionInfo[0]][selectionInfo[1]];
                            double[,] componentCoordinates = (double[,])modelRenderer_.componentSet_.nodeCoordinates[selectionInfo[0]];
                            int tInd = (int)axis - 1;
                            coordinates.Add(componentCoordinates[indNode, tInd]);
                            // Retreiving the function value
                            ResponseHolder response = project.signals_[node][direction];
                            double[,] refFullData = response.data[units];
                            values.Add(refFullData[indResonance, 1]); // Imaginary part of the signal
                        }
                        int nNodes = coordinates.Count;
                        double[,] data = new double[nNodes, 2];
                        for (int i = 0; i != nNodes; ++i)
                        {
                            data[i, 0] = coordinates[i];
                            data[i, 1] = values[i] / norm;
                        }
                        excelResult.addSeries(chart, data, treeSelection.Nodes[iSelected].Text);
                    }
                }
            }
            ChartPosition.lastRow = 0;
            excelResult.open();
            setStatus("The results were successfully processed");
        }

        private List<string> retrieveNodes(List<int> listNodeIndices)
        {
            List<string> selectedNodes = new List<string>();
            bool isLine;
            foreach (int index in listNodeIndices)
            {
                TreeNode mainNode = treeSelection.Nodes[index];
                isLine = mainNode.Nodes.Count != 0;
                if (!isLine)
                    selectedNodes.Add(mainNode.Text);
            }
            return selectedNodes;
        }

        private List<string> retrieveNodesFromLine(int index)
        {
            List<string> selectedNodes = new List<string>();
            bool isLine;
            TreeNode mainNode = treeSelection.Nodes[index];
            isLine = mainNode.Nodes.Count != 0;
            if (isLine)
            {
                foreach (TreeNode node in mainNode.Nodes)
                    selectedNodes.Add(node.Text);
            }
            return selectedNodes;
        }

        static class MouseWeights
        {
            public const float scaling = 0.001f;
            public const float translation = 1.0f;
            public const float rotation = 1.0f;
        }

        private void buttonSelectResonanceFrequency_Click(object sender, EventArgs e)
        {
            if (listBoxFrequencies.SelectedIndices.Count == 0)
                return;
            textBoxResonanceFrequency.Tag = listBoxFrequencies.SelectedIndices[0];
            textBoxResonanceFrequency.Text = listBoxFrequencies.Items[(int)textBoxResonanceFrequency.Tag].ToString();
        }
    }
}
