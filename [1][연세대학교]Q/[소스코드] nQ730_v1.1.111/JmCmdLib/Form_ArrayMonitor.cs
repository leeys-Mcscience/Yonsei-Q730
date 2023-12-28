using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JmCmdLib
{
    public partial class Form_ArrayMonitor : Form
    {
        IList _array;
        public Form_ArrayMonitor(IList array)
        {
            InitializeComponent();

            if( array != null )
            {
                _array = array;

                for(var i = 0; i < _array.Count; i++ )
                {
                    var index = dataGridView1.Rows.Add();
                    dataGridView1.Rows[index].Cells[0].Value = index.ToString();
                    dataGridView1.Rows[index].Cells[1].Value = _array[i].ToString();
                }
                timer1.Start();
            }
        }

        private void timer1_Tick( object sender, EventArgs e )
        {
            try
            {
                if( dataGridView1.Rows.Count != _array.Count )
                {
                    dataGridView1.Rows.Clear();
                    for ( var i = 0; i < _array.Count; i++ )
                    {
                        var index = dataGridView1.Rows.Add();
                        dataGridView1.Rows[index].Cells[0].Value = index.ToString();
                        dataGridView1.Rows[index].Cells[1].Value = _array[i].ToString();
                    }
                }

                for(var i = 0; i < _array.Count; i++ )
                {
                    dataGridView1.Rows[i].Cells[1].Value = _array[i].ToString();
                    Application.DoEvents();
                }
            }
            catch
            {
                timer1.Stop();
            }
        }

        private void Form_ArrayMonitor_FormClosing( object sender, FormClosingEventArgs e )
        {
            timer1.Stop();
        }
    }
}
