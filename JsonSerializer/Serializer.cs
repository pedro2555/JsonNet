using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JsonSerializer
{
    /// <summary>
    /// Serializes and deserializes an object, or an entire graph of connected
    /// objects, in JSON format.
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// Returns all properties of a class that have the  JsonSerializable
        /// attribute, or all properties if the class itself has such attribute
        /// </summary>
        /// <param name="type">The class type</param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            // Get the attribute type
            var attributeType = typeof(JsonSerializable);

            // Return all properties if JsonSerializable is set on the class
            if (Attribute.GetCustomAttribute(type, attributeType) != null)
                return type.GetProperties();
            // Return only JsonSerializable properties
            else
                return type
                    .GetProperties()
                    .Where(
                        prop => prop.GetCustomAttributes(
                            attributeType,
                            true).Any());
        }

        #region Composing methods

        /// <summary>
        /// Composes a .NET string into it's JSON representation
        /// 
        /// C#
        /// string test = "test";
        /// 
        /// JSON
        /// "test"
        /// </summary>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        private static void ComposeString(Stream target, object obj)
        {
            WriteToStream(target, String.Format("\"{0}\"", (string)obj));
        }

        /// <summary>
        /// Composes a .NET int into it's JSON representation
        /// 
        /// C#
        /// int test = 12;
        /// 
        /// JSON
        /// 12
        /// </summary>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        private static void ComposeInt(Stream target, object obj)
        {
            WriteToStream(target, (int)obj);
        }

        /// <summary>
        /// Composes a .NET float into it's JSON representation
        /// 
        /// C#
        /// float test = 12.3f;
        /// 
        /// JSON
        /// 12.3
        /// </summary>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        private static void ComposeFloat(Stream target, object obj)
        {
            WriteToStream(target, (float)obj);
        }

        /// <summary>
        /// Composes a .NET null into it's JSON representation
        /// 
        /// C#
        /// var test = null;
        /// 
        /// JSON
        /// null
        /// </summary>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        private static void ComposeNull(Stream target)
        {
            WriteToStream(target, "null");
        }

        /// <summary>
        /// Composes a .NET bool into it's JSON representation
        /// 
        /// C#
        /// bool test = true;
        /// 
        /// JSON
        /// true
        /// </summary>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        private static void ComposeBool(Stream target, object obj)
        {
            WriteToStream(target, (bool)obj);
        }

        /// <summary>
        /// Composes a .NET Array into it's JSON representation
        /// 
        /// C#
        /// object[] test = new object[0];
        /// 
        /// JSON
        /// []
        /// </summary>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        private static void ComposeArray(Stream target, object obj)
        {
            bool firstEntryFlag = true;

            WriteToStream(target, (firstEntryFlag) ? '[' : ',');
            firstEntryFlag = false;

            foreach (object o_obj in (object[])obj)
                ComposeValue(target, o_obj);

            WriteToStream(target, ']');
        }

        /// <summary>
        /// Handles a .NET object to it's respective composer method
        /// </summary>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        private static void ComposeValue(Stream target, object obj)
        {
            if (obj is string)
            {
                ComposeString(target, obj);
                return;
            }

            if (obj is int)
            {
                ComposeInt(target, obj);
                return;
            }

            if (obj is float)
            {
                ComposeFloat(target, obj);
                return;
            }

            if (obj == null)
            {
                ComposeNull(target);
                return;
            }

            if (obj is bool)
            {
                ComposeBool(target, obj);
                return;
            }

            if (obj is Array)
            {
                ComposeArray(target, obj);
                return;
            }

            ComposeObject(target, obj);
            return;
        }

        /// <summary>
        /// Composes a .NET object into it's JSON representation
        /// </summary>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        private static void ComposeObject(Stream target, object obj)
        {
            bool firstEntryFlag = true;

            WriteToStream(target, '{');

            // Get serialzable properties
            var properties = GetProperties(obj.GetType());

            var attributeType = typeof(JsonSerializable);
            foreach (var propertyInfo in properties)
            {
                if (!firstEntryFlag)
                    WriteToStream(target, ',');

                WriteToStream(
                    target,
                    String.Format("\"{0}\":", propertyInfo.Name));
                ComposeValue(
                    target,
                    propertyInfo.GetValue(obj, null));

                firstEntryFlag = false;
            }

            WriteToStream(target, '}');
        }

        /// <summary>
        /// Helper method to write to stream while ensuring correct character
        /// enconding and stream writability
        /// </summary>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        private static void WriteToStream(Stream target, object obj)
        {
            using (var writer = new StreamWriter(
                target,
                Encoding.UTF8,
                16 * 1024,
                true))
                writer.Write(obj);
        }

        #endregion Composing methods

        /// <summary>
        /// Serializes an object, or graph of connected objects, to the given
        /// stream.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        public static void Serialize(Stream target, object obj)
        {
            ComposeValue(target, obj);

            // clean stream lenght
            long targetLenght = 0;
            target.Position = 0;

            while (target.ReadByte() != (int)'\0')
                targetLenght++;

            target.SetLength(targetLenght);
        }

        #region Parsing methods

        public static object ReadValue(Stream serializationStream)
        {
            // identifiy value type to send to proper reader

            int c = PeekNext(serializationStream);

            // null
            if ((char)c == 'n')
                return ReadNull(serializationStream);

            // boolean
            if ((char)c == 't')
                return ReadBoolean(serializationStream);
            if ((char)c == 'f')
                return ReadBoolean(serializationStream);

            // strings
            if ((char)c == '"')
                return ReadString(serializationStream);

            // numbers
            switch ((char)c)
            {
                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return ReadNumber(serializationStream);
            }

            // object
            if (c == (int)Tokens.BEGIN_OBJECT)
                return ReadObject(serializationStream);

            // array
            if (c == (int)Tokens.BEGIN_ARRAY)
                return ReadArray(serializationStream);

            throw new InvalidCastException();
        }

        private static int ReadNext(
            Stream target,
            bool returnWhitespace = false)
        {
            return GetNext(target, returnWhitespace, true);
        }

        private static int PeekNext(
            Stream target,
            bool returnWhitespace = false)
        {
            return GetNext(target, returnWhitespace, false);
        }

        private static int GetNext(
            Stream target,
            bool returnWhitespace = false,
            bool consume = true)
        {
            int c = -1;

            using (var reader = new StreamReader(
                target,
                Encoding.UTF8,
                false,
                Convert.ToInt32(target.Length),
                true))
            {
                while (target.CanRead)
                {
                    if (consume)
                        c = reader.Read();
                    else
                        c = reader.Peek();

                    if (Enum.IsDefined(typeof(Whitespace), (int)c))
                    {
                        if (returnWhitespace)
                            break;
                        else
                            reader.Read();
                    }
                    else
                        break;
                }
            }

            if (c == -1)
                throw new IndexOutOfRangeException();

            return c;
        }

        #region JSON value readers

        private static string ReadString(Stream target)
        {
            string s = "";
            bool escape = false;

            if (ReadNext(target, true) != (int)Tokens.QUOTATION_MARK)
                throw new InvalidCastException();

            do
            {
                // return at unescaped quotation mark
                if (PeekNext(target, true) == (int)Tokens.QUOTATION_MARK
                    && !escape)
                    break;

                // set escape character to avoid early return
                escape = (PeekNext(target, true) == (int)Tokens.ESCAPE
                    && !escape) ? true : false;

                // add character to result
                s += (char)ReadNext(target, true);
            } while (true);
            return s;
        }

        private static object ReadNumber(Stream target)
        {
            string s = "";
            int c;

            do
            {
                switch (PeekNext(target))
                {
                    case (int)Tokens.END_OBJECT:
                    case (int)Tokens.END_ARRAY:
                    case (int)Tokens.VALUE_SEPARATOR:
                        string result_raw = s.ToString().Trim();
                        // try to return float
                        float result_f;
                        int result_i;
                        if (int.TryParse(result_raw, out result_i))
                            return result_i;
                        else if (float.TryParse(
                            result_raw,
                            NumberStyles.Float,
                            CultureInfo.InvariantCulture,
                            out result_f))
                            return result_f;
                        else
                            throw new IndexOutOfRangeException();
                    default:
                        s += (char)ReadNext(target);
                        break;
                }
            } while (true);

            // we should not have reached this far
            throw new InvalidCastException();
        }

        private static Hashtable ReadObject(Stream target)
        {
            Hashtable result = new Hashtable();
            object key, value;

            if (ReadNext(target) != (int)Tokens.BEGIN_OBJECT)
                throw new InvalidCastException();

            do
            {
                if (PeekNext(target) == (int)Tokens.END_OBJECT)
                    return result;

                if (PeekNext(target) != (int)Tokens.QUOTATION_MARK)
                    throw new InvalidCastException();

                key = ReadString(target);

                if (PeekNext(target) != (int)Tokens.NAME_SEPARATOR)
                    throw new InvalidCastException();

                value = ReadValue(target);

                result.Add(key, value);

            } while (PeekNext(target) == (int)Tokens.VALUE_SEPARATOR);

            if (PeekNext(target) == (int)Tokens.END_OBJECT)
                return result;

            throw new InvalidCastException();
        }

        private static object[] ReadArray(Stream target)
        {
            List<object> result = new List<object>();

            do
            {
                if (PeekNext(target) == (int)Tokens.END_ARRAY)
                    return result.ToArray<object>();

                result.Add(ReadValue(target));

            } while (PeekNext(target) == (int)Tokens.VALUE_SEPARATOR);

            if (PeekNext(target) == (int)Tokens.END_ARRAY)
                return result.ToArray<object>();

            throw new InvalidCastException();
        }

        private static bool ReadBoolean(Stream target)
        {
            int c = ReadNext(target);
            bool result;
            string result_value;
            string s = "";

            switch ((char)c)
            {
                case 't':
                    result = true;
                    result_value = "true";
                    break;
                case 'f':
                    result = false;
                    result_value = "false";
                    break;
                default:
                    throw new InvalidCastException();
            }

            s += (char)c;
            for (int i = 0; i < result_value.Length - 1; i++)
                s += (char)ReadNext(target);

            if (s.ToString() == result_value)
                return result;

            throw new InvalidCastException();
        }

        private static object ReadNull(Stream target)
        {
            int c = (int)ReadNext(target);
            string s = "";

            switch ((char)c)
            {
                case 'n':
                    break;
                default:
                    throw new InvalidCastException();
            }

            s += c;
            for (int i = 0; i < 3; i++)
                s += (char)ReadNext(target);

            if (s.ToString() == "null")
                return null;
            
            throw new InvalidCastException();
        }

        #endregion JSON value readers

        private enum Whitespace
        {
            TABULATION = '\t',      // (int)0x09,
            LINE_FEED = '\n',       // (int)0x0A,
            CARRIAGE_RETURN = '\r', // (int)0x0D,
            SPACE = ' '             // (int)0x20
        }
        private enum Tokens
        {
            QUOTATION_MARK = 0x22,
            REVERSE_SOLIDUS = 0x5c,
            SOLIDUS = 0x2f,
            BACKSPACE = 0x62,
            FORM_FEED = 0x66,
            LINE_FEED = 0x6e,
            CARRIAGE_RETURN = 0x72,
            TAB = 0x74,
            ESCAPE = 0x5c,

            BEGIN_ARRAY = 0x5b,
            BEGIN_OBJECT = 0x7b,
            END_ARRAY = 0x5d,
            END_OBJECT = 0x7d,
            NAME_SEPARATOR = 0x3a,
            VALUE_SEPARATOR = 0x2c
        }

        #endregion Parsing methods

        ///// <summary>
        ///// Deserializes a stream into an object graph.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="source"></param>
        ///// <returns></returns>
        //public static T Deserialize<T>(Stream source) where T : class, new()
        //{
        //    var properties = GetProperties(typeof(T));
        //    var obj = new T();
        //    using (var reader = new StreamReader(source))
        //    {
        //        var attributeType = typeof(JsonSerializable);
        //        foreach (var propertyInfo in properties)
        //        {
        //            var attr = (JsonSerializable)propertyInfo.GetCustomAttributes(attributeType, false).First();
        //            var buffer = new char[attr.Length];
        //            reader.Read(buffer, 0, buffer.Length);
        //            var value = new string(buffer).Trim();

        //            if (propertyInfo.PropertyType != typeof(string))
        //                propertyInfo.SetValue(obj, Convert.ChangeType(value, propertyInfo.PropertyType), null);
        //            else
        //                propertyInfo.SetValue(obj, value.Trim(), null);
        //        }
        //    }
        //    return obj;
        //}
    }
}
