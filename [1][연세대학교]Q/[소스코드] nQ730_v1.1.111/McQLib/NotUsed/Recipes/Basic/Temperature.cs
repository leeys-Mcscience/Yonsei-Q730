using System;
using McQLib.Core;
using McQLib.NotUsed.Core;

namespace McQLib.NotUsed.Recipes
{
    public enum TemperatureMode
    {
        Constant,
        Variable
    }
    [Serializable]
    // 수정 필요함 - Temperature는 Save, Safety Condition만 사용 (End Condition 없음)
    public sealed class Temperature : BaseBasicRecipe
    {
        public override byte[] ToCommand( ushort stepNo, ushort endStepNo, ushort errorStepNo )
        {
            throw new NotImplementedException();
        }

        #region Parameters
        [Group( "Parameter" )]
        [Parameter( "Mode", ParameterValueType.Enum, "080000" )]
        public TemperatureMode TemperatureMode;

        [Parameter( "Target Temperature", ParameterValueType.Double, "080100" )]
        [Unit( "℃" )]
        public double TargetTemperature;

        [Parameter( "Target Margin", ParameterValueType.Double, "080200" )]
        [Unit( "℃" )]
        public double TargetMargin;

        [Parameter( "Stabilization Time", ParameterValueType.Integer, "080300" )]
        [Unit( "sec" )]
        public uint StabilizationTime;

        [Parameter( "Step Temperature", ParameterValueType.Double, "080400" )]
        [Unit( "℃" )]
        public double StepTemperature;

        [Parameter( "Limit Temperature", ParameterValueType.Double, "080500" )]
        [Unit( "℃" )]
        public double LimitTemperature;

        [Parameter( "Cycle for Step", ParameterValueType.Integer, "080600" )]
        public uint CycleForStep;
        #endregion
    }
}
