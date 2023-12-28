using McQLib.Core;
using McQLib.Recipes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataViewer
{
    public enum ViewMode { Icon, Details }

    public partial class UserControl_RecipeItem : UserControl
    {
        private UserControl_RecipeItem()
        {
            InitializeComponent();
        }
        public UserControl_RecipeItem( Recipe recipe ) : this()
        {
            if ( recipe == null ) throw new QException( QExceptionType.DEVELOP_NULL_REFERENCE_ERROR );
            _recipe = recipe;

            pictureBox_RecipeIcon.Image = _recipe.Icon;
        }

        public void SetToLabel()
        {
            tableLayoutPanel3.ColumnStyles[1].Width = 0;
            Height = 25;
        }
        public void RefreshText()
        {
            if ( _recipe == null ) return;

            try
            {
                string text;
                if ( _recipe.Error != null ) text = _recipe.Error;
                else text = _recipe.GetSummaryString();

                if ( text != label_Content.Text ) label_Content.Text = text;
            }
            catch { }
        }

        public bool Indentation
        {
            get => _indentation;
            set
            {
                if ( _indentation = value )
                {
                    Margin = new Padding( 20, 2, 2, 2 );
                    //tableLayoutPanel3.ColumnStyles[0].Width = 20;
                }
                else
                {
                    Margin = new Padding( 2 );
                    //tableLayoutPanel3.ColumnStyles[0].Width = 0;
                }
            }
        }
        public Recipe Recipe => _recipe;

        private bool _indentation;
        private Recipe _recipe;

        private void label_Content_MouseHover( object sender, EventArgs e )
        {
            //if ( !SoftwareConfiguration.SequenceBuilder.ShowDetailsToolTip ) return;

            try
            {
                var str = _recipe.GetDetailString();

                if ( str != string.Empty )
                {
                    toolTip_Detail.SetToolTip( label_Content, str );
                }
            }
            catch ( NotImplementedException ) { }
        }

        private void pictureBox_RecipeIcon_MouseHover( object sender, EventArgs e )
        {
            //if ( !SoftwareConfiguration.SequenceBuilder.ShowManualToolTip ) return;

            try
            {
                var str = _recipe.GetManualString();

                if ( str != string.Empty )
                {
                    toolTip_Manual.ToolTipTitle = _recipe.Name;
                    toolTip_Manual.SetToolTip( pictureBox_RecipeIcon, str );
                }
            }
            catch ( NotImplementedException ) { }
        }
    }
}
