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
    public partial class UserControl_FraAcrGraph : UserControl, IGraphControl
    {
        public UserControl_FraAcrGraph()
        {
            InitializeComponent();

            Application.Idle += application_Idle;
        }

        private void application_Idle( object sender, EventArgs e )
        {
            Application.Idle -= application_Idle;

            graph_Nyquist.GraphPane.Title.Text = "Nyquist Plot";
            graph_Nyquist.GraphPane.XAxis.MajorGrid.IsVisible = true;
            graph_Nyquist.GraphPane.Legend.IsVisible = false;

            graph_Nyquist.GraphPane.YAxisList.Clear();
            var y = graph_Nyquist.GraphPane.YAxisList.Add( "-Image_Z''(Ω)" );
            graph_Nyquist.GraphPane.YAxisList[y].Scale.MinAuto = true;
            graph_Nyquist.GraphPane.YAxisList[y].Scale.MaxAuto = true;
            graph_Nyquist.GraphPane.YAxisList[y].Title.FontSpec.FontColor = Color.Blue;
            graph_Nyquist.GraphPane.YAxisList[y].Title.FontSpec.IsBold = true;
            graph_Nyquist.GraphPane.YAxisList[y].Scale.FontSpec.FontColor = Color.Blue;
            graph_Nyquist.GraphPane.YAxisList[y].Scale.FontSpec.IsBold = true;
            graph_Nyquist.GraphPane.YAxisList[y].MajorTic.IsOpposite = false;

            graph_Nyquist.GraphPane.Y2AxisList.Clear();

            graph_Nyquist.GraphPane.XAxis.Title.Text = "Real_Z'(Ω)";
            graph_Nyquist.GraphPane.XAxis.Scale.Min = 0;
            graph_Nyquist.GraphPane.XAxis.Title.FontSpec.IsBold = true;
            graph_Nyquist.GraphPane.XAxis.Scale.FontSpec.IsBold = true;

            // ================================================================================================= //

            graph_Bode.GraphPane.Title.Text = "Bode Plot";
            graph_Bode.GraphPane.XAxis.MajorGrid.IsVisible = true;
            graph_Bode.GraphPane.Legend.IsVisible = false;

            graph_Bode.GraphPane.YAxisList.Clear();
            y = graph_Bode.GraphPane.YAxisList.Add( "ABS(Z)" );
            graph_Bode.GraphPane.YAxisList[y].Scale.MinAuto = true;
            graph_Bode.GraphPane.YAxisList[y].Scale.MaxAuto = true;
            graph_Bode.GraphPane.YAxisList[y].Title.FontSpec.FontColor = Color.Blue;
            graph_Bode.GraphPane.YAxisList[y].Title.FontSpec.IsBold = true;
            graph_Bode.GraphPane.YAxisList[y].Scale.FontSpec.FontColor = Color.Blue;
            graph_Bode.GraphPane.YAxisList[y].Scale.FontSpec.IsBold = true;
            graph_Bode.GraphPane.YAxisList[y].MajorTic.IsOpposite = false;

            graph_Bode.GraphPane.Y2AxisList.Clear();
            var y2 = graph_Bode.GraphPane.Y2AxisList.Add( "Phase(Rad)" );
            graph_Bode.GraphPane.YAxisList[y2].Scale.MinAuto = true;
            graph_Bode.GraphPane.YAxisList[y2].Scale.MaxAuto = true;
            graph_Bode.GraphPane.YAxisList[y2].Title.FontSpec.FontColor = Color.Blue;
            graph_Bode.GraphPane.YAxisList[y2].Title.FontSpec.IsBold = true;
            graph_Bode.GraphPane.YAxisList[y2].Scale.FontSpec.FontColor = Color.Blue;
            graph_Bode.GraphPane.YAxisList[y2].Scale.FontSpec.IsBold = true;
            graph_Bode.GraphPane.YAxisList[y2].MajorTic.IsOpposite = false;

            graph_Bode.GraphPane.XAxis.Title.Text = "Freq(Hz)";
            graph_Bode.GraphPane.XAxis.Scale.Min = 0;
            graph_Bode.GraphPane.XAxis.Title.FontSpec.IsBold = true;
            graph_Bode.GraphPane.XAxis.Scale.FontSpec.IsBold = true;

            Dock = DockStyle.Fill;
        }

        LineItem nyquistLine;

        private LineItem createLine(string name, Color color)
        {
            var line = graph_Nyquist.GraphPane.AddCurve( name, new PointPairList(), color );
            line.Line.Width = 2.0f;
            line.IsY2Axis = false;
            line.Symbol.Type = SymbolType.None;

            return line;
        }
        public void AddData( MeasureData data ) //double time, double voltage, double current )
        {
            nyquistLine?.AddPoint( data.Z_Real, data.Z_Img );
            //RefreshGraph();
        }
        public void AddData(MeasureData data, McQLib.Device.Channel channel)
        {

        }

        public void RefreshGraph()
        {
            graph_Nyquist.RefreshGraph();
            graph_Bode.RefreshGraph();
        }
        public void ClearGraph()
        {
            nyquistLine?.Clear();
            RefreshGraph();
        }
    }
}
