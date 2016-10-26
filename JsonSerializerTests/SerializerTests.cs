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

            public override bool Equals(System.Object obj)
            {
                if (obj == null)
                    return false;

                DriverLicense o = obj as DriverLicense;
                if ((DriverLicense)o == null)
                    return false;

                return (o.Category == Category)
                    && (o.ValidUntilYear == ValidUntilYear);
            }
        }
        [JsonSerializable]
        public class Person
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public DriverLicense License { get; set; }

            public Person()
            {

            }

            public Person(string name, int age)
            {
                this.Name = name;
                this.Age = age;

                this.License = new DriverLicense("B", 2040);
            }

            public override bool Equals(System.Object obj)
            {
                if (obj == null)
                    return false;

                Person o = obj as Person;
                if ((Person)o == null)
                    return false;

                return (o.Name == Name)
                    && (o.Age == Age)
                    && (o.License.Equals(License));
            }
        }

        [TestMethod]
        public void SerializeTest()
        {
            string actual;

            using (var stream = new MemoryStream(new byte[16*1024], true))
            {
                Person p = new Person("Pedro", 26);
                Serializer.Serialize(stream, p);

                stream.Position = 0;
                var sr = new StreamReader(stream);
                actual = sr.ReadToEnd();
            }
            Assert.AreEqual(
                "{\"Name\":\"Pedro\",\"Age\":26,\"License\":{\"Category\":\"B\",\"ValidUntilYear\":2040}}",
                actual);
        }

        [TestMethod]
        public void DeserializeTest()
        {
            MemoryStream stream;
            Person expected, actual;

            using (stream = new MemoryStream(new byte[16 * 1024], true))
            {
                expected = new Person("Pedro", 26);
                Serializer.Serialize(stream, expected);

                stream.Seek(0, SeekOrigin.Begin);

                actual = Serializer.Deserialize<Person>(stream);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
