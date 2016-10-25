using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JsonSerializer
{
    public class Serializer
    {
        /// <summary>
        /// Serializes an object, or graph of connected objects, to the given
        /// stream.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="target"></param>
        public static void Serialize(Stream target, object obj)
        {
            // Get serialzable properties
            var properties = GetProperties(obj.GetType());

            var attributeType = typeof(JsonSerializable);
            foreach (var propertyInfo in properties)
            {
                var name = propertyInfo.Name;
                var value = propertyInfo.GetValue(obj, null).ToString();

                ///
                /// TODO: Pass value to proper parser before writing
                ///
                JsonComposer.ComposeValue(target, value);
            }
        }


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
    }
}
