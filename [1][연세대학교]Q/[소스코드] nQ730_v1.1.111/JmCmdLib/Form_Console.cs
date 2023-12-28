using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace JmCmdLib
{
    internal sealed partial class Form_Console : Form
    {
        private static bool _globalLock;

        private MessageListener _listener;
        internal Form_Console( MessageListener listener )
        {
            if( _globalLock ) throw new Exception( "Console has locked due to too many password errors." );

            InitializeComponent();

            richTextBox_Log.LanguageOption = 0;

            label_Caption.Text = $"JmCmdLib Console v{FileVersionInfo.GetVersionInfo( System.Reflection.Assembly.GetExecutingAssembly().Location ).ProductVersion}";
            richTextBox_Log.Text = _information;

            _listener += preprocessing;
            _listener += listener;
        }

        private string _information = $" JmCmdLib Console [Version {FileVersionInfo.GetVersionInfo( System.Reflection.Assembly.GetExecutingAssembly().Location ).ProductVersion}]\r\n" +
                                      $" This application developed by DevJaemin.\r\n" +
                                      $" Copyright 2021. DevJaemin all rights reserved.\r\n\r\n";

        internal bool AutoScrollToCaret { get; set; } = true;

        private string _titleText;
        internal string Title
        {
            get => _titleText;
            set
            {
                _titleText = value;
                label_Caption.Text = $"JmCmdLib Console v{FileVersionInfo.GetVersionInfo( System.Reflection.Assembly.GetExecutingAssembly().Location ).ProductVersion}";
                if( _titleText != null && _titleText.Length != 0 ) label_Caption.Text += $" - {_titleText}";
            }
        }
        internal bool TimeStamp { get; set; } = true;
        internal string TimeStampFormat { get; set; } = "hh:mm:ss";

        private string _locationText;
        internal string LocationText
        {
            get => _locationText;
            set
            {
                if( value == null ) _locationText = "";
                else _locationText = value.Trim();

                if( _locationText.Length == 0 )
                {
                    label_Location.Text = ">>";
                    tableLayoutPanel3.ColumnStyles[0].Width = 24;
                }
                else
                {
                    label_Location.Text = _locationText + " >>";
                    tableLayoutPanel3.ColumnStyles[0].Width = 24 + (label_Location.Text.Length * 7);
                }
            }
        }
        internal string LogText
        {
            get => richTextBox_Log.Text;
            set => richTextBox_Log.Text = value;
        }
        internal string CommandText
        {
            get => textBox_Command.Text;
            set => textBox_Command.Text = value;
        }

        internal void Write( string value )
        {
            if( TimeStamp ) printTime();

            richTextBox_Log.AppendText( value );

            if( AutoScrollToCaret ) richTextBox_Log.ScrollToCaret();
        }
        internal void WriteLine()
        {
            richTextBox_Log.AppendText( Environment.NewLine );
            if( AutoScrollToCaret ) richTextBox_Log.ScrollToCaret();
        }
        internal void WriteLine( string value )
        {
            if( TimeStamp ) printTime();

            richTextBox_Log.AppendText( value + Environment.NewLine );

            if( AutoScrollToCaret ) richTextBox_Log.ScrollToCaret();
        }
        internal void Clear()
        {
            richTextBox_Log.Text = _information;
        }

        private string _password;
        private bool _locked = false;
        private int _failCount = 0;
        internal void Lock( string password )
        {
            if( password != null && password.Length >= 4 )
            {
                _password = password;
                _locked = true;
                textBox_Command.PasswordChar = '●';
                WriteLine( "Console has locked. Enter the password." );
            }
        }
        internal string GetHelpString()
        {
            return string.Format( " {0, -35}{1}\r\n", "!get [option_name]", "Get console option value." ) +
                   string.Format( " {0, -35}{1}\r\n", "!set[option_name][value]", "Set console option value to [value]." ) +
                   string.Format( " {0, -35}{1}\r\n", "!clear", "Clear all log messages." ) +
                   string.Format( " {0, -35}{1}\r\n", "!close", "Close the console." );
        }

        private string get( string[] args, int index )
        {
            if( index < 0 || index >= args.Length ) return string.Empty;
            else return args[index];
        }
        /// <summary>
        /// 전처리 프로세스, 콘솔창의 옵션을 제어할 수 있다.
        /// <br>명령어의 첫 문자가 '!'인 경우 전처리 프로세스에 진입한다.</br>
        /// </summary>
        /// <param name="cmd">명령어</param>
        private void preprocessing( string cmd )
        {
            if( _locked )
            {
                if( _password == cmd )
                {
                    _locked = false;
                    textBox_Command.PasswordChar = '\0';
                    _password = null;
                    _failCount = 0;
                    WriteLine( "Unlokced." );
                    throw new Exception( "Preprocessed." );
                }
                else
                {
                    _failCount++;
                    WriteLine( $"Wrong password. ({5 - _failCount})" );

                    if( _failCount == 5 )
                    {
                        _globalLock = true;
                        Close();
                    }

                    throw new Exception( "Preprocessed." );
                }
            }

            if( cmd[0] != '!' ) return;

            var split = cmd.ToLower().Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

            switch( get( split, 0 ) )
            {
                case "!get":
                    switch( get( split, 1 ) )
                    {
                        case "timestamp":
                            WriteLine( $"TimeStamp : '{TimeStamp}'" );
                            break;

                        case "timestampformat":
                            WriteLine( $"TimeStampFormat : '{TimeStampFormat}'" );
                            break;

                        case "autoscroll":
                            WriteLine( $"AutoScroll : '{AutoScrollToCaret}'" );
                            break;

                        default:
                            WriteLine( $"Can not found option name '{get( split, 1 )}'." );
                            break;
                    }
                    break;

                case "!set":
                    switch( get( split, 1 ) )
                    {
                        case "timestamp":
                            if( bool.TryParse( get( split, 2 ), out bool timeStamp ) )
                            {
                                TimeStamp = timeStamp;
                                WriteLine( $"TimeStamp set to '{TimeStamp}'." );
                            }
                            else
                            {
                                WriteLine( $"'{get( split, 2 )}' can not be converted to boolean." );
                            }
                            break;

                        case "timestampformat":
                            var format = "";
                            for( var i = 2; i < split.Length; i++ ) format += split[i] + " ";
                            format = format.Trim();

                            TimeStampFormat = format;
                            WriteLine( $"TimeStampFormat set to '{TimeStampFormat}'." );
                            break;

                        case "autoscroll":
                            if( bool.TryParse( get( split, 2 ), out bool autoScroll ) )
                            {
                                AutoScrollToCaret = autoScroll;
                                WriteLine( $"AutoScroll set to '{AutoScrollToCaret}'." );
                            }
                            else
                            {
                                WriteLine( $"'{get( split, 2 )}' can not be converted to boolean." );
                            }
                            break;

                        default:
                            WriteLine( $"Can not found option name '{get( split, 1 )}'." );
                            break;
                    }
                    break;

                case "!clear":
                    Clear();
                    break;

                case "!close":
                    Close();
                    break;

                case "!help":
                    WriteLine( string.Format( "\r\n {0, -35}{1}\r\n", "Command", "Destruction" ) + GetHelpString() );
                    break;

                default:
                    WriteLine( $"Wrong command. You can use 'help' to look all commands." );
                    break;
            }

            // Preprocess되었으면 다음 체인으로 넘어가지 않기 위해 예외를 발생시킨다.
            // 발생시킨 예외는 catch 절에서 잡아서 아무 처리하지 않도록 하자.
            throw new Exception( "Preprocessed." );
        }
        private void printTime()
        {
            if( TimeStampFormat != null )
            {
                try
                {
                    richTextBox_Log.AppendText( $"[{DateTime.Now.ToString( TimeStampFormat )}] " );
                }
                catch
                {
                    richTextBox_Log.AppendText( DateTime.Now.ToString( "[hh:mm:ss] " ) );
                }
            }
            else richTextBox_Log.AppendText( DateTime.Now.ToString( "[hh:mm:ss] " ) );
        }

        private void textBox_Command_KeyPress( object sender, KeyPressEventArgs e )
        {
            if( e.KeyChar == ( char )Keys.Enter )
            {
                if( textBox_Command.Text.Length == 0 ) return;

                var cmd = textBox_Command.Text;
                textBox_Command.Text = string.Empty;

                if( !_locked ) WriteLine( cmd );

                try
                {
                    _listener?.Invoke( cmd );
                }
                catch( Exception ex )
                {
                    if( ex.Message != "Preprocessed." )
                    {
                        WriteLine( ex.Message );
                    }
                }
            }
        }

        Point mousePoint;
        private void label_Caption_MouseDown( object sender, MouseEventArgs e )
        {
            mousePoint = new Point( e.X, e.Y );
        }
        private void label_Caption_MouseMove( object sender, MouseEventArgs e )
        {
            if( (e.Button & MouseButtons.Left) == MouseButtons.Left )
            {
                Location = new Point( Left - (mousePoint.X - e.X), Top - (mousePoint.Y - e.Y) );
            }
        }

        private void button_Close_Click( object sender, EventArgs e )
        {
            Close();
        }

        private void richTextBox_Log_KeyPress( object sender, KeyPressEventArgs e )
        {
            if( 33 <= e.KeyChar && e.KeyChar <= 126 )
            {
                textBox_Command.Text += e.KeyChar;
                textBox_Command.Focus();
                textBox_Command.SelectionStart = textBox_Command.Text.Length;
            }
        }

        #region 전역 키 커멘드 이벤트
        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            Keys key = keyData & ~(Keys.Shift | Keys.Control);

            switch( key )
            {
                case Keys.Escape:
                    Close();
                    break;
            }

            return base.ProcessCmdKey( ref msg, keyData );
        }
        #endregion
    }
}