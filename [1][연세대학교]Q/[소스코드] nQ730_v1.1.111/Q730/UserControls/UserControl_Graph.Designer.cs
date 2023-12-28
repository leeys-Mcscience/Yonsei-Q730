
namespace Q730.UserControls
{
    partial class UserControl_Graph
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_ZoomEnable = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_ShowPointValues = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_SetScaleToDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_SaveAsImage = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_ZoomEnable,
            this.toolStripMenuItem_ShowPointValues,
            this.toolStripMenuItem_SetScaleToDefault,
            this.toolStripMenuItem_SaveAsImage});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(182, 114);
            // 
            // toolStripMenuItem_ZoomEnable
            // 
            this.toolStripMenuItem_ZoomEnable.Checked = true;
            this.toolStripMenuItem_ZoomEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem_ZoomEnable.Name = "toolStripMenuItem_ZoomEnable";
            this.toolStripMenuItem_ZoomEnable.Size = new System.Drawing.Size(181, 22);
            this.toolStripMenuItem_ZoomEnable.Text = "Zoom Enable";
            this.toolStripMenuItem_ZoomEnable.Click += new System.EventHandler(this.toolStripMenuItem_ZoomEnable_Click);
            // 
            // toolStripMenuItem_ShowPointValues
            // 
            this.toolStripMenuItem_ShowPointValues.Name = "toolStripMenuItem_ShowPointValues";
            this.toolStripMenuItem_ShowPointValues.Size = new System.Drawing.Size(181, 22);
            this.toolStripMenuItem_ShowPointValues.Text = "Show Point Values";
            this.toolStripMenuItem_ShowPointValues.Click += new System.EventHandler(this.toolStripMenuItem_ShowPointValues_Click);
            // 
            // toolStripMenuItem_SetScaleToDefault
            // 
            this.toolStripMenuItem_SetScaleToDefault.Name = "toolStripMenuItem_SetScaleToDefault";
            this.toolStripMenuItem_SetScaleToDefault.Size = new System.Drawing.Size(181, 22);
            this.toolStripMenuItem_SetScaleToDefault.Text = "Set Scale to Default";
            this.toolStripMenuItem_SetScaleToDefault.Click += new System.EventHandler(this.toolStripMenuItem_SetScaleToDefault_Click);
            // 
            // toolStripMenuItem_SaveAsImage
            // 
            this.toolStripMenuItem_SaveAsImage.Name = "toolStripMenuItem_SaveAsImage";
            this.toolStripMenuItem_SaveAsImage.Size = new System.Drawing.Size(181, 22);
            this.toolStripMenuItem_SaveAsImage.Text = "Save as Image...";
            this.toolStripMenuItem_SaveAsImage.Click += new System.EventHandler(this.toolStripMenuItem_SaveAsImage_Click);
            // 
            // UserControl_Graph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UserControl_Graph";
            this.Size = new System.Drawing.Size(673, 328);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_ZoomEnable;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_ShowPointValues;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SetScaleToDefault;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SaveAsImage;
    }
}
