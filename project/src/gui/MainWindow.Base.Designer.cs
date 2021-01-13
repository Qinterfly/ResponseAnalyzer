using System.Collections.Generic;

namespace ResponseAnalyzer
{
    partial class ResponseAnalyzer
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // Data
        private LMSProject project;
        private LMSModel modelRenderer_;
        // Charts
        private ExcelObject excelTemplate_;
        private ChartsData charts_;
        // Opengl
        private int[] lastMousePosition_;
        private bool isTranslation_ = false;
        private bool isRotation_ = false;
        // Selection
        private int indLine_ = 0;
        private const char selectionDelimiter_ = ':';
        private bool isEditSelection_ = false;
        private int iSelectedSet_ = -1;

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResponseAnalyzer));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.layoutControlPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tabStages = new System.Windows.Forms.TabControl();
            this.tabTemplate = new System.Windows.Forms.TabPage();
            this.groupBoxExcelTemplate = new System.Windows.Forms.GroupBox();
            this.layoutExcelTemplate = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.labelExcelPath = new System.Windows.Forms.Label();
            this.textBoxExcelTemplatePath = new System.Windows.Forms.TextBox();
            this.buttonOpenExcelTemplate = new System.Windows.Forms.Button();
            this.layoutTemplateObjects = new System.Windows.Forms.TableLayoutPanel();
            this.listBoxTemplateCharts = new System.Windows.Forms.ListBox();
            this.treeTemplateObjects = new System.Windows.Forms.TreeView();
            this.layoutTemplateConfiguration = new System.Windows.Forms.TableLayoutPanel();
            this.layoutExcelTemplateControls = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAddTemplateObject = new System.Windows.Forms.Button();
            this.buttonEditTemplateSelection = new System.Windows.Forms.Button();
            this.buttonRemoveTemplateObject = new System.Windows.Forms.Button();
            this.buttonCopyTemplateObjects = new System.Windows.Forms.Button();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxTemplateType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxTemplateDirection = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxTemplateUnits = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelTemplateNormalization = new System.Windows.Forms.Label();
            this.numericTemplateNormalization = new System.Windows.Forms.NumericUpDown();
            this.labelTemplateAxis = new System.Windows.Forms.Label();
            this.comboBoxTemplateAxis = new System.Windows.Forms.ComboBox();
            this.checkBoxSwapAxes = new System.Windows.Forms.CheckBox();
            this.layoutLoadTemplate = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonOpenTemplateSettings = new System.Windows.Forms.Button();
            this.buttonSaveTemplateSettings = new System.Windows.Forms.Button();
            this.tabMeasure = new System.Windows.Forms.TabPage();
            this.tabProcess = new System.Windows.Forms.TabPage();
            this.layoutProcess = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxChannelSelection = new System.Windows.Forms.GroupBox();
            this.layoutChannelSelection = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxResonanceFrequency = new System.Windows.Forms.TextBox();
            this.buttonSelectResonanceFrequency = new System.Windows.Forms.Button();
            this.layoutTestlabSelection = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSelectTestLab = new System.Windows.Forms.Button();
            this.labelSelectionInfo = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.listBoxFoundSignals = new System.Windows.Forms.ListBox();
            this.listBoxFrequencies = new System.Windows.Forms.ListBox();
            this.buttonProcess = new System.Windows.Forms.Button();
            this.groupBoxExcelResult = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel13 = new System.Windows.Forms.TableLayoutPanel();
            this.labelNameExcel = new System.Windows.Forms.Label();
            this.textBoxNameExcel = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel14 = new System.Windows.Forms.TableLayoutPanel();
            this.labelDirectoryExcel = new System.Windows.Forms.Label();
            this.buttonSelectDirectory = new System.Windows.Forms.Button();
            this.textBoxDirectoryExcel = new System.Windows.Forms.TextBox();
            this.groupBoxProject = new System.Windows.Forms.GroupBox();
            this.layoutProjectPath = new System.Windows.Forms.TableLayoutPanel();
            this.labelProjectPath = new System.Windows.Forms.Label();
            this.buttonOpenProject = new System.Windows.Forms.Button();
            this.buttonUpdateProject = new System.Windows.Forms.Button();
            this.textBoxProjectPath = new System.Windows.Forms.TextBox();
            this.glWindow = new OpenTK.GLControl();
            this.glContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.stripPolygonMode = new System.Windows.Forms.ToolStripMenuItem();
            this.glPolygonModeLine = new System.Windows.Forms.ToolStripMenuItem();
            this.glPolygonModeFill = new System.Windows.Forms.ToolStripMenuItem();
            this.stripView = new System.Windows.Forms.ToolStripMenuItem();
            this.glViewFront = new System.Windows.Forms.ToolStripMenuItem();
            this.glViewBack = new System.Windows.Forms.ToolStripMenuItem();
            this.glViewUp = new System.Windows.Forms.ToolStripMenuItem();
            this.glViewDown = new System.Windows.Forms.ToolStripMenuItem();
            this.glViewLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.glViewRight = new System.Windows.Forms.ToolStripMenuItem();
            this.glViewIsometric = new System.Windows.Forms.ToolStripMenuItem();
            this.stripComponentVisualisation = new System.Windows.Forms.ToolStripMenuItem();
            this.stripNodeNames = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.layoutControlPanel.SuspendLayout();
            this.tabStages.SuspendLayout();
            this.tabTemplate.SuspendLayout();
            this.groupBoxExcelTemplate.SuspendLayout();
            this.layoutExcelTemplate.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.layoutTemplateObjects.SuspendLayout();
            this.layoutTemplateConfiguration.SuspendLayout();
            this.layoutExcelTemplateControls.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericTemplateNormalization)).BeginInit();
            this.layoutLoadTemplate.SuspendLayout();
            this.tabProcess.SuspendLayout();
            this.layoutProcess.SuspendLayout();
            this.groupBoxChannelSelection.SuspendLayout();
            this.layoutChannelSelection.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            this.layoutTestlabSelection.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.groupBoxExcelResult.SuspendLayout();
            this.tableLayoutPanel12.SuspendLayout();
            this.tableLayoutPanel13.SuspendLayout();
            this.tableLayoutPanel14.SuspendLayout();
            this.groupBoxProject.SuspendLayout();
            this.layoutProjectPath.SuspendLayout();
            this.glContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusStripLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 707);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1008, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusStripLabel
            // 
            this.statusStripLabel.BackColor = System.Drawing.Color.Transparent;
            this.statusStripLabel.Name = "statusStripLabel";
            this.statusStripLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.layoutControlPanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.glWindow, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1008, 707);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // layoutControlPanel
            // 
            this.layoutControlPanel.AutoSize = true;
            this.layoutControlPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.layoutControlPanel.ColumnCount = 1;
            this.layoutControlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layoutControlPanel.Controls.Add(this.tabStages, 0, 1);
            this.layoutControlPanel.Controls.Add(this.groupBoxProject, 0, 0);
            this.layoutControlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlPanel.Location = new System.Drawing.Point(3, 3);
            this.layoutControlPanel.MaximumSize = new System.Drawing.Size(600, 0);
            this.layoutControlPanel.Name = "layoutControlPanel";
            this.layoutControlPanel.RowCount = 2;
            this.layoutControlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutControlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutControlPanel.Size = new System.Drawing.Size(366, 701);
            this.layoutControlPanel.TabIndex = 11;
            // 
            // tabStages
            // 
            this.tabStages.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabStages.Controls.Add(this.tabTemplate);
            this.tabStages.Controls.Add(this.tabMeasure);
            this.tabStages.Controls.Add(this.tabProcess);
            this.tabStages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabStages.Location = new System.Drawing.Point(3, 56);
            this.tabStages.Name = "tabStages";
            this.tabStages.SelectedIndex = 0;
            this.tabStages.Size = new System.Drawing.Size(360, 642);
            this.tabStages.TabIndex = 10;
            // 
            // tabTemplate
            // 
            this.tabTemplate.BackColor = System.Drawing.Color.White;
            this.tabTemplate.Controls.Add(this.groupBoxExcelTemplate);
            this.tabTemplate.Location = new System.Drawing.Point(4, 4);
            this.tabTemplate.Name = "tabTemplate";
            this.tabTemplate.Padding = new System.Windows.Forms.Padding(3);
            this.tabTemplate.Size = new System.Drawing.Size(352, 614);
            this.tabTemplate.TabIndex = 0;
            this.tabTemplate.Text = "Template";
            // 
            // groupBoxExcelTemplate
            // 
            this.groupBoxExcelTemplate.Controls.Add(this.layoutExcelTemplate);
            this.groupBoxExcelTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxExcelTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBoxExcelTemplate.Location = new System.Drawing.Point(3, 3);
            this.groupBoxExcelTemplate.MaximumSize = new System.Drawing.Size(0, 1000);
            this.groupBoxExcelTemplate.Name = "groupBoxExcelTemplate";
            this.groupBoxExcelTemplate.Size = new System.Drawing.Size(346, 608);
            this.groupBoxExcelTemplate.TabIndex = 4;
            this.groupBoxExcelTemplate.TabStop = false;
            this.groupBoxExcelTemplate.Text = "Excel";
            // 
            // layoutExcelTemplate
            // 
            this.layoutExcelTemplate.ColumnCount = 1;
            this.layoutExcelTemplate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutExcelTemplate.Controls.Add(this.tableLayoutPanel8, 0, 0);
            this.layoutExcelTemplate.Controls.Add(this.layoutTemplateObjects, 0, 1);
            this.layoutExcelTemplate.Controls.Add(this.layoutTemplateConfiguration, 0, 2);
            this.layoutExcelTemplate.Controls.Add(this.layoutLoadTemplate, 0, 3);
            this.layoutExcelTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutExcelTemplate.Location = new System.Drawing.Point(3, 17);
            this.layoutExcelTemplate.Name = "layoutExcelTemplate";
            this.layoutExcelTemplate.RowCount = 4;
            this.layoutExcelTemplate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.layoutExcelTemplate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80.15414F));
            this.layoutExcelTemplate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.84586F));
            this.layoutExcelTemplate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.layoutExcelTemplate.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.layoutExcelTemplate.Size = new System.Drawing.Size(340, 588);
            this.layoutExcelTemplate.TabIndex = 1;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 3;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.47826F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 81.52174F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tableLayoutPanel8.Controls.Add(this.labelExcelPath, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.textBoxExcelTemplatePath, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.buttonOpenExcelTemplate, 2, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(334, 32);
            this.tableLayoutPanel8.TabIndex = 4;
            // 
            // labelExcelPath
            // 
            this.labelExcelPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelExcelPath.AutoSize = true;
            this.labelExcelPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelExcelPath.Location = new System.Drawing.Point(3, 8);
            this.labelExcelPath.Name = "labelExcelPath";
            this.labelExcelPath.Size = new System.Drawing.Size(44, 15);
            this.labelExcelPath.TabIndex = 0;
            this.labelExcelPath.Text = "Name:";
            // 
            // textBoxExcelTemplatePath
            // 
            this.textBoxExcelTemplatePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExcelTemplatePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxExcelTemplatePath.Location = new System.Drawing.Point(56, 5);
            this.textBoxExcelTemplatePath.Name = "textBoxExcelTemplatePath";
            this.textBoxExcelTemplatePath.ReadOnly = true;
            this.textBoxExcelTemplatePath.Size = new System.Drawing.Size(227, 21);
            this.textBoxExcelTemplatePath.TabIndex = 1;
            // 
            // buttonOpenExcelTemplate
            // 
            this.buttonOpenExcelTemplate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonOpenExcelTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonOpenExcelTemplate.Image = global::ResponseAnalyzer.Properties.Resources.add;
            this.buttonOpenExcelTemplate.Location = new System.Drawing.Point(289, 4);
            this.buttonOpenExcelTemplate.Name = "buttonOpenExcelTemplate";
            this.buttonOpenExcelTemplate.Size = new System.Drawing.Size(34, 23);
            this.buttonOpenExcelTemplate.TabIndex = 2;
            this.buttonOpenExcelTemplate.UseVisualStyleBackColor = true;
            this.buttonOpenExcelTemplate.Click += new System.EventHandler(this.buttonOpenExcelTemplate_Click);
            // 
            // layoutTemplateObjects
            // 
            this.layoutTemplateObjects.ColumnCount = 2;
            this.layoutTemplateObjects.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutTemplateObjects.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutTemplateObjects.Controls.Add(this.listBoxTemplateCharts, 0, 0);
            this.layoutTemplateObjects.Controls.Add(this.treeTemplateObjects, 1, 0);
            this.layoutTemplateObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutTemplateObjects.Location = new System.Drawing.Point(0, 38);
            this.layoutTemplateObjects.Margin = new System.Windows.Forms.Padding(0);
            this.layoutTemplateObjects.Name = "layoutTemplateObjects";
            this.layoutTemplateObjects.RowCount = 1;
            this.layoutTemplateObjects.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutTemplateObjects.Size = new System.Drawing.Size(340, 416);
            this.layoutTemplateObjects.TabIndex = 5;
            // 
            // listBoxTemplateCharts
            // 
            this.listBoxTemplateCharts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxTemplateCharts.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBoxTemplateCharts.FormattingEnabled = true;
            this.listBoxTemplateCharts.ItemHeight = 15;
            this.listBoxTemplateCharts.Location = new System.Drawing.Point(3, 3);
            this.listBoxTemplateCharts.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            this.listBoxTemplateCharts.Name = "listBoxTemplateCharts";
            this.listBoxTemplateCharts.Size = new System.Drawing.Size(164, 411);
            this.listBoxTemplateCharts.TabIndex = 0;
            this.listBoxTemplateCharts.SelectedIndexChanged += new System.EventHandler(this.listBoxTemplateCharts_SelectedIndexChanged);
            // 
            // treeTemplateObjects
            // 
            this.treeTemplateObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeTemplateObjects.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeTemplateObjects.Location = new System.Drawing.Point(173, 3);
            this.treeTemplateObjects.Margin = new System.Windows.Forms.Padding(3, 3, 3, 5);
            this.treeTemplateObjects.Name = "treeTemplateObjects";
            this.treeTemplateObjects.Size = new System.Drawing.Size(164, 408);
            this.treeTemplateObjects.TabIndex = 1;
            this.treeTemplateObjects.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeTemplateObjects_AfterSelect);
            this.treeTemplateObjects.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeTemplateObjects_KeyDown);
            // 
            // layoutTemplateConfiguration
            // 
            this.layoutTemplateConfiguration.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.layoutTemplateConfiguration.ColumnCount = 1;
            this.layoutTemplateConfiguration.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutTemplateConfiguration.Controls.Add(this.layoutExcelTemplateControls, 0, 0);
            this.layoutTemplateConfiguration.Controls.Add(this.flowLayoutPanel3, 0, 1);
            this.layoutTemplateConfiguration.Controls.Add(this.flowLayoutPanel4, 0, 2);
            this.layoutTemplateConfiguration.Location = new System.Drawing.Point(4, 457);
            this.layoutTemplateConfiguration.Name = "layoutTemplateConfiguration";
            this.layoutTemplateConfiguration.RowCount = 3;
            this.layoutTemplateConfiguration.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 43.47826F));
            this.layoutTemplateConfiguration.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 56.52174F));
            this.layoutTemplateConfiguration.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.layoutTemplateConfiguration.Size = new System.Drawing.Size(331, 97);
            this.layoutTemplateConfiguration.TabIndex = 6;
            // 
            // layoutExcelTemplateControls
            // 
            this.layoutExcelTemplateControls.Controls.Add(this.buttonAddTemplateObject);
            this.layoutExcelTemplateControls.Controls.Add(this.buttonEditTemplateSelection);
            this.layoutExcelTemplateControls.Controls.Add(this.buttonRemoveTemplateObject);
            this.layoutExcelTemplateControls.Controls.Add(this.buttonCopyTemplateObjects);
            this.layoutExcelTemplateControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutExcelTemplateControls.Location = new System.Drawing.Point(0, 0);
            this.layoutExcelTemplateControls.Margin = new System.Windows.Forms.Padding(0);
            this.layoutExcelTemplateControls.Name = "layoutExcelTemplateControls";
            this.layoutExcelTemplateControls.Size = new System.Drawing.Size(331, 26);
            this.layoutExcelTemplateControls.TabIndex = 6;
            // 
            // buttonAddTemplateObject
            // 
            this.buttonAddTemplateObject.Enabled = false;
            this.buttonAddTemplateObject.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonAddTemplateObject.Image = global::ResponseAnalyzer.Properties.Resources.rightArrow;
            this.buttonAddTemplateObject.Location = new System.Drawing.Point(3, 0);
            this.buttonAddTemplateObject.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.buttonAddTemplateObject.Name = "buttonAddTemplateObject";
            this.buttonAddTemplateObject.Size = new System.Drawing.Size(35, 23);
            this.buttonAddTemplateObject.TabIndex = 1;
            this.buttonAddTemplateObject.UseVisualStyleBackColor = true;
            this.buttonAddTemplateObject.Click += new System.EventHandler(this.buttonAddTemplateObject_Click);
            // 
            // buttonEditTemplateSelection
            // 
            this.buttonEditTemplateSelection.Enabled = false;
            this.buttonEditTemplateSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonEditTemplateSelection.Image = global::ResponseAnalyzer.Properties.Resources.edit;
            this.buttonEditTemplateSelection.Location = new System.Drawing.Point(44, 0);
            this.buttonEditTemplateSelection.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.buttonEditTemplateSelection.Name = "buttonEditTemplateSelection";
            this.buttonEditTemplateSelection.Size = new System.Drawing.Size(32, 23);
            this.buttonEditTemplateSelection.TabIndex = 2;
            this.buttonEditTemplateSelection.UseVisualStyleBackColor = true;
            this.buttonEditTemplateSelection.Click += new System.EventHandler(this.buttonEditTemplateSelection_Click);
            // 
            // buttonRemoveTemplateObject
            // 
            this.buttonRemoveTemplateObject.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonRemoveTemplateObject.Enabled = false;
            this.buttonRemoveTemplateObject.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonRemoveTemplateObject.Image = global::ResponseAnalyzer.Properties.Resources.leftArrow;
            this.buttonRemoveTemplateObject.Location = new System.Drawing.Point(82, 0);
            this.buttonRemoveTemplateObject.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.buttonRemoveTemplateObject.Name = "buttonRemoveTemplateObject";
            this.buttonRemoveTemplateObject.Size = new System.Drawing.Size(33, 23);
            this.buttonRemoveTemplateObject.TabIndex = 2;
            this.buttonRemoveTemplateObject.UseVisualStyleBackColor = true;
            this.buttonRemoveTemplateObject.Click += new System.EventHandler(this.buttonRemoveTemplateObject_Click);
            // 
            // buttonCopyTemplateObjects
            // 
            this.buttonCopyTemplateObjects.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonCopyTemplateObjects.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonCopyTemplateObjects.Image = global::ResponseAnalyzer.Properties.Resources.copy;
            this.buttonCopyTemplateObjects.Location = new System.Drawing.Point(121, 0);
            this.buttonCopyTemplateObjects.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.buttonCopyTemplateObjects.Name = "buttonCopyTemplateObjects";
            this.buttonCopyTemplateObjects.Size = new System.Drawing.Size(34, 23);
            this.buttonCopyTemplateObjects.TabIndex = 11;
            this.buttonCopyTemplateObjects.UseVisualStyleBackColor = true;
            this.buttonCopyTemplateObjects.Click += new System.EventHandler(this.buttonCopyTemplateObjects_Click);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.label1);
            this.flowLayoutPanel3.Controls.Add(this.comboBoxTemplateType);
            this.flowLayoutPanel3.Controls.Add(this.label3);
            this.flowLayoutPanel3.Controls.Add(this.comboBoxTemplateDirection);
            this.flowLayoutPanel3.Controls.Add(this.label4);
            this.flowLayoutPanel3.Controls.Add(this.comboBoxTemplateUnits);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 29);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(325, 27);
            this.flowLayoutPanel3.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Type: ";
            // 
            // comboBoxTemplateType
            // 
            this.comboBoxTemplateType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboBoxTemplateType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTemplateType.Enabled = false;
            this.comboBoxTemplateType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxTemplateType.FormattingEnabled = true;
            this.comboBoxTemplateType.Items.AddRange(new object[] {
            "",
            "Real",
            "Imaginary",
            "Modeset",
            "Force"});
            this.comboBoxTemplateType.Location = new System.Drawing.Point(45, 3);
            this.comboBoxTemplateType.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.comboBoxTemplateType.Name = "comboBoxTemplateType";
            this.comboBoxTemplateType.Size = new System.Drawing.Size(88, 23);
            this.comboBoxTemplateType.TabIndex = 3;
            this.comboBoxTemplateType.SelectedIndexChanged += new System.EventHandler(this.comboBoxTemplateType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(139, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Dir:";
            // 
            // comboBoxTemplateDirection
            // 
            this.comboBoxTemplateDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTemplateDirection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxTemplateDirection.FormattingEnabled = true;
            this.comboBoxTemplateDirection.Items.AddRange(new object[] {
            "",
            "X",
            "Y",
            "Z"});
            this.comboBoxTemplateDirection.Location = new System.Drawing.Point(171, 3);
            this.comboBoxTemplateDirection.Name = "comboBoxTemplateDirection";
            this.comboBoxTemplateDirection.Size = new System.Drawing.Size(40, 23);
            this.comboBoxTemplateDirection.TabIndex = 6;
            this.comboBoxTemplateDirection.SelectedIndexChanged += new System.EventHandler(this.comboBoxTemplateDirection_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(217, 8);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "Units:";
            // 
            // comboBoxTemplateUnits
            // 
            this.comboBoxTemplateUnits.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboBoxTemplateUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTemplateUnits.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxTemplateUnits.FormattingEnabled = true;
            this.comboBoxTemplateUnits.Items.AddRange(new object[] {
            "",
            "mm",
            "m/s^2"});
            this.comboBoxTemplateUnits.Location = new System.Drawing.Point(258, 3);
            this.comboBoxTemplateUnits.Margin = new System.Windows.Forms.Padding(0);
            this.comboBoxTemplateUnits.Name = "comboBoxTemplateUnits";
            this.comboBoxTemplateUnits.Size = new System.Drawing.Size(65, 23);
            this.comboBoxTemplateUnits.TabIndex = 4;
            this.comboBoxTemplateUnits.SelectedIndexChanged += new System.EventHandler(this.comboBoxTemplateUnits_SelectedIndexChanged);
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Controls.Add(this.labelTemplateNormalization);
            this.flowLayoutPanel4.Controls.Add(this.numericTemplateNormalization);
            this.flowLayoutPanel4.Controls.Add(this.labelTemplateAxis);
            this.flowLayoutPanel4.Controls.Add(this.comboBoxTemplateAxis);
            this.flowLayoutPanel4.Controls.Add(this.checkBoxSwapAxes);
            this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(3, 62);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(325, 32);
            this.flowLayoutPanel4.TabIndex = 10;
            // 
            // labelTemplateNormalization
            // 
            this.labelTemplateNormalization.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelTemplateNormalization.AutoSize = true;
            this.labelTemplateNormalization.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelTemplateNormalization.Location = new System.Drawing.Point(3, 7);
            this.labelTemplateNormalization.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this.labelTemplateNormalization.Name = "labelTemplateNormalization";
            this.labelTemplateNormalization.Size = new System.Drawing.Size(41, 15);
            this.labelTemplateNormalization.TabIndex = 7;
            this.labelTemplateNormalization.Text = "Norm:";
            // 
            // numericTemplateNormalization
            // 
            this.numericTemplateNormalization.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numericTemplateNormalization.DecimalPlaces = 3;
            this.numericTemplateNormalization.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.numericTemplateNormalization.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericTemplateNormalization.Location = new System.Drawing.Point(50, 4);
            this.numericTemplateNormalization.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericTemplateNormalization.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericTemplateNormalization.Name = "numericTemplateNormalization";
            this.numericTemplateNormalization.Size = new System.Drawing.Size(83, 21);
            this.numericTemplateNormalization.TabIndex = 8;
            this.numericTemplateNormalization.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericTemplateNormalization.ValueChanged += new System.EventHandler(this.numericNormalization_ValueChanged);
            this.numericTemplateNormalization.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericTemplateNormalization_KeyDown);
            // 
            // labelTemplateAxis
            // 
            this.labelTemplateAxis.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelTemplateAxis.AutoSize = true;
            this.labelTemplateAxis.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelTemplateAxis.Location = new System.Drawing.Point(139, 7);
            this.labelTemplateAxis.Name = "labelTemplateAxis";
            this.labelTemplateAxis.Size = new System.Drawing.Size(32, 15);
            this.labelTemplateAxis.TabIndex = 9;
            this.labelTemplateAxis.Text = "Axis:";
            // 
            // comboBoxTemplateAxis
            // 
            this.comboBoxTemplateAxis.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboBoxTemplateAxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTemplateAxis.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxTemplateAxis.FormattingEnabled = true;
            this.comboBoxTemplateAxis.Items.AddRange(new object[] {
            "",
            "X",
            "Y",
            "Z"});
            this.comboBoxTemplateAxis.Location = new System.Drawing.Point(177, 3);
            this.comboBoxTemplateAxis.Name = "comboBoxTemplateAxis";
            this.comboBoxTemplateAxis.Size = new System.Drawing.Size(40, 23);
            this.comboBoxTemplateAxis.TabIndex = 10;
            this.comboBoxTemplateAxis.SelectedIndexChanged += new System.EventHandler(this.comboBoxTemplateAxis_SelectedIndexChanged);
            // 
            // checkBoxSwapAxes
            // 
            this.checkBoxSwapAxes.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxSwapAxes.AutoSize = true;
            this.checkBoxSwapAxes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBoxSwapAxes.Location = new System.Drawing.Point(223, 5);
            this.checkBoxSwapAxes.Name = "checkBoxSwapAxes";
            this.checkBoxSwapAxes.Size = new System.Drawing.Size(86, 19);
            this.checkBoxSwapAxes.TabIndex = 11;
            this.checkBoxSwapAxes.Text = "Swap axes";
            this.checkBoxSwapAxes.UseVisualStyleBackColor = true;
            this.checkBoxSwapAxes.CheckedChanged += new System.EventHandler(this.checkBoxSwapAxes_CheckedChanged);
            // 
            // layoutLoadTemplate
            // 
            this.layoutLoadTemplate.Controls.Add(this.buttonSaveTemplateSettings);
            this.layoutLoadTemplate.Controls.Add(this.buttonOpenTemplateSettings);
            this.layoutLoadTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutLoadTemplate.Location = new System.Drawing.Point(0, 557);
            this.layoutLoadTemplate.Margin = new System.Windows.Forms.Padding(0);
            this.layoutLoadTemplate.Name = "layoutLoadTemplate";
            this.layoutLoadTemplate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.layoutLoadTemplate.Size = new System.Drawing.Size(340, 31);
            this.layoutLoadTemplate.TabIndex = 7;
            // 
            // buttonOpenTemplateSettings
            // 
            this.buttonOpenTemplateSettings.Enabled = false;
            this.buttonOpenTemplateSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonOpenTemplateSettings.Location = new System.Drawing.Point(203, 3);
            this.buttonOpenTemplateSettings.Name = "buttonOpenTemplateSettings";
            this.buttonOpenTemplateSettings.Size = new System.Drawing.Size(63, 23);
            this.buttonOpenTemplateSettings.TabIndex = 0;
            this.buttonOpenTemplateSettings.Text = "Open";
            this.buttonOpenTemplateSettings.UseVisualStyleBackColor = true;
            this.buttonOpenTemplateSettings.Click += new System.EventHandler(this.buttonOpenTemplateSettings_Click);
            // 
            // buttonSaveTemplateSettings
            // 
            this.buttonSaveTemplateSettings.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonSaveTemplateSettings.Enabled = false;
            this.buttonSaveTemplateSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonSaveTemplateSettings.Location = new System.Drawing.Point(272, 3);
            this.buttonSaveTemplateSettings.Name = "buttonSaveTemplateSettings";
            this.buttonSaveTemplateSettings.Size = new System.Drawing.Size(65, 23);
            this.buttonSaveTemplateSettings.TabIndex = 1;
            this.buttonSaveTemplateSettings.Text = "Save";
            this.buttonSaveTemplateSettings.UseVisualStyleBackColor = true;
            this.buttonSaveTemplateSettings.Click += new System.EventHandler(this.buttonSaveTemplateSettings_Click);
            // 
            // tabMeasure
            // 
            this.tabMeasure.Location = new System.Drawing.Point(4, 4);
            this.tabMeasure.Name = "tabMeasure";
            this.tabMeasure.Padding = new System.Windows.Forms.Padding(3);
            this.tabMeasure.Size = new System.Drawing.Size(352, 614);
            this.tabMeasure.TabIndex = 1;
            this.tabMeasure.Text = "Measure";
            this.tabMeasure.UseVisualStyleBackColor = true;
            // 
            // tabProcess
            // 
            this.tabProcess.Controls.Add(this.layoutProcess);
            this.tabProcess.Location = new System.Drawing.Point(4, 4);
            this.tabProcess.Name = "tabProcess";
            this.tabProcess.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcess.Size = new System.Drawing.Size(352, 614);
            this.tabProcess.TabIndex = 2;
            this.tabProcess.Text = "Process";
            this.tabProcess.UseVisualStyleBackColor = true;
            // 
            // layoutProcess
            // 
            this.layoutProcess.ColumnCount = 1;
            this.layoutProcess.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutProcess.Controls.Add(this.groupBoxChannelSelection, 0, 0);
            this.layoutProcess.Controls.Add(this.buttonProcess, 0, 2);
            this.layoutProcess.Controls.Add(this.groupBoxExcelResult, 0, 1);
            this.layoutProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutProcess.Location = new System.Drawing.Point(3, 3);
            this.layoutProcess.Margin = new System.Windows.Forms.Padding(0);
            this.layoutProcess.Name = "layoutProcess";
            this.layoutProcess.RowCount = 3;
            this.layoutProcess.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 84.20139F));
            this.layoutProcess.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15.79861F));
            this.layoutProcess.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.layoutProcess.Size = new System.Drawing.Size(346, 608);
            this.layoutProcess.TabIndex = 0;
            // 
            // groupBoxChannelSelection
            // 
            this.groupBoxChannelSelection.Controls.Add(this.layoutChannelSelection);
            this.groupBoxChannelSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxChannelSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBoxChannelSelection.Location = new System.Drawing.Point(0, 0);
            this.groupBoxChannelSelection.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxChannelSelection.Name = "groupBoxChannelSelection";
            this.groupBoxChannelSelection.Size = new System.Drawing.Size(346, 485);
            this.groupBoxChannelSelection.TabIndex = 0;
            this.groupBoxChannelSelection.TabStop = false;
            this.groupBoxChannelSelection.Text = "Channel selection";
            // 
            // layoutChannelSelection
            // 
            this.layoutChannelSelection.ColumnCount = 1;
            this.layoutChannelSelection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutChannelSelection.Controls.Add(this.flowLayoutPanel5, 0, 2);
            this.layoutChannelSelection.Controls.Add(this.layoutTestlabSelection, 0, 0);
            this.layoutChannelSelection.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.layoutChannelSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutChannelSelection.Location = new System.Drawing.Point(3, 17);
            this.layoutChannelSelection.Margin = new System.Windows.Forms.Padding(0);
            this.layoutChannelSelection.Name = "layoutChannelSelection";
            this.layoutChannelSelection.RowCount = 3;
            this.layoutChannelSelection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.356322F));
            this.layoutChannelSelection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 92.64368F));
            this.layoutChannelSelection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.layoutChannelSelection.Size = new System.Drawing.Size(340, 465);
            this.layoutChannelSelection.TabIndex = 0;
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.Controls.Add(this.label2);
            this.flowLayoutPanel5.Controls.Add(this.textBoxResonanceFrequency);
            this.flowLayoutPanel5.Controls.Add(this.buttonSelectResonanceFrequency);
            this.flowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel5.Location = new System.Drawing.Point(0, 435);
            this.flowLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(340, 30);
            this.flowLayoutPanel5.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Resonance frequency: ";
            // 
            // textBoxResonanceFrequency
            // 
            this.textBoxResonanceFrequency.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxResonanceFrequency.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxResonanceFrequency.Location = new System.Drawing.Point(141, 4);
            this.textBoxResonanceFrequency.Name = "textBoxResonanceFrequency";
            this.textBoxResonanceFrequency.ReadOnly = true;
            this.textBoxResonanceFrequency.Size = new System.Drawing.Size(73, 21);
            this.textBoxResonanceFrequency.TabIndex = 1;
            this.textBoxResonanceFrequency.Tag = "-1";
            // 
            // buttonSelectResonanceFrequency
            // 
            this.buttonSelectResonanceFrequency.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonSelectResonanceFrequency.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonSelectResonanceFrequency.Location = new System.Drawing.Point(220, 3);
            this.buttonSelectResonanceFrequency.Name = "buttonSelectResonanceFrequency";
            this.buttonSelectResonanceFrequency.Size = new System.Drawing.Size(58, 23);
            this.buttonSelectResonanceFrequency.TabIndex = 2;
            this.buttonSelectResonanceFrequency.Text = "Select";
            this.buttonSelectResonanceFrequency.UseVisualStyleBackColor = true;
            this.buttonSelectResonanceFrequency.Click += new System.EventHandler(this.buttonSelectResonanceFrequency_Click);
            // 
            // layoutTestlabSelection
            // 
            this.layoutTestlabSelection.ColumnCount = 2;
            this.layoutTestlabSelection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.79822F));
            this.layoutTestlabSelection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.20178F));
            this.layoutTestlabSelection.Controls.Add(this.buttonSelectTestLab, 1, 0);
            this.layoutTestlabSelection.Controls.Add(this.labelSelectionInfo, 0, 0);
            this.layoutTestlabSelection.Location = new System.Drawing.Point(0, 0);
            this.layoutTestlabSelection.Margin = new System.Windows.Forms.Padding(0);
            this.layoutTestlabSelection.Name = "layoutTestlabSelection";
            this.layoutTestlabSelection.RowCount = 1;
            this.layoutTestlabSelection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutTestlabSelection.Size = new System.Drawing.Size(337, 30);
            this.layoutTestlabSelection.TabIndex = 2;
            // 
            // buttonSelectTestLab
            // 
            this.buttonSelectTestLab.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonSelectTestLab.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonSelectTestLab.Location = new System.Drawing.Point(218, 3);
            this.buttonSelectTestLab.Name = "buttonSelectTestLab";
            this.buttonSelectTestLab.Size = new System.Drawing.Size(114, 23);
            this.buttonSelectTestLab.TabIndex = 1;
            this.buttonSelectTestLab.Text = "Select via TestLab";
            this.buttonSelectTestLab.UseVisualStyleBackColor = true;
            this.buttonSelectTestLab.Click += new System.EventHandler(this.buttonSelectTestLab_Click);
            // 
            // labelSelectionInfo
            // 
            this.labelSelectionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSelectionInfo.AutoSize = true;
            this.labelSelectionInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelSelectionInfo.Location = new System.Drawing.Point(3, 7);
            this.labelSelectionInfo.Name = "labelSelectionInfo";
            this.labelSelectionInfo.Size = new System.Drawing.Size(209, 15);
            this.labelSelectionInfo.TabIndex = 2;
            this.labelSelectionInfo.Text = "Selected signals: ";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.60544F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.39456F));
            this.tableLayoutPanel4.Controls.Add(this.listBoxFoundSignals, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.listBoxFrequencies, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 32);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(340, 403);
            this.tableLayoutPanel4.TabIndex = 4;
            // 
            // listBoxFoundSignals
            // 
            this.listBoxFoundSignals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxFoundSignals.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBoxFoundSignals.FormattingEnabled = true;
            this.listBoxFoundSignals.ItemHeight = 15;
            this.listBoxFoundSignals.Location = new System.Drawing.Point(0, 0);
            this.listBoxFoundSignals.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.listBoxFoundSignals.Name = "listBoxFoundSignals";
            this.listBoxFoundSignals.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBoxFoundSignals.Size = new System.Drawing.Size(213, 403);
            this.listBoxFoundSignals.TabIndex = 0;
            // 
            // listBoxFrequencies
            // 
            this.listBoxFrequencies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxFrequencies.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBoxFrequencies.FormattingEnabled = true;
            this.listBoxFrequencies.ItemHeight = 15;
            this.listBoxFrequencies.Location = new System.Drawing.Point(219, 0);
            this.listBoxFrequencies.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.listBoxFrequencies.Name = "listBoxFrequencies";
            this.listBoxFrequencies.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxFrequencies.Size = new System.Drawing.Size(121, 403);
            this.listBoxFrequencies.TabIndex = 1;
            this.listBoxFrequencies.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxFrequencies_KeyDown);
            // 
            // buttonProcess
            // 
            this.buttonProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonProcess.Location = new System.Drawing.Point(268, 579);
            this.buttonProcess.Name = "buttonProcess";
            this.buttonProcess.Size = new System.Drawing.Size(75, 23);
            this.buttonProcess.TabIndex = 3;
            this.buttonProcess.Text = "Process";
            this.buttonProcess.UseVisualStyleBackColor = true;
            this.buttonProcess.Click += new System.EventHandler(this.buttonProcess_Click);
            // 
            // groupBoxExcelResult
            // 
            this.groupBoxExcelResult.Controls.Add(this.tableLayoutPanel12);
            this.groupBoxExcelResult.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxExcelResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBoxExcelResult.Location = new System.Drawing.Point(3, 488);
            this.groupBoxExcelResult.Name = "groupBoxExcelResult";
            this.groupBoxExcelResult.Size = new System.Drawing.Size(340, 85);
            this.groupBoxExcelResult.TabIndex = 2;
            this.groupBoxExcelResult.TabStop = false;
            this.groupBoxExcelResult.Text = "Excel";
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.ColumnCount = 1;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel12.Controls.Add(this.tableLayoutPanel13, 0, 0);
            this.tableLayoutPanel12.Controls.Add(this.tableLayoutPanel14, 0, 1);
            this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel12.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 2;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.60241F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.39759F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(334, 65);
            this.tableLayoutPanel12.TabIndex = 0;
            // 
            // tableLayoutPanel13
            // 
            this.tableLayoutPanel13.ColumnCount = 2;
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.46342F));
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 83.53658F));
            this.tableLayoutPanel13.Controls.Add(this.labelNameExcel, 0, 0);
            this.tableLayoutPanel13.Controls.Add(this.textBoxNameExcel, 1, 0);
            this.tableLayoutPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel13.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel13.Name = "tableLayoutPanel13";
            this.tableLayoutPanel13.RowCount = 1;
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel13.Size = new System.Drawing.Size(328, 26);
            this.tableLayoutPanel13.TabIndex = 0;
            // 
            // labelNameExcel
            // 
            this.labelNameExcel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelNameExcel.AutoSize = true;
            this.labelNameExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelNameExcel.Location = new System.Drawing.Point(3, 5);
            this.labelNameExcel.Name = "labelNameExcel";
            this.labelNameExcel.Size = new System.Drawing.Size(44, 15);
            this.labelNameExcel.TabIndex = 0;
            this.labelNameExcel.Text = "Name:";
            // 
            // textBoxNameExcel
            // 
            this.textBoxNameExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxNameExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxNameExcel.Location = new System.Drawing.Point(57, 3);
            this.textBoxNameExcel.Name = "textBoxNameExcel";
            this.textBoxNameExcel.Size = new System.Drawing.Size(268, 21);
            this.textBoxNameExcel.TabIndex = 1;
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.ColumnCount = 3;
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.80952F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76.19048F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tableLayoutPanel14.Controls.Add(this.labelDirectoryExcel, 0, 0);
            this.tableLayoutPanel14.Controls.Add(this.buttonSelectDirectory, 2, 0);
            this.tableLayoutPanel14.Controls.Add(this.textBoxDirectoryExcel, 1, 0);
            this.tableLayoutPanel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel14.Location = new System.Drawing.Point(0, 32);
            this.tableLayoutPanel14.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.RowCount = 1;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.Size = new System.Drawing.Size(334, 33);
            this.tableLayoutPanel14.TabIndex = 1;
            // 
            // labelDirectoryExcel
            // 
            this.labelDirectoryExcel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelDirectoryExcel.AutoSize = true;
            this.labelDirectoryExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelDirectoryExcel.Location = new System.Drawing.Point(3, 9);
            this.labelDirectoryExcel.Name = "labelDirectoryExcel";
            this.labelDirectoryExcel.Size = new System.Drawing.Size(61, 15);
            this.labelDirectoryExcel.TabIndex = 0;
            this.labelDirectoryExcel.Text = "Directory: ";
            // 
            // buttonSelectDirectory
            // 
            this.buttonSelectDirectory.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonSelectDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonSelectDirectory.Image = global::ResponseAnalyzer.Properties.Resources.add;
            this.buttonSelectDirectory.Location = new System.Drawing.Point(289, 5);
            this.buttonSelectDirectory.Name = "buttonSelectDirectory";
            this.buttonSelectDirectory.Size = new System.Drawing.Size(32, 22);
            this.buttonSelectDirectory.TabIndex = 1;
            this.buttonSelectDirectory.UseVisualStyleBackColor = true;
            this.buttonSelectDirectory.Click += new System.EventHandler(this.buttonSelectDirectory_Click);
            // 
            // textBoxDirectoryExcel
            // 
            this.textBoxDirectoryExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDirectoryExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxDirectoryExcel.Location = new System.Drawing.Point(71, 6);
            this.textBoxDirectoryExcel.Name = "textBoxDirectoryExcel";
            this.textBoxDirectoryExcel.ReadOnly = true;
            this.textBoxDirectoryExcel.Size = new System.Drawing.Size(212, 21);
            this.textBoxDirectoryExcel.TabIndex = 2;
            // 
            // groupBoxProject
            // 
            this.groupBoxProject.AutoSize = true;
            this.groupBoxProject.BackColor = System.Drawing.Color.White;
            this.groupBoxProject.Controls.Add(this.layoutProjectPath);
            this.groupBoxProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBoxProject.Location = new System.Drawing.Point(3, 0);
            this.groupBoxProject.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.groupBoxProject.MaximumSize = new System.Drawing.Size(0, 60);
            this.groupBoxProject.MinimumSize = new System.Drawing.Size(0, 50);
            this.groupBoxProject.Name = "groupBoxProject";
            this.groupBoxProject.Size = new System.Drawing.Size(360, 50);
            this.groupBoxProject.TabIndex = 9;
            this.groupBoxProject.TabStop = false;
            this.groupBoxProject.Text = "Project";
            // 
            // layoutProjectPath
            // 
            this.layoutProjectPath.ColumnCount = 4;
            this.layoutProjectPath.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.95745F));
            this.layoutProjectPath.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84.04256F));
            this.layoutProjectPath.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layoutProjectPath.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.layoutProjectPath.Controls.Add(this.labelProjectPath, 0, 0);
            this.layoutProjectPath.Controls.Add(this.buttonOpenProject, 2, 0);
            this.layoutProjectPath.Controls.Add(this.buttonUpdateProject, 3, 0);
            this.layoutProjectPath.Controls.Add(this.textBoxProjectPath, 1, 0);
            this.layoutProjectPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutProjectPath.Location = new System.Drawing.Point(3, 17);
            this.layoutProjectPath.MinimumSize = new System.Drawing.Size(0, 28);
            this.layoutProjectPath.Name = "layoutProjectPath";
            this.layoutProjectPath.RowCount = 1;
            this.layoutProjectPath.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutProjectPath.Size = new System.Drawing.Size(354, 30);
            this.layoutProjectPath.TabIndex = 1;
            // 
            // labelProjectPath
            // 
            this.labelProjectPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelProjectPath.AutoSize = true;
            this.labelProjectPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelProjectPath.Location = new System.Drawing.Point(3, 7);
            this.labelProjectPath.Name = "labelProjectPath";
            this.labelProjectPath.Size = new System.Drawing.Size(35, 15);
            this.labelProjectPath.TabIndex = 0;
            this.labelProjectPath.Text = "Path:";
            // 
            // buttonOpenProject
            // 
            this.buttonOpenProject.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonOpenProject.Image = global::ResponseAnalyzer.Properties.Resources.add;
            this.buttonOpenProject.Location = new System.Drawing.Point(277, 4);
            this.buttonOpenProject.Name = "buttonOpenProject";
            this.buttonOpenProject.Size = new System.Drawing.Size(28, 22);
            this.buttonOpenProject.TabIndex = 2;
            this.buttonOpenProject.UseVisualStyleBackColor = true;
            this.buttonOpenProject.Click += new System.EventHandler(this.buttonOpenProject_Click);
            // 
            // buttonUpdateProject
            // 
            this.buttonUpdateProject.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonUpdateProject.Image = global::ResponseAnalyzer.Properties.Resources.refresh;
            this.buttonUpdateProject.Location = new System.Drawing.Point(311, 4);
            this.buttonUpdateProject.Name = "buttonUpdateProject";
            this.buttonUpdateProject.Size = new System.Drawing.Size(30, 22);
            this.buttonUpdateProject.TabIndex = 3;
            this.buttonUpdateProject.UseVisualStyleBackColor = true;
            // 
            // textBoxProjectPath
            // 
            this.textBoxProjectPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProjectPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxProjectPath.Location = new System.Drawing.Point(46, 4);
            this.textBoxProjectPath.Name = "textBoxProjectPath";
            this.textBoxProjectPath.ReadOnly = true;
            this.textBoxProjectPath.Size = new System.Drawing.Size(225, 21);
            this.textBoxProjectPath.TabIndex = 1;
            // 
            // glWindow
            // 
            this.glWindow.AutoSize = true;
            this.glWindow.BackColor = System.Drawing.Color.White;
            this.glWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glWindow.Location = new System.Drawing.Point(380, 8);
            this.glWindow.Margin = new System.Windows.Forms.Padding(8);
            this.glWindow.Name = "glWindow";
            this.glWindow.Size = new System.Drawing.Size(620, 691);
            this.glWindow.TabIndex = 12;
            this.glWindow.VSync = false;
            this.glWindow.Load += new System.EventHandler(this.glWindow_Load);
            this.glWindow.Paint += new System.Windows.Forms.PaintEventHandler(this.glWindow_Paint);
            this.glWindow.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glWindow_KeyDown);
            this.glWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glWindow_MouseDown);
            this.glWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glWindow_MouseMove);
            this.glWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glWindow_MouseUp);
            this.glWindow.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glWindow_MouseWheel);
            this.glWindow.Resize += new System.EventHandler(this.glWindow_Resize);
            // 
            // glContextMenu
            // 
            this.glContextMenu.ImageScalingSize = new System.Drawing.Size(15, 0);
            this.glContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stripPolygonMode,
            this.stripView,
            this.stripComponentVisualisation,
            this.stripNodeNames});
            this.glContextMenu.Name = "glMenu";
            this.glContextMenu.Size = new System.Drawing.Size(207, 92);
            // 
            // stripPolygonMode
            // 
            this.stripPolygonMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.glPolygonModeLine,
            this.glPolygonModeFill});
            this.stripPolygonMode.Name = "stripPolygonMode";
            this.stripPolygonMode.Size = new System.Drawing.Size(206, 22);
            this.stripPolygonMode.Text = "Polygon mode";
            this.stripPolygonMode.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.stripMode_DropDownItemClicked);
            // 
            // glPolygonModeLine
            // 
            this.glPolygonModeLine.Name = "glPolygonModeLine";
            this.glPolygonModeLine.Size = new System.Drawing.Size(96, 22);
            this.glPolygonModeLine.Text = "Line";
            // 
            // glPolygonModeFill
            // 
            this.glPolygonModeFill.Name = "glPolygonModeFill";
            this.glPolygonModeFill.Size = new System.Drawing.Size(96, 22);
            this.glPolygonModeFill.Text = "Fill";
            // 
            // stripView
            // 
            this.stripView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.glViewFront,
            this.glViewBack,
            this.glViewUp,
            this.glViewDown,
            this.glViewLeft,
            this.glViewRight,
            this.glViewIsometric});
            this.stripView.Name = "stripView";
            this.stripView.Size = new System.Drawing.Size(206, 22);
            this.stripView.Text = "View";
            this.stripView.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.stripView_DropDownItemClicked);
            // 
            // glViewFront
            // 
            this.glViewFront.Name = "glViewFront";
            this.glViewFront.Size = new System.Drawing.Size(129, 22);
            this.glViewFront.Text = "Front";
            // 
            // glViewBack
            // 
            this.glViewBack.Name = "glViewBack";
            this.glViewBack.Size = new System.Drawing.Size(129, 22);
            this.glViewBack.Text = "Back";
            // 
            // glViewUp
            // 
            this.glViewUp.Name = "glViewUp";
            this.glViewUp.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.glViewUp.Size = new System.Drawing.Size(129, 22);
            this.glViewUp.Text = "Up";
            // 
            // glViewDown
            // 
            this.glViewDown.Name = "glViewDown";
            this.glViewDown.Size = new System.Drawing.Size(129, 22);
            this.glViewDown.Text = "Down";
            // 
            // glViewLeft
            // 
            this.glViewLeft.Name = "glViewLeft";
            this.glViewLeft.Size = new System.Drawing.Size(129, 22);
            this.glViewLeft.Text = "Left";
            // 
            // glViewRight
            // 
            this.glViewRight.Name = "glViewRight";
            this.glViewRight.Size = new System.Drawing.Size(129, 22);
            this.glViewRight.Text = "Right";
            // 
            // glViewIsometric
            // 
            this.glViewIsometric.Name = "glViewIsometric";
            this.glViewIsometric.Size = new System.Drawing.Size(129, 22);
            this.glViewIsometric.Text = "Isometric";
            // 
            // stripComponentVisualisation
            // 
            this.stripComponentVisualisation.Name = "stripComponentVisualisation";
            this.stripComponentVisualisation.Size = new System.Drawing.Size(206, 22);
            this.stripComponentVisualisation.Text = "Component visualisation";
            // 
            // stripNodeNames
            // 
            this.stripNodeNames.CheckOnClick = true;
            this.stripNodeNames.Name = "stripNodeNames";
            this.stripNodeNames.Size = new System.Drawing.Size(206, 22);
            this.stripNodeNames.Text = "Node names";
            this.stripNodeNames.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.stripNodeNames.CheckedChanged += new System.EventHandler(this.stripNodeNames_CheckedChanged);
            // 
            // ResponseAnalyzer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "ResponseAnalyzer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ResponseAnalyzer";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ResponseAnalyzer_KeyDown);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.layoutControlPanel.ResumeLayout(false);
            this.layoutControlPanel.PerformLayout();
            this.tabStages.ResumeLayout(false);
            this.tabTemplate.ResumeLayout(false);
            this.groupBoxExcelTemplate.ResumeLayout(false);
            this.layoutExcelTemplate.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.layoutTemplateObjects.ResumeLayout(false);
            this.layoutTemplateConfiguration.ResumeLayout(false);
            this.layoutExcelTemplateControls.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericTemplateNormalization)).EndInit();
            this.layoutLoadTemplate.ResumeLayout(false);
            this.tabProcess.ResumeLayout(false);
            this.layoutProcess.ResumeLayout(false);
            this.groupBoxChannelSelection.ResumeLayout(false);
            this.layoutChannelSelection.ResumeLayout(false);
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            this.layoutTestlabSelection.ResumeLayout(false);
            this.layoutTestlabSelection.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.groupBoxExcelResult.ResumeLayout(false);
            this.tableLayoutPanel12.ResumeLayout(false);
            this.tableLayoutPanel13.ResumeLayout(false);
            this.tableLayoutPanel13.PerformLayout();
            this.tableLayoutPanel14.ResumeLayout(false);
            this.tableLayoutPanel14.PerformLayout();
            this.groupBoxProject.ResumeLayout(false);
            this.layoutProjectPath.ResumeLayout(false);
            this.layoutProjectPath.PerformLayout();
            this.glContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel layoutControlPanel;
        private System.Windows.Forms.GroupBox groupBoxProject;
        private System.Windows.Forms.TableLayoutPanel layoutProjectPath;
        private System.Windows.Forms.Label labelProjectPath;
        private System.Windows.Forms.TextBox textBoxProjectPath;
        private System.Windows.Forms.Button buttonOpenProject;
        private System.Windows.Forms.Button buttonUpdateProject;
        private System.Windows.Forms.TabControl tabStages;
        private System.Windows.Forms.TabPage tabTemplate;
        private System.Windows.Forms.GroupBox groupBoxExcelTemplate;
        private System.Windows.Forms.TabPage tabMeasure;
        private OpenTK.GLControl glWindow;
        private System.Windows.Forms.ContextMenuStrip glContextMenu;
        private System.Windows.Forms.ToolStripMenuItem stripPolygonMode;
        private System.Windows.Forms.ToolStripMenuItem glPolygonModeLine;
        private System.Windows.Forms.ToolStripMenuItem glPolygonModeFill;
        private System.Windows.Forms.ToolStripMenuItem stripView;
        private System.Windows.Forms.ToolStripMenuItem glViewFront;
        private System.Windows.Forms.ToolStripMenuItem glViewBack;
        private System.Windows.Forms.ToolStripMenuItem glViewUp;
        private System.Windows.Forms.ToolStripMenuItem glViewDown;
        private System.Windows.Forms.ToolStripMenuItem glViewLeft;
        private System.Windows.Forms.ToolStripMenuItem glViewRight;
        private System.Windows.Forms.ToolStripMenuItem glViewIsometric;
        private System.Windows.Forms.ToolStripStatusLabel statusStripLabel;
        private System.Windows.Forms.TabPage tabProcess;
        private System.Windows.Forms.TableLayoutPanel layoutProcess;
        private System.Windows.Forms.GroupBox groupBoxChannelSelection;
        private System.Windows.Forms.TableLayoutPanel layoutChannelSelection;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxResonanceFrequency;
        private System.Windows.Forms.Button buttonSelectResonanceFrequency;
        private System.Windows.Forms.TableLayoutPanel layoutTestlabSelection;
        private System.Windows.Forms.Button buttonSelectTestLab;
        private System.Windows.Forms.Label labelSelectionInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.ListBox listBoxFoundSignals;
        private System.Windows.Forms.ListBox listBoxFrequencies;
        private System.Windows.Forms.Button buttonProcess;
        private System.Windows.Forms.GroupBox groupBoxExcelResult;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel12;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel13;
        private System.Windows.Forms.Label labelNameExcel;
        private System.Windows.Forms.TextBox textBoxNameExcel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel14;
        private System.Windows.Forms.Label labelDirectoryExcel;
        private System.Windows.Forms.Button buttonSelectDirectory;
        private System.Windows.Forms.TextBox textBoxDirectoryExcel;
        private System.Windows.Forms.TableLayoutPanel layoutExcelTemplate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.Label labelExcelPath;
        private System.Windows.Forms.TextBox textBoxExcelTemplatePath;
        private System.Windows.Forms.Button buttonOpenExcelTemplate;
        private System.Windows.Forms.TableLayoutPanel layoutTemplateObjects;
        private System.Windows.Forms.ListBox listBoxTemplateCharts;
        private System.Windows.Forms.TableLayoutPanel layoutTemplateConfiguration;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.Label labelTemplateNormalization;
        private System.Windows.Forms.NumericUpDown numericTemplateNormalization;
        private System.Windows.Forms.Label labelTemplateAxis;
        private System.Windows.Forms.ComboBox comboBoxTemplateAxis;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxTemplateType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxTemplateDirection;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxTemplateUnits;
        private System.Windows.Forms.FlowLayoutPanel layoutExcelTemplateControls;
        private System.Windows.Forms.Button buttonAddTemplateObject;
        private System.Windows.Forms.Button buttonEditTemplateSelection;
        private System.Windows.Forms.Button buttonRemoveTemplateObject;
        private System.Windows.Forms.TreeView treeTemplateObjects;
        private System.Windows.Forms.Button buttonCopyTemplateObjects;
        private System.Windows.Forms.ToolStripMenuItem stripNodeNames;
        private System.Windows.Forms.ToolStripMenuItem stripComponentVisualisation;
        private System.Windows.Forms.CheckBox checkBoxSwapAxes;
        private System.Windows.Forms.FlowLayoutPanel layoutLoadTemplate;
        private System.Windows.Forms.Button buttonOpenTemplateSettings;
        private System.Windows.Forms.Button buttonSaveTemplateSettings;
    }
}

