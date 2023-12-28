using McQLib.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Q730.UserControls.Graphs
{
    public partial class UserControl_TraGraph : UserControl, IGraphControl
    {
        public UserControl_TraGraph()
        {
            InitializeComponent();
        }

        public void AddData(MeasureData data ) { }
        public void AddData(MeasureData data, McQLib.Device.Channel channel) { }
        public void RefreshGraph() { }
        public void ClearGraph() { }
    }
}
