using System;
using System.Drawing;
using System.Windows.Forms;

namespace Q730.UserControls
{
    public partial class UserControl_CaptionBar : UserControl
    {
        public FormWindowState WindowState
        {
            get => ParentForm.WindowState;
            set
            {
                // 버튼 아이콘 변경
                if( value == FormWindowState.Maximized )
                {
                    button_Maximize.BackgroundImage = Properties.Resources.Normalize;
                }
                else if( value == FormWindowState.Normal )
                {
                    button_Maximize.BackgroundImage = Properties.Resources.Maximize;
                }


                ParentForm.WindowState = value;
            }
        }

        public UserControl_CaptionBar()
        {
            InitializeComponent();

            Dock = DockStyle.Fill;
        }

        private void button_Minimize_Click( object sender, EventArgs e )
        {
            ParentForm.WindowState = FormWindowState.Minimized;
            ProcessTabKey( false );
        }

        private void button_Maximize_Click( object sender, EventArgs e )
        {
            if( WindowState == FormWindowState.Maximized )
            {
                WindowState = FormWindowState.Normal;
            }
            else
            {
                WindowState = FormWindowState.Maximized;
            }

            ProcessTabKey( false );
        }

        private void button_Close_Click( object sender, EventArgs e )
        {
            ParentForm.Close();
        }

        Point mousePoint;
        bool readyToMaximized = false;
        bool clickedWhenMaximized = false;

        private void CaptionSpace_DoubleClick( object sender, EventArgs e )
        {
            if( WindowState == FormWindowState.Maximized ) WindowState = FormWindowState.Normal;
            else WindowState = FormWindowState.Maximized;
        }
        private void CaptionSpace_MouseDown( object sender, MouseEventArgs e )
        {
            if( WindowState == FormWindowState.Maximized )
            {
                clickedWhenMaximized = true;
                return;
            }
            mousePoint = new Point( e.X, e.Y );
        }
        private void CaptionSpace_MouseMove( object sender, MouseEventArgs e )
        {
            if( clickedWhenMaximized && WindowState == FormWindowState.Maximized )
            {
                WindowState = FormWindowState.Normal;

                Point p = PointToScreen( e.Location );
                if( p.X > ParentForm.Left + ParentForm.Width )
                {
                    ParentForm.Location = new Point( ParentForm.Left + p.X - ParentForm.Right + ParentForm.Width / 2, p.Y - 25 );
                    mousePoint = PointToClient( p );
                }
                clickedWhenMaximized = false;
                return;
            }

            if( !clickedWhenMaximized && ((e.Button & MouseButtons.Left) == MouseButtons.Left) )
            {
                ParentForm.Location = new Point( ParentForm.Left - (mousePoint.X - e.X), ParentForm.Top - (mousePoint.Y - e.Y) );
                readyToMaximized = PointToScreen( e.Location ).Y == 0 ? true : false;
            }
        }
        private void CaptionSpace_MouseUp( object sender, MouseEventArgs e )
        {
            if( clickedWhenMaximized ) clickedWhenMaximized = false;
            if( readyToMaximized )
            {
                WindowState = FormWindowState.Maximized;
                readyToMaximized = false;
            }
        }
    }
}
