using System;
using System.Collections.Generic;
using System.Text;
using McQLib;
using McQLib.Device;
using System.IO;
using McQLib.Core;
using System.Threading;
using System.Net;

namespace QComChecker
{
    class Program
    {
        static string log( string msg ) => $"[{DateTime.Now:HH:mm:ss.fff}] {msg}";

        static StreamWriter _writer;
        static int intercepted = 0;
        static void intercept( Packet packet )
        {
            intercepted++;
            _writer.WriteLine( $"Received => {Util.BytesToString( packet.RawPacket )}" );
        }

        static bool checkDevice( Communicator com, StreamWriter writer )
        {
            #region 0. Connection & Ping
            // Connection check
            writer.WriteLine( log( $"Try connect to {com.IP} ..." ) );
            if ( !com.Connect() )
            {
                writer.WriteLine( log( "Connect failed." ) );
                return false;
            }

            writer.WriteLine( log( $"Connected. (Port = {com.Port})" ) );

            Packet send, receive;

            send = new SendPacket( 0, 0 );
            send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.Ping_G ) );
            writer.WriteLine( log( $"Ping request." ) );
            writer.WriteLine( log( $"Send => {Util.BytesToString( send.ToByteArray() )}" ) );
            receive = com.SendAndReceive( send );

            if ( receive == null )
            {
                writer.WriteLine( log( $"Ping(0x0000) no response." ) );
            }
            else
            {
                writer.WriteLine( log( $"Received => {Util.BytesToString( receive.ToByteArray() )}" ) );
                if ( receive.SubPacket.ERR != 0 )
                {
                    writer.WriteLine( log( $"Ping(0x0000) ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2})" ) );
                }
                else
                {
                    writer.WriteLine( log( "Ping responsed." ) );
                }
            }
            #endregion

            #region 1. Get & Set master register
            send = new SendPacket( 0, 0 );
            send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.InitStateResister_GS ) );

            writer.WriteLine( log( $"Get master register ..." ) );
            writer.WriteLine( log( $"Send => {Util.BytesToString( send.ToByteArray() )}" ) );
            receive = com.SendAndReceive( send );

            if ( receive == null )
            {
                writer.WriteLine( log( $"GetRegister(0x0010) no response." ) );
            }
            else
            {
                writer.WriteLine( log( $"Received => {Util.BytesToString( receive.ToByteArray() )}" ) );
                if ( receive.SubPacket.ERR != 0 )
                {
                    writer.WriteLine( log( $"GetRegister(0x0010) ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2})" ) );
                }
                else
                {
                    try
                    {
                        writer.WriteLine( log( $"Master register : 0x{receive.SubPacket.DATA[0]:X2}{receive.SubPacket.DATA[1]:X2}" ) );
                    }
                    catch
                    {
                        writer.WriteLine( log( $"Master register parsing error." ) );
                    }

                    send = new SendPacket( Address.Master );
                    send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.InitStateResister_GS ) );
                    send.SubPacket.ERR = Packet.SET;
                    send.SubPacket.DATA.Add( 0x00, 0x00 );

                    writer.WriteLine( log( $"Init master register ..." ) );
                    writer.WriteLine( log( $"Send => {Util.BytesToString( send.ToByteArray() )}" ) );
                    receive = com.SendAndReceive( send );
                    if ( receive == null )
                    {
                        writer.WriteLine( log( $"InitRegister(0x0010) no response." ) );
                    }
                    else
                    {
                        writer.WriteLine( log( $"Received => {Util.BytesToString( receive.ToByteArray() )}" ) );
                    }
                }
            }
            #endregion

            #region 2. Master board info
            send = new SendPacket( 0, 0 );
            send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.BoardInformation_G ) );

            writer.WriteLine( log( $"Check board information ..." ) );
            writer.WriteLine( log( $"Send => {Util.BytesToString( send.ToByteArray() )}" ) );
            receive = com.SendAndReceive( send );

            if ( receive == null )
            {
                writer.WriteLine( log( $"BoardInformation(0x0002) no response." ) );
                return false;
            }
            else
            {
                writer.WriteLine( log( $"Received => {Util.BytesToString( receive.ToByteArray() )}" ) );
                if ( receive.SubPacket.ERR != 0 )
                {
                    writer.WriteLine( log( $"BoardInformation(0x0002) ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2})" ) );
                }
                else
                {
                    writer.WriteLine( $"== Master board info ==" );

                    int position = 0;
                    byte[] barray;

                    try
                    {
                        barray = new byte[15];
                        for ( var i = 0; i < 15; i++ ) barray[i] = receive.SubPacket.DATA[position++];
                        var model = Encoding.ASCII.GetString( barray );
                        writer.WriteLine( $"\t\tModel : {model}" );

                        barray = new byte[10];
                        for ( var i = 0; i < 10; i++ ) barray[i] = receive.SubPacket.DATA[position++];

                        var serial = Encoding.ASCII.GetString( barray );
                        writer.WriteLine( $"\t\tSerial : {serial}" );

                        var date = new DateTime( receive.SubPacket.DATA[position++] + 2000,
                                                 receive.SubPacket.DATA[position++],
                                                 receive.SubPacket.DATA[position++] );
                        writer.WriteLine( $"\t\tDate : {date:yyyy-MM-dd}" );

                        var firmware = new VersionInfo( receive.SubPacket.DATA[position++], receive.SubPacket.DATA[position++] );
                        writer.WriteLine( $"\t\tFirmware : {firmware}" );

                        var fpga = new VersionInfo( receive.SubPacket.DATA[position++], receive.SubPacket.DATA[position++] );
                        writer.WriteLine( $"\t\tFPGA : {fpga}" );

                        var protocol = new VersionInfo( receive.SubPacket.DATA[position++], receive.SubPacket.DATA[position++], additional: ( ( char )receive.SubPacket.DATA[position++] ).ToString() );
                        writer.WriteLine( $"\t\tProtocol : {protocol}" );

                        var dac = receive.SubPacket.DATA[position++];
                        writer.WriteLine( $"\t\tDAC : {dac}" );

                        var adc = receive.SubPacket.DATA[position++];
                        writer.WriteLine( $"\t\tADC : {adc}" );

                        var auxadc = receive.SubPacket.DATA[position++];
                        writer.WriteLine( $"\t\tAUXADC : {auxadc}" );
                    }
                    catch
                    {
                        writer.WriteLine( log( $"Board information parsing error." ) );
                    }
                }
            }
            writer.WriteLine();
            #endregion

            #region 3. Slave board info
            var channels = new List<Address>();

            send = new SendPacket( 0, 0 );
            send.SubPackets.Add( new SendSubPacket( Commands.MultiChannelCommands.M_BoardSearching_GS ) );

            writer.WriteLine( log( $"Check slave boards ..." ) );
            writer.WriteLine( log( $"Send => {Util.BytesToString( send.ToByteArray() )}" ) );

            int tryCount = 0;
            do
            {
                receive = com.SendAndReceive( send );

                if ( receive == null )
                {
                    writer.WriteLine( log( $"BoardSearching(0x0B01) no response." ) );
                    break;
                }
                // 아직 Board Research가 진행중인 경우
                else if ( receive.SubPacket.DATA[0] == 1 )
                {
                    // 1초간 대기 후 패킷을 다시 송신한다.
                    Thread.Sleep( 1000 );
                    tryCount++;
                }
                else break;
            } while ( tryCount < 5 );    // 재탐색 진행중 대기를 최대 5회까지 수행

            if ( receive == null || tryCount == 5 )
            {
                writer.WriteLine( log( $"Check slave boards failed. (Cause: Try count ({tryCount}))" ) );
                return false;
            }
            else
            {
                writer.WriteLine( log( $"Received => {Util.BytesToString( receive.ToByteArray() )}" ) );
                if ( receive.SubPacket.ERR != 0 )
                {
                    writer.WriteLine( log( $"BoardSearching(0x0B01) ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2})" ) );
                }
                else
                {
                    try
                    {
                        writer.WriteLine( "=== Slave board states ===" );

                        var realChannelCount = new Q_UInt16( receive.SubPacket.DATA[1], receive.SubPacket.DATA[2] ).Value;
                        writer.WriteLine( $"Detected channels: {realChannelCount}" );

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
                                    state = State.DISCONNECTED;
                                    break;
                            }

                            writer.WriteLine( $"Channel[{addr},{ch}] = {state}" );
                            channels.Add( new Address( addr, ch ) );
                        }
                    }
                    catch
                    {
                        writer.WriteLine( log( $"Slave board information parsing error." ) );
                    }
                }
            }

            writer.WriteLine();
            #endregion

            #region 4. Get & Set channel register & Ping
            for ( var i = 0; i < channels.Count; i++ )
            {
                send = new SendPacket( channels[i].ADDR, channels[i].CH );
                send.ByPass = Packet.ON;
                send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.InitStateResister_GS ) );

                writer.WriteLine( log( $"Get channel[{channels[i].ADDR},{channels[i].CH}] register ..." ) );
                writer.WriteLine( log( $"Send => {Util.BytesToString( send.ToByteArray() )}" ) );
                receive = com.SendAndReceive( send );

                if ( receive == null )
                {
                    writer.WriteLine( log( $"GetRegister(0x0010) no response." ) );
                    continue;
                }
                else
                {
                    writer.WriteLine( log( $"Received => {Util.BytesToString( receive.ToByteArray() )}" ) );
                    if ( receive.SubPacket.ERR != 0 )
                    {
                        writer.WriteLine( log( $"GetRegister(0x0010) ERR => {( ErrorField )receive.SubPacket.ERR}(0x{receive.SubPacket.ERR:X2})" ) );
                    }
                    else
                    {
                        try
                        {
                            writer.WriteLine( $"Channel[{channels[i].ADDR},{channels[i].CH}] register : 0x{receive.SubPacket.DATA[0]:X2}{receive.SubPacket.DATA[1]:X2}" );
                        }
                        catch
                        {
                            writer.WriteLine( log( $"Master register parsing error." ) );
                        }
                    }
                }

                send = new SendPacket( channels[i].ADDR, channels[i].CH );
                send.ByPass = Packet.ON;
                send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.InitStateResister_GS ) );
                send.SubPacket.ERR = Packet.SET;
                send.SubPacket.DATA.Add( 0x00, 0x00 );

                writer.WriteLine( log( $"Init channel[{channels[i].ADDR},{channels[i].CH}] register ..." ) );
                writer.WriteLine( log( $"Send => {Util.BytesToString( send.ToByteArray() )}" ) );
                receive = com.SendAndReceive( send );
                if ( receive == null )
                {
                    writer.WriteLine( log( $"InitRegister(0x0010) no response." ) );
                    continue;
                }
                else
                {
                    writer.WriteLine( log( $"Received => {Util.BytesToString( receive.ToByteArray() )}" ) );
                }

                send = new SendPacket( channels[i].ADDR, channels[i].CH );
                send.ByPass = Packet.ON;
                send.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.Ping_G ) );

                writer.WriteLine( log( $"Channel[{channels[i].ADDR},{channels[i].CH}] ping request." ) );
                writer.WriteLine( log( $"Send => {Util.BytesToString( send.ToByteArray() )}" ) );
                receive = com.SendAndReceive( send );
                if ( receive == null )
                {
                    writer.WriteLine( log( $"Ping(0x0000) no response." ) );
                    continue;
                }
                else
                {
                    writer.WriteLine( log( $"Received => {Util.BytesToString( receive.ToByteArray() )}" ) );
                    writer.WriteLine( log( $"Ping responsed." ) );
                }
                writer.WriteLine();
            }
            #endregion

            #region 5. Remain data check
            writer.WriteLine( log( $"Remain data checking ..." ) );
            _writer = writer;
            intercepted = 0;
            com.Intercept = intercept;
            Util.CallMethod( com, "SetTransmissionControl", true );
            //com.SetTransmissionControl( true );

            Thread.Sleep( 1000 * 10 );

            Util.CallMethod( com, "SetTransmissionControl", false );

            //com.SetTransmissionControl( false );
            Util.CallMethod( com, "RemoveSavedData" );

            //com.RemoveSavedData();
            com.Intercept = null;
            _writer = null;

            writer.WriteLine( log( $"Remain data : {intercepted}" ) );
            writer.WriteLine();
            #endregion

            #region 6. Disconnection
            writer.WriteLine( log( $"Disconnect ..." ) );
            com.Disconnect();
            writer.WriteLine( log( $"Done." ) );
            #endregion

            return true;
        }

        static void Main( string[] args )
        {
            Console.WriteLine( "Check device systems..." );

            using ( var writer = new StreamWriter( $"CheckLog_{DateTime.Now:yyyyMMdd_HHmmss}.log" ) )
            {
                if ( !File.Exists( "ip.txt" ) )
                {
                    writer.WriteLine( log( $"Cannot found IP info." ) );
                    return;
                }

                var ip = new List<string>();
                var count = new List<int>();

                using ( var reader = new StreamReader( "ip.txt" ) )
                {
                    var line = reader.ReadToEnd().Split( Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

                    for ( var i = 0; i < line.Length; i++ )
                    {
                        var split = line[i].Split( ',' );
                        if ( split.Length == 2 && IPAddress.TryParse( split[0], out IPAddress dummy ) && int.TryParse( split[1], out int c ) )
                        {
                            ip.Add( split[0] );
                            count.Add( c );
                        }
                    }
                }

                for ( var i = 0; i < ip.Count; i++ )
                {
                    var com = new Communicator( i, count[i] )
                    {
                        IP = ip[i]
                    };

                    try
                    {
                        Console.Write( $"Device [{i + 1}] check..." );

                        if (checkDevice( com, writer ) )
                        {
                            Console.WriteLine( $" Done. (No error)" );
                        }
                        else
                        {
                            Console.WriteLine( $" Failed. (Error occured)" );
                        }
                    }
                    catch ( Exception ex )
                    {
                        Console.WriteLine( $" Failed. (Exception)" );
                        writer.WriteLine( log( $"Unexpected exception thrown : {ex.Message}" ) );
                    }
                }

                Console.WriteLine( "All Done." );
                Console.Write( "Press any key to exit..." );
                Console.ReadKey();
            }
        }
    }
}
