using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using McQLib.Device;
using McQLib.Recipes;

namespace McQLib.Core
{
    /// <summary>
    /// McQLib에서 지원하는 유틸리티 클래스입니다.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// 소프트웨어의 시작 경로입니다.
        /// </summary>
        public static string StartDirectory = ""; // => Path.Combine( Assembly.GetEntryAssembly().Location.Replace( Assembly.GetEntryAssembly().ManifestModule.Name, "" ) );
        /// <summary>
        /// 지정된 바이트 배열을 표현하는 문자열로 변환합니다.
        /// </summary>
        /// <param name="packet">변환할 바이트 배열입니다.</param>
        /// <returns>변환된 문자열입니다.</returns>
        public static string BytesToString( byte[] packet )
        {
            var builder = new StringBuilder();

            for ( var i = 0; i < packet.Length; i++ )
            {
                builder.Append( $"{packet[i]:X2} " );
            }

            return builder.ToString();
        }
        /// <summary>
        /// 지정된 ADDR과 CH로 채널의 인덱스를 계산하여 반환합니다.
        /// </summary>
        /// <param name="addr">보드 번호입니다.</param>
        /// <param name="ch">채널 번호입니다.</param>
        /// <returns>계산된 인덱스입니다.</returns>
        public static int GetIndex( byte addr, byte ch )
        {
            if ( addr != 0 )
            {
                return ( addr - 1 ) * 8 + ch - 1;
            }
            else return ch - 1;
        }
        /// <summary>
        /// 지정된 인덱스로 채널의 ADDR(보드 번호)를 계산하여 반환합니다.
        /// </summary>
        /// <param name="channelNo">채널의 인덱스입니다.</param>
        /// <returns>계산된 ADDR입니다.</returns>
        public static byte GetADDR( int channelNo ) => ( byte )( channelNo / 8 + 1 );
        /// <summary>
        /// 지정된 인덱스로 채널의 CH(채널 번호)을 계산하여 반환합니다.
        /// </summary>
        /// <param name="channelNo">채널의 인덱스입니다.</param>
        /// <returns>계산된 CH입니다.</returns>
        public static byte GetCH( int channelNo ) => ( byte )( channelNo % 8 + 1 );

        /// <summary>
        /// (Commands 내부 열거형 전용) Enum 형식으로 박싱된 값을 Declaring Enum 형식 값으로 변환합니다.
        /// </summary>
        /// <param name="cmd">변환할 Enum 형식 값입니다.</param>
        /// <returns></returns>
        internal static object ConvertEnumToValue( Enum cmd )
        {
            return Convert.ChangeType( cmd, cmd.GetTypeCode() );
        }
        /// <summary>
        /// Enum 형식으로 박싱된 값을 Declaring 값 형식으로 변환합니다.
        /// </summary>
        /// <param name="e">변환할 Enum 값입니다.</param>
        /// <returns>변환된 값입니다.</returns>
        internal static object ToValue( this Enum e ) => ConvertEnumToValue( e );

        /// <summary>
        /// (Commands 내부 열거형 전용) ushort 형식 값을 정확히 일치하는 Commands 내부 형식으로 변환합니다.
        /// </summary>
        /// <param name="cmd">변환할 ushort 형식 값입니다.</param>
        /// <returns></returns>
        internal static Enum ConvertCmdToEnum( ushort cmd )
        {
            var value = new Q_UInt16( cmd );

            switch ( value.Offset0 )
            {
                case 0x00:
                    return ( Commands.CommonCommands )cmd;

                case 0x0B:
                    return ( Commands.MultiChannelCommands )cmd;

                case 0x10:
                    return ( Commands.BatteryCycler_SetGetCommands )cmd;

                case 0x11:
                    return ( Commands.BatteryCycler_GetMeasureCommands )cmd;

                case 0x14:
                    return ( Commands.Q3000Q2000_SlotCommands )cmd;

                case 0x80:
                    return ( Commands.ProductionCommonCommands )cmd;

                case 0xF0:
                    return ( Commands.CalibrationCommands )cmd;

                default:
                    return null;
            }
        }
        internal static void SetBrowsable( object obj, string propertyName, bool browsable )
        {
            var descriptor = TypeDescriptor.GetProperties( obj.GetType() )[propertyName];
            var attribute = ( BrowsableAttribute )descriptor.Attributes[typeof( BrowsableAttribute )];

            attribute.GetType().GetField( "browsable", BindingFlags.NonPublic | BindingFlags.Instance ).SetValue( attribute, browsable );
        }
        internal static void SetReadonly( object obj, string propertyName, bool isReadOnly )
        {
            var descriptor = TypeDescriptor.GetProperties( obj.GetType() )[propertyName];
            var attribute = ( ReadOnlyAttribute )descriptor.Attributes[typeof( ReadOnlyAttribute )];

            attribute.GetType().GetField( "isReadOnly", BindingFlags.NonPublic | BindingFlags.Instance ).SetValue( attribute, isReadOnly );
        }
        internal static RecipeType ConvertModeToRecipeType( Mode1 mode1, Mode2 mode2 )
        {
            switch ( mode1 )
            {
                case Mode1.Rest:
                    return RecipeType.Rest;

                case Mode1.Cycle:
                    return RecipeType.Cycle;

                case Mode1.Loop:
                    return RecipeType.Loop;

                case Mode1.Jump:
                    return RecipeType.Jump;

                case Mode1.End:
                    return RecipeType.End;

                case Mode1.Charge:
                    return RecipeType.Charge;

                case Mode1.Discharge:
                    return RecipeType.Discharge;

                case Mode1.Pattern:
                    return RecipeType.Pattern;

                case Mode1.AnodeCharge:
                    return RecipeType.AnodeCharge;

                case Mode1.AnodeDischarge:
                    return RecipeType.AnodeDischarge;

                case Mode1.Measure:
                    switch ( mode2 )
                    {
                        case Mode2.TRA:
                            return RecipeType.TransientResponse;

                        case Mode2.OCV:
                            return RecipeType.OpenCircuitVoltage;

                        case Mode2.DCR:
                            return RecipeType.DcResistance;

                        case Mode2.ACR:
                            return RecipeType.AcResistance;

                        case Mode2.FRA:
                            return RecipeType.FrequencyResponse;
                    }
                    break;
            }

            return RecipeType.NotDefined;
        }
        /// <summary>
        /// ms(밀리 초, Milliseconds) 형태의 시간 값을 HH:mm:ss 포맷 문자열로 변환합니다.
        /// </summary>
        /// <param name="ms">변환할 ms 값입니다.</param>
        /// <returns>변환된 문자열입니다.</returns>
        public static string ConvertMsToString( ulong ms )
        {
            // ms를 hh:mm:ss 형식 문자열로 변환

            string result = string.Empty;
            ulong f, s, m, h, d;

            f = ms % 1000;

            // ms로 남는 애들은 그냥 버린다.
            ms /= 1000;

            s = ms % 60;

            ms /= 60;

            m = ms % 60;

            ms /= 60;

            h = ms;
            //h = ms % 24;

            //ms /= 24;

            //d = ms;

            return $"{h:#00}:{m:00}:{s:00}";
        }
        internal static uint? ConvertTimsStringToMs( string value )
        {
            if ( value == null || value.Trim().Length == 0 ) return null;

            var split = value.Split( ':' ).Select( i => i.Length == 0 ? 0 : double.Parse( i ) ).ToArray();

            double days = 0, hours = 0, minutes = 0, seconds = 0, milliseconds = 0;

            if ( split.Length == 1 )
            {
                seconds = split[0];
            }
            else if ( split.Length == 2 )
            {
                minutes = split[0];
                seconds = split[1];
            }
            else if ( split.Length == 3 )
            {
                hours = split[0];
                minutes = split[1];
                seconds = split[2];
            }
            else if ( split.Length == 4 )
            {
                days = split[0];
                hours = split[1];
                minutes = split[2];
                seconds = split[3];
            }

            milliseconds += days * 3600 * 1000;
            milliseconds += hours * 3600 * 1000;
            milliseconds += minutes * 60 * 1000;
            milliseconds += seconds * 1000;

            return ( uint )milliseconds;

            //if ( seconds >= 60 )
            //{
            //    minutes += 1;
            //    seconds -= 60;
            //}
            //if ( minutes >= 60 )
            //{
            //    hours += 1;
            //    minutes -= 60;
            //}
            //if ( hours >= 24 )
            //{
            //    days += 1;
            //    hours -= 24;
            //}
            //if ( seconds - ( int )seconds > 0 )
            //{
            //    milliseconds = seconds - ( int )seconds;
            //    seconds -= milliseconds;
            //    milliseconds *= 1000;
            //}

            //return new TimeSpan( ( int )days, ( int )hours, ( int )minutes, ( int )seconds, ( int )milliseconds );
        }

        internal static string ConvertRegisterErrorToString( this RegisterError registerError )
        {
            if ( registerError == RegisterError.NoError ) return "NoError";

            var flags = new List<string>();

            if ( registerError.HasFlag( RegisterError.RecipeSendingFail ) ) flags.Add( RegisterError.RecipeSendingFail.ToString() );
            if ( registerError.HasFlag( RegisterError.BatterySafeAlarm ) ) flags.Add( RegisterError.BatterySafeAlarm.ToString() );
            if ( registerError.HasFlag( RegisterError.SdFileMeet4GB ) ) flags.Add( RegisterError.SdFileMeet4GB.ToString() );
            if ( registerError.HasFlag( RegisterError.SdFreeSpaceTooSmall ) ) flags.Add( RegisterError.SdFreeSpaceTooSmall.ToString() );
            if ( registerError.HasFlag( RegisterError.SdReadWriteFail ) ) flags.Add( RegisterError.SdReadWriteFail.ToString() );
            if ( registerError.HasFlag( RegisterError.SdInitialFail ) ) flags.Add( RegisterError.SdInitialFail.ToString() );
            if ( registerError.HasFlag( RegisterError.InitialFail ) ) flags.Add( RegisterError.InitialFail.ToString() );

            if ( flags.Count == 0 ) return "WrongFlags";

            var res = flags[0];
            for ( var i = 1; i < flags.Count; i++ )
            {
                res += $" | {flags[i]}";
            }

            return res;
        }
        /// <summary>
        /// 지정된 개체에서 지정된 이름과 인수 목록이 일치하는 멤버 메서드를 찾아 호출합니다.
        /// <br>Public 또는 NonPublic의 멤버 메서드가 모두 검색 대상이며, 상속된 멤버는 검색 대상에서 제외됩니다.</br>
        /// </summary>
        /// <param name="obj">멤버 메서드를 소유하는 개체입니다.</param>
        /// <param name="methodName">호출할 메서드의 이름(대/소문자 구분)입니다.</param>
        /// <param name="parameters">호출할 메서드에 사용될 인수 리스트입니다.</param>
        /// <returns>메서드의 실행 결과(반환값)입니다. 반환형이 Void인 메서드의 경우에도 null입니다.</returns>
        /// <exception cref="Exception">지정된 조건으로 검색된 메서드가 존재하지 않습니다.</exception>
        /// <exception cref="AmbiguousMatchException">지정된 조건으로 검색된 메서드가 2개 이상입니다.</exception>
        public static object CallMethod( object obj, string methodName, params object[] parameters )
        {
            Type[] types;

            if ( parameters == null || parameters.Length == 0 )
            {
                types = null;
                parameters = null;
            }
            else
            {
                types = new Type[parameters.Length];
                for ( var i = 0; i < parameters.Length; i++ )
                {
                    if ( parameters[i] == null ) types[i] = null;
                    else types[i] = parameters[i].GetType();
                }
            }

            var methods = obj.GetType().GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly );

            var canCallMethods = new List<MethodInfo>();

            for ( var i = 0; i < methods.Length; i++ )
            {
                if ( methods[i].Name != methodName ) continue;

                var parameterInfos = methods[i].GetParameters();

                var isSame = true;

                if ( parameters != null )
                {
                    for ( var j = 0; j < parameterInfos.Length; j++ )
                    {
                        if ( types[j] == null )
                        {
                            if ( parameterInfos[j].ParameterType.IsValueType )
                            {
                                isSame = false;
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if ( parameterInfos[j].ParameterType != types[j] )
                        {
                            isSame = false;
                            break;
                        }
                    }
                }

                if ( isSame )
                {
                    canCallMethods.Add( methods[i] );
                }
            }

            if ( canCallMethods.Count == 0 )
            {
                throw new Exception( "Method not found." );
            }
            else if ( canCallMethods.Count == 1 )
            {
                return methods[0].Invoke( obj, parameters );
            }
            else
            {
                throw new AmbiguousMatchException( "Too many methods." );
            }
        }
    }
}