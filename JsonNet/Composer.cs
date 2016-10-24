using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonNet
{
    /// <summary>
    /// TODO
    /// </summary>
    public static class Composer
    {
        private static string ComposeString(object value)
        {
            string var = (string)value;
            StringBuilder sb = new StringBuilder();

            sb.Append("\"");
            sb.Append(var);
            sb.Append("\"");

            return sb.ToString();
        }
    }
}
