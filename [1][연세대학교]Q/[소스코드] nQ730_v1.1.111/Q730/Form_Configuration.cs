using McQLib.Core;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Q730
{
    public partial class Form_Configuration : Form
    {
        public bool IsModified = false;
        public Form_Configuration( bool deviceChangable = false )
        {
            InitializeComponent();

            loadConfigurations();
            tabControl2.Enabled = deviceChangable;
            label_ChangableMessage.Visible = !deviceChangable;
            tableLayoutPanel3.RowStyles[0].Height = deviceChangable ? 0 : 20;
        }

        private SoftwareConfiguration.LIST.Columns[] _columns;
        private void loadConfigurations()
        {
            checkBox_GlobalSafety.Checked = SoftwareConfiguration.SequenceBuilder.GlobalSafetyCondition;
            checkBox_CycleLoopPair.Checked = SoftwareConfiguration.SequenceBuilder.AutoAddCycleLoopPair;
            checkBox_Indentation.Checked = SoftwareConfiguration.SequenceBuilder.Indentation;
            checkBox_ShowDetail.Checked = SoftwareConfiguration.SequenceBuilder.ShowDetailsToolTip;
            checkBox_ShowManual.Checked = SoftwareConfiguration.SequenceBuilder.ShowManualToolTip;

            panel_Idle_Back.BackColor = SoftwareConfiguration.GRID.IdleBackColor;
            panel_Idle_Fore.BackColor = SoftwareConfiguration.GRID.IdleForeColor;
            panel_Run_Back.BackColor = SoftwareConfiguration.GRID.RunBackColor;
            panel_Run_Fore.BackColor = SoftwareConfiguration.GRID.RunForeColor;
            panel_Paused_Back.BackColor = SoftwareConfiguration.GRID.PausedBackColor;
            panel_Paused_Fore.BackColor = SoftwareConfiguration.GRID.PausedForeColor;
            panel_Safety_Back.BackColor = SoftwareConfiguration.GRID.SafetyBackColor;
            panel_Safety_Fore.BackColor = SoftwareConfiguration.GRID.SafetyForeColor;
            panel_Error_Back.BackColor = SoftwareConfiguration.GRID.ErrorBackColor;
            panel_Error_Fore.BackColor = SoftwareConfiguration.GRID.ErrorForeColor;

            textBox_DefaultDirectory.Text = SoftwareConfiguration.Measurement.DefaultDirectory;
            checkBox_ChannelLogging.Checked = SoftwareConfiguration.Measurement.ChannelLogging;
            comboBox_VoltageUnit.SelectedIndex = ( int )SoftwareConfiguration.Measurement.VoltageUnit.SiUnit;
            comboBox_CurrentUnit.SelectedIndex = ( int )SoftwareConfiguration.Measurement.CurrentUnit.SiUnit;
            comboBox_PowerUnit.SelectedIndex = ( int )SoftwareConfiguration.Measurement.PowerUnit.SiUnit;
            checkBox_IsClearSequence.Checked = SoftwareConfiguration.Measurement.IsClearSequenceWhenEnd;
            numericUpDown_VoltageDecimal.Value = SoftwareConfiguration.Measurement.VoltageUnit.DecimalPlace;
            numericUpDown_CurrentDecimal.Value = SoftwareConfiguration.Measurement.CurrentUnit.DecimalPlace;
            numericUpDown_PowerDecimal.Value = SoftwareConfiguration.Measurement.PowerUnit.DecimalPlace;
            numericUpDown_GraphDecimal.Value = SoftwareConfiguration.Measurement.GraphDecimalPlace;

            comboBox_ExVoltageUnit.SelectedIndex = (int)SoftwareConfiguration.Measurement.ExportVoltageUnit.SiUnit;
            comboBox_ExCurrentUnit.SelectedIndex = ( int )SoftwareConfiguration.Measurement.ExportCurrentUnit.SiUnit;

            numericUpDown_SkipCount.Value = SoftwareConfiguration.DETAIL.SkipPoints;

            _columns = SoftwareConfiguration.LIST.ColumnOrders.ToArray();
            refreshListBox();

            DeviceConfiguration.Load();
            for ( var i = 0; i < DeviceConfiguration.Devices.Count; i++ )
            {
                var item = new ListViewItem( ( listView1.Items.Count + 1 ).ToString() );
                item.SubItems.Add( DeviceConfiguration.Devices[i].IP );
                item.SubItems.Add( DeviceConfiguration.Devices[i].ChannelCount.ToString() );
                listView1.Items.Add( item );
            }
        }

        private void refreshListBox()
        {
            listBox_Columns.Items.Clear();

            listBox_Columns.Items.Add( "CH (Fixed)" );
            for ( var i = 1; i < _columns.Length; i++ )
            {
                listBox_Columns.Items.Add( _columns[i].ToString() );
            }
        }
        private void colorPanel_Clicked( object sender, EventArgs e )
        {
            var panel = sender as Panel;
            colorDialog1.Color = panel.BackColor;
            if ( colorDialog1.ShowDialog() == DialogResult.OK )
            {
                panel.BackColor = colorDialog1.Color;
            }
        }

        private void button_SelectDefaultDirectory_Click( object sender, EventArgs e )
        {
            using ( var dialog = new CommonOpenFileDialog() )
            {
                dialog.IsFolderPicker = true;
                if ( textBox_DefaultDirectory.Text.Length != 0 && Directory.Exists( textBox_DefaultDirectory.Text ) ) dialog.InitialDirectory = textBox_DefaultDirectory.Text;

                if ( dialog.ShowDialog() == CommonFileDialogResult.Ok )
                {
                    SoftwareConfiguration.Measurement.DefaultDirectory = textBox_DefaultDirectory.Text = dialog.FileName;
                }
            }
        }

        private void reindex()
        {
            for ( var i = 0; i < listView1.Items.Count; i++ )
            {
                listView1.Items[i].Text = ( i + 1 ).ToString();
            }
        }

        private void button_AddDevice_Click( object sender, EventArgs e )
        {
            if ( System.Net.IPAddress.TryParse( textBox_IP.Text, out System.Net.IPAddress dummy ) )
            {
                var item = new ListViewItem( ( listView1.Items.Count + 1 ).ToString() );
                item.SubItems.Add( textBox_IP.Text );
                item.SubItems.Add( numericUpDown_ChannelCount.Value.ToString() );
                listView1.Items.Add( item );

                DeviceConfiguration.Devices.Add( new DeviceInfo()
                {
                    IP = textBox_IP.Text,
                    ChannelCount = ( int )numericUpDown_ChannelCount.Value
                } );

                IsModified = true;
            }
            else
            {
                MessageBox.Show( "IP 형식이 올바르지 않습니다.", "Q730 알림 메시지" );
            }
        }

        private void button_RemoveDevice_Click( object sender, EventArgs e )
        {
            if ( listView1.SelectedItems.Count == 0 ) return;

            var index = listView1.SelectedIndices[0];
            listView1.Items.RemoveAt( index );
            DeviceConfiguration.Devices.RemoveAt( index );
            reindex();

            IsModified = true;
        }

        private bool isChanged = false;
        private void button_Up_Click( object sender, EventArgs e )
        {
            if ( listBox_Columns.SelectedIndex == -1 ) return;
            else if ( listBox_Columns.SelectedIndex <= 1 )
            {
                MessageBox.Show( "CH 열의 자리는 변경될 수 없습니다.", "Q730 알림 메시지" );
                return;
            }

            var cur = listBox_Columns.SelectedIndex;

            var tmp = _columns[cur];
            _columns[cur] = _columns[cur - 1];
            _columns[cur - 1] = tmp;

            refreshListBox();

            listBox_Columns.SelectedIndex = cur - 1;
            isChanged = true;
        }

        private void button_Down_Click( object sender, EventArgs e )
        {
            if ( listBox_Columns.SelectedIndex == -1 || listBox_Columns.SelectedIndex == listBox_Columns.Items.Count - 1 ) return;
            else if ( listBox_Columns.SelectedIndex == 0 )
            {
                MessageBox.Show( "CH 열의 자리는 변경될 수 없습니다.", "Q730 알림 메시지" );
                return;
            }

            var cur = listBox_Columns.SelectedIndex;

            var tmp = _columns[cur];
            _columns[cur] = _columns[cur + 1];
            _columns[cur + 1] = tmp;

            refreshListBox();

            listBox_Columns.SelectedIndex = cur + 1;
            isChanged = true;
        }

        private void button_Apply_Click( object sender, EventArgs e )
        {
            SoftwareConfiguration.SequenceBuilder.Indentation = checkBox_Indentation.Checked;
            SoftwareConfiguration.SequenceBuilder.AutoAddCycleLoopPair = checkBox_CycleLoopPair.Checked;
            SoftwareConfiguration.SequenceBuilder.GlobalSafetyCondition = checkBox_GlobalSafety.Checked;
            SoftwareConfiguration.SequenceBuilder.ShowDetailsToolTip = checkBox_ShowDetail.Checked;
            SoftwareConfiguration.SequenceBuilder.ShowManualToolTip = checkBox_ShowManual.Checked;

            SoftwareConfiguration.GRID.IdleBackColor = panel_Idle_Back.BackColor;
            SoftwareConfiguration.GRID.IdleForeColor = panel_Idle_Fore.BackColor;
            SoftwareConfiguration.GRID.RunBackColor = panel_Run_Back.BackColor;
            SoftwareConfiguration.GRID.RunForeColor = panel_Run_Fore.BackColor;
            SoftwareConfiguration.GRID.PausedBackColor = panel_Paused_Back.BackColor;
            SoftwareConfiguration.GRID.PausedForeColor = panel_Paused_Fore.BackColor;
            SoftwareConfiguration.GRID.SafetyBackColor = panel_Safety_Back.BackColor;
            SoftwareConfiguration.GRID.SafetyForeColor = panel_Safety_Fore.BackColor;
            SoftwareConfiguration.GRID.ErrorBackColor = panel_Error_Back.BackColor;
            SoftwareConfiguration.GRID.ErrorForeColor = panel_Error_Fore.BackColor;

            SoftwareConfiguration.Measurement.ChannelLogging = checkBox_ChannelLogging.Checked;
            SoftwareConfiguration.Measurement.VoltageUnit = new UnitInfo( UnitType.Voltage, ( SiUnits )comboBox_VoltageUnit.SelectedIndex, ( int )numericUpDown_VoltageDecimal.Value );
            SoftwareConfiguration.Measurement.CurrentUnit = new UnitInfo( UnitType.Current, ( SiUnits )comboBox_CurrentUnit.SelectedIndex, ( int )numericUpDown_CurrentDecimal.Value );
            //SoftwareConfiguration.Measurement.VoltageUnit.SiUnit = ( SiUnits )comboBox_VoltageUnit.SelectedIndex;
            //SoftwareConfiguration.Measurement.CurrentUnit.SiUnit = ( SiUnits )comboBox_CurrentUnit.SelectedIndex;
            SoftwareConfiguration.Measurement.PowerUnit.SiUnit = ( SiUnits )comboBox_PowerUnit.SelectedIndex;
            SoftwareConfiguration.Measurement.IsClearSequenceWhenEnd = checkBox_IsClearSequence.Checked;
            //SoftwareConfiguration.Measurement.VoltageUnit.DecimalPlace = ( int )numericUpDown_VoltageDecimal.Value;
            //SoftwareConfiguration.Measurement.CurrentUnit.DecimalPlace = ( int )numericUpDown_CurrentDecimal.Value;
            SoftwareConfiguration.Measurement.PowerUnit.DecimalPlace = ( int )numericUpDown_PowerDecimal.Value;
            SoftwareConfiguration.Measurement.GraphDecimalPlace = ( int )numericUpDown_GraphDecimal.Value;

            SoftwareConfiguration.Measurement.ExportVoltageUnit.SiUnit = ( SiUnits )comboBox_ExVoltageUnit.SelectedIndex;
            SoftwareConfiguration.Measurement.ExportCurrentUnit.SiUnit = ( SiUnits )comboBox_ExCurrentUnit.SelectedIndex;

            SoftwareConfiguration.LIST.ColumnOrders = _columns.ToList();
            SoftwareConfiguration.LIST.ColumnChanged = isChanged;

            SoftwareConfiguration.DETAIL.SkipPoints = ( int )numericUpDown_SkipCount.Value;

            SoftwareConfiguration.Save();
            DeviceConfiguration.Save();
        }

        private void button_Cancel_Click( object sender, EventArgs e )
        {
            Close();
        }

        private void checkBox_Appending_CheckedChanged( object sender, EventArgs e )
        {
            if ( !checkBox_Appending.Checked )
            {
                if ( MessageBox.Show( "이어붙이기 옵션이 비활성화되면 측정 중 소프트웨어가 종료 후 재시작되어도 장비로부터 측정 데이터를 받아오지 않습니다.\r\n정말 이어붙이기 옵션을 비활성화 하시겠습니까?", "Q730 알림 메시지", MessageBoxButtons.YesNo ) == DialogResult.No )
                {
                    checkBox_Appending.Checked = true;
                }
            }
        }

        private void button_Init_Click( object sender, EventArgs e )
        {

        }
    }
}