using DataViewer.Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace DataViewer
{
    public partial class Form_Export : Form
    {
        public Form_Export(List<RecipeData> datas)
        {
            InitializeComponent();

            this.datas = datas;
            Application.Idle += application_Idle;
        }

        private void application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= application_Idle;

            label4.Text = $"선택된 레시피 : {datas.Count}개";
            comboBox_Seperator.SelectedIndex = 0;

            _nonSelected.AddRange(Enum.GetNames(typeof(DataType)));
            _nonSelected.RemoveAt(0); // None 제거

            _nonSelected.Remove("Cycle");
            _nonSelected.Remove("ChargeCapacity");
            _nonSelected.Remove("DisChargeCapacity");
            _nonSelected.Remove("CoulombEfficiency");

            _nonSelected.Remove("Frequency");
            _nonSelected.Remove("Z");
            _nonSelected.Remove("Z_Real");
            _nonSelected.Remove("Z_Img");
            _nonSelected.Remove("DeltaV");
            _nonSelected.Remove("DeltaI");
            _nonSelected.Remove("DeltaT");
            _nonSelected.Remove("StartOcv");
            _nonSelected.Remove("EndOcv");
            _nonSelected.Remove("Phase");
            _nonSelected.Remove("V1");
            _nonSelected.Remove("I1");
            _nonSelected.Remove("V2");
            _nonSelected.Remove("I2");
            _nonSelected.Remove("R");

            listBox_NonSelected.Items.AddRange(_nonSelected.ToArray());
            refreshListBoxes();
        }

        List<RecipeData> datas;
        List<string> _selected = new List<string>();
        List<string> _nonSelected = new List<string>();

        private void button_Select_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog() { Filter = "쉼표로 구분된 데이터(*.csv)|*.csv" })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (radioButton_FileSplit.Checked)
                    {
                        textBox_Path.Text = new FileInfo(dialog.FileName).FullName;
                        textBox_Path.Text = textBox_Path.Text.Substring(0, textBox_Path.Text.Length - 4);
                    }
                    else
                    {
                        textBox_Path.Text = dialog.FileName;
                    }
                }
            }
        }

        private void listBox_List_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox_NonSelected.SelectedIndex == -1) return;

            _nonSelected.Remove(listBox_NonSelected.SelectedItem.ToString());
            _selected.Add(listBox_NonSelected.SelectedItem.ToString());
            refreshListBoxes();
        }

        private void listBox_Selected_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox_Selected.SelectedIndex == -1) return;

            _selected.Remove(listBox_Selected.SelectedItem.ToString());
            _nonSelected.Add(listBox_Selected.SelectedItem.ToString());
            refreshListBoxes();
        }

        private void refreshListBoxes()
        {
            listBox_NonSelected.Items.Clear();
            listBox_Selected.Items.Clear();

            listBox_Selected.Items.AddRange(_selected.ToArray());
            listBox_NonSelected.Items.AddRange(_nonSelected.ToArray());
        }

        private void button_Up_Click(object sender, EventArgs e)
        {
            if (listBox_Selected.SelectedIndex <= 0) return;

            var index = listBox_Selected.SelectedIndex;

            var item = _selected[index];
            _selected.RemoveAt(index);
            _selected.Insert(index - 1, item);

            refreshListBoxes();

            listBox_Selected.SelectedIndex = index - 1;
            listBox_Selected.Focus();
        }

        private void button_Down_Click(object sender, EventArgs e)
        {
            if (listBox_Selected.SelectedIndex >= listBox_Selected.Items.Count - 1) return;

            var index = listBox_Selected.SelectedIndex;

            var item = _selected[index];
            _selected.RemoveAt(index);
            _selected.Insert(index + 1, item);

            refreshListBoxes();

            listBox_Selected.SelectedIndex = index + 1;
            listBox_Selected.Focus();
        }

        private void checkBox_IsSeperatorSelf_CheckedChanged(object sender, EventArgs e)
        {
            comboBox_Seperator.Enabled = !(textBox_Separator.Enabled = checkBox_IsSeperatorSelf.Checked);
        }

        private void button_Export_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox_Path.Text))
                {
                    MessageBox.Show("파일이 저장될 경로가 설정되지 않았습니다.", "Q730 알림 메시지");
                    return;
                }

                DataType[] columns;
                if (_selected.Count == 0)
                {
                    MessageBox.Show("내보낼 데이터가 선택되지 않았습니다.", "Q730 알림 메시지");
                    return;
                }
                else
                {
                    columns = new DataType[_selected.Count];
                    for (var i = 0; i < _selected.Count; i++)
                    {
                        Enum.TryParse(_selected[i], out columns[i]);
                    }
                }

                var columnText = "Index";
                for (var i = 0; i < columns.Length; i++) columnText += $",{columns[i]}({SoftwareConfiguration.GraphSetting.GetUnitText(columns[i])})";

                var separator = string.Empty;
                if (!checkBox_IsSeperatorSelf.Checked)
                {
                    switch (comboBox_Seperator.SelectedIndex)
                    {
                        case 0:
                            separator = ",";
                            break;

                        case 1:
                            separator = "\t";
                            break;

                        case 2:
                            separator = " ";
                            break;

                        default:
                            MessageBox.Show("구분자가 선택되지 않았습니다.", "Q730 알림 메시지");
                            return;
                    }
                }
                else
                {
                    if (textBox_Separator.Text.Length == 0)
                    {
                        MessageBox.Show("구분자가 지정되지 않았습니다.", "Q730 알림 메시지");
                        return;
                    }

                    separator = textBox_Separator.Text;
                }

                if (radioButton_FileSplit.Checked)
                {
                    var fileNo = 0;

                    for (var i = 0; i < datas.Count; i++)
                    {
                        using (var sw = new StreamWriter($"{new FileInfo(textBox_Path.Text).FullName}_{fileNo++}.csv"))
                        {
                            sw.Write(columnText);     // 컬럼 헤더 삽입

                            var dataList = new List<double[]>();
                            for (var j = 0; j < columns.Length; j++)
                            {
                                dataList.Add(datas[i].GetData(columns[j]));
                            }

                            for (var j = 0; j < datas[i].Count; j++)
                            {
                                var line = (j + 1).ToString();
                                for (var k = 0; k < columns.Length; k++)
                                {
                                    line += $"{separator}{dataList[k][j]}";
                                }
                                sw.Write($"\r\n{line}");
                            }
                        }
                    }
                }
                else if (radioButton_NotFileSplit.Checked)
                {
                    if (checkBox_TimeSplit.Checked)
                    {
                        var dataList = new List<List<double>>();
                        var SaveDataList = new List<List<double>>();
                        var ChooseIdx = new List<int>();
                        var TimeInterval = int.Parse(textBox_IntervalTime.Text);
                        for (var i = 0; i < columns.Length; i++)
                        {
                            dataList.Add(new List<double>());
                            for (var j = 0; j < datas.Count; j++)
                            {
                                dataList[i].AddRange(datas[j].GetData(columns[i]));
                            }
                        }
                        int totaltime = -1;
                        int steptime = -1;
                        for (int i = 0; i < columns.Length; i++)
                        {
                            if (columns[i] == DataType.TotalTime)
                            {
                                totaltime = i;
                            }
                            if (columns[i] == DataType.StepTime)
                            {
                                steptime = i;
                            }
                        }

                        var SerchTime = dataList[totaltime][0];
                        int cnt = 0;
                        for (int j = 0; j < dataList[totaltime].Count; j++)
                        {
                            if (Math.Round(SerchTime) <= Math.Round(dataList[totaltime][j]))
                            {
                                var ListBuf = new List<double>();
                                for (int k = 0; k < dataList.Count; k++)
                                {
                                    if (k == totaltime || k == steptime)
                                    {
                                        if (radioButton_Hour.Checked)
                                        {
                                            ListBuf.Add(dataList[k][j] / 3600);
                                        }
                                        else
                                        {
                                            ListBuf.Add(dataList[k][j]);
                                        }
                                    }
                                    else
                                    {
                                        ListBuf.Add(dataList[k][j]);

                                    }
                                }
                                SaveDataList.Add(ListBuf);
                                cnt++;
                                SerchTime += TimeInterval;
                            }
                            else
                            {

                            }
                        }



                        using (var sw = new StreamWriter(textBox_Path.Text))
                        {
                            sw.Write(columnText);     // 컬럼 헤더 삽입

                            for (var j = 0; j < SaveDataList.Count; j++)
                            {
                                var line = (j + 1).ToString();
                                for (var k = 0; k < columns.Length; k++)
                                {
                                    line += $"{separator}{SaveDataList[j][k]}";
                                }
                                sw.Write($"\r\n{line}");
                            }
                        }
                    }
                    else
                    {
                        var dataList = new List<List<double>>();
                        for (var i = 0; i < columns.Length; i++)
                        {
                            dataList.Add(new List<double>());
                            for (var j = 0; j < datas.Count; j++)
                            {
                                dataList[i].AddRange(datas[j].GetData(columns[i]));
                            }
                        }

                        using (var sw = new StreamWriter(textBox_Path.Text))
                        {
                            sw.Write(columnText);     // 컬럼 헤더 삽입

                            for (var j = 0; j < dataList[0].Count; j++)
                            {
                                var line = (j + 1).ToString();
                                for (var k = 0; k < columns.Length; k++)
                                {
                                    line += $"{separator}{dataList[k][j]}";
                                }
                                sw.Write($"\r\n{line}");
                            }
                        }
                    }

                }

                if (checkBox_RunAfterExport.Checked)
                {
                    Process.Start(textBox_Path.Text);
                }
                else if (MessageBox.Show("내보내기가 완료되었습니다. 파일 탐색기에서 폴더를 여시겠습니까?", "Q730 알림 메시지", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Process.Start(new FileInfo(textBox_Path.Text).DirectoryName);
                }
                //Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show($"내보내기에 실패했습니다. (원인 : {ex.Message})", "Q730 알림 메시지");
            }
        }

        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_FileSplit.Checked)
            {   // 레시피 별로 파일 분리 -> 확장자 제거
                if (textBox_Path.Text.Length != 0 && textBox_Path.Text.LastIndexOf(".csv") == textBox_Path.Text.Length - 4)
                {
                    textBox_Path.Text = textBox_Path.Text.Substring(0, textBox_Path.Text.Length - 4);
                }

                checkBox_RunAfterExport.Checked = false;
                checkBox_RunAfterExport.Enabled = false;
                groupBox4.Enabled = false;
                groupBox5.Enabled = false;
            }
            else
            {   // 한 파일에 모두 쓰기
                if (textBox_Path.Text.Length != 0 && textBox_Path.Text.LastIndexOf(".csv") != textBox_Path.Text.Length - 4)
                {
                    textBox_Path.Text += ".csv";
                }

                checkBox_RunAfterExport.Enabled = true;
                groupBox4.Enabled = true;
                groupBox5.Enabled = true;

            }
        }

        private void checkBox_TimeSplit_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_TimeSplit.Checked) textBox_IntervalTime.Enabled = true;
            else textBox_IntervalTime.Enabled = false;
        }
    }
}