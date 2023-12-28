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
using DataViewer.Class;

namespace DataViewer
{
    public partial class Form_FileInfo : Form
    {
        public Form_FileInfo(QDataManager mgr)
        {
            InitializeComponent();

            if (mgr.Sequence == null || mgr.Sequence.Count == 0 )
            {
                label_SeqHelp.Visible = true;
            }
            else
            {
                userControl_SequenceViewer1.SetSequence( mgr.Sequence );
            }

            var filepath = mgr.FileName;
            var fileInfo = new FileInfo( filepath );

            // File
            listView1.Items[0].SubItems.Add( fileInfo.Name );
            listView1.Items[1].SubItems.Add( "Qrd(Q Raw Data) 파일" );
            listView1.Items[2].SubItems.Add( fileInfo.DirectoryName );
            listView1.Items[3].SubItems.Add( $"{fileInfo.Length} 바이트" );
            listView1.Items[4].SubItems.Add( fileInfo.CreationTime.ToString() );
            listView1.Items[5].SubItems.Add( fileInfo.LastWriteTime.ToString() );

            // Data
            listView1.Items[6].SubItems.Add( $"{mgr.Sequence.Count}개" );

            var sum = 0;
            for ( var i = 0; i < mgr.Count; i++ ) sum += mgr[i].Count;
            listView1.Items[7].SubItems.Add( $"{sum}개" );
            listView1.Items[8].SubItems.Add( $"{mgr.MajorVersion}.{mgr.MinorVersion}" );
        }
    }
}
