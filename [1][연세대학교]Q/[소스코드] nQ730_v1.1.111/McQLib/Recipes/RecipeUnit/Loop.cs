using McQLib.Core;
using System.ComponentModel;

namespace McQLib.Recipes
{
    /// <summary>
    /// Loop 레시피입니다.
    /// </summary>
    public sealed class Loop : BaseConvertableRecipe
    {
        public override string GetManualString()
        {
            return "레시피들을 반복 실행합니다.\r\n" +
                   "Loop 레시피와 Loop 레시피의 위쪽에 위치한 가장 가까운 Cycle 레시피 사이에 존재하는 레시피들이 반복 실행의 대상이며, 반복 횟수를 임의로 지정할 수 있습니다.";
        }

        public override string GetSummaryString()
        {
            return $"Loop count : {LoopCount}";
        }

        public override string GetDetailString()
        {
            var str = _title;
                   
            str += $"Loop Count : {LoopCount}";

            return str;
        }

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_Loop;

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

            // Mode1 (1Byte) - Loop : 6
            builder.Add( ( byte )Mode1.Loop );

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
            if(LoopCount <= 0 )
            {
                throw new QException( QExceptionType.RECIPE_INVALID_LOOP_COUNT_ERROR );
            }
            builder.Add( LoopCount );

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

            var position = 248;
            LoopCount = new Q_UInt32( data[position++], data[position++], data[position++], data[position++] );

            // 안전 조건, 종료 조건, 저장 조건 - 없음

            return true;
        }

        internal Loop() { }

        public override object Clone()
        {
            var clone = new Loop();

            clone.LoopCount = LoopCount;

            return clone;
        }

        #region Properties
        [Category( "Option" )]
        [DisplayName( "Loop Count" )]
        [Description( "위쪽에 위치하는 가장 가까운 Cycle 레시피부터 이 Loop 레시피까지의 시퀀스를 반복 수행할 횟수입니다." )]
        [ID("060000")]
        public uint LoopCount { get; set; }

        [Browsable( false )]
        public override SafetyCondition SafetyCondition => null;
        [Browsable( false )]
        public override SaveCondition SaveCondition => null;
        [Browsable( false )]
        public override EndCondition EndCondition => null;
        #endregion
    }
}
