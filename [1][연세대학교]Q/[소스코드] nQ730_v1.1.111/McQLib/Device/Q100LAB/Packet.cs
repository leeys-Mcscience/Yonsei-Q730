using System;
using System.Collections.Generic;
using System.Text;
using McQLib.Core;

namespace McQLib.Device
{
    /// <summary>
    /// 패킷의 Parsing 결과를 나타내는 열거형입니다.
    /// 각 상태는 패킷의 필드를 나타내며, Parsing이 종료된 후 <see cref="Packet.ParsingState"/>가 <see cref="ParsingState.Complete"/>가 아니라면 해당 필드에서 오류가 발생해 Parsing이 중단됐음을 나타냅니다.
    /// </summary>
    public enum ParsingState : byte
    {
        /// <summary>
        /// STX2 필드입니다.
        /// </summary>
        STX2,
        /// <summary>
        /// ADDR2 필드입니다.
        /// </summary>
        ADDR2,
        /// <summary>
        /// CH2 필드입니다.
        /// </summary>
        CH2,
        /// <summary>
        /// ByPass 필드입니다.
        /// </summary>
        ByPass,
        /// <summary>
        /// LEN2 필드입니다.
        /// </summary>
        LEN2,
        /// <summary>
        /// SubPacket 필드 즉, 통합프로토콜1 파트입니다.
        /// </summary>
        SubPackets,
        /// <summary>
        /// STX 필드입니다.
        /// </summary>
        STX,
        /// <summary>
        /// ADDR 필드입니다.
        /// </summary>
        ADDR,
        /// <summary>
        /// CH 필드입니다.
        /// </summary>
        CH,
        /// <summary>
        /// CMD 필드입니다.
        /// </summary>
        CMD,
        /// <summary>
        /// ERR 필드입니다.
        /// </summary>
        ERR,
        /// <summary>
        /// LEN 필드입니다.
        /// </summary>
        LEN,
        /// <summary>
        /// DATA 필드입니다.
        /// </summary>
        DATA,
        /// <summary>
        /// CRC 필드입니다.
        /// </summary>
        CRC,
        /// <summary>
        /// 저장된 시퀀스 데이터 송신 패킷 전용 State입니다.
        /// <br>내부 패킷의 사이에 CRLF가 존재하지 않는 경우 이 필드에서 중단됩니다.</br>
        /// </summary>
        CRLF,
        /// <summary>
        /// 저장된 시퀀스 데이터 송신 패킷 전용 State입니다.
        /// <br>내부 패킷을 Packet으로 생성하는 도중 오류가 발생한 경우 이 필드에서 중단됩니다.</br>
        /// </summary>
        INNER,
        /// <summary>
        /// ETX 필드입니다.
        /// </summary>
        ETX,
        /// <summary>
        /// CRC2 필드입니다.
        /// <br>일반적으로 CRC2 오류가 발생했을 때 이 필드에서 중단됩니다.</br>
        /// </summary>
        CRC2,
        /// <summary>
        /// ETX2 필드입니다.
        /// </summary>
        ETX2,
        /// <summary>
        /// Parsing이 정상 종료되었음을 의미합니다.
        /// </summary>
        Complete
    };

    public abstract class Packet
    {
        internal const uint _STX2 = 0xFEFEDCBA;
        internal const byte _STX2_1 = 0xFE;
        internal const byte _STX2_2 = 0xFE;
        internal const byte _STX2_3 = 0xDC;
        internal const byte _STX2_4 = 0xBA;

        internal const ushort _ETX2 = 0xF5F5;
        internal const byte _ETX2_1 = 0xF5;
        internal const byte _ETX2_2 = 0xF5;

        internal const byte _CRC2 = 0x10;          // CRC2에서 XOR하는 값
        internal const byte _STX = 0x02;
        internal const byte _ETX = 0x03;
        internal const byte _CRC = 0x40;           // CRC에서 XOR하는 값

        internal const byte _CR = 0x0D;    // Carriage Return(\r)  (0x0D)
        internal const byte _LF = 0x0A;    // Line Feed(\n)        (0x0A)

        /// <summary>
        /// 장비로 명령을 전달할 때 ERR 필드에 사용되는 값입니다. 값을 설정하기 위한 명령입니다.
        /// </summary>
        public const byte SET = 0x01;
        /// <summary>
        /// 장비로 명령을 전달할 때 ERR 필드에 사용되는 값입니다. 값을 가져오기 위한 명령입니다.
        /// </summary>
        public const byte GET = 0x00;
        /// <summary>
        /// 장비로 명령을 전달할 때 ByPass를 On 하기 위해 사용하는 값입니다.
        /// </summary>
        public const byte ON = 0x01;
        /// <summary>
        /// 장비로 명령을 전달할 때 ByPass를 Off 하기 위해 사용하는 값입니다.
        /// </summary>
        public const byte OFF = 0x00;

        public const byte NO_ERROR = 0x00;

        public const int MASTER = -9;

        public abstract ParsingState ParsingState { get; }
        public abstract int ErrorPosition { get; }

        public SubPacket this[int index]
        {
            get
            {
                if ( SubPackets.Count <= index || index < 0 ) throw new QException( QExceptionType.PACKET_SUBPACKET_INDEX_OUT_OF_RANGE_ERROR );

                return SubPackets[index];
            }
        }

        public abstract byte[] RawPacket { get; }
        public abstract byte STX2_1 { get; }
        public abstract byte STX2_2 { get; }
        public abstract byte STX2_3 { get; }
        public abstract byte STX2_4 { get; }

        //public abstract int ChannelNo { get; set; }

        public abstract byte ADDR2 { get; set; }
        public abstract byte CH2 { get; set; }

        public abstract byte ByPass { get; set; }

        public abstract ushort LEN2 { get; }
        public abstract byte LEN2_1 { get; }
        public abstract byte LEN2_2 { get; }
        /// <summary>
        /// 이 속성은 일반적으로 SubPackets의 첫 번째 요소를 반환합니다.
        /// </summary>
        public SubPacket SubPacket => SubPackets[0];
        public readonly List<SubPacket> SubPackets = new List<SubPacket>();

        public abstract byte CRC2 { get; }

        public abstract byte ETX2_1 { get; }
        public abstract byte ETX2_2 { get; }

        public byte[] ToByteArray()
        {
            var array = new List<byte>();

            array.Add( STX2_1 );
            array.Add( STX2_2 );
            array.Add( STX2_3 );
            array.Add( STX2_4 );

            array.Add( ADDR2 );
            array.Add( CH2 );

            array.Add( ByPass );

            array.Add( LEN2_1 );
            array.Add( LEN2_2 );

            foreach ( var b in SubPackets ) array.AddRange( b.ToByteArray() );

            array.Add( CRC2 );
            array.Add( ETX2_1 );
            array.Add( ETX2_2 );

            return array.ToArray();
        }
        public static implicit operator byte[]( Packet p ) => p.ToByteArray();

        /// <summary>
        /// 패킷 정보를 로그 메시지 형태로 정리하여 반환합니다.
        /// </summary>
        /// <param name="rawPacket">로그 메시지에 패킷의 Raw 형태 데이터를 출력할지의 여부입니다.</param>
        /// <returns>생성된 로그 메시지입니다.</returns>
        public abstract string ToLogText( bool rawPacket = true );
    }
    public abstract class SubPacket
    {
        public abstract byte STX { get; }

        public abstract byte ADDR { get; set; }
        public abstract byte CH { get; set; }

        public Enum Command => Util.ConvertCmdToEnum( CMD );

        public abstract ushort CMD { get; set; }
        public abstract byte CMD_1 { get; set; }
        public abstract byte CMD_2 { get; set; }

        public abstract byte ERR { get; set; }

        public abstract ushort LEN { get; }
        public abstract byte LEN_1 { get; }
        public abstract byte LEN_2 { get; }

        //public readonly List<byte> DATA = new List<byte>();
        public readonly DataBuilder DATA = new DataBuilder();


        public abstract byte CRC { get; }
        public abstract byte ETX { get; }

        public byte[] ToByteArray()
        {
            var array = new DataBuilder();

            array.Add( STX );
            array.Add( ADDR );
            array.Add( CH );
            array.Add( CMD_1 );
            array.Add( CMD_2 );
            array.Add( ERR );
            array.Add( LEN_1 );
            array.Add( LEN_2 );
            array.Add( DATA );
            array.Add( CRC );
            array.Add( ETX );

            return array;
        }
        public static implicit operator byte[]( SubPacket p ) => p.ToByteArray();
    }
    /// <summary>
    /// 보내기 전용 패킷입니다.
    /// <br>ADDR2, CH2, ByPass 및 SubPackets 필드만 쓰기 가능, 나머지는 읽기 전용입니다.</br>
    /// </summary>
    public sealed class SendPacket : Packet
    {
        public override ParsingState ParsingState => ParsingState.Complete;
        public override int ErrorPosition => -1;

        public override byte[] RawPacket => ToByteArray();

        public override byte STX2_1 => Packet._STX2_1;
        public override byte STX2_2 => Packet._STX2_2;
        public override byte STX2_3 => Packet._STX2_3;
        public override byte STX2_4 => Packet._STX2_4;

        //public override int ChannelNo
        //{
        //    get => Util.GetIndex( _addr2, _ch2 );
        //    set
        //    {
        //        _addr2 = Util.GetADDR( value );
        //        _ch2 = Util.GetCH( value );
        //    }
        //}
        public override byte ADDR2 { get => _addr2; set => _addr2 = value; }
        public override byte CH2 { get => _ch2; set => _ch2 = value; }

        private byte _addr2;
        private byte _ch2;

        public override byte ByPass { get; set; }

        public override ushort LEN2
        {
            get
            {
                ushort count = 0;

                foreach ( var sub in SubPackets ) count += ( ushort )sub.ToByteArray().Length;

                return count;
            }
        }
        public override byte LEN2_1 => new Q_UInt16( LEN2 ).Offset0;
        public override byte LEN2_2 => new Q_UInt16( LEN2 ).Offset1;

        public override byte CRC2
        {
            get
            {
                byte crc = 0;
                foreach ( var sub in SubPackets )
                {
                    var arr = sub.ToByteArray();
                    foreach ( var b in arr )
                    {
                        crc ^= b;
                    }
                }

                return ( byte )( crc ^ Packet._CRC2 );
            }
        }
        public override byte ETX2_1 => Packet._ETX2_1;
        public override byte ETX2_2 => Packet._ETX2_2;

        //public SendPacket( int channelNo )
        //{
        //    //ChannelNo = channelNo;
        //}
        public SendPacket( byte addr, byte ch )
        {
            _addr2 = addr;
            _ch2 = ch;
        }
        /// <summary>
        /// 지정된 주소로 송신하기 위한 패킷을 생성합니다.
        /// </summary>
        /// <param name="address"></param>
        public SendPacket( Address address ) : this( address.ADDR, address.CH ) { }
        /// <summary>
        /// 지정된 주소로 송신하기 위한 패킷을 생성합니다.
        /// <br>기본적으로 1개의 내부 패킷을 지정된 커멘드로 함께 생성합니다.</br>
        /// </summary>
        /// <param name="address">패킷의 목적지 주소입니다.</param>
        /// <param name="command">패킷에 추가할 명령어입니다.</param>
        public SendPacket( Address address, Enum command ) : this( address )
        {
            SubPackets.Add( new SendSubPacket( command ) );
        }

        public override string ToLogText( bool rawPacket = true )
        {
            var builder = new StringBuilder();

            builder.Append( $"SendPacket({ParsingState}) : ADDR = 0x{ADDR2:X2}, CH = 0x{CH2:X2}, CMD = 0x{SubPacket.CMD_1:X2}{SubPacket.CMD_2:X2}({SubPacket.Command}), ERR = 0x{SubPacket.ERR:X2}, LEN = 0x{SubPacket.LEN:X4}({SubPacket.LEN})" );

            // 패킷의 SubPakcet의 개수가 0개인 패킷은 있을 수 없다.
            //if( SubPackets.Count != 0 ) builder.Append( $"CMD = 0x{SubPacket.CMD_1:X2}{SubPacket.CMD_2:X2}({SubPacket.Command}), ERR = 0x{SubPacket.ERR:X2}, LEN = 0x{SubPacket.LEN:X2}, Length = {SubPacket.LEN + 10}" );

            if ( rawPacket )
            {
                builder.Append( $"\r\nRawPacket = {Util.BytesToString( RawPacket )}" );
            }

            return builder.ToString();
        }
    }
    public sealed class SendSubPacket : SubPacket
    {
        public override byte STX => Packet._STX;
        public override byte ADDR { get => 0xFF; set => throw new QException( QExceptionType.PACKET_REJECT_WRITING_ERROR ); }
        public override byte CH { get => 0xFF; set => throw new QException( QExceptionType.PACKET_REJECT_WRITING_ERROR ); }

        public override ushort CMD
        {
            get => new Q_UInt16( _cmd_1, _cmd_2 ).Value;
            set
            {
                var cmd = new Q_UInt16( value );
                _cmd_1 = cmd.Offset0;
                _cmd_2 = cmd.Offset1;
            }
        }
        public override byte CMD_1 { get => _cmd_1; set => _cmd_1 = value; }
        public override byte CMD_2 { get => _cmd_2; set => _cmd_2 = value; }

        private byte _cmd_1;
        private byte _cmd_2;

        public override byte ERR { get; set; }

        public override ushort LEN => ( ushort )DATA.Count;
        public override byte LEN_1 => new Q_UInt16( LEN ).Offset0;
        public override byte LEN_2 => new Q_UInt16( LEN ).Offset1;

        public override byte CRC
        {
            get
            {
                byte crc = 0;
                // 보내는 패킷의 ADDR과 CH는 항상 0xFF로 고정이기 때문에 xor해도 서로 상쇄되어 굳이 안해도 되긴 함.
                crc ^= ADDR;
                crc ^= CH;
                crc ^= _cmd_1;
                crc ^= _cmd_2;
                crc ^= ERR;
                crc ^= LEN_1;
                crc ^= LEN_2;

                for ( var i = 0; i < LEN; i++ ) crc ^= DATA[i];

                return ( byte )( crc ^ Packet._CRC );
            }
        }
        public override byte ETX => Packet._ETX;
        /// <summary>
        /// 지정된 커멘드를 사용하여 새로운 SendSubPacket 인스턴스를 생성합니다.
        /// <br>ERR 필드는 0, DATA 필드는 Empty로 초기화됩니다.</br>
        /// </summary>
        /// <param name="cmd">SendSubPacket에 추가할 명령어입니다.</param>
        public SendSubPacket( Enum cmd )
        {
            CMD = ( ushort )cmd.ToValue();
        }
        /// <summary>
        /// 지정된 커멘드와 Error/Query 필드 값을 사용하여 새로운 SendSubPacket 인스턴스를 생성합니다.
        /// <br>DATA 필드는 Empty로 초기화됩니다.</br>
        /// </summary>
        /// <param name="cmd">SendSubPacket에 추가할 명령어입니다.</param>
        /// <param name="err">SendSubPacket에 추가할 Error/Query 필드 값입니다.</param>
        public SendSubPacket( Enum cmd, byte err ) : this( cmd )
        {
            ERR = err;
        }
        /// <summary>
        /// 지정된 커멘드, Error/Query 필드 값 및 DATA 필드 값을 사용하여 새로운 SendSubPacket 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="cmd">SendSubPacket에 추가할 명령어입니다.</param>
        /// <param name="err">SendSubPacket에 추가할 Error/Query 필드 값입니다.</param>
        /// <param name="data">SendSubPacket에 추가할 DATA 필드 값입니다.</param>
        public SendSubPacket( Enum cmd, byte err, params byte[] data ) : this( cmd, err )
        {
            DATA.Add( data );
        }
    }

    public sealed class ReceivedPacket : Packet
    {
        private ReceivedPacket() { }

        public override ParsingState ParsingState => _parsingState;
        public override int ErrorPosition => _errorPosition;

        public override byte[] RawPacket => _rawPacket;
        private byte[] _rawPacket;

        public override byte STX2_1 => _stx2_1;
        public override byte STX2_2 => _stx2_2;
        public override byte STX2_3 => _stx2_3;
        public override byte STX2_4 => _stx2_4;

        //public override int ChannelNo { get => Util.GetIndex( _addr2, _ch2 ); set => throw new QException( QExceptionType.PACKET_REJECT_WRITING_ERROR ); }
        public override byte ADDR2 { get => _addr2; set => throw new QException( QExceptionType.PACKET_REJECT_WRITING_ERROR ); }
        public override byte CH2 { get => _ch2; set => throw new QException( QExceptionType.PACKET_REJECT_WRITING_ERROR ); }

        public override byte ByPass { get => _byPass; set => throw new QException( QExceptionType.PACKET_REJECT_WRITING_ERROR ); }

        public override ushort LEN2 => new Q_UInt16( _len2_1, _len2_2 ).Value;
        public override byte LEN2_1 => _len2_1;
        public override byte LEN2_2 => _len2_2;

        public override byte CRC2 => _crc2;

        public override byte ETX2_1 => _etx2_1;
        public override byte ETX2_2 => _etx2_2;

        private ParsingState _parsingState;
        private int _errorPosition;

        private byte _stx2_1;
        private byte _stx2_2;
        private byte _stx2_3;
        private byte _stx2_4;

        private byte _addr2;
        private byte _ch2;

        private byte _byPass;
        private byte _len2_1;
        private byte _len2_2;

        private byte _crc2;
        private byte _etx2_1;
        private byte _etx2_2;

        /// <summary>
        /// 지정된 <see cref="byte"/>형식 배열로 구성된 Raw Packet 데이터를 사용하여 새로운 <see cref="ReceivedPacket"/> 인스턴스를 구조화합니다.
        /// <br>이 메서드는 언제나 <see cref="ReceivedPacket"/>의 인스턴스를 생성하여 반환합니다. 반환되는 패킷을 사용하기 전에 <seealso cref="Q730.ParsingState"/> <see cref="ReceivedPacket.ParsingState"/> 필드를 통해 패킷의 무결성을 확인하십시오.</br>
        /// </summary>
        /// <param name="rawPacket"><see cref="ReceivedPacket"/>을 구성할 Raw Packet 데이터입니다.</param>
        /// <returns>생성된 <see cref="ReceivedPacket"/> 인스턴스입니다.</returns>
        public static ReceivedPacket Parse( byte[] rawPacket, int startPosition = 0 )
        {
            // MainPacket 구조
            // ┌───────────────────┬──────────┬────────┬───────────┬─────────┬──────────┬─────────┬─────────┐
            // │      STX2(4)      │ ADDR2(1) │ CH2(1) │ ByPass(1) │ LEN2(2) │  DATA(N) │ CRC2(1) │ ETX2(2) │
            // ├────┬────┬────┬────┼──────────┼────────┼───────────┼────┬────┼──────────┼─────────┼────┬────┤
            // │ FE │ FE │ DC │ BA │    FF    │   FF   │    00     │ XX | XX │   ....   │    XX   │ F5 │ F5 │
            // └────┴────┴────┴────┴──────────┴────────┴───────────┴────┴────┴──────────┴─────────┴────┴────┘

            // SubPacket 구조
            // ┌────────┬─────────┬───────┬─────────┬─────────┬─────────┬────────┬────────┐
            // │ STX(1) │ ADDR(1) │ CH(1) │  CMD(2) │  LEN(2) │ DATA(N) │ CRC(1) │ ETX(1) │
            // ├────────┼─────────┼───────┼────┬────┼────┬────┼─────────┼────────┼────────┤
            // │   02   │   XX    │  XX   │ XX │ XX │ XX │ XX │  ....   │   XX   │   03   │
            // └────────┴─────────┴───────┴────┴────┴────┴────┴─────────┴────────┴────────┘ 

            bool errorFlag = false;
            var packet = new ReceivedPacket();
            packet._rawPacket = ( byte[] )rawPacket.Clone();
            packet._errorPosition = startPosition;
            packet._parsingState = ParsingState.STX2;

            ushort len2 = 0;
            int pMemory = 0;
            ReceivedSubPacket subTemp;
            byte crc2 = 0;

            while ( packet._parsingState != ParsingState.Complete && !errorFlag )
            {
                try
                {
                    switch ( packet._parsingState )
                    {
                        case ParsingState.STX2:
                            packet._stx2_1 = rawPacket[packet._errorPosition++];
                            packet._stx2_2 = rawPacket[packet._errorPosition++];
                            packet._stx2_3 = rawPacket[packet._errorPosition++];
                            packet._stx2_4 = rawPacket[packet._errorPosition++];

                            if ( new Q_UInt32( packet._stx2_1, packet._stx2_2, packet._stx2_3, packet._stx2_4 ).Value != Packet._STX2 ) errorFlag = true;
                            else packet._parsingState = ParsingState.ADDR2;
                            break;

                        case ParsingState.ADDR2:
                            packet._addr2 = rawPacket[packet._errorPosition++];
                            packet._parsingState = ParsingState.CH2;
                            break;

                        case ParsingState.CH2:
                            packet._ch2 = rawPacket[packet._errorPosition++];
                            packet._parsingState = ParsingState.ByPass;
                            break;

                        case ParsingState.ByPass:
                            packet._byPass = rawPacket[packet._errorPosition++];
                            packet._parsingState = ParsingState.LEN2;
                            break;

                        case ParsingState.LEN2:
                            packet._len2_1 = rawPacket[packet._errorPosition++];
                            packet._len2_2 = rawPacket[packet._errorPosition++];
                            len2 = new Q_UInt16( packet._len2_1, packet._len2_2 ).Value;
                            pMemory = packet._errorPosition;
                            packet._parsingState = ParsingState.SubPackets;
                            break;

                        case ParsingState.SubPackets:
                            if ( len2 - ( packet._errorPosition - pMemory ) == 0 ) packet._parsingState = ParsingState.CRC2;
                            else
                            {
                                var state = ReceivedSubPacket.Parse( rawPacket, ref packet._errorPosition, ref crc2, out subTemp );
                                packet.SubPackets.Add( subTemp );

                                if ( state != ParsingState.Complete )
                                {
                                    packet._parsingState = state;
                                    errorFlag = true;
                                }
                            }
                            break;

                        case ParsingState.CRC2:
                            packet._crc2 = rawPacket[packet._errorPosition++];
                            crc2 ^= Packet._CRC2;
                            if ( packet._crc2 != crc2 ) errorFlag = true;
                            else packet._parsingState = ParsingState.ETX2;
                            break;

                        case ParsingState.ETX2:
                            packet._etx2_1 = rawPacket[packet._errorPosition++];
                            packet._etx2_2 = rawPacket[packet._errorPosition++];
                            if ( new Q_UInt16( packet._etx2_1, packet._etx2_2 ).Value != Packet._ETX2 ) errorFlag = true;
                            else
                            {
                                packet._parsingState = ParsingState.Complete;
                            }
                            break;
                    }
                }
                catch ( IndexOutOfRangeException ) { break; }
            }

            //packet._rawPacket = new byte[packet._errorPosition];
            //Array.Copy( rawPacket, 0, packet._rawPacket, 0, rawPacket.Length < packet._errorPosition ? rawPacket.Length : packet._errorPosition );
            return packet;
        }

        public override string ToLogText( bool rawPacket = true )
        {
            var builder = new StringBuilder();

            builder.Append( $"ReceivePacket({ParsingState})" );
            for ( var i = 0; i < SubPackets.Count; i++ ) builder.Append( $"\r\nSubPacket[{i}] : ADDR = 0x{SubPackets[i].ADDR:X2}, CH = 0x{SubPackets[i].CH:X2}, CMD = 0x{SubPacket.CMD_1:X2}{SubPacket.CMD_2:X2}({SubPacket.Command}), ERR = 0x{SubPacket.ERR:X2}, LEN = 0x{SubPacket.LEN:X4}({SubPacket.LEN})" );

            if ( rawPacket )
            {
                builder.Append( $"\r\nRawPacket = {Util.BytesToString( RawPacket )}" );
            }

            return builder.ToString();
        }
    }
    public sealed class ReceivedSubPacket : SubPacket
    {
        public override byte STX => _stx;
        public override byte ADDR { get => _addr; set => throw new QException( QExceptionType.PACKET_REJECT_WRITING_ERROR ); }
        public override byte CH { get => _ch; set => throw new QException( QExceptionType.PACKET_REJECT_WRITING_ERROR ); }
        public override ushort CMD { get => new Q_UInt16( _cmd_1, _cmd_2 ).Value; set => throw new QException( QExceptionType.PACKET_REJECT_WRITING_ERROR ); }
        public override byte CMD_1 { get => _cmd_1; set => throw new QException( QExceptionType.PACKET_REJECT_WRITING_ERROR ); }
        public override byte CMD_2 { get => _cmd_2; set => throw new QException( QExceptionType.PACKET_REJECT_WRITING_ERROR ); }
        public override byte ERR { get => _err; set => throw new QException( QExceptionType.PACKET_REJECT_WRITING_ERROR ); }
        public override ushort LEN => new Q_UInt16( _len_1, _len_2 ).Value;
        public override byte LEN_1 => _len_1;
        public override byte LEN_2 => _len_2;
        public override byte CRC => _crc;
        public override byte ETX => _etx;

        private byte _stx;
        private byte _addr;
        private byte _ch;
        private byte _cmd_1;
        private byte _cmd_2;
        private byte _err;
        private byte _len_1;
        private byte _len_2;
        private byte _crc;
        private byte _etx;

        /// <summary>
        /// 통합 프로토콜II로 감싸지지 않은 단일 통합 프로토콜I의 패킷을 파싱하기 위한 메서드입니다.
        /// </summary>
        /// <param name="rawPacket"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public static ParsingState Parse( byte[] rawPacket, out ReceivedSubPacket sub )
        {
            sub = new ReceivedSubPacket();

            int position = 0;
            var state = ParsingState.STX;
            bool errorFlag = false;
            byte crc = 0;

            while ( state != ParsingState.Complete && !errorFlag )
            {
                switch ( state )
                {
                    case ParsingState.STX:
                        sub._stx = rawPacket[position++];
                        if ( sub._stx != Packet._STX ) errorFlag = true;
                        else state = ParsingState.ADDR;
                        break;

                    case ParsingState.ADDR:
                        sub._addr = rawPacket[position++];
                        crc ^= sub._addr;
                        state = ParsingState.CH;
                        break;

                    case ParsingState.CH:
                        sub._ch = rawPacket[position++];
                        crc ^= sub._ch;
                        state = ParsingState.CMD;
                        break;

                    case ParsingState.CMD:
                        sub._cmd_1 = rawPacket[position++];
                        sub._cmd_2 = rawPacket[position++];
                        crc ^= sub._cmd_1;
                        crc ^= sub._cmd_2;
                        state = ParsingState.ERR;
                        break;

                    case ParsingState.ERR:
                        sub._err = rawPacket[position++];
                        crc ^= sub._err;
                        state = ParsingState.LEN;
                        break;

                    case ParsingState.LEN:
                        sub._len_1 = rawPacket[position++];
                        sub._len_2 = rawPacket[position++];
                        crc ^= sub._len_1;
                        crc ^= sub._len_2;
                        state = ParsingState.DATA;
                        break;

                    case ParsingState.DATA:
                        for ( var i = 0; i < sub.LEN; i++ )
                        {
                            var pick = rawPacket[position++];
                            crc ^= pick;
                            sub.DATA.Add( pick );
                        }
                        state = ParsingState.CRC;
                        break;

                    case ParsingState.CRC:
                        sub._crc = rawPacket[position++];
                        // XOR은 결합법칙이 적용되므로, ADDR ~ DATA와 CRC SEED(0x40)가 xor된 crc를 crc2에 xor한다.
                        crc ^= Packet._CRC;
                        if ( crc != sub._crc ) errorFlag = true; // CRC 오류
                        else state = ParsingState.ETX;
                        break;

                    case ParsingState.ETX:
                        sub._etx = rawPacket[position++];
                        if ( sub._etx != Packet._ETX ) errorFlag = true;
                        else state = ParsingState.Complete;

                        break;
                }
            }

            return state;
        }
        /// <summary>
        /// 이 메서드는 <seealso cref="ReceivedPacket.Parse(byte[])"/> 메서드를 통해 호출되어야 합니다. 이 메서드를 직접 호출하지 마십시오.
        /// </summary>
        /// <param name="rawPacket">현재 파싱중인 패킷의 전체 Raw Packet 데이터입니다.</param>
        /// <param name="position">현재 파싱중인 위치입니다.</param>
        /// <param name="crc2">현재 파싱중인 패킷의 계산중인 crc2 값입니다.</param>
        /// <param name="sub">생성된 서브 패킷의 반환을 위한 out 형식 파라미터입니다.</param>
        /// <returns>서브 패킷의 파싱 결과를 나타내는 ParsingState입니다.</returns>
        internal static ParsingState Parse( byte[] rawPacket, ref int position, ref byte crc2, out ReceivedSubPacket sub )
        {
            sub = new ReceivedSubPacket();

            var state = ParsingState.STX;
            bool errorFlag = false;
            byte crc = 0;

            while ( state != ParsingState.Complete && !errorFlag )
            {
                try
                {
                    switch ( state )
                    {
                        case ParsingState.STX:
                            sub._stx = rawPacket[position++];
                            crc2 ^= sub._stx;
                            if ( sub._stx != Packet._STX ) errorFlag = true;
                            else state = ParsingState.ADDR;
                            break;

                        case ParsingState.ADDR:
                            sub._addr = rawPacket[position++];
                            crc ^= sub._addr;
                            state = ParsingState.CH;
                            break;

                        case ParsingState.CH:
                            sub._ch = rawPacket[position++];
                            crc ^= sub._ch;
                            state = ParsingState.CMD;
                            break;

                        case ParsingState.CMD:
                            sub._cmd_1 = rawPacket[position++];
                            sub._cmd_2 = rawPacket[position++];
                            crc ^= sub._cmd_1;
                            crc ^= sub._cmd_2;

                            state = ParsingState.ERR;
                            break;

                        case ParsingState.ERR:
                            sub._err = rawPacket[position++];
                            crc ^= sub._err;
                            state = ParsingState.LEN;
                            break;

                        case ParsingState.LEN:
                            sub._len_1 = rawPacket[position++];
                            sub._len_2 = rawPacket[position++];
                            crc ^= sub._len_1;
                            crc ^= sub._len_2;
                            state = ParsingState.DATA;
                            break;

                        case ParsingState.DATA:
                            for ( var i = 0; i < sub.LEN; i++ )
                            {
                                var pick = rawPacket[position++];
                                crc ^= pick;
                                sub.DATA.Add( pick );
                            }
                            state = ParsingState.CRC;
                            break;

                        case ParsingState.CRC:
                            sub._crc = rawPacket[position++];
                            // XOR은 결합법칙이 적용되므로, ADDR ~ DATA와 CRC SEED(0x40)가 xor된 crc를 crc2에 xor한다.
                            crc2 ^= crc;
                            crc2 ^= sub._crc;
                            crc ^= Packet._CRC;
                            if ( crc != sub._crc ) errorFlag = true; // CRC 오류
                            else state = ParsingState.ETX;
                            break;

                        case ParsingState.ETX:
                            sub._etx = rawPacket[position++];
                            crc2 ^= sub._etx;
                            if ( sub._etx != Packet._ETX ) errorFlag = true;
                            else state = ParsingState.Complete;
                            break;

                    }
                }
                catch ( IndexOutOfRangeException ) { return state; }
            }

            return state;
        }
    }
}
