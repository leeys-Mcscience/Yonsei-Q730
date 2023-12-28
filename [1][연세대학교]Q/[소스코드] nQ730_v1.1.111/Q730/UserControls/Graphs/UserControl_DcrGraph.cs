using McQLib.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace Q730.UserControls.Graphs
{
    public partial class UserControl_DcrGraph : UserControl, IGraphControl
    {
        public UserControl_DcrGraph()
        {
            InitializeComponent();

            Application.Idle += application_Idle;
        }

        private void application_Idle( object sender, EventArgs e )
        {
            Application.Idle -= application_Idle;

            graph_VC.GraphPane.Title.Text = "DCR Plot";
            graph_VC.GraphPane.XAxis.MajorGrid.IsVisible = true;
            graph_VC.GraphPane.Legend.IsVisible = false;

            graph_VC.GraphPane.YAxisList.Clear();
            var y = graph_VC.GraphPane.YAxisList.Add( "Voltage(V)" );
            graph_VC.GraphPane.YAxisList[y].Scale.MinAuto = true;
            graph_VC.GraphPane.YAxisList[y].Scale.MaxAuto = true;
            graph_VC.GraphPane.YAxisList[y].Title.FontSpec.FontColor = Color.Blue;
            graph_VC.GraphPane.YAxisList[y].Title.FontSpec.IsBold = true;
            graph_VC.GraphPane.YAxisList[y].Scale.FontSpec.FontColor = Color.Blue;
            graph_VC.GraphPane.YAxisList[y].Scale.FontSpec.IsBold = true;
            graph_VC.GraphPane.YAxisList[y].MajorTic.IsOpposite = false;

            graph_VC.GraphPane.Y2AxisList.Clear();
            var y2 = graph_VC.GraphPane.Y2AxisList.Add( "Current(A)" );
            graph_VC.GraphPane.YAxisList[y2].Scale.MinAuto = true;
            graph_VC.GraphPane.YAxisList[y2].Scale.MaxAuto = true;
            graph_VC.GraphPane.YAxisList[y2].Title.FontSpec.FontColor = Color.Blue;
            graph_VC.GraphPane.YAxisList[y2].Title.FontSpec.IsBold = true;
            graph_VC.GraphPane.YAxisList[y2].Scale.FontSpec.FontColor = Color.Blue;
            graph_VC.GraphPane.YAxisList[y2].Scale.FontSpec.IsBold = true;
            graph_VC.GraphPane.YAxisList[y2].MajorTic.IsOpposite = false;

            graph_VC.GraphPane.XAxis.Title.Text = "Time(min)";
            graph_VC.GraphPane.XAxis.Scale.Min = 0;
            graph_VC.GraphPane.XAxis.Title.FontSpec.IsBold = true;
            graph_VC.GraphPane.XAxis.Scale.FontSpec.IsBold = true;

            //nyquistLine = graph_VC.GraphPane.AddCurve( "DCR Plot", new PointPairList(), Color.Blue );
            //nyquistLine.Line.Width = 2.0f;
            //nyquistLine.IsY2Axis = false;
            //nyquistLine.Symbol.Type = SymbolType.None;

            // ================================================================================================= //

            graph_Dcr.GraphPane.Title.Text = "DCR Plot";
            graph_Dcr.GraphPane.XAxis.MajorGrid.IsVisible = true;
            graph_Dcr.GraphPane.Legend.IsVisible = false;

            graph_Dcr.GraphPane.YAxisList.Clear();
            y = graph_Dcr.GraphPane.YAxisList.Add( "DCR(Ω)" );
            graph_Dcr.GraphPane.YAxisList[y].Scale.MinAuto = true;
            graph_Dcr.GraphPane.YAxisList[y].Scale.MaxAuto = true;
            graph_Dcr.GraphPane.YAxisList[y].Title.FontSpec.FontColor = Color.Blue;
            graph_Dcr.GraphPane.YAxisList[y].Title.FontSpec.IsBold = true;
            graph_Dcr.GraphPane.YAxisList[y].Scale.FontSpec.FontColor = Color.Blue;
            graph_Dcr.GraphPane.YAxisList[y].Scale.FontSpec.IsBold = true;
            graph_Dcr.GraphPane.YAxisList[y].MajorTic.IsOpposite = false;

            graph_Dcr.GraphPane.Y2AxisList.Clear();

            graph_Dcr.GraphPane.XAxis.Title.Text = "Count";
            graph_Dcr.GraphPane.XAxis.Scale.Min = 0;
            graph_Dcr.GraphPane.XAxis.Title.FontSpec.IsBold = true;
            graph_Dcr.GraphPane.XAxis.Scale.FontSpec.IsBold = true;

            Dock = DockStyle.Fill;
        }

        LineItem nyquistLine;

        public void AddData( MeasureData data ) //double time, double voltage, double current )
        {
            //nyquistLine?.AddPoint( data.Z_Real, data.Z_Img );
            //RefreshGraph();
        }

        public void AddData(MeasureData data, McQLib.Device.Channel channel)
        {

        }

        public void RefreshGraph()
        {
            //graph_Nyquist.RefreshGraph();
            //graph_Bode.RefreshGraph();
        }
        public void ClearGraph()
        {
            nyquistLine?.Clear();
            RefreshGraph();
        }
    }
}
