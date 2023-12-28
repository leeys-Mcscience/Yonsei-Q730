using McQLib.Recipes;
using McQLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McQLib.OldSupport
{
    public static class OldSequence
    {
        public static Sequence FromFile( string filename )
        {
            var sequence = new Sequence();

            using( var sr = new StreamReader( filename ) )
            {
                sequence.FilePath = filename;

                sr.ReadLine();
                sr.ReadLine();

                string line;
                while( (line = sr.ReadLine()) != null )
                {
                    var split = line.Split( ',' );

                    Recipe recipe = null;
                    RecipeInfo recipeInfo = null;
                    int index;

                    switch( split[0] )
                    {
                        case "eMeasurement":
                            index = 4;

                            switch( split[1] )
                            {
                                case "eACR":
                                    #region AcResistance
                                    recipe = RecipeFactory.CreateInstance( RecipeType.AcResistance );
                                    recipeInfo = recipe.GetRecipeInfo();

                                    recipeInfo["030900"].SetValue( recipe, double.Parse( split[index++] ) );
                                    index++;
                                    index++;
                                    recipeInfo["030901"].SetValue( recipe, double.Parse( split[index++] ) );
                                    index++;    // Mode select(Log, Linear)
                                    recipeInfo["030902"].SetValue( recipe, double.Parse( split[index++] ) );
                                    index++;    // Step count
                                    index++;    // Sampling
                                    index++;    // Delay2
                                    index++;    // Width2
                                    recipeInfo["030904"].SetValue( recipe, split[index++] == "1" );       // Raw Data
                                    switch( split[index++] )                                                // Dc cancel range (Tr 증폭 배율)
                                    {
                                        case "0":
                                            recipeInfo["030903"].SetValue( recipe, AmplifyMode.x1 );
                                            break;

                                        case "1":
                                            recipeInfo["030903"].SetValue( recipe, AmplifyMode.x100 );
                                            break;

                                        case "2":
                                            recipeInfo["030903"].SetValue( recipe, AmplifyMode.x500 );
                                            break;
                                    }
                                    index++;    // Use tra

                                    index += 12;    // end condition - not used

                                    // Safety Condition
                                    recipeInfo["FF0004"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Ah)
                                    recipeInfo["FF0002"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Current
                                    recipeInfo["FF0006"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Max Temperature
                                    recipeInfo["FF0000"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Voltage
                                    recipeInfo["FF0005"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Wh)
                                    recipeInfo["FF0003"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Current
                                    recipeInfo["FF0007"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Min Temperature
                                    recipeInfo["FF0001"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Voltage

                                    sequence.Add( recipe );
                                    break;
                                #endregion

                                case "eDCR":
                                    #region DcResistance
                                    recipe = RecipeFactory.CreateInstance( RecipeType.DcResistance );
                                    recipeInfo = recipe.GetRecipeInfo();

                                    index++;    // recipeInfo[ "050200" ].SetValue( recipe, double.Parse( split[index++] ) ); // Bias = 3rd Current (사용 안 함)
                                    recipeInfo["030802"].SetValue( recipe, double.Parse( split[index++] ) / 1000000 ); // EndFreq = 1st Width
                                    recipeInfo["030801"].SetValue( recipe, double.Parse( split[index++] ) ); // HighCurrent = 2nd Current
                                    recipeInfo["030800"].SetValue( recipe, double.Parse( split[index++] ) ); // LowCurrent = 1st Current
                                    index++;    // ModeSelect
                                    index++;    // recipeInfo[ "050500" ].SetValue( recipe, double.Parse( split[index++] ) ); // StartFreq (고정값 사용)
                                    index++;    // StepCount
                                    index++;    // recipeInfo["050700"].SetValue( recipe, double.Parse( split[index++] ) / 1000000 ); // Sampling (1st width + 2nd width)
                                    index++;    // recipeInfo[ "050600" ].SetValue( recipe, double.Parse( split[index++] ) ); // Delay2 (고정값 사용)
                                    recipeInfo["030803"].SetValue( recipe, uint.Parse( split[index++] ) / 1000000 );   // Width2
                                    index++;    // SaveRawData
                                    switch( split[index++] )                                                // Dc cancel range (Tr 증폭 배율)
                                    {
                                        case "0":
                                            recipeInfo["030806"].SetValue( recipe, AmplifyMode.x1 );
                                            break;

                                        case "1":
                                            recipeInfo["030806"].SetValue( recipe, AmplifyMode.x100 );
                                            break;

                                        case "2":
                                            recipeInfo["030806"].SetValue( recipe, AmplifyMode.x500 );
                                            break;
                                    }
                                    index++;    // Use tra

                                    index += 12;    // End condition - not used

                                    // Safety Condition
                                    recipeInfo["FF0004"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Ah)
                                    recipeInfo["FF0002"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Current
                                    recipeInfo["FF0006"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Max Temperature
                                    recipeInfo["FF0000"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Voltage
                                    recipeInfo["FF0005"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Wh)
                                    recipeInfo["FF0003"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Current
                                    recipeInfo["FF0007"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Min Temperature
                                    recipeInfo["FF0001"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Voltage

                                    // Save Condition
                                    recipeInfo["FF0201"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Delta Current
                                    recipeInfo["FF0203"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) ); // Delta Temperature
                                    recipeInfo["FF0202"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Delta Voltage
                                    recipeInfo["FF0200"].SetValue( recipe, split[index++] == "0" ? null : ( object )Util.ConvertMsToString( (uint)double.Parse( split[index - 1] ) ) );   // Interval

                                    sequence.Add( recipe );
                                    break;
                                #endregion

                                case "eFRA":
                                    #region FrequencyResponse
                                    recipe = RecipeFactory.CreateInstance( RecipeType.FrequencyResponse );
                                    recipeInfo = recipe.GetRecipeInfo();

                                    recipeInfo["031000"].SetValue( recipe, double.Parse( split[index++] ) );// Bias - Bias
                                    recipeInfo["031003"].SetValue( recipe, double.Parse( split[index++] ) );// EndFreq - End Freq
                                    index++; // HighCurrent - not used
                                    recipeInfo["031001"].SetValue( recipe, double.Parse( split[index++] ) ); // LowCurrent - amplitude
                                    switch( split[index++] )                                                        // Tr Scale Mode
                                    {
                                        case "0":
                                            recipeInfo["031004"].SetValue( recipe, ScaleMode.Log );
                                            break;

                                        case "1":
                                            recipeInfo["031004"].SetValue( recipe, ScaleMode.Linear );
                                            break;
                                    }
                                    recipeInfo["031002"].SetValue( recipe, double.Parse( split[index++] ) ); // StartFreq - start freq
                                    recipeInfo["031005"].SetValue( recipe, double.Parse( split[index++] ) ); // StepCount - step count
                                    recipeInfo["031007"].SetValue( recipe, double.Parse( split[index++] ) ); // Sampling - measuretime
                                    index++;    // Delay2 - not used
                                    index++;    // Width2 - not used
                                    recipeInfo["031006"].SetValue( recipe, split[index++] == "1" );          // SaveRawData - raw data
                                    switch( split[index++] )                                                // Dc cancel range (Tr 증폭 배율)
                                    {
                                        case "0":
                                            recipeInfo["031008"].SetValue( recipe, AmplifyMode.x1 );
                                            break;

                                        case "1":
                                            recipeInfo["031008"].SetValue( recipe, AmplifyMode.x100 );
                                            break;

                                        case "2":
                                            recipeInfo["031008"].SetValue( recipe, AmplifyMode.x500 );
                                            break;
                                    }
                                    index++;    // UseTra - not used

                                    index += 12;    // End condition - not used

                                    // Safety Condition
                                    recipeInfo["FF0004"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Ah)
                                    recipeInfo["FF0002"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Current
                                    recipeInfo["FF0006"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Max Temperature
                                    recipeInfo["FF0000"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Voltage
                                    recipeInfo["FF0005"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Wh)
                                    recipeInfo["FF0003"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Current
                                    recipeInfo["FF0007"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Min Temperature
                                    recipeInfo["FF0001"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Voltage

                                    sequence.Add( recipe );
                                    break;
                                #endregion

                                case "eOCV":
                                    #region OpenCircuitVoltage
                                    recipe = RecipeFactory.CreateInstance( RecipeType.OpenCircuitVoltage );
                                    recipeInfo = recipe.GetRecipeInfo();

                                    index++;    // Bias - Bias
                                    index++;    // EndFreq - End Freq
                                    index++;    // HighCurrent - not used
                                    index++;    // LowCurrent - amplitude
                                    switch( split[index++] )                                                        // Tr Scale Mode
                                    {
                                        case "0":
                                            recipeInfo["030703"].SetValue( recipe, ScaleMode.Log );
                                            break;

                                        case "1":
                                            recipeInfo["030703"].SetValue( recipe, ScaleMode.Linear );
                                            break;
                                    }
                                    index++;    // StartFreq - start freq
                                    index++;    // StepCount - transition
                                    recipeInfo["030702"].SetValue( recipe, double.Parse( split[index++] ) ); // Sampling - measuretime
                                    index++;    // Delay2 - not used
                                    index++;    // Width2 - not used
                                    index++;    // SaveRawData - raw data
                                    switch( split[index++] )                                                // Dc cancel range (Tr 증폭 배율)
                                    {
                                        case "0":
                                            recipeInfo["030701"].SetValue( recipe, AmplifyMode.x1 );
                                            break;

                                        case "1":
                                            recipeInfo["030701"].SetValue( recipe, AmplifyMode.x100 );
                                            break;

                                        case "2":
                                            recipeInfo["030701"].SetValue( recipe, AmplifyMode.x500 );
                                            break;
                                    }
                                    switch( split[index++] )                                                        // Tr Step Mode
                                    {
                                        case "0":
                                            recipeInfo["030700"].SetValue( recipe, TrStepMode.None );
                                            break;

                                        case "1":
                                            recipeInfo["030700"].SetValue( recipe, TrStepMode.Before );
                                            break;

                                        case "2":
                                            recipeInfo["030700"].SetValue( recipe, TrStepMode.After );
                                            break;

                                        case "3":
                                            recipeInfo["030700"].SetValue( recipe, TrStepMode.All );
                                            break;
                                    }

                                    // End Condition
                                    recipeInfo["FF0104"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Ah
                                    recipeInfo["FF0101"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Current
                                    recipeInfo["FF0103"].SetValue( recipe, split[index++] == "0" ? null : ( object )Util.ConvertMsToString( (uint)double.Parse( split[index - 1] ) ) ); // CvTime
                                    recipeInfo["FF0108"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Delta Temperature
                                    recipeInfo["FF0107"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Delta Voltage
                                    index++;                                                                                                                   // MaxPer
                                    index++;                                                                                                                   // MaxPerStepCount
                                    recipeInfo["FF0109"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Temperature
                                    recipeInfo["FF0102"].SetValue( recipe, split[index++] == "0" ? null : ( object )Util.ConvertMsToString( ( uint )double.Parse( split[index - 1] ) ) ); // Time
                                    recipeInfo["FF0100"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Voltage
                                    recipeInfo["FF0105"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Watt
                                    recipeInfo["FF0106"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Watt Hour

                                    // Safety Condition
                                    recipeInfo["FF0004"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Ah)
                                    recipeInfo["FF0002"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Current
                                    recipeInfo["FF0006"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Max Temperature
                                    recipeInfo["FF0000"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Voltage
                                    recipeInfo["FF0005"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Wh)
                                    recipeInfo["FF0003"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Current
                                    recipeInfo["FF0007"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Min Temperature
                                    recipeInfo["FF0001"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Voltage

                                    // Save Condition
                                    recipeInfo["FF0201"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Delta Current
                                    recipeInfo["FF0203"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) ); // Delta Temperature
                                    recipeInfo["FF0202"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Delta Voltage
                                    recipeInfo["FF0200"].SetValue( recipe, split[index++] == "0" ? null : ( object )Util.ConvertMsToString( ( uint )double.Parse( split[index - 1] ) ) );   // Interval

                                    sequence.Add( recipe );
                                    break;
                                #endregion

                                case "eTRA":
                                    #region TransientResponse
                                    recipe = RecipeFactory.CreateInstance( RecipeType.TransientResponse );
                                    recipeInfo = recipe.GetRecipeInfo();

                                    index++;    // Bias - not used
                                    recipeInfo["030604"].SetValue( recipe, double.Parse( split[index++] ) ); // end freq - width
                                    recipeInfo["030601"].SetValue( recipe, double.Parse( split[index++] ) ); // highcurrent - high amp
                                    recipeInfo["030600"].SetValue( recipe, double.Parse( split[index++] ) ); // lowcurrent - low amp
                                    switch( split[index++] )                                                        // Tr Scale Mode
                                    {
                                        case "0":
                                            recipeInfo["030602"].SetValue( recipe, ScaleMode.Log );
                                            break;

                                        case "1":
                                            recipeInfo["030602"].SetValue( recipe, ScaleMode.Linear );
                                            break;
                                    }
                                    recipeInfo["030604"].SetValue( recipe, double.Parse( split[index++] ) ); // startfreq - delay
                                    index++;    // stepcount - transition
                                    recipeInfo["030605"].SetValue( recipe, double.Parse( split[index++] ) ); // sampling - measuretime
                                    index++;    // delay2 - not used
                                    index++;    // width2 - not used
                                    index++;    // saveraw - not used
                                    switch( split[index++] )                                                // Dc cancel range (Tr 증폭 배율)
                                    {
                                        case "0":
                                            recipeInfo["030606"].SetValue( recipe, AmplifyMode.x1 );
                                            break;

                                        case "1":
                                            recipeInfo["030606"].SetValue( recipe, AmplifyMode.x100 );
                                            break;

                                        case "2":
                                            recipeInfo["030606"].SetValue( recipe, AmplifyMode.x500 );
                                            break;
                                    }
                                    index++;    // usetra - not used

                                    index += 12;    // end condition - not used

                                    // Safety Condition
                                    recipeInfo["FF0004"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Ah)
                                    recipeInfo["FF0002"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Current
                                    recipeInfo["FF0006"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Max Temperature
                                    recipeInfo["FF0000"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Voltage
                                    recipeInfo["FF0005"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Wh)
                                    recipeInfo["FF0003"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Current
                                    recipeInfo["FF0007"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Min Temperature
                                    recipeInfo["FF0001"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Voltage

                                    sequence.Add( recipe );
                                    break;
                                    #endregion
                            }
                            break;

                        case "eCycle":
                            #region Cycle
                            recipe = RecipeFactory.CreateInstance( RecipeType.Cycle );

                            sequence.Add( recipe );
                            break;
                        #endregion

                        case "eRest":
                            #region Rest
                            recipe = RecipeFactory.CreateInstance( RecipeType.Rest );
                            recipeInfo = recipe.GetRecipeInfo();

                            index = 3;

                            // End Condition
                            recipeInfo["FF0104"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Ah
                            recipeInfo["FF0101"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Current
                            recipeInfo["FF0103"].SetValue( recipe, split[index++] == "0" ? null : ( object )Util.ConvertMsToString( ( uint )double.Parse( split[index - 1] ) ) ); // CvTime
                            recipeInfo["FF0108"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Delta Temperature
                            recipeInfo["FF0107"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Delta Voltage
                            index++;                                                                                                            // MaxPer
                            index++;                                                                                                            // MaxPerStepCount
                            recipeInfo["FF0109"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Temperature
                            recipeInfo["FF0102"].SetValue( recipe, split[index++] == "0" ? null : ( object )Util.ConvertMsToString( ( uint )double.Parse( split[index - 1] ) ) ); // Time
                            recipeInfo["FF0100"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Voltage
                            recipeInfo["FF0105"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Watt
                            recipeInfo["FF0106"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Watt Hour

                            // Safety Condition
                            recipeInfo["FF0004"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Ah)
                            recipeInfo["FF0002"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Current
                            recipeInfo["FF0006"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Max Temperature
                            recipeInfo["FF0000"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Voltage
                            recipeInfo["FF0005"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Wh)
                            recipeInfo["FF0003"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Current
                            recipeInfo["FF0007"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Min Temperature
                            recipeInfo["FF0001"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Voltage

                            // Save Condition
                            recipeInfo["FF0201"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Delta Current
                            recipeInfo["FF0203"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Delta Temperature
                            recipeInfo["FF0202"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Delta Voltage
                            recipeInfo["FF0200"].SetValue( recipe, split[index++] == "0" ? null : ( object )Util.ConvertMsToString( ( uint )double.Parse( split[index - 1] ) ) );   // Interval

                            sequence.Add( recipe );
                            break;
                        #endregion

                        case "eCharge":
                            #region Charge
                            recipe = RecipeFactory.CreateInstance( RecipeType.Charge );
                            recipeInfo = recipe.GetRecipeInfo();

                            switch( split[1] )
                            {
                                case "eCC":
                                    recipeInfo["010000"].SetValue( recipe, SourcingType_Charge.CC );
                                    break;

                                case "eCCV":
                                    recipeInfo["010000"].SetValue( recipe, SourcingType_Charge.CCCV );
                                    break;

                                case "eCR":
                                    recipeInfo["010000"].SetValue( recipe, SourcingType_Charge.CR );
                                    break;

                                case "eCP":
                                    recipeInfo["010000"].SetValue( recipe, SourcingType_Charge.CP );
                                    break;

                                // 사용하지 않는 SourcingType이나, 일단 분류만 만들어 놓음
                                case "eOCVC":
                                case "ePC":
                                default:
                                    break;
                            }

                            index = 4;

                            index++;                                                                        // Change Voltage (사용 안 함)
                            switch( split[index++] )                                                        //  Duty (Tr 증폭 배율)
                            {
                                case "0":
                                    recipeInfo["010007"].SetValue( recipe, AmplifyMode.x1 );
                                    break;

                                case "1":
                                    recipeInfo["010007"].SetValue( recipe, AmplifyMode.x100 );
                                    break;

                                case "2":
                                    recipeInfo["010007"].SetValue( recipe, AmplifyMode.x500 );
                                    break;
                            }
                            recipeInfo["010006"].SetValue( recipe, double.Parse( split[index++] ) ); // Frequency/SmplingTime
                            index++;                                                                 // High Current (사용 안 함)
                            recipeInfo["010002"].SetValue( recipe, double.Parse( split[index++] ) ); // Low Current(Current)
                            recipeInfo["010004"].SetValue( recipe, double.Parse( split[index++] ) ); // Resistance
                            recipeInfo["010001"].SetValue( recipe, double.Parse( split[index++] ) ); // Voltage
                            recipeInfo["010003"].SetValue( recipe, double.Parse( split[index++] ) ); // Power
                            switch( split[index++] )                                                        // Tr Step Mode
                            {
                                case "0":
                                    recipeInfo["010005"].SetValue( recipe, TrStepMode.None );
                                    break;

                                case "1":
                                    recipeInfo["010005"].SetValue( recipe, TrStepMode.Before );
                                    break;

                                case "2":
                                    recipeInfo["010005"].SetValue( recipe, TrStepMode.After );
                                    break;

                                case "3":
                                    recipeInfo["010005"].SetValue( recipe, TrStepMode.All );
                                    break;
                            }
                            switch( split[index++] )                                                        // Tr Scale Mode
                            {
                                case "0":
                                    recipeInfo["010008"].SetValue( recipe, ScaleMode.Log );
                                    break;

                                case "1":
                                    recipeInfo["010008"].SetValue( recipe, ScaleMode.Linear );
                                    break;
                            }
                            // End Condition
                            recipeInfo["FF0104"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Ah
                            recipeInfo["FF0101"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Current
                            recipeInfo["FF0103"].SetValue( recipe, split[index++] == "0" ? null : ( object )Util.ConvertMsToString( ( uint )double.Parse( split[index - 1] ) ) ); // CvTime
                            recipeInfo["FF0108"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Delta Temperature
                            recipeInfo["FF0107"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Delta Voltage
                            index++;                                                                                                                   // MaxPer
                            index++;                                                                                                                   // MaxPerStepCount
                            recipeInfo["FF0109"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Temperature
                            recipeInfo["FF0102"].SetValue( recipe, split[index++] == "0" ? null : ( object )Util.ConvertMsToString( ( uint )double.Parse( split[index - 1] ) ) ); // Time
                            recipeInfo["FF0100"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Voltage
                            recipeInfo["FF0105"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Watt
                            recipeInfo["FF0106"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Watt Hour

                            // Safety Condition
                            recipeInfo["FF0004"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Ah)
                            recipeInfo["FF0002"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Current
                            recipeInfo["FF0006"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Max Temperature
                            recipeInfo["FF0000"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Voltage
                            recipeInfo["FF0005"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Wh)
                            recipeInfo["FF0003"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Current
                            recipeInfo["FF0007"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Min Temperature
                            recipeInfo["FF0001"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Voltage

                            // Save Condition
                            recipeInfo["FF0201"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Delta Current
                            recipeInfo["FF0203"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) ); // Delta Temperature
                            recipeInfo["FF0202"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Delta Voltage
                            recipeInfo["FF0200"].SetValue( recipe, split[index++] == "0" ? null : ( object )Util.ConvertMsToString( ( uint )double.Parse( split[index - 1] ) ) );   // Interval

                            sequence.Add( recipe );
                            break;
                        #endregion

                        case "eDisCharge":
                            #region Discharge
                            recipe = RecipeFactory.CreateInstance( RecipeType.Discharge );
                            recipeInfo = recipe.GetRecipeInfo();

                            switch( split[1] )
                            {
                                case "eCC":
                                case "eCCV":
                                    recipeInfo["020000"].SetValue( recipe, SourcingType_Discharge.CC );
                                    break;

                                case "eCR":
                                    recipeInfo["020000"].SetValue( recipe, SourcingType_Discharge.CR );
                                    break;

                                case "eCP":
                                    recipeInfo["020000"].SetValue( recipe, SourcingType_Discharge.CP );
                                    break;

                                // 사용하지 않는 SourcingType이나, 일단 분류만 만들어 놓음
                                case "eOCVC":
                                case "ePC":
                                default:
                                    break;
                            }

                            index = 4;

                            index++;                                                                        // Change Voltage (사용 안 함)
                            switch( split[index++] )                                                        //  Duty (Tr 증폭 배율)
                            {
                                case "0":
                                    recipeInfo["020007"].SetValue( recipe, AmplifyMode.x1 );
                                    break;

                                case "1":
                                    recipeInfo["020007"].SetValue( recipe, AmplifyMode.x100 );
                                    break;

                                case "2":
                                    recipeInfo["020007"].SetValue( recipe, AmplifyMode.x500 );
                                    break;
                            }
                            recipeInfo["020006"].SetValue( recipe, double.Parse( split[index++] ) );        // Frequency/SmplingTime
                            index++;                                                                        // High Current (사용 안 함)
                            recipeInfo["020002"].SetValue( recipe, double.Parse( split[index++] ) );        // Low Current(Current)
                            recipeInfo["020004"].SetValue( recipe, double.Parse( split[index++] ) );        // Resistance
                            index++;
                            //recipeInfo[ "010100" ].SetValue( recipe, double.Parse( split[index++] ) );    // Voltage (Discharge에 사용 안 함)
                            recipeInfo["020003"].SetValue( recipe, double.Parse( split[index++] ) );        // Power
                            switch( split[index++] )                                                        // Tr Step Mode
                            {
                                case "0":
                                    recipeInfo["020005"].SetValue( recipe, TrStepMode.None );
                                    break;

                                case "1":
                                    recipeInfo["020005"].SetValue( recipe, TrStepMode.Before );
                                    break;

                                case "2":
                                    recipeInfo["020005"].SetValue( recipe, TrStepMode.After );
                                    break;

                                case "3":
                                    recipeInfo["020005"].SetValue( recipe, TrStepMode.All );
                                    break;
                            }
                            switch( split[index++] )                                                        // Tr Scale Mode
                            {
                                case "0":
                                    recipeInfo["020008"].SetValue( recipe, ScaleMode.Log );
                                    break;

                                case "1":
                                    recipeInfo["020008"].SetValue( recipe, ScaleMode.Linear );
                                    break;
                            }
                            // End Condition
                            recipeInfo["FF0104"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Ah
                            recipeInfo["FF0101"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Current
                            recipeInfo["FF0103"].SetValue( recipe, split[index++] == "0" ? null : ( object )Util.ConvertMsToString( ( uint )double.Parse( split[index - 1] ) ) ); // CvTime
                            recipeInfo["FF0108"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Delta Temperature
                            recipeInfo["FF0107"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Delta Voltage
                            index++;                                                                                                                   // MaxPer
                            index++;                                                                                                                   // MaxPerStepCount
                            recipeInfo["FF0109"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Temperature
                            recipeInfo["FF0102"].SetValue( recipe, split[index++] == "0" ? null : ( object )Util.ConvertMsToString( ( uint )double.Parse( split[index - 1] ) ) ); // Time
                            recipeInfo["FF0100"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Voltage
                            recipeInfo["FF0105"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Watt
                            recipeInfo["FF0106"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Watt Hour

                            // Safety Condition
                            recipeInfo["FF0004"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Ah)
                            recipeInfo["FF0002"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Current
                            recipeInfo["FF0006"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Max Temperature
                            recipeInfo["FF0000"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Voltage
                            recipeInfo["FF0005"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Max Capacity(Wh)
                            recipeInfo["FF0003"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Current
                            recipeInfo["FF0007"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) );  // Min Temperature
                            recipeInfo["FF0001"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Min Voltage

                            // Save Condition
                            recipeInfo["FF0201"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Delta Current
                            recipeInfo["FF0203"].SetValue( recipe, split[index++] == "0" ? null : ( object )float.Parse( split[index - 1] ) ); // Delta Temperature
                            recipeInfo["FF0202"].SetValue( recipe, split[index++] == "0" ? null : ( object )double.Parse( split[index - 1] ) ); // Delta Voltage
                            recipeInfo["FF0200"].SetValue( recipe, split[index++] == "0" ? null : ( object )Util.ConvertMsToString( ( uint )double.Parse( split[index - 1] ) ) );   // Interval

                            sequence.Add( recipe );
                            break;
                        #endregion

                        case "eLoop":
                            #region Loop
                            recipe = RecipeFactory.CreateInstance( RecipeType.Loop );
                            recipeInfo = recipe.GetRecipeInfo();

                            index = 3;
                            recipeInfo["060000"].SetValue( recipe, uint.Parse( split[index++] ) );   // Loop Count

                            sequence.Add( recipe );
                            break;
                        #endregion

                        //case "ePattern":
                        //    recipe = RecipeFactory.CreateInstance( RecipeType.Pattern );

                        case "eTemp":
                            //recipe = RecipeFactory.CreateInstance( RecipeType.Temperature );
                            //recipeInfo = recipe.GetRecipeInfo();

                            //index = 4;

                            //switch( split[index++] )
                            //{
                            //    case "eConstantTemperature":
                            //        recipeInfo["080000"].SetValue( recipe, TemperatureMode.Constant );
                            //        break;

                            //    case "eVariableTemperature":
                            //        recipeInfo["080000"].SetValue( recipe, TemperatureMode.Variable );
                            //        break;
                            //}


                            //sequence.Add( recipe );
                            break;

                    }
                }
            }

            return sequence;
        }
    }
}
