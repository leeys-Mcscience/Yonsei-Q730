using McQLib.Core;
using McQLib.Device;
using McQLib.IO;
using McQLib.Recipes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using Q730.UserControls.Graphs;

namespace Q730.UserControls
{
    public partial class UserControl_ChannelDetailView : UserControl
    {
        IGraphControl graph_Dc, graph_Tra, graph_Fra, graph_Acr, graph_Dcr, graph_Pattern, graph_Ca, graph_DisCa;
        string[] _indicators;
        System.Windows.Forms.Label[] _indicatorTitles;
        System.Windows.Forms.Label[] _indicatorValues;

        public UserControl_ChannelDetailView()
        {
            InitializeComponent();

            Dock = DockStyle.Fill;

            Application.Idle += Application_Idle;
        }

        void Application_Idle( object sender, EventArgs e )
        {
            Application.Idle -= Application_Idle;

            graph_Ca = new UserControl_ChargeCapacity();
            graph_DisCa = new UserControl_DisChargeCapacity();
            graph_Dc = new UserControl_DcGraph();
            graph_Tra = new UserControl_TraGraph();
            graph_Fra = new UserControl_FraAcrGraph();
            graph_Acr = new UserControl_FraAcrGraph();
            graph_Dcr = new UserControl_DcrGraph();
            graph_Pattern = new UserControl_PatternGraph();

            tableLayoutPanel_GraphSpace.Controls.Add( graph_Dc as Control, 0, 1 );

            _indicators = new string[] { SoftwareConfiguration.DETAIL.CustomIndicator1,
                                         SoftwareConfiguration.DETAIL.CustomIndicator2,
                                         SoftwareConfiguration.DETAIL.CustomIndicator3,
                                         SoftwareConfiguration.DETAIL.CustomIndicator4, };
            _indicatorTitles = new System.Windows.Forms.Label[] { label_CustomTitle1,
                                                                  label_CustomTitle2,
                                                                  label_CustomTitle3,
                                                                  label_CustomTitle4 };
            _indicatorValues = new System.Windows.Forms.Label[] { label_CustomValue1,
                                                                  label_CustomValue2,
                                                                  label_CustomValue3,
                                                                  label_CustomValue4 };

            SoftwareConfiguration.PropertyChanged += onConfigChanged;

            if ( _channels != null )
            {
                onConfigChanged( null, new PropertyChangedEventArgs( nameof( SoftwareConfiguration.Measurement.VoltageUnit ) ) );
                onConfigChanged( null, new PropertyChangedEventArgs( nameof( SoftwareConfiguration.Measurement.CurrentUnit ) ) );

            }
            onConfigChanged( null, new PropertyChangedEventArgs( nameof( SoftwareConfiguration.DETAIL.CustomIndicator1 ) ) );
            onConfigChanged( null, new PropertyChangedEventArgs( nameof( SoftwareConfiguration.DETAIL.CustomIndicator2 ) ) );
            onConfigChanged( null, new PropertyChangedEventArgs( nameof( SoftwareConfiguration.DETAIL.CustomIndicator3 ) ) );
            onConfigChanged( null, new PropertyChangedEventArgs( nameof( SoftwareConfiguration.DETAIL.CustomIndicator4 ) ) );
        }

        public UserControl_SequenceViewer Viewer { get; set; }

        [Browsable( true )]
        public event EventHandler SelectedChannelChanged;

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

            if ( _channels != null )
            {
                for ( var i = 0; i < _channels.Length; i++ )
                {
                    if ( _channels[i] != null ) _channels[i].PropertyChanged += onPropertyChanged;
                }
            }

            comboBox1.Items.Clear();
            button_After.Enabled = button_Before.Enabled = comboBox1.Enabled = false;

            if ( _channels != null && _channels.Length != 0 )
            {
                comboBox1.Items.AddRange( Enumerable.Range( 1, channels.Length ).Select( i => i.ToString() ).ToArray() );
                button_After.Enabled = button_Before.Enabled = comboBox1.Enabled = true;
            }
        }

        private Channel[] _channels;

        private int _index;
        public int SelectedIndex
        {
            get => _index;
            set
            {
                if ( _channels == null ) return;

                SelectChannel( value );
            }
        }
        /// <summary>
        /// 지정된 인덱스의 채널을 선택합니다.
        /// </summary>
        /// <param name="index"></param>
        public void SelectChannel( int index )
        {
            if ( 0 <= index && index < _channels.Length )
            {
                comboBox1.SelectedIndex = index;
            }
        }
        private void changeChannel( int index )
        {
            _index = index;

            SelectedChannelChanged?.Invoke( this, null );

            Viewer?.SetSequence( _channels[_index].Sequence, true );
            Viewer?.SelectItem( _channels[_index].State != State.RUN ? -1 : ( int )_channels[_index].StepNo );

            progressBar1.Maximum = _channels[_index].TotalSteps;
            if ( _channels[_index].StepCount <= progressBar1.Maximum ) progressBar1.Value = ( int )_channels[_index].StepCount;

            onPropertyChanged( null, new PropertyChangedEventArgs( nameof( Channel.TotalTime ) ) );
            onPropertyChanged( null, new PropertyChangedEventArgs( nameof( Channel.State ) ) );
            onConfigChanged( null, new PropertyChangedEventArgs( nameof( SoftwareConfiguration.DETAIL.CustomIndicator1 ) ) );
            onConfigChanged( null, new PropertyChangedEventArgs( nameof( SoftwareConfiguration.DETAIL.CustomIndicator2 ) ) );
            onConfigChanged( null, new PropertyChangedEventArgs( nameof( SoftwareConfiguration.DETAIL.CustomIndicator3 ) ) );
            onConfigChanged( null, new PropertyChangedEventArgs( nameof( SoftwareConfiguration.DETAIL.CustomIndicator4 ) ) );

            _points = new ConcurrentQueue<MeasureData[]>();

            clearAllGraphs();
        }

        void clearAllGraphs()
        {
            graph_Dc.ClearGraph();
            graph_Dc.RefreshGraph();

            graph_Ca.ClearGraph();
            graph_Ca.RefreshGraph();

            graph_DisCa.ClearGraph();
            graph_DisCa.RefreshGraph();
        }

        private string _lastFileName;

        public void Start()
        {
            //new Thread(updateLoop) { Name = "UpdateLoop", IsBackground = true }.Start();
            //new Thread(pointAddLoop) { Name = "PointAddLoop", IsBackground = true }.Start();
            //_isUpdateRun = true;
        }
        public void Stop()
        {
            //_updateLoopRun = false;
            //_pointAddRun = false;
            //_isUpdateRun = false;
        }

        private object getValue(object obj, string propName)=> obj.GetType().GetProperty(propName).GetValue(obj);
     
        private void onPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if ( !_isUpdateRun ) return;

            if ( InvokeRequired )
            {
                BeginInvoke( ( Action )( () => onPropertyChanged( sender, e ) ) );
                return;
            }
            int index;
            if (sender == null) return;
            if (sender != null) index = (sender as Channel).GlobalIndex;
            else index = 0;
            if ( index != _index ) return;

            switch ( e.PropertyName )
            {
                //case nameof( Channel.Voltage ):
                //    label_CustomValue2.Text =
                //    SoftwareConfiguration.Measurement.VoltageUnit.GetString( ( double )getValue( sender, e.PropertyName ) );
                //    break;

                //case nameof( Channel.Current ):
                //    label_CustomValue3.Text =
                //    SoftwareConfiguration.Measurement.CurrentUnit.GetString( ( double )getValue( sender, e.PropertyName ) );
                //    break;
                case nameof( Channel.State ):
                    label_State.Text = getValue( _channels[_index], nameof( Channel.State ) ).ToString();
                    break;

                case nameof( Channel.TotalTime ):
                    label_Time.Text =
                    Util.ConvertMsToString((ulong)getValue(sender, e.PropertyName));
                    break;

                case nameof( Channel.StepNo ):
                    //dataGridView1.Rows[index].Cells[INDEX_STEP_NO].Value = getValue( sender, e.PropertyName );
                    break;

                case nameof( Channel.StepCount ):
                case nameof( Channel.TotalSteps ):
                    var stepCount = ( int )( uint )getValue( sender, nameof( Channel.StepCount ) );
                    var totalSteps = ( int )getValue( sender, nameof( Channel.TotalSteps ) );
                    progressBar1.Maximum = totalSteps;
                    if ( stepCount >= 0 && stepCount <= progressBar1.Maximum )
                        progressBar1.Value = stepCount;
                    break;

                default:
                    for ( var i = 0; i < _indicators.Length; i++ )
                    {
                        if ( _indicators[i] == e.PropertyName )
                        {
                            _indicatorValues[i].Text = changeIndicatorValue( _indicators[i] );
                            return;
                        }
                    }
                    break;
            }
        }
        private void onConfigChanged( object sender, PropertyChangedEventArgs e )
        {
            switch ( e.PropertyName )
            {
                //label_CustomTitle2.Text = $"Voltage({SoftwareConfiguration.Measurement.VoltageUnit.UnitString})";
                //if ( _channels != null && 0 <= _index && _index < _channels.Length )
                //    label_CustomValue2.Text =
                //        SoftwareConfiguration.Measurement.VoltageUnit.GetString( ( double )getValue( _channels[_index], nameof( Channel.Voltage ) ) );
                //break;
                //label_CustomTitle3.Text = $"Current({SoftwareConfiguration.Measurement.CurrentUnit.UnitString})";
                //if ( _channels != null && 0 <= _index && _index < _channels.Length )
                //    label_CustomValue3.Text =
                //    SoftwareConfiguration.Measurement.CurrentUnit.GetString( ( double )getValue( _channels[_index], nameof( Channel.Current ) ) );
                //break;

                case nameof( SoftwareConfiguration.Measurement.VoltageUnit ):
                case nameof( SoftwareConfiguration.Measurement.CurrentUnit ):
                case nameof( SoftwareConfiguration.Measurement.PowerUnit ):
                    changeIndicator( SoftwareConfiguration.DETAIL.CustomIndicator1, 0 );
                    changeIndicator( SoftwareConfiguration.DETAIL.CustomIndicator2, 1 );
                    changeIndicator( SoftwareConfiguration.DETAIL.CustomIndicator3, 2 );
                    changeIndicator( SoftwareConfiguration.DETAIL.CustomIndicator4, 3 );
                    break;

                case nameof( SoftwareConfiguration.DETAIL.CustomIndicator1 ):
                    changeIndicator( SoftwareConfiguration.DETAIL.CustomIndicator1, 0 );
                    break;

                case nameof( SoftwareConfiguration.DETAIL.CustomIndicator2 ):
                    changeIndicator( SoftwareConfiguration.DETAIL.CustomIndicator2, 1 );
                    break;

                case nameof( SoftwareConfiguration.DETAIL.CustomIndicator3 ):
                    changeIndicator( SoftwareConfiguration.DETAIL.CustomIndicator3, 2 );
                    break;

                case nameof( SoftwareConfiguration.DETAIL.CustomIndicator4 ):
                    changeIndicator( SoftwareConfiguration.DETAIL.CustomIndicator4, 3 );
                    break;
            }
        }
        private void changeIndicator( string indicator, int index )
        {
            _indicators[index] = _indicatorTitles[index].Text = indicator;

            switch ( indicator )
            {
                case nameof( Channel.Voltage ):
                    _indicatorTitles[index].Text += $"({SoftwareConfiguration.Measurement.VoltageUnit.UnitString})";
                    _indicatorValues[index].Text = changeIndicatorValue( nameof( Channel.Voltage ) );
                    break;

                case nameof( Channel.Current ):
                    _indicatorTitles[index].Text += $"({SoftwareConfiguration.Measurement.CurrentUnit.UnitString})";
                    _indicatorValues[index].Text = changeIndicatorValue( nameof( Channel.Current ) );
                    break;

                case nameof( Channel.Temperature ):
                    _indicatorTitles[index].Text += $"(℃)";
                    _indicatorValues[index].Text = changeIndicatorValue( nameof( Channel.Temperature ) );
                    break;

                case nameof( Channel.Power ):
                    _indicatorTitles[index].Text += $"({SoftwareConfiguration.Measurement.PowerUnit.UnitString})";
                    _indicatorValues[index].Text = changeIndicatorValue( nameof( Channel.Power ) );
                    break;

                case nameof( Channel.Capacity ):
                    _indicatorTitles[index].Text += $"({SoftwareConfiguration.Measurement.CurrentUnit.UnitString}h)";
                    _indicatorValues[index].Text = changeIndicatorValue( nameof( Channel.Capacity ) );
                    break;
            }
        }
        private string changeIndicatorValue( string indicator )
        {
            if ( _channels == null || _channels.Length == 0 ) return string.Empty;

            switch ( indicator )
            {
                case nameof( Channel.Voltage ):
                    return SoftwareConfiguration.Measurement.VoltageUnit.GetString( ( double )getValue( _channels[_index], nameof( Channel.Voltage ) ) );

                case nameof( Channel.Current ):
                    return SoftwareConfiguration.Measurement.CurrentUnit.GetString( ( double )getValue( _channels[_index], nameof( Channel.Current ) ) );


                case nameof( Channel.Temperature ):
                    return _channels[_index].Temperature.ToString( "f1" );

                case nameof( Channel.Power ):
                    return SoftwareConfiguration.Measurement.PowerUnit.GetString( ( double )getValue( _channels[_index], nameof( Channel.Power ) ) );


                case nameof( Channel.Capacity ):
                    return SoftwareConfiguration.Measurement.CurrentUnit.GetString( ( double )getValue( _channels[_index], nameof( Channel.Capacity ) ) );

                default:
                    return "-";
            }
        }
        private void label_Custom_Click( object sender, EventArgs e )
        {
            var location = ( sender as System.Windows.Forms.Label ).Location;
            location.Y += ( sender as System.Windows.Forms.Label ).Height;

            var cur = Cursor.Position;
            var loc = new Point( cur.X - ParentForm.Location.X, cur.Y - ParentForm.Location.Y );

            var listBox = new ListBox();
            listBox.Items.AddRange( new object[] { "Voltage", "Current", "Temperature", "Power", "Capacity" } );
            listBox.SelectedIndex = listBox.Items.IndexOf( _indicators[int.Parse( ( sender as Control ).Tag.ToString() ) - 1] );
            listBox.Size = new Size( 100, 150 );
            listBox.Tag = ( sender as Control ).Tag;
            listBox.Leave += ( object s, EventArgs a ) =>
            {
                ParentForm.Controls.Remove( ( Control )s );
            };
            listBox.SelectedIndexChanged += ( object s, EventArgs a ) =>
            {
                var item = ( s as ListBox ).SelectedItem.ToString();
                var idx = int.Parse( ( s as Control ).Tag.ToString() );

                switch ( idx )
                {
                    case 1: SoftwareConfiguration.DETAIL.CustomIndicator1 = item; break;
                    case 2: SoftwareConfiguration.DETAIL.CustomIndicator2 = item; break;
                    case 3: SoftwareConfiguration.DETAIL.CustomIndicator3 = item; break;
                    case 4: SoftwareConfiguration.DETAIL.CustomIndicator4 = item; break;
                }

                ParentForm.Controls.Remove( ( Control )s );
            };

            listBox.Location = loc;
            ParentForm.Controls.Add( listBox );
            listBox.BringToFront();
            listBox.Focus();
        }

        private bool _isUpdateRun = false;
        private bool _updateLoopRun = false;
        private long _lastPosition = 0;
        private IGraphControl _saveGraphType;

        private void updateLoop()
        {
            if ( _updateLoopRun ) return; 

            _updateLoopRun = true;
            var lastIndex = _index;

            while ( _updateLoopRun && !IsDisposed )
            {
                Thread.Sleep( 100 );
                Application.DoEvents();

                if ( _isUpdateRun )
                {
                    if ( _channels == null || _channels.Length == 0 ) continue;

                    if ( _lastFileName != _channels[_index].SaveFileName )
                    {
                        // 불러올 파일의 경로가 바뀐 경우 - 새로운 측정임(그래프 초기화)
#if CONSOLEOUT
                        Console.WriteLine( $"File Changed : {_lastFileName} -> {_channels[_index].SaveFileName}" );
#endif

                        lastIndex = _index;
                        _lastFileName = _channels[_index].SaveFileName;
                        _lastPosition = 0;

                        clearAllGraphs();

                        Application.DoEvents();
                    }

                    else if (current != _saveGraphType)
                    {
                        lastIndex = _index;
                        _lastFileName = _channels[_index].SaveFileName;
                        _lastPosition = 0;

                        clearAllGraphs(); 

                        Application.DoEvents();
                    }

                    _saveGraphType = current;

                    if ( !File.Exists( Path.Combine( _channels[_index].SaveDirectory, _channels[_index].SaveFileName ) ) ) continue;

                    var reader = new QDataReader( Path.Combine( _channels[_index].SaveDirectory, _channels[_index].SaveFileName ) );
                    reader.Position = _lastPosition;

                    MeasureData data = null;
                    var tmpList = new List<MeasureData>();

                    // 데이터를 읽어서 _points에 Enqueue하는데, 한 번에 최대 5000개씩 Enqueue한다.
                    while ( ( data = reader.Read() ) != null && lastIndex == _index )
                    {
                        tmpList.Add( data );

                        reader.Skip(SoftwareConfiguration.DETAIL.SkipPoints); //20230710 Skip 10개 단위로 짤라서 보이는거 삭제 장재훈
                        if ( _points.Count == 0 )
                        {
                            _points.Enqueue( tmpList.ToArray() );
                            tmpList.Clear();
                        }

                        Application.DoEvents();
                    }

                    if ( lastIndex == _index )
                    {

                        if ( tmpList.Count != 0 )
                        {
                            _points.Enqueue( tmpList.ToArray() );
                        }

                        _lastPosition = reader.Position;
                    }
                    else
                    {
                        _points = new ConcurrentQueue<MeasureData[]>();
                    }
                }
                else
                {
                    // Pause -> Do nothing
                }
            }

            if ( !IsDisposed )
            {
                Invoke( new Action( delegate ()
                {
                    Viewer.Undetail();
                } ) );
            }
        }
        ConcurrentQueue<MeasureData[]> _points = new ConcurrentQueue<MeasureData[]>();
        private bool _pointAddRun = false;

        IGraphControl current = null;
        IGraphControl current2 = null;
        private void pointAddLoop()
        {
            if ( _pointAddRun ) return;
#if CONSOLEOUT
            Console.WriteLine( "PointAddLoop start." );
#endif
            _pointAddRun = true;

            var needUpdate = false;

            while ( _pointAddRun && !IsDisposed )
            {
                Application.DoEvents();

                if ( _points.TryDequeue( out MeasureData[] datas ) )
                {
                    var _channel = _channels[SelectedIndex];

                    for ( var i = 0; i < datas.Length; i++ )
                    {
                        switch ( datas[i].RecipeType )
                        {
                            case RecipeType.Rest:
                            case RecipeType.Charge:
                            case RecipeType.Discharge:
                            case RecipeType.OpenCircuitVoltage:
                            case RecipeType.AnodeCharge:
                            case RecipeType.AnodeDischarge:
                                if (radioButton_Dc.Checked && current != graph_Dc)
                                {
                                    Invoke( new Action( delegate ()
                                    {
                                        radioButton_Dc.PerformClick();
                                    } ) );
                                    current = graph_Dc;
                                }
                                else if(radioButton_charge.Checked && current != graph_Ca)
                                {
                                    Invoke(new Action(delegate ()
                                    {
                                        radioButton_charge.PerformClick();
                                    }));
                                    current = graph_Ca;
                                    current2 = graph_DisCa;
                                }
                                break;

                            case RecipeType.Pattern:
                                if ( current != graph_Pattern )
                                {
                                    Invoke( new Action( delegate ()
                                    {
                                        radioButton_Pattern.PerformClick();
                                    } ) );
                                    current = graph_Pattern;
                                }
                                break;
                        }
                        if(current == graph_Dc)
                        {
                            current?.AddData(datas[i]);
                        }
                        else if(current == graph_Ca)
                        {
                            current?.AddData(datas[i], _channel);
                            current2?.AddData(datas[i], _channel);
                        }
                        
                        needUpdate = true;
                    }

                    if ( needUpdate )
                    {
                        needUpdate = false;
                        current?.RefreshGraph();
                        current2?.RefreshGraph();
                        Application.DoEvents();
                    }
                }
                else
                {
                    if ( needUpdate )
                    {
                        needUpdate = false;
                        current?.RefreshGraph();
                        current2?.RefreshGraph();
                    }
                }
            }

#if CONSOLEOUT
            Console.WriteLine( "PointAddLoop stopped." );
#endif
        }

        private void radioButtons_Click( object sender, EventArgs e )
        {
            radioButton_charge.ForeColor =
            radioButton_Dc.ForeColor =
            radioButton_Fra.ForeColor =
            radioButton_Tra.ForeColor =
            radioButton_Dcr.ForeColor =
            radioButton_Acr.ForeColor =
            radioButton_Pattern.ForeColor = Color.FromArgb( 201, 201, 202 );

            if ( radioButton_Dc.Checked )
            {
                if(tableLayoutPanel_GraphSpace.Controls.Count > 2)
                {
                    tableLayoutPanel_GraphSpace.Controls.RemoveAt(2);
                }
                tableLayoutPanel_GraphSpace.Controls.RemoveAt(1);


                tableLayoutPanel_GraphSpace.Controls.Add(graph_Dc as Control, 1, 1);
                tableLayoutPanel_GraphSpace.SetRowSpan(graph_Dc as Control, 2);


                radioButton_Dc.ForeColor = Color.Black;
            }

            else if (radioButton_charge.Checked)
            {
                tableLayoutPanel_GraphSpace.Controls.RemoveAt(1);
                tableLayoutPanel_GraphSpace.Controls.Add(graph_Ca as Control, 0, 1);
                tableLayoutPanel_GraphSpace.Controls.Add(graph_DisCa as Control, 0, 2);


                radioButton_charge.ForeColor = Color.Black;
            }
            _points = new ConcurrentQueue<MeasureData[]>();

            clearAllGraphs();
            //else if ( radioButton_Fra.Checked )
            //{
            //    tableLayoutPanel_GraphSpace.Controls.RemoveAt( 1 );
            //    tableLayoutPanel_GraphSpace.Controls.Add( graph_Fra as Control, 0, 1 );

            //    radioButton_Fra.ForeColor = Color.Black;
            //}
            //else if ( radioButton_Tra.Checked )
            //{
            //    tableLayoutPanel_GraphSpace.Controls.RemoveAt( 1 );
            //    tableLayoutPanel_GraphSpace.Controls.Add( graph_Tra as Control, 0, 1 );

            //    radioButton_Tra.ForeColor = Color.Black;
            //}
            //else if ( radioButton_Dcr.Checked )
            //{
            //    tableLayoutPanel_GraphSpace.Controls.RemoveAt( 1 );
            //    tableLayoutPanel_GraphSpace.Controls.Add( graph_Dcr as Control, 0, 1 );

            //    radioButton_Dcr.ForeColor = Color.Black;
            //}
            //else if ( radioButton_Acr.Checked )
            //{
            //    tableLayoutPanel_GraphSpace.Controls.RemoveAt( 1 );
            //    tableLayoutPanel_GraphSpace.Controls.Add( graph_Acr as Control, 0, 1 );

            //    radioButton_Acr.ForeColor = Color.Black;
            //}
            //else if ( radioButton_Pattern.Checked )
            //{
            //    tableLayoutPanel_GraphSpace.Controls.RemoveAt( 1 );
            //    tableLayoutPanel_GraphSpace.Controls.Add( graph_Pattern as Control, 0, 1 );

            //    radioButton_Pattern.ForeColor = Color.Black;
            //}
        }

        private void button_After_Click( object sender, EventArgs e )
        {
            if ( comboBox1.SelectedIndex == comboBox1.Items.Count - 1 ) comboBox1.SelectedIndex = 0;
            else comboBox1.SelectedIndex++;
        }

        private void button_Before_Click( object sender, EventArgs e )
        {
            if ( comboBox1.SelectedIndex == 0 ) comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            else comboBox1.SelectedIndex--;
        }

        private void comboBox1_ValueChanged( object sender, EventArgs e )
        {
            if ( _channels?.Length == 0 ) return;

            var index = int.Parse( comboBox1.SelectedItem.ToString() ) - 1;

            if ( 0 <= index && index < _channels.Length )
            {
                changeChannel( index );
                Application.DoEvents();
            }
        }
    }
}
