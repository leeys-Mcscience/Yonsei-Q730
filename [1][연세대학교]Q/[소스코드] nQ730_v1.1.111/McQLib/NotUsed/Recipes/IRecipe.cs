namespace McQLib.NotUsed.Recipes
{
    public interface IRecipe
    {
        /// <summary>
        /// 레시피의 이름을 반환합니다.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 레시피 정보를 패킷의 DATA Field 형태로 변환합니다.
        /// </summary>
        /// <param name="stepNo">현재 레시피의 시퀀스상 순번입니다.</param>
        /// <param name="endStepNo">현재 레시피가 종료된 후 이동할 목표 레시피의 시퀀스상 순번입니다.</param>
        /// <param name="errorStepNo">현재 레시피가 비정상 종료된 후 이동할 목표 레시피의 시퀀스상 순번입니다.</param>
        /// <returns>변환된 DATA Field 형태의 byte형식 배열입니다.</returns>
        byte[] ToCommand( ushort stepNo, ushort endStepNo, ushort errorStepNo );
    }
}
