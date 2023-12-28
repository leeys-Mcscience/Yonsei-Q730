using McQLib.Device;
using McQLib.Core;

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Drawing;

namespace PacketParser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var regex = new Regex( "[-][0-9][0-9][-][0-9]" );
            var match = regex.Match( "220325 #5-11-1" );
            //bool t = regex.IsMatch( "[13:50:03.491] SendPacket(Complete) " );
            //var match = regex.Match( "zz zz [13:50:03.491] SendPacket(Complete) [13:50:03.491] SendPacket(Complete) " );

            //t = dateTime.IsMatch( "[2021-11-09 12:59:18.675]  0000036539   sender - data send ok rcv" );
            init();
        }

        public bool IsOpen
        {
            get
            {
                foreach ( Form f in Application.OpenForms )
                {
                    if ( f == this ) return true;
                }

                return false;
            }
        }
        public void TryParse( string source )
        {
            richTextBox1.Text = source;
            button1.PerformClick();
        }
        public string ParsingText
        {
            get => richTextBox1.Text;
            set => richTextBox1.Text = value;
        }

        private void init()
        {
            richTextBox1.LanguageOption = 0;
            richTextBox2.LanguageOption = 0;
            richTextBox3.LanguageOption = 0;

            //toolTip1.SetToolTip( checkBox_AutoCrawling, "텍스트박스에서 드래그한 내용을 자동으로 Raw Bytes 필드로 가져오는 기능입니다." );
            toolTip1.SetToolTip( checkBox_AddZero, "Raw Bytes 필드의 문자 수가 홀수개일 때 자동으로 맨 앞에 0을 추가하는 기능입니다." );
            toolTip1.SetToolTip( textBox_Seed, "비트 연산에 특정한 바이트를 고정으로 참가시키려면 이 필드에 추가하십시오." );
            toolTip1.SetToolTip( button2, "위쪽 텍스트박스에서 16진수 숫자를 제외한 모든 문자를 제거합니다." );
            comboBox1.SelectedIndex = 2;

            listView1.GetType().GetProperty( "DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic ).SetValue( listView1, true );
            richTextBox2.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic ).SetValue( richTextBox2, true );
        }

        private void button1_Click( object sender, EventArgs e )
        {
            richTextBox2.Clear();
            richTextBox3.Clear();

            if ( richTextBox1.Text.Trim().Length == 0 ) return;

            var str = richTextBox1.Text.Replace( " ", "" );

            var p = new List<byte>();

            try
            {
                for ( var i = 0; i < str.Length - 1; i += 2 )
                {
                    p.Add( Convert.ToByte( $"{str[i]}{str[i + 1]}", 16 ) );
                }
            }
            catch ( Exception )
            {
                MessageBox.Show( "Raw Packet에 byte 형식으로 변환할 수 없는 문자가 포함되어 있습니다.", "변환 실패" );
                return;
            }

            var watch = new Stopwatch();
            // Protocol2 로 파싱 시도
            watch.Start();
            var packet = ReceivedPacket.Parse( p.ToArray() );
            watch.Stop();

            // 만약 STX2 단계에서 파싱 종료된 경우 Protocol1 패킷일 수도 있다.
            if ( packet.ParsingState == ParsingState.STX2 )
            {
                ReceivedSubPacket sub;
                watch.Reset();
                watch.Start();
                var state = ReceivedSubPacket.Parse( p.ToArray(), out sub );
                watch.Stop();

                // 그런 경우 Protocol1 패킷 정보만 출력하고 종료
                if ( state == ParsingState.Complete )
                {
                    richTextBox2.AppendText( "이 패킷은 통합프로토콜1 패킷입니다." );
                    richTextBox2.AppendText( $"\r\n-- Start of Protocol1 Packet --\r\n" +
                                        $"STX : \t\t0x {sub.STX:X2}\r\n" +
                                        $"ADDR : \t0x {sub.ADDR:X2}\r\n" +
                                        $"CH : \t\t0x {sub.CH:X2}\r\n" +
                                        $"CMD : \t0x {sub.CMD_1:X2} {sub.CMD_2:X2} ({sub.Command})\r\n" +
                                        $"ERR : \t\t0x {sub.ERR:X2}\r\n" +
                                        $"LEN : \t\t0x {sub.LEN_1:X2} {sub.LEN_2:X2} (={sub.LEN})\r\n" +
                                        $"DATA : " );
                    if ( sub.DATA.Count != 0 ) richTextBox2.AppendText( "\t0x " );
                    for ( var j = 0; j < sub.DATA.Count; j++ ) richTextBox2.AppendText( $"{sub.DATA[j]:X2} " );
                    richTextBox2.AppendText( $"\r\nCRC : \t\t0x {sub.CRC:X2}\r\n" +
                                             $"ETX : \t\t0x {sub.ETX:X2}\r\n" +
                                             $"-- End of Protocol1 Packet --\r\n" );

                    var tmp = new SendPacket( new Address( sub.ADDR, sub.CH ) );
                    tmp.SubPackets.Add( sub );
                    showDetail( tmp );

                    return;
                }
                else if ( state == ParsingState.CRC )
                {
                    richTextBox2.AppendText( "이 패킷은 통합프로토콜1 패킷으로 추정됩니다." );
                    richTextBox2.AppendText( $"\r\n-- Start of Protocol1 Packet --\r\n" +
                                        $"STX : \t\t0x {sub.STX:X2}\r\n" +
                                        $"ADDR : \t0x {sub.ADDR:X2}\r\n" +
                                        $"CH : \t\t0x {sub.CH:X2}\r\n" +
                                        $"CMD : \t0x {sub.CMD_1:X2} {sub.CMD_2:X2} ({sub.Command})\r\n" +
                                        $"ERR : \t\t0x {sub.ERR:X2}\r\n" +
                                        $"LEN : \t\t0x {sub.LEN_1:X2} {sub.LEN_2:X2} (={sub.LEN})\r\n" +
                                        $"DATA : " );
                    if ( sub.DATA.Count != 0 ) richTextBox2.AppendText( "\t0x " );
                    for ( var j = 0; j < sub.DATA.Count; j++ ) richTextBox2.AppendText( $"{sub.DATA[j]:X2} " );
                    richTextBox2.AppendText( $"\r\nCRC : \t\t0x {sub.CRC:X2}\r\n" +
                                             $"ETX : \t\t0x {sub.ETX:X2}\r\n" +
                                             $"-- End of Protocol1 Packet --\r\n" );
                    return;
                }
            }

            if ( packet.ParsingState == ParsingState.Complete ) richTextBox2.AppendText( "이 패킷은 통합프로토콜2 패킷입니다.\r\n" );
            else richTextBox2.AppendText( "이 패킷은 정상 패킷이 아닙니다.\r\n" );

            richTextBox2.AppendText( $"Parsing State : {packet.ParsingState}\r\n" +
                                    $"== Start of Protocol2 Packet ==\r\n" +
                                    $"STX2 : \t\t0x {packet.STX2_1:X2} {packet.STX2_2:X2} {packet.STX2_3:X2} {packet.STX2_4:X2}\r\n" +
                                    $"ADDR2 : \t0x {packet.ADDR2:X2}\r\n" +
                                    $"CH2 : \t\t0x {packet.CH2:X2}\r\n" +
                                    $"ByPass : \t0x {packet.ByPass:X2}\r\n" +
                                    $"LEN2 : \t\t0x {packet.LEN2_1:X2} {packet.LEN2_2:X2} (={packet.LEN2})\r\n" );

            for ( var i = 0; i < packet.SubPackets.Count; i++ )
            {
                richTextBox2.AppendText( $"\r\n-- Start of Protocol1 Packet[{i}] --\r\n" +
                                        $"STX : \t\t0x {packet[i].STX:X2}\r\n" +
                                        $"ADDR : \t0x {packet[i].ADDR:X2}\r\n" +
                                        $"CH : \t\t0x {packet[i].CH:X2}\r\n" +
                                        $"CMD : \t0x {packet[i].CMD_1:X2} {packet[i].CMD_2:X2} ({packet[i].Command})\r\n" +
                                        $"ERR : \t\t0x {packet[i].ERR:X2}\r\n" +
                                        $"LEN : \t\t0x {packet[i].LEN_1:X2} {packet[i].LEN_2:X2} (={packet[i].LEN})\r\n" +
                                        $"DATA : " );

                if ( packet[i].DATA.Count != 0 ) richTextBox2.AppendText( "\t0x " );
                for ( var j = 0; j < packet[i].DATA.Count; j++ ) richTextBox2.AppendText( $"{packet[i].DATA[j]:X2} " );
                richTextBox2.AppendText( $"\r\nCRC : \t\t0x {packet[i].CRC:X2}\r\n" +
                                            $"ETX : \t\t0x {packet[i].ETX:X2}\r\n" +
                                            $"-- End of Protocol1 Packet[{i}] --\r\n" );
            }

            richTextBox2.AppendText( $"\r\nCRC2 : \t0x {packet.CRC2:X2}\r\n" +
                                    $"ETX2 : \t\t0x {packet.ETX2_1:X2} {packet.ETX2_2:X2}\r\n" +
                                    $"== End of Protocol2 Packet ==" );

            if ( packet.ErrorPosition < packet.RawPacket.Length )
            {
                richTextBox2.AppendText( "\r\n패킷의 ETX2까지 정상적으로 확인하였으나, 추가 바이트가 존재합니다.\r\n\t\t0x " );
                for ( var i = packet.ErrorPosition; i < packet.RawPacket.Length; i++ )
                {
                    richTextBox2.AppendText( $"{packet.RawPacket[i]:X2} " );
                }
            }

            richTextBox2.AppendText( $"\r\n경과 시간 : {watch.ElapsedMilliseconds}ms\r\n" );

            showDetail( packet );
        }

        private void showDetail( Packet packet )
        {
            // Details
            if ( packet.ParsingState == ParsingState.Complete )
            {
                if ( packet.SubPackets.Count >= 1 )
                {
                    var po = 0;
                    switch ( packet[0].Command )
                    {
                        case Commands.CommonCommands.BoardInformation_G:
                            richTextBox3.AppendText( "== 보드 정보 ==\r\n" );

                            richTextBox3.AppendText( $"Model : {( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}" +
                                                             $"{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}" +
                                                             $"{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}\r\n" +
                                                    $"Serial : {( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}" +
                                                             $"{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}\r\n" +
                                                    $"DateTime : {new DateTime( packet[0].DATA[po++] + 2000, packet[0].DATA[po++], packet[0].DATA[po++] ):yyyy-MM-dd}\r\n" +
                                                    $"Firmware Version : v{packet[0].DATA[po++]}.{packet[0].DATA[po++]}\r\n" +
                                                    $"FPGA Version : v{packet[0].DATA[po++]}.{packet[0].DATA[po++]}\r\n" +
                                                    $"Protocol Version : v{packet[0].DATA[po++]}.{packet[0].DATA[po++]}{( char )packet[0].DATA[po++]}\r\n" +
                                                    $"DAC Count : {packet[0].DATA[po++]}\r\n" +
                                                    $"ADC Count : {packet[0].DATA[po++]}\r\n" +
                                                    $"AUX ADC Count : {packet[0].DATA[po++]}\r\n" );
                            break;

                        case Commands.BatteryCycler_SetGetCommands.SequenceStep_GS:
                        case Commands.BatteryCycler_SetGetCommands.SequenceChargeDischarge_GS:
                        case Commands.BatteryCycler_SetGetCommands.SequenceMeasurement_GS:
                        case Commands.BatteryCycler_SetGetCommands.SequencePattern_GS:
                            if ( packet[0].ERR != 1 ) break;

                            while ( true )
                            {
                                richTextBox3.AppendText( "== 시퀀스 데이터 ==\r\n" );

                                richTextBox3.AppendText( $"Reserved : \t0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"Step count : 0x {packet[0].DATA[po++]} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"Step no. : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"Cycle no. : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"Mode1 : 0x {packet[0].DATA[po++]:X2} ({( Mode1 )packet[0].DATA[po - 1]})\r\n" +
                                                        $"Mode2 : 0x {packet[0].DATA[po++]:X2} ({( Mode2 )packet[0].DATA[po - 1]})\r\n" );
                                richTextBox3.AppendText( "\r\n-- 설정 조건 --\r\n" );
                                richTextBox3.AppendText( $"설정 사용 항목 : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" );

                                byte condition = 0;
                                // 설정 조건에 대한 출력 부분만 구분하여 처리
                                switch ( packet[0].Command )
                                {
                                    case Commands.BatteryCycler_SetGetCommands.SequenceStep_GS:
                                        // 스탭 - 설정값 없음
                                        richTextBox3.AppendText( $"Reserved : 0x " );
                                        for ( var i = 0; i < 64; i++ )
                                            richTextBox3.AppendText( $"{packet[0].DATA[po++]:X2} " );
                                        richTextBox3.AppendText( $"\r\n" );
                                        break;

                                    case Commands.BatteryCycler_SetGetCommands.SequenceChargeDischarge_GS:
                                        // 충방전
                                        condition = packet[0].DATA[po - 1];
                                        richTextBox3.AppendText( $"설정 V  : {( ( condition & 0b10000000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 I/설정L-I : {( ( condition & 0b01000000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"변환 V : {( ( condition & 0b00100000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 P : {( ( condition & 0b00010000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 R : {( ( condition & 0b00001000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 H-I : {( ( condition & 0b00000100 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 Freq. : {( ( condition & 0b00000010 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 Duty : {( ( condition & 0b00000001 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );

                                        richTextBox3.AppendText( $"\r\n설정 V : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                 $"설정 I/L-I : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                 $"변환 V : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                 $"설정 P : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                 $"설정 R : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                 $"설정 H-I : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                 $"설정 Freq. : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                 $"설정 Duty : 0x {packet[0].DATA[po++]:X2} \r\n" +
                                                                 $"설정 mode : 0x {packet[0].DATA[po++]:X2}\r\n" );

                                        richTextBox3.AppendText( $"Reserved : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" );
                                        break;

                                    case Commands.BatteryCycler_SetGetCommands.SequenceMeasurement_GS:
                                        // 측정
                                        condition = packet[0].DATA[po - 1];
                                        richTextBox3.AppendText( $"설정 bias  : {( ( condition & 0b10000000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 low amp : {( ( condition & 0b01000000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 high amp : {( ( condition & 0b00100000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 start freq : {( ( condition & 0b00010000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 end freq : {( ( condition & 0b00001000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 mode select : {( ( condition & 0b00000100 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 step count : {( ( condition & 0b00000010 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 sampling 수행 시간 : {( ( condition & 0b00000001 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );

                                        richTextBox3.AppendText( $"\r\n설정 bias(FRA)/설정 3rd Val(DCR) : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                $"설정 low amp(FRA/TRA)/설정 1st Val(DCR) : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                $"설정 high amp(TRA)/설정 2nd Val(DCR) : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                $"설정 start freq(FRA)/설정 delay(TRA/DCR) : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                $"설정 end freq(FRA)/설정 width(TRA/DCR) : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                $"설정 mode select : 0x {packet[0].DATA[po++]:X2}\r\n" +
                                                                $"설정 step count(FRA)/설정 측정 transition(TRA) : 0x {packet[0].DATA[po++]:X2}\r\n" +
                                                                $"설정 sampling 수행 시간 : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                $"설정 delay(DCR) : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                $"설정 width(DCR) : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                                $"설정 raw data mode(FRA) : 0x {packet[0].DATA[po++]:X2}\r\n" +
                                                                $"설정 증폭 배율(FRA, TRA, DCR, ACR) : 0x {packet[0].DATA[po++]:X2}\r\n" );

                                        richTextBox3.AppendText( $"Reserved : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" );
                                        break;

                                    case Commands.BatteryCycler_SetGetCommands.SequencePattern_GS:
                                        // 패턴
                                        condition = packet[0].DATA[po - 1];
                                        richTextBox3.AppendText( $"설정 mode  : {( ( condition & 0b10000000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 time resolution : {( ( condition & 0b01000000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                        richTextBox3.AppendText( $"설정 총 데이터 개수 : {( ( condition & 0b00100000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );

                                        richTextBox3.AppendText( $"\r\n설정 mode : 0x {packet[0].DATA[po++]:X2}\r\n" +
                                                                $"설정 time resolution : 0x {packet[0].DATA[po++]:X2}\r\n" +
                                                                $"설정 총 데이터 개수 : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" );

                                        richTextBox3.AppendText( $"Reserved : 0x" );
                                        for ( var i = 0; i < 60; i++ ) richTextBox3.AppendText( $" {packet[0].DATA[po++]:X2}" );
                                        richTextBox3.AppendText( "\r\n" );
                                        break;
                                }

                                richTextBox3.AppendText( "\r\n-- 안전 조건 --\r\n" );
                                richTextBox3.AppendText( $"안전 사용 항목 : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" );

                                condition = packet[0].DATA[po - 2];
                                richTextBox3.AppendText( $"안전 최대 V : {( ( condition & 0b10000000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"안전 최소 V : {( ( condition & 0b01000000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"안전 최대 I : {( ( condition & 0b00100000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"안전 최소 I : {( ( condition & 0b00010000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"안전 최대 용량 : {( ( condition & 0b00001000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"안전 최대 Wh : {( ( condition & 0b00000100 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"안전 최대 Temp : {( ( condition & 0b00000010 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"안전 최소 Temp : {( ( condition & 0b00000001 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );

                                richTextBox3.AppendText( $"\r\n안전 최대 V : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"안전 최소 V : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"안전 최대 I : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"안전 최소 I : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"안전 최대 용량 : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"안전 최대 Wh : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"안전 최대 Temp : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"안전 최소 Temp : 0x {packet[0].DATA[po++]:X2}{packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" );

                                richTextBox3.AppendText( "\r\n-- 종료 조건 --\r\n" );
                                richTextBox3.AppendText( $"종료 사용 항목 : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" );

                                condition = packet[0].DATA[po - 2];
                                richTextBox3.AppendText( $"종료 V : {( ( condition & 0b10000000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"종료 I : {( ( condition & 0b01000000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"종료 Time : {( ( condition & 0b00100000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"종료 CV Time : {( ( condition & 0b00010000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"종료 용량 : {( ( condition & 0b00001000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"종료 P : {( ( condition & 0b00000100 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"종료 Wh : {( ( condition & 0b00000010 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"종료 delta-V : {( ( condition & 0b00000001 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );

                                condition = packet[0].DATA[po - 1];
                                richTextBox3.AppendText( $"종료 delta-Temp : {( ( condition & 0b10000000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"종료 Temp : {( ( condition & 0b01000000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"종료 Max 용량 비율 : {( ( condition & 0b00100000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );

                                richTextBox3.AppendText( $"\r\n종료 V : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"종료 I : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"종료 Time : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"종료 CV Time : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"종료 용량 : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"종료 P : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"종료 Wh : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"종료 delta-V : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"종료 delta-Temp : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"종료 Temp : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"종료 Max 용량 비율 Monitor Step count : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"종료 Max 용량 비율 : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" );

                                richTextBox3.AppendText( "\r\n-- 기록 조건 --\r\n" );
                                richTextBox3.AppendText( $"기록 사용 항목 : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" );

                                condition = packet[0].DATA[po - 2];
                                richTextBox3.AppendText( $"기록 Interval : {( ( condition & 0b10000000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"기록 delta-V : {( ( condition & 0b01000000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"기록 delta-I : {( ( condition & 0b00100000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );
                                richTextBox3.AppendText( $"기록 delta-Temp : {( ( condition & 0b00010000 ) != 0 ? "사용" : "사용 안 함" )}\r\n" );

                                richTextBox3.AppendText( $"\r\n기록 Interval : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"기록 delta-V : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"기록 delta-I : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                        $"기록 delta-Temp : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" );

                                richTextBox3.AppendText( $"\r\n정상 종료시 이동 Step : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                         $"비정상 종료시 이동 Step : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                         $"반복 횟수 : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                         $"스텝 진행 : 0x {packet[0].DATA[po++]:X2}\r\n" +
                                                         $"Reserved : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" );

                                if ( packet[0].DATA.Count == po ) break;
                                else
                                {
                                    richTextBox3.AppendText( $"\r\n" );
                                }
                            }
                            break;

                        case Commands.BatteryCycler_GetMeasureCommands.ChannelSequenceData_R:
                            // 채널 측정 데이터
                            try
                            {
                                richTextBox3.AppendText( "== 채널별 측정 데이터 ==\r\n" );
                                richTextBox3.AppendText( $"포함된 채널 데이터 수 : {packet.SubPackets.Count}개\r\n\r\n" );

                                for ( var i = 0; i < packet.SubPackets.Count; i++ )
                                {
                                    richTextBox3.AppendText( $"-- 채널 {Util.GetIndex( packet[i].ADDR, packet[i].CH )} (0x{packet[i].ADDR:X2} 0x{packet[i].CH:X2}) --\r\n" );
                                    var d = new ChannelSequenceData( packet[i].DATA );
                                    richTextBox3.AppendText( $"Slave Alarm status : {d.SlaveAlarmStatus}\r\n" );
                                    richTextBox3.AppendText( $"Channel state : {d.ChannelState}\r\n" );
                                    for ( var j = 0; j < d.Count; j++ )
                                    {
                                        richTextBox3.AppendText( $"Total time : {d[j].TotalTime}\r\n" +
                                                                $"Step count : {d[j].StepCount}\r\n" +
                                                                $"Step number : {d[j].StepNumber}\r\n" +
                                                                $"Cycle number : {d[j].CycleCount}\r\n" +
                                                                $"Mode1 : {d[j].Mode1}\r\n" +
                                                                $"Mode2 : {d[j].Mode2}\r\n" +
                                                                $"Index : {d[j].DataIndex}\r\n" +
                                                                $"Voltage : {d[j].Voltage}\r\n" +
                                                                $"Current : {d[j].Current}\r\n" +
                                                                $"Temperature : {d[j].Temperature}\r\n" +
                                                                $"Capacity : {d[j].Capacity}\r\n" +
                                                                $"Power : {d[j].Power}\r\n" +
                                                                $"WattHour : {d[j].WattHour}\r\n" +
                                                                $"Delta-V : {d[j].DeltaV}\r\n" +
                                                                $"Delta-I : {d[j].DeltaI}\r\n" +
                                                                $"Delta-T : {d[j].DeltaT}\r\n" +
                                                                $"Stopped By : {d[j].StoppedType}\r\n" +
                                                                $"Total time overflow : {d[j].TotalTimeOverflow}\r\n\r\n" );
                                    }
                                }
                            }
                            catch ( Exception )
                            {
                                richTextBox3.AppendText( "Detail 분석 중 오류가 발생했습니다. DATA 필드의 구조가 올바르지 않거나, ChannelSequenceData_R 명령의 ACK 패킷일 수 있습니다." );
                            }
                            break;

                        case Commands.BatteryCycler_GetMeasureCommands.ChannelState_G:
                            richTextBox3.AppendText( "== 채널 상태 조회 ==\r\n" );
                            richTextBox3.AppendText( $"총 채널 수 : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"Reserved : 0x {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"동작 상태 : 0x {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"총 수행 시간 : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"Step count : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"설정 Step number : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"Cycle count : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"Sequence Step 상태 : 0x {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"Sequence Step mode : 0x {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"종료된 조건 : 0x {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"총 수행 시간 초과 횟수 : 0x {packet[0].DATA[po++]:X2}" );
                            break;

                        case Commands.BatteryCycler_SetGetCommands.SequencePatternData_GS:
                            richTextBox3.AppendText( "== 패턴 데이터 ==\r\n" );
                            richTextBox3.AppendText( $"Reserved : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"Step count(Reserved) : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"Step no : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"Cycle no(Reserved) : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"Mode1 : 0x {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"Mode2 : 0x {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"설정 데이터 시작 Index : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" +
                                                    $"Reserved : 0x" );
                            for ( var i = 0; i < 28; i++ ) richTextBox3.AppendText( $" {packet[0].DATA[po++]:X2}" );
                            richTextBox3.AppendText( "\r\n" );
                            for ( var i = 0; i < 25; i++ ) richTextBox3.AppendText( $"{i}번 데이터 : 0x {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2} {packet[0].DATA[po++]:X2}\r\n" );

                            richTextBox3.AppendText( "Reserved : 0x" );
                            for ( var i = 0; i < 16; i++ ) richTextBox3.AppendText( $" {packet[0].DATA[po++]:X2}" );
                            richTextBox3.AppendText( "\r\n" );
                            break;

                        case Commands.BatteryCycler_GetMeasureCommands.M_SavedSequenceData_R:
                            richTextBox3.AppendText( "== 저장된 시퀀스 데이터 ==\r\n" );
                            richTextBox3.AppendText( "Unmasked data fields.\r\n" );

                            var maskedString = "";
                            for ( var i = 0; i < packet[0].DATA.Count; i++ ) maskedString += ( ( byte )( packet[0].DATA[i] ^ 0x80 ) ).ToString( "X2" ) + " ";

                            richTextBox3.AppendText( maskedString );

                            richTextBox3.Text = richTextBox3.Text.Replace( "FE FE DC BA", "\r\nFE FE DC BA" );
                            break;


                        default:
                            richTextBox3.AppendText( "이곳에 표시할 내용이 없습니다." );
                            break;
                    }
                }
            }
            else
            {
                richTextBox3.AppendText( "이곳에 표시할 내용이 없습니다." );
            }
        }
        
        private void printSequenceData( Packet packet )
        {
            // 채널 측정 데이터
            try
            {
                richTextBox3.AppendText( "== 채널별 측정 데이터 ==\r\n" );
                richTextBox3.AppendText( $"포함된 채널 데이터 수 : {packet.SubPackets.Count}개\r\n\r\n" );

                for ( var i = 0; i < packet.SubPackets.Count; i++ )
                {
                    richTextBox3.AppendText( $"-- 채널 {Util.GetIndex( packet[i].ADDR, packet[i].CH )} (0x{packet[i].ADDR:X2} 0x{packet[i].CH:X2}) --\r\n" );
                    var d = new ChannelSequenceData( packet[i].DATA );
                    richTextBox3.AppendText( $"Slave Alarm status : {d.SlaveAlarmStatus}\r\n" );
                    richTextBox3.AppendText( $"Channel state : {d.ChannelState}\r\n" );
                    for ( var j = 0; j < d.Count; j++ )
                    {
                        richTextBox3.AppendText( $"Total time : {d[j].TotalTime}\r\n" +
                                                $"Step count : {d[j].StepCount}\r\n" +
                                                $"Step number : {d[j].StepNumber}\r\n" +
                                                $"Cycle number : {d[j].CycleCount}\r\n" +
                                                $"Mode1 : {d[j].Mode1}\r\n" +
                                                $"Mode2 : {d[j].Mode2}\r\n" +
                                                $"Index : {d[j].DataIndex}\r\n" +
                                                $"Voltage : {d[j].Voltage}\r\n" +
                                                $"Current : {d[j].Current}\r\n" +
                                                $"Temperature : {d[j].Temperature}\r\n" +
                                                $"Capacity : {d[j].Capacity}\r\n" +
                                                $"Power : {d[j].Power}\r\n" +
                                                $"WattHour : {d[j].WattHour}\r\n" +
                                                $"Delta-V : {d[j].DeltaV}\r\n" +
                                                $"Delta-I : {d[j].DeltaI}\r\n" +
                                                $"Delta-T : {d[j].DeltaT}\r\n" +
                                                $"Stopped By : {d[j].StoppedType}\r\n" +
                                                $"Total time overflow : {d[j].TotalTimeOverflow}\r\n\r\n" );
                    }
                }
            }
            catch ( Exception )
            {
                richTextBox3.AppendText( "Detail 분석 중 오류가 발생했습니다. DATA 필드의 구조가 올바르지 않거나, ChannelSequenceData_R 명령의 ACK 패킷일 수 있습니다." );
            }
        }

        private string convertByteToBinary( byte b )
        {
            var bits = "";
            byte mask = 0x01;
            for ( var i = 0; i < 8; i++ )
            {
                if ( ( b & ( byte )( mask << i ) ) != 0 ) bits = "1" + bits;
                else bits = "0" + bits;
            }

            return bits;
        }
        private void convertBytesToOthers()
        {
            textBox_Binary.Text = textBox_Uint8.Text = textBox_Uint16.Text = textBox_Uint32.Text = textBox_Float.Text = textBox_Double.Text = "Error";

            var target = textBox_Bytes.Text.Replace( " ", "" );
            if ( target.Length % 2 == 1 ) target = "0" + target;

            var p = new List<byte>();
            for ( var i = 0; i < target.Length - 1; i += 2 )
            {
                try
                {
                    p.Add( Convert.ToByte( $"{target[i]}{target[i + 1]}", 16 ) );
                }
                catch
                {
                    return;
                }
            }

            if ( p.Count == 0 ) return;

            if ( p.Count >= 1 )
            {
                textBox_Binary.Text = convertByteToBinary( p[0] );
                textBox_Uint8.Text = p[0].ToString();
                for ( var i = 1; i < p.Count; i++ )
                {
                    textBox_Binary.Text = textBox_Binary.Text + " " + convertByteToBinary( p[i] );
                    textBox_Uint8.Text = textBox_Uint8.Text + " " + p[i].ToString();
                }
            }

            if ( p.Count >= 2 )
            {
                textBox_Uint16.Text = new Q_UInt16( p[0], p[1] ).Value.ToString();
                for ( var i = 2; i < p.Count - 1; i += 2 )
                {
                    try
                    {
                        textBox_Uint16.Text = textBox_Uint16.Text + " " + new Q_UInt16( p[i], p[i + 1] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }

            if ( p.Count >= 4 )
            {
                textBox_Uint32.Text = new Q_UInt32( p[0], p[1], p[2], p[3] ).Value.ToString();
                textBox_Float.Text = new Q_Float( p[0], p[1], p[2], p[3] ).Value.ToString();
                for ( var i = 4; i < p.Count - 3; i += 4 )
                {
                    try
                    {
                        textBox_Uint32.Text = textBox_Uint32.Text + " " + new Q_UInt32( p[i], p[i + 1], p[i + 2], p[i + 3] ).Value.ToString();
                        textBox_Float.Text = textBox_Float.Text + " " + new Q_Float( p[i], p[i + 1], p[i + 2], p[i + 3] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }

            if ( p.Count >= 8 )
            {
                textBox_Double.Text = new Q_Double( p[0], p[1], p[2], p[3], p[4], p[5], p[6], p[7] ).Value.ToString();
                for ( var i = 8; i < p.Count - 7; i += 8 )
                {
                    try
                    {
                        textBox_Uint32.Text = textBox_Uint32.Text + " " + new Q_Double( p[i], p[i + 1], p[i + 2], p[i + 3], p[i + 4], p[i + 5], p[i + 6], p[i + 7] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }
        }
        private void convertBinaryToOthers()
        {
            textBox_Bytes.Text = textBox_Uint8.Text = textBox_Uint16.Text = textBox_Uint32.Text = textBox_Float.Text = textBox_Double.Text = "Error";

            var target = textBox_Binary.Text.Replace( " ", "" );
            var zeroCount = 8 - target.Length % 8;
            for ( var i = 0; i < zeroCount; i++ ) target = "0" + target;

            var p = new List<byte>();

            byte bits = 0;
            for ( var i = 0; i < target.Length; i++ )
            {
                if ( target[i] == '1' )
                    bits += 1;

                if ( i % 8 == 7 )
                {
                    p.Add( bits );
                    bits = 0;
                }
                else
                {
                    bits <<= 1;
                }
            }

            if ( p.Count == 0 ) return;

            if ( p.Count >= 1 )
            {
                textBox_Uint8.Text = p[0].ToString();
                textBox_Bytes.Text = p[0].ToString( "X2" );
                for ( var i = 1; i < p.Count; i++ )
                {
                    textBox_Uint8.Text = textBox_Uint8.Text + " " + p[i].ToString();
                    textBox_Bytes.Text = textBox_Bytes.Text + " " + p[i].ToString( "X2" );
                }
            }
            else

            if ( p.Count >= 2 )
            {
                textBox_Uint16.Text = new Q_UInt16( p[0], p[1] ).Value.ToString();
                for ( var i = 2; i < p.Count - 1; i += 2 )
                {
                    try
                    {
                        textBox_Uint16.Text = textBox_Uint16.Text + " " + new Q_UInt16( p[i], p[i + 1] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }

            if ( p.Count >= 4 )
            {
                textBox_Uint32.Text = new Q_UInt32( p[0], p[1], p[2], p[3] ).Value.ToString();
                textBox_Float.Text = new Q_Float( p[0], p[1], p[2], p[3] ).Value.ToString();
                for ( var i = 4; i < p.Count - 3; i += 4 )
                {
                    try
                    {
                        textBox_Uint32.Text = textBox_Uint32.Text + " " + new Q_UInt32( p[i], p[i + 1], p[i + 2], p[i + 3] ).Value.ToString();
                        textBox_Float.Text = textBox_Float.Text + " " + new Q_Float( p[i], p[i + 1], p[i + 2], p[i + 3] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }

            if ( p.Count >= 8 )
            {
                textBox_Double.Text = new Q_Double( p[0], p[1], p[2], p[3], p[4], p[5], p[6], p[7] ).Value.ToString();
                for ( var i = 8; i < p.Count - 7; i += 8 )
                {
                    try
                    {
                        textBox_Uint32.Text = textBox_Uint32.Text + " " + new Q_Double( p[i], p[i + 1], p[i + 2], p[i + 3], p[i + 4], p[i + 5], p[i + 6], p[i + 7] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }
        }
        private void convertUint8ToOthers()
        {
            textBox_Bytes.Text = textBox_Binary.Text = textBox_Uint16.Text = textBox_Uint32.Text = textBox_Float.Text = textBox_Double.Text = "Error";

            var target = textBox_Binary.Text.Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

            var p = new List<byte>();

            for ( var i = 0; i < target.Length; i++ )
            {
                if ( byte.TryParse( target[i], out byte b ) ) p.Add( b );
                else return;
            }

            if ( p.Count == 0 ) return;

            if ( p.Count >= 1 )
            {
                textBox_Bytes.Text = p[0].ToString( "X2" );
                textBox_Binary.Text = convertByteToBinary( p[0] );
                for ( var i = 1; i < p.Count; i++ )
                {
                    textBox_Bytes.Text = textBox_Bytes.Text + " " + p[i].ToString( "X2" );
                    textBox_Binary.Text = textBox_Binary.Text + " " + convertByteToBinary( p[i] );
                }
            }

            if ( p.Count >= 2 )
            {
                textBox_Uint16.Text = new Q_UInt16( p[0], p[1] ).Value.ToString();
                for ( var i = 2; i < p.Count - 1; i += 2 )
                {
                    try
                    {
                        textBox_Uint16.Text = textBox_Uint16.Text + " " + new Q_UInt16( p[i], p[i + 1] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }

            if ( p.Count >= 4 )
            {
                textBox_Uint32.Text = new Q_UInt32( p[0], p[1], p[2], p[3] ).Value.ToString();
                textBox_Float.Text = new Q_Float( p[0], p[1], p[2], p[3] ).Value.ToString();
                for ( var i = 4; i < p.Count - 3; i += 4 )
                {
                    try
                    {
                        textBox_Uint32.Text = textBox_Uint32.Text + " " + new Q_UInt32( p[i], p[i + 1], p[i + 2], p[i + 3] ).Value.ToString();
                        textBox_Float.Text = textBox_Float.Text + " " + new Q_Float( p[i], p[i + 1], p[i + 2], p[i + 3] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }

            if ( p.Count >= 8 )
            {
                textBox_Double.Text = new Q_Double( p[0], p[1], p[2], p[3], p[4], p[5], p[6], p[7] ).Value.ToString();
                for ( var i = 8; i < p.Count - 7; i += 8 )
                {
                    try
                    {
                        textBox_Uint32.Text = textBox_Uint32.Text + " " + new Q_Double( p[i], p[i + 1], p[i + 2], p[i + 3], p[i + 4], p[i + 5], p[i + 6], p[i + 7] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }
        }
        private void convertUint16ToOthers()
        {
            textBox_Bytes.Text = textBox_Binary.Text = textBox_Uint8.Text = textBox_Uint32.Text = textBox_Float.Text = textBox_Double.Text = "Error";

            var target = textBox_Uint16.Text.Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

            var p = new List<byte>();

            for ( var i = 0; i < target.Length; i++ )
            {
                if ( ushort.TryParse( target[i], out ushort b ) )
                {
                    var s = new Q_UInt16( b );
                    p.Add( s.Offset0 );
                    p.Add( s.Offset1 );
                }
                else return;
            }

            if ( p.Count == 0 ) return;

            if ( p.Count >= 1 )
            {
                textBox_Bytes.Text = p[0].ToString( "X2" );
                textBox_Uint8.Text = p[0].ToString();
                textBox_Binary.Text = convertByteToBinary( p[0] );
                for ( var i = 1; i < p.Count; i++ )
                {
                    textBox_Bytes.Text = textBox_Bytes.Text + " " + p[i].ToString( "X2" );
                    textBox_Uint8.Text = textBox_Uint8.Text + " " + p[i].ToString();
                    textBox_Binary.Text = textBox_Binary.Text + " " + convertByteToBinary( p[i] );
                }
            }

            if ( p.Count >= 4 )
            {
                textBox_Uint32.Text = new Q_UInt32( p[0], p[1], p[2], p[3] ).Value.ToString();
                textBox_Float.Text = new Q_Float( p[0], p[1], p[2], p[3] ).Value.ToString();
                for ( var i = 4; i < p.Count - 3; i += 4 )
                {
                    try
                    {
                        textBox_Uint32.Text = textBox_Uint32.Text + " " + new Q_UInt32( p[i], p[i + 1], p[i + 2], p[i + 3] ).Value.ToString();
                        textBox_Float.Text = textBox_Float.Text + " " + new Q_Float( p[i], p[i + 1], p[i + 2], p[i + 3] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }

            if ( p.Count >= 8 )
            {
                textBox_Double.Text = new Q_Double( p[0], p[1], p[2], p[3], p[4], p[5], p[6], p[7] ).Value.ToString();
                for ( var i = 8; i < p.Count - 7; i += 8 )
                {
                    try
                    {
                        textBox_Uint32.Text = textBox_Uint32.Text + " " + new Q_Double( p[i], p[i + 1], p[i + 2], p[i + 3], p[i + 4], p[i + 5], p[i + 6], p[i + 7] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }
        }
        private void convertUint32ToOthers()
        {
            textBox_Bytes.Text = textBox_Binary.Text = textBox_Uint8.Text = textBox_Uint16.Text = textBox_Float.Text = textBox_Double.Text = "Error";

            var target = textBox_Uint32.Text.Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

            var p = new List<byte>();

            for ( var i = 0; i < target.Length; i++ )
            {
                if ( uint.TryParse( target[i], out uint b ) )
                {
                    var s = new Q_UInt32( b );
                    p.Add( s.Offset0 );
                    p.Add( s.Offset1 );
                    p.Add( s.Offset2 );
                    p.Add( s.Offset3 );
                }
                else return;
            }

            if ( p.Count == 0 ) return;

            if ( p.Count >= 1 )
            {
                textBox_Bytes.Text = p[0].ToString( "X2" );
                textBox_Uint8.Text = p[0].ToString();
                textBox_Binary.Text = convertByteToBinary( p[0] );
                for ( var i = 1; i < p.Count; i++ )
                {
                    textBox_Bytes.Text = textBox_Bytes.Text + " " + p[i].ToString( "X2" );
                    textBox_Uint8.Text = textBox_Uint8.Text + " " + p[i].ToString();
                    textBox_Binary.Text = textBox_Binary.Text + " " + convertByteToBinary( p[i] );
                }
            }

            if ( p.Count >= 2 )
            {
                textBox_Uint16.Text = new Q_UInt16( p[0], p[1] ).Value.ToString();
                for ( var i = 2; i < p.Count - 1; i += 2 )
                {
                    try
                    {
                        textBox_Uint16.Text = textBox_Uint16.Text + " " + new Q_UInt16( p[i], p[i + 1] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }

            if ( p.Count >= 4 )
            {
                textBox_Float.Text = new Q_Float( p[0], p[1], p[2], p[3] ).Value.ToString();
                for ( var i = 4; i < p.Count - 3; i += 4 )
                {
                    try
                    {
                        textBox_Float.Text = textBox_Float.Text + " " + new Q_Float( p[i], p[i + 1], p[i + 2], p[i + 3] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }

            if ( p.Count >= 8 )
            {
                textBox_Double.Text = new Q_Double( p[0], p[1], p[2], p[3], p[4], p[5], p[6], p[7] ).Value.ToString();
                for ( var i = 8; i < p.Count - 7; i += 8 )
                {
                    try
                    {
                        textBox_Uint32.Text = textBox_Uint32.Text + " " + new Q_Double( p[i], p[i + 1], p[i + 2], p[i + 3], p[i + 4], p[i + 5], p[i + 6], p[i + 7] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }
        }
        private void convertFloatToOthers()
        {
            textBox_Bytes.Text = textBox_Binary.Text = textBox_Uint8.Text = textBox_Uint16.Text = textBox_Uint32.Text = textBox_Double.Text = "Error";

            var target = textBox_Float.Text.Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

            var p = new List<byte>();

            for ( var i = 0; i < target.Length; i++ )
            {
                if ( float.TryParse( target[i], out float b ) )
                {
                    var s = new Q_Float( b );
                    p.Add( s.Offset0 );
                    p.Add( s.Offset1 );
                    p.Add( s.Offset2 );
                    p.Add( s.Offset3 );
                }
                else return;
            }

            if ( p.Count == 0 ) return;

            if ( p.Count >= 1 )
            {
                textBox_Bytes.Text = p[0].ToString( "X2" );
                textBox_Uint8.Text = p[0].ToString();
                textBox_Binary.Text = convertByteToBinary( p[0] );
                for ( var i = 1; i < p.Count; i++ )
                {
                    textBox_Bytes.Text = textBox_Bytes.Text + " " + p[i].ToString( "X2" );
                    textBox_Uint8.Text = textBox_Uint8.Text + " " + p[i].ToString();
                    textBox_Binary.Text = textBox_Binary.Text + " " + convertByteToBinary( p[i] );
                }
            }

            if ( p.Count >= 2 )
            {
                textBox_Uint16.Text = new Q_UInt16( p[0], p[1] ).Value.ToString();
                for ( var i = 2; i < p.Count - 1; i += 2 )
                {
                    try
                    {
                        textBox_Uint16.Text = textBox_Uint16.Text + " " + new Q_UInt16( p[i], p[i + 1] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }

            if ( p.Count >= 4 )
            {
                textBox_Uint32.Text = new Q_UInt32( p[0], p[1], p[2], p[3] ).Value.ToString();
                for ( var i = 4; i < p.Count - 3; i += 4 )
                {
                    try
                    {
                        textBox_Uint32.Text = textBox_Uint32.Text + " " + new Q_UInt32( p[i], p[i + 1], p[i + 2], p[i + 3] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }

            if ( p.Count >= 8 )
            {
                textBox_Double.Text = new Q_Double( p[0], p[1], p[2], p[3], p[4], p[5], p[6], p[7] ).Value.ToString();
                for ( var i = 8; i < p.Count - 7; i += 8 )
                {
                    try
                    {
                        textBox_Uint32.Text = textBox_Uint32.Text + " " + new Q_Double( p[i], p[i + 1], p[i + 2], p[i + 3], p[i + 4], p[i + 5], p[i + 6], p[i + 7] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }
        }
        private void convertDoubleToOthers()
        {
            textBox_Bytes.Text = textBox_Binary.Text = textBox_Uint8.Text = textBox_Uint16.Text = textBox_Uint32.Text = textBox_Float.Text = "Error";

            var target = textBox_Double.Text.Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

            var p = new List<byte>();

            for ( var i = 0; i < target.Length; i++ )
            {
                if ( double.TryParse( target[i], out double b ) )
                {
                    var s = new Q_Double( b );
                    p.Add( s.Offset0 );
                    p.Add( s.Offset1 );
                    p.Add( s.Offset2 );
                    p.Add( s.Offset3 );
                    p.Add( s.Offset4 );
                    p.Add( s.Offset5 );
                    p.Add( s.Offset6 );
                    p.Add( s.Offset7 );
                }
                else return;
            }

            if ( p.Count == 0 ) return;

            if ( p.Count >= 1 )
            {
                textBox_Bytes.Text = p[0].ToString( "X2" );
                textBox_Binary.Text = convertByteToBinary( p[0] );
                textBox_Uint8.Text = p[0].ToString();
                for ( var i = 1; i < p.Count; i++ )
                {
                    textBox_Bytes.Text = textBox_Bytes.Text + " " + p[i].ToString( "X2" );
                    textBox_Uint8.Text = textBox_Uint8.Text + " " + p[i].ToString();
                    textBox_Binary.Text = textBox_Binary.Text + " " + convertByteToBinary( p[i] );
                }
            }

            if ( p.Count >= 2 )
            {
                textBox_Uint16.Text = new Q_UInt16( p[0], p[1] ).Value.ToString();
                for ( var i = 2; i < p.Count - 1; i += 2 )
                {
                    try
                    {
                        textBox_Uint16.Text = textBox_Uint16.Text + " " + new Q_UInt16( p[i], p[i + 1] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }

            if ( p.Count >= 4 )
            {
                textBox_Uint32.Text = new Q_UInt32( p[0], p[1], p[2], p[3] ).Value.ToString();
                textBox_Float.Text = new Q_Float( p[0], p[1], p[2], p[3] ).Value.ToString();
                for ( var i = 4; i < p.Count - 3; i += 4 )
                {
                    try
                    {
                        textBox_Uint32.Text = textBox_Uint32.Text + " " + new Q_UInt32( p[i], p[i + 1], p[i + 2], p[i + 3] ).Value.ToString();
                        textBox_Float.Text = textBox_Float.Text + " " + new Q_Float( p[i], p[i + 1], p[i + 2], p[i + 3] ).Value.ToString();
                    }
                    catch ( IndexOutOfRangeException ) { break; }
                }
            }
        }

        private void richTextBox2_SelectionChanged( object sender, EventArgs e )
        {
            return;
            //if ( !checkBox_AutoCrawling.Checked ) return;

            textBox_Bytes.Text = textBox2.Text = clean( ( sender as RichTextBox ).SelectedText );

            convertBytesToOthers();
            //convert();
            bit();
        }

        bool flag;
        private void textBox_KeyDown( object sender, KeyEventArgs e )
        {
            flag = e.Control && ( e.KeyCode == Keys.C || e.KeyCode == Keys.V || e.KeyCode == Keys.A || e.KeyCode == Keys.X );
        }
        private void textBox_KeyPress_HexOnly( object sender, KeyPressEventArgs e )
        {
            if ( flag ) return;

            switch ( e.KeyChar )
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'A':
                case 'a':
                case 'B':
                case 'b':
                case 'C':
                case 'c':
                case 'D':
                case 'd':
                case 'E':
                case 'e':
                case 'F':
                case 'f':
                case '\b':
                case ' ':
                    break;

                default:
                    e.Handled = true;
                    break;
            }
        }
        private void textBox_KeyPress_BinaryOnly( object sender, KeyPressEventArgs e )
        {
            if ( flag ) return;

            switch ( e.KeyChar )
            {
                case '0':
                case '1':
                case ' ':
                case '\b':
                    break;

                default:
                    e.Handled = true;
                    break;
            }
        }
        private void textBox_KeyPress_DecimalOnly( object sender, KeyPressEventArgs e )
        {
            if ( flag ) return;

            switch ( e.KeyChar )
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case ' ':
                case '\b':
                    break;

                default:
                    e.Handled = true;
                    break;
            }
        }
        private void textBox_KeyPress_RealOnly( object sender, KeyPressEventArgs e )
        {
            if ( flag ) return;

            switch ( e.KeyChar )
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case ' ':
                case '.':
                case '\b':
                    break;

                default:
                    e.Handled = true;
                    break;
            }
        }

        bool checkCharactor( char c )
        {
            switch ( c )
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'A':
                case 'a':
                case 'B':
                case 'b':
                case 'C':
                case 'c':
                case 'D':
                case 'd':
                case 'E':
                case 'e':
                case 'F':
                case 'f':
                    return true;
            }

            return false;
        }
        private string smartClean( string value )
        {
            var builder = new StringBuilder();

            value = value.ToLower().Replace( "\t", " " ).Replace( "\n", " " ).Replace( Environment.NewLine, " " );
            var split = value.Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

            for ( var i = 0; i < split.Length; i++ )
            {
                if ( split[i].Length != 2 )
                {   // 공백을 기준으로 찢은 문자열의 길이가 2가 아닌 경우 중에서
                    // 0x1111(StartStepSequence) 와 같은 문자열인 경우
                    // "0x"의 인덱스를 찾아서 0x가 존재한다면 
                    if ( split[i].IndexOf( "0x" ) is var index && index != -1 )
                    {
                        var temp = string.Empty;

                        // 인덱스 + 2의 위치부터 16진수를 구성하는 숫자에 해당하는 문자가 나오면 temp에 더한다
                        for ( var j = index + 2; j < split[i].Length; j++ )
                        {
                            if ( checkCharactor( split[i][j] ) )
                                temp += split[i][j];
                        }

                        // 만약 최종 temp의 길이가 0이 아니고, temp의 길이가 짝수일 때는 builder에 추가한다
                        if ( temp.Length != 0 && temp.Length % 2 == 0 )
                            builder.Append( temp );
                    }
                    else
                    {
                        // 만약 0x는 없지만 문자열을 구성하는 모든 문자가 16진수로 판단된다면
                        // 개수가 홀수인 경우에만 추가한다.
                        var temp = string.Empty;
                        for ( var j = 0; j < split[i].Length; j++ )
                            if ( checkCharactor( split[i][j] ) )
                                temp += split[i][j];

                        if ( temp.Length != 0 && temp.Length == split[i].Length && temp.Length % 2 == 0 )
                            builder.Append( temp );
                    }
                }
                else
                {
                    // 길이가 2인 경우에는
                    // 두 문자가 모두 16진수를 구성하는 숫자일 때만 builder에 추가한다.
                    if ( checkCharactor( split[i][0] ) && checkCharactor( split[i][1] ) )
                    {
                        builder.Append( split[i][0] );
                        builder.Append( split[i][1] );
                    }
                }
            }

            var result = new StringBuilder();
            for ( var i = 0; i < builder.Length; i++ )
            {
                result.Append( builder[i] );
                if ( i % 2 != 0 ) result.Append( ' ' );
            }
            return result.ToString().ToUpper();
        }
        private string clean( string value )
        {
            var builder = new StringBuilder();

            value = value.Replace( "0x", "" ).Replace( "0X", "" );

            foreach ( var c in value )
            {
                switch ( c )
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case 'A':
                    case 'a':
                    case 'B':
                    case 'b':
                    case 'C':
                    case 'c':
                    case 'D':
                    case 'd':
                    case 'E':
                    case 'e':
                    case 'F':
                    case 'f':
                        builder.Append( c );
                        break;
                }
            }

            return builder.ToString().ToUpper();
        }

        private void bit()
        {
            var target = clean( textBox2.Text );
            var seed = clean( textBox_Seed.Text );

            if ( target.Length % 2 != 0 ) return;

            byte result = 0x00;
            try
            {
                for ( var i = 0; i < target.Length - 1; i += 2 )
                {
                    switch ( comboBox1.SelectedIndex )
                    {
                        case 0:
                            result &= Convert.ToByte( $"{target[i]}{target[i + 1]}", 16 );
                            break;

                        case 1:
                            result |= Convert.ToByte( $"{target[i]}{target[i + 1]}", 16 );
                            break;

                        case 2:
                            result ^= Convert.ToByte( $"{target[i]}{target[i + 1]}", 16 );
                            break;
                    }
                }
                for ( var i = 0; i < seed.Length - 1; i += 2 )
                {
                    switch ( comboBox1.SelectedIndex )
                    {
                        case 0:
                            result &= Convert.ToByte( $"{seed[i]}{seed[i + 1]}", 16 );
                            break;

                        case 1:
                            result |= Convert.ToByte( $"{seed[i]}{seed[i + 1]}", 16 );
                            break;

                        case 2:
                            result ^= Convert.ToByte( $"{seed[i]}{seed[i + 1]}", 16 );
                            break;
                    }
                }
            }
            catch ( Exception )
            {
                textBox3.Text = "ERR";
                return;
            }

            textBox3.Text = $"{result:X2}";
        }
        private void textBox2_KeyUp( object sender, KeyEventArgs e )
        {
            bit();
        }

        private void comboBox1_SelectedIndexChanged( object sender, EventArgs e )
        {
            bit();
        }

        private void button2_Click( object sender, EventArgs e )
        {
            richTextBox1.Text = smartClean( richTextBox1.Text );
        }

        private void textBox_Bytes_KeyUp( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Enter ) convertBytesToOthers();
        }

        private void textBox_Binary_KeyUp( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Enter ) convertBinaryToOthers();
        }

        private void textBox_Uint8_KeyUp( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Enter ) convertUint8ToOthers();
        }

        private void textBox_Uint16_KeyUp( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Enter ) convertUint16ToOthers();
        }

        private void textBox_Uint32_KeyUp( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Enter ) convertUint32ToOthers();
        }

        private void textBox_Float_KeyUp( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Enter ) convertFloatToOthers();
        }

        private void textBox_Double_KeyUp( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.Enter ) convertDoubleToOthers();
        }

        private void button3_Click( object sender, EventArgs e )
        {
            var target = smartClean( richTextBox2.SelectedText ).Replace( " ", "" );

            var p = new List<byte>();
            for ( var i = 0; i < target.Length - 1; i += 2 )
            {
                try
                {
                    p.Add( Convert.ToByte( $"{target[i]}{target[i + 1]}", 16 ) );
                }
                catch
                {
                    return;
                }
            }

            var maskedString = "";
            for ( var i = 0; i < p.Count; i++ ) maskedString += ( ( byte )( p[i] ^ 0x80 ) ).ToString( "X2" ) + " ";

            maskedString += $" (unmasked {richTextBox2.SelectedText.Length} -> {maskedString.Length})";
            var startIndex = richTextBox2.SelectionStart;
            var selectionCount = richTextBox2.SelectedText.Length;
            richTextBox2.Text = richTextBox2.Text.Substring( 0, startIndex ) + maskedString + richTextBox2.Text.Substring( startIndex + selectionCount, richTextBox2.Text.Length - ( startIndex + selectionCount ) );
        }

        enum PacketType
        {
            Send,
            Received
        }
        enum CommType
        {
            None,
            MainComm,
            SlaveComm,
        }
        struct PacketInfo
        {
            public string Time;
            public PacketType PacketType;
            public Packet Packet;

            public CommType CommType;
            public int Ch;

            public PacketInfo( string time, PacketType packetType, byte[] rawPacket )
            {
                Time = time;
                PacketType = packetType;
                Packet = ReceivedPacket.Parse( rawPacket );

                CommType = CommType.None;
                Ch = -1;
            }

            public PacketInfo(string time, PacketType packetType, byte[] rawPacket, CommType commType, int ch )
            {
                Time = time;
                PacketType = packetType;
                Packet = ReceivedPacket.Parse( rawPacket );

                CommType = commType;
                Ch = ch;
            }
        }
        List<PacketInfo> packetInfos = new List<PacketInfo>();
        List<PacketInfo> currentDisplayed = new List<PacketInfo>();
        Regex regex = new Regex( "^[[][0-9][0-9]:[0-9][0-9]:[0-9][0-9][.][0-9][0-9][0-9][]]$" );
        private void button4_Click( object sender, EventArgs e )
        {
            using ( var dialog = new OpenFileDialog()
            {
                Filter = "*.log|*.log"
            } )
            {
                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    packetInfos.Clear();

                    using ( var sr = new StreamReader( dialog.FileName ) )
                    {
                        while ( true ) {
                            string time;
                            PacketType packetType;

                            string line = sr.ReadLine();
                            if ( line != null )
                            {
                                var split = line.Split( ' ' );
                                if ( split.Length == 0 ) continue;

                                //, RegexOptions.Compiled );
                                if ( regex.IsMatch( split[0] ) )
                                {
                                    // 날짜, Send/Receive 정보 읽기
                                    time = split[0].Replace( "[", "" ).Replace( "]", "" );
                                }
                                else
                                {
                                    continue;
                                }

                                if ( line.Contains( "SendPacket" ) )
                                {
                                    packetType = PacketType.Send;
                                    line = sr.ReadLine();
                                }
                                else if ( line.Contains( "ReceivePacket" ) )
                                {
                                    packetType = PacketType.Received;
                                    while ( ( line = sr.ReadLine() ) != null && line.Contains( "SubPacket" ) ) ;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                break;
                            }

                            if ( line != null )
                            {
                                // Raw Packet 읽기
                                if ( line.Contains( "RawPacket = " ) )
                                {
                                    var str = line.Replace( "RawPacket = ", "" ).Replace( " ", "" );

                                    var p = new List<byte>();

                                    try
                                    {
                                        for ( var i = 0; i < str.Length - 1; i += 2 )
                                        {
                                            p.Add( Convert.ToByte( $"{str[i]}{str[i + 1]}", 16 ) );
                                        }
                                    }
                                    catch ( Exception )
                                    {
                                        continue;
                                    }

                                    packetInfos.Add( new PacketInfo( time, packetType, p.ToArray() ) );
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        //isQ730Log = true;
                        refreshListViewItems();
                    }
                }
            }
        }
        //private bool isQ730Log = true;
        //private void refreshListViewItems(string keyWord = "" )
        //{
        //    if ( isQ730Log ) refreshListViewItems_Q730( keyWord );
        //    else refreshListViewItems_Teraterm( keyWord );
        //}
        private void refreshListViewItems(string keyWord = "")
        {
            listView1.Items.Clear();

            currentDisplayed = packetInfos.Where( i => (( i.PacketType == PacketType.Send && checkBox_Send.Checked ) ||
                                                       (  i.PacketType == PacketType.Received && checkBox_Receive.Checked ))
                                                            && (keyWord == "" || i.Packet.SubPacket.Command.ToString().ToLower().Contains(keyWord.ToLower()) 
                                                                              || $"0x{i.Packet.SubPacket.CMD_1:X2}{i.Packet.SubPacket.CMD_2:X2}".ToLower().Contains(keyWord.ToLower()))).Select( j => j ).ToList();

            for(var i = 0; i < currentDisplayed.Count; i++ )
            {
                var item = new ListViewItem( ( i + 1 ).ToString() );

                item.SubItems.Add( currentDisplayed[i].Time );
                item.SubItems.Add( $"0x{currentDisplayed[i].Packet.ADDR2:X2}" );
                item.SubItems.Add( $"0x{currentDisplayed[i].Packet.CH2:X2}" );

                if ( currentDisplayed[i].CommType == CommType.None ) item.SubItems.Add( $"-" );
                else
                {
                    if(currentDisplayed[i].Ch != -1 )
                    {
                        item.SubItems.Add( $"slave ch{currentDisplayed[i].Ch} comm rx" );
                    }
                    else
                    {
                        var str = currentDisplayed[i].CommType == CommType.MainComm ? "main comm " : "slave comm ";
                        str += currentDisplayed[i].PacketType == PacketType.Send ? "tx" : "rx";
                        item.SubItems.Add( str );
                    }
                }
                item.SubItems.Add( $"{currentDisplayed[i].Packet.SubPacket.Command}(0x{currentDisplayed[i].Packet.SubPacket.CMD_1:X2}{currentDisplayed[i].Packet.SubPacket.CMD_2:X2})" );

                if(currentDisplayed[i].PacketType == PacketType.Send )
                {
                    item.BackColor = Color.PaleGreen;
                }
                else
                {
                    item.BackColor = Color.MistyRose;
                }

                listView1.Items.Add( item );
            }
        }
        private void refreshListViewItems_Teraterm(string keyWord = "" )
        {
            listView1.Items.Clear();

            currentDisplayed = packetInfos.Where( i => ( ( i.PacketType == PacketType.Send && checkBox_Send.Checked ) ||
                                                       ( i.PacketType == PacketType.Received && checkBox_Receive.Checked ) )
                                                            && ( keyWord == "" || i.Packet.SubPacket.Command.ToString().ToLower().Contains( keyWord.ToLower() )
                                                                              || $"0x{i.Packet.SubPacket.CMD_1:X2}{i.Packet.SubPacket.CMD_2:X2}".ToLower().Contains( keyWord.ToLower() ) ) ).Select( j => j ).ToList();

            for ( var i = 0; i < currentDisplayed.Count; i++ )
            {
                var item = new ListViewItem( ( i + 1 ).ToString() );

                item.SubItems.Add( currentDisplayed[i].Time );
                item.SubItems.Add( $"0x{currentDisplayed[i].Packet.ADDR2:X2}" );
                item.SubItems.Add( $"0x{currentDisplayed[i].Packet.CH2:X2}" );

                item.SubItems.Add( $"-" );
                item.SubItems.Add( $"{currentDisplayed[i].Packet.SubPacket.Command}(0x{currentDisplayed[i].Packet.SubPacket.CMD_1:X2}{currentDisplayed[i].Packet.SubPacket.CMD_2:X2})" );

                if ( currentDisplayed[i].PacketType == PacketType.Send )
                {
                    item.BackColor = Color.PaleGreen;
                }
                else
                {
                    item.BackColor = Color.MistyRose;
                }

                listView1.Items.Add( item );
            }
        }

        private void listView1_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( listView1.SelectedIndices.Count == 0 ) return;

            richTextBox1.Text = Util.BytesToString( currentDisplayed[listView1.SelectedIndices[0]].Packet.RawPacket );
        }

        private void listView1_DoubleClick( object sender, EventArgs e )
        {
            button1.PerformClick();
        }

        private void checkBox_Send_CheckedChanged( object sender, EventArgs e )
        {
            refreshListViewItems();
        }

        private void button5_Click( object sender, EventArgs e )
        {
            MessageBox.Show( listView1.Columns[4].Width.ToString() );
        }

        private void listView_ColumnWidthChanging( object sender, ColumnWidthChangingEventArgs e )
        {
            e.Cancel = true;
            e.NewWidth = listView1.Columns[e.ColumnIndex].Width;
        }

        private void textBox1_KeyDown( object sender, KeyEventArgs e )
        {
            if(e.KeyCode == Keys.Enter )
            {
                refreshListViewItems( textBox1.Text );
            }
        }

        Regex dateTime = new Regex( "^[[][0-9][0-9][0-9][0-9][-][0-9][0-9][-][0-9][0-9] [0-9][0-9][:][0-9][0-9][:][0-9][0-9][.][0-9][0-9][0-9][]]" );
        Regex findPacket = new Regex( "FE FE DC BA " );
        Regex slaveComm = new Regex( "slave ch[0-9] comm" );
        private void button5_Click_1( object sender, EventArgs e )
        {
            using ( var dialog = new OpenFileDialog()
            {
                Filter = "*.log|*.log"
            } )
            {
                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    packetInfos.Clear();

                    using ( var sr = new StreamReader( dialog.FileName ) )
                    {
                        string line;
                        while ( ( line = sr.ReadLine() ) != null )
                        {
                            var packetMatch = findPacket.Match( line );
                            if ( !packetMatch.Success ) continue;

                            var timeMatch = dateTime.Match( line );
                            if ( !timeMatch.Success ) continue;

                            var time = timeMatch.Value.Substring( 12, 12 );
                            var packetString = line.Substring( packetMatch.Index );

                            int ch;
                            CommType commType;
                            PacketType packetType;

                            if ( line.Contains( "slave sender" ) )
                            {
                                ch = -1;
                                packetType = PacketType.Send;
                                commType = CommType.SlaveComm;
                            }
                            else if ( line.Contains( "main comm rx" ) )
                            {
                                ch = -1;
                                packetType = PacketType.Received;
                                commType = CommType.MainComm;
                            }
                            else if ( line.Contains( "main comm tx" ) )
                            {
                                ch = -1;
                                packetType = PacketType.Send;
                                commType = CommType.MainComm;
                            }
                            else if (line.Contains("slave comm rx" ) )
                            {
                                ch = -1;
                                packetType = PacketType.Received;
                                commType = CommType.SlaveComm;
                            }
                            else if ( line.Contains( "slave comm tx" ) )
                            {
                                ch = -1;
                                packetType = PacketType.Send;
                                commType = CommType.SlaveComm;
                            }
                            else if (slaveComm.Match(line) is Match match && match.Success)
                            {
                                ch = int.Parse( match.Value.Substring( 8, 1 ) );
                                packetType = PacketType.Received;
                                commType = CommType.SlaveComm;
                            }
                            else
                            {
                                continue;
                            }

                            var str = packetString.Replace( " ", "" );

                            var p = new List<byte>();

                            try
                            {
                                for ( var i = 0; i < str.Length - 1; i += 2 )
                                {
                                    p.Add( Convert.ToByte( $"{str[i]}{str[i + 1]}", 16 ) );
                                }
                            }
                            catch ( Exception )
                            {
                                continue;
                            }

                            packetInfos.Add( new PacketInfo( time, packetType, p.ToArray(), commType, ch ) );
                        }

                        //isQ730Log = false;
                        refreshListViewItems();
                    }
                }
            }
        }
    }
}
