using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ResponseAnalyzer
{
    public partial class ResponseAnalyzer : Form
    {
        public ResponseAnalyzer()
        {
            InitializeComponent();
            comboBoxTemplateType.SelectedIndex = 0;
            comboBoxTestlabSelectionMode.SelectedIndex = 0;
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
                singleFrequencyIndices_ = new List<int>();
                multiFrequency_ = new Dictionary<string, double[]>();
                multiFrequencyIndices_ = new Dictionary<string, List<int>>();
                mapResponses_ = new Dictionary<string, string>();
                if (excelTemplate_ != null && excelTemplate_.isOpened())
                    updateExcelTemplateList();
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
            List<string> chartNames = excelTemplate_.getChartNames();
            // Preparing containers to hold the properties of charts
            charts_ = new ChartsData();
            foreach (string chart in chartNames) { 
                listBoxTemplateCharts.Items.Add(chart);
                ChartTypes defaultType = ChartTypes.UNKNOWN;
                SignalUnits defaultUnits = SignalUnits.UNKNOWN;
                // Presetting based on a chart name
                string lowerChart = chart.ToLower();
                // FRF
                if (lowerChart.Contains("мним"))
                {
                    defaultType = ChartTypes.IMAG_FRF;
                    if (lowerChart.Contains("-вр"))
                        defaultType = ChartTypes.MULTI_IMAG_FRF;
                    defaultUnits = SignalUnits.METERS_PER_SECOND2;
                }
                else if (lowerChart.Contains("реал") || lowerChart.Contains("действ"))
                {
                    defaultType = ChartTypes.REAL_FRF;
                    if (lowerChart.Contains("-вр"))
                        defaultType = ChartTypes.MULTI_REAL_FRF;
                    defaultUnits = SignalUnits.METERS_PER_SECOND2;
                }
                // Modeset
                else if (lowerChart.Contains("-ф") )
                {
                    defaultType = ChartTypes.MODESET;
                    defaultUnits = SignalUnits.METERS_PER_SECOND2;
                }
                // Frequency function
                else if (lowerChart.Contains("f(a)"))
                {
                    defaultType = ChartTypes.IMAG_FREQUENCY;
                    defaultUnits = SignalUnits.METERS_PER_SECOND2;
                }
                // Specifying the data
                charts_.type.Add(chart, defaultType);
                charts_.units.Add(chart, defaultUnits);
                charts_.direction.Add(chart, ChartDirection.UNKNOWN);
                charts_.selection.Add(chart, new List<ISelection>());
                charts_.normalization.Add(chart, 1.0);
                charts_.axis.Add(chart, ChartDirection.UNKNOWN);
                charts_.swapAxes.Add(chart, false);
                charts_.dependency.Add(chart, null);
            }
            listBoxTemplateCharts.SelectedIndex = 0;
            string selectedChart = listBoxTemplateCharts.SelectedItem.ToString();
            comboBoxTemplateType.SelectedIndex = (int)charts_.type[selectedChart];
            comboBoxTemplateUnits.SelectedIndex = (int)charts_.units[selectedChart];
            // Constructing dependencies
            createDependency(ChartTypes.IMAG_FRF, ChartTypes.REAL_FRF);
            createDependency(ChartTypes.MULTI_IMAG_FRF, ChartTypes.MULTI_REAL_FRF);
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
            ChartTypes type = charts_.type[chart];
            // Copy template objects
            if (!buttonCopyTemplateObjects.Enabled && buttonCopyTemplateObjects.Tag != null)
            {
                buttonCopyTemplateObjects.Enabled = true;
                string baseChart = buttonCopyTemplateObjects.Tag.ToString();
				buttonCopyTemplateObjects.Tag = null;
                if (baseChart.Equals(chart))
                    return;
                listBoxTemplateCharts.SelectedItem = baseChart;
                ChartTypes baseType = charts_.type[baseChart];
                if (charts_.selection[baseChart].Count == 0 || baseType == ChartTypes.UNKNOWN || type == ChartTypes.UNKNOWN)
                    return;
                convertSelection(chart, baseChart, type, baseType);
                type = baseType;
                chart = baseChart;
            }
            comboBoxTemplateType.SelectedIndex = (int)type;
            comboBoxTemplateUnits.SelectedIndex = (int)charts_.units[chart];
            comboBoxTemplateDirection.SelectedIndex = (int)charts_.direction[chart];
            comboBoxTemplateAxis.SelectedIndex = (int)charts_.axis[chart];
            numericTemplateNormalization.Value = (decimal)charts_.normalization[chart];
            checkBoxSwapAxes.Checked = charts_.swapAxes[chart];
            treeTemplateObjects.Nodes.Clear();
            // Check if the type is defined
            if (type == ChartTypes.UNKNOWN)
                return;
            // Check if the selected signal is dependent
            setDependencyEnabled();
            if (charts_.dependency[chart] != null)
                return;
            // Nodes
            if (isNodeType(type))
            {
                List<ISelection> objects = charts_.selection[chart];
                foreach (ISelection item in objects)
                    treeTemplateObjects.Nodes.Add((string)item.retrieveSelection());
            }
            // Lines
            if (isLineType(type))
            {
                List<ISelection> objects = charts_.selection[chart];
                foreach (ISelection item in objects)
                {
                    Lines line = (Lines)item;
                    TreeNode parent = treeTemplateObjects.Nodes.Add(line.lineName_);
                    foreach (string node in line.nodeNames_)
                        parent.Nodes.Add(node);
                }
            }
        }
        
        // Save template settings
        private void buttonSaveTemplateSettings_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "ResponseAnalyzer settings files (*" + ChartsData.binaryExtension + ")|*" + ChartsData.binaryExtension;
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK) 
                charts_.write(dialog.FileName);
        }

        // Read template settings from a file
        private void buttonOpenTemplateSettings_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "ResponseAnalyzer settings files (*" + ChartsData.binaryExtension + ")|*" + ChartsData.binaryExtension;
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
                charts_.read(dialog.FileName, modelRenderer_.containesNode, selectionDelimiter_);
            listBoxTemplateCharts_SelectedIndexChanged();
        }

        // -- Setters ---------------------------------------------------------------------------------------------------------------------

        private void setDependencyEnabled()
        {
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            bool isDependent = charts_.dependency[chart] == null;
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
            buttonCopyTemplateObjects.Enabled = flag;
            comboBoxTemplateType.Enabled = flag;
            buttonSaveTemplateSettings.Enabled = flag;
            buttonOpenTemplateSettings.Enabled = flag;
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
