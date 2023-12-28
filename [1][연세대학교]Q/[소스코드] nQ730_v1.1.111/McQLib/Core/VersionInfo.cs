using System;

namespace McQLib
{
    /// <summary>
    /// McQLib에서 사용되는 버전 정보를 정의하는 구조체입니다.
    /// </summary>
    public struct VersionInfo
    {
        /// <summary>
        /// Major 버전을 나타냅니다.
        /// <br>Major 버전은 하위 버전과 더이상 호환되지 않는 변화가 발생했을 때만 증가시킵니다.</br>
        /// </summary>
        public readonly int Major;
        /// <summary>
        /// Minor 버전을 나타냅니다.
        /// <br>Minor 버전은 기존 기능이 변경되거나, 새로운 기능이 추가될 때 증가시킵니다.</br>
        /// <br>Minor 버전이 증가해도 하위 버전과 호환이 가능해야 합니다.</br>
        /// </summary>
        public readonly int Minor;
        /// <summary>
        /// Patch 버전을 나타냅니다.
        /// <br>Patch 버전은 버그가 수정되었을 때 증가시킵니다.</br>
        /// <br>Patch 버전이 증가해도 하위 버전과 호환이 가능해야 합니다.</br>
        /// </summary>
        public readonly int Patch;
        /// <summary>
        /// Patch 버전의 뒤에 추가로 붙는 문자 또는 문자열입니다.
        /// <br>일반적으로 1.0.3b 와 같은 형식의 버전 정보를 나타낼 때 사용합니다.</br>
        /// </summary>
        public readonly string Additional;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="patch"></param>
        /// <param name="additional"></param>
        public VersionInfo( int major, int minor, int patch = 0, string additional = "" )
        {
            Major = major;
            Minor = minor;
            Patch = patch;

            if( additional == null ) Additional = "";
            else Additional = additional;
        }

        /// <summary>
        /// VersionInfo 정보를 Major.Minor.Patch + Additional 형식의 문자열로 변환합니다.
        /// <br>Patch가 0인 경우 Patch와 Additional은 문자열에 포함되지 않습니다.</br>
        /// </summary>
        /// <returns>변환된 문자열입니다.</returns>
        public override string ToString()
        {
            if( Patch == 0 && Additional.Length != 0)
            {
                return $"v{Major}.{Minor}{Additional}";
            }
            else if(Patch == 0 && Additional.Length == 0)
            {
                return $"v{Major}.{Minor}";
            }
            else
            {
                return $"v{Major}.{Minor}.{Patch}{Additional}";
            }
        }

        /// <summary>
        /// 지정된 VersionInfo의 버전이 호환 가능한 버전인지의 여부를 확인합니다.
        /// </summary>
        /// <param name="version">비교할 버전입니다.</param>
        /// <returns>두 버전이 호환 가능한 경우(Major 버전이 동일한 경우) true이고, 그렇지 않은 경우 false입니다.</returns>
        public bool CompatibleWith( VersionInfo version ) => Major == version.Major && 
                                                             Minor == version.Minor && 
                                                             Patch == version.Patch && 
                                                             Additional == version.Additional;
    }
}
