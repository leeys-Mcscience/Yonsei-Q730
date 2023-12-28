using McQLib.Core;
using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace Q730.UserControls.Graphs
{
    public partial class UserControl_PatternGraph : UserControl, IGraphControl
    {
        public UserControl_PatternGraph()
        {
            InitializeComponent();
            Application.Idle += application_Idle;
        }

        private void application_Idle( object sender, EventArgs e )
        {
            Application.Idle -= application_Idle;

            graph_Pattern.GraphPane.Title.Text = "Pattern Graph";
            graph_Pattern.GraphPane.XAxis.MajorGrid.IsVisible = true;

            graph_Pattern.InitFontSpec( graph_Pattern.GraphPane.Legend.FontSpec, Color.Black );

            graph_Pattern.GraphPane.YAxisList.Clear();
            var y = graph_Pattern.GraphPane.YAxisList.Add( "Voltage(V)" );
            graph_Pattern.InitAxis( graph_Pattern.GraphPane.YAxisList[y], Color.Blue );
            graph_Pattern.GraphPane.YAxisList[y].MajorGrid.IsVisible = true;


            graph_Pattern.GraphPane.Y2AxisList.Clear();
            var y2 = graph_Pattern.GraphPane.Y2AxisList.Add( "Current(A)" );
            graph_Pattern.InitAxis( graph_Pattern.GraphPane.Y2AxisList[y2], Color.Red );

            graph_Pattern.GraphPane.XAxis.Title.Text = "Time(sec)";
            graph_Pattern.InitAxis( graph_Pattern.GraphPane.XAxis, Color.Black );
            graph_Pattern.GraphPane.XAxis.Scale.Min = 0;
            graph_Pattern.GraphPane.XAxis.Scale.MaxAuto = true;

            voltageLine = graph_Pattern.GraphPane.AddCurve( "Voltage", new PointPairList(), Color.Blue );
            voltageLine.Line.Width = 2.0f;
            voltageLine.IsY2Axis = false;
            voltageLine.Symbol.Type = SymbolType.None;

            currentLine = graph_Pattern.GraphPane.AddCurve( "Current", new PointPairList(), Color.Red );
            currentLine.Line.Width = 2.0f;
            currentLine.IsY2Axis = true;
            currentLine.Symbol.Type = SymbolType.None;

            graph_Pattern.RefreshGraph();
            Dock = DockStyle.Fill;
        }

        LineItem voltageLine;
        LineItem currentLine;

        public void AddData( MeasureData data )
        {
            voltageLine?.AddPoint( data.TotalTime / 1000.0, data.Voltage );
            currentLine?.AddPoint( data.TotalTime / 1000.0, data.Current );
        }
        public void AddData(MeasureData data, McQLib.Device.Channel channel)
        {

        }

        public void RefreshGraph()
        {
            graph_Pattern.RefreshGraph();
        }
        public void ClearGraph()
        {
            voltageLine?.Clear();
            currentLine?.Clear();
            RefreshGraph();
        }
    }
}
