using System;
using McQLib.Core;
using McQLib.NotUsed.Core;

namespace McQLib.NotUsed.Recipes
{


    [Serializable]
    public abstract class BaseMeasureRecipe : BaseRecipe
    {
        #region Parameters
        [Group( "Parameter" )]
        [Parameter( "Amplify", ParameterValueType.Enum, "F30000" )]
        public AmplifyMode Amplify;
        #endregion
    }
}
