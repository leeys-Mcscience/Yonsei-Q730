using System;

namespace McQLib.NotUsed.Core
{
    /// <summary>
    /// 레시피 파라미터의 설명을 지정하는 특성입니다.
    /// </summary>
    [AttributeUsage( AttributeTargets.Field )]
    public class HelpAttribute : Attribute
    {
        public string Help { get; }

        public HelpAttribute( string help )
        {
            Help = help;
        }
    }
}
