using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JsonSerializer
{
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
        /// Serializes an object, or graph of connected objects, to the given
        /// stream in JSON format.
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
    }
}
