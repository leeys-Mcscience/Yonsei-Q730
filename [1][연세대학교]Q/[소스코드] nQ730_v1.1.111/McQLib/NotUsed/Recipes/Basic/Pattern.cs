using System;
using McQLib.Core;
using McQLib.NotUsed.Core;

namespace McQLib.NotUsed.Recipes
{
    [Serializable]
    public sealed class Pattern : BaseRecipe
    {
        public override byte[] ToCommand( ushort stepNo, ushort endStepNo, ushort errorStepNo )
        {
            if( PatternName == null || PatternName.Length == 0 ) throw new QException( QExceptionType.PATTERN_INVALID_FILE_NAME_ERROR );

            PatternData pattern;
            try
            {
                pattern = PatternData.FromFile( PatternName );
            }
            catch( QException ex )
            {
                throw ex;
            }

            var builder = new DataBuilder();

            // Reserved (2Byte) - Reserved
            builder.AddCount( 0, 2 );

            // Step count (4Byte) - Reserved
            builder.AddCount( 0, 4 );

            // Step no (2Byte)
            builder.Add( new Q_UInt16( stepNo ) );

            // Cycle no (4Byte) - Reserved
            builder.AddCount( 0, 4 );

            // Mode1 (1Byte) - Pattern : 4
            builder.Add( Mode1.Pattern );

            // Mode2 (1Byte) - Set : 0
            builder.Add( Mode2.Set );

            // [설정 조건 - 사용 여부] (2Byte)
            byte high = 0, low = 0;
            low |= 0b10000000; // 설정 mode (사용)
            low |= 0b01000000; // 설정 time resolution (사용)
            low |= 0b00100000; // 설정 총 데이터 개수 (사용)

            builder.Add( high, low );

            // [설정 조건 - 값] (4Byte)
            builder.Add( pattern.BiasMode );                                // 설정 mode
            builder.Add( ( byte )(pattern.PulseWidth / 100) );                // 설정 time resolution (10 / 100 = 0, 100 / 100 = 1 임)
            builder.Add( new Q_UInt16( pattern.TotalCount ) );    // 설정 총 패턴 데이터 개수

            // Reserved (60Byte) - Reserved
            builder.AddCount( 0, 60 );

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

            // 스텝 진행 (1Byte) - 사용 안 함
            builder.Add( 0 );

            // Reserved (7Byte) - Reserved
            builder.AddCount( 0, 7 );

            PatternData = pattern;

            return builder;
        }

        internal Pattern() { }

        internal PatternData PatternData = null;

        #region Parameters
        [Group( "Paramter" )]
        [Parameter( "Pattern Name", ParameterValueType.Pattern, "090000" )] public string PatternName;
        #endregion
    }
}
