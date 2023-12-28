using System.IO;
using System.Windows.Forms;

namespace Q730
{
    public partial class Form_PatchNote : Form
    {
        public Form_PatchNote()
        {
            InitializeComponent();

            if ( new FileInfo( "PatchNote.txt" ).Exists )
            {
                using ( var sr = new StreamReader( "PatchNote.txt", System.Text.Encoding.Default ) )
                {
                    SoftwareConfiguration.Common.LastPatchNoteCreationTime = new FileInfo( "PatchNote.txt" ).LastWriteTime.ToString( "yyyyMMddHHmmss" );
                    richTextBox1.Text = sr.ReadToEnd();
                }
            }
        }
    }
}
