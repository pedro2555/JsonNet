using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonSerializer
{
    public class Serializer
    {
        private static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            var attributeType = typeof(JsonSerializable);

            if (Attribute.GetCustomAttribute(type, attributeType) != null)
                return type.GetProperties();
            else
                return type
                    .GetProperties()
                    .Where(
                        prop => prop.GetCustomAttributes(
                            attributeType,
                            true).Any());
        }

        public static void Serialize(object obj, Stream target)
        {
            var properties = GetProperties(obj.GetType());

            using (var writer = new StreamWriter(target))
            {
                var attributeType = typeof(JsonSerializable);
                foreach (var propertyInfo in properties)
                {
                    var name = propertyInfo.Name;
                    var value = propertyInfo.GetValue(obj, null).ToString();

                    ///
                    /// TODO: Pass value to proper parser before writing
                    ///
                    writer.Write(value);
                }
            }
        }
    }
}
