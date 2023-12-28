using McQLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McQLib.OldSupport
{
    public static class OldPattern
    {
        /// <summary>
        /// 구 버전 Q730 소프트웨어에서 생성한 패턴 데이터 파일을 신규 Q730 패턴 형식 파일로 변환합니다.
        /// </summary>
        /// <param name="oldFile">변환할 구 버전 패턴 데이터 파일의 전체 경로입니다.</param>
        /// <param name="newFile">변환된 패턴 데이터를 저장할 파일의 전체 경로입니다.</param>
        /// <returns>변환에 성공한 경우 true이고, 그렇지 않은 경우 false입니다.</returns>
        [Obsolete( "이 메서드는 아직 구현되지 않았습니다.", true )]
        public static bool TryConvert( string oldFile, string newFile )
        {
            throw new NotImplementedException();

            using( var sr = new StreamReader( oldFile ) )
            {
                bool ccMode = true;

                var list = new List<KeyValuePair<double, double>>();

                string line;
                var minWidth = double.MaxValue;
                while( (line = sr.ReadLine()) != null )
                {
                    var split = line.Split( ",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
                    if( split.Length != 4 ) return false;

                    if( !double.TryParse( split[2], out double value ) ) throw new QException( QExceptionType.PATTERN_INVALID_PULSE_ITEM_ERROR );
                    if( !double.TryParse( split[3], out double width ) ) throw new QException( QExceptionType.PATTERN_INVALID_PULSE_ITEM_ERROR );

                    if( width < minWidth ) minWidth = width;

                    list.Add( new KeyValuePair<double, double>( value, width ) );
                }

                // minWidth 설정
                if( minWidth > 100 ) minWidth = 100;
                else minWidth = 10;


            }
        }
    }
}
