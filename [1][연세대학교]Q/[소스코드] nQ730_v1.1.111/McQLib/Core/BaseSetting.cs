using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace McQLib.Core
{
    /// <summary>
    /// 설정값을 구성하기 위한 기본 클래스입니다.
    /// </summary>
    public abstract class BaseSetting
    {
        private static List<object> _initialValues = new List<object>();

        /// <summary>
        /// 기본 클래스의 내부 인스턴스를 초기화합니다.
        /// </summary>
        public BaseSetting()
        {
            // 각 필드들의 초기값을 저장해둔다.
            var type = GetType();

            var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Instance );

            foreach ( var property in properties )
            {
                // ID 특성 굳이 저장하지 않아도 된다. tpyes와 properties는 항상 동일 순서로 가져와짐.
                // 단, ID 특성이 있는지 없는지의 여부는 확인할 필요가 있음.
                if ( ( IDAttribute )property.GetCustomAttribute( typeof( IDAttribute ) ) == null ) continue;

                _initialValues.Add( property.GetValue( this ) );
            }
        }
        /// <summary>
        /// 지정된 파일로부터 설정값을 읽어옵니다.
        /// </summary>
        /// <returns>파일을 읽는데 성공했다면 true이고, 그렇지 않은 경우 false입니다.</returns>
        public bool Load(string filepath)
        {
            var configurations = new List<KeyValuePair<string, string>>();

            if ( new FileInfo( filepath ).Exists )
            {
                using ( var sr = new StreamReader( filepath ) )
                {
                    var lines = sr.ReadToEnd().Split( Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

                    for ( var i = 0; i < lines.Length; i++ )
                    {
                        var indexOfColon = lines[i].IndexOf( ':' );

                        // 콜론이 없거나, 처음 위치에 있거나, 마지막 위치에 있는 경우 유효하지 않은 라인
                        if ( indexOfColon == -1 || indexOfColon == 0 || indexOfColon == lines[i].Length - 1 ) continue;

                        configurations.Add( new KeyValuePair<string, string>( lines[i].Substring( 0, indexOfColon ), lines[i].Substring( indexOfColon + 1, lines[i].Length - indexOfColon - 1 ) ) );
                    }
                }

                var type = GetType();

                var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Instance );

                foreach ( var property in properties )
                {
                    var id = ( IDAttribute )property.GetCustomAttribute( typeof( IDAttribute ) );
                    if ( id == null ) continue;

                    var configuration = configurations.Where( i => i.Key == id.ID ).ToArray();
                    if ( configuration.Length != 1 ) continue;

                    try
                    {
                        // 중요!!
                        // 기본적으로 값-형식(정수, 실수 등), 열거형, 그리고 Color 구조체에 대한 변환만이 구현되어 있습니다.
                        // 만약 설정값에 아래에서 정의되지 않은 형식의 값이 포함되었다면 반드시 string <-> object의 변환 메커니즘을 아래에 추가로 정의하십시오.
                        if ( TryParse( configuration[0].Value, out object result ) )
                        {
                            property.SetValue( this, result );
                        }
                        else if ( Equals( property.PropertyType.BaseType, typeof( Enum ) ) )
                        {   // 열거형 형식
                            property.SetValue( this, Enum.Parse( property.PropertyType, configuration[0].Value ) );
                        }
                        else if ( Equals( property.PropertyType, typeof( Color ) ) )
                        {   // struct Color 형식
                            var colorString = configuration[0].Value.Replace( "Color [", "" ).Replace( "]", "" );

                            // 알려진 이름의 Color인 경우 "Color [color_name]"으로 저장됨
                            var color = Color.FromName( colorString );
                            if ( color.IsKnownColor )
                            {
                                property.SetValue( this, color );
                            }
                            // ARGB가 직접 지정된 경우 "Color [R=red, G=green, B=blue]" 또는 "Color [A=alpha, R=red, G=green, B=blue]"으로 저장됨
                            else
                            {
                                colorString = colorString.Replace( "A=", "" ).Replace( "R=", "" ).Replace( "G=", "" ).Replace( "B=", "" ).Replace( ",", "" );
                                var split = colorString.Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

                                if ( split.Length == 3 )
                                {
                                    if ( int.TryParse( split[0], out int r ) && 0 <= r && r <= 255 &&
                                        int.TryParse( split[1], out int g ) && 0 <= g && g <= 255 &&
                                        int.TryParse( split[2], out int b ) && 0 <= b && b <= 255 )
                                    {
                                        property.SetValue( this, Color.FromArgb( r, g, b ) );
                                    }
                                }
                                else if ( split.Length == 4 )
                                {
                                    if ( int.TryParse( split[0], out int a ) && 0 <= a && a <= 255 &&
                                        int.TryParse( split[1], out int r ) && 0 <= r && r <= 255 &&
                                        int.TryParse( split[2], out int g ) && 0 <= g && g <= 255 &&
                                        int.TryParse( split[3], out int b ) && 0 <= b && b <= 255 )
                                    {
                                        property.SetValue( this, Color.FromArgb( a, r, g, b ) );
                                    }
                                }
                            }
                        }
                        else if ( property.PropertyType.BaseType == typeof( Array ) )
                        {
                            var split = configuration[0].Value.Split( ",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

                            var elType = property.PropertyType.GetElementType();
                            var list = Activator.CreateInstance( typeof( List<> ).MakeGenericType( new Type[] { elType } ) ) as IList;

                            for ( var i = 0; i < split.Length; i++ )
                            {
                                try
                                {
                                    if ( Equals( elType.BaseType, typeof( Enum ) ) )
                                    {   // 열거형 형식
                                        list.Add( Enum.Parse( elType, split[i] ) );
                                    }
                                    else
                                    {
                                        list.Add( Convert.ChangeType( split[i], elType ) );
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //list.Add( null );
                                }
                            }

                            property.SetValue( this, list.GetType().GetMethod( "ToArray" ).Invoke( list, null ) );
                        }
                        else if ( property.PropertyType.GetInterface( "IList" ) != null )
                        {
                            var split = configuration[0].Value.Split( ",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

                            var elType = property.PropertyType.GetGenericArguments()[0];
                            var list = Activator.CreateInstance( typeof( List<> ).MakeGenericType( new Type[] { elType } ) ) as IList;

                            for ( var i = 0; i < split.Length; i++ )
                            {
                                try
                                {
                                    if ( Equals( elType.BaseType, typeof( Enum ) ) )
                                    {   // 열거형 형식
                                        list.Add( Enum.Parse( elType, split[i] ) );
                                    }
                                    else
                                    {
                                        list.Add( Convert.ChangeType( split[i], elType ) );
                                    }
                                }
                                catch
                                {
                                    //list.Add( null );
                                }
                            }

                            property.SetValue( this, list );
                        }
                        else
                        {   // 값 형식
                            property.SetValue( this, Convert.ChangeType( configuration[0].Value, property.PropertyType ) );
                        }
                    }
                    catch
                    {
                        // Configuration value converting error.
                        continue;
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
        /// 지정된 경로에 설정값을 저장합니다.
        /// <br><see cref="IDAttribute"/> 특성이 지정된 모든 Public 및 NonPublic 필드 및 속성의 값을 저장합니다.</br>
        /// </summary>
        public int Save(string filepath)
        {
            var type = GetType();
            var count = 0;

            using ( var sw = new StreamWriter( filepath ) )
            {
                var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Instance );

                foreach ( var property in properties )
                {
                    var id = ( IDAttribute )property.GetCustomAttribute( typeof( IDAttribute ) );
                    if ( id == null ) continue;

                    var value = property.GetValue( this );

                    if ( value == null )
                    {
                        value = "Null";
                    }
                    else if ( value is IList list )
                    {
                        if ( list.Count == 0 )
                        {
                            value = "";
                        }
                        else
                        {
                            var str = list[0].ToString();

                            for ( var i = 1; i < list.Count; i++ )
                            {
                                str += $",{list[i]}";
                            }

                            value = str;
                        }
                    }

                    sw.WriteLine( $"{id.ID}:{value}" );
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// 설정 값을 각 속성의 초기값으로 되돌립니다.
        /// </summary>
        public void Initialize()
        {
            var types = typeof( BaseSetting ).GetNestedTypes();
            var index = 0;
            foreach ( var type in types )
            {
                var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Instance );

                foreach ( var property in properties )
                {
                    // ID 특성 굳이 저장하지 않아도 된다. tpyes와 fields는 항상 동일 순서로 가져와짐.
                    // 단, ID 특성이 있는지 없는지의 여부는 확인할 필요가 있음.
                    if ( ( IDAttribute )property.GetCustomAttribute( typeof( IDAttribute ) ) == null ) continue;

                    property.SetValue( this, Convert.ChangeType( _initialValues[index++], property.PropertyType ) );
                }
            }
        }

        /// <summary>
        /// 값 형식 또는 열거형 형식이 아닌 타입에 대해 문자열로부터의 변환을 구현하는 메서드입니다.
        /// <br>설정값을 읽어오는 과정에서 각 설정값에 대해 최초로 이 메서드를 호출하여 문자열을 변환하고자 하는 속성의 형식으로 변환을 시도합니다.</br>
        /// </summary>
        /// <param name="value">변환할 문자열입니다.</param>
        /// <param name="result">변환 결과값입니다.</param>
        /// <returns>변환에 성공했는지의 여부입니다.</returns>
        public virtual bool TryParse( string value, out object result )
        {
            result = null;

            return false;
        }
    }
}
