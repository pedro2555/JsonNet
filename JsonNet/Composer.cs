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

        private static string ComposeInt(object value)
        {
            int var = (int)value;
            StringBuilder sb = new StringBuilder();

            sb.Append(var.ToString());

            return sb.ToString();
        }

        private static string ComposeFloat(object value)
        {
            float var = (float)value;
            StringBuilder sb = new StringBuilder();

            sb.Append(var.ToString());

            return sb.ToString();
        }

        private static string ComposeNull()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("null");

            return sb.ToString();
        }

        private static string ComposeBool(object value)
        {
            bool var = (bool)value;
            StringBuilder sb = new StringBuilder();

            sb.Append(var.ToString());

            return sb.ToString();
        }
    }
}
