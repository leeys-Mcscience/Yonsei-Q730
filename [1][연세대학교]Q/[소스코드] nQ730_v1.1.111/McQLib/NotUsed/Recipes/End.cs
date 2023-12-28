using McQLib.Core;

namespace McQLib.NotUsed.Recipes
{
    /// <summary>
    /// End 레시피입니다.
    /// </summary>
    internal sealed class End : IRecipe
    {
        public string Name => GetType().Name;

        /// <summary>
        /// 레시피 정보를 패킷의 DATA Field 형태로 변환합니다.
        /// <br>시퀀스의 마지막 스텝에 자동으로 추가되는 레시피로, 이 클래스의 인스턴스를 라이브러리 외부에서 직접 만들어서 사용할 수 없습니다.</br>
        /// </summary>
        /// <param name="stepNo">현재 레시피의 시퀀스상 순번입니다.</param>
        /// <param name="endStepNo_NotUsed">사용되지 않음(stepNo와 같은 값으로 사용됩니다.)</param>
        /// <param name="errorStepNo_NotUsed">사용되지 않음(stepNo와 같은 값으로 사용됩니다.)</param>
        /// <returns>변환된 DATA Field 형태의 byte형식 배열입니다.</returns>
        public byte[] ToCommand( ushort stepNo, ushort endStepNo_NotUsed, ushort errorStepNo_NotUsed )
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
            builder.Add( ( byte )Mode1.End );

            // Mode2 (1Byte) - Reserved
            builder.Add( 0x00 );

            // 설정 조건 (66Byte) - None
            builder.AddCount( 0, 66 );

            // 안전 조건, 종료 조건, 기록 조건 (164Byte) - None
            builder.AddCount( 0, 164 );

            // 종료값 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( stepNo ) );

            // 비정상 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( stepNo ) );

            // 반복횟수 (4Byte)
            builder.AddCount( 0, 4 );

            // 스텝 진행 (1Byte)
            builder.Add( 0 );

            // Reserved (7Byte) - Reserved
            builder.AddCount( 0, 7 );

            return builder;
        }

        internal End() { }
    }
}
