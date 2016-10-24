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
            object result = Parser.ReadValue(
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
                ((Hashtable)((Hashtable)result)[5])["string"]);
        }

        [TestMethod]
        public void SingleStringTest()
        {
            string json_string = "\"\"";
            int position = 0;

            object result = Parser.ReadValue(
                new UTF8Encoding().GetBytes(json_string),
                ref position);
        }

        [TestMethod]
        public void SingleIntTest()
        {
            string json_string = "0";
            int position = 0;

            object result = Parser.ReadValue(
                new UTF8Encoding().GetBytes(json_string),
                ref position);
        }

        [TestMethod]
        public void SingleFloatTest()
        {
            string json_string = "0.1";
            int position = 0;

            object result = Parser.ReadValue(
                new UTF8Encoding().GetBytes(json_string),
                ref position);
        }

        [TestMethod]
        public void SingleObjectTest()
        {
            string json_string = "{}";
            int position = 0;

            object result = Parser.ReadValue(
                new UTF8Encoding().GetBytes(json_string),
                ref position);
        }

        [TestMethod]
        public void SingleArrayTest()
        {
            string json_string = "[]";
            int position = 0;

            object result = Parser.ReadValue(
                new UTF8Encoding().GetBytes(json_string),
                ref position);
        }

        [TestMethod]
        public void SingleBooleanTest()
        {
            string json_string = "true";
            int position = 0;

            object result = Parser.ReadValue(
                new UTF8Encoding().GetBytes(json_string),
                ref position);


            json_string = "false";
            position = 0;

            result = Parser.ReadValue(
                new UTF8Encoding().GetBytes(json_string),
                ref position);
        }

        [TestMethod]
        public void SingleNullTest()
        {
            string json_string = "null";
            int position = 0;

            object result = Parser.ReadValue(
                new UTF8Encoding().GetBytes(json_string),
                ref position);
        }

        [TestMethod]
        public void EscapedStringCharactersTest()
        {
            string json_string = "{\"name\":\" \\\" \"}";
            int position = 0;

            object result = Parser.ReadValue(
                new UTF8Encoding().GetBytes(json_string),
                ref position);

            Assert.AreEqual(" \\\" ", ((Hashtable)result)["name"]);
        }
    }
}
