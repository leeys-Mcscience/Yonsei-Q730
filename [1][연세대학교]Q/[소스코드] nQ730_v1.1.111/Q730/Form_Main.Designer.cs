
namespace Q730
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
            if( disposing && (components != null) )
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Main));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.radioButton_Graph = new System.Windows.Forms.RadioButton();
            this.radioButton_Grid = new System.Windows.Forms.RadioButton();
            this.radioButton_Detail = new System.Windows.Forms.RadioButton();
            this.radioButton_List = new System.Windows.Forms.RadioButton();
            this.label_SdError = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox_SquencePath = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.button_Custom_Load = new System.Windows.Forms.Button();
            this.listView_SequenceList = new System.Windows.Forms.ListView();
            this.textBox_Custom_Path = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox_Name = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button_Apply = new System.Windows.Forms.Button();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.button_Path = new System.Windows.Forms.Button();
            this.textBox_Path = new System.Windows.Forms.TextBox();
            this.comboBox_SequenceList = new System.Windows.Forms.ComboBox();
            this.userControl_SequenceViewer1 = new Q730.UserControls.UserControl_SequenceViewer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label_Version = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.label12 = new System.Windows.Forms.Label();
            this.button_Read = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button_NextStep = new System.Windows.Forms.Button();
            this.button_Configuration = new System.Windows.Forms.Button();
            this.button_Connect = new System.Windows.Forms.Button();
            this.button_Stop = new System.Windows.Forms.Button();
            this.button_SequenceBuilder = new System.Windows.Forms.Button();
            this.button_Start = new System.Windows.Forms.Button();
            this.button_Pause = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.timer_ConnectionChecker = new System.Windows.Forms.Timer(this.components);
            this.timer_RunChecker = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(5674, 1895);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel8, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 67);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(5668, 1825);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(62, 0);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(5376, 1825);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 8;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.Controls.Add(this.radioButton_Graph, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.radioButton_Grid, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.radioButton_Detail, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.radioButton_List, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label_SdError, 7, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(5376, 28);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // radioButton_Graph
            // 
            this.radioButton_Graph.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_Graph.AutoSize = true;
            this.radioButton_Graph.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.radioButton_Graph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButton_Graph.Enabled = false;
            this.radioButton_Graph.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(176)))), ((int)(((byte)(176)))));
            this.radioButton_Graph.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.radioButton_Graph.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton_Graph.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_Graph.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(201)))), ((int)(((byte)(202)))));
            this.radioButton_Graph.Location = new System.Drawing.Point(181, 2);
            this.radioButton_Graph.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButton_Graph.Name = "radioButton_Graph";
            this.radioButton_Graph.Size = new System.Drawing.Size(58, 24);
            this.radioButton_Graph.TabIndex = 3;
            this.radioButton_Graph.Text = "GRAPH";
            this.radioButton_Graph.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_Graph.UseVisualStyleBackColor = false;
            this.radioButton_Graph.Visible = false;
            this.radioButton_Graph.Click += new System.EventHandler(this.radioButtons_Click);
            // 
            // radioButton_Grid
            // 
            this.radioButton_Grid.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_Grid.AutoSize = true;
            this.radioButton_Grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.radioButton_Grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButton_Grid.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(176)))), ((int)(((byte)(176)))));
            this.radioButton_Grid.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.radioButton_Grid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton_Grid.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_Grid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(201)))), ((int)(((byte)(202)))));
            this.radioButton_Grid.Location = new System.Drawing.Point(61, 2);
            this.radioButton_Grid.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButton_Grid.Name = "radioButton_Grid";
            this.radioButton_Grid.Size = new System.Drawing.Size(58, 24);
            this.radioButton_Grid.TabIndex = 0;
            this.radioButton_Grid.Text = "GRID";
            this.radioButton_Grid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_Grid.UseVisualStyleBackColor = false;
            this.radioButton_Grid.Click += new System.EventHandler(this.radioButtons_Click);
            // 
            // radioButton_Detail
            // 
            this.radioButton_Detail.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_Detail.AutoSize = true;
            this.radioButton_Detail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.radioButton_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButton_Detail.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(176)))), ((int)(((byte)(176)))));
            this.radioButton_Detail.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.radioButton_Detail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton_Detail.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_Detail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(201)))), ((int)(((byte)(202)))));
            this.radioButton_Detail.Location = new System.Drawing.Point(121, 2);
            this.radioButton_Detail.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButton_Detail.Name = "radioButton_Detail";
            this.radioButton_Detail.Size = new System.Drawing.Size(58, 24);
            this.radioButton_Detail.TabIndex = 5;
            this.radioButton_Detail.Text = "DETAIL";
            this.radioButton_Detail.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_Detail.UseVisualStyleBackColor = false;
            this.radioButton_Detail.Visible = false;
            this.radioButton_Detail.Click += new System.EventHandler(this.radioButtons_Click);
            // 
            // radioButton_List
            // 
            this.radioButton_List.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_List.AutoSize = true;
            this.radioButton_List.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.radioButton_List.Checked = true;
            this.radioButton_List.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButton_List.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(176)))), ((int)(((byte)(176)))));
            this.radioButton_List.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.radioButton_List.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton_List.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_List.ForeColor = System.Drawing.Color.Black;
            this.radioButton_List.Location = new System.Drawing.Point(1, 2);
            this.radioButton_List.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButton_List.Name = "radioButton_List";
            this.radioButton_List.Size = new System.Drawing.Size(58, 24);
            this.radioButton_List.TabIndex = 1;
            this.radioButton_List.TabStop = true;
            this.radioButton_List.Text = "LIST";
            this.radioButton_List.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_List.UseVisualStyleBackColor = false;
            this.radioButton_List.Click += new System.EventHandler(this.radioButtons_Click);
            // 
            // label_SdError
            // 
            this.label_SdError.AutoSize = true;
            this.label_SdError.BackColor = System.Drawing.Color.Red;
            this.label_SdError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_SdError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_SdError.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_SdError.Location = new System.Drawing.Point(5277, 1);
            this.label_SdError.Margin = new System.Windows.Forms.Padding(1);
            this.label_SdError.Name = "label_SdError";
            this.label_SdError.Size = new System.Drawing.Size(98, 26);
            this.label_SdError.TabIndex = 5;
            this.label_SdError.Text = "SD ERROR!!";
            this.label_SdError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_SdError.Visible = false;
            this.label_SdError.Click += new System.EventHandler(this.label_SdError_Click);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.textBox_SquencePath, 0, 5);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel9, 0, 4);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel7, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.button_Apply, 0, 8);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel6, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.comboBox_SequenceList, 0, 6);
            this.tableLayoutPanel4.Controls.Add(this.userControl_SequenceViewer1, 0, 7);
            this.tableLayoutPanel4.Controls.Add(this.pictureBox1, 0, 9);
            this.tableLayoutPanel4.Controls.Add(this.label_Version, 0, 10);
            this.tableLayoutPanel4.Controls.Add(this.label9, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label10, 0, 3);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(5438, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 11;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(230, 1825);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // textBox_SquencePath
            // 
            this.textBox_SquencePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_SquencePath.Location = new System.Drawing.Point(3, 127);
            this.textBox_SquencePath.Multiline = true;
            this.textBox_SquencePath.Name = "textBox_SquencePath";
            this.textBox_SquencePath.ReadOnly = true;
            this.textBox_SquencePath.Size = new System.Drawing.Size(224, 44);
            this.textBox_SquencePath.TabIndex = 14;
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 1;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Controls.Add(this.button_Custom_Load, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.listView_SequenceList, 0, 2);
            this.tableLayoutPanel9.Controls.Add(this.textBox_Custom_Path, 0, 1);
            this.tableLayoutPanel9.Location = new System.Drawing.Point(3, 99);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 3;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(224, 22);
            this.tableLayoutPanel9.TabIndex = 13;
            // 
            // button_Custom_Load
            // 
            this.button_Custom_Load.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button_Custom_Load.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Custom_Load.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.button_Custom_Load.FlatAppearance.BorderSize = 0;
            this.button_Custom_Load.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Custom_Load.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Custom_Load.ForeColor = System.Drawing.Color.White;
            this.button_Custom_Load.Location = new System.Drawing.Point(1, 1);
            this.button_Custom_Load.Margin = new System.Windows.Forms.Padding(1);
            this.button_Custom_Load.Name = "button_Custom_Load";
            this.button_Custom_Load.Size = new System.Drawing.Size(222, 23);
            this.button_Custom_Load.TabIndex = 12;
            this.button_Custom_Load.Text = "Custom Sequence  File Load";
            this.button_Custom_Load.UseVisualStyleBackColor = false;
            this.button_Custom_Load.Click += new System.EventHandler(this.button_Custom_Load_Click);
            // 
            // listView_SequenceList
            // 
            this.listView_SequenceList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_SequenceList.FullRowSelect = true;
            this.listView_SequenceList.GridLines = true;
            this.listView_SequenceList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView_SequenceList.HideSelection = false;
            this.listView_SequenceList.Location = new System.Drawing.Point(3, 78);
            this.listView_SequenceList.MultiSelect = false;
            this.listView_SequenceList.Name = "listView_SequenceList";
            this.listView_SequenceList.Size = new System.Drawing.Size(218, 1);
            this.listView_SequenceList.TabIndex = 11;
            this.listView_SequenceList.UseCompatibleStateImageBehavior = false;
            this.listView_SequenceList.View = System.Windows.Forms.View.Details;
            // 
            // textBox_Custom_Path
            // 
            this.textBox_Custom_Path.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Custom_Path.Location = new System.Drawing.Point(3, 28);
            this.textBox_Custom_Path.Multiline = true;
            this.textBox_Custom_Path.Name = "textBox_Custom_Path";
            this.textBox_Custom_Path.ReadOnly = true;
            this.textBox_Custom_Path.Size = new System.Drawing.Size(218, 44);
            this.textBox_Custom_Path.TabIndex = 13;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Controls.Add(this.textBox_Name, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.label8, 0, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(0, 48);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(230, 28);
            this.tableLayoutPanel7.TabIndex = 8;
            // 
            // textBox_Name
            // 
            this.textBox_Name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Name.Location = new System.Drawing.Point(103, 3);
            this.textBox_Name.Name = "textBox_Name";
            this.textBox_Name.Size = new System.Drawing.Size(124, 27);
            this.textBox_Name.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.Location = new System.Drawing.Point(3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 28);
            this.label8.TabIndex = 1;
            this.label8.Text = "파일 저장 이름";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_Apply
            // 
            this.button_Apply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.button_Apply.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Apply.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.button_Apply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Apply.ForeColor = System.Drawing.Color.White;
            this.button_Apply.Location = new System.Drawing.Point(1, 1710);
            this.button_Apply.Margin = new System.Windows.Forms.Padding(1);
            this.button_Apply.Name = "button_Apply";
            this.button_Apply.Size = new System.Drawing.Size(228, 26);
            this.button_Apply.TabIndex = 1;
            this.button_Apply.TabStop = false;
            this.button_Apply.Text = "Apply";
            this.button_Apply.UseVisualStyleBackColor = false;
            this.button_Apply.Click += new System.EventHandler(this.button_Apply_Click);
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.button_Path, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.textBox_Path, 1, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 20);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(230, 28);
            this.tableLayoutPanel6.TabIndex = 4;
            // 
            // button_Path
            // 
            this.button_Path.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button_Path.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Path.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(191)))), ((int)(((byte)(191)))));
            this.button_Path.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Path.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Path.ForeColor = System.Drawing.Color.White;
            this.button_Path.Location = new System.Drawing.Point(2, 2);
            this.button_Path.Margin = new System.Windows.Forms.Padding(2);
            this.button_Path.Name = "button_Path";
            this.button_Path.Size = new System.Drawing.Size(96, 24);
            this.button_Path.TabIndex = 0;
            this.button_Path.TabStop = false;
            this.button_Path.Text = "저장 경로 설정";
            this.button_Path.UseVisualStyleBackColor = false;
            this.button_Path.Click += new System.EventHandler(this.button_Path_Click);
            // 
            // textBox_Path
            // 
            this.textBox_Path.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_Path.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Path.Location = new System.Drawing.Point(103, 3);
            this.textBox_Path.Name = "textBox_Path";
            this.textBox_Path.ReadOnly = true;
            this.textBox_Path.Size = new System.Drawing.Size(124, 27);
            this.textBox_Path.TabIndex = 1;
            this.textBox_Path.TabStop = false;
            // 
            // comboBox_SequenceList
            // 
            this.comboBox_SequenceList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_SequenceList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_SequenceList.FormattingEnabled = true;
            this.comboBox_SequenceList.Location = new System.Drawing.Point(2, 176);
            this.comboBox_SequenceList.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_SequenceList.Name = "comboBox_SequenceList";
            this.comboBox_SequenceList.Size = new System.Drawing.Size(226, 28);
            this.comboBox_SequenceList.TabIndex = 0;
            this.comboBox_SequenceList.TabStop = false;
            this.comboBox_SequenceList.DropDown += new System.EventHandler(this.comboBox1_DropDown);
            this.comboBox_SequenceList.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // userControl_SequenceViewer1
            // 
            this.userControl_SequenceViewer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.userControl_SequenceViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControl_SequenceViewer1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.userControl_SequenceViewer1.Location = new System.Drawing.Point(1, 203);
            this.userControl_SequenceViewer1.Margin = new System.Windows.Forms.Padding(1);
            this.userControl_SequenceViewer1.Name = "userControl_SequenceViewer1";
            this.userControl_SequenceViewer1.Size = new System.Drawing.Size(228, 1505);
            this.userControl_SequenceViewer1.TabIndex = 5;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.Image = global::Q730.Properties.Resources.Banner;
            this.pictureBox1.Location = new System.Drawing.Point(31, 1744);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(167, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // label_Version
            // 
            this.label_Version.AutoSize = true;
            this.label_Version.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Version.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_Version.Location = new System.Drawing.Point(3, 1801);
            this.label_Version.Name = "label_Version";
            this.label_Version.Size = new System.Drawing.Size(224, 24);
            this.label_Version.TabIndex = 7;
            this.label_Version.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label_Version.DoubleClick += new System.EventHandler(this.label_Version_DoubleClick);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Orange;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.Location = new System.Drawing.Point(1, 1);
            this.label9.Margin = new System.Windows.Forms.Padding(1);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(228, 18);
            this.label9.TabIndex = 9;
            this.label9.Text = "1) Result Data ";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Orange;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label10.Location = new System.Drawing.Point(1, 77);
            this.label10.Margin = new System.Windows.Forms.Padding(1);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(228, 18);
            this.label10.TabIndex = 10;
            this.label10.Text = "2) Sequence Setting";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel8.ColumnCount = 1;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Controls.Add(this.label12, 0, 15);
            this.tableLayoutPanel8.Controls.Add(this.button_Read, 0, 14);
            this.tableLayoutPanel8.Controls.Add(this.label7, 0, 13);
            this.tableLayoutPanel8.Controls.Add(this.label6, 0, 11);
            this.tableLayoutPanel8.Controls.Add(this.label5, 0, 9);
            this.tableLayoutPanel8.Controls.Add(this.label4, 0, 7);
            this.tableLayoutPanel8.Controls.Add(this.label3, 0, 5);
            this.tableLayoutPanel8.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel8.Controls.Add(this.button_NextStep, 0, 8);
            this.tableLayoutPanel8.Controls.Add(this.button_Configuration, 0, 12);
            this.tableLayoutPanel8.Controls.Add(this.button_Connect, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.button_Stop, 0, 10);
            this.tableLayoutPanel8.Controls.Add(this.button_SequenceBuilder, 0, 2);
            this.tableLayoutPanel8.Controls.Add(this.button_Start, 0, 4);
            this.tableLayoutPanel8.Controls.Add(this.button_Pause, 0, 6);
            this.tableLayoutPanel8.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel8.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 17;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(60, 1823);
            this.tableLayoutPanel8.TabIndex = 0;
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label12.Location = new System.Drawing.Point(10, 620);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(40, 19);
            this.label12.TabIndex = 18;
            this.label12.Text = "Read";
            this.label12.Visible = false;
            // 
            // button_Read
            // 
            this.button_Read.BackgroundImage = global::Q730.Properties.Resources.Icon_Data;
            this.button_Read.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Read.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Read.FlatAppearance.BorderSize = 0;
            this.button_Read.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.button_Read.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.button_Read.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Read.Location = new System.Drawing.Point(5, 565);
            this.button_Read.Margin = new System.Windows.Forms.Padding(5);
            this.button_Read.Name = "button_Read";
            this.button_Read.Size = new System.Drawing.Size(50, 50);
            this.button_Read.TabIndex = 16;
            this.button_Read.TabStop = false;
            this.button_Read.UseVisualStyleBackColor = true;
            this.button_Read.Visible = false;
            this.button_Read.Click += new System.EventHandler(this.button_Read_Click);
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(3, 540);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 19);
            this.label7.TabIndex = 1;
            this.label7.Text = "Setting";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(11, 460);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 19);
            this.label6.TabIndex = 4;
            this.label6.Text = "Stop";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(10, 380);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 19);
            this.label5.TabIndex = 7;
            this.label5.Text = "Next";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(7, 300);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 19);
            this.label4.TabIndex = 13;
            this.label4.Text = "Pause";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(10, 220);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 19);
            this.label3.TabIndex = 12;
            this.label3.Text = "Start";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(3, 140);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "Sequence";
            // 
            // button_NextStep
            // 
            this.button_NextStep.BackgroundImage = global::Q730.Properties.Resources.Icon_Next_New;
            this.button_NextStep.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_NextStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_NextStep.FlatAppearance.BorderSize = 0;
            this.button_NextStep.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.button_NextStep.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.button_NextStep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_NextStep.Location = new System.Drawing.Point(5, 325);
            this.button_NextStep.Margin = new System.Windows.Forms.Padding(5);
            this.button_NextStep.Name = "button_NextStep";
            this.button_NextStep.Size = new System.Drawing.Size(50, 50);
            this.button_NextStep.TabIndex = 8;
            this.button_NextStep.TabStop = false;
            this.button_NextStep.UseVisualStyleBackColor = true;
            this.button_NextStep.Click += new System.EventHandler(this.button_NextStep_Click);
            // 
            // button_Configuration
            // 
            this.button_Configuration.BackgroundImage = global::Q730.Properties.Resources.Icon_Setting_New;
            this.button_Configuration.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Configuration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Configuration.FlatAppearance.BorderSize = 0;
            this.button_Configuration.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.button_Configuration.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.button_Configuration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Configuration.Location = new System.Drawing.Point(5, 485);
            this.button_Configuration.Margin = new System.Windows.Forms.Padding(5);
            this.button_Configuration.Name = "button_Configuration";
            this.button_Configuration.Size = new System.Drawing.Size(50, 50);
            this.button_Configuration.TabIndex = 2;
            this.button_Configuration.TabStop = false;
            this.button_Configuration.UseVisualStyleBackColor = true;
            this.button_Configuration.Click += new System.EventHandler(this.button_Configuration_Click);
            // 
            // button_Connect
            // 
            this.button_Connect.BackgroundImage = global::Q730.Properties.Resources.Icon_Disconnected_New;
            this.button_Connect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Connect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Connect.FlatAppearance.BorderSize = 0;
            this.button_Connect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.button_Connect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.button_Connect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Connect.Location = new System.Drawing.Point(5, 5);
            this.button_Connect.Margin = new System.Windows.Forms.Padding(5);
            this.button_Connect.Name = "button_Connect";
            this.button_Connect.Size = new System.Drawing.Size(50, 50);
            this.button_Connect.TabIndex = 11;
            this.button_Connect.TabStop = false;
            this.button_Connect.UseVisualStyleBackColor = true;
            this.button_Connect.Click += new System.EventHandler(this.button_Connect_Click);
            // 
            // button_Stop
            // 
            this.button_Stop.BackgroundImage = global::Q730.Properties.Resources.Icon_Stop_New;
            this.button_Stop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Stop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Stop.FlatAppearance.BorderSize = 0;
            this.button_Stop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.button_Stop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.button_Stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Stop.Location = new System.Drawing.Point(5, 405);
            this.button_Stop.Margin = new System.Windows.Forms.Padding(5);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(50, 50);
            this.button_Stop.TabIndex = 5;
            this.button_Stop.TabStop = false;
            this.button_Stop.UseVisualStyleBackColor = true;
            this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
            // 
            // button_SequenceBuilder
            // 
            this.button_SequenceBuilder.BackgroundImage = global::Q730.Properties.Resources.Icon_Builder_New;
            this.button_SequenceBuilder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_SequenceBuilder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_SequenceBuilder.FlatAppearance.BorderSize = 0;
            this.button_SequenceBuilder.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.button_SequenceBuilder.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.button_SequenceBuilder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SequenceBuilder.Location = new System.Drawing.Point(5, 85);
            this.button_SequenceBuilder.Margin = new System.Windows.Forms.Padding(5);
            this.button_SequenceBuilder.Name = "button_SequenceBuilder";
            this.button_SequenceBuilder.Size = new System.Drawing.Size(50, 50);
            this.button_SequenceBuilder.TabIndex = 1;
            this.button_SequenceBuilder.TabStop = false;
            this.button_SequenceBuilder.UseVisualStyleBackColor = true;
            this.button_SequenceBuilder.Click += new System.EventHandler(this.button_SequenceBuilder_Click);
            // 
            // button_Start
            // 
            this.button_Start.BackgroundImage = global::Q730.Properties.Resources.Icon_Start_New;
            this.button_Start.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Start.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Start.FlatAppearance.BorderSize = 0;
            this.button_Start.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.button_Start.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.button_Start.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Start.Location = new System.Drawing.Point(5, 165);
            this.button_Start.Margin = new System.Windows.Forms.Padding(5);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(50, 50);
            this.button_Start.TabIndex = 2;
            this.button_Start.TabStop = false;
            this.button_Start.UseVisualStyleBackColor = true;
            this.button_Start.Click += new System.EventHandler(this.button_Start_Click);
            // 
            // button_Pause
            // 
            this.button_Pause.BackgroundImage = global::Q730.Properties.Resources.Icon_Pause_New;
            this.button_Pause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Pause.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Pause.FlatAppearance.BorderSize = 0;
            this.button_Pause.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.button_Pause.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.button_Pause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Pause.Location = new System.Drawing.Point(5, 245);
            this.button_Pause.Margin = new System.Windows.Forms.Padding(5);
            this.button_Pause.Name = "button_Pause";
            this.button_Pause.Size = new System.Drawing.Size(50, 50);
            this.button_Pause.TabIndex = 3;
            this.button_Pause.TabStop = false;
            this.button_Pause.UseVisualStyleBackColor = true;
            this.button_Pause.Click += new System.EventHandler(this.button_Pause_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(11, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 19);
            this.label1.TabIndex = 12;
            this.label1.Text = "연결";
            // 
            // timer_ConnectionChecker
            // 
            this.timer_ConnectionChecker.Interval = 3000;
            this.timer_ConnectionChecker.Tick += new System.EventHandler(this.timer_ConnectionChecker_Tick);
            // 
            // timer_RunChecker
            // 
            this.timer_RunChecker.Enabled = true;
            this.timer_RunChecker.Tick += new System.EventHandler(this.timer_RunChecker_Tick);
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(201)))), ((int)(((byte)(202)))));
            this.ClientSize = new System.Drawing.Size(5674, 1895);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "Form_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Q730";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Main_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel9.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.ComboBox comboBox_SequenceList;
        private System.Windows.Forms.Button button_Apply;
        private System.Windows.Forms.Button button_Connect;
        private System.Windows.Forms.Button button_Configuration;
        private System.Windows.Forms.Button button_Stop;
        private System.Windows.Forms.Button button_Pause;
        private System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.Button button_SequenceBuilder;
        private System.Windows.Forms.Timer timer_ConnectionChecker;
        private System.Windows.Forms.Button button_NextStep;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Button button_Path;
        private System.Windows.Forms.TextBox textBox_Path;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private UserControls.UserControl_SequenceViewer userControl_SequenceViewer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.RadioButton radioButton_Graph;
        private System.Windows.Forms.RadioButton radioButton_Grid;
        private System.Windows.Forms.RadioButton radioButton_Detail;
        private System.Windows.Forms.RadioButton radioButton_List;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_Version;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TextBox textBox_Name;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Timer timer_RunChecker;
        private System.Windows.Forms.Label label_SdError;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private System.Windows.Forms.Button button_Custom_Load;
        private System.Windows.Forms.ListView listView_SequenceList;
        private System.Windows.Forms.TextBox textBox_Custom_Path;
        private System.Windows.Forms.TextBox textBox_SquencePath;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button button_Read;
    }
}

