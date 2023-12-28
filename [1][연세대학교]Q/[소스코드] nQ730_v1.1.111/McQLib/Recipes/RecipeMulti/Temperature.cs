using System;

namespace McQLib.Recipes
{
    public sealed class Temperature : Recipe
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

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_Temperature;

        internal Temperature() { }

        public override object Clone()
        {
            var clone = new Temperature();

            clone._save = _save.Clone() as SaveCondition;
            clone._end = _end.Clone() as EndCondition;
            clone._safety = _safety.Clone() as SafetyCondition;

            return clone;
        }
    }
}
