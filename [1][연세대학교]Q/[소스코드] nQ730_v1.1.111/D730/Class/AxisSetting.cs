using McQLib.Core;
using McQLib.Recipes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DataViewer.Class
{
    public class AxisSetting : BaseSetting, ICloneable
    {
        #region Statics
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
            if ( colorIndex == int.MaxValue ) colorIndex = 0;
            return defaultColors[colorIndex++ % defaultColors.Length];
        }
        private static Random rand = new Random();
        public static Color RandomColor()
        {
            return defaultColors[rand.Next( 0, defaultColors.Length - 1 )];
        }
        #endregion

        #region Properties
        [DisplayName( "Min Value" )]
        [Description( "그래프 축의 Min Scale입니다." )]
        [Category( "Data" )]
        [ID( "A01" )]
        public double Min { get; set; }

        [DisplayName( "Max Value" )]
        [Description( "그래프 축의 Max Scale입니다." )]
        [Category( "Data" )]
        [ID( "A02" )]
        public double Max { get; set; }

        [DisplayName( "Min Auto" )]
        [Description( "그래프 축의 Min Scale을 자동으로 설정할지의 여부입니다." )]
        [Category( "Data" )]
        [ID( "A03" )]
        public bool MinAuto { get; set; } = true;

        [DisplayName( "Max Auto" )]
        [Description( "그래프 축의 Max Scale을 자동으로 설정할지의 여부입니다." )]
        [Category( "Data" )]
        [ID( "A04" )]
        public bool MaxAuto { get; set; } = true;

        [DisplayName( "Log Scale" )]
        [Description( "그래프 축을 Log Scale로 표시할지의 여부입니다." )]
        [Category( "Data" )]
        [ID( "A05" )]
        public bool LogScale { get; set; }

        [DisplayName( "Skip Points" )]
        [Description( "그래프에 포함된 데이터가 많아 느려지는 경우 그래프에 표시할 포인트의 개수를 지정된 개수씩 건너뛰며 표시하는 옵션입니다." )]
        [Category( "Data" )]
        [ID( "A0B" )]
        public int SkipPoints { get; set; }

        [DisplayName( "Thickness" )]
        [Description( "그래프 선분의 두께입니다." )]
        [Category("Data")]
        [ID( "A06" )]
        public float LineWidth { get; set; } = 1.0f;

        [DisplayName( "Color" )]
        [Description( "그래프 선분 색상입니다." )]
        [Category("Data")]
        [ID( "A07" )]
        public Color Color { get; set; } = pickColor();

        [DisplayName( "Zero Line" )]
        [Description( "그래프에 y 요소 0점 선을 표시할지의 여부입니다." )]
        [Category( "Display" )]
        [ID( "A08" )]
        public bool ZeroLine { get; set; }

        [DisplayName("Symbol")]
        [Description("그래프 선분의 각 점을 강조하는 표시 방법입니다.")]
        [Category("Display")]
        [ID("A09")]
        public ZedGraph.SymbolType SymbolType { get; set; } = ZedGraph.SymbolType.Circle;

        [DisplayName( "Data Column" )]
        [Description( "그래프를 구성할 데이터입니다." )]
        [Category( "Data" )]
        public DataTypeInfo Data { get; set; } = new DataTypeInfo();

        [ID( "A0A" )]
        private DataType _dataType
        {
            get => Data.SelectedData;
            set => Data.SelectedData = value;
        }
        #endregion

        public override string ToString()
        {
            return Data.ToString();
        }
        public object Clone()
        {
            return new AxisSetting()
            {
                Min = Min,
                Max = Max,
                MinAuto = MinAuto,
                MaxAuto = MaxAuto,
                LogScale = LogScale,
                SkipPoints = SkipPoints,
                LineWidth = LineWidth,
                Color = Color,
                ZeroLine = ZeroLine,
                SymbolType = SymbolType,
                Data = new DataTypeInfo(_dataType.ToString())
            };
        }

        [Editor( typeof( DataTypeInfoEditor ), typeof( UITypeEditor ) )]
        public class DataTypeInfo
        {
            public static DataType[] ColumnList = new DataType[] { DataType.None };

            public static void SetColumnList( params RecipeType[] types )
            {
                var columnList = new List<DataType>();

                for ( var i = 0; i < types.Length; i++ )
                {
                    switch ( types[i] )
                    {
                        case RecipeType.Charge:
                        case RecipeType.Discharge:
                        case RecipeType.AnodeCharge:
                        case RecipeType.AnodeDischarge:
                            columnList.AddRange( new DataType[] { DataType.Cycle,DataType.ChargeCapacity, DataType.DisChargeCapacity, DataType.TotalTime, DataType.StepTime, DataType.Voltage, DataType.Current, DataType.Capacity, DataType.WattHour});
                                //, DataType.Power, DataType.WattHour, DataType.Temperature } );
                            break;

                        case RecipeType.Rest:
                        case RecipeType.OpenCircuitVoltage:
                            columnList.AddRange( new DataType[] { DataType.TotalTime, DataType.StepTime, DataType.Voltage });
                            //, DataType.Temperature } );
                            break;

                        case RecipeType.Pattern:
                            columnList.AddRange( new DataType[] { DataType.TotalTime, DataType.StepTime, DataType.Voltage, DataType.Current, DataType.Temperature } );
                            break;

                        case RecipeType.Unknown:
                            columnList.AddRange( new DataType[] { DataType.Cycle, DataType.ChargeCapacity, DataType.DisChargeCapacity, DataType.TotalTime, DataType.StepTime, DataType.Voltage, DataType.Current, DataType.Capacity, DataType.Power, DataType.WattHour, DataType.Temperature } );
                            break;
                    }
                }
           
                columnList.Insert( 0, DataType.None );
                ColumnList = columnList.Distinct().ToArray();
            }

            public DataType SelectedData { get; set; } = DataType.None;
            public override string ToString()
            {
                return SelectedData.ToString();
            }
            public DataTypeInfo() { }
            public DataTypeInfo( string columnName )
            {
                for ( var i = 0; i < ColumnList.Length; i++ )
                {
                    if ( ColumnList[i].ToString() == columnName )
                    {
                        SelectedData = ColumnList[i];
                        return;
                    }
                }

                SelectedData = DataType.None;
            }

            public static implicit operator DataType( DataTypeInfo info )
            {
                return info.SelectedData;
            }

            public class DataTypeInfoEditor : UITypeEditor
            {
                private IWindowsFormsEditorService _editorService;
                public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
                {
                    return UITypeEditorEditStyle.DropDown;
                }
                private void OnListBoxSelectedValueChanged( object sender, EventArgs e )
                {
                    _editorService.CloseDropDown();
                }
                public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
                {
                    _editorService = ( IWindowsFormsEditorService )provider.GetService( typeof( IWindowsFormsEditorService ) );

                    var listBox = new ListBox();
                    listBox.SelectionMode = SelectionMode.One;
                    listBox.SelectedValueChanged += OnListBoxSelectedValueChanged;

                    listBox.Items.AddRange( ColumnList.Select( i => i.ToString() ).ToArray() );
                    for ( var i = 0; i < listBox.Items.Count; i++ )
                    {
                        if ( listBox.Items[i].ToString() == value.ToString() )
                        {
                            listBox.SelectedIndex = i;
                        }
                    }

                    _editorService.DropDownControl( listBox );
                    if ( listBox.SelectedItem == null ) return value;

                    return new DataTypeInfo( listBox.SelectedItem.ToString() );
                }
            }
        }

        public class AxisSettingConveter : TypeConverter
        {
            public override bool GetPropertiesSupported( ITypeDescriptorContext context )
            {
                return true;
            }

            public override PropertyDescriptorCollection GetProperties( ITypeDescriptorContext context, object value, Attribute[] attributes )
            {
                return TypeDescriptor.GetProperties( typeof( AxisSetting ) );
            }
        }
    }
}
