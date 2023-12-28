using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace JmCmdLib
{
    public delegate void MessageListener( string msg );
    public class JmConsole
    {
        Form_Console _console;

        /// <summary>
        /// <see cref="JmConsole"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        public JmConsole()
        {
            _console = new Form_Console( Processing );
            //new Action()
        }

        #region Properties
        /// <summary>
        /// 콘솔에 새로운 문자열이 추가될 때 추가되는 문자열이 보이도록 자동으로 해당 위치로 스크롤할지의 여부입니다.
        /// 기본값은 true입니다.
        /// </summary>
        public bool AutoScrollToCaret
        {
            get => _console.AutoScrollToCaret;
            set => _console.AutoScrollToCaret = value;
        }
        /// <summary>
        /// 콘솔창의 제목입니다.
        /// </summary>
        public string Title
        {
            get => _console.Title;
            set => _console.Title = value;
        }
        /// <summary>
        /// 콘솔창의 로그 텍스트를 가져오거나 설정합니다.
        /// </summary>
        public string LogText
        {
            get => _console.LogText;
            set => _console.LogText = value;
        }
        /// <summary>
        /// 콘솔창의 명령어 텍스트를 가져오거나 설정합니다.
        /// </summary>
        public string CommandText
        {
            get => _console.CommandText;
            set => _console.CommandText = value;
        }
        /// <summary>
        /// 콘솔창의 로그 영역에 입력이 발생할 때 타임스탬프를 함께 출력할지의 여부입니다. 기본값은 false입니다.
        /// </summary>
        public bool TimeStamp
        {
            get => _console.TimeStamp;
            set => _console.TimeStamp = value;
        }
        /// <summary>
        /// 타임스탬프를 출력할 포맷입니다. <see cref="TimeStamp"/>가 true일 경우에만 적용됩니다.
        /// </summary>
        public string TimeStampFormat
        {
            get => _console.TimeStampFormat;
            set => _console.TimeStampFormat = value;
        }
        /// <summary>
        /// 콘솔창의 명령어 영역 좌측에 표시되는 위치를 표시하기 위한 텍스트를 가져오거나 설정합니다.
        /// </summary>
        public string LocationText
        {
            get => _console.LocationText;
            set => _console.LocationText = value;
        }

        private List<object> _historyObject = new List<object>();
        private List<string> _historyFieldName = new List<string>();

        public object Target
        {
            get => _target;
            set
            {
                _target = value;
                _historyObject.Clear();
                _historyFieldName.Clear();
            }
        }

        private object _target;
        #endregion

        #region Public Methods
        /// <summary>
        /// 콘솔창을 모달리스 대화상자로 표시합니다.
        /// </summary>
        public void Show()
        {
            try
            {
                _console.Show();
            }
            catch( ObjectDisposedException )
            {
                _console = new Form_Console( Processing );
                _console.Show();
            }

            _console.BringToFront();
        }
        public void Show( string password )
        {
            try
            {
                _console.Lock( password );
                _console.Show();
            }
            catch( ObjectDisposedException )
            {
                _console = new Form_Console( Processing );
                _console.Lock( password );
                _console.Show();
            }

            _console.BringToFront();
        }
        /// <summary>
        /// 콘솔창을 모달 대화상자로 표시합니다.
        /// </summary>
        public void ShowDialog()
        {
            try
            {
                _console.ShowDialog();
            }
            catch( ObjectDisposedException )
            {
                _console = new Form_Console( Processing );
                _console.ShowDialog();
            }

        }
        /// <summary>
        /// 콘솔창을 닫습니다.
        /// </summary>
        public void Close()
        {
            _console.Close();
        }
        /// <summary>
        /// 콘솔창을 숨깁니다.
        /// </summary>
        public void Hide()
        {
            _console.Hide();
        }

        /// <summary>
        /// 지정된 문자열을 콘솔에 씁니다.
        /// </summary>
        /// <param name="value">쓸 문자열입니다.</param>
        public void Write( string value )
        {
            _console.Write( value );
        }
        /// <summary>
        /// 줄 종결자를 콘솔에 씁니다.
        /// </summary>
        public void WriteLine()
        {
            _console.WriteLine();
        }
        /// <summary>
        /// 지정된 문자열 뒤에 줄 종결자를 추가하여 콘솔에 씁니다.
        /// </summary>
        /// <param name="value">쓸 문자열입니다.</param>
        public void WriteLine( string value )
        {
            _console.WriteLine( value );
        }
        /// <summary>
        /// 콘솔의 내용을 모두 지웁니다.
        /// </summary>
        public void Clear()
        {
            _console.Clear();
        }
        /// <summary>
        /// 콘솔창을 잠급니다.
        /// <br>비밀번호는 4자리 이상의 문자열이어야 합니다.</br>
        /// <br>비밀번호 입력 횟수(5회)를 초과하는 경우 어셈블리가 종료되기 전까지 모든 콘솔창 개체를 생성할 수 없도록 생성자에서 예외를 발생시킵니다.</br>
        /// </summary>
        /// <param name="password">잠금 해제를 위한 비밀번호입니다.</param>
        public void Lock( string password )
        {
            _console.Lock( password );
        }

        private enum State
        {
            Idle,
            CallFunction,
            SetField
        }

        private State _state;
        private string _memory;
        protected internal int a;
        internal protected int b;

        public BindingFlags DefaultBindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        /// <summary>
        /// 콘솔창에 입력된 명령어를 처리하는 메서드입니다.
        /// <br>Target에 지정된 object에 대한 Call, Set, Get이 구현되어 있습니다.</br>
        /// <br>이 메서드를 override하여 구현하고자 하는 명령어 처리 체계를 완성하십시오.</br>
        /// </summary>
        /// <param name="cmd"></param>
        public virtual void Processing( string cmd )
        {
            switch( _state )
            {
                case State.CallFunction:
                    var temp = _memory.Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
                    var methods = FindMethods( Target.GetType(), Get( temp, 1 ), DefaultBindingFlags, temp.Length - 2 );
                    if( int.TryParse( cmd, out int index ) && index > 0 && index <= methods.Length )
                    {
                        _console.WriteLine( $"{CallMethod( methods[index - 1], GetFrom( temp, 2 ) )}" );
                    }
                    else
                    {
                        _console.WriteLine( $"Wrong index." );
                    }
                    _state = State.Idle;
                    _memory = null;
                    break;
            }

            var split = cmd.Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

            switch( Get( split, 0 ).ToLower() )
            {
                case "call":
                    if( Target == null )
                    {
                        _console.WriteLine( "Call failed. Target object was null." );
                        break;
                    }

                    var methods = FindMethods( Target.GetType(), Get( split, 1 ), DefaultBindingFlags, split.Length - 2 );
                    if( methods.Length == 0 )
                    {
                        _console.WriteLine( $"Method not found, name '{Get( split, 1 )}'." );
                    }
                    else if( methods.Length != 1 )
                    {
                        var methodListString = "";
                        for( var i = 0; i < methods.Length; i++ )
                        {
                            methodListString += $"\r\n\t[{i + 1}] {methods[i].Name}(";
                            var parameters = methods[i].GetParameters();
                            for( var j = 0; j < parameters.Length; j++ ) methodListString += $"{parameters[j].GetType().Name}{(j != parameters.Length - 1 ? ", " : "")}";
                            methodListString += ")";
                        }
                        _console.WriteLine( $"There are {methods.Length} methods. Select the number of method to call.{methodListString}" );
                        _state = State.CallFunction;
                        _memory = cmd;
                    }
                    else
                    {
                        _console.WriteLine( $"{CallMethod( methods[0], GetFrom( split, 2 ) )}" );
                    }
                    break;

                case "set":
                    if( Target == null )
                    {
                        _console.WriteLine( "Set failed. Target object was null." );
                        break;
                    }

                    var field = FindField( Target.GetType(), Get( split, 1 ) );
                    if( field == null )
                    {
                        methods = FindMethods( Target.GetType(), "set_" + Get( split, 1 ), DefaultBindingFlags, 1 );
                        if( methods.Length == 1 )
                        {
                            _console.WriteLine( $"{SetFieldValue( methods[0], Get( split, 2 ) )}" );
                        }
                        else
                        {
                            _console.WriteLine( $"Field not found, name '{Get( split, 1 )}'." );
                        }
                        break;
                    }
                    else
                    {
                        _console.WriteLine( $"{SetFieldValue( field, Get( split, 2 ) )}" );
                    }

                    break;

                case "get":
                    if( Target == null )
                    {
                        _console.WriteLine( "Get failed. Target object was null." );
                        break;
                    }

                    field = FindField( Target.GetType(), Get( split, 1 ) );
                    if( field == null )
                    {
                        methods = FindMethods( Target.GetType(), "get_" + Get( split, 1 ), DefaultBindingFlags, 0 );
                        if( methods.Length == 1 )
                        {
                            _console.WriteLine( $"{GetFieldValue( methods[0] )}" );
                        }
                        else
                        {
                            _console.WriteLine( $"Field not found, name '{Get( split, 1 )}'." );
                        }
                        break;
                    }
                    else
                    {
                        _console.WriteLine( $"{GetFieldValue( field )}" );
                    }

                    break;

                case "monitor":
                case "mon":
                    if(Target == null )
                    {
                        _console.WriteLine( "Monitor open failed. Target object was null." );
                        break;
                    }

                    object obj = null;
                    field = FindField( Target.GetType(), Get( split, 1 ) );
                    if ( field == null )
                    {
                        methods = FindMethods( Target.GetType(), "get_" + Get( split, 1 ), DefaultBindingFlags, 1 );
                        if ( methods.Length == 1 )
                        {
                            obj = methods[0].Invoke( Target, null );
                        }
                        else
                        {
                            _console.WriteLine( $"Field not found, name '{Get( split, 1 )}'." );
                            break;
                        }
                    }
                    else
                    {
                        obj = field.GetValue( Target );
                    }

                    if( obj != null && obj is IList array )
                    {
                        new Form_ArrayMonitor( array ).Show();
                    }
                    else
                    {
                        _console.Write( $"Monitor open failed. Target was not IList." );
                    }

                    break;

                case "pick":
                    if( Target == null )
                    {
                        _console.WriteLine( "Get failed. Target object was null." );
                        break;
                    }

                    field = FindField( Target.GetType(), Get( split, 1 ) );
                    if( field != null )
                    {
                        GrabField( field );
                        _console.WriteLine( $"Picked '{Get( split, 1 )}'." );
                        LocationText = field.Name;
                    }
                    else
                    {
                        _console.WriteLine( $"Field not found, name '{Get( split, 1 )}'." );
                    }

                    break;

                case "return":
                    if(_historyObject.Count == 0 )
                    {
                        _console.WriteLine( "There is no object to return anymore." );
                        break;
                    }

                    _target = _historyObject[_historyObject.Count - 1];

                    _historyObject.RemoveAt( _historyObject.Count - 1 );

                    LocationText = _historyFieldName[_historyFieldName.Count - 1];
                    _historyFieldName.RemoveAt( _historyFieldName.Count - 1 );
                    _console.WriteLine( $"Return to '{LocationText}'." );
                    break;

                case "mem":
                case "memory":
                    var proc = Process.GetCurrentProcess();
                    _console.WriteLine(proc.PrivateMemorySize64.ToString());
                    break;

                case "show":
                    switch( Get( split, 1 ).ToLower() )
                    {
                        case "method":
                        case "methods":
                            if( Target == null )
                            {
                                _console.WriteLine( "Show methods failed. Target object was null." );
                                break;
                            }

                            methods = Target.GetType().GetMethods( DefaultBindingFlags );
                            var methodListString = string.Format( "{0, -16}{1, -35}{2, -35}{3}", "Modifier", "Type", "Name", "Parameter Types" );
                            for( var i = 0; i < methods.Length; i++ )
                            {
                                string modifier = string.Empty;
                                if( methods[i].IsFamilyOrAssembly ) modifier = "Internal";
                                else if( methods[i].IsPublic && methods[i].IsFamily ) modifier = "Protected";
                                else if( methods[i].IsPublic ) modifier = "Public";
                                else if( methods[i].IsPrivate ) modifier = "Private";

                                if( methods[i].IsStatic ) modifier += " Static";

                                string paramTypes = string.Empty;
                                var parameters = methods[i].GetParameters();
                                for( var j = 0; j < parameters.Length; j++ ) paramTypes += parameters[j].ParameterType.Name + " ";

                                methodListString += string.Format( "\r\n{0, -16}{1, -35}{2, -35}{3}", modifier, methods[i].ReturnType.Name, methods[i].Name, paramTypes );
                            }

                            _console.WriteLine( "All methods list.\r\n" + methodListString );
                            break;

                        case "field":
                        case "fields":
                            if( Target == null )
                            {
                                _console.WriteLine( "Show fields failed. Target object was null." );
                                break;
                            }

                            var fields = Target.GetType().GetFields( DefaultBindingFlags );
                            var fieldListString = string.Format( "{0, -16}{1, -35}{2, -35}", "Modifier", "Type", "Name" );
                            for( var i = 0; i < fields.Length; i++ )
                            {
                                string modifier = string.Empty;
                                if( fields[i].IsFamilyOrAssembly ) modifier = "Internal";
                                else if( fields[i].IsPublic && fields[i].IsFamily ) modifier = "Protected";
                                else if( fields[i].IsPublic ) modifier = "Public";
                                else if( fields[i].IsPrivate ) modifier = "Private";

                                if( fields[i].IsStatic ) modifier += " Static";

                                fieldListString += string.Format( "\r\n{0, -16}{1, -35}{2, -35}", modifier, fields[i].FieldType.Name, fields[i].Name );
                            }

                            _console.WriteLine( "All fields list.\r\n" + fieldListString );
                            break;

                        case "property":
                        case "properties":
                            if( Target == null )
                            {
                                _console.WriteLine( "Show properties failed. Target object was null." );
                                break;
                            }

                            var properties = Target.GetType().GetProperties( DefaultBindingFlags );
                            var propertyListString = string.Format( "{0, -16}{1, -35}{2, -35}", "Modifier", "Type", "Name" );
                            for( var i = 0; i < properties.Length; i++ )
                            {
                                string modifier = string.Empty;
                                //if( properties[i].is ) modifier = "Internal";
                                //else if( fields[i].IsPublic && fields[i].IsFamily ) modifier = "Protected";
                                //else if( fields[i].IsPublic ) modifier = "Public";
                                //else if( fields[i].IsPrivate ) modifier = "Private";

                                //if( properties[i].IsStatic ) modifier += " Static";

                                propertyListString += string.Format( "\r\n{0, -16}{1, -35}{2, -35}", modifier, properties[i].PropertyType.Name, properties[i].Name );
                            }

                            _console.WriteLine( "All properties list.\r\n" + propertyListString );
                            break;
                    }
                    break;

                case "help":
                    WriteLine( string.Format( "\r\n {0, -35}{1}\r\n", "Command", "Destruction" ) + GetHelpString() );
                    break;
            }
        }
        protected virtual string GetHelpString()
        {
            return _console.GetHelpString() +
                string.Format( " {0, -35}{1}\r\n", "call [target_name] [params...]", "Call methods." ) +
                string.Format( " {0, -35}{1}\r\n", "get [target_name]", "Get value from the field/property." ) +
                string.Format( " {0, -35}{1}\r\n", "set [target_name] [value]", "Set value to the field/property." ) +
                string.Format( " {0, -35}{1}\r\n", "pick [target_name]", "Pick field to do something." ) +
                string.Format( " {0, -35}{1}\r\n", "return", "Put current target down and go back to before target." ) +
                string.Format( " {0, -35}{1}\r\n", "show fields", "Show all fields in target." ) +
                string.Format( " {0, -35}{1}\r\n", "show methods", "Show all methods in target." );
        }
        #endregion

        #region Tool Methods
        /// <summary>
        /// 지정된 이름과 BindingFlags를 이용하여 type에서 메서드를 검색합니다.
        /// </summary>
        /// <param name="type">메서드를 검색할 타입입니다.</param>
        /// <param name="name">검색할 메서드의 이름입니다.</param>
        /// <param name="bindingFlags">메서드를 검색할 조건입니다.</param>
        /// <returns>조건에 맞게 검색된 메서드의 정보를 가지는 MethodInfo 배열입니다.</returns>
        protected MethodInfo[] FindMethods( Type type, string name, BindingFlags bindingFlags )
        {
            return type.GetMethods( bindingFlags ).Where( i => i.Name == name ).ToArray();
        }
        /// <summary>
        /// 지정된 이름, BindingFlags와 paramCount를 이용하여 type에서 메서드를 검색합니다.
        /// </summary>
        /// <param name="type">메서드를 검색할 타입입니다.</param>
        /// <param name="name">검색할 메서드의 이름입니다.</param>
        /// <param name="bindingFlags">메서드를 검색할 조건입니다.</param>
        /// <param name="paramCount">검색하려고 하는 메서드의 매개변수의 수 입니다.</param>
        /// <returns>조건에 맞게 검색된 메서드의 정보를 가지는 MethodInfo 배열입니다.</returns>
        protected MethodInfo[] FindMethods( Type type, string name, BindingFlags bindingFlags, int paramCount )
        {
            return type.GetMethods( bindingFlags ).Where( i => i.Name == name && i.GetParameters().Length == paramCount ).ToArray();
        }
        /// <summary>
        /// 지정된 이름과 BindingFlags와 Type 배열을 이용하여 type에서 메서드를 검색합니다.
        /// </summary>
        /// <param name="type">메서드를 검색할 타입입니다.</param>
        /// <param name="name">검색할 메서드의 이름입니다.</param>
        /// <param name="bindingFlags">메서드를 검색할 조건입니다.</param>
        /// <param name="types">검색하려고 하는 메서드의 각 매개변수들의 타입 입니다.</param>
        /// <returns>조건에 맞게 검색된 메서드의 정보를 가지는 MethodInfo 배열입니다.</returns>
        protected MethodInfo[] FindMethods( Type type, string name, BindingFlags bindingFlags, Type[] types )
        {
            if( types == null || types.Length == 0 ) return FindMethods( type, name, bindingFlags );

            var methods = FindMethods( type, name, bindingFlags, types.Length );

            return methods.Where( i => compareTypes( types, i.GetParameters() ) ).ToArray();
        }

        protected FieldInfo FindField( Type type, string name )
        {
            return type.GetField( name, DefaultBindingFlags );
        }
        protected PropertyInfo FindProperty(Type type, string name )
        {
            return type.GetProperty( name, DefaultBindingFlags );
        }
        protected object CallMethod( MethodInfo method, object[] args )
        {
            var parameterInfos = method.GetParameters();
            object result = null;

            for( var i = 0; i < parameterInfos.Length; i++ )
            {
                if( !TryChangeType( parameterInfos[i].ParameterType, args[i], out result ) )
                {
                    return $"Can not convert '{args[i]}' to type of '{parameterInfos[i].ParameterType}'.";
                }
                else args[i] = result;
            }

            result = null;
            try
            {
                result = method.Invoke( Target, args );
                result = result ?? "Void(or Null)";
            }
            catch( Exception ex )
            {
                result = ex;
            }
            finally
            {
                result = $"{method.Name} called. Method result was '{result}'.";
            }

            return result;
        }
        protected bool TryChangeType( Type type, object value, out object result )
        {
            result = null;

            if( value == null )
                return false;
            else if( value.ToString().ToLower() == "null" )
                return false;
            else
            {
                try
                {
                    if( Equals( type.BaseType, typeof( Enum ) ) )
                    {
                        result = Enum.Parse( type, value.ToString() );
                    }
                    else
                    {
                        result = Convert.ChangeType( value, type );
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        protected object SetFieldValue( FieldInfo field, object value )
        {
            if( !TryChangeType( field.FieldType, value, out object result ) )
            {
                return $"Can not convert '{value}' to type of '{field.FieldType}'.";
            }
            else
            {
                try
                {
                    field.SetValue( Target, result );
                    return $"{field.Name} set to '{value}'.";
                }
                catch
                {
                    return "Set failed.";
                }
            }
        }
        protected object SetFieldValue( MethodInfo set_method, object value )
        {
            if( !TryChangeType( set_method.GetParameters()[0].ParameterType, value, out object result ) )
            {
                return $"Can not convert '{value}' to type of '{set_method.GetParameters()[0].ParameterType}'.";
            }
            else
            {
                try
                {
                    set_method.Invoke( Target, new object[1] { result } );
                    return $"{set_method.Name} set to '{value}'.";
                }
                catch
                {
                    return "Set failed.";
                }
            }
        }
        protected object GetFieldValue( FieldInfo field )
        {
            try
            {
                var obj = field.GetValue( Target );
                if( obj == null ) obj = "Null";
                return $"{field.Name} is '{obj}'";
            }
            catch
            {
                return $"Get failed.";
            }
        }
        protected object GetFieldValue( MethodInfo get_method )
        {
            try
            {
                var obj = get_method.Invoke( Target, null );
                if( obj == null ) obj = "Null";
                return $"{get_method.Name.Substring( 4 )} is '{obj}'";
            }
            catch
            {
                return $"Get failed.";
            }
        }
        protected void GrabField( FieldInfo field )
        {
            _historyObject.Add( _target );
            _historyFieldName.Add( LocationText );

            _target = field.GetValue( _target );
        }

        private bool compareTypes( Type[] types, ParameterInfo[] parameters )
        {
            if( types.Length != parameters.Length ) return false;

            for( var i = 0; i < types.Length; i++ )
            {
                if( types[i] == typeof( Type ) ) continue;

                if( !Equals( parameters[i].ParameterType, types[i] ) ) return false;
            }

            return true;
        }

        /// <summary>
        /// 지정된 배열의 index 위치에서 요소를 반환합니다.
        /// </summary>
        /// <param name="args">배열</param>
        /// <param name="index">인덱스</param>
        /// <returns>인덱스 범위 초과인 경우 빈 문자열을 반환하고, 그렇지 않은 경우 지정된 위치의 문자열을 반환한다.</returns>
        protected string Get( string[] args, int index )
        {
            if( index < 0 || index >= args.Length ) return string.Empty;
            else return args[index];
        }
        /// <summary>
        /// 지정된 배열의 startIndex 위치부터 마지막 요소까지를 새 배열에 복사하여 반환합니다.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        protected object[] GetFrom( string[] args, int startIndex )
        {
            if( startIndex >= args.Length ) return null;
            else
            {
                var arr = new List<object>();
                for( var i = startIndex; i < args.Length; i++ ) arr.Add( args[i] );
                return arr.ToArray();
            }
        }
        /// <summary>
        /// 지정된 배열의 startIndex 위치부터 count개의 요소를 새 배열에 복사하여 반환합니다.
        /// <br>지정된 count가 너무 많아서 배열의 인덱스를 초과하는 경우 마지막 요소까지만 복사합니다.</br>
        /// </summary>
        /// <param name="args"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected object[] GetFrom( string[] args, int startIndex, int count )
        {
            if( startIndex >= args.Length ) return null;
            else
            {
                var arr = new List<object>();
                for( var i = 0; i < count && i + startIndex < args.Length; i++ ) arr.Add( args[i + startIndex] );
                return arr.ToArray();
            }
        }
        #endregion
    }
}
