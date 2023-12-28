using System;
using McQLib.Core;
using McQLib.NotUsed.Core;

namespace McQLib.NotUsed.Recipes
{
    [Serializable]
    public abstract class BaseRecipe : IRecipe
    {
        public string Name => GetType().Name;
        /// <summary>
        /// 안전 조건에 대한 정보를 DATA Field 형태로 구성하여 반환합니다.
        /// <br>안전 조건 사용 여부 2Byte, 각 조건의 설정값 56Byte로 총 58Byte입니다.</br>
        /// </summary>
        /// <param name="stepNo_notUsed">사용되지 않는 매개변수입니다.</param>
        /// <param name="endStepNo_notUsed">사용되지 않는 매개변수입니다.</param>
        /// <param name="errorStepNo_notUsed">사용되지 않는 매개변수입니다.</param>
        /// <returns>안전 조건에 대한 DATA Field입니다.</returns>
        public virtual byte[] ToCommand( ushort stepNo_notUsed, ushort endStepNo_notUsed, ushort errorStepNo_notUsed )
        {
            var builder = new DataBuilder();

            // [안전 조건 - 사용 여부] 2Byte
            //byte high = 0;
            byte high = 0, low = 0;
            if( Safety_Voltage_Max != null ) high |= 0b10000000;
            if( Safety_Voltage_Min != null ) high |= 0b01000000;
            if( Safety_Current_Max != null ) high |= 0b00100000;
            if( Safety_Current_Min != null ) high |= 0b00010000;
            if( Safety_Capacity_Max_Ah != null ) high |= 0b00001000;
            if( Safety_Capacity_Max_Wh != null ) high |= 0b00000100;
            if( Safety_Temperature_Max != null ) high |= 0b00000010;
            if( Safety_Temperature_Min != null ) high |= 0b00000001;
            builder.Add( high, low );

            // [안전 조건 - 값] 56Byte
            builder.Add( Safety_Voltage_Max );      // 안전 최대 V
            builder.Add( Safety_Voltage_Min );      // 안전 최소 V
            builder.Add( Safety_Current_Max );      // 안전 최대 I
            builder.Add( Safety_Current_Min );      // 안전 최소 I
            builder.Add( Safety_Capacity_Max_Ah );  // 안전 최대 용량
            builder.Add( Safety_Capacity_Max_Wh );  // 안전 최대 Wh
            builder.Add( Safety_Temperature_Max );  // 안전 최대 Temp
            builder.Add( Safety_Temperature_Min );  // 안전 최소 Temp

            return builder;
        }

        #region Parameters
        [Group( "Safety Condition" )]
        [Parameter( "Max Voltage", ParameterValueType.Double, "F10000" )]
        [Unit( "V" )]
        [Help( "Voltage(V)가 지정된 값 이상으로 올라가면 측정을 강제로 중단합니다." )]
        public double? Safety_Voltage_Max;

        [Parameter( "Min Voltage", ParameterValueType.Double, "F10100" )]
        [Unit( "V" )]
        [Help( "Voltage(V)가 지정된 값 이하로 내려가면 측정을 강제로 중단합니다." )]
        public double? Safety_Voltage_Min;

        [Parameter( "Max Current", ParameterValueType.Double, "F10200" )]
        [Unit( "A" )]
        [Help( "Current(A)가 지정된 값 이상으로 올래가면 측정을 강제로 중단합니다." )]
        public double? Safety_Current_Max;

        [Parameter( "Min Current", ParameterValueType.Double, "F10300" )]
        [Unit( "A" )]
        [Help( "Current(A)가 지정된 값 이하로 내려가면 측정을 강제로 중단합니다." )]
        public double? Safety_Current_Min;

        [Parameter( "Max Capacity", ParameterValueType.Double, "F10400" )]
        [Unit( "Ah" )]
        [Help( "Capacity(Ah)가 지정된 값 이상으로 올라가면 측정을 강제로 중단합니다." )]
        public double? Safety_Capacity_Max_Ah;

        [Parameter( "Max Capacity", ParameterValueType.Double, "F10500" )]
        [Unit( "Wh" )]
        [Help( "Capacity(Wh)가 지정된 값 이상으로 올라가면 측정을 강제로 중단합니다." )]
        public double? Safety_Capacity_Max_Wh;

        [Parameter( "Max Temperature", ParameterValueType.Float, "F10600" )]
        [Unit( "℃" )]
        [Help( "Temperature(℃)가 지정된 값 이상으로 올라가면 측정을 강제로 중단합니다." )]
        public float? Safety_Temperature_Max;

        [Parameter( "Min Temperature", ParameterValueType.Float, "F10700" )]
        [Unit( "℃" )]
        [Help( "Temperature(℃)가 지정된 값 이하로 내려가면 측정을 강제로 중단합니다." )]
        public float? Safety_Temperature_Min;
        #endregion
    }
}
