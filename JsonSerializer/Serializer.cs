﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
            return type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.IsDefined(typeof(JsonSerializable)));
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

            WriteToStream(target, '[');

            foreach (object o_obj in (object[])obj)
            {
                if (!firstEntryFlag)
                    WriteToStream(target, ',');

                ComposeValue(target, o_obj);

                firstEntryFlag = false;
            }

            WriteToStream(target, ']');
        }

        /// <summary>
        /// Handles a .NET object to it's respective composer method
        /// </summary>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        private static void ComposeValue(Stream target, object obj)
        {
            if (obj is string || obj is Enum || obj is Guid)
            {
                ComposeString(target, obj.ToString());
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

            if (obj is Hashtable)
            {
                foreach (DictionaryEntry e in (Hashtable)obj)
                {
                    if (!firstEntryFlag)
                        WriteToStream(target, ',');

                    WriteToStream(
                        target,
                        String.Format("\"{0}\":", e.Key));
                    ComposeValue(
                        target,
                        e.Value);

                    firstEntryFlag = false;
                }
            }
            else
            {
                // Get serialzable properties
                var properties = GetProperties(obj.GetType());

                foreach (var propertyInfo in properties)
                {
                    if (!firstEntryFlag)
                        WriteToStream(target, ',');

                    WriteToStream(
                        target,
                        String.Format("\"{0}\":", propertyInfo.Name));
                    ComposeValue(
                        target,
                        propertyInfo.GetValue(obj));

                    firstEntryFlag = false;
                }
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
        public void Serialize(Stream target, object obj)
        {
            ComposeValue(target, obj);

            // using preallocated mmemory streams might require over allocation
            // prevention logic, as this snippet tries to be
            //// clean stream lenght
            //long targetLenght = 0;
            //target.Position = 0;

            // while (target.CanRead && target.ReadByte() != (int)'\0')
            //    targetLenght++;

            //target.SetLength(targetLenght);
        }

        #region Parsing methods

        /// <summary>
        /// Reads any valid JSON value, otherwise an InvalidCastException is
        /// thrown.
        /// </summary>
        /// <param name="bytes">An UTF-8 Enconded byte array</param>
        /// <param name="position">The position in the array where the value
        /// starts.</param>
        /// <returns>
        ///     The return value can be one of:
        ///         * null
        ///         * bool
        ///         * string
        ///         * int
        ///         * float
        ///         * Hashtable (JSON objects and arrays)
        /// </returns>
        public static object ReadValue(byte[] bytes, ref int position)
        {
            // identifiy value type to send to proper reader

            char byte_c = (char)ReadNext(bytes, ref position);

            // null
            if (byte_c.ToString().ToLower() == "n")
            {
                position--;
                return ReadNull(bytes, ref position);
            }

            // boolean
            if (byte_c.ToString().ToLower() == "t")
            {
                position--;
                return ReadBoolean(bytes, ref position);
            }
            if (byte_c.ToString().ToLower() == "f")
            {
                position--;
                return ReadBoolean(bytes, ref position);
            }

            // strings
            if (byte_c == '"')
                return ReadString(bytes, ref position);

            // numbers
            switch (byte_c)
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
                    position--;
                    return ReadNumber(bytes, ref position);
            }

            // object
            if (byte_c == '{')
                return ReadObject(bytes, ref position);

            // array
            if (byte_c == '[')
                return ReadArray(bytes, ref position);

            throw new InvalidCastException();
        }

        /// <summary>
        /// Reads the next character in the given position
        /// </summary>
        /// <param name="bytes">The byte[] from where to read</param>
        /// <param name="position">The position in the byte[] to read</param>
        /// <param name="ignoreWhitespace">Whitespace is jumped and not 
        /// returned</param>
        /// <returns></returns>
        private static byte ReadNext(
            byte[] bytes,
            ref int position,
            bool returnWhitespace = false,
            bool acceptEscaped = false)
        {
            while (true)
            {
                // fail if index out of bounds
                if (position >= bytes.Length)
                    throw new IndexOutOfRangeException();

                byte c = bytes[position];

                // handle whitespace
                if (Enum.IsDefined(typeof(Whitespace), (int)c))
                    if (!returnWhitespace)
                    {
                        position++;
                        continue;
                    }

                position++;
                return c;
            }
        }

        #region JSON value readers

        /// <summary>
        /// Reads a string value
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static string ReadString(byte[] bytes, ref int position)
        {
            char byte_c;
            bool escape = false;
            StringBuilder result = new StringBuilder();

            // read till "
            while (true)
            {
                byte_c = (char)ReadNext(bytes, ref position, true);

                if (escape)
                {
                    result.Append(byte_c);
                    escape = false;
                    continue;
                }

                if (byte_c == '\\')
                {
                    escape = true;
                    result.Append(byte_c);
                    continue;
                }

                if (byte_c == '"')
                    return result.ToString();

                result.Append(byte_c);
            }
        }

        /// <summary>
        /// Reads a float or integer value
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static object ReadNumber(byte[] bytes, ref int position)
        {
            // out of a number we should find } ] , 
            char byte_c;
            StringBuilder result = new StringBuilder();
            while (true)
                switch ((byte_c = (char)ReadNext(bytes, ref position)))
                {
                    case '}':
                    case ']':
                    case ',':
                        // get our position indicator back
                        position--;
                        string result_raw = result.ToString().Trim();
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
                        result.Append(byte_c);
                        break;
                }

            // we should not have reached this far
            throw new InvalidCastException();
        }

        /// <summary>
        /// Reads a JSON object, delimited by { }
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static Hashtable ReadObject(byte[] bytes, ref int position)
        {
            char byte_c;
            Hashtable result = new Hashtable();
            object key, value;

            do
            {
                byte_c = (char)ReadNext(bytes, ref position);

                if (byte_c == '}')
                    return result;

                if (byte_c != '"')
                    throw new InvalidCastException();

                key = ReadString(bytes, ref position);

                if ((byte_c = (char)ReadNext(bytes, ref position)) != ':')
                    throw new InvalidCastException();

                value = ReadValue(bytes, ref position);

                result.Add(key, value);

            } while ((byte_c = (char)ReadNext(bytes, ref position)) == ',');

            if (byte_c == '}')
                return result;

            throw new InvalidCastException();
        }

        /// <summary>
        /// Reads a JSON array, delimited by [ ]
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static object[] ReadArray(byte[] bytes, ref int position)
        {
            char byte_c;
            List<object> r = new List<object>();

            do
            {
                byte_c = (char)ReadNext(bytes, ref position);

                if (byte_c == ']')
                    return r.ToArray();
                else
                    position--;

                r.Add(ReadValue(bytes, ref position));

            } while ((byte_c = (char)ReadNext(bytes, ref position)) == ',');

            if (byte_c != ']')
                throw new InvalidCastException();

            return r.ToArray();
        }

        /// <summary>
        /// Reads a bool value
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static bool ReadBoolean(byte[] bytes, ref int position)
        {
            char byte_c = (char)ReadNext(bytes, ref position);
            bool result;
            string result_value;
            StringBuilder result_test = new StringBuilder();
            switch (byte_c.ToString().ToLower())
            {
                case "t":
                    result = true;
                    result_value = "true";
                    break;
                case "f":
                    result = false;
                    result_value = "false";
                    break;
                default:
                    throw new InvalidCastException();
            }

            result_test.Append(byte_c);
            for (int i = 0; i < result_value.Length - 1; i++)
                result_test.Append((char)ReadNext(bytes, ref position, true));

            if (result_test.ToString().ToLower() == result_value)
                return result;

            throw new InvalidCastException();
        }

        /// <summary>
        /// Reads a null value
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static object ReadNull(byte[] bytes, ref int position)
        {
            char byte_c = (char)ReadNext(bytes, ref position);
            StringBuilder result_test = new StringBuilder();
            switch (byte_c.ToString().ToLower())
            {
                case "n":
                    break;
                default:
                    throw new InvalidCastException();
            }

            result_test.Append(byte_c);
            for (int i = 0; i < 3; i++)
                result_test.Append((char)ReadNext(bytes, ref position, true));

            if (result_test.ToString().ToLower() == "null")
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

        public T Deserialize<T>(object source)
        {
            // our return value
            object r = null;

            if (source is Array)
            {
                Array _obj = (Array)source;
                // arrays
                r = Array.CreateInstance(
                    typeof(T).GetElementType(),
                    _obj.Length);
                int i = 0;
                foreach (object o in _obj)
                {
                    ((Array)r).SetValue(
                        typeof(Serializer)
                            .GetMethods()
                            .First(
                                m =>
                                m.Name == "Deserialize"
                                && m.ReturnType.Name == "T"
                                && m.IsGenericMethod)
                            .MakeGenericMethod(typeof(T).GetElementType())
                            .Invoke(
                                this,
                                new object[] { o }),
                        i++);
                }
            }
            else if (source is Hashtable)
            {
                Hashtable _obj = (Hashtable)source;
                r = Activator.CreateInstance(typeof(T));

                // iteract over every T property
                foreach (PropertyInfo pInfo in GetProperties(typeof(T)))
                {
                    MethodInfo method = typeof(Serializer)
                                .GetMethods()
                                .First(
                                    m =>
                                    m.Name == "Deserialize"
                                    && m.ReturnType.Name == "T"
                                    && m.IsGenericMethod);

                    // try get the corresponding value from the hashtable
                    if (!_obj.ContainsKey(pInfo.Name))
                        // property has no match on hashtable
                        throw new Exception();

                    if (_obj[pInfo.Name] is Hashtable || _obj[pInfo.Name] is Array)
                        // recursive recall
                        pInfo.SetValue(
                            r,
                            typeof(Serializer)
                                .GetMethods()
                                .First(
                                    m =>
                                    m.Name == "Deserialize"
                                    && m.ReturnType.Name == "T"
                                    && m.IsGenericMethod)
                                .MakeGenericMethod(pInfo.PropertyType)
                                .Invoke(
                                    this,
                                    new object[] { _obj[pInfo.Name] }));
                    else if (pInfo.PropertyType.BaseType == typeof(Enum))
                    {
                        // parse the Enum
                        pInfo.SetValue(
                            r,
                            Enum.Parse(
                                pInfo.PropertyType,
                                _obj[pInfo.Name].ToString()));
                    }
                    else
                        // just set the value
                        pInfo.SetValue(
                            r,
                            Convert.ChangeType(
                                _obj[pInfo.Name],
                                _obj[pInfo.Name].GetType()));
                }
            }
            else if (source is Enum)
            {
                return (T)Enum.Parse(typeof(T), source.ToString());
            }
            else
                try
                {
                    if (typeof(T).BaseType == typeof(Enum))
                        return (T)Enum.Parse(typeof(T), (string)source);
                    else
                        return (T)source;
                }
                catch (Exception crap)
                {
                    throw crap;
                }

            return (T)r;
        }

        /// <summary>
        /// Deserializes a stream into an object graph.
        /// </summary>
        /// <typeparam name="T">The type to which to cast.</typeparam>
        /// <param name="source">The stream from which to deserialize the object
        ///  graph.</param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T Deserialize<T>(Stream source)
        {
            return Deserialize<T>(Deserialize(source));
        }

        /// <summary>
        /// Deserializes a stream into an object graph that is direct
        /// correspondence of JSON data types into .NET types.
        /// </summary>
        /// <param name="source">The stream from which to deserialize the object
        /// graph.</param>
        /// <returns></returns>
        public object Deserialize(Stream source)
        {
            object jsonObj;

            // parse JSON stream
            using (var reader = new StreamReader(source))
            {
                char[] buffer = new char[reader.BaseStream.Length];
                int position = 0;

                reader.ReadBlock(
                        buffer,
                        0,
                        buffer.Length);

                jsonObj = Serializer.ReadValue(
                    Encoding.UTF8.GetBytes(buffer),
                    ref position);
            }

            return jsonObj;
        }
    }
}
