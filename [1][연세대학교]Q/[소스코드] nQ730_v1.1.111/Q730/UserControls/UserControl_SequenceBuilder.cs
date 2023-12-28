using McQLib.Recipes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using McQLib.Core;
using System.Linq;
using System.Diagnostics;

namespace Q730.UserControls
{
    public partial class UserControl_SequenceBuilder : UserControl
    {
        private double _outCrate;
        private double _outElcCapa;
        private double _elcCaps = 0;
        private System.Windows.Forms.Label _helpLabel = new System.Windows.Forms.Label()
        {
            Text = "이곳으로 레시피 아이콘을 끌어다 놓으십시오.",
            ForeColor = Color.White,
            BackColor = Color.FromArgb( 150, 150, 150 ),
            AutoSize = true
        };
        public UserControl_SequenceBuilder()
        {
            InitializeComponent();

            Indentation = SoftwareConfiguration.SequenceBuilder.Indentation;
            AutoAddCycleLoopPair = SoftwareConfiguration.SequenceBuilder.AutoAddCycleLoopPair;
            GlobalSafetyCondition = SoftwareConfiguration.SequenceBuilder.GlobalSafetyCondition;

            Application.Idle += Application_Idle;
            flowLayoutPanel_RecipesBox.GetType().GetProperty( "DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance ).SetValue( flowLayoutPanel_RecipesBox, true );

            checkBox_Crate.Checked = false;
            check_CrateMode();
            Controls.Add( _helpLabel );
            _helpLabel.Location = new Point( 65, 330 );
            _helpLabel.BringToFront();
        }

         #region Properties
        /// <summary>
        /// 안전 조건을 전역으로 사용하는 옵션입니다.
        /// <br>이 옵션이 켜져있을 때, 안전 조건의 값을 변경할 경우 시퀀스에 존재하는 모든 레시피의 안전 조건이 동일한 내용으로 함께 수정됩니다.</br>
        /// <br>새로운 레시피를 시퀀스에 추가하는 경우에도 가장 마지막으로 사용된 안전 조건이 적용됩니다.</br>
        /// </summary>
        public bool GlobalSafetyCondition { get; set; }
        /// <summary>
        /// Cycle-Loop 레시피 사이의 레시피들에 대해 들여쓰기 스타일을 적용하는 옵션입니다.
        /// <br><see langword="true"/> : 사용</br>
        /// <br><see langword="false"/> : 사용 안 함</br>
        /// </summary>
        public bool Indentation
        {
            get => _indentation;
            set
            {
                if ( !( _indentation = value ) )
                {
                    foreach ( var control in flowLayoutPanel_RecipesBox.Controls )
                    {
                        ( control as UserControl_RecipeItem ).Indentation = false;
                    }
                }
                else
                {
                    applyIndentation();
                }
            }
        }
        private bool _indentation;
        /// <summary>
        /// Cycle/Loop 레시피를 시퀀스에 추가할 때 자동으로 짝이 맞도록 Loop/Cycle 레시피를 함께 추가해주는 옵션입니다.
        /// <br><see langword="true"/> : 사용</br>
        /// <br><see langword="false"/> : 사용 안 함</br>
        /// </summary>
        public bool AutoAddCycleLoopPair { get; set; }
        /// <summary>
        /// 시퀀스 박스에서 지정된 순서의 레시피를 선택 상태로 설정합니다.
        /// <br>기존에 선택된 개체의 선택은 해제됩니다.</br>
        /// </summary>
        [Browsable( false )]
        [DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public int SelectItem
        {
            get => _selected;
            set
            {
                if ( value >= flowLayoutPanel_RecipesBox.Controls.Count ) return;

                selectItem( flowLayoutPanel_RecipesBox.Controls[_selected = value] as UserControl_RecipeItem );
            }
        }
        #endregion

        #region Members
        private enum ActionType { Add, Insert, Remove, RemoveAt, Clear }
        private class EdittingAction
        {
            public ActionType ActionType;
            public object Object;
            public int? Index;

            public EdittingAction( ActionType actionType, object obj )
            {
                ActionType = actionType;
                Object = obj;
            }
            public EdittingAction( ActionType actionType, object obj, int index )
            {
                ActionType = actionType;
                Object = obj;
                Index = index;
            }
        }

        private int _selected;
        private bool _isContent;
        private Sequence _sequence = new Sequence();
        private UserControl_RecipeItem _dummy = UserControl_RecipeItem.DummyItem;
        private Stack<EdittingAction> _actionStack = new Stack<EdittingAction>();

        public readonly List<string> DefaultPathSequences = new List<string>();
        public readonly List<string> CustomPathSequences = new List<string>();
        public bool SetSequence( Sequence sequence )
        {
            if ( !checkSave() ) return false;

            if ( sequence == null )
            {
                MessageBox.Show( "올바르지 않은 형식의 시퀀스 파일입니다.", "Q730 알림 메시지" );
                return false;
            }
            else
            {
                _sequence = sequence;
                _helpLabel.Visible = false;

                var cycleCnt = _sequence._recipes.FindAll(x => x.Name == "Cycle").Count;
                var loopCnt = _sequence._recipes.FindAll(x => x.Name == "Loop").Count;
                var Item = sequence._recipes.FindAll(x => x.Name.Contains("Charge") || x.Name.Contains("Discharge"));
                comboBox_Loop.Items.Clear();

                bool oldSequence = false;
                
                if (_sequence._crateList.Count == 0)
                {
                    MessageBox.Show("C-Rate 관련 패치 전 시퀀스입니다. 수정 바랍니다.");
                    oldSequence = true;
                }

                if (cycleCnt == loopCnt && cycleCnt != 0 && loopCnt != 0 && Item.Count != 0)
                {
                    //if (oldSequence)
                    //{
                    //    comboBox_Loop.Enabled = false;
                    //}
                    //else
                    //{
                    //    comboBox_Loop.Enabled = true;
                    //}
                    comboBox_Loop.Enabled = true;
                    for (int i = 0; i < cycleCnt; i++)
                    {
                        comboBox_Loop.Items.Add(i + 1);
                    }

                    comboBox_Loop.SelectedIndex = 0;
                }
                else if (cycleCnt == 0 && loopCnt == 0)
                {
                    comboBox_Loop.Items.Add("");
                    comboBox_Loop.SelectedIndex = 0;
                    comboBox_Loop.Enabled = false;
                    checkBox_Crate.Checked = false;
                    check_CrateMode();

                    MessageBox.Show("Cycle Loop 설정이 되어있지 않습니다.", "Q730 알림 메시지");
                }
                else if (cycleCnt != 0 && loopCnt != 0 && Item.Count == 0)
                {
                    comboBox_Loop.Items.Add("");
                    comboBox_Loop.SelectedIndex = 0;
                    comboBox_Loop.Enabled = false;
                    checkBox_Crate.Checked = false;
                    check_CrateMode();
                } 
                else
                {
                    MessageBox.Show("Cycle Loop 설정이 잘못 되었습니다.", "Q730 알림 메시지");
                    return false;
                }

                ////////저장을 위한 Crate 정보를 comboBox_Loop 아이템을 가지고 _sequence._crateList 갯수를 생성한다.
                if (_sequence._crateList.Count == 0)
                {
                    _sequence._crateList.Clear();
                    if (comboBox_Loop.Items[0] != "")
                    {
                        for (var i = 0; i < comboBox_Loop.Items.Count; i++)
                        {
                            if (comboBox_Loop.Items[0] == " Cycle Loop 세팅 필요")
                            {
                                _sequence._crateList.Add(new CrateInfo
                                {
                                    index = -1,
                                    cRate = 0,
                                    elcCapa = 0,
                                    checkCrate = false
                                });
                            }
                            else
                            {
                                if (oldSequence)
                                {
                                    _sequence._crateList.Add(new CrateInfo
                                    {
                                        index = (int)comboBox_Loop.Items[i],
                                        cRate = 0,
                                        elcCapa = 0,
                                        checkCrate = false
                                    });
                                }

                                else
                                {
                                    _sequence._crateList.Add(new CrateInfo
                                    {
                                        index = (int)comboBox_Loop.Items[i],
                                        cRate = 0,
                                        elcCapa = 0,
                                        checkCrate = true
                                    });
                                }
                             
                            }
                        }

                        checkBox_Crate.Checked = false;
                        //textBox_Crate.Text = _sequence._crateInfos.cRate.ToString();
                        //textBox_ElcCapa.Text = _sequence._crateInfos.elcCapa.ToString();
                        check_CrateMode();
                    }
                }
                else
                {
                    comboBox_Loop.SelectedIndex = 0;
                    //checkBox_Crate.Checked = true;
                    textBox_Crate.Text = _sequence._crateList[0].cRate.ToString();
                    textBox_ElcCapa.Text = _sequence._crateList[0].elcCapa.ToString();
                    if (_sequence._crateList[0].checkCrate)
                    {
                        checkBox_Crate.Checked = true;
                        check_CrateMode();
                    }
                    else
                    {
                        checkBox_Crate.Checked = false;
                        check_CrateMode();
                    }

                }

                //if (_sequence._crateInfos.cRate != 0 && _sequence._crateInfos.elcCapa != 0)
                //{
                //    checkBox_Crate.Checked = true;
                //    textBox_Crate.Text = _sequence._crateInfos.cRate.ToString();
                //    textBox_ElcCapa.Text = _sequence._crateInfos.elcCapa.ToString();
                //}
                //else
                //{
                //    checkBox_Crate.Checked = false;
                //    textBox_Crate.Text = _sequence._crateInfos.cRate.ToString();
                //    textBox_ElcCapa.Text = _sequence._crateInfos.elcCapa.ToString();
                //    check_CrateMode();
                //}

                textBox_area.Text = _sequence._batteryInfo.batteryArea.ToString();

                textBox_mass.Text = _sequence._batteryInfo.batteryMass.ToString();


                RefreshSequence();
                return true;
            }
        }
        public Sequence GetSequence()
        {
            return _sequence;
        }
        public void ClearSequence()
        {
            _actionStack.Push( new EdittingAction( ActionType.Clear, _sequence ) );

            _sequence = new Sequence();
            RefreshSequence();
        }
        public void RefreshSequence()
        {
            flowLayoutPanel_RecipesBox.Controls.Clear();

            if ( _sequence == null ) return;

            for ( var i = 0; i < _sequence.Count; i++ )
            {
                var item = new UserControl_RecipeItem( _sequence[i] );
                if ( _sequence[i].GetRecipeType() == RecipeType.Label ) item.SetToLabel();
                item.MouseDown += item_MouseDown;
                item.MouseMove += item_MouseMove;
                item.MouseUp += item_MouseUp;
                flowLayoutPanel_RecipesBox.Controls.Add( item );
            }

            if ( _sequence.Comment == null || _sequence.Comment.Trim() == "" )
            {
                _isContent = false;
                textBox_Comment.ForeColor = Color.LightGray;
                textBox_Comment.Text = "Comment here...";
            }
            else
            {
                textBox_Comment.ForeColor = Color.Black;
                textBox_Comment.Text = _sequence.Comment;
                _isContent = true;
            }

            textBox_Name.Text = _sequence.Name;
        
            applyIndentation();
            resizeRecipeItems();
        }

        private void Application_Idle( object sender, EventArgs e )
        {
            Application.Idle -= Application_Idle;

            listView_SequenceList.Columns.Add( string.Empty );
            listView_SequenceList.Columns[0].Width = listView_SequenceList.Width - 4;

            foreach ( var r in RecipeFactory.Recipes )
            {
                var item = new UserControl_RecipeItem( r, true )
                {
                    //ViewMode = ViewMode.Icon,
                    Width = 40,
                    Height = 40
                };

                // 임시 사용 불가
                switch ( r.GetRecipeType() )
                {
                    case RecipeType.Temperature:
                        item.Lock = true;
                        break;
                    case RecipeType.Jump:
                        item.Lock = true;
                        break;
                    case RecipeType.Label:
                        item.Lock = true;
                        break;


                }

                item.MouseDown += icon_MouseDown;

                if (r.Name != "Jump" && r.Name != "Label")
                {
                    flowLayoutPanel_IconBox.Controls.Add(item);
                }
               
            }

            toolTip2.Show( string.Empty, textBox_Name, 0 );
            toolTip2.Hide( textBox_Name );

            timer_FileLoader.Start();
            timer_ContentRefresher.Start();
            timer_ItemMover.Start();

            if ( _sequence == null )
            {
                _sequence = new Sequence();
                RefreshSequence();
            }
        }
        #endregion

        #region Sequence Box Features
        private UserControl_RecipeItem _selectItem = null;
        private List<UserControl_RecipeItem> _selectItems = new List<UserControl_RecipeItem>();

        private bool _IsLeftDown => MouseButtons == MouseButtons.Left;
        private Point _MousePoint => FindForm().PointToClient( MousePosition );
        private int _DummyIndex => flowLayoutPanel_RecipesBox.Controls.IndexOf( _dummy );
        
        private Point _downPoint;
        private bool _ready;
        private bool _newCycleLoop = false;

        private Rectangle _RecipeBoxBound
        {
            get
            {
                Rectangle rect = flowLayoutPanel_RecipesBox.Bounds;
                rect.Location = getLocationOnClient( flowLayoutPanel_RecipesBox );
                return rect;
            }
        }
        private Point getLocationOnClient( Control c )
        {
            Point p = new Point( 0, 0 );
            for ( ; c.Parent != null; c = c.Parent )
            {
                p.Offset( c.Location ); 
            }

            return p;
        }

        private void timer_ItemMover_Tick( object sender, EventArgs e )
        {
            if ( _selectItem == null ) return;

            if ( _IsLeftDown )
            {
                // 제자리에 멈춘 상태일 때 깜빡이는 현상 제거용
                if ( _selectItem.Location == new Point( _MousePoint.X - 27, _MousePoint.Y - 27 ) )
                    return;

                _selectItem.Location = new Point( _MousePoint.X - 27, _MousePoint.Y - 27 );
                if ( _RecipeBoxBound.Contains( _MousePoint ) )
                {
                    //Cursor = Cursors.NoMove2D;
                    setDummy( new Point( _MousePoint.X - _RecipeBoxBound.X, _MousePoint.Y - _RecipeBoxBound.Y ) );
                    //resizeRecipeItems();
                }
                else
                {
                    //Cursor = Cursors.PanEast;
                    removeDummy();
                }
            }
            else
            {
                //Cursor = Cursors.Default;
                if ( !_RecipeBoxBound.Contains( _MousePoint ) )
                {
                    ParentForm.Controls.Remove( _selectItem );
                    if ( flowLayoutPanel_RecipesBox.Controls.Count == 0 ) _helpLabel.Visible = true;
                    removeDummy();
                    applyIndentation();
                }
                else
                {
                    var index = _DummyIndex;
                    _selectItem.IsNew = false;

                    addItem( _DummyIndex, _selectItem );
                    //_actionStack.Push(new EditAction(ActionType.))

                    // Cycle, Loop 짝 맞게 자동 추가
                    if ( AutoAddCycleLoopPair && _newCycleLoop )
                    {
                        if ( index == -1 ) index = _sequence.Count - 1;
                        if ( _sequence[index].GetRecipeType() == RecipeType.Cycle )
                        {
                            bool loopFound = false;
                            for ( var i = index + 1; i < _sequence.Count; i++ )
                            {
                                var recipeType = _sequence[i].GetRecipeType();
                                if ( recipeType == RecipeType.Loop )
                                {
                                    loopFound = true;
                                    break;
                                }
                                else if ( recipeType == RecipeType.Cycle ) break;
                            }

                            if ( !loopFound )
                            {
                                var item = new UserControl_RecipeItem( RecipeFactory.CreateInstance( RecipeType.Loop ) );
                                item.GetType().GetProperty( "DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic ).SetValue( item, true );
                                addItem( index + 1, item );
                            }
                        }
                        else if ( _sequence[index].GetRecipeType() == RecipeType.Loop )
                        {
                            bool cycleFound = false;
                            for ( var i = index - 1; i >= 0; i-- )
                            {
                                var recipeType = _sequence[i].GetRecipeType();
                                if ( recipeType == RecipeType.Cycle )
                                {
                                    cycleFound = true;
                                    break;
                                }
                                else if ( recipeType == RecipeType.Loop ) break;
                            }

                            if ( !cycleFound )
                            {
                                var item = new UserControl_RecipeItem( RecipeFactory.CreateInstance( RecipeType.Cycle ) );
                                item.GetType().GetProperty( "DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic ).SetValue( item, true );
                                addItem( index, item );
                            }
                        }
                    }

                    resizeRecipeItems();
                }

                _selectItem = null;
            }
        }
        private void icon_MouseDown( object sender, MouseEventArgs e )
        {
            if ( e.Button == MouseButtons.Left )
            {
                if ( ( getParentUserControl( sender as Control ) as UserControl_RecipeItem ).Lock )
                {
                    MessageBox.Show( "이 레시피는 현재 사용이 제한된 레시피입니다.", "Q730 알림 메시지" );
                    return;
                }
                if ( _sequence.Count == Sequence.MAX_STORAGE )
                {
                    MessageBox.Show( $"시퀀스가 가득 차서 더이상 새로운 레시피를 추가할 수 없습니다. 최대로 추가 가능한 레시피 개수는 {Sequence.MAX_STORAGE}개입니다.", "Q730 알림 메시지" );
                    return;
                }
                var recipeType = ( getParentUserControl( sender as Control ) as UserControl_RecipeItem ).Recipe.GetRecipeType();
                _selectItem = new UserControl_RecipeItem( RecipeFactory.CreateInstance( recipeType ) );
                if ( GlobalSafetyCondition )
                {
                    _selectItem.Recipe.SafetyCondition?.CopyFrom( _globalSafetyCondition );
                }

                if ( recipeType == RecipeType.Cycle || recipeType == RecipeType.Loop ) _newCycleLoop = true;
                else _newCycleLoop = false;

                if ( recipeType == RecipeType.Label ) _selectItem.SetToLabel();

                _selectItem.GetType().GetProperty( "DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic ).SetValue( _selectItem, true, null );
                _selectItem.IsNew = true;

                _selectItem.Location = new Point( _MousePoint.X - 27, _MousePoint.Y - 27 );
                ParentForm.Controls.Add( _selectItem );
                _selectItem.BringToFront();
            }
        }

        private void item_MouseDown( object sender, MouseEventArgs e )
        {
            if ( _IsLeftDown )
            {
                _downPoint = e.Location;
                _ready = true;
            }
        }
        private double getDistance( Point p1, Point p2 )
        {
            return Math.Sqrt( Math.Pow( p1.X - p2.X, 2 ) + Math.Pow( p1.Y - p2.Y, 2 ) );
        }
        private void item_MouseMove( object sender, MouseEventArgs e )
        {
            if ( !_ready ) return;

            //if( _IsLeftDown && _downPoint != e.Location )
            // 마우스 다운한 지점이랑 현재 지점의 거리가 5픽셀 초과인 경우에만 이동을 시작한 것으로 간주.
            if ( _IsLeftDown && getDistance( _downPoint, e.Location ) > 5 )
            {
                _newCycleLoop = false;
                var item = getParentUserControl( ( Control )sender ) as UserControl_RecipeItem;
                item.Indentation = false;
                var index = flowLayoutPanel_RecipesBox.Controls.IndexOf( item );
                flowLayoutPanel_RecipesBox.Controls.Remove( item );
                resizeRecipeItems();
                setDummy( index );

                _sequence.RemoveAt( index );
                _selectItem = item;
                ParentForm.Controls.Add( _selectItem );
                item.BringToFront();
                _ready = false;
            }
        }
        private void item_MouseUp( object sender, MouseEventArgs e )
        {
            //if( ModifierKeys == Keys.Control ) return;
            if ( e.Button == MouseButtons.Left && _ready && _downPoint == e.Location )
            {
                _ready = false;
                if ( propertyGrid1 != null )
                {
                    if ( ModifierKeys == Keys.Control ) selectItem( getParentUserControl( ( Control )sender ) as UserControl_RecipeItem, false );
                    else selectItem( getParentUserControl( ( Control )sender ) as UserControl_RecipeItem );
                }
            }
        }

        private void resizeRecipeItems()
        {
            var totalHeight = 0;
            foreach ( Control c in flowLayoutPanel_RecipesBox.Controls )
            {
                totalHeight += c.Height + c.Margin.Top + c.Margin.Bottom;
            }

            if ( totalHeight > flowLayoutPanel_RecipesBox.Bounds.Height )
            {
                foreach ( UserControl_RecipeItem c in flowLayoutPanel_RecipesBox.Controls )
                {
                    c.Width = flowLayoutPanel_RecipesBox.Bounds.Width - 23;
                    if ( c.Indentation ) c.Width -= 18;
                }
            }
            else
            {
                foreach ( UserControl_RecipeItem c in flowLayoutPanel_RecipesBox.Controls )
                {
                    c.Width = flowLayoutPanel_RecipesBox.Bounds.Width - 6;
                    if ( c.Indentation ) c.Width -= 18;
                }
            }
        }
        private void timer_ContentRefresher_Tick( object sender, EventArgs e )
        {
            if ( flowLayoutPanel_RecipesBox.Controls.Count == 0 ) return;

            timer_ContentRefresher.Stop();

            foreach ( UserControl_RecipeItem item in flowLayoutPanel_RecipesBox.Controls )
            {
                item.RefreshText();
                Application.DoEvents();
            }

            timer_ContentRefresher.Start();
        }

        private void setDummy( Point mouseLocation )
        {
            int index = -1;

            if ( _dummy.Bounds.Contains( mouseLocation ) ) return;

            var boundList = new List<Rectangle>();
            foreach ( Control item in flowLayoutPanel_RecipesBox.Controls )
            {
                var rect = item.Bounds;
                rect.X -= 1;
                rect.Y -= 1;
                rect.Width += 2;
                rect.Height += 2;
                boundList.Add( rect );
            }

            for ( var i = 0; i < boundList.Count; i++ )
            {
                Rectangle up, down;
                up = down = boundList[i];

                up.Height /= 2;
                down.Height /= 2;
                down.Y += down.Height;

                if ( up.Contains( mouseLocation ) )
                {
                    index = i;
                    break;
                }
                else if ( down.Contains( mouseLocation ) )
                {
                    index = i + 1;
                    break;
                }
            }

            if ( index != -1 )
            {
                setDummy( index );
            }
            else
            {
                removeDummy();
            }
        }
        private void setDummy( int index )
        {
            if ( flowLayoutPanel_RecipesBox.Controls.IndexOf( _dummy ) == -1 )
                flowLayoutPanel_RecipesBox.Controls.Add( _dummy );
            flowLayoutPanel_RecipesBox.Controls.SetChildIndex( _dummy, index );
            flowLayoutPanel_RecipesBox.ScrollControlIntoView( _dummy );
        }
        private void addItem( int index, UserControl_RecipeItem item )
        {
            if ( index < 0 ) index = flowLayoutPanel_RecipesBox.Controls.Count;
            _sequence.Insert( index, item.Recipe );
            
            item.MouseDown += item_MouseDown;
            item.MouseMove += item_MouseMove;
            item.MouseUp += item_MouseUp;

            removeDummy();
            flowLayoutPanel_RecipesBox.Controls.Add( item );
            flowLayoutPanel_RecipesBox.Controls.SetChildIndex( item, index );

            applyIndentation();

            selectItem( item );

            var cycleCnt = _sequence._recipes.FindAll(x => x.Name == "Cycle").Count;
            var loopCnt = _sequence._recipes.FindAll(x => x.Name == "Loop").Count;
            var hasNotItemsBetweenCycleAndLoop = false;
            var hasItemsBetweenCycleAndLoop = false;
            for (int i = 0; i < _sequence._recipes.Count - 1; i++)
            {
                if ((_sequence._recipes[i].Name == "Cycle" && _sequence._recipes[i + 1].Name == "Loop"))
                {
                    hasNotItemsBetweenCycleAndLoop = true;
                }
                else
                {
                    hasItemsBetweenCycleAndLoop = true;
                }
            }
            comboBox_Loop.Items.Clear();

            if (cycleCnt == loopCnt && cycleCnt != 0 && loopCnt != 0 && hasItemsBetweenCycleAndLoop)
            {
                for (int i = 0; i < cycleCnt; i++)
                {
                    comboBox_Loop.Items.Add(i + 1);
                }

                if (comboBox_Loop.Items.Count > 0)
                {
                    comboBox_Loop.SelectedIndex = 0;
                }
                this.button_Apply.Enabled = true;

                this.comboBox_Loop.Enabled = true;

                this.textBox_Crate.Enabled = false;//c-rate는 처음 생성되면 무조건 해제된 상태로
                this.label_Crate.Enabled = false;

                this.textBox_ElcCapa.Enabled = true;
                this.label_mah.Enabled = true;
             
                checkBox_Crate.Checked = false; //처음 생성되면 무조건 해제된 상태로
                checkBox_Crate.Enabled = true;
         
                label_CrateUnit.Enabled = true;
                label_ElcCapaUnit.Enabled = true;
                label_mah.Enabled = true;
                label_Crate.Enabled = true;

            }
            else
            {
                comboBox_Loop.Items.Clear();
                comboBox_Loop.Items.Add(" Cycle Loop 세팅 필요");
                comboBox_Loop.SelectedIndex = 0;


                //this.button_Apply.Enabled = false;

                this.comboBox_Loop.Enabled = false;

                this.textBox_Crate.Enabled = false;
                this.label_Crate.Enabled = false;

                //this.textBox_ElcCapa.Enabled = false;
                //this.label_mah.Enabled = false;



                checkBox_Crate.Checked = false;
                checkBox_Crate.Enabled = false;
         
                label_CrateUnit.Enabled = false;
                //label_ElcCapaUnit.Enabled = false;
                //label_mah.Enabled = false;
                label_Crate.Enabled = false;
            }

            //Controls.Remove( _helpLabel );
            ////////저장을 위한 Crate 정보를 comboBox_Loop 아이템을 가지고 _sequence._crateList 갯수를 생성한다.
           
            if (_sequence._crateList.Count > 0)
            {
                if (_sequence._crateList[0].elcCapa != 0)
                {
                    _elcCaps = _sequence._crateList[0].elcCapa;
                }
            }

            _sequence._crateList.Clear(); //20231212 장재훈 이걸 왜 Clear 하지?? 여태 잘 추가 해놓은것들인데
            for (var i = 0; i < comboBox_Loop.Items.Count; i++)
            {
                if (comboBox_Loop.Items[0] == " Cycle Loop 세팅 필요")
                {
                    _sequence._crateList.Add(new CrateInfo
                    {
                        index = -1,
                        cRate = 0,
                        elcCapa = _elcCaps,
                        checkCrate = false
                    });
                }
                else
                {
                    _sequence._crateList.Add(new CrateInfo
                    {
                        index = (int)comboBox_Loop.Items[i],
                        cRate = 0,
                        elcCapa = _elcCaps,
                        checkCrate = false
                    });
                }
            }

            _helpLabel.Visible = false;
        }
        private UserControl getParentUserControl( Control control )
        {
            while ( control.Parent != null )
            {
                if ( control is UserControl ) return ( UserControl )control;
                else control = control.Parent;
            }

            return null;
        }
        private void removeDummy()
        {
            flowLayoutPanel_RecipesBox.Controls.Remove( _dummy );
        }
        private void applyIndentation()
        {
            if ( !Indentation ) return;

            int cycleIndex = -1;

            for ( var i = 0; i < flowLayoutPanel_RecipesBox.Controls.Count; i++ )
            {
                var item = flowLayoutPanel_RecipesBox.Controls[i] as UserControl_RecipeItem;
                item.Indentation = false;

                if ( item.Recipe.GetRecipeType() == RecipeType.Cycle ) cycleIndex = i;
                else if ( item.Recipe.GetRecipeType() == RecipeType.Loop )
                {
                    if ( cycleIndex != -1 )
                    {
                        for ( var j = i - 1; j > cycleIndex; j-- )
                        {
                            ( flowLayoutPanel_RecipesBox.Controls[j] as UserControl_RecipeItem ).Indentation = true;
                        }
                        cycleIndex = -1;
                    }
                }
            }
        }

        private SafetyCondition _globalSafetyCondition;
        private UserControl_RecipeItem _selectIconItem;
        private void selectItem( UserControl_RecipeItem item, bool clear = true )
        {
            if ( clear ) clearSelection();

            this.ActiveControl = null;
            var list = propertyGrid1.SelectedObjects.ToList();
            _selectIconItem = item;

            if ( item.Selected )
            {
                list.Remove( item.Recipe );
            }
            else
            {
                list.Add( item.Recipe );
                item.Recipe?.Refresh();
                //item.Recipe?.SaveCondition?.Refresh();
                //item.Recipe?.EndCondition?.Refresh();
            }

            // 중간에 클릭을해서 지워진 경우에는 다시 Loop 카운터 갯수를 확인하고 나머지 _sequence._crateList에서 나머지 갯수는 지운다.
            // 현재 마지막 껄 무조건 지우는 방법으로 되지 않음.
            // 중간 loop에 있는 step이 지워지면


            //// ComboBox 항목 개수 확인
            //int comboBoxItemCount = comboBox_Loop.Items.Count;

            //// List의 개수 확인
            //int crateListCount = _sequence._crateList.Count;

            //if (comboBoxItemCount < crateListCount)
            //{
            //    // List에서 첫 번째부터 2개를 제외한 나머지 항목을 삭제
            //    _sequence._crateList.RemoveRange(comboBoxItemCount, crateListCount - comboBoxItemCount);
            //}






            int cycleCount = -1;
            var totalCycleList = _sequence._recipes.FindAll(x => x.Name == "Cycle");
            var totalLoopList = _sequence._recipes.FindAll(x => x.Name == "Loop");
            if (totalCycleList.Count == _sequence._crateList.Count && totalLoopList.Count == _sequence._crateList.Count)
            //장재훈 중간에 스텝을 지우면 _sequence._crateList 이거 갯수랑 현재 cycle loop 갯수랑 맞지가 않아서 갱신이 안되..... 
            {
                foreach (var recipe in _sequence._recipes)
                {
                    if (recipe.Name == "Cycle")
                    {
                        cycleCount++;
                    }
                    if (recipe == item.Recipe)
                    {
                        //comboBox_Loop.SelectedIndex = _sequence
                        if (_sequence._crateList.Count > 0 && cycleCount != -1)
                        {
                            comboBox_Loop.SelectedIndex = cycleCount;

                            textBox_Crate.Text = _sequence._crateList[cycleCount].cRate.ToString();

                            textBox_ElcCapa.Text = _sequence._crateList[cycleCount].elcCapa.ToString();

                            checkBox_Crate.Checked = _sequence._crateList[cycleCount].checkCrate;

                            textBox_area.Text = _sequence._batteryInfo.batteryArea.ToString();

                            textBox_mass.Text = _sequence._batteryInfo.batteryMass.ToString();
                        }

                    }
                }
            
            }
           


            propertyGrid1.SelectedObjects = list.ToArray();
            expandAll();

            item.Selected = !item.Selected;

            // Application.DoEvents()로 인해 실제 처리 순서가 뒤섞이면서 인덱스 오류 발생하여 제거함..
            //Application.DoEvents();
        }
        private void clearSelection()
        {
            propertyGrid1.SelectedObjects = null;

            foreach ( UserControl_RecipeItem control in flowLayoutPanel_RecipesBox.Controls ) control.Selected = false;
        }
        private void expandAll()
        {
            if ( propertyGrid1.SelectedObjects == null || propertyGrid1.SelectedObjects.Length == 0 ) return;

            var view = propertyGrid1.GetType().GetField( "gridView", System.Reflection.BindingFlags.NonPublic |
                                                                     System.Reflection.BindingFlags.Instance ).GetValue( propertyGrid1 );
            // Splitter를 이동시킴.
            view.GetType().GetMethod( "MoveSplitterTo", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic ).Invoke( view, new object[] { 180 } );

            // 모든 Category를 확장.
            var collection = view.GetType().InvokeMember( "GetAllGridEntries", System.Reflection.BindingFlags.InvokeMethod |
                                                                               System.Reflection.BindingFlags.NonPublic |
                                                                               System.Reflection.BindingFlags.Instance, null, view, null ) as GridItemCollection;

            if ( collection == null ) return;

            foreach ( GridItem item in collection )
            {
                item.Expanded = true;
            }
        }
        #endregion

        #region Editing Features
        private void textBox_Comment_Leave( object sender, EventArgs e )
        {
            if ( textBox_Comment.Text.Trim() == "" )
            {
                _isContent = false;
                textBox_Comment.ForeColor = Color.LightGray;
                textBox_Comment.Text = "Comment here...";
                _sequence.Comment = "";
            }
            else
            {
                _sequence.Comment = textBox_Comment.Text;
                _isContent = true;
            }
        }
        private void textBox_Comment_Enter( object sender, EventArgs e )
        {
            if ( !_isContent )
            {
                textBox_Comment.ForeColor = Color.Black;
                textBox_Comment.Text = "";
                _sequence.Comment = "";
            }
        }
        private void textBox_Name_MouseHover( object sender, EventArgs e )
        {
            return;
            //toolTip1.Show( _sequence.FilePath, textBox_Name );
        }
        private void textBox_Name_KeyPress( object sender, KeyPressEventArgs e )
        {
            // 파일 이름에 사용할 수 없는 문자 걸러내기

            // \, /, :, *, ?, ", <, >, |
            if ( e.KeyChar == '\\' ||
                e.KeyChar == '/' ||
                e.KeyChar == ':' ||
                e.KeyChar == '*' ||
                e.KeyChar == '?' ||
                e.KeyChar == '\"' ||
                e.KeyChar == '<' ||
                e.KeyChar == '>' ||
                e.KeyChar == '|' )
            {
                e.Handled = true;
                //toolTip2.SetToolTip( textBox_Name, string.Empty );

                toolTip2.Show( "시퀀스 이름에는 다음 문자를 사용할 수 없습니다.\r\n" +
                    "\t\t\\ / : * ? \" < > |", textBox_Name, textBox_Name.Width / 2, textBox_Name.Height, 5000 );
                // 
            }
            else
            {
                toolTip2.Hide( textBox_Name );
            }
        }
        private void textBox_Name_Leave( object sender, EventArgs e )
        {
            _sequence.Name = textBox_Name.Text;
        }

        private void button_New_Click( object sender, EventArgs e )
        {
            if ( _sequence.IsChanged )
            {
                switch ( MessageBox.Show( "시퀀스가 변경되었습니다. 변경 사항을 저장하시겠습니까?", "Q730 알림 메시지", MessageBoxButtons.YesNoCancel ) )
                {
                    case DialogResult.Yes:
                        if ( _sequence.Name.Trim().Length == 0 )
                        {
                            MessageBox.Show( "시퀀스의 이름을 입력하세요.", "Q730 알림 메시지" );
                            return;
                        }
                        _sequence.Save();
                        break;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        return;
                }
            }

            _sequence = new Sequence();
            clearSelection();
            RefreshSequence();
        }
        private void button_SaveAs_Click(object sender, EventArgs e)
        {
            if (_sequence.Count == 0)
            {
                MessageBox.Show("빈 시퀀스를 저장할 수 없습니다.", "Q730 알림 메시지");
                return;
            }

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Sequence Files (*.seq)|*.seq|All Files (*.*)|*.*"; // Filter to restrict file types
                saveFileDialog.Title = "Save As"; // Dialog title
                saveFileDialog.ShowDialog(); // Show the save file dialog

                if (saveFileDialog.FileName != "")
                {
                    _sequence.SaveAs(saveFileDialog.FileName);
                }

                
            }
            catch (QException ex)
            {
                foreach (UserControl_RecipeItem item in flowLayoutPanel_RecipesBox.Controls)
                    item.RefreshError();

                MessageBox.Show(ex.Message, "Q730 알림 메시지");
            }
            // Check if the user selected a file
           
        }
        private void button_Save_Click( object sender, EventArgs e )
        {
            if ( textBox_Name.Text.Length == 0 )
            {
                MessageBox.Show( "시퀀스의 이름을 입력하세요.", "Q730 알림 메시지" );
                return;
            }
            else if (_sequence.Count == 0 )
            {
                MessageBox.Show( "빈 시퀀스를 저장할 수 없습니다.", "Q730 알림 메시지" );
                return;
            }

            if (checkBox_Crate.Checked == true)
            {
                if (_outCrate == 0 || _outElcCapa == 0)
                {
                    MessageBox.Show("C-rate Apply 버튼을 눌러주세요.", "Q730 알림 메시지");
                    return;
                }
                else
                {
                    if (_sequence._crateList.Count == 0)
                    {
                        MessageBox.Show("C-rate 설정을 해주세요.", "Q730 알림 메시지");
                        return;
                    }
                    //체크박스 처리 어떻게 할건지???????????? 0608 체크
                }
            }


            foreach (var recipe in _sequence._recipes)
            {
                if (recipe.SaveCondition != null)
                {
                    //if (recipe.SaveCondition.SaveCondition_Type == SaveCondition_Type.시간)
                    //{
                    //    if (recipe.SaveCondition.Save_Interval == null)
                    //    {
                    //        MessageBox.Show($"저장 조건 : '{recipe.SaveCondition.SaveCondition_Type}'을 설정 해주세요.", "Q730 알림 메시지");
                    //        return;
                    //    }
                    //}
                    //else if (recipe.SaveCondition.SaveCondition_Type == SaveCondition_Type.전압)
                    //{
                    //    if (recipe.SaveCondition.Save_Voltage == null)
                    //    {
                    //        MessageBox.Show($"저장 조건 : '{recipe.SaveCondition.SaveCondition_Type}'을 설정 해주세요.", "Q730 알림 메시지");
                    //        return;
                    //    }
                    //}
                    //else if (recipe.SaveCondition.SaveCondition_Type == SaveCondition_Type.전류)
                    //{
                    //    if (recipe.SaveCondition.Save_Current == null)
                    //    {
                    //        MessageBox.Show($"저장 조건 : '{recipe.SaveCondition.SaveCondition_Type}'을 설정 해주세요.", "Q730 알림 메시지");
                    //        return;
                    //    }
                    //}
                }

                if (recipe.EndCondition != null)
                {
                    //if (recipe.EndCondition.EndCondition_Type == EndCondition_Type.시간)
                    //{
                    //    if (recipe.EndCondition.End_Time == null)
                    //    {
                    //        MessageBox.Show($"종료 조건 : '{recipe.EndCondition.EndCondition_Type}'을 설정 해주세요.", "Q730 알림 메시지");
                    //        return;
                    //    }
                    //}
                    //else if (recipe.EndCondition.EndCondition_Type == EndCondition_Type.전압)
                    //{
                    //    if (recipe.EndCondition.End_Voltage == null)
                    //    {
                    //        MessageBox.Show($"종료 전압을 설정 해주세요.", "Q730 알림 메시지");
                    //        return;
                    //    }
                    //}
                    //else if (recipe.EndCondition.EndCondition_Type == EndCondition_Type.전류)
                    //{
                    //    if (recipe.EndCondition.End_Current == null)
                    //    {
                    //        MessageBox.Show($"종료 전류를 설정 해주세요.", "Q730 알림 메시지");
                    //        return;
                    //    }
                    //}
                }
            }

            try
            {
                List<int> cycleIndices = _sequence._recipes.FindAll(x => x.Name == "Cycle")
                                            .Select(x => _sequence._recipes.IndexOf(x))
                                            .ToList();
                List<int> loopIndices = _sequence._recipes.FindAll(x => x.Name == "Loop")
                                            .Select(x => _sequence._recipes.IndexOf(x))
                                            .ToList();

                if (cycleIndices.Count == loopIndices.Count)
                {
                    for (int i = 0; i < cycleIndices.Count; i++)
                    {
                        //cycleIndices 첫번쨰번호와 loopIndices 첫번째 번호 사이에 _sequence._recipes.FindAll(x => x.Name == "Charge") 가 2개이상이면 예외처리 해야하는 코드
                        int startIdx = cycleIndices[i];
                        int endIdx = loopIndices[i];
                        int chargeCount = _sequence._recipes.FindAll(x => x.Name == "Charge" && _sequence._recipes.IndexOf(x) > startIdx && _sequence._recipes.IndexOf(x) < endIdx).Count;
                        int disChargeCount = _sequence._recipes.FindAll(x => x.Name == "Discharge" && _sequence._recipes.IndexOf(x) > startIdx && _sequence._recipes.IndexOf(x) < endIdx).Count;
                        int anodeChargeCount = _sequence._recipes.FindAll(x => x.Name == "AnodeCharge" && _sequence._recipes.IndexOf(x) > startIdx && _sequence._recipes.IndexOf(x) < endIdx).Count;
                        int anodeDisChargeCount = _sequence._recipes.FindAll(x => x.Name == "AnodeDischarge" && _sequence._recipes.IndexOf(x) > startIdx && _sequence._recipes.IndexOf(x) < endIdx).Count;

                        if (chargeCount >= 2 || disChargeCount >= 2 || anodeChargeCount >= 2 || anodeDisChargeCount >= 2)
                        {
                            MessageBox.Show("중복 요소가 2개 이상 있습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else if ((chargeCount > 0 && anodeChargeCount > 0)||(disChargeCount > 0 && anodeDisChargeCount > 0))
                        {
                            MessageBox.Show("양극셀, 음극셀 설정 확인 부탁드립니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    if (textBox_Custom_Path.Text == "")
                    {
                        MessageBox.Show("경로 설정 버튼으로 저장 경로 설정을 부탁드립니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        string path = Path.Combine(textBox_Custom_Path.Text, $"{textBox_Name.Text}.seq");
                        _sequence.Save(path);
                    }
                 
                }
                else
                    MessageBox.Show("Cycle, Loop 설정이 잘못되었습니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch ( QException ex )
            {
                foreach ( UserControl_RecipeItem item in flowLayoutPanel_RecipesBox.Controls )
                    item.RefreshError();

                MessageBox.Show( ex.Message, "Q730 알림 메시지" );
            }
        }
        private void button_Load_Click( object sender, EventArgs e )
        {
            if ( !checkSave() ) return;

            using ( var dialog = new OpenFileDialog()
            {
                Filter = "Q730 Sequence File(*.seq)|*.seq",
                InitialDirectory = Sequence.DefaultDirectory
            } )
            {
                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    _sequence = Sequence.FromFile( dialog.FileName );

                    if ( _sequence == null )
                    {
                        MessageBox.Show( "올바르지 않은 형식의 시퀀스 파일입니다.", "Q730 알림 메시지" );
                    }

                    clearSelection();
                    RefreshSequence();
                    applyIndentation();
                    _helpLabel.Visible = false;
                }
            }
        }
        private void button_Clear_Click( object sender, EventArgs e )
        {
            //ParameterBox.ClearRecipes();
            sequenceClear();
        }

        public void sequenceClear()
        {
            propertyGrid1.SelectedObjects = null;
            flowLayoutPanel_RecipesBox.Controls.Clear();
            textBox_Crate.Enabled = false;
            textBox_Crate.Text = "";
            //textBox_ElcCapa.Enabled = false;
            //textBox_ElcCapa.Text = "";
            label_Crate.Enabled = false;
            //label_mah.Enabled = false;
            checkBox_Crate.Checked = false;
            _sequence.Clear();
            _helpLabel.Visible = true;

            textBox_area.Text = "";
            textBox_mass.Text = "";
            textBox_ElcCapa.Text = "";

        }
        private bool checkSave()
        {
            if ( _sequence == null ) return true;

            if ( _sequence.IsChanged )
            {
                switch ( MessageBox.Show( "시퀀스가 변경되었습니다. 변경 사항을 저장하시겠습니까?\r\n[아니오]를 선택할 경우 변경 사항을 모두 잃게 됩니다.", "Q730 알림 메시지", MessageBoxButtons.YesNoCancel ) )
                {
                    case DialogResult.Yes:
                        if ( _sequence.Name.Trim().Length == 0 )
                        {
                            MessageBox.Show( "시퀀스의 이름을 입력해야 합니다.", "Q730 알림 메시지" );
                            return false;
                        }

                        try
                        {
                            _sequence.Save();
                        }
                        catch ( QException ex )
                        {
                            foreach ( UserControl_RecipeItem item in flowLayoutPanel_RecipesBox.Controls )
                                item.RefreshError();

                            MessageBox.Show( ex.Message );
                            return false;
                        }
                        return true;

                    case DialogResult.No:
                        return true;

                    case DialogResult.Cancel:
                        return false;
                }
            }

            return true;
        }
        #endregion

        #region Sequence List Features
        private void timer_FileLoader_Tick( object sender, EventArgs e )
        {
            timer_FileLoader.Stop();

            Application.DoEvents();

            // 매 1초마다 Default Sequence Path의 파일 목록이 현재 시퀀스 빌더의 시퀀스 리스트와 일치하는지 검사하여, 실제 파일 목록과 일치하지 않는다면 갱신한다.

            if(textBox_Custom_Path.Text != "")
            {
                var files = new DirectoryInfo(textBox_Custom_Path.Text).GetFiles("*.seq");

                bool needRefresh = true;
                if (files.Length == DefaultPathSequences.Count)
                {
                    needRefresh = false;
                    for (var i = 0; i < files.Length; i++)
                    {
                        if (files[i].FullName != DefaultPathSequences[i])
                        {
                            needRefresh = true;
                            break;
                        }
                    }
                }

                if (!needRefresh)
                {
                    timer_FileLoader.Start();
                    return;
                }

                DefaultPathSequences.Clear();
                listView_SequenceList.Items.Clear();

                DefaultPathSequences.AddRange(files.Select(i =>
                {
                    listView_SequenceList.Items.Add(i.Name.Replace(i.Extension, ""));
                    return i.FullName;
                }));

                refreshColumnHeaderSize();

             
            }
            timer_FileLoader.Start();
        }
        private void listView_SequenceList_DoubleClick( object sender, EventArgs e )
        {
            if ( listView_SequenceList.SelectedIndices.Count == 0 || listView_SequenceList.SelectedIndices[0] == -1 ) return;

            SetSequence( Sequence.FromFile( DefaultPathSequences[listView_SequenceList.SelectedIndices[0]] ) );
        }
        private void toolStripMenuItem_Remove_Click( object sender, EventArgs e )
        {
            if ( listView_SequenceList.SelectedItems.Count == 0 ) return;

            switch ( SoftwareConfiguration.SequenceBuilder.RemoveSequenceFile )
            {
                case Answer.NotAlways:
                    if ( CustomMessageBox.Show( "정말 삭제하시겠습니까?", "Q730 알림 메시지", MessageBoxButtons.YesNo, "항상 묻지 않고 삭제", out bool answer ) == DialogResult.Yes )
                    {
                        if ( answer ) SoftwareConfiguration.SequenceBuilder.RemoveSequenceFile = Answer.AlwaysYes;

                        new FileInfo( DefaultPathSequences[listView_SequenceList.SelectedIndices[0]] ).Delete();
                    }
                    break;

                case Answer.AlwaysYes:
                    new FileInfo( DefaultPathSequences[listView_SequenceList.SelectedIndices[0]] ).Delete();
                    break;
            }
        }
        private void toolStripMenuItem_ShowInBrowser_Click( object sender, EventArgs e )
        {
            if ( listView_SequenceList.SelectedIndices.Count == 0 ) return;

            Process.Start( new FileInfo( DefaultPathSequences[listView_SequenceList.SelectedIndices[0]] ).Directory.FullName );
        }
        private void refreshColumnHeaderSize()
        {
            if ( listView_SequenceList.Items.Count == 0 )
            {
                listView_SequenceList.Columns[0].Width = listView_SequenceList.Width - 4;
            }
            else
            {
                var height = listView_SequenceList.Items[0].GetBounds( ItemBoundsPortion.Entire ).Height;
                if ( height * listView_SequenceList.Items.Count > listView_SequenceList.Height )
                {
                    listView_SequenceList.Columns[0].Width = listView_SequenceList.Width - SystemInformation.VerticalScrollBarWidth - 4;
                }
                else
                {
                    listView_SequenceList.Columns[0].Width = listView_SequenceList.Width - 4;
                }
            }
        }
        #endregion

        #region Command Key Features
        private List<Recipe> _clipboard = new List<Recipe>();
        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            Keys key = keyData & ~( Keys.Shift | Keys.Control );

            switch ( key )
            {
                case Keys.A:
                    if ( ( keyData & Keys.Control ) != 0 )
                    {
                        // Ctrl + A
                        clearSelection();
                        foreach ( UserControl_RecipeItem item in flowLayoutPanel_RecipesBox.Controls )
                        {
                            selectItem( item, false );
                        }
                    }
                    break;
                //return true;

                case Keys.C:
                    if ( ( keyData & Keys.Control ) != 0 )
                    {
                        // Ctrl + C
                        Clipboard.Clear();
                        _clipboard.Clear();
                        foreach ( UserControl_RecipeItem item in flowLayoutPanel_RecipesBox.Controls )
                        {
                            if ( item.Selected )
                            {
                                _clipboard.Add( item.Recipe.Clone() as Recipe );
                            }
                        }
                    }
                    break;
                //return true;

                case Keys.V:
                    if ( ( keyData & Keys.Control ) != 0 )
                    {
                        if ( _clipboard.Count != 0 )
                        {

                            var inputPosition = flowLayoutPanel_RecipesBox.Controls.Count;
                            for ( var i = 0; i < flowLayoutPanel_RecipesBox.Controls.Count; i++ )
                            {
                                if ( ( flowLayoutPanel_RecipesBox.Controls[i] as UserControl_RecipeItem ).Selected ) inputPosition = i + 1;
                            }

                            for ( var i = 0; i < _clipboard.Count; i++ )
                            {
                                addItem( inputPosition++, new UserControl_RecipeItem( _clipboard[i] ) );
                            }

                            resizeRecipeItems();
                        }
                    }
                    break;

                case Keys.Z:
                    // Undo
                    break;

                case Keys.Delete:
                    for ( var i = flowLayoutPanel_RecipesBox.Controls.Count - 1; i >= 0; i-- )
                    {
                        if ( ( flowLayoutPanel_RecipesBox.Controls[i] as UserControl_RecipeItem ).Selected )
                        {
                            flowLayoutPanel_RecipesBox.Controls.RemoveAt( i );
                            _sequence.RemoveAt( i );
                            propertyGrid1.SelectedObject = null;
                        }
                    }

                    var cycleCnt = _sequence._recipes.FindAll(x => x.Name == "Cycle").Count;
                    var loopCnt = _sequence._recipes.FindAll(x => x.Name == "Loop").Count;
                    var hasNotItemsBetweenCycleAndLoop = false;
                    var hasItemsBetweenCycleAndLoop = false;

                    for (int i = 0; i < _sequence._recipes.Count - 1; i++)
                    {
                        if ((_sequence._recipes[i].Name == "Cycle" && _sequence._recipes[i + 1].Name == "Loop")) 
                        {
                            hasNotItemsBetweenCycleAndLoop = true;
                        }
                        else
                        {
                            hasItemsBetweenCycleAndLoop = true;
                        }
                    }


                    comboBox_Loop.Items.Clear();

                    _sequence._crateList.Clear(); //20231212 장재훈 여기 클리어하는부분도 굉장히 중요 삭제했을 경우 

                    if (cycleCnt == loopCnt && hasItemsBetweenCycleAndLoop)
                    {
                        for (int i = 0; i < cycleCnt; i++)
                        {
                            comboBox_Loop.Items.Add(i + 1);
                        }

                        if (comboBox_Loop.Items.Count != 0)
                        {
                            comboBox_Loop.SelectedIndex = 0;
                            this.comboBox_Loop.Enabled = true;
                            this.textBox_Crate.Enabled = true;
                            this.label_Crate.Enabled = true;
                            this.textBox_ElcCapa.Enabled = true;
                            this.label_mah.Enabled = true;
                            this.button_Apply.Enabled = true;
                        }
                       
                    }
                    else
                    {
                        comboBox_Loop.Items.Clear();
                        comboBox_Loop.Items.Add(" Cycle Loop 세팅 필요");
                        if (comboBox_Loop.Items.Count != 0)
                        {
                            comboBox_Loop.SelectedIndex = 0;
                            this.comboBox_Loop.Enabled = false;
                            this.textBox_Crate.Enabled = false;
                            this.label_Crate.Enabled = false;
                            //this.textBox_ElcCapa.Enabled = false; //이건 계속 풀려있도록 수정
                            //this.label_mah.Enabled = false;
                            this.button_Apply.Enabled = false;
                        }
                    }

                    for (var i = 0; i < comboBox_Loop.Items.Count; i++)
                    {
                        if (comboBox_Loop.Items[0] == " Cycle Loop 세팅 필요")
                        {
                            _sequence._crateList.Add(new CrateInfo
                            {
                                index = -1,
                                cRate = 0,
                                elcCapa = _elcCaps,
                                checkCrate = false
                            });
                        }
                        else
                        {
                            _sequence._crateList.Add(new CrateInfo
                            {
                                index = (int)comboBox_Loop.Items[i],
                                cRate = 0,
                                elcCapa = _elcCaps,
                                checkCrate = false
                            });
                        }
                    }

                    if (_sequence._recipes.Count == 0)
                    {
                        sequenceClear();
                    }
                    break;
            }

            ////////저장을 위한 Crate 정보를 comboBox_Loop 아이템을 가지고 _sequence._crateList 갯수를 생성한다.
            //_sequence._crateList.Clear();
            //for (var i = 0; i < comboBox_Loop.Items.Count; i++)
            //{
            //    _sequence._crateList.Add(new CrateInfo
            //    {
            //        index = (int)comboBox_Loop.Items[i],
            //        cRate = 0,
            //        elcCapa = 0
            //    });
            //}

            return base.ProcessCmdKey( ref msg, keyData );
        }
        #endregion

        private void check_CrateMode()
        {
            if (!this.checkBox_Crate.Checked)
            {
                this.textBox_Crate.Enabled = false;
                this.label_Crate.Enabled = false;
                //this.label_mah.Enabled = false;
                //this.textBox_ElcCapa.Enabled = false;
            }
        }


        private void propertyGrid1_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
        {
            if ( ( e.ChangedItem.PropertyDescriptor.Attributes[typeof( CategoryAttribute )] as CategoryAttribute ).Category != "Safety Condition" ) return;

            if ( GlobalSafetyCondition )
            {
                UserControl_RecipeItem selectedItem = null;
                foreach ( UserControl_RecipeItem item in flowLayoutPanel_RecipesBox.Controls )
                {
                    if ( item.Selected && item.Recipe.SafetyCondition != null )
                    {
                        selectedItem = item;
                        break;
                    }
                }

                if ( selectedItem != null )
                {
                    _globalSafetyCondition = selectedItem.Recipe.SafetyCondition.Clone() as SafetyCondition;
                    for ( var i = 0; i < _sequence.Count; i++ )
                    {
                        _sequence[i].SafetyCondition?.CopyFrom( _globalSafetyCondition );
                    }
                }
            }
        }

        private void button_Custom_Load_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBox_Custom_Path.Text = dialog.SelectedPath;
                    // 선택한 폴더 경로를 사용하여 다른 작업을 수행합니다.
                    var files = new DirectoryInfo(dialog.SelectedPath).GetFiles("*.seq");


                    CustomPathSequences.Clear();
                    DefaultPathSequences.Clear();
                    listView_SequenceList.Items.Clear();


                    DefaultPathSequences.AddRange(files.Select(i =>
                    {
                        listView_SequenceList.Items.Add(i.Name.Replace(i.Extension, ""));
                        return i.FullName;
                    }));
                }
            }
        }

        private void checkBox_Crate_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.checkBox_Crate.Checked)
            {
                //button_Apply.Enabled = false;

                textBox_Crate.Enabled = false;
                label_CrateUnit.Enabled = false;
                
                //textBox_ElcCapa.Enabled = false;
                //label_ElcCapaUnit.Enabled = false;
                
                //label_mah.Enabled = false;
                //label_Crate.Enabled = false; //굳이 이거까지 장재훈

                if (comboBox_Loop.Items[0].ToString() == "")
                {
                    return;
                }
                else if (comboBox_Loop.Items.Count > 0 && comboBox_Loop.Items[0].ToString() != " Cycle Loop 세팅 필요")
                {
                    var selectLoopIndex = (int)comboBox_Loop.SelectedItem;
                    _sequence._crateList[selectLoopIndex - 1].checkCrate = false;
                }

            }
            else
            {
                textBox_Crate.Enabled = true;
                textBox_ElcCapa.Enabled = true;
                button_Apply.Enabled = true;
                label_CrateUnit.Enabled = true;
                label_ElcCapaUnit.Enabled = true;
                label_mah.Enabled = true;
                label_Crate.Enabled = true;
                if (comboBox_Loop.Items[0].ToString() == "")
                {
                    return;
                }
                else if (comboBox_Loop.Items.Count > 0 && comboBox_Loop.Items[0].ToString() != " Cycle Loop 세팅 필요")

                {
                    var selectLoopIndex = (int)comboBox_Loop.SelectedItem;
                    _sequence._crateList[selectLoopIndex - 1].checkCrate = true;

                }
            }
        }


        private void button_Apply_Click(object sender, EventArgs e)
        {
            double.TryParse(textBox_Crate.Text, out _outCrate);
            double.TryParse(textBox_ElcCapa.Text, out _outElcCapa);
            
            if (textBox_mass.Text != null) double.TryParse(textBox_mass.Text, out _sequence._batteryInfo.batteryMass);
            if (textBox_area.Text != null) double.TryParse(textBox_area.Text, out _sequence._batteryInfo.batteryArea);
            

            if (_outCrate == 0 && _outElcCapa == 0 && checkBox_Crate.Checked == true)
            {
                MessageBox.Show("C-Rate 값 또는 전극용량 값 오류 \n\n범위에 맞게 다시 입력 부탁드립니다.", "Q730 알림 메시지");
                return;
            }

            int loopSelectedIndex;
            if (comboBox_Loop.Enabled == false)
            {
                loopSelectedIndex = 0;
                var sequenceClone = _sequence._recipes.ToList();
                var item = sequenceClone.FindAll(x => x.Name.Contains("Charge") || x.Name.Contains("Discharge"));

                var convertCurrent = _outCrate * _outElcCapa;

                if (item.Count > 0)
                {
                    foreach (var recipe in item)
                    {
                        var temp = recipe.GetRecipeInfo();

                        for (var i = 0; i < temp.Count; i++)
                        {
                            if (temp[i].Name == "Current")
                            {
                                double current = Convert.ToDouble(temp[i].GetValue(recipe));

                                temp[i].SetValue(recipe, convertCurrent);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Charge or DisCharge Step이 존재하지 않습니다", "Q730 알림 메시지");
                    checkBox_Crate.Checked = false;
                    return;
                }
            }
            else
            {
                if (comboBox_Loop.Text != null)
                {
                    loopSelectedIndex = int.Parse(comboBox_Loop.Text);
                }
                else
                {
                    return;
                }

                var cycleIndex = new List<int>();
                var loopIndex = new List<int>();
                for (int i = 0; i < _sequence._recipes.Count; i++)
                {
                    if (_sequence._recipes[i].Name == "Cycle")
                    {
                        cycleIndex.Add(i);
                    }

                    if (_sequence._recipes[i].Name == "Loop")
                    {
                        loopIndex.Add(i);
                    }
                }

                var sequenceClone = _sequence._recipes.ToList();
                var selectedIndex = comboBox_Loop.SelectedIndex;
                if (selectedIndex == -1)
                {
                    MessageBox.Show("Loop를 선택 해주세요.", "Q730 알림 메시지");
                    return;
                }
                else
                {
                    var isLastIndex = selectedIndex == comboBox_Loop.Items.Count - 1;

                    if (comboBox_Loop.SelectedIndex == 0)
                    {
                        //sequenceClone.RemoveRange(loopIndex[0], (sequenceClone.Count - 1) - loopIndex[0]);
                        sequenceClone.RemoveRange(loopIndex[0], sequenceClone.Count - loopIndex[0]);
                    }
                    else if (isLastIndex)
                    {
                        sequenceClone.RemoveRange(0, cycleIndex[loopSelectedIndex - 1]);
                    }
                    else
                    {
                        sequenceClone = sequenceClone.GetRange(cycleIndex[loopSelectedIndex - 1],
                            loopIndex[loopSelectedIndex - 1] - cycleIndex[loopSelectedIndex - 1] + 1);
                    }

                    var Item = sequenceClone.FindAll(x => x.Name.Contains("Charge") || x.Name.Contains("Discharge"));

                    var convertCurrent = _outCrate * _outElcCapa;
                    _sequence._crateList[comboBox_Loop.SelectedIndex].cRate = _outCrate;

                    foreach (var data in _sequence._crateList) //장재훈 전체 용량 초기화
                    {
                        data.elcCapa = _outElcCapa;
                    }
                    _sequence._crateList[comboBox_Loop.SelectedIndex].elcCapa = _outElcCapa;
                    _sequence._crateList[comboBox_Loop.SelectedIndex].checkCrate = checkBox_Crate.Checked;

                    if (Item.Count > 0)
                    {
                        foreach (var recipe in Item)
                        {

                            if (_selectIconItem.Recipe == recipe)
                            {
                                var temp = recipe.GetRecipeInfo();

                                for (var i = 0; i < temp.Count; i++)
                                {
                                    if (temp[i].Name == "Current")
                                    {
                                        double current = Convert.ToDouble(temp[i].GetValue(recipe));

                                        temp[i].SetValue(recipe, convertCurrent);
                                    }
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        MessageBox.Show("Charge or DisCharge Step이 존재하지 않습니다", "Q730 알림 메시지");
                        return;
                    }

                }
            }
        }

        private void comboBox_Loop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_Loop.Enabled && _sequence._crateList.Count > 0 )
            {
                if (comboBox_Loop.SelectedIndex != -1)
                {
                    textBox_Crate.Text = _sequence._crateList[comboBox_Loop.SelectedIndex].cRate.ToString();

                    textBox_ElcCapa.Text = _sequence._crateList[comboBox_Loop.SelectedIndex].elcCapa.ToString();

                    checkBox_Crate.Checked = _sequence._crateList[comboBox_Loop.SelectedIndex].checkCrate;

                    textBox_area.Text = _sequence._batteryInfo.batteryArea.ToString();

                    textBox_mass.Text = _sequence._batteryInfo.batteryMass.ToString();
                }
            
            }
        }
    }
}
