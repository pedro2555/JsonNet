using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonNet
{
    public static class Parsers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="position"></param>
        /// <returns></returns>
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
        /// Returns the next character in the given position
        /// </summary>
        /// <param name="bytes">The byte[] from where to read</param>
        /// <param name="position">The position in the byte[] to read</param>
        /// <param name="ignoreWhitespace">Whitespace is jumped and not 
        /// returned</param>
        /// <returns></returns>
        private static byte ReadNext(
            byte[] bytes,
            ref int position,
            bool returnWhitespace = false)
        {
            while (true)
            {
                // fail if index out of bounds
                if (position >= bytes.Length)
                    throw new IndexOutOfRangeException();

                // may or not return whitespace
                if (!Enum.IsDefined(
                    typeof(Whitespace),
                    (int)bytes[position]))
                    return bytes[position++];
                else if (returnWhitespace)
                    return bytes[position++];

                // advance if whitespace
                position++;
            }
        }

        /// <summary>
        /// Reads a string value
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static string ReadString(byte[] bytes, ref int position)
        {
            char byte_c;
            StringBuilder result = new StringBuilder();

            // read till "
            while ((byte_c = (char)ReadNext(bytes, ref position, true)) != '"')
                result.Append(byte_c);

            return result.ToString();
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
                        break;
                    default:
                        result.Append(byte_c);
                        break;
                }

            // we should not have reached this far
            throw new InvalidCastException();
        }


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
        

        private static Hashtable ReadArray(byte[] bytes, ref int position)
        {
            char byte_c;
            Hashtable result = new Hashtable();

            do
            {
                byte_c = (char)ReadNext(bytes, ref position);

                if (byte_c == ']')
                    return result;
                else
                    position--;

                result.Add(result.Count, ReadValue(bytes, ref position));

            } while ((byte_c = (char)ReadNext(bytes, ref position)) == ',');

            if (byte_c != ']')
                throw new InvalidCastException();
            
            return result;
        }


        private static bool ReadBoolean(byte[] bytes, ref int position)
        {
            char byte_c = (char)ReadNext(bytes, ref position);
            bool result;
            string result_value;
            StringBuilder result_test = new StringBuilder();
            switch(byte_c.ToString().ToLower())
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













        public enum Whitespace
        {
            TABULATION = '\t',      // (int)0x09,
            LINE_FEED = '\n',       // (int)0x0A,
            CARRIAGE_RETURN = '\r', // (int)0x0D,
            SPACE = ' '             // (int)0x20
        }
    }
}
