using McQLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace McQLib.Recipes
{
    public static class RecipeSetting
    {
        private static string _saveFilePath = Path.Combine( Util.StartDirectory, "Recipes.config" );

        public static class Charge
        {
            [ID("Charge enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class Discharge
        {
            [ID("Discharge enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class Rest
        {
            [ID("Rest enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class Pattern
        {
            [ID("Pattern enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class Temperature
        {
            [ID("Temperature enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class AcResistance
        {
            [ID("AcResistance enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class DcResistance
        {
            [ID("DcResistance enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class FrequencyResponse
        {
            [ID("FrequencyResponse enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class OpenCircuitVoltage
        {
            [ID("OpenCircuitVoltage enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class TransientResponse
        {
            [ID("TransientResponse enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class Cycle
        {
            [ID("Cycle enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class Loop
        {
            [ID("Loop enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class Jump
        {
            [ID("Jump enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class Label
        {
            [ID("Label enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class CdCycle
        {
            [ID("CdCycle enabled")]
            public static bool Enabled { get; set; } = false;
        }
        public static class AnodeCharge
        {
            [ID( "AnodeCharge enabled" )]
            public static bool Enabled { get; set; } = false;
        }
        public static class AnodeDischarge
        {
            [ID( "AnodeDischarge enabled" )]
            public static bool Enabled { get; set; } = false;
        }

        /// <summary>
        /// 저장된 구성 설정 정보를 불러옵니다.
        /// <br>현재 설정 정보가 저장되지 않은 경우 변경 사항을 잃게 됩니다.</br>
        /// </summary>
        /// <returns></returns>
        public static bool Load()
        {
            var configurations = new List<KeyValuePair<string, string>>();

            if( new FileInfo( _saveFilePath ).Exists )
            {
                using( var sr = new StreamReader( _saveFilePath ) )
                {
                    var lines = sr.ReadToEnd().Decrypt( "Recipes.config" ).Split( Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

                    for( var i = 0; i < lines.Length; i++ )
                    {
                        var indexOfColon = lines[i].IndexOf( ':' );

                        // 콜론이 없거나, 처음 위치에 있거나, 마지막 위치에 있는 경우 유효하지 않은 라인
                        if( indexOfColon == -1 || indexOfColon == 0 || indexOfColon == lines[i].Length - 1 ) continue;

                        configurations.Add( new KeyValuePair<string, string>( lines[i].Substring( 0, indexOfColon ), lines[i].Substring( indexOfColon + 1, lines[i].Length - indexOfColon - 1 ) ) );
                    }
                }

                var types = typeof( RecipeSetting ).GetNestedTypes();

                foreach( var type in types )
                {
                    var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly );

                    foreach( var property in properties )
                    {
                        var id = ( IDAttribute )property.GetCustomAttribute( typeof( IDAttribute ) );
                        if( id == null ) continue;

                        var configuration = configurations.Where( i => i.Key == id.ID ).ToArray();
                        if( configuration.Length != 1 ) continue;

                        try
                        {
                            // 중요!!
                            // 기본적으로 값-형식(정수, 실수 등), 열거형, 그리고 Color 구조체에 대한 변환만이 구현되어 있습니다.
                            // 만약 설정값에 아래에서 정의되지 않은 형식의 값이 포함되었다면 반드시 string <-> object의 변환 메커니즘을 아래에 추가로 정의하십시오.
                            if( Equals( property.PropertyType.BaseType, typeof( Enum ) ) )
                            {   // 열거형 형식
                                property.SetValue( null, Enum.Parse( property.PropertyType, configuration[0].Value ) );
                            }
                            else
                            {   // 값 형식
                                property.SetValue( null, Convert.ChangeType( configuration[0].Value, property.PropertyType ) );
                            }
                        }
                        catch
                        {
                            // Configuration value converting error.
                            continue;
                        }
                    }
                }

                return true;
            }
            else
            {   // Configuration.ini 파일이 존재하지 않는 경우
                return false;
            }
        }
        /// <summary>
        /// 현재 구성 설정 정보를 저장합니다.
        /// <br>기존 설정 정보가 존재하는 경우 덮어씌워집니다.</br>
        /// </summary>
        public static void Save()
        {
            var types = typeof( RecipeSetting ).GetNestedTypes();

            var builder = new StringBuilder();
            foreach( var type in types )
            {
                var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly );

                foreach( var property in properties )
                {
                    var id = ( IDAttribute )property.GetCustomAttribute( typeof( IDAttribute ) );
                    if( id == null ) continue;

                    builder.AppendLine( $"{id.ID}:{property.GetValue( null )}" );
                }
            }

            using( var sw = new StreamWriter( _saveFilePath ) )
            {
                sw.Write( builder.ToString().Encrypt( "Recipes.config" ) );


                //foreach( var type in types )
                //{
                //    var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly );

                //    foreach( var property in properties )
                //    {
                //        var id = ( IDAttribute )property.GetCustomAttribute( typeof( IDAttribute ) );
                //        if( id == null ) continue;

                //        sw.WriteLine( $"{id.ID}:{property.GetValue( null )}" );
                //    }
                //}
            }
        }

        internal static bool Set( string cmd )
        {
            var split = cmd.ToLower().Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

            // split[0] = RecipeName
            // split[1] = Property
            // split[2] = Value
            if( split.Length != 3 ) return false;

            var types = typeof( RecipeSetting ).GetNestedTypes();

            foreach( var type in types )
            {
                var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly );

                foreach( var property in properties )
                {
                    var id = ( IDAttribute )property.GetCustomAttribute( typeof( IDAttribute ) );
                    if( id == null ) continue;

                    var idSplit = id.ID.ToLower().Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
                    if( idSplit.Length != 2 ) continue;

                    if(idSplit[0] == split[0] && idSplit[1] == split[1] )
                    {
                        property.SetValue( null, Convert.ChangeType( split[2], property.PropertyType ) );
                        return true;
                    }
                }
            }

            return false;
        }
        internal static object Get( string cmd )
        {
            var split = cmd.ToLower().Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

            // split[0] = RecipeName
            // split[1] = Property
            if( split.Length != 2 ) return null;

            var types = typeof( RecipeSetting ).GetNestedTypes();

            foreach( var type in types )
            {
                var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly );

                foreach( var property in properties )
                {
                    var id = ( IDAttribute )property.GetCustomAttribute( typeof( IDAttribute ) );
                    if( id == null ) continue;

                    var idSplit = id.ID.ToLower().Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
                    if( idSplit.Length != 2 ) continue;

                    if( idSplit[0] == split[0] && idSplit[1] == split[1] )
                    {
                        return property.GetValue( null );
                    }
                }
            }

            return null;
        }

        private static List<object> _initialValues = new List<object>();

        static RecipeSetting()
        {
            // 각 필드들의 초기값을 저장해둔다.
            var types = typeof( RecipeSetting ).GetNestedTypes();
            foreach( var type in types )
            {
                var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly );

                foreach( var property in properties )
                {
                    // ID 특성 굳이 저장하지 않아도 된다. tpyes와 properties는 항상 동일 순서로 가져와짐.
                    // 단, ID 특성이 있는지 없는지의 여부는 확인할 필요가 있음.
                    if( ( IDAttribute )property.GetCustomAttribute( typeof( IDAttribute ) ) == null ) continue;

                    _initialValues.Add( property.GetValue( null ) );
                }
            }
        }

        /// <summary>
        /// 구성 설정 정보를 초기값으로 설정하고 저장합니다.
        /// <br>기존 설정 정보가 존재하는 경우 모든 설정값이 초기화됩니다.</br>
        /// </summary>
        public static void Initialize()
        {
            var types = typeof( RecipeSetting ).GetNestedTypes();
            var index = 0;
            foreach( var type in types )
            {
                var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly );

                foreach( var property in properties )
                {
                    // ID 특성 굳이 저장하지 않아도 된다. tpyes와 fields는 항상 동일 순서로 가져와짐.
                    // 단, ID 특성이 있는지 없는지의 여부는 확인할 필요가 있음.
                    if( ( IDAttribute )property.GetCustomAttribute( typeof( IDAttribute ) ) == null ) continue;

                    property.SetValue( null, Convert.ChangeType( _initialValues[index++], property.PropertyType ) );
                }
            }
        }

        public static bool IsRecipeEnabled( RecipeType recipeType )
        {
            switch ( recipeType )
            {
                case RecipeType.Charge:
                    return Charge.Enabled;

                case RecipeType.Discharge:
                    return Discharge.Enabled;

                case RecipeType.Rest:
                    return Rest.Enabled;

                case RecipeType.Cycle:
                    return Cycle.Enabled;

                case RecipeType.Loop:
                    return Loop.Enabled;

                case RecipeType.Jump:
                    return Jump.Enabled;

                case RecipeType.Label:
                    return Label.Enabled;

                case RecipeType.Pattern:
                    return Pattern.Enabled;

                case RecipeType.TransientResponse:
                    return TransientResponse.Enabled;

                case RecipeType.FrequencyResponse:
                    return FrequencyResponse.Enabled;

                case RecipeType.AcResistance:
                    return AcResistance.Enabled;

                case RecipeType.DcResistance:
                    return DcResistance.Enabled;

                case RecipeType.Temperature:
                    return Temperature.Enabled;

                case RecipeType.AnodeCharge:
                    return AnodeCharge.Enabled;

                case RecipeType.AnodeDischarge:
                    return AnodeDischarge.Enabled;

                default:
                    return true;
            }
        }
    }
}
