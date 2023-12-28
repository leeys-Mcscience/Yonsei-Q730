using System;
using System.Collections.Generic;
using System.IO;
using McQLib.Recipes;
using McQLib.Core;

namespace McQLib.IO
{
    public sealed class QDataWriter : QDataStream
    {
        private Action<byte[]> _write;
        private Action<MeasureData> _writeMeasureData;
        private Action<Sequence> _writeSequenceData;
        private Action _writeHeaderData;

        private QDataWriter() { }

        public static QDataWriter Open( string filename )
        {
            if ( File.Exists( filename ) )
            {
                var writer = new QDataWriter();

                var fileVersionExists = false;
                var reader = new FileStream( filename, FileMode.Open, FileAccess.Read );
                if ( reader.Length > 1 )
                {
                    writer._major = ( byte )reader.ReadByte();
                    writer._minor = ( byte )reader.ReadByte();

                    if ( !writer.selectVersion() ) return null;
                }
                else
                {
                    fileVersionExists = true;
                }
                reader.Close();

                writer._stream = new FileStream( filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite );

                if ( !fileVersionExists )
                {
                    writer._stream.Position = 0;
                    writer._stream.WriteByte( writer._major );
                    writer._stream.WriteByte( writer._minor );
                }

                writer._stream.Position = writer._stream.Length;

                return writer;
            }
            else
            {
                return QDataWriter.Create( filename );
            }
        }
        /// <summary>
        /// 새로운 Q 측정 데이터 파일을 생성합니다.
        /// <br>기존에 생성된 파일이 이미 존재한다면 새로운 파일로 교체됩니다.</br>
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="sequence"></param>
        public static QDataWriter Create( string filename, Sequence sequence = null )
        {
            var writer = new QDataWriter();

            try
            {
                writer._stream = new FileStream( filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite );
            }
            catch
            {
                return null;
            }

            writer._stream.WriteByte( writer._major );
            writer._stream.WriteByte( writer._minor );
            writer._stream.Flush();

            if ( !writer.selectVersion() )
            {
                return null;
            }

            writer._writeHeaderData?.Invoke();

            if ( sequence != null )
            {
                writer._writeSequenceData?.Invoke( sequence );
            }

            return writer;
        }

        /// <summary>
        /// <see cref="MeasureData"/> 형식 데이터를 미리 정의된 형식의 바이트 배열로 변환하여 스트림에 쓰고, 실제로 쓰인 바이트 배열을 반환합니다.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void Write( MeasureData data )
        {
            _writeMeasureData?.Invoke( data );
        }

        protected override sealed bool selectVersion()
        {
            switch ( _major )
            {
                case 0:
                    switch ( _minor )
                    {
                        case 1:
                            _write = write_0_1;
                            _writeMeasureData = writeMeasureData_0_1;
                            _writeHeaderData = null;
                            return true;

                        case 2:
                            _write = write_0_2;
                            _writeMeasureData = writeMeasureData_0_2;
                            _writeHeaderData = null;
                            _writeSequenceData = writeSequenceData_0_2;
                            return true;

                        case 3:
                            _write = write_0_3;
                            _writeMeasureData = writeMeasureData_0_3;
                            _writeHeaderData = null;
                            _writeSequenceData = writeSequenceData_0_3;
                            return true;
                    }
                    break;
            }

            return false;
        }

        #region Version 0.1
        private void writeMeasureData_0_1( MeasureData data )
        {
            var tmp = new List<byte>();

            switch ( data.RecipeType )
            {
                case RecipeType.Rest:
                    tmp.Add( ( byte )DataType.Rest );
                    tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                    tmp.AddRange( BitConverter.GetBytes( data.DataIndex ) );
                    tmp.AddRange( BitConverter.GetBytes( data.TotalTime_Uint ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    break;

                case RecipeType.Charge:
                    tmp.Add( ( byte )DataType.Charge );
                    tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                    tmp.AddRange( BitConverter.GetBytes( data.DataIndex ) );
                    tmp.AddRange( BitConverter.GetBytes( data.TotalTime_Uint ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Capacity ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Power ) );
                    tmp.AddRange( BitConverter.GetBytes( data.WattHour ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    break;

                //_field5 = getUInt32( bytes, ref p ),    // Step Count
                //        _field6 = getUInt16( bytes, ref p ),    // Step Number
                //        _field10 = getUInt32( bytes, ref p ),   // Data Index
                //        _field4 = getUInt32( bytes, ref p ),    // Total Time
                //        _field11 = getDouble( bytes, ref p ),   // Voltage
                //        _field12 = getDouble( bytes, ref p ),   // Current
                //        _field14 = getDouble( bytes, ref p ),   // Capacity
                //        _field15 = getDouble( bytes, ref p ),   // Power
                //        _field16 = getDouble( bytes, ref p ),   // WattHour
                //        _field13 = getDouble( bytes, ref p ),   // Temperature

                case RecipeType.Discharge:
                    tmp.Add( ( byte )DataType.Discharge );
                    tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                    tmp.AddRange( BitConverter.GetBytes( data.DataIndex ) );
                    tmp.AddRange( BitConverter.GetBytes( data.TotalTime_Uint ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Capacity ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Power ) );
                    tmp.AddRange( BitConverter.GetBytes( data.WattHour ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    break;

                case RecipeType.AcResistance:
                    if ( data.IsRaw )
                    {
                        tmp.Add( ( byte )DataType.AcResistance_Raw );
                        tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepTime_Uint ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    }
                    else
                    {
                        tmp.Add( ( byte )DataType.AcResistance );
                        tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                        tmp.AddRange( BitConverter.GetBytes( data.DataIndex ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Frequency ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Z ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Phase ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Z_Real ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Z_Img ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StartOcv ) );
                        tmp.AddRange( BitConverter.GetBytes( data.EndOcv ) );
                    }
                    break;

                case RecipeType.DcResistance:
                    if ( data.IsRaw )
                    {
                        tmp.Add( ( byte )DataType.DcResistance );
                        tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepTime_Uint ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    }
                    else
                    {
                        tmp.Add( ( byte )DataType.DcResistance_Raw );
                        tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                        tmp.AddRange( BitConverter.GetBytes( data.V1 ) );
                        tmp.AddRange( BitConverter.GetBytes( data.I1 ) );
                        tmp.AddRange( BitConverter.GetBytes( data.V2 ) );
                        tmp.AddRange( BitConverter.GetBytes( data.I2 ) );
                        tmp.AddRange( BitConverter.GetBytes( data.R ) );
                    }
                    break;

                case RecipeType.TransientResponse:
                    tmp.Add( ( byte )DataType.TransientResponse );
                    tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepTime_Uint ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    break;

                case RecipeType.FrequencyResponse:
                    if ( data.IsRaw )
                    {
                        tmp.Add( ( byte )DataType.FrequencyResponse_Raw );
                        tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepTime_Uint ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    }
                    else
                    {
                        tmp.Add( ( byte )DataType.FrequencyResponse );
                        tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Frequency ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Z ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Phase ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Z_Real ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Z_Img ) );
                    }
                    break;

                case RecipeType.OpenCircuitVoltage:
                    tmp.Add( ( byte )DataType.OpenCircuitVoltage );
                    tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                    tmp.AddRange( BitConverter.GetBytes( data.DataIndex ) );
                    tmp.AddRange( BitConverter.GetBytes( data.TotalTime_Uint ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Capacity ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Power ) );
                    tmp.AddRange( BitConverter.GetBytes( data.WattHour ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    break;

                case RecipeType.Pattern:
                    tmp.Add( ( byte )DataType.Pattern );
                    tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                    tmp.AddRange( BitConverter.GetBytes( data.TotalTime_Uint ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    break;
            }

            if ( tmp.Count != 0 ) _write( tmp.ToArray() );
        }
        private void write_0_1( byte[] bytes )
        {
            _stream.WriteByte( STX );          // STX
            _stream.WriteByte( ( byte )bytes.Length );          // LEN
            _stream.Write( bytes, 0, bytes.Length );            // DATA
            _stream.WriteByte( ETX );          // ETX
            _stream.Flush();
        }
        #endregion

        #region Version 0.2
        private void writeMeasureData_0_2( MeasureData data )
        {
            writeMeasureData_0_1( data );
        }
        private void writeSequenceData_0_2( Sequence seq )
        {
            if ( seq == null ) return;

            var tmp = new List<byte>();
            tmp.Add( SEQ );
            tmp.AddRange( BitConverter.GetBytes( seq.Count ) );

            _write( tmp.ToArray() );

            for ( var i = 0; i < seq.Count; i++ )
            {
                writeReipce_0_2( seq[i] );
            }
        }
        private void writeReipce_0_2( Recipe recipe )
        {
            var type = recipe.GetRecipeType();

            var tmp = new List<byte>();
            tmp.Add( RCP );
            tmp.Add( ( byte )type );

            switch ( type )
            {
                case RecipeType.Charge:
                    var c = recipe as Charge;
                    tmp.Add( ( byte )c.SourcingType );
                    switch ( c.SourcingType )
                    {
                        case SourcingType_Charge.CC:
                            tmp.AddRange( BitConverter.GetBytes( c.Current ) );
                            break;

                        case SourcingType_Charge.CCCV:
                            tmp.AddRange( BitConverter.GetBytes( c.Current ) );
                            tmp.AddRange( BitConverter.GetBytes( c.Voltage ) );
                            break;

                        case SourcingType_Charge.CP:
                            tmp.AddRange( BitConverter.GetBytes( c.Power ) );
                            break;

                        case SourcingType_Charge.CR:
                            tmp.AddRange( BitConverter.GetBytes( c.Resistance ) );
                            break;
                    }
                    break;

                case RecipeType.Discharge:
                    var d = recipe as Discharge;
                    tmp.Add( ( byte )d.SourcingType );
                    switch ( d.SourcingType )
                    {
                        case SourcingType_Discharge.CC:
                            tmp.AddRange( BitConverter.GetBytes( d.Current ) );
                            break;

                        case SourcingType_Discharge.CP:
                            tmp.AddRange( BitConverter.GetBytes( d.Power ) );
                            break;

                        case SourcingType_Discharge.CR:
                            tmp.AddRange( BitConverter.GetBytes( d.Resistance ) );
                            break;
                    }
                    break;

                case RecipeType.Rest:
                    break;

                case RecipeType.Loop:
                    var l = recipe as Loop;
                    tmp.AddRange( BitConverter.GetBytes( l.LoopCount ) );
                    break;

                case RecipeType.Jump:
                    var j = recipe as Jump;
                    tmp.AddRange( BitConverter.GetBytes( j.JumpCount ) );
                    break;
            }

            _write( tmp.ToArray() );
        }
        private void write_0_2( byte[] bytes ) => write_0_1( bytes );
        #endregion

        #region Version 0.3
        private void writeMeasureData_0_3( MeasureData data )
        {
            var tmp = new List<byte>();

            switch ( data.RecipeType )
            {
                case RecipeType.Rest:
                    tmp.Add( ( byte )DataType.Rest );
                    tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                    tmp.AddRange( BitConverter.GetBytes( data.TotalTime_Uint ) );
                    tmp.Add( data.TotalTimeOverflow );
                    tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    break;

                case RecipeType.Charge:
                    tmp.Add( ( byte )DataType.Charge );
                    tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                    tmp.AddRange( BitConverter.GetBytes( data.TotalTime_Uint ) );
                    tmp.Add( data.TotalTimeOverflow );
                    tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Capacity ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Power ) );
                    tmp.AddRange( BitConverter.GetBytes( data.WattHour ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    break;

                case RecipeType.Discharge:
                    tmp.Add( ( byte )DataType.Discharge );
                    tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                    tmp.AddRange( BitConverter.GetBytes( data.TotalTime_Uint ) );
                    tmp.Add( data.TotalTimeOverflow );
                    tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Capacity ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Power ) );
                    tmp.AddRange( BitConverter.GetBytes( data.WattHour ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    break;

                case RecipeType.AnodeCharge:
                    tmp.Add( ( byte )DataType.AnodeCharge );
                    tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                    tmp.AddRange( BitConverter.GetBytes( data.TotalTime_Uint ) );
                    tmp.Add( data.TotalTimeOverflow );
                    tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Capacity ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Power ) );
                    tmp.AddRange( BitConverter.GetBytes( data.WattHour ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    break;

                case RecipeType.AnodeDischarge:
                    tmp.Add( ( byte )DataType.AnodeDischarge );
                    tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                    tmp.AddRange( BitConverter.GetBytes( data.TotalTime_Uint ) );
                    tmp.Add( data.TotalTimeOverflow );
                    tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Capacity ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Power ) );
                    tmp.AddRange( BitConverter.GetBytes( data.WattHour ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    break;

                case RecipeType.OpenCircuitVoltage:
                    tmp.Add( ( byte )DataType.OpenCircuitVoltage );
                    tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                    tmp.AddRange( BitConverter.GetBytes( data.TotalTime_Uint ) );
                    tmp.Add( data.TotalTimeOverflow );
                    tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Capacity ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Power ) );
                    tmp.AddRange( BitConverter.GetBytes( data.WattHour ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    break;

                case RecipeType.AcResistance:
                    if ( data.IsRaw )
                    {
                        tmp.Add( ( byte )DataType.AcResistance_Raw );
                        tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepTime_Uint ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    }
                    else
                    {
                        tmp.Add( ( byte )DataType.AcResistance );
                        tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Frequency ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Z ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Phase ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Z_Real ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Z_Img ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StartOcv ) );
                        tmp.AddRange( BitConverter.GetBytes( data.EndOcv ) );
                    }
                    break;

                case RecipeType.DcResistance:
                    if ( data.IsRaw )
                    {
                        tmp.Add( ( byte )DataType.DcResistance );
                        tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepTime_Uint ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    }
                    else
                    {
                        tmp.Add( ( byte )DataType.DcResistance_Raw );
                        tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                        tmp.AddRange( BitConverter.GetBytes( data.V1 ) );
                        tmp.AddRange( BitConverter.GetBytes( data.I1 ) );
                        tmp.AddRange( BitConverter.GetBytes( data.V2 ) );
                        tmp.AddRange( BitConverter.GetBytes( data.I2 ) );
                        tmp.AddRange( BitConverter.GetBytes( data.R ) );
                    }
                    break;

                case RecipeType.TransientResponse:
                    tmp.Add( ( byte )DataType.TransientResponse );
                    tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepTime_Uint ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    break;

                case RecipeType.FrequencyResponse:
                    if ( data.IsRaw )
                    {
                        tmp.Add( ( byte )DataType.FrequencyResponse_Raw );
                        tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepTime_Uint ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    }
                    else
                    {
                        tmp.Add( ( byte )DataType.FrequencyResponse );
                        tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                        tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Frequency ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Z ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Phase ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Z_Real ) );
                        tmp.AddRange( BitConverter.GetBytes( data.Z_Img ) );
                    }
                    break;

                case RecipeType.Pattern:
                    tmp.Add( ( byte )DataType.Pattern );
                    tmp.AddRange( BitConverter.GetBytes( data.StepCount ) );
                    tmp.AddRange( BitConverter.GetBytes( data.StepNumber ) );
                    tmp.AddRange( BitConverter.GetBytes( data.TotalTime_Uint ) );
                    tmp.Add( data.TotalTimeOverflow );
                    tmp.AddRange( BitConverter.GetBytes( data.Voltage ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Current ) );
                    tmp.AddRange( BitConverter.GetBytes( data.Temperature ) );
                    break;
            }

            if ( tmp.Count != 0 ) _write( tmp.ToArray() );
        }
        private void writeSequenceData_0_3( Sequence seq )
        {
            if ( seq == null ) return;

            var packets = seq.ToPacketArray_Old( 0, 0 );

            var tmp = new List<byte>();
            tmp.Add( SEQ );
            tmp.AddRange( BitConverter.GetBytes( packets.Length ) );

            _write( tmp.ToArray() );

            for ( var i = 0; i < packets.Length; i++ )
            {
                tmp = new List<byte>();
                tmp.Add( RCP );
                tmp.AddRange( packets[i].SubPacket.DATA.ToByteArray() );

                _write( tmp.ToArray() );
            }
        }
        private void write_0_3( byte[] bytes )
        {
            _stream.WriteByte( STX );                           // STX
            var len = new Q_UInt16( ( ushort )bytes.Length );

            _stream.WriteByte( len.Offset0 );                   // LEN
            _stream.WriteByte( len.Offset1 );

            _stream.Write( bytes, 0, bytes.Length );            // DATA
            _stream.WriteByte( ETX );                           // ETX
            _stream.Flush();
        }
        #endregion
    }
}
