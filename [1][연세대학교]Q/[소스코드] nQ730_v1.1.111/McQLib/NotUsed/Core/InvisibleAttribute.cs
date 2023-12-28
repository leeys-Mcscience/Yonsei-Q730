using System;

namespace McQLib.NotUsed.Core
{
    /// <summary>
    /// 레시피 파라미터가 UI상에 표시되지 않도록 지정하는 특성입니다.
    /// <br>프로토콜의 필드 상에는 존재하나 항상 고정된 값을 사용하는 파라미터, 
    /// 사용자가 편집할 수 없고 상황에 따라 자동으로 값이 결정되는 파라미터, 
    /// 형식상 내부적으로 존재해야는 파라미터 등에 이 특성을 적용하십시오.</br>
    /// </summary>
    [AttributeUsage( AttributeTargets.Field )]
    public class InvisibleAttribute : Attribute
    {
    }
}
