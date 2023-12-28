using McQLib.Core;
using McQLib.Recipes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Q730
{
    public partial class Form_SequenceBuilder : Form
    {
        UserControls.UserControl_SequenceBuilder userControl_SequenceBox1;

        public Form_SequenceBuilder()
        {
            InitializeComponent();

            userControl_SequenceBox1 = new UserControls.UserControl_SequenceBuilder()
            {
                Dock = DockStyle.Fill
            };
            Controls.Add( userControl_SequenceBox1 );
        }

        public Form_SequenceBuilder(string sequencePath ) : this()
        {
            userControl_SequenceBox1.SetSequence( Sequence.FromFile( sequencePath ) );
        }

        private void Form_SequenceBuilder_FormClosing( object sender, FormClosingEventArgs e )
        {

        }
    }
}
