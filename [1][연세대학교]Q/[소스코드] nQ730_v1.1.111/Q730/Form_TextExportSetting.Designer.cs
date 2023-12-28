
namespace Q730
{
    partial class Form_TextExportSetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_TextExportSetting));
            this.checkBox_Use = new System.Windows.Forms.CheckBox();
            this.button_Path = new System.Windows.Forms.Button();
            this.textBox_Path = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBox_Use
            // 
            this.checkBox_Use.AutoSize = true;
            this.checkBox_Use.Location = new System.Drawing.Point(12, 13);
            this.checkBox_Use.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox_Use.Name = "checkBox_Use";
            this.checkBox_Use.Size = new System.Drawing.Size(142, 19);
            this.checkBox_Use.TabIndex = 0;
            this.checkBox_Use.Text = "텍스트 내보내기 사용";
            this.checkBox_Use.UseVisualStyleBackColor = true;
            this.checkBox_Use.CheckedChanged += new System.EventHandler(this.checkBox_Use_CheckedChanged);
            // 
            // button_Path
            // 
            this.button_Path.Enabled = false;
            this.button_Path.Location = new System.Drawing.Point(364, 38);
            this.button_Path.Name = "button_Path";
            this.button_Path.Size = new System.Drawing.Size(25, 23);
            this.button_Path.TabIndex = 1;
            this.button_Path.Text = "...";
            this.button_Path.UseVisualStyleBackColor = true;
            this.button_Path.Click += new System.EventHandler(this.button_Path_Click);
            // 
            // textBox_Path
            // 
            this.textBox_Path.Location = new System.Drawing.Point(91, 39);
            this.textBox_Path.Name = "textBox_Path";
            this.textBox_Path.ReadOnly = true;
            this.textBox_Path.Size = new System.Drawing.Size(269, 23);
            this.textBox_Path.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "저장 경로 : ";
            // 
            // button_OK
            // 
            this.button_OK.Location = new System.Drawing.Point(276, 68);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(113, 23);
            this.button_OK.TabIndex = 21;
            this.button_OK.Text = "적용";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // Form_TextExportSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 109);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.button_Path);
            this.Controls.Add(this.textBox_Path);
            this.Controls.Add(this.checkBox_Use);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_TextExportSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "텍스트 내보내기";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_Use;
        private System.Windows.Forms.Button button_Path;
        private System.Windows.Forms.TextBox textBox_Path;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_OK;
    }
}