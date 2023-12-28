using McQLib.Core;
using System.ComponentModel;

namespace McQLib.Recipes
{
    /// <summary>
    /// Cycle 레시피입니다.
    /// </summary>
    public sealed class Cycle : BaseConvertableRecipe
    {
        public override string GetManualString()
        {
            return "레시피들을 반복 실행합니다.\r\n" +
                   "Cycle 레시피와 Cycle 레시피의 아래쪽에 위치한 가장 가까운 Loop 레시피 사이에 존재하는 레시피들이 반복 실행의 대상입니다.";
        }

        public override string GetSummaryString()
        {
            return string.Empty;
        }

        public override string GetDetailString()
        {
            return "=== Parameter ===\r\n" +
                   "Empty";
        }

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_Cycle;

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

            // Mode1 (1Byte) - Cycle : 5
            builder.Add( ( byte )Mode1.Cycle );

            // Mode2 (1Byte) - Reserved
            builder.Add( 0 );

            // [설정 조건] (66Byte) - 설정값 없음
            builder.AddCount( 0, 66 );

            // [안전 조건, 종료 조건, 기록 조건] (164Byte) - 설정값 없음
            builder.AddCount( 0, 164 );

            // 종료값 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( endStepNo ) );

            // 비정상 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( errorStepNo ) );

            // 반복횟수 (4Byte) - Loop, Jump만 해당
            builder.AddCount( 0, 4 );

            // 스텝 진행 (1Byte) - Reserved
            builder.Add( 0 );

            // Reserved (7Byte) - Reserved
            builder.AddCount( 0, 7 );

            return builder;
        }

        /// <summary>
        /// 패킷의 DATA Field 형태로부터 레시피 정보를 추출합니다. 이 메서드는 지정된 바이트 배열의 인덱스 0부터 259까지의 총 260개의 바이트를 사용합니다.
        /// </summary>
        /// <returns>변환된 DATA Field 형태의 byte형식 배열입니다.</returns>
        public override bool FromDataField( byte[] data )
        {
            if ( data == null || data.Length < 260 ) return false;

            // 안전 조건, 종료 조건, 저장 조건 - 없음

            return true;
        }

        internal Cycle() { }

        public override object Clone()
        {
            var clone = new Cycle();

            return clone;
        }

        #region Properties
        // 속성값 없음
        [Browsable( false )]
        public override SafetyCondition SafetyCondition => null;
        [Browsable( false )]
        public override SaveCondition SaveCondition => null;
        [Browsable( false )]
        public override EndCondition EndCondition => null;
        #endregion
    }
}
