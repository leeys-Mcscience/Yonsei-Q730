using McQLib.Core;
using System.ComponentModel;

namespace McQLib.Recipes
{
    /// <summary>
    /// Transient Response(TR/TRA) 레시피입니다.
    /// </summary>
    public sealed class TransientResponse : BaseConvertableRecipe
    {
        public override string GetManualString()
        {
            return "2차 전지의 과도 응답 특성을 측정합니다.\r\n" +
                   "정전류 펄스를 인가하고, 인가된 전류에 따라 전압이 변화하는 양상을 us단위로 정밀하게 고속 측정하여 Transient Response를 측정할 수 있습니다.";
        }

        public override string GetSummaryString()
        {
            var str = string.Empty;

            str += $"Amplitude : {LowAmplitude}A → {HighAmplitude}A\r\n" +
                   $"Width : {Width}Sec\r\n" +
                   $"Delay : {Delay}Sec\r\n" +
                   $"Measure Time : {MeasureTime}us";
            
            return str;
        }

        public override string GetDetailString()
        {
            var str = _title;

            str += $"Low Amplitude : {LowAmplitude}A\r\n" +
                   $"High Amplitude : {HighAmplitude}A\r\n" +
                   $"Delay : {Delay}Sec\r\n" +
                   $"Width : {Width}Sec\r\n" +
                   $"Measure Time : {MeasureTime}Sec\r\n" +
                   $"Data Scale : {ScaleMode}\r\n" +
                   $"Amplify Mode : {AmplifyMode}\r\n";

            str += $"\r\n{base.GetDetailString()}";

            return str;
        }

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_TransientResponse;

        /// <summary>
        /// 레시피 정보를 패킷의 DATA Field 형태로 변환합니다.
        /// </summary>
        /// <param name="stepNo">현재 레시피의 시퀀스상 순번입니다.</param>
        /// <param name="endStepNo">현재 레시피가 종료된 후 이동할 목표 레시피의 시퀀스상 순번입니다.</param>
        /// <param name="errorStepNo">현재 레시피가 비정상 종료된 후 이동할 목표 레시피의 시퀀스상 순번입니다.</param>
        /// <returns>변환된 DATA Field 형태의 byte형식 배열입니다.</returns>
        public override byte[] ToDataField( ushort stepNo, ushort endStepNo, ushort errorStepNo )
        {
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

            // Mode2 (1Byte) - TRA : 6
            builder.Add( ( byte )Mode2.TRA );

            byte high = 0, low = 0;

            // 설정 Bias              사용 안 함
            // 설정 Low amplitude     사용
            // 설정 High amplitude    사용
            // 설정 Start frequency   사용
            // 설정 End frequency     사용
            // 설정 Mode select       사용
            // 설정 Transition        사용
            // 설정 Sampling 수행 시간 사용
            low |= 0b01111111;

            // [설정 조건 - 사용 여부] (2Byte) 
            builder.Add( high, low );

            // [설정 조건 - 값] (64Byte)
            builder.Add( new Q_Double( 0 ) );   // 설정 Bias              (Not used)
            builder.Add( LowAmplitude );        // 설정 Low amplitude     
            builder.Add( HighAmplitude );       // 설정 High amplitude

            if( Delay < 1 ) throw new QException( QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR, "Delay의 값이 1(sec)보다 작을 수 없습니다." );
            builder.Add( Delay * 1000000 );     // 설정 Delay

            if( Width < 1 ) throw new QException( QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR, "Width의 값이 1(sec)보다 작을 수 없습니다." );
            builder.Add( Width * 1000000 );     // 설정 Width

            builder.Add( ScaleMode );           // 설정 Mode select
            builder.Add( 0 );                   // 설정 Transition        (Fixed)
            builder.Add( MeasureTime );         // 설정 Sampling 수행 시간
            builder.Add( new Q_UInt32( 0 ) );   // 설정 Delay2            (Not used)
            builder.Add( new Q_UInt32( 0 ) );   // 설정 Width2            (Not used)
            builder.Add( 0 );                   // 설정 Raw data mode     (Not used)
            builder.Add( AmplifyMode );         // 설정 증폭 배율
            builder.AddCount( 0, 4 );           // Reserved

            // [안전 조건(O), 종료 조건(X), 기록 조건(X)] (164Byte)
            builder.Add( base.ToDataField( stepNo, endStepNo, errorStepNo ) );

            // 종료값 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( endStepNo ) );

            // 비정상 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( errorStepNo ) );

            // 반복횟수 (4Byte) - Loop, Jump만 해당
            builder.AddCount( 0, 4 );

            // 스텝 진행 (1Byte)
            builder.Add( 0 );

            // Reserved (7Byte) - Reserved
            builder.AddCount( 0, 7 );

            return builder;
        }

        internal TransientResponse() { }

        public override object Clone()
        {
            var clone = new TransientResponse();

            clone.LowAmplitude = LowAmplitude;
            clone.HighAmplitude = HighAmplitude;
            clone.ScaleMode = ScaleMode;
            clone.Delay = Delay;
            clone.Width = Width;
            clone.MeasureTime = MeasureTime;
            clone.AmplifyMode = AmplifyMode;

            clone._save = _save.Clone() as SaveCondition;
            clone._safety = _safety.Clone() as SafetyCondition;

            return clone;
        }

        #region Properties
        [Category( "\tParameter" )]
        [DisplayName( "Low Amplitude(A)" )]
        [Description( "" )]
        [ID("030600")]
        public double LowAmplitude { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "High Amplitude(A)" )]
        [Description( "" )]
        [ID( "030601" )]
        public double HighAmplitude { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "Data Scale" )]
        [Description( "" )]
        [ID( "030602" )]
        public ScaleMode ScaleMode { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "Delay(sec)" )]
        [Description( "" )]
        [ID( "030603" )]
        public double Delay { get; set; } = 60;

        [Category( "\tParameter" )]
        [DisplayName( "Width(sec)" )]
        [Description( "" )]
        [ID( "030604" )]
        public double Width { get; set; } = 60;

        [Category( "\tParameter" )]
        [DisplayName( "Measure Time(sec)" )]
        [Description( "" )]
        [ID( "030605" )]
        public double MeasureTime { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "Amplify" )]
        [Description( "" )]
        [ID( "030606" )]
        public AmplifyMode AmplifyMode { get; set; }

        [Browsable( false )]
        public override EndCondition EndCondition => null;
        #endregion
    }
}
