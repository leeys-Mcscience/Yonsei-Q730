using System;
using System.Collections.Generic;
using System.Windows.Forms;
using McQLib.Core;
using McQLib.NotUsed.Recipes;

namespace Q730.NotUsed
{
    public partial class UserControl_ParameterBox : UserControl
    {
        public UserControl_ParameterBox()
        {
            InitializeComponent();
            DoubleBuffered = true;
            flowLayoutPanel1.GetType().GetProperty( "DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance ).SetValue( flowLayoutPanel1, true );
        }

        //public IRecipe Recipe
        //{
        //    get => _recipe;
        //    set
        //    {
        //        _recipe = value;
        //        showParameters();
        //        ResizeControl();
        //    }
        //}

        private  List<IRecipe> _recipes = new List<IRecipe>();

        public IRecipe GetRecipe(int index )
        {
            if( index < 0 || index >= _recipes.Count ) return null;

            return _recipes[index];
        }
        public void AddRecipe(IRecipe recipe )
        {
            if(_recipes.IndexOf(recipe) == -1)
            {
                _recipes.Add( recipe );
                showParameters();
                ResizeControl();
            }
        }
        public void RemoveRecipe(IRecipe recipe )
        {
            if(_recipes.IndexOf(recipe) != -1 )
            {
                _recipes.Remove( recipe );
                showParameters();
                ResizeControl();
            }
        }
        public void ClearRecipes()
        {
            _recipes.Clear();
            showParameters();
            ResizeControl();
        }

        private void showParameters()
        {
            flowLayoutPanel1.Controls.Clear();
            label_Name.Text = label_Help.Text = "";

            if( _recipes.Count == 0 ) return;

            // ParameterBox에 추가된 레시피 목록에서 공통된 파라미터들을 뽑는다.
            // FieldInfo를 비교하여 같은 파라미터들만 추가해야 함.

            var recipeInfo = RecipeFactory.GetRecipeInfo( _recipes.ToArray() );
            //var recipeInfo = _recipe.GetRecipeInfo();

            for( var i = 0; i < recipeInfo.GroupCount; i++ )
            {
                flowLayoutPanel1.Controls.Add( new UserControl_ParameterItem( recipeInfo[i] )
                {
                    Width = Width - 2
                } );

                for( var j = 0; j < recipeInfo[i].ParameterCount; j++ )
                {
                    flowLayoutPanel1.Controls.Add( new UserControl_ParameterItem( recipeInfo[i][j], _recipes.ToArray() )
                    {
                        Width = Width - 2,
                        HelpLabel = label_Help,
                        NameLabel = label_Name
                    } );
                }
            }
        }

        //public void Save()
        //{
        //    return;
        //    //if( Recipe == null ) return;
        //    if( _recipes.Count == 0 ) return;

        //    foreach( UserControl_ParameterItem item in flowLayoutPanel1.Controls )
        //    {
        //        foreach( var r in _recipes )
        //        {
        //            if( item.ItemType == ItemType.Group ) continue;

        //            try
        //            {
        //                item.FieldInfo.SetValue( r, item.Value );
        //            }
        //            catch( ArgumentException )
        //            {
        //                var obj = item.Value;

        //                if( obj == null ) item.FieldInfo.SetValue( r, null );
        //                else item.FieldInfo.SetValue( r, Convert.ToUInt32( obj ) );
        //            }
        //        }
        //    }
        //}
        public bool CanSave(out string msg)
        {
            msg = null;
            if( _recipes.Count == 0 ) return true;
            //if( Recipe == null ) return true;

            foreach( var r in _recipes )
            {
                IRecipe clone = RecipeFactory.CreateInstance( r.GetRecipeType() );
                foreach( UserControl_ParameterItem item in flowLayoutPanel1.Controls )
                {
                    if( item.ItemType == ItemType.Group ) continue;

                    try
                    {
                        item.FieldInfo.SetValue( clone, item.Value );
                    }
                    catch( ArgumentException )
                    {
                        var obj = item.Value;

                        if( obj == null ) item.FieldInfo.SetValue( clone, null );
                        else item.FieldInfo.SetValue( clone, Convert.ToUInt32( obj ) );
                    }
                    catch( QException ex )
                    {
                        msg = ex.Message;
                        return false;
                    }
                }
            }

            return true;
        }

        public void ResizeControl()
        {
            var totalHeight = 0;
            foreach( var c in flowLayoutPanel1.Controls )
            {
                totalHeight += (c as Control).Bounds.Height;
            }

            if( totalHeight > flowLayoutPanel1.Bounds.Height )
            {
                foreach( var c in flowLayoutPanel1.Controls )
                {
                    (c as Control).Width = Bounds.Width - 19;
                }
            }
            else
            {
                foreach( var c in flowLayoutPanel1.Controls )
                {
                    (c as Control).Width = Bounds.Width - 2;
                }
            }
        }

        private void flowLayoutPanel1_ControlChanged( object sender, ControlEventArgs e )
        {
            //resizeControl();
        }
    }
}
