using McQLib.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DataViewer;

namespace Q730.UserControls
{
    public partial class UserControl_ChannelListView : UserControl
    {
        Channel[] _channels;

        int INDEX_CH = 0;
        int INDEX_STATE = 1;
        int INDEX_TOTAL_TIME = 2;
        int INDEX_VOLTAGE = 3;
        int INDEX_CURRENT = 4;
        int INDEX_STEP_NO = 5;
        int INDEX_STEP_PROGRESS = 6;
        int INDEX_RECIPE = 7;
        int INDEX_SEQUENCE = 8;
        int INDEX_NAME = 9;
        int INDEX_SAVE_DIRECTORY = 10;
        int INDEX_MESSAGE = 11;

        public UserControl_ChannelListView()
        {
            InitializeComponent();

            dataGridView1.GetType().GetProperty( "DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance ).SetValue( dataGridView1, true );

            Application.Idle += application_Idle;
        }

        private void application_Idle( object sender, EventArgs e )
        {
            Application.Idle -= application_Idle;

            foreach ( DataGridViewColumn c in dataGridView1.Columns )
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            for ( var i = 0; i < dataGridView1.Columns.Count; i++ )
            {
                dataGridView1.Columns[i].Width = SoftwareConfiguration.LIST.ColumnSizes[i];
            }

            dataGridView1.ColumnWidthChanged += dataGridView1_ColumnWidthChanged;

            SoftwareConfiguration.PropertyChanged += onConfigChanged;
            onConfigChanged( null, new PropertyChangedEventArgs( nameof( SoftwareConfiguration.LIST.ColumnOrders ) ) );
            onConfigChanged( null, new PropertyChangedEventArgs( nameof( SoftwareConfiguration.Measurement.VoltageUnit ) ) );
            onConfigChanged( null, new PropertyChangedEventArgs( nameof( SoftwareConfiguration.Measurement.CurrentUnit ) ) );
        }

        [Browsable( true )]
        public event EventHandler SelectedIndexChanged;
        [Browsable( true )]
        public event EventHandler ChannelDoubleClick;

        private List<int> _selectedIndices = new List<int>();
        private bool _isUpdateRun = true;

        public int[] SelectedIndices => _selectedIndices.ToArray();

        public void BindChannels( Channel[] channels )
        {
            // 기존 채널 개체들에 등록된 이벤트 제거
            if ( _channels != null )
            {
                for ( var i = 0; i < _channels.Length; i++ )
                {
                    if ( _channels[i] != null ) _channels[i].PropertyChanged -= onPropertyChanged;
                }
            }

            _channels = channels;
            
            dataGridView1.Rows.Clear();

            if ( _channels == null ) return;

            for ( var i = 0; i < _channels.Length; i++ )
            {
                if ( _channels[i] != null )
                {
                    dataGridView1.Rows.Add( new object[]
                    {
                        i + 1,
                        _channels[i].State,
                        TimeSpan.FromMilliseconds( _channels[i].TotalTime ),
                        _channels[i].Voltage,
                        _channels[i].Current,
                        _channels[i].StepNo,
                        getProgressString(_channels[i].StepCount, _channels[i].TotalSteps),
                        _channels[i].RecipeName,
                        "-",
                        _channels[i].Name,
                        _channels[i].SaveDirectory,
                        _channels[i].Message
                    } );

                    _channels[i].PropertyChanged += onPropertyChanged;
                }
            }

            RefreshAll();
        }
        //public void Start() => timer_Updater.Start();
        //public void Stop() => timer_Updater.Stop();
        public void Start()
        {
            _isUpdateRun = true;
            if(Handle != IntPtr.Zero) RefreshAll();
        }
        public void Stop() => _isUpdateRun = false;

        public void RefreshAll()
        {
            if ( _channels == null ) return;
            for ( var i = 0; i < _channels.Length; i++ )
            {
                onPropertyChanged( _channels[i], new PropertyChangedEventArgs( nameof( Channel.State ) ) );
                onPropertyChanged( _channels[i], new PropertyChangedEventArgs( nameof( Channel.Voltage ) ) );
                onPropertyChanged( _channels[i], new PropertyChangedEventArgs( nameof( Channel.Current ) ) );
                onPropertyChanged( _channels[i], new PropertyChangedEventArgs( nameof( Channel.TotalTime ) ) );
                onPropertyChanged( _channels[i], new PropertyChangedEventArgs( nameof( Channel.StepNo ) ) );
                onPropertyChanged( _channels[i], new PropertyChangedEventArgs( nameof( Channel.StepCount ) ) );
                onPropertyChanged( _channels[i], new PropertyChangedEventArgs( nameof( Channel.TotalSteps ) ) );
                onPropertyChanged( _channels[i], new PropertyChangedEventArgs( nameof( Channel.Message ) ) );
                onPropertyChanged( _channels[i], new PropertyChangedEventArgs( nameof( Channel.RecipeName ) ) );
                onPropertyChanged( _channels[i], new PropertyChangedEventArgs( nameof( Channel.Sequence ) ) );
                onPropertyChanged( _channels[i], new PropertyChangedEventArgs( nameof( Channel.Name ) ) );
                onPropertyChanged( _channels[i], new PropertyChangedEventArgs( nameof( Channel.SaveDirectory ) ) );
            }
        }
        private object getValue( object obj, string propName ) => obj.GetType().GetProperty( propName ).GetValue( obj );
        private string getProgressString( uint stepCount, int totalSteps )
        {
            if ( totalSteps == 0 ) return $"-";
            else return $"{stepCount + 1}/{totalSteps} ({( ( ( stepCount + 1 ) / ( double )totalSteps ) * 100 ).ToString( "0.0#" ) }%)";
        }
        private void onPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if ( !_isUpdateRun ) return;

            if ( InvokeRequired )
            {
                BeginInvoke( ( Action )( () => onPropertyChanged( sender, e ) ) );
                return;
            }

            var index = ( sender as Channel ).GlobalIndex;

            switch ( e.PropertyName )
            {
                case nameof( Channel.State ):
                    var state = ( State )getValue( sender, e.PropertyName );
                    dataGridView1.Rows[index].Cells[INDEX_STATE].Value = state;

                    if ( state == McQLib.Device.State.RUN )
                    {
                        dataGridView1.Rows[index].Cells[INDEX_STATE].Style.BackColor = Color.Gold;
                    }
                    else if ( state == McQLib.Device.State.SAFETY || state == McQLib.Device.State.ERROR )
                    {
                        dataGridView1.Rows[index].Cells[INDEX_STATE].Style.BackColor = Color.Salmon;
                    }
                    else
                    {
                        dataGridView1.Rows[index].Cells[INDEX_STATE].Style.BackColor = dataGridView1.Rows[index].Cells[INDEX_CH].Style.BackColor;
                    }
                    break;

                case nameof( Channel.Voltage ):
                    dataGridView1.Rows[index].Cells[INDEX_VOLTAGE].Value =
                    SoftwareConfiguration.Measurement.VoltageUnit.GetString( ( double )getValue( sender, e.PropertyName ) );
                    break;

                case nameof( Channel.Current ):
                    dataGridView1.Rows[index].Cells[INDEX_CURRENT].Value =
                    SoftwareConfiguration.Measurement.CurrentUnit.GetString( ( double )getValue( sender, e.PropertyName ) );
                    break;

                case nameof( Channel.TotalTime ):
                    dataGridView1.Rows[index].Cells[INDEX_TOTAL_TIME].Value =
                    McQLib.Core.Util.ConvertMsToString( ( ulong )getValue( sender, e.PropertyName ) );
                    break;

                case nameof( Channel.StepNo ):
                    dataGridView1.Rows[index].Cells[INDEX_STEP_NO].Value = getValue( sender, e.PropertyName );
                    break;

                case nameof( Channel.StepCount ):
                case nameof( Channel.TotalSteps ):
                    var stepCount = getValue( sender, nameof( Channel.StepCount ) );
                    var totalSteps = getValue( sender, nameof( Channel.TotalSteps ) );
                    dataGridView1.Rows[index].Cells[INDEX_STEP_PROGRESS].Value = getProgressString( ( uint )stepCount, ( int )totalSteps );
                    break;

                case nameof( Channel.Message ):
                    dataGridView1.Rows[index].Cells[INDEX_MESSAGE].Value = getValue( sender, e.PropertyName );
                    break;

                case nameof( Channel.RecipeName ):
                    dataGridView1.Rows[index].Cells[INDEX_RECIPE].Value = getValue( sender, e.PropertyName );
                    break;

                case nameof( Channel.Sequence ):
                    object tmp = getValue( sender, e.PropertyName );
                    tmp = tmp == null ? "-" : ( tmp as McQLib.Recipes.Sequence )?.Name;
                    dataGridView1.Rows[index].Cells[INDEX_SEQUENCE].Value = tmp;
                    break;

                case nameof( Channel.Name ):
                    dataGridView1.Rows[index].Cells[INDEX_NAME].Value = getValue( sender, e.PropertyName );
                    break;

                case nameof( Channel.SaveDirectory ):
                    dataGridView1.Rows[index].Cells[INDEX_SAVE_DIRECTORY].Value = getValue( sender, e.PropertyName );
                    break;
            }
        }
        private void onConfigChanged( object sender, PropertyChangedEventArgs e )
        {
            switch ( e.PropertyName )
            {
                case nameof( SoftwareConfiguration.LIST.ColumnOrders ):
                    _columns = SoftwareConfiguration.LIST.ColumnOrders.ToArray();

                    for ( var i = 1; i < _columns.Length; i++ )
                    {
                        dataGridView1.Columns[i].HeaderText = _columns[i].ToString();
                        var alignment = DataGridViewContentAlignment.MiddleCenter;

                        switch ( _columns[i] )
                        {
                            case SoftwareConfiguration.LIST.Columns.State:
                                INDEX_STATE = i;
                                break;

                            case SoftwareConfiguration.LIST.Columns.TotalTime:
                                INDEX_TOTAL_TIME = i;
                                break;

                            case SoftwareConfiguration.LIST.Columns.Voltage:
                                INDEX_VOLTAGE = i;
                                dataGridView1.Columns[i].HeaderText += $"({SoftwareConfiguration.Measurement.VoltageUnit.UnitString})";
                                break;

                            case SoftwareConfiguration.LIST.Columns.Current:
                                INDEX_CURRENT = i;
                                dataGridView1.Columns[i].HeaderText += $"({SoftwareConfiguration.Measurement.CurrentUnit.UnitString})";
                                break;

                            case SoftwareConfiguration.LIST.Columns.StepNo:
                                INDEX_STEP_NO = i;
                                break;

                            case SoftwareConfiguration.LIST.Columns.StepProgress:
                                INDEX_STEP_PROGRESS = i;
                                break;

                            case SoftwareConfiguration.LIST.Columns.Recipe:
                                INDEX_RECIPE = i;
                                break;

                            case SoftwareConfiguration.LIST.Columns.Sequence:
                                INDEX_SEQUENCE = i;
                                break;

                            case SoftwareConfiguration.LIST.Columns.Name:
                                INDEX_NAME = i;
                                break;

                            case SoftwareConfiguration.LIST.Columns.SaveDirectory:
                                INDEX_SAVE_DIRECTORY = i;
                                alignment = DataGridViewContentAlignment.MiddleLeft;
                                break;

                            case SoftwareConfiguration.LIST.Columns.Message:
                                INDEX_MESSAGE = i;
                                alignment = DataGridViewContentAlignment.MiddleLeft;
                                break;
                        }

                        dataGridView1.Columns[i].DefaultCellStyle.Alignment = alignment;
                    }
                    RefreshAll();
                    break;

                case nameof( SoftwareConfiguration.Measurement.VoltageUnit ):
                    dataGridView1.Columns[INDEX_VOLTAGE].HeaderText = $"Voltage({SoftwareConfiguration.Measurement.VoltageUnit.UnitString})";
                    RefreshAll();
                    break;

                case nameof( SoftwareConfiguration.Measurement.CurrentUnit ):
                    dataGridView1.Columns[INDEX_CURRENT].HeaderText = $"Current({SoftwareConfiguration.Measurement.CurrentUnit.UnitString})";
                    RefreshAll();
                    break;
            }
        }

        private SoftwareConfiguration.LIST.Columns[] _columns = SoftwareConfiguration.LIST.ColumnOrders.ToArray();

        private void dataGridView1_SelectionChanged( object sender, EventArgs e )
        {
            dataGridView1.ClearSelection();
        }

        private void dataGridView1_ColumnHeaderMouseClick( object sender, DataGridViewCellMouseEventArgs e )
        {
            if ( e.ColumnIndex == 0 )
            {
                if ( _selectedIndices.Count == 0 )
                {
                    for ( var i = 0; i < dataGridView1.Rows.Count; i++ )
                    {
                        selectRow( i, true );
                    }
                }
                else
                {
                    for ( var i = 0; i < dataGridView1.Rows.Count; i++ )
                    {
                        selectRow( i, false );
                    }
                }
            }
            else
            {
                for ( var i = 0; i < dataGridView1.Rows.Count; i++ )
                {
                    selectRow( i, false );
                }
            }

            SelectedIndexChanged?.Invoke( this, e );
        }

        private void dataGridView1_CellDoubleClick( object sender, DataGridViewCellEventArgs e )
        {
            if ( _selectedIndices.Count == 0 || e.RowIndex == -1 ) return;

            ChannelDoubleClick?.Invoke( this, e );
        }

        private void selectRow( int rowIndex, bool select )
        {
            if (rowIndex != -1)
            {
                var targetRow = dataGridView1.Rows[rowIndex];

                if (select)
                {
                    if (_selectedIndices.IndexOf(rowIndex) != -1) return;

                    for (var i = 0; i < targetRow.Cells.Count; i++)
                    {
                        if (i == INDEX_STATE && (_channels[rowIndex].State == McQLib.Device.State.RUN ||
                                                 _channels[rowIndex].State == McQLib.Device.State.ERROR ||
                                                 _channels[rowIndex].State == McQLib.Device.State.SAFETY)) continue;

                        targetRow.Cells[i].Style.BackColor = Color.LightGreen;
                    }
                    _selectedIndices.Add(rowIndex);
                }
                else
                {
                    if (_selectedIndices.IndexOf(rowIndex) == -1) return;

                    for (var i = 0; i < targetRow.Cells.Count; i++)
                    {
                        if (i == INDEX_STATE && (_channels[rowIndex].State == McQLib.Device.State.RUN ||
                                                 _channels[rowIndex].State == McQLib.Device.State.ERROR ||
                                                 _channels[rowIndex].State == McQLib.Device.State.SAFETY)) continue;

                        targetRow.Cells[i].Style.BackColor = Color.Empty;
                    }
                    _selectedIndices.Remove(rowIndex);
                }

                _selectedIndices.Sort();
            }
            
        }

        int lastClickedRow = -1;
        private void dataGridView1_CellMouseClick( object sender, DataGridViewCellMouseEventArgs e )
        {
            if ( e.RowIndex == -1 ) return;

            var shiftDown = ( ModifierKeys & Keys.Shift ) != 0;
            var ctrlDown = ( ModifierKeys & Keys.Control ) != 0;

            // Shift & Ctrl - 현재 선택된 개체 + 마지막 클릭된 개체부터 현재 클릭 개체까지 (마지막 클릭 개체 갱신 x)
            if ( shiftDown && ctrlDown )
            {
                if ( lastClickedRow == -1 )
                {
                    // Do nothing but refreshing lastClickedRow
                    //lastClickedRow = e.RowIndex;
                    return;
                }

                var start = lastClickedRow;
                var end = e.RowIndex;
                if ( start > end )
                {
                    var tmp = start;
                    start = end;
                    end = tmp;
                }

                for ( var i = start; i <= end; i++ )
                {
                    selectRow( i, true );
                }
            }
            // Shift Only - 마지막 클릭했던 개체부터 현재 클릭 개체까지 (마지막 클릭 개체 갱신 x)
            else if ( shiftDown )
            {
                if ( lastClickedRow == -1 )
                {
                    selectRow( e.RowIndex, true );
                    lastClickedRow = e.RowIndex;
                    return;
                }

                var start = lastClickedRow;
                var end = e.RowIndex;
                if ( start > end )
                {
                    var tmp = start;
                    start = end;
                    end = tmp;
                }

                for ( var i = 0; i < dataGridView1.Rows.Count; i++ )
                {
                    selectRow( i, start <= i && i <= end );
                }
            }
            // Ctrl Only - 현재 선택된 개체 ± 현재 클릭 개체 (마지막 클릭 개체 갱신 o)
            else if ( ctrlDown )
            {
                lastClickedRow = e.RowIndex;

                selectRow( lastClickedRow, _selectedIndices.IndexOf( lastClickedRow ) == -1 );
            }
            // 현재 클릭 개체 ±
            else
            {
                for ( var i = 0; i < dataGridView1.Rows.Count; i++ )
                {
                    selectRow( i, i == e.RowIndex );
                }

                lastClickedRow = e.RowIndex;
            }

            SelectedIndexChanged?.Invoke( this, new EventArgs() );
        }

        private void dataGridView1_ColumnWidthChanged( object sender, DataGridViewColumnEventArgs e )
        {
            for ( var i = 0; i < dataGridView1.Columns.Count; i++ )
            {
                SoftwareConfiguration.LIST.ColumnSizes[i] = dataGridView1.Columns[i].Width;
            }
        }

        private void menu_ShowInBrowser_Click( object sender, EventArgs e )
        {
            if ( rightDownIndex == -1 ) return;

            var directory = _channels[rightDownIndex].SaveDirectory;

            if ( string.IsNullOrEmpty( directory?.Trim() ) || !Directory.Exists( directory ) )
            {
                MessageBox.Show( "지정된 경로를 찾을 수 없습니다.", "Q730 알림 메시지" );
                return;
            }

            Process.Start( _channels[rightDownIndex].SaveDirectory );
        }

        int rightDownIndex = -1;
        private void dataGridView1_CellMouseDown( object sender, DataGridViewCellMouseEventArgs e )
        {
            if ( e.Button == MouseButtons.Right )
            {
                rightDownIndex = e.RowIndex;

                for ( var i = 0; i < _channels.Length; i++ )
                {
                    selectRow( i, false );
                }
                selectRow( rightDownIndex, true );
                SelectedIndexChanged?.Invoke( this, new EventArgs() );
            }
        }
        

        private void toolStripMenuItem_ShowInViewer_Click(object sender, EventArgs e)
        {
            if (rightDownIndex == -1)
            {
                return;
            }
            DataViewer.Form_Main dataViewer = new DataViewer.Form_Main(_channels[rightDownIndex].Sequence);

            if (rightDownIndex > -1 && _channels[rightDownIndex].SaveDirectory != null)
            {
                string folderPath = _channels[rightDownIndex].SaveDirectory;
                string searchFileName = $"{_channels[rightDownIndex].Name}_CH{rightDownIndex+1}_";
                
                string[] filePaths = Directory.GetFiles(folderPath, "*" + searchFileName + "*");

                if (filePaths.Length > 0)
                {
                    // 최신 파일을 찾기 위해 파일들을 날짜순으로 정렬합니다.
                    Array.Sort(filePaths, (a, b) => File.GetLastWriteTime(b).CompareTo(File.GetLastWriteTime(a)));
                }

                if (filePaths.Length > 1)
                {
                    Array.Resize(ref filePaths, 1);
                }

                Form_PresetSelectBox formPreset = new Form_PresetSelectBox();
                var result = formPreset.ShowDialog();

                if (result == DialogResult.OK)
                {
                    dataViewer.presetPath = formPreset.presetFilePath;
                    dataViewer.FileLoad(filePaths);
                    dataViewer.TextPreset = formPreset.presetFileName;

                    dataViewer.Show();
                }
            }
          
        }

        private void toolStripMenuItem_TextExport_Click( object sender, EventArgs e )
        {
            if ( rightDownIndex == -1 ) return;

            if ( _channels[rightDownIndex].State != McQLib.Device.State.IDLE )
            {
                MessageBox.Show( "내보내기 구성은 채널이 측정중이 아닐 때만 설정할 수 있습니다.", "Q730 알림 메시지" );
                return;
            }

            


            using ( var dialog = new Form_TextExportSetting( rightDownIndex, _channels[rightDownIndex].ExportFilePath ) )
            {
                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    if ( dialog.Use )
                    {
                        _channels[rightDownIndex].ExportFilePath = dialog.Path;
                    }
                    else
                    {
                        _channels[rightDownIndex].ExportFilePath = string.Empty;
                    }
                }
            }
        }
    }
}
