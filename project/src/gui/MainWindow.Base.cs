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
            ToolTip toolTip = new ToolTip();
            toolTip.InitialDelay = 200;
            toolTip.ShowAlways = true;
            toolTip.SetToolTip(buttonAddTemplateObject, "Press A to add a template object");
            toolTip.SetToolTip(buttonRemoveTemplateObject, "Press D to remove a template object");
            toolTip.SetToolTip(buttonEditTemplateSelection, "Press E to edit a template object");
            toolTip.SetToolTip(buttonCopyTemplateObjects, "Press C to copy all the template objects from the selected chart");
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
                createComponentStrips();
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

        private void buttonEditTemplateSelection_Click(object sender = null, EventArgs e = null)
        {
            // If not a line was selected
            if (!isEditSelection_ && (treeTemplateObjects.SelectedNode == null || treeTemplateObjects.SelectedNode.Nodes.Count == 0))
                return;
            if (isEditSelection_)
            {
                TreeNode line = treeTemplateObjects.Nodes[iSelectedSet_];
                line.Nodes.Clear();
                var selection = modelRenderer_.getSelection();
                string chart = listBoxTemplateCharts.SelectedItem.ToString();
                List<ISelection> objects = chartSelection_[chart];
                Lines selLine = null;
                int iSelectedLine = 0;
                bool isFound = false;
                foreach (ISelection item in objects)
                {
                    selLine = (Lines)item;
                    if (selLine.lineName_ == line.Text) {
                        isFound = true;
                        break;
                    }
                    ++iSelectedLine;
                }
                if (isFound == false)
                    return;
                if (selection.Count < 2)
                {
                    chartSelection_[chart].RemoveAt(iSelectedLine);
                    line.Remove();
                }
                else
                {
                    selLine.nodeNames_.Clear();
                    foreach (string item in selection)
                    {
                        line.Nodes.Add(item);
                        selLine.nodeNames_.Add(item);
                    }
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
            buttonAddTemplateObject.Enabled = isEditSelection_;
            buttonRemoveTemplateObject.Enabled = isEditSelection_;
            listBoxTemplateCharts.Enabled = isEditSelection_;
            isEditSelection_ = !isEditSelection_;
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
            treeTemplateObjects.Nodes.Clear();
            List<string> charts = excelTemplate_.getChartNames();
            chartTypes_ = new Dictionary<string, ChartTypes>();
            chartUnits_ = new Dictionary<string, SignalUnits>();
            chartSelection_ = new Dictionary<string, List<ISelection>>();
            chartDirection_ = new Dictionary<string, ChartDirection>();
            chartNormalization_ = new Dictionary<string, double>();
            chartAxis_ = new Dictionary<string, ChartDirection>();
            foreach (string chart in charts) { 
                listBoxTemplateCharts.Items.Add(chart);
                ChartTypes defaultType = ChartTypes.UNKNOWN;
                SignalUnits defaultUnits = SignalUnits.UNKNOWN;
                // Presetting based on a chart name
                string lowerChart = chart.ToLower();
                if (lowerChart.Contains("мним") || lowerChart.Contains("imag"))
                {
                    defaultType = ChartTypes.IMAGFRF;
                    defaultUnits = SignalUnits.METERS_PER_SECOND2;
                }
                if (lowerChart.Contains("реал") || lowerChart.Contains("действ") || lowerChart.Contains("real"))
                {
                    defaultType = ChartTypes.REALFRF;
                    defaultUnits = SignalUnits.METERS_PER_SECOND2;
                }
                if (lowerChart.Contains("-ф") )
                {
                    defaultType = ChartTypes.MODESET;
                    defaultUnits = SignalUnits.METERS_PER_SECOND2;
                }
                // Specifying data
                chartTypes_.Add(chart, defaultType);
                chartUnits_.Add(chart, defaultUnits);
                chartDirection_.Add(chart, ChartDirection.UNKNOWN);
                chartSelection_.Add(chart, new List<ISelection>());
                chartNormalization_.Add(chart, 1.0);
                chartAxis_.Add(chart, ChartDirection.UNKNOWN);
            }
            listBoxTemplateCharts.SelectedIndex = 0;
            string selectedChart = listBoxTemplateCharts.SelectedItem.ToString();
            comboBoxTemplateType.SelectedIndex = (int)chartTypes_[selectedChart];
            comboBoxTemplateUnits.SelectedIndex = (int)chartUnits_[selectedChart];
        }

        private void listBoxTemplateCharts_SelectedIndexChanged(object sender = null, EventArgs e = null)
        {          
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            ChartTypes type = chartTypes_[chart];
            // Copy template objects
            if (!buttonCopyTemplateObjects.Enabled)
            {
                buttonCopyTemplateObjects.Enabled = true;
                string baseChart = buttonCopyTemplateObjects.Tag.ToString();
                if (baseChart.Equals(chart))
                    return;
                listBoxTemplateCharts.SelectedItem = baseChart;
                ChartTypes baseType = chartTypes_[baseChart];
                if (chartSelection_[chart].Count == 0 || baseType == ChartTypes.UNKNOWN || type == ChartTypes.UNKNOWN)
                    return;
                convertSelection(baseChart, chart, baseType, type);
                type = baseType;
                chart = baseChart;
            }
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
            if (isNodeType(type))
            {
                List<ISelection> objects = chartSelection_[chart];
                foreach (ISelection item in objects)
                    treeTemplateObjects.Nodes.Add((string)item.retrieveSelection());
            }
            // Lines
            if (isLineType(type))
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

        private void convertSelection(string baseChart, string refChart, ChartTypes baseType, ChartTypes refType)
        {
            List<ISelection> refObjects = chartSelection_[refChart];
            List<ISelection> tempObjects = refObjects.GetRange(0, refObjects.Count);
            // Convert to nodes
            if (isNodeType(baseType))
            {
                // Nodes -> Nodes
                if (isNodeType(refType))
                    chartSelection_[baseChart] = tempObjects;
                // Lines -> Nodes
                if (isLineType(refType)) { 
                    foreach (ISelection item in tempObjects)
                    {
                        List<string> nodeNames = (List<string>)item.retrieveSelection();
                        foreach (string name in nodeNames)
                            chartSelection_[baseChart].Add(new Nodes { nodeName_ = name });
                    }
                }
            }
            // Convert to lines
            if (isLineType(baseType))
            {
                // Lines -> Lines
                if (isLineType(refType))
                    chartSelection_[baseChart] = tempObjects;
                // Nodes -> Line
                if (isNodeType(refType))
                {
                    Lines line = new Lines();
                    line.nodeNames_ = new List<string>();
                    line.lineName_ = "Line" + indLine_.ToString();
                    foreach (ISelection item in tempObjects)
                        line.nodeNames_.Add((string)item.retrieveSelection());
                    List<ISelection> res = new List<ISelection>();
                    res.Add((ISelection)line);
                    chartSelection_[baseChart] = res;
                }

            }
        }

        private bool isNodeType(ChartTypes type)
        {
            return type == ChartTypes.REALFRF || type == ChartTypes.IMAGFRF || type == ChartTypes.FORCE;
        }

        private bool isLineType(ChartTypes type)
        {
            return type == ChartTypes.MODESET;
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

        private void buttonAddTemplateObject_Click(object sender = null, EventArgs e = null)
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
            if (nSelection > 1 && isLineType(type))
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
            if (isNodeType(type))
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

        private void buttonRemoveTemplateObject_Click(object sender = null, EventArgs e = null)
        {
            if (treeTemplateObjects.SelectedNode == null || treeTemplateObjects.SelectedNode.Parent != null)
                return;
            int iSelected = treeTemplateObjects.SelectedNode.Index;
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            chartSelection_[chart].RemoveAt(iSelected);
            treeTemplateObjects.Nodes.RemoveAt(iSelected);
        }

        private void treeTemplateObjects_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData) {
                case Keys.D:
                    buttonRemoveTemplateObject_Click();
                    break;
                case Keys.E:
                    buttonEditTemplateSelection_Click();
                    break;
            }
        }

        private void ResponseAnalyzer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData) { 
                case Keys.A:
                    buttonAddTemplateObject_Click();
                    break;
                case Keys.C:
                    buttonCopyTemplateObjects_Click();
                    break;
            }
        }

        private void buttonCopyTemplateObjects_Click(object sender = null, EventArgs e = null)
        {
            if (listBoxTemplateCharts.SelectedIndex >= 0)
            {
                buttonCopyTemplateObjects.Enabled = false;
                buttonCopyTemplateObjects.Tag = listBoxTemplateCharts.SelectedItem.ToString();
            }
        }

        private void createComponentStrips()
        {
            List<string> componentNames = modelRenderer_.getComponentNames();
            ToolStripItemCollection items = stripComponentVisualisation.DropDownItems;
            items.Clear();
            foreach (string component in componentNames) {
                ToolStripMenuItem item = (ToolStripMenuItem)items.Add(component);
                item.Checked = true;
                item.CheckOnClick = true;
                item.CheckedChanged += new EventHandler(selectComponents);
            }
        }

    }
}
