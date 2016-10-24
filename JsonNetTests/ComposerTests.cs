using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonNet;
using System.Collections;

namespace JsonNetTests
{
    [TestClass]
    public class ComposerTests
    {
        [TestMethod]
        public void ComposeValueTest()
        {
            Hashtable[] test = new Hashtable[2];

            test[0] = new Hashtable();
            test[0].Add("Name", "Pedro Rodrigues");
            test[0].Add("Gender", "Male");
            test[0].Add("Height", 165);

            test[1] = new Hashtable();
            test[1].Add("Name", "Joana Marques");
            test[1].Add("Gender", "Female");
            test[1].Add("Height", 162);

            string result = Composer.ComposeValue(test);

            Assert.AreEqual("[{\"Name\":\"Pedro Rodrigues\",\"Height\":165,\"Gender\":\"Male\"},{\"Name\":\"Joana Marques\",\"Height\":162,\"Gender\":\"Female\"}]", result);
        }

        [TestMethod]
        public void ComposeSingleStringTest()
        {
            string test = "teste";

            string result = Composer.ComposeValue(test);

            Assert.AreEqual("\"" + test + "\"", result);
        }

        [TestMethod]
        public void ComposeSingleNullTest()
        {
            string result = Composer.ComposeValue(null);

            Assert.AreEqual("null", result);
        }

        [TestMethod]
        public void ComposeSingleBooleanTest()
        {
            bool test = true;

            string result = Composer.ComposeValue(test);

            Assert.AreEqual("true", result);
        }

        [TestMethod]
        public void ComposeSingleArrayTest()
        {
            string[] test = new string[0];

            string result = Composer.ComposeValue(test);

            Assert.AreEqual("[]", result);
        }

        [TestMethod]
        public void ComposeSingleObjectTest()
        {
            Hashtable test = new Hashtable();

            string result = Composer.ComposeValue(test);

            Assert.AreEqual("{}", result);
        }
    }
}
