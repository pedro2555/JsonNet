using JsonSerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.IO;

namespace JsonSerializerTests
{
    [TestClass]
    public class GlossaryTestCase
    {
        #region Glossary Test Case

        #region Definitions

        public class _GlossDef
        {
            public string para { get; set; }

            public string[] GlossSeeAlso { get; set; }

            public _GlossDef() { }

            public _GlossDef(string para, string[] GlossSeeAlso)
            {
                this.para = para;
                this.GlossSeeAlso = GlossSeeAlso;
            }

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

            public _GlossEntry(
                string ID,
                string SortAs,
                string GlossTerm,
                string Acronym,
                string Abbrev,
                _GlossDef GlossDef,
                string GlossSee)
            {
                this.ID = ID;
                this.SortAs = SortAs;
                this.GlossTerm = GlossTerm;
                this.Acronym = Acronym;
                this.Abbrev = Abbrev;
                this.GlossDef = GlossDef;
                this.GlossSee = GlossSee;
            }

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

            public _GlossList(_GlossEntry[] GlossEntry)
            {
                this.GlossEntry = GlossEntry;
            }

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

            public _GlossDiv(string title, _GlossList GlossList)
            {
                this.title = title;
                this.GlossList = GlossList;
            }

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

            public _Glossary(string title, _GlossDiv GlossDiv)
            {
                this.title = title;
                this.GlossDiv = GlossDiv;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                _Glossary _glossary = obj as _Glossary;

                return (_glossary.title == title)
                    && (_glossary.GlossDiv.Equals(GlossDiv));
            }
        }

        #endregion Definition

        #region Subjects and Controls

        private static _Glossary Glossary_Class = new _Glossary(
            "example glossary",
            new _GlossDiv(
                "S",
                new _GlossList(
                    new _GlossEntry[] {
                        new _GlossEntry(
                            "SGML",
                            "SGML",
                            "Standard Generalized Markup Language",
                            "SGML",
                            "ISO 8879:1986",
                            new _GlossDef(
                                "A meta-markup language, used to create markup languages such as DocBook.",
                                new string [] { "GML", "XML" }),
                            "markup")})));

        private static Hashtable Glossary_Hashtable = new Hashtable()
        {
            { "title", "example glossary" },
            { "GlossDiv", new Hashtable()
            {
                { "title", "S" },
                { "GlossList", new Hashtable()
                {
                    { "GlossEntry", new Hashtable[]
                    {
                        new Hashtable()
                        {
                            { "Abbrev", "ISO 8879:1986" },
                            { "Acronym", "SGML" },
                            { "GlossTerm", "Standard Generalized Markup Language" },
                            { "ID", "SGML" },
                            { "SortAs", "SGML" },
                            { "GlossDef", new Hashtable()
                            {
                                { "para", "A meta-markup language, used to create markup languages such as DocBook." },
                                { "GlossSeeAlso", new string[]
                                {
                                    "GML",
                                    "XML"
                                }
                                }
                            }
                            }
                        }
                    }
                    }
                }
                }
            }
            }
        };

        private static _Glossary expected_GlossaryTestCaseDeserializeClass = Glossary_Class;

        #endregion Subjects and Controls

        #region Cases

        // serialize from class instance
        [TestMethod]
        public void GlossaryTestCase_Class()
        {
            _Glossary actual;

            using (var stream = new MemoryStream(new byte[16 * 1024], true))
            {
                Serializer serializer = new Serializer();
                serializer.Serialize(stream, Glossary_Class);

                stream.Position = 0;
                actual = serializer.Deserialize<_Glossary>(stream);
            }

            Assert.AreEqual(Glossary_Class, actual);
        }

        // serialize from Hashtable
        [TestMethod]
        public void GlossaryTestCase_Hashtable()
        {
            Hashtable actual;

            using (var stream = new MemoryStream(new byte[16 * 1024], true))
            {
                Serializer serializer = new Serializer();
                serializer.Serialize(stream, Glossary_Hashtable);

                stream.Position = 0;
                actual = (Hashtable)serializer.Deserialize(stream);
            }
            
            // Comparing the count maybe enough (? TODO: compare every single value)
            Assert.AreEqual(
                Glossary_Hashtable.Count,
                actual.Count);

            Hashtable Glossary_Hashtable_GlossDiv = (Hashtable)Glossary_Hashtable["GlossDiv"];
            Hashtable actual_GlossDiv = (Hashtable)actual["GlossDiv"];
            Assert.AreEqual(
                Glossary_Hashtable_GlossDiv.Count,
                actual_GlossDiv.Count);

            Hashtable Glossary_Hashtable_GlossList = (Hashtable)Glossary_Hashtable_GlossDiv["GlossList"];
            Hashtable actual_GlossList = (Hashtable)actual_GlossDiv["GlossList"];
            Assert.AreEqual(
                Glossary_Hashtable_GlossList.Count,
                actual_GlossList.Count);

            object[] Glossary_Hashtable_GlossEntries = (object[])Glossary_Hashtable_GlossList["GlossEntry"];
            object[] actual_GlossEntries = (object[])actual_GlossList["GlossEntry"];
            Assert.AreEqual(
                Glossary_Hashtable_GlossEntries.Length,
                actual_GlossEntries.Length);

            Hashtable Glossary_Hashtable_GlossEntry = (Hashtable)Glossary_Hashtable_GlossEntries[0];
            Hashtable actual_GlossEntry = (Hashtable)actual_GlossEntries[0];
            Assert.AreEqual(
                Glossary_Hashtable_GlossEntry.Count,
                actual_GlossEntry.Count);

            Hashtable Glossary_Hashtable_GlossDef = (Hashtable)Glossary_Hashtable_GlossEntry["GlossDef"];
            Hashtable actual_GlossDef = (Hashtable)actual_GlossEntry["GlossDef"];
            Assert.AreEqual(
                Glossary_Hashtable_GlossDef.Count,
                Glossary_Hashtable_GlossDef.Count);

            object[] Glossary_Hashtable_GlossSeeAlso = (object[])Glossary_Hashtable_GlossDef["GlossSeeAlso"];
            object[] actual_GlossSeeAlso = (object[])actual_GlossDef["GlossSeeAlso"];
            Assert.AreEqual(
                Glossary_Hashtable_GlossSeeAlso.Length,
                actual_GlossSeeAlso.Length);
        }

        #endregion Cases

        #endregion Glossary Test Case
    }
}
