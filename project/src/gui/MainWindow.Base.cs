using System;
using System.Windows.Forms;
using System.Collections.Generic;
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
                if (project.isProjectOpened()) {
                    textBoxProjectPath.Text = filePath;
                    setStatus("The project was successfully opened");
                }
                modelRenderer_.setGeometry(project.geometry_);
                modelRenderer_.setView(LMSModel.Views.ISOMETRIC);
                modelRenderer_.draw();
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
            if (treeSelection.SelectedNode == null || templateType == ChartTypes.UNKNOWN)
                return;
            // Check if an appopriate object was chosen
            bool isNeedInsertion = false;
            TreeNode node = treeSelection.SelectedNode;
            bool isLineSelected = node.Nodes.Count != 0;
            if (templateType == ChartTypes.REALFRF || templateType == ChartTypes.IMAGFRF)
                isNeedInsertion = !isLineSelected;
            if (templateType == ChartTypes.MODESHAPE)
                isNeedInsertion = isLineSelected;
            if (!isNeedInsertion)
                return;
            // Adding the object
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
            public const float rotation = 0.6f;
        }


    }
}
