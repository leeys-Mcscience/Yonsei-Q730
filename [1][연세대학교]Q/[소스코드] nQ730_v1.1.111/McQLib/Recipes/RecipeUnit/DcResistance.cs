using McQLib.Core;
using System.ComponentModel;

namespace McQLib.Recipes
{
    /// <summary>
    /// DC Resistance(DCR) 레시피입니다.
    /// </summary>
    public sealed class DcResistance : BaseConvertableRecipe
    {
        public override string GetManualString()
        {
            return "2차 전지에 전류 스텝을 인가한 다음, 전/후의 전압차를 이용하여 내부 직류 저항을 측정합니다.\r\n" +
                   "(전/후 스텝 각각에 대한 전류와 간격을 임의로 조정할 수 있으며, IEC61960-2003 규격에서는 0.2C로 10초, 이후 0.1C로 1초 방전하여 저항을 계산하는 방법을 제시하고 있습니다.)";
        }

        public override string GetSummaryString()
        {
            var str = string.Empty;

            str += $"Current : {Current1st:f2}A → {Current2nd:f2}A\r\n" +
                   $"Width : {Width1st}Sec → {Width2nd}Sec\r\n" +
                   $"Delay : {Delay1st}Sec → {Delay2nd}Sec\r\n" +
                   $"Amplify Mode : {AmplifyMode}";

            return str;
        }

        public override string GetDetailString()
        {
            var str = _title;

            str += $"1st Current : {Current1st:f2}A\r\n" +
                   $"2nd Current : {Current2nd:f2}A\r\n" +
                   $"1st Width : {Width1st}Sec\r\n" +
                   $"2nd Width : {Width2nd}Sec\r\n" +
                   $"1st Delay : {Delay1st}Sec\r\n" +
                   $"2nd Delay : {Delay2nd}Sec\r\n" +
                   $"Total Time : {TotalTime}Sec\r\n" +
                   $"Amplify Mode : {AmplifyMode}\r\n";

            str += $"\r\n{base.GetDetailString()}";

            return str;
        }

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_DcResistance;

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

            // Mode2 (1Byte) - DCR : 8
            builder.Add( ( byte )Mode2.DCR );

            byte high = 0, low = 0;

            // 설정 3rd Current       사용
            // 설정 1st Current       사용
            // 설정 2nd Current       사용
            // 설정 1st Delay         사용
            // 설정 1st Width         사용
            // 설정 Mode select       사용 안 함
            // 설정 Step count        사용 안 함
            // 설정 Sampling 수행 시간 사용
            low |= 0b11111001;

            // [설정 조건 - 사용 여부] (2Byte) 
            builder.Add( high, low );

            // [설정 조건 - 값] (64Byte)
            builder.Add( Current3rd );          // 설정 3rd Current
            builder.Add( Current1st );          // 설정 1st Current
            builder.Add( Current2nd );          // 설정 2nd Current
            builder.Add( Delay1st );            // 설정 1st Delay

            if( Width1st < 1 ) throw new QException( QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR, "1st Width의 값이 1(sec)보다 작을 수 없습니다." );
            builder.Add( Width1st * 1000000 );  // 설정 1st Width(us)

            builder.Add( 0 );                   // 설정 Mode select       (Not used)
            builder.Add( 0 );                   // 설정 Step count        (Not used)

            if( TotalTime < 1 ) throw new QException( QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR, "Total Time의 값이 1(sec)보다 작을 수 없습니다." );
            builder.Add( TotalTime * 1000000 ); // 설정 Sampling 수행 시간(us)

            builder.Add( Delay2nd );            // 설정 Delay2

            if( Width2nd < 1 ) throw new QException( QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR, "2nd Width의 값이 1(sec)보다 작을 수 없습니다." );
            builder.Add( Width2nd * 1000000 );  // 설정 Width2(us)

            builder.Add( 0 );                   // 설정 Raw data mode     (Not used)
            builder.Add( AmplifyMode );         // 설정 증폭 배율
            builder.AddCount( 0, 4 );           // Reserved

            // [안전 조건(O), 종료 조건(X), 기록 조건(O)] (164Byte)
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

        internal DcResistance() { }

        public override object Clone()
        {
            var clone = new DcResistance();

            clone.Current1st = Current1st;
            clone.Current2nd = Current2nd;
            clone.Current3rd = Current3rd;
            clone.Width1st = Width1st;
            clone.Width2nd = Width2nd;
            clone.Delay1st = Delay1st;
            clone.Delay2nd = Delay2nd;
            clone.AmplifyMode = AmplifyMode;

            clone._save = _save.Clone() as SaveCondition;
            clone._safety = _safety.Clone() as SafetyCondition;

            return clone;
        }

        #region Properties
        [Category( "\tParameter" )]
        [DisplayName( "1st Current(A)" )]
        [Description( "인가할 첫 번째 전류(A)입니다." )]
        [ID( "030800" )]
        public double Current1st { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "2nd Current(A)" )]
        [Description( "인가할 두 번째 전류(A)입니다." )]
        [ID( "030801" )]
        public double Current2nd { get; set; }

        [Category( "\tParameter" )]
        [DisplayName( "3rd Current(A)" )]
        [Description( "인가할 세 번째 전류(A)입니다. 지원되지 않습니다." )]
        [Browsable( false )]
        public double Current3rd { get; set; } = 0;

        [Category( "\tParameter" )]
        [DisplayName( "1st Width(sec)" )]
        [Description( "첫 번째 전류를 인가할 시간(sec)입니다." )]
        [ID( "030802" )]
        [RefreshProperties(RefreshProperties.All)]
        public double Width1st { get; set; } = 60;

        [Category( "\tParameter" )]
        [DisplayName( "2nd Width(sec)" )]
        [Description( "두 번째 전류를 인가할 시간(sec)입니다." )]
        [ID( "030803" )]
        [RefreshProperties( RefreshProperties.All )]
        public uint Width2nd { get; set; } = 60;

        [Category( "\tParameter" )]
        [DisplayName( "1st Delay(sec)" )]
        [Description( "첫 번째 전류를 인가하하기 전의 지연 시간(sec)입니다." )]
        [ReadOnly( true )]
        public double Delay1st { get; set; } = 60;

        [Category( "\tParameter" )]
        [DisplayName( "2nd Delay(sec)" )]
        [Description( "두 번째 전류를 인가하하기 전의 지연 시간(sec)입니다." )]
        [ID( "030804" )]
        public uint Delay2nd { get; set; } = 60;

        [Category( "\tParameter" )]
        [DisplayName( "Total Time(sec)" )]
        [Description( "전체 전류를 인가하는 시간(1st Width + 2nd Width)입니다. 자동으로 계산됩니다." )]
        [ReadOnly( true )]
        //[ID( "030805" )]
        public double TotalTime => Width1st + Width2nd;

        [Category( "\tParameter" )]
        [DisplayName( "Amplify" )]
        [Description( "전압 측정 정밀도입니다." )]
        [ID( "030806" )]
        public AmplifyMode AmplifyMode { get; set; }

        [Browsable( false )]
        public override EndCondition EndCondition => null;
        #endregion
    }
}
