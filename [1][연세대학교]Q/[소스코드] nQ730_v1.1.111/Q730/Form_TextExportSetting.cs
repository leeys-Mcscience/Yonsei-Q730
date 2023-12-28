using System;
using System.Windows.Forms;

namespace Q730
{
    public partial class Form_TextExportSetting : Form
    {
        public string Path
        {
            get => textBox_Path.Text;
            set => textBox_Path.Text = value;
        }
        public bool Use
        {
            get => checkBox_Use.Checked;
            set => checkBox_Use.Checked = value;
        }

        public Form_TextExportSetting( int channelNo, string path )
        {
            InitializeComponent();

            Text = Text + $" - CH{channelNo + 1}";

            if ( !string.IsNullOrEmpty( path ) )
            {
                textBox_Path.Text = path;
                checkBox_Use.Checked = true;
            }
            else
            {
                textBox_Path.Text = string.Empty;
                checkBox_Use.Checked = false;
            }
        }

        private void checkBox_Use_CheckedChanged( object sender, EventArgs e )
        {
            button_Path.Enabled = checkBox_Use.Checked;
        }

        private void button_Path_Click( object sender, EventArgs e )
        {
            using ( var dialog = new SaveFileDialog()
            {
                Filter = "텍스트(*.txt)|*.txt|모든 파일(*.*)|*.*"
            } )
            {
                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    textBox_Path.Text = dialog.FileName;
                }
            }
        }

        private void button_OK_Click( object sender, EventArgs e )
        {
            if ( checkBox_Use.Checked && string.IsNullOrWhiteSpace( textBox_Path.Text ) )
            {
                MessageBox.Show( "파일이 저장될 경로가 설정되지 않았습니다.", "Q730 알림 메시지" );
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}