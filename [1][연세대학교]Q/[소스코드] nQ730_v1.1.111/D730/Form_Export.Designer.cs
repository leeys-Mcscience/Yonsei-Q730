
namespace DataViewer
{
    partial class Form_Export
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Export));
            this.label1 = new System.Windows.Forms.Label();
            this.button_Select = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Path = new System.Windows.Forms.TextBox();
            this.listBox_Selected = new System.Windows.Forms.ListBox();
            this.listBox_NonSelected = new System.Windows.Forms.ListBox();
            this.button_Up = new System.Windows.Forms.Button();
            this.button_Down = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_Seperator = new System.Windows.Forms.ComboBox();
            this.textBox_Separator = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button_Export = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_IsSeperatorSelf = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButton_FileSplit = new System.Windows.Forms.RadioButton();
            this.radioButton_NotFileSplit = new System.Windows.Forms.RadioButton();
            this.checkBox_RunAfterExport = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioButton_Hour = new System.Windows.Forms.RadioButton();
            this.radioButton_Sec = new System.Windows.Forms.RadioButton();
            this.checkBox_TimeSplit = new System.Windows.Forms.CheckBox();
            this.textBox_IntervalTime = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "선택됨";
            // 
            // button_Select
            // 
            this.button_Select.Location = new System.Drawing.Point(541, 35);
            this.button_Select.Name = "button_Select";
            this.button_Select.Size = new System.Drawing.Size(75, 23);
            this.button_Select.TabIndex = 2;
            this.button_Select.Text = "Select";
            this.button_Select.UseVisualStyleBackColor = true;
            this.button_Select.Click += new System.EventHandler(this.button_Select_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Output Path";
            // 
            // textBox_Path
            // 
            this.textBox_Path.Location = new System.Drawing.Point(92, 36);
            this.textBox_Path.Name = "textBox_Path";
            this.textBox_Path.ReadOnly = true;
            this.textBox_Path.Size = new System.Drawing.Size(443, 23);
            this.textBox_Path.TabIndex = 4;
            // 
            // listBox_Selected
            // 
            this.listBox_Selected.FormattingEnabled = true;
            this.listBox_Selected.ItemHeight = 15;
            this.listBox_Selected.Location = new System.Drawing.Point(8, 48);
            this.listBox_Selected.Name = "listBox_Selected";
            this.listBox_Selected.Size = new System.Drawing.Size(156, 199);
            this.listBox_Selected.TabIndex = 5;
            this.listBox_Selected.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox_Selected_MouseDoubleClick);
            // 
            // listBox_NonSelected
            // 
            this.listBox_NonSelected.FormattingEnabled = true;
            this.listBox_NonSelected.ItemHeight = 15;
            this.listBox_NonSelected.Location = new System.Drawing.Point(211, 48);
            this.listBox_NonSelected.Name = "listBox_NonSelected";
            this.listBox_NonSelected.Size = new System.Drawing.Size(151, 199);
            this.listBox_NonSelected.TabIndex = 6;
            this.listBox_NonSelected.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox_List_MouseDoubleClick);
            // 
            // button_Up
            // 
            this.button_Up.Location = new System.Drawing.Point(170, 113);
            this.button_Up.Name = "button_Up";
            this.button_Up.Size = new System.Drawing.Size(35, 35);
            this.button_Up.TabIndex = 7;
            this.button_Up.Text = "▲";
            this.button_Up.UseVisualStyleBackColor = true;
            this.button_Up.Click += new System.EventHandler(this.button_Up_Click);
            // 
            // button_Down
            // 
            this.button_Down.Location = new System.Drawing.Point(170, 154);
            this.button_Down.Name = "button_Down";
            this.button_Down.Size = new System.Drawing.Size(35, 35);
            this.button_Down.TabIndex = 8;
            this.button_Down.Text = "▼";
            this.button_Down.UseVisualStyleBackColor = true;
            this.button_Down.Click += new System.EventHandler(this.button_Down_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(127, 250);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "더블 클릭으로 추가 및 제거";
            // 
            // comboBox_Seperator
            // 
            this.comboBox_Seperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Seperator.FormattingEnabled = true;
            this.comboBox_Seperator.Items.AddRange(new object[] {
            "Comma(\',\')",
            "Tab(\'\\t\')",
            "Space(\' \')"});
            this.comboBox_Seperator.Location = new System.Drawing.Point(5, 20);
            this.comboBox_Seperator.Name = "comboBox_Seperator";
            this.comboBox_Seperator.Size = new System.Drawing.Size(134, 23);
            this.comboBox_Seperator.TabIndex = 11;
            // 
            // textBox_Separator
            // 
            this.textBox_Separator.Enabled = false;
            this.textBox_Separator.Location = new System.Drawing.Point(6, 74);
            this.textBox_Separator.Name = "textBox_Separator";
            this.textBox_Separator.Size = new System.Drawing.Size(134, 23);
            this.textBox_Separator.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(208, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 15);
            this.label5.TabIndex = 13;
            this.label5.Text = "선택 가능";
            // 
            // button_Export
            // 
            this.button_Export.Location = new System.Drawing.Point(627, 315);
            this.button_Export.Name = "button_Export";
            this.button_Export.Size = new System.Drawing.Size(131, 34);
            this.button_Export.TabIndex = 14;
            this.button_Export.Text = "Export";
            this.button_Export.UseVisualStyleBackColor = true;
            this.button_Export.Click += new System.EventHandler(this.button_Export_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_IsSeperatorSelf);
            this.groupBox1.Controls.Add(this.textBox_Separator);
            this.groupBox1.Controls.Add(this.comboBox_Seperator);
            this.groupBox1.Location = new System.Drawing.Point(396, 188);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(220, 104);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "구분자";
            // 
            // checkBox_IsSeperatorSelf
            // 
            this.checkBox_IsSeperatorSelf.AutoSize = true;
            this.checkBox_IsSeperatorSelf.Location = new System.Drawing.Point(6, 49);
            this.checkBox_IsSeperatorSelf.Name = "checkBox_IsSeperatorSelf";
            this.checkBox_IsSeperatorSelf.Size = new System.Drawing.Size(78, 19);
            this.checkBox_IsSeperatorSelf.TabIndex = 13;
            this.checkBox_IsSeperatorSelf.Text = "직접 입력";
            this.checkBox_IsSeperatorSelf.UseVisualStyleBackColor = true;
            this.checkBox_IsSeperatorSelf.CheckedChanged += new System.EventHandler(this.checkBox_IsSeperatorSelf_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.listBox_Selected);
            this.groupBox2.Controls.Add(this.listBox_NonSelected);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.button_Up);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.button_Down);
            this.groupBox2.Location = new System.Drawing.Point(12, 75);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(378, 271);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data Columns";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 15);
            this.label4.TabIndex = 17;
            this.label4.Text = "선택된 레시피 : 0개";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButton_FileSplit);
            this.groupBox3.Controls.Add(this.radioButton_NotFileSplit);
            this.groupBox3.Location = new System.Drawing.Point(396, 75);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(220, 107);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "파일 분리 방법";
            // 
            // radioButton_FileSplit
            // 
            this.radioButton_FileSplit.Location = new System.Drawing.Point(14, 48);
            this.radioButton_FileSplit.Name = "radioButton_FileSplit";
            this.radioButton_FileSplit.Size = new System.Drawing.Size(197, 44);
            this.radioButton_FileSplit.TabIndex = 1;
            this.radioButton_FileSplit.Text = "레시피 별로 분리 후 파일 이름에 번호 붙이기";
            this.radioButton_FileSplit.UseVisualStyleBackColor = true;
            // 
            // radioButton_NotFileSplit
            // 
            this.radioButton_NotFileSplit.AutoSize = true;
            this.radioButton_NotFileSplit.Checked = true;
            this.radioButton_NotFileSplit.Location = new System.Drawing.Point(14, 22);
            this.radioButton_NotFileSplit.Name = "radioButton_NotFileSplit";
            this.radioButton_NotFileSplit.Size = new System.Drawing.Size(197, 19);
            this.radioButton_NotFileSplit.TabIndex = 0;
            this.radioButton_NotFileSplit.TabStop = true;
            this.radioButton_NotFileSplit.Text = "분리 안 함(한 파일에 모두 추가)";
            this.radioButton_NotFileSplit.UseVisualStyleBackColor = true;
            this.radioButton_NotFileSplit.CheckedChanged += new System.EventHandler(this.radioButtons_CheckedChanged);
            // 
            // checkBox_RunAfterExport
            // 
            this.checkBox_RunAfterExport.AutoSize = true;
            this.checkBox_RunAfterExport.Location = new System.Drawing.Point(612, 295);
            this.checkBox_RunAfterExport.Name = "checkBox_RunAfterExport";
            this.checkBox_RunAfterExport.Size = new System.Drawing.Size(146, 19);
            this.checkBox_RunAfterExport.TabIndex = 19;
            this.checkBox_RunAfterExport.Text = "내보내기 후 파일 실행";
            this.checkBox_RunAfterExport.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.textBox_IntervalTime);
            this.groupBox4.Controls.Add(this.checkBox_TimeSplit);
            this.groupBox4.Location = new System.Drawing.Point(627, 75);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(129, 107);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "시간 단위 분할 지정";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioButton_Sec);
            this.groupBox5.Controls.Add(this.radioButton_Hour);
            this.groupBox5.Location = new System.Drawing.Point(627, 188);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(129, 107);
            this.groupBox5.TabIndex = 21;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Time Format";
            // 
            // radioButton_Hour
            // 
            this.radioButton_Hour.AutoSize = true;
            this.radioButton_Hour.Location = new System.Drawing.Point(19, 24);
            this.radioButton_Hour.Name = "radioButton_Hour";
            this.radioButton_Hour.Size = new System.Drawing.Size(67, 19);
            this.radioButton_Hour.TabIndex = 0;
            this.radioButton_Hour.TabStop = true;
            this.radioButton_Hour.Text = "Hour(h)";
            this.radioButton_Hour.UseVisualStyleBackColor = true;
            // 
            // radioButton_Sec
            // 
            this.radioButton_Sec.AutoSize = true;
            this.radioButton_Sec.Location = new System.Drawing.Point(19, 57);
            this.radioButton_Sec.Name = "radioButton_Sec";
            this.radioButton_Sec.Size = new System.Drawing.Size(57, 19);
            this.radioButton_Sec.TabIndex = 0;
            this.radioButton_Sec.TabStop = true;
            this.radioButton_Sec.Text = "Sec(s)";
            this.radioButton_Sec.UseVisualStyleBackColor = true;
            // 
            // checkBox_TimeSplit
            // 
            this.checkBox_TimeSplit.AutoSize = true;
            this.checkBox_TimeSplit.Location = new System.Drawing.Point(11, 28);
            this.checkBox_TimeSplit.Name = "checkBox_TimeSplit";
            this.checkBox_TimeSplit.Size = new System.Drawing.Size(78, 19);
            this.checkBox_TimeSplit.TabIndex = 0;
            this.checkBox_TimeSplit.Text = "시간 분할";
            this.checkBox_TimeSplit.UseVisualStyleBackColor = true;
            this.checkBox_TimeSplit.CheckedChanged += new System.EventHandler(this.checkBox_TimeSplit_CheckedChanged);
            // 
            // textBox_IntervalTime
            // 
            this.textBox_IntervalTime.Enabled = false;
            this.textBox_IntervalTime.Location = new System.Drawing.Point(11, 58);
            this.textBox_IntervalTime.Name = "textBox_IntervalTime";
            this.textBox_IntervalTime.Size = new System.Drawing.Size(75, 23);
            this.textBox_IntervalTime.TabIndex = 1;
            this.textBox_IntervalTime.Text = "1";
            this.textBox_IntervalTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(90, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "Sec";
            // 
            // Form_Export
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 358);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.checkBox_RunAfterExport);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_Export);
            this.Controls.Add(this.textBox_Path);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_Select);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_Export";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Q730 - Export Data";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_Select;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Path;
        private System.Windows.Forms.ListBox listBox_Selected;
        private System.Windows.Forms.ListBox listBox_NonSelected;
        private System.Windows.Forms.Button button_Up;
        private System.Windows.Forms.Button button_Down;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_Seperator;
        private System.Windows.Forms.TextBox textBox_Separator;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_Export;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_IsSeperatorSelf;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButton_FileSplit;
        private System.Windows.Forms.RadioButton radioButton_NotFileSplit;
        private System.Windows.Forms.CheckBox checkBox_RunAfterExport;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_IntervalTime;
        private System.Windows.Forms.CheckBox checkBox_TimeSplit;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radioButton_Sec;
        private System.Windows.Forms.RadioButton radioButton_Hour;
    }
}