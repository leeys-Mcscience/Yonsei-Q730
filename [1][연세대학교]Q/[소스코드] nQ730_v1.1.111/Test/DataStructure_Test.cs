using System.Runtime.InteropServices;

namespace Test
{
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
        public Q_UInt16( byte order0, byte order1 )
        {
            _value = 0;
            _offset1 = order1;
            _offset0 = order0;
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
        public Q_UInt32( byte order0, byte order1, byte order2, byte order3 )
        {
            _value = 0;
            _offset0 = order0;
            _offset1 = order1;
            _offset2 = order2;
            _offset3 = order3;
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
        public Q_Float( byte order0, byte order1, byte order2, byte order3 )
        {
            _value = 0;
            _offset0 = order0;
            _offset1 = order1;
            _offset2 = order2;
            _offset3 = order3;
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
        public Q_Double( byte order0, byte order1, byte order2, byte order3, byte order4, byte order5, byte order6, byte order7 )
        {
            _value = 0;
            _offset0 = order0;
            _offset1 = order1;
            _offset2 = order2;
            _offset3 = order3;
            _offset4 = order4;
            _offset5 = order5;
            _offset6 = order6;
            _offset7 = order7;
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
        public Q_UInt16_Unsafe( byte order0, byte order1 )
        {
            Value = 0;
            Offset1 = order1;
            Offset0 = order0;
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
        public Q_UInt32_Unsafe( byte order0, byte order1, byte order2, byte order3 )
        {
            Value = 0;
            Offset0 = order0;
            Offset1 = order1;
            Offset2 = order2;
            Offset3 = order3;
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
        public Q_Float_Unsafe( byte order0, byte order1, byte order2, byte order3 )
        {
            Value = 0;
            Offset0 = order0;
            Offset1 = order1;
            Offset2 = order2;
            Offset3 = order3;
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
        public Q_Double_Unsafe( byte order0, byte order1, byte order2, byte order3, byte order4, byte order5, byte order6, byte order7 )
        {
            Value = 0;
            Offset0 = order0;
            Offset1 = order1;
            Offset2 = order2;
            Offset3 = order3;
            Offset4 = order4;
            Offset5 = order5;
            Offset6 = order6;
            Offset7 = order7;
        }
        public static implicit operator double( Q_Double_Unsafe q_double ) => q_double.Value;
    }
}
