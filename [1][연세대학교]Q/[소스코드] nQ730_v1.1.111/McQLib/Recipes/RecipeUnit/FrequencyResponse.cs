using McQLib.Core;
using System.ComponentModel;

namespace McQLib.Recipes
{
    /// <summary>
    /// Frequency Response(FR/FRA) 레시피입니다.
    /// </summary>
    public sealed class FrequencyResponse : BaseConvertableRecipe
    {
        public override string GetManualString()
        {
            return "2차 전지의 주파수 응답 특성을 측정합니다. 정현파 교류 전류를 인가하여 그에 따른 전압 응답 신호를 측정해 임피던스 값을 계산합니다.\r\n" +
                   "특정 주파수 값의 교류 전류에 의한 임피던스 단일 결과 뿐 아니라, 주파수 Sweep을 통해 주파수 변화에 따른 임피던스 변화 특성을 측정할 수 있으며, 주파수 응답 결과를 Nyquist Plot의 기하학적 특징점을 이용하여 파라미터를 추출합니다.";
        }

        public override string GetSummaryString()
        {
            var str = string.Empty;

            str += $"Bias : {Bias}A, Amplitude : {Amplitude}A\r\n" +
                   $"Frequency : {StartFrequency}Hz → {EndFrequency}Hz\r\n" +
                   $"Step Count : {StepCount}\r\n" +
                   $"Measure Time : {MeasureTime}us";

            return str;
        }

        public override string GetDetailString()
        {
            var str = _title;

            str += $"Bias : {Bias}A\r\n" +
                   $"Amplitude : {Amplitude}A\r\n" +
                   $"Start Frequency : {StartFrequency}Hz\r\n" +
                   $"End Frequency : {EndFrequency}Hz\r\n" +
                   $"Step Count : {StepCount}\r\n" +
                   $"Measure Time : {MeasureTime}us\r\n" +
                   $"Data Scale : {ScaleMode}\r\n" +
                   $"Amplify Mode : {AmplifyMode}\r\n" +
                   $"Save Raw Data : {(RawData ? "Yes" : "No")}\r\n";

            str += $"\r\n{base.GetDetailString()}";

            return str;
        }

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_FrequencyResponse;

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

            // Mode2 (1Byte) - FRA : 10
            builder.Add( ( byte )Mode2.FRA );

            byte high = 0, low = 0;

            // 설정 Bias              사용
            // 설정 Low amplitude     사용
            // 설정 High amplitude    사용 안 함
            // 설정 Start frequency   사용
            // 설정 End frequency     사용
            // 설정 Mode select       사용
            // 설정 Step count        사용
            // 설정 Sampling 수행 시간 사용
            low |= 0b11011111;

            // [설정 조건 - 사용 여부] (2Byte) 
            builder.Add( high, low );

            // [설정 조건 - 값] (64Byte)
            builder.Add( Bias );                // 설정 Bias
            builder.Add( Amplitude );           // 설정 Low amplitude
            builder.Add( new Q_Double( 0 ) );   // 설정 High amplitude        (Not used)
            builder.Add( StartFrequency );      // 설정 Start frequency
            builder.Add( EndFrequency );        // 설정 End frequency
            builder.Add( ScaleMode );              // 설정 Mode select

            if( StepCount <= 0 ) StepCount = 1;
            else if( StepCount > 200 ) StepCount = 200;

            builder.Add( ( byte )StepCount );   // 설정 Step count
            builder.Add( MeasureTime );         // 설정 Sampling 수행 시간    (확인 필요)
            builder.Add( new Q_UInt32( 0 ) );   // 설정 Delay2                (Not used)
            builder.Add( new Q_UInt32( 0 ) );   // 설정 Width2                (Not used)
            builder.Add( RawData );             // 설정 Raw data mode
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

        internal FrequencyResponse() { }

        public override object Clone()
        {
            var clone = new FrequencyResponse();

            clone.Bias = Bias;
            clone.Amplitude = Amplitude;
            clone.StartFrequency = StartFrequency;
            clone.EndFrequency = EndFrequency;
            clone.ScaleMode = ScaleMode;
            clone.StepCount = StepCount;
            clone.RawData = RawData;
            clone.MeasureTime = MeasureTime;
            clone.AmplifyMode = AmplifyMode;

            clone._safety = _safety.Clone() as SafetyCondition;

            return clone;
        }

        #region Properties
        [Category( "\tParameter" )]
        [DisplayName( "Bias(A)" )]
        [Description( "인가할 교류 전류의 영점(A)입니다." )]
        [ID( "030A00" )]
        public double Bias { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "Amplitude(A)" )]
        [Description( "인가할 교류 전류의 진폭(A)입니다." )]
        [ID( "030A01" )]
        public double Amplitude { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "Start Frequency(Hz)" )]
        [Description( "Sweep할 주파수의 시작 주파수 값(Hz)입니다." )]
        [ID( "030A02" )]
        public double StartFrequency { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "End Frequency(Hz)" )]
        [Description( "Sweep할 주파수의 끝 주파수 값(Hz)입니다." )]
        [ID( "030A03" )]
        public double EndFrequency { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "Data Scale" )]
        [Description( "출력 주파수의 스텝 모드입니다." )]
        [ID( "030A04" )]
        public ScaleMode ScaleMode { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "Step Count" )]
        [Description( "출력 주파수의 스텝 수입니다." )]
        [ID( "030A05" )]
        public double StepCount { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "Raw Data" )]
        [Description( "출력된 Raw Data 값을 함께 저장할 지의 여부입니다." )]
        [ID( "030A06" )]
        public bool RawData { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "Measure Time(us)" )]
        [Description( "측정을 진행할 시간(us)입니다." )]
        [ID( "030A07" )]
        public double MeasureTime { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "Amplify" )]
        [Description( "전압 측정 정밀도입니다." )]
        [ID( "030A08" )]
        public AmplifyMode AmplifyMode { get; set; }

        [Browsable( false )]
        public override EndCondition EndCondition => null;
        [Browsable( false )]
        public override SaveCondition SaveCondition => null;
        #endregion
    }
}
