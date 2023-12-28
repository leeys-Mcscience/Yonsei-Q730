using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    /// <summary>
    /// 패킷 분석 상태를 나타내는 열거형입니다. 
    /// <br>PacketState는 Bit Flag 연산을 통해 상태 정보를 저장합니다.</br>
    /// <br>패킷 무결성 검사를 통과한 경우 ERRORBIT 비트가 0이며, 패킷에 오류가 있는 경우 ERRORBIT 비트가 1입니다.</br>
    /// <br>PacketState 세부 비트 구조에 대한 설명은 아래 별도 주석을 참고하십시오.</br>
    /// </summary>

    // PacketState 구조
    //  (1)        (16)              (7)                     (8)
    // ┌────┬────────────────┬──────────────────┬───────────────────────────┐
    // │ EB │ ERROR POSITION │ SUB PACKET COUNT │ PACKET ANALYSIS STATEMENT │
    // ├────┼────┬──────┬────┼────┬────────┬────┼────┬─────────────────┬────┤
    // │ 31 │ 30 │ .... │ 15 │ 14 │  ....  │ 08 │ 07 │       ....      │ 00 │
    // └────┴────┴──────┴────┴────┴────────┴────┴────┴─────────────────┴────┘
    // (MSB)                                                            (LSB)
    //
    // EB : 에러 비트 (0 : 에러 없음, 1 : 에러)
    // ERROR POSITION : 에러가 발생한 패킷 바이트 배열의 위치(인덱스) (에러 없을 경우 RawPacket.Length와 같은 값)
    // SUB PACKET COUNT : 정상적으로 분류된 SubPacket 개수
    // PACKET ANALYSIS STATEMENT : 마지막 패킷 분석 상태, 정상 종료시 COMPLETE (= 0x11)

    public enum PacketState : uint
    {
        STX2, ADDR2, CH2, BYPASS, LEN2, DATA2,
        STX, ADDR, CH, CMD,
        /// <summary>
        /// 오류를 의미하는 ERROR가 아닌 패킷의 ERROR/QUERY 비트를 의미한다.
        /// </summary>
        ERROR, LEN, DATA, CRC, ETX,
        CRC2, ETX2,
        COMPLETE,
        ErrorBit = 0x80000000,
        StatementMask = 0x000000FF,
        PositionMask = 0xEFFF8000,
        SubPacketMask = 0x00007F00,
        PoisitionShift = 15,
        SubPacketShift = 8
    };

    public static class CONSTS
    {
        public const uint STX2 = 0xFEFEDCBA;
        public const byte STX2_1 = 0xFE;
        public const byte STX2_2 = 0xFE;
        public const byte STX2_3 = 0xDC;
        public const byte STX2_4 = 0xBA;

        public const ushort ETX2 = 0xF5F5;
        public const byte ETX2_1 = 0xF5;
        public const byte ETX2_2 = 0xF5;

        public const byte CRC2 = 0x10;         // CRC2에서 XOR하는 값
        public const byte STX = 0x02;
        public const byte ETX = 0x03;
        public const byte CRC = 0x40;          // CRC에서 XOR하는 값

        public const int MASTER = -9;
    }

    /// <summary>
    /// 수신 전용 패킷 분석 클래스입니다.
    /// <br>수신된 byte[] 형태의 Raw Packet을 분석하는데 사용합니다.</br>
    /// <br>struct를 사용할 경우 수신되는 패킷이 많아질수록 stack 공간 제약이 있기 때문에 struct가 아니라 class로 생성됩니다.</br>
    /// </summary>
    public class ReceivedPacket
    {
        /// <summary>
        /// 이 패킷에 포함된 서브 패킷 리스트에서 지정된 인덱스 번호의 패킷을 반환합니다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ReceivedSubPacket this[int index] => _subPackets[index];
        /// <summary>
        /// 이 패킷에 포함된 서브 패킷의 개수입니다.
        /// </summary>
        public int Count => _subPackets.Count;
        /// <summary>
        /// 이 패킷의 실제 바이트 수 입니다.
        /// </summary>
        public int Length => _rawPacket.Length;

        /// <summary>
        /// Packet Analyser가 종료된 시점에서의 최종 Statement입니다.
        /// </summary>
        public PacketState PacketState => _packetState & PacketState.StatementMask;
        /// <summary>
        /// Packet Analyser가 종료된 시점에서의 최종 Position입니다.
        /// </summary>
        public ushort ErrorPosition => ( ushort )(( uint )(_packetState & PacketState.PositionMask) >> ( int )PacketState.PoisitionShift);
        /// <summary>
        /// Packet Analyser가 종료된 시점에서의 완료된 Sub Packet Count입니다.
        /// </summary>
        public ushort SubPacketCount => ( ushort )(( uint )(_packetState & PacketState.SubPacketMask) >> ( int )PacketState.SubPacketShift);

        /// <summary>
        /// 패킷 무결성 검사 과정 중 발생하는 가장 최초의 오류 정보를 가집니다.
        /// <br>패킷 분석 과정은 STX2 -> LEN2 -> CRC2 -> SubPackets -> ETX2 순으로 이루어지며, 오류가 발생한 경우 오류 시점 이후의 단계는 수행되지 않습니다.</br>
        /// </summary>
        private byte[] _rawPacket;
        private List<ReceivedSubPacket> _subPackets;
        private PacketState _packetState;


        /// <summary>
        /// State Machine으로 설계된 Packet Analyser를 수행하고, 새로운 Packet 인스턴스를 생성합니다.
        /// <br>생성된 Packet 인스턴스의 PacketState가 PacketState.COMPLETE인 경우 정상 패킷입니다.</br>
        /// </summary>
        /// <param name="rawPacket"></param>
        public ReceivedPacket( byte[] rawPacket )
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

            _subPackets = new List<ReceivedSubPacket>();
            int position = 0;
            int len2 = 0, pMemory = 0;
            PacketState state = PacketState.STX2;
            ReceivedSubPacket subTemp = null;
            byte crc = 0, crc2 = 0;
            byte subNo = 0;

            // Packet Analyser
            while( (state & PacketState.ErrorBit) == 0 && state != PacketState.COMPLETE )
            {
                switch( state )
                {
                    case PacketState.STX2:
                        if( new Q_UInt32( rawPacket[position++], rawPacket[position++], rawPacket[position++], rawPacket[position++] ) != CONSTS.STX2 )
                            state |= PacketState.ErrorBit;
                        else
                            state = PacketState.ADDR2;
                        break;

                    case PacketState.ADDR2:
                        position++;
                        state = PacketState.CH2;
                        break;

                    case PacketState.CH2:
                        position++;
                        state = PacketState.BYPASS;
                        break;

                    case PacketState.BYPASS:
                        position++;
                        state = PacketState.LEN2;
                        break;

                    case PacketState.LEN2:
                        len2 = new Q_UInt16( rawPacket[position++], rawPacket[position++] );
                        state = PacketState.DATA2;
                        pMemory = position; // SubPacket 시작 위치 기억
                        break;

                    case PacketState.DATA2:
                        //len2 -= (position - pMemory);   // position이 이동한 값을 len2에서 뺀다.
                        if( len2 - (position - pMemory) == 0 ) state = PacketState.CRC2;
                        // len2가 딱 0이 되어야 함 (Sub Packet State는 len2만큼 읽도록 설계되었기 때문)
                        else if( len2 < 0 ) state |= PacketState.ErrorBit;
                        else state = PacketState.STX;
                        break;

                    case PacketState.STX:
                        if( rawPacket[position++] != CONSTS.STX ) state |= PacketState.ErrorBit;
                        else
                        {
                            subTemp = new ReceivedSubPacket();
                            state = PacketState.ADDR;
                            crc = 0;
                            crc2 = ( byte )(crc2 ^ CONSTS.STX);
                        }
                        break;

                    case PacketState.ADDR:
                        subTemp.ADDR = rawPacket[position++];
                        crc = ( byte )(crc ^ subTemp.ADDR);
                        state = PacketState.CH;
                        break;

                    case PacketState.CH:
                        subTemp.CH = rawPacket[position++];
                        crc = ( byte )(crc ^ subTemp.CH);
                        state = PacketState.CMD;
                        break;

                    case PacketState.CMD:
                        subTemp.CMD = new Q_UInt16( rawPacket[position++], rawPacket[position++] );
                        crc = ( byte )(crc ^ new Q_UInt16( subTemp.CMD ).Offset0);
                        crc = ( byte )(crc ^ new Q_UInt16( subTemp.CMD ).Offset1);
                        state = PacketState.ERROR;
                        break;

                    case PacketState.ERROR:
                        subTemp.ERROR = rawPacket[position++];
                        crc = ( byte )(crc ^ subTemp.ERROR);
                        state = PacketState.LEN;
                        break;

                    case PacketState.LEN:
                        subTemp.LEN = new Q_UInt16( rawPacket[position++], rawPacket[position++] );
                        crc = ( byte )(crc ^ new Q_UInt16( subTemp.LEN ).Offset0);
                        crc = ( byte )(crc ^ new Q_UInt16( subTemp.LEN ).Offset1);
                        state = PacketState.DATA;
                        break;

                    case PacketState.DATA:
                        for( int i = 0; i < subTemp.LEN; i++ )
                        {
                            byte pick = rawPacket[position++];
                            crc = ( byte )(crc ^ pick);
                            subTemp.DATA.Add( pick );
                        }
                        state = PacketState.CRC;
                        break;

                    case PacketState.CRC:
                        byte c = rawPacket[position++];
                        crc2 = ( byte )(crc2 ^ c);
                        if( ( byte )(crc ^ CONSTS.CRC) != c ) state |= PacketState.ErrorBit;
                        else state = PacketState.ETX;
                        break;

                    case PacketState.ETX:
                        if( rawPacket[position++] != CONSTS.ETX ) state |= PacketState.ErrorBit;
                        else
                        {
                            // 생성된 Sub Packet을 리스트에 넣는다.
                            crc2 = ( byte )(crc2 ^ crc);
                            crc2 = ( byte )(crc2 ^ CONSTS.ETX);
                            _subPackets.Add( subTemp );
                            subNo++;
                            state = PacketState.DATA2;
                        }
                        break;

                    case PacketState.CRC2:
                        if( ( byte )(crc2 ^ CONSTS.CRC2) != rawPacket[position++] ) state |= PacketState.ErrorBit;
                        else state = PacketState.ETX2;
                        break;

                    case PacketState.ETX2:
                        if( new Q_UInt16( rawPacket[position++], rawPacket[position++] ) != CONSTS.ETX2 ) state |= PacketState.ErrorBit;
                        else
                        {
                            position++;
                            state = PacketState.COMPLETE;
                        }
                        break;
                }
            }

            ushort s = subNo;
            state |= ( PacketState )(s << 8);
            state |= ( PacketState )((position - 1) << 15);

            _rawPacket = rawPacket;
            _packetState = state;
        }
    }

    public class ReceivedSubPacket
    {
        public byte ADDR;
        public byte CH;
        public ushort CMD;
        public byte ERROR;
        public ushort LEN;
        public List<byte> DATA = null;

        public byte this[int index] => 0;
        public ReceivedSubPacket()
        {
            DATA = new List<byte>();
        }
    }

    /// <summary>
    /// 발신 전용 패킷 클래스입니다.
    /// </summary>
    public class SendPacket
    {
        public int ChannelNo => _channelNo;
        public byte ADDR2 => _addr2;
        public byte CH2 => _ch2;
        public byte ByPass;
        public List<byte[]> SubPackets => DATA;

        public ushort LEN2
        {
            get
            {
                ushort count = 0;

                foreach( var arr in DATA )
                {
                    count += ( ushort )arr.Length;
                }

                return count;
            }
        }
        public readonly List<byte[]> DATA = new List<byte[]>();

        public byte LEN2_1
        {
            get
            {
                return new Q_UInt16( LEN2 ).Offset0;
            }
        }
        public byte LEN2_2
        {
            get
            {
                return new Q_UInt16( LEN2 ).Offset1;
            }
        }

        public byte CRC2
        {
            get
            {
                byte crc = 0;
                foreach( var s in DATA )
                {
                    foreach( var b in s )
                    {
                        crc ^= b;
                    }
                }

                return (crc ^= CONSTS.CRC2);
            }
        }

        private int _channelNo;
        private byte _addr2;
        private byte _ch2;

        /// <summary>
        /// 송신용 패킷 생성 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="channelNo">Q730 Software 기준의 채널 번호로, 0부터 시작하여 최대 채널 개수 - 1로 끝나는 값입니다.</param>
        public SendPacket( int channelNo )
        {
            _channelNo = channelNo;
            _addr2 = ( byte )(_channelNo / 8 + 1);
            _ch2 = ( byte )(_channelNo % 8 + 1);
        }
        /// <summary>
        /// 송신용 패킷 생성 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="boardNo">Q100 Component 기준의 보드 번호로, 1부터 시작하여 최대 보드 개수 - 1로 끝나는 값입니다. 일반적으로 1 ~ 8입니다.</param>
        /// <param name="channelNo">Q100 Component 기준의 채널 번호로, 1부터 시작하여 보드당 최대 채널 개수 - 1로 끝나는 값입니다. 일반적으로 1 ~ 8입니다.</param>
        public SendPacket( byte boardNo, byte channelNo )
        {
            _addr2 = boardNo;
            _ch2 = channelNo;
            _channelNo = (_addr2 - 1) * 8 + _ch2 - 1;
        }

        // 발신 전용 Raw Packet으로 변환하는 함수
        public byte[] ToRawPacket()
        {
            var arr = new List<byte>();

            arr.Add( CONSTS.STX2_1 );
            arr.Add( CONSTS.STX2_2 );
            arr.Add( CONSTS.STX2_3 );
            arr.Add( CONSTS.STX2_4 );

            arr.Add( ADDR2 );
            arr.Add( CH2 );
            arr.Add( ByPass );

            arr.Add( LEN2_1 );
            arr.Add( LEN2_2 );

            foreach( var s in DATA )
            {
                arr.AddRange( s );
            }

            arr.Add( CRC2 );
            arr.Add( CONSTS.ETX2_1 );
            arr.Add( CONSTS.ETX2_2 );

            return arr.ToArray();
        }
    }
    public class SendSubPacket
    {
        public ushort Command
        {
            get
            {
                var cmd = new Q_UInt16( _cmd1, _cmd2 );
                return cmd.Value;
            }
            set
            {
                var cmd = new Q_UInt16( value );
                _cmd1 = cmd.Offset0;
                _cmd2 = cmd.Offset1;
            }
        }
        /// <summary>
        /// 패킷을 Raw Packet 형태로 변환했을 때의 바이트 수이다.
        /// </summary>
        public int Length
        {
            get
            {
                return 10 + LEN;
            }
        }

        // 이 두 개는 고정이다. 나중에 고정 해제하려면 readonly 키워드만 제거하고 사용할 것.
        public readonly byte ADDR = 0xFF;
        public readonly byte CH = 0xFF;
        public byte CMD1 => _cmd1;
        public byte CMD2 => _cmd2;
        public byte ERR;
        public ushort LEN => ( ushort )(DATA == null ? 0 : DATA.Count);
        private byte LEN1
        {
            get
            {
                var len = new Q_UInt16( LEN );
                return len.Offset0;
            }
        }
        private byte LEN2
        {
            get
            {
                var len = new Q_UInt16( LEN );
                return len.Offset1;
            }
        }
        public readonly List<byte> DATA = new List<byte>();
        public byte CRC
        {
            get
            {
                byte crc = 0;
                // 보내는 패킷의 ADDR과 CH는 항상 0xFF로 고정이기 때문에 xor해도 서로 상쇄되어 굳이 안해도 되긴 함.
                crc ^= ADDR;
                crc ^= CH;
                crc ^= CMD1;
                crc ^= CMD2;
                crc ^= ERR;
                crc ^= LEN1;
                crc ^= LEN2;

                for( var i = 0; i < LEN; i++ ) crc ^= DATA[i];

                return (crc ^= CONSTS.CRC);
            }
        }

        private byte _cmd1;
        private byte _cmd2;

        public byte[] ToRawPacket()
        {
            var arr = new List<byte>();

            arr.Add( CONSTS.STX );
            arr.Add( ADDR );
            arr.Add( CH );
            arr.Add( CMD1 );
            arr.Add( CMD2 );
            arr.Add( ERR );
            arr.Add( LEN1 );
            arr.Add( LEN2 );
            for( var i = 0; i < LEN; i++ ) arr.AddRange( DATA );
            arr.Add( CRC );
            arr.Add( CONSTS.ETX );

            return arr.ToArray();
        }

        /// <summary>
        /// DATA 필드의 길이가 0인 단순 명령 송신용 서브 패킷을 생성하는 생성자입니다.
        /// </summary>
        /// <param name="command"></param>
        public SendSubPacket( ushort command )
        {
            Command = command;
            ERR = 0;
        }
        /// <summary>
        /// CMD1, CMD2, ERR 및 DATA 필드를 사용하여 송신용 서브 패킷을 생성하는 생성자입니다.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="err"></param>
        /// <param name="data"></param>
        public SendSubPacket( ushort command, byte err, byte[] data )
        {
            Command = command;
            ERR = err;
            DATA.AddRange( data );
        }
        public static implicit operator byte[]( SendSubPacket s )
        {
            return s.ToRawPacket();
        }
    }
}
