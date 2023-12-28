using McQLib.Core;
using System.ComponentModel;

namespace McQLib.Recipes
{
    /// <summary>
    /// AC Resistance(ACR) 레시피입니다.
    /// </summary>
    public sealed class AcResistance : BaseConvertableRecipe
    {
        public override string GetManualString()
        {
            return "2차 전지 교류 전류에 대한 임피던스를 측정하는 레시피입니다.\r\n" +
                   "특정 주파수의 정현파 교류 전류를 인가해 그에 따른 전압 반응을 측정하고, 측정된 전압과 전류 값을 이용해 임피던스 값을 계산합니다.";
        }

        public override string GetSummaryString()
        {
            var str = string.Empty;

            str += $"Bias : {Bias:f2}A, Amplitude : {Amplitude:f2}A\r\n" +
                   $"Frequency : {Frequency:f2}Hz\r\n" +
                   $"Amplify Mode : {AmplifyMode}\r\n" +
                   $"Save Raw Data : {(RawData ? "Yes" : "No")}";

            return str;
        }

        public override string GetDetailString()
        {
            var str = _title;

            str += $"Bias : {Bias:f2}A\r\n" +
                   $"Amplitude : {Amplitude:f2}A\r\n" +
                   $"Frequency : {Frequency:f2}Hz\r\n" +
                   $"Amplify Mode : {AmplifyMode}\r\n" +
                   $"Save Raw Data : {(RawData ? "Yes" : "No")}\r\n";

            str += $"\r\n{base.GetDetailString()}";

            return str;
        }

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_AcResistance;

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

            // Mode2 (1Byte) - ACR : 9
            builder.Add( ( byte )Mode2.ACR );

            byte high = 0, low = 0;

            // 설정 Bias                  사용
            // 설정 Low amplitude         사용
            // 설정 High amplitude        사용 안 함
            // 설정 Start freq.           사용
            // 설정 End freq.             사용 안 함
            // 설정 Mode select           사용 안 함
            // 설정 Step count            사용 안 함
            // 설정 Sampling 수행 시간     사용 안 함
            low |= 0b11010000;

            // [설정 조건 - 사용 여부] (2Byte) 
            builder.Add( high, low );

            // [설정 조건 - 값] (64Byte)
            builder.Add( Bias );                // 설정 Bias
            builder.Add( Amplitude );           // 설정 Low amplitude
            builder.Add( new Q_Double( 0 ) );   // 설정 High amplitude        (Not used)
            builder.Add( Frequency );           // 설정 Start frequency
            builder.Add( new Q_Double( 0 ) );   // 설정 End frequency         (Not used)
            builder.Add( 0 );                   // 설정 Mode select           (Not used)
            builder.Add( 0 );                   // 설정 Step count            (Not used)
            builder.Add( new Q_Double( 0 ) );   // 설정 Sampling 수행 시간     (Not used)
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

        internal AcResistance() { }

        public override object Clone()
        {
            var clone = new AcResistance();

            clone.Bias = Bias;
            clone.Amplitude = Amplitude;
            clone.Frequency = Frequency;
            clone.AmplifyMode = AmplifyMode;
            clone.RawData = RawData;

            clone._safety = _safety.Clone() as SafetyCondition;

            return clone;
        }

        #region Properties
        [Category( "\tParameter" )]
        [DisplayName( "Bias(A)" )]
        [Description( "인가할 교류 전류의 영점(A)입니다." )]
        [ID("030900")]
        public double Bias { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "Amplitude(A)" )]
        [Description( "인가할 교류 전류의 진폭(A)입니다." )]
        [ID( "030901" )]
        public double Amplitude { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "Frequency(Hz)" )]
        [Description( "인가할 주파수(Hz)입니다." )]
        [ID( "030902" )]
        public double Frequency { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "Amplify" )]
        [Description( "전압 측정 정밀도입니다." )]
        [ID( "030903" )]
        public AmplifyMode AmplifyMode { get; set; }

        [Category("\tParameter")]
        [DisplayName("Raw Data")]
        [Description( "출력된 Raw Data 값을 함께 저장할 지의 여부입니다." )]
        [ID("030904")]
        public bool RawData { get; set; }

        // 종료 조건과 기록 조건은 사용자가 변경할 필요 없으므로 속성 창에 노출되지 않도록 Browsable을 false로 재정의한다.
        [Browsable( false )]
        public override EndCondition EndCondition => null;
        [Browsable( false )]
        public override SaveCondition SaveCondition => null;
        #endregion
    }
}
