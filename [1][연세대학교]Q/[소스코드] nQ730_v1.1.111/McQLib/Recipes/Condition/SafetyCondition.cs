using McQLib.Core;
using System;
using System.ComponentModel;

namespace McQLib.Recipes
{
    /// <summary>
    /// 안전 조건에 대한 설정값을 구성하는 클래스입니다.
    /// </summary>
    [TypeConverter( typeof( SafetyConverter ) )]
    public sealed class SafetyCondition : ICloneable
    {
        public string GetSummaryString()
        {
            var str = string.Empty;

            str += "Safety : ";

            // 조건의 우선순위에 따라 대표 조건 1개만 표시 (우선 순위를 바꾸려면 if/else-if절의 순서를 바꿀 것)
            if( Safety_MaxVoltage != null )
            {
                str += $"↑{Safety_MaxVoltage:0.00####}V";
            }
            else if( Safety_MinVoltage != null )
            {
                str += $"↓{Safety_MinVoltage:0.00####}V";
            }
            else if( Safety_MaxCurrent != null )
            {
                str += $"↑{Safety_MaxCurrent:0.00####}{CurrentUnit}";
            }
            else if( Safety_MinCurrent != null )
            {
                str += $"↓{Safety_MinCurrent:0.00####}{CurrentUnit}";
            }
            else if( Safety_MaxCapacity_Wh != null )
            {
                str += $"↑{Safety_MaxCapacity_Wh:0.00####}Wh";
            }
            else if( Safety_MaxCapacity_Ah != null )
            {
                str += $"↑{Safety_MaxCapacity_Ah:0.00####}Ah";
            }
            else if( Safety_MaxTemperature != null )
            {
                str += $"↑{Safety_MaxTemperature}℃";
            }
            else if( Safety_MinTemperature != null )
            {
                str += $"↓{Safety_MinTemperature}℃";
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
            var str = "=== Safety Condition ===\r\n";

            if( Safety_MaxVoltage != null )     str += $"Max Voltage : {Safety_MaxVoltage:0.00####}V\r\n";
            if( Safety_MinVoltage != null )     str += $"Min Voltage : {Safety_MinVoltage:0.00####}V\r\n";
            if( Safety_MaxCurrent != null )     str += $"Max Current : {Safety_MinVoltage:0.00####}{CurrentUnit}\r\n";
            if( Safety_MinCurrent != null )     str += $"Min Current : {Safety_MinCurrent:0.00####}{CurrentUnit}\r\n";
            if( Safety_MaxCapacity_Ah != null ) str += $"Max Capacity(Ah) : {Safety_MaxCapacity_Ah:0.00####}Ah\r\n";
            if( Safety_MaxCapacity_Wh != null ) str += $"Max Capacity(Wh) : {Safety_MaxCapacity_Wh:0.00####}Wh\r\n";
            if( Safety_MaxTemperature != null ) str += $"Max Temperature : {Safety_MaxTemperature:0.00####}℃\r\n";
            if( Safety_MinTemperature != null ) str += $"Min Temperature : {Safety_MinTemperature:0.00####}℃\r\n";

            if( str == "=== Safety Condition ===\r\n" ) str += "Empty\r\n";

            return str;
        }

        /// <summary>
        /// 설정된 안전 조건에 대한 정보를 패킷에 포함될 수 있는 DATA 필드 형태로 구성하여 반환합니다.
        /// </summary>
        /// <returns>안전 조건을 DATA 필드 형태로 구성한 58Byte 길이의 byte 배열입니다.</returns>
        public byte[] ToDataField()
        {
            var builder = new DataBuilder();

            // [안전 조건 - 사용 항목] (2Byte)
            byte high = 0, low = 0;
            if( Safety_MaxVoltage != null ) high |= 0b10000000;
            if( Safety_MinVoltage != null ) high |= 0b01000000;
            if( Safety_MaxCurrent != null ) high |= 0b00100000;
            if( Safety_MinCurrent != null ) high |= 0b00010000;
            if( Safety_MaxCapacity_Ah != null ) high |= 0b00001000;
            if( Safety_MaxCapacity_Wh != null ) high |= 0b00000100;
            if( Safety_MaxTemperature != null ) high |= 0b00000010;
            if( Safety_MinTemperature != null ) high |= 0b00000001;

            if( (high | low) == 0 ) throw new QException( QExceptionType.RECIPE_SAFETY_CONDITION_NOT_SET_ERROR );
            builder.Add( high, low );

            // [안전 조건 - 값] (56Byte)
            builder.Add( Safety_MaxVoltage );       // 안전 최대 V
            builder.Add( Safety_MinVoltage );       // 안전 최소 V
            builder.Add( Safety_MaxCurrent );       // 안전 최대 I
            builder.Add( Safety_MinCurrent );       // 안전 최소 I
            builder.Add( Safety_MaxCapacity_Ah );   // 안전 최대 용량(Ah)
            builder.Add( Safety_MaxCapacity_Wh );   // 안전 최대 용량(Wh)
            builder.Add( Safety_MaxTemperature );   // 안전 최대 온도
            builder.Add( Safety_MinTemperature );   // 안전 최소 온도


            if (Safety_MaxVoltage == null || Safety_MinVoltage == null)
            {
                throw new QException(QExceptionType.RECIPE_SAFETY_CONDITION_NOT_SET_ERROR);
            }
            return builder;
        }
        

        /// <summary>
        /// 바이트 형태의 DATA 필드에서 안전 조건을 추출합니다. 이 메서드는 지정된 바이트 배열의 인덱스 80부터 137까지의 총 58개의 바이트를 사용합니다.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool FromDataField( byte[] data )
        {
            if ( data == null || data.Length < 138 ) return false;

            var position = 80;

            var high = data[position++];
            var low = data[position++];

            Safety_MaxVoltage = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            Safety_MinVoltage = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            Safety_MaxCurrent = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            Safety_MinCurrent = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            Safety_MaxCapacity_Ah = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            Safety_MaxCapacity_Wh = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            Safety_MaxTemperature = new Q_Float( data[position++], data[position++], data[position++], data[position++] );
            Safety_MinTemperature = new Q_Float( data[position++], data[position++], data[position++], data[position++] );

            if ( ( high & 0b10000000 ) == 0 ) Safety_MaxVoltage = null;
            if ( ( high & 0b01000000 ) == 0 ) Safety_MinVoltage = null;
            if ( ( high & 0b00100000 ) == 0 ) Safety_MaxCurrent = null;
            if ( ( high & 0b00010000 ) == 0 ) Safety_MinCurrent = null;
            if ( ( high & 0b00001000 ) == 0 ) Safety_MaxCapacity_Ah = null;
            if ( ( high & 0b00000100 ) == 0 ) Safety_MaxCapacity_Wh = null;
            if ( ( high & 0b00000010 ) == 0 ) Safety_MaxTemperature = null;
            if ( ( high & 0b00000001 ) == 0 ) Safety_MinTemperature = null;

            return true;
        }

        public object Clone()
        {
            var clone = new SafetyCondition();

            clone.Safety_MaxVoltage = Safety_MaxVoltage;
            clone.Safety_MinVoltage = Safety_MinVoltage;
            clone.Safety_MaxCurrent = Safety_MaxCurrent;
            clone.Safety_MinCurrent = Safety_MinCurrent;
            clone.CurrentUnit = CurrentUnit;
            clone.Safety_MaxCapacity_Ah = Safety_MaxCapacity_Ah;
            clone.Safety_MaxCapacity_Wh = Safety_MaxCapacity_Wh;
            clone.Safety_MaxTemperature = Safety_MaxTemperature;
            clone.Safety_MinTemperature = Safety_MinTemperature;

            return clone;
        }
        public void CopyFrom( SafetyCondition source )
        {
            if( source == null ) return;

            Safety_MaxVoltage = source.Safety_MaxVoltage;
            Safety_MinVoltage = source.Safety_MinVoltage;
            Safety_MaxCurrent = source.Safety_MaxCurrent;
            Safety_MinCurrent = source.Safety_MinCurrent;
            CurrentUnit = source.CurrentUnit;
            Safety_MaxCapacity_Ah = source.Safety_MaxCapacity_Ah;
            Safety_MaxCapacity_Wh = source.Safety_MaxCapacity_Wh;
            Safety_MaxTemperature = source.Safety_MaxTemperature;
            Safety_MinTemperature = source.Safety_MinTemperature;
        }

        #region Properties
        [Category( "Safety Condition" )]
        [DisplayName( "Max Voltage(V)" )]
        [Description( "Voltage(V)가 지정된 값에 도달하면 측정을 강제로 중단합니다." )]
        [ID("FF0000")]
        public double? Safety_MaxVoltage { get; set; }

        [Category( "Safety Condition" )]
        [DisplayName( "Min Voltage(V)" )]
        [Description( "Voltage(V)가 지정된 값에 도달하면 측정을 강제로 중단합니다." )]
        [ID( "FF0001" )]
        public double? Safety_MinVoltage { get; set; }

        [Category( "Safety Condition" )]
        [DisplayName( "Max Current" )]
        [Description( "Current가 지정된 값에 도달하면 측정을 강제로 중단합니다." )]
        [ID( "FF0002" )]
        [Browsable(false)]
        public double? Safety_MaxCurrent { get; set; }

        [Category( "Safety Condition" )]
        [DisplayName( "Min Current" )]
        [Description( "Current가 지정된 값에 도달하면 측정을 강제로 중단합니다." )]
        [ID( "FF0003" )]
        [Browsable(false)]
        public double? Safety_MinCurrent { get; set; }

        [Category("Safety Condition")]
        [DisplayName("Current Unit")]
        [Description("인가할 Current 단위 입니다.")]
        [ID("FF0008")]
        [Browsable(false)]
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

        [Category( "Safety Condition" )]
        [DisplayName( "Max Capacity(Ah)" )]
        [Description( "Capacity(Ah)가 지정된 값에 도달하면 측정을 강제로 중단합니다." )]
        [ID( "FF0004" )]
        [Browsable(false)]
        public double? Safety_MaxCapacity_Ah { get; set; }

        [Category( "Safety Condition" )]
        [DisplayName( "Max Capacity(Wh)" )]
        [Description( "Capacity(Wh)가 지정된 값에 도달하면 측정을 강제로 중단합니다." )]
        [ID( "FF0005" )]
        [Browsable(false)]
        public double? Safety_MaxCapacity_Wh { get; set; }

        [Category( "Safety Condition" )]
        [DisplayName( "Max Temperature(℃)" )]
        [Description( "Temperature(℃)가 지정된 값에 도달하면 측정을 강제로 중단합니다." )]
        [ID( "FF0006" )]
        [Browsable(false)]
        public float? Safety_MaxTemperature { get; set; }

        [Category( "Safety Condition" )]
        [DisplayName( "Min Temperature(℃)" )]
        [Description( "Temperature(℃)가 지정된 값에 도달하면 측정을 강제로 중단합니다." )]
        [ID( "FF0007" )]
        [Browsable(false)]
        public float? Safety_MinTemperature { get; set; }
        #endregion
    }

    public class SafetyConverter : TypeConverter
    {
        public override bool GetPropertiesSupported( ITypeDescriptorContext context )
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            //PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(SafetyCondition));

            //// Save_Temperature 속성을 제외한 속성들을 필터링하여 반환합니다.
            //PropertyDescriptorCollection filteredProperties = new PropertyDescriptorCollection(null);
            //foreach (PropertyDescriptor property in properties)
            //{
                 
            //    if (property.IsBrowsable)
            //    {
            //        filteredProperties.Add(property);
            //    }
            //}

            //return filteredProperties;

            return TypeDescriptor.GetProperties(typeof(SafetyCondition), attributes);
        }
    }
}
