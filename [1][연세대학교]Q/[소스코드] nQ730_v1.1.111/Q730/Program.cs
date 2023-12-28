using McQLib.Core;
using McQLib.Device;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Q730
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main( string[] args )
        {
            string currentProcessName = Process.GetCurrentProcess().ProcessName;

            // 현재 실행 중인 모든 프로세스를 가져와서 확인
            Process[] processes = Process.GetProcessesByName(currentProcessName);

            // 현재 실행 중인 프로세스가 2개 이상이라면 중복 실행으로 간주
            if (processes.Length > 1)
            {
                //Console.WriteLine("프로그램이 이미 실행 중입니다.");
                MessageBox.Show("이미 실행 중입니다.");
                return;
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            Util.StartDirectory = Application.StartupPath;

            if ( args.Length != 0 )
            {
                Application.Run( new Form_SequenceBuilder( args[0] ) );
            }
            else
            {
                while ( true )
                {
                    var form = new Form_Main();

                    Application.Run( form );

                    if ( form.TestSwStart ) Application.Run( new Tester.Form_Tester() );
                    else break;
                }
            }
        }
    }
}
