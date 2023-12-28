using System;
using McQLib.Core;
using McQLib.NotUsed.Core;

namespace McQLib.NotUsed.Recipes
{
    // 개발중 (사용 금지)
    [Flags]
    public enum RestLocation
    {
        Front,
        Middle,
        End,
        All,

        FrontAndMiddle,
        MiddleAndEnd,
        FrontAndEnd
    }
    public sealed class ChargeDischargeCycle : BaseBasicRecipe
    {
        #region Parameters
        [Group( "Source" )]
        [Parameter( "Mode", ParameterValueType.Enum, "FF0000" )]
        [Help( "인가될 소스의 타입입니다." )]
        public SourcingType_Charge SourcingType;

        [Parameter( "Voltage", ParameterValueType.Double, "FF0100" )]
        [Unit( "V" )]
        [Help( "인가될 Voltage(V)의 값입니다." )]
        public double Voltage;

        [Parameter( "Current", ParameterValueType.Double, "FF0200" )]
        [Unit( "A" )]
        [Help( "인가될 Current(A)의 값입니다." )]
        public double Current;

        [Parameter( "Power", ParameterValueType.Double, "FF0300" )]
        [Unit( "W" )]
        [Help( "인가될 Power(W)의 값입니다." )]
        public double Power;

        [Parameter( "Resistance", ParameterValueType.Double, "FF0400" )]
        [Unit( "Ω" )]
        [Help( "인가될 Resistance(Ω)의 값입니다." )]
        public double Resistance;

        [Parameter( "Loop count", ParameterValueType.Integer, "FF0500" )]
        [Unit( "회" )]
        [Help( "충방전 시퀀스를 반복할 횟수입니다." )]
        public int LoopCount;

        [Parameter( "Rest Location", ParameterValueType.Enum, "FF0600" )]
        [Help( "Rest 레시피를 수행할 위치입니다." )]
        public RestLocation RestLocation;

        [Parameter( "Reverse", ParameterValueType.Boolean, "FF0700" )]
        [Help( "Charge 레시피와 Discharge 레시피를 역순으로 수행할지의 여부입니다." )]
        public bool Reverse;
        #endregion
    }
}
