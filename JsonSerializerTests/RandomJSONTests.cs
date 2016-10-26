using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonSerializer;
using System.IO;

namespace JsonSerializerTests
{
    public class _GlossDef
    {
        public string para { get; set; }

        public string[] GlossSeeAlso { get; set; }

        public _GlossDef() { }
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

        [JsonSerializable(2)]
        public string GlossSee { get; set; }

        public _GlossEntry() { }
    }

    public class _GlossList
    {
        [JsonSerializable]
        public _GlossEntry[] GlossEntry { get; set; }
        
        public _GlossList() { }
    }

    public class _GlossDiv
    {
        [JsonSerializable]
        public string title { get; set; }

        [JsonSerializable]
        public _GlossList GlossList { get; set; }

        public _GlossDiv() { }
    }

    public class _Glossary
    {
        [JsonSerializable]
        public string title { get; set; }

        [JsonSerializable]
        public _GlossDiv GlossDiv { get; set; }

        public _Glossary() { }
    }

    [TestClass]
    public class RandomJSONTests
    {
        [TestMethod]
        public void TestMethod1()
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

            _Glossary Glossary = new _Glossary();
            Glossary.GlossDiv = GlossDiv;
            Glossary.title = "example glossary";

            string actual;
            using (var stream = new MemoryStream(new byte[16 * 1024], true))
            {
                Serializer.Serialize(stream, Glossary);

                stream.Position = 0;
                var sr = new StreamReader(stream);
                actual = sr.ReadToEnd();
            }

            Assert.AreEqual("{\"title\":\"example glossary\",\"GlossDiv\":{\"title\":\"S\",\"GlossList\":{\"GlossEntry\":[{\"ID\":\"SGML\",\"SortAs\":\"SGML\",\"GlossTerm\":\"Standard Generalized Markup Language\",\"Acronym\":\"SGML\",\"Abbrev\":\"ISO 8879:1986\",\"GlossDef\":{\"para\":\"A meta-markup language, used to create markup languages such as DocBook.\",\"GlossSeeAlso\":[\"GML\",\"XML\"]},\"GlossSee\":\"markup\"}]}}}", actual);
        }
    }
}
