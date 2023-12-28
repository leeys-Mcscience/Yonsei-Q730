using McQLib.Core;
using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace Q730.UserControls.Graphs
{
    public partial class UserControl_DcGraph : UserControl, IGraphControl
    {
        public UserControl_DcGraph()
        {
            InitializeComponent();

            Application.Idle += application_Idle;
        }

        private void application_Idle(object sender, EventArgs e )
        {
            Application.Idle -= application_Idle;

            graph_Dc.GraphPane.Title.Text = "DC Graph";
            graph_Dc.GraphPane.XAxis.MajorGrid.IsVisible = true;

            graph_Dc.InitFontSpec( graph_Dc.GraphPane.Legend.FontSpec, Color.Black );

            graph_Dc.GraphPane.YAxisList.Clear();
            var y = graph_Dc.GraphPane.YAxisList.Add( "Voltage(V)" );
            graph_Dc.InitAxis( graph_Dc.GraphPane.YAxisList[y], Color.Blue );
            graph_Dc.GraphPane.YAxisList[y].MajorGrid.IsVisible = true;


            graph_Dc.GraphPane.Y2AxisList.Clear();
            var y2 = graph_Dc.GraphPane.Y2AxisList.Add( "Current(A)" );
            graph_Dc.InitAxis( graph_Dc.GraphPane.Y2AxisList[y2], Color.Red );

            graph_Dc.GraphPane.XAxis.Title.Text = "Time(sec)";
            graph_Dc.InitAxis( graph_Dc.GraphPane.XAxis, Color.Black );
            graph_Dc.GraphPane.XAxis.Scale.Min = 0;
            graph_Dc.GraphPane.XAxis.Scale.MaxAuto = true;


            voltageLine = graph_Dc.GraphPane.AddCurve( "Voltage", new PointPairList(), Color.Blue );
            voltageLine.Line.Width = 2.0f;
            voltageLine.IsY2Axis = false;
            voltageLine.Symbol.Type = SymbolType.None;

            currentLine = graph_Dc.GraphPane.AddCurve( "Current", new PointPairList(), Color.Red );
            currentLine.Line.Width = 2.0f;
            currentLine.IsY2Axis = true;
            currentLine.Symbol.Type = SymbolType.None;

            graph_Dc.RefreshGraph();
            Dock = DockStyle.Fill;
        }

        LineItem voltageLine;
        LineItem currentLine;

        public void AddData(MeasureData data)
        {
                voltageLine?.AddPoint(data.TotalTime / 1000.0, data.Voltage);
                currentLine?.AddPoint(data.TotalTime / 1000.0, data.Current);
        }
        public void AddData(MeasureData data, McQLib.Device.Channel channel)
        {

        }
        public void RefreshGraph()
        {
            graph_Dc.RefreshGraph();
        }
        public void ClearGraph()
        {
            voltageLine?.Clear();
            currentLine?.Clear();
            RefreshGraph();
        }
    }
}
