namespace McQLib.Device
{
    /// <summary>
    /// 통합 프로토콜 명령 분류를 구성하는 열거형입니다.
    /// <br>세부 명령의 이름 앞에 M_이 붙은 것은 Master Board 전용, S_가 붙은 것은 Slave Board 전용 명령어임.</br>
    /// <br>세부 명령의 이름 뒤에 _G가 붙은 것은 조회 전용 명령, _S가 붙은 것은 설정 전용 명령, _GS가 붙은 것은 조회 및 설정 가능 명령,</br>
    /// <br>세부 명령의 이름 뒤에 _R이 붙은 것은 수신 전용 명령임.</br>
    /// </summary>
    public static class Commands
    {
        /// <summary>
        /// 공통 명령을 구성하는 열거형입니다.
        /// <br>명령 분류 코드는 0x00입니다.</br>
        /// </summary>
        public enum CommonCommands : ushort
        {
            /// <summary>
            /// 보드와 기본 통신 상태를 확인하는 명령입니다.
            /// <br>정상 동작 상태일 경우 응답 코드, 그렇지 않은 경우 오류 코드로 응답합니다.</br>
            /// </summary>
            Ping_G = 0x0000,
            /// <summary>
            /// 보드를 소프트웨어적으로 리셋시키는 명령입니다.
            /// <br>장비에서는 이 명령에 대한 응답을 보낸 후 리셋 동작에 들어갑니다.</br>
            /// </summary>
            ResetBoard_S = 0x0001,
            /// <summary>
            /// 보드의 기본 정보를 조회하는 명령입니다.
            /// </summary>
            BoardInformation_G = 0x0002,
            /// <summary>
            /// 에러 상태나 이벤트를 조회하거나, '0' 값으로 초기화하도록 하는 명령입니다.
            /// <br>에러가 한 번이라도 발생했다면 값을 초기화하기 전까지 그 값을 유지하고 있습니다.</br>
            /// </summary>
            InitStateResister_GS = 0x0010,
            /// <summary>
            /// 저장 항목들을 메모리에 저장하도록 하는 명령입니다.
            /// <br>장비에서는 이 명령에 대한 응답을 보낸 후 저장 동작에 들어갑니다.</br>
            /// </summary>
            SaveRegister_S = 0x0014
        }
        /// <summary>
        /// 다채널 명령을 구성하는 열거형입니다.
        /// <br>명령 분류 코드는 0x0B입니다.</br>
        /// </summary>
        public enum MultiChannelCommands : ushort
        {
            /// <summary>
            /// Slave 보드가 장착되었는지 재탐색할 수 있도록 설정합니다.
            /// <br>Slave 보드의 개수에 따라 Slave Address부터 채널 상태까지 반복됩니다.</br>
            /// </summary>
            M_BoardSearching_GS = 0x0B01,
            /// <summary>
            /// Slave 보드가 장착되어 있다면 Slave 보드의 기본 정보를 조회합니다.
            /// </summary>
            M_SlaveBoardInformation_G = 0x0B02,
            /// <summary>
            /// Slave 모듈이 데이터를 송신할 수 있도록 제어권을 설정 및 해제합니다.
            /// <br>멀티 채널일 경우 일정한 시간 간격으로 다른 채널로 제어권을 넘겨줘야 합니다.</br>
            /// </summary>
            S_SlaveBoardTransmissionControl_GS = 0x0BFF
        }
        /// <summary>
        /// Battery Cycler 설정/조회 명령을 구성하는 열거형입니다.
        /// <br>명령 분류 코드는 0x10입니다.</br>
        /// </summary>
        public enum BatteryCycler_SetGetCommands : ushort
        {
            /// <summary>
            /// 시퀀스를 시작 또는 진행중인 시퀀스를 정지(종료)합니다.
            /// <br>정지되기 전까지 정지 조건을 변경할 수 있으며 시퀀스가 일시 정지 상태라면 재시작합니다.</br>
            /// </summary>
            StartStopSequence = 0x1000,
            /// <summary>
            /// 진행중인 시퀀스를 일시 정지합니다.
            /// <br>정지되기 전까지 정지 조건을 변경할 수 있습니다.</br>
            /// </summary>
            PauseSequence = 0x1001,
            SkipSequence = 0x1002,
            TimeStamp_GS = 0x100A,
            SequenceStep_GS = 0x1010,
            SequenceChargeDischarge_GS = 0x1011,
            SequenceMeasurement_GS = 0x1012,
            SequencePattern_GS = 0x1013,
            SequencePatternData_GS = 0x1014,
            SequenceWait_GS = 0x1015,
            M_MemoryFormat = 0x10F0,
            M_RemoveChannelData = 0x10FA
        }
        /// <summary>
        /// Battery Cycler 조회/측정 명령을 구성하는 열거형입니다.
        /// <br>명령 분류 코드는 0x11입니다.</br>
        /// </summary>
        public enum BatteryCycler_GetMeasureCommands : ushort
        {
            ChannelState_G = 0x1100,
            M_ChannelSequenceData_G = 0x1101,
            M_PatternSequenceData_G = 0x1102,
            M_FastMeasureSequenceData_G = 0x1103,
            ChannelSequenceData_R = 0x1111,
            PatternSequenceData_R = 0x1112,
            FastMeasureSequenceData_R = 0x1113,
            M_SavedSequenceData_R = 0x1120
        }
        /// <summary>
        /// Q3000/Q2000 Slot 명령을 구성하는 열거형입니다.
        /// <br>명령 분류 코드는 0x14입니다.</br>
        /// </summary>
        public enum Q3000Q2000_SlotCommands : ushort
        {
            InitAlarm_GS = 0x1401,
            OutputSwitch_GS = 0x1402,
            InputSwitch_GS = 0x1403,
            ProgramOutput_GS = 0x1410,
            VoltageMinMaxOutput_GS = 0x1411,
            DcCancelOutput_GS = 0x1413,
            AdcInputData_G = 0x1420,
            MeasureTemperature_G = 0x1421,
            SineWave_S = 0x1433,
            FastMeasure_S = 0x143A,
            SequentialOutput = 0x1440,
            SequentialOutputState_G = 0x144A,
            FastMeasure_G = 0x1450,
            VoltageSpec_GS = 0x14F0,
            CurrentSpec_GS = 0x14F1,
            AmplifiedVoltageSpec_GS = 0x14FA
        }
        /// <summary>
        /// 생산 명령을 구성하는 열거형입니다.
        /// <br>명령 분류 코드는 0x80입니다.</br>
        /// </summary>
        public enum ProductionCommonCommands : ushort
        {
            BoardInformation_S = 0x8002,
            EhternetAddress_GS = 0x8003,
            StateRegister_GS = 0x8010,
            SaveSetting = 0x8014,
            InitSave = 0x8015,
            ApplyCalibrationValues = 0x8020
        }
        /// <summary>
        /// 측정 보상값 명령을 구성하는 열거형입니다.
        /// <br>명령 분류 코드는 0xF0입니다.</br>
        /// </summary>
        public enum CalibrationCommands : ushort
        {
            OutputCurrentSlope_GS = 0xF000,
            OutputCurrentOffset_GS = 0xF001,
            OutputVoltageSlope_GS = 0xF002,
            OutputVoltageOffset_GS = 0xF003,
            LimitSlope_GS = 0xF004,
            LimitOffset_GS = 0xF005,
            OutputCancelVoltageSlope_GS = 0xF006,
            OutputCancelVoltageOffset_GS = 0xF007,
            InputCurrentSlope_GS = 0xF010,
            InputCurrentOffset_GS = 0xF011,
            InputVoltageSlope_GS = 0xF012,
            InputVoltageOffset_GS = 0xF013,
            InputAmplifiedVoltageSlope_GS = 0xF014,
            InputAmplifiedVoltageOffset_GS = 0xF015,
            TemperatureSlope_GS = 0xF0A0,
            TemperatureOffset_GS = 0xF0A1,
            ReadADC = 0x1420
        }
    }
}
