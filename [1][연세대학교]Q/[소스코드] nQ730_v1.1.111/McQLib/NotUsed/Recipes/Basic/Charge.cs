﻿using System;
using McQLib.Core;
using McQLib.NotUsed.Core;

namespace McQLib.NotUsed.Recipes
{
    [Serializable]
    public sealed class Charge : BaseBasicRecipe
    {
        /// <summary>
        /// 레시피 정보를 패킷의 DATA Field 형태로 변환합니다.
        /// </summary>
        /// <param name="stepNo">현재 레시피의 시퀀스상 순번입니다.</param>
        /// <param name="endStepNo">현재 레시피가 종료된 후 이동할 목표 레시피의 시퀀스상 순번입니다.</param>
        /// <param name="errorStepNo">현재 레시피가 비정상 종료된 후 이동할 목표 레시피의 시퀀스상 순번입니다.</param>
        /// <returns>변환된 DATA Field 형태의 byte형식 배열입니다.</returns>
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

            // Mode1 (1Byte) - Charge : 1
            builder.Add( ( byte )Mode1.Charge );

            byte high = 0, low = 0;

            // Mode2 (1Byte) - Reserved
            switch( SourcingType )
            {
                case SourcingType_Charge.CC:
                    builder.Add( ( byte )Mode2.CC );
                    low |= 0b01000000;
                    break;

                case SourcingType_Charge.CCCV:
                    builder.Add( ( byte )Mode2.CCCV );
                    low |= 0b11000000;
                    break;

                case SourcingType_Charge.CP:
                    builder.Add( ( byte )Mode2.CP );
                    low |= 0b00010000;
                    break;

                case SourcingType_Charge.CR:
                    builder.Add( ( byte )Mode2.CR );
                    low |= 0b00001000;
                    break;

                default:
                    throw new QException( QExceptionType.DEVELOP_WRONG_MODE2_ERROR );
            }

            // TrStep이 None이 아닌 경우 Tr 조건 사용 여부 설정
            if( Tr_StepMode != TrStepMode.None )
            {
                low |= 0b000000011;
            }

            // [설정 조건 - 사용 여부] (2Byte)
            builder.Add( high );
            builder.Add( low );

            // [설정 조건 - 값] (64Byte)
            builder.Add( Voltage );                 // 설정 V
            builder.Add( Current );                 // 설정 I/설정 L-I
            builder.Add( new Q_Double( 0 ) );       // 변환 V
            builder.Add( Power );                   // 설정 P
            builder.Add( Resistance );              // 설정 R
            builder.Add( new Q_Double( 0 ) );       // 설정 H-I
            builder.Add( Tr_MeasureTime );          // 설정 Freq/설정 Sampling 수행 시간(TRA)
            builder.Add( ( byte )Tr_AmplifyMode );  // 설정 Duty/설정 증폭 배율(TRA)
            builder.Add( ( byte )Tr_ScaleMode );    // 설정 mode select(TRA)
            builder.AddCount( 0, 6 );               // Reserved

            // [안전 조건, 종료 조건, 기록 조건] (164Byte)
            builder.Add( base.ToCommand( stepNo, endStepNo, errorStepNo ) );

            // 종료값 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( endStepNo ) );

            // 비정상 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( errorStepNo ) );

            // 반복횟수 (4Byte) - Loop, Jump만 해당
            builder.AddCount( 0, 4 );

            // 스텝 진행 (1Byte)
            builder.Add( ( byte )Tr_StepMode );

            // Reserved (7Byte) - Reserved
            builder.AddCount( 0, 7 );

            return builder;
        }

        internal Charge() { }

        #region Parameters
        [Group( "Source" )]
        [Parameter( "Mode", ParameterValueType.Enum, "000000" )]
        [Help( "인가될 소스의 타입입니다." )]
        public SourcingType_Charge SourcingType;

        [Parameter( "Voltage", ParameterValueType.Double, "000100" )]
        [Unit( "V" )]
        [Help( "인가될 Voltage(V)의 값입니다." )]
        public double Voltage;

        [Parameter( "Current", ParameterValueType.Double, "000200" )]
        [Unit( "A" )]
        [Help( "인가될 Current(A)의 값입니다." )]
        public double Current;

        [Parameter( "Power", ParameterValueType.Double, "000300" )]
        [Unit( "W" )]
        [Help( "인가될 Power(W)의 값입니다." )]
        public double Power;

        [Parameter( "Resistance", ParameterValueType.Double, "000400" )]
        [Unit( "Ω" )]
        [Help( "인가될 Resistance(Ω)의 값입니다." )]
        public double Resistance;

        [Group( "TR Option" )]
        [Parameter( "Step", ParameterValueType.Enum, "000500" )]
        [Help( "이 레시피의 전/후에 TR 레시피를 측정하는 옵션입니다." )]
        public TrStepMode Tr_StepMode;

        [Parameter( "Measure Time", ParameterValueType.Double, "000600" )]
        [Unit( "us" )]
        [Help( "TR 측정시 사용되는 Measure Time(us)입니다." )]
        public double Tr_MeasureTime;

        [Parameter( "Amplify", ParameterValueType.Enum, "000700" )]
        [Help( "TR 측정시 사용되는 증폭 배율입니다." )]
        public AmplifyMode Tr_AmplifyMode;

        [Parameter( "Scale", ParameterValueType.Enum, "000800" )]
        [Help( "TR 측정시 사용되는 TR 모드입니다." )]
        public ScaleMode Tr_ScaleMode;
        #endregion
    }
}
