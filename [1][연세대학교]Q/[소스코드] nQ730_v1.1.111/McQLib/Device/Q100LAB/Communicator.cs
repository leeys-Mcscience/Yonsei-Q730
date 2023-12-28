using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.UI.Design.WebControls;
using System.Windows.Forms;
using McQLib.Core;
using McQLib.Recipes;
using ZedGraph;

namespace McQLib.Device
{
    /// <summary>
    /// 장비와 통신이 필요한 모든 기능을 총괄하는 클래스입니다.
    /// </summary>
    public sealed class Communicator : IDisposable
    {
        #region Statics
        private static VersionInfo[] _compatibleFirmwareVersions = new VersionInfo[]
        {
            new VersionInfo(0, 42)
        };

        /// <summary>
        /// 생성된 모든 Communicator에 연결된 채널의 리스트입니다.
        /// </summary>
        public static Channel[] TotalChannels => _totalChannels.ToArray();
        private static readonly List<Channel> _totalChannels = new List<Channel>();

        /// <summary>
        /// 이어붙이기 기능을 사용할지의 여부입니다.
        /// <br>모든 Communicator에 일괄 적용됩니다.</br>
        /// </summary>
        public static bool Appending  { get; set; }
        public static bool OldSequenceSending { get; set; }

        public static bool IsRunInclude
        {
            get
            {
                for ( var i = 0; i < _totalChannels.Count; i++ )
                {
                    if ( _totalChannels[i].State == State.RUN )
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// 지정된 인덱스 채널의 정보를 <see cref="Application.StartupPath"/>\Channels\Ch<paramref name="channelGlobalIndex"/>.config 파일에 저장합니다.
        /// </summary>
        /// <param name="channelGlobalIndex">정보를 저장할 채널의 인덱스입니다. -1로 지정한 경우 생성된 모든 채널의 정보를 저장합니다.</param>
        public static void SaveChannelInfos(int channelGlobalIndex = -1)
        {
            var path = Path.Combine( Application.StartupPath, "Channels" );
            if ( !Directory.Exists( path ) ) Directory.CreateDirectory( path );

            for ( var i = 0; i < Communicator.TotalChannels.Length; i++ )
            {
                if ( channelGlobalIndex != -1 && channelGlobalIndex != i ) continue;

                var filename = Path.Combine( path, $"Ch{i}.config" );
                new FileInfo( filename ).Delete();

                using ( var fs = new FileStream( Path.Combine( path, $"Ch{i}.config" ), FileMode.CreateNew, FileAccess.Write, FileShare.Read ) )
                {
                    using ( var sw = new StreamWriter( fs ) )
                    {
                        var info = Communicator.TotalChannels[i].ToChannelInfo();
                        sw.WriteLine( $"{nameof( ChannelInfo.ChannelIndex )}={info.ChannelIndex}" );
                        sw.WriteLine( $"{nameof( ChannelInfo.SaveDirectory )}={info.SaveDirectory}" );
                        sw.WriteLine( $"{nameof( ChannelInfo.SaveFileName )}={info.SaveFileName}" );
                        sw.WriteLine( $"{nameof( ChannelInfo.SequencePath )}={info.SequencePath}" );
                        sw.WriteLine( $"{nameof( ChannelInfo.StepNo )}={info.StepNo}" );
                        sw.WriteLine( $"{nameof( ChannelInfo.Name )}={info.Name}" );
                        sw.WriteLine( $"{nameof( ChannelInfo.ExportFilePath )}={( info.ExportFilePath == null ? string.Empty : info.ExportFilePath )}" );
                    }
                }
            }
        }
        /// <summary>
        /// 지정된 인덱스 채널에 대해 <see cref="Application.StartupPath"/>\Channels\Ch<paramref name="channelGlobalIndex"/>.config 파일로부터 불러옵니다.
        /// <br>채널 인덱스를 -1로 지정한 경우 생성된 모든 채널의 정보를 불러옵니다.</br>
        /// </summary>
        /// <param name="channelGlobalIndex"></param>
        public static void LoadChannelInfos(int channelGlobalIndex = -1)
        {
            for ( var i = 0; i < Communicator.TotalChannels.Length; i++ )
            {
                if ( channelGlobalIndex != -1 && channelGlobalIndex != i ) continue;

                var filename = Path.Combine( Application.StartupPath, "Channels", $"Ch{i}.config" );

                if ( !File.Exists( filename ) ) continue;

                try
                {
                    var info = new ChannelInfo();
                    using ( var sr = new StreamReader( filename ) )
                    {
                        var lines = sr.ReadToEnd().Split( Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

                        for ( var j = 0; j < lines.Length; j++ )
                        {
                            var split = lines[j].Split( '=' );

                            // = 으로 스플릿한 개수가 2개보다 많은 경우 -> 파일 경로 등에 =이 들어간 경우로 뒷 인덱스 애들은 =을 끼워서 다 붙여준다.
                            if ( split.Length > 2 )
                            {
                                for ( var k = 2; k < split.Length; k++ )
                                {
                                    split[1] += $"={split[k]}";
                                }
                            }

                            switch ( split[0] )
                            {
                                case nameof( ChannelInfo.ChannelIndex ):
                                    //int.TryParse( split[1], out info.ChannelIndex ); => 사용 안 함
                                    break;

                                case nameof( ChannelInfo.SaveDirectory ):
                                    info.SaveDirectory = split[1];
                                    break;

                                case nameof( ChannelInfo.SaveFileName ):
                                    info.SaveFileName = split[1];
                                    break;

                                case nameof( ChannelInfo.SequencePath ):
                                    info.SequencePath = split[1];
                                    break;

                                case nameof( ChannelInfo.StepNo ):
                                    uint.TryParse( split[1], out info.StepNo );
                                    break;

                                case nameof( ChannelInfo.Name ):
                                    info.Name = split[1];
                                    break;

                                case nameof( ChannelInfo.ExportFilePath ):
                                    info.ExportFilePath = split[1];
                                    break;
                            }
                        }
                    }

                    Communicator.TotalChannels[i].ApplyChannelInfo( info );
                }
                catch ( Exception ex )
                {
#if CONSOLEOUT
                    Console.WriteLine( $"loadChannelInfos error at line[{i}], {ex.Message}" );
#endif
                }
            }
        }

        #endregion

        #region Creators
        private Communicator()
        {
            PacketLog += log;
            //ThreadPool.QueueUserWorkItem( new WaitCallback( timeUpdater ) );
        }
        /// <summary>
        /// 하나의 Q 장비에 대한 통신을 처리하는 Communicator 개체를 생성합니다.
        /// </summary>
        /// <param name="index">장비의 번호입니다. 0부터 시작합니다.</param>
        /// <param name="maxChannelCount">장비의 최대 채널 수 입니다.</param>
        public Communicator( int index, int maxChannelCount ) : this()
        {
            _index = index;
            _channels = new Channel[maxChannelCount];

            for ( var i = 0; i < _channels.Length; i++ )
            {
                _totalChannels.Add( _channels[i] = new Channel( this, i, Util.GetADDR( i ), Util.GetCH( i ), State.DISCONNECTED ) );
            }
        }
        #endregion

        #region Logging & Debugging tools
        /// <summary>
        /// Communicator에서의 로그 기록용 델리게이트입니다.
        /// </summary>
        /// <param name="msg">기록할 메시지를 나타내는 문자열입니다.</param>
        public delegate void LogHandler( string msg );
        /// <summary>
        /// Communicator에 수신된 패킷을 McQLib 외부에서 대신 처리하기 위한 델리게이트입니다.
        /// </summary>
        /// <param name="packet">수신된 패킷입니다.</param>
        public delegate void PacketIntercpetHandler( Packet packet );
        /// <summary>
        /// 로그 출력을 위한 이벤트 핸들러입니다.
        /// <br>Communicator는 동작이 발생될 때마다 ActionLog에 등록된 메서드에 로그 메시지를 인자로 전달합니다.</br>
        /// </summary>
        public LogHandler ActionLog;
        /// <summary>
        /// 로그 출력을 위한 이벤트 핸들러입니다. Communicator는 패킷이 수신될 때마다 PacketLog에 등록된 메서드에 패킷의 Raw 바이트를 표현하는 문자열을 인자로 전달합니다.
        /// </summary>
        public LogHandler PacketLog;
        /// <summary>
        /// SendAndReceive로 송신되지 않은 모든 패킷의 처리를 가로채기 위한 핸들러입니다.
        /// </summary>
        public PacketIntercpetHandler Intercept;
        private bool _showPacketLog = false;
        private FileStream logWriter;
        /// <summary>
        /// 로그에 패킷 정보를 출력할지의 여부입니다.
        /// </summary>
        public bool ShowPacketLog
        {
            get => _showPacketLog;
            set
            {
                if ( value && logWriter == null )
                {
                    logWriter = new FileStream( _logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite );
                }

                _showPacketLog = value;
            }
        }
        /// <summary>
        /// CommunicationLog에 전달되는 로그 메시지에 Raw Packet의 내용을 포함할지의 여부입니다.
        /// </summary>
        public bool WithRawPacket { get; set; }
        /// <summary>
        /// 로그를 기록할지의 여부입니다.
        /// </summary>
        public bool Logging { get; set; } = true;

        private bool _useAppendingState = false;
        internal int[] GetAllQueueCounts()
        {
            return new int[4] { _sendQueue.Count, _receiveWaiterList.Count, _byteList.Count, _rawPacketQueue.Count };
        }
        private string _logPath => Path.Combine( Application.StartupPath, "Log", $"PacketLog_{DateTime.Now:yyyyMMdd}.log" );

        private void log( Packet packet )
        {
            if ( !ShowPacketLog ) return;

            PacketLog.Invoke( packet.ToLogText( true ) );
        }
        private void log( string msg )
        {
            var logMessage = $"[{DateTime.Now:HH:mm:ss.fff}] {msg}{Environment.NewLine}";

            var bytes = Encoding.ASCII.GetBytes( logMessage );
            lock ( logWriter )
            {
                logWriter.Write( bytes, 0, bytes.Length );
                logWriter.Flush();
            }
        }
        internal void Log( string message )
        {
            //if ( !Logging ) return;

            //ActionLog?.Invoke( $"Dev{Index}==> {message}" );
        }
        internal void Log( string message, Address address )
        {
            //if ( !Logging ) return;

            //ActionLog?.Invoke( $"Dev{Index}[{address.ADDR},{address.CH}]==> {message}" );
        }
        #endregion

        #region Data Structures
        private enum DeviceType
        {
            NotDefine,
            SingleChannelBoard,
            MultiChannelBoard
        }
        /// <summary>
        /// 송신 패킷에 대한 응답 패킷이 왔는지를 검사하기 위한 구조를 가지는 클래스입니다.
        /// <br>수신 패킷의 ADDR, CH, CMD1, CMD2를 검사하여 이 클래스 인스턴스의 정보와 일치하는 경우 응답 패킷으로 간주하고 <see cref="Received"/>에 해당 패킷을 할당합니다.</br>
        /// </summary>
        private class ReceiveWaiter
        {
            /// <summary>
            /// 이 ReceiveWaiter가 기다리는 ADDR입니다.
            /// </summary>
            public byte ADDR => Send.ADDR2;
            /// <summary>
            /// 이 ReceiveWaiter가 기다리는 CH입니다.
            /// </summary>
            public byte CH => Send.CH2;
            /// <summary>
            /// 이 ReceiveWaiter가 기다리는 CMD1입니다.
            /// </summary>
            public byte CMD1 => Send.SubPacket.CMD_1;
            /// <summary>
            /// 이 ReceiveWaiter가 기다리는 CMD2입니다.
            /// </summary>
            public byte CMD2 => Send.SubPacket.CMD_2;
            /// <summary>
            /// 보낸 패킷입니다.
            /// </summary>
            public readonly Packet Send;
            /// <summary>
            /// 응답 패킷입니다.
            /// <br>아직 응답이 오지 않은 경우 <see langword="null"/>입니다.</br>
            /// </summary>
            public Packet Received;

            /// <summary>
            /// 이 <see cref="ReceiveWaiter"/>가 기다리는 응답 패킷에 대한 송신 패킷이 SendQueue에 Enqueue된 시간입니다.
            /// <br><see cref="ReceiveWaiter"/>가 <see cref="SendAndReceive(Packet, int, int)"/>에서 등록된 경우 <see langword="null"/>입니다.</br>
            /// </summary>
            public readonly DateTime? SendTime;
            /// <summary>
            /// 이 <see cref="ReceiveWaiter"/>가 응답 패킷을 기다릴 시간(초)입니다.
            /// <br><see cref="ReceiveWaiter"/>가 <see cref="SendAndReceive(Packet, int, int)"/>에서 등록된 경우 <see langword="null"/>입니다.</br>
            /// </summary>
            public readonly int? TimeOut;

            /// <summary>
            /// <see cref="ReceiveWaiter"/>의 새 인스턴스를 초기화합니다.
            /// <br>이렇게 생성된 <see cref="ReceiveWaiter"/> 인스턴스는 <see cref="SendAndReceive(Packet, int, int)"/>에서 추가 및 제거됩니다.</br>
            /// </summary>
            /// <param name="packet"><see cref="ReceiveWaiter"/>가 기다릴 응답 패킷에 대한 송신 패킷입니다.</param>
            public ReceiveWaiter( Packet send )
            {
                Send = send;
                Received = null;

                SendTime = null;
                TimeOut = null;
            }
            /// <summary>
            /// <see cref="ReceiveWaiter"/>의 새 인스턴스를 초기화합니다.
            /// <br>이렇게 생성된 <see cref="ReceiveWaiter"/> 인스턴스는 <see cref="Send(Packet)"/>에서 추가되고, CheckLoop 스레드에서 제거됩니다.</br>
            /// </summary>
            /// <param name="send"><see cref="ReceiveWaiter"/>가 기다릴 응답 패킷에 대한 송신 패킷입니다.</param>
            /// <param name="sendTime"><paramref name="packet"/>이 SendQueue에 Enqueue된 시간입니다.</param>
            /// <param name="timeOut"><see cref="ReceiveWaiter"/>가 기다릴 시간입니다.</param>
            public ReceiveWaiter( Packet send, DateTime sendTime, int timeOut ) : this( send )
            {
                SendTime = sendTime;
                TimeOut = timeOut;
            }

            /// <summary>
            /// 지정된 패킷이 이 <see cref="ReceiveWaiter"/> 인스턴스가 기다리는 응답 패킷인지 검사합니다.
            /// </summary>
            /// <param name="packet">응답 패킷인지 비교할 패킷입니다.</param>
            /// <returns><paramref name="packet"/>의 ADDR, CH, CMD1, CMD2가 이 <see cref="ReceiveWaiter"/>의 ADDR, CH, CMD1, CMD2와 모두 일치하는 경우 <see langword="true"/>이고,
            /// 그렇지 않은 경우 <see langword="false"/>입니다.</returns>
            public bool CompareTo( Packet packet )
            {
                if ( packet[0].ADDR == ADDR &&
                     packet[0].CH == CH &&
                     packet[0].CMD_1 == CMD1 &&
                     packet[0].CMD_2 == CMD2 ) return true;

                return false;
            }
        }
        /// <summary>
        /// 보낼 패킷이 저장되는 큐
        /// </summary>
        private ConcurrentQueue<Packet> _sendQueue = new ConcurrentQueue<Packet>();
        /// <summary>
        /// 수신된 바이트가 쌓이는 리스트
        /// </summary>
        private List<byte> _byteList = new List<byte>();
        /// <summary>
        /// 바이트가 STX2, ETX2 단위로 끊어져서 들어오는 큐
        /// </summary>
        private ConcurrentQueue<byte[]> _rawPacketQueue = new ConcurrentQueue<byte[]>();
        /// <summary>
        /// 송신 후 응답을 기다리는 패킷 리스트
        /// </summary>
        private List<ReceiveWaiter> _receiveWaiterList = new List<ReceiveWaiter>();
        /// <summary>
        /// 바이트 리스트에 남은 쓰레기값을 저장하는 리스트
        /// </summary>
        private List<object> _garbageList = new List<object>();
        #endregion

        #region Members & Properties
        private Socket _socket;
        private NetworkStream _stream;
        private Channel[] _channels;
        private List<byte> _savedDataBuffer = new List<byte>();
        private bool _isAppending = false;
        private bool _appendingDetected = false;
        // 장비와 마지막으로 통신을 한 시간
        private DateTime _lastComTime;

        /// <summary>
        /// 장비의 인덱스입니다.
        /// </summary>
        public int Index => _index;
        private int _index;
        /// <summary>
        /// 장비가 연결되었는지의 여부입니다.
        /// </summary>
        public bool IsConnected => _isConnected;
        private bool _isConnected = false;
        /// <summary>
        /// IP 주소를 가져오거나 설정합니다.
        /// </summary>
        public string IP
        {
            get => _ip;
            set
            {
                if ( IPAddress.Parse( value ) != null ) _ip = value;
            }
        }
        private string _ip;
        /// <summary>
        /// 포트 번호를 가져옵니다.
        /// <br>Communicator는 포트 번호를 자동으로 할당 받아 연결합니다.</br>
        /// </summary>
        public int Port => _port;
        private int _port;
        /// <summary>
        /// 일정 간격마다 Ping을 주고 받기 위한 시간 간격(초)입니다.
        /// <br>기본값은 20초 입니다.</br>
        /// </summary>
        public int PingInterval = 20;
        /// <summary>
        /// 연결 작업에서 소켓 연결 요청을 보내고 기다릴 시간(초)입니다.
        /// <br>기본값은 3초 입니다.</br>
        /// </summary>
        public int Timeout = 3;
        /// <summary>
        /// 일정 간격마다 Ping을 주고 받을지의 여부입니다.
        /// </summary>
        public bool IsAutoPing
        {
            get => _isAutoPing;
            set
            {
                if ( _isAutoPing = value )
                {
                    if ( _isConnected && !_isPingRun )
                    {
                        new Thread( pingLoop ) { Name = "PingLoop", IsBackground = true }.Start();
                    }
                }
                else
                {
                    _pingRun = false;
                }
            }
        }
        private bool _isAutoPing;
        /// <summary>
        /// 연결된 장비가 구 장비(보드 당 단일 채널)인지의 여부입니다.
        /// </summary>
        public bool IsOldDevice => _deviceType == DeviceType.SingleChannelBoard;
        private DeviceType _deviceType;
        /// <summary>
        /// 장비에 포함된 채널 목록에서 index 위치의 <see cref="Channel"/> 개체를 가져옵니다.
        /// </summary>
        /// <param name="index">가져올 채널의 번호입니다.</param>
        /// <returns>지정된 index의 채널입니다.</returns>
        public Channel this[int index] => _channels[index];
        /// <summary>
        /// 현재 Communicator에 속한 채널의 배열입니다.
        /// </summary>
        public Channel[] Channels => _channels;
        private BoardInfo _boardInformation;
        public VersionInfo _versionInfo;
        #endregion

        #region Communicating Methods
        /// <summary>
        /// 장비에 연결을 시도합니다.
        /// <br>이미 장비와 연결된 상태인 경우, 연결을 해제한 후 다시 연결합니다.</br>
        /// </summary>
        /// <returns>연결에 성공한 경우 true이고, 그렇지 않은 경우 false입니다.</returns>
        public bool Connect()
        {
            Log( $"Start connecting." );

            if ( _isConnected )
            {
                Disconnect();
            }

            if ( _socket == null )
            {
                _socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            }
            return _isConnected = connect();
        }
        private bool _onConnecting = false;
        private bool _onDisconnecting = false;
        /// <summary>
        /// 새로운 포트 번호를 할당 받아 소켓으로 연결을 시도합니다.
        /// </summary>
        /// <returns>연결에 성공한 경우 true이고, 그렇지 않은 경우 false입니다.</returns>
        private bool connect()
        {
            _onConnecting = true;

            lock ( _socket )
            {
                _socket.Close();
                _socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

                var s = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

                if ( !connectWithTimeout( s, _ip, 7000, Timeout ) ) return _onConnecting = false;

                if ( s.Connected )
                {
                    var stream = new NetworkStream( s );
                    var buffer = Encoding.ASCII.GetBytes( "MCSERVER NEW?" );

                    Log( $"Req new port." );
                    stream.Write( buffer, 0, buffer.Length );

                    // 포트 번호 할당용 소켓에서 데이터를 읽을 수 있을 때까지 대기
                    var time = DateTime.Now;
                    while ( !stream.DataAvailable )
                        if ( ( DateTime.Now - time ).TotalSeconds > Timeout ) return _onConnecting = false;

                    // 7000번 포트로부터 통신용 포트 번호를 받아온다.
                    var receiveBuffer = new byte[256];
                    stream.Read( receiveBuffer, 0, 256 );

                    s.Close();

                    var str = Encoding.ASCII.GetString( receiveBuffer, 0, 256 );
                    try
                    {
                        _port = int.Parse( str.Split( ' ' )[2] );

                        Log( $"{_port} port received." );
                        if ( !connectWithTimeout( _socket, _ip, _port, Timeout ) ) return _onConnecting = false;
                    }
                    catch ( IndexOutOfRangeException )
                    {
                        // MCSERVER NEW: xxxxx 에서 인덱스 범위 초과함
                        Log( $"Req port failed. (Cause: Invalid rcv({str}))" );
                        return false;
                    }
                    catch ( Exception ex )
                    {
                        Log( $"Req port failed. (Cause: {ex.Message})" );
                        return false;
                    }
                }
            }

            _stream = new NetworkStream( _socket );

            // 소켓 문제로 인해 중단된 스레드가 있다면 재실행 시켜준다.
            startAllThreads();

            _onConnecting = false;

            Log( $"Connect done." );
            return true;
        }
        /// <summary>
        /// 소켓에 연결을 시도합니다.
        /// <br>지정된 시간 내에 연결이 성공하지 못한 경우 즉시 연결 시도를 중단하고 처리를 반환합니다.</br>
        /// </summary>
        /// <param name="socket">연결 시도할 소켓입니다.</param>
        /// <param name="ip">연결할 IP입니다.</param>
        /// <param name="port">연결할 Port입니다.</param>
        /// <param name="timeOutSec">연결 시도 후 대기할 시간(초)입니다.</param>
        /// <returns>연결에 성공한 경우 true이고, 연결에 실패하거나 시간이 초과한 경우 false입니다.</returns>
        private bool connectWithTimeout( Socket socket, string ip, int port, int timeOutSec )
        {
            try
            {
                if ( socket.Connected ) return true;

                Log( $"Socket connect to {ip}:{port}." );
                var result = socket.BeginConnect( new IPEndPoint( IPAddress.Parse( ip ), port ), null, null );
                bool success = result.AsyncWaitHandle.WaitOne( timeOutSec * 1000, true );

                if ( success )
                {
                    socket.EndConnect( result );
                    if ( socket.Connected )
                    {
                        Log( $"Socket connect done." );
                        return true;
                    }
                    else return false;
                }
                else
                {
                    socket.Close();
                    return false;
                }
            }
            catch ( ArgumentNullException ex )
            {
                Log( $"Socket connect failed. (Cause: IP was null)" );
                return false;
            }
            catch ( FormatException ex )
            {
                Log( $"Socket connect failed. (Cause: Invalid IP format)" );
                return false;
            }
            catch ( Exception ex )
            {
                Log( $"Socket connect failed. (Cause: {ex.Message})" );
                return false;
            }
        }

        private bool disconnectWithTimeout( Socket socket, int timeOutSec )
        {
            try
            {
                if ( !socket.Connected ) return true;

                Log( $"Socket disconnecting." );
                var result = socket.BeginDisconnect( true, null, null );
                bool success = result.AsyncWaitHandle.WaitOne( timeOutSec * 1000, true );

                if ( success )
                {
                    socket.EndDisconnect( result );
                    if ( !socket.Connected )
                    {
                        Log( $"Socket disconnect done." );
                        return true;
                    }
                    else return false;
                }
                else
                {
                    socket.Close();
                    return true;
                }
            }
            catch ( Exception ex )
            {
                Log( $"Socket disconnect failed. (Cause: {ex.Message})" );
                return false;
            }
        }
        /// <summary>
        /// 장비와의 연결을 끊습니다.
        /// <br>모든 통신 스레드가 종료되며, 송신 대기중인 패킷이 남아있는 경우 버려집니다.</br>
        /// </summary>
        /// <returns>연결 종료에 성공한 경우 true이고, 그렇지 않은 경우 false입니다.</returns>
        public bool Disconnect()
        {
            Log( $"Start disconnecting." );

            if ( !_isConnected ) return true;
            else if ( _onDisconnecting ) return false;

            _onDisconnecting = true;

            while ( !_sendQueue.IsEmpty ) _sendQueue.TryDequeue( out Packet dummy );

            stopAllThreads();

            foreach ( var c in _channels )
            {
                c.Stop();
                c.State = ( State.DISCONNECTED );
            }

            disconnectWithTimeout( _socket, 2 );

            _isConnected = false;
            _onDisconnecting = false;

            return true;
        }
        /// <summary>
        /// 현재 장비에서 SD카드 오류가 발생했는지의 여부입니다.
        /// </summary>
        public bool SdFail => _sdFail;
        private bool _sdFail;

        /// <summary>
        /// 장비로부터 보드 정보, 채널 정보 등을 받아와 <see cref="Communicator"/> 개체를 구성하고, 동작을 준비합니다.
        /// </summary>
        /// <exception cref="QExceptionType.COMMUNICATION_CANNOT_READ_BOARDINFO_ERROR">장비로부터 보드 정보를 읽어오지 못했습니다.</exception>
        /// <exception cref="QExceptionType.PACKET_BOARD_INFORMATION_DATAFIELD_LENGTH_ERROR">보드 정보 패킷이 올바르지 않습니다.</exception>
        /// <exception cref="QExceptionType.ACTION_DEVICE_NOT_RESPONSE_ERROR">장비가 응답하지 않습니다.</exception>
        /// <exception cref="QExceptionType.COMMUNICATION_COMMANDING_FAILED_ERROR">장비가 명령을 정상적으로 수행하지 못했습니다.</exception>
        public void InitializeCommunicator()
        {
            if ( !_isConnected ) return;
            Log( "Init com start." );

            Application.DoEvents();
            Thread.Sleep( 100 );

            Packet send, receive;

            Log( "Step1: Register check & clear" );
            send = new SendPacket( 0, 0 );
            send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.InitStateResister_GS ) );
            send.SubPacket.ERR = Packet.GET;

            var initError = false;

            // 레지스터 조회 - 3회까지 재시도
            for ( var i = 0; i < 3; i++ )
            {
                receive = SendAndReceive( send );
                if ( receive == null )
                {
                    Log( $"Get register no response. ({i})" );
                }
                else
                {
                    // 마스터 보드의 레지스터에서 RecipeSendingFail, SdInitializeFail, SdReadWriteFail 플래그를 초기화해준다.
                    if ( receive.SubPacket.DATA.Count == 2 )
                    {
                        var reg0 = ( RegisterError0 )receive.SubPacket.DATA[0];
                        var reg1 = ( RegisterError1 )receive.SubPacket.DATA[1];

                        // Initialize 오류인 경우에는 여기에서 처리할 수 없다. (장비 재시작 필요함)
                        if ( reg1.HasFlag( RegisterError1.InitializeFail ) )
                        {
                            initError = true;
                            break;
                        }

                        send = new SendPacket( 0, 0 );
                        send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.InitStateResister_GS ) );
                        send.SubPacket.ERR = Packet.SET;

                        byte needClear = 0xFF;
                        if ( reg0.HasFlag( RegisterError0.RecipeSendingFail ) ) needClear ^= ( byte )RegisterError0.RecipeSendingFail;

                        send.SubPacket.DATA.Add( needClear );

                        needClear = 0xFF;
                        _sdFail = false;
                        if ( reg1.HasFlag( RegisterError1.SdInitializeFail ) )
                        {
                            _sdFail = true;
                        }
                        if ( reg1.HasFlag( RegisterError1.SdReadWriteFail ) ) needClear ^= ( byte )RegisterError1.SdReadWriteFail;

                        send.SubPacket.DATA.Add( needClear );

                        if ( send.SubPacket.DATA[0] == 0xFF && send.SubPacket.DATA[1] == 0xFF ) break;

                        receive = SendAndReceive( send );

                        if ( receive != null && receive.SubPacket.ERR == 0 ) break;
                    }
                    else continue;
                }
            }

            // 장비 재시작 필요
            if ( initError ) throw new QException( QExceptionType.REGISTER_INITIAL_FAIL_ERROR );

            Log( "Step2: Get board info." );
            send = new SendPacket( 0, 0 );
            send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.BoardInformation_G ) );

            receive = SendAndReceive( send );

            if ( receive == null )
            {
                // 보드 정보 조회 실패
                Log( "Get board info failed. (Cause: No response)" );
                throw new QException( QExceptionType.COMMUNICATION_CANNOT_READ_BOARDINFO_ERROR );
            }
            else
            {
                int position = 0;
                byte[] barray;

                try
                {
                    barray = new byte[15];
                    for ( var i = 0; i < 15; i++ ) barray[i] = receive.SubPacket.DATA[position++];
                    var model = Encoding.ASCII.GetString( barray );

                    barray = new byte[10];
                    for ( var i = 0; i < 10; i++ ) barray[i] = receive.SubPacket.DATA[position++];
                    var serial = Encoding.ASCII.GetString( barray );

                    var date = new DateTime( receive.SubPacket.DATA[position++] + 2000,
                                             receive.SubPacket.DATA[position++],
                                             receive.SubPacket.DATA[position++] );

                    var firmware = new VersionInfo( receive.SubPacket.DATA[position++], receive.SubPacket.DATA[position++] );

                    var fpga = new VersionInfo( receive.SubPacket.DATA[position++], receive.SubPacket.DATA[position++] );

                    var protocol = new VersionInfo( receive.SubPacket.DATA[position++], receive.SubPacket.DATA[position++], additional: ( ( char )receive.SubPacket.DATA[position++] ).ToString() );

                    var dac = receive.SubPacket.DATA[position++];

                    var adc = receive.SubPacket.DATA[position++];

                    var auxadc = receive.SubPacket.DATA[position++];

                    _boardInformation = new BoardInfo( model, serial, date, firmware, fpga, protocol, dac, adc, auxadc );
                    _versionInfo = firmware;
                    //var checkFirmwareVersion = false;
                    //for ( var i = 0; i < _compatibleFirmwareVersions.Length; i++ )
                    //{
                    //    if ( _compatibleFirmwareVersions[i].CompatibleWith( BoardInformation.FirmwareVersion ) )
                    //    {
                    //        checkFirmwareVersion = true;
                    //        break;
                    //    }
                    //}
                    //if ( !checkFirmwareVersion )
                    //{
                    //    throw new QException( QExceptionType.COMMUNICATION_FIRMWARE_VERSION_NOT_COMPATIBLE_ERROR );
                    //}
                }
                catch ( IndexOutOfRangeException )
                {
                    Log( "Get board info failed. (Cause: Data field length)" );
                    throw new QException( QExceptionType.PACKET_BOARD_INFORMATION_DATAFIELD_LENGTH_ERROR );
                }
            }

            Log( "Step3: Search slave boards." );
            int tryCount = 0;
            do
            {
                send = new SendPacket( 0, 0 );
                send.SubPackets.Add( new SendSubPacket( Commands.MultiChannelCommands.M_BoardSearching_GS ) );

                receive = SendAndReceive( send );

                if ( receive == null )
                {
                    Log( "Search slave boards failed. (Cause: No response)" );
                    throw new QException( QExceptionType.ACTION_DEVICE_NOT_RESPONSE_ERROR );
                }
                // 아직 Board Research가 진행중인 경우
                else if ( receive.SubPacket.DATA[0] == 1 )
                {
                    // 3초간 대기 후 패킷을 다시 송신한다.
                    Thread.Sleep( 1000 );
                    tryCount++;
                }
                else break;
            } while ( tryCount < 5 );    // 재탐색 진행중 대기를 최대 5회까지 수행

            if ( tryCount == 5 )
            {
                Log( "Search slave boards failed. (Cause: Try count)" );
                throw new QException( QExceptionType.COMMUNICATION_COMMANDING_FAILED_ERROR );
            }

            // DATA 필드 구조
            // Index    : Contents
            // 0        : 재탐색 진행 상태
            // 1        : 총 채널 수
            // 2        : 총 채널 수
            // 3        : Slave Address
            // 4        : Slave Channel
            // 5        : Slave State   (0: Idle, 1: Run, 2: Pause, 3: Not Insert)

            // 채널들을 생성한다.
            //_deviceType = DeviceType.NotDefine;

            try
            {
                // 패킷상에 존재하는 실제 장비에서 인식한 채널 개수를 가져온다.
                var realChannelCount = new Q_UInt16( receive.SubPacket.DATA[1], receive.SubPacket.DATA[2] ).Value;
                Log( $"Detected channels: {realChannelCount}" );

                if ( _channels != null )
                {
                    for ( var i = 0; i < _channels.Length; i++ )
                    {
                        _channels[i]?.Stop();
                    }
                }

                if ( realChannelCount < _channels.Length )
                {
                    // 사용자가 입력한 채널 수 보다 실제 채널수가 적은 경우에 대한 처리
                    // 아무 처리 안 함
                }
                else if ( realChannelCount > _channels.Length )
                {
                    // 사용자가 입력한 채널 수 보다 실제 채널 수가 많은 경우에 대한 처리
                    // 아무 처리 안 함
                }

                var position = 3;
                for ( var i = 0; i < realChannelCount; i++ )
                {
                    var addr = receive.SubPacket.DATA[position++];

                    var ch = receive.SubPacket.DATA[position++];
                    var channelState = ( ChannelState )receive.SubPacket.DATA[position++];

                    State state;
                    switch ( channelState )
                    {
                        case ChannelState.Idle:
                            state = State.IDLE;
                            break;
                        case ChannelState.Run:
                            state = State.RUN;
                            break;
                        case ChannelState.Pause:
                        case ChannelState.Pausing:
                            state = State.PAUSED;
                            break;
                        default:
                            state = State.IDLE; ; //20230413 DISCONNECTED 상태 이상현상 해결 정혹욱 수석님 미팅
                            break;
                    }

                    // 해당되는 채널의 객체를 생성한다. 
                    // 변경 : 채널의 개체는 생성자에서 생성하므로 채널의 상태만 갱신해준다.
                    _channels[Util.GetIndex( addr, ch )].State = state;
                    _channels[Util.GetIndex( addr, ch )].SetMessage( "" );

                    if ( state == State.PAUSED ) RefreshChannelState( new Address( addr, ch ) );
                }
                // 삭제 : 이제 Communicator의 생성자에서 채널 개체를 생성함.
                //// 전체 채널 리스트에 대해 위에서 생성되지 않은 채널들은 연결되지 않았다고 판단하고 NotInsert 상태로 생성한다.
                //for( var i = 0; i < _channels.Length; i++ )
                //    if( _channels[i] == null ) _channels[i] = new Channel( Util.GetADDR( i ), Util.GetCH( i ), State.NotConnected );
            }
            catch ( IndexOutOfRangeException )
            {
                // 인덱스 오류 -> 사용자가 입력한 채널 개수보다 실제 채널 개수가 더 많은 경우 발생
                // 예를 들어 실제 장비의 채널 개수는 64개이나, 사용자가 8채널 짜리 장비로 설정한 경우 -> 앞에서부터 8개 채널만 생성된다.
                // 아무 처리하지 않음
            }

            _appendingDetected = false;

            Log( $"Init com done." );
        }
        /// <summary>
        /// 장비로 송신 제어권 설정 명령을 송신하고, 저장된 데이터가 있다면 송신하도록 합니다.
        /// </summary>
        public void CheckAppendingData()
        {
            SetTransmissionControl(true);
            //if ( Appending )
            //{
            //    SetTransmissionControl( true );
            //}
            //else
            //{
            //    RemoveSavedData();
            //}
        }

        public double ReadADC(Address address, int index)
        {
            double data = 0;

            var send = new SendPacket(address.ADDR, address.CH);
            //send.SubPackets.Add(new SendSubPacket(Commands.BatteryCycler_SetGetCommands.StartStopSequence));
            //var send = new SendPacket(address, Commands.CalibrationCommands.ReadADC);
            send.SubPackets.Add(new SendSubPacket(Commands.CalibrationCommands.ReadADC));
            send.ByPass = Packet.ON;
            //send.SubPacket.ADDR = send.ADDR2;
            //send.SubPacket.CH = send.CH2;
            if (index == 0)
            {
                send.SubPacket.DATA.Add(0);
            }
            else
            {
                send.SubPacket.DATA.Add(1);
            }

            var received = SendAndReceive(send);
            if (received == null)
            {
                return 0;
            }
            else if (received.SubPacket.ERR != 0)
            {
                return 0;
            }
            else
            {
                if (received.SubPacket.DATA.Count < 10)
                {
                    return 0;
                }
                data = new Q_Double(received.SubPacket.DATA[1],
                    received.SubPacket.DATA[2],
                    received.SubPacket.DATA[3],
                    received.SubPacket.DATA[4],
                    received.SubPacket.DATA[5],
                    received.SubPacket.DATA[6],
                    received.SubPacket.DATA[7],
                    received.SubPacket.DATA[8]).Value;
            }

            return data;
        }
        /// <summary>
        /// 채널에 시퀀스를 전송합니다.
        /// <br>이 명령은 채널 전용 명령으로 마스터보드로 보낼 수 없습니다.</br>
        /// </summary>
        /// <param name="address">시퀀스를 전송할 채널의 번호입니다. 0부터 시작합니다.</param>
        /// <param name="seq">전송할 시퀀스입니다.</param>
        /// <returns><see cref="Result.NoError"/> : 전송 성공
        /// <br><see cref="Result.NoResponse"/> : 응답 없음</br>
        /// <br><see cref="Result.InvalidSequence"/> : 송신할 수 없는 시퀀스</br>
        /// <br><see cref="Result.TryCount"/> : 송신 재시도 횟수 초과</br>
        /// <br><see cref="Result.RegClearFail"/> : 레지스터 초기화 실패</br>
        /// <br><see cref="Result.Com_ActionError"/> ~ <see cref="Result.Com_NotDefinedError"/> : ERR 필드가 0이 아닌 경우</br></returns>
        internal Result SendSequence( Address address, Sequence seq )
        {
            SendPacket[] packets = null;
            var cloneSeq = seq.Clone() as Sequence;
            var changeCurrentUnitData = cloneSeq._recipes.FindAll(x => x.Name is "Charge" or "Discharge" or "AnodeCharge" or "AnodeDischarge");
            //if (changeCurrentUnitData.Count > 1)
            //{
                foreach (var temp in changeCurrentUnitData)
                {
                    switch (temp.Name)
                    {
                        case "Charge":
                            ((Charge)temp).Current = UpdateCurrent(((Charge)temp).Current, ((Charge)temp).CurrentUnit);
                            break;
                        case "Discharge":
                            ((Discharge)temp).Current = UpdateCurrent(((Discharge)temp).Current, ((Discharge)temp).CurrentUnit);
                            break;
                        case "AnodeCharge":
                            ((AnodeCharge)temp).Current = UpdateCurrent(((AnodeCharge)temp).Current, ((AnodeCharge)temp).CurrentUnit);
                            break;
                        case "AnodeDischarge":
                            ((AnodeDischarge)temp).Current = UpdateCurrent(((AnodeDischarge)temp).Current, ((AnodeDischarge)temp).CurrentUnit);
                            break;
                    }
                }
            //}

            foreach (var temp in cloneSeq._recipes)
            {
                if (temp.SaveCondition != null)
                {
                    temp.SaveCondition.Save_Current =
                        UpdateCurrent(temp.SaveCondition.Save_Current, temp.SaveCondition.CurrentUnit);
                }
                if (temp.EndCondition != null)
                {
                    temp.EndCondition.End_Current =
                        UpdateCurrent(temp.EndCondition.End_Current, temp.EndCondition.CurrentUnit);
                }
                if (temp.SafetyCondition != null)
                {
                    temp.SafetyCondition.Safety_MaxCurrent =
                        UpdateCurrent(temp.SafetyCondition.Safety_MaxCurrent, temp.SafetyCondition.CurrentUnit);
                    temp.SafetyCondition.Safety_MinCurrent =
                        UpdateCurrent(temp.SafetyCondition.Safety_MaxCurrent, temp.SafetyCondition.CurrentUnit);
                }
            }
        
            try
            {
                if (IsOldDevice)
                {
                    packets = cloneSeq.ToPacketArray_Old(address.ADDR, address.CH);
                }
                else
                {
                    packets = cloneSeq.ToPacketArray(address.ADDR, address.CH);
                }
            }
            catch
            {
                return Result.InvalidSequence;
            }

            Log( $"Send seq start. (Name: {seq.Name}, Packets: {packets.Length} EA)", address );

            var tryClearAlready = false;
            for ( var i = 0; i < packets.Length; i++ )
            {
                var errorCount = 0;

                do
                {
                    Log( $"Send rcp[{i}]. (ErrCount = {errorCount})", address );

                    var receive = SendAndReceive( packets[i], 10, 1 );
                    if ( receive == null )
                    {   // 응답 없음
                        Log( $"Send rcp[{i}] failed. (Cause: No response)", address );

                        // 마스터 보드 레지스터 초기화 후 다시 처음부터 전송
                        if ( tryClearAlready )
                        {
                            return Result.NoResponse;
                        }
                        else if ( InitRecipeFailError() == Result.NoError )
                        {
                            i = -1;
                            errorCount = 0;
                            tryClearAlready = true;
                            break;
                        }
                        else
                        {   // 레지스터 초기화 조차 실패시 false 반환
                            return Result.RegClearFail;
                        }
                    }
                    else if ( receive.SubPacket.ERR != Packet.NO_ERROR )
                    {   // 재전송
                        Log( $"Send rcp[{i}] failed. (Cause: ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2}))", address );

                        errorCount++;
                    }
                    else
                    {
                        errorCount = 0;
                        Log( $"Send rcp[{i}] done.", address );
                        break;
                    }
                } while ( errorCount < 3 );

                if ( errorCount == 4 )
                {
                    Log( $"Send seq failed. (Cause: No response)", address );
                    return Result.NoResponse;
                }
                else if ( errorCount != 0 )
                {
                    Log( $"Send seq failed. (Cause: Try count)", address );
                    return Result.TryCount;
                }
            }

            Log( $"Send seq done.", address );
            return Result.NoError;
        }
        public double UpdateCurrent(double current, SourcingType_CurrentUnit unit)
        {
            if (unit == SourcingType_CurrentUnit.mA)
            {
                current *= 0.001;
            }
            else if (unit == SourcingType_CurrentUnit.μA)
            {
                current *= 0.000001;
            }

            return current;
        }

        public double? UpdateCurrent(double? current, SourcingType_CurrentUnit unit)
        {
            if (unit == SourcingType_CurrentUnit.mA)
            {
                current *= 0.001;
            }
            else if (unit == SourcingType_CurrentUnit.μA)
            {
                current *= 0.000001;
            }

            return current;
        }

        /// <summary>
        /// 지정된 채널의 측정을 시작합니다.
        /// <br>채널이 일시정지 상태인 경우 처음부터 재시작합니다.</br>
        /// <br>이 명령은 채널 전용 명령으로 마스터보드로 보낼 수 없습니다.</br>
        /// </summary>
        /// <param name="address">측정을 시작할 채널의 번호입니다. <see cref="Address"/> 구조체를 사용합니다.</param>
        /// <param name="applyWhen">명령한 동작을 수행할 시점을 나타내는 <see cref="ApplyWhen"/>입니다. 기본값은 <seealso cref="ApplyWhen.Immediately"/>입니다.</param>
        internal Result StartChannel( Address address, ApplyWhen applyWhen = ApplyWhen.Immediately )
        {
            Log( $"Func: StartChannel", address );

            var send = new SendPacket( address.ADDR, address.CH );
            send.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.StartStopSequence ) );
            send.SubPacket.ERR = Packet.SET;
            send.SubPacket.DATA.AddCount( 0, 2 );
            send.SubPacket.DATA.Add( StartStopType.StartOrKeep );
            send.SubPacket.DATA.Add( applyWhen );

            var receive = SendAndReceive( send, 3, 2 );
            if ( receive == null )
            {
                Log( $"Start channel failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( receive.SubPacket.ERR != 0 )
            {
                Log( $"Start channel failed. (Cause: ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2}))", address );
                return ( Result )receive.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }
        /// <summary>
        /// 지정된 채널의 측정을 중단합니다.
        /// </summary>
        /// <param name="address">측정을 중단할 채널의 번호입니다. 0부터 시작합니다.</param>
        /// <param name="applyWhen">명령한 동작을 수행할 시점을 나타내는 <see cref="ApplyWhen"/>입니다. 기본값은 <seealso cref="ApplyWhen.Immediately"/>입니다.</param>
        internal Result StopChannel( Address address, ApplyWhen applyWhen = ApplyWhen.Immediately )
        {
            Log( $"Func: StopChannel.", address );

            var packet = new SendPacket( address.ADDR, address.CH );
            packet.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.StartStopSequence ) );
            packet.SubPacket.ERR = Packet.SET;
            packet.SubPacket.DATA.AddCount( 0, 2 );
            packet.SubPacket.DATA.Add( StartStopType.Stop );
            packet.SubPacket.DATA.Add( applyWhen );

            var receive = SendAndReceive( packet, 3, 2 );
            if ( receive == null )
            {
                Log( $"Stop channel failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( receive.SubPacket.ERR != 0 )
            {
                Log( $"Stop channel failed. (Cause: ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2}))", address );
                return ( Result )receive.SubPacket.ERR;
            }
            else
            {
                packet = new SendPacket(address.ADDR, address.CH);
                packet.SubPackets.Add(new SendSubPacket(Commands.BatteryCycler_SetGetCommands.StartStopSequence));
                packet.SubPacket.ERR = Packet.SET;
                packet.SubPacket.DATA.AddCount(0, 2);
                packet.SubPacket.DATA.Add(StartStopType.Stop);
                packet.SubPacket.DATA.Add(applyWhen);
                receive = SendAndReceive(packet, 3, 2);
                return Result.NoError;
            }
        }
        /// <summary>
        /// 지정된 채널의 측정을 일시정지합니다.
        /// <br>일시정지된 채널을 이어서 진행할 때는 <see cref="RestartChannel(Address, ApplyWhen)"/>을 사용하십시오.</br>
        /// </summary>
        /// <param name="address">측정을 일시정지할 채널의 번호입니다. 0부터 시작합니다.</param>
        /// <param name="applyWhen">명령한 동작을 수행할 시점을 나타내는 <see cref="ApplyWhen"/>입니다. 기본값은 <seealso cref="ApplyWhen.Immediately"/>입니다.</param>
        internal Result PauseChannel( Address address, ApplyWhen applyWhen = ApplyWhen.Immediately )
        {
            Log( $"Func: PauseChannel.", address );

            var packet = new SendPacket( address.ADDR, address.CH );
            packet.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.PauseSequence ) );
            packet.SubPacket.ERR = Packet.SET;
            packet.SubPacket.DATA.AddCount( 0, 2 );
            packet.SubPacket.DATA.Add( PauseStartType.Pause );
            packet.SubPacket.DATA.Add( applyWhen );

            var receive = SendAndReceive( packet );
            if ( receive == null )
            {
                Log( $"Pause channel failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( receive.SubPacket.ERR != 0 )
            {
                Log( $"Pause channel failed. (Cause: ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2}))", address );
                return ( Result )receive.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }
        /// <summary>
        /// 일시정지된 채널의 측정을 다시 시작합니다.
        /// </summary>
        /// <param name="address">다시 시작할 채널의 번호입니다. 0부터 시작합니다.</param>
        /// <param name="applyWhen">명령한 동작을 수행할 시점을 나타내는 <see cref="ApplyWhen"/>입니다. 기본값은 <seealso cref="ApplyWhen.Immediately"/>입니다.</param>
        internal Result RestartChannel( Address address, ApplyWhen applyWhen = ApplyWhen.Immediately )
        {
            Log( $"Func: RestartChannel.", address );

            // Pause 상태에서의 재시작의 경우는 레지스터를 한 번 초기화한 뒤 시작한다.
            InitSafetyAlarm( address );

            var packet = new SendPacket( address.ADDR, address.CH );
            packet.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.PauseSequence ) );
            packet.SubPacket.ERR = Packet.SET;
            packet.SubPacket.DATA.AddCount( 0, 2 );
            packet.SubPacket.DATA.Add( PauseStartType.StartOrKeep );
            packet.SubPacket.DATA.Add( applyWhen );

            var receive = SendAndReceive( packet );
            if ( receive == null )
            {
                Log( $"Restart channel failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( receive.SubPacket.ERR != 0 )
            {
                Log( $"Restart channel failed. (Cause: ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2}))", address );
                return ( Result )receive.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }
        /// <summary>
        /// 지정된 채널이 현재 수행하고 있는 스텝을 강제 종료하고 다음 스텝을 진행하도록 합니다.
        /// </summary>
        /// <param name="address">강제 진행할 채널의 번호입니다. 0부터 시작합니다.</param>
        internal Result SkipChannel( Address address )
        {
            Log( $"Func: SkipChannel.", address );

            var packet = new SendPacket( address.ADDR, address.CH );
            packet.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.SkipSequence ) );
            packet.SubPacket.ERR = Packet.SET;

            var receive = SendAndReceive( packet );
            if ( receive == null )
            {
                Log( $"Skip channel failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( receive.SubPacket.ERR != 0 )
            {
                Log( $"Skip channel failed. (Cause: ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2}))", address );
                return ( Result )receive.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }

        /// <summary>
        /// 지정된 채널의 RTC를 현재 시각으로 설정합니다.
        /// </summary>
        /// <param name="channelNo"></param>
        internal void SetTime( Address address )
        {
            var send = new SendPacket( address );
            send.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.TimeStamp_GS ) );
            send.SubPacket.ERR = Packet.SET;
            send.SubPacket.DATA.AddCount( 0, 2 );

            var dateTime = DateTime.Now;
            send.SubPacket.DATA.Add( new Q_UInt16( ( ushort )dateTime.Year ) );
            send.SubPacket.DATA.Add( ( byte )dateTime.Month );
            send.SubPacket.DATA.Add( ( byte )dateTime.Day );
            send.SubPacket.DATA.Add( ( byte )dateTime.Hour );
            send.SubPacket.DATA.Add( ( byte )dateTime.Minute );
            send.SubPacket.DATA.Add( ( byte )dateTime.Second );
            send.SubPacket.DATA.Add( 0 );

            SendAndReceive( send );
        }
        /// <summary>
        /// 지정된 채널의 RTC로부터 저장된 시간을 가져옵니다.
        /// </summary>
        /// <param name="channelNo"></param>
        /// <returns></returns>
        internal DateTime GetTime( Address address )
        {
            var send = new SendPacket( address );
            send.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.TimeStamp_GS ) );
            send.SubPacket.ERR = Packet.GET;

            var receive = SendAndReceive( send );

            //checkPacket( receive );

            var position = 2;
            return new DateTime( new Q_UInt16( receive.SubPacket.DATA[position++], receive.SubPacket.DATA[position++] ).Value,  // Year
                                 receive.SubPacket.DATA[position++],     // Month
                                 receive.SubPacket.DATA[position++],     // Day
                                 receive.SubPacket.DATA[position++],     // Hour
                                 receive.SubPacket.DATA[position++],     // Minute
                                 receive.SubPacket.DATA[position++] );   // Second
        }

        internal Result InitSafetyAlarm( byte addr, byte ch, int tryCount = 2 ) => InitSafetyAlarm( new Address( addr, ch ) );
        /// <summary>
        /// 지정된 채널의 상태 레지스터를 초기화합니다.
        /// <br>이 명령은 슬레이브 보드로 송신하는 경우 ByPass로 송신됩니다.</br>
        /// </summary>
        /// <param name="address">상태 레지스터를 초기화할 채널의 주소입니다.</param>
        /// <returns><see cref="Result.NoError"/> : 전송 성공
        /// <br><see cref="Result.NoResponse"/> : 응답 없음</br>
        /// <br><see cref="Result.Com_ActionError"/> ~ <see cref="Result.Com_NotDefinedError"/> : ERR 필드가 0이 아닌 경우</br></returns>
        internal Result InitSafetyAlarm( Address address )
        {
            Log( $"Func: InitSafetyAlarm", address );

            var send = new SendPacket( address );
            if ( !Address.IsMaster( address ) ) send.ByPass = Packet.ON;

            send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.InitStateResister_GS ) );
            send.SubPacket.ERR = Packet.SET;
            send.SubPacket.DATA.Add( 0xFF );
            send.SubPacket.DATA.Add( 0xFF ^ ( byte )RegisterError1.BatterySafeAlarm );

            var receive = SendAndReceive( send );

            if ( receive == null )
            {
                Log( $"Init safety alarm failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( receive.SubPacket.ERR != 0 )
            {
                Log( $"Init safety alarm failed. (Cause: ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2}))", address );
                return ( Result )receive.SubPacket.ERR;
            }
            else
            {
                Log( $"Init safety alarm done.", address );
                return Result.NoError;
            }
        }
        /// <summary>
        /// 지정된 채널의 상태 레지스터를 읽어옵니다.
        /// <br>이 명령은 슬레이브 보드로 송신하는 경우 ByPass로 송신됩니다.</br>
        /// </summary>
        /// <param name="address">상태 레지스터를 읽어올 채널의 번호입니다. 0부터 시작합니다.</param>
        /// <returns>읽어온 상태 레지스터 정보입니다.</returns>
        internal RegisterError0 GetRegister0( byte addr, byte ch )
        {
            var address = new Address( addr, ch );

            Log( "Func: GetRegister", address );

            var send = new SendPacket( address );
            if ( !Address.IsMaster( address ) ) send.ByPass = Packet.ON;

            send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.InitStateResister_GS ) );

            var receive = SendAndReceive( send );

            if ( receive == null )
            {
                Log( $"Get reg failed. (Cause: No response)", address );

                throw new QException( QExceptionType.ACTION_DEVICE_NOT_RESPONSE_ERROR );
            }
            else
            {
                var reg = ( RegisterError0 )receive.SubPacket.DATA[0];
                Log( $"Get reg done. (Res: {reg})", address );

                return reg;
            }
        }
        /// <summary>
        /// 지정된 채널의 상태 레지스터를 읽어옵니다.
        /// <br>이 명령은 슬레이브 보드로 송신하는 경우 ByPass로 송신됩니다.</br>
        /// </summary>
        /// <param name="address">상태 레지스터를 읽어올 채널의 번호입니다. 0부터 시작합니다.</param>
        /// <returns>읽어온 상태 레지스터 정보입니다.</returns>
        internal RegisterError1 GetRegister1( byte addr, byte ch )
        {
            var address = new Address( addr, ch );

            Log( "Func: GetRegister", address );

            var send = new SendPacket( address );
            if ( !Address.IsMaster( address ) ) send.ByPass = Packet.ON;

            send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.InitStateResister_GS ) );

            var receive = SendAndReceive( send );

            if ( receive == null )
            {
                Log( $"Get reg failed. (Cause: No response)", address );

                throw new QException( QExceptionType.ACTION_DEVICE_NOT_RESPONSE_ERROR );
            }
            else
            {
                var reg = ( RegisterError1 )receive.SubPacket.DATA[1];
                Log( $"Get reg done. (Res: {reg})", address );

                return reg;
            }
        }
        internal Result GetRegister( byte addr, byte ch, out RegisterError0 err0, out RegisterError1 err1 )
        {
            var address = new Address( addr, ch );

            err0 = RegisterError0.NoError;
            err1 = RegisterError1.NoError;

            Log( "Func: GetRegister", address );

            var send = new SendPacket( address );
            if ( !Address.IsMaster( address ) ) send.ByPass = Packet.ON;

            send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.InitStateResister_GS ) );

            var receive = SendAndReceive( send );

            if ( receive == null )
            {
                Log( $"Get reg failed. (Cause: No response)", address );

                return Result.NoResponse;
            }
            else
            {
                err0 = ( RegisterError0 )receive.SubPacket.DATA[0];
                err1 = ( RegisterError1 )receive.SubPacket.DATA[1];
                var reg = ( RegisterError )( ushort )( ( receive.SubPacket.DATA[0] << 8 ) | receive.SubPacket.DATA[1] );
                Log( $"Get reg done. (Res: 0x{( byte )err0:X2}{( byte )err1:X2} = {reg.ConvertRegisterErrorToString()})" );

                return Result.NoError;
            }
        }
        /// <summary>
        /// 마스터 보드 상태 레지스터의 SD카드 오류 플래그를 초기화합니다.
        /// <br>이 명령은 SD카드의 마운트를 해제한 뒤 다시 마운트하는 동작을 수행합니다.</br>
        /// </summary>
        /// /// <returns><see cref="Result.NoError"/> : 전송 성공
        /// <br><see cref="Result.NoResponse"/> : 응답 없음</br>
        /// <br><see cref="Result.Com_ActionError"/> ~ <see cref="Result.Com_NotDefinedError"/> : ERR 필드가 0이 아닌 경우</br></returns>
        internal Result InitSdCardError()
        {
            Log( $"Func: InitSdCardAlarm" );

            var send = new SendPacket( Address.Master );

            send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.InitStateResister_GS ) );
            send.SubPacket.ERR = Packet.SET;
            send.SubPacket.DATA.Add( 0xFF );
            send.SubPacket.DATA.Add( 0xFF ^ ( byte )( RegisterError1.SdReadWriteFail ) );

            var receive = SendAndReceive( send );

            if ( receive == null )
            {
                Log( $"Init sd alarm failed. (Cause: No response)" );
                return Result.NoResponse;
            }
            else if ( receive.SubPacket.ERR != 0 )
            {
                Log( $"Init sd alarm failed. (Cause: ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2}))" );
                return ( Result )receive.SubPacket.ERR;
            }
            else
            {
                Log( $"Init sd alarm done." );
                return Result.NoError;
            }
        }
        /// <summary>
        /// 마스터 보드 상태 레지스터의 Recipe Sending Fail 오류 플래그를 초기화합니다.
        /// </summary>
        /// /// <returns><see cref="Result.NoError"/> : 전송 성공
        /// <br><see cref="Result.NoResponse"/> : 응답 없음</br>
        /// <br><see cref="Result.Com_ActionError"/> ~ <see cref="Result.Com_NotDefinedError"/> : ERR 필드가 0이 아닌 경우</br></returns>
        internal Result InitRecipeFailError()
        {
            Log( $"Func: InitRecipeFailError" );

            var send = new SendPacket( Address.Master );

            send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.InitStateResister_GS ) );
            send.SubPacket.ERR = Packet.SET;
            send.SubPacket.DATA.Add( 0xFF ^ ( byte )RegisterError0.RecipeSendingFail );
            send.SubPacket.DATA.Add( 0xFF );

            var receive = SendAndReceive( send );

            if ( receive == null )
            {
                Log( $"Init recipe fail reg failed. (Cause: No response)" );
                return Result.NoResponse;
            }
            else if ( receive.SubPacket.ERR != 0 )
            {
                Log( $"Init recipe fail reg failed. (Cause: ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2}))" );
                return ( Result )receive.SubPacket.ERR;
            }
            else
            {
                Log( $"Init recipe fail reg done." );
                return Result.NoError;
            }
        }
        /// <summary>
        /// 마스터 보드 상태 레지스터에서 SD RW fail 플래그를 제외한 모든 오류 플래그를 초기화합니다.
        /// </summary>
        /// <returns><see cref="Result.NoError"/> : 전송 성공
        /// <br><see cref="Result.NoResponse"/> : 응답 없음</br>
        /// <br><see cref="Result.Com_ActionError"/> ~ <see cref="Result.Com_NotDefinedError"/> : ERR 필드가 0이 아닌 경우</br></returns>
        internal Result InitRegister()
        {
            Log( $"Func: InitRegister" );

            var send = new SendPacket( Address.Master );

            send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.InitStateResister_GS ) );
            send.SubPacket.ERR = Packet.SET;
            send.SubPacket.DATA.Add( 0x00 );
            send.SubPacket.DATA.Add( 0x00 | RegisterError1.SdReadWriteFail );

            var receive = SendAndReceive( send );

            if ( receive == null )
            {
                Log( $"Init master reg failed. (Cause: No response)" );
                return Result.NoResponse;
            }
            else if ( receive.SubPacket.ERR != 0 )
            {
                Log( $"Init master reg failed. (Cause: ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2}))" );
                return ( Result )receive.SubPacket.ERR;
            }
            else
            {
                Log( $"Init master reg done." );
                return Result.NoError;
            }
        }
        /// <summary>
        /// 지정된 ADDR, CH의 상태 레지스터에서 지정된 플래그에 해당하는 오류 플래그를 초기화합니다.
        /// <br>지정할 플래그는 0인 경우 초기화, 1인 경우 상태 유지입니다. 초기화할 RegisterError0 또는 RegisterError1 플래그들을 Or 연산으로 합친 뒤, Not 연산으로 뒤집어 지정하십시오.</br>
        /// </summary>
        /// <param name="addr">초기화할 상태 레지스터의 ADDR입니다.</param>
        /// <param name="ch">초기화할 상태 레지스터의 CH입니다.</param>
        /// <param name="reg0">초기화할 첫 번째 바이트의 플래그입니다. RegisterError0 열거형 값을 사용해 Or 연산으로 여러 플래그를 지정할 수 있습니다.</param>
        /// <param name="reg1">초기화할 두 번째 바이트의 플래그입니다. RegisterError1 열거형 값을 사용해 Or 연산으로 여러 플래그를 지정할 수 있습니다.</param>
        /// <returns></returns>
        internal Result InitRegister( byte addr, byte ch, byte reg0, byte reg1 )
        {
            Log( $"Func: InitRegister" );

            var send = new SendPacket( addr, ch );

            send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.InitStateResister_GS ) );
            send.SubPacket.ERR = Packet.SET;
            send.SubPacket.DATA.Add( ( byte )reg0 );
            send.SubPacket.DATA.Add( ( byte )reg1 );

            var receive = SendAndReceive( send );

            if ( receive == null )
            {
                Log( $"Init reg failed. (Cause: No response)" );
                return Result.NoResponse;
            }
            else if ( receive.SubPacket.ERR != 0 )
            {
                Log( $"Init reg failed. (Cause: ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2}))" );
                return ( Result )receive.SubPacket.ERR;
            }
            else
            {
                Log( $"Init reg done." );
                return Result.NoError;
            }
        }

        /// <summary>
        /// 지정된 채널의 상태 정보를 읽어옵니다.
        /// <br>이 명령은 채널 전용 명령으로 ByPass로 송신되며, 마스터보드로 보낼 수 없습니다.</br>
        /// </summary>
        /// <param name="address">상태 정보를 읽어올 채널의 번호입니다. 0부터 시작합니다.</param>
        /// <returns>읽어온 채널 상태 정보입니다.</returns>
        /// <exception cref="QExceptionType.DEVELOP_WRONG_ADDRESS_USAGE_ERROR">마스터보드로 송신하려고 한 경우</exception>
        public ChannelStateData GetChannelState( Address address )
        {
            var send = new SendPacket( address );
            send.ByPass = Packet.ON;

            send.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_GetMeasureCommands.ChannelState_G ) );

            var receive = SendAndReceive( send );

            return new ChannelStateData( receive.SubPacket.DATA );
        }

        public void RefreshChannelState( byte addr, byte ch ) => RefreshChannelState( new Address( addr, ch ) );
        /// <summary>
        /// 지정된 채널의 상태를 최신 상태로 갱신하도록 요청합니다.
        /// </summary>
        /// <param name="address"></param>
        public void RefreshChannelState( Address address )
        {
            Log( "Func: RefreshChannelState", address );
            var send = new SendPacket( address );
            send.ByPass = Packet.ON;
            send.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_GetMeasureCommands.ChannelState_G ) );

            // 이곳에서 Receive를 기다리지 않고, 응답 패킷은 ParsingThread에서 처리됨.
            Send( send );
        }

        /// <summary>
        /// SD카드에 저장된 모든 데이터 파일을 삭제합니다.
        /// </summary>
        internal void RemoveSavedData()
        {
            Log( "Func: RemoveSavedData" );

            var send = new SendPacket( Address.Master );
            send.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.M_RemoveChannelData ) );
            send.SubPacket.ERR = Packet.SET;
            send.SubPacket.DATA.Add( 0 );   // Reserved

            var receive = SendAndReceive( send );
            if ( receive == null )
            {
                Log( "Remove saved data failed. (Cause: No response)" );
            }
            else if ( receive.SubPacket.ERR != 0 )
            {
                Log( "Remove saved data failed. (Cause: ERR not zero)" );
            }
        }
        /// <summary>
        /// 데이터 송신 제어권을 설정하거나 해제합니다.
        /// </summary>
        /// <param name="onOff"><see langword="true"/>: ON, <see langword="false"/>: OFF</param>
        internal void SetTransmissionControl( bool onOff )
        {
            Log( "Func: SetTransmissionControl" );

            var send = new SendPacket( Address.Master );
            send.SubPackets.Add( new SendSubPacket( Commands.MultiChannelCommands.S_SlaveBoardTransmissionControl_GS ) );
            send.SubPacket.ERR = Packet.SET;
            send.SubPacket.DATA.Add( onOff ? Packet.ON : Packet.OFF );  // 0: 해제, 1: 설정

            var receive = SendAndReceive( send );
            if ( receive == null )
            {
                Log( "Set transmission control failed. (Cause: No response)" );
            }
            else if ( receive.SubPacket.ERR != 0 )
            {
                Log( "Set transmission control failed. (Cause: ERR not zero)" );
            }
        }

        private const int C_RANGE_COUNT = 2;
        /// <summary>
        /// 장비로부터 Calibration 값을 불러와 지정된 경로의 파일에 저장합니다.
        /// </summary>
        /// <param name="filepath"></param>
        internal void SaveCalValues( string filepath )
        {
            // 전류 Input Range1,2
            // 전류 Output Range1,2,
            // 전압 Input Range
            // 전압 Output Range

            var calValues = new ChannelCalValues[_channels.Length];
            double tmp;

            for ( var i = 0; i < _channels.Length; i++ )
            {
                calValues[i] = new ChannelCalValues( C_RANGE_COUNT );

                // Current (Range 2개)
                for ( var j = 0; j < C_RANGE_COUNT; j++ )
                {
                    // current input
                    if ( Get_CurrentInput_Slope( _channels[i].Address, ( byte )j, out tmp ) == Result.NoError ) calValues[i].CurrentInput[j].Slope = tmp;
                    if ( Get_CurrentInput_Offset( _channels[i].Address, ( byte )j, out tmp ) == Result.NoError ) calValues[i].CurrentInput[j].Offset = tmp;

                    // current output
                    if ( Get_CurrentOutput_Slope( _channels[i].Address, ( byte )j, out tmp ) == Result.NoError ) calValues[i].CurrentOutput[j].Slope = tmp;
                    if ( Get_CurrentOutput_Offset( _channels[i].Address, ( byte )j, out tmp ) == Result.NoError ) calValues[i].CurrentOutput[j].Offset = tmp;
                }

                // Voltage (Range X)
                // voltage input
                if ( Get_VoltageInput_Slope( _channels[i].Address, out tmp ) == Result.NoError ) calValues[i].VoltageInput.Slope = tmp;
                if ( Get_VoltageInput_Offset( _channels[i].Address, out tmp ) == Result.NoError ) calValues[i].VoltageInput.Offset = tmp;

                // voltage output
                if ( Get_VoltageOutput_Slope( _channels[i].Address, out tmp ) == Result.NoError ) calValues[i].VoltageOutput.Slope = tmp;
                if ( Get_VoltageOutput_Offset( _channels[i].Address, out tmp ) == Result.NoError ) calValues[i].VoltageOutput.Offset = tmp;
            }

            using ( var sw = new StreamWriter( filepath ) )
            {
                for ( var i = 0; i < _channels.Length; i++ )
                {
                    for ( var j = 0; j < C_RANGE_COUNT; j++ )
                    {
                        sw.WriteLine( $"C_In,{j},{_channels[i].Address.ADDR},{_channels[i].Address.CH},{calValues[i].CurrentInput[j].Slope},{calValues[i].CurrentInput[j].Offset}" );
                        sw.WriteLine( $"C_Out,{j},{_channels[i].Address.ADDR},{_channels[i].Address.CH},{calValues[i].CurrentOutput[j].Slope},{calValues[i].CurrentOutput[j].Offset}" );
                    }

                    sw.WriteLine( $"V_In,{_channels[i].Address.ADDR},{_channels[i].Address.CH},{calValues[i].VoltageInput.Slope},{calValues[i].VoltageInput.Offset}" );
                    sw.WriteLine( $"V_Out,{_channels[i].Address.ADDR},{_channels[i].Address.CH},{calValues[i].VoltageOutput.Slope},{calValues[i].VoltageOutput.Offset}" );
                }
            }
        }
        /// <summary>
        /// 지정된 경로의 파일로부터 Calibration 값을 읽어와 장비로 설정합니다.
        /// </summary>
        /// <param name="filepath"></param>
        internal void LoadCalValues( string filepath )
        {
            if ( File.Exists( filepath ) )
            {
                var calValues = new ChannelCalValues[_channels.Length];

                for ( var i = 0; i < _channels.Length; i++ )
                {
                    calValues[i] = new ChannelCalValues( C_RANGE_COUNT );
                }

                using ( var sr = new StreamReader( filepath ) )
                {
                    var lines = sr.ReadToEnd().Split( Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
                    for ( var i = 0; i < lines.Length; i++ )
                    {
                        var split = lines[i].Split( ",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
                        if ( split.Length != 0 )
                        {
                            switch ( split[0].ToLower() )
                            {
                                case "c_in":
                                case "c_out":
                                    if ( split.Length == 6 )
                                    {
                                        if ( int.TryParse( split[1], out int r ) && int.TryParse( split[2], out int addr ) && int.TryParse( split[3], out int ch ) &&
                                            double.TryParse( split[4], out double slope ) && double.TryParse( split[5], out double offset ) )
                                        {
                                            var idx = Util.GetIndex( ( byte )addr, ( byte )ch );
                                            if ( 0 <= idx && idx < calValues.Length && 0 <= r && r < C_RANGE_COUNT )
                                            {
                                                if ( split[0].ToLower() == "c_in" )
                                                {
                                                    calValues[idx].CurrentInput[r].Slope = slope;
                                                    calValues[idx].CurrentInput[r].Offset = offset;
                                                }
                                                else
                                                {
                                                    calValues[idx].CurrentOutput[r].Slope = slope;
                                                    calValues[idx].CurrentOutput[r].Offset = offset;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case "v_in":
                                case "v_out":
                                    if ( split.Length == 5 )
                                    {
                                        if ( int.TryParse( split[1], out int addr ) && int.TryParse( split[2], out int ch ) &&
                                            double.TryParse( split[3], out double slope ) && double.TryParse( split[4], out double offset ) )
                                        {
                                            var idx = Util.GetIndex( ( byte )addr, ( byte )ch );

                                            if ( 0 <= idx && idx < calValues.Length )
                                            {
                                                if ( split[0].ToLower() == "v_in" )
                                                {
                                                    calValues[idx].VoltageInput.Slope = slope;
                                                    calValues[idx].VoltageInput.Offset = offset;
                                                }
                                                else
                                                {
                                                    calValues[idx].VoltageOutput.Slope = slope;
                                                    calValues[idx].VoltageOutput.Offset = offset;
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }

                for ( var i = 0; i < calValues.Length; i++ )
                {
                    var address = new Address( Util.GetADDR( i ), Util.GetCH( i ) );

                    for ( var j = 0; j < C_RANGE_COUNT; j++ )
                    {
                        Set_CurrentInput_Slope( address, ( byte )j, calValues[i].CurrentInput[j].Slope );
                        Set_CurrentInput_Offset( address, ( byte )j, calValues[i].CurrentInput[j].Offset );

                        Set_CurrentOutput_Slope( address, ( byte )j, calValues[i].CurrentOutput[j].Slope );
                        Set_CurrentOutput_Offset( address, ( byte )j, calValues[i].CurrentOutput[j].Offset );
                    }

                    Set_VoltageInput_Slope( address, calValues[i].VoltageInput.Slope );
                    Set_VoltageInput_Offset( address, calValues[i].VoltageInput.Offset );

                    Set_VoltageOutput_Slope( address, calValues[i].VoltageOutput.Slope );
                    Set_VoltageOutput_Offset( address, calValues[i].VoltageOutput.Offset );
                }
            }
        }
        #endregion

        #region Send And Receive
        /// <summary>
        /// 패킷을 송신합니다. 응답을 대기하지 않습니다.
        /// </summary>
        /// <param name="sendPacket">송신할 패킷입니다.</param>
        /// <returns></returns>
        public void Send( Packet sendPacket )
        {
            if ( !_isConnected ) return;
            
            _sendQueue.Enqueue( sendPacket );
            //try
            //{
            //    var rawPacket = sendPacket.ToByteArray();
            //    _stream.Write(rawPacket, 0, rawPacket.Length);
            //    log(sendPacket);
            //}
            //catch (IOException ex)
            //{
            //    Log($"Socket exception detected. (Cause: {ex.Message})");

            //    // 보내려다 실패한 패킷은 소실되지 않도록 다시 sendQueue에 넣는다.
            //    //_sendQueue.Enqueue(packet);
            //    //continue;
            //}
            //catch (Exception ex)
            //{
            //    Log($"Unknown exception detected. (Cause: {ex.Message})");
            //}
        }
        /// <summary>
        /// 패킷을 송신합니다. 이 메서드에서 응답을 대기하지는 않지만 별도의 스레드가 지정된 타임아웃 시간 내에 응답이 오는지 확인합니다.
        /// <br>지정된 시간 내에 응답이 오지 않는 경우 응답 확인 스레드가 예외를 던집니다.</br>
        /// </summary>
        /// <param name="sendPacket">송신할 패킷입니다.</param>
        /// <param name="timeOutSec">응답이 올 때까지 기다릴 시간(초)입니다. 기본 값은 5입니다.</param>
        [Obsolete( "Use send.", true )]
        public void Send( Packet sendPacket, int timeOutSec = 5 )
        {
            if ( !_isConnected ) return;

            var waiter = new ReceiveWaiter( sendPacket, DateTime.Now, timeOutSec );

            _receiveWaiterList.Add( waiter );

            _sendQueue.Enqueue( sendPacket );
        }
        /// <summary>
        /// 패킷을 송신하고, 응답 패킷이 올 때까지 대기합니다.
        /// <br>패킷의 에러는 처리하지 않습니다.</br>
        /// </summary>
        /// <param name="sendPacket">송신할 패킷입니다.</param>
        /// <param name="timeOutSec">응답이 올 때까지 기다릴 시간(초)입니다. 기본 값은 5이며, -1로 설정할 경우 무한 대기합니다.</param>
        /// <returns>보낸 패킷에 대한 응답 패킷으로 판단된 패킷입니다. 장비가 연결되어 있지 않거나 타임아웃이 발생한 경우 null입니다.</returns>
        public Packet SendAndReceive( Packet sendPacket, int timeOutSec = 3, int tryCount = 3 )
        {
            if ( !_isConnected ) return null;

            do
            {
                // Waiter 개체를 생성한다.
                var waiter = new ReceiveWaiter( sendPacket );

                _receiveWaiterList.Add( waiter );

                _sendQueue.Enqueue( sendPacket );
                //Send(sendPacket);

                var sendTime = DateTime.Now;
                while ( ( DateTime.Now - sendTime ).TotalSeconds < timeOutSec )
                {
                    if ( waiter.Received != null )
                    {
                        var received = waiter.Received;
                        lock ( _receiveWaiterList )
                        {
                            _receiveWaiterList.Remove( waiter );
                        }
                        return received;
                    }
                    else Application.DoEvents();
                }

                lock ( _receiveWaiterList )
                {
                    _receiveWaiterList.Remove( waiter );
                    tryCount--;
                }
            } while ( tryCount > 0 );

            return null;
        }
        #endregion

        #region Calibration Methods
        // 전류 출력 보상값
        internal Result Get_CurrentOutput_Slope( Address address, byte range, out double slope )
        {
            Log( $"Func: {nameof( Get_CurrentOutput_Slope )}", address );
            slope = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputCurrentSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( range );   // Range

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_CurrentOutput_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_CurrentOutput_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : Range no
                // DATA[1] : Reserved
                // DATA[2]~DATA[9] : Slope
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                slope = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_CurrentOutput_Slope( Address address, byte range, double slope )
        {
            Log( $"Func: {nameof( Set_CurrentOutput_Slope )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputCurrentSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( range );   // Range
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( slope );   // Slope

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_CurrentOutput_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_CurrentOutput_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }
        internal Result Get_CurrentOutput_Offset( Address address, byte range, out double offset )
        {
            Log( $"Func: {nameof( Get_CurrentOutput_Offset )}", address );
            offset = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputCurrentOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( range );   // Range

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_CurrentOutput_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_CurrentOutput_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : Range no
                // DATA[1] : Reserved
                // DATA[2]~DATA[9] : Offset
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                offset = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_CurrentOutput_Offset( Address address, byte range, double offset )
        {
            Log( $"Func: {nameof( Set_CurrentOutput_Offset )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputCurrentOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( range );   // Range
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( offset );  // Offset

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_CurrentOutput_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_CurrentOutput_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }

        //// 전압 출력 보상값
        internal Result Get_VoltageOutput_Slope( Address address, out double slope )
        {
            Log( $"Func: {nameof( Get_VoltageOutput_Slope )}", address );
            slope = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputVoltageSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( 0 );   // Reserved

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_VoltageOutput_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_VoltageOutput_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : Reserved
                // DATA[1] : Reserved
                // DATA[2]~DATA[9] : Slope
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                slope = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_VoltageOutput_Slope( Address address, double slope )
        {
            Log( $"Func: {nameof( Set_VoltageOutput_Slope )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputVoltageSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( slope );   // Slope

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_VoltageOutput_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_VoltageOutput_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }
        internal Result Get_VoltageOutput_Offset( Address address, out double offset )
        {
            Log( $"Func: {nameof( Get_VoltageOutput_Offset )}", address );
            offset = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputVoltageOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( 0 );   // Reserved

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_VoltageOutput_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_VoltageOutput_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : Reserved
                // DATA[1] : Reserved
                // DATA[2]~DATA[9] : Offset
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                offset = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_VoltageOutput_Offset( Address address, double offset )
        {
            Log( $"Func: {nameof( Set_VoltageOutput_Offset )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputVoltageOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( offset );  // Offset

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_VoltageOutput_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_VoltageOutput_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }

        //// Limit 보상값
        internal Result Get_Limit_Slope( Address address, LimitType limitType, LimitModeRange limitModeRange, out double slope )
        {
            Log( $"Func: {nameof( Get_Limit_Slope )}", address );
            slope = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.LimitSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( limitType );       // LimitType
            send.SubPacket.DATA.Add( limitModeRange );  // LimitModeRange

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_Limit_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_Limit_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : LimitType
                // DATA[1] : LimitModeRange
                // DATA[2]~DATA[9] : Slope
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                slope = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_Limit_Slope( Address address, LimitType limitType, LimitModeRange limitModeRange, double slope )
        {
            Log( $"Func: {nameof( Set_Limit_Slope )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.LimitSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( limitType );       // LimitType
            send.SubPacket.DATA.Add( limitModeRange );  // LimitModeRange
            send.SubPacket.DATA.Add( slope );           // Slope

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_Limit_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_Limit_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }
        internal Result Get_Limit_Offset( Address address, LimitType limitType, LimitModeRange limitModeRange, out double offset )
        {
            Log( $"Func: {nameof( Get_Limit_Offset )}", address );
            offset = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.LimitOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( limitType );       // LimitType
            send.SubPacket.DATA.Add( limitModeRange );  // LimitModeRange

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_Limit_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_Limit_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : LimitType
                // DATA[1] : LimitModeRange
                // DATA[2]~DATA[9] : Offset
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                offset = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_Limit_Offset( Address address, LimitType limitType, LimitModeRange limitModeRange, double offset )
        {
            Log( $"Func: {nameof( Set_Limit_Offset )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.LimitOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( limitType );       // LimitType
            send.SubPacket.DATA.Add( limitModeRange );  // LimitModeRange
            send.SubPacket.DATA.Add( offset );          // Offset

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_Limit_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_Limit_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }

        //// Cancel 전압 출력 보상값
        internal Result Get_CancelVoltageOutput_Slope( Address address, CancelChannel cancelChannel, out double slope )
        {
            Log( $"Func: {nameof( Get_CancelVoltageOutput_Slope )}", address );
            slope = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputCancelVoltageSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( cancelChannel );   // CancelChannel

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_CancelVoltageOutput_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_CancelVoltageOutput_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : CancelChannel
                // DATA[1] : Reserved
                // DATA[2]~DATA[9] : Slope
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                slope = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_CancelVoltageOutput_Slope( Address address, CancelChannel cancelChannel, double slope )
        {
            Log( $"Func: {nameof( Set_CancelVoltageOutput_Slope )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputCancelVoltageSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( cancelChannel );   // CancelChannel
            send.SubPacket.DATA.Add( 0 );               // Reserved
            send.SubPacket.DATA.Add( slope );           // Slope

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_CancelVoltageOutput_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_CancelVoltageOutput_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }
        internal Result Get_CancelVoltageOutput_Offset( Address address, CancelChannel cancelChannel, out double offset )
        {
            Log( $"Func: {nameof( Get_CancelVoltageOutput_Offset )}", address );
            offset = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputCancelVoltageOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( cancelChannel );   // CancelChannel

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_CancelVoltageOutput_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_CancelVoltageOutput_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : CancelChannel
                // DATA[1] : Reserved
                // DATA[2]~DATA[9] : Offset
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                offset = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_CancelVoltageOutput_Offset( Address address, CancelChannel cancelChannel, double offset )
        {
            Log( $"Func: {nameof( Set_CancelVoltageOutput_Offset )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputCancelVoltageOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( cancelChannel );   // CancelChannel
            send.SubPacket.DATA.Add( 0 );               // Reserved
            send.SubPacket.DATA.Add( offset );          // Offset

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_CancelVoltageOutput_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_CancelVoltageOutput_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }

        //// 전류 입력 보상값
        internal Result Get_CurrentInput_Slope( Address address, byte range, out double slope )
        {
            Log( $"Func: {nameof( Get_CurrentInput_Slope )}", address );
            slope = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.InputCurrentSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( range );   // Range

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_CurrentInput_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_CurrentInput_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : Range no
                // DATA[1] : Reserved
                // DATA[2]~DATA[9] : Slope
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                slope = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_CurrentInput_Slope( Address address, byte range, double slope )
        {
            Log( $"Func: {nameof( Set_CurrentInput_Slope )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.InputCurrentSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( range );   // Range
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( slope );   // Slope

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_CurrentInput_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_CurrentInput_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }
        internal Result Get_CurrentInput_Offset( Address address, byte range, out double offset )
        {
            Log( $"Func: {nameof( Get_CurrentInput_Offset )}", address );
            offset = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.InputCurrentOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( range );   // Range

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_CurrentInput_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_CurrentInput_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : Range no
                // DATA[1] : Reserved
                // DATA[2]~DATA[9] : Offset
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                offset = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_CurrentInput_Offset( Address address, byte range, double offset )
        {
            Log( $"Func: {nameof( Set_CurrentInput_Offset )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.InputCurrentOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( range );   // Range
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( offset );  // Offset

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_CurrentInput_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_CurrentInput_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }

        //// 전압 입력 보상값
        internal Result Get_VoltageInput_Slope( Address address, out double slope )
        {
            Log( $"Func: {nameof( Get_VoltageInput_Slope )}", address );
            slope = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.InputVoltageSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( 0 );   // Reserved

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_VoltageInput_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_VoltageInput_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : Reserved
                // DATA[1] : Reserved
                // DATA[2]~DATA[9] : Slope
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                slope = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_VoltageInput_Slope( Address address, double slope )
        {
            Log( $"Func: {nameof( Set_VoltageInput_Slope )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.InputVoltageSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( slope );   // Slope

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_VoltageInput_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_VoltageInput_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }
        internal Result Get_VoltageInput_Offset( Address address, out double offset )
        {
            Log( $"Func: {nameof( Get_VoltageInput_Offset )}", address );
            offset = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.InputVoltageOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( 0 );   // Reserved

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_VoltageInput_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_VoltageInput_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : Reserved
                // DATA[1] : Reserved
                // DATA[2]~DATA[9] : Offset
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                offset = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_VoltageInput_Offset( Address address, double offset )
        {
            Log( $"Func: {nameof( Set_VoltageInput_Offset )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.InputVoltageOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( offset );  // Offset

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_VoltageInput_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_VoltageInput_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }

        //// 증폭 전압 입력 보상값
        internal Result Get_AmplifiedVoltageInput_Slope( Address address, AmplifyMode amplifyMode, out double slope )
        {
            Log( $"Func: {nameof( Get_AmplifiedVoltageInput_Slope )}", address );
            slope = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.InputAmplifiedVoltageSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( amplifyMode );   // AmplifyMode

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_AmplifiedVoltageInput_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_AmplifiedVoltageInput_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : AmplifyMode
                // DATA[1] : Reserved
                // DATA[2]~DATA[9] : Slope
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                slope = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_AmplifiedVoltageInput_Slope( Address address, AmplifyMode amplifyMode, double slope )
        {
            Log( $"Func: {nameof( Set_AmplifiedVoltageInput_Slope )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.InputAmplifiedVoltageSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( amplifyMode );     // AmplifyMode
            send.SubPacket.DATA.Add( 0 );               // Reserved
            send.SubPacket.DATA.Add( slope );           // Slope

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_AmplifiedVoltageInput_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_AmplifiedVoltageInput_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }
        internal Result Get_AmplifiedVoltageInput_Offset( Address address, AmplifyMode amplifyMode, out double offset )
        {
            Log( $"Func: {nameof( Get_AmplifiedVoltageInput_Offset )}", address );
            offset = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.InputAmplifiedVoltageOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( amplifyMode );   // AmplifyMode

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_AmplifiedVoltageInput_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_AmplifiedVoltageInput_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : AmplifyMode
                // DATA[1] : Reserved
                // DATA[2]~DATA[9] : Offset
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                offset = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_AmplifiedVoltageInput_Offset( Address address, AmplifyMode amplifyMode, double offset )
        {
            Log( $"Func: {nameof( Set_AmplifiedVoltageInput_Offset )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.InputAmplifiedVoltageSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( amplifyMode );     // AmplifyMode
            send.SubPacket.DATA.Add( 0 );               // Reserved
            send.SubPacket.DATA.Add( offset );          // Offset

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_AmplifiedVoltageInput_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_AmplifiedVoltageInput_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }

        //// 온도 보상값
        internal Result Get_Temperature_Slope( Address address, out double slope )
        {
            Log( $"Func: {nameof( Get_Temperature_Slope )}", address );
            slope = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.TemperatureSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( 0 );   // Reserved

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_Temperature_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_Temperature_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : Reserved
                // DATA[1] : Reserved
                // DATA[2]~DATA[9] : Slope
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                slope = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_Temperature_Slope( Address address, double slope )
        {
            Log( $"Func: {nameof( Set_Temperature_Slope )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputVoltageSlope_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( slope );   // Slope

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_Temperature_Slope )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_Temperature_Slope )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }
        internal Result Get_Temperature_Offset( Address address, out double offset )
        {
            Log( $"Func: {nameof( Get_Temperature_Offset )}", address );
            offset = 0;

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputVoltageOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( 0 );   // Reserved

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Get_Temperature_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Get_Temperature_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                // DATA[0] : Reserved
                // DATA[1] : Reserved
                // DATA[2]~DATA[9] : Offset
                if ( received.SubPacket.DATA.Count < 10 ) return Result.Sw_PacketError;
                offset = new Q_Double( received.SubPacket.DATA[2], received.SubPacket.DATA[3], received.SubPacket.DATA[4], received.SubPacket.DATA[5],
                                      received.SubPacket.DATA[6], received.SubPacket.DATA[7], received.SubPacket.DATA[8], received.SubPacket.DATA[9] ).Value;
                return Result.NoError;
            }
        }
        internal Result Set_Temperature_Offset( Address address, double offset )
        {
            Log( $"Func: {nameof( Set_Temperature_Offset )}", address );

            var send = new SendPacket( address, Commands.CalibrationCommands.OutputVoltageOffset_GS );
            send.ByPass = Packet.ON;
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( 0 );       // Reserved
            send.SubPacket.DATA.Add( offset );  // Offset

            var received = SendAndReceive( send );
            if ( received == null )
            {
                Log( $"{nameof( Set_Temperature_Offset )} failed. (Cause: No response)", address );
                return Result.NoResponse;
            }
            else if ( received.SubPacket.ERR != 0 )
            {
                Log( $"{nameof( Set_Temperature_Offset )} failed. (Cause: ERR => {( ErrorField )received.SubPacket.ERR}(0x{received.SubPacket.ERR:X2}))", address );
                return ( Result )received.SubPacket.ERR;
            }
            else
            {
                return Result.NoError;
            }
        }
        #endregion

        #region Thread Methods
        // 각 스레드 종료용 변수
        private bool _sendRun;
        private bool _receiveRun;
        private bool _pingRun;
        private bool _splitRun;
        private bool _parsingRun;
        private bool _scheduleRun;

        // 각 스레드 동작 상태 확인용 속성
        /// <summary>
        /// 송신 스레드가 동작중인지의 여부입니다.
        /// </summary>
        public bool IsSendRun => _isSendRun;
        /// <summary>
        /// 수신 스레드가 동작중인지의 여부입니다.
        /// </summary>
        public bool IsReceiveRun => _isReceiveRun;
        /// <summary>
        /// Ping 스레드가 동작중인지의 여부입니다.
        /// </summary>
        public bool IsPingRun => _isPingRun;
        /// <summary>
        /// 수신 바이트 분리 스레드가 동작중인지의 여부입니다.
        /// </summary>
        public bool IsSplitRun => _isSplitRun;
        /// <summary>
        /// 패킷 파싱 스레드가 동작중인지의 여부입니다.
        /// </summary>
        public bool IsParsingRun => _isParsingRun;
        /// <summary>
        /// 스케쥴러 스레드가 동작중인지의 여부입니다.
        /// </summary>
        public bool IsScheduleRun => _isScheduleRun;

        // 스레드 동작 상태를 저장하는 변수, 각 스레드에서 값을 갱신한다. 스레드 외부에서 값을 변경하지 말 것.
        private bool _isSendRun;
        private bool _isReceiveRun;
        private bool _isPingRun;
        private bool _isSplitRun;
        private bool _isParsingRun;
        private bool _isScheduleRun;


        public void Wait(int milliseconds = 1)
        {
            int Timing = Environment.TickCount + milliseconds;
            while (Timing > Environment.TickCount)
            {
                Application.DoEvents();
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 모든 Communicator 스레드를 실행합니다.
        /// </summary>
        private bool startAllThreads( int timeOutSec = 2 )
        {
            // 이미 스레드가 동작중인지 스레드 내부에서 자체적으로 확인하는 기능이 있으므로 별도로 확인할 필요 없다.
            Log( $"Func: startAllThreads" );

            new Thread( receiveLoop ) { Name = "ReceiveLoop", IsBackground = true, Priority = ThreadPriority.Normal }.Start();
            new Thread( splitLoop ) { Name = "SplitLoop", IsBackground = true }.Start();
            new Thread( parsingLoop ) { Name = "ParsingLoop", IsBackground = true }.Start();
            new Thread( sendLoop ) { Name = "SendLoop", IsBackground = true }.Start();
            new Thread( scheduleLoop ) { Name = "ScheduleLoop", IsBackground = true }.Start();
            if ( _isAutoPing ) new Thread( pingLoop ) { Name = "PingLoop", IsBackground = true }.Start();

            var time = DateTime.Now;

            while ( ( timeOutSec == -1 ) || ( DateTime.Now - time ).TotalSeconds < timeOutSec )
            {
                if ( _isSendRun && _isReceiveRun && _isParsingRun && _isSplitRun && _isScheduleRun && ( _isAutoPing == _isPingRun ) )
                {
                    Log( $"All trd started." );
                    return true;
                }

                Application.DoEvents();
            }

            return false;
        }
        /// <summary>
        /// 모든 Communicator 스레드에 중단 요청을 보낸 후 스레드가 완전히 종료될 때까지 대기합니다.
        /// </summary>
        /// <param name="timeOutSec">대기할 시간(초)입니다. -1인 경우 무한히 대기합니다.</param>
        /// <returns>모든 스레드가 완전히 종료되었다고 판단된 경우 true이고, 타임 아웃인 경우 false입니다.</returns>
        private bool stopAllThreads( int timeOutSec = 2 )
        {
            Log( $"Func: stopAllThreads" );

            _receiveRun = false;
            _sendRun = false;
            _pingRun = false;
            _splitRun = false;
            _parsingRun = false;

            for ( var i = 0; i < _channels.Length; i++ ) _channels[i].ChannelCommand = ChannelCommand.Idle;
            _scheduleRun = false;

            var time = DateTime.Now;

            while ( ( timeOutSec == -1 ) || ( DateTime.Now - time ).TotalSeconds < timeOutSec )
            {
                if ( !_isSendRun && !_isReceiveRun && !_isPingRun && !_isParsingRun && !_isSplitRun && !_scheduleRun )
                {
                    Log( $"All trd stopped." );
                    return true;
                }

                Application.DoEvents();
            }

            Thread.Sleep( 1000 );
            if ( _isSendRun ) Log( $"Send trd still alive." );
            if ( _isReceiveRun ) Log( $"Receive trd still alive." );
            if ( _isPingRun ) Log( $"Ping trd still alive." );
            if ( _isSplitRun ) Log( $"Split trd still alive." );
            if ( _isParsingRun ) Log( $"Parsing trd still alive." );
            if ( _isScheduleRun ) Log( $"Scheduler trd still alive." );
            return false;
        }

        /// <summary>
        /// sendQueue에 들어온 패킷을 순차적으로 Socket의 NetworkStream에 쓰는 스레드.
        /// </summary>
        private void sendLoop()
        {
            if ( _isSendRun ) return;

            Log( $"Send trd start." );

            _sendRun = true;

            // SendQueue에 들어온 패킷을 Socket에 쓴다.
            while ( _sendRun )
            {
                _isSendRun = true;
                //Application.DoEvents();
                Wait(1);
                Packet packet = null;

                if ( !_sendQueue.TryDequeue( out packet ) || packet == null )
                {
                    continue;
                }

                try
                {
                    var rawPacket = packet.ToByteArray();
                    _stream.Write( rawPacket, 0, rawPacket.Length );
                    log( packet );
                }
                catch ( IOException ex )
                {
                    Log( $"Socket exception detected. (Cause: {ex.Message})" );

                    // 보내려다 실패한 패킷은 소실되지 않도록 다시 sendQueue에 넣는다.
                    _sendQueue.Enqueue( packet );
                    continue;
                }
                catch ( Exception ex )
                {
                    Log( $"Unknown exception detected. (Cause: {ex.Message})" );
                }
            }

            _isSendRun = false;
            _sendRun = false;

            Log( $"Send trd stopped." );
        }

        /// <summary>
        /// Socket의 DataAvailable이 true일 때 NetworkStream의 모든 바이트를 읽어서 byteQueue에 넣는 스레드.
        /// </summary>
        private void receiveLoop()
        {
            if ( _isReceiveRun ) return;

            Log( $"Rcv trd start." );

            _receiveRun = true;
            var receiveBuffer = new byte[8252];

            // Socket의 NetworkStream에서 내용을 계속 읽어서 byteStack에 push한다.
            while ( _receiveRun )
            {
                _isReceiveRun = true;
                //Application.DoEvents();
                Wait(10);
                try
                {
                    if ( _stream != null && _stream.DataAvailable )
                    {
                        var read = _stream.Read( receiveBuffer, 0, receiveBuffer.Length );

                        if ( read != 0 )
                        {
                            var tmp = new byte[read];
                            Array.Copy( receiveBuffer, tmp, read );
                            _byteList.AddRange( tmp );
                            //split();
                            _lastComTime = DateTime.Now;
                        }
                    }
                }
                catch ( Exception ex )
                {
                    Log( $"Unknown exception detected. (Rcv loop : {ex.Message})" );
                    continue;
                }
            }

            _receiveRun = false;
            _isReceiveRun = false;

            Log( $"Rcv trd stopped." );
        }

        /// <summary>
        /// byteQueue에 들어온 데이터를 STX2부터 ETX2까지 잘라서 packetQueue로 보내는 스레드.
        /// </summary>
        ///
        private int _startIndex = -1;
        private void split()
        {
            _isSplitRun = true;

            if (_byteList.Count < 4)
            {
                return;
            }

            // STX2 4byte를 찾는다.
            if (_startIndex == -1)
            {
                for (var i = 0; i < _byteList.Count - 4; i++)
                {
                    if (_byteList[i] == Packet._STX2_1 &&
                         _byteList[i + 1] == Packet._STX2_2 &&
                         _byteList[i + 2] == Packet._STX2_3 &&
                         _byteList[i + 3] == Packet._STX2_4)
                    {
                        _startIndex = i;
                        break;
                    }
                }
            }

            if (_startIndex == -1) return;

            // 첫 번째 STX2가 i일 때 i+7, i+8이 LEN2이다. 이 값을 가져온다.
            // index가 0일 때 기준 7, 8번 인덱스의 값을 가져와야 하므로
            if (_byteList.Count < _startIndex + 9) return;

            var len2 = new Q_UInt16(_byteList[_startIndex + 7], _byteList[_startIndex + 8]).Value;
            var packetLength = 9 + len2 + 3;
            if (_byteList.Count < _startIndex + packetLength) return;

            // i+8+LEN2+2 가 ETX2_1이고, i+8+LEN2+3이 ETX2_2인지 확인하고, 맞다면 찢어서 패킷 큐로 보낸다.
            if (_byteList[_startIndex + packetLength - 2] == Packet._ETX2_1 && _byteList[_startIndex + packetLength - 1] == Packet._ETX2_2)
            {
                // STX2 앞에까지를 버리고 STX2부터 ETX2까지를 찢어서 패킷 큐로 보낸다.
                byte[] packet = new byte[packetLength];
                _byteList.CopyTo(_startIndex, packet, 0, packetLength);

                lock (_byteList)
                {
                    _byteList.RemoveRange(0, _startIndex + packetLength);
                }

                _rawPacketQueue.Enqueue(packet);
                _startIndex = -1;
            }
            else
            {
                // 버림, 0부터 STX2까지만 버림
                // 잘못된 STX2의 앞에 있는 값들은 STX2가 확실히 없으므로 같이 버림
                Log($"Err bytes detected(Split loop): {Util.BytesToString(_byteList.GetRange(0, _startIndex + 4).ToArray())}");
                //_errorQueue.Enqueue( new ErrorPackage( _byteList.GetRange( 0, startIndex + 4 ).ToArray(), "WrongBytes" ) );

                lock (_byteList)
                {
                    _byteList.RemoveRange(0, _startIndex + 4);
                    _startIndex = -1;
                }
            }
        }
        private void splitLoop()
        {
            if ( _isSplitRun ) return;

            Log( $"Split trd start." );

            _splitRun = true;

            var startIndex = -1;

            while ( _splitRun )
            {
                _isSplitRun = true;
                //Application.DoEvents();
                Wait(10);

                if ( _byteList.Count < 4 )
                {
                    continue;
                }

                // STX2 4byte를 찾는다.
                if ( startIndex == -1 )
                {
                    for ( var i = 0; i < _byteList.Count - 4; i++ )
                    {
                        if ( _byteList[i] == Packet._STX2_1 &&
                             _byteList[i + 1] == Packet._STX2_2 &&
                             _byteList[i + 2] == Packet._STX2_3 &&
                             _byteList[i + 3] == Packet._STX2_4 )
                        {
                            startIndex = i;
                            break;
                        }
                    }
                }

                if ( startIndex == -1 ) continue;

                // 첫 번째 STX2가 i일 때 i+7, i+8이 LEN2이다. 이 값을 가져온다.
                // index가 0일 때 기준 7, 8번 인덱스의 값을 가져와야 하므로
                if ( _byteList.Count < startIndex + 9 ) continue;

                var len2 = new Q_UInt16( _byteList[startIndex + 7], _byteList[startIndex + 8] ).Value;
                var packetLength = 9 + len2 + 3;
                if ( _byteList.Count < startIndex + packetLength ) continue;

                // i+8+LEN2+2 가 ETX2_1이고, i+8+LEN2+3이 ETX2_2인지 확인하고, 맞다면 찢어서 패킷 큐로 보낸다.
                if ( _byteList[startIndex + packetLength - 2] == Packet._ETX2_1 && _byteList[startIndex + packetLength - 1] == Packet._ETX2_2 )
                {
                    // STX2 앞에까지를 버리고 STX2부터 ETX2까지를 찢어서 패킷 큐로 보낸다.
                    byte[] packet = new byte[packetLength];
                    _byteList.CopyTo( startIndex, packet, 0, packetLength );

                    lock ( _byteList )
                    {
                        _byteList.RemoveRange( 0, startIndex + packetLength );
                    }

                    _rawPacketQueue.Enqueue( packet );
                    startIndex = -1;
                }
                else
                {
                    // 버림, 0부터 STX2까지만 버림
                    // 잘못된 STX2의 앞에 있는 값들은 STX2가 확실히 없으므로 같이 버림
                    Log( $"Err bytes detected(Split loop): {Util.BytesToString( _byteList.GetRange( 0, startIndex + 4 ).ToArray() )}" );
                    //_errorQueue.Enqueue( new ErrorPackage( _byteList.GetRange( 0, startIndex + 4 ).ToArray(), "WrongBytes" ) );

                    lock ( _byteList )
                    {
                        _byteList.RemoveRange( 0, startIndex + 4 );
                        startIndex = -1;
                    }
                }
            }

            Log( $"Split trd stopped." );

            _isSplitRun = false;
            _splitRun = false;
        }

        /// <summary>
        /// Communicator.PingInterval(초)마다 SendAndReceive()로 Ping 명령을 보내어 장비 연결 상태를 확인하는 스레드.
        /// <br><see cref="PingInterval"/>초 내로 Ping에 대한 응답이 오지 않을 경우 Reconnect()로 재연결을 시도한다.</br>
        /// </summary>
        private void pingLoop()
        {
            if ( _isPingRun ) return;

            Log( $"Ping trd start." );

            _pingRun = true;

            // 일정 시간마다 sendQueue에 Ping 명령을 enqueue한다.
            while ( _pingRun && IsAutoPing )
            {
                _isPingRun = true;
                Application.DoEvents();

                // 장비와 마지막으로 통신한 시간으로부터 PingInterval이 경과한 경우
                if ( ( DateTime.Now - _lastComTime ).TotalSeconds > PingInterval )
                {
                    var pingPacket = new SendPacket( Address.Master, Commands.CommonCommands.Ping_G );
                    Log( "Ping." );

                    if ( SendAndReceive( pingPacket ) == null )
                    {
                        // Ping not responsed
                        Log( "Ping timeout. Try reconnect." );

                        if ( !connect() )
                        {
                            Log( "Reconnect failed." );
                            stopAllThreads();
                            Disconnect();
                        }
                    }
                    else
                    {
                        _lastComTime = DateTime.Now;
                        Log( "Ping received." );
                    }
                }

                //var pingPacket = new SendPacket( Address.Master );
                //pingPacket.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.Ping_G ) );

                //if ( SendAndReceive( pingPacket ) != null )
                //{
                //    for ( var i = 0; i < PingInterval && _pingRun; i++ )
                //    {
                //        Thread.Sleep( 1000 );
                //    }
                //}
                //else
                //{
                //    Log( $"Ping timeout." );

                //    // Ping 응답이 오지 않는다 -> Reconnect 시도
                //    if ( _onConnecting ) continue;

                //    Log( $"Try reconnect." );
                //    if ( !connect() )
                //    {
                //        // Reconnect 실패 시
                //        Log( $"Reconnect failed." );
                //        _isPingRun = false;
                //        _pingRun = false;

                //        if ( !_onDisconnecting ) Disconnect();

                //        break;
                //    }
                //}
            }

            Log( $"Ping trd stopped." );

            _isPingRun = false;
            _pingRun = false;
        }

        /// <summary>
        /// packetQueue에 들어온 데이터를 Packet 객체 형태로 구성하여 처리 Queue로 보내는 스레드
        /// </summary>
        private void parsingLoop()
        {
            if ( _isParsingRun ) return;

            Log( $"Parsing trd start." );

            _parsingRun = true;
            _savedDataBuffer.Clear();

            // packetQueue에 들어온 패킷을 dequeue하여 Command형태로 구분하고,
            // 패킷의 최종 목적지의 패킷큐에 전달한다.
            while ( _parsingRun )
            {
                _isParsingRun = true;
                //Application.DoEvents();
                Wait(10);
                if ( !_rawPacketQueue.TryDequeue( out byte[] packetArray ) || packetArray == null )
                {
                    continue;
                }

                Packet packet = ReceivedPacket.Parse( packetArray );
                log( packet );

                // Packet Parsing
                if ( packet.ParsingState == ParsingState.Complete )
                {
                    bool answered = false;
                    lock ( _receiveWaiterList )
                    {
                        foreach ( var w in _receiveWaiterList )
                        {
                            if ( w.CompareTo( packet ) )
                            {
                                w.Received = packet;
                                answered = true;
                                break;
                            }
                        }
                    }

                    // ReceiveWaiter에 응답 패킷으로 지정했다면 SendAndReceive()를 호출한 쪽에서 응답 패킷 처리를 위해 대기하고 있을 것이므로, 이쪽에서는 더이상 처리할 필요가 없다.
                    if ( answered ) continue;

                    if ( Intercept != null )
                    {
                        Intercept.Invoke( packet );
                        continue;
                    }

                    // 이건 Send로 Master한테 명령 보낸것임.
                    if ( packet.SubPacket.CH == 0 ) continue;

                    // 수신된 패킷의 명령어에 따라 필요한 처리가 다른 경우 아래 switch 문에서 case로 작성
                    switch ( packet[0].Command )
                    {
                        #region 측정 데이터 패킷 처리 - ACK만 보냄
                        // 채널 시퀀스 데이터, 패턴 데이터, 고속 측정 데이터의 경우 ACK를 보낸다.
                        case Commands.BatteryCycler_GetMeasureCommands.ChannelSequenceData_R:
                        case Commands.BatteryCycler_GetMeasureCommands.PatternSequenceData_R:
                        case Commands.BatteryCycler_GetMeasureCommands.FastMeasureSequenceData_R:
                            // ACK 패킷 생성
                            // ADDR2, CH2는 첫 번째 SubPacket의 ADDR과 CH를 사용한다.
                            var ackPacket = new SendPacket( packet[0].ADDR, packet[0].CH );

                            ackPacket.SubPackets.Add( new SendSubPacket( packet.SubPacket.Command ) );

                            // SubPacket의 DATA 필드 구성
                            // 처음 Reserved 필드 2Byte
                            ackPacket[0].DATA.AddCount( 0, 2 );
                            // 첫 번째 SubPacket의 Step count 4Byte
                            ackPacket[0].DATA.Add( packet[0].DATA[8], packet[0].DATA[9], packet[0].DATA[10], packet[0].DATA[11] );
                            // 첫 번째 SubPacket의 Step number 2Byte
                            ackPacket[0].DATA.Add( packet[0].DATA[12], packet[0].DATA[13] );
                            // Cycle no, mode1, mode2는 0으로 6Byte
                            ackPacket[0].DATA.AddCount( 0, 6 );
                            // Reserved 항목에 첫 번째 SubPacket의 index 4Byte
                            ackPacket[0].DATA.Add( packet[0].DATA[20], packet[0].DATA[21], packet[0].DATA[22], packet[0].DATA[23] );
                            // 나머지 Reserved 필드 4Byte
                            ackPacket[0].DATA.AddCount( 0, 4 );
                            // ACK 패킷 송신
                            Send( ackPacket );
                            break;
                        #endregion

                        #region 저장 데이터 처리(이어붙이기) - ACK 보내고 패킷을 채널로 보내는 과정까지 다 진행 후 continue로 루프 종료함
                        case Commands.BatteryCycler_GetMeasureCommands.M_SavedSequenceData_R:
                            // 저장된 시퀀스 데이터의 경우 ACK를 보내고, 내부 패킷을 다시 새로운 통합 프로토콜2 패킷으로 분할하여 각 채널로 보낸다.
                            if ( !_appendingDetected )
                            {
                                Log( $"Appending data detected.", new Address( packet[0].ADDR, packet[0].CH ) );
                                _appendingDetected = true;
                            }
                            //_isAppending = true;

                            // 저장된 시퀀스 데이터 패킷의 경우 ADDR2, CH2는 0xFF로 고정이고,
                            // SubPacket의 ADDR, CH는 첫 번째 내부 패킷의 첫 번재 SubPacket의 ADDR, CH이다.
                            // 일단 ACK를 보낸다.
                            ackPacket = new SendPacket( packet[0].ADDR, packet[0].CH );

                            ackPacket.SubPackets.Add( new SendSubPacket( packet.SubPacket.Command ) );

                            Send( ackPacket );

                            // 첫 번째 SubPacket의 DATA 필드를 마스킹을 해제하여 저장 데이터 버퍼에 넣는다.
                            for ( var i = 0; i < packet.SubPacket.DATA.Count; i++ )
                                _savedDataBuffer.Add( ( byte )( packet.SubPacket.DATA[i] ^ 0x80 ) );

                            while ( true )
                            {
                                var startIndex = -1;
                                byte[] savedPacketRaw = null;

                                // STX2 4byte를 찾는다.
                                if ( startIndex == -1 )
                                {
                                    for ( var i = 0; i < _savedDataBuffer.Count - 4; i++ )
                                    {
                                        if ( _savedDataBuffer[i] == Packet._STX2_1 &&
                                             _savedDataBuffer[i + 1] == Packet._STX2_2 &&
                                             _savedDataBuffer[i + 2] == Packet._STX2_3 &&
                                             _savedDataBuffer[i + 3] == Packet._STX2_4 )
                                        {
                                            startIndex = i;
                                            break;
                                        }
                                    }
                                }

                                if ( startIndex == -1 ) break;

                                // 첫 번째 STX2가 i일 때 i+7, i+8이 LEN2이다. 이 값을 가져온다.
                                // index가 0일 때 기준 7, 8번 인덱스의 값을 가져와야 하므로
                                if ( _savedDataBuffer.Count < startIndex + 9 ) break;

                                var len2 = new Q_UInt16( _savedDataBuffer[startIndex + 7], _savedDataBuffer[startIndex + 8] ).Value;
                                var packetLength = 9 + len2 + 3;
                                if ( _savedDataBuffer.Count < startIndex + packetLength ) break;

                                // i+8+LEN2+2 가 ETX2_1이고, i+8+LEN2+3이 ETX2_2인지 확인하고, 맞다면 찢어서 패킷 큐로 보낸다.
                                if ( _savedDataBuffer[startIndex + packetLength - 2] == Packet._ETX2_1 && _savedDataBuffer[startIndex + packetLength - 1] == Packet._ETX2_2 )
                                {
                                    // STX2 앞에까지를 버리고 STX2부터 ETX2까지를 찢어서 패킷 큐로 보낸다.
                                    savedPacketRaw = new byte[packetLength];
                                    _savedDataBuffer.CopyTo( startIndex, savedPacketRaw, 0, packetLength );
                                    _savedDataBuffer.RemoveRange( 0, startIndex + packetLength );

                                    startIndex = -1;
                                }
                                else
                                {
                                    // 버림, 0부터 STX2까지만 버림
                                    // 잘못된 STX2의 앞에 있는 값들은 STX2가 확실히 없으므로 같이 버림
                                    Log( $"Err bytes detected at saved data packet: {Util.BytesToString( _savedDataBuffer.GetRange( 0, startIndex + packetLength ).ToArray() )}" );
                                    //_errorQueue.Enqueue( new ErrorPackage( _savedDataBuffer.GetRange( 0, startIndex + 4 ).ToArray(), "WrongBytes" ) );

                                    lock ( _savedDataBuffer )
                                    {
                                        _savedDataBuffer.RemoveRange( 0, startIndex + 4 );
                                        startIndex = -1;
                                    }
                                }

                                ReceivedPacket savedPacket = null;

                                try
                                {
                                    if ( savedPacketRaw != null )
                                    {
                                        savedPacket = ReceivedPacket.Parse( savedPacketRaw );
                                    }
                                    else break;
                                }
                                catch ( IndexOutOfRangeException ex )
                                {
#if CONSOLEOUT
                                    Console.WriteLine( $"Saved data packet parsing exception. ParsingState : {savedPacket.ParsingState}" );
#endif
                                }

                                // 파싱 성공한 경우 각 채널로 SubPacket들을 보낸다.
                                if ( savedPacket.ParsingState == ParsingState.Complete )
                                {
                                    foreach ( var subPacket in savedPacket.SubPackets )
                                    {
                                        if ( subPacket.ADDR != 0 && subPacket.CH != 0 && subPacket.ERR == 0 )
                                        {
                                            // 이어붙이기 데이터의 목적지 채널의 State가 Appending이 아닌 경우 Appending으로 변경한다.
                                            var index = Util.GetIndex( subPacket.ADDR, subPacket.CH );

                                            if ( index >= 0 && index < _channels.Length )
                                            {
                                                if ( _useAppendingState )
                                                {
                                                    if ( _channels[index].State != State.APPENDING )
                                                    {
                                                        _channels[index].State = State.APPENDING;
                                                        Application.DoEvents();
                                                    }
                                                }
                                                else
                                                {
                                                    _channels[index].IsAppending = true;
                                                }
                                                _channels[index].Push( subPacket );
                                            }
                                            else
                                            {
                                                Log( $"Saved data packet has wrong index. {Util.BytesToString( savedPacketRaw )}" );
                                            }
                                        }
                                    }

                                    if ( _savedDataBuffer.Count >= 8 )
                                    {
                                        // 원래 FFFFFFFF00000000은 마스킹되지 않은 상태로 오지만 DATA 필드를 무조건 마스킹해제해서 버퍼에 넣었으므로 얘네도 마스킹이 해제된 상태이다.
                                        if ( _savedDataBuffer[0] == 0x7F &&
                                             _savedDataBuffer[1] == 0x7F &&
                                             _savedDataBuffer[2] == 0x7F &&
                                             _savedDataBuffer[3] == 0x7F &&
                                             _savedDataBuffer[4] == 0x80 &&
                                             _savedDataBuffer[5] == 0x80 &&
                                             _savedDataBuffer[6] == 0x80 &&
                                             _savedDataBuffer[7] == 0x80 )
                                        {
                                            Log( $"Appending data end. (ADDR = 0x{savedPacket.SubPacket.ADDR:X2})", new Address( savedPacket.SubPacket.ADDR, 0 ) );

                                            if ( IsOldDevice )
                                            {
                                                _channels[packet[0].CH].State = State.IDLE;
                                            }
                                            else
                                            {
                                                var board = packet[0].ADDR;
                                                for ( var i = 1; i <= 8; i++ )
                                                {
                                                    var index = Util.GetIndex( board, ( byte )i );
                                                    if (index < _channels.Length) {
                                                        if (_useAppendingState)
                                                        {
                                                            if (_channels[index].State == State.APPENDING)
                                                            {
                                                                _channels[index].State = State.IDLE;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            _channels[index].IsAppending = false;
                                                        }
                                                    }
                             
                                                }
                                            }

                                            _savedDataBuffer.RemoveRange( 0, 8 );
                                        }
                                    }
                                }
                                else
                                {
                                    // Error Packet
                                    Log( $"Err packet detected at saved data packet. (Cause: Parsing failed) {Util.BytesToString( savedPacketRaw )}" );
                                }
                            }

                            // 저장데이터 패킷 처리 후 밑에는 갈 필요 없음
                            continue;
                            #endregion
                    }

                    // 패킷을 SubPacket 단위로 찢어서 각 채널의 Receiver에게 보내는 부분
                    for ( var i = 0; i < packet.SubPackets.Count; i++ )
                    {
                        if ( packet.SubPackets[i].CH != 0 )
                        {
                            // 정상 채널로 송신된 패킷 (CH이 0이 아닌 경우)
                            // 단, SubPacket의 ERR 필드가 0이 아닌 경우에는 errorQueue로 보낸다.
                            if ( Util.GetIndex( packet[i].ADDR, packet[i].CH ) is int index &&
                                index >= 0 && index < _channels.Length )
                            {
                                _channels[index].Push( packet[i] );
                            }
                            else
                            {
                                Log( $"Err packet (Parsing Loop). (Cause: Idx out of range) {Util.BytesToString( packet[i] )}" );
                            }
                        }
                        else
                        {
                            // ADDR과 CH가 잘못된 패킷 (CH가 0인 경우)
                            Log( $"Err packet (Parsing Loop). (Cause: Wrong address) {Util.BytesToString( packet[i] )}" );
                        }
                    }
                }
                else if ( packet.ParsingState == ParsingState.CRC2 )
                {
                    // 패킷의 CRC 오류 처리
                    Log( $"Err packet. (Cause: CRC2) {packet.ToLogText( true )}" );
                }
                else
                {
                    Log( $"Err packet. (Cause: Parsing) {Util.BytesToString( packet )}" );
                }
            }

            Log( $"Parsing trd stopped." );

            _isParsingRun = false;
            _parsingRun = false;
        }

        private Result displayMessage( Channel ch, Result result )
        {
            switch ( result )
            {
                case Result.NoResponse:
                    ch.SetMessage( "No response" );
                    break;

                case Result.Com_ActionError:
                case Result.Com_InitError:
                case Result.Com_PacketError:
                case Result.Com_CmdError:
                case Result.Com_BusyError:
                case Result.Com_SetError:
                case Result.Com_SdError:
                case Result.Com_AddressError:
                case Result.Com_NotDefinedError:
                    ch.SetMessage( $"Com error(0x{( byte )result:X2})" );
                    break;

                case Result.InvalidSequence:
                    ch.SetMessage( "Invalid Sequence" );
                    break;

                case Result.RegClearFail:
                    ch.SetMessage( "Clear failed" );
                    break;

                case Result.TryCount:
                    ch.SetMessage( "Try count failed" );
                    break;

            }

            return result;
        }
        /// <summary>
        /// 각 채널에 지정된 ChannelCommand에 따라 일련의 동작을 수행하는 스레드
        /// <br>(명령 송수신은 무조건 한 번에 한 채널씩 진행하는 것이 옳으므로)</br>
        /// </summary>
        private void scheduleLoop()
        {
            if ( _isScheduleRun ) return;

            //Log( $"Schedule trd start." );

            _scheduleRun = true;

            while ( _scheduleRun )
            {
                _isScheduleRun = true;
                //Application.DoEvents();
                Wait(10);

                for ( var i = 0; i < _channels.Length && _scheduleRun; i++ )
                {
                    var ch = _channels[i];

                    try
                    {
                        if ( ch.ChannelCommand == ChannelCommand.Idle )
                        {
                            if ( ch.State == State.SENDING )
                            {
                                ch.State = State.IDLE;
                                ch.SetMessage( "" );
                            }
                            continue;
                        }

                        // 채널 메시지는 ChannelCommand가 Idle이 아니고서야 일단 삭제 - 만약 또 오류가 발생한다면 어차피 아래에서 다시 올려줄 것임
                        ch.SetMessage( "" );

                        switch ( ch.ChannelCommand )
                        {
                            case ChannelCommand.Start:
                                if ( ch.State != State.IDLE && ch.State != State.ERROR )
                                {
                                    //Log( $"Start denied. (Cause: Ch state not idle)", ch.Address );
                                    break;
                                }

                                if ( displayMessage( ch, GetRegister( 0, 0, out RegisterError0 err0, out RegisterError1 err1 ) ) == Result.NoError )
                                {
                                    if ( err0 != RegisterError0.NoError || err1 != RegisterError1.NoError )
                                    {
                                        byte needClear1 = 0xFF, needClear2 = 0xFF;
                                        if ( err0.HasFlag( RegisterError0.RecipeSendingFail ) ) needClear1 ^= ( byte )RegisterError0.RecipeSendingFail;

                                        if ( err1.HasFlag( RegisterError1.SdInitializeFail ) )
                                        {
                                            _sdFail = true;
                                        }
                                        else
                                        {
                                            _sdFail = false;
                                        }

                                        if ( err1.HasFlag( RegisterError1.SdReadWriteFail ) ) needClear2 ^= ( byte )RegisterError1.SdReadWriteFail;

                                        if ( needClear1 != 0xFF || needClear2 != 0xFF )
                                        {
                                            if ( displayMessage( ch, InitRegister( 0, 0, needClear1, needClear2 ) ) != Result.NoError )
                                            {
                                                ch.State = State.ERROR;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {   // no response
                                    ch.State = State.ERROR;
                                    break;
                                }

                                if ( displayMessage( ch, InitSafetyAlarm( ch.Address ) ) != Result.NoError )
                                {   // 레지스터 초기화 실패
                                    ch.State = State.ERROR;
                                    break;
                                }

                                ch.State = State.SENDING;

                                if ( displayMessage( ch, SendSequence( ch.Address, ch.Sequence ) ) == Result.NoError )
                                {   // 시퀀스 전송 성공

                                    ch.Ready();
                                    ch.State = State.READY;
                                    ch.ChannelCommand = ChannelCommand.Ready;
                                    //continue;

                                    //ch.State = State.IDLE;
                                    //if ( displayMessage( ch, StartChannel( ch.Address ) ) == Result.NoError )
                                    //{   // 시작 명령 전송 성공
                                    //    //ch.State =( State.Idle );
                                    //    ch.SetMessage( "" );
                                    //}
                                    //else
                                    //{   // 시작 명령 전송 실패
                                    //    ch.State = State.ERROR;
                                    //}
                                }
                                else
                                {   // 시퀀스 전송 실패
                                    ch.State = State.ERROR;
                                }
                                break;

                            case ChannelCommand.Ready:
                                if ( displayMessage( ch, StartChannel( ch.Address ) ) == Result.NoError )
                                {   // 시작 명령 전송 성공
                                    ch.State = State.IDLE;
                                    ch.SetMessage( "" );
                                    SaveChannelInfos( i );
                                }
                                else
                                {   // 시작 명령 전송 실패
                                    ch.State = State.ERROR;
                                }
                                ch.ChannelCommand = ChannelCommand.Idle;
                                break;

                            case ChannelCommand.Stop:
                                if ( displayMessage( ch, StopChannel( ch.Address ) ) == Result.NoError )
                                {
                                    ch.Lock = true;
                                    ch.State = State.IDLE;
                                    ch.Init();
                                    SaveChannelInfos( i );
                                }
                                else
                                {
                                    ch.State = State.ERROR;
                                }
                                break;

                            case ChannelCommand.Pause:
                                if ( displayMessage( ch, PauseChannel( ch.Address ) ) == Result.NoError )
                                {
                                    ch.State = State.PAUSED;
                                }
                                else
                                {
                                    ch.State = State.ERROR;
                                }
                                break;

                            case ChannelCommand.Skip:
                                SkipChannel( ch.Address );
                                break;

                            case ChannelCommand.SafetyRestart:
                                InitSafetyAlarm( ch.Address );
                                if ( RestartChannel( ch.Address ) == Result.NoError )
                                {
                                    ch.State = State.IDLE;
                                }
                                break;

                            case ChannelCommand.Restart:
                                RestartChannel( ch.Address );
                                break;

                            case ChannelCommand.Cancel:
                                ch.State = State.IDLE;
                                ch.SetMessage( "" );
                                break;

                            case ChannelCommand.ErrorClear:
                                try
                                {
                                    // 마스터보드 상태 레지스터 조회 후 초기화 (오류 있는 경우만)
                                    var reg0 = GetRegister0( 0, 0 );
                                    var reg1 = GetRegister1( 0, 0 );

                                    byte clearReg0 = 0x00;
                                    byte clearReg1 = 0x00;

                                    if ( reg0.HasFlag( RegisterError0.RecipeSendingFail ) ) clearReg0 |= ( byte )RegisterError0.RecipeSendingFail;
                                    if ( reg1.HasFlag( RegisterError1.SdReadWriteFail ) ) clearReg1 |= ( byte )RegisterError1.SdReadWriteFail;
                                    if ( reg1.HasFlag( RegisterError1.SdInitializeFail ) ) clearReg1 |= ( byte )RegisterError1.SdInitializeFail;

                                    if ( clearReg0 != 0x00 || clearReg1 != 0x00 )
                                    {
                                        if ( InitRegister( 0, 0, ( byte )~clearReg0, ( byte )~clearReg1 ) != Result.NoError )
                                        {
                                            ch.SetMessage( "Register clear failed" );
                                        }
                                    }

                                    // 채널 상태 레지스터 초기화
                                    if ( displayMessage( ch, InitSafetyAlarm( ch.Address ) ) != Result.NoError )
                                    {   // 레지스터 초기화 실패
                                        ch.State = State.ERROR;
                                    }

                                }
                                catch ( QException ex )
                                {
                                    // 응답 없음
                                    ch.SetMessage( "No response" );
                                    break;
                                }
                                finally
                                {
                                    // 채널 정지 시도
                                    if ( displayMessage( ch, StopChannel( ch.Address ) ) != Result.NoError )
                                    {
                                        ch.State = State.ERROR;
                                    }
                                    else
                                    {
                                        // 클리어
                                        ch.State = State.IDLE;
                                        ch.Init();
                                    }
                                }
                                break;

                            case ChannelCommand.StopAppending:
                                for ( var j = 0; j < _channels.Length; j++ )
                                {
                                    if ( _channels[j].State != State.IDLE )
                                    {
                                        StopChannel( _channels[j].Address );
                                        _channels[j].State = State.IDLE;
                                    }

                                    _channels[j].ChannelCommand = ChannelCommand.Idle;
                                }
                                SetTransmissionControl( false );
                                RemoveSavedData();
                                break;
                        }
                    }
                    catch ( QException ex )
                    {
#if CONSOLEOUT
                        Console.WriteLine( $"Scheduler exception : {ex.Message}" );
#endif
                    }
                    finally
                    {
                        if ( ch.ChannelCommand != ChannelCommand.Ready )
                        {
                            ch.ChannelCommand = ChannelCommand.Idle;
                        }
                        Application.DoEvents();
                    }
                }
            }

            _isScheduleRun = false;

            for ( var i = 0; i < _channels.Length; i++ ) _channels[i].ChannelCommand = ChannelCommand.Idle;
        }
        #endregion

        #region Do not modify
        private bool _disposedValue;
        private void Dispose( bool disposing )
        {
            if ( !_disposedValue )
            {
                if ( disposing )
                {
                    // TODO: 관리형 상태(관리형 개체)를 삭제합니다.
                    Disconnect();
                    stopAllThreads();
                    _socket?.Close();
                    for ( var i = 0; i < _channels.Length; i++ )
                    {
                        _channels[i]?.Stop();
                    }
                    _receiveWaiterList.Clear();
                    _byteList.Clear();
                    while ( !_rawPacketQueue.IsEmpty ) _rawPacketQueue.TryDequeue( out byte[] dummy );
                    while ( !_sendQueue.IsEmpty ) _sendQueue.TryDequeue( out Packet dummy );

                    foreach ( var c in _channels ) _totalChannels.Remove( c );
                }

                // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                _disposedValue = true;
            }
        }

        // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        // ~Communicator()
        // {
        //     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            Dispose( disposing: true );
            GC.SuppressFinalize( this );
        }
        #endregion
    }
}
