using System;
using McQLib.Core;
using McQLib.NotUsed.Core;

namespace McQLib.NotUsed.Recipes
{
    // 20210728 작성
    // 작성자 : DevJaemin
    // ACR Recipe To Command 메서드 완료
    [Serializable]
    public sealed class AcResistance : BaseMeasureRecipe
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
            builder.Add( Amplify );             // 설정 증폭 배율
            builder.AddCount( 0, 4 );                // Reserved

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

        internal AcResistance() { }

        #region Parameters
        [Group( "Parameter" )]
        [Parameter( "Bias", ParameterValueType.Double, "040000" )]
        [Unit( "A" )]
        public double Bias;

        [Parameter( "Amplitude", ParameterValueType.Double, "040100" )]
        [Unit( "A" )]
        public double Amplitude;

        [Parameter( "Frequency", ParameterValueType.Double, "040200" )]
        [Unit( "Hz" )]
        public double Frequency;

        [Parameter( "Raw Data", ParameterValueType.Boolean, "040300" )]
        public bool RawData;
        #endregion
    }
}
