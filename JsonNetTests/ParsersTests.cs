using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonNet;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections;

namespace JsonNetTests
{
    [TestClass]
    public class ParsersTests
    {
        [TestMethod]
        public void ParseObjectTest()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[ \"strings\", -12.35654,345 ,\r\n \t-234,12.3,{\"string\":\"value\",\"number\":-12.35654},[],true,false,null,]");

            int position = 0;
            object result = Parsers.ReadValue(
                new UTF8Encoding().GetBytes(sb.ToString()),
                ref position);

            Assert.AreEqual(
                "strings",
                ((Hashtable)result)[0]);
            Assert.AreEqual(
                -12.35654f,
                ((Hashtable)result)[1]);
            Assert.AreEqual(
                345,
                ((Hashtable)result)[2]);
            Assert.AreEqual(
                -234,
                ((Hashtable)result)[3]);
            Assert.AreEqual(
                12.3f,
                ((Hashtable)result)[4]);
            Assert.AreEqual(
                "value",
                ((Hashtable)result)[5]["string"]);
        }
    }
}
