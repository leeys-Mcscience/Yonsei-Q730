using DataViewer.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataViewer
{
    public partial class Form_Loading : Form
    {
        public Form_Loading( string[] filenames )
        {
            InitializeComponent();

            this.filenames = filenames;
            Application.Idle += Application_Idle;
        }

        private void Application_Idle( object sender, EventArgs e )
        {
            Application.Idle -= Application_Idle;

            progressBar1.Maximum = filenames.Length;
            label_FileCount.Text = $"0/{filenames.Length}";
            new Thread( fileLoadLoop ) { IsBackground = true }.Start();
        }

        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        private string[] filenames;
        private bool _cancel;
        public readonly List<QDataManager> Result = new List<QDataManager>();
        public readonly List<string> FailFiles = new List<string>();

        public void Cancel()
        {
            _cancel = true;
            tokenSource.Cancel();
        }

        private void fileLoadLoop()
        {
            int count = 0;

            while ( !_cancel && count < filenames.Length )
            {
                Application.DoEvents();

                try
                {
                    var mgr = QDataManager.FromFileAsync( filenames[count], tokenSource );
                    if ( mgr != null ) Result.Add( mgr );
                }
                catch (Exception ex)
                {
                    FailFiles.Add( filenames[count] );
                }

                count++;

                Invoke( new Action( delegate ()
                {
                    progressBar1.PerformStep();
                    label_FileCount.Text = $"{count}/{filenames.Length}";
                } ) );
            }

            if ( _cancel )
            {
                Result.Clear();
                DialogResult = DialogResult.Cancel;
                //Close();
            }
            else
            {
                DialogResult = DialogResult.OK;
                //Close();
            }
        }

        private void button_Cancel_Click( object sender, EventArgs e )
        {
            Cancel();
        }

        string dots = string.Empty;
        private void timer1_Tick( object sender, EventArgs e )
        {
            Application.DoEvents();

            Text = $"Loading{dots}";
            dots += ".";

            if ( dots.Length == 4 ) dots = string.Empty;
        }
    }
}
