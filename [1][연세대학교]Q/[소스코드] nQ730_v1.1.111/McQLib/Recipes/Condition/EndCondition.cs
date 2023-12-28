using McQLib.Core;
using System;
using System.ComponentModel;

namespace McQLib.Recipes
{
    /// <summary>
    /// 종료 조건에 대한 설정값을 구성하는 클래스입니다.
    /// </summary>
    [TypeConverter( typeof( EndConverter ) )]
    public sealed class EndCondition : ICloneable
    {
        public string GetSummaryString()
        {
            var str = string.Empty;

            str += "End : ";

            // 조건의 우선순위에 따라 대표 조건 1개만 표시 (우선 순위를 바꾸려면 if/else-if절의 순서를 바꿀 것)
            if( End_Voltage != null )
            {
                str += $"{End_Voltage:0.00####}V";
            }
            else if( _end_Time != null )
            {
                str += $"{Util.ConvertMsToString( ( uint )_end_Time.Value )}";
            }
            else if( End_Current != null )
            {
                str += $"{End_Current:0.00####}{CurrentUnit}";
            }
            else if( End_Capacity_Ah != null )
            {
                str += $"{End_Capacity_Ah:0.00####}Ah";
            }
            else if( _end_CvTime != null )
            {
                str += $"{Util.ConvertMsToString( ( uint )_end_CvTime.Value )}";
            }
            else if( End_Capacity_Wh != null )
            {
                str += $"{End_Capacity_Wh}Wh";
            }
            else if( End_Power != null )
            {
                str += $"{End_Power}W";
            }
            else if( End_DeltaVoltage != null ) 
            {
                str += $"△{End_DeltaVoltage}V";
            }
            else if( End_Temperature != null )
            {
                str += $"{End_Temperature}℃";
            }
            else if( End_DeltaTemperature != null )
            {
                str += $"△{End_DeltaTemperature}℃";
            }
            else if( End_MaxCapacityRatio != null )
            {
                str += $"{End_MaxCapacityRatio}%";
            }
            else
            {
                str += "Empty";
            }

            return str;
        }

        // Sequence Builder에서 속성 그룹 상단 텍스트가 클래스명으로 표기되는 것 방지용
        public override string ToString()
        {
            return string.Empty;
        }

        public string GetDetailString()
        {
            var str = "=== End Condition ===\r\n";

            if( End_Voltage != null )           str += $"Voltage : {End_Voltage.Value:0.00####}V\r\n";
            if( End_Current != null )           str += $"Current : {End_Current.Value:0.00####}{CurrentUnit}\r\n";
            if( _end_Time != null )             str += $"Time : {Util.ConvertMsToString( (uint)_end_Time.Value )}\r\n";
            if( _end_CvTime != null )           str += $"CV Time : {Util.ConvertMsToString( (uint) _end_CvTime.Value )}\r\n";
            if( End_Capacity_Ah != null )       str += $"Capacity(Ah) : {End_Capacity_Ah.Value:0.00####}Ah\r\n";
            if( End_Capacity_Wh != null )       str += $"Capacity(Wh) : {End_Capacity_Wh.Value:0.00####}Wh\r\n";
            if( End_Power != null )             str += $"Power {End_Power.Value:0.00####}W\r\n";
            if( End_DeltaVoltage != null )      str += $"△Voltage : {End_DeltaVoltage.Value:0.00####}V\r\n";
            if( End_Temperature != null )       str += $"Temperature : {End_Temperature.Value:f2}℃\r\n";
            if( End_DeltaTemperature != null )  str += $"△Temperature : {End_DeltaTemperature:f2}℃\r\n";
            if( End_MaxCapacityRatio != null )  str += $"Max Capacity Ratio : {End_MaxCapacityRatio.Value:f2}%\r\n";

            if( str == "=== End Condition ===\r\n" ) str += "Empty\r\n";

            return str;
        }

        /// <summary>
        /// 설정된 종료 조건에 대한 정보를 패킷에 포함될 수 있는 DATA 필드 형태로 구성하여 반환합니다.
        /// </summary>
        /// <returns>안전 조건을 DATA 필드 형태로 구성한 80Byte 길이의 byte 배열입니다.</returns>
        public byte[] ToDataField()
        {
            var builder = new DataBuilder();

            // [종료 조건 - 사용 여부] 2Byte
            byte high = 0, low = 0;
            if (End_Voltage != null)
            {
                high |= 0b10000000;
            }

            if (End_Current != null)
            {
                high |= 0b01000000;
            }

            if (_end_Time != null)
            {
                high |= 0b00100000;
            }

            if (_end_CvTime != null)
            {
                high |= 0b00010000;
            }

            if (End_Capacity_Ah != null)
            {
                high |= 0b00001000;
            }

            if (End_Power != null)
            {
                high |= 0b00000100;
            }

            if (End_Capacity_Wh != null)
            {
                high |= 0b00000010;
            }

            if (End_DeltaVoltage != null)
            {
                high |= 0b00000001;
            }

            if (End_DeltaTemperature != null)
            {
                low |= 0b10000000;
            }

            if (End_Temperature != null)
            {
                low |= 0b01000000;
            }

            if (End_MaxCapacityRatio != null)
            {
                low |= 0b00100000;
            }

            if (low == 0 && high == 0)
            {
                throw new QException( QExceptionType.RECIPE_END_CONDITION_NOT_SET_ERROR );
            }
            builder.Add( high, low );

            // [종료 조건 - 값] 78Byte
            builder.Add( End_Voltage );             // 종료 V
            builder.Add( End_Current );             // 종료 I
            builder.Add( _end_Time );               // 종료 Time
            builder.Add( _end_CvTime );             // 종료 CV 시간
            builder.Add( End_Capacity_Ah );         // 종료 용량
            builder.Add( End_Power );               // 종료 P
            builder.Add( End_Capacity_Wh );         // 종료 Wh
            builder.Add( End_DeltaVoltage );        // 종료 delta-V
            builder.Add( End_DeltaTemperature );    // 종료 delta-Temp
            builder.Add( End_Temperature );         // 종료 Temp
            builder.Add( new Q_UInt16( 0 ) );       // 종료 Max 용량 비율 Monitor Step count (0 고정)
            builder.Add( End_MaxCapacityRatio );    // 종료 Max 용량 비율

            return builder;
        }

        /// <summary>
        /// 바이트 형태의 DATA 필드에서 종료 조건을 추출합니다. 이 메서드는 지정된 바이트 배열의 인덱스 138부터 217까지의 총 80개의 바이트를 사용합니다.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool FromDataField( byte[] data )
        {
            if ( data == null || data.Length < 218 ) return false;

            var position = 138;

            var high = data[position++];
            var low = data[position++];

            End_Voltage = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            End_Current = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            _end_Time = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            _end_CvTime = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            End_Capacity_Ah = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            End_Power = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            End_Capacity_Wh = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            End_DeltaVoltage = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            End_DeltaTemperature = new Q_Float( data[position++], data[position++], data[position++], data[position++] );
            End_Temperature = new Q_Float( data[position++], data[position++], data[position++], data[position++] );
            position += 2;
            End_MaxCapacityRatio = new Q_Float( data[position++], data[position++], data[position++], data[position++] );

            if ( ( high & 0b10000000 ) == 0 ) End_Voltage = null;
            if ( ( high & 0b01000000 ) == 0 ) End_Current = null;
            if ( ( high & 0b00100000 ) == 0 ) _end_Time = null;
            if ( ( high & 0b00010000 ) == 0 ) _end_CvTime = null;
            if ( ( high & 0b00001000 ) == 0 ) End_Capacity_Ah = null;
            if ( ( high & 0b00000100 ) == 0 ) End_Power = null;
            if ( ( high & 0b00000010 ) == 0 ) End_Capacity_Wh = null;
            if ( ( high & 0b00000001 ) == 0 ) End_DeltaVoltage = null;

            if ( ( low & 0b10000000 ) == 0 ) End_DeltaTemperature = null;
            if ( ( low & 0b01000000 ) == 0 ) End_Temperature = null;
            if ( ( low & 0b00100000 ) == 0 ) End_MaxCapacityRatio = null;

            return true;
        }

        public object Clone()
        {
            var clone = new EndCondition();

            //clone.EndCondition_Type = EndCondition_Type;
            clone.End_Voltage = End_Voltage;
            clone.End_Current = End_Current;
            clone.CurrentUnit = CurrentUnit;
            clone._end_Time = _end_Time;
            clone._end_CvTime = _end_CvTime;
            clone.End_Capacity_Ah = End_Capacity_Ah;
            clone.End_Power = End_Power;
            clone.End_Capacity_Wh = End_Capacity_Wh;
            clone.End_DeltaVoltage = End_DeltaVoltage;
            clone.End_DeltaTemperature = End_DeltaTemperature;
            clone.End_Temperature = End_Temperature;
            clone.End_MaxCapacityRatio = End_MaxCapacityRatio;


            return clone;
        }
        //public void Refresh()
        //{

        //    switch (_endCondition_Type)
        //    {
        //        case EndCondition_Type.시간:
        //            Util.SetBrowsable(this, "End_Time", true);
        //            Util.SetBrowsable(this, "End_Current", false);
        //            Util.SetBrowsable(this, "CurrentUnit", false);
        //            Util.SetBrowsable(this, "End_Voltage", false);
        //            End_Voltage = null;
        //            End_Current = null;
        //            return;

        //        case EndCondition_Type.전류:
        //            Util.SetBrowsable(this, "End_Time", false);
        //            Util.SetBrowsable(this, "End_Current", true);
        //            Util.SetBrowsable(this, "CurrentUnit", true);
        //            Util.SetBrowsable(this, "End_Voltage", false);
        //            End_Time = null;
        //            End_Voltage = null;
        //            return;

        //        case EndCondition_Type.전압:
        //            Util.SetBrowsable(this, "End_Time", false);
        //            Util.SetBrowsable(this, "End_Current", false);
        //            Util.SetBrowsable(this, "CurrentUnit", false);
        //            Util.SetBrowsable(this, "End_Voltage", true);
        //            End_Time = null;
        //            End_Current = null;
        //            return;
        //    }
        //}
        #region Properties

        //[Category("End Condition")]
        //[DisplayName("종료 조건")]
        //[Description("종료 조건을 선택해주세요.")]
        //[ID("FF010C")]
        //[RefreshProperties(RefreshProperties.All)]
        //public EndCondition_Type EndCondition_Type
        //{
        //    get
        //    {
        //        return _endCondition_Type;
        //    }
        //    set
        //    {
        //        _endCondition_Type = value;
        //        Refresh();
        //    }
        //}
        //private EndCondition_Type _endCondition_Type = EndCondition_Type.시간;

        [Category( "End Condition" )]
        [DisplayName( "Voltage(V)" )]
        [Description( "Voltage(V)가 지정된 값에 도달하면 측정을 종료합니다." )]
        [ID( "FF0100" )]
        //[Browsable(false)]
        public double? End_Voltage { get; set; }

        [Category( "End Condition" )]
        [DisplayName( "Current" )]
        [Description( "Current가 지정된 값에 도달하면 측정을 종료합니다." )]
        [ID( "FF0101" )]
        //[Browsable(false)]
        public double? End_Current { get; set; }

        [Category("End Condition")]
        [DisplayName("Current Unit")]
        [Description("인가할 Current 단위 입니다.")]
        [ID("FF010B")]
        //[Browsable(false)]
        public SourcingType_CurrentUnit CurrentUnit
        {
            get
            {
                return _currentUnit;
            }
            set
            {
                _currentUnit = value;
            }
        }
        private SourcingType_CurrentUnit _currentUnit = SourcingType_CurrentUnit.mA;

        [Category( "End Condition" )]
        [DisplayName( "시간" )]
        [Description( "Time이 지정된 값만큼 경과하면 측정을 종료합니다. 시간:분:초 순입니다." )]
        [ID( "FF0102" )]
        [Browsable(true)]
        public string End_Time
        {
            get
            {
                if( !_end_Time.HasValue ) return null;

                return Util.ConvertMsToString( (uint) _end_Time.Value );
            }
            set
            {
                _end_Time = Util.ConvertTimsStringToMs( value );

                //var timeSpan = Util.ConvertStringToMs( value );

                //if( timeSpan == null ) _end_Time = null;
                //else _end_Time = timeSpan.Value.TotalMilliseconds;
            }
        }
        private double? _end_Time;

        [Category( "End Condition" )]
        [DisplayName( "CV Time" )]
        [Description( "CV Time이 지정된 값만큼 경과하면 측정을 종료합니다. 시간:분:초 순입니다." )]
        [ID( "FF0103" )]
        [Browsable(false)]
        public string End_CvTime
        {
            get
            {
                if( !_end_CvTime.HasValue ) return null;

                return Util.ConvertMsToString( (uint) _end_CvTime.Value );

                //else return TimeSpan.FromMilliseconds( _end_CvTime.Value );
            }
            set
            {
                _end_CvTime = Util.ConvertTimsStringToMs( value );

                //var timeSpan = Util.ConvertStringToMs( value );

                //if( timeSpan == null ) _end_CvTime = null;
                //else _end_CvTime = timeSpan.Value.TotalMilliseconds;
            }
        }
        private double? _end_CvTime;

        [Category( "End Condition" )]
        [DisplayName( "Capacity(Ah)" )]
        [Description( "Capacity(Ah)가 지정된 값에 도달하면 측정을 종료합니다." )]
        [ID( "FF0104" )]
        [Browsable(false)]
        public double? End_Capacity_Ah { get; set; }

        [Category( "End Condition" )]
        [DisplayName( "Power(W)" )]
        [Description( "Power(W)가 지정된 값에 도달하면 측정을 종료합니다." )]
        [ID( "FF0105" )]
        [Browsable(false)]
        public double? End_Power { get; set; }

        [Category( "End Condition" )]
        [DisplayName( "Capacity(Wh)" )]
        [Description( "Capacity(Wh)가 지정된 값에 도달하면 측정을 종료합니다." )]
        [ID( "FF0106" )]
        [Browsable(false)]
        public double? End_Capacity_Wh { get; set; }

        [Category( "End Condition" )]
        [DisplayName( "△Voltage(V)" )]
        [Description( "Voltage 변화량(V)이 지정된 값에 도달하면 측정을 종료합니다." )]
        [ID( "FF0107" )]
        [Browsable(false)]
        public double? End_DeltaVoltage { get; set; }

        [Category( "End Condition" )]
        [DisplayName( "△Temperature(℃)" )]
        [Description( "온도 변화량(℃)이 지정된 값에 도달하면 측정을 종료합니다." )]
        [ID( "FF0108" )]
        [Browsable(false)]
        public float? End_DeltaTemperature { get; set; }

        [Category( "End Condition" )]
        [DisplayName( "Temperature(℃)" )]
        [Description( "온도(℃)가 지정된 값에 도달하면 측정을 종료합니다." )]
        [ID( "FF0109" )]
        [Browsable(false)]
        public float? End_Temperature { get; set; }

        [Category( "End Condition" )]
        [DisplayName( "Capacity Radio(%)" )]
        [Description( "용량의 비율이 지정된 값에 도달할 경우 측정을 종료합니다." )]
        [ID( "FF010A" )]
        [Browsable(false)]
        public float? End_MaxCapacityRatio { get; set; }
        #endregion
    }

    public class EndConverter : TypeConverter
    {
        public override bool GetPropertiesSupported( ITypeDescriptorContext context )
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(EndCondition));

            // Save_Temperature 속성을 제외한 속성들을 필터링하여 반환합니다.
            PropertyDescriptorCollection filteredProperties = new PropertyDescriptorCollection(null);
            foreach (PropertyDescriptor property in properties)
            {

                if (property.IsBrowsable)
                {
                    filteredProperties.Add(property);
                }
            }

            return filteredProperties;
        }
    }
}
