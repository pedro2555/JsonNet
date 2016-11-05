using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using JsonSerializer;

namespace JsonSerializerTests
{
    public enum State
    {
        On,
        Off
    }

    public class Stateable
    {
        [JsonSerializable]
        public State State
        { get; private set; }

        public Stateable() { }

        public Stateable(State state)
        {
            this.State = state;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Stateable _obj = obj as Stateable;

            if (_obj.State != State)
                return false;

            return true;
        }
    }

    [TestClass]
    public class EnumTestCase
    {
        [TestMethod]
        public void ClassWithEnumCase()
        {
            Stateable expected = new Stateable(State.Off);

            //using (var stream = new MemoryStream(new byte[16 * 1024], true))
            using (var stream = new FileStream("test.json", FileMode.Create))
            {
                Serializer serializer = new Serializer();
                serializer.Serialize(stream, expected);

                stream.Position = 0;
                Stateable actual = serializer.Deserialize<Stateable>(stream);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void SingeEnumCase()
        {
            State expected = State.Off;

            //using (var stream = new MemoryStream(new byte[16 * 1024], true))
            using (var stream = new FileStream("test.json", FileMode.Create))
            {
                Serializer serializer = new Serializer();
                serializer.Serialize(stream, expected);

                stream.Position = 0;
                State actual = serializer.Deserialize<State>(stream);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
