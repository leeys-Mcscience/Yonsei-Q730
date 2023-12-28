
namespace Q730.UserControls
{
    partial class UserControl_SequenceBuilder
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
            if( disposing && (components != null) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer_ItemMover = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.timer_FileLoader = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_ShowInBrowser = new System.Windows.Forms.ToolStripMenuItem();
            this.timer_ContentRefresher = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.label_mass = new System.Windows.Forms.Label();
            this.label_mah = new System.Windows.Forms.Label();
            this.textBox_ElcCapa = new System.Windows.Forms.TextBox();
            this.label_ElcCapaUnit = new System.Windows.Forms.Label();
            this.label_Crate = new System.Windows.Forms.Label();
            this.textBox_Crate = new System.Windows.Forms.TextBox();
            this.label_CrateUnit = new System.Windows.Forms.Label();
            this.label_MassUnit = new System.Windows.Forms.Label();
            this.label_areaUnit = new System.Windows.Forms.Label();
            this.textBox_mass = new System.Windows.Forms.TextBox();
            this.textBox_area = new System.Windows.Forms.TextBox();
            this.label_area = new System.Windows.Forms.Label();
            this.listView_SequenceList = new System.Windows.Forms.ListView();
            this.textBox_Custom_Path = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.button_Clear = new System.Windows.Forms.Button();
            this.button_SaveAs = new System.Windows.Forms.Button();
            this.button_Custom_Load = new System.Windows.Forms.Button();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.button_Apply = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox_Crate = new System.Windows.Forms.CheckBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel_RecipesBox = new System.Windows.Forms.FlowLayoutPanel();
            this.textBox_Comment = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_Loop = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_Name = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel_IconBox = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer_ItemMover
            // 
            this.timer_ItemMover.Interval = 1;
            this.timer_ItemMover.Tick += new System.EventHandler(this.timer_ItemMover_Tick);
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 0;
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 0;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 0;
            // 
            // toolTip2
            // 
            this.toolTip2.AutomaticDelay = 0;
            // 
            // timer_FileLoader
            // 
            this.timer_FileLoader.Interval = 1000;
            this.timer_FileLoader.Tick += new System.EventHandler(this.timer_FileLoader_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Remove,
            this.toolStripMenuItem_ShowInBrowser});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(165, 48);
            // 
            // toolStripMenuItem_Remove
            // 
            this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
            this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(164, 22);
            this.toolStripMenuItem_Remove.Text = "Remove";
            this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
            // 
            // toolStripMenuItem_ShowInBrowser
            // 
            this.toolStripMenuItem_ShowInBrowser.Name = "toolStripMenuItem_ShowInBrowser";
            this.toolStripMenuItem_ShowInBrowser.Size = new System.Drawing.Size(164, 22);
            this.toolStripMenuItem_ShowInBrowser.Text = "Show in browser";
            this.toolStripMenuItem_ShowInBrowser.Click += new System.EventHandler(this.toolStripMenuItem_ShowInBrowser_Click);
            // 
            // timer_ContentRefresher
            // 
            this.timer_ContentRefresher.Tick += new System.EventHandler(this.timer_ContentRefresher_Tick);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel10, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.listView_SequenceList, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.textBox_Custom_Path, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel9, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel7, 0, 4);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(708, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 5;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(349, 711);
            this.tableLayoutPanel4.TabIndex = 12;
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.tableLayoutPanel10.ColumnCount = 3;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel10.Controls.Add(this.label_mass, 0, 2);
            this.tableLayoutPanel10.Controls.Add(this.label_mah, 0, 1);
            this.tableLayoutPanel10.Controls.Add(this.textBox_ElcCapa, 1, 1);
            this.tableLayoutPanel10.Controls.Add(this.label_ElcCapaUnit, 2, 1);
            this.tableLayoutPanel10.Controls.Add(this.label_Crate, 0, 0);
            this.tableLayoutPanel10.Controls.Add(this.textBox_Crate, 1, 0);
            this.tableLayoutPanel10.Controls.Add(this.label_CrateUnit, 2, 0);
            this.tableLayoutPanel10.Controls.Add(this.label_MassUnit, 2, 2);
            this.tableLayoutPanel10.Controls.Add(this.label_areaUnit, 2, 3);
            this.tableLayoutPanel10.Controls.Add(this.textBox_mass, 1, 2);
            this.tableLayoutPanel10.Controls.Add(this.textBox_area, 1, 3);
            this.tableLayoutPanel10.Controls.Add(this.label_area, 0, 3);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(3, 533);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 4;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(343, 134);
            this.tableLayoutPanel10.TabIndex = 15;
            // 
            // label_mass
            // 
            this.label_mass.AutoSize = true;
            this.label_mass.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_mass.Location = new System.Drawing.Point(1, 65);
            this.label_mass.Margin = new System.Windows.Forms.Padding(1);
            this.label_mass.Name = "label_mass";
            this.label_mass.Size = new System.Drawing.Size(100, 30);
            this.label_mass.TabIndex = 9;
            this.label_mass.Text = "전극 질량 :";
            this.label_mass.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_mah
            // 
            this.label_mah.AutoSize = true;
            this.label_mah.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_mah.Location = new System.Drawing.Point(1, 33);
            this.label_mah.Margin = new System.Windows.Forms.Padding(1);
            this.label_mah.Name = "label_mah";
            this.label_mah.Size = new System.Drawing.Size(100, 30);
            this.label_mah.TabIndex = 6;
            this.label_mah.Text = "전극 용량 :";
            this.label_mah.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_ElcCapa
            // 
            this.textBox_ElcCapa.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox_ElcCapa.Location = new System.Drawing.Point(148, 36);
            this.textBox_ElcCapa.Margin = new System.Windows.Forms.Padding(1);
            this.textBox_ElcCapa.Name = "textBox_ElcCapa";
            this.textBox_ElcCapa.Size = new System.Drawing.Size(95, 23);
            this.textBox_ElcCapa.TabIndex = 7;
            // 
            // label_ElcCapaUnit
            // 
            this.label_ElcCapaUnit.AutoSize = true;
            this.label_ElcCapaUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_ElcCapaUnit.Location = new System.Drawing.Point(291, 33);
            this.label_ElcCapaUnit.Margin = new System.Windows.Forms.Padding(1);
            this.label_ElcCapaUnit.Name = "label_ElcCapaUnit";
            this.label_ElcCapaUnit.Size = new System.Drawing.Size(51, 30);
            this.label_ElcCapaUnit.TabIndex = 8;
            this.label_ElcCapaUnit.Text = "mAh";
            this.label_ElcCapaUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_Crate
            // 
            this.label_Crate.AutoSize = true;
            this.label_Crate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Crate.Location = new System.Drawing.Point(1, 1);
            this.label_Crate.Margin = new System.Windows.Forms.Padding(1);
            this.label_Crate.Name = "label_Crate";
            this.label_Crate.Size = new System.Drawing.Size(100, 30);
            this.label_Crate.TabIndex = 3;
            this.label_Crate.Text = "C-rate :";
            this.label_Crate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_Crate
            // 
            this.textBox_Crate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox_Crate.Location = new System.Drawing.Point(148, 4);
            this.textBox_Crate.Margin = new System.Windows.Forms.Padding(1);
            this.textBox_Crate.Name = "textBox_Crate";
            this.textBox_Crate.Size = new System.Drawing.Size(95, 23);
            this.textBox_Crate.TabIndex = 4;
            // 
            // label_CrateUnit
            // 
            this.label_CrateUnit.AutoSize = true;
            this.label_CrateUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_CrateUnit.Location = new System.Drawing.Point(291, 1);
            this.label_CrateUnit.Margin = new System.Windows.Forms.Padding(1);
            this.label_CrateUnit.Name = "label_CrateUnit";
            this.label_CrateUnit.Size = new System.Drawing.Size(51, 30);
            this.label_CrateUnit.TabIndex = 5;
            this.label_CrateUnit.Text = "C";
            this.label_CrateUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_MassUnit
            // 
            this.label_MassUnit.AutoSize = true;
            this.label_MassUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_MassUnit.Location = new System.Drawing.Point(291, 65);
            this.label_MassUnit.Margin = new System.Windows.Forms.Padding(1);
            this.label_MassUnit.Name = "label_MassUnit";
            this.label_MassUnit.Size = new System.Drawing.Size(51, 30);
            this.label_MassUnit.TabIndex = 11;
            this.label_MassUnit.Text = "g";
            this.label_MassUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_areaUnit
            // 
            this.label_areaUnit.AutoSize = true;
            this.label_areaUnit.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.label_areaUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_areaUnit.Location = new System.Drawing.Point(291, 97);
            this.label_areaUnit.Margin = new System.Windows.Forms.Padding(1);
            this.label_areaUnit.Name = "label_areaUnit";
            this.label_areaUnit.Size = new System.Drawing.Size(51, 36);
            this.label_areaUnit.TabIndex = 12;
            this.label_areaUnit.Text = "㎠";
            this.label_areaUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_mass
            // 
            this.textBox_mass.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox_mass.Location = new System.Drawing.Point(148, 68);
            this.textBox_mass.Margin = new System.Windows.Forms.Padding(1);
            this.textBox_mass.Name = "textBox_mass";
            this.textBox_mass.Size = new System.Drawing.Size(95, 23);
            this.textBox_mass.TabIndex = 14;
            // 
            // textBox_area
            // 
            this.textBox_area.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox_area.Location = new System.Drawing.Point(148, 103);
            this.textBox_area.Margin = new System.Windows.Forms.Padding(1);
            this.textBox_area.Name = "textBox_area";
            this.textBox_area.Size = new System.Drawing.Size(95, 23);
            this.textBox_area.TabIndex = 15;
            // 
            // label_area
            // 
            this.label_area.AutoSize = true;
            this.label_area.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_area.Location = new System.Drawing.Point(1, 97);
            this.label_area.Margin = new System.Windows.Forms.Padding(1);
            this.label_area.Name = "label_area";
            this.label_area.Size = new System.Drawing.Size(100, 36);
            this.label_area.TabIndex = 16;
            this.label_area.Text = "전극 면적 :";
            this.label_area.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // listView_SequenceList
            // 
            this.listView_SequenceList.ContextMenuStrip = this.contextMenuStrip1;
            this.listView_SequenceList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_SequenceList.FullRowSelect = true;
            this.listView_SequenceList.GridLines = true;
            this.listView_SequenceList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView_SequenceList.HideSelection = false;
            this.listView_SequenceList.Location = new System.Drawing.Point(3, 83);
            this.listView_SequenceList.MultiSelect = false;
            this.listView_SequenceList.Name = "listView_SequenceList";
            this.listView_SequenceList.Size = new System.Drawing.Size(343, 444);
            this.listView_SequenceList.TabIndex = 11;
            this.listView_SequenceList.UseCompatibleStateImageBehavior = false;
            this.listView_SequenceList.View = System.Windows.Forms.View.Details;
            this.listView_SequenceList.DoubleClick += new System.EventHandler(this.listView_SequenceList_DoubleClick);
            // 
            // textBox_Custom_Path
            // 
            this.textBox_Custom_Path.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Custom_Path.Location = new System.Drawing.Point(3, 33);
            this.textBox_Custom_Path.Multiline = true;
            this.textBox_Custom_Path.Name = "textBox_Custom_Path";
            this.textBox_Custom_Path.ReadOnly = true;
            this.textBox_Custom_Path.Size = new System.Drawing.Size(343, 44);
            this.textBox_Custom_Path.TabIndex = 13;
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 3;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel9.Controls.Add(this.button_Clear, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.button_SaveAs, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.button_Custom_Load, 0, 0);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 1;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(343, 24);
            this.tableLayoutPanel9.TabIndex = 14;
            // 
            // button_Clear
            // 
            this.button_Clear.BackColor = System.Drawing.Color.Silver;
            this.button_Clear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Clear.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.button_Clear.FlatAppearance.BorderSize = 0;
            this.button_Clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Clear.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Clear.ForeColor = System.Drawing.Color.Black;
            this.button_Clear.Location = new System.Drawing.Point(229, 1);
            this.button_Clear.Margin = new System.Windows.Forms.Padding(1);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(113, 22);
            this.button_Clear.TabIndex = 16;
            this.button_Clear.Text = "Clear";
            this.button_Clear.UseVisualStyleBackColor = false;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // button_SaveAs
            // 
            this.button_SaveAs.BackColor = System.Drawing.Color.Silver;
            this.button_SaveAs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_SaveAs.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.button_SaveAs.FlatAppearance.BorderSize = 0;
            this.button_SaveAs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SaveAs.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_SaveAs.ForeColor = System.Drawing.Color.Black;
            this.button_SaveAs.Location = new System.Drawing.Point(115, 1);
            this.button_SaveAs.Margin = new System.Windows.Forms.Padding(1);
            this.button_SaveAs.Name = "button_SaveAs";
            this.button_SaveAs.Size = new System.Drawing.Size(112, 22);
            this.button_SaveAs.TabIndex = 15;
            this.button_SaveAs.Text = "저장";
            this.button_SaveAs.UseVisualStyleBackColor = false;
            this.button_SaveAs.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // button_Custom_Load
            // 
            this.button_Custom_Load.BackColor = System.Drawing.Color.Silver;
            this.button_Custom_Load.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Custom_Load.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.button_Custom_Load.FlatAppearance.BorderSize = 0;
            this.button_Custom_Load.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Custom_Load.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Custom_Load.ForeColor = System.Drawing.Color.Black;
            this.button_Custom_Load.Location = new System.Drawing.Point(1, 1);
            this.button_Custom_Load.Margin = new System.Windows.Forms.Padding(1);
            this.button_Custom_Load.Name = "button_Custom_Load";
            this.button_Custom_Load.Size = new System.Drawing.Size(112, 22);
            this.button_Custom_Load.TabIndex = 14;
            this.button_Custom_Load.Text = "경로 설정";
            this.button_Custom_Load.UseVisualStyleBackColor = false;
            this.button_Custom_Load.Click += new System.EventHandler(this.button_Custom_Load_Click);
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.tableLayoutPanel7.ColumnCount = 3;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Controls.Add(this.button_Apply, 2, 0);
            this.tableLayoutPanel7.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.checkBox_Crate, 1, 0);
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 673);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(342, 35);
            this.tableLayoutPanel7.TabIndex = 16;
            // 
            // button_Apply
            // 
            this.button_Apply.BackColor = System.Drawing.Color.Silver;
            this.button_Apply.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Apply.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.button_Apply.FlatAppearance.BorderSize = 0;
            this.button_Apply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Apply.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Apply.ForeColor = System.Drawing.Color.Black;
            this.button_Apply.Location = new System.Drawing.Point(151, 1);
            this.button_Apply.Margin = new System.Windows.Forms.Padding(1);
            this.button_Apply.Name = "button_Apply";
            this.button_Apply.Size = new System.Drawing.Size(190, 33);
            this.button_Apply.TabIndex = 3;
            this.button_Apply.Text = "Apply";
            this.button_Apply.UseVisualStyleBackColor = false;
            this.button_Apply.Click += new System.EventHandler(this.button_Apply_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Use C-rate :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBox_Crate
            // 
            this.checkBox_Crate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox_Crate.AutoSize = true;
            this.checkBox_Crate.Location = new System.Drawing.Point(85, 8);
            this.checkBox_Crate.Name = "checkBox_Crate";
            this.checkBox_Crate.Size = new System.Drawing.Size(59, 19);
            this.checkBox_Crate.TabIndex = 5;
            this.checkBox_Crate.Text = "Check";
            this.checkBox_Crate.UseVisualStyleBackColor = true;
            this.checkBox_Crate.CheckedChanged += new System.EventHandler(this.checkBox_Crate_CheckedChanged);
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
            this.propertyGrid1.Location = new System.Drawing.Point(343, 3);
            this.propertyGrid1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid1.SelectedItemWithFocusBackColor = System.Drawing.SystemColors.MenuHighlight;
            this.propertyGrid1.Size = new System.Drawing.Size(362, 994);
            this.propertyGrid1.TabIndex = 10;
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.ViewBackColor = System.Drawing.Color.White;
            this.propertyGrid1.ViewBorderColor = System.Drawing.Color.DarkGray;
            this.propertyGrid1.ViewForeColor = System.Drawing.Color.Black;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel_RecipesBox, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBox_Comment, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel8, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(49, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(291, 994);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel_RecipesBox
            // 
            this.flowLayoutPanel_RecipesBox.AutoScroll = true;
            this.flowLayoutPanel_RecipesBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.flowLayoutPanel_RecipesBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel_RecipesBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_RecipesBox.Location = new System.Drawing.Point(0, 80);
            this.flowLayoutPanel_RecipesBox.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel_RecipesBox.Name = "flowLayoutPanel_RecipesBox";
            this.flowLayoutPanel_RecipesBox.Size = new System.Drawing.Size(291, 834);
            this.flowLayoutPanel_RecipesBox.TabIndex = 4;
            // 
            // textBox_Comment
            // 
            this.textBox_Comment.ForeColor = System.Drawing.Color.LightGray;
            this.textBox_Comment.Location = new System.Drawing.Point(1, 915);
            this.textBox_Comment.Margin = new System.Windows.Forms.Padding(1);
            this.textBox_Comment.Multiline = true;
            this.textBox_Comment.Name = "textBox_Comment";
            this.textBox_Comment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Comment.Size = new System.Drawing.Size(289, 78);
            this.textBox_Comment.TabIndex = 2;
            this.textBox_Comment.Text = "Comment here...";
            this.textBox_Comment.Enter += new System.EventHandler(this.textBox_Comment_Enter);
            this.textBox_Comment.Leave += new System.EventHandler(this.textBox_Comment_Leave);
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 3;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel8.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.comboBox_Loop, 1, 0);
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 43);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(285, 34);
            this.tableLayoutPanel8.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label2.Location = new System.Drawing.Point(1, 1);
            this.label2.Margin = new System.Windows.Forms.Padding(1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 32);
            this.label2.TabIndex = 0;
            this.label2.Text = "Loop 선택 :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comboBox_Loop
            // 
            this.comboBox_Loop.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBox_Loop.FormattingEnabled = true;
            this.comboBox_Loop.Location = new System.Drawing.Point(86, 5);
            this.comboBox_Loop.Margin = new System.Windows.Forms.Padding(1);
            this.comboBox_Loop.Name = "comboBox_Loop";
            this.comboBox_Loop.Size = new System.Drawing.Size(154, 23);
            this.comboBox_Loop.TabIndex = 1;
            this.comboBox_Loop.SelectedIndexChanged += new System.EventHandler(this.comboBox_Loop_SelectedIndexChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBox_Name, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(285, 34);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Location = new System.Drawing.Point(1, 1);
            this.label3.Margin = new System.Windows.Forms.Padding(1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 32);
            this.label3.TabIndex = 4;
            this.label3.Text = "RecipeName :";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_Name
            // 
            this.textBox_Name.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox_Name.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_Name.Location = new System.Drawing.Point(88, 5);
            this.textBox_Name.Margin = new System.Windows.Forms.Padding(1);
            this.textBox_Name.Name = "textBox_Name";
            this.textBox_Name.Size = new System.Drawing.Size(150, 23);
            this.textBox_Name.TabIndex = 3;
            // 
            // flowLayoutPanel_IconBox
            // 
            this.flowLayoutPanel_IconBox.AutoScroll = true;
            this.flowLayoutPanel_IconBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.flowLayoutPanel_IconBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel_IconBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_IconBox.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel_IconBox.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel_IconBox.Name = "flowLayoutPanel_IconBox";
            this.flowLayoutPanel_IconBox.Size = new System.Drawing.Size(46, 1000);
            this.flowLayoutPanel_IconBox.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 297F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel_IconBox, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel1, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.propertyGrid1, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 3, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 718F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1068, 1000);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // UserControl_SequenceBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel3);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "UserControl_SequenceBuilder";
            this.Size = new System.Drawing.Size(1068, 1000);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel10.PerformLayout();
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer_ItemMover;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip2;
        private System.Windows.Forms.Timer timer_FileLoader;
        private System.Windows.Forms.Timer timer_ContentRefresher;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Remove;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_ShowInBrowser;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.ListView listView_SequenceList;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_IconBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TextBox textBox_Custom_Path;
        private System.Windows.Forms.TextBox textBox_Comment;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_RecipesBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_Loop;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private System.Windows.Forms.Button button_Custom_Load;
        private System.Windows.Forms.Button button_SaveAs;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_Name;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.Label label_mah;
        private System.Windows.Forms.TextBox textBox_ElcCapa;
        private System.Windows.Forms.Label label_Crate;
        private System.Windows.Forms.TextBox textBox_Crate;
        private System.Windows.Forms.Label label_CrateUnit;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Button button_Apply;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox_Crate;
        private System.Windows.Forms.Label label_mass;
        private System.Windows.Forms.Label label_ElcCapaUnit;
        private System.Windows.Forms.Label label_MassUnit;
        private System.Windows.Forms.Label label_areaUnit;
        private System.Windows.Forms.TextBox textBox_mass;
        private System.Windows.Forms.TextBox textBox_area;
        private System.Windows.Forms.Label label_area;
    }
}
