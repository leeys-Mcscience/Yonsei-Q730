using System;
using McQLib.Core;
using System.ComponentModel;
using System.Collections.Generic;

namespace McQLib.Recipes
{
    public sealed class CdCycle : Recipe
    {
        public override string GetManualString()
        {
            throw new System.NotImplementedException();
        }

        public override string GetSummaryString()
        {
            throw new NotImplementedException();
        }

        public override string GetDetailString()
        {
            throw new System.NotImplementedException();
        }

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_Cyc;

        public override object Clone()
        {
            var clone = new CdCycle();

            clone.CycleCount = CycleCount;
            clone.FrontRest = FrontRest;
            clone.MiddleRest = MiddleRest;
            clone.BackRest = BackRest;
            clone.Reverse = Reverse;

            clone.Charge = Charge.Clone() as Charge;
            clone.Discharge = Charge.Clone() as Discharge;

            clone._save = _save.Clone() as SaveCondition;
            //clone._end = _end.Clone() as EndCondition;
            clone._safety = _safety.Clone() as SafetyCondition;

            return clone;
        }

        public override void Refresh()
        {
            Charge.Refresh();
            Discharge.Refresh();
            base.Refresh();
        }

        #region Properties
        [Category( "\t\tCycle Options" )]
        [DisplayName( "Cycle Count" )]
        [Description( "사이클을 반복할 횟수입니다.")]
        public uint CycleCount { get; set; }

        [Category("\t\tCycle Options")]
        [DisplayName( "Front rest" )]
        [Description("시퀀스의 앞에 Rest 레시피를 추가하는 옵션입니다.")]
        public bool FrontRest { get; set; }

        [Category( "\t\tCycle Options" )]
        [DisplayName( "Middle rest" )]
        [Description( "시퀀스의 중간에 Rest 레시피를 추가하는 옵션입니다." )]
        public bool MiddleRest { get; set; }

        [Category( "\t\tCycle Options" )]
        [DisplayName( "Back rest" )]
        [Description( "시퀀스의 뒤에 Rest 레시피를 추가하는 옵션입니다." )]
        public bool BackRest { get; set; }

        [Category( "\t\tCycle Options" )]
        [DisplayName( "Reverse" )]
        [Description( "Charge와 Discharge의 순서를 뒤바꾸는 옵션입니다." )]
        public bool Reverse { get; set; }

        [Category( "\tRecipe Options" )]
        [DisplayName( "Charge" )]
        [TypeConverter(typeof(ChargeConverterForCyc))]
        [RefreshProperties(RefreshProperties.All)]
        public Charge Charge { get; set; } = new Charge();

        [Category( "\tRecipe Options" )]
        [DisplayName( "Discharge" )]
        [TypeConverter( typeof( DischargeConverterForCyc ) )]
        [RefreshProperties( RefreshProperties.All )]
        public Discharge Discharge { get; set; } = new Discharge();

        [Browsable(false)]
        public override EndCondition EndCondition => null;
        #endregion
    }

    public class ChargeConverterForCyc : TypeConverter
    {
        public override bool GetPropertiesSupported( ITypeDescriptorContext context )
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties( ITypeDescriptorContext context, object value, Attribute[] attributes )
        {

            var list = new List<PropertyDescriptor>();

            var properties = TypeDescriptor.GetProperties( typeof( Charge ) );
            list.Add( properties.Find( "SourcingType", false ) );
            list.Add( properties.Find( "Voltage", false ) );
            list.Add( properties.Find( "Current", false ) );
            list.Add( properties.Find( "Power", false ) );
            list.Add( properties.Find( "Resistance", false ) );

            list.Add( properties.Find( "EndCondition", false ) );

            var collection = new PropertyDescriptorCollection( list.ToArray() );

            return collection;
        }
    }
    public class DischargeConverterForCyc : TypeConverter
    {
        public override bool GetPropertiesSupported( ITypeDescriptorContext context )
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties( ITypeDescriptorContext context, object value, Attribute[] attributes )
        {

            var list = new List<PropertyDescriptor>();

            var properties = TypeDescriptor.GetProperties( typeof( Discharge ) );
            list.Add( properties.Find( "SourcingType", false ) );
            list.Add( properties.Find( "Voltage", false ) );
            list.Add( properties.Find( "Current", false ) );
            list.Add( properties.Find( "Power", false ) );
            list.Add( properties.Find( "Resistance", false ) );

            list.Add( properties.Find( "EndCondition", false ) );

            var collection = new PropertyDescriptorCollection( list.ToArray() );

            return collection;
        }
    }
}
