using McQLib.Core;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms.Design;
using ZedGraph;

namespace DataViewer.Class
{
    public class GraphSetting : BaseSetting
    {
        private static Color[] defaultColors = new Color[]
        {
            Color.Blue,
            Color.DarkViolet,
            Color.Magenta,
            Color.DodgerBlue,
            Color.Pink,
            Color.LimeGreen,
            Color.Red,
            Color.Purple,
            Color.Navy,
            Color.Black,
            Color.Orange,
            Color.RosyBrown,
            Color.Gold,
            Color.Yellow,
            Color.DeepSkyBlue,
            Color.SpringGreen,
            Color.Crimson,
            Color.Peru,
            Color.PaleGoldenrod,
            Color.Aquamarine,
            Color.Salmon,
            Color.Brown,
            Color.Cyan,
        };
        private static int colorIndex = 0;
        private static Color pickColor()
        {
            if (colorIndex == int.MaxValue) colorIndex = 0;
            return defaultColors[colorIndex++ % defaultColors.Length];
        }

        [Category( "\tGraph Setting" )]
        [DisplayName( "Title" )]
        [Description( "그래프의 제목입니다." )]
        [ID( "G01" )]
        public string Title { get; set; } = string.Empty;

        //[Category( "\tGraph Setting" )]
        //[ID( "G02" )]
        //public bool ShowLegends { get; set; } = false;

        //[Category( "\tGraph Setting" )]
        //[ID( "G03" )]
        //public bool MajorGrid { get; set; } = true;

        //[Category( "\tGraph Setting" )]
        //[DisplayName( "Curve Creating Type" )]
        //[Description( "커브를 생성하는 규칙입니다.\r\n" +
        //             "Columns : 동일한 Data Column들은 동일한 커브에 추가됩니다.\r\n" +
        //             "Recipes : 동일한 Data Column들은 동일한 커브에 추가되지만, 각 레시피 단위로는 커브가 구분됩니다.\r\n" +
        //             "Loops : 동일한 Data Column들은 동일한 커브에 추가되지만, 루프 단위로 커브가 구분됩니다.\r\n" +
        //             "Files : 모든 파일에서 동일한 Data Column들은 하나의 커브에 추가됩니다." )]
        //[ID( "G04" )]
        //public CurveCreatingType CurveCreatingType { get; set; } = CurveCreatingType.Columns;

        //[Category( "\tGraph Setting" )]
        //[DisplayName( "Clear" )]
        //[Description( "그래프를 그릴 때 기존에 그래프에 표시된 커브를 지울지의 여부입니다." )]
        //[ID( "G05" )]
        //public bool Clear { get; set; } = true;

        //[Category( "\tGraph Setting" )]
        //[DisplayName( "Random Color" )]
        //[Description( "그래프를 그릴 때 생성되는 각 커브의 색상을 무작위로 설정합니다." )]
        //[ID( "G06" )]
        //public bool RandomColor { get; set; } = false;

        //[Category( "\tGraph Setting" )]
        //[DisplayName( "Remove Empty" )]
        //[Description( "그래프 트리에서 데이터가 없는 레시피 노드를 제거할지의 여부입니다." )]
        //[ID( "G07" )]
        //public bool RemoveEmpty { get; set; } = true;


        [Category("\tGraph Setting")]
        [DisplayName("Symbol")]
        [Description("그래프 데이터 표시 심볼 설정.")]
        [ID("G02")]
        public SymbolType Symbol
        {
            get => YAxis.SymbolType; // 현재 심볼 타입을 반환합니다.
            set => YAxis.SymbolType = value; // 입력된 심볼 타입으로 설정합니다.
        }

        [Category("\tGraph Setting")]
        [DisplayName("Symbol Size")]
        [Description("그래프 데이터 표시 심볼 Size 설정.")]
        [ID("G03")]
        public int SymbolSize { get; set; } = 0;
        

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////
        /// </summary>
        
       


        public AxisSetting XAxis = new AxisSetting();
        [Category( "\tX Axis" )]
        [DisplayName( "Min Value" )]
        [Description( "그래프 축의 Min Scale입니다." )]
        [ID( "X01" )]
        public double XMin { get => XAxis.Min; set => XAxis.Min = value; }

        [Category( "\tX Axis" )]
        [DisplayName( "Max Value" )]
        [Description( "그래프 축의 Max Scale입니다." )]
        [ID( "X02" )]
        public double XMax { get => XAxis.Max; set => XAxis.Max = value; }

        [Category( "\tX Axis")]
        [DisplayName( "Min Auto" )]
        [Description( "그래프 축의 Min Scale을 자동으로 설정할지의 여부입니다." )]
        [ID( "X03" )]
        public bool XMinAuto { get => XAxis.MinAuto; set => XAxis.MinAuto = value; }

        [Category( "\tX Axis")]
        [DisplayName( "Max Auto" )]
        [Description( "그래프 축의 Max Scale을 자동으로 설정할지의 여부입니다." )]
        [ID( "X04" )]
        public bool XMaxAuto { get => XAxis.MaxAuto; set => XAxis.MaxAuto = value; }

        [Category( "\tX Axis" )]
        [DisplayName( "Log Scale" )]
        [Description( "그래프 축을 Log Scale로 표시할지의 여부입니다." )]
        [ID( "X05" )]
        public bool XLogScale { get => XAxis.LogScale; set => XAxis.LogScale = value; }

        [Category("\tX Axis")]
        [DisplayName("X Data Column")]
        [Description("그래프를 구성할 데이터입니다.")]
        public AxisSetting.DataTypeInfo XDataType { get => XAxis.Data; set => XAxis.Data = value; }

        [ID("X06")]
        private DataType _xData
        {
            get => XDataType.SelectedData;
            set => XDataType.SelectedData = value;
        }


        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////
        /// </summary>

        //[Category("Types of Y Axis")]
        //[DisplayName("Edit Y Axis")]
        //[Editor(typeof(AxisSettingEditor), typeof(UITypeEditor))]
        //public List<AxisSetting> YAxisList { get; } = new List<AxisSetting>();

        //[Category("Types of Y Axis")]
        //[DisplayName("Edit Y2 Axis")]
        //[Editor(typeof(AxisSettingEditor), typeof(UITypeEditor))]
        //public List<AxisSetting> Y2AxisList { get; } = new List<AxisSetting>();



        public AxisSetting YAxis = new AxisSetting();
        [Category("\tY")]
        [DisplayName("Min Value")]
        [Description("그래프 축의 Min Scale입니다.")]
        [ID("Y01")]
        public double YMin { get => YAxis.Min; set => YAxis.Min = value; }

        [Category("\tY")]
        [DisplayName("Max Value")]
        [Description("그래프 축의 Max Scale입니다.")]
        [ID("Y02")]
        public double YMax { get => YAxis.Max; set => YAxis.Max = value; }

        [Category("\tY")]
        [DisplayName("Min Auto")]
        [Description("그래프 축의 Min Scale을 자동으로 설정할지의 여부입니다.")]
        [ID("Y03")]
        public bool YMinAuto { get => YAxis.MinAuto; set => YAxis.MinAuto = value; }

        [Category("\tY")]
        [DisplayName("Max Auto")]
        [Description("그래프 축의 Max Scale을 자동으로 설정할지의 여부입니다.")]
        [ID("Y04")]
        public bool YMaxAuto { get => YAxis.MaxAuto; set => YAxis.MaxAuto = value; }

        [Category("\tY")]
        [DisplayName("Log Scale")]
        [Description("그래프 축을 Log Scale로 표시할지의 여부입니다.")]
        [ID("Y05")]
        public bool YLogScale { get => YAxis.LogScale; set => YAxis.LogScale = value; }



        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// 
        [Category("\tY Axis")]
        [DisplayName("Y Data Column")]
        [Description("그래프를 구성할 데이터입니다.")]
        public AxisSetting.DataTypeInfo YDataType { get => YAxis.Data; set => YAxis.Data = value; }

        [ID("Z01")]
        private DataType _yData
        {
            get => YDataType.SelectedData;
            set => YDataType.SelectedData = value;
        }

        [Category("\tY Axis")]
        [DisplayName("Y Axis Color")]
        [Description("그래프 축을 색상 설정.")]
        [ID("Z02")]
        public Color YGraphColor { get; set; } = pickColor();
        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////
        /// </summary>


        public AxisSetting Y2Axis = new AxisSetting();
   
        [Category("\tY2 Axis")]
        [DisplayName("Y2 Data Column")]
        [Description("그래프를 구성할 데이터입니다.")]
        public AxisSetting.DataTypeInfo Y2DataType { get => Y2Axis.Data; set => Y2Axis.Data = value; }

        [ID("V01")]
        private DataType _y2Data
        {
            get => Y2DataType.SelectedData;
            set => Y2DataType.SelectedData = value;
        }


        [Category("\tY2 Axis")]
        [DisplayName("Y2 Axis Color")]
        [Description("그래프 축을 색상 설정.")]
        [ID("V02")]
        public Color Y2GraphColor { get; set; } = pickColor();
        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////
        /// </summary>


        [ID( "D01" )]
        private DataType[] _dataTypes
        {
            get => AxisSetting.DataTypeInfo.ColumnList;
            set => AxisSetting.DataTypeInfo.ColumnList = value;
        }

        

        //[Category( "Types of Y Axis" )]
        //[DisplayName( "Edit Y Axis" )]
        //[Editor(typeof(AxisSettingEditor), typeof(UITypeEditor))]
        //public List<AxisSetting> YAxisList { get; } = new List<AxisSetting>();

        //[Category( "Types of Y Axis" )]
        //[DisplayName( "Edit Y2 Axis" )]
        //[Editor( typeof( AxisSettingEditor ), typeof( UITypeEditor ) )]
        //public List<AxisSetting> Y2AxisList { get; } = new List<AxisSetting>();

        

        [Category( "Units" )]
        [DisplayName( "Voltage" )]
        [TypeConverter( typeof( UnitInfoConverter ) )]
        public UnitInfoWrapper VoltageUnit { get; set; } = new UnitInfoWrapper( UnitType.Voltage );
        [Category( "Units" )]
        [DisplayName( "Current" )]
        [TypeConverter( typeof( UnitInfoConverter ) )]
        public UnitInfoWrapper CurrentUnit { get; set; } = new UnitInfoWrapper( UnitType.Current );
        [Category( "Units" )]
        [DisplayName( "Power" )]
        [TypeConverter( typeof( UnitInfoConverter ) )]
        public UnitInfoWrapper PowerUnit { get; set; } = new UnitInfoWrapper( UnitType.Power );
        //[Category( "Units" )]
        //[DisplayName( "Resistance" )]
        //[TypeConverter( typeof( UnitInfoConverter ) )]
        //public UnitInfoWrapper ResistanceUnit { get; set; } = new UnitInfoWrapper( UnitType.Resistance );
        [Category( "Units" )]
        [DisplayName( "Time" )]
        [TypeConverter( typeof( UnitInfoConverter ) )]
        public UnitInfoWrapper TimeUnit { get; set; } = new UnitInfoWrapper( UnitType.Time );

        [ID( "S00" )]
        [Browsable( false )]
        public int LeftSplitterLocation { get; set; } = 300;
        [ID( "S01" )]
        [Browsable( false )]
        public int RightSplitterLocation { get; set; } = 230;
        [ID( "S02" )]
        [Browsable( false )]
        public int MainFormWidth { get; set; } = 1250;
        [ID( "S03" )]
        [Browsable( false )]
        public int MainFormHeight { get; set; } = 860;

        #region UnitInfoWrapper 저장용
        [ID( "U01" )]
        private string _voltageUnit
        {
            get => VoltageUnit.ToSaveString();
            set => VoltageUnit.FromSaveString( value );
        }
        [ID( "U02" )]
        private string _currentUnit
        {
            get => CurrentUnit.ToSaveString();
            set => CurrentUnit.FromSaveString( value );
        }
        [ID( "U03" )]
        private string _powerUnit
        {
            get => PowerUnit.ToSaveString();
            set => PowerUnit.FromSaveString( value );
        }
        //[ID( "U04" )]
        //private string _resistanceUnit
        //{
        //    get => ResistanceUnit.ToSaveString();
        //    set => ResistanceUnit.FromSaveString( value );
        //}
        [ID( "U04" )]
        private string _timeUnit
        {
            get => TimeUnit.ToSaveString();
            set => TimeUnit.FromSaveString( value );
        }
        #endregion

        public new void Save(string filename)
        {
            var yAxisSettings = string.Empty;

            //for (var i = 0; i < YAxisList.Count; i++)
            //{
            //    YAxisList[i].Save("axissetting.tmp");

            //    using (var sr = new StreamReader("axissetting.tmp"))
            //    {
            //        yAxisSettings += $"YAxis:";
            //        yAxisSettings += sr.ReadToEnd().Replace(":", "=").Replace("\r\n", "?") + "\r\n";
            //    }

            //}

            //for (var i = 0; i < Y2AxisList.Count; i++)
            //{
            //    Y2AxisList[i].Save("axissetting.tmp");

            //    using (var sr = new StreamReader("axissetting.tmp"))
            //    {
            //        yAxisSettings += $"Y2Axis:";
            //        yAxisSettings += sr.ReadToEnd().Replace(":", "=").Replace("\r\n", "?") + "\r\n";
            //    }
            //}

            File.Delete("axissetting.tmp");

            base.Save(filename);

            using (var sw = new StreamWriter(filename, true))
            {
                sw.WriteLine(yAxisSettings);
            }
        }
        public new void Load(string filename)
        {
            base.Load(filename);

            //YAxisList.Clear();
            //Y2AxisList.Clear();

            using (var sr = new StreamReader(filename))
            {
                //var lines = sr.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                //for (var i = 0; i < lines.Length; i++)
                //{
                //    if (lines[i].IndexOf("YAxis") != -1)
                //    {
                //        using (var sw = new StreamWriter("axissetting.tmp"))
                //        {
                //            sw.WriteLine(lines[i].Replace("YAxis:", "").Replace("=", ":").Replace("?", "\r\n"));
                //        }

                //        var axis = new AxisSetting();
                //        axis.Load("axissetting.tmp");
                //        YAxisList.Add(axis);
                //    }
                //    else if (lines[i].IndexOf("Y2Axis") != -1)
                //    {
                //        using (var sw = new StreamWriter("axissetting.tmp"))
                //        {
                //            sw.WriteLine(lines[i].Replace("Y2Axis:", "").Replace("=", ":").Replace("?", "\r\n"));
                //        }

                //        var axis = new AxisSetting();
                //        axis.Load("axissetting.tmp");
                //        Y2AxisList.Add(axis);
                //    }
                //    else if (lines[i].IndexOf("X05") != -1)
                //    {
                //        if ((lines[i - 1].IndexOf("D01") != -1))
                //        {
                //            var data = (lines[i].Substring(lines[i].IndexOf(":") + 1));
                //            _xData = GetDataTypeFromString(data);

                //        }
                //    }
                //}

                File.Delete("axissetting.tmp");
            }
        }
        Dictionary<string, DataType> dataTypeMap = new Dictionary<string, DataType>
        {
            {"None", Class.DataType.None},
            {"TotalTime", Class.DataType.TotalTime},
            {"Cycle", Class.DataType.Cycle},
            {"ChargeCapacity", Class.DataType.ChargeCapacity},
            {"DisChargeCapacity", Class.DataType.DisChargeCapacity},
            {"StepTime", Class.DataType.StepTime},
            {"Voltage", Class.DataType.Voltage},
            {"Current", Class.DataType.Current},
            {"Capacity", Class.DataType.Capacity},
            {"Power", Class.DataType.Power},
            {"WattHour", Class.DataType.WattHour},
            {"Temperature", Class.DataType.Temperature},
            {"Frequency", Class.DataType.Frequency},
            {"Z", Class.DataType.Z},
            {"Z_Real", Class.DataType.Z_Real},
            {"Z_Img", Class.DataType.Z_Img},
            {"DeltaV", Class.DataType.DeltaV},
            {"DeltaI", Class.DataType.DeltaI},
            {"DeltaT", Class.DataType.DeltaT},
            {"StartOcv", Class.DataType.StartOcv},
            {"EndOcv", Class.DataType.EndOcv},
            {"Phase", Class.DataType.Phase},
            {"V1", Class.DataType.V1},
            {"I1", Class.DataType.I1},
            {"V2", Class.DataType.V2},
            {"I2", Class.DataType.I2},
            {"R", Class.DataType.R},
        };

        public DataType GetDataTypeFromString(string dataTypeString)
        {
            if (dataTypeMap.ContainsKey(dataTypeString))
            {
                return dataTypeMap[dataTypeString];
            }
            else
            {
                return Class.DataType.None;
            }
        }

        public string GetUnitText( DataType dataType )
        {
            switch ( dataType )
            {
                case Class.DataType.TotalTime:
                    return TimeUnit.ToString();

                case Class.DataType.StepTime:
                    return TimeUnit.ToString();

                case Class.DataType.Voltage:
                    return VoltageUnit.ToString();

                case Class.DataType.Current:
                    return CurrentUnit.ToString();

                case Class.DataType.Temperature:
                    return "℃";

                case Class.DataType.Frequency:
                    return "Hz";

                case Class.DataType.Capacity:
                    return "Ah";

                case Class.DataType.Power:
                    return PowerUnit.ToString();

                //case Class.DataType.R:
                //    return ResistanceUnit.ToString();

                case Class.DataType.WattHour:
                    return "Wh";

                case Class.DataType.DeltaV:
                    return VoltageUnit.ToString();

                case Class.DataType.DeltaI:
                    return CurrentUnit.ToString();

                case Class.DataType.DeltaT:
                    return "℃";

                case Class.DataType.V1:
                    return VoltageUnit.ToString();

                case Class.DataType.V2:
                    return VoltageUnit.ToString();

                case Class.DataType.I1:
                    return CurrentUnit.ToString();

                case Class.DataType.I2:
                    return CurrentUnit.ToString();

                case Class.DataType.StartOcv:
                    return VoltageUnit.ToString();

                case Class.DataType.EndOcv:
                    return VoltageUnit.ToString();

                case Class.DataType.Phase:
                case Class.DataType.Z:
                case Class.DataType.Z_Real:
                case Class.DataType.Z_Img:
                default:
                    return string.Empty;
            }
        }

        public struct UnitInfoWrapper
        {
            public UnitInfo UnitInfo;
            public SiUnits SiUnit
            {
                get => UnitInfo.SiUnit;
                set => UnitInfo.SiUnit = value;
            }
            public TimeUnit TimeUnit
            {
                get => UnitInfo.TimeUnit;
                set => UnitInfo.TimeUnit = value;
            }

            public override string ToString()
            {
                return UnitInfo.UnitString;
            }

            public UnitInfoWrapper( UnitType unitType )
            {
                UnitInfo = new UnitInfo( unitType );
            }

            public string ToSaveString()
            {
                if ( UnitInfo.UnitType == UnitType.Time )
                {
                    return TimeUnit.ToString();
                }
                else
                {
                    return SiUnit.ToString();
                }
            }
            public void FromSaveString( string value )
            {
                if ( UnitInfo.UnitType == UnitType.Time )
                {
                    TimeUnit tmp;
                    if ( Enum.TryParse( value, out tmp ) )
                    {
                        TimeUnit = tmp;
                    }
                    else
                    {
                        TimeUnit = TimeUnit.Second;
                    }
                }
                else
                {
                    SiUnits tmp;
                    if ( Enum.TryParse( value, out tmp ) )
                    {
                        SiUnit = tmp;
                    }
                    else
                    {
                        SiUnit = SiUnits.Default;
                    }
                }
            }
        }
        public class UnitInfoConverter : TypeConverter
        {
            public override bool GetPropertiesSupported( ITypeDescriptorContext context )
            {
                return true;
            }

            public override PropertyDescriptorCollection GetProperties( ITypeDescriptorContext context, object value, Attribute[] attributes )
            {
                var props = TypeDescriptor.GetProperties( typeof( UnitInfoWrapper ) );
                var result = new PropertyDescriptorCollection( new PropertyDescriptor[]
                {
                    ((UnitInfoWrapper)value).UnitInfo.UnitType == UnitType.Time ? props.Find("TimeUnit", false) :
                    props.Find( "SiUnit", false )
                } );

                return result;
            }
        }

        public class AxisSettingEditor : UITypeEditor
        {
            private IWindowsFormsEditorService _editorService;
            public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
            {
                return UITypeEditorEditStyle.Modal;
            }
            private void OnListBoxSelectedValueChanged( object sender, EventArgs e )
            {
                _editorService.CloseDropDown();
            }
            public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
            {
                _editorService = ( IWindowsFormsEditorService )provider.GetService( typeof( IWindowsFormsEditorService ) );

                var list_bak = value as List<AxisSetting>;
                var list = new List<AxisSetting>();
                for(var i = 0; i < list_bak.Count; i++ )
                {
                    list.Add( list_bak[i].Clone() as AxisSetting );
                }

                using ( var dlg = new Form_AxisSettingEditor( list ) )
                {
                    if (_editorService.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK )
                    {
                        list_bak.Clear();
                        list_bak.AddRange( list );
                    }

                    return list_bak;
                }
            }
        }
    }
}
