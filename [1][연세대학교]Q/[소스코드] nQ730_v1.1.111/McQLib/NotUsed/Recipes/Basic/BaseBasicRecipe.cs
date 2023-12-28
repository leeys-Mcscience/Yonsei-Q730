using System;
using McQLib.Core;
using McQLib.NotUsed.Core;

namespace McQLib.NotUsed.Recipes
{
    [Serializable]
    public abstract class BaseBasicRecipe : BaseRecipe
    {
        /// <summary>
        /// 안전 조건, 종료 조건 및 기록 조건에 대한 정보를 DATA Field 형태로 구성하여 반환합니다.
        /// <br>안전 조건 58Byte, 종료 조건에 대한 사용 여부 2Byte, 각 종료 조건의 설정값 78Byte, 기록 조건에 대한 사용 여부 2Byte, 각 기록 조건의 설정값 24Byte로 총 164Byte입니다.</br>
        /// </summary>
        /// <param name="stepNo_notUsed">사용되지 않는 매개변수입니다.</param>
        /// <param name="endStepNo_notUsed">사용되지 않는 매개변수입니다.</param>
        /// <param name="errorStepNo_notUsed">사용되지 않는 매개변수입니다.</param>
        /// <returns>안전 조건, 종료 조건, 기록 조건에 대한 DATA Field입니다.</returns>
        public override byte[] ToCommand( ushort stepNo_notUsed, ushort endStepNo_notUsed, ushort errorStepNo_notUsed )
        {
            var builder = new DataBuilder();

            // [안전 조건]
            builder.Add( base.ToCommand( stepNo_notUsed, endStepNo_notUsed, errorStepNo_notUsed ) );

            // [종료 조건 - 사용 여부] 2Byte
            byte high = 0, low = 0;
            if( End_Voltage != null ) high |= 0b10000000;
            if( End_Current != null ) high |= 0b01000000;
            if( End_Time != null ) high |= 0b00100000;
            if( End_CvTime != null ) high |= 0b00010000;
            if( End_Capacity != null ) high |= 0b00001000;
            if( End_Power != null ) high |= 0b00000100;
            if( End_Watt_Hour != null ) high |= 0b00000010;
            if( End_Voltage_Pick_Delta != null ) high |= 0b00000001;
            if( End_Temperature_Delta != null ) low |= 0b10000000;
            if( End_Temperature != null ) low |= 0b01000000;
            if( End_MaxCapacity != null ) low |= 0b00100000;

            if( low == 0 && high == 0 ) throw new QException( QExceptionType.RECIPE_END_CONDITION_NOT_SET_ERROR );
            builder.Add( high, low );

            // [종료 조건 - 값] 78Byte
            builder.Add( End_Voltage );             // 종료 V
            builder.Add( End_Current );             // 종료 I
            builder.Add( End_Time );                // 종료 Time
            builder.Add( End_CvTime );              // 종료 CV 시간
            builder.Add( End_Capacity );            // 종료 용량
            builder.Add( End_Power );               // 종료 P
            builder.Add( End_Watt_Hour );           // 종료 Wh
            builder.Add( End_Voltage_Pick_Delta );  // 종료 delta-V
            builder.Add( End_Temperature_Delta );   // 종료 delta-Temp
            builder.Add( End_Temperature );         // 종료 Temp
            builder.Add( new Q_UInt16( 0 ) );       // 종료 Max 용량 비율 Monitor Step count (0 고정)
            builder.Add( End_MaxCapacity );   // 종료 Max 용량 비율

            // [기록 조건 - 사용 여부] 2Byte
            high = low = 0;
            if( Save_Interval != null ) high |= 0b10000000;
            if( Save_Voltage != null ) high |= 0b01000000;
            if( Save_Current != null ) high |= 0b00100000;
            if( Save_Temperature != null ) high |= 0b00010000;
            builder.Add( high, low );

            // [기록 조건 - 값] 24Byte
            builder.Add( Save_Interval );           // 기록 Interval
            builder.Add( Save_Voltage );            // 기록 delta-V
            builder.Add( Save_Current );            // 기록 delta-I
            builder.Add( Save_Temperature );        // 기록 delta-Temp

            return builder;
        }

        #region Parameters
        [Group( "Save Condition" )]
        [Parameter( "△Time", ParameterValueType.Time, "F20000" )]
        [Help( "Time이 지정된 값만큼 경과할 때마다 데이터를 저장합니다. \r\n시간:분:초 순입니다." )]
        public uint? Save_Interval;

        [Parameter( "△Current", ParameterValueType.Double, "F20100" )]
        [Unit( "A" )]
        [Help( "Current(A)가 지정된 값만큼 변화할 때마다 데이터를 저장합니다." )]
        public double? Save_Current;

        [Parameter( "△Voltage", ParameterValueType.Double, "F20200" )]
        [Unit( "V" )]
        [Help( "Voltage(V)가 지정된 값만큼 변화할 때마다 데이터를 저장합니다." )]
        public double? Save_Voltage;

        [Parameter( "△Temperature", ParameterValueType.Float, "F20300" )]
        [Unit( "℃" )]
        [Help( "온도가 지정된 값만큼 변화할 때마다 데이터를 저장합니다." )]
        public float? Save_Temperature;


        [Group( "End Condition" )]
        [Parameter( "Voltage", ParameterValueType.Double, "F20400" )]
        [Unit( "V" )]
        [Help( "Voltage(V)가 지정된 값에 도달하면 측정을 종료합니다." )] public double? End_Voltage;

        [Parameter( "Current", ParameterValueType.Double, "F20500" )]
        [Unit( "A" )]
        [Help( "Current(A)가 지정된 값에 도달하면 측정을 종료합니다." )]
        public double? End_Current;

        [Parameter( "Time", ParameterValueType.Time, "F20600" )]
        [Help( "Time이 지정된 값만큼 경과하면 측정을 종료합니다. \r\n시간:분:초 순입니다." )]
        public double? End_Time;

        [Parameter( "CV Time", ParameterValueType.Time, "F20700" )]
        [Help( "CV Time이 지정된 값만큼 경과하면 측정을 종료합니다. \r\n시간:분:초 순입니다." )]
        public double? End_CvTime;

        [Parameter( "Capacity", ParameterValueType.Double, "F20800" )]
        [Unit( "Ah" )]
        [Help( "Capacity(Ah)가 지정된 값에 도달하면 측정을 종료합니다." )] public double? End_Capacity;

        [Parameter( "Power", ParameterValueType.Double, "F20900" )]
        [Unit( "W" )]
        [Help( "Watt(W)가 지정된 값에 도달하면 측정을 종료합니다." )] public double? End_Power;

        [Parameter( "Watt Hour", ParameterValueType.Double, "F20A00" )]
        [Unit( "Wh" )]
        [Help( "Watt Hour(Wh)가 지정된 값에 도달하면 측정을 종료합니다." )]
        public double? End_Watt_Hour;

        [Parameter( "△Voltage Pick", ParameterValueType.Double, "F20B00" )]
        [Unit( "V" )]
        [Help( "Voltage Pick(V)가 지정된 값에 도달하면 측정을 종료합니다." )]
        public double? End_Voltage_Pick_Delta;

        [Parameter( "△Temperature", ParameterValueType.Float, "F20C00" )]
        [Unit( "℃" )]
        [Help( "온도 변화량이 지정된 값에 도달하면 측정을 종료합니다." )]
        public float? End_Temperature_Delta;

        [Parameter( "Temperature", ParameterValueType.Float, "F20D00" )]
        [Unit( "℃" )]
        [Help( "온도가 지정된 값에 도달하면 측정을 종료합니다." )]
        public float? End_Temperature;

        [Parameter( "Max Capacity", ParameterValueType.Float, "F20E00" )]
        [Unit( "%" )]
        [Help( "최대 용량의 비율에 도달할 경우 측정을 종료합니다." )]
        public float? End_MaxCapacity;
        #endregion
    }
}
