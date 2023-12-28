using McQLib.Core;
using System;
using System.ComponentModel;

namespace McQLib.Recipes
{
    public sealed class Label : Recipe
    {
        public override string GetManualString()
        {
            return "Jump 레시피에서 처리를 이동할 지점에 대한 표시입니다.";
        }

        public override string GetSummaryString()
        {
            return LabelName;
        }

        public override string GetDetailString()
        {
            var str = _title;

            str += $"Jump to \"{LabelName}\"";

            return str;
        }

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_Label;

        internal Label() { }

        public override object Clone()
        {
            var clone = new Label();

            clone.LabelName = LabelName;

            return clone;
        }

        #region Properties
        [Category( "Option" )]
        [DisplayName( "Name" )]
        [Description( "라벨의 이름입니다." )]
        [ID( "FF0400" )]
        public string LabelName { get; set; }
        //{
        //    get => _labelName;
        //    set
        //    {
        //        Jump.LabelList.Remove( _labelName );
        //        _labelName = value;
        //        Jump.LabelList.Add( value );
        //    }
        //}

        //private string _labelName = string.Empty;

        [Browsable( false )]
        public override SafetyCondition SafetyCondition => null;
        [Browsable( false )]
        public override SaveCondition SaveCondition => null;
        [Browsable( false )]
        public override EndCondition EndCondition => null;
        #endregion
    }
}
