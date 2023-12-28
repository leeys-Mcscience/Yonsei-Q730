using McQLib.Core;

namespace McQLib.Recipes
{
    /// <summary>
    /// Rest 레시피입니다.
    /// </summary>
    public sealed class Rest : BaseConvertableRecipe
    {
        public override string GetManualString()
        {
            return "2차 전지에 전류나 전압을 인가하지 않고 휴지시킵니다.";
        }

        public override string GetSummaryString()
        {
            var str = string.Empty;

            str += $"{_save.GetSummaryString()}\r\n{_end.GetSummaryString()}\r\n{_safety.GetSummaryString()}";

            return str;
        }

        public override string GetDetailString()
        {
            return base.GetDetailString();
        }

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_Rest;

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

            // Mode1 (1Byte) - Rest : 0
            builder.Add( ( byte )Mode1.Rest );

            // Mode2 (1Byte) - Reserved
            builder.Add( 0 );

            // [설정 조건] (66Byte) - Reserved
            builder.AddCount( 0, 66 );

            // [안전 조건, 종료 조건, 기록 조건] (164Byte)
            builder.Add( base.ToDataField( stepNo, endStepNo, errorStepNo ) );

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

            // 안전 조건, 종료 조건, 저장 조건
            base.FromDataField( data );

            return true;
        }

        internal Rest() { }

        public override object Clone()
        {
            var clone = new Rest();

            clone._save = _save.Clone() as SaveCondition;
            clone._end = _end.Clone() as EndCondition;
            clone._safety = _safety.Clone() as SafetyCondition;

            return clone;
        }

        #region Properties
        // 속성값 없음
        #endregion
    }
}
