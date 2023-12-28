using McQLib.Core;

namespace McQLib.Recipes
{
    /// <summary>
    /// 패킷으로 변환이 필요한 모든 레시피 클래스의 안전 조건, 종료 조건 및 기록 조건에 대한 패킷 변환 메서드를 구현하는 차상위 클래스입니다.
    /// </summary>
    public abstract class BaseConvertableRecipe : Recipe, IPacketConvertable
    {
        /// <summary>
        /// 레시피 정보를 장비로 송신하기 위한 DATA Field로 변환합니다.
        /// <br>기본적으로 안전 조건, 종료 조건 및 기록 조건에 대한 정보를 DATA Field 형태로 구성하도록 구현되어 있습니다.</br>
        /// <br>안전 조건 58Byte, 종료 조건에 대한 사용 여부 2Byte, 각 종료 조건의 설정값 78Byte, 기록 조건에 대한 사용 여부 2Byte, 각 기록 조건의 설정값 24Byte로 총 164Byte입니다.</br>
        /// </summary>
        /// <param name="stepNo_notUsed">사용되지 않는 매개변수입니다.</param>
        /// <param name="endStepNo_notUsed">사용되지 않는 매개변수입니다.</param>
        /// <param name="errorStepNo_notUsed">사용되지 않는 매개변수입니다.</param>
        /// <returns>안전 조건, 종료 조건, 기록 조건에 대한 DATA Field입니다.</returns>
        public virtual byte[] ToDataField( ushort stepNo_notUsed, ushort endStepNo_notUsed, ushort errorStepNo_notUsed )
        {
            var builder = new DataBuilder();

            if ( SafetyCondition != null ) builder.Add( SafetyCondition.ToDataField() );
            else builder.AddCount( 0, 58 );

            if ( EndCondition != null ) builder.Add( EndCondition.ToDataField() );
            else builder.AddCount( 0, 80 );

            if ( SaveCondition != null ) builder.Add( SaveCondition.ToDataField() );
            else builder.AddCount( 0, 26 );

            return builder;
        }

        /// <summary>
        /// DATA Field로부터 레시피 정보를 추출합니다.
        /// <br>기본적으로 안전 조건, 종료 조건 및 기록 조건에 대한 정보를 DATA Field로부터 읽어와 구성하도록 구현되어 있습니다.</br>
        /// <br>안전 조건에 대한 사용 여부 2Byte, 각 안전 조건의 설정값 58Byte, 종료 조건에 대한 사용 여부 2Byte, 각 종료 조건의 설정값 78Byte, 기록 조건에 대한 사용 여부 2Byte, 각 기록 조건의 설정값 24Byte로 총 164Byte를 사용하여 구성합니다.</br>
        /// </summary>
        /// <returns> DATA Field로부터 안전 조건, 종료 조건, 기록 조건을 성공적으로 추출했는지의 여부입니다.</returns>
        public virtual bool FromDataField( byte[] data )
        {
            if ( SafetyCondition != null && !SafetyCondition.FromDataField( data ) ) return false;
            if ( EndCondition != null && !EndCondition.FromDataField( data ) ) return false;
            if ( SaveCondition != null && !SaveCondition.FromDataField( data ) ) return false;

            return true;
        }
    }
}
