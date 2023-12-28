using McQLib.Core;
using McQLib.NotUsed.Core;
using McQLib.Device;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace McQLib.NotUsed.Recipes
{
    [Serializable]
    public sealed class Sequence
    {
        public const ushort MAX_STORAGE = 99;

        private enum ActionType { Add, Insert, Remove, RemoveAt, Clear, Name, Comment }
        /// <summary>
        /// Sequence의 변경이 발생했을 때 변경 내역을 기억하기 위한 클래스
        /// </summary>
        private class EditingAction
        {
            /// <summary>
            /// 변경 액션
            /// </summary>
            public ActionType ActionType;
            /// <summary>
            /// 변경에 참여된 주체
            /// </summary>
            public IRecipe Object;
            /// <summary>
            /// 인덱스(필요에 따라)
            /// </summary>
            public int Index;

            /// <summary>
            /// 액션과 주체를 사용하여 EditingAction 클래스의 새 인스턴스를 생성합니다.
            /// </summary>
            /// <param name="action">발생한 액션입니다.</param>
            /// <param name="obj">액션에 참여된 주체(Recipe)입니다.</param>
            public EditingAction( ActionType action, IRecipe obj )
            {
                ActionType = action;
                Object = obj;
                Index = -1;
            }
            /// <summary>
            /// 액션, 주체 및 인덱스를 사용하여 EditingAction 클래스의 새 인스턴스를 생성합니다.
            /// </summary>
            /// <param name="action">발생한 액션입니다.</param>
            /// <param name="obj">액션에 참여된 주체(Recipe)입니다.</param>
            /// <param name="index">액션에 사용된 인덱스입니다.</param>
            public EditingAction( ActionType action, IRecipe obj, int index )
            {
                ActionType = action;
                Object = obj;
                Index = index;
            }
        }

        private List<IRecipe> _recipes;
        private bool _isChanged;
        // default path
        private string _path;
        private string _comment;

        //[NonSerialized]
        //private readonly List<EditAction> _actionStack;

        #region Properties
        public static string DefaultDirectory => Path.Combine( Assembly.GetEntryAssembly().Location.Replace( Assembly.GetEntryAssembly().ManifestModule.Name, "" ), "Sequence" );
        public int Count => _recipes.Count;
        public IRecipe this[int index] => _recipes[index];

        public void Add( IRecipe recipe )
        {
            if( _recipes.Count == MAX_STORAGE ) throw new QException( QExceptionType.SEQUENCE_ALREADY_FULL_ERROR );

            //_actionStack.Add( new EditAction( ActionType.Add, recipe ) );
            _recipes.Add( recipe );
            _isChanged = true;
        }
        public void Insert( int index, IRecipe recipe )
        {
            //_actionStack.Add( new EditAction( ActionType.Insert, recipe, index ) );
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
        public void Remove( IRecipe recipe )
        {
            //_actionStack.Add( new EditAction( ActionType.Remove, recipe, _recipes.IndexOf( recipe ) ) );
            _recipes.Remove( recipe );

            _isChanged = true;
        }
        public void RemoveAt( int index )
        {
            //_actionStack.Add( new EditAction( ActionType.RemoveAt, _recipes[index], index ) );
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
            //_actionStack.Add( new EditAction( ActionType.Clear, new List<IRecipe>( _recipes ) ) );
            _recipes.Clear();
            _isChanged = true;
        }
        //public void Undo()
        //{
        //    if( _actionStack.Count == 0 ) return;

        //    var action = _actionStack[_actionStack.Count - 1];
        //    _actionStack.Remove( action );

        //    switch( action.ActionType )
        //    {
        //        case ActionType.Add:
        //            _recipes.Remove( action.Object as IRecipe );
        //            break;

        //        case ActionType.Insert:
        //            _recipes.Remove( action.Object as IRecipe );
        //            break;

        //        case ActionType.Remove:
        //            _recipes.Insert( action.Index, action.Object as IRecipe );
        //            break;

        //        case ActionType.RemoveAt:
        //            _recipes.Insert( action.Index, action.Object as IRecipe );
        //            break;

        //        case ActionType.Clear:
        //            _recipes = new List<IRecipe>( action.Object as List<IRecipe> );
        //            break;

        //        case ActionType.Name:
        //            _name = action.Object as string;
        //            break;

        //        case ActionType.Comment:
        //            _comment = action.Object as string;
        //            break;
        //    }
        //}
        /// <summary>
        /// 시퀀스가 생성된 날짜 및 시간입니다.
        /// <br>시퀀스가 신규 파일로 생성 및 저장되는 시점의 시간이 저장됩니다.</br>
        /// </summary>
        public DateTime CreateDateTime => _createDateTime;
        /// <summary>
        /// 시퀀스가 마지막으로 수정된 날짜 및 시간입니다.
        /// <br>시퀀스가 파일로 저장되는 시점의 시간이 저장됩니다.</br>
        /// </summary>
        public DateTime LastModifiedDateTime => _lastModifiedDateTime;

        private DateTime _createDateTime;
        private DateTime _lastModifiedDateTime;

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
            _recipes = new List<IRecipe>();
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
                var seq = load( filename );
                seq._isChanged = false;
                return seq;
            }
            catch
            {
                return null;
            }
            //return deserialize( filename );
        }

        public void Save()
        {
            if( Name == null || Name == "" ) throw new QException( QExceptionType.SEQUENCE_NAME_EMPTY_ERROR );
            //serialize( _path );

            save( _path );
            _isChanged = false;
        }

        private void save( string filename )
        {
            if( !new FileInfo( _path ).Exists )
            {
                _createDateTime = DateTime.Now;
            }
            _lastModifiedDateTime = DateTime.Now;

            using( var sw = new StreamWriter( filename ) )
            {
                sw.WriteLine( "== 이 파일의 내용을 절대로 직접 수정하지 마십시오. ==" );
                // 코멘트
                sw.WriteLine( $"{Comment}" );

                for( var i = 0; i < _recipes.Count; i++ )
                {
                    var recipeInfo = _recipes[i].GetRecipeInfo();

                    // 레시피 정보
                    sw.WriteLine( $"{( byte )recipeInfo.RecipeType:X2}" );

                    for( var j = 0; j < recipeInfo.GroupCount; j++ )
                    {
                        for( var k = 0; k < recipeInfo[j].ParameterCount; k++ )
                        {
                            //  파라미터[k]에 대해
                            //  파라미터 번호 : 값
                            var value = recipeInfo[j][k].GetValue( _recipes[i] );
                            sw.WriteLine( $"{recipeInfo[j][k].Code}:{recipeInfo[j][k].Name}:{(value == null ? "null" : value)}" );
                        }
                    }
                }

                // 종료
            }
        }
        private static Sequence load( string filename )
        {
            var seq = new Sequence();

            using( var sr = new StreamReader( filename ) )
            {
                string line;

                seq._path = filename;

                line = sr.ReadLine();  // 경고문 제거
                if( line == "[SequenceInfomation]" )
                {
                    //sr.Close();
                    try
                    {
                        return null;
                        //return OldSupport.OldSequence.FromFile( filename );
                    }
                    catch
                    {
                        // 구 버전 시퀀스 파일 읽기에 실패한 경우
                        // 원래 여기서 null을 반환하는게 맞긴 하나, 정말 만에 하나라도 누군가가 의도적으로 파일의 첫 번째 줄에
                        // [SequenceInfomation]을 적은 경우를 대비하여 냅둔다.
                    }
                }

                seq._comment = sr.ReadLine();

                RecipeInfo recipeInfo = null;

                while( (line = sr.ReadLine()) != null )
                {
                    var split = line.Split( ':' );
                    if( split.Length == 1 )
                    {
                        // 레시피 형식을 읽음
                        seq.Add( RecipeFactory.CreateInstance( ( RecipeType )Convert.ToInt32( split[0], 16 ) ) );
                        recipeInfo = seq[seq.Count - 1].GetRecipeInfo();

                        continue;
                    }
                    else if( split.Length == 3 )
                    {
                        // 파라미터를 읽음

                        // 파라미터를 읽었는데 RecipeInfo가 null인 경우 = 최초에 레시피 형식 없이 파라미터가 먼저 나온 경우 (정상적으로 저장되었다면 있을 수 없는 경우임)
                        if( recipeInfo == null ) continue;

                        var param = recipeInfo.Find( split[0] );

                        // 해당하는 코드를 가지는 파라미터가 존재하지 않는 경우 (파라미터 정보가 바뀐 경우)
                        if( param == null ) continue;

                        object value = null;
                        try
                        {
                            switch( param.ParameterValueType )
                            {
                                case ParameterValueType.Boolean:
                                    value = bool.Parse( split[2] );
                                    break;

                                case ParameterValueType.Double:
                                case ParameterValueType.Time:
                                    value = double.Parse( split[2] );
                                    break;

                                case ParameterValueType.Enum:
                                    value = Enum.Parse( param.FieldInfo.FieldType, split[2] );
                                    break;

                                case ParameterValueType.Float:
                                    value = float.Parse( split[2] );
                                    break;

                                case ParameterValueType.Integer:
                                    value = uint.Parse( split[2] );
                                    break;

                                case ParameterValueType.String:
                                case ParameterValueType.Pattern:
                                    value = split[2];
                                    break;
                            }
                        }
                        catch( Exception )
                        {   // 데이터 변환에 실패한 경우
                            continue;
                            throw new QException( QExceptionType.IO_WRONG_DATA_FORMAT_ERROR );
                        }

                        try
                        {
                            param.SetValue( seq[seq.Count - 1], value );
                        }
                        catch( ArgumentException )
                        {
                            var obj = value;

                            if( obj == null ) param.SetValue( seq[seq.Count - 1], null );
                            else param.SetValue( seq[seq.Count - 1], Convert.ToUInt32( obj ) );
                        }
                    }
                    else if( split.Length == 4 && recipeInfo.RecipeType == RecipeType.Pattern )
                    {   // Pattern 레시피의 경우 패턴 데이터의 경로에 드라이브명이 C:\ 와 같이 :가 들어가므로 한 단계 더 스플릿되어버린다.
                        // 따라서 패턴 레시피인 경우 별도로 처리해줌
                        var param = recipeInfo.Find( split[0] );

                        param.SetValue( seq[seq.Count - 1], split[2] + ":" + split[3] );
                    }
                    else
                    {
                        // 파라미터 저장 형식 (Code:Name:Value)이 올바르지 않은 경우
                        return null;
                        //continue;
                    }
                }
            }

            return seq;
        }

        [Obsolete( "Use 'void save(string filename)'", true )]
        private void serialize( string filename )
        {
            var formatter = new BinaryFormatter();
            using( var fs = new FileStream( filename, FileMode.OpenOrCreate, FileAccess.Write ) )
            {
                formatter.Serialize( fs, this );
            }
        }
        [Obsolete( "Use 'Sequence load(string filename)'", true )]
        private static Sequence deserialize( string filename )
        {
            var formatter = new BinaryFormatter();
            using( var fs = new FileStream( filename, FileMode.Open, FileAccess.Read ) )
            {
                var obj = formatter.Deserialize( fs ) as Sequence;
                if( obj == null ) throw new QException( QExceptionType.UNDEFINED_ERROR );

                return obj;
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

            int cyclePosition = -1;
            int recipeIndex = 0;

            while( true )
            {
                if( recipeIndex == _recipes.Count ) break;

                var packet = new SendPacket( addr, ch );

                Enum cmd;
                var type = _recipes[recipeIndex].GetRecipeType();

                switch( type )
                {
                    case RecipeType.Rest:
                    case RecipeType.Cycle:
                    case RecipeType.Loop:
                        cmd = Commands.BatteryCycler_SetGetCommands.SequenceStep_GS;
                        break;

                    case RecipeType.Charge:
                    case RecipeType.Discharge:
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

                if( cmd == null ) throw new QException( QExceptionType.DEVELOP_UNDEFINED_RECIPE_TYPE_ERROR );

                // Cycle을 만난 경우 해당 위치를 기억한다.
                if( type == RecipeType.Cycle ) cyclePosition = recipeIndex;

                packet.SubPackets.Add( new SendSubPacket( cmd ) );
                packet.SubPacket.ERR = 1;

                try
                {
                    if( type == RecipeType.Loop )
                    {
                        // Loop Recipe일 경우 ErrorStepNo를 가장 최근에 만난 Cycle Recipe의 Position으로 해준다.
                        // 만약 가장 최근에 만난 Cycle이 없는 경우 Cycle-Loop Not Matched 에러
                        if( cyclePosition == -1 ) throw new QException( QExceptionType.SEQUENCE_CYCLE_LOOP_NOT_MATCHED_ERROR, $"Recipe[{recipeIndex}]" );

                        packet.SubPacket.DATA.Add( _recipes[recipeIndex].ToCommand( ( ushort )recipeIndex, ( ushort )(recipeIndex + 1), ( ushort )cyclePosition ) );

                        // 해당 Cycle은 매칭된 것이므로 cyclePosition은 다시 -1로 
                        cyclePosition = -1;
                    }
                    else
                    {
                        packet.SubPacket.DATA.Add( _recipes[recipeIndex].ToCommand( ( ushort )recipeIndex, ( ushort )(recipeIndex + 1), ( ushort )_recipes.Count ) );
                    }

                    packets.Add( packet );

                    // 패턴 레시피인 경우
                    // 패턴 정보를 담는 패킷을 추가적으로 생성하여 넣는다.
                    if( type == RecipeType.Pattern )
                    {
                        var patternData = (_recipes[recipeIndex] as Pattern).PatternData.ToDataField( ( ushort )recipeIndex );

                        for( var i = 0; i < patternData.Count; i++ )
                        {
                            var patternDataPacket = new SendPacket( addr, ch );
                            patternDataPacket.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.SequencePatternData_GS ) );
                            patternDataPacket.SubPackets[0].ERR = 1;

                            patternDataPacket.SubPackets[0].DATA.Add( patternData[i] );

                            packets.Add( patternDataPacket );
                        }
                    }

                    recipeIndex++;
                }
                catch( QException ex )
                {
                    throw ex;
                }
            }

            // End Recipe
            var endPacket = new SendPacket( addr, ch );
            endPacket.SubPackets.Add( new SendSubPacket( Commands.BatteryCycler_SetGetCommands.SequenceStep_GS ) );
            endPacket.SubPacket.ERR = 1;
            endPacket.SubPacket.DATA.Add( new End().ToCommand( ( ushort )recipeIndex, ( ushort )recipeIndex, ( ushort )recipeIndex ) );
            packets.Add( endPacket );

            return packets.ToArray();
        }
    }
}
