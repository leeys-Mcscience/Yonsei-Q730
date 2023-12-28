using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataViewer.Class;

namespace DataViewer
{
    public partial class Form_AxisSettingEditor : Form
    {
        private List<AxisSetting> _list;
        public Form_AxisSettingEditor(List<AxisSetting> list)
        {
            InitializeComponent();

            _list = list;
            refreshListView();
        }

        private void refreshListView()
        {
            listView1.Items.Clear();

            for(var i = 0; i < _list.Count; i++ )
            {
                listView1.Items.Add( _list[i].ToString() );
            }
        }

        private void button_Add_Click( object sender, EventArgs e )
        {
            _list.Add( new AxisSetting() );

            refreshListView();
            listView1.Items[listView1.Items.Count - 1].Selected = true;
            listView1.Focus();
        }

        private void button_Remove_Click( object sender, EventArgs e )
        {
            if ( listView1.SelectedIndices.Count == 0 || listView1.SelectedIndices[0] == -1 ) return;

            var removed = listView1.SelectedIndices[0];

            _list.RemoveAt( removed );
            refreshListView();

            if ( listView1.Items.Count == 0 ) return;
            if ( listView1.Items.Count >= removed ) removed--;
            listView1.Items[removed].Selected = true;
            listView1.Focus();
        }

        private void button_Up_Click( object sender, EventArgs e )
        {
            if ( listView1.SelectedIndices.Count == 0 || listView1.SelectedIndices[0] == -1 ) return;
            var index = listView1.SelectedIndices[0];
            if ( index == 0 )
            {
                listView1.Focus();
                return;
            }

            var tmp = _list[index];
            _list.RemoveAt( index );
            _list.Insert( index - 1, tmp );

            refreshListView();
            listView1.Items[index - 1].Selected = true;
            listView1.Focus();
        }

        private void button_Down_Click( object sender, EventArgs e )
        {
            if ( listView1.SelectedIndices.Count == 0 || listView1.SelectedIndices[0] == -1 ) return;
            var index = listView1.SelectedIndices[0];
            if (index == listView1.Items.Count - 1 )
            {
                listView1.Focus();
                return;
            }

            var tmp = _list[index];
            _list.RemoveAt( index );
            _list.Insert( index + 1, tmp );

            refreshListView();
            listView1.Items[index + 1].Selected = true;
            listView1.Focus();
        }

        private void button_OK_Click( object sender, EventArgs e )
        {
            DialogResult = DialogResult.OK;
        }

        private void button_Cancel_Click( object sender, EventArgs e )
        {
            DialogResult = DialogResult.Cancel;
        }

        private void listView1_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( listView1.SelectedIndices.Count == 0 || listView1.SelectedIndices[0] == -1 ) return;

            propertyGrid1.SelectedObject = _list[listView1.SelectedIndices[0]];
        }

        private void propertyGrid1_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
        {
            for(var i = 0; i < listView1.Items.Count; i++ )
            {
                listView1.Items[i].Text = _list[i].ToString();
            }
        }
    }
}
