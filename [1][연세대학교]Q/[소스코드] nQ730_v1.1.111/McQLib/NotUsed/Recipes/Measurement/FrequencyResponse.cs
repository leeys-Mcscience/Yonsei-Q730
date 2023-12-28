using System;
using McQLib.Core;
using McQLib.NotUsed.Core;

namespace McQLib.NotUsed.Recipes
{
    [Serializable]
    public class FrequencyResponse : BaseMeasureRecipe
    {
        public override byte[] ToCommand( ushort stepNo, ushort endStepNo, ushort errorStepNo )
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
            builder.Add( MeasureTime );         // 설정 Sampling 수행 시간
            builder.Add( new Q_UInt32( 0 ) );   // 설정 Delay2                (Not used)
            builder.Add( new Q_UInt32( 0 ) );   // 설정 Width2                (Not used)
            builder.Add( RawData );             // 설정 Raw data mode
            builder.Add( Amplify );             // 설정 증폭 배율
            builder.Add( 0, 4 );                // Reserved

            // [안전 조건] (58Byte)
            builder.Add( base.ToCommand( stepNo, endStepNo, errorStepNo ) );

            // [종료 조건, 기록 조건 - 사용 안 함] (106Byte)
            builder.AddCount( 0, 106 );

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

        #region Parameters
        [Group( "Parameter" )]
        [Parameter( "Bias", ParameterValueType.Double, "060000" )]
        [Unit( "A" )]
        public double Bias;

        [Parameter( "Amplitude", ParameterValueType.Double, "060100" )]
        [Unit( "A" )]
        public double Amplitude;

        [Parameter( "Start Frequency", ParameterValueType.Double, "060200" )]
        [Unit( "Hz" )]
        public double StartFrequency;

        [Parameter( "End Frequency", ParameterValueType.Double, "060300" )]
        [Unit( "Hz" )]
        public double EndFrequency;

        [Parameter( "Scale", ParameterValueType.Enum, "060400" )]
        public ScaleMode ScaleMode;

        [Parameter( "Step Count", ParameterValueType.Double, "060500" )]
        public double StepCount;

        [Parameter( "Raw Data", ParameterValueType.Boolean, "060600" )]
        public bool RawData;

        [Parameter( "Measure Time", ParameterValueType.Double, "060700" )]
        [Unit( "us" )]
        public double MeasureTime;
        #endregion
    }
}
