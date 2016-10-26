using JsonSerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace JsonSerializerTests
{
    [TestClass]
    public class SerializerTests
    {
        [JsonSerializable]
        public class DriverLicense
        {
            public string Category { get; set; }
            public int ValidUntilYear { get; set; }

            public DriverLicense() { }

            public DriverLicense(string category, int validUntilYear)
            {
                this.Category = category;
                this.ValidUntilYear = validUntilYear;
            }
        }
        [JsonSerializable]
        public class MyClass
        {
            public string Property1 { get; set; }

            public int Property2 { get; set; }

            public DriverLicense License { get; set; }

            public MyClass()
            {

            }

            public MyClass(string property1, int property2)
            {
                this.Property1 = property1;
                this.Property2 = property2;

                this.License = new DriverLicense("B", 2040);
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
            MemoryStream stream;
            MyClass expected, actual;

            // Serialize an object
            using (stream = new MemoryStream(new byte[16 * 1024], true))
            {
                expected = new MyClass("Pedro", 26);
                Serializer.Serialize(stream, expected);

                stream.Seek(0, SeekOrigin.Begin);

                actual = Serializer.Deserialize<MyClass>(stream);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
