
namespace DataViewer
{
    partial class UserControl_ExportWindow
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.button_Separate = new System.Windows.Forms.Button();
            this.button_Total = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox_Time = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(23, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(405, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "데이터 추출 방법을 선택 해주세요.";
            // 
            // button_Separate
            // 
            this.button_Separate.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.button_Separate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Separate.Location = new System.Drawing.Point(61, 136);
            this.button_Separate.Name = "button_Separate";
            this.button_Separate.Size = new System.Drawing.Size(145, 71);
            this.button_Separate.TabIndex = 1;
            this.button_Separate.Text = "데이터 구분 추출";
            this.button_Separate.UseVisualStyleBackColor = false;
            this.button_Separate.Click += new System.EventHandler(this.button_Separate_Click);
            // 
            // button_Total
            // 
            this.button_Total.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.button_Total.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Total.Location = new System.Drawing.Point(240, 140);
            this.button_Total.Name = "button_Total";
            this.button_Total.Size = new System.Drawing.Size(144, 71);
            this.button_Total.TabIndex = 2;
            this.button_Total.Text = "데이터 일괄 추출";
            this.button_Total.UseVisualStyleBackColor = false;
            this.button_Total.Click += new System.EventHandler(this.button_Total_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "X축 : 사이클";
            // 
            // checkBox_Time
            // 
            this.checkBox_Time.AutoSize = true;
            this.checkBox_Time.Location = new System.Drawing.Point(22, 100);
            this.checkBox_Time.Name = "checkBox_Time";
            this.checkBox_Time.Size = new System.Drawing.Size(362, 19);
            this.checkBox_Time.TabIndex = 4;
            this.checkBox_Time.Text = "시간 누적 기능 : (x축이 Total Time인 경우만에만)";
            this.checkBox_Time.UseVisualStyleBackColor = true;
            this.checkBox_Time.CheckedChanged += new System.EventHandler(this.checkBox_Time_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(328, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Y축 : 충전용량, 방전용량, 쿨룽효율  = 일괄추출";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.button_Separate);
            this.groupBox1.Location = new System.Drawing.Point(27, 93);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 246);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "데이터 구분 추출";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.checkBox_Time);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.button_Total);
            this.groupBox2.Location = new System.Drawing.Point(293, 93);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(405, 249);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "데이터 일괄 추출";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(240, 19);
            this.label4.TabIndex = 4;
            this.label4.Text = "Step 별로 저장이 되는 기능";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(258, 19);
            this.label5.TabIndex = 5;
            this.label5.Text = "예) Charge / Rest / Discharge";
            // 
            // UserControl_ExportWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(712, 356);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "UserControl_ExportWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Data Export";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_Separate;
        private System.Windows.Forms.Button button_Total;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox_Time;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
    }
}
