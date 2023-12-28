using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DataViewer.Class;
using McQLib.Recipes;
using ZedGraph;

namespace DataViewer
{
    public partial class UserControl_Graph : UserControl
    {
        public GraphPane GraphPane => graph.GraphPane;
        public ZedGraphControl graph = new ZedGraphControl();

        private Sequence _sequence;
        public List<QDataManager> _fileList
        {
            get;
            set;
        }

        public GraphSetting _graphSetting;
        public UserControl_Graph(GraphSetting graphSetting, Sequence sequence)
        {
            InitializeComponent();
            _graphSetting = graphSetting;
            _sequence = sequence;
            Application.Idle += application_Idle;
        }

        private void application_Idle( object sender, EventArgs e )
        {
            Application.Idle -= application_Idle;
           
            Dock = DockStyle.Fill;
            graph.Dock = DockStyle.Fill;
            graph.Margin = new Padding( 0 );

            InitFontSpec( graph.GraphPane.Title.FontSpec, Color.Black );
            graph.GraphPane.TitleGap = 0;
            graph.ContextMenuStrip = contextMenuStrip1;
            //graph.PointValueEvent += Graph_PointValueEvent;
            graph.IsEnableVZoom = graph.IsEnableHZoom = true;
            

            Controls.Add( graph );
        }

        //private string Graph_PointValueEvent( ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt )
        //{
        //    var format = "f" + SoftwareConfiguration.Measurement.GraphDecimalPlace.ToString();
        //    return $"{curve[iPt].X.ToString( format )},{curve[iPt].Y.ToString( format )}";
        //}

        [Browsable( false )]
        public int CurveCount => graph.GraphPane.CurveList.Count;
        [Browsable( false )]
        public CurveItem this[int index] => graph.GraphPane.CurveList[index];

        public XAxis XAxis => graph.GraphPane.XAxis;

        public YAxis YAxis => graph.GraphPane.YAxis;

        public Y2Axis Y2Axis => graph.GraphPane.Y2Axis;

        public YAxisList YAxisList => graph.GraphPane.YAxisList;
        public Y2AxisList Y2AxisList => graph.GraphPane.Y2AxisList;

        public void InitFontSpec( FontSpec spec, Color color )
        {
            spec.Family = "맑은 고딕";
            spec.Size = 9.0f;
            spec.IsBold = true;
            spec.IsAntiAlias = false;
            spec.FontColor = color;
        }
        public void InitAxis( Axis axis, Color color )
        {
            axis.Title.Gap = 0;
            axis.Scale.LabelGap = 0.5f;

            InitFontSpec( axis.Title.FontSpec, color );
            InitFontSpec( axis.Scale.FontSpec, color );

            axis.MajorTic.IsOpposite = false;
            axis.MajorTic.IsOutside = false;
            axis.MinorTic.IsOpposite = false;
            axis.MinorTic.IsOutside = false;
            axis.MinorTic.IsAllTics = false;

            axis.Scale.MinAuto = true;
            axis.Scale.MaxAuto = true;

            axis.IsVisible = true;
        }


        public void RefreshGraph()
        {
            if ( graph.GraphPane.XAxis != null && graph.GraphPane.YAxisList.Count != 0 )
            {
                graph.AxisChange();
            }
            graph.Invalidate();
        }
        public void Add( string title, double[] x, double[] y, Color color, SymbolType symbolType, float width = 1.0f )
        {
            var item = new LineItem( title, x, y, color, symbolType, width );
            graph.GraphPane.CurveList.Add( item );
            RefreshGraph();
        }
        public void RemoveAt( int index )
        {
            graph.GraphPane.CurveList.RemoveAt( index );
            RefreshGraph();
        }
        public void ClearGraph( bool refresh = false )
        {
            graph.GraphPane.CurveList.Clear();
            if ( refresh ) RefreshGraph();
        }

        private void toolStripMenuItem_ShowPointValues_Click( object sender, EventArgs e )
        {
            if ( toolStripMenuItem_ShowPointValues.Checked ) toolStripMenuItem_ShowPointValues.Checked = graph.IsShowPointValues = false;
            else toolStripMenuItem_ShowPointValues.Checked = graph.IsShowPointValues = true;
        }
        private void toolStripMenuItem_SetScaleToDefault_Click( object sender, EventArgs e )
        {
            if ( graph.GraphPane.CurveList.Count == 0 ) return;

            graph.ZoomOutAll( graph.GraphPane );
            RefreshGraph();
        }
        private void toolStripMenuItem_ZoomEnable_Click( object sender, EventArgs e )
        {
            if ( toolStripMenuItem_ZoomEnable.Checked ) toolStripMenuItem_ZoomEnable.Checked = graph.IsEnableHZoom = graph.IsEnableVZoom = false;
            else toolStripMenuItem_ZoomEnable.Checked = graph.IsEnableVZoom = graph.IsEnableHZoom = true;
        }

        private void toolStripMenuItem_SaveAsImage_Click( object sender, EventArgs e )
        {
            graph.SaveAsBitmap();
        }
        double xDataLine;
        double yDataLine;
        double y2DataLine;
        private void toolStripMenuItem_Export_Click(object sender, EventArgs e)
        {
     
            var pane = graph.GraphPane;
            bool exportFinish = false;
            UserControl_ExportWindow userControlExportWindow = new UserControl_ExportWindow();

            userControlExportWindow.SetSeparateButtonEnabled(true);
            userControlExportWindow.SetTotalButtonEnabled(true);

            if (pane.XAxis == null || (pane.YAxisList.Count == 0 && pane.Y2AxisList.Count == 0) || pane.CurveList.Count == 0)
            {
                MessageBox.Show("그래프가 비었습니다.", "D730 알림 메시지");
                return;
            }

            if (XAxis.Title.Text.ToString() == "Cycle")
            {
                userControlExportWindow.SetSeparateButtonEnabled(false);
            }
            //if (_graphSetting.Y2DataType.ToString() != "None" && _graphSetting.YDataType.ToString() != "None")
            //{
            //    MessageBox.Show("Y1, Y2 두 항목에 대한 그래프는 일괄추출만 가능합니다.", "D730 알림 메시지");

            //    userControlExportWindow.SetSeparateButtonEnabled(false);
            //}

            using (var dialog = new SaveFileDialog() { Filter = "*.csv|*.csv" })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
            
                    userControlExportWindow.FormClosed += (s, args) =>
                    {
                        var worker = new BackgroundWorker();
                        var lines = new List<string>();
                        var index = 0;
                        const string pattern = @"\d+";
                        var checkedData = new List<RecipeData>();
                        var nodeCheckData = new List<int>();

                        if (userControlExportWindow.exportSignal) //사이클 구분하고 
                        {
                            worker.DoWork += (workerSender, workerArgs) =>
                            {
                                if (XAxis.Title.Text.ToString() == "Cycle")
                                {
                                    MessageBox.Show("X축 사이클은 일괄추출로 선택해주세요.", "Q730 알림 메시지");
                                    return;
                                }


                                for (var i = 0; i < _fileList.Count; i++)
                                {
                                    for (var j = 0; j < _fileList[i].Count; j++)
                                    {
                                        if (_fileList[i][j].Node.Checked && _fileList[i][j].Count != 0)
                                        {
                                            checkedData.Add(_fileList[i][j]);
                                            nodeCheckData.Add(i);
                                        }
                                    }
                                }
                                var line = new StringBuilder();
                                //lines.Add($"X : {pane.XAxis.Title.Text} // Y : {pane.YAxis.Title.Text}");
                                //line.Append(_graphSetting.Y2Axis.Data != DataType.None
                                //    ? $"{_graphSetting.XAxis.Data},{_graphSetting.YAxis.Data},{_graphSetting.Y2Axis.Data}\n"
                                //    : $"{_graphSetting.XAxis.Data},{_graphSetting.YAxis.Data}\n");


                                var maxIndex = checkedData.Max(x => x.Datas.Count);
                                StringBuilder firstLine = new StringBuilder();
                                lines.Add(line.ToString());
                                string LineText = null;
                                string extractedNumber = null;
                                int check = 0;
                                foreach (var data in checkedData)
                                {
                                    if (check != 0)
                                    {
                                        if (nodeCheckData[check] != nodeCheckData[check - 1])
                                        {
                                            firstLine.Append(",");
                                        }
                                    }

                                    LineText = data.Node.Parent.Text;
                                    Match match = Regex.Match(LineText, @"\d+");
                                    if (match.Success)
                                    {
                                        extractedNumber = match.Value;
                                    }

                                    firstLine.Append("(" + extractedNumber + ")" + _graphSetting.XAxis.Data.ToString());
                                    firstLine.Append(",");

                                    firstLine.Append(data.Node.Text + " " + _graphSetting.YAxis.Data);
                                    firstLine.Append(",");

                                    if (_graphSetting.Y2Axis.Data != DataType.None)
                                    {
                                        firstLine.Append(data.Node.Text + " " + _graphSetting.Y2Axis.Data);
                                        firstLine.Append(",");
                                    }

                                    check++;
                                }

                                LineText = firstLine.ToString();
                                lines.Add(LineText);


                                string xTitle = _graphSetting.XAxis.Data.ToString();
                                string YTitle = _graphSetting.YAxis.Data.ToString();
                                string Y2Title = null;
                                xDataLine = 0;
                                yDataLine = 0;
                                y2DataLine = 0;

                                line.Clear();

                                if (userControlExportWindow.checkTime)
                                {
                                    MessageBox.Show("시간 누적 기능은 일괄 추출에만 가능합니다.");
                                }
                                for (int i = 0; i < maxIndex; i++)
                                {
                                    for (int h = 0; h < checkedData.Count; h++)
                                    {
                                 
                                        if (h != 0)
                                        {
                                            if (nodeCheckData[h] != nodeCheckData[h - 1])
                                            {
                                                line.Append(",");
                                            }
                                        }
                                      

                                        if (i < checkedData[h].Datas.Count) // 현재 배열의 길이보다 작을 때만 데이터를 추가
                                        {
                                            switch (xTitle)
                                            {
                                                case "TotalTime":
                                                    xDataLine = checkedData[h].Datas[i].TotalTime;
                                                    break;

                                                case "StepTime":
                                                    xDataLine = checkedData[h].Datas[i].StepTime;
                                                    break;

                                                case "Voltage":
                                                    xDataLine = checkedData[h].Datas[i].Voltage;
                                                    break;

                                                case "Current":
                                                    xDataLine = checkedData[h].Datas[i].Current;
                                                    break;

                                                case "Capacity":
                                                    xDataLine = checkedData[h].Datas[i].Capacity;
                                                    break;

                                                case "Power":
                                                    xDataLine = checkedData[h].Datas[i].Power;
                                                    break;

                                                case "WattHour":
                                                    xDataLine = checkedData[h].Datas[i].WattHour;
                                                    break;
                                            }

                                            switch (YTitle)
                                            {
                                                case "Voltage":
                                                    yDataLine = checkedData[h].Datas[i].Voltage;
                                                    break;

                                                case "Current":
                                                    yDataLine = checkedData[h].Datas[i].Current;
                                                    break;

                                                case "Capacity":
                                                    yDataLine = checkedData[h].Datas[i].Capacity;
                                                    break;

                                                case "Power":
                                                    yDataLine = checkedData[h].Datas[i].Power;
                                                    break;

                                                case "WattHour":
                                                    yDataLine = checkedData[h].Datas[i].WattHour;
                                                    break;
                                            }

                                            if (_graphSetting.Y2Axis.Data != DataType.None)
                                            {
                                                Y2Title = _graphSetting.Y2Axis.Data.ToString();
                                                switch (Y2Title)
                                                {
                                                    case "Voltage":
                                                        y2DataLine = checkedData[h].Datas[i].Voltage;
                                                        break;

                                                    case "Current":
                                                        y2DataLine = checkedData[h].Datas[i].Current;
                                                        break;

                                                    case "Capacity":
                                                        y2DataLine = checkedData[h].Datas[i].Capacity;
                                                        break;

                                                    case "Power":
                                                        y2DataLine = checkedData[h].Datas[i].Power;
                                                        break;

                                                    case "WattHour":
                                                        y2DataLine = checkedData[h].Datas[i].WattHour;
                                                        break;
                                                }


                                                if (!_graphSetting.XMaxAuto && !_graphSetting.XMinAuto) //스케일 조절할떄 
                                                {
                                                    if (xDataLine <= _graphSetting.XMin && xDataLine >= _graphSetting.XMax)
                                                    {
                                                        xDataLine = 0;
                                                    }
                                                }

                                                if (!_graphSetting.YMaxAuto && !_graphSetting.YMinAuto) //스케일 조절할떄 
                                                {
                                                    if (yDataLine <= _graphSetting.YMin && yDataLine >= _graphSetting.YMax)
                                                    {
                                                        yDataLine = 0;
                                                    }
                                                }

                                                if (!_graphSetting.YMaxAuto && !_graphSetting.YMinAuto) //스케일 조절할떄 
                                                {
                                                    if (y2DataLine <= _graphSetting.YMin && y2DataLine >= _graphSetting.YMax)
                                                    {
                                                        y2DataLine = 0;
                                                    }
                                                }

                                                line.Append($"{xDataLine}, {yDataLine}, {y2DataLine},");

                                            }
                                            else
                                            {

                                                if (!_graphSetting.XMaxAuto && !_graphSetting.XMinAuto) //스케일 조절할떄 
                                                {
                                                    if (xDataLine <= _graphSetting.XMin && xDataLine >= _graphSetting.XMax)
                                                    {
                                                        xDataLine = 0;
                                                    }
                                                }

                                                if (!_graphSetting.YMaxAuto && !_graphSetting.YMinAuto) //스케일 조절할떄 
                                                {
                                                    if (yDataLine <= _graphSetting.YMin && yDataLine >= _graphSetting.YMax)
                                                    {
                                                        yDataLine = 0;
                                                    }
                                                }
                                              
                                                line.Append($"{xDataLine}, {yDataLine},");
                                            }
                                        }
                                        else // 현재 배열의 길이가 maxIndex보다 작으면 빈 값을 추가
                                        {
                                            if (_graphSetting.Y2Axis.Data != DataType.None)
                                            {
                                                line.Append(",,,"); // 빈 값 두 개 추가 (TotalTime과 Voltage)
                                            }
                                            else
                                            {
                                                line.Append(",,");
                                            }
                                        }
                                    }
                                    line.AppendLine();
                                }

                                LineText = line.ToString();
                                lines.Add(LineText);



                                File.WriteAllLines(dialog.FileName, lines, Encoding.UTF8);
                                exportFinish = true;
                            };

                            worker.RunWorkerCompleted += (workerSender, workerArgs) =>
                            {
                                if (exportFinish)
                                {
                                    MessageBox.Show("파일 내보내기가 완료되었습니다.", "D730 알림 메시지");
                                }
                              
                            };

                            worker.RunWorkerAsync();
                        }
                        else // 구분하지않고 바로 
                        {
                         
                            worker.DoWork += (workerSender, workerArgs) =>
                            {
                       
                                var line = new StringBuilder();

                                // 먼저 이 기능 : 그려진 그래프를 그대로 구분없이 출력한다.


                                // 다수의 예외처리 필요!! 장재훈

                                // 1) Y2가 없는 경우 예외처리.

                                // 2) 다수의 그래프 그려질때 처리 필요.

                                //line.Append($"The extracted data represents a batch of data from the drawn curve.\n");

                                //if (i == 0 && h == 0)
                                //{
                                //    interval = checkedData[0].Datas[2].TotalTime -
                                //                   checkedData[0].Datas[1].TotalTime;
                                //}

                                PointPair point = new PointPair();
                                PointPair point2 = new PointPair();
                                double xGap = 0;
                                double currentIndex = 0;
                                double totalGap = 0;
                                List<double> intervalList = new List<double>();
                                List<int> intervalIndex = new List<int>();

                                int ij = 0; // 루프 변수의 인덱스를 추적하기 위한 변수
                                foreach (var curve in pane.CurveList)
                                {
                                    if (ij % 2 == 0)
                                    {
                                        var jh = ij / 2;
                                        line.Append($"{_fileList[jh].Root.Text}\n");
                                        line.Append($"1) {_graphSetting.XAxis.Data},{_graphSetting.YAxis.Data}\n");
                                    }
                                    else
                                    {
                                        if (_graphSetting.Y2Axis.Data != DataType.None)
                                        {
                                            line.Append($"2) {_graphSetting.XAxis.Data},{_graphSetting.Y2Axis.Data}\n");
                                        }
                                    }

                                    if (userControlExportWindow.checkTime)
                                    {

                                        for (int dataCnt = 0; dataCnt < curve.Points.Count; dataCnt++)
                                        {
                                            if (curve.Points[dataCnt] != null)
                                            {
                                                point = curve.Points[dataCnt];

                                                if (dataCnt != curve.Points.Count - 1)
                                                {
                                                    point2 = curve.Points[dataCnt + 1];

                                                    intervalList.Add(Convert.ToInt32(point2.X - point.X)); //인터벌 간격을 모두 저장 한다.
                                                    intervalIndex.Add(dataCnt);
                                                }
                                            }
                                        }

                                        var gapList = intervalList
                                            .Select((value, ds) => new { Value = value, Index = ds })
                                            .Where(item => Math.Abs(item.Value - intervalList[0]) > double.Epsilon && item.Value > 0)
                                            .ToList();

                                        for (int dataCnt = 0; dataCnt < curve.Points.Count; dataCnt++)
                                        {
                                            if (curve.Points[dataCnt] != null)
                                            {
                                                point = curve.Points[dataCnt];

                                                foreach (var gapItem in gapList)
                                                {
                                                    if (curve.Points[dataCnt] != null && gapItem.Index == dataCnt)
                                                    {
                                                        currentIndex = gapItem.Index;

                                                        totalGap += gapItem.Value;
                                                        
                                                    }

                                                    if (currentIndex != 0)
                                                    {
                                                        currentIndex = gapItem.Index;
                                                    }
                                                }


                                                if (totalGap > 0)
                                                {
                                                    line.Append($"{point.X - totalGap},{point.Y}\n");
                                                }
                                                else
                                                {
                                                    line.Append($"{point.X},{point.Y}\n");

                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int dataCnt = 0; dataCnt < curve.Points.Count; dataCnt++)
                                        {
                                            if (curve.Points[dataCnt] != null)
                                            {
                                                point = curve.Points[dataCnt];
                                                line.Append($"{point.X},{point.Y}\n");
                                            }
                                        }
                                    }
                              
                                    line.Append($"\n");
                                    ij++;
                                    // CurveList 갯수가 다수인경우 // 커브별로 저장한다.
                                }
                                lines.Add(line.ToString());
                                File.WriteAllLines(dialog.FileName, lines, Encoding.UTF8);
                                exportFinish = true;
                            };

                            worker.RunWorkerCompleted += (workerSender, workerArgs) =>
                            {
                                if (exportFinish)
                                {
                                    MessageBox.Show("파일 내보내기가 완료되었습니다.", "D730 알림 메시지");
                                }
                            };

                            worker.RunWorkerAsync();
                        }
                     
                    };
                    userControlExportWindow.ShowDialog();

                }
            }
        }
    }
}
