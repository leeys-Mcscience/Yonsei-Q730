using System;

namespace McQLib.Core
{
    /// <summary>
    /// SI 단위계를 나타내는 열거형입니다.
    /// </summary>
    public enum SiUnits
    {
        /// <summary>
        /// 10^3 단위의 값입니다.
        /// </summary>
        Kilo,       // 0
        /// <summary>
        /// 10^0 단위의 값입니다.
        /// </summary>
        Default,    // 1
        /// <summary>
        /// 10^-3 단위의 값입니다.
        /// </summary>
        Milli,      // 2
        /// <summary>
        /// 10^-6 단위의 값입니다.
        /// </summary>
        Micro,      // 3
        /// <summary>
        /// 10^-9 단위의 값입니다.
        /// </summary>
        Nano        // 4
    }
    /// <summary>
    /// 시간 단위를 나타내는 열거형입니다.
    /// </summary>
    public enum TimeUnit
    {
        /// <summary>
        /// 시간입니다.
        /// </summary>
        Hour,
        /// <summary>
        /// 분입니다.
        /// </summary>
        Minute,
        /// <summary>
        /// 초입니다.
        /// </summary>
        Second,
        /// <summary>
        /// 밀리 초입니다.
        /// </summary>
        MilliSecond
    }
    /// <summary>
    /// 값이 사용할 단위의 종류를 나타내는 열거형입니다.
    /// </summary>
    public enum UnitType
    {
        /// <summary>
        /// 전압입니다.
        /// </summary>
        Voltage,
        /// <summary>
        /// 전류입니다.
        /// </summary>
        Current,
        /// <summary>
        /// 전력입니다.
        /// </summary>
        Power,
        /// <summary>
        /// 저항입니다.
        /// </summary>
        Resistance,
        /// <summary>
        /// 시간입니다.
        /// </summary>
        Time,
        /// <summary>
        /// 용량입니다.
        /// </summary>
        Capacity,
        /// <summary>
        /// 시퀀스스텝이름입니다.
        /// </summary>
        SequenceName,
    }

    /// <summary>
    /// 유닛 구성 설정을 저장 및 관리하는 클래스입니다.
    /// </summary>
    public class UnitInfo
    {
        private static int[] _siUnitE = new int[5] { 3, 0, -3, -6, -9 };

        private string[] _unitString;
        private bool _isTimeUnit;

        public UnitType UnitType { get; set; }
        public SiUnits SiUnit { get; set; } = SiUnits.Default;
        public TimeUnit TimeUnit { get; set; } = TimeUnit.Second;
        public int DecimalPlace { get; set; } = 4;


        public UnitInfo( UnitType unitType )
        {
            switch ( UnitType = unitType )
            {
                case UnitType.Voltage:
                    _unitString = new string[5] { "kV", "V", "mV", "μV", "nV" };
                    break;

                case UnitType.Current:
                    _unitString = new string[5] { "kA", "A", "mA", "μA", "nA" };
                    break;

                case UnitType.Power:
                    _unitString = new string[5] { "kW", "W", "mW", "μW", "nW" };
                    break;

                case UnitType.Resistance:
                    _unitString = new string[5] { "kΩ", "Ω", "mΩ", "", "" };
                    break;

                case UnitType.Time:
                    _isTimeUnit = true;
                    _unitString = new string[4] { "h", "min", "sec", "ms" };
                    break;

                case UnitType.Capacity:
                    _unitString = new string[4] { "", "Ah", "mAh", "μAh" };
                    break;

                case UnitType.SequenceName:
                    _unitString = new string[4] { "", "", "", "" };
                    break;
            }
        }
        public UnitInfo( UnitType unitType, SiUnits siUnit, int decimalPlace ) : this( unitType )
        {
            SiUnit = siUnit;
            DecimalPlace = decimalPlace;
        }
        public UnitInfo( UnitType unitType, TimeUnit timeUnit ) : this( unitType )
        {
            TimeUnit = timeUnit;
        }

        public string UnitString
        {
            get
            {
                if ( _isTimeUnit )
                {
                    return _unitString[( int )TimeUnit];
                }
                else
                {
                    return _unitString[( int )SiUnit];
                }
            }
        }
        /// <summary>
        /// 지정된 SI 단위계의 Default 형식 값(10의 0승)을 현재 <see cref="UnitInfo"/> 개체가 가지는 SI 단위계 자릿수 값을 반영한 문자열 표현으로 변환합니다.
        /// </summary>
        /// <param name="value">변환할 값입니다.</param>
        /// <param name="forcedDecimalPlace">소숫점 이하 몇자리까지 표기할지의 값입니다. 기본값은 -1이며, 기본값으로 지정된 경우 <see cref="UnitInfo.DecimalPlace"/>가 자릿수를 지정하는데 사용됩니다.</param>
        /// <returns></returns>
        public string GetString( double value, int forcedDecimalPlace = -1 )
        {
            if ( _isTimeUnit )
            {
                return change( value, TimeUnit ).ToString( $"f{( forcedDecimalPlace == -1 ? DecimalPlace : forcedDecimalPlace )}" );

            }
            else
            {
                return change( value, SiUnit ).ToString( $"f{( forcedDecimalPlace == -1 ? DecimalPlace : forcedDecimalPlace )}" );
            }
        }

        public double ChangeValue( double value )
        {
            if ( _isTimeUnit )
            {
                return change( value, TimeUnit );

            }
            else
            {
                return change( value, SiUnit );
            }
        }

        public override string ToString()
        {
            if ( _isTimeUnit )
            {
                return $"{( int )UnitType},{( int )TimeUnit},{DecimalPlace}";
            }
            else
            {

                return $"{( int )UnitType},{( int )SiUnit},{DecimalPlace}";
            }
        }

        /// <summary>
        /// 지정된 실수의 단위를 SI 단위계를 사용하여 변환합니다.
        /// </summary>
        /// <param name="value">SI 단위계 상 10의 0승 값(V, A, W 등)입니다. </param>
        /// <param name="unit">변환할 SI 단위계 값 입니다.</param>
        /// <returns></returns>
        private static double change( double value, SiUnits unit )
        {
            return value / Math.Pow( 10, _siUnitE[( int )unit] );
        }
        /// <summary>
        /// 지정된 밀리초 시간 값을 시간 단위를 사용하여 변환합니다.
        /// </summary>
        /// <param name="value">밀리 초(Millisecond) 단위의 시간 값입니다.</param>
        /// <param name="unit">변환할 시간 단위입니다.</param>
        /// <returns></returns>
        private static double change( double value, TimeUnit unit )
        {
            switch ( unit )
            {
                case TimeUnit.MilliSecond:
                    return value;

                case TimeUnit.Second:
                    return value / 1000;

                case TimeUnit.Minute:
                    return value / 1000 / 60;

                case TimeUnit.Hour:
                    return value / 1000 / 60 / 60;

                default:
                    return value;
            }
        }
    }
}
