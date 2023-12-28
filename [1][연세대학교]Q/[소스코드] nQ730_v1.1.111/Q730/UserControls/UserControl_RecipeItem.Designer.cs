
namespace Q730.UserControls
{
    partial class UserControl_RecipeItem
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
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label_Content = new System.Windows.Forms.Label();
            this.pictureBox_RecipeIcon = new System.Windows.Forms.PictureBox();
            this.toolTip_Detail = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Manual = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RecipeIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 53F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.label_Content, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.pictureBox_RecipeIcon, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(235, 53);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // label_Content
            // 
            this.label_Content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Content.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Content.Location = new System.Drawing.Point(54, 1);
            this.label_Content.Margin = new System.Windows.Forms.Padding(1);
            this.label_Content.Name = "label_Content";
            this.label_Content.Size = new System.Drawing.Size(180, 51);
            this.label_Content.TabIndex = 2;
            this.label_Content.MouseHover += new System.EventHandler(this.label_Content_MouseHover);
            // 
            // pictureBox_RecipeIcon
            // 
            this.pictureBox_RecipeIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_RecipeIcon.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_RecipeIcon.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox_RecipeIcon.Name = "pictureBox_RecipeIcon";
            this.pictureBox_RecipeIcon.Size = new System.Drawing.Size(53, 53);
            this.pictureBox_RecipeIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_RecipeIcon.TabIndex = 1;
            this.pictureBox_RecipeIcon.TabStop = false;
            this.pictureBox_RecipeIcon.MouseHover += new System.EventHandler(this.pictureBox_RecipeIcon_MouseHover);
            // 
            // toolTip_Detail
            // 
            this.toolTip_Detail.AutoPopDelay = 32767;
            this.toolTip_Detail.BackColor = System.Drawing.Color.DimGray;
            this.toolTip_Detail.ForeColor = System.Drawing.Color.White;
            this.toolTip_Detail.InitialDelay = 800;
            this.toolTip_Detail.ReshowDelay = 100;
            // 
            // toolTip_Manual
            // 
            this.toolTip_Manual.AutoPopDelay = 32767;
            this.toolTip_Manual.InitialDelay = 2000;
            this.toolTip_Manual.ReshowDelay = 100;
            // 
            // UserControl_RecipeItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.Controls.Add(this.tableLayoutPanel3);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "UserControl_RecipeItem";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(237, 55);
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RecipeIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label_Content;
        private System.Windows.Forms.PictureBox pictureBox_RecipeIcon;
        private System.Windows.Forms.ToolTip toolTip_Detail;
        private System.Windows.Forms.ToolTip toolTip_Manual;
    }
}
