
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
        private int[] lastMousePosition_;
        private bool isTranslation_ = false;
        private bool isRotation_ = false;
        private int indLine_ = 0;
        private const char selectionDelimiter_ = ':';
        private bool isEditSelection = false;
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
            this.statusStripLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tabStages = new System.Windows.Forms.TabControl();
            this.tabTemplate = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.comboBoxSelection = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBoxSelection = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.treeSelection = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAddSelection = new System.Windows.Forms.Button();
            this.buttonRemoveSelection = new System.Windows.Forms.Button();
            this.buttonEditSelection = new System.Windows.Forms.Button();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonSaveTemplate = new System.Windows.Forms.Button();
            this.buttonOpenTemplate = new System.Windows.Forms.Button();
            this.tabMeasure = new System.Windows.Forms.TabPage();
            this.tabProcess = new System.Windows.Forms.TabPage();
            this.groupBoxProject = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
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
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tabStages.SuspendLayout();
            this.tabTemplate.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.groupBoxSelection.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.groupBoxProject.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.glContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.statusStrip.Location = new System.Drawing.Point(0, 496);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(831, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusStripLabel
            // 
            this.statusStripLabel.AutoSize = true;
            this.statusStripLabel.Location = new System.Drawing.Point(3, 711);
            this.statusStripLabel.Name = "statusStripLabel";
            this.statusStripLabel.Size = new System.Drawing.Size(0, 15);
            this.statusStripLabel.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.77381F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.22619F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.glWindow, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(831, 496);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.tabStages, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.groupBoxProject, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.65574F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.34426F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(332, 490);
            this.tableLayoutPanel5.TabIndex = 11;
            // 
            // tabStages
            // 
            this.tabStages.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabStages.Controls.Add(this.tabTemplate);
            this.tabStages.Controls.Add(this.tabMeasure);
            this.tabStages.Controls.Add(this.tabProcess);
            this.tabStages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabStages.Location = new System.Drawing.Point(3, 55);
            this.tabStages.Name = "tabStages";
            this.tabStages.SelectedIndex = 0;
            this.tabStages.Size = new System.Drawing.Size(326, 432);
            this.tabStages.TabIndex = 10;
            // 
            // tabTemplate
            // 
            this.tabTemplate.BackColor = System.Drawing.Color.White;
            this.tabTemplate.Controls.Add(this.groupBox1);
            this.tabTemplate.Controls.Add(this.groupBoxSelection);
            this.tabTemplate.Controls.Add(this.flowLayoutPanel2);
            this.tabTemplate.Location = new System.Drawing.Point(4, 4);
            this.tabTemplate.Name = "tabTemplate";
            this.tabTemplate.Padding = new System.Windows.Forms.Padding(3);
            this.tabTemplate.Size = new System.Drawing.Size(318, 404);
            this.tabTemplate.TabIndex = 0;
            this.tabTemplate.Text = "Template";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel7);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(3, 163);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(312, 209);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Excel";
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 1;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel8, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel10, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel9, 0, 2);
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 3;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 77.12418F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.87582F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(306, 188);
            this.tableLayoutPanel7.TabIndex = 1;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 3;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.18182F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76.81818F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tableLayoutPanel8.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.textBox1, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.button2, 2, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(300, 32);
            this.tableLayoutPanel8.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(3, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Name:";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(61, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(188, 21);
            this.textBox1.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2.Image = global::ResponseAnalyzer.Properties.Resources.add;
            this.button2.Location = new System.Drawing.Point(255, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(34, 23);
            this.button2.TabIndex = 2;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 2;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Controls.Add(this.listBox1, 0, 0);
            this.tableLayoutPanel10.Controls.Add(this.listBox2, 1, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(0, 38);
            this.tableLayoutPanel10.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 1;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(306, 115);
            this.tableLayoutPanel10.TabIndex = 5;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(3, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(147, 109);
            this.listBox1.TabIndex = 0;
            // 
            // listBox2
            // 
            this.listBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 15;
            this.listBox2.Location = new System.Drawing.Point(156, 3);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(147, 109);
            this.listBox2.TabIndex = 1;
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel9.ColumnCount = 2;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.71504F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.28496F));
            this.tableLayoutPanel9.Controls.Add(this.comboBoxSelection, 1, 0);
            this.tableLayoutPanel9.Controls.Add(this.flowLayoutPanel3, 0, 0);
            this.tableLayoutPanel9.Location = new System.Drawing.Point(0, 156);
            this.tableLayoutPanel9.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 1;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(306, 29);
            this.tableLayoutPanel9.TabIndex = 3;
            // 
            // comboBoxSelection
            // 
            this.comboBoxSelection.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboBoxSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelection.Enabled = false;
            this.comboBoxSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxSelection.FormattingEnabled = true;
            this.comboBoxSelection.Items.AddRange(new object[] {
            "Modeshape",
            "Force",
            "Real part of FRF",
            "Imaginary part of FRF"});
            this.comboBoxSelection.Location = new System.Drawing.Point(158, 3);
            this.comboBoxSelection.Margin = new System.Windows.Forms.Padding(0);
            this.comboBoxSelection.Name = "comboBoxSelection";
            this.comboBoxSelection.Size = new System.Drawing.Size(142, 23);
            this.comboBoxSelection.TabIndex = 1;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.button3);
            this.flowLayoutPanel3.Controls.Add(this.button1);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(158, 29);
            this.flowLayoutPanel3.TabIndex = 2;
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button3.Image = global::ResponseAnalyzer.Properties.Resources.rightArrow;
            this.button3.Location = new System.Drawing.Point(3, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(35, 23);
            this.button3.TabIndex = 1;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.button1.Enabled = false;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Image = global::ResponseAnalyzer.Properties.Resources.leftArrow;
            this.button1.Location = new System.Drawing.Point(44, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(33, 23);
            this.button1.TabIndex = 2;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBoxSelection
            // 
            this.groupBoxSelection.Controls.Add(this.tableLayoutPanel2);
            this.groupBoxSelection.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBoxSelection.Location = new System.Drawing.Point(3, 3);
            this.groupBoxSelection.Name = "groupBoxSelection";
            this.groupBoxSelection.Size = new System.Drawing.Size(312, 160);
            this.groupBoxSelection.TabIndex = 0;
            this.groupBoxSelection.TabStop = false;
            this.groupBoxSelection.Text = "Selection";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.treeSelection, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(306, 140);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // treeSelection
            // 
            this.treeSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeSelection.Location = new System.Drawing.Point(3, 3);
            this.treeSelection.Name = "treeSelection";
            this.treeSelection.Size = new System.Drawing.Size(300, 101);
            this.treeSelection.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.07423F));
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 110);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(300, 27);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.buttonAddSelection);
            this.flowLayoutPanel1.Controls.Add(this.buttonRemoveSelection);
            this.flowLayoutPanel1.Controls.Add(this.buttonEditSelection);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(300, 27);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // buttonAddSelection
            // 
            this.buttonAddSelection.Enabled = false;
            this.buttonAddSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonAddSelection.Image = global::ResponseAnalyzer.Properties.Resources.plus;
            this.buttonAddSelection.Location = new System.Drawing.Point(3, 3);
            this.buttonAddSelection.Name = "buttonAddSelection";
            this.buttonAddSelection.Size = new System.Drawing.Size(32, 22);
            this.buttonAddSelection.TabIndex = 0;
            this.buttonAddSelection.UseVisualStyleBackColor = true;
            this.buttonAddSelection.Click += new System.EventHandler(this.buttonAddSelection_Click);
            // 
            // buttonRemoveSelection
            // 
            this.buttonRemoveSelection.Enabled = false;
            this.buttonRemoveSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonRemoveSelection.Image = global::ResponseAnalyzer.Properties.Resources.minus;
            this.buttonRemoveSelection.Location = new System.Drawing.Point(41, 3);
            this.buttonRemoveSelection.Name = "buttonRemoveSelection";
            this.buttonRemoveSelection.Size = new System.Drawing.Size(32, 23);
            this.buttonRemoveSelection.TabIndex = 1;
            this.buttonRemoveSelection.UseVisualStyleBackColor = true;
            this.buttonRemoveSelection.Click += new System.EventHandler(this.buttonRemoveSelection_Click);
            // 
            // buttonEditSelection
            // 
            this.buttonEditSelection.Enabled = false;
            this.buttonEditSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonEditSelection.Image = global::ResponseAnalyzer.Properties.Resources.edit;
            this.buttonEditSelection.Location = new System.Drawing.Point(79, 3);
            this.buttonEditSelection.Name = "buttonEditSelection";
            this.buttonEditSelection.Size = new System.Drawing.Size(32, 23);
            this.buttonEditSelection.TabIndex = 2;
            this.buttonEditSelection.UseVisualStyleBackColor = true;
            this.buttonEditSelection.Click += new System.EventHandler(this.buttonEditSelection_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.buttonSaveTemplate);
            this.flowLayoutPanel2.Controls.Add(this.buttonOpenTemplate);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 373);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(312, 28);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // buttonSaveTemplate
            // 
            this.buttonSaveTemplate.Enabled = false;
            this.buttonSaveTemplate.Location = new System.Drawing.Point(234, 3);
            this.buttonSaveTemplate.Name = "buttonSaveTemplate";
            this.buttonSaveTemplate.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveTemplate.TabIndex = 1;
            this.buttonSaveTemplate.Text = "&Save";
            this.buttonSaveTemplate.UseVisualStyleBackColor = true;
            // 
            // buttonOpenTemplate
            // 
            this.buttonOpenTemplate.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonOpenTemplate.Enabled = false;
            this.buttonOpenTemplate.Location = new System.Drawing.Point(153, 3);
            this.buttonOpenTemplate.Name = "buttonOpenTemplate";
            this.buttonOpenTemplate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.buttonOpenTemplate.Size = new System.Drawing.Size(75, 23);
            this.buttonOpenTemplate.TabIndex = 0;
            this.buttonOpenTemplate.Text = "&Open";
            this.buttonOpenTemplate.UseVisualStyleBackColor = true;
            // 
            // tabMeasure
            // 
            this.tabMeasure.Location = new System.Drawing.Point(4, 4);
            this.tabMeasure.Name = "tabMeasure";
            this.tabMeasure.Padding = new System.Windows.Forms.Padding(3);
            this.tabMeasure.Size = new System.Drawing.Size(318, 404);
            this.tabMeasure.TabIndex = 1;
            this.tabMeasure.Text = "Measure";
            this.tabMeasure.UseVisualStyleBackColor = true;
            // 
            // tabProcess
            // 
            this.tabProcess.Location = new System.Drawing.Point(4, 4);
            this.tabProcess.Name = "tabProcess";
            this.tabProcess.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcess.Size = new System.Drawing.Size(318, 404);
            this.tabProcess.TabIndex = 2;
            this.tabProcess.Text = "Process";
            this.tabProcess.UseVisualStyleBackColor = true;
            // 
            // groupBoxProject
            // 
            this.groupBoxProject.BackColor = System.Drawing.Color.White;
            this.groupBoxProject.Controls.Add(this.tableLayoutPanel6);
            this.groupBoxProject.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxProject.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBoxProject.Location = new System.Drawing.Point(3, 0);
            this.groupBoxProject.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.groupBoxProject.Name = "groupBoxProject";
            this.groupBoxProject.Size = new System.Drawing.Size(326, 49);
            this.groupBoxProject.TabIndex = 9;
            this.groupBoxProject.TabStop = false;
            this.groupBoxProject.Text = "Project";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 4;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.2766F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78.7234F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel6.Controls.Add(this.labelProjectPath, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.buttonOpenProject, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.buttonUpdateProject, 3, 0);
            this.tableLayoutPanel6.Controls.Add(this.textBoxProjectPath, 1, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel6.MinimumSize = new System.Drawing.Size(0, 28);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(320, 28);
            this.tableLayoutPanel6.TabIndex = 1;
            // 
            // labelProjectPath
            // 
            this.labelProjectPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelProjectPath.AutoSize = true;
            this.labelProjectPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelProjectPath.Location = new System.Drawing.Point(3, 6);
            this.labelProjectPath.Name = "labelProjectPath";
            this.labelProjectPath.Size = new System.Drawing.Size(35, 15);
            this.labelProjectPath.TabIndex = 0;
            this.labelProjectPath.Text = "Path:";
            // 
            // buttonOpenProject
            // 
            this.buttonOpenProject.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonOpenProject.Image = global::ResponseAnalyzer.Properties.Resources.add;
            this.buttonOpenProject.Location = new System.Drawing.Point(249, 3);
            this.buttonOpenProject.Name = "buttonOpenProject";
            this.buttonOpenProject.Size = new System.Drawing.Size(29, 22);
            this.buttonOpenProject.TabIndex = 2;
            this.buttonOpenProject.UseVisualStyleBackColor = true;
            this.buttonOpenProject.Click += new System.EventHandler(this.buttonOpenProject_Click);
            // 
            // buttonUpdateProject
            // 
            this.buttonUpdateProject.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonUpdateProject.Image = global::ResponseAnalyzer.Properties.Resources.refresh;
            this.buttonUpdateProject.Location = new System.Drawing.Point(284, 3);
            this.buttonUpdateProject.Name = "buttonUpdateProject";
            this.buttonUpdateProject.Size = new System.Drawing.Size(30, 22);
            this.buttonUpdateProject.TabIndex = 3;
            this.buttonUpdateProject.UseVisualStyleBackColor = true;
            // 
            // textBoxProjectPath
            // 
            this.textBoxProjectPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProjectPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxProjectPath.Location = new System.Drawing.Point(55, 3);
            this.textBoxProjectPath.Name = "textBoxProjectPath";
            this.textBoxProjectPath.ReadOnly = true;
            this.textBoxProjectPath.Size = new System.Drawing.Size(188, 21);
            this.textBoxProjectPath.TabIndex = 1;
            // 
            // glWindow
            // 
            this.glWindow.BackColor = System.Drawing.Color.White;
            this.glWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glWindow.Location = new System.Drawing.Point(346, 8);
            this.glWindow.Margin = new System.Windows.Forms.Padding(8);
            this.glWindow.Name = "glWindow";
            this.glWindow.Size = new System.Drawing.Size(477, 480);
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
            this.glContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stripPolygonMode,
            this.stripView});
            this.glContextMenu.Name = "glMenu";
            this.glContextMenu.Size = new System.Drawing.Size(181, 70);
            // 
            // stripPolygonMode
            // 
            this.stripPolygonMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.glPolygonModeLine,
            this.glPolygonModeFill});
            this.stripPolygonMode.Name = "stripPolygonMode";
            this.stripPolygonMode.Size = new System.Drawing.Size(180, 22);
            this.stripPolygonMode.Text = "Polygon mode";
            this.stripPolygonMode.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.stripMode_DropDownItemClicked);
            // 
            // glPolygonModeLine
            // 
            this.glPolygonModeLine.Name = "glPolygonModeLine";
            this.glPolygonModeLine.Size = new System.Drawing.Size(180, 22);
            this.glPolygonModeLine.Text = "Line";
            // 
            // glPolygonModeFill
            // 
            this.glPolygonModeFill.Name = "glPolygonModeFill";
            this.glPolygonModeFill.Size = new System.Drawing.Size(180, 22);
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
            this.stripView.Size = new System.Drawing.Size(180, 22);
            this.stripView.Text = "View";
            this.stripView.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.stripView_DropDownItemClicked);
            // 
            // glViewFront
            // 
            this.glViewFront.Name = "glViewFront";
            this.glViewFront.Size = new System.Drawing.Size(180, 22);
            this.glViewFront.Text = "Front";
            // 
            // glViewBack
            // 
            this.glViewBack.Name = "glViewBack";
            this.glViewBack.Size = new System.Drawing.Size(180, 22);
            this.glViewBack.Text = "Back";
            // 
            // glViewUp
            // 
            this.glViewUp.Name = "glViewUp";
            this.glViewUp.Size = new System.Drawing.Size(180, 22);
            this.glViewUp.Text = "Up";
            // 
            // glViewDown
            // 
            this.glViewDown.Name = "glViewDown";
            this.glViewDown.Size = new System.Drawing.Size(180, 22);
            this.glViewDown.Text = "Down";
            // 
            // glViewLeft
            // 
            this.glViewLeft.Name = "glViewLeft";
            this.glViewLeft.Size = new System.Drawing.Size(180, 22);
            this.glViewLeft.Text = "Left";
            // 
            // glViewRight
            // 
            this.glViewRight.Name = "glViewRight";
            this.glViewRight.Size = new System.Drawing.Size(180, 22);
            this.glViewRight.Text = "Right";
            // 
            // glViewIsometric
            // 
            this.glViewIsometric.Name = "glViewIsometric";
            this.glViewIsometric.Size = new System.Drawing.Size(180, 22);
            this.glViewIsometric.Text = "Isometric";
            // 
            // ResponseAnalyzer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(831, 518);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStripLabel);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ResponseAnalyzer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ResponseAnalyzer";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tabStages.ResumeLayout(false);
            this.tabTemplate.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel9.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.groupBoxSelection.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.groupBoxProject.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.glContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Label statusStripLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.GroupBox groupBoxProject;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label labelProjectPath;
        private System.Windows.Forms.TextBox textBoxProjectPath;
        private System.Windows.Forms.Button buttonOpenProject;
        private System.Windows.Forms.Button buttonUpdateProject;
        private System.Windows.Forms.TabControl tabStages;
        private System.Windows.Forms.TabPage tabTemplate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private System.Windows.Forms.ComboBox comboBoxSelection;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.GroupBox groupBoxSelection;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TreeView treeSelection;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button buttonSaveTemplate;
        private System.Windows.Forms.Button buttonOpenTemplate;
        private System.Windows.Forms.TabPage tabMeasure;
        private System.Windows.Forms.TabPage tabProcess;
        private OpenTK.GLControl glWindow;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonAddSelection;
        private System.Windows.Forms.Button buttonRemoveSelection;
        private System.Windows.Forms.Button buttonEditSelection;
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
    }
}

