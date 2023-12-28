using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DataViewer
{
    static class Program
    {
        [DllImport( "shell32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        public static extern void SHChangeNotify( uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2 );
        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const int SHCNF_FLUSH = 0x1000;

        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main( string[] args )
        {
            //bool madeChanges = false;
            //madeChanges |= SetAssociation( ".qrd", "D730", "Q Raw Data File", "D:\\McScience\\Software\\D730\\D730.exe" );
            //if ( madeChanges ) SHChangeNotify( SHCNE_ASSOCCHANGED, SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero );

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            if ( args.Length != 0 )
            {
                Application.Run( new Form_Main( args[0] ) );
            }
            else
            {
                Application.Run( new Form_Main() );
            }
        }

        static bool SetAssociation( string extension, string progId, string fileTypeDescription, string applicationFilePath )
        {
            var reg = Registry.CurrentUser.OpenSubKey( @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts", true );
            reg?.DeleteSubKeyTree( ".qrd" );
            reg?.Close();

            reg = Registry.CurrentUser.CreateSubKey( @"Software\Classes\.qrd\shell\open\command" );
            reg?.SetValue( null, $"\"{applicationFilePath}\" \"%1\"" );
            reg?.Close();

            return true;
        }
        static bool SetKeyDefaultValue( string keyPath, string value )
        {
            using ( var key = Registry.CurrentUser.CreateSubKey( keyPath ) )
            {
                if ( key.GetValue( null ) as string != value )
                {
                    key.SetValue( null, value );
                    return true;
                }
            }

            return false;
        }
    }
}
