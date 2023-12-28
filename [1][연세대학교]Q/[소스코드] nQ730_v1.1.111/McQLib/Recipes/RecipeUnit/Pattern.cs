using McQLib.Core;
using System;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel;
using System.IO;

namespace McQLib.Recipes
{
    /// <summary>
    /// Pattern 레시피입니다.
    /// </summary>
    public sealed class Pattern : BaseConvertableRecipe
    {
        public override string GetManualString()
        {
            return "사용자가 임의로 전류 패턴을 제작해 인가하여 전압 반응을 측정할 수 있습니다.\r\n" +
                   "패턴을 제작하기 위해 Pattern Editor를 제공하며, Pattern Editor에서 패턴을 저장하거나 불러오고 편집할 수도 있습니다.";
        }

        public override string GetSummaryString()
        {
            var str = string.Empty;
            
            str += $"Pattern File Name : {_patternInfo.FileName}";

            return str;
        }

        public override string GetDetailString()
        {
            var str = _title;
            
            str += $"Pattern File Name : {_patternInfo.FileName}\r\n";

            str += $"\r\n{base.GetDetailString()}";

            return str;
        }

        public sealed override System.Drawing.Image Icon => Properties.Resources.Icon_Pattern;

        /// <summary>
        /// 레시피 정보를 패킷의 DATA Field 형태로 변환합니다.
        /// </summary>
        /// <param name="stepNo">현재 레시피의 시퀀스상 순번입니다.</param>
        /// <param name="endStepNo">현재 레시피가 종료된 후 이동할 목표 레시피의 시퀀스상 순번입니다.</param>
        /// <param name="errorStepNo">현재 레시피가 비정상 종료된 후 이동할 목표 레시피의 시퀀스상 순번입니다.</param>
        /// <returns>변환된 DATA Field 형태의 byte형식 배열입니다.</returns>
        public override byte[] ToDataField( ushort stepNo, ushort endStepNo, ushort errorStepNo )
        {
            if( _patternInfo.FilePath == null || _patternInfo.FilePath.Length == 0 ) throw new QException( QExceptionType.PATTERN_INVALID_FILE_NAME_ERROR );

            PatternData pattern;
            try
            {
                pattern = PatternData.FromFile( _patternInfo.FilePath );
            }
            catch( QException ex )
            {
                throw ex;
            }

            var builder = new DataBuilder();

            // Reserved (2Byte) - Reserved
            builder.AddCount( 0, 2 );

            // Step count (4Byte) - Reserved
            builder.AddCount( 0, 4 );

            // Step no (2Byte)
            builder.Add( new Q_UInt16( stepNo ) );

            // Cycle no (4Byte) - Reserved
            builder.AddCount( 0, 4 );

            // Mode1 (1Byte) - Pattern : 4
            builder.Add( Mode1.Pattern );

            // Mode2 (1Byte) - Set : 0
            builder.Add( Mode2.Set );

            // [설정 조건 - 사용 여부] (2Byte)
            byte high = 0, low = 0;
            low |= 0b10000000; // 설정 mode (사용)
            low |= 0b01000000; // 설정 time resolution (사용)
            low |= 0b00100000; // 설정 총 데이터 개수 (사용)

            builder.Add( high, low );

            // [설정 조건 - 값] (4Byte)
            builder.Add( pattern.BiasMode );                      // 설정 mode
            builder.Add( ( byte )(pattern.PulseWidth / 100) );    // 설정 time resolution (10 / 100 = 0, 100 / 100 = 1 임)
            builder.Add( new Q_UInt16( pattern.TotalCount ) );    // 설정 총 패턴 데이터 개수

            // Reserved (60Byte) - Reserved
            builder.AddCount( 0, 60 );

            // [안전 조건(O), 종료 조건(X), 기록 조건(X)] (164Byte)
            builder.Add( base.ToDataField( stepNo, endStepNo, errorStepNo ) );

            // 종료값 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( endStepNo ) );

            // 비정상 종료 시 이동할 step no (2Byte)
            builder.Add( new Q_UInt16( errorStepNo ) );

            // 반복횟수 (4Byte) - Loop, Jump만 해당
            builder.AddCount( 0, 4 );

            // 스텝 진행 (1Byte) - 사용 안 함
            builder.Add( 0 );

            // Reserved (7Byte) - Reserved
            builder.AddCount( 0, 7 );

            PatternData = pattern;

            return builder;
        }

        internal Pattern() { }

        public override object Clone()
        {
            var clone = new Pattern();

            clone._patternInfo = _patternInfo;
            clone.PatternData = PatternData.Clone() as PatternData;

            clone._safety = _safety.Clone() as SafetyCondition;

            return clone;
        }

        #region Properties
        [Category( "\tParameter" )]
        [DisplayName( "Pattern Name" )]
        [Description( "Pattern 레시피에 사용할 패턴 파일입니다." )]
        [Editor( typeof( PatternEditor ), typeof( UITypeEditor ) )]
        // PropertyGrid에 표시하기 위한 용도 (실제 값은 Path로, GUI에는 FileName만)
        public PatternInfo PatternInfo
        {
            get => _patternInfo;
            set => _patternInfo = value;
        }
        private PatternInfo _patternInfo;

        [Browsable(false)]
        [ID("040000")]
        // Sequence를 파일로 저장할 때 전체 경로를 저장하기 위한 용도 (GUI에는 보이지 않음)
        public string PatternPath
        {
            get => _patternInfo.FilePath;
            set => _patternInfo.FilePath = value;
        }

        // Pattern 레시피를 보낼 때 PatternData로 정상적으로 읽어지는지 확인 후 PatternData에 저장하기 위한 용도
        internal PatternData PatternData = null;

        [Browsable( false )]
        public override EndCondition EndCondition => null;
        [Browsable( false )]
        public override SaveCondition SaveCondition => null;
        #endregion
    }


    public struct PatternInfo
    {
        public string FileName => FilePath == null ? null : new FileInfo( FilePath ).Name;
        public string FilePath;

        public override string ToString() => FileName;
    }
    public class PatternEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override bool GetPaintValueSupported( ITypeDescriptorContext context )
        {
            return false;
        }
        public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
        {
            using( var form = new GUI.Form_PatternEditor( (( PatternInfo )value).FilePath ) )
            {
                if( form.ShowDialog() == DialogResult.OK )
                {
                    return new PatternInfo() { FilePath = form.LastLoaded };
                }
                else
                {
                    return value;
                }
            }
        }
        public override void PaintValue( PaintValueEventArgs e )
        {
            base.PaintValue( e );
        }
    }
}
