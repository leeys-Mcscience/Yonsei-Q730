using McQLib.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Drawing.Design;
//using System.Windows.Forms;
//using System.Windows.Forms.Design;

namespace McQLib.Recipes
{
    public sealed class Jump : BaseConvertableRecipe
    {
        //public static List<string> LabelList = new List<string>();
        public override string GetManualString()
        {
            return "임의의 지점으로 처리를 이동시킵니다.\r\n" +
                   "이동할 지점은 Label 레시피를 사용해 선택할 수 있습니다.";
        }

        public override string GetSummaryString()
        {
            return $"Jump count : {JumpCount}\r\n" +
                   $"Jump to \"{LabelName}\"";
        }

        public override string GetDetailString()
        {
            var str = _title;

            str += $"Jump count : {JumpCount}\r\n" +
                   $"Jump to \"{LabelName}\"";

            return str;
        }

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_Jump;

        /// <summary>
        /// 레시피 정보를 패킷의 DATA Field 형태로 변환합니다.
        /// </summary>
        /// <param name="stepNo">현재 레시피의 시퀀스상 순번입니다.</param>
        /// <param name="endStepNo">현재 레시피가 종료된 후 이동할 목표 레시피의 시퀀스상 순번입니다.</param>
        /// <param name="errorStepNo">현재 레시피가 비정상 종료된 후 이동할 목표 레시피의 시퀀스상 순번입니다.</param>
        /// <returns>변환된 DATA Field 형태의 byte형식 배열입니다.</returns>
        public override byte[] ToDataField( ushort stepNo, ushort endStepNo, ushort errorStepNo )
        {
            var builder = new DataBuilder();

            // Reserved (2Byte) - Reserved
            builder.AddCount( 0, 2 );

            // Step count (4Byte) - Reserved
            builder.AddCount( 0, 4 );

            // Step no. (2Byte) - 설정 Step number
            builder.Add( new Q_UInt16( stepNo ) );

            // Cycle no. (4Byte) - Reserved
            builder.AddCount( 0, 4 );

            // Mode1 (1Byte) - Loop : 7
            builder.Add( ( byte )Mode1.Jump );

            // Mode2 (1Byte) - Reserved
            builder.Add( 0 );

            // [설정 조건] (66Byte) - 설정값 없음
            builder.AddCount( 0, 66 );

            // [안전 조건, 종료 조건, 기록 조건] (164Byte) - 설정값 없음
            builder.AddCount( 0, 164 );

            // 종료값 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( endStepNo ) );

            // 비정상 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( errorStepNo ) );

            // 반복횟수 (4Byte) - Loop, Jump만 해당
            if ( JumpCount <= 0 )
            {
                throw new QException( QExceptionType.RECIPE_INVALID_JUMP_COUNT_ERROR );
            }
            builder.Add( JumpCount );

            // 스텝 진행 (1Byte) - Reserved
            builder.Add( 0 );

            // Reserved (7Byte) - Reserved
            builder.AddCount( 0, 7 );

            return builder;
        }

        /// <summary>
        /// 패킷의 DATA Field 형태로부터 레시피 정보를 추출합니다. 이 메서드는 지정된 바이트 배열의 인덱스 0부터 259까지의 총 260개의 바이트를 사용합니다.
        /// </summary>
        /// <returns>변환된 DATA Field 형태의 byte형식 배열입니다.</returns>
        public override bool FromDataField( byte[] data )
        {
            if ( data == null || data.Length < 260 ) return false;

            var position = 248;
            JumpCount = new Q_UInt32( data[position++], data[position++], data[position++], data[position++] );
            LabelName = new Q_UInt16( data[244], data[245] ).Value.ToString();

            // 안전 조건, 종료 조건, 저장 조건 - 없음

            return true;
        }

        internal Jump() { }

        public override object Clone()
        {
            var clone = new Jump();

            clone.JumpCount = JumpCount;
            clone.LabelName = LabelName;

            return clone;
        }

        #region Properties
        [Category( "Option" )]
        [DisplayName( "Jump Count" )]
        [Description( "지정된 Label로의 점프를 수행할 횟수입니다." )]
        [ID( "070000" )]
        public uint JumpCount { get; set; }

        [Category( "Option" )]
        [DisplayName( "Jump to" )]
        [Description( "점프할 Label의 이름입니다." )]
        [ID( "070001" )]
        public string LabelName { get; set; }

        //public LabelNameType LabelName { get; set; } = "";

        //[Editor(typeof(LabelNameTypeEditor), typeof(UITypeEditor))]
        //public class LabelNameType
        //{
        //    public override string ToString()
        //    {
        //        return _labelName;
        //    }

        //    private string _labelName = string.Empty;

        //    public static implicit operator LabelNameType(string labelName) => new LabelNameType() { _labelName = labelName };
        //    public static implicit operator string(LabelNameType labelNameType) => labelNameType._labelName;

        //    public class LabelNameTypeEditor : UITypeEditor
        //    {
        //        private IWindowsFormsEditorService _editorService;
        //        public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
        //        {
        //            return UITypeEditorEditStyle.DropDown;
        //        }
        //        private void OnListBoxSelectedValueChanged( object sender, EventArgs e )
        //        {
        //            _editorService.CloseDropDown();
        //        }
        //        public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
        //        {
        //            _editorService = ( IWindowsFormsEditorService )provider.GetService( typeof( IWindowsFormsEditorService ) );

        //            var listBox = new ListBox();
        //            listBox.SelectionMode = SelectionMode.One;
        //            listBox.SelectedValueChanged += OnListBoxSelectedValueChanged;

        //            listBox.Items.AddRange( LabelList.ToArray() ); //.Select( i => i.ToString() ).ToArray() );
        //            for ( var i = 0; i < listBox.Items.Count; i++ )
        //            {
        //                if ( listBox.Items[i].ToString() == value?.ToString() )
        //                {
        //                    listBox.SelectedIndex = i;
        //                }
        //            }

        //            _editorService.DropDownControl( listBox );
        //            if ( listBox.SelectedItem == null ) return value;

        //            return new LabelNameType() { _labelName = listBox.SelectedItem.ToString() };
        //        }
            //}
        //}

        [Browsable( false )]
        public override SafetyCondition SafetyCondition => null;
        [Browsable( false )]
        public override SaveCondition SaveCondition => null;
        [Browsable( false )]
        public override EndCondition EndCondition => null;
        #endregion
    }
}
