namespace McQLib.Recipes
{
    interface IPacketConvertable
    {
        byte[] ToDataField( ushort stepNo, ushort endStepNo, ushort errorStepNo );
        bool FromDataField( byte[] data );
    }
}
