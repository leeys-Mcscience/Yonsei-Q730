namespace DataViewer.Class
{
    public enum QrdReadMode
    {
        Simple,
        Detail
    }

    public enum DataType
    {
        None,
        TotalTime,
        Cycle,
        ChargeCapacity,
        DisChargeCapacity,
        CoulombEfficiency,
        StepTime,
        Voltage,
        Current,
        Capacity,
        Power,
        WattHour,
        Temperature,
        Frequency,
        Z,
        Z_Real,
        Z_Img,
        DeltaV,
        DeltaI,
        DeltaT,
        StartOcv,
        EndOcv,
        Phase,
        V1,
        I1,
        V2,
        I2,
        R
    }
    /// <summary>
    /// 커브를 생성하는 옵션입니다.
    /// </summary>
    public enum CurveCreatingType
    {
        /// <summary>
        /// 동일한 컬럼은 동일한 커브에 추가됩니다.
        /// </summary>
        Columns,
        /// <summary>
        /// 각 레시피 단위로 커브가 생성됩니다.
        /// </summary>
        Recipes,
        /// <summary>
        /// 하나의 Loop에 포함된 레시피들이 하나의 커브에 추가됩니다.
        /// </summary>
        Loops,
        /// <summary>
        /// 하나의 파일이 하나의 커브에 추가됩니다.
        /// </summary>
        Files,
    }
}
