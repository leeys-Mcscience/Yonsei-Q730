using System;
using McQLib.Core;
using McQLib.NotUsed.Core;

namespace McQLib.NotUsed.Recipes
{
    [Serializable]
    public sealed class Loop : IRecipe
    {
        public string Name => GetType().Name;
        public byte[] ToCommand( ushort stepNo, ushort endStepNo, ushort errorStepNo )
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

            // Mode1 (1Byte) - Rest : 0
            builder.Add( ( byte )Mode1.Loop );

            // Mode2 (1Byte) - Reserved
            builder.Add( 0 );

            // [설정 조건] (66Byte)
            builder.AddCount( 0, 66 );

            // [안전 조건, 종료 조건, 기록 조건] (164Byte)
            builder.AddCount( 0, 164 );

            // 종료값 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( endStepNo ) );

            // 비정상 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( errorStepNo ) );

            // 반복횟수 (4Byte) - Loop, Jump만 해당
            builder.Add( LoopCount );

            // 스텝 진행 (1Byte)
            builder.Add( 0 );

            // Reserved (7Byte) - Reserved
            builder.AddCount( 0, 7 );

            return builder;
        }

        internal Loop() { }

        #region Parameters
        [Group( "Option" )]
        [Parameter( "Loop Count", ParameterValueType.Integer, "0B0000" )]
        [Unit( "회" )]
        [Help( "현재 레시피의 위쪽에 위치한 가장 가까운 Cycle 레시피부터 현재 레시피까지를 반복할 횟수입니다." )]
        public uint LoopCount;
        #endregion
    }
}
