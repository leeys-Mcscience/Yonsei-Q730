using System.ComponentModel;

namespace McQLib.Recipes
{
    internal sealed class Idle : Recipe
    {
        public override string GetManualString() => string.Empty;

        public override string GetSummaryString() => string.Empty;

        public override string GetDetailString() => string.Empty;

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_Idle;

        internal Idle() { }

        public override object Clone()
        {
            var clone = new Idle();

            return clone;
        }

        [Browsable( false )]
        public override SafetyCondition SafetyCondition => null;
        [Browsable( false )]
        public override SaveCondition SaveCondition => null;
        [Browsable( false )]
        public override EndCondition EndCondition => null;
    }
}
