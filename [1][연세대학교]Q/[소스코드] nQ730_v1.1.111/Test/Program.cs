using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McQLib.Core;
using McQLib.Recipes;
using JmCmdLib;
using System.Reflection;
using McQLib.IO;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;

namespace Test
{
    class Program
    {
        [Flags]
        enum TestEnum
        {
            E1 = 0b00000001,
            E2 = 0b00000010,
            E3 = 0b00000100,
            E4 = 0b00001000,
            E5 = 0b00010000
        }

        static void dataCreating(string filename)
        {
            var seq = new Sequence();

            seq.Add( RecipeFactory.CreateInstance( RecipeType.Discharge ) );    // 0
            seq.Add( RecipeFactory.CreateInstance( RecipeType.Rest ) );         // 1
            seq.Add( RecipeFactory.CreateInstance( RecipeType.Cycle ) );        // 2    8   14  20  25

            seq.Add( RecipeFactory.CreateInstance( RecipeType.Charge ) );       // 3    9   15  21  26
            seq.Add( RecipeFactory.CreateInstance( RecipeType.Rest ) );         // 4    10  16  22  27
            seq.Add( RecipeFactory.CreateInstance( RecipeType.Discharge ) );    // 5    11  17  23  28
            seq.Add( RecipeFactory.CreateInstance( RecipeType.Rest ) );         // 6    12  18  24  29

            seq.Add( RecipeFactory.CreateInstance( RecipeType.Loop ) );         // 7    13  19  24  30
            seq.Add( RecipeFactory.CreateInstance( RecipeType.End ) );          // 

            ( seq[7] as Loop ).LoopCount = 5;

            uint totalTime = 0;
            //uint stepCount = 0;
            //ushort stepNo = 0;

            var dataWriter = QDataWriter.Create( filename, seq );

            addData( dataWriter, RecipeType.Discharge, ref totalTime, 1000, 0, 0, 10000 );
            addData( dataWriter, RecipeType.Rest, ref totalTime, 1000, 1, 1, 10000 );

            addData( dataWriter, RecipeType.Charge, ref totalTime, 1000, 3, 3, 10000 );
            addData( dataWriter, RecipeType.Rest, ref totalTime, 1000, 4, 4, 10000 );
            addData( dataWriter, RecipeType.Discharge, ref totalTime, 1000, 5, 5, 10000 );
            addData( dataWriter, RecipeType.Rest, ref totalTime, 1000, 6, 6, 10000 );

            addData( dataWriter, RecipeType.Charge, ref totalTime, 1000, 9, 3, 10000 );
            addData( dataWriter, RecipeType.Rest, ref totalTime, 1000, 10, 4, 10000 );
            addData( dataWriter, RecipeType.Discharge, ref totalTime, 1000, 11, 5, 10000 );
            addData( dataWriter, RecipeType.Rest, ref totalTime, 1000, 12, 6, 10000 );

            addData( dataWriter, RecipeType.Charge, ref totalTime, 1000, 15, 3, 10000 );
            addData( dataWriter, RecipeType.Rest, ref totalTime, 1000, 16, 4, 10000 );
            addData( dataWriter, RecipeType.Discharge, ref totalTime, 1000, 17, 5, 10000 );
            addData( dataWriter, RecipeType.Rest, ref totalTime, 1000, 18, 6, 10000 );

            addData( dataWriter, RecipeType.Charge, ref totalTime, 1000, 21, 3, 10000 );
            addData( dataWriter, RecipeType.Rest, ref totalTime, 1000, 22, 4, 10000 );
            addData( dataWriter, RecipeType.Discharge, ref totalTime, 1000, 23, 5, 10000 );
            addData( dataWriter, RecipeType.Rest, ref totalTime, 1000, 24, 6, 10000 );

            addData( dataWriter, RecipeType.Charge, ref totalTime, 1000, 26, 3, 10000 );
            addData( dataWriter, RecipeType.Rest, ref totalTime, 1000, 27, 4, 10000 );
            addData( dataWriter, RecipeType.Discharge, ref totalTime, 1000, 28, 5, 10000 );
            addData( dataWriter, RecipeType.Rest, ref totalTime, 1000, 29, 6, 10000 );

            dataWriter.Close();
        }

        static Random rand = new Random( DateTime.Now.Second );
        static void addData( QDataWriter writer, RecipeType type, ref uint totalTimeStart, uint timeIncrease, uint stepCount, ushort stepNumber, int count )
        {
            for ( uint i = 0; i < count; i++ )
            {
                //writer.Write( new MeasureData(
                //    type,
                //    totalTimeStart,
                //    stepCount,
                //    stepNumber,
                //    0,              // cycle count
                //    0,              // mode1
                //    0,              // mode2
                //    i,              // data index
                //    rand.NextDouble() + 3,
                //    rand.NextDouble(),
                //    rand.NextDouble(),
                //    rand.NextDouble(),
                //    rand.NextDouble(),
                //    rand.NextDouble(),
                //    rand.NextDouble(), 
                //    0, 0

                //    ) );

                totalTimeStart += timeIncrease;
            }
        }

        public class TestClass
        {
            public int IntegerValue { get; set; }
            public double DoubleValue { get; set; }

            public TestClass(int i, double d )
            {
                IntegerValue = i;
                DoubleValue = d;
            }
        }
        public struct TestStruct
        {
            public int[] TestStructArr;
            public TestClass[] TestClassArr;
        }

        delegate void writeChain( string test );
        static writeChain chain;

        private static void testWrite(string str )
        {
            Console.WriteLine( $"{nameof( testWrite )}: {str}" );
        }
        private static void testWrite2(string str )
        {
            Console.WriteLine( $"{nameof( testWrite2 )}: {str}" );
        }

        static void Main( string[] args )
        {
            chain += testWrite;
            chain += testWrite2;

            chain?.Invoke( "test" );

            TestStruct t1 = new TestStruct()
            {
                TestStructArr = new int[] { 1, 2, 3, 4, 5 },
                TestClassArr = new TestClass[]
                {
                    new TestClass(1, 1.1),
                    new TestClass(2, 2.2),
                    new TestClass(3, 3.3),
                    new TestClass(4, 4.4),
                    new TestClass(5, 5.5)
                }
            };

            TestStruct t2 = t1;



            //TestEnum e = TestEnum.E1 | TestEnum.E2;
            //Console.WriteLine( e.ToString() );

            dataCreating( "test.qrd" );

            //var seq = new Sequence();

            //seq.Add( RecipeFactory.CreateInstance( RecipeType.Label ) );
            //seq.Add( RecipeFactory.CreateInstance( RecipeType.Cycle ) );
            //seq.Add( RecipeFactory.CreateInstance( RecipeType.Charge ) );
            //seq.Add( RecipeFactory.CreateInstance( RecipeType.Rest ) );
            //seq.Add( RecipeFactory.CreateInstance( RecipeType.Discharge ) );
            //seq.Add( RecipeFactory.CreateInstance( RecipeType.Rest ) );
            //seq.Add( RecipeFactory.CreateInstance( RecipeType.Loop ) );
            //seq.Add( RecipeFactory.CreateInstance( RecipeType.Jump ) );
            //seq.Add( RecipeFactory.CreateInstance( RecipeType.End ) );

            //( seq[6] as Loop ).LoopCount = 5;
            //( seq[7] as Jump ).JumpCount = 5;

            //var w = QDataWriter.Create( "testSeqData.qrd", seq );

            Console.ReadKey();
        }

    }
}
