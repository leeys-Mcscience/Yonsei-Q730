using McQLib.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace Q730.UserControls.Graphs
{
    public partial class UserControl_DisChargeCapacity : UserControl, IGraphControl
    {
        LineItem _disChargeCapacityLine;
        public UserControl_DisChargeCapacity()
        {
            InitializeComponent();

            Application.Idle += application_Idle;
        }

        private void application_Idle(object sender, EventArgs e )
        {
            Application.Idle -= application_Idle;

            graph_DisCa.GraphPane.Title.Text = "Cycle / DisChargeCapacity Graph";
            graph_DisCa.GraphPane.XAxis.MajorGrid.IsVisible = true;

            graph_DisCa.InitFontSpec(graph_DisCa.GraphPane.Legend.FontSpec, Color.Black );

            graph_DisCa.GraphPane.YAxisList.Clear();
            var y = graph_DisCa.GraphPane.YAxisList.Add( "DisCharge Capacity(mAh)" );
            graph_DisCa.InitAxis(graph_DisCa.GraphPane.YAxisList[y], Color.Red );
            graph_DisCa.GraphPane.YAxisList[y].MajorGrid.IsVisible = true;

            graph_DisCa.GraphPane.XAxis.Title.Text = "Cycle";
            graph_DisCa.InitAxis(graph_DisCa.GraphPane.XAxis, Color.Black );
            graph_DisCa.GraphPane.XAxis.Scale.Min = 0;
            graph_DisCa.GraphPane.XAxis.Scale.MinAuto = true;
            graph_DisCa.GraphPane.XAxis.Scale.MaxAuto = true;
            graph_DisCa.GraphPane.XAxis.Scale.MajorStep = 1;

            _disChargeCapacityLine = graph_DisCa.GraphPane.AddCurve("DisCharge Capacity", new PointPairList(), Color.Red);
            _disChargeCapacityLine.Line.Width = 2.0f;
            _disChargeCapacityLine.IsY2Axis = true;
            _disChargeCapacityLine.Symbol.Type = SymbolType.None;

            graph_DisCa.RefreshGraph();
            Dock = DockStyle.Fill;
        }


        public void AddData(MeasureData data)
        {

        }
       
        public void AddData(MeasureData data, McQLib.Device.Channel channel)
        {
            if(data.RecipeType == McQLib.Recipes.RecipeType.Discharge
                || data.RecipeType == McQLib.Recipes.RecipeType.AnodeDischarge)
            {
                var cycle = data.StepCount / channel.Sequence.Count;
                _disChargeCapacityLine?.AddPoint(cycle + 1, data.Capacity * 1000);
            }
        }

        public void RefreshGraph()
        {
            graph_DisCa.RefreshGraph();
        }
        public void ClearGraph()
        {
            _disChargeCapacityLine?.Clear();
            RefreshGraph();
        }
    }
}
