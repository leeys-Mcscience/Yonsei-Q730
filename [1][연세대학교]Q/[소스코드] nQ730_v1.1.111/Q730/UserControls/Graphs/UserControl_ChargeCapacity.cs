using McQLib.Core;
using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace Q730.UserControls.Graphs
{
    public partial class UserControl_ChargeCapacity : UserControl, IGraphControl
    {
        LineItem _chargeCapacityLine;

        public UserControl_ChargeCapacity()
        {
            InitializeComponent();

            Application.Idle += application_Idle;
        }

        private void application_Idle(object sender, EventArgs e )
        {
            Application.Idle -= application_Idle;

            graph_Ca.GraphPane.Title.Text = "Cycle / ChargeCapacity Graph";
            graph_Ca.GraphPane.XAxis.MajorGrid.IsVisible = true;

            graph_Ca.InitFontSpec(graph_Ca.GraphPane.Legend.FontSpec, Color.Black );

            graph_Ca.GraphPane.YAxisList.Clear();
            var y = graph_Ca.GraphPane.YAxisList.Add( "Charge Capacity(mAh)" );
            graph_Ca.InitAxis(graph_Ca.GraphPane.YAxisList[y], Color.Blue );
            graph_Ca.GraphPane.YAxisList[y].MajorGrid.IsVisible = true;

            graph_Ca.GraphPane.XAxis.Title.Text = "Cycle";
            graph_Ca.InitAxis(graph_Ca.GraphPane.XAxis, Color.Black );
            graph_Ca.GraphPane.XAxis.Scale.Min = 0;
            graph_Ca.GraphPane.XAxis.Scale.MinAuto = true;
            graph_Ca.GraphPane.XAxis.Scale.MaxAuto = true;
            graph_Ca.GraphPane.XAxis.Scale.MajorStep = 1;

            _chargeCapacityLine = graph_Ca.GraphPane.AddCurve("Charge Capacity", new PointPairList(), Color.Blue );
            _chargeCapacityLine.Line.Width = 2.0f;
            _chargeCapacityLine.IsY2Axis = false;
            _chargeCapacityLine.Symbol.Type = SymbolType.None;

            graph_Ca.RefreshGraph();
            Dock = DockStyle.Fill;
        }


        public void AddData(MeasureData data)
        {
               
        }

        public void AddData(MeasureData data, McQLib.Device.Channel channel)
        {
            if (data.RecipeType == McQLib.Recipes.RecipeType.Charge 
                || data.RecipeType == McQLib.Recipes.RecipeType.AnodeCharge)
            {
                var cycle =  data.StepCount / channel.Sequence.Count ;
                _chargeCapacityLine?.AddPoint(cycle + 1, data.Capacity * 1000);
            }
        }

        public void RefreshGraph()
        {
            graph_Ca.RefreshGraph();
        }
        public void ClearGraph()
        {
            _chargeCapacityLine?.Clear();
            RefreshGraph();
        }
    }
}
