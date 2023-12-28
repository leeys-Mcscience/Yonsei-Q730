using McQLib.Core;
using System.ComponentModel;

namespace McQLib.Recipes
{
    /// <summary>
    /// Charge 레시피입니다.
    /// </summary>
    public sealed class AnodeDischarge : BaseConvertableRecipe
    {
        public override string GetManualString()
        {
            return "음극재 하프셀 전지를 정전류(CC), 정전류/정전압(CCCV)의 방법으로 방전합니다.\r\n" +
                   "충전 중 시간, 전압, 전류, 온도 등을 모니터링하여 측정값을 이용해 충전 용량(Ah)이나 정전압 모드에서 전류의 감소 사상수 등의 파라미터 값을 계산할 수 있습니다.";
        }

        public override string GetSummaryString()
        {
            var str = string.Empty;
            str += $"Sourcing type : {_sourcingType}(";
            switch ( _sourcingType )
            {
                case SourcingType_Anode.CC:
                    str += $"{Current:0.00####}{CurrentUnit})\r\n";
                    break;

                case SourcingType_Anode.CCCV:
                    str += $"{Current:0.00####}{CurrentUnit}, {Voltage:0.00####}V)\r\n";
                    break;

                    //case SourcingType_Charge.CP:
                    //    str += $"{Power:0.00####}W)\r\n";
                    //    break;

                    //case SourcingType_Charge.CR:
                    //    str += $"{Resistance:0.00####}Ω)\r\n";
                    //    break;
            }

            str += $"{_save.GetSummaryString()}\r\n{_end.GetSummaryString()}\r\n{_safety.GetSummaryString()}";

            return str;
        }
        public override string GetDetailString()
        {
            var str = _title;

            str += $"Sourcing type : {_sourcingType}\r\n";
            str += $"Source : ";
            switch ( _sourcingType )
            {
                case SourcingType_Anode.CC:
                    str += $"{Current:0.00####}{CurrentUnit}\r\n";
                    break;

                case SourcingType_Anode.CCCV:
                    str += $"{Current:0.00####}{CurrentUnit}, {Voltage:0.00####}V\r\n";
                    break;

                    //case SourcingType_Anode.CP:
                    //    str += $"{Power:0.00####}W\r\n";
                    //    break;

                    //case SourcingType_Anode.CR:
                    //    str += $"{Resistance:0.00####}Ω\r\n";
                    //    break;
            }

            str += $"\r\n{_trTitle}";
            str += $"Step Mode : {_tr_StepMode}\r\n";

            if ( _tr_StepMode != TrStepMode.None )
            {
                str += $"Measure Time : {Tr_MeasureTime}us\r\n" +
                       $"Scale Mode : {Tr_ScaleMode}\r\n" +
                       $"Amplify Mode : {Tr_AmplifyMode}\r\n";
            }

            str += $"\r\n{base.GetDetailString()}";

            return str;
        }

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_AnodeDischarge;

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

            if ( !RecipeSetting.TransientResponse.Enabled || // TR 레시피가 비활성화된 상태인 경우 (지원하지 않는 장비)
                _tr_StepMode == TrStepMode.None ||          // TR Option을 OFF 한 경우
                ( _sourcingType != SourcingType_Anode.CC && _sourcingType != SourcingType_Anode.CCCV ) )    // TR Option이 ON이지만 SourcingType이 CC/CCCV가 아닌 경우 (TR 옵션은 CC/CCCV 에서만 사용 가능함)
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

            // Mode1 (1Byte) - AnodeDischarge : 82
            builder.Add( ( byte )Mode1.AnodeDischarge );

            byte high = 0, low = 0;

            // Mode2 (1Byte)
            switch ( _sourcingType )
            {
                case SourcingType_Anode.CC:
                    builder.Add( ( byte )Mode2.CC );
                    low |= 0b01000000;
                    break;

                case SourcingType_Anode.CCCV:
                    builder.Add( ( byte )Mode2.CCCV );
                    low |= 0b11000000;
                    break;

                //case SourcingType_Charge.CP:
                //    builder.Add( ( byte )Mode2.CP );
                //    low |= 0b00010000;
                //    break;

                //case SourcingType_Charge.CR:
                //    builder.Add( ( byte )Mode2.CR );
                //    low |= 0b00001000;
                //    break;

                default:
                    throw new QException( QExceptionType.DEVELOP_WRONG_MODE2_ERROR );
            }

            // TrStep이 None이 아닌 경우 Tr 조건 사용 여부 설정
            if ( trStepMode != TrStepMode.None )
            {
                low |= 0b000000011;
            }

            // [설정 조건 - 사용 여부] (2Byte)
            builder.Add( high, low );

            // [설정 조건 - 값] (64Byte)
            builder.Add( Voltage );                 // 설정 V
            builder.Add( Current );                 // 설정 I/설정 L-I
            builder.Add( new Q_Double( 0 ) );       // 변환 V
            builder.Add( Power );                   // 설정 P
            builder.Add( Resistance );              // 설정 R
            builder.Add( new Q_Double( 0 ) );       // 설정 H-I

            if ( trStepMode != TrStepMode.None && trMeasureTime < 1 )
                throw new QException( QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR, "Measure Time의 값이 1(sec)보다 작을 수 없습니다." );
            builder.Add( trMeasureTime );           // 설정 Freq/설정 Sampling 수행 시간(TRA)

            builder.Add( trAmplifyMode );           // 설정 Duty/설정 증폭 배율(TRA)
            builder.Add( trScaleMode );             // 설정 mode select(TRA)
            builder.AddCount( 0, 6 );               // Reserved

            // [안전 조건, 종료 조건, 기록 조건] (164Byte)
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

        /// <summary>
        /// 패킷의 DATA Field 형태로부터 레시피 정보를 추출합니다. 이 메서드는 지정된 바이트 배열의 인덱스 0부터 259까지의 총 260개의 바이트를 사용합니다.
        /// </summary>
        /// <returns>변환된 DATA Field 형태의 byte형식 배열입니다.</returns>
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
            _sourcingType = ( SourcingType_Anode )data[position++];

            // high, low
            position += 2;

            Voltage = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            Current = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            position += 8;
            Power = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            Resistance = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            position += 8;

            Tr_MeasureTime = new Q_Double( data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++], data[position++] );
            Tr_AmplifyMode = ( AmplifyMode )data[position++];
            Tr_ScaleMode = ( ScaleMode )data[position++];

            // 안전 조건, 종료 조건, 저장 조건
            base.FromDataField( data );

            _tr_StepMode = ( TrStepMode )data[252];

            if ( !RecipeSetting.TransientResponse.Enabled ) // TR 레시피가 비활성화된 상태인 경우 (지원하지 않는 장비)
            {
                _tr_StepMode = TrStepMode.None;
            }

            return true;
        }

        internal AnodeDischarge() { }

        public override void Refresh()
        {
            base.Refresh();

            switch ( _sourcingType )
            {
                case SourcingType_Anode.CC:
                    Util.SetBrowsable( this, "Current", true );
                    Util.SetBrowsable( this, "Voltage", false );
                    Util.SetBrowsable( this, "Power", false );
                    Util.SetBrowsable( this, "Resistance", false );

                    if ( RecipeSetting.TransientResponse.Enabled )
                    {
                        Util.SetBrowsable( this, "Tr_StepMode", true );
                        if ( Tr_StepMode != TrStepMode.None )
                        {
                            Util.SetBrowsable( this, "Tr_MeasureTime", true );
                            Util.SetBrowsable( this, "Tr_AmplifyMode", true );
                            Util.SetBrowsable( this, "Tr_ScaleMode", true );
                        }
                        else
                        {
                            Util.SetBrowsable( this, "Tr_MeasureTime", false );
                            Util.SetBrowsable( this, "Tr_AmplifyMode", false );
                            Util.SetBrowsable( this, "Tr_ScaleMode", false );
                        }
                    }
                    return;

                case SourcingType_Anode.CCCV:
                    Util.SetBrowsable( this, "Current", true );
                    Util.SetBrowsable( this, "Voltage", true );
                    Util.SetBrowsable( this, "Power", false );
                    Util.SetBrowsable( this, "Resistance", false );

                    if ( RecipeSetting.TransientResponse.Enabled )
                    {
                        Util.SetBrowsable( this, "Tr_StepMode", true );
                        if ( Tr_StepMode != TrStepMode.None )
                        {
                            Util.SetBrowsable( this, "Tr_MeasureTime", true );
                            Util.SetBrowsable( this, "Tr_AmplifyMode", true );
                            Util.SetBrowsable( this, "Tr_ScaleMode", true );
                        }
                        else
                        {
                            Util.SetBrowsable( this, "Tr_MeasureTime", false );
                            Util.SetBrowsable( this, "Tr_AmplifyMode", false );
                            Util.SetBrowsable( this, "Tr_ScaleMode", false );
                        }
                    }
                    return;

                    // 사용 안 함 - 추후 사용시 주석 해제
                    //case SourcingType_Charge.CP:
                    //    Util.SetBrowsable( this, "Current", false );
                    //    Util.SetBrowsable( this, "Voltage", false );
                    //    Util.SetBrowsable( this, "Power", true );
                    //    Util.SetBrowsable( this, "Resistance", false );

                    //    if ( RecipeSetting.TransientResponse.Enabled )
                    //    {
                    //        Util.SetBrowsable( this, "Tr_StepMode", false );
                    //        Util.SetBrowsable( this, "Tr_MeasureTime", false );
                    //        Util.SetBrowsable( this, "Tr_AmplifyMode", false );
                    //        Util.SetBrowsable( this, "Tr_ScaleMode", false );
                    //    }
                    //    return;

                    //case SourcingType_Charge.CR:
                    //    Util.SetBrowsable( this, "Current", false );
                    //    Util.SetBrowsable( this, "Voltage", false );
                    //    Util.SetBrowsable( this, "Power", false );
                    //    Util.SetBrowsable( this, "Resistance", true );

                    //    if ( RecipeSetting.TransientResponse.Enabled )
                    //    {
                    //        Util.SetBrowsable( this, "Tr_StepMode", false );
                    //        Util.SetBrowsable( this, "Tr_MeasureTime", false );
                    //        Util.SetBrowsable( this, "Tr_AmplifyMode", false );
                    //        Util.SetBrowsable( this, "Tr_ScaleMode", false );
                    //    }
                    //    return;
            }
        }

        public override object Clone()
        {
            var clone = new AnodeDischarge();
            clone._sourcingType = _sourcingType;
            clone.Voltage = Voltage;
            clone.Current = Current;
            //clone.Power = Power;
            //clone.Resistance = Resistance;
            clone._tr_StepMode = _tr_StepMode;
            clone.Tr_MeasureTime = Tr_MeasureTime;
            clone.Tr_AmplifyMode = Tr_AmplifyMode;
            clone.Tr_ScaleMode = Tr_ScaleMode;

            clone._save = _save.Clone() as SaveCondition;
            clone._end = _end.Clone() as EndCondition;
            clone._safety = _safety.Clone() as SafetyCondition;

            return clone;
        }

        #region Properties
        [Category( "\tSource" )]
        [DisplayName( "Sourcing Type" )]
        [Description( "인가할 소스의 타입입니다." )]
        [ID( "010000" )]
        [RefreshProperties( RefreshProperties.All )]
        public SourcingType_Anode SourcingType
        {
            get
            {
                return _sourcingType;
            }
            set
            {
                _sourcingType = value;
                Refresh();
            }
        }
        private SourcingType_Anode _sourcingType = SourcingType_Anode.CC;

        [Category( "\tSource" )]
        [DisplayName( "Voltage(V)" )]
        [Description( "인가할 Voltage(V)의 값입니다." )]
        [Browsable( false )]
        [ID( "010001" )]
        public double Voltage { get; set; }

        [Category( "\tSource" )]
        [DisplayName( "Current" )]
        [Description( "인가할 Current의 값입니다." )]
        [Browsable( true )]
        [ID( "010002" )]
        public double Current { get; set; }

        [Category( "\tSource" )]
        [DisplayName( "Power(W)" )]
        [Description( "인가할 Power(W)의 값입니다." )]
        [Browsable( false )]
        [ID( "010003" )]
        public double Power { get; set; }

        [Category( "\tSource" )]
        [DisplayName( "Resistance(Ω)" )]
        [Description( "인가할 Resistance(Ω)의 값입니다." )]
        [Browsable( false )]
        [ID( "010004" )]
        public double Resistance { get; set; }

        // 이 아래는 TR Option 관련 속성으로, RecipeSetting.TransientResponse.Enabled 속성이 true인 경우에만 노출됩니다.
        [Category( "\tTR Option" )]
        [DisplayName( "Step" )]
        [Description( "이 레시피의 전/후에 TR 측정을 수행하는 옵션입니다." )]
        [RefreshProperties( RefreshProperties.All )]
        [Browsable( false )]
        [ID( "010005" )]
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
        [DisplayName( "Measure Time(us)" )]
        [Description( "TR 측정 시간(us)입니다." )]
        [Browsable( false )]
        [ID( "010006" )]
        public double Tr_MeasureTime { get; set; }

        [Category( "\tTR Option" )]
        [DisplayName( "Amplify" )]
        [Description( "TR 측정에서의 전압 측정 정밀도입니다." )]
        [Browsable( false )]
        [ID( "010007" )]
        public AmplifyMode Tr_AmplifyMode { get; set; }

        [Category( "\tTR Option" )]
        [DisplayName( "Scale" )]
        [Description( "TR 측정에서의 데이터 스케일 모드입니다." )]
        [Browsable( false )]
        [ID( "010008" )]
        public ScaleMode Tr_ScaleMode { get; set; }

        [Category("\tSource")]
        [DisplayName("Current Unit")]
        [Description("인가할 Current 단위 입니다.")]
        [ID("010009")]
        [RefreshProperties(RefreshProperties.All)]
        public SourcingType_CurrentUnit CurrentUnit
        {
            get
            {
                return _currentUnit;
            }
            set
            {
                _currentUnit = value;
                Refresh();
            }
        }
        private SourcingType_CurrentUnit _currentUnit = SourcingType_CurrentUnit.mA;

        [Browsable(false)]
        public SourcingType_CurrentUnit _saveCurrentUnit { get; set; } = SourcingType_CurrentUnit.A;
        #endregion
    }
}
