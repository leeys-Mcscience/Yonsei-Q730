using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataViewer
{
    public partial class UserControl_ExportWindow : Form
    {
        
        public bool exportSignal { get; set; }
        public bool checkTime { get; set; }
        public UserControl_ExportWindow()
        {
            InitializeComponent();
            exportSignal = false;
        }

        private void button_Separate_Click(object sender, EventArgs e)
        {
            // "확인" 버튼을 클릭한 경우 처리할 작업을 여기에 추가

            // 작업이 완료되면 다이얼로그 창을 닫습니다.
            exportSignal = true;
            Form parentForm = this.FindForm();
            parentForm.Close();
        }

        private void button_Total_Click(object sender, EventArgs e)
        {
            // "취소" 버튼을 클릭한 경우 처리할 작업을 여기에 추가

            // 작업이 완료되면 다이얼로그 창을 닫습니다.
            exportSignal = false;
            Form parentForm = this.FindForm();
            parentForm.Close();
        }

        public void SetSeparateButtonEnabled(bool enabled)
        {
            button_Separate.Enabled = enabled;
            button_Separate.Visible = enabled;
        }
        public void SetTotalButtonEnabled(bool enabled)
        {
            button_Total.Enabled = enabled;
            button_Total.Visible = enabled;
        }

        private void checkBox_Time_CheckedChanged(object sender, EventArgs e)
        {
            checkTime = this.checkBox_Time.Checked;
        }
    }
}
