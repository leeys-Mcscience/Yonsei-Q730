
namespace McQLib.GUI
{
    partial class Form_QueueMonitor
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
            if( disposing && (components != null) )
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label_Send = new System.Windows.Forms.Label();
            this.label_Byte = new System.Windows.Forms.Label();
            this.label_ReceivePacket = new System.Windows.Forms.Label();
            this.label_Error = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.label_ReceiveWaiter = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 139F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label_ReceiveWaiter, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label_Error, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label_ReceivePacket, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label_Byte, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label_Send, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(290, 154);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(1, 1);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Send Queue";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(1, 61);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 29);
            this.label2.TabIndex = 1;
            this.label2.Text = "Received Byte Queue";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(1, 91);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 29);
            this.label3.TabIndex = 1;
            this.label3.Text = "Received Packet Queue";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(1, 121);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 32);
            this.label4.TabIndex = 2;
            this.label4.Text = "Error Queue";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_Send
            // 
            this.label_Send.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Send.Location = new System.Drawing.Point(141, 1);
            this.label_Send.Margin = new System.Windows.Forms.Padding(0);
            this.label_Send.Name = "label_Send";
            this.label_Send.Size = new System.Drawing.Size(148, 29);
            this.label_Send.TabIndex = 3;
            this.label_Send.Text = "0";
            this.label_Send.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_Byte
            // 
            this.label_Byte.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Byte.Location = new System.Drawing.Point(141, 61);
            this.label_Byte.Margin = new System.Windows.Forms.Padding(0);
            this.label_Byte.Name = "label_Byte";
            this.label_Byte.Size = new System.Drawing.Size(148, 29);
            this.label_Byte.TabIndex = 4;
            this.label_Byte.Text = "0";
            this.label_Byte.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_ReceivePacket
            // 
            this.label_ReceivePacket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_ReceivePacket.Location = new System.Drawing.Point(141, 91);
            this.label_ReceivePacket.Margin = new System.Windows.Forms.Padding(0);
            this.label_ReceivePacket.Name = "label_ReceivePacket";
            this.label_ReceivePacket.Size = new System.Drawing.Size(148, 29);
            this.label_ReceivePacket.TabIndex = 5;
            this.label_ReceivePacket.Text = "0";
            this.label_ReceivePacket.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_Error
            // 
            this.label_Error.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Error.Location = new System.Drawing.Point(141, 121);
            this.label_Error.Margin = new System.Windows.Forms.Padding(0);
            this.label_Error.Name = "label_Error";
            this.label_Error.Size = new System.Drawing.Size(148, 32);
            this.label_Error.TabIndex = 6;
            this.label_Error.Text = "0";
            this.label_Error.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label9
            // 
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Location = new System.Drawing.Point(1, 31);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(139, 29);
            this.label9.TabIndex = 7;
            this.label9.Text = "Receive Waiter";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_ReceiveWaiter
            // 
            this.label_ReceiveWaiter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_ReceiveWaiter.Location = new System.Drawing.Point(141, 31);
            this.label_ReceiveWaiter.Margin = new System.Windows.Forms.Padding(0);
            this.label_ReceiveWaiter.Name = "label_ReceiveWaiter";
            this.label_ReceiveWaiter.Size = new System.Drawing.Size(148, 29);
            this.label_ReceiveWaiter.TabIndex = 8;
            this.label_ReceiveWaiter.Text = "0";
            this.label_ReceiveWaiter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form_QueueMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 154);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_QueueMonitor";
            this.ShowIcon = false;
            this.Text = "Queue Monitor";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label_Error;
        private System.Windows.Forms.Label label_ReceivePacket;
        private System.Windows.Forms.Label label_Byte;
        private System.Windows.Forms.Label label_Send;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label_ReceiveWaiter;
        private System.Windows.Forms.Label label9;
    }
}