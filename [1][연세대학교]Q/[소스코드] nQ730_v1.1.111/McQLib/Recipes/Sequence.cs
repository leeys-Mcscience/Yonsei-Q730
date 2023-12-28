using McQLib.Core;
using McQLib.Device;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace McQLib.Recipes
{
    public class Sequence : ICloneable
    {
        public const ushort MAX_STORAGE = 99;
        public List<Recipe> _recipes;
        private bool _isChanged;
        private string _path;
        private string _comment = string.Empty;

        public CrateInfo _crateInfos;

        public List<CrateInfo> _crateList;

        public BatteryInfo _batteryInfo;


    #region Properties
    public static string DefaultDirectory => Path.Combine( Util.StartDirectory, "Sequence" );
        public static bool IsNullOrEmpty( Sequence sequence ) => sequence == null || sequence.Count == 0;
        public int Count => _recipes.Count;
        public Recipe this[int index] => _recipes[index];

        public void Add( Recipe recipe )
        {
            if( _recipes.Count == MAX_STORAGE ) throw new QException( QExceptionType.SEQUENCE_ALREADY_FULL_ERROR );

            //if ( !RecipeSetting.IsRecipeEnabled( recipe.GetRecipeType() ) ) return;

            _recipes.Add( recipe );
            _isChanged = true;
        }
        public void Insert( int index, Recipe recipe )
        {
            try
            {
                _recipes.Insert( index, recipe );
            }
            catch( ArgumentOutOfRangeException ex )
            {
                throw new QException( QExceptionType.SEQUENCE_INDEX_OUT_OF_RANGE_ERROR, $"Index = {index}", ex );
            }
            _isChanged = true;
        }
        public void Remove( Recipe recipe )
        {
            _recipes.Remove( recipe );

            _isChanged = true;
        }
        public void RemoveAt( int index )
        {
            try
            {
                _recipes.RemoveAt( index );
            }
            catch( IndexOutOfRangeException ex )
            {
                throw new QException( QExceptionType.SEQUENCE_INDEX_OUT_OF_RANGE_ERROR, $"Index = {index}", ex );
            }
            _isChanged = true;
        }
        public void Clear()
        {
            _recipes.Clear();
            _isChanged = true;
        }

        /// <summary>
        /// 시퀀스의 이름입니다.
        /// <br>시퀀스가 실제로 저장되는 경로와 파일명은 "Application.StartupPath\Sequence\Name.seq"입니다. </br>
        /// </summary>
        public string Name
        {
            get => Path.GetFileName( _path ).Replace( ".seq", "" );
            set
            {
                if( value == Name ) return;
                if( value.Length == 0 ) return;
                _path = _path.Replace( Name, value.Replace( ".seq", "" ) );

                //_actionStack.Add( new EditAction( ActionType.Name, _name ) );
                _isChanged = true;
            }
        }
        public string Comment
        {
            get => _comment;
            set
            {
                if( value == _comment ) return;

                //_actionStack.Add( new EditAction( ActionType.Comment, _comment ) );
                _comment = value;
                _isChanged = true;
            }
        }
        public string FilePath
        {
            get => _path;
            set => _path = value;
        }
        /// <summary>
        /// 시퀀스가 수정되었는지의 여부입니다.
        /// </summary>
        public bool IsChanged => _isChanged;
        #endregion

        /// <summary>
        /// 비어있는 상태의 시퀀스 인스턴스를 생성합니다.
        /// </summary>
        public Sequence()
        {
            //_actionStack = new List<EditAction>();
            _path = Path.Combine( DefaultDirectory, createSequenceName() + ".seq" );
            _recipes = new List<Recipe>();
            _crateInfos = new CrateInfo();
            _crateList = new List<CrateInfo>();
            _batteryInfo = new BatteryInfo();
        }

        /// <summary>
        /// 지정된 경로의 시퀀스 파일로부터 시퀀스 정보를 읽어와 시퀀스의 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Sequence FromFile( string filename )
        {
            try
            {
                if ( !File.Exists( filename ) ) return null;

                var seq = load( filename );
                seq._isChanged = false;
                return seq;
            }
            catch (Exception ex)
            {
                Console.WriteLine( $"Load sequence file exception : {ex.Message}" );
                return null;
            }

        }
        public void Save()
        {
            if( Name == null || Name == "" ) throw new QException( QExceptionType.SEQUENCE_NAME_EMPTY_ERROR );

            try
            {
                ToPacketArray( 0, 0 );
            }
            catch ( QException ex )
            {
                throw ex;
            }

            save( _path );
            _isChanged = false;
        }
        public void Save(string path)
        {
            if (Name == null || Name == "") throw new QException(QExceptionType.SEQUENCE_NAME_EMPTY_ERROR);

            try
            {
                ToPacketArray(0, 0);
            }
            catch (QException ex)
            {
                throw ex;
            }

            save(path);
            _isChanged = false;
        }
        public void SaveAs(string path)
        {
            if (Name == null || Name == "") throw new QException(QExceptionType.SEQUENCE_NAME_EMPTY_ERROR);

            try
            {
                ToPacketArray(0, 0);
            }
            catch (QException ex)
            {
                throw ex;
            }

            save(path);
            _isChanged = false;
        }

        private void save( string filename )
        {
            using ( var sw = new StreamWriter( filename ) )
            {
                sw.WriteLine( "== 이 파일의 내용을 절대로 직접 수정하지 마십시오. ==" );
                // 코멘트
                sw.WriteLine( $"{Comment.Replace( "\r\n", "/r/n" )}" );
                
                for ( var i = 0; i < _recipes.Count; i++ )
                {
                    var recipeInfo = _recipes[i].GetRecipeInfo();
                    // 레시피 정보
                    sw.WriteLine( $"{( ushort )recipeInfo.RecipeType:X4}" );

                    for (var j = 0; j < recipeInfo.Count; j++)
                    {
                        object value = recipeInfo[j].GetValue(_recipes[i]);
                        sw.WriteLine($"{recipeInfo[j].ID}?{(value == null ? "Null" : value)}");
                    }

                }
                //sw.WriteLine($"1?CRATE?{_crateInfos.cRate }");
                //sw.WriteLine($"2?ELCCAPA?{_crateInfos.elcCapa}");
                // 종료

                foreach (var crateInfo in _crateList)
                {
                    sw.WriteLine($"{crateInfo.index}?{crateInfo.cRate}?{crateInfo.elcCapa}?{crateInfo.checkCrate}");
                }

               
                sw.WriteLine($"{_batteryInfo.batteryMass}#{_batteryInfo.batteryArea}");
               


            }
        }
        private static Sequence load( string filename )
        {
            var seq = new Sequence();

            seq._path = filename;

            using (var sr = new StreamReader(filename))
            {
                string line;

                line = sr.ReadLine();
                try
                {
                    if (line == "[SequenceInfomation]")
                    {
                        // 구버전 시퀀스 파일
                        try
                        {
                            return OldSupport.OldSequence.FromFile(filename);
                        }
                        catch (Exception ex)
                        {
                            throw new QException(QExceptionType.IO_WRONG_DATA_FORMAT_ERROR);
                        }
                        //catch (Exception ex)
                        //{
                        //    // 구 버전 시퀀스 파일 읽기에 실패한 경우
                        //    // 원래 여기서 null을 반환하는게 맞긴 하나, 정말 만에 하나라도 누군가가 의도적으로 파일의 첫 번째 줄에
                        //    // [SequenceInfomation]을 적은 경우를 대비하여 냅둔다.
                        //}
                    }

                    //if (seq._comment.ToString() != "")
                    //{ 
                    seq._comment = sr.ReadLine().Replace("/r/n", "\r\n");

                    Recipe current = null;
                    RecipeInfo recipeInfo = null;
                    int cnt = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!line.Contains("#"))
                        {
                            var split = line.Split('?');
                            if (split.Length == 1)
                            {
                                if (current != null) seq.Add(current);

                                current = RecipeFactory.CreateInstance((RecipeType)Convert.ToUInt16(split[0], 16));
                                recipeInfo = current.GetRecipeInfo();

                                continue;
                            }
                            else if (split.Length == 2)
                            {
                                var param = recipeInfo[split[0]];
                                if (param == null) continue;

                                try
                                {
                                    if (split[1].ToLower() == "null") param.SetValue(current, null);
                                    else
                                    {
                                        Type t;
                                        object value;
                                        if (param.ValueType.Name == "Nullable`1")
                                            t = Nullable.GetUnderlyingType(param.ValueType);
                                        else if (param.ValueType.BaseType == typeof(Enum)) t = typeof(Enum);
                                        else t = param.ValueType;

                                        switch (t.Name)
                                        {
                                            case "Double":
                                                value = double.Parse(split[1]);
                                                break;
                                            case "Int32":
                                            case "UInt32":
                                                value = uint.Parse(split[1]);
                                                break;
                                            case "Int16":
                                            case "Uint16":
                                                value = ushort.Parse(split[1]);
                                                break;
                                            case "Single":
                                                value = float.Parse(split[1]);
                                                break;
                                            case "Enum":
                                                //if (split[0] == "0100010")
                                                //{
                                                //    value = SourcingType_CurrentUnit.A;
                                                //}
                                                //else
                                                //{
                                                //    value = Enum.Parse(param.ValueType, split[1]);
                                                //}
                                                value = Enum.Parse(param.ValueType, split[1]);
                                                break;
                                            case "Boolean":
                                                value = bool.Parse(split[1]);
                                                break;
                                            case "DateTime":
                                                value = DateTime.Parse(split[1]);
                                                break;
                                            case "TimeSpan":
                                                value = TimeSpan.Parse(split[1]);
                                                break;
                                            case "String":
                                                value = split[1];
                                                break;

                                            default:
                                                value = null;
                                                break;
                                        }

                                        param.SetValue(current, value);
                                    }
                                    //else param.SetValue( current, Convert.ChangeType( split[1], param.ValueType ) );

                                }
                                catch (Exception ex)
                                {
                                    throw new QException(QExceptionType.IO_WRONG_DATA_FORMAT_ERROR);
                                }
                            }
                            else if (split.Length == 4)
                            {
                                seq._crateList.Add(new CrateInfo
                                {
                                    index = Convert.ToInt32(split[0]),
                                    cRate = Convert.ToDouble(split[1]),
                                    elcCapa = Convert.ToDouble(split[2]),
                                    checkCrate = Convert.ToBoolean(split[3])
                                });
                            }
                        }

                        else if (line.Contains("#"))
                        {
                            var temp = line.Split('#');
                            if (temp.Length == 2)
                            {
                                seq._batteryInfo.batteryMass = Convert.ToDouble(temp[0]);
                                seq._batteryInfo.batteryArea = Convert.ToDouble(temp[1]);
                            }
                        }

                        cnt++;
                    }

                    if (current != null) seq.Add(current);
                    return seq;

                }

                catch (Exception ex)
                {
                    throw new QException(QExceptionType.IO_WRONG_DATA_FORMAT_ERROR);
                }


            }
        }

        private string createSequenceName()
        {
            int num;
            string path = DefaultDirectory;

            if( !new FileInfo( Path.Combine( path, "NewSequence.seq" ) ).Exists ) return "NewSequence";

            for( num = 2; ; num++ )
            {
                if( !new FileInfo( Path.Combine( path, $"NewSequence ({num}).seq" ) ).Exists )
                    return $"NewSequence ({num})";
            }
        }

        /// <summary>
        /// 현재 시퀀스를 장비로 송신하기 위한 패킷 배열을 생성하여 반환합니다.
        /// <br>현재 메서드를 통해 반환되는 패킷 배열을 순서대로 장비에 모두 송신하십시오.</br>
        /// </summary>
        /// <param name="addr">현재 시퀀스를 수신할 채널이 위치하는 보드의 번호입니다.</param>
        /// <param name="ch">현재 시퀀스를 수신할 채널의 번호입니다.</param>
        /// <returns>생성된 패킷 배열입니다.</returns>
        public SendPacket[] ToPacketArray( byte addr, byte ch )
        {
            var packets = new List<SendPacket>();

            // 패킷을 변환할 때 _recipes가 아닌 복사본을 이용한다.
            // Jump-Label 때문에 리스트를 변경해야할 수도 있기 때문.
            var recipes = new List<Recipe>( _recipes.ToArray() );

            int cyclePosition = -1;
            int recipeIndex = 0;

            // Label의 위치와 LabelName을 기억해둔다.
            var labels = new List<KeyValuePair<Recipe, int>>();
            for( var i = 0; i < recipes.Count; i++ )
            {
                if( recipes[i] is Label )
                {
                    // 현재 Label 레시피의 위쪽에 Cycle 레시피가 존재하는지 확인한다.
                    // Jump 레시피는 Cycle의 내부로 점프할 수 없음.
                    for( var j = i; j >= 0; j-- )
                    {
                        if( recipes[j] is Cycle )
                        {
                            var ex = new QException( QExceptionType.SEQUENCE_INVALID_LABEL_LOCATION_ERROR );
                            recipes[i].Error = ex.Message;
                            throw ex;
                        }
                        else if( recipes[j] is Loop )
                        {
                            break;
                        }
                    }
                    // label인 경우 label의 위치를 확인해야 한다.
                    labels.Add( new KeyValuePair<Recipe, int>( recipes[i], i ) );
                }
            }

            // Label은 다 지워버림. Label 때문에 인덱스가 한 칸씩 밀리므로
            for( var i = 0; i < labels.Count; i++ ) recipes.Remove( labels[i].Key );

            SendPacket packet = null;
            var recipesInPacket = 0;

            while ( true )
            {
                if ( recipeIndex == recipes.Count ) break;

                var type = recipes[recipeIndex].GetRecipeType();

                if ( !RecipeSetting.IsRecipeEnabled( type ) ) throw new QException( QExceptionType.RECIPE_NOT_ALLOWED_ERROR );

                if ( packet == null )
                {
                    packet = new SendPacket( addr, ch );

                    Enum cmd;

                    switch ( type )
                    {
                        case RecipeType.Rest:
                        case RecipeType.Cycle:
                        case RecipeType.Loop:
                        case RecipeType.Jump:
                            cmd = Commands.BatteryCycler_SetGetCommands.SequenceStep_GS;
                            break;

                        case RecipeType.Charge:
                        case RecipeType.Discharge:
                        case RecipeType.AnodeCharge:
                        case RecipeType.AnodeDischarge:
                            cmd = Commands.BatteryCycler_SetGetCommands.SequenceChargeDischarge_GS;
                            break;

                        case RecipeType.TransientResponse:
                        case RecipeType.OpenCircuitVoltage:
                        case RecipeType.DcResistance:
                        case RecipeType.AcResistance:
                        case RecipeType.FrequencyResponse:
                            cmd = Commands.BatteryCycler_SetGetCommands.SequenceMeasurement_GS;
                            break;

                        case RecipeType.Pattern:
                            cmd = Commands.BatteryCycler_SetGetCommands.SequencePattern_GS;
                            break;

                        default:
                            cmd = null;
                            break;
                    }

                    if ( cmd == null ) throw new QException( QExceptionType.DEVELOP_UNDEFINED_RECIPE_TYPE_ERROR );

                    packet.SubPackets.Add( new SendSubPacket( cmd ) );
                    packet.SubPacket.ERR = Packet.SET;
                }

                try
                {
                    // packet이 null이 아니고, 지금 넣을 레시피가 패턴인 경우 Flush
                    if ( type == RecipeType.Pattern )
                    {
                        packets.Add( packet );

                        packet = new SendPacket( addr, ch );
                        packet.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.SequencePattern_GS ) );
                        packet.SubPacket.ERR = Packet.SET;
                        packet.SubPacket.DATA.Add( ( recipes[recipeIndex] as IPacketConvertable ).ToDataField( ( ushort )recipeIndex, ( ushort )( recipeIndex + 1 ), ( ushort )recipes.Count ) );
                        packets.Add( packet );

                        packet = null;
                        recipesInPacket = 0;

                        // 패턴 정보를 담는 패킷을 추가적으로 생성하여 넣는다.
                        var patternData = ( recipes[recipeIndex] as Pattern ).PatternData.ToDataField( ( ushort )recipeIndex );

                        for ( var i = 0; i < patternData.Count; i++ )
                        {
                            var patternDataPacket = new SendPacket( addr, ch );
                            patternDataPacket.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.SequencePatternData_GS ) );
                            patternDataPacket.SubPackets[0].ERR = Packet.SET;

                            patternDataPacket.SubPackets[0].DATA.Add( patternData[i] );

                            packets.Add( patternDataPacket );
                        }
                    }
                    else
                    {
                        // Cycle을 만난 경우 해당 위치를 기억한다.
                        if ( type == RecipeType.Cycle ) cyclePosition = recipeIndex;
                        ushort endStepNo, errorStepNo;

                        endStepNo = ( ushort )( recipeIndex + 1 );
                        if ( type == RecipeType.Loop )
                        {
                            // Loop Recipe일 경우 ErrorStepNo를 가장 최근에 만난 Cycle Recipe의 Position으로 해준다.
                            // 만약 가장 최근에 만난 Cycle이 없는 경우 Cycle-Loop Not Matched 에러
                            if ( cyclePosition == -1 )
                            {
                                //recipes[recipeIndex].Error = true;
                                throw new QException( QExceptionType.SEQUENCE_CYCLE_LOOP_NOT_MATCHED_ERROR, $"Recipe[{recipeIndex}]" );
                            }

                            errorStepNo = ( ushort )cyclePosition;

                            // 해당 Cycle은 매칭된 것이므로 cyclePosition은 다시 -1로 
                            cyclePosition = -1;
                        }
                        else if ( type == RecipeType.Jump )
                        {
                            // Jump Recipe인 경우 Jump할 Label을 찾아서 Label이 있던 위치를 errorStepNo로 지정한다.
                            var jumpPosition = -1;
                            for ( var i = 0; i < labels.Count; i++ )
                            {
                                if ( ( labels[i].Key as Label ).LabelName == ( recipes[recipeIndex] as Jump ).LabelName )
                                {
                                    jumpPosition = labels[i].Value;
                                    // Jump 레시피는 Jump보다 아래로는 점프할 수 없다.
                                    if ( recipeIndex < jumpPosition )
                                    {
                                        throw new QException( QExceptionType.SEQUENCE_CANNOT_UNDER_JUMP_ERROR );
                                    }
                                    break;
                                }
                            }

                            if ( jumpPosition == -1 )
                            {
                                //recipes[recipeIndex].Error = true;
                                throw new QException( QExceptionType.SEQUENCE_LABEL_NOT_FOUND_ERROR );
                            }
                            else if ( cyclePosition != -1 )
                            {
                                throw new QException( QExceptionType.SEQUENCE_INVALID_JUMP_LOCATION_ERROR );
                            }

                            // Jump의 경우 EndStepNo = 점프할 위치
                            // ErrorStepNo = 점프 횟수 도달 후 이동할 위치(즉, 다음 레시피)
                            endStepNo = ( ushort )jumpPosition;
                            errorStepNo = ( ushort )( recipeIndex + 1 );
                        }
                        else
                        {
                            errorStepNo = ( ushort )recipes.Count;  // 20230621 정홍욱 수석님 미팅 : Safty 상태는 일시정지 상태이고 이 상태에서 Run을 하게되면 
                                                                    // 가장 마지막으로 점프하게끔 기존 사람이 코드를 짜놓은것 확인.
                                                                    // 다른 회사들은 이런 상태일때 이어 가능하도록 지정되어 있음.
                        }

                        //if (recipes[recipesInPacket].Name == "Charge")
                        //{
                        //    (recipes[recipesInPacket] as Charge).Current = 0.008;
                        //}
                        packet.SubPacket.DATA.Add( ( recipes[recipeIndex] as IPacketConvertable ).ToDataField( ( ushort )recipeIndex, endStepNo, errorStepNo ) );
                        recipesInPacket++;

                        if ( recipesInPacket == 10 )
                        {
                            packets.Add( packet );
                            packet = null;
                            recipesInPacket = 0;
                        }
                    }

                }
                catch ( QException ex )
                {
                    recipes[recipeIndex].Error = ex.Message;
                    throw ex;
                }

                recipeIndex++;
            }

            // End Recipe
            if ( packet != null )
            {
                packet.SubPacket.DATA.Add( new End().ToDataField( ( ushort )recipeIndex, ( ushort )recipeIndex, ( ushort )recipeIndex ) );
                packets.Add( packet );
            }
            else
            {
                packet = new SendPacket( addr, ch );
                packet.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.SequenceStep_GS ) );
                packet.SubPacket.ERR = Packet.SET;
                packet.SubPacket.DATA.Add( new End().ToDataField( ( ushort )recipeIndex, ( ushort )recipeIndex, ( ushort )recipeIndex ) );
                packets.Add( packet );
            }

            return packets.ToArray();
        }

        /// <summary>
        /// 현재 시퀀스를 장비로 송신하기 위한 패킷 배열을 생성하여 반환합니다.
        /// <br>현재 메서드를 통해 반환되는 패킷 배열을 순서대로 장비에 모두 송신하십시오.</br>
        /// </summary>
        /// <param name="addr">현재 시퀀스를 수신할 채널이 위치하는 보드의 번호입니다.</param>
        /// <param name="ch">현재 시퀀스를 수신할 채널의 번호입니다.</param>
        /// <returns>생성된 패킷 배열입니다.</returns>
        public SendPacket[] ToPacketArray_Old( byte addr, byte ch )
        {
            var packets = new List<SendPacket>();

            // 패킷을 변환할 때 _recipes가 아닌 복사본을 이용한다.
            // Jump-Label 때문에 리스트를 변경해야할 수도 있기 때문.
            var recipes = new List<Recipe>( _recipes.ToArray() );

            int cyclePosition = -1;
            int recipeIndex = 0;

            // Label의 위치와 LabelName을 기억해둔다.
            var labels = new List<KeyValuePair<Recipe, int>>();
            for ( var i = 0; i < recipes.Count; i++ )
            {
                if ( recipes[i] is Label )
                {
                    // 현재 Label 레시피의 위쪽에 Cycle 레시피가 존재하는지 확인한다.
                    // Jump 레시피는 Cycle의 내부로 점프할 수 없음.
                    for ( var j = i; j >= 0; j-- )
                    {
                        if ( recipes[j] is Cycle )
                        {
                            var ex = new QException( QExceptionType.SEQUENCE_INVALID_LABEL_LOCATION_ERROR );
                            recipes[i].Error = ex.Message;
                            throw ex;
                        }
                        else if ( recipes[j] is Loop )
                        {
                            break;
                        }
                    }
                    // label인 경우 label의 위치를 확인해야 한다.
                    labels.Add( new KeyValuePair<Recipe, int>( recipes[i], i ) );
                }
            }

            // Label은 다 지워버림. Label 때문에 인덱스가 한 칸씩 밀리므로
            for ( var i = 0; i < labels.Count; i++ ) recipes.Remove( labels[i].Key );

            while ( true )
            {
                if ( recipeIndex == recipes.Count ) break;

                var packet = new SendPacket( addr, ch );

                Enum cmd;
                var type = recipes[recipeIndex].GetRecipeType();

                switch ( type )
                {
                    case RecipeType.Rest:
                    case RecipeType.Cycle:
                    case RecipeType.Loop:
                    case RecipeType.Jump:
                        cmd = Commands.BatteryCycler_SetGetCommands.SequenceStep_GS;
                        break;

                    case RecipeType.Charge:
                    case RecipeType.Discharge:
                    case RecipeType.AnodeCharge:
                    case RecipeType.AnodeDischarge:
                        cmd = Commands.BatteryCycler_SetGetCommands.SequenceChargeDischarge_GS;
                        break;

                    case RecipeType.TransientResponse:
                    case RecipeType.OpenCircuitVoltage:
                    case RecipeType.DcResistance:
                    case RecipeType.AcResistance:
                    case RecipeType.FrequencyResponse:
                        cmd = Commands.BatteryCycler_SetGetCommands.SequenceMeasurement_GS;
                        break;

                    case RecipeType.Pattern:
                        cmd = Commands.BatteryCycler_SetGetCommands.SequencePattern_GS;
                        break;

                    default:
                        cmd = null;
                        break;
                }

                if ( cmd == null ) throw new QException( QExceptionType.DEVELOP_UNDEFINED_RECIPE_TYPE_ERROR );

                // Cycle을 만난 경우 해당 위치를 기억한다.
                if ( type == RecipeType.Cycle ) cyclePosition = recipeIndex;

                packet.SubPackets.Add( new SendSubPacket( cmd ) );
                packet.SubPacket.ERR = Packet.SET;

                try
                {
                    ushort endStepNo, errorStepNo;

                    endStepNo = ( ushort )( recipeIndex + 1 );
                    if ( type == RecipeType.Loop )
                    {
                        // Loop Recipe일 경우 ErrorStepNo를 가장 최근에 만난 Cycle Recipe의 Position으로 해준다.
                        // 만약 가장 최근에 만난 Cycle이 없는 경우 Cycle-Loop Not Matched 에러
                        if ( cyclePosition == -1 )
                        {
                            //recipes[recipeIndex].Error = true;
                            throw new QException( QExceptionType.SEQUENCE_CYCLE_LOOP_NOT_MATCHED_ERROR, $"Recipe[{recipeIndex}]" );
                        }

                        errorStepNo = ( ushort )cyclePosition;

                        // 해당 Cycle은 매칭된 것이므로 cyclePosition은 다시 -1로 
                        cyclePosition = -1;
                    }
                    else if ( type == RecipeType.Jump )
                    {
                        // Jump Recipe인 경우 Jump할 Label을 찾아서 Label이 있던 위치를 errorStepNo로 지정한다.
                        var jumpPosition = -1;
                        for ( var i = 0; i < labels.Count; i++ )
                        {
                            if ( ( labels[i].Key as Label ).LabelName == ( recipes[recipeIndex] as Jump ).LabelName )
                            {
                                jumpPosition = labels[i].Value;
                                break;
                            }
                        }

                        if ( jumpPosition == -1 )
                        {
                            //recipes[recipeIndex].Error = true;
                            throw new QException( QExceptionType.SEQUENCE_LABEL_NOT_FOUND_ERROR );
                        }
                        else if ( cyclePosition != -1 )
                        {
                            throw new QException( QExceptionType.SEQUENCE_INVALID_JUMP_LOCATION_ERROR );
                        }

                        // Jump의 경우
                        // EndStepNo = 점프할 위치
                        // ErrorStepNo = 점프 횟수 도달 후 이동할 위치(즉, 다음 레시피)
                        endStepNo = ( ushort )jumpPosition;
                        errorStepNo = ( ushort )( recipeIndex + 1 );
                    }
                    else
                    {
                        errorStepNo = ( ushort )recipes.Count;
                    }

                    packet.SubPacket.DATA.Add( ( recipes[recipeIndex] as IPacketConvertable ).ToDataField( ( ushort )recipeIndex, endStepNo, errorStepNo ) );

                    packets.Add( packet );

                    // 패턴 레시피인 경우
                    // 패턴 정보를 담는 패킷을 추가적으로 생성하여 넣는다.
                    if ( type == RecipeType.Pattern )
                    {
                        var patternData = ( recipes[recipeIndex] as Pattern ).PatternData.ToDataField( ( ushort )recipeIndex );

                        for ( var i = 0; i < patternData.Count; i++ )
                        {
                            var patternDataPacket = new SendPacket( addr, ch );
                            patternDataPacket.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.SequencePatternData_GS ) );
                            patternDataPacket.SubPackets[0].ERR = Packet.SET;

                            patternDataPacket.SubPackets[0].DATA.Add( patternData[i] );

                            packets.Add( patternDataPacket );
                        }
                    }

                    recipeIndex++;
                }
                catch ( QException ex )
                {
                    recipes[recipeIndex].Error = ex.Message;
                    throw ex;
                }
            }

            // End Recipe
            var endPacket = new SendPacket( addr, ch );
            endPacket.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.SequenceStep_GS ) );
            endPacket.SubPacket.ERR = Packet.SET;
            endPacket.SubPacket.DATA.Add( new End().ToDataField( ( ushort )recipeIndex, ( ushort )recipeIndex, ( ushort )recipeIndex ) );
            packets.Add( endPacket );

            return packets.ToArray();
        }

        /// <summary>
        /// 현재 시퀀스의 전체 스텝 수를 계산하여 반환합니다.
        /// <br>Cycle, Loop, Jump 및 End 레시피도 전체 스텝 수에 포함됩니다.</br>
        /// </summary>
        /// <returns>
        /// 1보다 큰 정수 : 계산된 전체 스텝 수
        /// <br>1 : 시퀀스가 빈 경우 (End 레시피만 세어진 경우)</br>
        /// <br>-1 : Cycle-Loop 짝이 맞지 않는 경우</br>
        /// <br>-2 : Jump-Label 짝이 맞지 않는 경우</br>
        /// </returns>
        public int GetTotalSteps()
        {
            int count = 0;

            int lastCycleNo = -1;
            var countStack = new List<int>();

            for(var i = 0; i < _recipes.Count; i++ )
            {
                switch( _recipes[i].GetRecipeType() )
                {
                    case RecipeType.Cycle:
                        lastCycleNo = i;
                        count++;
                        countStack.Add( count );
                        break;

                    case RecipeType.Loop:
                        count++;
                        if(lastCycleNo != -1 )
                        {
                            count += (i - lastCycleNo + 1) * ( int )((_recipes[i] as Loop).LoopCount - 1);
                            lastCycleNo = -1;
                        }
                        else
                        {
                            // 이 경우는 Loop와 짝이 되는 Cycle이 없는 경우이다.
                            return -1;
                        }
                        countStack.Add( count );
                        break;

                    case RecipeType.Label:
                        countStack.Add( count );
                        // do nothing
                        break;

                    case RecipeType.Jump:
                        var labelIndex = -1;
                        var jump = _recipes[i] as Jump;
                        for(var j = 0; j < _recipes.Count; j++ )
                        {
                            if(_recipes[j] is Label label && (label.LabelName == jump.LabelName) )
                            {
                                labelIndex = j;
                                break;
                            }
                        }
                        if( labelIndex != -1)
                        {
                            if( labelIndex < i )
                            {
                                // CountStack에서 Jump 바로 전까지의 Count에서 Label 바로 전까지의 Count를 뺀 값이 Jump와 Label 사이 구간의 스텝수다.
                                count += ( int )( ( countStack[i - 1] - countStack[labelIndex] ) * (jump.JumpCount - 1 ) + jump.JumpCount);
                                countStack.Add( count );
                            }
                            else
                            {
                                // 이 경우는 Jump 목적지가 Jump 레시피보다 더 뒤에 있는 (시퀀스를 아무 생각 없이 작성한)경우로,
                                // Jump로 되돌아올 일이 없으니 Jump Count는 의미가 없다.
                                // Jump 레시피에 해당하는 Step만 센 후 i를 labelIndex로 강제로 바꾼다.
                                count++;
                                countStack.Add( count );
                                i = labelIndex;
                            }
                        }
                        else
                        {
                            return -2;
                        }
                        break;

                    case RecipeType.CdCycle:
                        // C-D Cycle 레시피의 경우 시퀀스를 장비로 전달할 때 R-C-R-D-R와 같은 형태로 풀어서 전달하므로,
                        // 여기서도 풀어서 계산할 필요가 있다.
                        var cyc = _recipes[i] as CdCycle;
                        if( cyc.FrontRest ) count++;
                        if( cyc.MiddleRest ) count++;
                        if( cyc.BackRest ) count++;
                        count += 2;
                        countStack.Add( count );
                        break;

                    default:
                        count++;
                        countStack.Add( count );
                        break;
                }
            }

            return count + 1;
        }

        public object Clone()
        {
            var seq = new Sequence();

            for(var i = 0; i < _recipes.Count; i++ )
            {
                seq.Add( _recipes[i].Clone() as Recipe );
            }

            return seq;
        }
    }
}
