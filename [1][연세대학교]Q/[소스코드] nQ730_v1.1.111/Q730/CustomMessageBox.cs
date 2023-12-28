using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Q730
{
    public partial class CustomMessageBox : Form
    {
        [DllImport( "user32" )]
        private static extern IntPtr GetSystemMenu( IntPtr hWnd, bool bRevert );
        [DllImport( "user32" )]
        private static extern bool EnableMenuItem( IntPtr hMenu, uint itemId, uint uEnable );

        private static void ChangeCloseButtonEnabled( Form form, bool enabled )
        {
            EnableMenuItem( GetSystemMenu( form.Handle, false ), 0xF060, ( uint )(enabled ? 0 : 1) );
        }

        public static DialogResult Show( string message, string caption, MessageBoxButtons messageBoxButtons )
        {
            var dialog = new CustomMessageBox();

            dialog.Text = caption;

            var count = 0;

            switch( messageBoxButtons )
            {
                case MessageBoxButtons.OK:
                    addButton( dialog, "확인", DialogResult.OK );
                    count = 1;

                    dialog.DialogResult = DialogResult.OK;
                    break;

                case MessageBoxButtons.OKCancel:
                    dialog.buttonCancel = addButton( dialog, "취소", DialogResult.Cancel );
                    var button = addButton( dialog, "확인", DialogResult.OK );
                    button.TabIndex = 0;
                    dialog.buttonCancel.TabIndex = 1;
                    count = 2;

                    dialog.DialogResult = DialogResult.Cancel;
                    break;

                case MessageBoxButtons.YesNo:
                    dialog.buttonYes = addButton( dialog, "예(Y)", DialogResult.Yes );
                    dialog.buttonNo = addButton( dialog, "아니오(N)", DialogResult.No );
                    dialog.buttonYes.TabIndex = 0;
                    dialog.buttonNo.TabIndex = 1;
                    count = 2;

                    // 예/아니오의 경우 제목표시줄의 X버튼을 비활성화해준다.
                    ChangeCloseButtonEnabled( dialog, false );
                    dialog.canEscape = false;
                    break;

                case MessageBoxButtons.YesNoCancel:
                    dialog.buttonCancel = addButton( dialog, "취소", DialogResult.Cancel );
                    dialog.buttonNo = addButton( dialog, "아니오(N)", DialogResult.No );
                    dialog.buttonYes = addButton( dialog, "예(Y)", DialogResult.Yes );
                    dialog.buttonYes.TabIndex = 0;
                    dialog.buttonNo.TabIndex = 1;
                    dialog.buttonCancel.TabIndex = 2;
                    count = 3;

                    dialog.DialogResult = DialogResult.Cancel;
                    break;
            }

            dialog.Width = count * 100 + 20;
            dialog.label1.Text = message;

            int minHeight = int.MaxValue, widthWhenMinHeight = -1;
            for( var i = dialog.Width - 20; i < dialog.MaximumSize.Width - 20; i++ )
            {
                var size = TextRenderer.MeasureText( message, dialog.Font, new Size( i, int.MaxValue ), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl );
                if( size.Height < minHeight )
                {
                    minHeight = size.Height;
                    widthWhenMinHeight = i;
                }
            }

            var labelSize = TextRenderer.MeasureText( message, dialog.Font, new Size( widthWhenMinHeight, int.MaxValue ), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl );
            var captionSize = TextRenderer.MeasureText( caption, dialog.Font, new Size( 0, 0 ) );

            if( captionSize.Width > dialog.MaximumSize.Width - 30 )
            {
                labelSize.Width = dialog.MaximumSize.Width - 20;
                dialog.Width = dialog.MaximumSize.Width;
            }
            else
            {
                dialog.Width = labelSize.Width + 80;
            }

            dialog.label1.Size = labelSize;

            dialog.Height = labelSize.Height + 60 + 40 + 20; // 제목표시줄(60) + 버튼표시영역(40) + Label 상하 공백(20) + CheckBox 높이(30)

            return dialog.ShowDialog();
        }
        public static DialogResult Show( string message, string caption, MessageBoxButtons messageBoxButtons, string checkBoxMessage, out bool isChecked )
        {
            var dialog = new CustomMessageBox();

            dialog.Text = caption;

            var count = 0;

            switch( messageBoxButtons )
            {
                case MessageBoxButtons.OK:
                    addButton( dialog, "확인", DialogResult.OK );
                    count = 1;

                    dialog.DialogResult = DialogResult.OK;
                    break;

                case MessageBoxButtons.OKCancel:
                    dialog.buttonCancel = addButton( dialog, "취소", DialogResult.Cancel );
                    var button = addButton( dialog, "확인", DialogResult.OK );
                    button.TabIndex = 0;
                    dialog.buttonCancel.TabIndex = 1;
                    count = 2;

                    dialog.DialogResult = DialogResult.Cancel;
                    break;

                case MessageBoxButtons.YesNo:
                    dialog.buttonYes = addButton( dialog, "예(Y)", DialogResult.Yes );
                    dialog.buttonNo = addButton( dialog, "아니오(N)", DialogResult.No );
                    dialog.buttonYes.TabIndex = 0;
                    dialog.buttonNo.TabIndex = 1;
                    count = 2;

                    // 예/아니오의 경우 제목표시줄의 X버튼을 비활성화해준다.
                    ChangeCloseButtonEnabled( dialog, false );
                    dialog.canEscape = false;
                    break;

                case MessageBoxButtons.YesNoCancel:
                    dialog.buttonCancel = addButton( dialog, "취소", DialogResult.Cancel );
                    dialog.buttonNo = addButton( dialog, "아니오(N)", DialogResult.No );
                    dialog.buttonYes = addButton( dialog, "예(Y)", DialogResult.Yes );
                    dialog.buttonYes.TabIndex = 0;
                    dialog.buttonNo.TabIndex = 1;
                    dialog.buttonCancel.TabIndex = 2;
                    count = 3;

                    dialog.DialogResult = DialogResult.Cancel;
                    break;
            }

            dialog.Width = count * 100 + 20;
            dialog.label1.Text = message;

            int minHeight = int.MaxValue, widthWhenMinHeight = -1;
            for( var i = dialog.Width - 20; i < dialog.MaximumSize.Width - 20; i++ )
            {
                var size = TextRenderer.MeasureText( message, dialog.Font, new Size( i, int.MaxValue ), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl );
                if( size.Height < minHeight )
                {
                    minHeight = size.Height;
                    widthWhenMinHeight = i;
                }
            }

            var labelSize = TextRenderer.MeasureText( message, dialog.Font, new Size( widthWhenMinHeight, int.MaxValue ), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl );
            var checkBoxSize = TextRenderer.MeasureText( checkBoxMessage, dialog.Font, new Size( dialog.Width - 40, int.MaxValue ), TextFormatFlags.WordBreak );
            var captionSize = TextRenderer.MeasureText( caption, dialog.Font, new Size( 0, 0 ) );

            if(captionSize.Width > dialog.MaximumSize.Width - 30 )
            {
                labelSize.Width = dialog.MaximumSize.Width - 20;
                dialog.Width = dialog.MaximumSize.Width;
            }
            else
            {
                dialog.Width = labelSize.Width + 80;
            }

            dialog.tableLayoutPanel1.RowStyles.Insert( 1, new RowStyle( SizeType.Absolute, checkBoxSize.Height + 4 ) );
            dialog.tableLayoutPanel1.Controls.Add( dialog.checkBox = new CheckBox()
            {
                Text = checkBoxMessage,
                Anchor = AnchorStyles.Right,
                Margin = new Padding( 0, 0, 5, 0 ),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, 1 );

            dialog.label1.Size = labelSize;
            dialog.checkBox.Size = checkBoxSize;

            dialog.Height = labelSize.Height + 60 + 40 + 20 + checkBoxSize.Height; // 제목표시줄(60) + 버튼표시영역(40) + Label 상하 공백(20) + CheckBox 높이(30)

            var result = dialog.ShowDialog();

            isChecked = dialog.checkBox.Checked;
            return result;
        }

        private static Button addButton( CustomMessageBox form, string message, DialogResult dialogResult )
        {
            var button = createButton( message );
            button.Click += delegate ( object sender, EventArgs e ) { form.DialogResult = dialogResult; form.Close(); };
            form.flowLayoutPanel1.Controls.Add( button );
            Application.DoEvents();

            return button;
        }

        private static Button createButton( string text )
        {
            return new Button()
            {
                Text = text,
                Size = new Size( 80, 23 ),
                Margin = new Padding( 10, 10, 0, 0 ),
                Anchor = AnchorStyles.None,
            };
        }

        private CustomMessageBox()
        {
            InitializeComponent();
        }

        CheckBox checkBox;

        bool canEscape = true;
        Button buttonYes, buttonNo, buttonCancel;
        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            Keys keys = keyData & ~(Keys.Shift | Keys.Control);

            switch( keys )
            {
                case Keys.Y:
                    if( buttonYes != null ) buttonYes.PerformClick();
                    break;

                case Keys.N:
                    if( buttonNo != null ) buttonNo.PerformClick();
                    break;

                case Keys.Escape:
                    if( buttonCancel != null ) buttonCancel.PerformClick();
                    else if( canEscape )
                    {
                        DialogResult = DialogResult.Cancel;
                        Close();
                    }
                    break;

            }
            return base.ProcessCmdKey( ref msg, keyData );
        }
    }
}
