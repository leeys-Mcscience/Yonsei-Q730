using McQLib.Core;
using McQLib.NotUsed.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace McQLib.NotUsed.Recipes
{
    public enum RecipeType : byte
    {
        Charge = 0x00,
        Discharge = 0x01,
        Rest = 0x02,
        OpenCircuitVoltage = 0x03,
        AcResistance = 0x04,
        DcResistance = 0x05,
        FrequencyResponse = 0x06,
        TransientResponse = 0x07,
        Temperature = 0x08,
        Pattern = 0x09,
        Cycle = 0x0A,
        Loop = 0x0B,

        // 새로 추가되는 레시피는 이 줄 위에 작성하시오.

        // 이 아래는 직접 사용 금지 (어차피 사용할 곳도 없을 것이다)
        Multi = 0xA0,

        IRecipe = 0xF0,
        BaseRecipe = 0xF1,
        BaseBasicRecipe = 0xF2,
        BaseMeasureRecipe = 0xF3,
        NotSupport = 0xFF
    }

    public static class RecipeFactory
    {
        /// <summary>
        /// 기본적으로 제공되는 레시피 목록입니다.
        /// </summary>
        public static IRecipe[] Recipes
        {
            get
            {
                if( _recipes == null )
                {
                    _recipes = new List<IRecipe>();

                    // 시퀀스 빌더에서 보이지 않게 할 레시피는 이곳에서 삭제 또는 주석처리 할 것.
                    _recipes.Add( new Charge() );
                    _recipes.Add( new Discharge() );
                    _recipes.Add( new Rest() );
                    _recipes.Add( new OpenCircuitVoltage() );
                    _recipes.Add( new AcResistance() );
                    _recipes.Add( new DcResistance() );
                    _recipes.Add( new FrequencyResponse() );
                    _recipes.Add( new TransientResponse() );
                    _recipes.Add( new Temperature() );
                    _recipes.Add( new Pattern() );
                    _recipes.Add( new Cycle() );
                    _recipes.Add( new Loop() );
                }

                return _recipes.ToArray();
            }
        }
        private static List<IRecipe> _recipes;

        /// <summary>
        /// 라이브러리 외부에서 생성 가능하도록 할 레시피는 레시피 타입과 함께 이곳에서 처리한다.
        /// </summary>
        /// <param name="recipeType"></param>
        /// <returns></returns>
        public static IRecipe CreateInstance( RecipeType recipeType )
        {
            switch( recipeType )
            {
                case RecipeType.Charge:
                    return new Charge();

                case RecipeType.Discharge:
                    return new Discharge();

                case RecipeType.Rest:
                    return new Rest();

                case RecipeType.OpenCircuitVoltage:
                    return new OpenCircuitVoltage();

                case RecipeType.AcResistance:
                    return new AcResistance();

                case RecipeType.DcResistance:
                    return new DcResistance();

                case RecipeType.FrequencyResponse:
                    return new FrequencyResponse();

                case RecipeType.TransientResponse:
                    return new TransientResponse();

                case RecipeType.Temperature:
                    return new Temperature();

                case RecipeType.Pattern:
                    return new Pattern();

                case RecipeType.Cycle:
                    return new Cycle();

                case RecipeType.Loop:
                    return new Loop();

                default:
                    return null;
            }
        }

        public static RecipeInfo GetRecipeInfo( this IRecipe recipe )
        {
            return new RecipeInfo( recipe );
        }
        public static RecipeInfo GetRecipeInfo( this IRecipe[] recipes )
        {
            return new RecipeInfo( recipes );
        }
        public static RecipeType GetRecipeType( this IRecipe recipe )
        {
            if( recipe is Charge ) return RecipeType.Charge;
            else if( recipe is Discharge ) return RecipeType.Discharge;
            else if( recipe is Rest ) return RecipeType.Rest;
            else if( recipe is OpenCircuitVoltage ) return RecipeType.OpenCircuitVoltage;
            else if( recipe is AcResistance ) return RecipeType.AcResistance;
            else if( recipe is DcResistance ) return RecipeType.DcResistance;
            else if( recipe is FrequencyResponse ) return RecipeType.FrequencyResponse;
            else if( recipe is TransientResponse ) return RecipeType.TransientResponse;
            else if( recipe is Temperature ) return RecipeType.Temperature;
            else if( recipe is Pattern ) return RecipeType.Pattern;
            else if( recipe is Cycle ) return RecipeType.Cycle;
            else if( recipe is Loop ) return RecipeType.Loop;
            else throw new QException( QExceptionType.DEVELOP_UNDEFINED_RECIPE_TYPE_ERROR );
        }
    }
    /// <summary>
    /// 레시피의 정보를 나타내는 클래스입니다.
    /// </summary>
    public class RecipeInfo
    {
        List<Group> _groups;
        /// <summary>
        /// 레시피의 종류를 나타내는 <see cref="RecipeType"/>입니다.
        /// <br>만약 이 <see cref="RecipeInfo"/> 인스턴스가 서로 다른 종류의 레시피 여러 개에 대한 정보를 가지는 경우 <see cref="RecipeType.Multi"/>입니다.</br>
        /// </summary>
        public RecipeType RecipeType { get; }
        /// <summary>
        /// 레시피에 속하는 그룹의 개수입니다.
        /// </summary>
        public int GroupCount => _groups.Count;
        /// <summary>
        /// 레시피에 속한 그룹 중 <paramref name="index"/> 위치에 해당하는 그룹의 참조를 반환합니다.
        /// </summary>
        /// <param name="index">가져올 그룹의 위치입니다.</param>
        /// <returns><paramref name="index"/> 위치의 그룹입니다.</returns>
        public Group this[int index] => _groups[index];

        /// <summary>
        /// 지정된 코드로 파라미터를 찾습니다.
        /// </summary>
        /// <param name="code">파라미터의 고유 번호입니다.</param>
        /// <returns>검색된 파라미터가 Group에 있는 경우 <see cref="Parameter"/>이고, 그렇지 않은 경우 <see langword="null"/>입니다.</returns>
        public Group.Parameter Find( string code )
        {
            foreach( var g in _groups )
            {
                for( var i = 0; i < g.ParameterCount; i++ )
                {
                    if( g[i].Code == code ) return g[i];
                }
            }

            return null;
        }
        /// <summary>
        ///  지정된 레시피 배열에서 공통된 <see cref="Group"/>과 <see cref="Group.Parameter"/>의 정보를 가지는 새 RecipeInfo 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="recipes">레시피 정보를 조회할 레시피 목록입니다.</param>
        public RecipeInfo( params IRecipe[] recipes )
        {
            if( recipes == null ) throw new ArgumentNullException( nameof( recipes ) );
            else if( recipes.Length == 0 ) throw new ArgumentException( nameof( recipes ) );

            RecipeType = recipes[0].GetRecipeType();
            for( var i = 1; i < recipes.Length; i++ )
            {
                if( recipes[i].GetRecipeType() != RecipeType )
                {
                    RecipeType = RecipeType.Multi;
                    break;
                }
            }

            _groups = new List<Group>();
            FieldInfo[] fields;

            if( RecipeType != RecipeType.Multi )
            {   // 모두 같은 종류의 레시피인 경우
                //RecipeType = recipes[0].GetRecipeType();

                fields = recipes[0].GetType().GetFields( BindingFlags.Public | BindingFlags.Instance );
            }
            else
            {
                var fieldInfos = new List<List<FieldInfo>>();
                for( var i = 0; i < recipes.Length; i++ )
                {
                    fieldInfos.Add( new List<FieldInfo>( recipes[i].GetType().GetFields() ) );
                }

                var commonFields = new List<FieldInfo>();

                for( var i = 0; i < fieldInfos.Count; i++ )
                {
                    for( var j = 0; j < fieldInfos[i].Count; j++ )
                    {
                        // i번째 레시피의 j번째 필드
                        // 모든 레시피에 대해 존재하는지 검사
                        bool isIn = true;
                        for( var k = 0; k < fieldInfos.Count; k++ )
                        {
                            if( k == i ) continue;

                            if( fieldInfos[k].Select( f => f.DeclaringType ).ToList().IndexOf( fieldInfos[i][j].DeclaringType ) == -1 )
                            {
                                isIn = false;
                                break;
                            }
                        }

                        if( isIn )
                        {
                            commonFields.Add( fieldInfos[i][j] );
                        }
                    }
                }

                // 중복된 것들 제거
                commonFields = commonFields.Distinct().ToList();

                fields = commonFields.ToArray();
            }

            Group currentGroup = null;
            for( var i = 0; i < fields.Length; i++ )
            {
                var groupInfo = ( GroupAttribute )Attribute.GetCustomAttribute( fields[i], typeof( GroupAttribute ) );

                if( groupInfo != null )
                {
                    currentGroup = null;
                    for( var j = 0; j < _groups.Count; j++ )
                        if( _groups[j].Name == groupInfo.Name ) currentGroup = _groups[j];

                    if( currentGroup == null ) _groups.Add( currentGroup = new Group( groupInfo ) );
                }
                else if( currentGroup == null ) throw new QException( QExceptionType.DEVELOP_GROUP_ATTRIBUTE_NOT_DEFINED_ERROR );

                if( ( InvisibleAttribute )Attribute.GetCustomAttribute( fields[i], typeof( InvisibleAttribute ) ) != null ) continue;

                var parameter = ( ParameterAttribute )Attribute.GetCustomAttribute( fields[i], typeof( ParameterAttribute ) );
                var unit = ( UnitAttribute )Attribute.GetCustomAttribute( fields[i], typeof( UnitAttribute ) );
                var help = ( HelpAttribute )Attribute.GetCustomAttribute( fields[i], typeof( HelpAttribute ) );

                if( parameter == null ) continue; //throw new QException(QExceptionType.PARAMETER_ATTRIBUTE_NOT_DEFINED_ERROR);

                currentGroup.AddParameter( parameter, unit, help, fields[i] );
            }
        }
        /// <summary>
        /// 레시피가 가지는 하나의 그룹에 대한 정보를 나타내는 클래스입니다.
        /// </summary>
        public class Group
        {
            /// <summary>
            /// 그룹의 이름입니다.
            /// </summary>
            public string Name { get; }
            /// <summary>
            /// 그룹에 속한 파라미터의 개수입니다.
            /// </summary>
            public int ParameterCount => _parameters.Count;
            /// <summary>
            /// 그룹에 속한 파라미터 중 <paramref name="index"/> 위치에 해당하는 파라미터의 참조를 반환합니다.
            /// </summary>
            /// <param name="index">가져올 파라미터의 위치입니다.</param>
            /// <returns><paramref name="index"/> 위치의 파라미터입니다.</returns>
            public Parameter this[int index] => _parameters[index];

            private List<Parameter> _parameters;

            internal void AddParameter( ParameterAttribute parameter, UnitAttribute unit, HelpAttribute help, FieldInfo fieldInfo )
            {
                foreach( Parameter p in _parameters ) if( p.Code == parameter.Code ) return;

                _parameters.Add( new Parameter( parameter, unit, help, fieldInfo ) );
            }
            internal Group( GroupAttribute groupAttribute )
            {
                _parameters = new List<Parameter>();
                Name = groupAttribute.Name;
            }

            /// <summary>
            /// 레시피가 가지는 하나의 파라미터에 대한 정보를 나타내는 클래스입니다.
            /// </summary>
            public class Parameter
            {
                /// <summary>
                /// 파라미터의 이름입니다.
                /// </summary>
                public string Name { get; }
                /// <summary>
                /// 파라미터가 가질 수 있는 값 형식을 나타내는 <see cref="ParameterValueType"/>입니다.
                /// </summary>
                public ParameterValueType ParameterValueType { get; }
                /// <summary>
                /// 파라미터의 고유 번호입니다.
                /// </summary>
                public string Code { get; }
                /// <summary>
                /// 파라미터의 단위입니다.
                /// <br>단위가 지정되지 않은 파라미터인 경우 <see langword="null"/>입니다.</br>
                /// </summary>
                public string Unit { get; }
                /// <summary>
                /// 파라미터에 대한 도움말입니다.
                /// <br>도움말이 지정되지 않은 파라미터인 경우 <see langword="null"/>입니다.</br>
                /// </summary>
                public string Help { get; }
                /// <summary>
                /// 실제 파라미터에 해당하는 필드에 대한 <see cref="System.Reflection.FieldInfo"/> 인스턴스입니다.
                /// </summary>
                public FieldInfo FieldInfo => _fieldInfo;

                /// <summary>
                /// 이 파라미터가 Null 값을 허용하는지의 여부입니다.
                /// </summary>
                public bool IsAllowNull
                {
                    get
                    {
                        if( _fieldInfo.FieldType == null ) throw new QException( QExceptionType.DEVELOP_FIELD_TYPE_NOT_DEFINED_ERROR );

                        if( !_fieldInfo.FieldType.IsValueType ) return true;
                        if( Nullable.GetUnderlyingType( _fieldInfo.FieldType ) != null ) return true;
                        return false;
                    }
                }
                /// <summary>
                /// 이 파라미터의 열거형 멤버 리스트를 반환합니다.
                /// </summary>
                /// <returns>이 파라미터의 <see cref="ParameterValueType"/>이 <see cref="ParameterValueType.Enum"/>인 경우 열거형 멤버들의 문자열 표현 배열이고, 열거형이 아닌 경우 null입니다.</returns>
                public string[] GetEnumValues()
                {
                    if( ParameterValueType != ParameterValueType.Enum ) return null;

                    return Enum.GetNames( _fieldInfo.FieldType );
                }

                /// <summary>
                /// 지정된 개체에서 이 파라미터에 해당하는 값을 가져옵니다.
                /// </summary>
                /// <param name="obj">값을 가져올 개체입니다.</param>
                /// <returns>가져온 값입니다.</returns>
                public object GetValue( object obj ) => _fieldInfo.GetValue( obj );
                /// <summary>
                /// 지정된 개체에서 이 파라미터에 해당하는 메모리의 값을 설정합니다.
                /// </summary>
                /// <param name="obj">값을 설정할 개체입니다.</param>
                /// <param name="value">설정할 값입니다.</param>
                public void SetValue( object obj, object value ) => _fieldInfo.SetValue( obj, value );

                private FieldInfo _fieldInfo;

                internal Parameter( ParameterAttribute parameter, UnitAttribute unit, HelpAttribute help, FieldInfo fieldInfo )
                {
                    Name = parameter.Name;
                    ParameterValueType = parameter.ParameterValueType;
                    Code = parameter.Code;
                    Unit = unit == null ? null : unit.Unit;
                    Help = help == null ? null : help.Help;
                    _fieldInfo = fieldInfo;
                }
            }
        }
    }
}
