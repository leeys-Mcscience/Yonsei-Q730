using McQLib.Device;
using System;

using System.Windows.Forms;

namespace McQLib.GUI
{
    public partial class Form_QueueMonitor : Form
    {
        Communicator _communicator;
        public Form_QueueMonitor(Communicator communicator)
        {
            InitializeComponent();

            _communicator = communicator;

            timer1.Start();
        }

        private void timer1_Tick( object sender, EventArgs e )
        {
            var counts = _communicator.GetAllQueueCounts();

            label_Send.Text = counts[0].ToString();
            label_ReceiveWaiter.Text = counts[1].ToString();
            label_Byte.Text = counts[2].ToString();
            label_ReceivePacket.Text = counts[3].ToString();
            //label_Error.Text = counts[4].ToString();
        }
    }
}
