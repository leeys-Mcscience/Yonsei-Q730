using System;
using System.IO;
using System.Collections.Generic;
using McQLib.Recipes;
using McQLib.Core;

namespace McQLib.IO
{
    public sealed class QDataReader : QDataStream
    {
        private Func<MeasureData> _readMeasureData;
        private Func<Sequence> _readSequenceData;
        /// <summary>
        /// _position 위치부터 시작해서 패킷을 하나 읽어옵니다.
        /// <br>패킷의 STX, ETX, LEN은 제거되며, 반환되는 배열은 TYPE과 DATA 필드로만 구성되어 있습니다.</br>
        /// </summary>
        /// <returns></returns>
        private Func<byte[]> _read;
        private Func<RecipeType, bool, string> _getColumns;
        private Func<MeasureData, string> _toDataString;

        public readonly Sequence Sequence;
        // 파일로부터 데이터를 한 번에 다 읽어온 후 처리하도록 구현을 하긴 했으나, 이건 나중에 position을 사용하도록 바꿀 필요가 있을 수도 있음.
        // (파일 크기가 커질 경우 전체 데이터를 메모리에 계속 유지하는 과정에서 예상치 못한 문제가 발생할 수도 있기 때문에)
        public QDataReader( string filename )
        {
            _stream = new FileStream( filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite );

            if ( _stream.Length >= 2 )
            {
                _major = ( byte )_stream.ReadByte();
                _minor = ( byte )_stream.ReadByte(); // _data[1];
            }

            selectVersion();

            Sequence = _readSequenceData?.Invoke();
        }

        public static void ConvertQrdToCsv( string qrdFilePath, string csvFilePath )
        {
            var reader = new QDataReader( qrdFilePath );

            var datas = reader.ReadToEnd();

            using ( var sw = new StreamWriter( csvFilePath ) )
            {
                uint? stepCount = null;

                for ( var i = 0; i < datas.Length; i++ )
                {
                    if ( stepCount == null || stepCount != datas[i].StepCount )
                    {
                        sw.WriteLine( $"{Environment.NewLine}{datas[i].RecipeType}" );
                        sw.WriteLine( $"Step Count,{datas[i].StepCount},Step Number,{datas[i].StepNumber}" );
                        sw.WriteLine( $"{reader._getColumns?.Invoke( datas[i].RecipeType, datas[i].IsRaw )}" );
                        stepCount = datas[i].StepCount;
                    }

                    sw.WriteLine( $"{reader._toDataString?.Invoke( datas[i] )}" );
                }
            }
        }

        /// <summary>
        /// 지정된 숫자만큼의 데이터를 스킵합니다.
        /// <br>이미 열었던 파일을 다시 열어 새로 갱신된 부분부터 가져오고 싶을 때 사용합니다.</br>
        /// </summary>
        /// <param name="count">스킵할 데이터의 개수입니다.</param>
        public void Skip( int count )
        {
            for ( var i = 0; i < count; i++ ) _read();
        }
        /// <summary>
        /// 스트림으로부터 하나의 데이터를 읽어와 <see cref="MeasureData"/> 형식으로 변환하여 반환합니다.
        /// </summary>
        /// <returns>읽어온 데이터를 <see cref="MeasureData"/> 형식으로 변환한 값이거나, 스트림의 끝을 읽은 경우 null입니다.</returns>
        public MeasureData Read()
        {
            //if ( _readMeasureData == null ) return null;

            while ( true )
            {
                try
                {
                    MeasureData data = _readMeasureData.Invoke();
                    return data;
                    //if ( data == null ) return null;
                    //else return data;
                }
                catch ( QException ex )
                {
#if CONSOLEOUT
                    Console.WriteLine( "Data read failed. Wrong data format." );
#endif
                    continue;
                }
            }
        }
        public MeasureData[] ReadToEnd()
        {
            var list = new List<MeasureData>();

            MeasureData data;
            while ( ( data = Read() ) != null ) list.Add( data );

            return list.ToArray();
        }
        public MeasureData[] ReadToEndAsync( System.Threading.CancellationTokenSource source )
        {
            var list = new List<MeasureData>();

            MeasureData data;
            while ( !source.IsCancellationRequested && ( data = Read() ) != null ) list.Add( data );

            if ( source.IsCancellationRequested ) return null;
            else return list.ToArray();
        }

        protected override sealed bool selectVersion()
        {
            switch ( _major )
            {
                case 0:
                    switch ( _minor )
                    {
                        case 1:
                            _readMeasureData = readMeasureData_0_1;
                            _getColumns = getColumns_0_1;
                            _toDataString = toDataString_0_1;
                            _read = read_0_1;
                            return true;

                        case 2:
                            _readMeasureData = readMeasureData_0_2;
                            _readSequenceData = readSequenceData_0_2;
                            _getColumns = getColumns_0_2;
                            _toDataString = toDataString_0_2;
                            _read = read_0_2;
                            return true;

                        case 3:
                            _readMeasureData = readMeasureData_0_3;
                            _readSequenceData = readSequenceData_0_3;
                            _getColumns = getColumns_0_3;
                            _toDataString = toDataString_0_3;
                            _read = read_0_3;
                            return true;
                    }
                    break;
            }

            return false;
        }

        #region Version 0.1
        private string getColumns_0_1( RecipeType recipeType, bool isRaw )
        {
            switch ( recipeType )
            {
                case RecipeType.Rest:
                    return "Data Index,Total Time(sec),Step Time(sec),Voltage(V),Temperature(℃)";

                case RecipeType.Charge:
                case RecipeType.Discharge:
                case RecipeType.OpenCircuitVoltage:
                    return "Data Index,Total Time(sec),Step Time(sec),Voltage(V),Current(A),Capacity(Ah),Power(W),Watt Hour(Wh),Temperature(℃)";

                case RecipeType.AcResistance:
                    if ( isRaw ) return "Step Time,Voltage,Current,Temperature";
                    else return "Data Index,Frequency(Hz),Z(Ω),Phase,Z_Real,Z_Img,Start OCV,End OCV";

                case RecipeType.DcResistance:
                    if ( isRaw ) return "Step Time,Voltage,Current,Temperature";
                    else return "V - 1,I - 1,V - 2,I - 2,R";

                case RecipeType.TransientResponse:
                    return "Step Time,Voltage,Current,Temperature";

                case RecipeType.FrequencyResponse:
                    if ( isRaw ) return "Step Time,Voltage,Current";
                    else return "Frequency,Z,Phase,Z_Real,Z_Img";
            }

            return string.Empty;
        }

        private ulong _lastTotalTime;
        private ulong _lastStepTime;
        private ulong _lastStepCount;
        /// <summary>
        /// MeasureData를 데이터의 레시피 타입마다 지정된 규칙에 따라 문자열로 변환합니다.
        /// <br>각 데이터는 콤마로 구분되어 있으며, 구분된 각 데이터들이 의미하는 값은 <see cref="getColumns_0_1(RecipeType, bool)"/>로 확인 가능합니다.</br>
        /// </summary>
        /// <param name="data">문자열로 변환할 <see cref="MeasureData"/> 형식 값입니다.</param>
        /// <returns></returns>
        private string toDataString_0_1( MeasureData data )
        {
            if ( _lastStepCount != data.StepCount )
            {
                if ( data.StepCount == 0 )
                {
                    _lastTotalTime = 0;
                }

                _lastStepTime = 0;
                _lastStepCount = data.StepCount;
            }

            _lastStepTime = data.TotalTime - _lastTotalTime + _lastStepTime;

            _lastTotalTime = data.TotalTime;

            switch ( data.RecipeType )
            {
                case RecipeType.Rest:
                    return $"{data.DataIndex}," +
                           $"{data.TotalTime / 1000.0:f2},{( data.TotalTime - _lastTotalTime + _lastStepTime ) / 1000.0:f2}," +
                           $"{data.Voltage},{data.Temperature}";

                case RecipeType.Charge:
                case RecipeType.Discharge:
                case RecipeType.OpenCircuitVoltage:
                    return $"{data.DataIndex}," +
                           $"{data.TotalTime / 1000.0:f2},{( data.TotalTime - _lastTotalTime + _lastStepTime ) / 1000.0:f2}," +
                           $"{data.Voltage},{data.Current},{data.Capacity},{data.Power},{data.WattHour},{data.Temperature}";

                case RecipeType.AcResistance:
                    if ( data.IsRaw )
                    {
                        return $"{data.StepTime},{data.Voltage},{data.Current},{data.Temperature}";
                    }
                    else
                    {
                        return $"{data.DataIndex}," +
                               $"{data.Frequency},{data.Z},{data.Phase},{data.Z_Real},{data.Z_Img},{data.StartOcv},{data.EndOcv}";
                    }

                case RecipeType.DcResistance:
                    if ( data.IsRaw )
                    {
                        return $"{data.StepTime},{data.Voltage},{data.Current},{data.Temperature}";
                    }
                    else
                    {
                        return $"{data.V1},{data.I1},{data.V2},{data.I2},{data.R}";
                    }

                case RecipeType.FrequencyResponse:
                    if ( data.IsRaw )
                    {
                        return $"{data.StepTime},{data.Voltage},{data.Current},{data.Temperature}";
                    }
                    else
                    {
                        return $"{data.Frequency},{data.Z},{data.Phase},{data.Z_Real},{data.Z_Img}";
                    }

                case RecipeType.TransientResponse:
                    return $"{data.StepTime},{data.Voltage},{data.Current},{data.Temperature}";
            }

            return string.Empty;
        }
        private MeasureData readMeasureData_0_1()
        {
            var bytes = _read();

            if ( bytes == null || bytes.Length == 0 ) return null;

            var p = 0;
            MeasureData data;
            try
            {
                switch ( ( DataType )bytes[p++] )
                {
                    case DataType.Rest:
                        return data = new MeasureData( RecipeType.Rest )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field10 = getUInt32( bytes, ref p ),   // Data Index
                            _field4 = getUInt32( bytes, ref p ),    // Total Time
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.Charge:
                        return data = new MeasureData( RecipeType.Charge )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field10 = getUInt32( bytes, ref p ),   // Data Index
                            _field4 = getUInt32( bytes, ref p ),    // Total Time
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field14 = getDouble( bytes, ref p ),   // Capacity
                            _field15 = getDouble( bytes, ref p ),   // Power
                            _field16 = getDouble( bytes, ref p ),   // WattHour
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.Discharge:
                        return data = new MeasureData( RecipeType.Discharge )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field10 = getUInt32( bytes, ref p ),   // Data Index
                            _field4 = getUInt32( bytes, ref p ),    // Total Time
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field14 = getDouble( bytes, ref p ),   // Capacity
                            _field15 = getDouble( bytes, ref p ),   // Power
                            _field16 = getDouble( bytes, ref p ),   // WattHour
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.OpenCircuitVoltage:
                        return data = new MeasureData( RecipeType.OpenCircuitVoltage )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field10 = getUInt32( bytes, ref p ),   // Data Index
                            _field4 = getUInt32( bytes, ref p ),    // Total Time
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field14 = getDouble( bytes, ref p ),   // Capacity
                            _field15 = getDouble( bytes, ref p ),   // Power
                            _field16 = getDouble( bytes, ref p ),   // WattHour
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.AcResistance:
                        return data = new MeasureData( RecipeType.AcResistance )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count   
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field10 = getUInt32( bytes, ref p ),   // Data Index
                            _field13 = getDouble( bytes, ref p ),   // Frequency
                            _field14 = getDouble( bytes, ref p ),   // Z
                            _field15 = getDouble( bytes, ref p ),   // Phase
                            _field16 = getDouble( bytes, ref p ),   // Z_Real
                            _field17 = getDouble( bytes, ref p ),   // Z_Img
                            _field18 = getDouble( bytes, ref p ),   // Start OCV
                            _field19 = getDouble( bytes, ref p ),   // End OCV
                        };

                    case DataType.AcResistance_Raw:
                        return data = new MeasureData( RecipeType.AcResistance )
                        {
                            IsRaw = true,
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field4 = getUInt32( bytes, ref p ),    // Step Time
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.DcResistance:
                        return data = new MeasureData( RecipeType.DcResistance )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field16 = getDouble( bytes, ref p ),   // V-1
                            _field18 = getDouble( bytes, ref p ),   // I-1
                            _field17 = getDouble( bytes, ref p ),   // V-2
                            _field19 = getDouble( bytes, ref p ),   // I-2
                            _field15 = getDouble( bytes, ref p ),   // R
                        };

                    case DataType.DcResistance_Raw:
                        return data = new MeasureData( RecipeType.DcResistance )
                        {
                            IsRaw = true,
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field4 = getUInt32( bytes, ref p ),    // Step Time
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.FrequencyResponse:
                        return data = new MeasureData( RecipeType.FrequencyResponse )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field13 = getDouble( bytes, ref p ),   // Frequency
                            _field14 = getDouble( bytes, ref p ),   // Z
                            _field15 = getDouble( bytes, ref p ),   // Phase
                            _field16 = getDouble( bytes, ref p ),   // Z_Real
                            _field17 = getDouble( bytes, ref p ),   // Z_Img
                        };

                    case DataType.FrequencyResponse_Raw:
                        return data = new MeasureData( RecipeType.FrequencyResponse )
                        {
                            IsRaw = true,
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field4 = getUInt32( bytes, ref p ),    // Step Time
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.TransientResponse:
                        return data = new MeasureData( RecipeType.TransientResponse )
                        {
                            IsRaw = true,
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field4 = getUInt32( bytes, ref p ),    // Step Time
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };
                }
            }
            catch
            {
                throw new QException( QExceptionType.IO_WRONG_DATA_FORMAT_ERROR );
            }

            return null;
        }
        private byte[] read_0_1()
        {
            if ( _stream == null ) return null;

            var list = new List<byte>();
            while ( true )
            {
                list.Clear();

                // 정상 패킷이 아닌 경우 FileStream.Position을 시작했던 위치 + 1로 되돌리기 위해 현재 루프에서 읽은 바이트 개수를 기억해둔다.
                int readCount = 0;
                int b = _stream.ReadByte();
                readCount++;

                // 읽은 바이트가 -1이라면 파일의 끝을 읽었으므로 무조건 null을 반환한다. (되돌릴 필요 없음)
                if ( b == -1 ) return null;

                // STX 검사
                if ( b != 0x02 )
                {
                    _stream.Position -= readCount - 1;
                    // STX가 아니면 다시 처음부터 (단, FileStream.Position은 처음 읽었던 위치 + 1부터)
                    continue;
                }

                b = _stream.ReadByte();
                if ( b == -1 ) return null;
                readCount++;

                // LEN 읽기
                byte len = ( byte )b;

                // LEN만큼 DATA 읽기
                var buffer = new byte[len];

                var read = _stream.Read( buffer, 0, len );
                readCount += read;

                if ( read != len ) return null;

                list.AddRange( buffer );

                b = _stream.ReadByte();
                if ( b == -1 ) return null;
                readCount++;

                // ETX 검사
                if ( b == 0x03 )
                {
                    return list.ToArray();
                }
                else if ( Array.IndexOf( buffer, 0x02 ) == -1 )
                {
                    // 여태까지 읽은 버퍼에 0x02가 없다면 다음 패킷을 검색하기 위해 Position을 되돌릴 필요 없다.
                    continue;
                }
                else
                {
                    _stream.Position -= readCount - 1;
                    // ETX가 아니면 다시 처음부터 (단, FileStream.Position은 처음 읽었던 위치 + 1부터)
                    continue;
                }
            }
        }
        #endregion

        #region Version 0.2
        private string getColumns_0_2( RecipeType recipeType, bool isRaw ) => getColumns_0_1( recipeType, isRaw );
        private string toDataString_0_2( MeasureData data ) => toDataString_0_1( data );
        private MeasureData readMeasureData_0_2() => readMeasureData_0_1();
        private Sequence readSequenceData_0_2()
        {
            var seqInfo = _read();

            if ( seqInfo == null || seqInfo.Length == 0 ) return null;

            var position = 0;
            if ( seqInfo[position++] != SEQ )
            {
                _stream.Position -= seqInfo.Length;
                return null;
            }

            var seq = new Sequence();

            var count = getInt32( seqInfo, ref position );
            for ( var i = 0; i < count; i++ )
            {
                var recipe = readRecipeData_0_2();
                if ( recipe != null ) seq.Add( recipe );
            }

            return seq;
        }
        private Recipe readRecipeData_0_2()
        {
            var recipeInfo = _read();

            if ( recipeInfo == null || recipeInfo.Length == 0 ) return null;

            var position = 0;
            if ( recipeInfo[position++] != RCP )
            {
                _stream.Position -= recipeInfo.Length;
                return null;
            }

            var recipe = RecipeFactory.CreateInstance( ( RecipeType )recipeInfo[position++] );

            if ( recipe == null ) return null;

            switch ( recipe.GetRecipeType() )
            {
                case RecipeType.Charge:
                    var c = recipe as Charge;
                    c.SourcingType = ( SourcingType_Charge )recipeInfo[position++];

                    switch ( c.SourcingType )
                    {
                        case SourcingType_Charge.CC:
                            c.Current = getDouble( recipeInfo, ref position );
                            break;

                        case SourcingType_Charge.CCCV:
                            c.Current = getDouble( recipeInfo, ref position );
                            c.Voltage = getDouble( recipeInfo, ref position );
                            break;

                        case SourcingType_Charge.CP:
                            c.Power = getDouble( recipeInfo, ref position );
                            break;

                        case SourcingType_Charge.CR:
                            c.Resistance = getDouble( recipeInfo, ref position );
                            break;
                    }
                    break;

                case RecipeType.Discharge:
                    var d = recipe as Discharge;
                    d.SourcingType = ( SourcingType_Discharge )recipeInfo[position++];

                    switch ( d.SourcingType )
                    {
                        case SourcingType_Discharge.CC:
                            d.Current = getDouble( recipeInfo, ref position );
                            break;

                        case SourcingType_Discharge.CP:
                            d.Power = getDouble( recipeInfo, ref position );
                            break;

                        case SourcingType_Discharge.CR:
                            d.Resistance = getDouble( recipeInfo, ref position );
                            break;
                    }
                    break;


                case RecipeType.Loop:
                    var l = recipe as Loop;
                    l.LoopCount = getUInt32( recipeInfo, ref position );
                    break;

                case RecipeType.Jump:
                    var j = recipe as Jump;
                    j.JumpCount = getUInt32( recipeInfo, ref position );
                    break;
            }

            return recipe;
        }
        private byte[] read_0_2() => read_0_1();
        #endregion

        #region Version 0.3
        private string getColumns_0_3( RecipeType recipeType, bool isRaw ) => getColumns_0_1( recipeType, isRaw );
        private string toDataString_0_3( MeasureData data ) => toDataString_0_1( data );
        private MeasureData readMeasureData_0_3()
        {
            var bytes = _read();

            if ( bytes == null || bytes.Length == 0 ) return null;

            var p = 0;
            MeasureData data;

            try
            {
                switch ( ( DataType )bytes[p++] )
                {
                    case DataType.Rest:
                        return data = new MeasureData( RecipeType.Rest )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field4 = getUInt32( bytes, ref p ),    // Total Time
                            _field21 = bytes[p++],                  // TotalTimeOverFlow
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.Charge:
                        return data = new MeasureData( RecipeType.Charge )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field4 = getUInt32( bytes, ref p ),    // Total Time
                            _field21 = bytes[p++],                  // TotalTimeOverFlow
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field14 = getDouble( bytes, ref p ),   // Capacity
                            _field15 = getDouble( bytes, ref p ),   // Power
                            _field16 = getDouble( bytes, ref p ),   // WattHour
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.Discharge:
                        return data = new MeasureData( RecipeType.Discharge )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field4 = getUInt32( bytes, ref p ),    // Total Time
                            _field21 = bytes[p++],                  // TotalTimeOverFlow
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field14 = getDouble( bytes, ref p ),   // Capacity
                            _field15 = getDouble( bytes, ref p ),   // Power
                            _field16 = getDouble( bytes, ref p ),   // WattHour
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.AnodeCharge:
                        return data = new MeasureData( RecipeType.AnodeCharge )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field4 = getUInt32( bytes, ref p ),    // Total Time
                            _field21 = bytes[p++],                  // TotalTimeOverFlow
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field14 = getDouble( bytes, ref p ),   // Capacity
                            _field15 = getDouble( bytes, ref p ),   // Power
                            _field16 = getDouble( bytes, ref p ),   // WattHour
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.AnodeDischarge:
                        return data = new MeasureData( RecipeType.AnodeDischarge )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field4 = getUInt32( bytes, ref p ),    // Total Time
                            _field21 = bytes[p++],                  // TotalTimeOverFlow
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field14 = getDouble( bytes, ref p ),   // Capacity
                            _field15 = getDouble( bytes, ref p ),   // Power
                            _field16 = getDouble( bytes, ref p ),   // WattHour
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.OpenCircuitVoltage:
                        return data = new MeasureData( RecipeType.OpenCircuitVoltage )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field4 = getUInt32( bytes, ref p ),    // Total Time
                            _field21 = bytes[p++],                  // TotalTimeOverFlow
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field14 = getDouble( bytes, ref p ),   // Capacity
                            _field15 = getDouble( bytes, ref p ),   // Power
                            _field16 = getDouble( bytes, ref p ),   // WattHour
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.AcResistance:
                        return data = new MeasureData( RecipeType.AcResistance )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count   
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field13 = getDouble( bytes, ref p ),   // Frequency
                            _field14 = getDouble( bytes, ref p ),   // Z
                            _field15 = getDouble( bytes, ref p ),   // Phase
                            _field16 = getDouble( bytes, ref p ),   // Z_Real
                            _field17 = getDouble( bytes, ref p ),   // Z_Img
                            _field18 = getDouble( bytes, ref p ),   // Start OCV
                            _field19 = getDouble( bytes, ref p ),   // End OCV
                        };

                    case DataType.AcResistance_Raw:
                        return data = new MeasureData( RecipeType.AcResistance )
                        {
                            IsRaw = true,
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field4 = getUInt32( bytes, ref p ),    // Step Time
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.DcResistance:
                        return data = new MeasureData( RecipeType.DcResistance )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field16 = getDouble( bytes, ref p ),   // V-1
                            _field18 = getDouble( bytes, ref p ),   // I-1
                            _field17 = getDouble( bytes, ref p ),   // V-2
                            _field19 = getDouble( bytes, ref p ),   // I-2
                            _field15 = getDouble( bytes, ref p ),   // R
                        };

                    case DataType.DcResistance_Raw:
                        return data = new MeasureData( RecipeType.DcResistance )
                        {
                            IsRaw = true,
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field4 = getUInt32( bytes, ref p ),    // Step Time
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.FrequencyResponse:
                        return data = new MeasureData( RecipeType.FrequencyResponse )
                        {
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field13 = getDouble( bytes, ref p ),   // Frequency
                            _field14 = getDouble( bytes, ref p ),   // Z
                            _field15 = getDouble( bytes, ref p ),   // Phase
                            _field16 = getDouble( bytes, ref p ),   // Z_Real
                            _field17 = getDouble( bytes, ref p ),   // Z_Img
                        };

                    case DataType.FrequencyResponse_Raw:
                        return data = new MeasureData( RecipeType.FrequencyResponse )
                        {
                            IsRaw = true,
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field4 = getUInt32( bytes, ref p ),    // Step Time
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };

                    case DataType.TransientResponse:
                        return data = new MeasureData( RecipeType.TransientResponse )
                        {
                            IsRaw = true,
                            _field5 = getUInt32( bytes, ref p ),    // Step Count
                            _field6 = getUInt16( bytes, ref p ),    // Step Number
                            _field4 = getUInt32( bytes, ref p ),    // Step Time
                            _field11 = getDouble( bytes, ref p ),   // Voltage
                            _field12 = getDouble( bytes, ref p ),   // Current
                            _field13 = getDouble( bytes, ref p ),   // Temperature
                        };
                }
            }
            catch ( Exception ex )
            {
                throw new QException( QExceptionType.IO_WRONG_DATA_FORMAT_ERROR );
            }

            return null;
        }
        private Sequence readSequenceData_0_3()
        {
            var seqInfo = _read();

            if ( seqInfo == null || seqInfo.Length == 0 ) return null;

            var position = 0;
            if ( seqInfo[position++] != SEQ )
            {
                _stream.Position -= seqInfo.Length;
                return null;
            }

            var seq = new Sequence();

            var count = getInt32( seqInfo, ref position );
            for ( var i = 0; i < count; i++ )
            {
                var recipe = readRecipeData_0_3();
                if ( recipe != null ) seq.Add( recipe );
                else break;
            }

            return seq;
        }
        private Recipe readRecipeData_0_3()
        {
            var recipeInfo = _read();

            if ( recipeInfo == null || recipeInfo.Length == 0 ) return null;

            var position = 0;
            if ( recipeInfo[position++] != RCP )
            {
                _stream.Position -= recipeInfo.Length;
                return null;
            }

            // Parsing
            var data = new byte[recipeInfo.Length - 1];
            Array.Copy( recipeInfo, 1, data, 0, recipeInfo.Length - 1 );

            return RecipeFactory.CreateInstance( data );
        }
        private byte[] read_0_3()
        {
            if ( _stream == null ) return null;

            var list = new List<byte>();
            while ( true )
            {
                list.Clear();

                // 정상 패킷이 아닌 경우 FileStream.Position을 시작했던 위치 + 1로 되돌리기 위해 현재 루프에서 읽은 바이트 개수를 기억해둔다.
                int readCount = 0;
                int b = _stream.ReadByte();
                readCount++;

                // 읽은 바이트가 -1이라면 파일의 끝을 읽었으므로 무조건 null을 반환한다. (되돌릴 필요 없음)
                if ( b == -1 ) return null;

                // STX 검사
                if ( b != 0x02 )
                {
                    _stream.Position -= readCount - 1;
                    // STX가 아니면 다시 처음부터 (단, FileStream.Position은 처음 읽었던 위치 + 1부터)
                    continue;
                }

                b = _stream.ReadByte();
                if ( b == -1 ) return null;
                readCount++;

                // LEN 읽기
                byte len1 = ( byte )b;

                b = _stream.ReadByte();
                if ( b == -1 ) return null;
                readCount++;

                byte len2 = ( byte )b;

                // 레시피 길이가 260byte인데 오버플로우로 인해 길이가 5로 들어간 옛 버전 오류 - 깔끔하게 포기하고 새로 진행하자
                //if( len1 == 0x05 && len2 != SEQ )
                //{
                //    list.Add( len2 );

                //    len1 = 0x01;
                //    len2 = 0x04;
                //}
                //else if (len2 > 0x01 )
                //{
                //    list.Add( len2 );

                //    len2 = (byte)(len1 - 1);
                //    len1 = 0;
                //}

                ushort len = new Q_UInt16( len1, len2 );

                // LEN만큼 DATA 읽기
                var buffer = new byte[len];

                var read = _stream.Read( buffer, 0, len );
                readCount += read;

                if ( read != len ) return null;

                list.AddRange( buffer );

                b = _stream.ReadByte();
                if ( b == -1 ) return null;
                readCount++;

                // ETX 검사
                if ( b == 0x03 )
                {
                    return list.ToArray();
                }
                else if ( Array.IndexOf( buffer, 0x02 ) == -1 )
                {
                    // 여태까지 읽은 버퍼에 0x02가 없다면 다음 패킷을 검색하기 위해 Position을 되돌릴 필요 없다.
                    continue;
                }
                else
                {
                    _stream.Position -= readCount - 1;
                    // ETX가 아니면 다시 처음부터 (단, FileStream.Position은 처음 읽었던 위치 + 1부터)
                    continue;
                }
            }
        }
        #endregion

        #region Byte Converting Functions
        private int getInt32( byte[] array, ref int position )
        {
            var result = BitConverter.ToInt32( array, position );
            position += sizeof( int );
            return result;
        }
        private uint getUInt32( byte[] array, ref int position )
        {
            var result = BitConverter.ToUInt32( array, position );
            position += sizeof( uint );
            return result;
        }
        private ushort getUInt16( byte[] array, ref int position )
        {
            var result = BitConverter.ToUInt16( array, position );
            position += sizeof( ushort );
            return result;
        }
        private double getDouble( byte[] array, ref int position )
        {
            var result = BitConverter.ToDouble( array, position );
            position += sizeof( double );
            return result;
        }
        private float getFloat( byte[] array, ref int position )
        {
            var result = BitConverter.ToSingle( array, position );
            position += sizeof( float );
            return result;
        }
        private ulong getUInt64( byte[] array, ref int position )
        {
            var result = BitConverter.ToUInt64( array, position );
            position += sizeof( ulong );
            return result;
        }
        #endregion
    }
}
