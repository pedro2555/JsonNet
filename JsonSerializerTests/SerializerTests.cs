using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonSerializer;
using System.IO;
using System.Diagnostics;

namespace JsonSerializerTests
{
    [TestClass]
    public class SerializerTests
    {
        [JsonSerializable]
        public class MyClass
        {
            public string Property1 { get; set; }
            
            public int Property2 { get; set; }

            public MyClass(string property1, int property2)
            {
                this.Property1 = property1;
                this.Property2 = property2;
            }
        }

        [TestMethod]
        public void SerializeTest()
        {
            string result;

            // Serialize an object
            using (var stream = new MemoryStream())
            {
                MyClass Person = new MyClass("Pedro", 26);
                Serializer.Serialize(Person, stream);
                result = stream.ToString();
            }
            Debug.Print(result);

            //// Deserialzie it from the file
            //MyObject readFromFile = null;
            //using (var stream = File.OpenRead("C:\\temp.dat"))
            //{
            //    readFromFile = Serializer.Deserialize<MyObject>(stream);
            //}
        }
    }
}
