using System;

namespace McQLib.NotUsed.Core
{
    /// <summary>
    /// 레시피 파라미터의 단위를 지정하는 특성입니다.
    /// </summary>
    [AttributeUsage( AttributeTargets.Field )]
    public class UnitAttribute : Attribute
    {
        public string Unit { get; }

        public UnitAttribute( string unit )
        {
            Unit = unit;
        }
    }
}
