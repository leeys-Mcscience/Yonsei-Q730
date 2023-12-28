
namespace DataViewer
{
    partial class Form_Main
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.radioButton_Sheet = new System.Windows.Forms.RadioButton();
            this.radioButton_Graph = new System.Windows.Forms.RadioButton();
            this.button_MassGraph = new System.Windows.Forms.Button();
            this.button_AreaGraph = new System.Windows.Forms.Button();
            this.textBox_Mass = new System.Windows.Forms.TextBox();
            this.textBox_Area = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.button_FileLoad = new System.Windows.Forms.Button();
            this.button_Clear = new System.Windows.Forms.Button();
            this.button_Config = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menu_Select = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Charge = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Discharge = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Rest = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menu_Cycle = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Loop = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Jump = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_AnodeCharge = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_AnodeDischarge = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_SelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Unselect = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Info = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.button_PresetSave = new System.Windows.Forms.Button();
            this.button_PresetLoad = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Preset = new System.Windows.Forms.TextBox();
            this.button_Apply = new System.Windows.Forms.Button();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menu_Fra = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Tra = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Acr = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Dcr = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menu_Pattern = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Ocv = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menu_DeleteFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Clear = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Export = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(987, 656);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(1, 61);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(1);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel7);
            this.splitContainer1.Panel2MinSize = 230;
            this.splitContainer1.Size = new System.Drawing.Size(985, 594);
            this.splitContainer1.SplitterDistance = 751;
            this.splitContainer1.TabIndex = 2;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(751, 594);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 7;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.Controls.Add(this.radioButton_Sheet, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.radioButton_Graph, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_MassGraph, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_AreaGraph, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBox_Mass, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBox_Area, 6, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(751, 25);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // radioButton_Sheet
            // 
            this.radioButton_Sheet.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_Sheet.AutoSize = true;
            this.radioButton_Sheet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.radioButton_Sheet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButton_Sheet.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(176)))), ((int)(((byte)(176)))));
            this.radioButton_Sheet.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.radioButton_Sheet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton_Sheet.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_Sheet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(201)))), ((int)(((byte)(202)))));
            this.radioButton_Sheet.Location = new System.Drawing.Point(81, 2);
            this.radioButton_Sheet.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButton_Sheet.Name = "radioButton_Sheet";
            this.radioButton_Sheet.Size = new System.Drawing.Size(78, 21);
            this.radioButton_Sheet.TabIndex = 0;
            this.radioButton_Sheet.Text = "SHEET";
            this.radioButton_Sheet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_Sheet.UseVisualStyleBackColor = false;
            this.radioButton_Sheet.Click += new System.EventHandler(this.radioButtons_Click);
            // 
            // radioButton_Graph
            // 
            this.radioButton_Graph.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_Graph.AutoSize = true;
            this.radioButton_Graph.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.radioButton_Graph.Checked = true;
            this.radioButton_Graph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButton_Graph.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(176)))), ((int)(((byte)(176)))));
            this.radioButton_Graph.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.radioButton_Graph.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton_Graph.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_Graph.ForeColor = System.Drawing.Color.Black;
            this.radioButton_Graph.Location = new System.Drawing.Point(1, 2);
            this.radioButton_Graph.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButton_Graph.Name = "radioButton_Graph";
            this.radioButton_Graph.Size = new System.Drawing.Size(78, 21);
            this.radioButton_Graph.TabIndex = 1;
            this.radioButton_Graph.TabStop = true;
            this.radioButton_Graph.Text = "GRAPH";
            this.radioButton_Graph.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_Graph.UseVisualStyleBackColor = false;
            this.radioButton_Graph.Click += new System.EventHandler(this.radioButtons_Click);
            // 
            // button_MassGraph
            // 
            this.button_MassGraph.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button_MassGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_MassGraph.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_MassGraph.ForeColor = System.Drawing.Color.White;
            this.button_MassGraph.Location = new System.Drawing.Point(432, 1);
            this.button_MassGraph.Margin = new System.Windows.Forms.Padding(1);
            this.button_MassGraph.Name = "button_MassGraph";
            this.button_MassGraph.Size = new System.Drawing.Size(78, 23);
            this.button_MassGraph.TabIndex = 2;
            this.button_MassGraph.TabStop = false;
            this.button_MassGraph.Text = "Mass";
            this.button_MassGraph.UseVisualStyleBackColor = false;
            this.button_MassGraph.Click += new System.EventHandler(this.button_MassGraph_Click);
            // 
            // button_AreaGraph
            // 
            this.button_AreaGraph.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button_AreaGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_AreaGraph.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_AreaGraph.ForeColor = System.Drawing.Color.White;
            this.button_AreaGraph.Location = new System.Drawing.Point(592, 1);
            this.button_AreaGraph.Margin = new System.Windows.Forms.Padding(1);
            this.button_AreaGraph.Name = "button_AreaGraph";
            this.button_AreaGraph.Size = new System.Drawing.Size(78, 23);
            this.button_AreaGraph.TabIndex = 3;
            this.button_AreaGraph.TabStop = false;
            this.button_AreaGraph.Text = "Area";
            this.button_AreaGraph.UseVisualStyleBackColor = false;
            this.button_AreaGraph.Click += new System.EventHandler(this.button_AreaGraph_Click);
            // 
            // textBox_Mass
            // 
            this.textBox_Mass.Location = new System.Drawing.Point(512, 1);
            this.textBox_Mass.Margin = new System.Windows.Forms.Padding(1);
            this.textBox_Mass.Name = "textBox_Mass";
            this.textBox_Mass.Size = new System.Drawing.Size(78, 27);
            this.textBox_Mass.TabIndex = 4;
            // 
            // textBox_Area
            // 
            this.textBox_Area.Location = new System.Drawing.Point(672, 1);
            this.textBox_Area.Margin = new System.Windows.Forms.Padding(1);
            this.textBox_Area.Name = "textBox_Area";
            this.textBox_Area.Size = new System.Drawing.Size(78, 27);
            this.textBox_Area.TabIndex = 5;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 1;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.splitContainer2, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.pictureBox1, 0, 2);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 3;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(230, 594);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 5;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.button_FileLoad, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.button_Clear, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.button_Config, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.label4, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(230, 65);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // button_FileLoad
            // 
            this.button_FileLoad.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button_FileLoad.BackColor = System.Drawing.Color.Transparent;
            this.button_FileLoad.BackgroundImage = global::DataViewer.Properties.Resources.LOAD;
            this.button_FileLoad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_FileLoad.FlatAppearance.BorderSize = 0;
            this.button_FileLoad.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.button_FileLoad.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.button_FileLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_FileLoad.Location = new System.Drawing.Point(3, 0);
            this.button_FileLoad.Margin = new System.Windows.Forms.Padding(0);
            this.button_FileLoad.Name = "button_FileLoad";
            this.button_FileLoad.Size = new System.Drawing.Size(48, 47);
            this.button_FileLoad.TabIndex = 2;
            this.button_FileLoad.TabStop = false;
            this.button_FileLoad.UseVisualStyleBackColor = false;
            this.button_FileLoad.Click += new System.EventHandler(this.button_FileLoad_Click);
            // 
            // button_Clear
            // 
            this.button_Clear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button_Clear.BackColor = System.Drawing.Color.Transparent;
            this.button_Clear.BackgroundImage = global::DataViewer.Properties.Resources.CLEAR;
            this.button_Clear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Clear.FlatAppearance.BorderSize = 0;
            this.button_Clear.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.button_Clear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.button_Clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Clear.Location = new System.Drawing.Point(58, 0);
            this.button_Clear.Margin = new System.Windows.Forms.Padding(0);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(48, 47);
            this.button_Clear.TabIndex = 15;
            this.button_Clear.TabStop = false;
            this.button_Clear.UseVisualStyleBackColor = false;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // button_Config
            // 
            this.button_Config.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button_Config.BackColor = System.Drawing.Color.Transparent;
            this.button_Config.BackgroundImage = global::DataViewer.Properties.Resources.CONFIG;
            this.button_Config.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Config.FlatAppearance.BorderSize = 0;
            this.button_Config.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.button_Config.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.button_Config.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Config.Location = new System.Drawing.Point(168, 0);
            this.button_Config.Margin = new System.Windows.Forms.Padding(0);
            this.button_Config.Name = "button_Config";
            this.button_Config.Size = new System.Drawing.Size(48, 47);
            this.button_Config.TabIndex = 16;
            this.button_Config.TabStop = false;
            this.button_Config.UseVisualStyleBackColor = false;
            this.button_Config.Visible = false;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(114, 47);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 18);
            this.label4.TabIndex = 17;
            this.label4.Text = "Config";
            this.label4.Visible = false;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(64, 47);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 18);
            this.label2.TabIndex = 12;
            this.label2.Text = "Clear";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(9, 47);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 18);
            this.label3.TabIndex = 14;
            this.label3.Text = "Load";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 65);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanel5);
            this.splitContainer2.Size = new System.Drawing.Size(230, 479);
            this.splitContainer2.SplitterDistance = 207;
            this.splitContainer2.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeView1.FullRowSelect = true;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(1);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(230, 207);
            this.treeView1.TabIndex = 3;
            this.treeView1.TabStop = false;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_Select,
            this.menu_SelectAll,
            this.menu_Unselect,
            this.menu_Info});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(194, 100);
            // 
            // menu_Select
            // 
            this.menu_Select.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_Charge,
            this.menu_Discharge,
            this.menu_Rest,
            this.toolStripSeparator1,
            this.menu_Cycle,
            this.menu_Loop,
            this.menu_Jump,
            this.menu_AnodeCharge,
            this.menu_AnodeDischarge});
            this.menu_Select.Name = "menu_Select";
            this.menu_Select.Size = new System.Drawing.Size(193, 24);
            this.menu_Select.Text = "레시피 일괄 선택";
            // 
            // menu_Charge
            // 
            this.menu_Charge.Name = "menu_Charge";
            this.menu_Charge.Size = new System.Drawing.Size(209, 26);
            this.menu_Charge.Tag = "Charge";
            this.menu_Charge.Text = "Charge";
            this.menu_Charge.Click += new System.EventHandler(this.menu_MultiSelect_Click);
            // 
            // menu_Discharge
            // 
            this.menu_Discharge.Name = "menu_Discharge";
            this.menu_Discharge.Size = new System.Drawing.Size(209, 26);
            this.menu_Discharge.Tag = "Discharge";
            this.menu_Discharge.Text = "Discharge";
            this.menu_Discharge.Click += new System.EventHandler(this.menu_MultiSelect_Click);
            // 
            // menu_Rest
            // 
            this.menu_Rest.Name = "menu_Rest";
            this.menu_Rest.Size = new System.Drawing.Size(209, 26);
            this.menu_Rest.Tag = "Rest";
            this.menu_Rest.Text = "Rest";
            this.menu_Rest.Click += new System.EventHandler(this.menu_MultiSelect_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(206, 6);
            // 
            // menu_Cycle
            // 
            this.menu_Cycle.Name = "menu_Cycle";
            this.menu_Cycle.Size = new System.Drawing.Size(209, 26);
            this.menu_Cycle.Tag = "Cycle";
            this.menu_Cycle.Text = "Cycle";
            this.menu_Cycle.Click += new System.EventHandler(this.menu_MultiSelect_Click);
            // 
            // menu_Loop
            // 
            this.menu_Loop.Name = "menu_Loop";
            this.menu_Loop.Size = new System.Drawing.Size(209, 26);
            this.menu_Loop.Tag = "Loop";
            this.menu_Loop.Text = "Loop";
            this.menu_Loop.Click += new System.EventHandler(this.menu_MultiSelect_Click);
            // 
            // menu_Jump
            // 
            this.menu_Jump.Name = "menu_Jump";
            this.menu_Jump.Size = new System.Drawing.Size(209, 26);
            this.menu_Jump.Tag = "Jump";
            this.menu_Jump.Text = "Jump";
            this.menu_Jump.Click += new System.EventHandler(this.menu_MultiSelect_Click);
            // 
            // menu_AnodeCharge
            // 
            this.menu_AnodeCharge.Name = "menu_AnodeCharge";
            this.menu_AnodeCharge.Size = new System.Drawing.Size(209, 26);
            this.menu_AnodeCharge.Tag = "AnodeCharge";
            this.menu_AnodeCharge.Text = "Anode Charge";
            this.menu_AnodeCharge.Click += new System.EventHandler(this.menu_MultiSelect_Click);
            // 
            // menu_AnodeDischarge
            // 
            this.menu_AnodeDischarge.Name = "menu_AnodeDischarge";
            this.menu_AnodeDischarge.Size = new System.Drawing.Size(209, 26);
            this.menu_AnodeDischarge.Tag = "AnodeDischarge";
            this.menu_AnodeDischarge.Text = "Anode Discharge";
            this.menu_AnodeDischarge.Click += new System.EventHandler(this.menu_MultiSelect_Click);
            // 
            // menu_SelectAll
            // 
            this.menu_SelectAll.Name = "menu_SelectAll";
            this.menu_SelectAll.Size = new System.Drawing.Size(193, 24);
            this.menu_SelectAll.Text = "모두 선택";
            this.menu_SelectAll.Click += new System.EventHandler(this.menu_SelectAll_Click);
            // 
            // menu_Unselect
            // 
            this.menu_Unselect.Name = "menu_Unselect";
            this.menu_Unselect.Size = new System.Drawing.Size(193, 24);
            this.menu_Unselect.Text = "모두 선택 해제";
            this.menu_Unselect.Click += new System.EventHandler(this.menu_Unselect_Click);
            // 
            // menu_Info
            // 
            this.menu_Info.Name = "menu_Info";
            this.menu_Info.Size = new System.Drawing.Size(193, 24);
            this.menu_Info.Text = "파일 정보";
            this.menu_Info.Visible = false;
            this.menu_Info.Click += new System.EventHandler(this.menu_Info_Click);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel6, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.textBox_Preset, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.button_Apply, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.propertyGrid1, 0, 3);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 4;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(230, 268);
            this.tableLayoutPanel5.TabIndex = 13;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 3;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel6.Controls.Add(this.button_PresetSave, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.button_PresetLoad, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(230, 25);
            this.tableLayoutPanel6.TabIndex = 12;
            // 
            // button_PresetSave
            // 
            this.button_PresetSave.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button_PresetSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_PresetSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.button_PresetSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_PresetSave.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_PresetSave.ForeColor = System.Drawing.Color.White;
            this.button_PresetSave.Location = new System.Drawing.Point(116, 1);
            this.button_PresetSave.Margin = new System.Windows.Forms.Padding(1);
            this.button_PresetSave.Name = "button_PresetSave";
            this.button_PresetSave.Size = new System.Drawing.Size(55, 23);
            this.button_PresetSave.TabIndex = 3;
            this.button_PresetSave.TabStop = false;
            this.button_PresetSave.Text = "Save";
            this.button_PresetSave.UseVisualStyleBackColor = false;
            this.button_PresetSave.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // button_PresetLoad
            // 
            this.button_PresetLoad.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button_PresetLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_PresetLoad.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.button_PresetLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_PresetLoad.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_PresetLoad.ForeColor = System.Drawing.Color.White;
            this.button_PresetLoad.Location = new System.Drawing.Point(173, 1);
            this.button_PresetLoad.Margin = new System.Windows.Forms.Padding(1);
            this.button_PresetLoad.Name = "button_PresetLoad";
            this.button_PresetLoad.Size = new System.Drawing.Size(56, 23);
            this.button_PresetLoad.TabIndex = 4;
            this.button_PresetLoad.TabStop = false;
            this.button_PresetLoad.Text = "Load";
            this.button_PresetLoad.UseVisualStyleBackColor = false;
            this.button_PresetLoad.Click += new System.EventHandler(this.button_Load_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Orange;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(1, 1);
            this.label1.Margin = new System.Windows.Forms.Padding(1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 23);
            this.label1.TabIndex = 5;
            this.label1.Text = "Preset (X축, Y축)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_Preset
            // 
            this.textBox_Preset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Preset.Location = new System.Drawing.Point(3, 28);
            this.textBox_Preset.Name = "textBox_Preset";
            this.textBox_Preset.ReadOnly = true;
            this.textBox_Preset.Size = new System.Drawing.Size(224, 27);
            this.textBox_Preset.TabIndex = 13;
            // 
            // button_Apply
            // 
            this.button_Apply.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button_Apply.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Apply.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.button_Apply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Apply.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Apply.ForeColor = System.Drawing.Color.White;
            this.button_Apply.Location = new System.Drawing.Point(3, 61);
            this.button_Apply.Name = "button_Apply";
            this.button_Apply.Size = new System.Drawing.Size(224, 22);
            this.button_Apply.TabIndex = 3;
            this.button_Apply.TabStop = false;
            this.button_Apply.Text = "Apply";
            this.button_Apply.UseVisualStyleBackColor = false;
            this.button_Apply.Click += new System.EventHandler(this.button_Apply_Click);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.BackColor = System.Drawing.Color.DimGray;
            this.propertyGrid1.CategoryForeColor = System.Drawing.Color.White;
            this.propertyGrid1.CategorySplitterColor = System.Drawing.Color.DimGray;
            this.propertyGrid1.CommandsVisibleIfAvailable = false;
            this.propertyGrid1.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.HelpBackColor = System.Drawing.Color.DimGray;
            this.propertyGrid1.HelpForeColor = System.Drawing.Color.White;
            this.propertyGrid1.LineColor = System.Drawing.Color.Gray;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 89);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid1.SelectedItemWithFocusBackColor = System.Drawing.SystemColors.MenuHighlight;
            this.propertyGrid1.Size = new System.Drawing.Size(224, 272);
            this.propertyGrid1.TabIndex = 11;
            this.propertyGrid1.TabStop = false;
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.ViewBackColor = System.Drawing.Color.DimGray;
            this.propertyGrid1.ViewBorderColor = System.Drawing.Color.DarkGray;
            this.propertyGrid1.ViewForeColor = System.Drawing.Color.White;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.pictureBox1.Image = global::DataViewer.Properties.Resources.Banner;
            this.pictureBox1.Location = new System.Drawing.Point(85, 547);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(142, 44);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 6);
            // 
            // menu_Fra
            // 
            this.menu_Fra.Name = "menu_Fra";
            this.menu_Fra.Size = new System.Drawing.Size(32, 19);
            // 
            // menu_Tra
            // 
            this.menu_Tra.Name = "menu_Tra";
            this.menu_Tra.Size = new System.Drawing.Size(32, 19);
            // 
            // menu_Acr
            // 
            this.menu_Acr.Name = "menu_Acr";
            this.menu_Acr.Size = new System.Drawing.Size(32, 19);
            // 
            // menu_Dcr
            // 
            this.menu_Dcr.Name = "menu_Dcr";
            this.menu_Dcr.Size = new System.Drawing.Size(32, 19);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 6);
            // 
            // menu_Pattern
            // 
            this.menu_Pattern.Name = "menu_Pattern";
            this.menu_Pattern.Size = new System.Drawing.Size(32, 19);
            // 
            // menu_Ocv
            // 
            this.menu_Ocv.Name = "menu_Ocv";
            this.menu_Ocv.Size = new System.Drawing.Size(32, 19);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 6);
            // 
            // menu_DeleteFile
            // 
            this.menu_DeleteFile.Name = "menu_DeleteFile";
            this.menu_DeleteFile.Size = new System.Drawing.Size(32, 19);
            // 
            // menu_Clear
            // 
            this.menu_Clear.Name = "menu_Clear";
            this.menu_Clear.Size = new System.Drawing.Size(32, 19);
            this.menu_Clear.Text = "Clear";
            this.menu_Clear.Click += new System.EventHandler(this.menu_Clear_Click);
            // 
            // menu_Export
            // 
            this.menu_Export.Name = "menu_Export";
            this.menu_Export.Size = new System.Drawing.Size(32, 19);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 6);
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(987, 656);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form_Main";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "D730";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Main_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.RadioButton radioButton_Sheet;
        private System.Windows.Forms.RadioButton radioButton_Graph;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menu_Select;
        private System.Windows.Forms.ToolStripMenuItem menu_Charge;
        private System.Windows.Forms.ToolStripMenuItem menu_Discharge;
        private System.Windows.Forms.ToolStripMenuItem menu_Rest;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menu_Cycle;
        private System.Windows.Forms.ToolStripMenuItem menu_Loop;
        private System.Windows.Forms.ToolStripMenuItem menu_Jump;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menu_Fra;
        private System.Windows.Forms.ToolStripMenuItem menu_Tra;
        private System.Windows.Forms.ToolStripMenuItem menu_Acr;
        private System.Windows.Forms.ToolStripMenuItem menu_Dcr;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem menu_Pattern;
        private System.Windows.Forms.ToolStripMenuItem menu_Ocv;
        private System.Windows.Forms.ToolStripMenuItem menu_SelectAll;
        private System.Windows.Forms.ToolStripMenuItem menu_Unselect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem menu_Clear;
        private System.Windows.Forms.ToolStripMenuItem menu_Export;
        private System.Windows.Forms.ToolStripMenuItem menu_DeleteFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem menu_Info;
        private System.Windows.Forms.ToolStripMenuItem menu_AnodeCharge;
        private System.Windows.Forms.ToolStripMenuItem menu_AnodeDischarge;
        private System.Windows.Forms.Button button_MassGraph;
        private System.Windows.Forms.Button button_AreaGraph;
        private System.Windows.Forms.TextBox textBox_Mass;
        private System.Windows.Forms.TextBox textBox_Area;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button button_FileLoad;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Button button_Config;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Button button_PresetSave;
        private System.Windows.Forms.Button button_PresetLoad;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Preset;
        private System.Windows.Forms.Button button_Apply;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}

