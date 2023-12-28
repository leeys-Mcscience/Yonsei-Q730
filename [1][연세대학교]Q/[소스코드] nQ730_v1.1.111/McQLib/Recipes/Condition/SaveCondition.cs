using McQLib.Core;
using System;
using System.ComponentModel;

namespace McQLib.Recipes
{
    /// <summary>
    /// 기록 조건에 대한 설정값을 구성하는 클래스입니다.
    /// </summary>
    [TypeConverter( typeof( SaveConverter ) )]
    public class SaveCondition : ICloneable
    {
        public string GetSummaryString()
        {
            var str = string.Empty;

            str += "Save : ";

            // 조건의 우선순위에 따라 대표 조건 1개만 표시 (우선 순위를 바꾸려면 if/else-if절의 순서를 바꿀 것)
            if( _save_Interval != null )
            {
                str += $"{Util.ConvertMsToString(_save_Interval.Value)}";
            }
            else if( Save_Voltage != null )
            {
                str += $"△{Save_Voltage:f2}V";
            }
            else if( Save_Current != null )
            {
                str += $"△{Save_Current:f2}{CurrentUnit}";
            }
            else if( Save_Temperature != null )
            {
                str += $"△{Save_Temperature:f2}℃";
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
            var str = "=== Save Condition ===\r\n";

            if( _save_Interval != null )    str += $"Interval : { Util.ConvertMsToString(_save_Interval.Value)}\r\n";
            if( Save_Voltage != null )      str += $"△Voltage : { Save_Voltage.Value:0.00####}V\r\n";
            if( Save_Current != null )      str += $"△Current : { Save_Current.Value:0.00####}{CurrentUnit}\r\n";
            if( Save_Temperature != null )  str += $"△Temperature : { Save_Temperature.Value:f2}℃\r\n";

            if( str == "=== Save Condition ===\r\n" ) str += "Empty\r\n";

            return str;
        }

        /// <summary>
        /// 설정된 기록 조건에 대한 정보를 패킷에 포함될 수 있는 DATA 필드 형태로 구성하여 반환합니다.
        /// </summary>
        /// <returns>안전 조건을 DATA 필드 형태로 구성한 26Byte 길이의 byte 배열입니다.</returns>
        public byte[] ToDataField()
        {
            var builder = new DataBuilder();

            // [기록 조건 - 사용 여부] 2Byte
            byte high = 0, low = 0;
            if( _save_Interval != null ) high |= 0b10000000;
            if( Save_Voltage != null ) high |= 0b01000000;
            if( Save_Current != null ) high |= 0b00100000;
            if( Save_Temperature != null ) high |= 0b00010000;
            builder.Add( high, low );

            // [기록 조건 - 값] 24Byte
            builder.Add( _save_Interval );
            //if (SaveCondition_Type == SaveCondition_Type.시간)
            //{
                if (_save_Interval < 1000 || _save_Interval == null)
                {
                    throw new QException(QExceptionType.RECIPE_SAVE_CONDITION_INTERVAL_TOO_SMALL_ERROR);
                }
            //}// 기록 Interval
          

            builder.Add( Save_Voltage );            // 기록 delta-V
            builder.Add( Save_Current );            // 기록 delta-I
            builder.Add( Save_Temperature );        // 기록 delta-Temp

            return builder;
        }

        /// <summary>
        /// 바이트 형태의 DATA 필드에서 종료 조건을 추출합니다. 이 메서드는 지정된 바이트 배열의 인덱스 218부터 243까지의 총 26개의 바이트를 사용합니다.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool FromDataField( byte[] data )
        {
            if ( data == null || data.Length < 244 ) return false;

            var position = 218;

            var high = data[position++];
            var low = data[position++];

            _save_Interval = new Q_UInt32( data[position++], data[position++], data[position++], data[position++] );
            Save_Voltage = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            Save_Current = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            Save_Temperature = new Q_Float( data[position++], data[position++], data[position++], data[position++] );

            if ( ( high & 0b10000000 ) == 0 ) _save_Interval = null;
            if ( ( high & 0b01000000 ) == 0 ) Save_Voltage = null;
            if ( ( high & 0b00100000 ) == 0 ) Save_Current = null;
            if ( ( high & 0b00010000 ) == 0 ) Save_Temperature = null;

            return true;
        }

        public object Clone()
        {
            var clone = new SaveCondition();

            //clone.SaveCondition_Type = _saveConditionType;
            clone._save_Interval = _save_Interval;
            clone.Save_Current = Save_Current;
            clone.CurrentUnit = CurrentUnit;
            clone.Save_Voltage = Save_Voltage;
            clone.Save_Temperature = Save_Temperature;

            return clone;
        }

        #region Porperties


        //[Category("Save Condition")]
        //[DisplayName("저장 조건")]
        //[Description("Time이 지정된 값만큼 경과할 때마다 데이터를 저장합니다. 시간:분:초 순입니다.")]
        //[ID("FF0205")]
        //[RefreshProperties(RefreshProperties.All)]
        //public SaveCondition_Type SaveCondition_Type
        //{
        //    get
        //    {
        //        return _saveConditionType;
        //    }
        //    set
        //    {
        //        _saveConditionType = value;

        //        Refresh();
        //    }
        //}
        //private SaveCondition_Type _saveConditionType = SaveCondition_Type.시간;


        [Category("Save Condition")]
        [DisplayName("시간")]
        [Description("Time이 지정된 값만큼 경과할 때마다 데이터를 저장합니다. 시간:분:초 순입니다.")]
        [ID("FF0200")]
        [Browsable(true)]
        public string Save_Interval
        {
            get
            {
                if (!_save_Interval.HasValue) return null;

                return Util.ConvertMsToString(_save_Interval.Value);  // ms를 초로 변환하여 반환
            }
            set
            {
                _save_Interval = Util.ConvertTimsStringToMs(value);  // 초를 ms로 변환하여 저장
            }
        }
        private uint? _save_Interval;


        [Category( "Save Condition" )]
        [DisplayName( "△Current" )]
        [Description( "Current가 지정된 값만큼 변화할 때마다 데이터를 저장합니다." )]
        [ID( "FF0201" )]
        [Browsable(true)]
        public double? Save_Current { get; set; }

        [Category("Save Condition")]
        [DisplayName("Current Unit")]
        [Description("인가할 Current 단위 입니다.")]
        [ID("FF0204")]
        [Browsable(true)]
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

      
        [Category( "Save Condition" )]
        [DisplayName( "△Voltage(V)" )]
        [Description( "Voltage(V)가 지정된 값만큼 변화할 때마다 데이터를 저장합니다." )]
        [ID( "FF0202" )]
        [Browsable(true)]
        public double? Save_Voltage { get; set; }

   
        [Category( "Save Condition" )]
        [DisplayName( "△Temperature(℃)" )]
        [Description( "온도(℃)가 지정된 값만큼 변화할 때마다 데이터를 저장합니다." )]
        [ID( "FF0203" )]
        [Browsable(false)]
        public float? Save_Temperature { get; set; }


        #endregion

        public void Refresh()
        {
           
            //switch (_saveConditionType)
            //{
            //    case SaveCondition_Type.시간:
            //        Util.SetBrowsable(this, "Save_Interval", true);
            //        Util.SetBrowsable(this, "Save_Current", false);
            //        Util.SetBrowsable(this, "CurrentUnit", false);
            //        Util.SetBrowsable(this, "Save_Voltage", false);
            //        Save_Voltage = null;
            //        Save_Current = null;

            //        return;
                    
            //    case SaveCondition_Type.전류:
            //        Util.SetBrowsable(this, "Save_Interval", false);
            //        Util.SetBrowsable(this, "Save_Current", true);
            //        Util.SetBrowsable(this, "CurrentUnit", true);
            //        Util.SetBrowsable(this, "Save_Voltage", false);
            //        Save_Interval = null;
            //        Save_Voltage = null;
            //        return;

            //    case SaveCondition_Type.전압:
            //        Util.SetBrowsable(this, "Save_Interval", false);
            //        Util.SetBrowsable(this, "Save_Current", false);
            //        Util.SetBrowsable(this, "CurrentUnit", false);
            //        Util.SetBrowsable(this, "Save_Voltage", true);
            //        Save_Interval = null;
            //        Save_Current = null;
            //        return;
            //}
        }
    }


    public class SaveConverter : TypeConverter
    {

        public override bool GetPropertiesSupported( ITypeDescriptorContext context )
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(SaveCondition));

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
