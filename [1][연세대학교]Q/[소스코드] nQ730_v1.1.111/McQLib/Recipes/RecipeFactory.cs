using McQLib.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace McQLib.Recipes
{
    [Flags]
    public enum RecipeType : byte
    {
        Rest = 0x00,
        Cycle = 0x01,
        Loop = 0x02,
        Jump = 0x03,
        End = 0x04,

        Charge = 0x10,
        Discharge = 0x11,
        AnodeCharge = 0x12,
        AnodeDischarge = 0x13,

        TransientResponse = 0x20,
        OpenCircuitVoltage = 0x21,
        DcResistance = 0x22,
        AcResistance = 0x23,
        FrequencyResponse = 0x24,

        Pattern = 0x30,

        SafetyCondition = 0x80,
        EndCondition = 0x81,
        SaveCondition = 0x82,
        Temperature = 0x83,
        Label = 0x84,
        CdCycle = 0x85,

        Idle = 0xF0,
        Unknown = 0xF1,
        NotDefined = 0xFF,
    }

    public static class RecipeFactory
    {
        /// <summary>
        /// 기본적으로 제공되는 레시피 목록입니다.
        /// </summary>
        public static Recipe[] Recipes
        {
            get
            {
                _recipes = new List<Recipe>();

                // 시퀀스 빌더에서 보이지 않게 할 레시피는 이곳에서 삭제 또는 주석처리 할 것.
                if ( RecipeSetting.Charge.Enabled ) _recipes.Add( new Charge() );
                if ( RecipeSetting.Discharge.Enabled ) _recipes.Add( new Discharge() );
                if ( RecipeSetting.Rest.Enabled ) _recipes.Add( new Rest() );
                if ( RecipeSetting.OpenCircuitVoltage.Enabled ) _recipes.Add( new OpenCircuitVoltage() );
                if ( RecipeSetting.AcResistance.Enabled ) _recipes.Add( new AcResistance() );
                if ( RecipeSetting.DcResistance.Enabled ) _recipes.Add( new DcResistance() );
                if ( RecipeSetting.FrequencyResponse.Enabled ) _recipes.Add( new FrequencyResponse() );
                if ( RecipeSetting.TransientResponse.Enabled ) _recipes.Add( new TransientResponse() );
                if ( RecipeSetting.Pattern.Enabled ) _recipes.Add( new Pattern() );
                if ( RecipeSetting.Temperature.Enabled ) _recipes.Add( new Temperature() );
                if ( RecipeSetting.Cycle.Enabled ) _recipes.Add( new Cycle() );
                if ( RecipeSetting.Loop.Enabled ) _recipes.Add( new Loop() );

                if ( RecipeSetting.Jump.Enabled ) _recipes.Add( new Jump() );
                if ( RecipeSetting.Label.Enabled ) _recipes.Add( new Label() );
                if ( RecipeSetting.CdCycle.Enabled ) _recipes.Add( new CdCycle() );
                if ( RecipeSetting.AnodeCharge.Enabled ) _recipes.Add( new AnodeCharge() );
                if ( RecipeSetting.AnodeDischarge.Enabled ) _recipes.Add( new AnodeDischarge() );

                return _recipes.ToArray();
            }
        }

        public static Image GetRecipeIcon(RecipeType recipeType )
        {
            switch ( recipeType )
            {
                case RecipeType.Charge: return Properties.Resources.Icon_Charge;

                case RecipeType.Discharge: return Properties.Resources.Icon_Discharge;

                case RecipeType.Rest: return Properties.Resources.Icon_Rest;

                case RecipeType.OpenCircuitVoltage: return Properties.Resources.Icon_OpenCircuitVoltage;

                case RecipeType.AcResistance: return Properties.Resources.Icon_AcResistance;

                case RecipeType.DcResistance: return Properties.Resources.Icon_DcResistance;

                case RecipeType.FrequencyResponse: return Properties.Resources.Icon_FrequencyResponse;

                case RecipeType.TransientResponse: return Properties.Resources.Icon_TransientResponse;

                case RecipeType.Temperature: return Properties.Resources.Icon_Temperature;

                case RecipeType.Pattern: return Properties.Resources.Icon_Pattern;

                case RecipeType.Cycle: return Properties.Resources.Icon_Cycle;

                case RecipeType.Loop: return Properties.Resources.Icon_Loop;

                case RecipeType.Jump: return Properties.Resources.Icon_Jump;

                case RecipeType.Label: return Properties.Resources.Icon_Label;

                case RecipeType.CdCycle: return Properties.Resources.Icon_Cyc;

                case RecipeType.Idle: return Properties.Resources.Icon_Idle;

                case RecipeType.End: return Properties.Resources.Icon_End;

                case RecipeType.AnodeCharge: return Properties.Resources.Icon_AnodeCharge;

                case RecipeType.AnodeDischarge: return Properties.Resources.Icon_AnodeDischarge;

                default:
                    return null;
            }
        }
        private static List<Recipe> _recipes;

        /// <summary>
        /// 라이브러리 외부에서 생성 가능하도록 할 레시피는 레시피 타입과 함께 이곳에서 처리한다.
        /// </summary>
        /// <param name="recipeType"></param>
        /// <returns></returns>
        public static Recipe CreateInstance( RecipeType recipeType )
        {
            switch ( recipeType )
            {
                case RecipeType.Charge: return new Charge();

                case RecipeType.Discharge: return new Discharge();

                case RecipeType.Rest: return new Rest();

                case RecipeType.OpenCircuitVoltage: return new OpenCircuitVoltage();

                case RecipeType.AcResistance: return new AcResistance();

                case RecipeType.DcResistance: return new DcResistance();

                case RecipeType.FrequencyResponse: return new FrequencyResponse();

                case RecipeType.TransientResponse: return new TransientResponse();

                case RecipeType.Temperature: return new Temperature();

                case RecipeType.Pattern: return new Pattern();

                case RecipeType.Cycle: return new Cycle();

                case RecipeType.Loop: return new Loop();

                case RecipeType.Jump: return new Jump();

                case RecipeType.Label: return new Label();

                case RecipeType.CdCycle: return new CdCycle();

                case RecipeType.Idle: return new Idle();

                case RecipeType.End: return new End();

                case RecipeType.AnodeCharge: return new AnodeCharge();

                case RecipeType.AnodeDischarge: return new AnodeDischarge();

                default:
                    return null;
            }
        }

        public static Recipe CreateInstance( byte[] data )
        {
            if ( data == null ) return null;

            var position = 0;
            // Reserved 2Byte
            position += 2;
            // Step count 4Byte
            position += 4;
            // Step no 2Byte
            position += 2;
            // Cycle no 4Byte
            position += 4;

            // Mode1
            var mode1 = ( Mode1 )data[position++];
            var mode2 = ( Mode2 )data[position++];

            Recipe recipe = CreateInstance( Util.ConvertModeToRecipeType( mode1, mode2 ) );

            //switch ( mode1 ) 
            //{
            //    case Mode1.Rest:
            //        recipe = new Rest();
            //        break;

            //    case Mode1.Charge:
            //        recipe = new Charge();
            //        break;

            //    case Mode1.Discharge:
            //        recipe = new Discharge();
            //        break;

            //    case Mode1.AnodeCharge:
            //        recipe = new AnodeCharge();
            //        break;

            //    case Mode1.AnodeDischarge:
            //        recipe = new AnodeDischarge();
            //        break;

            //    case Mode1.Measure:
            //        switch ( mode2 )
            //        {
            //            case Mode2.TRA:
            //                recipe = new TransientResponse();
            //                break;

            //            case Mode2.OCV:
            //                recipe = new OpenCircuitVoltage();
            //                break;

            //            case Mode2.DCR:
            //                recipe = new DcResistance();
            //                break;

            //            case Mode2.ACR:
            //                recipe = new AcResistance();
            //                break;

            //            case Mode2.FRA:
            //                recipe = new FrequencyResponse();
            //                break;
            //        }
            //        break;

            //    case Mode1.Pattern:
            //        switch ( mode2 )
            //        {
            //            case Mode2.Set:
            //                recipe = new Pattern();
            //                break;

            //            case (Mode2)1:
            //                break;
            //        }
            //        recipe = new Pattern();
            //        break;

            //    case Mode1.Cycle:
            //        recipe = new Cycle();
            //        break;

            //    case Mode1.Loop:
            //        recipe = new Loop();
            //        break;

            //    case Mode1.Jump:
            //        recipe = new Jump();
            //        break;

            //    case Mode1.End:
            //        recipe = new End();
            //        break;
            //}

            if ( recipe != null ) ( recipe as BaseConvertableRecipe ).FromDataField( data );

            return recipe;
        }

        public static RecipeType GetRecipeType( this Recipe recipe )
        {
            if ( recipe is Charge ) return RecipeType.Charge;
            else if ( recipe is Discharge ) return RecipeType.Discharge;
            else if ( recipe is Rest ) return RecipeType.Rest;
            else if ( recipe is OpenCircuitVoltage ) return RecipeType.OpenCircuitVoltage;
            else if ( recipe is AcResistance ) return RecipeType.AcResistance;
            else if ( recipe is DcResistance ) return RecipeType.DcResistance;
            else if ( recipe is FrequencyResponse ) return RecipeType.FrequencyResponse;
            else if ( recipe is TransientResponse ) return RecipeType.TransientResponse;
            else if ( recipe is Temperature ) return RecipeType.Temperature;
            else if ( recipe is Pattern ) return RecipeType.Pattern;
            else if ( recipe is Cycle ) return RecipeType.Cycle;
            else if ( recipe is Loop ) return RecipeType.Loop;
            else if ( recipe is Jump ) return RecipeType.Jump;
            else if ( recipe is Label ) return RecipeType.Label;
            else if ( recipe is CdCycle ) return RecipeType.CdCycle;
            else if ( recipe is End ) return RecipeType.End;
            else if ( recipe is Idle ) return RecipeType.Idle;
            else if ( recipe is AnodeCharge ) return RecipeType.AnodeCharge;
            else if ( recipe is AnodeDischarge ) return RecipeType.AnodeDischarge;
            // 정의되지 않은 레시피 형식
            else throw new QException( QExceptionType.DEVELOP_UNDEFINED_RECIPE_TYPE_ERROR );
        }

        public static RecipeInfo GetRecipeInfo( this Recipe recipe ) => new RecipeInfo( recipe );
    }
}
