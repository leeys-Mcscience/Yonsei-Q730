
namespace Q730.UserControls.Graphs
{
    partial class UserControl_TraGraph
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.userControl_Graph1 = new Q730.UserControls.UserControl_Graph();
            this.SuspendLayout();
            // 
            // userControl_Graph1
            // 
            this.userControl_Graph1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControl_Graph1.Location = new System.Drawing.Point(0, 0);
            this.userControl_Graph1.Margin = new System.Windows.Forms.Padding(0);
            this.userControl_Graph1.Name = "userControl_Graph1";
            this.userControl_Graph1.Size = new System.Drawing.Size(981, 467);
            this.userControl_Graph1.TabIndex = 0;
            // 
            // UserControl_TraGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.userControl_Graph1);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "UserControl_TraGraph";
            this.Size = new System.Drawing.Size(981, 467);
            this.ResumeLayout(false);

        }

        #endregion

        private UserControl_Graph userControl_Graph1;
    }
}
