using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Reflection;
using McQLib.Core;
using McQLib.Recipes;
using McQLib.Device;
using System.Collections;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Q730
{
    public enum Answer
    {
        NotAlways,  // 매번 묻기
        AlwaysYes,  // 모두 예
        AlwaysNo    // 모두 아니오
    }

    /// <summary>
    /// Software 설정 값입니다.
    /// <br><see cref="SoftwareConfiguration"/> 클래스 내부에 설정값의 카테고리를 <see langword="static"/> 형식 클래스로 지정하고, 마찬가지로 각 설정값을 <see langword="static"/> 형식의 속성으로 구성하십시오.</br>
    /// <br>각 설정값은 지정된 속성의 이름이 아닌 ID로 구분되어 저장 및 불러오기 됩니다. 따라서 각 속성에 <see cref="IDAttribute"/> 특성을 사용하여 다른 모든 <see cref="SoftwareConfiguration"/>의 중첩 클래스를 포함하여 중복되지 않는 적절한 ID를 부여하고 관리하십시오.</br>
    /// <br>설정 파일에 ID값 대신 노출하려는 문자열을 <see cref="IDAttribute"/> 특성으로 지정할 수도 있습니다. 단, 이러한 경우에도 중복은 허용되지 않습니다.</br>
    /// <br><see cref="IDAttribute"/> 특성이 존재하지 않는 속성 또는 일반 필드는 저장되지 않습니다.</br>
    /// </summary>
    public static class SoftwareConfiguration
    {
        private static string _saveFilePath = Path.Combine( Util.StartDirectory, "Software.config" );

        public static PropertyChangedEventHandler PropertyChanged;
        private static void NotifyPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke( null, new PropertyChangedEventArgs( propertyName ) );
        }

        // C
        public static class Common
        {
            [ID( "C00" )]
            public static string LastPatchNoteCreationTime { get; set; } = string.Empty;

            [ID( "C01" )]
            public static int Width { get; set; }

            [ID( "C02" )]
            public static int Height { get; set; }

        }

        // S
        public static class SequenceBuilder
        {
            /// <summary>
            /// 시퀀스 빌더에서 들여쓰기를 사용하는 옵션입니다.
            /// </summary>
            [ID( "S00" )]
            public static bool Indentation { get; set; } = true;
            /// <summary>
            /// 시퀀스 빌더에서 Cycle-Loop 짝을 자동으로 추가해주는 옵션입니다.
            /// </summary>
            [ID( "S01" )]
            public static bool AutoAddCycleLoopPair { get; set; } = false;
            /// <summary>
            /// 시퀀스 빌더에서 기본 시퀀스 경로가 아닌 다른 경로에서 시퀀스를 불러올 때, 해당 시퀀스 파일을 기본 시퀀스 경로에 자동으로 복사할지의 여부를 매번 묻는지의 여부입니다.
            /// </summary>
            [ID( "S02" )]
            public static Answer CopySequence { get; set; } = Answer.NotAlways;
            /// <summary>
            /// 시퀀스 파일을 삭제할 때 삭제 확인을 매번 묻는지의 여부입니다.
            /// </summary>
            [ID( "S03" )]
            public static Answer RemoveSequenceFile { get; set; } = Answer.NotAlways;
            /// <summary>
            /// 시퀀스 빌더에서 안전 조건을 글로벌로 사용하는 옵션입니다.
            /// </summary>
            [ID( "S04" )]
            public static bool GlobalSafetyCondition { get; set; } = false;
            /// <summary>
            /// 레시피 아이콘 영역에 마우스를 올려놓을 때 레시피에 대한 설명을 툴팁으로 표시해주는 옵션입니다.
            /// </summary>
            [ID( "S05" )]
            public static bool ShowManualToolTip { get; set; } = true;
            /// <summary>
            /// 레시피 파라미터 영역에 마우스를 올려놓을 때 레시피 파라미터 전체 내용을 툴팁으로 표시해주는 옵션입니다.
            /// </summary>
            [ID( "S06" )]
            public static bool ShowDetailsToolTip { get; set; } = true;
        }

        // G
        public static class GRID
        {
            [ID( "G00" )]
            public static Color IdleBackColor { get; set; } = Color.LightGray;
            [ID( "G01" )]
            public static Color RunBackColor { get; set; } = Color.LimeGreen;
            //[ID( "G02" )] 옵션 삭제
            //public static Color StoppedBackColor { get; set; } = Color.Orange;
            [ID( "G03" )]
            public static Color PausedBackColor { get; set; } = Color.LightBlue;
            [ID( "G04" )]
            public static Color SafetyBackColor { get; set; } = Color.Red;
            [ID( "G05" )]
            public static Color ErrorBackColor { get; set; } = Color.Red;

            [ID( "G06" )]
            public static Color IdleForeColor { get; set; } = Color.Black;
            [ID( "G07" )]
            public static Color RunForeColor { get; set; } = Color.Black;
            //[ID( "G08" )] 옵션 삭제
            //public static Color StoppedForeColor { get; set; } = Color.Black;
            [ID( "G09" )]
            public static Color PausedForeColor { get; set; } = Color.White;
            [ID( "G0A" )]
            public static Color SafetyForeColor { get; set; } = Color.White;
            [ID( "G0B" )]
            public static Color ErrorForeColor { get; set; } = Color.White;
        }

        // M
        public static class Measurement
        {
            [ID( "M00" )]
            public static string DefaultDirectory { get; set; } = string.Empty;
            //[ID( "M01" )] 옵션 삭제
            //public static bool Appending
            //{
            //    get => Communicator.Appending;
            //    set => Communicator.Appending = value;
            //}
            //[ID( "M02" )] 옵션 삭제
            //public static bool SaveOnRam 
            //{
            //    get => Channel.SaveOnRam; 
            //    set => Channel.SaveOnRam = value; 
            //}
            [ID( "M03" )]
            public static bool ChannelLogging
            {
                get => Channel.Logging;
                set => Channel.Logging = value;
            }
            //[ID( "M04" )] 옵션 삭제
            //public static int MaxDataCount { get; set; } = -1;
            [ID( "M05" )]
            public static string SaveFileNameFormat { get; set; } = "";

            //[ID( "M06" )] 옵션 삭제
            //public static int DecimalPlace_Voltage { get; set; } = 4;
            //[ID( "M07" )] 옵션 삭제
            //public static SiUnites VoltageUnit { get; set; } = SiUnites.Default;
            //[ID( "M08" )] 옵션 삭제
            //public static SiUnites CurrentUnit { get; set; } = SiUnites.Default;
            //[ID( "M09" )] 옵션 삭제
            //public static SiUnites PowerUnit { get; set; } = SiUnites.Default;
            [ID( "M0A" )]
            public static TimeUnit TimeUnit { get; set; } = TimeUnit.Hour;
            //[ID( "M0B" )] 옵션 삭제
            //public static int RamCount
            //{
            //    get => Channel.RamCount;
            //    set => Channel.RamCount = value;
            //}
            //[ID( "M12" )] 옵션 삭제
            //public static int DecimalPlace_Current { get; set; } = 4;
            //[ID( "M13" )] 옵션 삭제
            //public static int DecimalPlace_Power { get; set; } = 4;

            [ID( "M0C" )]
            public static UnitInfo VoltageUnit
            {
                get => _voltageUnit;
                set
                {
                    if ( _voltageUnit != value )
                    {
                        _voltageUnit = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private static UnitInfo _voltageUnit = new UnitInfo( UnitType.Voltage );

            [ID( "M0D" )]
            public static UnitInfo CurrentUnit
            {
                get => _currentUnit;
                set
                {
                    if ( _currentUnit != value )
                    {
                        _currentUnit = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private static UnitInfo _currentUnit = new UnitInfo( UnitType.Current );

            [ID( "M0E" )]
            public static UnitInfo PowerUnit { get; set; } = new UnitInfo( UnitType.Power );
            [ID( "M0F" )]
            public static int GraphDecimalPlace { get; set; } = 4;
            [ID( "M10" )]
            public static bool IsClearSequenceWhenEnd 
            {
                get => Channel.IsClearSequenceWhenEnd;
                set => Channel.IsClearSequenceWhenEnd = value;
            }
            [ID("M11")]
            public static UnitInfo ExportVoltageUnit
            {
                get => Channel.VoltageUnit;
                set => Channel.VoltageUnit = value;
            }
            [ID("M12")]
            public static UnitInfo ExportCurrentUnit
            {
                get => Channel.CurrentUnit;
                set => Channel.CurrentUnit = value;
            }
            [ID("M13")]
            public static UnitInfo CapacityUnit
            {
                get => _capacityUnit;
                set
                {
                    if (_capacityUnit != value)
                    {
                        _capacityUnit = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private static UnitInfo _capacityUnit = new UnitInfo(UnitType.Capacity);
        }

        // R
        public static class Recipe
        {
            [ID( "R00" )]
            public static bool ChargeEnabled
            {
                get => RecipeSetting.Charge.Enabled;
                set => RecipeSetting.Charge.Enabled = value;
            }
            [ID( "R01" )]
            public static bool DischargeEnabled
            {
                get => RecipeSetting.Discharge.Enabled;
                set => RecipeSetting.Discharge.Enabled = value;
            }
            [ID( "R02" )]
            public static bool RestEnabled
            {
                get => RecipeSetting.Rest.Enabled;
                set => RecipeSetting.Rest.Enabled = value;
            }
            [ID( "R03" )]
            public static bool PatternEnabled
            {
                get => RecipeSetting.Pattern.Enabled;
                set => RecipeSetting.Pattern.Enabled = value;
            }
            [ID( "R04" )]
            public static bool TemperatureEnabled
            {
                get => RecipeSetting.Temperature.Enabled;
                set => RecipeSetting.Temperature.Enabled = value;
            }
            [ID( "R05" )]
            public static bool AcResistanceEnabled
            {
                get => RecipeSetting.AcResistance.Enabled;
                set => RecipeSetting.AcResistance.Enabled = value;
            }
            [ID( "R06" )]
            public static bool DcResistanceEnabled
            {
                get => RecipeSetting.DcResistance.Enabled;
                set => RecipeSetting.DcResistance.Enabled = value;
            }
            [ID( "R07" )]
            public static bool FrequencyResponseEnabled
            {
                get => RecipeSetting.FrequencyResponse.Enabled;
                set => RecipeSetting.FrequencyResponse.Enabled = value;
            }
            [ID( "R08" )]
            public static bool OpenCircuitVoltageEnabled
            {
                get => RecipeSetting.OpenCircuitVoltage.Enabled;
                set => RecipeSetting.OpenCircuitVoltage.Enabled = value;
            }
            [ID( "R09" )]
            public static bool TransientResponseEnabled
            {
                get => RecipeSetting.TransientResponse.Enabled;
                set => RecipeSetting.TransientResponse.Enabled = value;
            }
            [ID( "R0A" )]
            public static bool CycleEnabled
            {
                get => RecipeSetting.Cycle.Enabled;
                set => RecipeSetting.Cycle.Enabled = value;
            }
            [ID( "R0B" )]
            public static bool LoopEnabled
            {
                get => RecipeSetting.Loop.Enabled;
                set => RecipeSetting.Loop.Enabled = value;
            }
        }

        // L
        public static class LIST
        {
            public enum Columns
            {
                CH,
                State,
                TotalTime,
                Voltage,
                Current,
                StepNo,
                Recipe,
                Sequence,
                Name,
                SaveDirectory,
                Message,
                StepProgress
            }

            [ID( "L00" )]
            public static List<Columns> ColumnOrders
            {
                get => _columnOrders;
                set
                {
                    if ( _columnOrders != value )
                    {
                        _columnOrders = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private static List<Columns> _columnOrders = new List<Columns>( new Columns[]
            { 
                Columns.CH,
                Columns.State, 
                Columns.TotalTime, 
                Columns.Voltage,
                Columns.Current, 
                Columns.StepNo,
                Columns.StepProgress,
                Columns.Recipe, 
                Columns.Sequence, 
                Columns.Name, 
                Columns.SaveDirectory, 
                Columns.Message
            } );

            [ID( "L01" )]
            public static int[] ColumnSizes { get; set; } = new int[12]
            {
                50,
                100,
                80,
                80,
                80,
                70,
                80,
                100,
                120,
                100,
                400,
                100
            };

            public static bool ColumnChanged = true;
        }

        // D
        public static class DETAIL
        {
            [ID( "D01" )]
            public static int SkipPoints { get; set; } = 10;
            [ID( "D02" )]
            public static string CustomIndicator1
            {
                get => _customIndicator1;
                set
                {
                    if (value != _customIndicator1 )
                    {
                        _customIndicator1 = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private static string _customIndicator1 = "Voltage";
            [ID( "D03" )]
            public static string CustomIndicator2
            {
                get => _customIndicator2;
                set
                {
                    if ( value != _customIndicator2 )
                    {
                        _customIndicator2 = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private static string _customIndicator2 = "Current";
            [ID( "D04" )]
            public static string CustomIndicator3
            {
                get => _customIndicator3;
                set
                {
                    if ( value != _customIndicator3 )
                    {
                        _customIndicator3 = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private static string _customIndicator3 = "Power";
            [ID( "D05" )]
            public static string CustomIndicator4
            {
                get => _customIndicator4;
                set
                {
                    if ( value != _customIndicator4 )
                    {
                        _customIndicator4 = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private static string _customIndicator4 = "Temperature";
        }

        /// <summary>
        /// 저장된 구성 설정 정보를 불러옵니다.
        /// <br>현재 설정 정보가 저장되지 않은 경우 변경 사항을 잃게 됩니다.</br>
        /// </summary>
        /// <returns></returns>
        public static bool Load()
        {
            var configurations = new List<KeyValuePair<string, string>>();

            if ( new FileInfo( _saveFilePath ).Exists )
            {
                using ( var sr = new StreamReader( _saveFilePath ) )
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

                var types = typeof( SoftwareConfiguration ).GetNestedTypes();

                foreach ( var type in types )
                {
                    var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly );

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
                            if ( Equals( property.PropertyType.BaseType, typeof( Enum ) ) )
                            {   // 열거형 형식
                                property.SetValue( null, Enum.Parse( property.PropertyType, configuration[0].Value ) );
                            }
                            else if ( Equals( property.PropertyType, typeof( Color ) ) )
                            {   // struct Color 형식
                                var colorString = configuration[0].Value.Replace( "Color [", "" ).Replace( "]", "" );

                                // 알려진 이름의 Color인 경우 "Color [color_name]"으로 저장됨
                                var color = Color.FromName( colorString );
                                if ( color.IsKnownColor )
                                {
                                    property.SetValue( null, color );
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
                                            property.SetValue( null, Color.FromArgb( r, g, b ) );
                                        }
                                    }
                                    else if ( split.Length == 4 )
                                    {
                                        if ( int.TryParse( split[0], out int a ) && 0 <= a && a <= 255 &&
                                            int.TryParse( split[1], out int r ) && 0 <= r && r <= 255 &&
                                            int.TryParse( split[2], out int g ) && 0 <= g && g <= 255 &&
                                            int.TryParse( split[3], out int b ) && 0 <= b && b <= 255 )
                                        {
                                            property.SetValue( null, Color.FromArgb( a, r, g, b ) );
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
                                        list.Add( Convert.ChangeType( split[i], elType ) );
                                    }
                                    catch
                                    {
                                        //list.Add( null );
                                    }
                                }

                                property.SetValue( null, list.GetType().GetMethod( "ToArray" ).Invoke( list, null ) );
                            }
                            else if ( property.PropertyType.GetInterface( "IList" ) != null )
                            {
                                var split = configuration[0].Value.Split( ",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

                                var elType = property.PropertyType.GetGenericArguments()[0];
                                
                                var list = Activator.CreateInstance( typeof( List<> ).MakeGenericType( new Type[] { elType } ) ) as IList;

                                if ( Equals( elType.BaseType, typeof( Enum ) ) )
                                {
                                    for ( var i = 0; i < split.Length; i++ )
                                    {
                                        try
                                        {
                                            list.Add( Enum.Parse( elType, split[i] ) );
                                        }
                                        catch ( Exception ex )
                                        {
                                            //list.Add( null );
                                        }
                                    }
                                }
                                else
                                {
                                    for ( var i = 0; i < split.Length; i++ )
                                    {
                                        try
                                        {
                                            list.Add( Convert.ChangeType( split[i], elType ) );
                                        }
                                        catch ( Exception ex )
                                        {
                                            //list.Add( null );
                                        }
                                    }
                                }

                                property.SetValue( null, list );
                            }
                            else if ( Equals( property.PropertyType, typeof( UnitInfo ) ) )
                            {
                                var split = configuration[0].Value.Split( ",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
                                if ( split.Length == 3 && int.TryParse( split[0], out int unitType ) && int.TryParse( split[1], out int siType ) && int.TryParse( split[2], out int decimalPlace ) )
                                {
                                    var unitInfo = new UnitInfo( ( UnitType )unitType );
                                    unitInfo.SiUnit = ( SiUnits )siType;
                                    unitInfo.DecimalPlace = decimalPlace;

                                    property.SetValue( null, unitInfo );
                                }
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
            var types = typeof( SoftwareConfiguration ).GetNestedTypes();

            using ( var sw = new StreamWriter( _saveFilePath ) )
            {
                foreach ( var type in types )
                {
                    var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly );

                    foreach ( var property in properties )
                    {
                        var id = ( IDAttribute )property.GetCustomAttribute( typeof( IDAttribute ) );
                        if ( id == null ) continue;

                        var value = property.GetValue( null );

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
                    }
                }
            }
        }

        private static List<object> _initialValues = new List<object>();

        static SoftwareConfiguration()
        {
            // 각 필드들의 초기값을 저장해둔다.
            var types = typeof( SoftwareConfiguration ).GetNestedTypes();
            foreach ( var type in types )
            {
                var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly );

                foreach ( var property in properties )
                {
                    // ID 특성 굳이 저장하지 않아도 된다. tpyes와 properties는 항상 동일 순서로 가져와짐.
                    // 단, ID 특성이 있는지 없는지의 여부는 확인할 필요가 있음.
                    if ( ( IDAttribute )property.GetCustomAttribute( typeof( IDAttribute ) ) == null ) continue;

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
            var types = typeof( SoftwareConfiguration ).GetNestedTypes();
            var index = 0;
            foreach ( var type in types )
            {
                var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly );

                foreach ( var property in properties )
                {
                    // ID 특성 굳이 저장하지 않아도 된다. tpyes와 fields는 항상 동일 순서로 가져와짐.
                    // 단, ID 특성이 있는지 없는지의 여부는 확인할 필요가 있음.
                    if ( ( IDAttribute )property.GetCustomAttribute( typeof( IDAttribute ) ) == null ) continue;

                    property.SetValue( null, Convert.ChangeType( _initialValues[index++], property.PropertyType ) );
                }
            }
        }
    }
}
