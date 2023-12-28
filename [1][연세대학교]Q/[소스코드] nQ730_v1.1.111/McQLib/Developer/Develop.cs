using System;
using JmCmdLib;
using McQLib.Device;
using McQLib.GUI;

namespace McQLib.Developer
{
    public class DevelopConsole : JmConsole
    {
        #region Custon members
        public Communicator[] Communicators;
        private int _mem_index;
        public Channel[] Dummy;
        public dynamic MainForm;
        #endregion

        public override void Processing( string cmd )
        {
            var split = cmd.Split( " ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );

            if ( Get( split, 0 ).ToLower() == "grab" )
            {
                switch ( Get( split, 1 ).ToLower() )
                {
                    case "com":
                    case "communicator":
                        if ( Communicators == null )
                        {
                            WriteLine( "Grab failed. 'Communicators' was null." );
                        }
                        else if ( Communicators.Length == 0 )
                        {
                            WriteLine( "Grab failed. 'Communicators' was empty." );
                        }
                        else if ( int.TryParse( Get( split, 2 ), out int index ) )
                        {
                            if ( index < 0 || index >= Communicators.Length )
                            {
                                WriteLine( "Grab failed. Index out of range." );
                            }
                            else
                            {
                                Target = Communicators[index];

                                LocationText = $"Communicator[{_mem_index = index}]";
                                WriteLine( $"Grabbed 'Communicator[{_mem_index}]'." );
                            }
                        }
                        else
                        {
                            if ( Get( split, 2 ) == string.Empty )
                            {
                                WriteLine( "Wrong usage. (Usage : 'grab communicator index')" );
                            }
                            else
                            {
                                WriteLine( $"'{Get( split, 2 )}' can not be converted to integer." );
                            }
                        }
                        break;

                    case "main":
                        Target = MainForm;
                        LocationText = "Main";
                        WriteLine( "Grabbed 'Main'." );
                        break;

                    case "channel":
                    case "ch":
                        if ( Communicator.TotalChannels == null )
                        {
                            WriteLine( "Grab failed. 'Communicators.TotalChannels' was null." );
                        }
                        else if ( int.TryParse( Get( split, 2 ), out int index ) )
                        {
                            if ( index < 0 || index >= Communicator.TotalChannels.Length )
                            {
                                WriteLine( "Grab failed. Index out of range." );
                            }
                            else
                            {
                                Target = Communicator.TotalChannels[index];

                                LocationText = $"Channel[{_mem_index = index}]";
                                WriteLine( $"Grabbed 'Channel[{_mem_index}]'." );
                            }
                        }
                        else
                        {
                            if ( Get( split, 2 ) == string.Empty )
                            {
                                WriteLine( "Wrong usage. (Usage : 'grab channel index')" );
                            }
                            else
                            {
                                WriteLine( $"'{Get( split, 2 )}' can not be converted to integer." );
                            }
                        }
                        break;

                    case "dummy":
                        if ( Dummy == null )
                        {
                            WriteLine( "Grab failed. 'Dummy' was null." );
                        }
                        else if ( int.TryParse( Get( split, 2 ), out int index ) )
                        {
                            if ( index < 0 || index >= Dummy.Length )
                            {
                                WriteLine( "Grab failed. Index out of range." );
                            }
                            else
                            {
                                Target = Dummy[index];

                                LocationText = $"Dummy[{_mem_index = index}]";
                                WriteLine( $"Grabbed 'Dummy[{_mem_index}]'." );
                            }
                        }
                        else
                        {
                            if ( Get( split, 2 ) == string.Empty )
                            {
                                WriteLine( "Wrong usage. (Usage : 'grab dummy index')" );
                            }
                            else
                            {
                                WriteLine( $"'{Get( split, 2 )}' can not be converted to integer." );
                            }
                        }
                        break;

                    default:
                        WriteLine( $"Grab failed. '{Get( split, 1 )}' not found." );
                        break;
                }
            }
            else if ( Get( split, 0 ).ToLower() == "free" )
            {
                Target = null;
                LocationText = "";
                WriteLine( "Grabbed nothing." );
            }
            else if ( Get( split, 0 ).ToLower() == "recipe" )
            {
                switch ( Get( split, 1 ).ToLower() )
                {
                    case "get":
                        var result = Recipes.RecipeSetting.Get( cmd.ToLower().Replace( "recipe get ", "" ) );

                        if ( result != null )
                        {
                            WriteLine( $"{split[2]}'s {split[3]} is {result}." );
                        }
                        else
                        {
                            WriteLine( "Failed." );
                        }
                        break;

                    case "set":
                        result = Recipes.RecipeSetting.Set( cmd.ToLower().Replace( "recipe set ", "" ) );

                        if ( ( bool )result )
                        {
                            WriteLine( $"{split[2]}'s {split[3]} set to {split[4]}." );
                        }
                        else
                        {
                            WriteLine( "Failed." );
                        }
                        break;

                    case "preset":
                        switch(Get(split, 2 ).ToLower() )
                        {
                            case "q100lab":
                                Recipes.RecipeSetting.Charge.Enabled = true;
                                Recipes.RecipeSetting.Discharge.Enabled = true;
                                Recipes.RecipeSetting.Rest.Enabled = true;
                                Recipes.RecipeSetting.Cycle.Enabled = true;
                                Recipes.RecipeSetting.Loop.Enabled = true;
                                Recipes.RecipeSetting.Jump.Enabled = true;
                                Recipes.RecipeSetting.Label.Enabled = true;
                                Recipes.RecipeSetting.OpenCircuitVoltage.Enabled = true;

                                Recipes.RecipeSetting.FrequencyResponse.Enabled = false;
                                Recipes.RecipeSetting.TransientResponse.Enabled = false;
                                Recipes.RecipeSetting.AcResistance.Enabled = false;
                                Recipes.RecipeSetting.DcResistance.Enabled = false;
                                Recipes.RecipeSetting.Pattern.Enabled = false;
                                Recipes.RecipeSetting.Temperature.Enabled = false;
                                Recipes.RecipeSetting.CdCycle.Enabled = false;
                                Recipes.RecipeSetting.AnodeCharge.Enabled = false;
                                Recipes.RecipeSetting.AnodeDischarge.Enabled = false;

                                WriteLine( "Recipe set to preset Q100LAB." );
                                break;

                            case "allon":
                                Recipes.RecipeSetting.Charge.Enabled = true;
                                Recipes.RecipeSetting.Discharge.Enabled = true;
                                Recipes.RecipeSetting.Rest.Enabled = true;
                                Recipes.RecipeSetting.Cycle.Enabled = true;
                                Recipes.RecipeSetting.Loop.Enabled = true;
                                Recipes.RecipeSetting.Jump.Enabled = true;
                                Recipes.RecipeSetting.Label.Enabled = true;
                                Recipes.RecipeSetting.OpenCircuitVoltage.Enabled = true;

                                Recipes.RecipeSetting.FrequencyResponse.Enabled = true;
                                Recipes.RecipeSetting.TransientResponse.Enabled = true;
                                Recipes.RecipeSetting.AcResistance.Enabled = true;
                                Recipes.RecipeSetting.DcResistance.Enabled = true;
                                Recipes.RecipeSetting.Pattern.Enabled = true;
                                Recipes.RecipeSetting.Temperature.Enabled = true;
                                Recipes.RecipeSetting.CdCycle.Enabled = true;
                                Recipes.RecipeSetting.AnodeCharge.Enabled = true;
                                Recipes.RecipeSetting.AnodeDischarge.Enabled = true;

                                WriteLine( "Recipe set to preset AllOn." );
                                break;

                            case "alloff":
                                Recipes.RecipeSetting.Charge.Enabled = false;
                                Recipes.RecipeSetting.Discharge.Enabled = false;
                                Recipes.RecipeSetting.Rest.Enabled = false;
                                Recipes.RecipeSetting.Cycle.Enabled = false;
                                Recipes.RecipeSetting.Loop.Enabled = false;
                                Recipes.RecipeSetting.Jump.Enabled = false;
                                Recipes.RecipeSetting.Label.Enabled = false;
                                Recipes.RecipeSetting.OpenCircuitVoltage.Enabled = false;

                                Recipes.RecipeSetting.FrequencyResponse.Enabled = false;
                                Recipes.RecipeSetting.TransientResponse.Enabled = false;
                                Recipes.RecipeSetting.AcResistance.Enabled = false;
                                Recipes.RecipeSetting.DcResistance.Enabled = false;
                                Recipes.RecipeSetting.Pattern.Enabled = false;
                                Recipes.RecipeSetting.Temperature.Enabled = false;
                                Recipes.RecipeSetting.CdCycle.Enabled = false;
                                Recipes.RecipeSetting.AnodeCharge.Enabled = false;
                                Recipes.RecipeSetting.AnodeDischarge.Enabled = false;

                                WriteLine( "Recipe set to preset AllOff." );
                                break;
                        }
                        break;

                    default:
                        WriteLine( "Wrong command." );
                        break;
                }
            }
            else if ( Get( split, 0 ).ToLower() == "help" )
            {
                WriteLine( string.Format( "\r\n {0, -35}{1}\r\n", "Command", "Destruction" ) + GetHelpString() );
            }
            else if ( Get( split, 0 ).ToLower() == "open" )
            {
                switch ( Get( split, 1 ).ToLower() )
                {
                    case "monitor":
                        if ( int.TryParse( Get( split, 2 ), out int index ) )
                        {
                            if ( index < 0 || index >= Communicators.Length )
                            {
                                WriteLine( "Failed. Index out of range." );
                            }
                            else
                            {
                                new Form_QueueMonitor( Communicators[index] ).Show();
                            }
                        }
                        else
                        {
                            if ( Get( split, 2 ) == string.Empty )
                            {
                                WriteLine( "Wrong usage. (Usage : 'monitor communicator_index')" );
                            }
                            else
                            {
                                WriteLine( $"'{Get( split, 2 )}' can not be converted to integer." );
                            }
                        }
                        break;

                    case "tester":
                        MainForm.startTestSw();
                        break;

                    case "parser":
                        MainForm.startParser();
                        break;
                }
            }
            else if ( Get( split, 0 ).ToLower() == "ping" )
            {
                if ( Communicators == null )
                {
                    WriteLine( "Ping failed. 'Communicators' was null." );
                }
                else if ( Communicators.Length == 0 )
                {
                    WriteLine( "Ping failed. 'Communicators' was empty." );
                }
                else if ( !int.TryParse( Get( split, 1 ), out int index ) )
                {
                    WriteLine( "Wrong usage. (Usage : 'ping {commIndex}' or 'ping {commIndex} {boardIndex}'." );
                }
                else
                {
                    if ( index < 0 || index >= Communicators.Length )
                    {
                        WriteLine( "Ping failed. Index out of range." );
                    }
                    else if ( int.TryParse( Get( split, 2 ), out int addr ) )
                    {
                        var packet = new SendPacket( ( byte )addr, 0 );
                        packet.ByPass = Packet.ON;
                        packet.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.Ping_G ) );

                        var receive = Communicators[index].SendAndReceive( packet, 2 );
                        if ( receive == null )
                        {
                            WriteLine( "Not response." );
                        }
                        else
                        {
                            WriteLine( receive.SubPacket.ERR == 0 ? "Ping received." : $"Ping received but error. ({receive.SubPacket.ERR})" );
                        }
                    }
                    else
                    {
                        var packet = new SendPacket( 0, 0 );
                        packet.SubPackets.Add( new SendSubPacket( Commands.CommonCommands.Ping_G ) );

                        var receive = Communicators[index].SendAndReceive( packet, 2 );
                        if ( receive == null )
                        {
                            WriteLine( "Not response." );
                        }
                        else
                        {
                            WriteLine( receive.SubPacket.ERR == 0 ? "Ping received." : $"Ping received but error. ({receive.SubPacket.ERR})" );
                        }
                    }
                }
            }
            else
            {
                base.Processing( cmd );
            }
        }

        protected override string GetHelpString()
        {
            return base.GetHelpString() +
                string.Format( " {0, -35}{1}\r\n", "grab [target_name]", "Grab some object to do something." ) +
                string.Format( " {0, -35}{1}\r\n", "grab [target_name] [index]", "Grab some object, in object array." ) +
                string.Format( " {0, -35}{1}\r\n", "free", "Put current target down." ) +
                string.Format( " {0, -35}{1}\r\n", "open monitor", "Open communicator queue monitor." ) +
                string.Format( " {0, -35}{1}\r\n", "open tester", "Open Q730 Test SW." );
        }
    }
}
