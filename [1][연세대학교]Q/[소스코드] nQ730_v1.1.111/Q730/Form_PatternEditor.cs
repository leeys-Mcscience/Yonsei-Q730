using McQLib.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Q730
{
    public partial class Form_PatternEditor : Form
    {
        enum BiasMode
        {
            CC,
            CP
        }

        const int MAX_COUNT = 100;
        public static string DefaultDirectory => Path.Combine( Assembly.GetEntryAssembly().Location.Replace( Assembly.GetEntryAssembly().ManifestModule.Name, "" ), "Pattern" );
        private void init()
        {
            loadFiles();

            initGraph();
            refreshGraph();
        }
        public Form_PatternEditor( string filename )
        {
            InitializeComponent();

            if( filename != null )
            {
                load( filename );
            }

            init();
        }

        PatternData _pattern = new PatternData( PatternBiasMode.CC, 10 );

        void initGraph()
        {
            graph.GraphPane.Title.Text = "";

            graph.GraphPane.XAxis.Title.Text = "Time(ms)";
            graph.GraphPane.XAxis.Title.FontSpec.Size = 15.0f;
            graph.GraphPane.XAxis.Title.FontSpec.IsBold = true;
            graph.GraphPane.XAxis.Title.FontSpec.Family = "맑은 고딕";

            graph.GraphPane.YAxis.Title.Text = "Current(A)";
            graph.GraphPane.YAxis.Title.FontSpec.Size = 15.0f;
            graph.GraphPane.YAxis.Title.FontSpec.IsBold = true;
            graph.GraphPane.YAxis.Title.FontSpec.Family = "맑은 고딕";

            //graph.GraphPane.XAxis.MajorTic.PenWidth = 2;
            graph.GraphPane.XAxis.MajorGrid.IsVisible = true;
            graph.GraphPane.XAxis.MajorTic.IsOutside = false;
            graph.GraphPane.XAxis.MinorTic.IsOutside = false;
            graph.GraphPane.XAxis.Color = Color.Black;
            graph.GraphPane.XAxis.MajorGrid.Color = Color.Black;
            graph.GraphPane.XAxis.MajorTic.Color = Color.Black;

            //graph.GraphPane.YAxis.MajorTic.PenWidth = 2;
            graph.GraphPane.YAxis.MajorGrid.IsVisible = true;
            graph.GraphPane.YAxis.MajorTic.IsOutside = false;
            graph.GraphPane.YAxis.MinorTic.IsOutside = false;
            graph.GraphPane.YAxis.Color = Color.Black;
            graph.GraphPane.YAxis.MajorGrid.Color = Color.Black;
            graph.GraphPane.YAxis.MajorTic.Color = Color.Black;

            //graph.GraphPane.XAxis.
        }
        private void refreshGraph()
        {
            graph.GraphPane.CurveList.Clear();

            var x = new List<double>();
            var y = new List<double>();

            var lastPosition = 0.0;

            for( var i = 0; i < _pattern.Count; i++ )
            {
                x.Add( lastPosition );
                y.Add( _pattern[i].Value );

                x.Add( lastPosition = lastPosition + (_pattern[i].Count * _pattern.PulseWidth) );
                y.Add( _pattern[i].Value );
            }

            //foreach( ListViewItem item in listView1.Items )
            //{
            //    var value = double.Parse( item.SubItems[1].Text );
            //    var width = double.Parse( item.SubItems[2].Text );

            //    x.Add( lastPosition );
            //    y.Add( value );

            //    x.Add( lastPosition = lastPosition + width );
            //    y.Add( value );
            //}

            var line = graph.GraphPane.AddCurve( "Pulse", x.ToArray(), y.ToArray(), Color.Red, ZedGraph.SymbolType.None );
            line.Line.Width = 2.5f;

            //if( listView1.Items.Count != 0 )
            if( _pattern.Count != 0 )
            {
                var min = y.Min();
                var max = y.Max();

                graph.GraphPane.XAxis.Scale.Min = 0;
                graph.GraphPane.XAxis.Scale.Max = lastPosition;

                if( min < 0 ) graph.GraphPane.YAxis.Scale.Min = min * 1.2;
                else graph.GraphPane.YAxis.Scale.Min = min * 0.8;

                if( max < 0 ) graph.GraphPane.YAxis.Scale.Max = max * 0.8;
                else graph.GraphPane.YAxis.Scale.Max = max * 1.2;

                if( graph.GraphPane.YAxis.Scale.Min == graph.GraphPane.YAxis.Scale.Max )
                {
                    graph.GraphPane.YAxis.Scale.Min = -1;
                    graph.GraphPane.YAxis.Scale.Max = 1;
                }
            }

            graph.Invalidate();
            graph.AxisChange();
        }
        void setRadioButtonEnabled()
        {
            radioButton_100ms.Enabled =
            radioButton_10ms.Enabled =
            radioButton_CC.Enabled =
            radioButton_CP.Enabled = listView1.Items.Count == 0;
        }
        // 현재 리스트뷰에 등록된 전체 펄스 아이템을 패킷에 넣었을 때 DATA 필드 개수가 몇 개나 나오는지 계산하는 메서드
        //int getPatternDataCount()
        //{
        //    var count = 0;
        //    for(var i = )
        //    foreach( ListViewItem item in listView1.Items )
        //    {
        //        var width = double.Parse( item.SubItems[2].Text );

        //        count += ( int )(width / _pulseWidth);
        //    }

        //    return count;
        //}

        private void radioButton_PulseWidth_CheckedChanged( object sender, EventArgs e )
        {
            if( radioButton_10ms.Checked )
            {
                _pattern = new PatternData( _pattern.BiasMode, 10 );
            }
            else
            {
                _pattern = new PatternData( _pattern.BiasMode, 100 );
            }
        }

        private void listView1_SelectedIndexChanged( object sender, EventArgs e )
        {
            if( listView1.SelectedIndices.Count == 0 || listView1.SelectedIndices[0] == -1 ) return;

            refreshGraph();

            var index = listView1.SelectedIndices[0];
            //var value = double.Parse( listView1.Items[listView1.SelectedIndices[0]].SubItems[1].Text );
            //var width = double.Parse( listView1.Items[listView1.SelectedIndices[0]].SubItems[2].Text );

            var lastPosition = 0.0;
            for( var i = 0; i < index; i++ )
            {
                //lastPosition += double.Parse( listView1.Items[i].SubItems[2].Text );
                lastPosition += _pattern[i].Count * _pattern.PulseWidth;
            }

            var line = graph.GraphPane.AddCurve( "Select", new double[2] { lastPosition, lastPosition + (_pattern[index].Count * _pattern.PulseWidth) },
                                                           new double[2] { _pattern[index].Value, _pattern[index].Value }, Color.Blue, ZedGraph.SymbolType.None );
            line.Line.Width = 8.0f;

            graph.Invalidate();
            graph.AxisChange();
        }

        private void radioButton_BiasMode_CheckedChanged( object sender, EventArgs e )
        {
            if( radioButton_CC.Checked )
            {
                graph.GraphPane.YAxis.Title.Text = label_Value.Text = "Current(A)";
                _pattern = new PatternData( PatternBiasMode.CC, _pattern.PulseWidth );
            }
            else
            {
                graph.GraphPane.YAxis.Title.Text = label_Value.Text = "Power(W)";
                _pattern = new PatternData( PatternBiasMode.CC, _pattern.PulseWidth );
            }

            graph.AxisChange();
            graph.Invalidate();
        }

        private void button_Add_Click( object sender, EventArgs e )
        {
            if( !double.TryParse( textBox_Value.Text, out double value ) )
            {
                MessageBox.Show( "올바른 Value를 입력하세요.", "Q730 알림 메시지" );
                return;
            }

            if( _pattern.TotalCount + numericUpDown_Count.Value > MAX_COUNT )
            {
                MessageBox.Show( "최대 패턴 길이를 초과합니다.", "Q730 알림 메시지" );
                return;
            }

            var item = new ListViewItem( (listView1.Items.Count + 1).ToString() );
            item.SubItems.Add( value.ToString() );
            item.SubItems.Add( (_pattern.PulseWidth * ( int )numericUpDown_Count.Value).ToString() );

            listView1.Items.Add( item );

            _pattern.Add( new PatternItem( value, ( int )numericUpDown_Count.Value ) );

            _isChanged = true;

            refreshGraph();
            setRadioButtonEnabled();
        }
        private void button_Up_Click( object sender, EventArgs e )
        {
            if( listView1.SelectedIndices.Count == 0 ) return;
            if( listView1.SelectedIndices[0] == 0 ) return;

            var index = listView1.SelectedIndices[0];
            var item = listView1.Items[index];

            listView1.Items.RemoveAt( index );
            listView1.Items.Insert( index - 1, item );

            var p = _pattern[index];
            _pattern.RemoveAt( index );
            _pattern.Insert( index - 1, p );

            _isChanged = true;

            refreshGraph();

            item.Selected = false;
            item.Selected = true;
            listView1.Focus();
        }
        private void button_Down_Click( object sender, EventArgs e )
        {
            if( listView1.SelectedIndices.Count == 0 ) return;
            if( listView1.SelectedIndices[0] == listView1.Items.Count - 1 ) return;

            var index = listView1.SelectedIndices[0];
            var item = listView1.Items[index];

            listView1.Items.RemoveAt( index );
            listView1.Items.Insert( index + 1, item );

            var p = _pattern[index];
            _pattern.RemoveAt( index );
            _pattern.Insert( index + 1, p );

            _isChanged = true;

            refreshGraph();

            item.Selected = false;
            item.Selected = true;
            listView1.Focus();
        }
        private void button_Remove_Click( object sender, EventArgs e )
        {
            if( listView1.SelectedIndices.Count == 0 ) return;
            var index = listView1.SelectedIndices[0];

            listView1.Items.RemoveAt( index );

            _pattern.RemoveAt( index );

            _isChanged = true;

            refreshGraph();
            setRadioButtonEnabled();
        }
        private void button_Clear_Click( object sender, EventArgs e )
        {
            if( MessageBox.Show( "모든 패턴 데이터가 사라집니다. 계속하시겠습니까?", "Q730 알림 메시지", MessageBoxButtons.YesNo ) == DialogResult.Yes )
            {
                listView1.Items.Clear();

                _pattern.Clear();

                _isChanged = true;

                refreshGraph();
                setRadioButtonEnabled();
            }
        }

        private void textBox_Value_KeyUp( object sender, KeyEventArgs e )
        {
            if( e.KeyCode == Keys.Enter ) button_Add.PerformClick();
        }

        bool _isChanged = false;
        List<string> _patternFiles = new List<string>();
        private void loadFiles()
        {
            listView2.Items.Clear();
            _patternFiles.Clear();

            var dinfo = new DirectoryInfo( DefaultDirectory );
            if( !dinfo.Exists ) dinfo.Create();

            var files = dinfo.GetFiles( "*.ptn" );

            foreach( var f in files )
            {
                _patternFiles.Add( f.FullName );
                listView2.Items.Add( f.Name );
            }
        }
        public string LastLoaded = "";

        private void load( string filename )
        {
            if( filename == null || filename.Length == 0 ) return;
            else if( !new FileInfo( filename ).Exists )
            {
                MessageBox.Show( "지정된 파일이 존재하지 않습니다.", "Q730 알림 메시지" );
                return;
            }

            listView1.Items.Clear();

            try
            {
                var temp = PatternData.FromFile( filename );

                _pattern = temp;
                if( _pattern.BiasMode == PatternBiasMode.CC ) radioButton_CC.Checked = true;
                if( _pattern.PulseWidth == 10 ) radioButton_10ms.Checked = true;
            }
            catch( QException ex )
            {
                MessageBox.Show( ex.Message, "Q730 알림 메시지" );
                return;
            }

            for( var i = 0; i < _pattern.Count; i++ )
            {
                var item = new ListViewItem( (listView1.Items.Count + 1).ToString() );
                item.SubItems.Add( _pattern[i].Value.ToString() );
                item.SubItems.Add( (_pattern[i].Count * _pattern.PulseWidth).ToString() );

                listView1.Items.Add( item );
            }

            refreshGraph();
            setRadioButtonEnabled();

            label_FileName.Text = LastLoaded = filename;
            _isChanged = false;
        }

        private bool save()
        {
            if( listView1.Items.Count == 0 )
            {
                MessageBox.Show( "저장할 패턴 데이터가 존재하지 않습니다.", "Q730 알림 메시지" );
                return false;
            }

            using( var dialog = new SaveFileDialog()
            {
                Filter = "패턴 데이터 파일(*.ptn)|*.ptn"
            } )
            {
                if( LastLoaded != "" ) dialog.FileName = LastLoaded;
                else dialog.InitialDirectory = DefaultDirectory;

                if( dialog.ShowDialog() == DialogResult.OK )
                {
                    _pattern.Save( dialog.FileName );

                    _isChanged = false;
                    MessageBox.Show( "저장되었습니다.", "Q730 알림 메시지" );

                    loadFiles();

                    label_FileName.Text = LastLoaded = dialog.FileName;

                    return true;
                }
            }

            return false;
        }

        private void listView2_SelectedIndexChanged( object sender, EventArgs e )
        {
            if( listView2.SelectedIndices.Count == 0 ) return;

            var filename = _patternFiles[listView2.SelectedIndices[0]];

            if( _isChanged )
            {
                switch( MessageBox.Show( "현재 패턴 데이터를 저장하시겠습니까?", "Q730 알림 메시지", MessageBoxButtons.YesNoCancel ) )
                {
                    case DialogResult.Yes:
                        if( !save() ) return;
                        break;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        return;
                }

            }

            load( filename );
        }

        private void button_Save_Click( object sender, EventArgs e )
        {
            save();
        }

        private void button_Load_Click( object sender, EventArgs e )
        {
            if( _isChanged )
            {
                switch( MessageBox.Show( "현재 패턴 데이터를 저장하시겠습니까?", "Q730 알림 메시지", MessageBoxButtons.YesNoCancel ) )
                {
                    case DialogResult.Yes:
                        if( !save() ) return;
                        break;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        return;
                }
            }

            using( var dialog = new OpenFileDialog()
            {
                Filter = "패턴 데이터 파일(*.ptn)|*.ptn",
                InitialDirectory = DefaultDirectory
            } )
            {
                if( dialog.ShowDialog() == DialogResult.OK )
                {
                    load( dialog.FileName );
                }
            }
        }

        private void button_OK_Click( object sender, EventArgs e )
        {
            if( _isChanged )
            {
                switch( MessageBox.Show( "현재 패턴 데이터를 저장하시겠습니까?", "Q730 알림 메시지", MessageBoxButtons.YesNoCancel ) )
                {
                    case DialogResult.Yes:
                        if( !save() ) return;
                        break;

                    case DialogResult.No:
                    case DialogResult.Cancel:
                        return;
                }
            }
            if( LastLoaded == "" )
            {
                MessageBox.Show( "빈 패턴을 지정할 수 없습니다.", "Q730 알림 메시지" );
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
