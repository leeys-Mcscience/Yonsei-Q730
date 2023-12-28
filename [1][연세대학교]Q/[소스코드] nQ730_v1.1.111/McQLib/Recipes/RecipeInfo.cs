using McQLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace McQLib.Recipes
{
    public class RecipeInfo
    {
        private List<Parameter> _parameters;

        private RecipeInfo()
        {
            _parameters = new List<Parameter>();
        }

        public RecipeInfo( Recipe recipe ) : this()
        {
            RecipeType = recipe.GetRecipeType();

            var properties = recipe.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public );

            foreach( var property in properties )
            {
                var id = ( IDAttribute )Attribute.GetCustomAttribute( property, typeof( IDAttribute ) );

                if( id == null ) continue; // throw new QException( QExceptionType.DEVELOP_PARAMETER_ATTRIBUTE_NOT_DEFINED_ERROR );
                else if( _parameters.Where( i => i.ID == id.ID ).Count() != 0 ) throw new QException( QExceptionType.DEVELOP_PARAMETER_ID_ALREADY_USING_ERROR );

                if( id.ID == "FF00" )
                {   // 안전 조건인 경우 안전 조건 내부 속성들을 집어넣는다.
                    // null로 override된 경우에는 제외
                    if( recipe.SafetyCondition == null ) continue;

                    var inner = recipe.SafetyCondition.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public );
                    foreach( var prop in inner )
                    {
                        var inId = ( IDAttribute )Attribute.GetCustomAttribute( prop, typeof( IDAttribute ) );
                        if( inId == null ) continue;
                        else if( _parameters.Where( i => i.ID == inId.ID ).Count() != 0 ) throw new QException( QExceptionType.DEVELOP_PARAMETER_ID_ALREADY_USING_ERROR );

                        _parameters.Add( new Parameter( prop.Name, inId.ID, prop, recipe.SafetyCondition ) );
                    }
                }
                else if( id.ID == "FF01" )
                {
                    if( recipe.EndCondition == null ) continue;

                    var inner = recipe.EndCondition.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public );
                    foreach( var prop in inner )
                    {
                        var inId = ( IDAttribute )Attribute.GetCustomAttribute( prop, typeof( IDAttribute ) );
                        if( inId == null ) continue;
                        else if( _parameters.Where( i => i.ID == inId.ID ).Count() != 0 ) throw new QException( QExceptionType.DEVELOP_PARAMETER_ID_ALREADY_USING_ERROR );

                        _parameters.Add( new Parameter( prop.Name, inId.ID, prop, recipe.EndCondition ) );
                    }
                }
                else if( id.ID == "FF02" )
                {
                    if( recipe.SaveCondition == null ) continue;

                    var inner = recipe.SaveCondition.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public );
                    foreach( var prop in inner )
                    {
                        var inId = ( IDAttribute )Attribute.GetCustomAttribute( prop, typeof( IDAttribute ) );
                        if( inId == null ) continue;
                        else if( _parameters.Where( i => i.ID == inId.ID ).Count() != 0 ) throw new QException( QExceptionType.DEVELOP_PARAMETER_ID_ALREADY_USING_ERROR );

                        _parameters.Add( new Parameter( prop.Name, inId.ID, prop, recipe.SaveCondition ) );
                    }
                }
                else
                {
                    _parameters.Add( new Parameter( property.Name, id.ID, property ) );
                }
            }
        }

        public int Count => _parameters.Count;
        public RecipeType RecipeType { get; }
        public Parameter this[int index] => _parameters[index];
        public Parameter this[string id] => _parameters.Where( i => i.ID == id ).First();

        public class Parameter
        {
            public string Name { get; }
            public string ID { get; }
            public Type ValueType => _propertyInfo.PropertyType;
            private object _conditionObj { get; }

            private PropertyInfo _propertyInfo;

            public object GetValue( object obj )
            {
                if( _conditionObj != null ) return _propertyInfo.GetValue( _conditionObj );
                else return _propertyInfo.GetValue( obj );
            }
            public void SetValue( object obj, object value )
            {
                if( _conditionObj != null ) _propertyInfo.SetValue( _conditionObj, value );
                else _propertyInfo.SetValue( obj, value );
            }

            internal Parameter( string name, string id, PropertyInfo propertyInfo )
            {
                Name = name;
                ID = id;
                _propertyInfo = propertyInfo;
            }
            internal Parameter(string name, string id, PropertyInfo propertyInfo, object conditionObj ) : this(name, id, propertyInfo )
            {
                _conditionObj = conditionObj;
            }
        }
    }
}
