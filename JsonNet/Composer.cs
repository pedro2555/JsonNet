using System;
using System.Collections;
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

            sb.Append(var.ToString().ToLower());

            return sb.ToString();
        }

        private static string ComposeObject(object value)
        {
            Hashtable var = (Hashtable)value;
            StringBuilder sb = new StringBuilder();

            sb.Append("{");

            foreach (DictionaryEntry valuePair in var)
            {
                sb.AppendFormat("\"{0}\":{1},",
                    valuePair.Key,
                    ComposeValue(valuePair.Value));
            }

            if (sb[sb.Length - 1] == ',')
                sb[sb.Length - 1] = '}';
            else
                sb.Append('}');

            return sb.ToString();
        }

        private static string ComposeArray(object value)
        {
            object[] var = (object[])value;
            StringBuilder sb = new StringBuilder();

            sb.Append("[");

            foreach(object o_var in var)
            {
                sb.AppendFormat("{0},", ComposeValue(o_var));
            }

            if (sb[sb.Length - 1] == ',')
                sb[sb.Length - 1] = ']';
            else
                sb.Append(']');

            return sb.ToString();
        }

        public static string ComposeValue(object value)
        {
            if (value is string)
                return ComposeString(value);

            if (value is int)
                return ComposeInt(value);

            if (value is float)
                return ComposeFloat(value);

            if (value == null)
                return ComposeNull();

            if (value is bool)
                return ComposeBool(value);

            if (value is Hashtable)
                return ComposeObject(value);

            if (value is Array)
                return ComposeArray(value);

            throw new Exception();
        }
    }
}
