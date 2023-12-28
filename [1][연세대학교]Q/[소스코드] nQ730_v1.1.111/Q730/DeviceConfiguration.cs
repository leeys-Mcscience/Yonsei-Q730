using McQLib.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace Q730
{
    public class DeviceInfo
    {
        public string IP;
        public int ChannelCount;

        public bool Logging;
    }

    public static class DeviceConfiguration
    {
        public static string SaveFilePath = Path.Combine( Util.StartDirectory, "Device.config" );
        public static readonly List<DeviceInfo> Devices = new List<DeviceInfo>();

        public static void Save()
        {
            using (var sw = new StreamWriter( SaveFilePath ) )
            {
                for(var i = 0; i < Devices.Count; i++ )
                {
                    sw.WriteLine( $"{Devices[i].IP}?{Devices[i].ChannelCount}" );
                }
            }
        }
        public static void Load()
        {
            if( !new FileInfo( SaveFilePath ).Exists ) return;

            Devices.Clear();
            using(var sr = new StreamReader( SaveFilePath ) )
            {
                var lines = sr.ReadToEnd().Split( Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

                for(var i = 0; i < lines.Length; i++ )
                {
                    var split = lines[i].Split( '?' );
                    try
                    {
                        Devices.Add( new DeviceInfo()
                        {
                            IP = split[0],
                            ChannelCount = int.Parse( split[1] )
                        } );
                    }
                    catch( Exception ex ) { }
                }
            }
        }
    }
}
