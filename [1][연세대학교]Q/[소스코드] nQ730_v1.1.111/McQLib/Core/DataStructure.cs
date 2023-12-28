using McQLib.Recipes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace McQLib.Core
{
    #region DataSet
    internal struct CalValue
    {
        public double Slope;
        public double Offset;
    }
    internal struct ChannelCalValues
    {
        internal CalValue[] CurrentInput;
        internal CalValue[] CurrentOutput;

        internal CalValue VoltageInput;
        internal CalValue VoltageOutput;

        internal ChannelCalValues( int currentRanges )
        {
            CurrentInput = new CalValue[currentRanges];
            CurrentOutput = new CalValue[currentRanges];

            for (var i = 0; i < currentRanges; i++ )
            {
                CurrentInput[i] = new CalValue();
                CurrentOutput[i] = new CalValue();
            }

            VoltageInput = new CalValue();
            VoltageOutput = new CalValue();
        }
    }
    #endregion

    #region Byte Structures
    /// <summary>
    /// Q통합 프로토콜 통신에 이용되는 바이트 순서(Big Endian)로 변경하기 위한 16비트 부호 없는 정수(<see langword="ushort"/>) 자료형 구조체입니다.
    /// <br>혼동을 막기 위해 Offset으로의 직접 대입은 불가하며 생성자를 이용하여 패킷의 데이터를 패킷 상 바이트 오더 그대로 인수로 전달하십시오.</br>
    /// <br>Offset으로의 직접 대입을 사용하려면 안전하지 않은 형식 <see cref="Q_UInt16_Unsafe"/>을 사용하십시오.</br>
    /// </summary>
    [StructLayout( LayoutKind.Explicit )]
    public struct Q_UInt16
    {
        [FieldOffset( 0 )] private ushort _value;
        [FieldOffset( 0 )] private byte _offset1;
        [FieldOffset( 1 )] private byte _offset0;

        public ushort Value => _value;
        public byte Offset0 => _offset0;
        public byte Offset1 => _offset1;

        public Q_UInt16( ushort value )
        {
            _offset0 = 0;
            _offset1 = 0;
            _value = value;
        }
        public Q_UInt16( byte packetOrder0, byte packetOrder1 )
        {
            _value = 0;
            _offset0 = packetOrder0;
            _offset1 = packetOrder1;
        }
        public static implicit operator ushort( Q_UInt16 q_uint16 ) => q_uint16._value;
    }

    /// <summary>
    /// Q통합 프로토콜 통신에 이용되는 바이트 순서(Big Endian)로 변경하기 위한 32비트 부호 없는 정수(<see langword="uint"/>) 자료형 구조체입니다.
    /// <br>혼동을 막기 위해 Offset으로의 직접 대입은 불가하며 생성자를 이용하여 패킷의 데이터를 패킷 상 바이트 오더 그대로 인수로 전달하십시오.</br>
    /// </summary>
    [StructLayout( LayoutKind.Explicit )]
    public struct Q_UInt32
    {
        [FieldOffset( 0 )] private uint _value;
        [FieldOffset( 0 )] private byte _offset3;
        [FieldOffset( 1 )] private byte _offset2;
        [FieldOffset( 2 )] private byte _offset1;
        [FieldOffset( 3 )] private byte _offset0;

        public uint Value => _value;
        public byte Offset0 => _offset0;
        public byte Offset1 => _offset1;
        public byte Offset2 => _offset2;
        public byte Offset3 => _offset3;

        public Q_UInt32( uint value )
        {
            _offset0 = 0;
            _offset1 = 0;
            _offset2 = 0;
            _offset3 = 0;
            _value = value;
        }
        public Q_UInt32( byte packetOrder0, byte packetOrder1, byte packetOrder2, byte packetOrder3 )
        {
            _value = 0;
            _offset0 = packetOrder0;
            _offset1 = packetOrder1;
            _offset2 = packetOrder2;
            _offset3 = packetOrder3;
        }
        public static implicit operator uint( Q_UInt32 q_uint32 ) => q_uint32._value;
    }

    /// <summary>
    /// Q통합 프로토콜 통신에 이용되는 바이트 순서(Big Endian)로 변경하기 위한 단정밀도 부동 소수점(<see langword="float"/>) 자료형 구조체입니다.
    /// <br>혼동을 막기 위해 Offset으로의 직접 대입은 불가하며 생성자를 이용하여 패킷의 데이터를 패킷 상 바이트 오더 그대로 인수로 전달하십시오.</br>
    /// </summary>
    [StructLayout( LayoutKind.Explicit )]
    public struct Q_Float
    {
        [FieldOffset( 0 )] public float _value;
        [FieldOffset( 0 )] private byte _offset3;
        [FieldOffset( 1 )] private byte _offset2;
        [FieldOffset( 2 )] private byte _offset1;
        [FieldOffset( 3 )] private byte _offset0;

        public float Value => _value;
        public byte Offset0 => _offset0;
        public byte Offset1 => _offset1;
        public byte Offset2 => _offset2;
        public byte Offset3 => _offset3;

        public Q_Float( float value )
        {
            _offset0 = 0;
            _offset1 = 0;
            _offset2 = 0;
            _offset3 = 0;
            _value = value;
        }
        public Q_Float( byte packetOrder0, byte packetOrder1, byte packetOrder2, byte packetOrder3 )
        {
            _value = 0;
            _offset0 = packetOrder0;
            _offset1 = packetOrder1;
            _offset2 = packetOrder2;
            _offset3 = packetOrder3;
        }
        public static implicit operator float( Q_Float q_float ) => q_float._value;
    }

    /// <summary>
    /// Q통합 프로토콜 통신에 이용되는 바이트 순서(Big Endian)로 변경하기 위한 배정밀도 부동 소수점(<see langword="double"/>) 자료형 구조체입니다.
    /// <br>혼동을 막기 위해 Offset으로의 직접 대입은 불가하며 생성자를 이용하여 패킷의 데이터를 패킷 상 바이트 오더 그대로 인수로 전달하십시오.</br>
    /// </summary>
    [StructLayout( LayoutKind.Explicit )]
    public struct Q_Double
    {
        [FieldOffset( 0 )] public double _value;
        [FieldOffset( 0 )] private byte _offset7;
        [FieldOffset( 1 )] private byte _offset6;
        [FieldOffset( 2 )] private byte _offset5;
        [FieldOffset( 3 )] private byte _offset4;
        [FieldOffset( 4 )] private byte _offset3;
        [FieldOffset( 5 )] private byte _offset2;
        [FieldOffset( 6 )] private byte _offset1;
        [FieldOffset( 7 )] private byte _offset0;

        public double Value => _value;
        public byte Offset0 => _offset0;
        public byte Offset1 => _offset1;
        public byte Offset2 => _offset2;
        public byte Offset3 => _offset3;
        public byte Offset4 => _offset4;
        public byte Offset5 => _offset5;
        public byte Offset6 => _offset6;
        public byte Offset7 => _offset7;

        public Q_Double( double value )
        {
            _offset0 = 0;
            _offset1 = 0;
            _offset2 = 0;
            _offset3 = 0;
            _offset4 = 0;
            _offset5 = 0;
            _offset6 = 0;
            _offset7 = 0;
            _value = value;
        }
        public Q_Double( byte packetOrder0, byte packetOrder1, byte packetOrder2, byte packetOrder3, byte packetOrder4, byte packetOrder5, byte packetOrder6, byte packetOrder7 )
        {
            _value = 0;
            _offset0 = packetOrder0;
            _offset1 = packetOrder1;
            _offset2 = packetOrder2;
            _offset3 = packetOrder3;
            _offset4 = packetOrder4;
            _offset5 = packetOrder5;
            _offset6 = packetOrder6;
            _offset7 = packetOrder7;
        }
        public static implicit operator double( Q_Double q_double ) => q_double._value;
    }

    /// <summary>
    /// Q통합 프로토콜 통신에 이용되는 바이트 순서(Big Endian)로 변경하기 위한 16비트 부호 없는 정수(<see langword="ushort"/>) 자료형 구조체입니다.
    /// <br>안전하지 않은 형식으로, Offset에 바이트를 직접 대입할 수 있으나, 시스템 상 바이트 순서(Little Endian)로 저장됩니다.</br>
    /// </summary>
    [StructLayout( LayoutKind.Explicit )]
    public struct Q_UInt16_Unsafe
    {
        [FieldOffset( 0 )] public ushort Value;
        [FieldOffset( 0 )] public byte Offset1;
        [FieldOffset( 1 )] public byte Offset0;

        public Q_UInt16_Unsafe( ushort value )
        {
            Offset0 = 0;
            Offset1 = 0;
            Value = value;
        }
        public Q_UInt16_Unsafe( byte packetOrder0, byte packetOrder1 )
        {
            Value = 0;
            Offset1 = packetOrder1;
            Offset0 = packetOrder0;
        }
        public static implicit operator ushort( Q_UInt16_Unsafe q_uint16 ) => q_uint16.Value;
    }

    /// <summary>
    /// Q통합 프로토콜 통신에 이용되는 바이트 순서(Big Endian)로 변경하기 위한 32비트 부호 없는 정수(<see langword="uint"/>) 자료형 구조체입니다.
    /// <br>안전하지 않은 형식으로, Offset에 바이트를 직접 대입할 수 있으나, 시스템 상 바이트 순서(Little Endian)로 저장됩니다.</br>
    /// </summary>
    [StructLayout( LayoutKind.Explicit )]
    public struct Q_UInt32_Unsafe
    {
        [FieldOffset( 0 )] public uint Value;
        [FieldOffset( 0 )] public byte Offset3;
        [FieldOffset( 1 )] public byte Offset2;
        [FieldOffset( 2 )] public byte Offset1;
        [FieldOffset( 3 )] public byte Offset0;

        public Q_UInt32_Unsafe( uint value )
        {
            Offset0 = 0;
            Offset1 = 0;
            Offset2 = 0;
            Offset3 = 0;
            Value = value;
        }
        public Q_UInt32_Unsafe( byte packetOrder0, byte packetOrder1, byte packetOrder2, byte packetOrder3 )
        {
            Value = 0;
            Offset0 = packetOrder0;
            Offset1 = packetOrder1;
            Offset2 = packetOrder2;
            Offset3 = packetOrder3;
        }
        public static implicit operator uint( Q_UInt32_Unsafe q_uint32 ) => q_uint32.Value;
    }

    /// <summary>
    /// Q통합 프로토콜 통신에 이용되는 바이트 순서(Big Endian)로 변경하기 위한 단정밀도 부동 소수점(<see langword="float"/>) 자료형 구조체입니다.
    /// <br>안전하지 않은 형식으로, Offset에 바이트를 직접 대입할 수 있으나, 시스템 상 바이트 순서(Little Endian)로 저장됩니다.</br>
    /// </summary>
    [StructLayout( LayoutKind.Explicit )]
    public struct Q_Float_Unsafe
    {
        [FieldOffset( 0 )] public float Value;
        [FieldOffset( 0 )] public byte Offset3;
        [FieldOffset( 1 )] public byte Offset2;
        [FieldOffset( 2 )] public byte Offset1;
        [FieldOffset( 3 )] public byte Offset0;

        public Q_Float_Unsafe( float value )
        {
            Offset0 = 0;
            Offset1 = 0;
            Offset2 = 0;
            Offset3 = 0;
            Value = value;
        }
        public Q_Float_Unsafe( byte packetOrder0, byte packetOrder1, byte packetOrder2, byte packetOrder3 )
        {
            Value = 0;
            Offset0 = packetOrder0;
            Offset1 = packetOrder1;
            Offset2 = packetOrder2;
            Offset3 = packetOrder3;
        }
        public static implicit operator float( Q_Float_Unsafe q_float ) => q_float.Value;
    }

    /// <summary>
    /// Q통합 프로토콜 통신에 이용되는 바이트 순서(Big Endian)로 변경하기 위한 배정밀도 부동 소수점(<see langword="double"/>) 자료형 구조체입니다.
    /// <br>안전하지 않은 형식으로, Offset에 바이트를 직접 대입할 수 있으나, 시스템 상 바이트 순서(Little Endian)로 저장됩니다.</br>
    /// </summary>
    [StructLayout( LayoutKind.Explicit )]
    public struct Q_Double_Unsafe
    {
        [FieldOffset( 0 )] public double Value;
        [FieldOffset( 0 )] public byte Offset7;
        [FieldOffset( 1 )] public byte Offset6;
        [FieldOffset( 2 )] public byte Offset5;
        [FieldOffset( 3 )] public byte Offset4;
        [FieldOffset( 4 )] public byte Offset3;
        [FieldOffset( 5 )] public byte Offset2;
        [FieldOffset( 6 )] public byte Offset1;
        [FieldOffset( 7 )] public byte Offset0;

        public Q_Double_Unsafe( double value )
        {
            Offset0 = 0;
            Offset1 = 0;
            Offset2 = 0;
            Offset3 = 0;
            Offset4 = 0;
            Offset5 = 0;
            Offset6 = 0;
            Offset7 = 0;
            Value = value;
        }
        public Q_Double_Unsafe( byte packetOrder0, byte packetOrder1, byte packetOrder2, byte packetOrder3, byte packetOrder4, byte packetOrder5, byte packetOrder6, byte packetOrder7 )
        {
            Value = 0;
            Offset0 = packetOrder0;
            Offset1 = packetOrder1;
            Offset2 = packetOrder2;
            Offset3 = packetOrder3;
            Offset4 = packetOrder4;
            Offset5 = packetOrder5;
            Offset6 = packetOrder6;
            Offset7 = packetOrder7;
        }
        public static implicit operator double( Q_Double_Unsafe q_double ) => q_double.Value;
    }
    #endregion

    #region DataBuilder
    /// <summary>
    /// 여러 형식의 값들을 바이트 형식으로 누적하여 관리하는 작업을 돕는 클래스입니다.
    /// </summary>
    public class DataBuilder
    {
        private List<byte> _list;

        /// <summary>
        /// 데이터 빌더의 지정된 인덱스 위치에서 바이트값을 가져옵니다.
        /// </summary>
        /// <param name="index">바이트를 가져올 인덱스입니다.</param>
        /// <returns>지정된 위치의 바이트값입니다.</returns>
        public byte this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list.RemoveAt( index );
                _list.Insert( index, value );
            }
        }
        /// <summary>
        /// 데이터 빌더에 포함된 바이트의 개수입니다.
        /// </summary>
        public int Count => _list.Count;
        /// <summary>
        /// 지정된 바이트 배열을 데이터 빌더의 끝에 추가합니다.
        /// </summary>
        /// <param name="values">추가할 값입니다.</param>
        public void Add( params byte[] values )
        {
            _list.AddRange( values );
        }
        /// <summary>
        /// 지정된 열거형 값에 해당하는 정수값을 데이터 빌더의 끝에 추가합니다.
        /// </summary>
        /// <param name="value">추가할 값입니다.</param>
        public void Add( Enum value )
        {
            var v = value.ToValue();

            var t = v.GetType();
            if ( t == typeof( byte ) ) _list.Add( ( byte )v );
            else if ( t == typeof( short ) || t == typeof( ushort ) ) Add( new Q_UInt16( ( ushort )v ) );
            else if ( t == typeof( int ) || t == typeof( uint ) ) Add( new Q_UInt32( ( uint )v ) );
            else throw new QException( QExceptionType.DEVELOP_ENUM_FORMAT_NOT_SUPPORTED_ERROR );
        }
        /// <summary>
        /// 지정된 값을 데이터 빌더의 끝에 추가합니다.
        /// </summary>
        /// <param name="value">추가할 값입니다.</param>
        public void Add( uint? value )
        {
            Add( value == null ? new Q_UInt32( 0 ) : new Q_UInt32( value.Value ) );
        }
        /// <summary>
        /// 지정된 값을 데이터 빌더의 끝에 추가합니다.
        /// </summary>
        /// <param name="value">추가할 값입니다.</param>
        public void Add( double? value )
        {
            Add( value == null ? new Q_Double( 0 ) : new Q_Double( value.Value ) );
        }
        /// <summary>
        /// 지정된 값을 데이터 빌더의 끝에 추가합니다.
        /// </summary>
        /// <param name="value">추가할 값입니다.</param>
        public void Add( float? value )
        {
            Add( value == null ? new Q_Float( 0 ) : new Q_Float( value.Value ) );
        }
        /// <summary>
        /// 지정된 값을 데이터 빌더의 끝에 추가합니다.
        /// </summary>
        /// <param name="value">추가할 값입니다.</param>
        public void Add( bool value )
        {
            Add( ( byte )( value ? 1 : 0 ) );
        }

        /// <summary>
        /// 지정된 값을 데이터 빌더의 끝에 추가합니다.
        /// </summary>
        /// <param name="value">추가할 값입니다.</param>
        public void Add( Q_UInt16 value )
        {
            _list.Add( value.Offset0 );
            _list.Add( value.Offset1 );
        }
        /// <summary>
        /// 지정된 값을 데이터 빌더의 끝에 추가합니다.
        /// </summary>
        /// <param name="value">추가할 값입니다.</param>
        public void Add( Q_UInt32 value )
        {
            _list.Add( value.Offset0 );
            _list.Add( value.Offset1 );
            _list.Add( value.Offset2 );
            _list.Add( value.Offset3 );
        }
        /// <summary>
        /// 지정된 값을 데이터 빌더의 끝에 추가합니다.
        /// </summary>
        /// <param name="value">추가할 값입니다.</param>
        public void Add( Q_Float value )
        {
            _list.Add( value.Offset0 );
            _list.Add( value.Offset1 );
            _list.Add( value.Offset2 );
            _list.Add( value.Offset3 );
        }
        /// <summary>
        /// 지정된 값을 데이터 빌더의 끝에 추가합니다.
        /// </summary>
        /// <param name="value">추가할 값입니다.</param>
        public void Add( Q_Double value )
        {
            _list.Add( value.Offset0 );
            _list.Add( value.Offset1 );
            _list.Add( value.Offset2 );
            _list.Add( value.Offset3 );
            _list.Add( value.Offset4 );
            _list.Add( value.Offset5 );
            _list.Add( value.Offset6 );
            _list.Add( value.Offset7 );
        }
        /// <summary>
        /// 동일한 값 여러 개를 연속하여 리스트 뒤에 추가합니다.
        /// </summary>
        /// <param name="value">추가할 값입니다.</param>
        /// <param name="count">추가할 개수입니다.</param>
        public void AddCount( byte value, int count )
        {
            for ( var i = 0; i < count; i++ ) _list.Add( value );
        }

        /// <summary>
        /// 빈 데이터 빌더 인스턴스를 생성합니다.
        /// </summary>
        public DataBuilder()
        {
            _list = new List<byte>();
        }
        /// <summary>
        /// 데이터 빌더의 요소를 새 배열에 복사하여 반환합니다.
        /// </summary>
        /// <returns>복사된 배열입니다.</returns>
        public byte[] ToByteArray() => _list.ToArray();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        public static implicit operator byte[]( DataBuilder d ) => d._list.ToArray();
    }
    #endregion

    #region Packet Field Enums
    /// <summary>
    /// Communicator에서 처리하는 직접 통신 메서드의 수행 결과를 나타내는 열거형입니다.
    /// </summary>
    public enum Result
    {
        NoError = 0x00,

        Com_ActionError = 0x01,
        Com_InitError = 0x02,
        Com_PacketError = 0x03,
        Com_CmdError = 0x04,
        Com_BusyError = 0x05,
        Com_SetError = 0x06,
        Com_SdError = 0x07,
        Com_AddressError = 0x08,
        Com_NotDefinedError = 0xFF,

        /// <summary>
        /// 응답 없음
        /// </summary>
        NoResponse = 0x80,
        /// <summary>
        /// 유효하지 않은 시퀀스
        /// </summary>
        InvalidSequence = 0x81,
        RegClearFail = 0x82,
        TryCount = 0x83,
        /// <summary>
        /// 소프트웨어에서 수신한 패킷의 프레임 오류
        /// </summary>
        Sw_PacketError = 0x84
    }
    public enum ErrorField : byte
    {
        /// <summary>
        /// 명령 정상 수행
        /// </summary>
        Complete = 0x00,

        /// <summary>
        /// <b>[동작 오류]</b>
        /// 설정한 에러가 동작 중 발생한 경우
        /// </summary>
        ActionError = 0x01,
        /// <summary>
        /// <b>[장비 초기화 오류]</b>
        /// 장비 초기화 수행 오류
        /// </summary>
        InitializeError = 0x02,
        /// <summary>
        /// <b>[수신 프레임 오류]</b>
        /// 수신된 데이터의 프레임 오류/CRC 오류
        /// </summary>
        PacketError = 0x03,
        /// <summary>
        /// <b>[수신 명령 오류]</b>
        /// 수신된 명령이 존재하지 않음
        /// </summary>
        CommandError = 0x04,
        /// <summary>
        /// <b>[명령 수행 중 오류]</b>
        /// 이전에 받은 명령을 수행 중인 경우
        /// </summary>
        OnBusyError = 0x05,
        /// <summary>
        /// <b>[명령 설정 오류]</b>
        /// 설정 동작 명령에 실패한 경우
        /// </summary>
        SetError = 0x06,
        /// <summary>
        /// <b>[SD 오류]</b>
        /// SD card의 공간 부족 등 오류가 검출되었을 경우
        /// </summary>
        SdCardError = 0x07,

        /// <summary>
        /// <b>[주소 오류]</b>
        /// 수신 주소 오류
        /// </summary>
        AddressError = 0xF0,

        /// <summary>
        /// <b>[정의되지 않은 오류]</b>
        /// 정의되지 않은 오류가 발생한 경우
        /// </summary>
        UndefinedError = 0xFF
    }
    [Flags]
    internal enum RegisterError0 : byte
    {
        NoError = 0,
        RecipeSendingFail = 0b00000010
    }
    [Flags]
    internal enum RegisterError1 : byte
    {
        NoError = 0,
        BatterySafeAlarm = 0b10000000,
        SdFileMeet4GB = 0b00010000,
        SdFreeSpaceTooSmall = 0b00001000,
        SdReadWriteFail = 0b00000100,
        SdInitializeFail = 0b00000010,
        InitializeFail = 0b00000001
    }

    [Flags]
    internal enum RegisterError : ushort
    {
        NoError = 0b0000000000000000,
        RecipeSendingFail = 0b0000001000000000,     // Master

        BatterySafeAlarm = 0b0000000010000000,      // Slave

        SdFileMeet4GB = 0b0000000000010000,         // Master
        SdFreeSpaceTooSmall = 0b0000000000001000,   // Master
        SdReadWriteFail = 0b0000000000000100,       // Master
        SdInitialFail = 0b0000000000000010,         // Master
        InitialFail = 0b0000000000000001            // Master, Slave
    }

    /// <summary>
    /// 시퀀스 관련 패킷의 Mode1 필드에 사용되는 값의 열거형입니다.
    /// </summary>
    public enum Mode1 : byte
    {
        Rest = 0,
        Charge = 1,
        Discharge = 2,
        Measure = 3,
        Pattern = 4,
        Cycle = 5,
        Loop = 6,
        Jump = 7,
        End = 8,
        AnodeCharge = 81,
        AnodeDischarge = 82
    }
    /// <summary>
    /// 시퀀스 관련 패킷의 Mode2 필드에 사용되는 값의 열거형입니다.
    /// </summary>
    public enum Mode2 : byte
    {
        Set = 0,
        CC = 1,
        CCCV = 2,
        CP = 3,
        CR = 4,
        PC = 5,
        TRA = 6,
        OCV = 7,
        DCR = 8,
        ACR = 9,
        FRA = 10,
        OCVC = 11,
        CVACR = 15,
        CVFRA = 16,
        MONITOR = 255,
    }
    /// <summary>
    /// 끝내는 방식입니다.
    /// </summary>
    public enum EndCondition_Type : byte
    {
        /// <summary>
        /// Time 모드입니다.
        /// </summary>
        시간,
        /// <summary>
        /// Voltage 모드입니다.
        /// </summary>
        전압,
        /// <summary>
        /// Current 모드입니다.
        /// </summary>
        전류,
    }
    /// <summary>
    /// 저장 방식입니다.
    /// </summary>
    public enum SaveCondition_Type : byte
    {
        /// <summary>
        /// Time 모드입니다.
        /// </summary>
        시간,
        /// <summary>
        /// Voltage 모드입니다.
        /// </summary>
        전압,
        /// <summary>
        /// Current 모드입니다.
        /// </summary>
        전류,
    }
    /// <summary>
    /// 충전 레시피의 충전 방식입니다.
    /// </summary>
    public enum SourcingType_Charge : byte
    {
        /// <summary>
        /// CC모드 입니다.
        /// </summary>
        CC,
        /// <summary>
        /// CC-CV모드입니다.
        /// </summary>
        CCCV,
        /// <summary>
        /// CP모드입니다.
        /// </summary>
        CP,
        /// <summary>
        /// CR모드입니다.
        /// </summary>
        CR
    }
    /// <summary>
    /// 방전 레시피의 충전 방식입니다.
    /// </summary>
    public enum SourcingType_Discharge : byte
    {
        /// <summary>
        /// CC모드입니다.
        /// </summary>
        CC,
        /// <summary>
        /// CP모드입니다.
        /// </summary>
        CP,
        /// <summary>
        /// CR모드입니다.
        /// </summary>
        CR,
    }
    /// <summary>
    /// 음극재 하프셀 레시피의 충방전 방식입니다.
    /// </summary>
    public enum SourcingType_Anode : byte
    {
        /// <summary>
        /// CC모드입니다.
        /// </summary>
        CC,
        /// <summary>
        /// CC-CV모드입니다.
        /// </summary>
        CCCV,
    }
    public enum SourcingType_CurrentUnit : byte
    {
        μA,
        mA,
        A,
    }
    /// <summary>
    /// TR 모드입니다.
    /// </summary>
    public enum TrStepMode : byte
    {
        /// <summary>
        /// TR을 사용하지 않습니다.
        /// </summary>
        None,
        /// <summary>
        /// 측정 전에 TR를 측정합니다.
        /// </summary>
        Before,
        /// <summary>
        /// 측정 후에 TR을 측정합니다.
        /// </summary>
        After,
        /// <summary>
        /// 측정 전과 후 모두 TR을 측정합니다.
        /// </summary>
        All
    }
    /// <summary>
    /// 증폭 배율을 나타내는 열거형입니다.
    /// </summary>
    public enum AmplifyMode : byte
    {
        x1,
        x100,
        x500
    }
    /// <summary>
    /// 데이터 스케일을 나타내는 열거형입니다.
    /// </summary>
    public enum ScaleMode : byte
    {
        Log,
        Linear
    }
    internal enum StartStopType : byte
    {
        Stop = 0x00,
        StartOrKeep = 0x01
    }
    internal enum ApplyWhen : byte
    {
        Immediately = 0x00,
        AfterStep = 0x01,
        AfterCycle = 0x02,
        NotApply = 0xFF
    }
    internal enum PauseStartType : byte
    {
        StartOrKeep = 0x01,
        Pause = 0x02
    }
    public enum ChannelState : byte
    {
        Idle = 0x00,
        Run = 0x01,
        Pause = 0x02,
        NotInsert = 0x03,
        Pausing = 0x80
    }
    /// <summary>
    /// Recipe가 종료된 원인을 나타내는 열거형입니다.
    /// <br>종료되지 않았다면 <see cref="StoppedType.Run"/>입니다.</br>
    /// <br>이 항목은 채널의 현재 상태를 나타내지 않습니다.</br>
    /// </summary>
    public enum StoppedType : byte
    {
        EndVoltage = 0,
        EndCurrent = 1,
        EndTime = 2,
        EndCvTime = 3,
        EndCapacity = 4,
        EndWatt = 5,
        EndWattHour = 6,
        EndDeltaVoltage = 7,
        EndDeltaCurrent = 8,
        EndDeltaTemperature = 9,
        EndTemperature = 10,
        EndMaxPercent = 11,
        SafetyMaxVoltage = 12,
        SafetyMinVoltage = 13,
        SafetyMaxCurrent = 14,
        SafetyMinCurrent = 15,
        SafetyMaxCapacity = 16,
        SafetyMaxWattHour = 17,
        SafetyMaxTemperature = 18,
        SafetyMinTemperature = 19,
        UserStop = 128,
        Run = 255
    }

    internal enum LimitType : byte
    {
        HighLimit = 0,
        LowLimit = 1
    }
    internal enum LimitModeRange : byte
    {
        CV_Range0 = 0x00,
        CV_Range1 = 0x01,
        CV_Range2 = 0x02,
        CV_Range3 = 0x03,
        CC_Range0 = 0x10
    }
    internal enum CancelChannel : byte
    {
        CancelX1 = 0,
        CancelX0_01 = 1,
        CancelX0_002 = 2
    }
    internal enum AmlifyType : byte
    {
        x100 = 0,
        x500 = 1
    }
    /// <summary>
    /// 패턴 레시피의 각 포인트를 인가하기 위한 모드를 타나내는 열거형입니다.
    /// </summary>
    public enum PatternBiasMode : byte
    {
        CC = 1,
        CP = 3
    }
    /// <summary>
    /// ADDR과 CH 정보를 구성하는 구조체입니다.
    /// </summary>
    public struct Address
    {
        /// <summary>
        /// 마스터 보드를 지정하는 <see cref="Address"/> 인스턴스를 가져옵니다.
        /// </summary>
        public static Address Master => new Address( 0, 0, 0 );

        public int LocalIndex;
        /// <summary>
        /// ADDR (보드 번호)입니다.
        /// </summary>
        public byte ADDR;
        /// <summary>
        /// CH (채널 번호)입니다.
        /// </summary>
        public byte CH;
        /// <summary>
        /// 지정된 ADDR과 CH을 사용하여 새 <see cref="Address"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="localIndex">채널의 지역 인덱스입니다.</param>
        /// <param name="addr">ADDR (보드 번호)입니다.</param>
        /// <param name="ch">CH (채널 번호)입니다.</param>
        public Address(byte addr, byte ch )
        {
            LocalIndex = 0;
            ADDR = addr;
            CH = ch;
        }
        /// <summary>
        /// 지정된 ADDR과 CH을 사용하여 새 <see cref="Address"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="localIndex">채널의 지역 인덱스입니다.</param>
        /// <param name="addr">ADDR (보드 번호)입니다.</param>
        /// <param name="ch">CH (채널 번호)입니다.</param>
        public Address( int localIndex, byte addr, byte ch )
        {
            LocalIndex = localIndex;
            ADDR = addr;
            CH = ch;
        }
        /// <summary>
        /// 지정된 <see cref="Address"/> 구조체의 인스턴스가 나타내는 ADDR과 CH이 마스터 보드를 지정하는지의 여부입니다.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool IsMaster( Address address )
        {
            return address.ADDR == 0 && address.CH == 0;
        }

        public override string ToString()
        {
            return $"ADDR : {ADDR}, CH : {CH}";
        }
        public string ToString( int deviceIndex )
        {
            return $"Device : {deviceIndex}, ADDR : {ADDR}, CH : {CH}";
        }
    }
    #endregion

    #region Packet Data
    /// <summary>
    /// 한 포인트의 데이터 집합을 관리하기 위한 클래스입니다.
    /// </summary>
    public class MeasureData
    {
        /// <summary>
        /// 현재 측정 데이터가 측정된 레시피의 종류입니다.
        /// </summary>
        public readonly RecipeType RecipeType;

        /// <summary>
        /// 현재 측정 데이터가 ACR, TR 등의 고속 측정 레시피의 Raw 데이터를 나타내는 인스턴스인지의 여부입니다.
        /// </summary>
        public bool IsRaw;

        /// <summary>
        /// 전체 측정이 진행된 시간입니다.
        /// </summary>
        public ulong TotalTime => uint.MaxValue * TotalTimeOverflow + _field4;

        /// <summary>
        /// 필드4로부터 TotalTime(ms)을 가져옵니다.
        /// </summary>
        public uint TotalTime_Uint => _field4;

        /// <summary>
        /// 필드4로부터 StartTime(ms)을 가져옵니다.
        /// </summary>
        public uint StartTime => _field4;
        /// <summary>
        /// 필드4로부터 TotalTime(ms)을 가져옵니다.
        /// </summary>
        public uint TotalTime_UInt => _field4;

        /// <summary>
        /// 이 필드는 장비로부터 전달받지 않습니다. 이전 스텝과의 TotalTime 차를 누적하여 채워야 합니다.
        /// <br>단, 고속 측정 데이터와 패턴 측정 데이터의 경우 이 필드를 가져옵니다.</br>
        /// </summary>
        public ulong StepTime;

        /// <summary>
        /// 필드4로부터 StepTime(ms)을 가져옵니다.
        /// </summary>
        public uint StepTime_Uint => _field4;

        /// <summary>
        /// 필드5로부터 StepCount를 가져옵니다.
        /// </summary>
        public uint StepCount => _field5;
        /// <summary>
        /// 필드6으로부터 StepNumber를 가져옵니다.
        /// </summary>
        public ushort StepNumber => _field6;
        /// <summary>
        /// 필드7로부터 CycleCount를 가져옵니다.
        /// </summary>
        public uint CycleCount => _field7;

        /// <summary>
        /// 필드8로부터 Mode1을 가져옵니다.
        /// </summary>
        public Mode1 Mode1 => ( Mode1 )_field8;
        /// <summary>
        /// 필드9로부터 Mode2를 가져옵니다.
        /// </summary>
        public Mode2 Mode2 => ( Mode2 )_field9;

        /// <summary>
        /// 필드10으로부터 DataIndex를 가져옵니다.
        /// </summary>
        public uint DataIndex => _field10;

        /// <summary>
        /// 필드11로부터 Voltage(V)를 가져옵니다.
        /// </summary>
        public double Voltage => _field11;
        /// <summary>
        /// 필드12로부터 Current(A)를 가져옵니다.
        /// </summary>
        public double Current => _field12;

        /// <summary>
        /// 필드13으로부터 Temperature(℃)를 가져옵니다.
        /// </summary>
        public double Temperature => _field13;
        /// <summary>
        /// 필드13으로부터 Frequency(Hz)를 가져옵니다.
        /// </summary>
        public double Frequency => _field13;

        /// <summary>
        /// 필드14로부터 Capacity(Ah)를 가져옵니다.
        /// </summary>
        public double Capacity => _field14;
        /// <summary>
        /// 필드14로부터 Z를 가져옵니다.
        /// </summary>
        public double Z => _field14;

        /// <summary>
        /// 필드15로부터 Power(W)를 가져옵니다.
        /// </summary>
        public double Power => _field15;
        /// <summary>
        /// 필드15로부터 Phase를 가져옵니다.
        /// </summary>
        public double Phase => _field15;
        /// <summary>
        /// 필드15로부터 R을 가져옵니다.
        /// </summary>
        public double R => _field15;

        /// <summary>
        /// 필드16으로부터 WattHour(Wh)를 가져옵니다.
        /// </summary>
        public double WattHour => _field16;
        /// <summary>
        /// 필드16으로부터 Z_Real을 가져옵니다.
        /// </summary>
        public double Z_Real => _field16;
        /// <summary>
        /// 필드16으로부터 V1을 가져옵니다.
        /// </summary>
        public double V1 => _field16;

        /// <summary>
        /// 필드17로부터 DeltaV를 가져옵니다.
        /// </summary>
        public double DeltaV => _field17;
        /// <summary>
        /// 필드17로부터 Z_Img를 가져옵니다.
        /// </summary>
        public double Z_Img => _field17;
        /// <summary>
        /// 필드17로부터 V2를 가져옵니다.
        /// </summary>
        public double V2 => _field17;

        /// <summary>
        /// 필드18로부터 DeltaI를 가져옵니다.
        /// </summary>
        public double DeltaI => _field18;
        /// <summary>
        /// 필드18로부터 StartOcv를 가져옵니다.
        /// </summary>
        public double StartOcv => _field18;
        /// <summary>
        /// 필드18로부터 I1을 가져옵니다.
        /// </summary>
        public double I1 => _field18;

        /// <summary>
        /// 필드19로부터 DeltaT를 가져옵니다.
        /// </summary>
        public double DeltaT => _field19;
        /// <summary>
        /// 필드19로부터 EndOcv를 가져옵니다.
        /// </summary>
        public double EndOcv => _field19;
        /// <summary>
        /// 필드19로부터 I2를 가져옵니다.
        /// </summary>
        public double I2 => _field19;

        /// <summary>
        /// 필드20으로부터 StoppedType을 가져옵니다.
        /// </summary>
        public StoppedType StoppedType => ( StoppedType )_field20;
        /// <summary>
        /// 필드21로부터 TotalTimeOverflow를 가져옵니다.
        /// </summary>
        public byte TotalTimeOverflow => _field21;


        // Field 번호는 통합 프로토콜 문서에서 패킷 상 필드 번호를 인용함.
        ///////////////////////////// 측정 스텝 정보 /////////////////////////////
        /// <summary>
        /// TotalTime 또는 StartTime으로 사용됨.
        /// </summary>
        internal uint _field4;
        /// <summary>
        /// StepCount로 사용됨.
        /// </summary>
        internal uint _field5;
        /// <summary>
        /// StepNumber로 사용됨.
        /// </summary>
        internal ushort _field6;
        /// <summary>
        /// CycleCount로 사용됨.
        /// </summary>
        internal uint _field7;
        /// <summary>
        /// Mode1로 사용됨.
        /// </summary>
        internal byte _field8;
        /// <summary>
        /// Mode2로 사용됨
        /// </summary>
        internal byte _field9;
        /// <summary>
        /// DataIndex로 사용됨.
        /// </summary>
        internal uint _field10;

        ///////////////////////////// 측정 데이터 /////////////////////////////
        /// <summary>
        /// Voltage로 사용됨.
        /// </summary>
        internal double _field11;
        /// <summary>
        /// Current로 사용됨.
        /// </summary>
        internal double _field12;
        /// <summary>
        /// Temperature 또는 Frequency로 사용됨.
        /// </summary>
        internal double _field13;
        /// <summary>
        /// Capacity 또는 Z로 사용됨.
        /// </summary>
        internal double _field14;
        /// <summary>
        /// Power, Phase 또는 R로 사용됨.
        /// </summary>
        internal double _field15;
        /// <summary>
        /// WattHour, Z_Real 또는 V1로 사용됨.
        /// </summary>
        internal double _field16;
        /// <summary>
        /// DeltaV, Z_Img 또는 V2로 사용됨.
        /// </summary>
        internal double _field17;
        /// <summary>
        /// DeltaI, StartOcv 또는 I1로 사용됨.
        /// </summary>
        internal double _field18;
        /// <summary>
        /// DeltaT, EndOcv 또는 I2로 사용됨.
        /// </summary>
        internal double _field19;

        ///////////////////////////// 기타 /////////////////////////////
        /// <summary>
        /// StoppedType으로 사용됨.
        /// </summary>
        internal byte _field20;
        /// <summary>
        /// TotalTimeOverflow로 사용됨.
        /// </summary>
        internal byte _field21;

        internal MeasureData( RecipeType recipeType ) { RecipeType = recipeType; }
        internal MeasureData( byte[] packetDataField, ref int position )
        {
            _field4 = new Q_UInt32( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] );
            _field5 = new Q_UInt32( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] );
            _field6 = new Q_UInt16( packetDataField[position++], packetDataField[position++] );
            _field7 = new Q_UInt32( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] );
            _field8 = packetDataField[position++];
            _field9 = packetDataField[position++];
            _field10 = new Q_UInt32( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] );

            _field11 = new Q_Double( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++],
            packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] );
            _field12 = new Q_Double( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++],
            packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] );
            _field13 = new Q_Double( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++],
            packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] );
            _field14 = new Q_Double( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++],
            packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] );
            _field15 = new Q_Double( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++],
            packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] );
            _field16 = new Q_Double( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++],
            packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] );
            _field17 = new Q_Double( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++],
            packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] );
            _field18 = new Q_Double( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++],
            packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] );
            _field19 = new Q_Double( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++],
            packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] );

            _field20 = packetDataField[position++];
            _field21 = packetDataField[position++];
            position += 2;

            RecipeType = Util.ConvertModeToRecipeType( Mode1, Mode2 );
        }
    }

    /// <summary>
    /// 채널 상태 데이터(0x1100)용 클래스입니다.
    /// </summary>
    public class ChannelStateData
    {
        public readonly ushort ChannelCount;

        public readonly ChannelState ChannelState;
        public readonly uint TotalTime;
        public readonly uint StepCount;
        public readonly ushort StepNumber;
        public readonly uint CycleCount;

        public readonly Mode1 Mode1;
        public readonly Mode2 Mode2;

        public readonly StoppedType StoppedType;
        public readonly byte TotalTimeOverflow;

        public ChannelStateData( byte[] packetDataField )
        {
            var position = 0;

            ChannelCount = new Q_UInt16( packetDataField[position++], packetDataField[position++] ).Value;
            position++; // Reserved
            ChannelState = ( ChannelState )packetDataField[position++];
            TotalTime = new Q_UInt32( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] ).Value;
            StepCount = new Q_UInt32( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] ).Value;
            StepNumber = new Q_UInt16( packetDataField[position++], packetDataField[position++] ).Value;
            CycleCount = new Q_UInt32( packetDataField[position++], packetDataField[position++], packetDataField[position++], packetDataField[position++] ).Value;
            Mode1 = ( Mode1 )packetDataField[position++];
            Mode2 = ( Mode2 )packetDataField[position++];
            StoppedType = ( StoppedType )packetDataField[position++];
            TotalTimeOverflow = packetDataField[position++];
        }
    }
    /// <summary>
    /// 모든 측정 데이터 패킷을 다루는 클래스의 부모 클래스가 되는 추상 클래스입니다.
    /// </summary>
    public abstract class ChannelData
    {
        /// <summary>
        /// 현재 데이터 개체에 포함된 <see cref="MeasureData"/> 개체의 개수입니다.
        /// </summary>
        public abstract int Count { get; }
        /// <summary>
        /// 지정된 위치의 <see cref="MeasureData"/>를 가져옵니다.
        /// </summary>
        /// <param name="index">가져올 위치입니다.</param>
        /// <returns></returns>
        public abstract MeasureData this[int index] { get; }

        public ChannelState ChannelState;

        public Mode2 Mode2;
    }
    /// <summary>
    /// 채널별 Sequence 데이터(0x1111)용 클래스입니다.
    /// <br>Charge, Discharge, Rest, ACR(Summuary), FR(Summary)의 데이터를 처리할 수 있습니다.</br>
    /// </summary>
    public class ChannelSequenceData : ChannelData
    {
        public override int Count => _measureDatas.Count;
        public override MeasureData this[int index] => _measureDatas[index];

        public byte SlaveAlarmStatus;
        //public ChannelState ChannelState;

        private List<MeasureData> _measureDatas;

        public ChannelSequenceData( byte[] rawData )
        {
            // rawData의 TotalTime 부터 반복되므로,
            // (전체 길이 - 반복되지 않는 구간의 길이) / 반복되는 구간의 고정 길이(96) = 반복되는 구간의 반복 횟수 = DataField 개수
            // 만약 반복되는 구간의 총 길이가 96으로 나누어 떨어지지 않는다면 처리 불가
            if ( ( rawData.Length - 4 ) % 96 != 0 ) throw new QException( QExceptionType.PACKET_CHANNEL_DATA_DATAFIELD_LENGTH_ERROR );
            var dataFieldCount = ( rawData.Length - 4 ) / 96;

            var position = 2;   // 맨 앞에 Reserved 2Byte

            SlaveAlarmStatus = rawData[position++];
            ChannelState = ( ChannelState )rawData[position++];
            Mode2 = (Mode2)rawData[19];
            _measureDatas = new List<MeasureData>();

            for ( var i = 0; i < dataFieldCount; i++ )
            {
                var dataField = new MeasureData( rawData, ref position );

                _measureDatas.Add( dataField );
            }
        }
    }
    /// <summary>
    /// 고속 측정 Seuqnece 데이터(0x1113)용 클래스입니다.
    /// <br>ACR, FR, TR의 Raw 데이터를 처리할 수 있습니다.</br>
    /// </summary>
    public class ChannelMeasureData : ChannelData
    {
        public override int Count => _measureDatas.Count;
        public override MeasureData this[int index] => _measureDatas[index];

        //public ChannelState ChannelState;
        public uint StartTime;
        public uint StepCount;
        public ushort StepNumber;
        public uint CycleCount;
        public byte Mode1;
        public byte Mode2;
        public StoppedType StoppedType;
        public byte TotalTimeOverflow;

        private List<MeasureData> _measureDatas;

        public ChannelMeasureData( byte[] rawData )
        {
            // 24
            if ( ( rawData.Length - 24 ) % 24 != 0 ) throw new QException( QExceptionType.PACKET_CHANNEL_DATA_DATAFIELD_LENGTH_ERROR );
            var dataFieldCount = ( rawData.Length - 24 ) / 24;

            var position = 3;

            ChannelState = ( ChannelState )rawData[position++];

            StepCount = new Q_UInt32( rawData[position++], rawData[position++], rawData[position++], rawData[position++] );
            StepNumber = new Q_UInt16( rawData[position++], rawData[position++] );
            CycleCount = new Q_UInt32( rawData[position++], rawData[position++], rawData[position++], rawData[position++] );

            Mode1 = rawData[position++];
            Mode2 = rawData[position++];
            StoppedType = ( StoppedType )rawData[position++];
            TotalTimeOverflow = rawData[position++];

            // Reserved 2bytes
            position += 2;

            // ChannelMeasureData는 반복되는 영역에 위 내용이 포함되지 않는다. 따라서 MeasureData를 생성하면서 위 내용을 직접 넣어준다.
            _measureDatas = new List<MeasureData>();
            var recipeType = Util.ConvertModeToRecipeType( ( Mode1 )Mode1, ( Mode2 )Mode2 );
            for ( var i = 0; i < dataFieldCount; i++ )
            {
                var d = new MeasureData( recipeType );

                d.IsRaw = true;

                d._field4 = StartTime;
                d._field5 = StepCount;
                d._field6 = StepNumber;
                d._field7 = CycleCount;

                d._field8 = Mode1;
                d._field9 = Mode2;

                d._field20 = ( byte )StoppedType;
                d._field21 = TotalTimeOverflow;

                d.StepTime = new Q_UInt32( rawData[position++], rawData[position++], rawData[position++], rawData[position++] );
                d._field11 = new Q_Double( rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++] );
                d._field12 = new Q_Double( rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++] );
                d._field13 = new Q_Float( rawData[position++], rawData[position++], rawData[position++], rawData[position++] );
            }
        }
    }
    /// <summary>
    /// 채널별 Pattern 데이터(0x1112)용 클래스입니다.
    /// <br>Pattern 데이터를 처리할 수 있습니다.</br>
    /// </summary>
    public class ChannelPatternData : ChannelData
    {
        public override int Count => _measureDatas.Count;
        public override MeasureData this[int index] => _measureDatas[index];

        //public ChannelState ChannelState;
        public uint StartTime;
        public uint StepCount;
        public ushort StepNumber;
        public uint CycleCount;
        public byte Mode1;
        public byte Mode2;
        public StoppedType StoppedType;
        public byte TotalTimeOverflow;

        private List<MeasureData> _measureDatas;

        public ChannelPatternData( byte[] rawData )
        {
            // 24
            if ( ( rawData.Length - 24 ) % 24 != 0 ) throw new QException( QExceptionType.PACKET_CHANNEL_DATA_DATAFIELD_LENGTH_ERROR );
            var dataFieldCount = ( rawData.Length - 24 ) / 24;

            var position = 3;

            ChannelState = ( ChannelState )rawData[position++];

            StepCount = new Q_UInt32( rawData[position++], rawData[position++], rawData[position++], rawData[position++] );
            StepNumber = new Q_UInt16( rawData[position++], rawData[position++] );
            CycleCount = new Q_UInt32( rawData[position++], rawData[position++], rawData[position++], rawData[position++] );

            Mode1 = rawData[position++];
            Mode2 = rawData[position++];
            StoppedType = ( StoppedType )rawData[position++];
            TotalTimeOverflow = rawData[position++];

            // Reserved 2bytes
            position += 2;

            // ChannelMeasureData는 반복되는 영역에 위 내용이 포함되지 않는다. 따라서 MeasureData를 생성하면서 위 내용을 직접 넣어준다.
            _measureDatas = new List<MeasureData>();
            var recipeType = Util.ConvertModeToRecipeType( ( Mode1 )Mode1, ( Mode2 )Mode2 );
            for ( var i = 0; i < dataFieldCount; i++ )
            {
                var d = new MeasureData( recipeType );

                d.IsRaw = true;

                d._field4 = StartTime;
                d._field5 = StepCount;
                d._field6 = StepNumber;
                d._field7 = CycleCount;

                d._field8 = Mode1;
                d._field9 = Mode2;

                d._field20 = ( byte )StoppedType;
                d._field21 = TotalTimeOverflow;

                d.StepTime = new Q_UInt32( rawData[position++], rawData[position++], rawData[position++], rawData[position++] );
                d._field11 = new Q_Double( rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++] );
                d._field12 = new Q_Double( rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++], rawData[position++] );
                d._field13 = new Q_Float( rawData[position++], rawData[position++], rawData[position++], rawData[position++] );
            }
        }
    }

    internal struct BoardInfo
    {
        internal readonly string Model;
        internal readonly string SerialNo;
        internal readonly DateTime Date;
        internal readonly VersionInfo FirmwareVersion;
        internal readonly VersionInfo FpgaVersion;
        internal readonly VersionInfo ProtocolVersion;
        internal readonly byte DacChannelCount;
        internal readonly byte AdcChannelCount;
        internal readonly byte AuxAdcChannelCount;

        internal BoardInfo( string model, string serialNo, DateTime date, VersionInfo firmware, VersionInfo fpga, VersionInfo protocol, byte dac, byte adc, byte auxadc )
        {
            Model = model;
            SerialNo = serialNo;
            Date = date;
            FirmwareVersion = firmware;
            FpgaVersion = fpga;
            ProtocolVersion = protocol;
            DacChannelCount = dac;
            AdcChannelCount = adc;
            AuxAdcChannelCount = auxadc;
        }

        public override string ToString()
        {
            return $"Model: {Model}\r\n" +
                   $"SerialNo: {SerialNo}\r\n" +
                   $"Date: {Date}\r\n" +
                   $"Firmware: {FirmwareVersion}\r\n" +
                   $"Fpga: {FpgaVersion}\r\n" +
                   $"Protocol: {ProtocolVersion}\r\n" +
                   $"DAC: {DacChannelCount}\r\n" +
                   $"ADC: {AdcChannelCount}\r\n" +
                   $"Aux ADC: {AuxAdcChannelCount}";
        }
    }
    #endregion

    #region Pattern
    public class PatternItem
    {
        public readonly double Value;
        public readonly int Count;
        public PatternItem( double value, int count )
        {
            Value = value;
            Count = count;
        }
    }
    public class PatternData : ICloneable
    {
        private const int MAX_PULSE_ITEM = 100;

        private PatternBiasMode _biasMode;
        private int _pulseWidth;
        private List<PatternItem> _items;
        private string _filename;

        public PatternBiasMode BiasMode => _biasMode;
        public int PulseWidth => _pulseWidth;

        public string FileName => _filename;
        public PatternItem this[int index] => _items[index];
        public int Count => _items.Count;

        public bool Add( PatternItem item )
        {
            if ( TotalCount + item.Count > MAX_PULSE_ITEM ) return false;
            _items.Add( item );
            return true;
        }
        public void Remove( PatternItem item ) => _items.Remove( item );
        public void RemoveAt( int index ) => _items.RemoveAt( index );
        public void Clear() => _items.Clear();
        public bool Insert( int index, PatternItem item )
        {
            if ( TotalCount + item.Count > MAX_PULSE_ITEM ) return false;
            _items.Insert( index, item );
            return true;
        }

        public void Save( string filename )
        {
            using ( var sw = new StreamWriter( filename ) )
            {
                sw.WriteLine( $"{_biasMode}" );
                sw.WriteLine( $"{_pulseWidth}" );

                foreach ( var item in _items )
                {
                    sw.WriteLine( $"{item.Value},{item.Count}" );
                }
            }
        }

        public static PatternData FromFile( string filename )
        {
            if ( filename == null || filename.Length == 0 ) throw new QException( QExceptionType.IO_INVALID_FILE_NAME_ERROR );
            else if ( !new FileInfo( filename ).Exists ) throw new QException( QExceptionType.IO_FILE_NOT_FOUND_ERROR );

            var result = new PatternData();
            using ( var sr = new StreamReader( filename ) )
            {
                switch ( sr.ReadLine() )
                {
                    case "CC":
                        result._biasMode = PatternBiasMode.CC;
                        break;

                    case "CP":
                        result._biasMode = PatternBiasMode.CP;
                        break;

                    default:
                        throw new QException( QExceptionType.PATTERN_INVALID_BIAS_MODE_ERROR );
                }

                switch ( sr.ReadLine() )
                {
                    case "10":
                        result._pulseWidth = 10;
                        break;

                    case "100":
                        result._pulseWidth = 100;
                        break;

                    default:
                        throw new QException( QExceptionType.PATTERN_INVALID_BIAS_MODE_ERROR );
                }

                string line;
                while ( ( line = sr.ReadLine() ) != null )
                {
                    var split = line.Split( ',' );
                    if ( split.Length != 2 ) throw new QException( QExceptionType.PATTERN_INVALID_PULSE_ITEM_ERROR );

                    if ( !double.TryParse( split[0], out double value ) || !int.TryParse( split[1], out int count ) )
                        throw new QException( QExceptionType.PATTERN_INVALID_PULSE_ITEM_ERROR );

                    if ( !result.Add( new PatternItem( value, count ) ) )
                    {
                        throw new QException( QExceptionType.PATTERN_TOO_MANY_PULSE_ITEM_ERROR );
                    }
                }
            }

            result._filename = filename;
            return result;
        }

        public ushort TotalCount => ( ushort )_items.Select( i => i.Count ).Sum();

        private PatternData() { _items = new List<PatternItem>(); }
        public PatternData( PatternBiasMode biasMode, int pulseWidth ) : this()
        {
            _biasMode = biasMode;
            _pulseWidth = pulseWidth;
        }

        public object Clone()
        {
            var clone = new PatternData();

            clone._items.AddRange( _items );
            clone._filename = _filename;
            clone._pulseWidth = _pulseWidth;
            clone._biasMode = _biasMode;

            return clone;
        }

        private DataBuilder createBuilder( ushort stepNo, int startIndex )
        {
            var builder = new DataBuilder();

            // Reserved (2Byte) - Reserved
            builder.AddCount( 0, 2 );

            // Step count (4Byte) - Reserved
            builder.AddCount( 0, 4 );

            // Step no (2Byte)
            builder.Add( new Q_UInt16( stepNo ) );

            // Cycle no (4Byte) - Reserved
            builder.AddCount( 0, 4 );

            // Mode1 (1Byte) - Pattern : 4
            builder.Add( Mode1.Pattern );

            // Mode2 (1Byte) - Data : 1
            builder.Add( 1 );

            // 설정 데이터 시작 Index
            builder.Add( new Q_UInt16( ( ushort )startIndex ) );

            // Reserved (28Byte) - Reserved
            builder.AddCount( 0, 28 );

            return builder;
        }
        /// <summary>
        /// 현재 패턴 데이터를 패킷의 DATA 필드 형태로 변환합니다.
        /// <br>하나의 패턴 데이터 패킷에는 최대 25개의 패턴 아이템이 포함될 수 있으며, 따라서 이 메서드가 반환하는 리스트에는 (<see cref="TotalCount"/> / 25)개의 DATA 배열이 포함되어 있습니다.</br>
        /// </summary>
        /// <returns></returns>
        public List<byte[]> ToDataField( ushort stepNo )
        {
            var itemIndex = 0;
            var packetNo = 0;

            List<byte[]> result = new List<byte[]>();
            DataBuilder builder = null;

            List<double> datas = new List<double>();
            for ( var i = 0; i < _items.Count; i++ )
            {
                for ( var j = 0; j < _items[i].Count; j++ ) datas.Add( _items[i].Value );
            }

            while ( true )
            {
                if ( builder == null ) builder = createBuilder( stepNo, packetNo * 25 + itemIndex );

                builder.Add( datas[( packetNo * 25 ) + itemIndex] );

                if ( packetNo * 25 + itemIndex + 1 == datas.Count ) break;
                else if ( itemIndex == 24 )
                {
                    itemIndex = 0;
                    packetNo++;

                    // Reserved (16Byte) - Reserved
                    builder.AddCount( 0, 16 );
                    result.Add( builder );

                    builder = null;
                }
                else itemIndex++;
            }

            if ( builder != null )
            {
                // 25개 데이터 필드 중 채워지지 않은 영역을 0으로 채움 (1개 당 8바이트씩)
                builder.AddCount( 0, ( 25 - itemIndex - 1 ) * 8 );
                // Reserved (16Byte) - Reserved
                builder.AddCount( 0, 16 );
                result.Add( builder );
            }

            return result;
        }
    }
    #endregion
}
