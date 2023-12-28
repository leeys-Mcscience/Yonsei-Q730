using System;
using McQLib.Core;
using McQLib.NotUsed.Core;

namespace McQLib.NotUsed.Recipes
{
    [Serializable]
    public sealed class OpenCircuitVoltage : BaseBasicRecipe
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
            low |= 0b00000111;

            // [설정 조건 - 사용 여부] (2Byte) 
            builder.Add( high, low );

            // [설정 조건 - 값] (64Byte)
            builder.Add( new Q_Double( 0 ) );   // 설정 Bias              (Not used)
            builder.Add( new Q_Double( 0 ) );   // 설정 Low amplitude     (Not used)
            builder.Add( new Q_Double( 0 ) );   // 설정 High amplitude    (Not used)
            builder.Add( new Q_Double( 0 ) );   // 설정 Delay             (Not used)
            builder.Add( new Q_Double( 0 ) );   // 설정 Width             (Not used)
            builder.Add( Tr_ScaleMode );        // 설정 Mode select
            builder.Add( 0 );                   // 설정 Transition        (Fixed)
            builder.Add( Tr_MeasureTime );      // 설정 Sampling 수행 시간
            builder.Add( new Q_UInt32( 0 ) );   // 설정 Delay2            (Not used)
            builder.Add( new Q_UInt32( 0 ) );   // 설정 Width2            (Not used)
            builder.Add( 0 );                   // 설정 Raw data mode     (Not used)
            builder.Add( Tr_AmplifyMode );      // 설정 증폭 배율
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
            builder.Add( Tr_StepMode );

            // Reserved (7Byte) - Reserved
            builder.AddCount( 0, 7 );

            return builder;
        }

        internal OpenCircuitVoltage() { }

        #region Parameters
        [Group( "TR Option" )]
        [Parameter( "Amplify", ParameterValueType.Enum, "030000" )]
        public AmplifyMode Tr_AmplifyMode;

        [Parameter( "Step", ParameterValueType.Enum, "030100" )]
        [Help( "이 레시피의 전/후에 TR 레시피를 측정하는 옵션입니다." )]
        public TrStepMode Tr_StepMode;

        [Parameter( "Measure Time", ParameterValueType.Double, "030200" )]
        [Unit( "us" )]
        [Help( "TR 측정시 사용되는 Measure Time(us)입니다." )]
        public double Tr_MeasureTime;

        [Parameter( "Scale", ParameterValueType.Enum, "030300" )]
        [Help( "TR 측정시 사용되는 TR 모드입니다." )]
        public ScaleMode Tr_ScaleMode;
        #endregion
    }
}
