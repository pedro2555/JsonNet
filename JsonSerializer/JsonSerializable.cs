using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonSerializer
{
    /// <summary>
    /// AttributeTargets.Struct : Apply to structs only
    /// Inherited = true        : Allow derived classes to maintain attribute on
    ///                             inhereted members
    /// AllowMultiple = false   : Allow any number of members to have the
    ///                             attribute on a single class
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class JsonSerializable : Attribute
    {
        public JsonSerializable()
        { }
    }
}
