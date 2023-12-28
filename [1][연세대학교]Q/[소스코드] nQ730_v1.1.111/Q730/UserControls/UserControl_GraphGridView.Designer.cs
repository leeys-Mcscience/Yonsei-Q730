
namespace Q730.UserControls
{
    partial class UserControl_GraphGridView
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
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.numericUpDown_Row = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_Column = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.button_Apply = new System.Windows.Forms.Button();
            this.tableLayoutPanel_Grid = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Row)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Column)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel_Grid, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1128, 608);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 5;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel7.Controls.Add(this.numericUpDown_Row, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.numericUpDown_Column, 3, 0);
            this.tableLayoutPanel7.Controls.Add(this.label1, 2, 0);
            this.tableLayoutPanel7.Controls.Add(this.button_Apply, 4, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(1128, 30);
            this.tableLayoutPanel7.TabIndex = 1;
            // 
            // numericUpDown_Row
            // 
            this.numericUpDown_Row.Enabled = false;
            this.numericUpDown_Row.Location = new System.Drawing.Point(931, 4);
            this.numericUpDown_Row.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numericUpDown_Row.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDown_Row.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Row.Name = "numericUpDown_Row";
            this.numericUpDown_Row.Size = new System.Drawing.Size(54, 23);
            this.numericUpDown_Row.TabIndex = 0;
            this.numericUpDown_Row.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDown_Column
            // 
            this.numericUpDown_Column.Enabled = false;
            this.numericUpDown_Column.Location = new System.Drawing.Point(1011, 4);
            this.numericUpDown_Column.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numericUpDown_Column.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDown_Column.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Column.Name = "numericUpDown_Column";
            this.numericUpDown_Column.Size = new System.Drawing.Size(54, 23);
            this.numericUpDown_Column.TabIndex = 1;
            this.numericUpDown_Column.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(991, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "x";
            // 
            // button_Apply
            // 
            this.button_Apply.Enabled = false;
            this.button_Apply.Location = new System.Drawing.Point(1071, 4);
            this.button_Apply.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_Apply.Name = "button_Apply";
            this.button_Apply.Size = new System.Drawing.Size(54, 22);
            this.button_Apply.TabIndex = 3;
            this.button_Apply.Text = "Apply";
            this.button_Apply.UseVisualStyleBackColor = true;
            this.button_Apply.Click += new System.EventHandler(this.button_Apply_Click);
            // 
            // tableLayoutPanel_Grid
            // 
            this.tableLayoutPanel_Grid.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_Grid.ColumnCount = 1;
            this.tableLayoutPanel_Grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Grid.Location = new System.Drawing.Point(1, 31);
            this.tableLayoutPanel_Grid.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel_Grid.Name = "tableLayoutPanel_Grid";
            this.tableLayoutPanel_Grid.RowCount = 1;
            this.tableLayoutPanel_Grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Grid.Size = new System.Drawing.Size(1126, 576);
            this.tableLayoutPanel_Grid.TabIndex = 2;
            // 
            // UserControl_GraphGridView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "UserControl_GraphGridView";
            this.Size = new System.Drawing.Size(1128, 608);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Row)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Column)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.NumericUpDown numericUpDown_Row;
        private System.Windows.Forms.NumericUpDown numericUpDown_Column;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_Apply;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Grid;
    }
}
