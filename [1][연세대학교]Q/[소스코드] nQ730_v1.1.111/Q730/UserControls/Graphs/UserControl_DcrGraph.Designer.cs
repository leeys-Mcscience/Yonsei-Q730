
namespace Q730.UserControls.Graphs
{
    partial class UserControl_DcrGraph
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.graph_VC = new Q730.UserControls.UserControl_Graph();
            this.graph_Dcr = new Q730.UserControls.UserControl_Graph();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.graph_VC, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.graph_Dcr, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(981, 467);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // graph_VC
            // 
            this.graph_VC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graph_VC.Location = new System.Drawing.Point(0, 0);
            this.graph_VC.Margin = new System.Windows.Forms.Padding(0);
            this.graph_VC.Name = "graph_VC";
            this.graph_VC.Size = new System.Drawing.Size(490, 467);
            this.graph_VC.TabIndex = 0;
            // 
            // graph_Dcr
            // 
            this.graph_Dcr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graph_Dcr.Location = new System.Drawing.Point(490, 0);
            this.graph_Dcr.Margin = new System.Windows.Forms.Padding(0);
            this.graph_Dcr.Name = "graph_Dcr";
            this.graph_Dcr.Size = new System.Drawing.Size(491, 467);
            this.graph_Dcr.TabIndex = 1;
            // 
            // UserControl_DcrGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "UserControl_DcrGraph";
            this.Size = new System.Drawing.Size(981, 467);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private UserControl_Graph graph_VC;
        private UserControl_Graph graph_Dcr;
    }
}
