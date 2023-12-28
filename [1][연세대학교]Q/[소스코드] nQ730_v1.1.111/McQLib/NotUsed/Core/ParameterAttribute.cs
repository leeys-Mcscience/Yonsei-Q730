using System;

namespace McQLib.NotUsed.Core
{
    public enum ParameterValueType
    {
        Integer,
        Double,
        Float,
        String,
        Boolean,
        Enum,
        Time,
        Pattern
    }

    /// <summary>
    /// 레시피 파라미터의 정보를 지정하는 특성입니다.
    /// <br>파라미터가 UI상에 표시될 이름과 값 형식을 지정할 수 있습니다.</br>
    /// </summary>
    [AttributeUsage( AttributeTargets.Field )]
    public class ParameterAttribute : Attribute
    {
        public string Name { get; }
        public ParameterValueType ParameterValueType { get; }
        public string Code { get; }

        /// <summary>
        /// ParameterAttribute 특성의 새 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="name">파라미터가 UI상에 표시될 이름입니다.</param>
        /// <param name="valueType">파라미터가 사용할 값 형식입니다. <see cref="ParameterValueType"/>으로 지정합니다.</param>
        /// <param name="code">파라미터가 가지는 고유 번호입니다. 이 값은 시퀀스를 저장하거나 로드할 때 파라미터를 구분하기 위해 사용됩니다.
        /// <br>따라서 고유 번호는 다른 레시피에 속한 어떠한 번호와도 중복되지 않아야 하며, 일반적으로 아래 규칙에 따라 지정하십시오.</br>
        /// <br>(1) 고유 번호는 6자리 문자열입니다.</br>
        /// <br>(2) 고유 번호의 첫 번째, 두 번째 문자는 파라미터가 속한 클래스에 해당하는 <see cref="Recipes.RecipeType"/>을 나타내는 두 자리 16진수 값입니다.</br>
        /// <br>(3) 고유 번호의 세 번째, 네 번째 문자는 해당 파라미터의 코드 상 순서를 나타내는 두 자리 16진수 값으로, 00~FF의 값입니다.</br>
        /// <br>(4) 고유 번호의 다섯 번째, 여섯 번째 문자는 해당 파라미터의 수정이 발생했을 때 증가시키는 두 자리 16진수 값으로, 00~FF의 값입니다. 새로운 파라미터가 추가되는 것이 아닌, 파라미터의 이름, 형식 등의 변화가 발생한 경우 이 값을 반드시 1씩 증가시키십시오.</br></param>
        public ParameterAttribute( string name, ParameterValueType valueType, string code )
        {
            Name = name;
            ParameterValueType = valueType;
            Code = code;
        }
    }
}
