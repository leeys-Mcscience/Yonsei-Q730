using McQLib.Core;
using McQLib.IO;
using McQLib.Recipes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataViewer.Class
{
    /// <summary>
    /// 하나의 Qrd 파일에 대한 데이터 집합입니다.
    /// <br>0.3 버전 이상의 QDataStream으로 저장된 qrd 파일을 불러오는 경우에만 정상 작동을 보장합니다.</br>
    /// </summary>
    public class QDataManager
    {
        public int Count => _recipeDatas.Count;
        public RecipeData this[int index] => _recipeDatas[index];

        public string FileName => _filename;
        public List<RecipeData> RecipeDatas => _recipeDatas;
        public Sequence Sequence => _sequence;
        public TreeNode Root => _root;

        private readonly QrdReadMode _readMode = QrdReadMode.Detail;
        private string _filename = string.Empty;
        private Sequence _sequence = new Sequence();

        public byte MajorVersion => _major;
        public byte MinorVersion => _minor;

        private byte _major;
        private byte _minor;

        // MeasureData의 StepCount와 1대 1 대응하는 순서로 생성된 RecipeData 리스트
        private List<RecipeData> _recipeDatas = new List<RecipeData>();
        // Sequence를 트리화 한 트리의 루트 노드
        private TreeNode _root;

        private QDataManager() { }

        public static QDataManager FromFileAsync( string filename, CancellationTokenSource cancel )
        {
            var mgr = new QDataManager();
            mgr._filename = filename;

            QDataReader reader;
            MeasureData[] datas;

            try
            {
                reader = new QDataReader( filename );
                mgr._major = reader.Major;
                mgr._minor = reader.Minor;
                datas = reader.ReadToEndAsync( cancel );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"{ex.Message} ({ex.StackTrace})" );

                return null;
            }

            if ( cancel.IsCancellationRequested ) return null;

            mgr._root = new TreeNode( Path.GetFileName( filename ).Replace( Path.GetExtension( filename ), "" ) );


            var result = mgr._recipeDatas;
            var seq = mgr._sequence = reader.Sequence;
            var current = mgr._root;
            TreeNode dummy;

            if ( seq == null || seq.Count == 0 )
            {
                result.Add( new RecipeData()
                {
                    RecipeType = RecipeType.Unknown
                } );

                current = new TreeNode( RecipeType.Unknown.ToString() );
                current.Tag = result[result.Count - 1];
                mgr._root.Nodes.Add( current );

                uint lastStepCount = 0;
                ulong lastTotalTime = 0;
                ulong lastStepTime = 0;

                foreach ( MeasureData data in datas )
                {
                    if ( data.StepCount != lastStepCount )
                    {
                        lastStepCount = data.StepCount;
                        lastStepTime = 0;
                    }

                    lastStepTime = data.StepTime = lastStepTime + ( data.TotalTime - lastTotalTime );
                    lastTotalTime = data.TotalTime;

                    result[0].Datas.Add( new QDataPoint( data, data.RecipeType ) );
                }
            }
            else
            {
                var index = 0;
                int loopIndex = 1;
                var cycleNo = -1;
                var labelNo = -1;
                var loopCount = -1;
                var jumpCount = -1;


                if ( seq[seq.Count - 1].GetRecipeType() != RecipeType.End )
                {
                    seq.Add( RecipeFactory.CreateInstance( RecipeType.End ) );
                }

                while ( true )
                {
                    var type = seq[index].GetRecipeType();
                    var recipeData = new RecipeData() { RecipeType = type };
                    result.Add( recipeData );

                    if ( type == RecipeType.End ) break;

                    switch ( type )
                    {
                        case RecipeType.Cycle:
                            cycleNo = index++;

                            if ( current == mgr._root )
                            {
                                dummy = new TreeNode( type.ToString() );
                                dummy.ImageKey = dummy.SelectedImageKey = type.ToString();
                                current.Nodes.Add( dummy );
                                current = dummy;
                            }

                            
                            dummy = new TreeNode( $"{loopIndex++} : Loop" );
                            dummy.ImageKey = dummy.SelectedImageKey = "Loop";
                            current.Nodes.Add( dummy );
                            current = dummy;
                            break;

                        case RecipeType.Loop:
                            current = current.Parent;

                            if ( loopCount == -1 )
                            {
                                loopCount = ( int )( seq[index] as Loop ).LoopCount - 1;
                            }
                            else
                            {
                                loopCount--;
                            }

                            if ( loopCount <= 0 )
                            {
                                loopCount = -1;
                                cycleNo = -1;

                                current = current.Parent;
                                index++;
                            }
                            else
                            {
                                index = cycleNo;
                            }
                            break;

                        case RecipeType.Jump:
                            if ( jumpCount == -1 )
                            {
                                jumpCount = ( int )( seq[index] as Jump ).JumpCount - 1;
                                // Qrd 파일로부터 읽은 시퀀스는 LabelName에 점프할 인덱스가 저장되어 있다.
                                labelNo = int.Parse( ( seq[index] as Jump ).LabelName );
                            }
                            else
                            {
                                jumpCount--;
                            }

                            if ( jumpCount <= 0 )
                            {
                                jumpCount = -1;
                                labelNo = -1;
                                index++;
                            }
                            else
                            {
                                index = labelNo;
                            }
                            break;

                        default:
                            dummy = new TreeNode( type.ToString() );
                            dummy.ImageKey = dummy.SelectedImageKey = type.ToString();
                            dummy.Tag = recipeData;
                            recipeData.Node = dummy;
                            current.Nodes.Add( dummy );
                            index++;
                            break;
                    }
                }

                var c = seq[0].GetRecipeType();
                uint lastStepCount = 0;
                ulong lastTotalTime = 0;
                ulong lastStepTime = 0;

                foreach ( MeasureData data in datas )
                {
                    if ( data.StepCount != lastStepCount )
                    {
                        c = seq[data.StepNumber].GetRecipeType();
                        lastStepCount = data.StepCount;
                        lastStepTime = 0;
                    }

                    lastStepTime = data.StepTime = lastStepTime + ( data.TotalTime - lastTotalTime );
                    lastTotalTime = data.TotalTime;

                    result[( int )data.StepCount].Datas.Add( new QDataPoint( data, c ) );
                }
            }

            //if ( SoftwareConfiguration.GraphSetting.RemoveEmpty )
            //{
            //    removeEmpty( mgr.Root );
            //}

            return mgr;
        }

        private static bool removeEmpty( TreeNode node )
        {
            if ( node == null )
            {
            }
            else if ( node.Nodes.Count != 0 )
            {
                var tmp = new List<TreeNode>();
                foreach ( TreeNode n in node.Nodes )
                {
                    if ( removeEmpty( n ) ) tmp.Add( n );
                }

                for ( var i = 0; i < tmp.Count; i++ ) tmp[i].Remove();
                if ( node.Nodes.Count == 0 ) return true;
            }
            else
            {
                if ( ( node.Tag as RecipeData ).Count == 0 )
                {
                    return true;
                }
            }

            return false;
        }
    }


    /// <summary>
    /// 하나의 측정 포인트에 대한 데이터 집합입니다.
    /// </summary>
    public class QDataPoint
    {
        public ulong TotalTime;
        public ulong StepTime;

        //                          DC               Rest            FRA             TRA             ACR             DCR            OCV
        private double _field1;   // Voltage         Voltage         Frequency       Voltage         Frequency       V-1            Voltage
        private double _field2;   // Current                         Z_Real          Current         Z_Real          I-1
        private double _field3;   // Power                           Z_Image                         Z_Image         V-2
        private double _field4;   // Capacity(Ah)                    Phase                           Phase           I-2
        private double _field5;   // Capacity(Wh)                    Start_Ocv                       Start_Ocv       R
        private double _field6;   // Temperature                     End_Ocv         Temperature     End_Ocv
        private double _field7;   //                                                                 Z

        public double Voltage => _field1;
        public double Current => _field2;
        public double Temperature => _field6;

        public double Capacity => _field4;
        public double Power => _field3;
        public double WattHour => _field5;

        public double Frequency => _field1;
        public double Phase => _field4;
        public double R => _field5;
        public double Z => _field7;
        public double Z_Real => _field2;
        public double Z_Img => _field3;

        public double DeltaV => 0;
        public double DeltaI => 0;
        public double DeltaT => 0;

        public double V1 => _field1;
        public double V2 => _field3;
        public double I1 => _field2;
        public double I2 => _field4;

        public double StartOcv => _field5;
        public double EndOcv => _field6;

        public QDataPoint( MeasureData data, RecipeType recipeType )
        {
            TotalTime = data.TotalTime;
            StepTime = data.StepTime;

            switch ( recipeType )
            {
                case RecipeType.Charge:
                case RecipeType.Discharge:
                case RecipeType.AnodeCharge:
                case RecipeType.AnodeDischarge:
                    _field1 = data.Voltage;
                    _field2 = data.Current;
                    _field3 = data.Power;
                    _field4 = data.Capacity;
                    _field5 = data.WattHour;
                    _field6 = data.Temperature;
                    break;

                case RecipeType.Rest:
                case RecipeType.OpenCircuitVoltage:
                    _field1 = data.Voltage;
                    break;
            }
        }
    }
    /// <summary>
    /// 하나의 레시피에 대한 데이터 집합입니다.
    /// </summary>
    public class RecipeData
    {
        public int Count => Datas.Count;
        public QDataPoint this[int index] => Datas[index];
        public TreeNode Node = new TreeNode();

        public RecipeType RecipeType;

        public readonly List<QDataPoint> Datas = new List<QDataPoint>();
        public override string ToString()
        {
            return $"{RecipeType} (Count={Datas.Count})";
        }

        public double[] GetData( DataType dataType, int skip = 0 )
        {
            skip += 1;
            // 데이터 개수 / (1 + Skip할 개수)를 올림 한 값이 최종 개수
            var result = new double[( int )( ( Datas.Count / skip ) + 0.5 )];

            switch ( dataType )
            {
                case DataType.TotalTime:
                    Parallel.For( 0, result.Length, i =>
                    {
                        result[i] = SoftwareConfiguration.GraphSetting.TimeUnit.UnitInfo.ChangeValue( Datas[skip * i].TotalTime );
                    } );
                    //Parallel.For(0, result.Length, i =>
                    //{
                    //    result[i] = SoftwareConfiguration.GraphSetting.TimeUnit.UnitInfo.ChangeValue( Datas[skip * i].TotalTime );
                    //}
                    break;

                case DataType.StepTime:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = SoftwareConfiguration.GraphSetting.TimeUnit.UnitInfo.ChangeValue( Datas[skip * i].StepTime );
                    });
                    break;

                case DataType.Voltage:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = SoftwareConfiguration.GraphSetting.VoltageUnit.UnitInfo.ChangeValue( Datas[skip * i].Voltage );
                    });
                    break;

                case DataType.Current:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = SoftwareConfiguration.GraphSetting.CurrentUnit.UnitInfo.ChangeValue( Datas[skip * i].Current );
                    });
                    break;

                case DataType.Temperature:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].Temperature;
                    });
                    break;

                case DataType.Frequency:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].Frequency;
                    });
                    break;

                case DataType.Capacity:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].Capacity;
                    });
                    break;


                case DataType.Power:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = SoftwareConfiguration.GraphSetting.PowerUnit.UnitInfo.ChangeValue( Datas[skip * i].Power );
                    });
                    break;

                case DataType.Phase:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].Phase;
                    });
                    break;

                //case DataType.R:
                //    Parallel.For(0, result.Length, i =>
                //    {
                //        result[i] = SoftwareConfiguration.GraphSetting.ResistanceUnit.UnitInfo.ChangeValue( Datas[skip * i].R );
                //    });
                //    break;

                case DataType.WattHour:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = SoftwareConfiguration.GraphSetting.PowerUnit.UnitInfo.ChangeValue( Datas[skip * i].WattHour );
                    });
                    break;

                case DataType.Z:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].Z;
                    });
                    break;

                case DataType.Z_Real:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].Z_Real;
                    });
                    break;

                case DataType.Z_Img:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].Z_Img;
                    });
                    break;

                case DataType.DeltaV:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].DeltaV;
                    });
                    break;

                case DataType.DeltaI:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].DeltaI;
                    });
                    break;

                case DataType.DeltaT:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].DeltaT;
                    });
                    break;

                case DataType.V1:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].V1;
                    });
                    break;

                case DataType.V2:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].V2;
                    });
                    break;

                case DataType.I1:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].I1;
                    });
                    break;

                case DataType.I2:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].I2;
                    });
                    break;

                case DataType.StartOcv:
                    Parallel.For(0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].StartOcv;
                    });
                    break;

                case DataType.EndOcv:
                    Parallel.For( 0, result.Length, i =>
                    {
                        result[i] = Datas[skip * i].EndOcv;
                    } );
                    break;
            }

            return result;
        }
    }
}
