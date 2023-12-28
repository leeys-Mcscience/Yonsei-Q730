using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Q730.NotUsed
{
    public partial class UserControl_TimeBox : UserControl
    {
        public TimeSpan Time
        {
            get
            {
                int h = textBox_Hour.Text.Trim().Length == 0 ? 0 : int.Parse( textBox_Hour.Text );
                int m = textBox_Min.Text.Trim().Length == 0 ? 0 : int.Parse( textBox_Min.Text );
                int s = textBox_Sec.Text.Trim().Length == 0 ? 0 : int.Parse( textBox_Sec.Text );
                double totalMs = ((h * 3600) + (m * 60) + s) * 1000;

                return TimeSpan.FromMilliseconds( totalMs );
            }
            set
            {
                textBox_Hour.Text = (value.Days * 24 + value.Hours).ToString();
                textBox_Min.Text = value.Minutes.ToString();
                textBox_Sec.Text = value.Seconds.ToString();
            }
        }
        //public override Color BackColor
        //{

        //}

        public UserControl_TimeBox()
        {
            InitializeComponent();
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e )
        {
            if( !char.IsDigit( e.KeyChar ) && e.KeyChar != 8 ) e.Handled = true;
        }
    }
}
