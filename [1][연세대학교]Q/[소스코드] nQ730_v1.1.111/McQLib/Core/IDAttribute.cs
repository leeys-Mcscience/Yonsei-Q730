using System;

namespace McQLib.Core
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IDAttribute : Attribute
    {
        public string ID { get; }

        public IDAttribute( string id ) => ID = id;
    }
}
