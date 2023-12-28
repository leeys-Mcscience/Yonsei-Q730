using McQLib.Core;
using McQLib.Recipes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Q730.UserControls
{
    public enum ViewMode { Icon, Details }

    public partial class UserControl_RecipeItem : UserControl
    {
        public new event MouseEventHandler MouseDown
        {
            add
            {
                lock ( label_Content )
                {
                    label_Content.MouseDown += value;
                }
                lock ( pictureBox_RecipeIcon )
                {
                    pictureBox_RecipeIcon.MouseDown += value;
                }
            }
            remove
            {
                lock ( label_Content )
                {
                    label_Content.MouseDown -= value;
                }
                lock ( pictureBox_RecipeIcon )
                {
                    pictureBox_RecipeIcon.MouseDown -= value;
                }
            }
        }
        public new event MouseEventHandler MouseMove
        {
            add
            {
                lock ( label_Content )
                {
                    label_Content.MouseMove += value;
                }
                lock ( pictureBox_RecipeIcon )
                {
                    pictureBox_RecipeIcon.MouseMove += value;
                }
            }
            remove
            {
                lock ( label_Content )
                {
                    label_Content.MouseMove -= value;
                }
                lock ( pictureBox_RecipeIcon )
                {
                    pictureBox_RecipeIcon.MouseMove -= value;
                }
            }
        }
        public new event MouseEventHandler MouseUp
        {
            add
            {
                lock ( label_Content )
                {
                    label_Content.MouseUp += value;
                }
                lock ( pictureBox_RecipeIcon )
                {
                    pictureBox_RecipeIcon.MouseUp += value;
                }
            }
            remove
            {
                lock ( label_Content )
                {
                    label_Content.MouseUp -= value;
                }
                lock ( pictureBox_RecipeIcon )
                {
                    pictureBox_RecipeIcon.MouseUp -= value;
                }
            }
        }

        public static UserControl_RecipeItem DummyItem
        {
            get
            {
                var result = new UserControl_RecipeItem();
                result.Controls.Remove( result.tableLayoutPanel3 );
                result.BackColor = Color.FromArgb( 150, 150, 150 );
                // 더미 아이템 테두리 없애려면 아래 라인 삭제
                //result.BorderStyle = BorderStyle.FixedSingle;
                return result;
            }
        }
        public bool IsNew = false;
        private UserControl_RecipeItem()
        {
            InitializeComponent();
        }
        public UserControl_RecipeItem( Recipe recipe, bool iconMode = false ) : this()
        {
            if ( recipe == null ) throw new QException( QExceptionType.DEVELOP_NULL_REFERENCE_ERROR );
            _recipe = recipe;

            if ( !RecipeSetting.IsRecipeEnabled( recipe.GetRecipeType() ) )
            {
                Enabled = false;
                _recipe.Error = "사용할 수 없는 레시피입니다.";
            }

            pictureBox_RecipeIcon.Image = _recipe.Icon;

            if ( iconMode )
            {
                tableLayoutPanel3.ColumnStyles[1].Width = 40;
                Padding = new Padding( 0 );
            }
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
        public void RefreshError()
        {
            if ( _recipe.Error != null ) tableLayoutPanel3.BackColor = Color.OrangeRed;
        }

        public bool Now
        {
            get => _isNow;
            set
            {
                if ( _isNow == value ) return;

                if ( _isNow = value )
                {
                    Padding = new Padding( 3 );
                    BackColor = Color.FromArgb( 246, 170, 36 );
                }
                else
                {
                    Padding = new Padding( 1 );
                    BackColor = Color.FromArgb( 159, 160, 160 );
                }
            }
        }
        private bool _isNow;

        public bool Lock { get; set; }
        public bool Selected
        {
            get => _selected;
            set
            {
                if ( _selected = value )
                {
                    if ( _recipe.Error != null ) _recipe.Error = null;
                    tableLayoutPanel3.BackColor = Color.LightGreen;
                }
                else
                {
                    tableLayoutPanel3.BackColor = Color.WhiteSmoke;
                }
            }
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
        private bool _selected;

        private void label_Content_MouseHover( object sender, EventArgs e )
        {
            if ( !SoftwareConfiguration.SequenceBuilder.ShowDetailsToolTip ) return;

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
            if ( !SoftwareConfiguration.SequenceBuilder.ShowManualToolTip ) return;

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
