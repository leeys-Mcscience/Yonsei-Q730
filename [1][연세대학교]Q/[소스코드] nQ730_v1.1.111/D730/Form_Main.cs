using DataViewer.Class;
using McQLib.Recipes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ZedGraph;

namespace DataViewer
{
    public partial class Form_Main : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style &= ~0xC00000;
                return cp;
            }
        }

        static GraphSetting GraphSetting = new GraphSetting();

        private Sequence _sequence;

        private UserControl_Graph graph;
        UserControl_Sheet sheet = new UserControl_Sheet();

        public Form_Main()
        {
            InitializeComponent();
           
            GraphSetting = SoftwareConfiguration.GraphSetting;
            Application.Idle += Application_Idle;
            textBox_Preset.Text = "";
            textBox_Mass.Enabled = false;
            textBox_Area.Enabled = false;
            graph = new UserControl_Graph(GraphSetting, _sequence);
            //registry();
        }
        public Form_Main(Sequence sequence) : this()
        {
            _sequence = sequence;
            textBox_Mass.Text = "0";
            textBox_Area.Text = "0";
            if (_sequence != null)
            { 
                if (_sequence._batteryInfo.batteryArea != 0 || _sequence._batteryInfo.batteryMass != 0)
                {
                    textBox_Mass.Text = _sequence._batteryInfo.batteryMass.ToString();
                    textBox_Area.Text = _sequence._batteryInfo.batteryArea.ToString();
                }
            }

        }
        public Form_Main(string filename) : this()
        {
            // qrd start

        }

        private void saveDefault()
        {
            GraphSetting.LeftSplitterLocation = splitContainer2.SplitterDistance;
            GraphSetting.RightSplitterLocation = splitContainer1.SplitterDistance;
            GraphSetting.MainFormWidth = Width;
            GraphSetting.MainFormHeight = Height;
            GraphSetting.Save( Path.Combine( Application.StartupPath, "ViewerAutoSave.preset" ) );
        }
        private void loadDefault()
        {
            if ( File.Exists( Path.Combine( Application.StartupPath, "ViewerAutoSave.preset" ) ) )
            {
                GraphSetting.Load( Path.Combine( Application.StartupPath, "ViewerAutoSave.preset" ) );
                propertyGrid1.Refresh();
            }
        }

        private void Application_Idle( object sender, EventArgs e )
        {
            Application.Idle -= Application_Idle;

            try
            {
                loadDefault();

                Width = SoftwareConfiguration.GraphSetting.MainFormWidth;
                Height = SoftwareConfiguration.GraphSetting.MainFormHeight;
                splitContainer1.SplitterDistance = SoftwareConfiguration.GraphSetting.RightSplitterLocation;
                splitContainer2.SplitterDistance = SoftwareConfiguration.GraphSetting.LeftSplitterLocation;
            }
            catch
            {
                Width = 1250;
                Height = 860;
            }

            tableLayoutPanel1.Controls.Add( new UserControl_CaptionBar(), 0, 0 );

            tableLayoutPanel4.Controls.Add( graph, 0, 1 );
            _selectedTabIndex = GRAPH;

            propertyGrid1.SelectedObject = GraphSetting;

            var defaultAxis = new AxisSetting()
            {
                Color = Color.Black,
            };

            initAxis( graph.GraphPane.XAxis, defaultAxis );
            initAxis( graph.GraphPane.YAxis, defaultAxis );
            initAxis( graph.GraphPane.Y2Axis, defaultAxis);

            graph.RefreshGraph();

            treeView1.ItemHeight = 25;
            treeView1.ImageList = new ImageList();
            treeView1.ImageList.ImageSize = new Size( 25, 25 );

            treeView1.ImageList.Images.Add( "Data", Properties.Resources.Icon_Data );
            treeView1.ImageList.Images.Add( "Charge", RecipeFactory.GetRecipeIcon( RecipeType.Charge ) );
            treeView1.ImageList.Images.Add( "Discharge", RecipeFactory.GetRecipeIcon( RecipeType.Discharge ) );
            treeView1.ImageList.Images.Add( "Rest", RecipeFactory.GetRecipeIcon( RecipeType.Rest ) );
            treeView1.ImageList.Images.Add( "Cycle", RecipeFactory.GetRecipeIcon( RecipeType.Cycle ) );
            treeView1.ImageList.Images.Add( "Loop", RecipeFactory.GetRecipeIcon( RecipeType.Loop ) );
            treeView1.ImageList.Images.Add( "Jump", RecipeFactory.GetRecipeIcon( RecipeType.Jump ) );
            treeView1.ImageList.Images.Add( "FrequencyResponse", RecipeFactory.GetRecipeIcon( RecipeType.FrequencyResponse ) );
            treeView1.ImageList.Images.Add( "TransientResponse", RecipeFactory.GetRecipeIcon( RecipeType.TransientResponse ) );
            treeView1.ImageList.Images.Add( "AcResistance", RecipeFactory.GetRecipeIcon( RecipeType.AcResistance ) );
            treeView1.ImageList.Images.Add( "DcResistance", RecipeFactory.GetRecipeIcon( RecipeType.DcResistance ) );
            treeView1.ImageList.Images.Add( "Pattern", RecipeFactory.GetRecipeIcon( RecipeType.Pattern ) );
            treeView1.ImageList.Images.Add( "OpenCircuitVoltage", RecipeFactory.GetRecipeIcon( RecipeType.OpenCircuitVoltage ) );
            treeView1.ImageList.Images.Add( "AnodeCharge", RecipeFactory.GetRecipeIcon( RecipeType.AnodeCharge ) );
            treeView1.ImageList.Images.Add( "AnodeDischarge", RecipeFactory.GetRecipeIcon( RecipeType.AnodeDischarge ) );


            menu_Charge.Image = treeView1.ImageList.Images["Charge"];
            menu_Discharge.Image = treeView1.ImageList.Images["Discharge"];
            menu_Rest.Image = treeView1.ImageList.Images["Rest"];

            menu_Cycle.Image = treeView1.ImageList.Images["Cycle"];
            menu_Loop.Image = treeView1.ImageList.Images["Loop"];
            menu_Jump.Image = treeView1.ImageList.Images["Jump"];

            menu_Fra.Image = treeView1.ImageList.Images["FrequencyResponse"];
            menu_Tra.Image = treeView1.ImageList.Images["TransientResponse"];
            menu_Acr.Image = treeView1.ImageList.Images["AcResistance"];
            menu_Dcr.Image = treeView1.ImageList.Images["DcResistance"];
            menu_Ocv.Image = treeView1.ImageList.Images["OpenCircuitVoltage"];

            menu_Pattern.Image = treeView1.ImageList.Images["Pattern"];

            menu_AnodeCharge.Image = treeView1.ImageList.Images["AnodeCharge"];
            menu_AnodeDischarge.Image = treeView1.ImageList.Images["AnodeDischarge"];


            Application.DoEvents();
        }

        List<RecipeType> _recipeList = new List<RecipeType>();

        List<QDataManager> _fileList = new List<QDataManager>();

        public string presetPath { get; set; }

        public string TextPreset
        {
            get => this.textBox_Preset.Text;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                this.textBox_Preset.Text = value;
            }
        }

        private void button_FileLoad_Click( object sender, EventArgs e )
        {
            // Data Viewer로 인해 측정 소프트웨어가 중단되는 불상사를 막기 위해 어쩔 수 없이 잡히지 않는 모든 예외는 무시하도록 했음.
            try
            {
                using ( var dialog = new OpenFileDialog()
                {
                    Filter = "*.qrd|*.qrd",
                    FileName = "",
                    Multiselect = true
                } )
                {
                    if ( dialog.ShowDialog() == DialogResult.OK )
                    {
                        var failFiles = new List<string>();

                        var beforeIndex = _fileList.Count;
                        using(var loadingForm = new Form_Loading( dialog.FileNames ) )
                        {
                            if ( loadingForm.ShowDialog() == DialogResult.Cancel ) return;

                            _fileList.AddRange( loadingForm.Result );
                            failFiles.AddRange( loadingForm.FailFiles );
                        }

                        for(var i = beforeIndex; i < _fileList.Count; i++ )
                        {
                            var mgr = _fileList[i];
                            for ( var j = 0; j < mgr.Count; j++ )
                            {
                                _recipeList.Add( mgr[j].RecipeType );
                            }

                            treeView1.Nodes.Add( mgr.Root );
                        }

                        propertyGrid1.Refresh();
                        if ( failFiles.Count != 0 )
                        {
                            var fileNameString = string.Empty;
                            for ( var i = 0; i < failFiles.Count; i++ ) fileNameString += $"\r\n{failFiles[i]}";
                            MessageBox.Show( $"아래 파일은 측정 데이터 집합을 구성하는데 실패했습니다.\r\n{fileNameString}", "Q730 알림 메시지" );
                        }

                        _recipeList = _recipeList.Distinct().ToList();
                        AxisSetting.DataTypeInfo.SetColumnList( _recipeList.ToArray() );

                        return;
                    }
                }
            }
            catch
            {

            }
        }

        public void FileLoad(string[] path)
        {

            var beforeIndex = _fileList.Count;
            using (var loadingForm = new Form_Loading(path))
            {
                if (loadingForm.ShowDialog() == DialogResult.Cancel) return;

                _fileList.AddRange(loadingForm.Result);
                //failFiles.AddRange(loadingForm.FailFiles);
            }

            for (var i = beforeIndex; i < _fileList.Count; i++)
            {
                var mgr = _fileList[i];
                for (var j = 0; j < mgr.Count; j++)
                {
                    _recipeList.Add(mgr[j].RecipeType);
                }

                treeView1.Nodes.Add(mgr.Root);
            }

            propertyGrid1.Refresh();


            GraphSetting.Load(presetPath);

            //if (failFiles.Count != 0)
            //{
            //    var fileNameString = string.Empty;
            //    for (var i = 0; i < failFiles.Count; i++) fileNameString += $"\r\n{failFiles[i]}";
            //    MessageBox.Show($"아래 파일은 측정 데이터 집합을 구성하는데 실패했습니다.\r\n{fileNameString}", "Q730 알림 메시지");
            //}

            foreach (TreeNode node in treeView1.Nodes)
            {
                node.Checked = true;
                //for (int i = 0; i < temp.Count; i++)
                //{
                //    temp[i].Node.Checked = true;
                //}
            }
         
            _recipeList = _recipeList.Distinct().ToList();
            AxisSetting.DataTypeInfo.SetColumnList(_recipeList.ToArray());


            if (GraphSetting.XAxis.Data.SelectedData == DataType.None)
            {
                MessageBox.Show("X축으로 사용할 데이터가 선택되지 않았습니다.");
                return;
            }

            if (GraphSetting.YAxis.Data.SelectedData == DataType.None)
            {
                MessageBox.Show("Y축으로 사용할 데이터가 선택되지 않았습니다.");
                return;
            }

            setGraphSetting();
            makeCurve();
            graph.RefreshGraph();


            //if (GraphSetting.YAxisList.Count == 0 && GraphSetting.Y2AxisList.Count == 0)
            //{
            //    MessageBox.Show("Y축이 추가되지 않았습니다.");
            //    return;
            //}

            //if (GraphSetting.Clear)
            //{
            //    graph.ClearGraph();
            //}

            //if (GraphSetting.Y2AxisList.Count > 0)
            //{
            //    makeCurve(GraphSetting.Y2AxisList.ToArray(), true);
            //}
        }

        private void treeView1_AfterCheck( object sender, TreeViewEventArgs e )
        {
            foreach ( TreeNode n in e.Node.Nodes )
            {
                n.Checked = e.Node.Checked;
            }
        }

        private void initAxis( Axis axis, AxisSetting setting )
        {
            // Display
            axis.IsVisible = true;
            axis.AxisGap = 0.0f;

            // Scale
            axis.Scale.Min = setting.Min;
            axis.Scale.Max = setting.Max;
            axis.Scale.MinAuto = setting.MinAuto;
            axis.Scale.MaxAuto = setting.MaxAuto;

            // Font
            axis.Title.FontSpec.FontColor = axis.Color;
            axis.Title.FontSpec.Size = 7.0f;
            axis.Scale.FontSpec.FontColor = axis.Color;
            axis.Scale.FontSpec.Size = 7.0f;

            // Tics
            axis.MinorTic.IsOpposite = false;
            axis.MinorTic.IsAllTics = false;
            axis.MinorTic.IsOutside = false;
            axis.MajorTic.IsOpposite = false;
            axis.MajorTic.IsOutside = false;

            // Grids
            axis.MajorGrid.IsZeroLine = setting.ZeroLine;

            axis.Type = setting.LogScale ? AxisType.Log : AxisType.Linear;

        }
        private void initCurve( LineItem item, AxisSetting setting )
        {
            item.Symbol.Type = setting.SymbolType;
            item.Line.Width = setting.LineWidth;
            item.Line.Color = setting.Color;
            item.Label.Text = setting.Data.ToString();
        }
        private void setGraphSetting()
        {
            //-------------------------------------------------------------------------------------
            graph.GraphPane.Title.Text = GraphSetting.Title;

            graph.XAxis.Title.Text = dataTypeToString( GraphSetting.XAxis.Data.SelectedData );
            graph.XAxis.Title.FontSpec.Size = 7.0f;

            graph.XAxis.Scale.FontSpec.Size = 7.0f;
            graph.XAxis.Scale.Min = GraphSetting.XAxis.Min;
            graph.XAxis.Scale.Max = GraphSetting.XAxis.Max;
            graph.XAxis.Scale.MinAuto = GraphSetting.XAxis.MinAuto;
            graph.XAxis.Scale.MaxAuto = GraphSetting.XAxis.MaxAuto;
            graph.XAxis.Type = GraphSetting.XAxis.LogScale ? AxisType.Log : AxisType.Linear;

            ////-------------------------------------------------------------------------------------
            //if (GraphSetting.YAxis.Data.SelectedData != DataType.None)
            //{
            //    graph.YAxis.Title.Text = dataTypeToString(GraphSetting.YAxis.Data.SelectedData);
            //    graph.YAxis.Title.FontSpec.Size = 7.0f;

            //    graph.YAxis.Scale.IsVisible = true;
            //    graph.YAxis.Scale.FontSpec.Size = 7.0f;
            //    graph.YAxis.Scale.Min = GraphSetting.YAxis.Min;
            //    graph.YAxis.Scale.Max = GraphSetting.YAxis.Max;
            //    graph.YAxis.Scale.MinAuto = GraphSetting.YAxis.MinAuto;
            //    graph.YAxis.Scale.MaxAuto = GraphSetting.YAxis.MaxAuto;
            //    graph.YAxis.Type = GraphSetting.YAxis.LogScale ? AxisType.Log : AxisType.Linear;
            //}
            //else
            //{
            //    graph.YAxis.Title.Text = "";
            //    graph.YAxis.Scale.IsVisible = false;
            //}


            ////-------------------------------------------------------------------------------------
            //if (GraphSetting.Y2Axis.Data.SelectedData != DataType.None)
            //{
            //    graph.Y2Axis.Title.Text = dataTypeToString(GraphSetting.Y2Axis.Data.SelectedData);
            //    graph.Y2Axis.Title.FontSpec.Size = 7.0f;

            //    graph.Y2Axis.Scale.IsVisible = true;
            //    graph.Y2Axis.Scale.FontSpec.Size = 7.0f;
            //    graph.Y2Axis.Scale.Min = GraphSetting.Y2Axis.Min;
            //    graph.Y2Axis.Scale.Max = GraphSetting.Y2Axis.Max;
            //    graph.Y2Axis.Scale.MinAuto = false;
            //    graph.Y2Axis.Scale.MaxAuto = false;
            //    graph.Y2Axis.Type = GraphSetting.Y2Axis.LogScale ? AxisType.Log : AxisType.Linear;



            //}
            //else
            //{
            //    graph.Y2Axis.Title.Text = "";
            //    graph.Y2Axis.Scale.IsVisible = false;
            //}


            //YAxisList ,  Y2AxisList 추가
            graph.YAxisList.Clear();
            graph.YAxisList.Add(dataTypeToString(GraphSetting.YDataType));
            initAxis(graph.YAxis, GraphSetting.YAxis);
            //}
            //if (GraphSetting.YAxisList.Count == 0)
            //{
            //    var idx = graph.YAxisList.Add("");
            //    graph.YAxisList[idx].IsVisible = false;
            //}

            graph.Y2AxisList.Clear();
            graph.Y2AxisList.Add(dataTypeToString(GraphSetting.Y2DataType));
            initAxis(graph.Y2Axis, GraphSetting.Y2Axis);


        
            graph.GraphPane.XAxis.MajorGrid.IsVisible = true;
            graph.GraphPane.YAxis.MajorGrid.IsVisible = true;
            //if (graph.YAxisList.Count != 0) graph.GraphPane.YAxis.MajorGrid.IsVisible = GraphSetting.MajorGrid;
        }
        private string dataTypeToString( DataType dataType )
        {
            switch ( dataType )
            {
                case DataType.TotalTime:
                    return $"Total Time ({GraphSetting.TimeUnit.UnitInfo.UnitString})";

                case DataType.StepTime:
                    return $"Step Time ({GraphSetting.TimeUnit.UnitInfo.UnitString})";

                case DataType.Voltage:
                    return $"Voltage ({GraphSetting.VoltageUnit.UnitInfo.UnitString})";

                case DataType.Current:
                    return $"Current ({GraphSetting.CurrentUnit.UnitInfo.UnitString})";

                case DataType.Power:
                    return $"Power ({GraphSetting.PowerUnit.UnitInfo.UnitString})";

                case DataType.WattHour:
                    return $"WattHour ({GraphSetting.PowerUnit.UnitInfo.UnitString}h)";

                case DataType.Capacity:
                    return $"Capacity ({GraphSetting.CurrentUnit.UnitInfo.UnitString}h)";

                //case DataType.R:
                //    return $"Resistance ({GraphSetting.ResistanceUnit.UnitInfo.UnitString})";

                case DataType.Temperature:
                    return $"Temperature (℃)";


                default:
                    return dataType.ToString();
            }
        }

        private void button_Apply_Click( object sender, EventArgs e )
        {
            // Data Viewer로 인해 측정 소프트웨어가 중단되는 불상사를 막기 위해 어쩔 수 없이 잡히지 않는 모든 예외는 무시하도록 했음.
            try
            {
                if ( GraphSetting.XAxis.Data.SelectedData == DataType.None )
                {
                    MessageBox.Show( "X축으로 사용할 데이터가 선택되지 않았습니다." );
                    return;
                }

                //if ( GraphSetting.YAxisList.Count == 0 && GraphSetting.Y2AxisList.Count == 0 )
                //{
                //    MessageBox.Show( "Y축이 추가되지 않았습니다." );
                //    return;
                //}

                //if ( GraphSetting.Clear )
                //{
                //    graph.ClearGraph();
                //}

                setGraphSetting();

                //if (GraphSetting.YAxisList.Count > 0)
                //{\
         
                makeCurve();


                //graph.GraphPane.XAxis.Scale.Min = GraphSetting.YMin;
                //graph.GraphPane.XAxis.Scale.Max = GraphSetting.YMax;
                //graph.Invalidate();
                graph.graph.ZoomOutAll(graph.GraphPane);
                graph.RefreshGraph();

                //}

                //if (GraphSetting.Y2AxisList.Count > 0)
                //{
                //    makeCurve(GraphSetting.Y2AxisList.ToArray(), true);
                //}



            }
            catch(Exception exception)
            {
                MessageBox.Show($"{exception.Message}", @"Sheet Page OverSize");
            }
        }



        private void makeCurve()
        {
            var xData = new List<double>();
            var yData = new List<double>();
            var y2Data = new List<double>();

            var cData = new List<double>();
            var dcData = new List<double>();

            var inCycleLoop = false;
            graph.ClearGraph();
            try
            {
                foreach (var qrdFile in _fileList)
                {
                    //가장먼저해야할게 노드가 체크안된거부터 확인
                    if (qrdFile.Root.Checked)
                    {

                        for (var j = 0; j < qrdFile.RecipeDatas.Count; j++) // _nodeList[i].Count = 레시피 개수
                        {
                            if (GraphSetting.XAxis.Data.SelectedData == DataType.Cycle)
                            {
                                //1) 사이클 별 충전 용량 그래프 데이터 --------------------------------------------------
                                if (GraphSetting.YAxis.Data.SelectedData == DataType.ChargeCapacity)
                                {
                                    if (qrdFile.RecipeDatas[j].RecipeType == RecipeType.Charge ||
                                        qrdFile.RecipeDatas[j].RecipeType == RecipeType.AnodeCharge)
                                    {
                                        if (qrdFile.RecipeDatas[j].Datas.Count > 0 && qrdFile[j].Node.Checked)
                                        {
                                            double max = qrdFile.RecipeDatas[j]
                                                .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity;
                                            string output = new string(qrdFile[j].Node.Parent.ToString()
                                                .Where(char.IsDigit).ToArray());
                                            int number = int.Parse(output);
                                            xData.Add(number);
                                            yData.Add(max);
                                        }
                                    }
                                }
                                //2) 사이클 별 방전 용량 그래프 데이터 --------------------------------------------------
                                else if (GraphSetting.YAxis.Data.SelectedData == DataType.DisChargeCapacity)
                                {
                                    if (qrdFile.RecipeDatas[j].RecipeType == RecipeType.Discharge ||
                                        qrdFile.RecipeDatas[j].RecipeType == RecipeType.AnodeDischarge)
                                    {
                                        if (qrdFile.RecipeDatas[j].Datas.Count > 0 && qrdFile[j].Node.Checked)
                                        {
                                            double max = qrdFile.RecipeDatas[j]
                                                .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity;
                                            string output = new string(qrdFile[j].Node.Parent.ToString()
                                                .Where(char.IsDigit).ToArray());
                                            int number = int.Parse(output);
                                            xData.Add(number);
                                            yData.Add(max);
                                        }
                                    }
                                }
                                //3) 사이클 별 쿨룽효율 용량 그래프 데이터 --------------------------------------------------
                                else if (GraphSetting.YAxis.Data.SelectedData == DataType.CoulombEfficiency)
                                {
                                    if (qrdFile.RecipeDatas[j].Datas.Count > 0 && qrdFile[j].Node.Checked)
                                    {
                                        switch (qrdFile.RecipeDatas[j].RecipeType)
                                        {
                                            case RecipeType.Charge:
                                            case RecipeType.AnodeCharge:
                                                {
                                                    cData.Add(qrdFile.RecipeDatas[j]
                                                        .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity);
                                                    var output = new string(qrdFile[j].Node.Parent.ToString()
                                                        .Where(char.IsDigit).ToArray());
                                                    var number = int.Parse(output);
                                                    if (!xData.Contains(number))
                                                    {
                                                        xData.Add(number);
                                                    }

                                                    break;
                                                }
                                            case RecipeType.Discharge:
                                            case RecipeType.AnodeDischarge:
                                                {
                                                    dcData.Add(qrdFile.RecipeDatas[j]
                                                        .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity);
                                                    var output = new string(qrdFile[j].Node.Parent.ToString()
                                                        .Where(char.IsDigit).ToArray());
                                                    var number = int.Parse(output);
                                                    if (!xData.Contains(number))
                                                    {
                                                        xData.Add(number);
                                                    }

                                                    break;
                                                }
                                        }
                                    }
                                }

                                //4) Y2 사이클 별 충전 용량 그래프 데이터 --------------------------------------------------
                                if (GraphSetting.Y2Axis.Data.SelectedData == DataType.ChargeCapacity)
                                {
                                    if (qrdFile.RecipeDatas[j].RecipeType == RecipeType.Charge ||
                                        qrdFile.RecipeDatas[j].RecipeType == RecipeType.AnodeCharge)
                                    {
                                        if (qrdFile.RecipeDatas[j].Datas.Count > 0 && qrdFile[j].Node.Checked)
                                        {
                                            double max = qrdFile.RecipeDatas[j]
                                                .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity;
                                            string output = new string(qrdFile[j].Node.Parent.ToString()
                                                .Where(char.IsDigit).ToArray());
                                            int number = int.Parse(output);
                                            //xData.Add(number);
                                            y2Data.Add(max);
                                        }
                                    }
                                }
                                //2) 사이클 별 방전 용량 그래프 데이터 --------------------------------------------------
                                else if (GraphSetting.Y2Axis.Data.SelectedData == DataType.DisChargeCapacity)
                                {
                                    if (qrdFile.RecipeDatas[j].RecipeType == RecipeType.Discharge ||
                                        qrdFile.RecipeDatas[j].RecipeType == RecipeType.AnodeDischarge)
                                    {
                                        if (qrdFile.RecipeDatas[j].Datas.Count > 0 && qrdFile[j].Node.Checked)
                                        {
                                            double max = qrdFile.RecipeDatas[j]
                                                .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity;
                                            string output = new string(qrdFile[j].Node.Parent.ToString()
                                                .Where(char.IsDigit).ToArray());
                                            int number = int.Parse(output);
                                            //xData.Add(number);
                                            y2Data.Add(max);
                                        }
                                    }
                                }
                                //3) 사이클 별 쿨룽효율 용량 그래프 데이터 --------------------------------------------------
                                else if (GraphSetting.Y2Axis.Data.SelectedData == DataType.CoulombEfficiency)
                                {
                                    if (qrdFile.RecipeDatas[j].Datas.Count > 0 && qrdFile[j].Node.Checked)
                                    {
                                        switch (qrdFile.RecipeDatas[j].RecipeType)
                                        {
                                            case RecipeType.Charge:
                                            case RecipeType.AnodeCharge:
                                                {
                                                    cData.Add(qrdFile.RecipeDatas[j]
                                                        .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity);
                                                    var output = new string(qrdFile[j].Node.Parent.ToString()
                                                        .Where(char.IsDigit).ToArray());
                                                    var number = int.Parse(output);
                                                    if (!xData.Contains(number))
                                                    {
                                                        xData.Add(number);
                                                    }

                                                    break;
                                                }
                                            case RecipeType.Discharge:
                                            case RecipeType.AnodeDischarge:
                                                {
                                                    dcData.Add(qrdFile.RecipeDatas[j]
                                                        .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity);
                                                    var output = new string(qrdFile[j].Node.Parent.ToString()
                                                        .Where(char.IsDigit).ToArray());
                                                    var number = int.Parse(output);
                                                    if (!xData.Contains(number))
                                                    {
                                                        xData.Add(number);
                                                    }

                                                    break;
                                                }
                                        }
                                    }
                                }
                            } // 사이클 별 데이터 확보 끝 // 이후 데이터 는 이후로 들어갑니다.

                            else // X축이 사이클이 아닌경우 모든 경우를 뜻한다. Step Time 외 등등..
                            {
                                if (qrdFile[j].RecipeType !=
                                    RecipeType.Cycle) // Cycle 시작 -> 기존 xData, yData Flush & Clear
                                {
                                    inCycleLoop = true;
                                }
                                else if (qrdFile[j].RecipeType !=
                                         RecipeType.Loop) // Cycle 종료 -> 현재 xData, yData Flush & Clear
                                {
                                    inCycleLoop = false;
                                }

                                //node가 체크 된 애들중에 x축이 StepTime인 경우에는 누적 처리 로직
                                if (qrdFile[j].Node.Checked)
                                {
                                    if (inCycleLoop && GraphSetting.XAxis.Data.SelectedData == DataType.StepTime)
                                    {
                                        // 특수 처리 - StepTime이 기준이고, Loops 단위로 커브를 생성하는 경우 StepTime 누적시키기
                                        var tmp = qrdFile[j].GetData(GraphSetting.XAxis.Data);

                                        if (xData.Count != 0)
                                        {
                                            for (var x = 0; x < tmp.Length; x++)
                                            {
                                                tmp[x] += xData[xData.Count - 1];
                                            }
                                        }

                                        xData.AddRange(tmp);

                                        if (GraphSetting.YAxis.Data != DataType.None)
                                        {
                                            yData.AddRange(qrdFile[j].GetData(GraphSetting.YAxis.Data));
                                        }

                                        if (GraphSetting.Y2Axis.Data != DataType.None)
                                        {
                                            y2Data.AddRange(qrdFile[j].GetData(GraphSetting.Y2Axis.Data));
                                        }
                                    }
                                    else
                                    {
                                        if (GraphSetting.XAxis.Data != DataType.None)
                                        {
                                            xData.AddRange(qrdFile[j].GetData(GraphSetting.XAxis.Data));
                                        }

                                        if (GraphSetting.YAxis.Data != DataType.None)
                                        {
                                            yData.AddRange(qrdFile[j].GetData(GraphSetting.YAxis.Data));
                                        }

                                        if (GraphSetting.Y2Axis.Data != DataType.None)
                                        {
                                            y2Data.AddRange(qrdFile[j].GetData(GraphSetting.Y2Axis.Data));
                                        }


                                    }
                                }
                            }
                            //x데이터 넣는곳

                        } //레시피 데이터 카운트 갯수 for 문 끝


                        //이제부터 Y데이터 넣는곳

                        //loopCount 때문에 맨뒤에서 처리 하도록 로직을 구현한것 같음 장재훈
                        if (GraphSetting.YAxis.Data.SelectedData == DataType.CoulombEfficiency)
                        {
                            yData.Clear();
                            int loopCount = cData.Count >= dcData.Count ? dcData.Count : cData.Count;

                            for (int temp = 0; temp < loopCount; temp++)
                            {
                                if (dcData[temp] != null && cData[temp] != null)
                                {
                                    double percent = ((dcData[temp] / cData[temp]) * 100);
                                    yData.Add(percent);
                                }
                            }
                        }

                        if (xData.Count != 0 || yData.Count != 0)
                        {
                            //graph.ClearGraph();

                            //GraphSetting.XMaxAuto
                            if (!GraphSetting.XMaxAuto)
                            {
                                xData = xData.Where(x => x <= GraphSetting.XMax).ToList();
                            }

                            if (!GraphSetting.XMinAuto)
                            {
                                xData = xData.Where(x => x >= GraphSetting.XMin).ToList();
                            }

                            if (!GraphSetting.YMaxAuto)
                            {
                                yData = yData.Where(x => x <= GraphSetting.YMax).ToList();
                            }

                            if (!GraphSetting.YMinAuto)
                            {
                                yData = yData.Where(x => x >= GraphSetting.YMin).ToList();
                            }

                            if (!GraphSetting.YMaxAuto)
                            {
                                y2Data = y2Data.Where(x => x <= GraphSetting.YMax).ToList();
                            }

                            if (!GraphSetting.YMinAuto)
                            {
                                y2Data = y2Data.Where(x => x >= GraphSetting.YMin).ToList();
                            }

                            if (GraphSetting.YAxis.Data != DataType.None)
                            {
                                var curve = graph.GraphPane.AddCurve("",
                                    xData.ToArray(),
                                    yData.ToArray(),
                                    GraphSetting.YGraphColor,
                                    GraphSetting.Symbol
                                );

                                //initCurve(curve, GraphSetting.YAxisList[0]);
                                curve.YAxisIndex = 0;
                                curve.IsY2Axis = false;
                                curve.Symbol.Size = GraphSetting.SymbolSize;
                                curve.Symbol.Fill = new Fill(GraphSetting.YGraphColor);
                            }

                            if (GraphSetting.Y2Axis.Data != DataType.None)
                            {
                                var curve2 = graph.GraphPane.AddCurve("",
                                    xData.ToArray(),
                                    y2Data.ToArray(),
                                    GraphSetting.Y2GraphColor,
                                    GraphSetting.Symbol
                                );

                                //initCurve(curve2, GraphSetting.YAxisList[0]);
                                curve2.YAxisIndex = 1;
                                curve2.IsY2Axis = true;
                                curve2.Symbol.Size = GraphSetting.SymbolSize;
                                curve2.Symbol.Fill = new Fill(GraphSetting.Y2GraphColor);
                            }


                            xData.Clear();
                            yData.Clear();
                            y2Data.Clear();
                        }
                        else
                        {
                            //데이터가 아무것도없으면 그래프 초기화
                            graph.ClearGraph();
                            return;
                        }

                        graph._fileList = _fileList;
                    }

                
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void makeCurveMassArea(bool massOrArea)
        {
            var xData = new List<double>();
            var yData = new List<double>();
            var y2Data = new List<double>();

            var cData = new List<double>();
            var dcData = new List<double>();

            var inCycleLoop = false;
            try
            {
                foreach (var qrdFile in _fileList)
                {
                    for (var j = 0; j < qrdFile.RecipeDatas.Count; j++) // _nodeList[i].Count = 레시피 개수
                    {
                        if (GraphSetting.XAxis.Data.SelectedData == DataType.Cycle)
                        {
                            //1) 사이클 별 충전 용량 그래프 데이터 --------------------------------------------------
                            if (GraphSetting.YAxis.Data.SelectedData == DataType.ChargeCapacity)
                            {
                                if (qrdFile.RecipeDatas[j].RecipeType == RecipeType.Charge ||
                                    qrdFile.RecipeDatas[j].RecipeType == RecipeType.AnodeCharge)
                                {
                                    if (qrdFile.RecipeDatas[j].Datas.Count > 0 && qrdFile[j].Node.Checked)
                                    {
                                        double max = qrdFile.RecipeDatas[j]
                                            .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity;
                                        string output = new string(qrdFile[j].Node.Parent.ToString()
                                            .Where(char.IsDigit).ToArray());
                                        int number = int.Parse(output);
                                        xData.Add(number);
                                        yData.Add(max);
                                    }
                                }
                            }
                            //2) 사이클 별 방전 용량 그래프 데이터 --------------------------------------------------
                            else if (GraphSetting.YAxis.Data.SelectedData == DataType.DisChargeCapacity)
                            {
                                if (qrdFile.RecipeDatas[j].RecipeType == RecipeType.Discharge ||
                                    qrdFile.RecipeDatas[j].RecipeType == RecipeType.AnodeDischarge)
                                {
                                    if (qrdFile.RecipeDatas[j].Datas.Count > 0 && qrdFile[j].Node.Checked)
                                    {
                                        double max = qrdFile.RecipeDatas[j]
                                            .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity;
                                        string output = new string(qrdFile[j].Node.Parent.ToString()
                                            .Where(char.IsDigit).ToArray());
                                        int number = int.Parse(output);
                                        xData.Add(number);
                                        yData.Add(max);
                                    }
                                }
                            }
                            //3) 사이클 별 쿨룽효율 용량 그래프 데이터 --------------------------------------------------
                            else if (GraphSetting.YAxis.Data.SelectedData == DataType.CoulombEfficiency)
                            {
                                if (qrdFile.RecipeDatas[j].Datas.Count > 0 && qrdFile[j].Node.Checked)
                                {
                                    switch (qrdFile.RecipeDatas[j].RecipeType)
                                    {
                                        case RecipeType.Charge:
                                        case RecipeType.AnodeCharge:
                                            {
                                                cData.Add(qrdFile.RecipeDatas[j]
                                                    .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity);
                                                var output = new string(qrdFile[j].Node.Parent.ToString()
                                                    .Where(char.IsDigit).ToArray());
                                                var number = int.Parse(output);
                                                if (!xData.Contains(number))
                                                {
                                                    xData.Add(number);
                                                }

                                                break;
                                            }
                                        case RecipeType.Discharge:
                                        case RecipeType.AnodeDischarge:
                                            {
                                                dcData.Add(qrdFile.RecipeDatas[j]
                                                    .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity);
                                                var output = new string(qrdFile[j].Node.Parent.ToString()
                                                    .Where(char.IsDigit).ToArray());
                                                var number = int.Parse(output);
                                                if (!xData.Contains(number))
                                                {
                                                    xData.Add(number);
                                                }

                                                break;
                                            }
                                    }
                                }
                            }

                            //4) Y2 사이클 별 충전 용량 그래프 데이터 --------------------------------------------------
                            if (GraphSetting.Y2Axis.Data.SelectedData == DataType.ChargeCapacity)
                            {
                                if (qrdFile.RecipeDatas[j].RecipeType == RecipeType.Charge ||
                                    qrdFile.RecipeDatas[j].RecipeType == RecipeType.AnodeCharge)
                                {
                                    if (qrdFile.RecipeDatas[j].Datas.Count > 0 && qrdFile[j].Node.Checked)
                                    {
                                        double max = qrdFile.RecipeDatas[j]
                                            .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity;
                                        string output = new string(qrdFile[j].Node.Parent.ToString()
                                            .Where(char.IsDigit).ToArray());
                                        int number = int.Parse(output);
                                        //xData.Add(number);
                                        y2Data.Add(max);
                                    }
                                }
                            }
                            //2) 사이클 별 방전 용량 그래프 데이터 --------------------------------------------------
                            else if (GraphSetting.Y2Axis.Data.SelectedData == DataType.DisChargeCapacity)
                            {
                                if (qrdFile.RecipeDatas[j].RecipeType == RecipeType.Discharge ||
                                    qrdFile.RecipeDatas[j].RecipeType == RecipeType.AnodeDischarge)
                                {
                                    if (qrdFile.RecipeDatas[j].Datas.Count > 0 && qrdFile[j].Node.Checked)
                                    {
                                        double max = qrdFile.RecipeDatas[j]
                                            .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity;
                                        string output = new string(qrdFile[j].Node.Parent.ToString()
                                            .Where(char.IsDigit).ToArray());
                                        int number = int.Parse(output);
                                        //xData.Add(number);
                                        y2Data.Add(max);
                                    }
                                }
                            }
                            //3) 사이클 별 쿨룽효율 용량 그래프 데이터 --------------------------------------------------
                            else if (GraphSetting.Y2Axis.Data.SelectedData == DataType.CoulombEfficiency)
                            {
                                if (qrdFile.RecipeDatas[j].Datas.Count > 0 && qrdFile[j].Node.Checked)
                                {
                                    switch (qrdFile.RecipeDatas[j].RecipeType)
                                    {
                                        case RecipeType.Charge:
                                        case RecipeType.AnodeCharge:
                                            {
                                                cData.Add(qrdFile.RecipeDatas[j]
                                                    .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity);
                                                var output = new string(qrdFile[j].Node.Parent.ToString()
                                                    .Where(char.IsDigit).ToArray());
                                                var number = int.Parse(output);
                                                if (!xData.Contains(number))
                                                {
                                                    xData.Add(number);
                                                }

                                                break;
                                            }
                                        case RecipeType.Discharge:
                                        case RecipeType.AnodeDischarge:
                                            {
                                                dcData.Add(qrdFile.RecipeDatas[j]
                                                    .Datas[qrdFile.RecipeDatas[j].Datas.Count - 1].Capacity);
                                                var output = new string(qrdFile[j].Node.Parent.ToString()
                                                    .Where(char.IsDigit).ToArray());
                                                var number = int.Parse(output);
                                                if (!xData.Contains(number))
                                                {
                                                    xData.Add(number);
                                                }

                                                break;
                                            }
                                    }
                                }
                            }




                        } // 사이클 별 데이터 확보 끝 // 이후 데이터 는 이후로 들어갑니다.

                        else // X축이 사이클이 아닌경우 모든 경우를 뜻한다. Step Time 외 등등..
                        {
                            if (qrdFile[j].RecipeType !=
                                RecipeType.Cycle) // Cycle 시작 -> 기존 xData, yData Flush & Clear
                            {
                                inCycleLoop = true;
                            }
                            else if (qrdFile[j].RecipeType !=
                                     RecipeType.Loop) // Cycle 종료 -> 현재 xData, yData Flush & Clear
                            {
                                inCycleLoop = false;
                            }

                            //node가 체크 된 애들중에 x축이 StepTime인 경우에는 누적 처리 로직
                            if (qrdFile[j].Node.Checked)
                            {
                                if (inCycleLoop && GraphSetting.XAxis.Data.SelectedData == DataType.StepTime)
                                {
                                    // 특수 처리 - StepTime이 기준이고, Loops 단위로 커브를 생성하는 경우 StepTime 누적시키기
                                    var tmp = qrdFile[j].GetData(GraphSetting.XAxis.Data);

                                    if (xData.Count != 0)
                                    {
                                        for (var x = 0; x < tmp.Length; x++)
                                        {
                                            tmp[x] += xData[xData.Count - 1];
                                        }
                                    }

                                    xData.AddRange(tmp);
                                }
                                else
                                {
                                    if (GraphSetting.XAxis.Data != DataType.None)
                                    {
                                        xData.AddRange(qrdFile[j].GetData(GraphSetting.XAxis.Data));
                                    }

                                    if (GraphSetting.YAxis.Data != DataType.None)
                                    {
                                        yData.AddRange(qrdFile[j].GetData(GraphSetting.YAxis.Data));
                                    }

                                    if (GraphSetting.Y2Axis.Data != DataType.None)
                                    {
                                        y2Data.AddRange(qrdFile[j].GetData(GraphSetting.Y2Axis.Data));
                                    }


                                }
                            }
                        }
                        //x데이터 넣는곳

                    } //레시피 데이터 카운트 갯수 for 문 끝


                    //이제부터 Y데이터 넣는곳

                    //loopCount 때문에 맨뒤에서 처리 하도록 로직을 구현한것 같음 장재훈
                    if (GraphSetting.YAxis.Data.SelectedData == DataType.CoulombEfficiency)
                    {
                        yData.Clear();
                        int loopCount = cData.Count >= dcData.Count ? dcData.Count : cData.Count;

                        for (int temp = 0; temp < loopCount; temp++)
                        {
                            if (dcData[temp] != null && cData[temp] != null)
                            {
                                double percent = ((dcData[temp] / cData[temp]) * 100);
                                yData.Add(percent);
                            }
                        }
                    }

                    if (xData.Count != 0 || yData.Count != 0)
                    {
                        graph.ClearGraph();

                        var dividedData = new List<double>();
                        var dividedData2 = new List<double>();

                        if (!GraphSetting.XMaxAuto)
                        {
                            xData = xData.Where(x => x <= GraphSetting.XMax).ToList();
                        }
                        if (!GraphSetting.XMinAuto)
                        {
                            xData = xData.Where(x => x >= GraphSetting.XMin).ToList();
                        }

                        if (!GraphSetting.YMaxAuto)
                        {
                            yData = yData.Where(x => x <= GraphSetting.YMax).ToList();
                        }
                        if (!GraphSetting.YMinAuto)
                        {
                            yData = yData.Where(x => x >= GraphSetting.YMin).ToList();
                        }

                        if (!GraphSetting.YMaxAuto)
                        {
                            y2Data = y2Data.Where(x => x <= GraphSetting.YMax).ToList();
                        }
                        if (!GraphSetting.YMinAuto)
                        {
                            y2Data = y2Data.Where(x => x >= GraphSetting.YMin).ToList();
                        }

                        if (massOrArea)
                        {
                            foreach (double value in yData.ToArray())
                            {
                                double dividedValue = value / _sequence._batteryInfo.batteryMass;
                                dividedData.Add(dividedValue);
                            }


                            var curve = graph.GraphPane.AddCurve("",
                                xData.ToArray(),
                                dividedData.ToArray(),
                                GraphSetting.YGraphColor,
                                GraphSetting.Symbol
                            );

                            var curve2 = graph.GraphPane.AddCurve("",
                                xData.ToArray(),
                                y2Data.ToArray(),
                                GraphSetting.Y2GraphColor,
                                GraphSetting.Symbol
                            );

                            curve.Symbol.Size = GraphSetting.SymbolSize;
                            curve.Symbol.Fill = new Fill(GraphSetting.YGraphColor); 

                            curve2.Symbol.Size = GraphSetting.SymbolSize;
                            curve2.Symbol.Fill = new Fill(GraphSetting.Y2GraphColor);
                        }
                        else
                        {
                            foreach (double value in yData.ToArray())
                            {
                                double dividedValue = value / _sequence._batteryInfo.batteryArea;
                                dividedData.Add(dividedValue);
                            }


                            var curve = graph.GraphPane.AddCurve("",
                                xData.ToArray(),
                                dividedData.ToArray(),
                                GraphSetting.YGraphColor,
                                GraphSetting.Symbol
                            );

                            var curve2 = graph.GraphPane.AddCurve("",
                                xData.ToArray(),
                                y2Data.ToArray(),
                                GraphSetting.Y2GraphColor,
                                GraphSetting.Symbol
                            );

                            curve.Symbol.Size = GraphSetting.SymbolSize;
                            curve.Symbol.Fill = new Fill(GraphSetting.YGraphColor);

                            curve2.Symbol.Size = GraphSetting.SymbolSize;
                            curve2.Symbol.Fill = new Fill(GraphSetting.Y2GraphColor);
                        }

                        xData.Clear();
                        yData.Clear();
                        y2Data.Clear();
                    }
                    else
                    {
                        return;
                    }

                    graph._fileList = _fileList;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Color GetRandomColor()
        {
            Random random = new Random();
            return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        }
        private void selectNode( TreeNode node, string key )
        {
            if ( node.Text == key )
            {
                node.Checked = true;
            }
            else
            {
                foreach ( TreeNode child in node.Nodes )
                {
                    selectNode( child, key );
                }
            }
        }
        private void menu_MultiSelect_Click( object sender, EventArgs e )
        {
            var key = ( sender as ToolStripMenuItem ).Tag.ToString();

            foreach ( TreeNode root in treeView1.Nodes )
            {
                foreach ( TreeNode node in root.Nodes )
                {
                    selectNode( node, key );
                }
            }
        }

        private void menu_Unselect_Click( object sender, EventArgs e )
        {
            // 체크 이벤트에서 자식 노드들을 자동으로 체크하므로 재귀호출할 필요 없음
            foreach ( TreeNode root in treeView1.Nodes )
            {
                root.Checked = false;
            }
        }

        private void menu_SelectAll_Click( object sender, EventArgs e )
        {
            // 체크 이벤트에서 자식 노드들을 자동으로 체크하므로 재귀호출할 필요 없음
            foreach ( TreeNode root in treeView1.Nodes )
            {
                root.Checked = true;
            }
        }

        private void menu_Clear_Click( object sender, EventArgs e )
        {
            _fileList.Clear();
            treeView1.Nodes.Clear();
            //_nodeList.Clear();
            _recipeList.Clear();
            graph.ClearGraph( true );
            GC.Collect();
            AxisSetting.DataTypeInfo.SetColumnList( new RecipeType[0] );
        }

        private void menu_Export_Click( object sender, EventArgs e )
        {
            var checkedDatas = new List<RecipeData>();

            for ( var i = 0; i < _fileList.Count; i++ )
            {
                for ( var j = 0; j < _fileList[i].Count; j++ )
                {
                    if ( _fileList[i][j].Node.Checked && _fileList[i][j].Count != 0 )
                    {
                        checkedDatas.Add( _fileList[i][j] );
                    }
                }
            }

            if ( checkedDatas.Count == 0 )
            {
                MessageBox.Show( "선택된 레시피가 없습니다.", "Q730 알림 메시지" );
                return;
            }

            using ( var form = new Form_Export( checkedDatas ) )
            {
                form.ShowDialog();
            }
        }

        private void button_Save_Click( object sender, EventArgs e )
        {
            using ( var dialog = new SaveFileDialog()
            {
                Filter = "Q 데이터 뷰어 프리셋 파일(*.preset)|*.preset"
            } )
            {
                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    try
                    {
                        GraphSetting.Save( dialog.FileName );
                    }
                    catch
                    {
                        MessageBox.Show( "프리셋을 저장하는데 실패했습니다.", "Q730 알림 메시지" );
                    }
                }
            }
        }

        private void button_Load_Click( object sender, EventArgs e )
        {
            using ( var dialog = new OpenFileDialog()
            {
                Filter = "Q 데이터 뷰어 프리셋 파일(*.preset)|*.preset"
            } )
            {
                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    try
                    {
                        GraphSetting.Load( dialog.FileName );
                        textBox_Preset.Text = Path.GetFileName(dialog.FileName);
                    }
                    catch
                    {
                        MessageBox.Show( "프리셋을 불러오는데 실패했습니다.", "Q730 알림 메시지" );
                    }

                    propertyGrid1.Refresh();
                }
            }
        }

        private void Form_Main_FormClosing( object sender, FormClosingEventArgs e )
        {
            saveDefault();
        }

        private int _selectedTabIndex;
        private const int GRAPH = 0;
        private const int SHEET = 1;
    

        private void radioButtons_Click( object sender, EventArgs e )
        {
            var checkedData = new List<RecipeData>();
            radioButton_Graph.ForeColor = radioButton_Sheet.ForeColor = Color.FromArgb( 201, 201, 202 );

            if ( radioButton_Graph.Checked )
            {
                if ( _selectedTabIndex != GRAPH )
                {
                    tableLayoutPanel4.Controls.RemoveAt( 1 );
                    tableLayoutPanel4.Controls.Add( graph, 0, 1 );
                }

                _selectedTabIndex = GRAPH;

                radioButton_Graph.ForeColor = Color.Black;
            }
            else if ( radioButton_Sheet.Checked )
            {
                if (_selectedTabIndex != SHEET )
                {
                    tableLayoutPanel4.Controls.RemoveAt( 1 );
                    tableLayoutPanel4.Controls.Add( sheet, 0, 1 );

                    for (var i = 0; i < _fileList.Count; i++)
                    {
                        for (var j = 0; j < _fileList[i].Count; j++)
                        {
                            if (_fileList[i][j].Node.Checked && _fileList[i][j].Count != 0)
                            {
                                checkedData.Add(_fileList[i][j]);
                            }
                        }
                    }

                    sheet.ShowData(checkedData, graph);
                }

                _selectedTabIndex = SHEET;
                radioButton_Sheet.ForeColor = Color.Black;
            }
        }

        private void menu_DeleteFile_Click( object sender, EventArgs e )
        {
            if ( treeView1.SelectedNode.Parent != null ) return;

            var index = treeView1.SelectedNode.Index;
            _fileList.RemoveAt( index );
            treeView1.Nodes.RemoveAt( index );
        }

        private void treeView1_NodeMouseClick( object sender, TreeNodeMouseClickEventArgs e )
        {
            if(e.Button == MouseButtons.Right )
            {
                treeView1.SelectedNode = e.Node;

                if(e.Node.Parent == null )
                {
                    menu_Info.Visible = true;
                    toolStripSeparator4.Visible = true;
                }
                else
                {
                    menu_Info.Visible = false;
                    toolStripSeparator4.Visible = false;
                }
            }
        }

        private void menu_Info_Click( object sender, EventArgs e )
        {
            using(var form = new Form_FileInfo( _fileList[treeView1.SelectedNode.Index] ) )
            {
                form.ShowDialog();
            }
        }

        private void treeView1_AfterSelect( object sender, TreeViewEventArgs e )
        {
            if ( treeView1.SelectedNode == null ) return;
            if ( _selectedTabIndex != SHEET ) return;

            //sheet.ShowData( treeView1.SelectedNode.Tag as RecipeData );
        }

        private void button_Clear_Click( object sender, EventArgs e )
        {
            menu_Clear.PerformClick();
        }

        private void button_Export_Click( object sender, EventArgs e )
        {
            menu_Export.PerformClick();
        }
        private void button_MassGraph_Click(object sender, EventArgs e)
        {
            try
            {
                if (GraphSetting.XAxis.Data.SelectedData == DataType.None)
                {
                    MessageBox.Show("X축으로 사용할 데이터가 선택되지 않았습니다.");
                    return;
                }

                setGraphSetting();
             
                makeCurveMassArea(true);

                graph.graph.ZoomOutAll(graph.GraphPane);
                graph.RefreshGraph();

            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}", @"Sheet Page OverSize");
            }
        }

        private void button_AreaGraph_Click(object sender, EventArgs e)
        {
            try
            {
                if (GraphSetting.XAxis.Data.SelectedData == DataType.None)
                {
                    MessageBox.Show("X축으로 사용할 데이터가 선택되지 않았습니다.");
                    return;
                }

                setGraphSetting();
             
                makeCurveMassArea(false);

                graph.graph.ZoomOutAll(graph.GraphPane);
                graph.RefreshGraph();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}", @"Sheet Page OverSize");
            }
        }

    }
}
