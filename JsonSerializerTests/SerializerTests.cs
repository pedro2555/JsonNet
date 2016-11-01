using JsonSerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace JsonSerializerTests
{
    [TestClass]
    public class SerializerTests
    {
        public class _GlossDef
        {
            public string para { get; set; }

            public string[] GlossSeeAlso { get; set; }

            public _GlossDef() { }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                _GlossDef _glossDef = obj as _GlossDef;

                if (_glossDef.para != para)
                    return false;

                int i = 0;
                foreach (string s in _glossDef.GlossSeeAlso)
                    if (s != GlossSeeAlso[i++])
                        return false;

                return true;
            }
        }

        public class _GlossEntry
        {
            [JsonSerializable]
            public string ID { get; set; }

            [JsonSerializable]
            public string SortAs { get; set; }

            [JsonSerializable]
            public string GlossTerm { get; set; }

            [JsonSerializable]
            public string Acronym { get; set; }

            [JsonSerializable]
            public string Abbrev { get; set; }

            [JsonSerializable]
            public _GlossDef GlossDef { get; set; }

            [JsonSerializable]
            public string GlossSee { get; set; }

            public _GlossEntry() { }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                _GlossEntry _glossEntry = obj as _GlossEntry;

                return (_glossEntry.ID == ID)
                    && (_glossEntry.SortAs == SortAs)
                    && (_glossEntry.GlossTerm == GlossTerm)
                    && (_glossEntry.Acronym == Acronym)
                    && (_glossEntry.Abbrev == Abbrev)
                    && (_glossEntry.GlossDef.Equals(GlossDef))
                    && (_glossEntry.GlossSee == GlossSee);
            }
        }

        public class _GlossList
        {
            [JsonSerializable]
            public _GlossEntry[] GlossEntry { get; set; }

            public _GlossList() { }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                _GlossList _glossList = obj as _GlossList;

                if (_glossList.GlossEntry.Length != GlossEntry.Length)
                    return false;

                int i = 0;
                foreach (_GlossEntry _glossEntry in _glossList.GlossEntry)
                    if (!_glossEntry.Equals(GlossEntry[i++]))
                        return false;

                return true;
            }
        }

        public class _GlossDiv
        {
            [JsonSerializable]
            public string title { get; set; }

            [JsonSerializable]
            public _GlossList GlossList { get; set; }

            public _GlossDiv() { }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                _GlossDiv _glossDiv = obj as _GlossDiv;

                return (_glossDiv.title == title)
                    && (_glossDiv.GlossList.Equals(GlossList));
            }
        }

        public class _Glossary
        {
            [JsonSerializable]
            public string title { get; set; }

            [JsonSerializable]
            public _GlossDiv GlossDiv { get; set; }

            public _Glossary() { }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                _Glossary _glossary = obj as _Glossary;

                return (_glossary.title == title)
                    && (_glossary.GlossDiv.Equals(GlossDiv));
            }
        }

        [TestMethod]
        public void SerializeTest()
        {
            string actual;
            string expected = "{\"title\":\"example glossary\",\"GlossDiv\":{\"title\":\"S\",\"GlossList\":{\"GlossEntry\":[{\"ID\":\"SGML\",\"SortAs\":\"SGML\",\"GlossTerm\":\"Standard Generalized Markup Language\",\"Acronym\":\"SGML\",\"Abbrev\":\"ISO 8879:1986\",\"GlossDef\":{\"para\":\"A meta-markup language, used to create markup languages such as DocBook.\",\"GlossSeeAlso\":[\"GML\",\"XML\"]},\"GlossSee\":\"markup\"}]}}}";

            _GlossDef GlossDef = new _GlossDef();
            GlossDef.para = "A meta-markup language, used to create markup languages such as DocBook.";
            GlossDef.GlossSeeAlso = new string[] { "GML", "XML" };

            _GlossEntry GlossEntry = new _GlossEntry();
            GlossEntry.Abbrev = "ISO 8879:1986";
            GlossEntry.Acronym = "SGML";
            GlossEntry.GlossDef = GlossDef;
            GlossEntry.GlossSee = "markup";
            GlossEntry.GlossTerm = "Standard Generalized Markup Language";
            GlossEntry.ID = "SGML";
            GlossEntry.SortAs = "SGML";

            _GlossList GlossList = new _GlossList();
            GlossList.GlossEntry = new _GlossEntry[] { GlossEntry };

            _GlossDiv GlossDiv = new _GlossDiv();
            GlossDiv.title = "S";
            GlossDiv.GlossList = GlossList;

            _Glossary Glossary = new _Glossary();
            Glossary.GlossDiv = GlossDiv;
            Glossary.title = "example glossary";

            using (var stream = new MemoryStream(new byte[16 * 1024], true))
            {
                Serializer.Serialize(stream, Glossary);

                stream.Position = 0;
                var sr = new StreamReader(stream);
                actual = sr.ReadToEnd();
            }

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DeserializeTest()
        {
            _GlossDef GlossDef = new _GlossDef();
            GlossDef.para = "A meta-markup language, used to create markup languages such as DocBook.";
            GlossDef.GlossSeeAlso = new string[] { "GML", "XML" };

            _GlossEntry GlossEntry = new _GlossEntry();
            GlossEntry.Abbrev = "ISO 8879:1986";
            GlossEntry.Acronym = "SGML";
            GlossEntry.GlossDef = GlossDef;
            GlossEntry.GlossSee = "markup";
            GlossEntry.GlossTerm = "Standard Generalized Markup Language";
            GlossEntry.ID = "SGML";
            GlossEntry.SortAs = "SGML";

            _GlossList GlossList = new _GlossList();
            GlossList.GlossEntry = new _GlossEntry[] { GlossEntry };

            _GlossDiv GlossDiv = new _GlossDiv();
            GlossDiv.title = "S";
            GlossDiv.GlossList = GlossList;

            _Glossary expected = new _Glossary();
            expected.GlossDiv = GlossDiv;
            expected.title = "example glossary";

            _Glossary actual;
            string expected_str = "{\"title\":\"example glossary\",\"GlossDiv\":{\"title\":\"S\",\"GlossList\":{\"GlossEntry\":[{\"ID\":\"SGML\",\"SortAs\":\"SGML\",\"GlossTerm\":\"Standard Generalized Markup Language\",\"Acronym\":\"SGML\",\"Abbrev\":\"ISO 8879:1986\",\"GlossDef\":{\"para\":\"A meta-markup language, used to create markup languages such as DocBook.\",\"GlossSeeAlso\":[\"GML\",\"XML\"]},\"GlossSee\":\"markup\"}]}}}";

            using (
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(expected_str)))
            {
                actual = (_Glossary)Serializer.Deserialize<_Glossary>(stream);
            }

            Assert.AreEqual(expected, actual);
        }
    }
}
