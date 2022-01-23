using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;

namespace Frends.Kungsbacka.Json.Tests
{

    [TestFixture]
    class MapTransformationsTests
    {
        readonly JObject sourceObject = JObject.Parse(@"{
            ""lower"": ""value"",
            ""upper"": ""VALUE"",
            ""valueWithSpaces"": ""   value   "",
            ""shortNum"": 1234567890,
            ""longNum"": 123456789012,
            ""shortDashedNum"" : ""123456-7890"",
            ""longDashedNum"" : ""12345678-9012""
        }");

        [Test]
        public void Trim()
        {
            string map = @"[
                {""from"": ""valueWithSpaces"", ""to"": ""trimmed"", ""trans"": [""Trim""]}
            ]";
            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual("value", (string)result.trimmed);
        }

        [Test]
        public void LCase()
        {
            string map = @"[
                {""from"": ""upper"", ""to"": ""lower"", ""trans"": [""LCase""]}
            ]";
            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual("value", (string)result.lower);
        }

        [Test]
        public void UCase()
        {
            string map = @"[
                {""from"": ""lower"", ""to"": ""upper"", ""trans"": [""UCase""]}
            ]";
            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual("VALUE", (string)result.upper);
        }

        [Test]
        public void SweSsn()
        {
            string map = @"[
                {""from"": ""shortNum"",       ""to"": ""sweSsn1"", ""trans"": [""SweSsn""]},
                {""from"": ""shortDashedNum"", ""to"": ""sweSsn2"", ""trans"": [""SweSsn""]},
                {""from"": ""longNum"",        ""to"": ""sweSsn3"", ""trans"": [""SweSsn""]},
                {""from"": ""longDashedNum"",  ""to"": ""sweSsn4"", ""trans"": [""SweSsn""]}
            ]";
            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual("123456-7890", (string)result.sweSsn1);
            Assert.AreEqual("123456-7890", (string)result.sweSsn2);
            Assert.AreEqual("12345678-9012", (string)result.sweSsn3);
            Assert.AreEqual("12345678-9012", (string)result.sweSsn4);
        }

        [Test]
        public void SweOrgNum()
        {
            string map = @"[
                {""from"": ""shortNum"",       ""to"": ""sweOrgNum1"", ""trans"": [""SweOrgNum""]},
                {""from"": ""shortDashedNum"", ""to"": ""sweOrgNum2"", ""trans"": [""SweOrgNum""]},
                {""from"": ""longNum"",        ""to"": ""sweOrgNum3"", ""trans"": [""SweOrgNum""]},
                {""from"": ""longDashedNum"",  ""to"": ""sweOrgNum4"", ""trans"": [""SweOrgNum""]}
            ]";
            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual("16123456-7890", (string)result.sweOrgNum1);
            Assert.AreEqual("16123456-7890", (string)result.sweOrgNum2);
            Assert.AreEqual("12345678-9012", (string)result.sweOrgNum3);
            Assert.AreEqual("12345678-9012", (string)result.sweOrgNum4);
        }
    }
}
