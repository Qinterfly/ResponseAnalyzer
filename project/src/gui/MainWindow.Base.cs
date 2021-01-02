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

        private void buttonEditTemplateSelection_Click(object sender, EventArgs e)
        {
            // If not a line was selected
            if (treeTemplateObjects.SelectedNode == null || treeTemplateObjects.SelectedNode.Nodes.Count == 0)
                return;
            if (isEditSelection)
            {
                TreeNode line = treeTemplateObjects.Nodes[iSelectedSet_];
                line.Nodes.Clear();
                var selection = modelRenderer_.getSelection();
                if (selection.Count < 2)
                {
                    line.Remove();
                    chartSelection_[]
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
                iSelectedSet_ = treeTemplateObjects.SelectedNode.Index;
                modelRenderer_.clearSelection();
                // Select all the nodes in the set
                foreach (TreeNode item in treeTemplateObjects.SelectedNode.Nodes)
                {
                    string[] selectionInfo = item.Text.Split(selectionDelimiter_);
                    modelRenderer_.select(selectionInfo[0], selectionInfo[1], false);
                }
                modelRenderer_.draw();
            }
            // Invert the states of the controls
            buttonAddTemplateObject.Enabled = isEditSelection;
            buttonRemoveTemplateObject.Enabled = isEditSelection;
            isEditSelection = !isEditSelection;
        }


        private void removeNodeFromTree(TreeNode node)
        {
            string selectedNode = node.Text;
            string[] selectionInfo = selectedNode.Split(selectionDelimiter_);
            modelRenderer_.removeSelection(selectionInfo[0], selectionInfo[1]);
        }

        private void setEnabled()
        {
            // Excel template
            bool flag = excelTemplate_ != null;
            buttonAddTemplateObject.Enabled = flag;
            buttonEditTemplateSelection.Enabled = flag;
            buttonRemoveTemplateObject.Enabled = flag;
            comboBoxTemplateType.Enabled = flag;
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
            chartSelection_ = new Dictionary<string, List<ISelection>>();
            chartDirection_ = new Dictionary<string, ChartDirection>();
            chartNormalization_ = new Dictionary<string, double>();
            chartAxis_ = new Dictionary<string, ChartDirection>();
            foreach (string chart in charts) { 
                listBoxTemplateCharts.Items.Add(chart);
                chartTypes_.Add(chart, ChartTypes.UNKNOWN);
                chartUnits_.Add(chart, SignalUnits.UNKNOWN);
                chartDirection_.Add(chart, ChartDirection.UNKNOWN);
                chartSelection_.Add(chart, new List<ISelection>());
                chartNormalization_.Add(chart, 1.0);
                chartAxis_.Add(chart, ChartDirection.UNKNOWN);
            }
            listBoxTemplateCharts.SelectedIndex = 0;
        }

        private void listBoxTemplateCharts_SelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            ChartTypes type = chartTypes_[chart];
            comboBoxTemplateType.SelectedIndex = (int)type;
            comboBoxTemplateUnits.SelectedIndex = (int)chartUnits_[chart];
            comboBoxTemplateDirection.SelectedIndex = (int)chartDirection_[chart];
            comboBoxTemplateAxis.SelectedIndex = (int)chartAxis_[chart];
            numericTemplateNormalization.Value = (decimal)chartNormalization_[chart];
            treeTemplateObjects.Nodes.Clear();
            // Check if the type is defined
            if (type == ChartTypes.UNKNOWN)
                return;
            // Nodes
            if (type == ChartTypes.REALFRF || type == ChartTypes.IMAGFRF || type == ChartTypes.FORCE)
            {
                List<ISelection> objects = chartSelection_[chart];
                foreach (ISelection item in objects)
                    treeTemplateObjects.Nodes.Add((string)item.retrieveSelection());
            }
            // Lines
            if (type == ChartTypes.MODESET)
            {
                List<ISelection> objects = chartSelection_[chart];
                foreach (ISelection item in objects)
                {
                    Lines line = (Lines)item;
                    TreeNode parent = treeTemplateObjects.Nodes.Add(line.lineName_);
                    foreach (string node in line.nodeNames_)
                        parent.Nodes.Add(node);
                }
            }
        }

        private void comboBoxTemplateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            ChartTypes iSelected = (ChartTypes)comboBoxTemplateType.SelectedIndex;
            chartTypes_[chart] = iSelected;
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

        private void numericNormalization_ValueChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            chartNormalization_[listBoxTemplateCharts.SelectedItem.ToString()] = (double)numericTemplateNormalization.Value;
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
            // Check the type of the current chart
            ChartTypes type = (ChartTypes)comboBoxTemplateType.SelectedIndex;
            if (type == ChartTypes.UNKNOWN)
                return;
            var selection = modelRenderer_.getSelection();
            int nSelection = selection.Count;
            if (nSelection == 0)
                return;
            // Adding the object to the tree
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            if (nSelection > 1 && type == ChartTypes.MODESET)
            {
                TreeNode line = treeTemplateObjects.Nodes.Add("Line" + indLine_.ToString());
                foreach (string item in selection)
                    line.Nodes.Add(item);
                ++indLine_;
                Lines selLines = new Lines();
                selLines.lineName_ = line.Text;
                selLines.nodeNames_ = selection;
                chartSelection_[chart].Add(selLines);
            }
            if (type == ChartTypes.REALFRF || type == ChartTypes.IMAGFRF || type == ChartTypes.FORCE)
            {
                foreach (string node in selection)
                { 
                    treeTemplateObjects.Nodes.Add(node);
                    Nodes selNode = new Nodes();
                    selNode.nodeName_ = node;
                    chartSelection_[chart].Add(selNode);
                }
            }
        }

        private void buttonRemoveTemplateObject_Click(object sender, EventArgs e)
        {
            if (treeTemplateObjects.SelectedNode == null || treeTemplateObjects.SelectedNode.Parent != null)
                return;
            int iSelected = treeTemplateObjects.SelectedNode.Index;
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            chartSelection_[chart].RemoveAt(iSelected);
            treeTemplateObjects.Nodes.RemoveAt(iSelected);
        }

        private List<string> retrieveNodes(List<int> listNodeIndices)
        {
            List<string> selectedNodes = new List<string>();
            //bool isLine;
            //foreach (int index in listNodeIndices)
            //{
            //    TreeNode mainNode = treeSelection.Nodes[index];
            //    isLine = mainNode.Nodes.Count != 0;
            //    if (!isLine)
            //        selectedNodes.Add(mainNode.Text);
            //}
            return selectedNodes;
        }

        private List<string> retrieveNodesFromLine(int index)
        {
            List<string> selectedNodes = new List<string>();
            //bool isLine;
            //TreeNode mainNode = treeSelection.Nodes[index];
            //isLine = mainNode.Nodes.Count != 0;
            //if (isLine)
            //{
            //    foreach (TreeNode node in mainNode.Nodes)
            //        selectedNodes.Add(node.Text);
            //}
            return selectedNodes;
        }


    }
}
