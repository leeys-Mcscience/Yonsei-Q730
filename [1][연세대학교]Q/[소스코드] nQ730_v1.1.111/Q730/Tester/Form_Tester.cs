using McQLib.Core;
using McQLib.Device;
using McQLib.Recipes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Q730.Tester
{
    public partial class Form_Tester : Form
    {
        Communicator communicator;
        public Form_Tester()
        {
            InitializeComponent();

            richTextBox1.LanguageOption = 0;
            timer_ThreadChecker.Start();
        }

        Channel[] channels;
        private void button_Connect_Click( object sender, EventArgs e )
        {
            if ( communicator != null ) communicator.Dispose();

            communicator = new Communicator( 0, ( int )numericUpDown_Channels.Value );
            communicator.PacketLog += log;
            communicator.ShowPacketLog = true;
            communicator.WithRawPacket = true;
            communicator.IP = textBox_IP.Text;

            MessageBox.Show( communicator.Connect().ToString() );
            if ( communicator.IsConnected )
            {
                communicator.InitializeCommunicator();
                channels = communicator.Channels;
                userControl_Channels1.BindChannels( channels );
            }
        }
        private void button_Disconnect_Click( object sender, EventArgs e )
        {
            MessageBox.Show( communicator.Disconnect().ToString() );
        }

        string logPath = null;
        void log( string msg )
        {
            var logMessage = $"[{DateTime.Now:HH:mm:ss:fff}] {msg}{Environment.NewLine}{Environment.NewLine}";
            Invoke( new Action( delegate ()
            {
                richTextBox1.AppendText( logMessage );
                if ( checkBox_AutoScroll.Checked )
                {
                    richTextBox1.ScrollToCaret();
                }
            } ) );

            if ( logPath != null )
            {
                using ( var sw = new StreamWriter( logPath, true ) )
                {
                    sw.Write( logMessage );
                }
            }
        }

        byte addr = 0;
        byte ch = 0;
        byte err = 0;
        byte byPass = 0;

        private void setDestinationLabel()
        {
            label_Destination.Text = addr == 0 && ch == 0 ? "Master" : "Slave";
            label_ADDR.Text = $"0x{addr:X2}";
            label_CH.Text = $"0x{ch:X2}";

            label_ByPass.Text = $"0x{byPass:X2}";
            label_ByPassText.Text = byPass == 0 ? "OFF" : "ON";

            label_ErrorQueryText.Text = err == 0 ? "Get" : "Set";
            label_ErrorQuery.Text = $"0x{err:X2}";
        }
        private void radioButton_ErrorQuery_CheckedChanged( object sender, EventArgs e )
        {
            if ( radioButton_ErrGet.Checked ) err = 0;
            else err = 1;
            setDestinationLabel();
        }
        private void radioButton_ByPass_CheckedChanged( object sender, EventArgs e )
        {
            if ( radioButton_ByPassOn.Checked ) byPass = 1;
            else byPass = 0;
            setDestinationLabel();
        }
        private void radioButton_ADDR_CheckedChanged( object sender, EventArgs e )
        {
            if ( ( sender as RadioButton ).Checked )
            {
                if ( radioButton_Master.Checked )
                {
                    radioButton_Master.Checked = false;
                    radioButton_CH1.Checked = true;
                }
                addr = ( byte )int.Parse( ( sender as Control ).Tag.ToString() );
            }
            setDestinationLabel();
        }
        private void radioButton_CH_CheckedChanged( object sender, EventArgs e )
        {
            if ( ( sender as RadioButton ).Checked )
            {
                if ( radioButton_Master.Checked )
                {
                    radioButton_Master.Checked = false;
                    radioButton_ADDR1.Checked = true;
                }
                ch = ( byte )int.Parse( ( sender as Control ).Tag.ToString() );
            }
            setDestinationLabel();
        }
        private void radioButton_Master_CheckedChanged( object sender, EventArgs e )
        {
            if ( radioButton_Master.Checked )
            {
                radioButton_ADDR0.Checked = false;
                radioButton_ADDR1.Checked = false;
                radioButton_ADDR2.Checked = false;
                radioButton_ADDR3.Checked = false;
                radioButton_ADDR4.Checked = false;
                radioButton_ADDR5.Checked = false;
                radioButton_ADDR6.Checked = false;
                radioButton_ADDR7.Checked = false;
                radioButton_ADDR8.Checked = false;
                radioButton_CH0.Checked = false;
                radioButton_CH1.Checked = false;
                radioButton_CH2.Checked = false;
                radioButton_CH3.Checked = false;
                radioButton_CH4.Checked = false;
                radioButton_CH5.Checked = false;
                radioButton_CH6.Checked = false;
                radioButton_CH7.Checked = false;
                radioButton_CH8.Checked = false;
                addr = 0;
                ch = 0;
            }
            setDestinationLabel();
        }

        private SendPacket createPacket( Enum cmd, byte[] data = null )
        {
            var packet = new SendPacket( addr, ch );
            packet.ByPass = byPass;
            if ( data != null )
            {
                packet.SubPackets.Add( new SendSubPacket( cmd, err, data ) );
            }
            else
            {
                packet.SubPackets.Add( new SendSubPacket( cmd, err ) );
            }

            return packet;
        }
        /// <summary>
        /// 지정한 Cmd, DATA와 전역으로 지정된 Addr, Ch, ByPass 및 Err 필드 값을 사용하여 패킷을 송신하고, 응답 패킷을 기다립니다.
        /// <br>이 메서드는 장비의 연결 상태를 검사하지 않습니다.</br>
        /// </summary>
        /// <param name="cmd">송신할 명령어입니다.</param>
        /// <param name="data">송신할 명령어와 함께 추가할 DATA 필드의 내용입니다. 기본값은 null입니다.</param>
        private void sendCommand( Enum cmd, byte[] data = null )
        {
            var packet = new SendPacket( addr, ch );
            packet.ByPass = byPass;
            packet.SubPackets.Add( new SendSubPacket( cmd ) );
            packet.SubPacket.ERR = err;
            if ( data != null ) packet.SubPacket.DATA.Add( data );

            if ( communicator.SendAndReceive( packet ) == null )
            {
                log( "No response" );
            }
        }
        /// <summary>
        /// 지정된 Addr, Ch, ByPass, Cmd, Err 및 DATA를 사용하여 패킷을 송신하고, 응답 패킷을 기다립니다.
        /// <br>이 메서드는 장비의 연결 상태를 검사하지 않습니다.</br>
        /// </summary>
        /// <param name="addr">송신할 패킷의 ADDR2 필드의 값 입니다.</param>
        /// <param name="ch">송신할 패킷의 CH2 필드의 값 입니다.</param>
        /// <param name="byPass">송신할 패킷의 ByPass 필드의 값 입니다.</param>
        /// <param name="cmd">송신할 패킷의 CMD 필드의 값 입니다.</param>
        /// <param name="err">송신할 패킷의 ERR 필드의 값 입니다.</param>
        /// <param name="data">송신할 패킷의 DATA 필드의 값 입니다.</param>
        private void sendCommand( byte addr, byte ch, byte byPass, Enum cmd, byte err, byte[] data = null )
        {
            var packet = new SendPacket( addr, ch );
            packet.ByPass = byPass;
            packet.SubPackets.Add( new SendSubPacket( cmd ) );
            packet.SubPacket.ERR = err;
            if ( data != null ) packet.SubPacket.DATA.Add( data );

            if ( communicator.SendAndReceive( packet ) == null )
            {
                log( "No response" );
            }
        }
        /// <summary>
        /// 지정된 Index, ByPass, Cmd, Err 및 DATA를 사용하여 패킷을 송신하고, 응답 패킷을 기다립니다.
        /// <br>이 메서드는 장비의 연결 상태를 검사하지 않습니다.</br>
        /// </summary>
        /// <param name="index">송신할 패킷의 목적지 채널의 Index입니다. ADDR2와 CH2로 변환됩니다.</param>
        /// <param name="byPass">송신할 패킷의 ByPass 필드의 값 입니다.</param>
        /// <param name="cmd">송신할 패킷의 CMD 필드의 값 입니다.</param>
        /// <param name="err">송신할 패킷의 ERR 필드의 값 입니다.</param>
        /// <param name="data">송신할 패킷의 DATA 필드의 값 입니다.</param>
        private void sendCommand( int index, byte byPass, Enum cmd, byte err, byte[] data = null )
        {
            var packet = new SendPacket( Util.GetADDR( index ), Util.GetCH( index ) );
            packet.ByPass = byPass;
            packet.SubPackets.Add( new SendSubPacket( cmd ) );
            packet.SubPacket.ERR = err;
            if ( data != null ) packet.SubPacket.DATA.Add( data );

            if ( communicator.SendAndReceive( packet ) == null )
            {
                log( "No response" );
            }
        }

        PacketParser.Form1 parser = new PacketParser.Form1();
        private void button_PacketParser_Click( object sender, EventArgs e )
        {
            try
            {
                parser.Show();
            }
            catch ( ObjectDisposedException )
            {
                parser = new PacketParser.Form1();
                parser.Show();
            }

            if ( richTextBox1.SelectedText.Trim().Length != 0 ) parser.TryParse( richTextBox1.SelectedText );
        }

        #region Test Button Events
        private void button_Ping_Click( object sender, EventArgs e )
        {
            sendCommand( Commands.CommonCommands.Ping_G );
        }
        private void button_GetBoardResearch_Click( object sender, EventArgs e )
        {
            sendCommand( Commands.MultiChannelCommands.M_BoardSearching_GS );
        }
        private void button_Register_Click( object sender, EventArgs e )
        {
            sendCommand( Commands.CommonCommands.InitStateResister_GS );
        }
        private void button_Clear_Click( object sender, EventArgs e )
        {
            richTextBox1.Text = "";
        }
        private void button_BoardInfo_Click( object sender, EventArgs e )
        {
            sendCommand( Commands.CommonCommands.BoardInformation_G );
        }
        private void button_ChannelStatus_Click( object sender, EventArgs e )
        {
            sendCommand( Commands.BatteryCycler_GetMeasureCommands.ChannelState_G );
        }

        #region Sequence Test Events
        private void button_SendSequence_Click( object sender, EventArgs e )
        {
            Sequence sequence = null;

            using ( var f = new Form_SequenceBuilder_Test() )
            {
                if ( f.ShowDialog() == DialogResult.OK )
                {
                    sequence = f.Result;
                }

            }

            if ( sequence == null ) return;

            try
            {
                if ( checkBox_ToAll.Checked )
                {
                    foreach ( var c in channels )
                        if ( c.State == State.IDLE )
                        {
                            channels[c.GlobalIndex].Sequence = sequence;
                            var ad = new Address( 0, 0 );
                            //communicator.SendSequence( c.Address, sequence );
                            Util.CallMethod( communicator, "SendSequence", ad, sequence );
                        }
                }
                else if ( checkBox_Custom.Checked )
                {
                    for ( var i = numericUpDown1.Value; i <= numericUpDown2.Value; i++ )
                    {
                        //channels[( int )( i - 1 )].Sequence = sequence;
                        //channels[( int )( i - 1 )].Owner.SendSequence( channels[( int )( i - 1 )].Address, sequence );
                        Util.CallMethod( channels[( int )( i - 1 )].Owner, "SendSequence", channels[( int )( i - 1 )].Address, sequence );
                        //channels[( int )(i - 1)].Sequence = sequence;
                        //communicator.SendSequence( channels[i - 1].Address, sequence );
                    }
                }
                else
                {
                    //channels[Util.GetIndex( addr, ch )].Owner.SendSequence( new Address( addr, ch ), sequence );
                    Util.CallMethod( channels[Util.GetIndex( addr, ch )].Owner, "SendSequence", new Address( addr, ch ), sequence );
                    //communicator.SendSequence( Util.GetIndex( addr, ch ), sequence );
                }
            }
            catch ( QException ex )
            {
                MessageBox.Show( ex.Message );
            }

        }
        private void button_StartSequence_Click( object sender, EventArgs e )
        {
            try
            {
                if ( checkBox_ToAll.Checked )
                {
                    foreach ( var c in channels )
                        if ( c.State == State.IDLE ) //c.Owner.StartChannel( c.Address ); //communicator.StartChannel( c.GlobalIndex, ApplyWhen.Immediately );
                            Util.CallMethod( c.Owner, "StartChannel", c.Address );
                }
                else if ( checkBox_Custom.Checked )
                {
                    for ( var i = numericUpDown1.Value; i <= numericUpDown2.Value; i++ )
                        Util.CallMethod( channels[( int )( i - 1 )].Owner, "StartChannel", channels[( int )( i - 1 )].Address );
                        //channels[( int )( i - 1 )].Owner.StartChannel( channels[( int )( i - 1 )].Address );
                    //communicator.StartChannel( ( int )i - 1 );
                }
                else
                {
                    //channels[Util.GetIndex( addr, ch )].Owner.StartChannel( new Address( addr, ch ) );
                    Util.CallMethod( channels[Util.GetIndex( addr, ch )].Owner, "StartChannel", new Address( addr, ch ) );
                    //communicator.StartChannel( Util.GetIndex( addr, ch ), ApplyWhen.Immediately );
                }
            }
            catch ( QException ex )
            {
                MessageBox.Show( ex.Message );
            }
        }
        private void button_StopSequence_Click( object sender, EventArgs e )
        {
            try
            {
                if ( checkBox_ToAll.Checked )
                {
                    foreach ( var c in channels )
                        if ( c.State != State.DISCONNECTED ) //c.Owner.StopChannel( c.Address ); //communicator.StopChannel( c.GlobalIndex, ApplyWhen.Immediately );
                            Util.CallMethod( c.Owner, "StopChannel", c.Address );
                }
                else if ( checkBox_Custom.Checked )
                {
                    for ( var i = numericUpDown1.Value; i <= numericUpDown2.Value; i++ )
                        Util.CallMethod( channels[( int )( i - 1 )].Owner, "StopChannel", channels[( int )( i - 1 )].Address );
                        //channels[( int )( i - 1 )].Owner.StopChannel( channels[( int )( i - 1 )].Address );
                    //communicator.StopChannel( ( int )i - 1 );
                }
                else
                {
                    Util.CallMethod( channels[Util.GetIndex( addr, ch )].Owner, "StopChannel", new Address( addr, ch ) );
                    //channels[Util.GetIndex( addr, ch )].Owner.StopChannel( new Address( addr, ch ) );
                    //communicator.StopChannel( Util.GetIndex( addr, ch ), ApplyWhen.Immediately );
                }
            }
            catch ( QException ex )
            {
                MessageBox.Show( ex.Message );
            }
        }
        private void button_PauseSequence_Click( object sender, EventArgs e )
        {
            try
            {
                if ( checkBox_ToAll.Checked )
                {
                    foreach ( var c in channels )
                        if ( c.State != State.DISCONNECTED ) //c.Owner.PauseChannel( c.Address ); // communicator.PauseChannel( c.GlobalIndex, ApplyWhen.Immediately );
                            Util.CallMethod( c.Owner, "PauseChannel", c.Address );

                }
                else if ( checkBox_Custom.Checked )
                {
                    for ( var i = numericUpDown1.Value; i <= numericUpDown2.Value; i++ )
                        Util.CallMethod( channels[( int )( i - 1 )].Owner, "PauseChannel", channels[( int )( i - 1 )].Address );

                    //channels[( int )( i - 1 )].Owner.PauseChannel( channels[( int )( i - 1 )].Address );
                    //communicator.PauseChannel( ( int )i - 1 );
                }
                else
                {
                    Util.CallMethod( channels[Util.GetIndex( addr, ch )].Owner, "PauseChannel", new Address( addr, ch ) );

                    //channels[Util.GetIndex( addr, ch )].Owner.PauseChannel( new Address( addr, ch ) );
                    //communicator.PauseChannel( Util.GetIndex( addr, ch ), ApplyWhen.Immediately );
                }
            }
            catch ( QException ex )
            {
                MessageBox.Show( ex.Message );
            }
        }
        private void button_RestartSequence_Click( object sender, EventArgs e )
        {
            try
            {
                if ( checkBox_ToAll.Checked )
                {
                    foreach ( var c in channels )
                        Util.CallMethod( c.Owner, "RestartChannel", c.Address );

                    //c.Owner.RestartChannel( c.Address );
                    //communicator.RestartChannel( c.GlobalIndex, ApplyWhen.Immediately );
                }
                else if ( checkBox_Custom.Checked )
                {
                    for ( var i = numericUpDown1.Value; i <= numericUpDown2.Value; i++ )
                        Util.CallMethod( channels[( int )( i - 1 )].Owner, "RestartChannel", channels[( int )( i - 1 )].Address );

                    //channels[( int )( i - 1 )].Owner.RestartChannel( channels[( int )( i - 1 )].Address );
                    //communicator.RestartChannel( ( int )i - 1 );
                }
                else
                {
                    Util.CallMethod( channels[Util.GetIndex( addr, ch )].Owner, "RestartChannel", new Address( addr, ch ) );

                    //channels[Util.GetIndex( addr, ch )].Owner.RestartChannel( new Address( addr, ch ) );
                    //communicator.RestartChannel( Util.GetIndex( addr, ch ), ApplyWhen.Immediately );
                }
            }
            catch ( QException ex )
            {
                MessageBox.Show( ex.Message );
            }
        }
        private void button_SkipSequence_Click( object sender, EventArgs e )
        {
            try
            {
                if ( checkBox_ToAll.Checked )
                {
                    foreach ( var c in channels )
                        Util.CallMethod( c.Owner, "SkipChannel", c.Address );

                    //c.Owner.SkipChannel( c.Address );
                    //communicator.SkipChannel( c.GlobalIndex );
                }
                else if ( checkBox_Custom.Checked )
                {
                    for ( var i = numericUpDown1.Value; i <= numericUpDown2.Value; i++ )
                        Util.CallMethod( channels[( int )( i - 1 )].Owner, "SkipChannel", channels[( int )( i - 1 )].Address );

                    //channels[( int )( i - 1 )].Owner.SkipChannel( channels[( int )( i - 1 )].Address );
                    //communicator.SkipChannel( ( int )i - 1 );
                }
                else
                {
                    Util.CallMethod( channels[Util.GetIndex( addr, ch )].Owner, "SkipChannel", new Address( addr, ch ) );

                    //channels[Util.GetIndex( addr, ch )].Owner.SkipChannel( new Address( addr, ch ) );
                    //communicator.SkipChannel( Util.GetIndex( addr, ch ) );
                }
            }
            catch ( QException ex )
            {
                MessageBox.Show( ex.Message );
            }
        }
        private void button_SetSavePath_Click( object sender, EventArgs e )
        {
            using ( var dialog = new CommonOpenFileDialog() )
            {
                dialog.IsFolderPicker = true;

                if ( dialog.ShowDialog() == CommonFileDialogResult.Ok )
                {
                    if ( checkBox_ToAll.Checked )
                    {
                        foreach ( var c in channels )
                        {
                            if ( c.State != State.DISCONNECTED )
                            {
                                c.SaveDirectory = dialog.FileName;
                            }
                        }
                    }
                    else if ( checkBox_Custom.Checked )
                    {
                        for ( var i = numericUpDown1.Value; i <= numericUpDown2.Value; i++ )
                        {
                            if ( channels[( int )i - 1].State != State.DISCONNECTED )
                                channels[( int )i - 1].SaveDirectory = dialog.FileName;
                        }
                    }
                    else
                    {
                        if ( addr == 0 || ch == 0 )
                        {
                            MessageBox.Show( "Master로 수행할 수 없는 동작입니다." );
                            return;
                        }
                        channels[Util.GetIndex( addr, ch )].SaveDirectory = dialog.FileName;
                    }
                }
            }
        }
        //private void Util.CallMethod( object obj, string methodName, params object[] parameters )
        //{
        //    var types = new Type[parameters.Length];
        //    for (var i = 0; i < parameters.Length; i++ )
        //    {
        //        types[i] = parameters[i].GetType();
        //    }

        //    var methods = obj.GetType().GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly );

        //    for ( var i = 0; i < methods.Length; i++ )
        //    {
        //        var parameterInfos = methods[i].GetParameters();

        //        var isSame = true;
        //        for (var j = 0; j < parameterInfos.Length; j++ )
        //        {
        //            if (parameterInfos[j].ParameterType != types[j])
        //            {
        //                isSame = false;
        //                break;
        //            }
        //        }

        //        if ( isSame )
        //        {
        //            methods[i].Invoke( obj, parameters );
        //            break;
        //        }
        //    }
        //}

        private void checkBox_ToAll_CheckedChanged( object sender, EventArgs e )
        {
            if ( checkBox_ToAll.Checked ) checkBox_Custom.Checked = false;
        }
        private void checkBox_Custom_CheckedChanged( object sender, EventArgs e )
        {
            if ( checkBox_Custom.Checked )
            {
                checkBox_ToAll.Checked = false;
                numericUpDown1.Enabled = numericUpDown2.Enabled = true;
            }
            else
            {
                numericUpDown1.Enabled = numericUpDown2.Enabled = false;
            }
        }
        #endregion

        private void checkBox_AutoPing_CheckedChanged( object sender, EventArgs e )
        {
            communicator.IsAutoPing = checkBox_AutoPing.Checked;
        }
        #endregion

        private void Form_Tester_FormClosing( object sender, FormClosingEventArgs e )
        {
            if (communicator != null )
            {
                communicator.Disconnect();
                communicator.Dispose();
            }
        }

        private void button1_Click( object sender, EventArgs e )
        {
            int stop;

            Communicator communicator = this.communicator;
            Channel[] channels = this.channels;

            stop = 1;
        }

        private void button_PatternEditor_Click( object sender, EventArgs e )
        {
            using ( var form = new Form_PatternEditor( null ) )
            {
                if ( form.ShowDialog() == DialogResult.OK )
                {
                    var pattern = PatternData.FromFile( form.LastLoaded );

                    var packetArray = pattern.ToDataField( 0 );

                    for ( var i = 0; i < packetArray.Count; i++ )
                    {
                        var packet = new SendPacket( 1, 1 );
                        packet.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.SequencePatternData_GS ) );
                        packet.SubPackets[0].ERR = 1;
                        packet.SubPackets[0].DATA.Add( packetArray[i] );

                        richTextBox1.AppendText( Util.BytesToString( packet ) + Environment.NewLine + Environment.NewLine );
                    }
                }
            }
        }
        private void button_SequencePacketTest_Click( object sender, EventArgs e )
        {
            Sequence sequence;
            var old = false;

            using ( var f = new Form_SequenceBuilder_Test() )
            {
                f.ShowDialog();

                old = f.Old;
                sequence = f.Result;
            }

            if ( sequence == null ) return;

            SendPacket[] packetArray = null;

            if ( old )
            {
                packetArray = sequence.ToPacketArray_Old( addr, ch );
            }
            else
            {
                packetArray = sequence.ToPacketArray( addr, ch );
            }

            richTextBox1.AppendText( "패킷 변환 테스트용 로그 메시지입니다. (이 패킷들은 실제로 송신되지 않았습니다.)\r\n" );
            for ( var i = 0; i < packetArray.Length; i++ )
            {
                richTextBox1.AppendText( $"{i}번째 패킷\r\n" );
                richTextBox1.AppendText( Util.BytesToString( packetArray[i] ) + "\r\n\r\n" );
            }
        }

        private void checkBox_Log_Click( object sender, EventArgs e )
        {
            if ( !checkBox_Log.Checked )
            {
                if ( MessageBox.Show( "로그를 종료하시겠습니까?", "", MessageBoxButtons.YesNo ) == DialogResult.No )
                {
                    checkBox_Log.Checked = true;
                    return;
                }
                else
                {
                    logPath = null;
                }
            }
            else
            {
                using ( var dialog = new SaveFileDialog()
                {
                    Filter = "*.log|*.log"
                } )
                {
                    if ( dialog.ShowDialog() == DialogResult.OK )
                    {
                        logPath = dialog.FileName;
                    }
                    else
                    {
                        checkBox_Log.Checked = false;
                    }
                }
            }
        }

        private void richTextBox1_SelectionChanged( object sender, EventArgs e )
        {
            if ( parser.IsOpen ) parser.ParsingText = richTextBox1.SelectedText;
        }

        private void timer_ThreadChecker_Tick( object sender, EventArgs e )
        {
            if ( communicator == null ) return;

            label_Ping.Text = communicator.IsPingRun ? "Run" : "Stop";
            label_Send.Text = communicator.IsSendRun ? "Run" : "Stop";
            label_Receive.Text = communicator.IsReceiveRun ? "Run" : "Stop";
            label_Split.Text = communicator.IsSplitRun ? "Run" : "Stop";
            label_Parsing.Text = communicator.IsParsingRun ? "Run" : "Stop";
        }

        private void button3_Click( object sender, EventArgs e )
        {
            var console = new McQLib.Developer.DevelopConsole();
            console.Communicators = new Communicator[1] { communicator };
            //console.Lock( "admin" );
            console.Show();
        }

        private void button2_Click( object sender, EventArgs e )
        {
            var send = new SendPacket( addr, ch );
            send.ByPass = byPass;
            send.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.M_RemoveChannelData ) );
            send.SubPacket.DATA.Add( 0 );

            var receive = communicator.SendAndReceive( send );

        }

        private void button4_Click( object sender, EventArgs e )
        {
        }
    }
}
