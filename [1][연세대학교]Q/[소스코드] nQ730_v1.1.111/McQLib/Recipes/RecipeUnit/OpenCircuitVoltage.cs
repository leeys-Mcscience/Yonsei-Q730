using McQLib.Core;
using System.ComponentModel;

namespace McQLib.Recipes
{
    /// <summary>
    /// Open Circuit Voltage(OCV) 레시피입니다.
    /// </summary>
    public sealed class OpenCircuitVoltage : BaseConvertableRecipe
    {
        public override string GetManualString()
        {
            return "2차 전지의 전극을 개방하여 개방 전압을 정밀 측정합니다.\r\n" +
                   "전압의 미세한 변화를 놓치지 않기 위해 초고정밀도로 측정합니다.";
        }

        public override string GetSummaryString()
        {
            var str = string.Empty;

            str += $"Step Mode : {_tr_StepMode}\r\n";
            str += $"{_save.GetSummaryString()}\r\n{_end.GetSummaryString()}\r\n{_safety.GetSummaryString()}";

            return str;
        }

        public override string GetDetailString()
        {
            var str = _trTitle;

            str += $"Step Mode : {_tr_StepMode}\r\n";
            if( _tr_StepMode != TrStepMode.None )
            {
                str += $"Measure Time : {Tr_MeasureTime}us\r\n" +
                       $"Scale Mode : {Tr_ScaleMode}\r\n" +
                       $"Amplify Mode : {Tr_AmplifyMode}\r\n";
            }

            str += $"\r\n{base.GetDetailString()}";

            return str;
        }

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_OpenCircuitVoltage;

        /// <summary>
        /// 레시피 정보를 패킷의 DATA Field 형태로 변환합니다.
        /// </summary>
        /// <param name="stepNo">현재 레시피의 시퀀스상 순번입니다.</param>
        /// <param name="endStepNo">현재 레시피가 종료된 후 이동할 목표 레시피의 시퀀스상 순번입니다.</param>
        /// <param name="errorStepNo">현재 레시피가 비정상 종료된 후 이동할 목표 레시피의 시퀀스상 순번입니다.</param>
        /// <returns>변환된 DATA Field 형태의 byte형식 배열입니다.</returns>
        public override byte[] ToDataField( ushort stepNo, ushort endStepNo, ushort errorStepNo )
        {
            // TR Step Mode용 값 가져오기
            var trStepMode = _tr_StepMode;
            var trMeasureTime = Tr_MeasureTime;
            var trAmplifyMode = Tr_AmplifyMode;
            var trScaleMode = Tr_ScaleMode;

            if( !RecipeSetting.TransientResponse.Enabled || // TR 레시피가 비활성화된 상태인 경우 (지원하지 않는 장비)
                _tr_StepMode == TrStepMode.None )           // TR Option을 OFF 한 경우
            {
                trStepMode = TrStepMode.None;
                trMeasureTime = 0;
                trAmplifyMode = 0;
                trScaleMode = 0;
            }

            var builder = new DataBuilder();

            // Reserved (2Byte) - Reserved
            builder.AddCount( 0, 2 );

            // Step count (4Byte) - Reserved
            builder.AddCount( 0, 4 );

            // Step no. (2Byte) - 설정 Step number
            builder.Add( new Q_UInt16( stepNo ) );

            // Cycle no. (4Byte) - Reserved
            builder.AddCount( 0, 4 );

            // Mode1 (1Byte) - Measure : 3
            builder.Add( ( byte )Mode1.Measure );

            // Mode2 (1Byte) - OCV : 7
            builder.Add( ( byte )Mode2.OCV );

            byte high = 0, low = 0;

            // 설정 Bias              사용 안 함
            // 설정 Low amplitude     사용 안 함
            // 설정 High amplitude    사용 안 함
            // 설정 Start frequency   사용 안 함
            // 설정 End frequency     사용 안 함
            // 설정 Mode select       사용
            // 설정 Transition        사용
            // 설정 Sampling 수행 시간 사용
            // TrStep이 None이 아닌 경우 Tr 조건 사용 여부 설정
            if( trStepMode != TrStepMode.None )
            {
                low |= 0b000000111;
            }

            // [설정 조건 - 사용 여부] (2Byte) 
            builder.Add( high, low );

            // [설정 조건 - 값] (64Byte)
            builder.Add( new Q_Double( 0 ) );           // 설정 Bias              (Not used)
            builder.Add( new Q_Double( 0 ) );           // 설정 Low amplitude     (Not used)
            builder.Add( new Q_Double( 0 ) );           // 설정 High amplitude    (Not used)
            builder.Add( new Q_Double( 0 ) );           // 설정 Delay             (Not used)
            builder.Add( new Q_Double( 0 ) );           // 설정 Width             (Not used)
            builder.Add( trScaleMode );                 // 설정 Mode select
            builder.Add( 0 );                           // 설정 Transition        (Fixed)

            if( trStepMode != TrStepMode.None && trMeasureTime < 1 )
                throw new QException( QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR, "Measure Time의 값이 1(sec)보다 작을 수 없습니다." );
            builder.Add( trMeasureTime * 1000000 );     // 설정 Sampling 수행 시간

            builder.Add( new Q_UInt32( 0 ) );           // 설정 Delay2            (Not used)
            builder.Add( new Q_UInt32( 0 ) );           // 설정 Width2            (Not used)
            builder.Add( 0 );                           // 설정 Raw data mode     (Not used)
            builder.Add( trAmplifyMode );               // 설정 증폭 배율
            builder.AddCount( 0, 4 );                   // Reserved

            // [안전 조건(O), 종료 조건(O), 기록 조건(O)] (164Byte)
            builder.Add( base.ToDataField( stepNo, endStepNo, errorStepNo ) );

            // 종료값 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( endStepNo ) );

            // 비정상 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( errorStepNo ) );

            // 반복횟수 (4Byte) - Loop, Jump만 해당
            builder.AddCount( 0, 4 );

            // 스텝 진행 (1Byte)
            builder.Add( trStepMode );

            // Reserved (7Byte) - Reserved
            builder.AddCount( 0, 7 );

            return builder;
        }

        public override bool FromDataField( byte[] data )
        {
            if ( data == null || data.Length < 260 ) return false;

            var position = 0;

            // Reserved (2Byte)
            position += 2;
            // Step count (4Byte)
            position += 4;
            // Step no (2Byte)
            position += 2;
            // Cycle no (4Byte)
            position += 4;

            // Mode1
            position += 1;

            // Mode2
            position += 1;

            // high, low
            var high = data[position++];
            var low = data[position++];

            position += 8;      // 설정 Bias
            position += 8;      // 설정 Low amplitude
            position += 8;      // 설정 High amplitude
            position += 8;      // 설정 Delay
            position += 8;      // 설정 Width
            Tr_ScaleMode = ( ScaleMode )data[position++];
            position += 1;      // Transition

            Tr_MeasureTime = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] ) / 1000000;

            position += 4;      // Delay2
            position += 4;      // Width2
            position += 1;      // Raw data mode
            Tr_AmplifyMode = ( AmplifyMode )data[position++];

            _tr_StepMode = ( TrStepMode )data[252];

            if ( !RecipeSetting.TransientResponse.Enabled ) // TR 레시피가 비활성화된 상태인 경우 (지원하지 않는 장비)
            {
                _tr_StepMode = TrStepMode.None;
            }

            return base.FromDataField( data );
        }

        internal OpenCircuitVoltage() { }

        public override void Refresh()
        {
            base.Refresh();

            if( RecipeSetting.TransientResponse.Enabled )
            {
                switch( _tr_StepMode )
                {
                    case TrStepMode.None:
                        Util.SetBrowsable( this, "Tr_MeasureTime", false );
                        Util.SetBrowsable( this, "Tr_AmplifyMode", false );
                        Util.SetBrowsable( this, "Tr_ScaleMode", false );
                        break;

                    default:
                        Util.SetBrowsable( this, "Tr_MeasureTime", true );
                        Util.SetBrowsable( this, "Tr_AmplifyMode", true );
                        Util.SetBrowsable( this, "Tr_ScaleMode", true );
                        break;
                }
            }
        }

        public override object Clone()
        {
            var clone = new OpenCircuitVoltage();

            clone._tr_StepMode = _tr_StepMode;
            clone.Tr_AmplifyMode = Tr_AmplifyMode;
            clone.Tr_MeasureTime = Tr_MeasureTime;
            clone.Tr_ScaleMode = Tr_ScaleMode;

            clone._save = _save.Clone() as SaveCondition;
            clone._end = _end.Clone() as EndCondition;
            clone._safety = _safety.Clone() as SafetyCondition;

            return clone;
        }

        #region Properties
        [Category( "\tTR Option" )]
        [DisplayName( "Step" )]
        [Description( "" )]
        [Browsable( false )]
        [ID( "030700" )]
        public TrStepMode Tr_StepMode
        {
            get => _tr_StepMode;
            set
            {
                _tr_StepMode = value;
                Refresh();
            }
        }
        private TrStepMode _tr_StepMode = TrStepMode.None;

        [Category( "\tTR Option" )]
        [DisplayName( "Amplify" )]
        [Description( "" )]
        [Browsable( false )]
        [ID( "030701" )]
        public AmplifyMode Tr_AmplifyMode { get; set; }

        [Category( "\tTR Option" )]
        [DisplayName( "Measure Time(sec)" )]
        [Description( "" )]
        [Browsable( false )]
        [ID( "030702" )]
        public double Tr_MeasureTime { get; set; }

        [Category( "\tTR Option" )]
        [DisplayName( "Data Scale" )]
        [Description( "" )]
        [Browsable( false )]
        [ID( "030703" )]
        public ScaleMode Tr_ScaleMode { get; set; }
        #endregion
    }
}
