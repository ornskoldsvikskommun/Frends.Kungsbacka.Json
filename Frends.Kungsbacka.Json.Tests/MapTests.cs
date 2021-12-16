using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Frends.Kungsbacka.Json.Tests
{
    
    [TestFixture]
    class MapTests
    {
        readonly JObject sourceObject = JObject.Parse(@"{
            ""firstname"": ""John"",
            ""lastname"": ""Doe"",
            ""ssn"": 1234567890,
            ""longSsn"": 123456789012,
            ""valueWithSpaces"": ""   value   "",
            ""$propWithDollarSign"": ""value"",
            ""nestedObject"": {
                ""prop"": ""value""
            },
            ""propWithCdata"": {
                ""#cdata-section"": ""value""
            }
        }");

        [Test]
        public void TestWithoutMap()
        {
            Assert.Throws(typeof(ArgumentException), () => JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = null }, null));
        }

        [Test]
        public void TestWithoutSourceObject()
        {
            string map = @"[
                {""from"": ""firstname"", ""to"": ""givenname""},
                {""from"": ""lastname"", ""to"": ""surname""}
            ]";

            Assert.Throws(typeof(ArgumentNullException), () => JsonTasks.Map(new MapInput() { SourceObject = null, DestinationObject = null, Map = map }, null));
        }

        [Test]
        public void TestWithoutDestinationObject()
        {
            string map = @"[]";

            Assert.DoesNotThrow(() => JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null));
        }

        [Test]
        public void TestWithFromMissing()
        {
            string map = @"[
                {""to"": ""givenname""}
            ]";

            Assert.Throws(typeof(JsonSerializationException), () => JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null));
        }

        [Test]
        public void TestSimpelMap()
        {
            string map = @"[
                {""from"": ""firstname"", ""to"": ""givenname""},
                {""from"": ""lastname"", ""to"": ""surname""}
            ]";

            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual("John", (string)result.givenname);
            Assert.AreEqual("Doe", (string)result.surname);
        }

        [Test]
        public void TestMapWithDefault()
        {
            string map = @"[
                {""from"": ""scrore"", ""to"": ""points"", ""def"": 5}
            ]";

            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual(5, (int)result.points);
        }

        [Test]
        public void TestMapWithoutDefault()
        {
            string map = @"[
                {""from"": ""score"", ""to"": ""points""}
            ]";

            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.IsNull(result["points"]);
        }

        [Test]
        public void TestOverwriteDestination()
        {
            dynamic destObject = JObject.Parse(@"{
                ""givenname"": ""Jane""
            }");

            string map = @"[
                {""from"": ""firstname"", ""to"": ""givenname""}
            ]";

            Assert.AreEqual("Jane", (string)destObject.givenname);
            JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = destObject, Map = map }, null);
            Assert.AreEqual("John", (string)destObject.givenname);
        }

        [Test]
        public void TestDontOverwriteDestination()
        {
            dynamic destObject = JObject.Parse(@"{
                ""givenname"": ""Jane""
            }");

            string map = @"[
                {""from"": ""firstname"", ""to"": ""givenname*""}
            ]";
            Assert.AreEqual("Jane", (string)destObject.givenname);
            JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = destObject, Map = map }, null);
            Assert.AreEqual("Jane", (string)destObject.givenname);
        }

        [Test]
        public void TestSelectNode()
        {
            string map = @"[
                {""from"": ""$.nestedObject.prop"", ""to"": ""destProp""},
                {""from"": ""$.lastname"", ""to"": ""surname""}
            ]";

            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual("value", (string)result.destProp);
            Assert.AreEqual("Doe", (string)result.surname);
        }

        [Test]
        public void TestEscapeDollarSignInFrom()
        {
            string map = @"[
                {""from"": ""$$propWithDollarSign"", ""to"": ""destProp""}
            ]";

            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual("value", (string)result.destProp);
        }

        [Test]
        public void TestEscapeStarInPropertyName()
        {
            dynamic destObject = JObject.Parse(@"{
                ""givenname*"": ""Jane""
            }");

            string map = @"[
                {""from"": ""firstname"", ""to"": ""givenname**""}
            ]";
            Assert.AreEqual("Jane", (string)destObject["givenname*"]);
            JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = destObject, Map = map }, null);
            Assert.AreEqual("John", (string)destObject["givenname*"]);
        }

        [Test]
        public void TestMapWithTrimTransformation()
        {
            string map = @"[
                {""from"": ""valueWithSpaces"", ""to"": ""trimmed"", ""trans"": [""Trim""]}
            ]";
            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual("value", (string)result.trimmed);
        }

        [Test]
        public void TestMapWithSweSsnTransformationShortSsn()
        {
            string map = @"[
                {""from"": ""ssn"", ""to"": ""sweSsn"", ""trans"": [""SweSsn""]}
            ]";
            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual("123456-7890", (string)result.sweSsn);
        }

        [Test]
        public void TestMapWithSweSsnTransformationLongSsn()
        {
            string map = @"[
                {""from"": ""longSsn"", ""to"": ""sweSsn"", ""trans"": [""SweSsn""]}
            ]";
            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual("12345678-9012", (string)result.sweSsn);
        }

        [Test]
        public void TestUnpackCdataSection()
        {
            string map = @"[
                {""from"": ""propWithCdata"", ""to"": ""dest""}
            ]";
            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, new MapOptions { UnpackCdataSection = true });
            Assert.AreEqual("value", (string)result.dest);
        }

        [Test]
        public void TestNotUnpackCdataSection()
        {
            string map = @"[
                {""from"": ""propWithCdata"", ""to"": ""dest""}
            ]";
            dynamic result = JsonTasks.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, new MapOptions { UnpackCdataSection = false });
            Assert.IsInstanceOf(typeof(JObject), result.dest);
        }
    }
}
