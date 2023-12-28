using McQLib.Device;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Q730.UserControls
{
    public partial class UserControl_ChannelGridView : UserControl
    {
        public UserControl_ChannelGridView()
        {
            InitializeComponent();

            Dock = DockStyle.Fill;

            dataGridView1.GetType().GetProperty( "DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance ).SetValue( dataGridView1, true );
        }

        [Browsable( true )]
        public event EventHandler ChannelDoubleClick;

        private Channel[] _channels;
        private int _rowcol;

        public int RowCol => _rowcol;
        public void BindChannels( Channel[] channels )
        {
            _channels = channels;

            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            if ( _channels.Length == 0 ) return;

            int col = 8;
            int row = _channels.Length / 8;
            if ( _channels.Length % 8 != 0 ) row++;

            row = ( _channels.Length - 1 ) / col + 1;

            for ( var i = 0; i < col; i++ ) dataGridView1.Columns.Add( "", "" );
            for ( var i = 0; i < row; i++ ) dataGridView1.Rows.Add();

            var index = 0;
            for ( var i = 0; i < row; i++ )
            {
                for ( var j = 0; j < col && index < _channels.Length; j++ )
                {
                    changeCell( dataGridView1.Rows[i].Cells[j], index, _channels[index++] );
                }
            }
        }

        public void Start() => timer_Updater.Start();
        public void Stop() => timer_Updater.Stop();

        private void changeCell( DataGridViewCell cell, int index, Channel channel )
        {
            switch ( channel.State )
            {
                case State.IDLE:
                    cell.Style.BackColor = SoftwareConfiguration.GRID.IdleBackColor;
                    cell.Style.ForeColor = SoftwareConfiguration.GRID.IdleForeColor;
                    break;

                case State.RUN:
                    cell.Style.BackColor = SoftwareConfiguration.GRID.RunBackColor;
                    cell.Style.ForeColor = SoftwareConfiguration.GRID.RunForeColor;
                    break;

                case State.PAUSED:
                    cell.Style.BackColor = SoftwareConfiguration.GRID.PausedBackColor;
                    cell.Style.ForeColor = SoftwareConfiguration.GRID.PausedForeColor;
                    break;

                case State.SAFETY:
                    cell.Style.BackColor = SoftwareConfiguration.GRID.SafetyBackColor;
                    cell.Style.ForeColor = SoftwareConfiguration.GRID.SafetyForeColor;
                    break;

                case State.ERROR:
                    cell.Style.BackColor = SoftwareConfiguration.GRID.ErrorBackColor;
                    cell.Style.ForeColor = SoftwareConfiguration.GRID.ErrorForeColor;
                    break;
            }
            
                cell.Value = $"[CH : {index + 1}] [Step : {channel.RecipeName}]\r\n" +
                             $"{SoftwareConfiguration.Measurement.VoltageUnit.GetString(channel.Voltage)}{SoftwareConfiguration.Measurement.VoltageUnit.UnitString}\r\n" +
                             $"{SoftwareConfiguration.Measurement.CurrentUnit.GetString(channel.Current)}{SoftwareConfiguration.Measurement.CurrentUnit.UnitString}\r\n" +
                             $"{SoftwareConfiguration.Measurement.CapacityUnit.GetString(channel.Capacity)}{SoftwareConfiguration.Measurement.CapacityUnit.UnitString}\r\n" +
                             $"[{McQLib.Core.Util.ConvertMsToString(channel.TotalTime)}]";

        }
        private void timer_Updater_Tick( object sender, EventArgs e )
        {
            panel_IdleColor.BackColor = SoftwareConfiguration.GRID.IdleBackColor;
            panel_RunColor.BackColor = SoftwareConfiguration.GRID.RunBackColor;
            panel_PausedColor.BackColor = SoftwareConfiguration.GRID.PausedBackColor;
            panel_SafetyColor.BackColor = SoftwareConfiguration.GRID.SafetyBackColor;
            panel_ErrorColor.BackColor = SoftwareConfiguration.GRID.ErrorBackColor;

            if ( _channels == null ) return;

            var index = 0;
            for ( var i = 0; i < dataGridView1.Rows.Count; i++ )
            {
                for ( var j = 0; j < dataGridView1.Columns.Count && index < _channels.Length; j++ )
                {
                    changeCell( dataGridView1.Rows[i].Cells[j], index, _channels[index++] );
                }
            }
        }

        private void dataGridView1_SelectionChanged( object sender, EventArgs e )
        {
            dataGridView1.ClearSelection();
        }

        private void dataGridView1_CellDoubleClick( object sender, DataGridViewCellEventArgs e )
        {
            ChannelDoubleClick?.Invoke( this, e );
        }

        int lastHeight = 0;
        private void dataGridView1_SizeChanged( object sender, EventArgs e )
        {
            if ( dataGridView1.ClientRectangle.Height == lastHeight ) return;

            lastHeight = dataGridView1.ClientRectangle.Height;
        }
    }
}
