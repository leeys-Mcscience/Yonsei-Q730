using McQLib.Recipes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Q730.UserControls
{
    public partial class UserControl_SequenceProgressBox : UserControl
    {
        public UserControl_SequenceProgressBox()
        {
            InitializeComponent();

            flowLayoutPanel1.VerticalScroll.Enabled = false;
        }

        int _stepNo = -1;
        public Sequence Sequence
        {
            get => _sequence;
        }
        Sequence _sequence;

        public void SetSequence( Sequence sequence )
        {
            _sequence = sequence;
            DrawIcons();
            SetStep( -1 );
        }

        private const int MIN_SIZE = 45;
        private const int MAX_SIZE = 55;

        public void DrawIcons()
        {
            flowLayoutPanel1.Controls.Clear();

            if( _sequence == null ) return;

            flowLayoutPanel1.Controls.Add( new PictureBox
            {
                Size = new Size( MIN_SIZE, MIN_SIZE ),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.None,
                //Image = Properties.Resources.Icon_Idle
            } );

            for( var i = 0; i < _sequence.Count; i++ )
            {
                var box = new PictureBox
                {
                    Size = new Size( MIN_SIZE, MIN_SIZE ),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BorderStyle = BorderStyle.FixedSingle,
                    Anchor = AnchorStyles.None
                };

                box.Image = _sequence[i].Icon;

                flowLayoutPanel1.Controls.Add( box );
            }

            flowLayoutPanel1.Controls.Add( new PictureBox
            {
                Size = new Size( MIN_SIZE, MIN_SIZE ),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.None,
                //Image = Properties.Resources.Icon_End
            } );
        }

        public void SetStep( int stepNo )
        {
            if( _stepNo == stepNo || flowLayoutPanel1.Controls.Count == 0 ) return;

            flowLayoutPanel1.Controls[_stepNo + 1].Size = new Size( MIN_SIZE, MIN_SIZE );
            flowLayoutPanel1.Controls[_stepNo + 1].BackColor = SystemColors.Control;

            _stepNo = stepNo;

            flowLayoutPanel1.Controls[_stepNo + 1].Size = new Size( MAX_SIZE, MAX_SIZE );
            flowLayoutPanel1.Controls[_stepNo + 1].BackColor = Color.Red;
        }
    }
}
