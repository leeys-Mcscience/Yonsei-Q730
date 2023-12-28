using McQLib.Recipes;
using System;
using System.Windows.Forms;

namespace Q730.UserControls
{
    public partial class UserControl_SequenceViewer : UserControl
    {
        public UserControl_SequenceViewer()
        {
            InitializeComponent();
        }

        Sequence _sequence = null;
        private bool _isDetail;

        public void SetSequence( Sequence sequence, bool detail = false )
        {
            _isDetail = detail;

            flowLayoutPanel1.Controls.Clear();

            //if( (_sequence = sequence) == null )
            //{
            //    return;
            //}
            _sequence = sequence;

            if( detail )
            {
                flowLayoutPanel1.Controls.Add( new UserControl_RecipeItem( RecipeFactory.CreateInstance( RecipeType.Idle ) ) );
            }

            if( _sequence != null )
            {
                for( var i = 0; i < _sequence.Count; i++ )
                {
                    var item = new UserControl_RecipeItem( _sequence[i] );
                    if ( _sequence[i].GetRecipeType() == RecipeType.Label ) item.SetToLabel();
                    item.Now = false;
                    flowLayoutPanel1.Controls.Add( item );
                    item.RefreshText();
                }
            }

            if( detail )
            {
                flowLayoutPanel1.Controls.Add( new UserControl_RecipeItem( RecipeFactory.CreateInstance( RecipeType.End ) ) );
            }

            Application.DoEvents();

            resize();
        }
        public Sequence GetSequence()
        {
            return _sequence;
        }
        public void Undetail()
        {
            SetSequence( _sequence );
        }

        int lastSet = -1;
        public void SelectItem( int stepNo )
        {
            if ( _sequence == null ) return;

            stepNo = recalcIndex( stepNo );

            if( stepNo == lastSet && (flowLayoutPanel1.Controls[stepNo] as UserControl_RecipeItem).Now ) return;
            lastSet = stepNo;

            for( var i = 0; i < flowLayoutPanel1.Controls.Count; i++ )
            {
                (flowLayoutPanel1.Controls[i] as UserControl_RecipeItem).Now = false;
            }

            //if( stepNo == -1 ) return;

            (flowLayoutPanel1.Controls[stepNo] as UserControl_RecipeItem).Now = true;

            // 여기서 현재 선택된 레시피의 Location을 찾아서 화면에 들어오도록 스크롤을 해줘야 함..
            flowLayoutPanel1.ScrollControlIntoView( flowLayoutPanel1.Controls[stepNo] );
        }
        // 이곳에서 장비상 stepNo를 시퀀스상 index로 변환한다.
        private int recalcIndex( int stepNo )
        {
            // stepNo가 -1임은 현재 해당 채널이 Idle이므로 그냥 -1 반환
            if( stepNo == -1 ) return 0;

            var index = stepNo;

            // 라벨 레시피의 경우 실제 장비 상에는 존재하지 않는 경우 이므로 stepNo가 label의 인덱스인 경우 실제 인덱스는 label의 다음 칸일 것이다.
            // 따라서 0부터 stepNo까지 존재하는 모든 label 레시피의 개수만큼 인덱스를 증가시킨다.
            for( var i = 0; i <= stepNo; i++ ) if( _sequence.Count > i && _sequence[i] is McQLib.Recipes.Label ) index++;

            return index + 1; // Idle 레시피 1 증가
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
