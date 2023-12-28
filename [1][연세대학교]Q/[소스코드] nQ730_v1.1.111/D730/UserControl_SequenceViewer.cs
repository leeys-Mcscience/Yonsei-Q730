using McQLib.Recipes;
using System;
using System.Windows.Forms;

namespace DataViewer
{
    public partial class UserControl_SequenceViewer : UserControl
    {
        public UserControl_SequenceViewer()
        {
            InitializeComponent();
        }

        Sequence _sequence = null;

        public void SetSequence( Sequence sequence )
        {
            flowLayoutPanel1.Controls.Clear();

            //if( (_sequence = sequence) == null )
            //{
            //    return;
            //}
            _sequence = sequence;

            if( _sequence != null )
            {
                for( var i = 0; i < _sequence.Count; i++ )
                {
                    var item = new UserControl_RecipeItem( _sequence[i] );
                    if ( _sequence[i].GetRecipeType() == RecipeType.Label ) item.SetToLabel();
                    flowLayoutPanel1.Controls.Add( item );
                    item.RefreshText();
                }
            }

            Application.DoEvents();

            resize();
        }
        public Sequence GetSequence()
        {
            return _sequence;
        }

        private void resize()
        {
            Application.DoEvents();

            var totalHeight = 0;
            foreach( Control item in flowLayoutPanel1.Controls ) totalHeight += item.Height + item.Margin.Top + item.Margin.Bottom;

            if( totalHeight > flowLayoutPanel1.Height )
            {
                foreach( Control item in flowLayoutPanel1.Controls )
                {
                    item.Width = flowLayoutPanel1.Width - 23;
                }
            }
            else
            {
                foreach( Control item in flowLayoutPanel1.Controls )
                {
                    item.Width = flowLayoutPanel1.Width - item.Margin.Left - item.Margin.Right - 2;
                }
            }
        }

        private void UserControl_SequenceViewer_ClientSizeChanged( object sender, EventArgs e )
        {
            resize();
        }
    }
}
