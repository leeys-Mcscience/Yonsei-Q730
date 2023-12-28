using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using McQLib.Core;
using McQLib.Device;
using McQLib.Recipes;
using Q730.UserControls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading;
using System.Reflection;
using System.Text;
using Microsoft.Win32;

namespace Q730
{
    public partial class Form_Main : Form
    {
        #region Base
        private Communicator[] _communicators = new Communicator[0];

        private McQLib.Developer.DevelopConsole console;

        private UserControl_ChannelListView channelList;
        //private UserControl_ChannelDetailView channelDetail;
        private UserControl_ChannelGridView channelGrid;

        private UserControl_GraphGridView channelGraphGrid = new UserControl_GraphGridView()
        {
            Dock = DockStyle.Fill
        };
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style &= ~0xC00000;
                return cp;
            }
        }
        private bool needShowPatchNote;

        public Form_Main()
        {
            InitializeComponent();

            if ( !SoftwareConfiguration.Load() ) SoftwareConfiguration.Save();
            if ( SoftwareConfiguration.Common.LastPatchNoteCreationTime != new FileInfo( "PatchNote.txt" ).LastWriteTime.ToString( "yyyyMMddHHmmss" ) )
            {
                //needShowPatchNote = true;
            }

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

            Application.Idle += Application_Idle;
        }
        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
               
            }
            else if (e.Mode == PowerModes.Resume)
            {
                if (MessageBox.Show("PC가 절전 모드에서 복귀되었습니다 ok버튼을 눌러주세요?", "Q730 알림 메시지", MessageBoxButtons.YesNo) ==
                    DialogResult.Yes)
                {
                    connectProtocol();
                    Thread.Sleep(10000);
                    connectProtocol();
                }
            }
            else if (e.Mode == PowerModes.StatusChange)
            {
            
            }
        }

        private void Application_Idle( object sender, EventArgs e )
        {
            Application.Idle -= Application_Idle;

            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

            tableLayoutPanel1.Controls.Add( new UserControl_CaptionBar(), 0, 0 );
            Size = new Size( SoftwareConfiguration.Common.Width, SoftwareConfiguration.Common.Height );

            ClientSizeChanged += Form_Main_ClientSizeChanged;

            Application.DoEvents();

            //checkSystem();

            _logPath = Path.Combine( Application.StartupPath, "Log", $"Log_{DateTime.Now:yyyyMMdd}.log" );
            logWriter = new FileStream( _logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite );

            logging( "Software start." );

            DeviceConfiguration.Load();

            initializeCommunicators();
            Communicator.LoadChannelInfos();

            channelList = new UserControl_ChannelListView() { Dock = DockStyle.Fill };
            //channelDetail = new UserControl_ChannelDetailView() { Dock = DockStyle.Fill };
            channelGrid = new UserControl_ChannelGridView() { Dock = DockStyle.Fill };

            channelList.BindChannels( Communicator.TotalChannels );
            channelGrid.BindChannels( Communicator.TotalChannels );
            //channelDetail.BindChannels( Communicator.TotalChannels );

            if ( _selectedTabIndex == LIST ) channelList.Start();
            else if ( _selectedTabIndex == GRID ) channelGrid.Start();
            //else if ( _selectedTabIndex == DETAIL ) channelDetail.Start();

            refreshSequenceList();

            if ( !RecipeSetting.Load() ) RecipeSetting.Save();

            console = new McQLib.Developer.DevelopConsole();
            console.Communicators = _communicators;
            console.MainForm = this;

            channelList.SelectedIndexChanged += selectedChannelChanged;
            //channelDetail.SelectedChannelChanged += selectedChannelChanged;

            channelGrid.ChannelDoubleClick += channelDoubleClick;
            channelList.ChannelDoubleClick += channelDoubleClick;

            tableLayoutPanel5.Controls.Add( channelList, 0, 1 );

            //channelDetail.Viewer = userControl_SequenceViewer1;

            channelList.Start();

#if DEBUG
            if ( needShowPatchNote )
            {
                using ( var dialog = new Form_PatchNote() )
                {
                    dialog.ShowDialog();
                }
            }
#endif
        }
        #endregion

        #region Sub Functions
        private void initializeCommunicators()
        {
            //channelDetail?.Stop();
            channelGrid?.Stop();
            channelList?.Stop();
            //channelGraphGrid.Stop();

            if ( _communicators.Length != 0 )
            {
                for ( var i = 0; i < _communicators.Length; i++ )
                {
                    _communicators[i]?.Dispose();
                }
            }

            _communicators = new Communicator[DeviceConfiguration.Devices.Count];
            for ( var i = 0; i < _communicators.Length; i++ )
            {
                _communicators[i] = new Communicator( i, DeviceConfiguration.Devices[i].ChannelCount )
                {
                    IP = DeviceConfiguration.Devices[i].IP,
                    Logging = true,
                    IsAutoPing = false,
                    ActionLog = logging
                };

            }
        }
        private void checkSystem()
        {
            // 소프트웨어 버전 정보
            label_Version.Text = $"Software Version {Assembly.GetEntryAssembly().GetName().Version.ToString( 3)}\n" +
                                 $"Firmware Version {_communicators[0]._versionInfo}";
     
            

            // 시퀀스 파일 경로 검사
            if ( !Directory.Exists( Path.Combine( Application.StartupPath, "Sequence" ) ) ) Directory.CreateDirectory( Path.Combine( Application.StartupPath, "Sequence" ) );
            if ( !Directory.Exists( Path.Combine( Application.StartupPath, "Log" ) ) ) Directory.CreateDirectory( Path.Combine( Application.StartupPath, "Log" ) );
        }

        private string _logPath = null;
        private FileStream logWriter;
        private void logging( string msg )
        {
            var logMessage = $"[{DateTime.Now:HH:mm:ss.fff}] {msg}{Environment.NewLine}";

            var bytes = Encoding.ASCII.GetBytes( logMessage );
            lock ( logWriter )
            {
                logWriter.Write( bytes, 0, bytes.Length );
                logWriter.Flush();
            }
        }

        private string[] _tempList;
        private string[] loadSequenceList()
        {
            // 매 1초마다 Default Sequence Path의 파일 목록이 현재 시퀀스 빌더의 시퀀스 리스트와 일치하는지 검사하여, 실제 파일 목록과 일치하지 않는다면 갱신한다.
            var files = new DirectoryInfo( Sequence.DefaultDirectory ).GetFiles( "*.seq" );

            var list = new List<string>();

            list.AddRange( files.Select( i =>
            {
                return i.FullName;
            } ) );

            return list.ToArray();
        }
        private string[] loadSequenceList(string path)
        {
            // 매 1초마다 Default Sequence Path의 파일 목록이 현재 시퀀스 빌더의 시퀀스 리스트와 일치하는지 검사하여, 실제 파일 목록과 일치하지 않는다면 갱신한다.
            var files = new DirectoryInfo(path).GetFiles("*.seq");

            var list = new List<string>();

            list.AddRange(files.Select(i =>
            {
                return i.FullName;
            }));

            return list.ToArray();
        }

        int _selectedTabIndex = 0;
        private const int LIST = 0;
        private const int GRID = 1;
        private const int DETAIL = 2;
        private const int GRAPH = 4;

        private void channelDoubleClick( object sender, EventArgs e )
        {
            if ( sender is UserControl_ChannelGridView grid )
            {
                if ( e is DataGridViewCellEventArgs args )
                {
                    var row = args.RowIndex;
                    var col = args.ColumnIndex;

                    radioButton_Detail.PerformClick();
                    //channelDetail.SelectChannel( row * 8 + col );

                }
            }
            else if ( sender is UserControl_ChannelListView list )
            {
                if ( e is DataGridViewCellEventArgs args )
                {
                    var row = args.RowIndex;

                    //channelDetail.SelectChannel( row );
                    radioButton_Detail.PerformClick();
                }
            }
        }

        private int[] _selectedChannels = new int[0];
        private void selectedChannelChanged( object sender, EventArgs e )
        {
            if ( Communicator.TotalChannels.Length == 0 )
            {
                return;
            }

            if ( sender is UserControl_ChannelListView list )
            {
                _selectedChannels = list.SelectedIndices;

                if ( _selectedChannels.Length == 0 ) return;

                var recipeFilePath = Communicator.TotalChannels[_selectedChannels[0]]?.Sequence?.FilePath;
                var saveDirectory = Communicator.TotalChannels[_selectedChannels[0]]?.SaveDirectory;
                var name = Communicator.TotalChannels[_selectedChannels[0]]?.Name;

                for ( var i = 1; i < _selectedChannels.Length; i++ )
                {
                    if ( recipeFilePath != Communicator.TotalChannels[_selectedChannels[i]]?.Sequence?.FilePath )
                    {
                        recipeFilePath = null;
                    }

                    if ( saveDirectory != Communicator.TotalChannels[_selectedChannels[i]]?.SaveDirectory )
                    {
                        saveDirectory = null;
                    }

                    if ( name != Communicator.TotalChannels[_selectedChannels[i]]?.Name )
                    {
                        name = null;
                    }
                }

                if ( recipeFilePath == null )
                {
                    userControl_SequenceViewer1.SetSequence( null );
                    comboBox_SequenceList.SelectedIndex = -1;
                }
                else if ( userControl_SequenceViewer1.GetSequence() == null || userControl_SequenceViewer1.GetSequence().FilePath != recipeFilePath )
                {
                    userControl_SequenceViewer1.SetSequence( Sequence.FromFile( recipeFilePath ) );
                    comboBox_SequenceList.SelectedIndex = comboBox_SequenceList.Items.IndexOf( userControl_SequenceViewer1.GetSequence().Name );
                }
                else if ( userControl_SequenceViewer1.GetSequence() != null && comboBox_SequenceList.SelectedIndex == -1 )
                {
                    comboBox_SequenceList.SelectedIndex = comboBox_SequenceList.Items.IndexOf( userControl_SequenceViewer1.GetSequence().Name );
                }

                textBox_Path.Text = saveDirectory == null ? string.Empty : saveDirectory;
                textBox_Name.Text = name == null ? string.Empty : name;
            }
            else if ( sender is UserControl_ChannelDetailView detail )
            {
                _selectedChannels = new int[1] { detail.SelectedIndex };
                userControl_SequenceViewer1.SetSequence( Communicator.TotalChannels[_selectedChannels[0]]?.Sequence, true );
            }
        }

        public bool TestSwStart = false;
        public void startTestSw()
        {
            var isConnected = false;
            for ( var i = 0; i < _communicators.Length; i++ )
            {
                if ( _communicators[i].IsConnected )
                {
                    isConnected = true;
                    break;
                }
            }

            if ( isConnected )
            {
                if ( MessageBox.Show( "테스트 모드를 실행하면 현재 UI는 종료되고, 장비 연결이 해제됩니다.\r\n" +
                                     "테스트 모드를 실행하시겠습니까?", "Q730 알림 메시지", MessageBoxButtons.YesNo ) == DialogResult.No )
                {
                    return;
                }

                timer_ConnectionChecker.Stop();
                for ( var i = 0; i < _communicators.Length; i++ )
                {
                    _communicators[i].Dispose();
                }
            }

            TestSwStart = true;
            Close();
        }
        #endregion

        #region Main Button Events
        private bool _connection = false;

        private void connectProtocol()
        {
            if (_communicators.Length == 0)
            {
                MessageBox.Show("연결 가능한 장비가 존재하지 않습니다. 먼저 장비 구성 설정에서 연결할 장비를 추가하십시오.", "Q730 알림 메시지");
                return;
            }


            if (_connection)
            {
                if (Communicator.IsRunInclude)
                {
                    if (MessageBox.Show("측정이 종료되지 않은 채널이 있습니다. 이대로 연결을 해제하더라도 장비에서의 측정은 계속 진행됩니다. 정말 연결을 해제하시겠습니까?", "Q730 알림 메시지", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    if (MessageBox.Show("정말 연결을 해제하시겠습니까?", "Q730 알림 메시지", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                }

                // 연결 해제 시도
                // 연결 해제는 무조건적으로 해제한다.
                label1.Text = "해제중...";
                Application.DoEvents();

                timer_ConnectionChecker.Stop();

                for (var i = 0; i < _communicators.Length; i++)
                {
                    if (_communicators[i].IsConnected)
                    {
                        _communicators[i].Disconnect();
                    }
                }

                button_Connect.BackgroundImage = Properties.Resources.Icon_Disconnected_New;
                _connection = false;
                label1.Text = "연결";
            }
            else
            {
                var failCount = 0;
                var initErrors = new List<string>();

                // 연결 시도
                label1.Text = "연결중...";
                Application.DoEvents();

                for (var i = 0; i < _communicators.Length; i++)
                {
                    if (!_communicators[i].IsConnected)
                    {
                        if (_communicators[i].Connect())
                        {
                            try
                            {
                                _communicators[i].InitializeCommunicator();
                                initErrors.Add("연결 성공.");
                            }
                            catch (QException ex)
                            {
                                logging($"Dev{i}==> Init failed. (Cause: {ex.Message})");

                                switch (ex.QExceptionType)
                                {
                                    case QExceptionType.REGISTER_INITIAL_FAIL_ERROR:
                                        initErrors.Add("초기화 실패. 장비를 재시동해주십시오. 증상이 반복된다면 A/S 문의를 요청해 주십시오.");
                                        break;

                                    case QExceptionType.ACTION_DEVICE_NOT_RESPONSE_ERROR:
                                    case QExceptionType.COMMUNICATION_COMMANDING_FAILED_ERROR:
                                        initErrors.Add("초기화 실패. 장비가 응답하지 않습니다.");
                                        break;

                                    case QExceptionType.COMMUNICATION_CANNOT_READ_BOARDINFO_ERROR:
                                    case QExceptionType.PACKET_BOARD_INFORMATION_DATAFIELD_LENGTH_ERROR:
                                        initErrors.Add("초기화 실패. 보드 정보 조회에 실패했습니다.");
                                        break;

                                    default:
                                        initErrors.Add("초기화 실패. 알 수 없는 원인으로 연결에 실패했습니다.");
                                        break;
                                }

                                _communicators[i].Disconnect();
                                failCount++;
                                continue;
                            }
                            _communicators[i].CheckAppendingData();
                        }
                        else
                        {
                            initErrors.Add("연결 실패. 장비의 전원이 켜져있는지 또는 IP 설정이 올바른지 확인해 주십시오.");
                            failCount++;
                        }
                    }
                }

                if (failCount < _communicators.Length)
                {
                    //loadChannelInfos();
                    label1.Text = "연결됨";
                    button_Connect.BackgroundImage = Properties.Resources.Icon_Connected_New;
                    _connection = true;
                    timer_ConnectionChecker.Start();
                }
                else
                {
                    label1.Text = "연결";
                    button_Connect.BackgroundImage = Properties.Resources.Icon_Disconnected_New;
                    _connection = false;
                }

                if (failCount != 0)
                {
                    var message = $"{_communicators.Length}개의 장비 중 {failCount}개의 장비에서 연결에 실패했습니다.";
                    for (var i = 0; i < initErrors.Count; i++)
                    {
                        message += $"\r\n{i + 1} : {initErrors[i]}";
                    }

                    MessageBox.Show(message, "Q730 알림 메시지");
                }
            }
        }

        private void button_Connect_Click( object sender, EventArgs e )
        {

            connectProtocol();
            checkSystem();
        }

        private async void button_Start_Click( object sender, EventArgs e )
        {
            if ( _selectedChannels == null )
            {
                MessageBox.Show( "선택된 채널이 없습니다.", "Q730 알림 메시지" );
                return;
            }

            switch ( _selectedTabIndex )
            {
                case LIST:
                case DETAIL:
                    var error_SavePath = new List<int>();
                    var error_Sequence = new List<int>();
                    var error_SendPacket = new List<string>();
                    var error_Connection = new List<int>();

                    var task = new Task( new Action( delegate ()
                    {
                        for ( var i = 0; i < _selectedChannels.Length; i++ )
                        {
                            var ch = Communicator.TotalChannels[_selectedChannels[i]];
                            if ( !ch.Owner.IsConnected )
                            {
                                error_Connection.Add( _selectedChannels[i] );
                                continue;
                            }

                            switch ( ch.State )
                            {
                                case State.IDLE:
                                    if ( Sequence.IsNullOrEmpty( ch.Sequence ) )
                                    {
                                        error_Sequence.Add( _selectedChannels[i] );
                                    }
                                    else if ( string.IsNullOrEmpty( ch.SaveDirectory ) )
                                    {
                                        error_SavePath.Add( _selectedChannels[i] );
                                    }
                                    else
                                    {
                                        ch.ChannelCommand = ChannelCommand.Start;
                                    }
                                    break;

                                case State.READY:
                                    break;

                                case State.SAFETY:
                                    ch.ChannelCommand = ChannelCommand.SafetyRestart;
                                    break;

                                case State.PAUSED:
                                    // 재시작
                                    ch.ChannelCommand = ChannelCommand.Restart;
                                    break;

                                case State.DISCONNECTED:
                                    error_Connection.Add( _selectedChannels[i] );
                                    break;

                                case State.ERROR:
                                    ch.ChannelCommand = ChannelCommand.Start;
                                    break;
                            }

                            Application.DoEvents();
                        }
                    } ) );

                    task.Start();
                    await task;

                    var totalErrorCount = error_SavePath.Count + error_Sequence.Count + error_SendPacket.Count + error_Connection.Count;
                    if ( totalErrorCount != 0 )
                    {
                        var errorMessage = $"{_selectedChannels.Length}개의 채널 중 {totalErrorCount}개의 채널에서 요청한 명령을 수행하지 못했습니다.";

                        if ( error_SavePath.Count != 0 )
                        {
                            errorMessage += $"\r\n\r\n저장 경로가 지정되지 않았거나 올바른 경로가 아닙니다. ({error_SavePath.Count}개) :";
                            for ( var i = 0; i < error_SavePath.Count; i++ )
                            {
                                errorMessage += $" CH{error_SavePath[i] + 1}";
                            }
                        }
                        if ( error_Sequence.Count != 0 )
                        {
                            errorMessage += $"\r\n\r\n시퀀스가 지정되지 않았거나 지정된 시퀀스가 빈 시퀀스입니다. ({error_Sequence.Count}개) :";
                            for ( var i = 0; i < error_Sequence.Count; i++ )
                            {
                                errorMessage += $" CH{error_Sequence[i] + 1}";
                            }
                        }
                        if ( error_SendPacket.Count != 0 )
                        {
                            errorMessage += $"\r\n\r\n통신 과정에서 문제가 발생했습니다. ({error_SendPacket.Count}개) :";
                            for ( var i = 0; i < error_SendPacket.Count; i++ )
                            {
                                errorMessage += $" CH{error_SendPacket[i]}";
                            }
                        }
                        if ( error_Connection.Count != 0 )
                        {
                            errorMessage += $"\r\n\r\n연결되지 않은 채널입니다. ({error_Connection.Count}개) :";
                            for ( var i = 0; i < error_Connection.Count; i++ )
                            {
                                errorMessage += $" CH{error_Connection[i]}";
                            }
                        }

                        MessageBox.Show( errorMessage, "Q730 알림 메시지" );
                    }
                    break;

                default:
                    MessageBox.Show( "List 또는 Detail에서 동작을 수행할 채널을 선택해야 합니다.", "Q730 알림 메시지" );
                    break;
            }
        }
        async private void button_Pause_Click( object sender, EventArgs e )
        {
            switch ( _selectedTabIndex )
            {
                case LIST:
                case DETAIL:
                    var error_SendPacket = new List<string>();
                    var error_Connection = new List<int>();

                    var task = new Task( new Action( delegate ()
                    {
                        for ( var i = 0; i < _selectedChannels.Length; i++ )
                        {
                            var ch = Communicator.TotalChannels[_selectedChannels[i]];

                            if ( !ch.Owner.IsConnected )
                            {
                                error_Connection.Add( _selectedChannels[i] );
                                continue;
                            }

                            switch ( ch.State )
                            {
                                case State.RUN:
                                    ch.ChannelCommand = ChannelCommand.Pause;
                                    break;
                            }

                            Application.DoEvents();
                        }
                    } ) );

                    task.Start();
                    await task;

                    var totalErrorCount = error_SendPacket.Count + error_Connection.Count;
                    if ( totalErrorCount != 0 )
                    {
                        var errorMessage = $"{_selectedChannels.Length}개의 채널 중 {totalErrorCount}개의 채널에서 요청한 명령을 수행하지 못했습니다.";

                        if ( error_SendPacket.Count != 0 )
                        {
                            errorMessage += $"\r\n\r\n통신 과정에서 문제가 발생했습니다. ({error_SendPacket.Count}개) :";
                            for ( var i = 0; i < error_SendPacket.Count; i++ )
                            {
                                errorMessage += $" CH{error_SendPacket[i]}";
                            }
                        }
                        if ( error_Connection.Count != 0 )
                        {
                            errorMessage += $"\r\n\r\n연결되지 않은 장비에 속한 채널입니다. ({error_Connection.Count}개) :";
                            for ( var i = 0; i < error_Connection.Count; i++ )
                            {
                                errorMessage += $" CH{error_Connection[i]}";
                            }
                        }

                        MessageBox.Show( errorMessage, "Q730 알림 메시지" );
                    }
                    break;

                default:
                    MessageBox.Show( "List 또는 Detail에서 동작을 수행할 채널을 선택해야 합니다.", "Q730 알림 메시지" );
                    break;
            }
        }
        async private void button_Stop_Click( object sender, EventArgs e )
        {
            // Stop의 경우에는 어떤 상태에서든 연결만 되어있다면 일단 Stop 명령을 날린다.
            // 혹시나 꼬여서 장비측 채널 상태와 소프트웨어측 채널 상태가 서로 다를 수 있으니.
            switch ( _selectedTabIndex )
            {
                case LIST:
                case DETAIL:
                    var error_SendPacket = new List<string>();
                    var error_Connection = new List<int>();
                    var check_Appending = new List<int>();

                    var task = new Task( new Action( delegate ()
                    {
                        for ( var i = 0; i < _selectedChannels.Length; i++ )
                        {
                            var ch = Communicator.TotalChannels[_selectedChannels[i]];

                            if ( !ch.Owner.IsConnected )
                            {
                                error_Connection.Add( _selectedChannels[i] );
                                continue;
                            }

                            switch ( ch.State )
                            {
                                case State.IDLE:
                                    if ( ch.ChannelCommand == ChannelCommand.Start )
                                    {
                                        ch.ChannelCommand = ChannelCommand.Idle;
                                    }
                                    else
                                    {
                                        ch.ChannelCommand = ChannelCommand.Stop;
                                    }
                                    break;

                                case State.READY:
                                    ch.ChannelCommand = ChannelCommand.Cancel;
                                    break;

                                case State.SENDING:
                                    ch.ChannelCommand = ChannelCommand.Idle;
                                    break;

                                case State.RUN:
                                case State.PAUSED:
                                case State.SAFETY:
                                    ch.ChannelCommand = ChannelCommand.Stop;
                                    break;

                                case State.APPENDING:
                                    check_Appending.Add( _selectedChannels[i] );
                                    break;

                                case State.ERROR:
                                    ch.ChannelCommand = ChannelCommand.ErrorClear;
                                    break;

                                default:
                                    ch.ChannelCommand = ChannelCommand.Idle;
                                    break;
                            }

                            Application.DoEvents();
                        }
                    } ) );

                    task.Start();
                    await task;

                    var totalErrorCount = error_SendPacket.Count + error_Connection.Count;
                    if ( totalErrorCount != 0 )
                    {
                        var errorMessage = $"{_selectedChannels.Length}개의 채널 중 {totalErrorCount}개의 채널에서 요청한 명령을 수행하지 못했습니다.";

                        if ( error_SendPacket.Count != 0 )
                        {
                            errorMessage += $"\r\n\r\n통신 과정에서 문제가 발생했습니다. ({error_SendPacket.Count}개) :";
                            for ( var i = 0; i < error_SendPacket.Count; i++ )
                            {
                                errorMessage += $" CH{error_SendPacket[i]}";
                            }
                        }
                        if ( error_Connection.Count != 0 )
                        {
                            errorMessage += $"\r\n\r\n연결되지 않은 장비에 속한 채널입니다. ({error_Connection.Count}개) :";
                            for ( var i = 0; i < error_Connection.Count; i++ )
                            {
                                errorMessage += $" CH{error_Connection[i]}";
                            }
                        }

                        MessageBox.Show( errorMessage, "Q730 알림 메시지" );
                    }

                    if ( check_Appending.Count != 0 )
                    {
                        if ( MessageBox.Show( "이어붙이기를 즉시 중단하려면 해당 채널이 속한 장비의 모든 채널의 동작을 중단해야 합니다. [예]를 누르면 측정이 진행중인 모든 채널의 측정이 중단되며, 저장되지 않은 데이터는 사라집니다. 계속하시겠습니까?", "Q730 알림 메시지", MessageBoxButtons.YesNo ) == DialogResult.Yes )
                        {
                            for ( var i = 0; i < check_Appending.Count; i++ )
                            {
                                Communicator.TotalChannels[_selectedChannels[i]].ChannelCommand = ChannelCommand.StopAppending;
                            }
                        }
                    }
                    break;

                default:
                    MessageBox.Show( "List 또는 Detail에서 동작을 수행할 채널을 선택해야 합니다.", "Q730 알림 메시지" );
                    break;
            }

        }
        async private void button_NextStep_Click( object sender, EventArgs e )
        {
            switch ( _selectedTabIndex )
            {
                case LIST:
                case DETAIL:
                    var error_SendPacket = new List<string>();
                    var error_Connection = new List<int>();

                    var task = new Task( new Action( delegate ()
                    {
                        // ChannelListView에서 여러 채널 선택 가능한 경우
                        for ( var i = 0; i < _selectedChannels.Length; i++ )
                        {
                            var ch = Communicator.TotalChannels[_selectedChannels[i]];
                            if ( !ch.Owner.IsConnected )
                            {
                                error_Connection.Add( _selectedChannels[i] );
                                continue;
                            }

                            switch ( ch.State )
                            {
                                case State.RUN:
                                case State.PAUSED:
                                    ch.ChannelCommand = ChannelCommand.Skip;
                                    Application.DoEvents();
                                    break;
                            }
                        }
                    } ) );

                    task.Start();
                    await task;

                    var totalErrorCount = error_SendPacket.Count + error_Connection.Count;
                    if ( totalErrorCount != 0 )
                    {
                        var errorMessage = $"{_selectedChannels.Length}개의 채널 중 {totalErrorCount}개의 채널에서 요청한 명령을 수행하지 못했습니다.";

                        if ( error_SendPacket.Count != 0 )
                        {
                            errorMessage += $"\r\n\r\n통신 과정에서 문제가 발생했습니다. ({error_SendPacket.Count}개) :";
                            for ( var i = 0; i < error_SendPacket.Count; i++ )
                            {
                                errorMessage += $" CH{error_SendPacket[i]}";
                            }
                        }
                        if ( error_Connection.Count != 0 )
                        {
                            errorMessage += $"\r\n\r\n연결되지 않은 장비에 속한 채널입니다. ({error_Connection.Count}개) :";
                            for ( var i = 0; i < error_Connection.Count; i++ )
                            {
                                errorMessage += $" CH{error_Connection[i]}";
                            }
                        }

                        MessageBox.Show( errorMessage, "Q730 알림 메시지" );
                    }
                    break;

                default:
                    MessageBox.Show( "List 또는 Detail에서 동작을 수행할 채널을 선택해야 합니다.", "Q730 알림 메시지" );
                    break;
            }
        }

        private void button_SequenceBuilder_Click( object sender, EventArgs e )
        {
            using ( var dialog = new Form_SequenceBuilder() )
            {
                dialog.ShowDialog();
            }
        }
        private void button_Configuration_Click( object sender, EventArgs e )
        {
            bool isConnected = false;
            for ( var i = 0; i < _communicators.Length; i++ )
            {
                if ( _communicators[i].IsConnected )
                {
                    isConnected = true;
                    break;
                }
            }

            using ( var dialog = new Form_Configuration( !isConnected ) )
            {
                if ( dialog.ShowDialog() == DialogResult.OK && dialog.IsModified )
                {
                    initializeCommunicators();

                    //channelDetail.BindChannels( Communicator.TotalChannels );
                    channelList.BindChannels( Communicator.TotalChannels );
                    channelGrid.BindChannels( Communicator.TotalChannels );

                    if ( _selectedTabIndex == LIST ) channelList.Start();
                    else if ( _selectedTabIndex == GRID ) channelGrid.Start();
                    //else if ( _selectedTabIndex == DETAIL ) channelDetail.Start();
                }
            }
        }
        #endregion

        #region Sub Events
        private void comboBox1_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( comboBox_SequenceList.SelectedIndex == -1 ) return;

            if ( comboBox_SequenceList.SelectedIndex == 0 )
            {
                using ( var dialog = new OpenFileDialog()
                {
                    Filter = "시퀀스 파일(*.seq)|*.seq",
                    InitialDirectory = Sequence.DefaultDirectory
                } )
                {
                    if ( dialog.ShowDialog() == DialogResult.OK )
                    {
                        userControl_SequenceViewer1.SetSequence( Sequence.FromFile( dialog.FileName ) );

                        var src = new FileInfo( dialog.FileName );
                        var originalString = dialog.SafeFileName;
                        var parts = originalString.Split('.');

                        textBox_Name.Text = parts[0];

                        if ( src.Directory.FullName == Sequence.DefaultDirectory ) return;

                        var always = false;
                        if ( SoftwareConfiguration.SequenceBuilder.CopySequence != Answer.AlwaysNo &&
                            ( SoftwareConfiguration.SequenceBuilder.CopySequence == Answer.AlwaysYes ||
                            CustomMessageBox.Show( "불러온 시퀀스 파일을 다음에도 사용하기 위해 기본 시퀀스 목록에 추가하시겠습니까?", "Q730 알림 메시지", MessageBoxButtons.YesNo, "이 메시지를 다시 표시 안 함", out always ) == DialogResult.Yes ) )
                        {
                            if ( always ) SoftwareConfiguration.SequenceBuilder.CopySequence = Answer.AlwaysYes;

                            var dest = new FileInfo( Path.Combine( Sequence.DefaultDirectory, src.Name ) );
                            if ( dest.Exists )
                            {
                                var srcRead = new StreamReader( src.FullName );
                                var destRead = new StreamReader( dest.FullName );

                                if ( srcRead.ReadToEnd() != destRead.ReadToEnd() )
                                {
                                    srcRead.Close();
                                    destRead.Close();

                                    if ( MessageBox.Show( "기본 시퀀스 목록에 동일한 이름이지만 다른 구성의 시퀀스 파일이 이미 존재합니다. 덮어쓸까요?", "Q730 알림 메시지", MessageBoxButtons.YesNo ) == DialogResult.Yes )
                                    {
                                        src.CopyTo( dest.FullName );
                                    }
                                }
                                else
                                {
                                    srcRead.Close();
                                    destRead.Close();

                                    if ( SoftwareConfiguration.SequenceBuilder.CopySequence != Answer.AlwaysYes ) MessageBox.Show( "이미 동일한 시퀀스가 존재합니다.", "Q730 알림 메시지" );
                                }
                            }
                            else
                            {
                                src.CopyTo( dest.FullName );
                            }
                        }
                        else
                        {
                            if ( always ) SoftwareConfiguration.SequenceBuilder.CopySequence = Answer.AlwaysNo;
                        }
                    }
                }
            }
            else
            {
                textBox_Name.Text = Sequence.FromFile(_tempList[comboBox_SequenceList.SelectedIndex - 1]).Name;
                userControl_SequenceViewer1.SetSequence( Sequence.FromFile( _tempList[comboBox_SequenceList.SelectedIndex - 1] ) );
            }
        }
        private void comboBox1_DropDown( object sender, EventArgs e )
        {
            refreshSequenceList();
        }
        private void refreshSequenceList()
        {
            comboBox_SequenceList.Items.Clear();
            comboBox_SequenceList.Items.Add("찾아보기...");

            if(textBox_SquencePath.Text != "")
            {
                _tempList = loadSequenceList(textBox_SquencePath.Text);
                comboBox_SequenceList.Items.AddRange(_tempList.Select(i =>
                {
                    var finfo = new FileInfo(i);
                    return finfo.Name.Replace(finfo.Extension, "");
                }).ToArray());
            }
            else
            {
                return;
            }
            //_tempList = loadSequenceList();

            
        }
        private void Form_Main_FormClosing( object sender, FormClosingEventArgs e )
        {
            var isConnected = false;
            for ( var i = 0; i < _communicators.Length; i++ )
            {
                if ( _communicators[i].IsConnected )
                {
                    isConnected = true;
                }
            }

            if ( isConnected )
            {
                if ( MessageBox.Show( "종료하시겠습니까?", "Q730 알림 메시지", MessageBoxButtons.YesNo ) != DialogResult.Yes )
                {
                    e.Cancel = true;
                    return;
                }
            }

            channelList.Stop();
            //channelDetail.Stop();
            channelGrid.Stop();
            channelGraphGrid.Stop();

            Communicator.SaveChannelInfos();
            for ( var i = 0; i < _communicators.Length; i++ )
            {
                _communicators[i].Dispose();
            }

            SoftwareConfiguration.Save();
            DeviceConfiguration.Save();
            RecipeSetting.Save();

            logging( "Software exit." + Environment.NewLine );
        }

        private void button_Path_Click( object sender, EventArgs e )
        {
            using ( var dialog = new CommonOpenFileDialog() )
            {
                dialog.IsFolderPicker = true;
                if ( textBox_Path.Text.Length == 0 && SoftwareConfiguration.Measurement.DefaultDirectory.Length != 0 && Directory.Exists( SoftwareConfiguration.Measurement.DefaultDirectory ) )
                {
                    dialog.InitialDirectory = SoftwareConfiguration.Measurement.DefaultDirectory;
                }
                else if ( textBox_Path.Text.Length != 0 && Directory.Exists( textBox_Path.Text ) ) dialog.InitialDirectory = textBox_Path.Text;

                if ( dialog.ShowDialog() == CommonFileDialogResult.Ok )
                {
                    textBox_Path.Text = dialog.FileName;
                }
            }
        }
        private void button_Apply_Click( object sender, EventArgs e )
        {
            if ( _selectedChannels.Length == 0 )
            {
                MessageBox.Show( "시퀀스를 적용할 채널을 먼저 선택하세요.", "Q730 알림 메시지" );
            }

            if (textBox_Name.Text == "")
            {
                MessageBox.Show("저장할 파일의 이름을 설정해주세요.", "Q730 알림 메시지");
                return;
            }
            else
            {
                for ( var i = 0; i < _selectedChannels.Length; i++ )
                {
                    switch ( Communicator.TotalChannels[_selectedChannels[i]].State )
                    {
                        case State.RUN:
                        case State.APPENDING:
                            continue;
                    }

                    Communicator.TotalChannels[_selectedChannels[i]].Sequence = userControl_SequenceViewer1.GetSequence();
                    Communicator.TotalChannels[_selectedChannels[i]].Name = textBox_Name.Text;
                    Communicator.TotalChannels[_selectedChannels[i]].SaveDirectory = textBox_Path.Text;
                }

                Communicator.SaveChannelInfos();
            }
        }
        private void radioButtons_Click( object sender, EventArgs e )
        {
            radioButton_List.ForeColor =
            radioButton_Grid.ForeColor =
            radioButton_Detail.ForeColor =
            radioButton_Graph.ForeColor = Color.FromArgb( 201, 201, 202 );

            //channelDetail.Stop();
            channelGrid.Stop();
            channelList.Stop();

            if ( radioButton_List.Checked )
            {
                if ( _selectedTabIndex != LIST )
                {
                    _selectedTabIndex = LIST;

                    tableLayoutPanel5.Controls.RemoveAt( 1 );
                    tableLayoutPanel5.Controls.Add( channelList, 0, 1 );


                    selectedChannelChanged( channelList, new EventArgs() );
                }
                radioButton_List.ForeColor = Color.Black;

                channelList.Start();
            }
            else if ( radioButton_Detail.Checked )
            {
                if ( _selectedTabIndex != DETAIL )
                {
                    //channelDetail.SelectChannel( channelDetail.SelectedIndex );

                    _selectedTabIndex = DETAIL;

                    tableLayoutPanel5.Controls.RemoveAt( 1 );
                    //tableLayoutPanel5.Controls.Add( channelDetail, 0, 1 );


                    //selectedChannelChanged( channelDetail, new EventArgs() );
                }
                radioButton_Detail.ForeColor = Color.Black;

                //channelDetail.Start();
            }
            else if ( radioButton_Grid.Checked )
            {
                if ( _selectedTabIndex != GRID )
                {
                    _selectedChannels = new int[0];

                    _selectedTabIndex = GRID;

                    tableLayoutPanel5.Controls.RemoveAt( 1 );
                    tableLayoutPanel5.Controls.Add( channelGrid, 0, 1 );

                }
                radioButton_Grid.ForeColor = Color.Black;

                channelGrid.Start();
            }
            else if ( radioButton_Graph.Checked )
            {
                //_selectedChannels = new int[0];
                //channelList.Stop();
                //channelDetail.Stop();
                //channelGrid.Stop();

                //_selectedTabIndex = GRAPH;

                //tableLayoutPanel5.Controls.RemoveAt( 1 );
                //tableLayoutPanel5.Controls.Add( channelGraphGrid, 0, 1 );

                //radioButton_Graph.ForeColor = Color.Black;
            }
        }
        private void Form_Main_ClientSizeChanged( object sender, EventArgs e )
        {
            SoftwareConfiguration.Common.Width = Width;
            SoftwareConfiguration.Common.Height = Height;
        }
        private void label_Version_DoubleClick( object sender, EventArgs e )
        {
#if DEBUG
            using ( var f = new Form_PatchNote() )
            {
                f.ShowDialog();
            }
#endif
        }
        private void channelGrid_CellDoubleClick( object sender, DataGridViewCellEventArgs e )
        {
            //channelDetail.SelectChannel( e.RowIndex * channelGrid.RowCol + e.ColumnIndex );
            //radioButton_Detail.Checked = true;
        }
        private void label_SdError_Click( object sender, EventArgs e )
        {
            var message = "장비에서 SD 카드 오류가 발생했습니다. 이어붙이기 기능이 정상적으로 동작하지 않을 수 있습니다.\r\n" +
                          "증상이 반복된다면 A/S를 문의하십시오.";

            for ( var i = 0; i < _communicators.Length; i++ )
            {
                if ( _communicators[i].SdFail )
                {
                    message += $"\r\nDevice {i + 1}";
                }
            }

            MessageBox.Show( message, "Q730 알림 메시지" );
        }
        #endregion

        #region Timer & Thread
        private void timer_ConnectionChecker_Tick( object sender, EventArgs e )
        {
            //return;

            for ( var i = 0; i < _communicators.Length; i++ )
            {
                if ( !_communicators[i].IsConnected )
                {
                    timer_ConnectionChecker.Stop();
                    _communicators[i].Disconnect();
                    button_Connect.BackgroundImage = Properties.Resources.Icon_Disconnected_New;

                    MessageBox.Show( "장비가 응답하지 않아 통신이 종료되었습니다.", "Q730 알림 메시지" );
                }
            }
        }
        private void timer_RunChecker_Tick( object sender, EventArgs e )
        {
            if ( _selectedChannels == null ) return;

            var errorExists = false;
            for ( var i = 0; i < _communicators.Length; i++ )
            {
                if ( _communicators[i].SdFail )
                {
                    errorExists = true;
                }
            }
            label_SdError.Visible = errorExists;

            var enabled = true;
            for ( var i = 0; i < _selectedChannels.Length; i++ )
            {
                if ( _selectedChannels[i] < 0 || _selectedChannels[i] >= Communicator.TotalChannels.Length ) continue;

                var ch = Communicator.TotalChannels[_selectedChannels[i]];

                if ( ch.State == State.RUN || ch.State == State.APPENDING || ch.State == State.SAFETY ||
                   ch.ChannelCommand != ChannelCommand.Idle )
                {
                    enabled = false;
                }
            }

            button_Path.Enabled = comboBox_SequenceList.Enabled = button_Apply.Enabled = textBox_Name.Enabled = enabled;
        }
        #endregion

        #region 전역 키 커멘드 이벤트
        public void startParser()
        {
            using ( var f = new PacketParser.Form1() )
            {
                f.ShowDialog();
            }
        }
        DateTime lastDDown;
        bool dFirst;
        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            Keys key = keyData & ~( Keys.Shift | Keys.Control );

            switch ( key )
            {
                case Keys.D:
                    if ( ( keyData & Keys.Control ) != 0 && ( keyData & Keys.Shift ) != 0 )
                    {
                        if ( !dFirst )
                        {
                            lastDDown = DateTime.Now;
                            dFirst = true;
                        }
                        else if ( dFirst && ( DateTime.Now - lastDDown ).TotalSeconds < 1 )
                        {
                            console.Show( "dvjm" );
                            dFirst = false;
                        }
                        else
                        {
                            lastDDown = DateTime.Now;
                        }
                    }
                    break;

                case Keys.Tab:
                    if ( ( keyData & Keys.Alt ) == 0 )
                    {
                        switch ( _selectedTabIndex )
                        {
                            case LIST:
                                radioButton_Grid.PerformClick();
                                break;

                            case GRID:
                                radioButton_Detail.PerformClick();
                                break;

                            case DETAIL:
                                radioButton_List.PerformClick();
                                break;
                        }

                        return true;
                    }
                    break;

            }

            return base.ProcessCmdKey( ref msg, keyData );
        }
        #endregion

   
        private void button_Custom_Load_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBox_SquencePath.Text = dialog.SelectedPath;
                    // 선택한 폴더 경로를 사용하여 다른 작업을 수행합니다.
                    var files = new DirectoryInfo(dialog.SelectedPath).GetFiles("*.seq");


                    //CustomPathSequences.Clear();
                    //DefaultPathSequences.Clear();
                    //listView_SequenceList.Items.Clear(); 필요없으.

                    comboBox_SequenceList.Items.Clear();
                    comboBox_SequenceList.Items.Add("찾아보기...");

                    _tempList = loadSequenceList(textBox_SquencePath.Text);

                    comboBox_SequenceList.Items.AddRange(files.Select(i =>
                    {
                        //var finfo = new FileInfo(i);
                        return i.Name.Replace(i.Extension, "");
                    }).ToArray());

                    //DefaultPathSequences.AddRange(files.Select(i =>
                    //{
                    //    listView_SequenceList.Items.Add(i.Name.Replace(i.Extension, ""));
                    //    //comboBox_SequenceList
                    //    return i.FullName;
                    //}));
                }
            }
        }

        private void resetButtonClick(object sender, EventArgs e)
        {
            _communicators[0].CheckAppendingData();
        }

        private void button_Read_Click(object sender, EventArgs e)
        {
            if (_selectedChannels.Length > 1)
            {
                return;
            }

            foreach (var t in _selectedChannels)
            {
                var ch = Communicator.TotalChannels[t];
                if (!ch.Owner.IsConnected)
                {
                    return;
                }

                if (ch.State == State.IDLE)
                {
                    var current = ch.Owner.ReadADC(ch.Address, 0);
                    ch.Current =  current;

                    var voltage = ch.Owner.ReadADC(ch.Address, 1);
                    ch.Voltage = voltage;

                }
            }
        }
    }
}