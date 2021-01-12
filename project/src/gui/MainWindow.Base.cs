using System;
using System.Windows.Forms;
using System.Collections.Generic;
using OpenTK;

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
            toolTip.SetToolTip(numericTemplateNormalization, "Press F1 to fill it out automatically based on the selected lines");
        }

        // Opening a project
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
                setProjectEnabled();
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

        // Opening an Excel template
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
                    setProjectEnabled();
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

        // Updating all template properties
        private void updateExcelTemplateList()
        {
            listBoxTemplateCharts.Items.Clear();
            treeTemplateObjects.Nodes.Clear();
            List<string> charts = excelTemplate_.getChartNames();
            // Preparing containers to hold the properties of charts
            chartTypes_ = new Dictionary<string, ChartTypes>();
            chartUnits_ = new Dictionary<string, SignalUnits>();
            chartSelection_ = new Dictionary<string, List<ISelection>>();
            chartDirection_ = new Dictionary<string, ChartDirection>();
            chartNormalization_ = new Dictionary<string, double>();
            chartAxis_ = new Dictionary<string, ChartDirection>();
            chartSwapAxes_ = new Dictionary<string, bool>();
            chartDependency_ = new Dictionary<string, string>();
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
                // Specifying the data
                chartTypes_.Add(chart, defaultType);
                chartUnits_.Add(chart, defaultUnits);
                chartDirection_.Add(chart, ChartDirection.UNKNOWN);
                chartSelection_.Add(chart, new List<ISelection>());
                chartNormalization_.Add(chart, 1.0);
                chartAxis_.Add(chart, ChartDirection.UNKNOWN);
                chartSwapAxes_.Add(chart, false);
                chartDependency_.Add(chart, null);
            }
            listBoxTemplateCharts.SelectedIndex = 0;
            string selectedChart = listBoxTemplateCharts.SelectedItem.ToString();
            comboBoxTemplateType.SelectedIndex = (int)chartTypes_[selectedChart];
            comboBoxTemplateUnits.SelectedIndex = (int)chartUnits_[selectedChart];
            // Constructing dependencies
            createDependency(ChartTypes.IMAGFRF, ChartTypes.REALFRF);
            setDependencyEnabled();
        }

        private void ResponseAnalyzer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                // Add a template object
                case Keys.A:
                    buttonAddTemplateObject_Click();
                    break;
                // Copy template objects from a selected chart
                case Keys.C:
                    buttonCopyTemplateObjects_Click();
                    break;
                // Edit a selected line
                case Keys.E:
                    buttonEditTemplateSelection_Click();
                    break;
                // Clear the selection
                case Keys.Escape:
                    modelRenderer_.clearSelection();
                    modelRenderer_.draw();
                    clearStatus();
                    break;
            }
        }

        // -- Template  ---------------------------------------------------------------------------------------------------------------------------

        private void listBoxTemplateCharts_SelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            ChartTypes type = chartTypes_[chart];
            // Copy template objects
            if (!buttonCopyTemplateObjects.Enabled && buttonCopyTemplateObjects.Tag != null)
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
            checkBoxSwapAxes.Checked = chartSwapAxes_[chart];
            treeTemplateObjects.Nodes.Clear();
            // Check if the type is defined
            if (type == ChartTypes.UNKNOWN)
                return;
            // Check if the selected signal is dependent
            setDependencyEnabled();
            if (chartDependency_[chart] != null)
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

        // -- Setters ---------------------------------------------------------------------------------------------------------------------

        private void setDependencyEnabled()
        {
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            bool isDependent = chartDependency_[chart] == null;
            treeTemplateObjects.Enabled = isDependent;
            // Properties
            comboBoxTemplateUnits.Enabled = isDependent;
            comboBoxTemplateDirection.Enabled = isDependent;
            numericTemplateNormalization.Enabled = isDependent;
            comboBoxTemplateAxis.Enabled = isDependent;
            checkBoxSwapAxes.Enabled = isDependent;
            // Manage
            buttonAddTemplateObject.Enabled = isDependent;
            buttonRemoveTemplateObject.Enabled = isDependent;
            buttonCopyTemplateObjects.Enabled = isDependent;
            buttonCopyTemplateObjects.Tag = null;
            buttonEditTemplateSelection.Enabled = isDependent;
        }

        // Set a state of the project controls
        private void setProjectEnabled()
        {
            // Excel template
            bool flag = excelTemplate_ != null;
            buttonAddTemplateObject.Enabled = flag;
            buttonEditTemplateSelection.Enabled = flag;
            buttonRemoveTemplateObject.Enabled = flag;
            comboBoxTemplateType.Enabled = flag;
        }

        // Set the status label
        private void setStatus(string status)
        {
            statusStripLabel.Text = status;
        }

        // -- Clearing -----------------------------------------------------------------------------------------------------------------------

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
    }
}
