using System;

namespace McQLib.Core
{
    public enum QExceptionType : ushort
    {
        // 통신 오류 (0x00) - 기본 통신 수단(Socket)에서 오류가 발생한 경우
        COMMUNICATION_TIMEOUT_ERROR = 0x0000,
        COMMUNICATION_SOCKET_CLOSED_ERROR,
        COMMUNICATION_PING_ACK_NOT_RECEIVED_ERROR,
        COMMUNICATION_PORT_SOCKET_WRONG_MESSAGE_ERROR,
        COMMUNICATION_INVALID_IP_FORMAT_ERROR,
        COMMUNICATION_COMMANDING_FAILED_ERROR,
        COMMUNICATION_CANNOT_READ_BOARDINFO_ERROR,
        COMMUNICATION_FIRMWARE_VERSION_NOT_COMPATIBLE_ERROR,
        COMMUNICATION_NOT_COMPATIBLE_DEVICE_ERROR,

        // 패킷 오류 (0x01) - 패킷이 올바르지 않은 경우
        PACKET_CRC_ERROR = 0x0100,
        PACKET_CRC2_ERROR,
        PACKET_REJECT_WRITING_ERROR,
        PACKET_SUBPACKET_INDEX_OUT_OF_RANGE_ERROR,
        PACKET_DATA_INDEX_OUT_OF_RANGE_ERROR,
        PACKET_DATA2_INDEX_OUT_OF_RANGE_ERROR,
        PACKET_WRONG_DESTINATION_ERROR,
        PACKET_CHANNEL_DATA_DATAFIELD_LENGTH_ERROR,
        PACKET_BOARD_INFORMATION_DATAFIELD_LENGTH_ERROR,

        // 명령 오류 (0x02) - 코드 처리 중 오류가 발생한 경우 (일반적으로 런타임에서 발생해서는 안 됨)
        DEVELOP_WRONG_COMMAND_QUERY_USAGE_ERROR = 0x0200,
        DEVELOP_WRONG_ADDRESS_USAGE_ERROR,
        DEVELOP_UNDEFINED_COMMAND_ERROR,
        DEVELOP_UNDEFINED_ENUMERATION_ERROR,
        DEVELOP_UNEXPECTED_ERROR_FIELD_ERROR,
        DEVELOP_NULL_REFERENCE_ERROR,
        DEVELOP_RECIPE_DATAFIELD_CONVERTING_DENIED_ERROR,

        // 동작 오류 (0x03) - 송수신 동작을 할 수 없거나, 또는 동작을 수행 중 오류가 발생한 경우
        ACTION_DEVICE_NOT_CONNECTED_ERROR = 0x0300,
        ACTION_CHANNEL_INDEX_OUT_OF_RANGE_ERROR,
        ACTION_TOO_MANY_CHANNEL_COUNT_ERROR,
        ACTION_TOO_LAW_CHANNEL_COUNT_ERROR,
        ACTION_DEVICE_NOT_RESPONSE_ERROR,
        ACTION_FILE_WRITING_ERROR,
        ACTION_CHANNEL_NOT_CONNECTED_ERROR,
        ACTION_SEQUENCE_SENDING_ERROR,
        ACTION_CHANNEL_ALREADY_RUN_ERROR,

        // 응답 오류 (0x04) - 수신 패킷의 ERROR 필드에 해당하는 오류
        DEVICE_UNDEFINED_ERROR_FIELD_ERROR = 0x0400,    // ERROR 필드에 정의되지 않은 값
        DEVICE_ACTION_ERROR = 0x0401,
        DEVICE_INITIALIZE_ERROR,
        DEVICE_PACKET_ERROR,
        DEVICE_COMMAND_ERROR,
        DEVICE_BUSY_ERROR,
        DEVICE_SET_ERROR,
        DEVICE_SD_CARD_ERROR,
        DEVICE_ADDRESS_ERROR = 0x04F0,
        DEVICE_UNDEFINED_ERROR = 0x04FF,

        // 입출력 오류 (0x05) - 파일 입출력 과정에서 기본적인 오류가 발생한 경우
        IO_FILE_NOT_FOUND_ERROR = 0x0500,
        IO_FILE_ALREADY_USING_ERROR,
        IO_FILE_ACCESS_DENIED_ERROR,
        IO_WRONG_DATA_FORMAT_ERROR,
        IO_INVALID_FILE_NAME_ERROR,

        IO_DECRYPT_FAILED_ERROR,
        IO_ENCRYPT_FAILED_ERROR,

        // 시퀀스, 레시피 유저 컨트롤 오류 (0x0A) - 코드 처리 중 오류가 발생한 경우 (일반적으로 런타임에서 발생해서는 안 됨)
        DEVELOP_GROUP_ATTRIBUTE_NOT_DEFINED_ERROR = 0x0A00,
        DEVELOP_PARAMETER_ATTRIBUTE_NOT_DEFINED_ERROR,
        DEVELOP_FIELD_TYPE_NOT_DEFINED_ERROR,
        DEVELOP_STEP_NO_NOT_SET_ERROR,
        DEVELOP_END_STEP_NO_NOT_SET_ERROR,
        DEVELOP_ERROR_STEP_NO_NOT_SET_ERROR,
        DEVELOP_WRONG_MODE1_ERROR,
        DEVELOP_WRONG_MODE2_ERROR,
        DEVELOP_ENUM_FORMAT_NOT_SUPPORTED_ERROR,
        DEVELOP_UNDEFINED_RECIPE_TYPE_ERROR,
        DEVELOP_TRY_READING_FROM_GROUP_ITEM_ERROR,
        DEVELOP_PARAMETER_ID_ALREADY_USING_ERROR,
        DEVELOP_NOT_ALLOWED_RECIPE_CREATING_ERROR,

        // Recipe, Pattern 및 Sequence 오류 (0x0B) - 사용자가 잘못된 시퀀스/레시피를 생성했을 때
        RECIPE_END_CONDITION_NOT_SET_ERROR = 0x0B00,
        RECIPE_SOURCING_VALUE_NOT_SET_ERROR,
        RECIPE_INVALID_PARAMETER_VALUE_ERROR,
        RECIPE_SAFETY_CONDITION_NOT_SET_ERROR,
        RECIPE_NOT_ALLOWED_ERROR,
        RECIPE_INVALID_LOOP_COUNT_ERROR,
        RECIPE_INVALID_JUMP_COUNT_ERROR,
        RECIPE_SAVE_CONDITION_INTERVAL_TOO_SMALL_ERROR,

        SEQUENCE_CYCLE_LOOP_NOT_MATCHED_ERROR,
        SEQUENCE_EMPTY_ERROR,
        SEQUENCE_NAME_EMPTY_ERROR,
        SEQUENCE_ALREADY_FULL_ERROR,
        SEQUENCE_INDEX_OUT_OF_RANGE_ERROR,
        SEQUENCE_LABEL_NOT_FOUND_ERROR,
        SEQUENCE_INVALID_LABEL_LOCATION_ERROR,
        SEQUENCE_INVALID_JUMP_LOCATION_ERROR,
        SEQUENCE_CANNOT_UNDER_JUMP_ERROR,

        PATTERN_INVALID_BIAS_MODE_ERROR,
        PATTERN_INVALID_PULSE_WIDTH_ERROR,
        PATTERN_INVALID_PULSE_ITEM_ERROR,
        PATTERN_IS_EMPTY_ERROR,
        PATTERN_FILE_NOT_EXISTS_ERROR,
        PATTERN_TOO_MANY_PULSE_ITEM_ERROR,
        PATTERN_INVALID_FILE_NAME_ERROR,


        // Resister 오류 (0x0C) - 상태 레지스터 조회의 결과 값에 해당하는 오류
        REGISTER_BATTERY_SAFE_ALARM_ERROR = 0x0C00,
        REGISTER_SD_FILE_MEET_4GM_ERROR,
        REGISTER_SD_FREE_SPACE_TOO_SMALL_ERROR,
        REGISTER_SD_RW_FAIL_ERROR,
        REGISTER_SD_INITIAL_FAIL_ERROR,
        REGISTER_INITIAL_FAIL_ERROR,
        REGISTER_READ_FAIL_ERROR,
        REGISTER_NORMAL_ERROR,

        UNDEFINED_ERROR = 0xFFFF
    }

    public class QException : Exception
    {
        public QExceptionType QExceptionType { get; }
        

        private static string getErrorMessage( ref QExceptionType errorType, string token = null )
        {
            string message = null;

            switch( errorType )
            {
                #region 통신 오류 (0x00)
                case QExceptionType.COMMUNICATION_TIMEOUT_ERROR:
                    message = "응답 대기시간을 초과했습니다.";
                    break;
                case QExceptionType.COMMUNICATION_SOCKET_CLOSED_ERROR:
                    message = "소켓이 닫혔습니다.";
                    break;
                case QExceptionType.COMMUNICATION_PING_ACK_NOT_RECEIVED_ERROR:
                    message = "PING의 응답이 돌아오지 않습니다.";
                    break;
                case QExceptionType.COMMUNICATION_PORT_SOCKET_WRONG_MESSAGE_ERROR:
                    message = "Port 할당 소켓의 응답 메시지가 잘못된 형식입니다.";
                    break;
                case QExceptionType.COMMUNICATION_INVALID_IP_FORMAT_ERROR:
                    message = "IP의 형식이 올바르지 않습니다.";
                    break;
                case QExceptionType.COMMUNICATION_FIRMWARE_VERSION_NOT_COMPATIBLE_ERROR:
                    message = "호환되지 않는 펌웨어 버전입니다.";
                    break;
                case QExceptionType.COMMUNICATION_NOT_COMPATIBLE_DEVICE_ERROR:
                    message = "호환되지 않는 장비입니다.";
                    break;
                #endregion

                #region 패킷 오류 (0x01)
                case QExceptionType.PACKET_CRC_ERROR:
                    message = "패킷의 CRC가 올바르지 않습니다.";
                    break;
                case QExceptionType.PACKET_CRC2_ERROR:
                    message = "패킷의 CRC2가 올바르지 않습니다.";
                    break;
                case QExceptionType.PACKET_REJECT_WRITING_ERROR:
                    message = "패킷의 읽기 전용 필드에 쓰기를 시도했습니다.";
                    break;
                case QExceptionType.PACKET_SUBPACKET_INDEX_OUT_OF_RANGE_ERROR:
                    message = "인덱스가 서브 패킷의 개수 범위를 초과했습니다.";
                    break;
                case QExceptionType.PACKET_DATA_INDEX_OUT_OF_RANGE_ERROR:
                    message = "인덱스가 패킷의 DATA 필드 범위를 초과했습니다.";
                    break;
                case QExceptionType.PACKET_DATA2_INDEX_OUT_OF_RANGE_ERROR:
                    message = "인덱스가 패킷의 DATA2 필드 범위를 초과했습니다.";
                    break;
                case QExceptionType.PACKET_WRONG_DESTINATION_ERROR:
                    message = "수신 패킷의 목적지가 올바르지 않습니다.";
                    break;
                case QExceptionType.PACKET_CHANNEL_DATA_DATAFIELD_LENGTH_ERROR:
                    message = "채널 데이터 패킷의 데이터 필드의 길이가 올바르지 않습니다.";
                    break;
                case QExceptionType.PACKET_BOARD_INFORMATION_DATAFIELD_LENGTH_ERROR:
                    message = "보드 정보 패킷의 데이터 필드의 길이가 올바르지 않습니다.";
                    break;
                #endregion

                #region 명령 오류 (0x02)
                case QExceptionType.DEVELOP_WRONG_COMMAND_QUERY_USAGE_ERROR:
                    message = "지정된 Command가 지원하지 않는 Query의 사용을 시도했습니다.";
                    break;
                case QExceptionType.DEVELOP_WRONG_ADDRESS_USAGE_ERROR:
                    message = "지정된 Address에서는 사용할 수 없는 Command의 송신을 시도했습니다.";
                    break;
                case QExceptionType.DEVELOP_UNDEFINED_COMMAND_ERROR:
                    message = "정의되지 않은 명령어 타입입니다.";
                    break;
                case QExceptionType.DEVELOP_UNDEFINED_ENUMERATION_ERROR:
                    message = "정의되지 않은 열거형입니다.";
                    break;
                case QExceptionType.DEVELOP_UNEXPECTED_ERROR_FIELD_ERROR:
                    message = "예기치 못한 ERROR 필드 오류가 발생했습니다.";
                    break;
                case QExceptionType.DEVELOP_NULL_REFERENCE_ERROR:
                    message = "런타임에 예기치 못한 Null 참조 오류가 발생했습니다.";
                    break;
                #endregion

                #region 동작 오류 (0x03)
                case QExceptionType.ACTION_DEVICE_NOT_CONNECTED_ERROR:
                    message = "장비가 연결 상태가 아닙니다.";
                    break;
                case QExceptionType.ACTION_CHANNEL_INDEX_OUT_OF_RANGE_ERROR:
                    message = "인덱스가 채널의 개수 범위를 초과했습니다.";
                    break;
                case QExceptionType.ACTION_TOO_MANY_CHANNEL_COUNT_ERROR:   // Obsolete //
                    message = "검색된 채널의 개수가 지정된 채널의 개수보다 많습니다.";
                    break;
                case QExceptionType.ACTION_TOO_LAW_CHANNEL_COUNT_ERROR:    // Obsolete //
                    message = "검색된 채널의 개수가 지정된 채널의 개수보다 적습니다.";
                    break;
                case QExceptionType.ACTION_DEVICE_NOT_RESPONSE_ERROR:
                    message = "장비가 응답하지 않습니다.";
                    break;
                case QExceptionType.ACTION_FILE_WRITING_ERROR:
                    message = "파일 쓰기에 실패했습니다.";
                    break;
                case QExceptionType.ACTION_CHANNEL_NOT_CONNECTED_ERROR:
                    message = "연결되지 않은 채널입니다.";
                    break;
                case QExceptionType.ACTION_SEQUENCE_SENDING_ERROR:  // 시퀀스 전송 실패의 원인은 일반적으로 응답 없음 인데, DEVICE_NOT_RESPONSE_ERROR로 빼는게 낫지 않을까 싶음.
                    message = "시퀀스를 전송하는데 실패했습니다.";
                    break;
                case QExceptionType.ACTION_CHANNEL_ALREADY_RUN_ERROR:
                    message = "채널이 이미 측정을 진행중입니다.";
                    break;
                #endregion

                #region 응답 오류 (0x04)
                case QExceptionType.DEVICE_UNDEFINED_ERROR_FIELD_ERROR:
                    message = "정의되지 않은 ERROR 필드의 값입니다.(DEVICE)";
                    break;
                case QExceptionType.DEVICE_ACTION_ERROR:
                    message = "명령 동작을 수행 중 오류가 발생했습니다.(DEVICE)";
                    break;
                case QExceptionType.DEVICE_INITIALIZE_ERROR:
                    message = "장비 초기화 수행 오류가 발생했습니다.(DEVICE)";
                    break;
                case QExceptionType.DEVICE_PACKET_ERROR:
                    message = "수신 데이터 패킷이 올바르지 않습니다.(DEVICE)";
                    break;
                case QExceptionType.DEVICE_COMMAND_ERROR:
                    message = "수신 명령이 정의되지 않은 명령입니다.(DEVICE)";
                    break;
                case QExceptionType.DEVICE_BUSY_ERROR:
                    message = "이전 명령을 수행중입니다.(DEVICE)";
                    break;
                case QExceptionType.DEVICE_SET_ERROR:
                    message = "설정 동작 수행 중 오류가 발생했습니다.(DEVICE)";
                    break;
                case QExceptionType.DEVICE_SD_CARD_ERROR:
                    message = "SD 카드 오류가 발생했습니다.(DEVICE)";
                    break;
                case QExceptionType.DEVICE_ADDRESS_ERROR:
                    message = "패킷 수신 주소가 올바르지 않습니다.(DEVICE)";
                    break;
                case QExceptionType.DEVICE_UNDEFINED_ERROR:
                    message = "정의되지 않은 오류가 발생했습니다.(DEVICE)";
                    break;
                #endregion

                #region 입출력 오류 (0x05)
                case QExceptionType.IO_FILE_NOT_FOUND_ERROR:
                    message = "파일을 찾을 수 없습니다.";
                    break;
                case QExceptionType.IO_FILE_ALREADY_USING_ERROR:
                    message = "파일이 이미 사용중입니다.";
                    break;
                case QExceptionType.IO_FILE_ACCESS_DENIED_ERROR:
                    message = "파일 액세스가 거부되었습니다.";
                    break;
                case QExceptionType.IO_WRONG_DATA_FORMAT_ERROR:
                    message = "파일에 잘못된 형식의 데이터가 포함되어 있습니다.";
                    break;
                case QExceptionType.IO_INVALID_FILE_NAME_ERROR:
                    message = "파일 이름이 올바르지 않습니다.";
                    break;
                #endregion

                #region 시퀀스, 레시피 유저 컨트롤 오류 (0x0A)
                case QExceptionType.DEVELOP_GROUP_ATTRIBUTE_NOT_DEFINED_ERROR:
                    message = "Group 특성이 정의되지 않은 필드가 존재합니다.";
                    break;
                case QExceptionType.DEVELOP_PARAMETER_ATTRIBUTE_NOT_DEFINED_ERROR:
                    message = "Paramter 특성이 정의되지 않은 필드가 존재합니다.";
                    break;
                case QExceptionType.DEVELOP_FIELD_TYPE_NOT_DEFINED_ERROR:
                    message = "특성에 필드의 형식이 지정되지 않은 필드가 존재합니다.";
                    break;
                case QExceptionType.DEVELOP_STEP_NO_NOT_SET_ERROR:
                    message = "레시피의 Step No 속성에 값이 지정되지 않아 데이터 필드로의 변환에 실패했습니다.";
                    break;
                case QExceptionType.DEVELOP_END_STEP_NO_NOT_SET_ERROR:
                    message = "레시피의 End Step No 속성에 값이 지정되지 않아 데이터 필드로의 변환에 실패했습니다.";
                    break;
                case QExceptionType.DEVELOP_ERROR_STEP_NO_NOT_SET_ERROR:
                    message = "레시피의 Error Step No 속성에 값이 지정되지 않아 데이터 필드로의 변환에 실패했습니다.";
                    break;
                case QExceptionType.DEVELOP_WRONG_MODE1_ERROR:
                    message = "알 수 없는 문제로 인해 레시피의 Mode1 속성이 잘못 지정되었습니다.";
                    break;
                case QExceptionType.DEVELOP_WRONG_MODE2_ERROR:
                    message = "알 수 없는 문제로 인해 레시피의 Mode2 속성이 잘못 지정되었습니다.";
                    break;
                case QExceptionType.DEVELOP_ENUM_FORMAT_NOT_SUPPORTED_ERROR:
                    message = "데이터 필드로의 변환이 지원되지 않는 열거형 형식입니다.";
                    break;
                case QExceptionType.DEVELOP_UNDEFINED_RECIPE_TYPE_ERROR:
                    message = "정의되지 않은 레시피 타입입니다.";
                    break;
                case QExceptionType.DEVELOP_TRY_READING_FROM_GROUP_ITEM_ERROR:
                    message = "그룹 아이템으로부터 값을 읽으려고 시도했습니다.";
                    break;
                #endregion

                #region 레시피/시퀀스 오류 (0x0B)
                case QExceptionType.RECIPE_END_CONDITION_NOT_SET_ERROR:
                    message = "종료 조건이 설정되지 않았습니다.";
                    break;
                case QExceptionType.RECIPE_SOURCING_VALUE_NOT_SET_ERROR:
                    message = "출력 값이 설정되지 않았습니다.";
                    break;
                case QExceptionType.RECIPE_INVALID_PARAMETER_VALUE_ERROR:
                    message = "유효하지 않은 파라미터 값을 사용했습니다.";
                    break;
                case QExceptionType.RECIPE_SAFETY_CONDITION_NOT_SET_ERROR:
                    message = "안전 조건이 설정되지 않았습니다.";
                    break;
                case QExceptionType.RECIPE_NOT_ALLOWED_ERROR:
                    message = "사용할 수 없는 레시피입니다.";
                    break;
                case QExceptionType.RECIPE_INVALID_LOOP_COUNT_ERROR:
                    message = "Loop 횟수는 0보다 커야 합니다.";
                    break;
                case QExceptionType.RECIPE_INVALID_JUMP_COUNT_ERROR:
                    message = "Jump 횟수는 0보다 커야 합니다.";
                    break;
                case QExceptionType.RECIPE_SAVE_CONDITION_INTERVAL_TOO_SMALL_ERROR:
                    message = "Interval은 1초 미만일 수 없습니다.";
                    break;


                case QExceptionType.SEQUENCE_CYCLE_LOOP_NOT_MATCHED_ERROR:
                    message = "Cycle과 Loop가 적절하게 배치되지 않았습니다.";
                    break;
                case QExceptionType.SEQUENCE_EMPTY_ERROR:
                    message = "빈 시퀀스롤 송신하려고 시도했습니다.";
                    break;
                case QExceptionType.SEQUENCE_NAME_EMPTY_ERROR:
                    message = "시퀀스의 이름이 설정되지 않았습니다.";
                    break;
                case QExceptionType.SEQUENCE_ALREADY_FULL_ERROR:
                    message = "시퀀스가 포화 상태입니다.";
                    break;
                case QExceptionType.SEQUENCE_INDEX_OUT_OF_RANGE_ERROR:
                    message = "인덱스가 시퀀스 레시피 개수의 범위를 초과했습니다.";
                    break;
                case QExceptionType.SEQUENCE_LABEL_NOT_FOUND_ERROR:
                    message = "라벨을 찾을 수 없습니다.";
                    break;
                case QExceptionType.SEQUENCE_INVALID_LABEL_LOCATION_ERROR:
                    message = "라벨의 위치가 유효하지 않습니다. 라벨은 사이클 내부에 위치할 수 없습니다.";
                    break;
                case QExceptionType.SEQUENCE_INVALID_JUMP_LOCATION_ERROR:
                    message = "점프의 위치가 유효하지 않습니다. 점프는 사이클 내부에 위치할 수 없습니다.";
                    break;
                case QExceptionType.SEQUENCE_CANNOT_UNDER_JUMP_ERROR:
                    message = "라벨의 위치가 유효하지 않습니다. 점프 레시피의 위로만 이동할 수 있습니다.";
                    break;

                case QExceptionType.PATTERN_INVALID_BIAS_MODE_ERROR:
                    message = "패턴 파일의 Bias Mode가 올바르지 않습니다.";
                    break;
                case QExceptionType.PATTERN_INVALID_PULSE_WIDTH_ERROR:
                    message = "패턴 파일의 Pulse Width가 올바르지 않습니다.";
                    break;
                case QExceptionType.PATTERN_INVALID_PULSE_ITEM_ERROR:
                    message = "패턴 파일의 Pulse Item이 올바르지 않습니다.";
                    break;
                case QExceptionType.PATTERN_IS_EMPTY_ERROR:
                    message = "빈 패턴 파일을 송신하려고 시도했습니다.";
                    break;
                case QExceptionType.PATTERN_FILE_NOT_EXISTS_ERROR:
                    message = "패턴 파일을 찾을 수 없습니다.";
                    break;
                case QExceptionType.PATTERN_TOO_MANY_PULSE_ITEM_ERROR:
                    message = "패턴 데이터가 최대 개수를 초과했습니다.";
                    break;
                case QExceptionType.PATTERN_INVALID_FILE_NAME_ERROR:
                    message = "패턴 파일이 설정되지 않았거나, 올바르지 않은 파일명입니다.";
                    break;
                #endregion

                #region 레지스터 오류 (0x0C)
                case QExceptionType.REGISTER_BATTERY_SAFE_ALARM_ERROR:
                    message = "Battery safe alarm 상태가 발생했습니다.(REGISTER)";
                    break;
                case QExceptionType.REGISTER_SD_FILE_MEET_4GM_ERROR:
                    message = "SD file meet 4GB 상태가 발생했습니다.(REGISTER)";
                    break;
                case QExceptionType.REGISTER_SD_FREE_SPACE_TOO_SMALL_ERROR:
                    message = "SD free space too small 상태가 발생했습니다.(REGISTER)";
                    break;
                case QExceptionType.REGISTER_SD_RW_FAIL_ERROR:
                    message = "SD RW fail 상태가 발생했습니다.(REGISTER)";
                    break;
                case QExceptionType.REGISTER_SD_INITIAL_FAIL_ERROR:
                    message = "SD initial fail 상태가 발생했습니다.(REGISTER)";
                    break;
                case QExceptionType.REGISTER_INITIAL_FAIL_ERROR:
                    message = "Initial fail 상태가 발생했습니다.(REGISTER)";
                    break;
                case QExceptionType.REGISTER_READ_FAIL_ERROR:
                    message = "상태 레지스터 조회에 실패했습니다.(REGISTER)";
                    break;
                case QExceptionType.REGISTER_NORMAL_ERROR:
                    message = "오류가 발생하여 상태 레지스터를 조회했으나 이상이 발견되지 않았습니다.(REGISTER)";
                    break;
                #endregion

                // 정의되지 않은 오류 (0xFF)
                default:
                    message = "정의되지 않은 오류가 발생했습니다.";
                    errorType = QExceptionType.UNDEFINED_ERROR;
                    break;
            }

            return message + $"{ (token == null ? "" : " (" + token + ")")}";
        }
        private static string toMessage( ref QExceptionType errorType, string token = null, Exception ex = null )
        {
            var message = $"Error : {getErrorMessage( ref errorType )} (ERROR_CODE = 0x{( ushort )errorType:X4})";
            if( token != null )
            {
                message += $"\r\n{token}";
            }

            if( ex != null )
            {
                message += "\r\nDetails\r\n";
                var split = ex.StackTrace?.Substring( ex.StackTrace.LastIndexOf( "위치:" ) + 3 ).Split( '(' );

                if( split != null )
                {
                    var methodSplit = split[0].Split( '.' );
                    var methodName = methodSplit[methodSplit.Length - 2] + "." + methodSplit[methodSplit.Length - 1];
                    var fileSplit = split[1].Split( '\\' );
                    fileSplit = fileSplit[fileSplit.Length - 1].Split( ':' );
                    var fileName = fileSplit[0].Trim();
                    var lineNumber = fileSplit[1].Substring( 2 );

                    message += $"파일 : {fileName}\r\n" +
                               $"위치 : {methodName} ({lineNumber} 줄)\r\n" +
                               $"내부 원인 : {ex.Message}";
                }
            }

            return message;
        }
        public QException( QExceptionType errorType, string token = null, Exception inner = null ) : base( toMessage( ref errorType, token, inner ) )
        {
            QExceptionType = errorType;
        }
    }
}
