using System;

namespace JsonSerializer
{
    /// <summary>
    /// AttributeTargets.Struct : Apply to structs only
    /// Inherited = true        : Allow derived classes to maintain attribute on
    ///                             inhereted members
    /// AllowMultiple = false   : Allow any number of members to have the
    ///                             attribute on a single class
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Property,
        Inherited = true,
        AllowMultiple = true)]
    public class JsonSerializable : Attribute
    {
        public int Order { get; set; }

        public JsonSerializable(int order = -1)
        {
            this.Order = order;
        }
    }
}
