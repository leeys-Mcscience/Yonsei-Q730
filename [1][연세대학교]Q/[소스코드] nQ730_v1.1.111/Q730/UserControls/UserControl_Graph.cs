using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace Q730.UserControls
{
    public partial class UserControl_Graph : UserControl
    {
        public GraphPane GraphPane => graph.GraphPane;
        ZedGraphControl graph = new ZedGraphControl();

        public UserControl_Graph()
        {
            InitializeComponent();

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
            graph.PointValueEvent += Graph_PointValueEvent;
            graph.IsEnableVZoom = graph.IsEnableHZoom = true;

            Controls.Add( graph );
        }

        private string Graph_PointValueEvent( ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt )
        {
            var format = "f" + SoftwareConfiguration.Measurement.GraphDecimalPlace.ToString();
            return $"{curve[iPt].X.ToString( format )},{curve[iPt].Y.ToString( format )}";
        }

        [Browsable( false )]
        public int CurveCount => graph.GraphPane.CurveList.Count;
        [Browsable( false )]
        public CurveItem this[int index] => graph.GraphPane.CurveList[index];

        public XAxis XAxis => graph.GraphPane.XAxis;
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

        public void AddYAxis( string title )
        {
            var axis = createAxis( title ) as YAxis;
            graph.GraphPane.YAxisList.Add( axis );
            axis.IsVisible = true;
        }
        public void AddY2Axis( string title )
        {
            var axis = createAxis( title, true ) as Y2Axis;
            graph.GraphPane.Y2AxisList.Add( axis );
            axis.IsVisible = true;
        }
        public void AddCurve()
        {

        }
        public void AddPoint()
        {

        }
        private Axis createAxis( string title, bool isY2 = false )
        {
            Axis axis = null;
            if ( isY2 ) axis = new Y2Axis( title );
            else axis = new YAxis( title );

            axis.MajorTic.IsOpposite = false;
            axis.MinorTic.IsAllTics = false;
            axis.Title.Gap = 0;
            axis.Scale.LabelGap = 0;
            return axis;
        }

        public void RefreshGraph()
        {
            try
            {
                if ( graph.GraphPane.XAxis != null && graph.GraphPane.YAxisList.Count != 0 )
                {
                    graph.AxisChange();
                }
                graph.Invalidate();
            }
            catch
            {

            }
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
    }
}
