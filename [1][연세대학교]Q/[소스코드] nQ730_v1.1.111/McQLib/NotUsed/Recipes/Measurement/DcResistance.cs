using System;
using McQLib.Core;
using McQLib.NotUsed.Core;

namespace McQLib.NotUsed.Recipes
{
    public sealed class DcResistance : BaseMeasureRecipe
    {
        public override byte[] ToCommand( ushort stepNo, ushort endStepNo, ushort errorStepNo )
        {
            var builder = new DataBuilder();

            // Reserved (2Byte) - Reserved
            builder.AddCount( 0, 2 );

            // Step count (4Byte) - Reserved
            builder.AddCount( 0, 4 );

            // Step no. (2Byte) - 설정 Step number
            builder.Add( new Q_UInt16( stepNo ) );

            // Cycle no. (4Byte) - Reserved
            builder.AddCount( 0, 4 );

            // Mode1 (1Byte) - Measure : 3
            builder.Add( ( byte )Mode1.Measure );

            // Mode2 (1Byte) - DCR : 8
            builder.Add( ( byte )Mode2.DCR );

            byte high = 0, low = 0;

            // 설정 3rd Current       사용
            // 설정 1st Current       사용
            // 설정 2nd Current       사용
            // 설정 1st Delay         사용
            // 설정 1st Width         사용
            // 설정 Mode select       사용 안 함
            // 설정 Step count        사용 안 함
            // 설정 Sampling 수행 시간 사용
            low |= 0b11111001;

            // [설정 조건 - 사용 여부] (2Byte) 
            builder.Add( high, low );

            // [설정 조건 - 값] (64Byte)
            builder.Add( Current3rd );          // 설정 3rd Current
            builder.Add( Current1st );          // 설정 1st Current
            builder.Add( Current2nd );          // 설정 2nd Current
            builder.Add( Delay1st );            // 설정 1st Delay

            if( Width1st < 1 ) throw new QException( QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR, "1st Width의 값이 1보다 작을 수 없습니다." );
            builder.Add( Width1st * 1000000 );            // 설정 1st Width

            builder.Add( 0 );                   // 설정 Mode select       (Not used)
            builder.Add( 0 );                   // 설정 Step count        (Not used)

            if( TotalTime < 1 ) throw new QException( QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR, "Total Time의 값이 1보다 작을 수 없습니다." );
            builder.Add( TotalTime * 1000000 );           // 설정 Sampling 수행 시간

            builder.Add( Delay2nd );            // 설정 Delay2

            if( Width2nd < 1 ) throw new QException( QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR, "2nd Width의 값이 1보다 작을 수 없습니다." );
            builder.Add( Width2nd * 1000000 );            // 설정 Width2

            builder.Add( 0 );                   // 설정 Raw data mode     (Not used)
            builder.Add( Amplify );             // 설정 증폭 배율
            builder.AddCount( 0, 4 );           // Reserved

            // [안전 조건] (58Byte)
            builder.Add( base.ToCommand( stepNo, endStepNo, errorStepNo ) );

            // [종료 조건 - 사용 안 함] (80Byte)
            builder.AddCount( 0, 80 );

            // [기록 조건 - 사용 여부] 2Byte
            high = low = 0;
            if( Save_Interval != null ) high |= 0b10000000;
            if( Save_Voltage != null ) high |= 0b01000000;
            if( Save_Current != null ) high |= 0b00100000;
            if( Save_Temperature != null ) high |= 0b00010000;
            builder.Add( high, low );

            // [기록 조건 - 값] 24Byte
            builder.Add( Save_Interval );           // 기록 Interval
            builder.Add( Save_Voltage );            // 기록 delta-V
            builder.Add( Save_Current );            // 기록 delta-I
            builder.Add( Save_Temperature );        // 기록 delta-Temp

            // 종료값 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( endStepNo ) );

            // 비정상 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( errorStepNo ) );

            // 반복횟수 (4Byte) - Loop, Jump만 해당
            builder.AddCount( 0, 4 );

            // 스텝 진행 (1Byte)
            builder.Add( 0 );

            // Reserved (7Byte) - Reserved
            builder.AddCount( 0, 7 );

            return builder;
        }

        internal DcResistance() { }

        #region Parameters
        [Group( "Parameter" )]
        [Parameter( "1st Current", ParameterValueType.Double, "050000" )]
        [Unit( "A" )]
        public double Current1st;

        [Parameter( "2nd Current", ParameterValueType.Double, "050100" )]
        [Unit( "A" )]
        public double Current2nd;

        [Invisible]
        [Parameter( "3rd Current", ParameterValueType.Double, "050200" )]
        [Unit( "A" )]
        public double Current3rd = 0;   // 사용 안 함

        [Parameter( "1st Width", ParameterValueType.Double, "050300" )]
        [Unit( "sec" )]
        public double Width1st = 60;

        [Parameter( "2nd Width", ParameterValueType.Integer, "050400" )]
        [Unit( "sec" )]
        public uint Width2nd = 60;

        [Invisible]
        [Parameter( "1st Delay", ParameterValueType.Double, "050500" )]
        [Unit( "sec" )]
        public double Delay1st = 60;    // 고정

        [Invisible]
        [Parameter( "2nd Delay", ParameterValueType.Integer, "050600" )]
        [Unit( "sec" )]
        public uint Delay2nd = 60;

        [Parameter( "Total Time", ParameterValueType.Double, "050700" )]
        [Unit( "sec" )]
        public double TotalTime = 1;

        [Group( "Save Condition" )]
        [Parameter( "△Time", ParameterValueType.Time, "050800" )]
        [Help( "Time이 지정된 값만큼 경과할 때마다 데이터를 저장합니다. \r\n시간:분:초 순입니다." )]
        public uint? Save_Interval;

        [Parameter( "△Current", ParameterValueType.Double, "050900" )]
        [Unit( "A" )]
        [Help( "Current(A)가 지정된 값만큼 변화할 때마다 데이터를 저장합니다." )]
        public double? Save_Current;

        [Parameter( "△Voltage", ParameterValueType.Double, "050A00" )]
        [Unit( "V" )]
        [Help( "Voltage(V)가 지정된 값만큼 변화할 때마다 데이터를 저장합니다." )]
        public double? Save_Voltage;

        [Parameter( "△Temperature", ParameterValueType.Float, "050B00" )]
        [Unit( "℃" )]
        [Help( "온도가 지정된 값만큼 변화할 때마다 데이터를 저장합니다." )]
        public float? Save_Temperature;
        #endregion
    }
}
