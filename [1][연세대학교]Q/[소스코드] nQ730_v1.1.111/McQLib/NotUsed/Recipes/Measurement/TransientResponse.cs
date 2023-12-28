using System;
using McQLib.Core;
using McQLib.NotUsed.Core;

namespace McQLib.NotUsed.Recipes
{
    [Serializable]
    public sealed class TransientResponse : BaseMeasureRecipe
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
            builder.Add( Delay );               // 설정 Delay
            builder.Add( Width );               // 설정 Width
            builder.Add( ScaleMode );           // 설정 Mode select
            builder.Add( 0 );                   // 설정 Transition        (Fixed)
            builder.Add( MeasureTime );         // 설정 Sampling 수행 시간
            builder.Add( new Q_UInt32( 0 ) );   // 설정 Delay2            (Not used)
            builder.Add( new Q_UInt32( 0 ) );   // 설정 Width2            (Not used)
            builder.Add( 0 );                   // 설정 Raw data mode     (Not used)
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

        internal TransientResponse() { }

        #region Parameters
        [Group( "Parameter" )]
        [Parameter( "Low amplitude", ParameterValueType.Double, "700000" )]
        [Unit( "A" )]
        public double LowAmplitude;

        [Parameter( "High amplitude", ParameterValueType.Double, "700100" )]
        [Unit( "A" )]
        public double HighAmplitude;

        [Parameter( "Scale", ParameterValueType.Enum, "700200" )]
        public ScaleMode ScaleMode;

        [Parameter( "Delay", ParameterValueType.Double, "700300" )]
        [Unit( "us" )]
        public double Delay = 60;

        [Parameter( "Width", ParameterValueType.Double, "700400" )]
        [Unit( "us" )]
        public double Width = 60;

        [Parameter( "Measure Time", ParameterValueType.Double, "700500" )]
        [Unit( "us" )]
        public double MeasureTime;
        #endregion
    }
}
