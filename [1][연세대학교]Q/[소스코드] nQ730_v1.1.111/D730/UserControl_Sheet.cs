using DataViewer.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
//using //Q730.Data;
using ZedGraph;

namespace DataViewer
{
    public partial class UserControl_Sheet : UserControl
    {
        public UserControl_Sheet()
        {
            InitializeComponent();
            Application.Idle += application_Idle;
        }

        private void application_Idle( object sender, EventArgs e )
        {
            Application.Idle -= application_Idle;

            dataGridView1.GetType().GetProperty( "DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance )?.SetValue( dataGridView1, true );
            Dock = DockStyle.Fill;

        }

        public void ShowData( List<RecipeData> recipeDatas, UserControl_Graph graphData)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.ReadOnly = true;

            if (recipeDatas == null ) return;
            var pane = graphData.GraphPane;

            try
            {
                if (pane.XAxis.Title.Text.Contains("Total Time (min)"))
                {

                    MakeColumns($"X ({pane.XAxis.Title.Text})", $"Y ({pane.YAxis.Title.Text})");
                }
                foreach (var curve in pane.CurveList)
                {

                    if (curve.Points.Count > 0 && !pane.XAxis.Title.Text.Contains("Total Time (min)"))
                    {
                        MakeColumns($"X : {pane.XAxis.Title.Text})", $"Y : {curve.Label.Text})");

                        for (var dataCnt = 0; dataCnt < curve.Points.Count; dataCnt++)
                        {
                            if (curve.Points.Count > 0 && curve.Points[dataCnt] != null)
                            {
                                var point = curve.Points[dataCnt];

                                if (dataGridView1.Rows.Count < dataCnt + 1)
                                {
                                    dataGridView1.Rows.Add("", "");
                                }

                                dataGridView1.Rows[dataCnt].Cells[dataGridView1.ColumnCount - 2].Value = point.X;
                                dataGridView1.Rows[dataCnt].Cells[dataGridView1.ColumnCount - 1].Value = point.Y;

                            }
                        }
                    }
                    else if(pane.XAxis.Title.Text.Contains("Total Time (min)"))
                    {
                        for (var dataCnt = 0; dataCnt < curve.Points.Count; dataCnt++)
                        {
                            if (curve.Points.Count > 0 && curve.Points[dataCnt] != null)
                            {
                                var point = curve.Points[dataCnt];
                                dataGridView1.Rows.Add(point.X, point.Y);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(@"Data 허용량이 넘어섰습니다.", @"Sheet Page OverSize");
            }
        }

        private void MakeColumns( params object[] columnHeaders )
        {
            for ( var i = 0; i < columnHeaders.Length; i++ )
            {
                dataGridView1.Columns[dataGridView1.Columns.Add( $"column{i}", columnHeaders[i].ToString() )].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
    }
}
