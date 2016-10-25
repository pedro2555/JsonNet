using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JsonSerializer
{
    public static class JsonComposer
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

        private static void ComposeString(Stream target, object obj)
        {
            WriteToStream(target, String.Format("\"{0}\"", (string)obj));
        }

        private static void ComposeInt(Stream target, object obj)
        {
            WriteToStream(target, (int)obj);
        }

        private static void ComposeFloat(Stream target, object obj)
        {
            WriteToStream(target, (float)obj);
        }

        private static void ComposeNull(Stream target)
        {
            WriteToStream(target, "null");
        }

        private static void ComposeBool(Stream target, object obj)
        {
            WriteToStream(target, (bool)obj);
        }

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

        private static void ComposeArray(Stream target, object obj)
        {
            bool firstEntryFlag = true;

            WriteToStream(target, (firstEntryFlag) ? '[' : ',');
            firstEntryFlag = false;

            foreach (object o_obj in (object[])obj)
                ComposeValue(target, o_obj);

            WriteToStream(target, ']');
        }

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

        private static void WriteToStream(Stream target, object obj)
        {
            using (var writer = new StreamWriter(
                target,
                Encoding.UTF8,
                16*1024,
                true))
                writer.Write(obj);
        }

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
