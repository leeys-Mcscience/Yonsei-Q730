using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using McQLib.Core;
using McQLib.NotUsed.Core;
using McQLib.NotUsed.Recipes;

namespace Q730.NotUsed
{
    public enum ItemType
    {
        Group,
        Parameter
    }

    public partial class UserControl_ParameterItem : UserControl
    {
        private UserControl_ParameterItem()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Height = 23;
        }

        List<Control> _box;
        bool _isFolded;
        bool _isGroup;
        Control _valueControl;
        CheckBox _nullControl;
        RecipeInfo.Group.Parameter _parameter;
        IRecipe[] _recipes;

        public Label HelpLabel, NameLabel;
        public ItemType ItemType { get; }
        public FieldInfo FieldInfo => _parameter.FieldInfo;
        public object Value
        {
            get
            {
                if( ItemType == ItemType.Group ) throw new QException( QExceptionType.DEVELOP_TRY_READING_FROM_GROUP_ITEM_ERROR );

                if( _parameter.IsAllowNull && _parameter.ParameterValueType != ParameterValueType.Pattern && !_nullControl.Checked ) return null;

                switch( _parameter.ParameterValueType )
                {
                    case ParameterValueType.Integer:
                        if( _valueControl.Text.Trim() == "" ) return 0;
                        else if( uint.TryParse( _valueControl.Text.Trim(), out uint itemp ) ) return itemp;
                        else throw new QException( QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR );

                    case ParameterValueType.Double:
                        if( _valueControl.Text.Trim() == "" ) return 0.0;
                        else if( double.TryParse( _valueControl.Text.Trim(), out double dtemp ) ) return dtemp;
                        else throw new QException( QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR );

                    case ParameterValueType.Float:
                        if( _valueControl.Text.Trim() == "" ) return 0.0;
                        else if( float.TryParse( _valueControl.Text.Trim(), out float ftemp ) ) return ftemp;
                        else throw new QException( QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR );

                    case ParameterValueType.String:
                        return _valueControl.Text;

                    case ParameterValueType.Time:
                        return (_valueControl as UserControl_TimeBox).Time.TotalMilliseconds;

                    case ParameterValueType.Enum:
                        try
                        {
                            return Enum.Parse( _parameter.FieldInfo.FieldType, _valueControl.Text );
                        }
                        catch
                        {
                            return Enum.Parse( _parameter.FieldInfo.FieldType, (_valueControl as ComboBox).Items[0].ToString() );
                        }

                    case ParameterValueType.Boolean:
                        if( _valueControl.Text == "True" ) return true;
                        else if( _valueControl.Text == "False" ) return false;
                        else throw new QException( QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR );

                    case ParameterValueType.Pattern:
                        return _valueControl.Text;
                }

                throw new QException( QExceptionType.UNDEFINED_ERROR );
            }
        }

        public UserControl_ParameterItem( RecipeInfo.Group group ) : this()
        {
            var label = new Label()
            {
                Font = new Font( "맑은 고딕", 10, FontStyle.Bold ),
                ForeColor = Color.Black,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "▼ " + group.Name,
                Margin = new Padding( 0 ),
                BorderStyle = BorderStyle.FixedSingle
            };

            label.Click += groupClick;

            _box = new List<Control>();
            _isGroup = true;

            ItemType = ItemType.Group;
            Controls.Add( label );
        }
        /// <summary>
        /// object형식 배열의 요소가 모두 서로 같은지 비교합니다.
        /// <br>이 메서드는 지정된 배열이 null인 경우, 지정된 배열의 요소가 1개인 경우에도 true를 반환합니다.</br>
        /// <br>단, 지정된 배열의 요소가 모두 null인 경우에는 false를 반환합니다.</br>
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        bool isAllSame( object[] objects )
        {
            if( objects == null ) return true;
            else if( objects.Length == 1 ) return true;

            for( var i = 0; i < objects.Length; i++ )
            {
                if( objects[i] == null ) continue;
                if( objects[0] != objects[i] ) return false;
            }

            if( objects[0] == null ) return false;
            return true;
        }
        /// <summary>
        /// 지정된 파라미터의 <see cref="ParameterValueType"/>에 따라 적절한 컨트롤을 사용하여 <see cref="UserControl_ParameterItem"/> 개체를 생성합니다.
        /// <br>생성된 ValueControl의 값은 <paramref name="recipes"/>에 지정된 레시피들의 값에 따라 초기화됩니다.</br>
        /// </summary>
        /// <param name="parameter"><see cref="UserControl_ParameterItem"/>를 생성할 <see cref="RecipeInfo.Group.Parameter"/> 개체입니다.</param>
        /// <param name="recipes"><see cref="UserControl_ParameterItem"/>에 초기값을 지정하기 위한 <see cref="IRecipe"/> 인스턴스를 가지는 배열입니다.</param>
        public UserControl_ParameterItem( RecipeInfo.Group.Parameter parameter, params IRecipe[] recipes ) : this()
        {
            _recipes = recipes;
            _parameter = parameter;

            var label = new Label()
            {
                Font = new Font( "맑은 고딕", 8, FontStyle.Bold ),
                ForeColor = Color.Black,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = parameter.Name
            };

            label.Click += parameterClick;
            if( parameter.Unit != null && parameter.Unit.Trim() != "" ) label.Text += $" ({parameter.Unit})";

            var table = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                Margin = new Padding( 0 )
            };

            table.GetType().GetProperty( "DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance ).SetValue( table, true );

            table.RowStyles.Clear();
            table.RowStyles.Add( new RowStyle( SizeType.Percent, 100 ) );
            table.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 50 ) );
            table.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 40 ) );

            switch( parameter.ParameterValueType )
            {
                case ParameterValueType.Integer:
                case ParameterValueType.Double:
                case ParameterValueType.String:
                case ParameterValueType.Float:
                    _valueControl = new TextBox()
                    {
                        Font = new Font( "맑은 고딕", 10, FontStyle.Bold ),
                        TextAlign = HorizontalAlignment.Left,
                        BorderStyle = BorderStyle.None
                    };
                    if( _recipes.Length == 1 )
                    {
                        object value = parameter.GetValue( _recipes[0] );
                        if( value != null ) _valueControl.Text = value.ToString();
                    }
                    else
                    {
                        var values = new object[_recipes.Length];
                        for( var i = 0; i < values.Length; i++ ) values[i] = parameter.GetValue( _recipes[i] );
                        if( isAllSame( values ) ) _valueControl.Text = values[0].ToString();
                    }
                    break;

                case ParameterValueType.Time:
                    _valueControl = new UserControl_TimeBox()
                    {
                        Font = new Font( "맑은 고딕", 10, FontStyle.Bold ),
                    };
                    if( _recipes.Length == 1 )
                    {
                        object value = parameter.GetValue( _recipes[0] );
                        if( value != null )
                        {
                            (_valueControl as UserControl_TimeBox).Time = TimeSpan.FromMilliseconds( Convert.ToDouble( value ) );
                        }
                    }
                    else
                    {
                        var values = new object[_recipes.Length];
                        for( var i = 0; i < values.Length; i++ ) values[i] = parameter.GetValue( _recipes[i] );
                        if( isAllSame( values ) ) (_valueControl as UserControl_TimeBox).Time = TimeSpan.FromMilliseconds( Convert.ToDouble( values[0] ) );
                    }
                    break;

                case ParameterValueType.Enum:
                case ParameterValueType.Boolean:
                    _valueControl = new ComboBox()
                    {
                        Font = new Font( "맑은 고딕", 8, FontStyle.Bold ),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };

                    List<string> items;
                    var index = 0;
                    if( parameter.ParameterValueType == ParameterValueType.Enum )
                    {
                        items = parameter.GetEnumValues().ToList();
                        if( _recipes.Length == 1 ) index = items.IndexOf( parameter.GetValue( _recipes[0] ).ToString() );
                        else
                        {
                            var values = new object[_recipes.Length];
                            for( var i = 0; i < values.Length; i++ ) values[i] = items.IndexOf( parameter.GetValue( _recipes[i] ).ToString() );
                            if( isAllSame( values ) ) index = Convert.ToInt32( values[0] );
                        }
                    }
                    else
                    {
                        items = new string[] { "False", "True" }.ToList();
                        if( _recipes.Length == 1 ) index = (( bool )parameter.GetValue( _recipes[0] )) ? 1 : 0;
                        else
                        {
                            var values = new object[_recipes.Length];
                            for( var i = 0; i < values.Length; i++ ) values[i] = parameter.GetValue( _recipes[i] );
                            if( isAllSame( values ) ) index = ( bool )values[0] ? 1 : 0;
                        }
                    }

                    (_valueControl as ComboBox).Items.AddRange( items.ToArray() );
                    (_valueControl as ComboBox).SelectedIndex = index;
                    break;

                case ParameterValueType.Pattern:
                    _valueControl = new TextBox()
                    {
                        Font = new Font( "맑은 고딕", 10, FontStyle.Bold ),
                        TextAlign = HorizontalAlignment.Left,
                        //BorderStyle = BorderStyle.None,
                        ReadOnly = true
                    };

                    _valueControl.Click += patternClick;

                    if( _recipes.Length == 1 )
                    {
                        object value = parameter.GetValue( _recipes[0] );
                        if( value != null && value.ToString().Trim() != "" ) _valueControl.Text = value.ToString();
                    }
                    else
                    {
                        var values = new object[_recipes.Length];
                        for( var i = 0; i < values.Length; i++ ) values[i] = parameter.GetValue( _recipes[i] );
                        if( isAllSame( values ) ) _valueControl.Text = values[0].ToString();
                    }
                    break;
            }

            _valueControl.Dock = DockStyle.Fill;
            _valueControl.Margin = new Padding( 0, 1, 0, 1 );
            _valueControl.Click += parameterClick;
            _valueControl.Leave += valueControl_Leave;

            table.Controls.Add( label, 0, 0 );

            if( _parameter.IsAllowNull && _parameter.ParameterValueType != ParameterValueType.Pattern )
            {
                table.ColumnStyles.Insert( 1, new ColumnStyle( SizeType.Absolute, 20 ) );

                _nullControl = new CheckBox()
                {
                    Dock = DockStyle.Fill,
                    Text = ""
                };

                if( _recipes.Length == 1 )
                {
                    if( parameter.GetValue( _recipes[0] ) == null ) _valueControl.Enabled = false;
                    else _nullControl.Checked = true;
                }
                else
                {
                    var values = new object[_recipes.Length];
                    for( var i = 0; i < values.Length; i++ ) values[i] = parameter.GetValue( _recipes[i] );
                    if( isAllSame( values ) && values[0] != null ) _nullControl.Checked = true;
                    else _valueControl.Enabled = false;
                }

                _nullControl.CheckedChanged += nullControlCheckedChanged;
                //if(!_nullControl.Checked) _valueControl.BackColor = Color.LightGray;
                table.Controls.Add( _nullControl, 1, 0 );
            }

            table.Controls.Add( _valueControl, table.ColumnStyles.Count - 1, 0 );

            ItemType = ItemType.Parameter;
            Controls.Add( table );

            Name = parameter.Name;
        }

        private void groupClick( object sender, EventArgs e )
        {
            FlowLayoutPanel box = Parent as FlowLayoutPanel;
            var index = box.Controls.IndexOf( this ) + 1;

            if( _isFolded )
            {
                for( var i = 0; i < _box.Count; i++ )
                {
                    box.Controls.Add( _box[i] );
                    box.Controls.SetChildIndex( _box[i], index + i );
                }

                _box.Clear();
                _isFolded = false;
                (Controls[0] as Label).Text = (Controls[0] as Label).Text.Replace( "▷", "▼" );
            }
            else
            {
                while( true )
                {
                    if( index >= box.Controls.Count ) break;

                    if( !(box.Controls[index] as UserControl_ParameterItem)._isGroup )
                    {
                        _box.Add( box.Controls[index] );
                        box.Controls.RemoveAt( index );
                    }
                    else break;
                }

                _isFolded = true;
                (Controls[0] as Label).Text = (Controls[0] as Label).Text.Replace( "▼", "▷" );
            }

            (Parent.Parent.Parent as UserControl_ParameterBox).ResizeControl();
        }
        private void valueControl_Leave( object sender, EventArgs e )
        {
            save();
        }
        private void save()
        {
            // ValueControl에서 포커스가 벗어날 시 파라미터 값을 레시피들에 적용시킴
            for( var i = 0; i < _recipes.Length; i++ )
            {
                try
                {
                    _parameter.SetValue( _recipes[i], Value );
                }
                catch( ArgumentException )
                {
                    var obj = Value;

                    if( obj == null ) _parameter.SetValue( _recipes[i], null );
                    else _parameter.SetValue( _recipes[i], Convert.ToUInt32( obj ) );
                }
            }
        }
        private void parameterClick( object sender, EventArgs e )
        {
            if( NameLabel != null )
            {
                NameLabel.Text = _parameter.Name;
                if( _parameter.Unit != null ) NameLabel.Text += " (" + _parameter.Unit + ")";
            }
            if( HelpLabel != null ) HelpLabel.Text = _parameter.Help;
        }
        private void nullControlCheckedChanged( object sender, EventArgs e )
        {
            _valueControl.Enabled = _nullControl.Checked;
            if( _valueControl.Enabled )
            {
                //_valueControl.BackColor = Color.White;
                _valueControl.Focus();
            }
            else
            {
                //_valueControl.BackColor = Color.LightGray;
            }

            save();
        }
        private void patternClick( object sender, EventArgs e )
        {
            using( var form = new Form_PatternEditor( Value != null ? Value.ToString() : null ) )
            {
                if( form.ShowDialog() == DialogResult.OK )
                {
                    _valueControl.Text = form.LastLoaded;
                }
            }
        }
    }
}
