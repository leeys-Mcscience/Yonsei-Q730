using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Q730
{
    public partial class Form_PresetSelectBox : Form
    {
        private string[] presetFiles;
        private string presetFolderPath;
        private string _defaultPresetPathFileName = "PresetPath.txt";
    

        public string presetFileName { get; set; }
        public string presetFilePath { get; set; }
        public Form_PresetSelectBox()
        {
            InitializeComponent();

            if (SoftwareConfiguration.Common.LastPatchNoteCreationTime != new FileInfo("PresetPath.txt").LastWriteTime.ToString("yyyyMMddHHmmss"))
            {
                string[] lines = File.ReadAllLines(_defaultPresetPathFileName);

                if (lines.Length > 0)
                {
                    presetFolderPath = lines[0];
                }
            }

            this.presetPathTextBox.Text = presetFolderPath;

            presetFiles = Directory.GetFiles(presetFolderPath, "*.preset");

            foreach (string presetFile in presetFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(presetFile);
              
                this.presetListComboBox.Items.Add(fileName);
            }

            this.presetListComboBox.SelectedIndex = 0;

            if (presetPathTextBox.Text == null)
            {
                presetListComboBox.Enabled = false;
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            presetListComboBox.Items.Clear();
            
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var files = new DirectoryInfo(dialog.SelectedPath).GetFiles("*.preset");

                    if (files.Length == 0)
                    {
                        MessageBox.Show("해당 경로에는 preset 파일이 존재하지 않습니다.", "preset Box 메시지");
                        presetPathTextBox.Clear();
                        presetListComboBox.Enabled = false;
                        return;
                    }

                    presetListComboBox.Enabled = true;

                    foreach (var fileInfo  in files)
                    {
                        presetListComboBox.Items.Add(fileInfo);
                    }
                    presetPathTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            presetFileName = this.presetListComboBox.Text;

            var item = presetFiles.FirstOrDefault(x => x.Contains(presetFileName));
            presetFilePath = item;
            Close();
        }
    }
}
