
namespace Q730.UserControls
{
    partial class UserControl_ChannelDetailView
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label_State = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.label_CustomValue1 = new System.Windows.Forms.Label();
            this.label_CustomTitle1 = new System.Windows.Forms.Label();
            this.label_CustomValue4 = new System.Windows.Forms.Label();
            this.label_CustomTitle4 = new System.Windows.Forms.Label();
            this.label_CustomValue3 = new System.Windows.Forms.Label();
            this.button_After = new System.Windows.Forms.Button();
            this.label_Time = new System.Windows.Forms.Label();
            this.label_CustomTitle3 = new System.Windows.Forms.Label();
            this.label_CustomValue2 = new System.Windows.Forms.Label();
            this.label_CustomTitle2 = new System.Windows.Forms.Label();
            this.button_Before = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel_GraphSpace = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.radioButton_charge = new System.Windows.Forms.RadioButton();
            this.radioButton_Pattern = new System.Windows.Forms.RadioButton();
            this.radioButton_Acr = new System.Windows.Forms.RadioButton();
            this.radioButton_Dcr = new System.Windows.Forms.RadioButton();
            this.radioButton_Tra = new System.Windows.Forms.RadioButton();
            this.radioButton_Fra = new System.Windows.Forms.RadioButton();
            this.radioButton_Dc = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel_GraphSpace.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel_GraphSpace, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1089, 624);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(52)))), ((int)(((byte)(52)))));
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label_State, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.progressBar1, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(1, 598);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1087, 25);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // label_State
            // 
            this.label_State.AutoSize = true;
            this.label_State.BackColor = System.Drawing.Color.White;
            this.label_State.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_State.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_State.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_State.Location = new System.Drawing.Point(2, 2);
            this.label_State.Margin = new System.Windows.Forms.Padding(2);
            this.label_State.Name = "label_State";
            this.label_State.Size = new System.Drawing.Size(96, 21);
            this.label_State.TabIndex = 2;
            this.label_State.Text = "-";
            this.label_State.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(102, 2);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(196, 21);
            this.progressBar1.TabIndex = 3;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(52)))), ((int)(((byte)(52)))));
            this.tableLayoutPanel7.ColumnCount = 13;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel7.Controls.Add(this.label_CustomValue1, 6, 0);
            this.tableLayoutPanel7.Controls.Add(this.label_CustomTitle1, 5, 0);
            this.tableLayoutPanel7.Controls.Add(this.label_CustomValue4, 12, 0);
            this.tableLayoutPanel7.Controls.Add(this.label_CustomTitle4, 11, 0);
            this.tableLayoutPanel7.Controls.Add(this.label_CustomValue3, 10, 0);
            this.tableLayoutPanel7.Controls.Add(this.button_After, 3, 0);
            this.tableLayoutPanel7.Controls.Add(this.label_Time, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.label_CustomTitle3, 9, 0);
            this.tableLayoutPanel7.Controls.Add(this.label_CustomValue2, 8, 0);
            this.tableLayoutPanel7.Controls.Add(this.label_CustomTitle2, 7, 0);
            this.tableLayoutPanel7.Controls.Add(this.button_Before, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.comboBox1, 2, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(1087, 25);
            this.tableLayoutPanel7.TabIndex = 2;
            // 
            // label_CustomValue1
            // 
            this.label_CustomValue1.AutoSize = true;
            this.label_CustomValue1.BackColor = System.Drawing.Color.White;
            this.label_CustomValue1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_CustomValue1.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_CustomValue1.ForeColor = System.Drawing.Color.Black;
            this.label_CustomValue1.Location = new System.Drawing.Point(389, 2);
            this.label_CustomValue1.Margin = new System.Windows.Forms.Padding(2);
            this.label_CustomValue1.Name = "label_CustomValue1";
            this.label_CustomValue1.Size = new System.Drawing.Size(96, 21);
            this.label_CustomValue1.TabIndex = 8;
            this.label_CustomValue1.Text = "-";
            this.label_CustomValue1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_CustomTitle1
            // 
            this.label_CustomTitle1.AutoSize = true;
            this.label_CustomTitle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(159)))), ((int)(((byte)(159)))));
            this.label_CustomTitle1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_CustomTitle1.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_CustomTitle1.ForeColor = System.Drawing.Color.White;
            this.label_CustomTitle1.Location = new System.Drawing.Point(289, 2);
            this.label_CustomTitle1.Margin = new System.Windows.Forms.Padding(2);
            this.label_CustomTitle1.Name = "label_CustomTitle1";
            this.label_CustomTitle1.Size = new System.Drawing.Size(96, 21);
            this.label_CustomTitle1.TabIndex = 7;
            this.label_CustomTitle1.Tag = "1";
            this.label_CustomTitle1.Text = "-";
            this.label_CustomTitle1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_CustomTitle1.Click += new System.EventHandler(this.label_Custom_Click);
            // 
            // label_CustomValue4
            // 
            this.label_CustomValue4.AutoSize = true;
            this.label_CustomValue4.BackColor = System.Drawing.Color.White;
            this.label_CustomValue4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_CustomValue4.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_CustomValue4.ForeColor = System.Drawing.Color.Black;
            this.label_CustomValue4.Location = new System.Drawing.Point(989, 2);
            this.label_CustomValue4.Margin = new System.Windows.Forms.Padding(2);
            this.label_CustomValue4.Name = "label_CustomValue4";
            this.label_CustomValue4.Size = new System.Drawing.Size(96, 21);
            this.label_CustomValue4.TabIndex = 5;
            this.label_CustomValue4.Text = "-";
            this.label_CustomValue4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_CustomTitle4
            // 
            this.label_CustomTitle4.AutoSize = true;
            this.label_CustomTitle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(159)))), ((int)(((byte)(159)))));
            this.label_CustomTitle4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_CustomTitle4.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_CustomTitle4.ForeColor = System.Drawing.Color.White;
            this.label_CustomTitle4.Location = new System.Drawing.Point(889, 2);
            this.label_CustomTitle4.Margin = new System.Windows.Forms.Padding(2);
            this.label_CustomTitle4.Name = "label_CustomTitle4";
            this.label_CustomTitle4.Size = new System.Drawing.Size(96, 21);
            this.label_CustomTitle4.TabIndex = 2;
            this.label_CustomTitle4.Tag = "4";
            this.label_CustomTitle4.Text = "-";
            this.label_CustomTitle4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_CustomTitle4.Click += new System.EventHandler(this.label_Custom_Click);
            // 
            // label_CustomValue3
            // 
            this.label_CustomValue3.AutoSize = true;
            this.label_CustomValue3.BackColor = System.Drawing.Color.White;
            this.label_CustomValue3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_CustomValue3.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_CustomValue3.ForeColor = System.Drawing.Color.Black;
            this.label_CustomValue3.Location = new System.Drawing.Point(789, 2);
            this.label_CustomValue3.Margin = new System.Windows.Forms.Padding(2);
            this.label_CustomValue3.Name = "label_CustomValue3";
            this.label_CustomValue3.Size = new System.Drawing.Size(96, 21);
            this.label_CustomValue3.TabIndex = 4;
            this.label_CustomValue3.Text = "-";
            this.label_CustomValue3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_After
            // 
            this.button_After.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(159)))), ((int)(((byte)(159)))));
            this.button_After.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_After.Enabled = false;
            this.button_After.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(118)))), ((int)(((byte)(118)))));
            this.button_After.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_After.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_After.ForeColor = System.Drawing.Color.White;
            this.button_After.Location = new System.Drawing.Point(176, 2);
            this.button_After.Margin = new System.Windows.Forms.Padding(1, 2, 2, 2);
            this.button_After.Name = "button_After";
            this.button_After.Size = new System.Drawing.Size(22, 21);
            this.button_After.TabIndex = 0;
            this.button_After.TabStop = false;
            this.button_After.Text = "▶";
            this.button_After.UseVisualStyleBackColor = false;
            this.button_After.Click += new System.EventHandler(this.button_After_Click);
            // 
            // label_Time
            // 
            this.label_Time.AutoSize = true;
            this.label_Time.BackColor = System.Drawing.Color.White;
            this.label_Time.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_Time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Time.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_Time.Location = new System.Drawing.Point(2, 2);
            this.label_Time.Margin = new System.Windows.Forms.Padding(2);
            this.label_Time.Name = "label_Time";
            this.label_Time.Size = new System.Drawing.Size(96, 21);
            this.label_Time.TabIndex = 2;
            this.label_Time.Text = "00:00:00";
            this.label_Time.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_CustomTitle3
            // 
            this.label_CustomTitle3.AutoSize = true;
            this.label_CustomTitle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(159)))), ((int)(((byte)(159)))));
            this.label_CustomTitle3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_CustomTitle3.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_CustomTitle3.ForeColor = System.Drawing.Color.White;
            this.label_CustomTitle3.Location = new System.Drawing.Point(689, 2);
            this.label_CustomTitle3.Margin = new System.Windows.Forms.Padding(2);
            this.label_CustomTitle3.Name = "label_CustomTitle3";
            this.label_CustomTitle3.Size = new System.Drawing.Size(96, 21);
            this.label_CustomTitle3.TabIndex = 1;
            this.label_CustomTitle3.Tag = "3";
            this.label_CustomTitle3.Text = "-";
            this.label_CustomTitle3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_CustomTitle3.Click += new System.EventHandler(this.label_Custom_Click);
            // 
            // label_CustomValue2
            // 
            this.label_CustomValue2.AutoSize = true;
            this.label_CustomValue2.BackColor = System.Drawing.Color.White;
            this.label_CustomValue2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_CustomValue2.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_CustomValue2.ForeColor = System.Drawing.Color.Black;
            this.label_CustomValue2.Location = new System.Drawing.Point(589, 2);
            this.label_CustomValue2.Margin = new System.Windows.Forms.Padding(2);
            this.label_CustomValue2.Name = "label_CustomValue2";
            this.label_CustomValue2.Size = new System.Drawing.Size(96, 21);
            this.label_CustomValue2.TabIndex = 3;
            this.label_CustomValue2.Text = "-";
            this.label_CustomValue2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_CustomTitle2
            // 
            this.label_CustomTitle2.AutoSize = true;
            this.label_CustomTitle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(159)))), ((int)(((byte)(159)))));
            this.label_CustomTitle2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_CustomTitle2.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_CustomTitle2.ForeColor = System.Drawing.Color.White;
            this.label_CustomTitle2.Location = new System.Drawing.Point(489, 2);
            this.label_CustomTitle2.Margin = new System.Windows.Forms.Padding(2);
            this.label_CustomTitle2.Name = "label_CustomTitle2";
            this.label_CustomTitle2.Size = new System.Drawing.Size(96, 21);
            this.label_CustomTitle2.TabIndex = 0;
            this.label_CustomTitle2.Tag = "2";
            this.label_CustomTitle2.Text = "-";
            this.label_CustomTitle2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_CustomTitle2.Click += new System.EventHandler(this.label_Custom_Click);
            // 
            // button_Before
            // 
            this.button_Before.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(159)))), ((int)(((byte)(159)))));
            this.button_Before.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Before.Enabled = false;
            this.button_Before.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(118)))), ((int)(((byte)(118)))));
            this.button_Before.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Before.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Before.ForeColor = System.Drawing.Color.White;
            this.button_Before.Location = new System.Drawing.Point(102, 2);
            this.button_Before.Margin = new System.Windows.Forms.Padding(2, 2, 1, 2);
            this.button_Before.Name = "button_Before";
            this.button_Before.Size = new System.Drawing.Size(22, 21);
            this.button_Before.TabIndex = 1;
            this.button_Before.TabStop = false;
            this.button_Before.Text = "◀";
            this.button_Before.UseVisualStyleBackColor = false;
            this.button_Before.Click += new System.EventHandler(this.button_Before_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(125, 0);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(0);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(50, 27);
            this.comboBox1.TabIndex = 6;
            this.comboBox1.TabStop = false;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_ValueChanged);
            // 
            // tableLayoutPanel_GraphSpace
            // 
            this.tableLayoutPanel_GraphSpace.ColumnCount = 1;
            this.tableLayoutPanel_GraphSpace.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_GraphSpace.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel_GraphSpace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_GraphSpace.Location = new System.Drawing.Point(1, 27);
            this.tableLayoutPanel_GraphSpace.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_GraphSpace.Name = "tableLayoutPanel_GraphSpace";
            this.tableLayoutPanel_GraphSpace.RowCount = 3;
            this.tableLayoutPanel_GraphSpace.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel_GraphSpace.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_GraphSpace.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_GraphSpace.Size = new System.Drawing.Size(1087, 570);
            this.tableLayoutPanel_GraphSpace.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 8;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.radioButton_charge, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.radioButton_Pattern, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.radioButton_Acr, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.radioButton_Dcr, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.radioButton_Tra, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.radioButton_Fra, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.radioButton_Dc, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1087, 25);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // radioButton_charge
            // 
            this.radioButton_charge.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_charge.AutoSize = true;
            this.radioButton_charge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.radioButton_charge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButton_charge.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(176)))), ((int)(((byte)(176)))));
            this.radioButton_charge.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.radioButton_charge.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton_charge.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_charge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(201)))), ((int)(((byte)(202)))));
            this.radioButton_charge.Location = new System.Drawing.Point(81, 2);
            this.radioButton_charge.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButton_charge.Name = "radioButton_charge";
            this.radioButton_charge.Size = new System.Drawing.Size(78, 21);
            this.radioButton_charge.TabIndex = 8;
            this.radioButton_charge.Text = "Charge";
            this.radioButton_charge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_charge.UseVisualStyleBackColor = false;
            this.radioButton_charge.Click += new System.EventHandler(this.radioButtons_Click);
            // 
            // radioButton_Pattern
            // 
            this.radioButton_Pattern.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_Pattern.AutoSize = true;
            this.radioButton_Pattern.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.radioButton_Pattern.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButton_Pattern.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(176)))), ((int)(((byte)(176)))));
            this.radioButton_Pattern.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.radioButton_Pattern.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton_Pattern.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_Pattern.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(201)))), ((int)(((byte)(202)))));
            this.radioButton_Pattern.Location = new System.Drawing.Point(161, 2);
            this.radioButton_Pattern.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButton_Pattern.Name = "radioButton_Pattern";
            this.radioButton_Pattern.Size = new System.Drawing.Size(78, 21);
            this.radioButton_Pattern.TabIndex = 7;
            this.radioButton_Pattern.Text = "Pattern";
            this.radioButton_Pattern.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_Pattern.UseVisualStyleBackColor = false;
            this.radioButton_Pattern.Visible = false;
            this.radioButton_Pattern.Click += new System.EventHandler(this.radioButtons_Click);
            // 
            // radioButton_Acr
            // 
            this.radioButton_Acr.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_Acr.AutoSize = true;
            this.radioButton_Acr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.radioButton_Acr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButton_Acr.Enabled = false;
            this.radioButton_Acr.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(176)))), ((int)(((byte)(176)))));
            this.radioButton_Acr.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.radioButton_Acr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton_Acr.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_Acr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(201)))), ((int)(((byte)(202)))));
            this.radioButton_Acr.Location = new System.Drawing.Point(481, 2);
            this.radioButton_Acr.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButton_Acr.Name = "radioButton_Acr";
            this.radioButton_Acr.Size = new System.Drawing.Size(78, 21);
            this.radioButton_Acr.TabIndex = 6;
            this.radioButton_Acr.Text = "ACR";
            this.radioButton_Acr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_Acr.UseVisualStyleBackColor = false;
            this.radioButton_Acr.Visible = false;
            this.radioButton_Acr.Click += new System.EventHandler(this.radioButtons_Click);
            // 
            // radioButton_Dcr
            // 
            this.radioButton_Dcr.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_Dcr.AutoSize = true;
            this.radioButton_Dcr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.radioButton_Dcr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButton_Dcr.Enabled = false;
            this.radioButton_Dcr.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(176)))), ((int)(((byte)(176)))));
            this.radioButton_Dcr.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.radioButton_Dcr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton_Dcr.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_Dcr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(201)))), ((int)(((byte)(202)))));
            this.radioButton_Dcr.Location = new System.Drawing.Point(401, 2);
            this.radioButton_Dcr.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButton_Dcr.Name = "radioButton_Dcr";
            this.radioButton_Dcr.Size = new System.Drawing.Size(78, 21);
            this.radioButton_Dcr.TabIndex = 5;
            this.radioButton_Dcr.Text = "DCR";
            this.radioButton_Dcr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_Dcr.UseVisualStyleBackColor = false;
            this.radioButton_Dcr.Visible = false;
            this.radioButton_Dcr.Click += new System.EventHandler(this.radioButtons_Click);
            // 
            // radioButton_Tra
            // 
            this.radioButton_Tra.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_Tra.AutoSize = true;
            this.radioButton_Tra.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.radioButton_Tra.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButton_Tra.Enabled = false;
            this.radioButton_Tra.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(176)))), ((int)(((byte)(176)))));
            this.radioButton_Tra.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.radioButton_Tra.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton_Tra.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_Tra.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(201)))), ((int)(((byte)(202)))));
            this.radioButton_Tra.Location = new System.Drawing.Point(321, 2);
            this.radioButton_Tra.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButton_Tra.Name = "radioButton_Tra";
            this.radioButton_Tra.Size = new System.Drawing.Size(78, 21);
            this.radioButton_Tra.TabIndex = 4;
            this.radioButton_Tra.Text = "TRA";
            this.radioButton_Tra.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_Tra.UseVisualStyleBackColor = false;
            this.radioButton_Tra.Visible = false;
            this.radioButton_Tra.Click += new System.EventHandler(this.radioButtons_Click);
            // 
            // radioButton_Fra
            // 
            this.radioButton_Fra.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_Fra.AutoSize = true;
            this.radioButton_Fra.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.radioButton_Fra.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButton_Fra.Enabled = false;
            this.radioButton_Fra.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(176)))), ((int)(((byte)(176)))));
            this.radioButton_Fra.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.radioButton_Fra.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton_Fra.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_Fra.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(201)))), ((int)(((byte)(202)))));
            this.radioButton_Fra.Location = new System.Drawing.Point(241, 2);
            this.radioButton_Fra.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButton_Fra.Name = "radioButton_Fra";
            this.radioButton_Fra.Size = new System.Drawing.Size(78, 21);
            this.radioButton_Fra.TabIndex = 3;
            this.radioButton_Fra.Text = "FRA";
            this.radioButton_Fra.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_Fra.UseVisualStyleBackColor = false;
            this.radioButton_Fra.Visible = false;
            this.radioButton_Fra.Click += new System.EventHandler(this.radioButtons_Click);
            // 
            // radioButton_Dc
            // 
            this.radioButton_Dc.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_Dc.AutoSize = true;
            this.radioButton_Dc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(137)))), ((int)(((byte)(137)))));
            this.radioButton_Dc.Checked = true;
            this.radioButton_Dc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButton_Dc.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(176)))), ((int)(((byte)(176)))));
            this.radioButton_Dc.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.radioButton_Dc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton_Dc.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioButton_Dc.ForeColor = System.Drawing.Color.Black;
            this.radioButton_Dc.Location = new System.Drawing.Point(1, 2);
            this.radioButton_Dc.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.radioButton_Dc.Name = "radioButton_Dc";
            this.radioButton_Dc.Size = new System.Drawing.Size(78, 21);
            this.radioButton_Dc.TabIndex = 1;
            this.radioButton_Dc.TabStop = true;
            this.radioButton_Dc.Text = "DC";
            this.radioButton_Dc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton_Dc.UseVisualStyleBackColor = false;
            this.radioButton_Dc.Click += new System.EventHandler(this.radioButtons_Click);
            // 
            // UserControl_ChannelDetailView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UserControl_ChannelDetailView";
            this.Size = new System.Drawing.Size(1089, 624);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel_GraphSpace.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button_Before;
        private System.Windows.Forms.Button button_After;
        private System.Windows.Forms.Label label_CustomTitle4;
        private System.Windows.Forms.Label label_CustomTitle3;
        private System.Windows.Forms.Label label_CustomTitle2;
        private System.Windows.Forms.Label label_CustomValue4;
        private System.Windows.Forms.Label label_CustomValue3;
        private System.Windows.Forms.Label label_CustomValue2;
        private System.Windows.Forms.Label label_Time;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_GraphSpace;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.RadioButton radioButton_Dc;
        private System.Windows.Forms.RadioButton radioButton_Pattern;
        private System.Windows.Forms.RadioButton radioButton_Acr;
        private System.Windows.Forms.RadioButton radioButton_Dcr;
        private System.Windows.Forms.RadioButton radioButton_Tra;
        private System.Windows.Forms.RadioButton radioButton_Fra;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label_State;
        private System.Windows.Forms.Label label_CustomValue1;
        private System.Windows.Forms.Label label_CustomTitle1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RadioButton radioButton_charge;
    }
}
