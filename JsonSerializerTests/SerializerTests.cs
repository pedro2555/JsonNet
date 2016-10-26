using JsonSerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

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
            using (var stream = new MemoryStream(new byte[16*1024], true))
            {
                MyClass Person = new MyClass("Pedro", 26);
                Serializer.Serialize(stream, Person);

                stream.Position = 0;
                var sr = new StreamReader(stream);
                result = sr.ReadToEnd();
            }
            Assert.AreEqual(
                "{\"Property1\":\"Pedro\",\"Property2\":26}",
                result);

            //// Deserialzie it from the file
            //MyObject readFromFile = null;
            //using (var stream = File.OpenRead("C:\\temp.dat"))
            //{
            //    readFromFile = Serializer.Deserialize<MyObject>(stream);
            //}
        }

        [TestMethod]
        public void DeserializeTest()
        {
            // Serialize an object
            using (var stream = new MemoryStream(new byte[16 * 1024], true))
            {
                MyClass Person = new MyClass("Pedro", 26);
                Serializer.Serialize(stream, Person);

                stream.Seek(0, SeekOrigin.Begin);

                var obj = Serializer.ReadValue(stream);
            }
        }
    }
}
