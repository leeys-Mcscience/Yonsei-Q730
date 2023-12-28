using McQLib.Device;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Q730.UserControls
{
    public partial class UserControl_GraphGridView : UserControl
    {
        public UserControl_GraphGridView()
        {
            InitializeComponent();

            Dock = DockStyle.Fill;

            
        }

        public void BindChannels( Channel[] channels )
        {
            _channels = channels;
            _allocating = new int[_channels.Length];

            numericUpDown_Column.Enabled = numericUpDown_Row.Enabled = button_Apply.Enabled = true;

            //_controls.Add( new UserControl_ChannelBoxView( this, channels[0], _channels.Length ) );
            tableLayoutPanel_Grid.Controls.Add( _controls[0] );
        }

        /// <summary>
        /// 지정된 인덱스의 다음 순서에 해당하는 채널 객체를 반환합니다.
        /// <br>인덱스가 Channel[]의 마지막 인덱스인 경우 인덱스 0의 Channel을 반환합니다.</br>
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public Channel GetNextChannel( int current )
        {
            if( current == -1 ) return null;

            var next = current + 1;
            if( next >= _channels.Length ) next = 0;

            _allocating[current]--;
            _allocating[next]++;
            return _channels[next];
        }
        /// <summary>
        /// 지정된 인덱스의 이전 순서에 해당하는 채널 객체를 반환합니다.
        /// <br>인덱스가 Channel[]의 첫 인덱스인 경우 마지막 인덱스의 Channel을 반환합니다.</br>
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public Channel GetBeforeChannel(int current )
        {
            if( current == -1 ) return null;

            var before = current - 1;
            if( before < 0 ) before = _channels.Length - 1;

            _allocating[current]--;
            _allocating[before]++;
            return _channels[before];
        }
        public Channel GetChannel(int current, int target )
        {
            _allocating[current]--;
            if( 0 <= target && target < _channels.Length )
            {
                _allocating[target]++;
                return _channels[target];
            }
            else return null;
        }

        private Channel[] _channels;
        private int[] _allocating;
        private List<UserControl_ChannelDetailView> _controls = new List<UserControl_ChannelDetailView>();

        private int getAnyChannel()
        {
            for(var i = 0; i < _allocating.Length; i++ )
            {
                if( _allocating[i] == 0 )
                {
                    _allocating[i]++;
                    return i;
                }
            }

            return 0;
        }
        private void button_Apply_Click( object sender, EventArgs e )
        {
            int row = ( int )numericUpDown_Row.Value, column = ( int )numericUpDown_Column.Value;

            _controls.Clear();

            for( var i = 0; i < tableLayoutPanel_Grid.RowStyles.Count; i++ )
            {
                for( var j = 0; j < tableLayoutPanel_Grid.ColumnStyles.Count; j++ )
                {
                    _controls.Add( tableLayoutPanel_Grid.Controls[i * tableLayoutPanel_Grid.ColumnStyles.Count + j] as UserControl_ChannelDetailView );
                }
            }

            tableLayoutPanel_Grid.Controls.Clear();

            if( _controls.Count > row * column )
            {
                for( var i = 0; i < _controls.Count - (row * column); i++ ) _controls.RemoveAt( _controls.Count - 1 );
            }
            else
            {
                //while( _controls.Count != row * column ) _controls.Add( new UserControl_ChannelBoxView( this, _channels[getAnyChannel()], _channels.Length ) );
            }

            tableLayoutPanel_Grid.ColumnStyles.Clear();
            tableLayoutPanel_Grid.RowStyles.Clear();

            tableLayoutPanel_Grid.RowCount = row;
            tableLayoutPanel_Grid.ColumnCount = column;

            for( var i = 0; i < row; i++ )
            {
                tableLayoutPanel_Grid.RowStyles.Add( new RowStyle( SizeType.Percent, 100 ) );
            }
            for( var j = 0; j < column; j++ )
            {
                tableLayoutPanel_Grid.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100 ) );
            }

            Application.DoEvents();

            var index = 0;
            for( var i = 0; i < row; i++ )
            {
                for( var j = 0; j < column; j++ )
                {
                    tableLayoutPanel_Grid.Controls.Add( _controls[index++], j, i );
                }
            }

            Start();
        }

        public void Start()
        {
            foreach( UserControl_ChannelDetailView box in tableLayoutPanel_Grid.Controls )
            {
                box.Start();
            }
        }
        public void Stop()
        {
            foreach( UserControl_ChannelDetailView box in tableLayoutPanel_Grid.Controls )
            {
                box.Stop();
            }
        }
    }
}
