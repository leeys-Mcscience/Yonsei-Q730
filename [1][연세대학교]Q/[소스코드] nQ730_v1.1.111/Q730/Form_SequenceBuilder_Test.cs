using McQLib.Core;
using McQLib.Device;
using McQLib.Recipes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Q730
{
    public partial class Form_SequenceBuilder_Test : Form
    {
        UserControls.UserControl_SequenceBuilder userControl_SequenceBox1;

        public Form_SequenceBuilder_Test()
        {
            InitializeComponent();

            userControl_SequenceBox1 = new UserControls.UserControl_SequenceBuilder()
            {
                Location = new System.Drawing.Point( 0, 0 )
            };
            Controls.Add( userControl_SequenceBox1 );
        }

        public Sequence Result = null;
        public bool Old = false;

        private void button1_Click( object sender, EventArgs e )
        {
            try
            {
                Result = userControl_SequenceBox1.GetSequence();
                Old = checkBox_Old.Checked;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch(QException ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void checkBox1_CheckedChanged( object sender, EventArgs e )
        {
            userControl_SequenceBox1.Indentation = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged( object sender, EventArgs e )
        {
            userControl_SequenceBox1.AutoAddCycleLoopPair = checkBox2.Checked;
        }
    }
}
