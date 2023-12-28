using McQLib.Core;
using McQLib.IO;
using McQLib.Recipes;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace McQLib.Device
{
    internal sealed class ChannelInfo
    {
        public int ChannelIndex;
        public string SaveDirectory;
        public string SaveFileName;
        public string Name;
        public string SequencePath;
        public uint StepNo;
        public DateTime StartTime;
        public string ExportFilePath;

        public ChannelInfo() { }
        public ChannelInfo( Channel channel )
        {
            ChannelIndex = channel.GlobalIndex;
            SaveDirectory = channel.SaveDirectory;
            SaveFileName = channel.SaveFileName;
            Name = channel.Name;
            SequencePath = channel.Sequence != null ? channel.Sequence.FilePath : "";
            StepNo = channel.StepNo;
            ExportFilePath = channel.ExportFilePath;
        }
    }

    /// <summary>
    /// 각 채널에 수행 대기중인 명령을 나타내는 열거형입니다.
    /// </summary>
    public enum ChannelCommand
    {
        /// <summary>
        /// 아무 동작도 예약되지 않음
        /// </summary>
        Idle,
        /// <summary>
        /// 시퀀스 송신 및 채널 시작 명령 예약
        /// </summary>
        Start,
        /// <summary>
        /// 시작 명령 예약 - 직접 지정 X
        /// </summary>
        Ready,
        /// <summary>
        /// 정지 명령 예약
        /// </summary>
        Stop,
        /// <summary>
        /// 일시정지 명령 예약
        /// </summary>
        Pause,
        /// <summary>
        /// 스킵 명령 예약
        /// </summary>
        Skip,
        /// <summary>
        /// 재시작 명령 예약
        /// </summary>
        Restart,
        /// <summary>
        /// 안전 조건 재시작 명령 예약
        /// </summary>
        SafetyRestart,
        /// <summary>
        /// 에러 클리어 명령 예약
        /// </summary>
        ErrorClear,
        /// <summary>
        /// 이어붙이기 취소 명령 예약
        /// </summary>
        StopAppending,
        /// <summary>
        /// 예약된 명령 취소
        /// </summary>
        Cancel,
    }
    /// <summary>
    /// 각 채널의 현재 상태를 나타내는 열거형입니다.
    /// </summary>
    public enum State
    {
        /// <summary>
        /// 채널이 아무것도 하지 않는 대기 상태입니다.
        /// </summary>
        IDLE,
        /// <summary>
        /// 사용되지 않음.
        /// </summary>
        APPENDING,
        /// <summary>
        /// 시퀀스 송신중 상태입니다.
        /// </summary>
        SENDING,
        /// <summary>
        /// 시퀀스 송신을 끝낸 후 준비상태입니다.
        /// </summary>
        READY,
        /// <summary>
        /// 측정이 진행중 또는 이어붙이기 진행중인 상태입니다.
        /// </summary>
        RUN,
        /// <summary>
        /// 측정이 일시정지된 상태입니다.
        /// </summary>
        PAUSED,
        /// <summary>
        /// 측정이 안전 조건에 의해 일시정지된 상태입니다.
        /// </summary>
        SAFETY,
        /// <summary>
        /// 채널에 오류가 발생한 상태입니다.
        /// </summary>
        ERROR,
        /// <summary>
        /// 채널이 연결되지 않은 상태입니다.
        /// </summary>
        DISCONNECTED,
    }

    /// <summary>
    /// 하나의 채널에 대한 데이터를 처리하기 위한 클래스입니다.
    /// </summary>
    public sealed class Channel : INotifyPropertyChanged
    {
        #region Channel Options (Global)
        /// <summary>
        /// 채널이 전압 값을 표기하기 위한 단위 정보입니다. 모든 채널에 대해 적용됩니다.
        /// </summary>
        public static UnitInfo VoltageUnit { get; set; } = new UnitInfo( UnitType.Voltage );
        /// <summary>
        /// 채널이 전류 값을 표기하기 위한 단위 정보입니다. 모든 채널에 대해 적용됩니다.
        /// </summary>
        public static UnitInfo CurrentUnit { get; set; } = new UnitInfo( UnitType.Current );

        /// <summary>
        /// 채널에서 레시피가 다음 단계로 넘어갈 때마다 레시피가 종료된 원인을 기록하기 위한 옵션입니다. 현재 지원하지 않는 기능입니다.
        /// </summary>
        public static bool Logging { get; set; } = true;
        /// <summary>
        /// 채널의 측정이 종료되었을 때 채널이 가진 시퀀스 정보를 초기화할지의 여부입니다. 모든 채널에 대해 적용됩니다.
        /// </summary>
        public static bool IsClearSequenceWhenEnd { get; set; } = false;
        #endregion

        #region Channel Options (Local)
        /// <summary>
        /// 텍스트 내보내기 확장 기능의 파일 출력 경로입니다.
        /// <br>기능을 사용하지 않는 경우 string.Empty입니다.</br>
        /// </summary>
        public string ExportFilePath { get; set; } = string.Empty;
        /// <summary>
        /// 현재 채널이 이어붙이기를 진행중인지의 여부입니다.
        /// </summary>
        public bool IsAppending { get; internal set; }

        /// <summary>
        /// 현재 채널의 측정데이터가 저장될 파일의 이름입니다.
        /// </summary>
        public string SaveFileName
        {
            get => _saveFileName;
            set
            {
                if (_saveFileName != value)
                {
                    _saveFileName = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _saveFileName;

        /// <summary>
        /// 현재 채널에 적용되어있는 시퀀스입니다.
        /// </summary>
        public Sequence Sequence
        {
            get => _sequence;
            set
            {
                if ( _sequence != value )
                {
                    _sequence = value;
                    OnPropertyChanged();

                    if ( _sequence != null )
                    {
                        try
                        {
                            TotalSteps = _sequence.GetTotalSteps();
                        }
                        catch
                        {
#if CONSOLEOUT
                            Console.WriteLine( "Calc total steps failed." );
#endif
                        }
                    }

                }
            }
        }
        private Sequence _sequence = null;
        #endregion

        #region Channel Informations
        /// <summary>
        /// 현재 채널을 소유하는 마스터 Communicator입니다.
        /// </summary>
        public readonly Communicator Owner;
        /// <summary>
        /// 연결된 모든 Q 장비에 대해서 현재 채널이 가지는 순번입니다.
        /// </summary>
        public int GlobalIndex => Array.IndexOf( Communicator.TotalChannels, this );

        /// <summary>
        /// 현재 채널의 주소입니다.
        /// </summary>
        public Address Address => _address;
        private Address _address;

        /// <summary>
        /// 현재 채널이 속한 컴포넌트에서 현재 채널이 가지는 순번입니다.
        /// </summary>
        public readonly int LocalIndex;
        /// <summary>
        /// 채널이 속한 보드의 번호입니다.
        /// </summary>
        public byte ADDR => _address.ADDR;
        /// <summary>
        /// 채널이 속한 보드 상에서의 현재 채널의 번호입니다.
        /// </summary>
        public byte CH => _address.CH;

        /// <summary>
        /// 채널의 수신 스레드가 동작하고 있는지의 여부입니다.
        /// </summary>
        internal bool IsReceiveRun => _isChannelRun;
        private bool _channelRun;
        private bool _isChannelRun;

        private QDataWriter _writer;
        #endregion

        #region Notify Properties
        private State _state;
        private ulong _totalTime;
        private double _voltage;
        private double _current;
        private double _temperature;
        private double _capacity;
        private double _power;
        private string _recipeName;
        private uint _stepNo;
        private uint _stepCount;
        private int _totalSteps;
        private string _message;
        private string _saveDirectory;
        private string _name;

        /// <summary>
        /// 채널의 상태입니다.
        /// </summary>
        public State State
        {
            get => _state;
            internal set
            {
                if ( _state != value )
                {
                    Owner.Log( $"Ch state changed: {State}->{value}.", _address );
                    _state = value;

                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// 채널의 측정 경과 시간입니다.
        /// </summary>
        public ulong TotalTime
        {
            get => _totalTime;
            set
            {
                if ( _totalTime != value )
                {
                    _totalTime = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// 전압(V)입니다.
        /// </summary>
        public double Voltage
        {
            get => _voltage;
            set
            {
                if ( _voltage != value )
                {
                    _voltage = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// 전류(A)입니다.
        /// </summary>
        public double Current
        {
            get => _current;
            set
            {
                if ( _current != value )
                {
                    _current = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// 온도(℃)입니다.
        /// </summary>
        public double Temperature
        {
            get => _temperature;
            private set
            {
                if ( _temperature != value )
                {
                    _temperature = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// 전력(W)입니다.
        /// <br>레시피가 Charge 또는 Discharge인 경우에만 유효합니다.</br>
        /// </summary>
        public double Power
        {
            get => _power;
            set
            {
                if ( _power != value )
                {
                    _power = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// 용량(Ah)입니다.
        /// <br>레시피가 Charge 또는 Discharge인 경우에만 유효합니다.</br>
        /// </summary>
        public double Capacity
        {
            get => _capacity;
            private set
            {
                if ( _capacity != value )
                {
                    _capacity = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// 현재 측정중인 레시피 이름입니다.
        /// </summary>
        public string RecipeName
        {
            get => _recipeName;
            set
            {
                if ( _recipeName != value )
                {
                    _recipeName = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// 현재 레시피의 시퀀스상 번호입니다.
        /// </summary>
        public uint StepNo
        {
            get => _stepNo;
            set
            {
                if ( _stepNo != value )
                {
                    _stepNo = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// 총 스텝이 진행된 횟수입니다.
        /// </summary>
        public uint StepCount
        {
            get => _stepCount;
            set
            {
                if ( _stepCount != value )
                {
                    _stepCount = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// 현재 시퀀스의 전체 스텝 수 입니다.
        /// </summary>
        public int TotalSteps
        {
            get => _totalSteps;
            private set
            {
                if ( _totalSteps != value )
                {
                    _totalSteps = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// 현재 채널의 상태 메시지입니다.
        /// <br>채널이 정상 상태일 경우에는 <see cref="string.Empty"/>입니다.</br>
        /// </summary>
        public string Message
        {
            get => _message;
            set
            {
                if ( _message != value )
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// 측정 데이터가 저장될 경로입니다.
        /// </summary>
        public string SaveDirectory
        {
            get => _saveDirectory;
            set
            {
                if ( _saveDirectory != value )
                {
                    _saveDirectory = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// 실험의 이름입니다. 측정 데이터 파일의 이름으로 사용됩니다.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if ( _name != value )
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Channel Control
        /// <summary>
        /// 새로운 채널 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="ownerCommunicator">채널을 소유하는 마스터 Communicator입니다.</param>
        /// <param name="localIndex">채널의 지역 인덱스입니다.</param>
        /// <param name="boardNo">채널이 속한 보드의 번호입니다.</param>
        /// <param name="channelNo">채널이 속한 보드 상에서의 채널 번호입니다.</param>
        /// <param name="state">채널의 상태입니다.</param>
        public Channel( Communicator ownerCommunicator, int localIndex, byte boardNo, byte channelNo, State state )
        {
            Owner = ownerCommunicator;

            _address = new Address( localIndex, boardNo, channelNo );
            State = state;
        }

        /// <summary>
        /// 채널을 정지한 후 데이터가 이어서 들어오는 현상을 방지하기 위해 채널을 잠글 수 있다.
        /// </summary>
        internal bool Lock;
        /// <summary>
        /// 이 필드는 각 채널에 동작을 명령하기 위한 변수입니다.
        /// <br>각 채널에는 최대 1개의 명령만이 대기할 수 있으며, 기존에 주어진 명령이 수행되어 명령 대기 상태가 <see cref="ChannelCommand.Idle"/>로 변경되기 전에 다른 명령을 할당하여 대기중인 명령을 덮어쓸 수 있습니다.</br>
        /// </summary>
        public ChannelCommand ChannelCommand;
        /// <summary>
        /// 채널의 ADDR을 0으로 변경합니다.
        /// <br>이 옵션은 장비의 사양이 1보드-1채널인 경우에 필요합니다.</br>
        /// </summary>
        internal void SetAddrToZero() => _address.ADDR = 0;

        /// <summary>
        /// 채널이 가진 정보를 <see cref="ChannelInfo"/>의 인스턴스화 합니다.
        /// </summary>
        /// <returns></returns>
        internal ChannelInfo ToChannelInfo() => new ChannelInfo( this );
        /// <summary>
        /// <see cref="ChannelInfo"/> 인스턴스가 가진 정보를 현재 채널에 적용합니다.
        /// </summary>
        /// <param name="channelInfo">채널 정보를 가지는 <see cref="ChannelInfo"/> 인스턴스입니다.</param>
        internal void ApplyChannelInfo( ChannelInfo channelInfo )
        {
            SaveDirectory = channelInfo.SaveDirectory;
            _saveFileName = channelInfo.SaveFileName;
            Name = channelInfo.Name;
            if ( !string.IsNullOrEmpty( channelInfo.SequencePath ) ) Sequence = Sequence.FromFile( channelInfo.SequencePath );
            StepNo = channelInfo.StepNo;
            ExportFilePath = channelInfo.ExportFilePath;
        }

        /// <summary>
        /// 채널의 측정 상태와 측정 정보를 초기화합니다. 이 메서드는 측정이 종료된 후에 호출되어야 합니다.
        /// <br>초기화 되는 내용 : Message, SaveFileName, ExportFilePath</br>
        /// </summary>
        internal void Init()
        {
            _writer?.Close();
            _writer = null;

            //StepNo = 0;
            //TotalTime = 0;
            //Voltage = 0;
            //Current = 0;
            //Temperature = 0;
            //StepCount = 0;
            //RecipeName = "";

            Message = "";
            //_saveFileName = "";

            if ( IsClearSequenceWhenEnd )
            {
                _sequence = null;
            }

            ExportFilePath = string.Empty;
        }
        /// <summary>
        /// 채널을 측정 준비 상태로 만듭니다. 이 메서드는 측정이 시작되기 전에 호출되어야 합니다.
        /// <br>초기화되는 내용 : StepNo, TotalTime, Voltage, Current, Temperature, StepCount, RecipeName</br>
        /// <br>준비되는 내용 : Writer, SaveFileName</br>
        /// </summary>
        internal bool Ready()
        {
            try
            {
                // 채널 정보 초기화 - 어차피 아래 부분은 측정데이터패킷을 통해서만 갱신되므로 초기화하지 않아도 무방하나,
                // UI상에 뿌려지는 값을 정리해주기 위해서 함.
                StepNo = 0;
                TotalTime = 0;
                Voltage = 0;
                Current = 0;
                Temperature = 0;
                StepCount = 0;
                RecipeName = "";

                Lock = false;

                // 기존 Writer 해제
                _writer?.Close();
                _writer = null;


                // 파일 이름 및 경로 설정
                if ( !string.IsNullOrWhiteSpace( SaveDirectory ) && !Directory.Exists( SaveDirectory ) ) Directory.CreateDirectory( SaveDirectory );

                var name = Name;
                if ( !string.IsNullOrEmpty( name ) ) name += "_";

                
                _saveFileName = $"{name}CH{GlobalIndex + 1}_{DateTime.Now:yyMMdd_HHmmss}.qrd";

                SaveFileName = _saveFileName;

                _writer = QDataWriter.Create( Path.Combine( SaveDirectory, _saveFileName ), _sequence );

                return true;
            }
            catch ( Exception ex )
            {
                Owner.Log( $"Ready exception : {ex.Message}", _address );
                return false;
            }
        }

        /// <summary>
        /// 채널의 패킷 처리 스레드를 시작합니다.
        /// <br><br>이 메서드는 측정을 시작하는 메서드가 아니며, <see cref="Communicator"/>의 관리 아래 호출되어야 합니다. 외부에서 임의로 호출하지 마십시오.</br></br>
        /// </summary>
        internal void Start()
        {
            new Thread( channelLoop ) { Name = $"CH[{Owner.Index}][{GlobalIndex}]", IsBackground = true }.Start();
            return;
        }
        /// <summary>
        /// 채널의 패킷 처리 스레드를 종료합니다.
        /// <br><br>이 메서드는 측정을 중단하는 메서드가 아니며, <see cref="Communicator"/>의 관리 아래 호출되어야 합니다. 외부에서 임의로 호출하지 마십시오.</br></br>
        /// </summary>
        internal void Stop()
        {
            _channelRun = false;
            State = State.DISCONNECTED;
        }
        /// <summary>
        /// 채널의 상태 메시지를 설정합니다.
        /// </summary>
        /// <param name="message">상태 메시지로 지정할 문자열입니다.</param>
        internal void SetMessage( string message )
        {
            Message = message;
        }
        /// <summary>
        /// 현재 채널로 수신된 패킷을 패킷 처리 큐의 후행에 삽입합니다.
        /// </summary>
        /// <param name="subPacket">현재 채널이 처리해야할 패킷입니다.</param>
        internal void Push( SubPacket subPacket )
        {
            _channelReceiveQueue.Enqueue( subPacket );

            if ( !_isChannelRun ) Start();
        }
        private ConcurrentQueue<SubPacket> _channelReceiveQueue = new ConcurrentQueue<SubPacket>();
        #endregion
        public void Wait(int milliseconds = 1)
        {
            int Timing = Environment.TickCount + milliseconds;
            while (Timing > Environment.TickCount)
            {
                Application.DoEvents();
                Thread.Sleep(1);
            }
        }
        #region Data Processing Functions & Threads
        /// <summary>
        /// Receive Queue에 들어온 SubPacket을 처리하는 스레드
        /// </summary>
        private void channelLoop()
        {
            if ( _isChannelRun ) return;
            _channelRun = true;

            Owner.Log( "Ch trd start.", _address );

            var lastReceivedTime = DateTime.Now;

            while ( _channelRun )
            {
                _isChannelRun = true;
                //Thread.Sleep(10);
                Application.DoEvents();
                Wait(10);
                if ( _channelReceiveQueue.Count != 0 )
                {
                    if ( !_channelReceiveQueue.TryDequeue( out SubPacket packet ) || packet == null ) continue;

                    // CMD에 따라 처리한다.
                    lastReceivedTime = DateTime.Now;

                    ChannelData data = null;

                    switch ( packet.Command )
                    {
                        case Commands.BatteryCycler_GetMeasureCommands.ChannelSequenceData_R:
                            data = new ChannelSequenceData( packet.DATA );
                            break;

                        case Commands.BatteryCycler_GetMeasureCommands.FastMeasureSequenceData_R:
                            data = new ChannelMeasureData( packet.DATA );
                            break;

                        case Commands.BatteryCycler_GetMeasureCommands.PatternSequenceData_R:
                            data = new ChannelPatternData( packet.DATA );
                            break;

                        case Commands.BatteryCycler_GetMeasureCommands.ChannelState_G:
                            var stateData = new ChannelStateData( packet.DATA );
                             
                            switch ( stateData.ChannelState )
                            {
                                case ChannelState.Idle:
                                    State = State.IDLE;
                                    break;

                                case ChannelState.Run:
                                    State = State.RUN;
                                    break;

                                case ChannelState.Pause:
                                case ChannelState.Pausing:
                                    State = State.PAUSED;
                                    applyStoppedType( stateData.StoppedType );
                                    break;

                                case ChannelState.NotInsert:
                                    State = State.DISCONNECTED;
                                    break;
                            }
                            break;
                    }
                    if (Lock)
                    {
                        if (data.ChannelState == ChannelState.Idle)
                        {
                            Lock = false;
                        }
                        else
                        {
                            //var test = 0;
                        }
                    }


                    if ( !Lock && data != null)
                    {
                        processData( data );
                    }

                }
                else
                {
                    // 30초 동안 패킷이 들어오지 않은 채널은 수신 스레드 종료( 새로운 패킷이 Push될 경우 다시 시작됨 )
                    //if ( ( DateTime.Now - lastReceivedTime ).TotalSeconds > 30 )
                    //{
                    //    Owner.Log( "No rcv data. Ch trd go to sleep.", _address );
                    //    _channelRun = false;
                    //}
                }
            }

            _isChannelRun = false;
            _channelRun = false;

            Owner.Log( "Ch trd stopped.", _address );

#if CONSOLEOUT
            Console.WriteLine( $"Ch[{_address.ADDR}, {_address.CH}] trd stopped." );
#endif
        }

        /// <summary>
        /// 채널 정보를 채널 시퀀스 데이터의 가장 마지막 데이터로 갱신
        /// </summary>
        /// <param name="data"></param>
        private bool updateData( MeasureData data )
        {
            StepNo = data.StepNumber;
            StepCount = data.StepCount;

            if (data.DataIndex == 0) return false;
            // 상시 갱신 측정 데이터
            TotalTime = data.TotalTime;
            Voltage = data.Voltage;
            Current = data.Current;
            Temperature = data.Temperature;

            // 특정 레시피의 경우에만 갱신하는 측정 데이터
            if ( data.RecipeType == RecipeType.Charge ||
                 data.RecipeType == RecipeType.Discharge ||
                 data.RecipeType == RecipeType.AnodeCharge ||
                 data.RecipeType == RecipeType.AnodeDischarge )
            {
                Power = data.Power;
                Capacity = data.Capacity;
            }
            else
            {
                Power = 0;
                Capacity = 0;
            }

            RecipeName = Util.ConvertModeToRecipeType( data.Mode1, data.Mode2 ).ToString();
            return true;
        }
        /// <summary>
        /// 채널 시퀀스 데이터 내용 전체를 파일에 쓰기
        /// </summary>
        /// <param name="data"></param>
        private void writeData( ChannelData data )
        {
            try
            {
                if ( _writer == null )
                {
                    _writer = QDataWriter.Open( Path.Combine( SaveDirectory, _saveFileName ) );
                }
                if ( _writer != null && data.Mode2 != Mode2.MONITOR )
                {
                    for ( var i = 0; i < data.Count; i++ )
                    {
                        if ( data[i].DataIndex == 0 ) continue;

                        _writer.Write( data[i] );

                        //if ( UseBuffer )
                        //{
                            //DataBuffer.Add( new DcData( data[i].TotalTime, data[i].Voltage, data[i].Current ) );
                        //}
                    }
                }

                if ( !string.IsNullOrWhiteSpace( ExportFilePath ) )
                {
                    using ( var sw = new StreamWriter( ExportFilePath, true ) )
                    {
                        sw.WriteLine( $"{VoltageUnit.ChangeValue( data[data.Count - 1].Voltage )};" +
                                      $"{CurrentUnit.ChangeValue( data[data.Count - 1].Current )}" );
                        sw.Flush();
                        sw.Close();
                    }
                }
         
            }
            catch ( Exception ex )
            {
#if CONSOLEOUT
                Console.WriteLine( $"Channel[{Owner.Index}][{GlobalIndex}] write error : {ex.Message}" );
#endif
            }
        }

        private void applyStoppedType( StoppedType s )
        {
            if ( s != StoppedType.Run )
            {
                // 채널 상태 갱신
                var stoppedType = ( byte )s;

                // StoppedType.UserStop(128)
                // 사용자 요청에 의한 종료
                if ( stoppedType == 128 )
                {
                }
                // StoppedType.EndVoltage(0) ~ StoppedType.EndMaxPercent(11)
                // 종료 조건에 의한 종료
                // 이 부분은 그냥 레시피 1개가 종료 조건에 의해 정상 종료됨을 의미함. 따라서 상태를 변경할 필요는 없고 그냥 확인만 한다.
                else if ( 0 <= stoppedType && stoppedType <= 11 )
                {
                }
                // StoppedType.SafetyMaxVoltage ~ SoppedType.SafetyMinTemperature(19)
                // 안전 조건에 의한 종료
                // 안전 조건에 걸렸을 경우 더이상 측정은 진행되지 않는다.
                else if ( 12 <= stoppedType && stoppedType <= 19 )
                {
                    State = State.SAFETY;
                    Message = s.ToString();
                }
            }
        }
        /// <summary>
        /// 데이터 처리 (update와 writeData)
        /// </summary>
        /// <param name="data"></param>
        private void processData( ChannelData data )
        {
            // 채널 대표 데이터를 가장 마지막 데이터로 갱신 (공통)
            if (updateData(data[data.Count - 1])) {

                // 데이터 쓰기 (공통) ,
                writeData(data);

                // 채널 상태 갱신
                // 패킷의 채널 상태 필드 값이 Run이 아닌 경우에만 종료된 조건을 읽으면 된다.
                if (data.ChannelState != ChannelState.Run)
                {
                    for (var i = 0; i < data.Count; i++)
                    {
                        if (data[i].RecipeType == RecipeType.End)
                        {
                            Init();
                            State = State.IDLE;
                            //Owner.Log($"Measuring end. ({data[i].StoppedType})", _address);
                            break;
                        }
                        var stoppedType = (byte)data[i].StoppedType;
                        // StoppedType.UserStop(128)
                        // 사용자 요청에 의한 종료
                        if (stoppedType == 128)
                        {
                            if (StepNo != 0 && data[i].DataIndex != 0)
                            {
                                Init();
                                State = State.IDLE;
                                //Owner.Log($"Measuring stopped. ({data[i].StoppedType})", _address);
                                break;
                            }
                        }
                        // StoppedType.EndVoltage(0) ~ StoppedType.EndMaxPercent(11)
                        // 종료 조건에 의한 종료
                        // 이 부분은 그냥 레시피 1개가 종료 조건에 의해 정상 종료됨을 의미함. 따라서 상태를 변경할 필요는 없고 그냥 확인만 한다.
                        else if (0 <= stoppedType && stoppedType <= 11)
                        {
                            if (data.ChannelState == ChannelState.Idle)
                            {
                                Init();
                                State = State.IDLE;
                                //Owner.Log($"Measuring end. ({data[i].StoppedType})", _address);
                                break;
                            }
                        }
                        // StoppedType.SafetyMaxVoltage ~ SoppedType.SafetyMinTemperature(19)
                        // 안전 조건에 의한 종료
                        // 안전 조건에 걸렸을 경우 더이상 측정은 진행되지 않는다.
                        else if (12 <= stoppedType && stoppedType <= 19)
                        {
                            if (State != State.SAFETY)
                            {
                                State = State.SAFETY;
                                Message = data[i].StoppedType.ToString();
                                //Owner.Log($"Measuring stopped. ({data[i].StoppedType})", _address);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    // Run인 경우에는 그냥 Run으로 설정 (Appending, Stopping이 아닌 경우에만)
                    if (State != State.APPENDING)
                    {
                        State = State.RUN;
                        SetMessage("");
                    }
                }
            }

        }
        #endregion

        #region INotifiyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }
        #endregion
    }
}
