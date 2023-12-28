using System;
using System.IO;
using System.Collections.Generic;

namespace McQLib.IO
{
    public enum DataType
    {
        // 각 데이터에 대한 구분기호
        Rest = 0x10,
        Cycle = 0x11,
        Loop = 0x12,
        Jump = 0x13,
        End = 0x14,

        Charge = 0x20,
        Discharge = 0x21,
        AnodeCharge = 0x22,
        AnodeDischarge = 0x23,

        TransientResponse = 0x30,
        OpenCircuitVoltage = 0x31,
        DcResistance = 0x32,
        AcResistance = 0x33,
        FrequencyResponse = 0x34,

        Pattern = 0x40,

        Temperature = 0x50,

        AcResistance_Raw = 0x60,
        FrequencyResponse_Raw = 0x61,
        DcResistance_Raw = 0x62,
    }

    public abstract class QDataStream
    {
        protected const byte STX = 0x02;
        protected const byte ETX = 0x03;
        protected const byte SEQ = 0xF0;
        protected const byte RCP = 0xF1;
        protected const byte INF = 0xF2;

        // Default Version Info
        protected byte _major = 0;
        protected byte _minor = 3;

        protected FileStream _stream;

        protected abstract bool selectVersion();

        public void Close()
        {
            _stream?.Close();
        }

        public byte Major => _major;
        public byte Minor => _minor;
        public long Position
        {
            get => _stream == null ? -1 : _stream.Position;
            set
            {
                if ( _stream != null ) _stream.Position = value;
            }
        }
    }
}