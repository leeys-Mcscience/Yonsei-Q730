using System;

namespace McQLib.NotUsed.Core
{
    [AttributeUsage( AttributeTargets.Field )]
    public class GroupAttribute : Attribute
    {
        public string Name { get; }
        public override string ToString() => Name;

        public GroupAttribute( string groupName ) => Name = groupName;
    }
}
