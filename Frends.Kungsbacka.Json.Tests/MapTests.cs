using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;

namespace Frends.Kungsbacka.Json.Tests
{

    [TestFixture]
    class MapTests
    {
        readonly JObject sourceObject = JObject.Parse(@"{
            ""firstname"": ""John"",
            ""lastname"": ""Doe"",
            ""?propWithQuestionMark"": ""value"",
            ""nestedObject"": {
                ""prop"": ""value""
            },
            ""propWithCdata"": {
                ""#cdata-section"": ""value""
            }
        }");

        [Test]
        public void ShouldThrowIfMapIsNull()
        {
            Assert.Throws(typeof(ArgumentException), () => MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = null }, null));
        }

        [Test]
        public void ShouldThrowIfSourceObjectIsNull()
        {
            string map = @"[
                {""from"": ""firstname"", ""to"": ""givenname""},
                {""from"": ""lastname"", ""to"": ""surname""}
            ]";

            Assert.Throws(typeof(ArgumentNullException), () => MapTask.Map(new MapInput() { SourceObject = null, DestinationObject = null, Map = map }, null));
        }

        [Test]
        public void SourceAndDestinationObjectAreSame()
        {
            string map = @"[
                {""from"": ""firstname"", ""to"": ""givenname""}
            ]";

            JObject source = JObject.Parse(@"{
                ""firstname"": ""John"",            
            }");

            dynamic result = MapTask.Map(new MapInput() { SourceObject = source, DestinationObject = source, Map = map }, null);
            Assert.AreSame(source, result);
        }

        [Test]
        public void ShouldCreateNewDestinationObjectIfSourceIsNull()
        {
            string map = @"[]";

            var result = MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.IsInstanceOf<JObject>(result);
            Assert.DoesNotThrow(() => MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null));
        }

        [Test]
        public void ShouldThrowIfFromIsMissingInMap()
        {
            string map = @"[
                {""to"": ""givenname""}
            ]";

            Assert.Throws(typeof(JsonSerializationException), () => MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null));
        }

        [Test]
        public void ShouldMapProperties()
        {
            string map = @"[
                {""from"": ""firstname"", ""to"": ""givenname""},
                {""from"": ""lastname"", ""to"": ""surname""}
            ]";

            dynamic result = MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual("John", (string)result.givenname);
            Assert.AreEqual("Doe", (string)result.surname);
        }

        [Test]
        public void ShouldAssignDefaultIfSourceTokenIsNotFound()
        {
            string map = @"[
                {""from"": ""scrore"", ""to"": ""points"", ""def"": 5}
            ]";

            dynamic result = MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual(5, (int)result.points);
        }

        [Test]
        public void ShouldAssignNullIfNoDefaultIsSpecified()
        {
            string map = @"[
                {""from"": ""score"", ""to"": ""points""}
            ]";

            dynamic result = MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.IsNull(result["points"]);
        }

        [Test]
        public void ShouldOverwriteDestination()
        {
            dynamic destObject = JObject.Parse(@"{
                ""givenname"": ""Jane""
            }");

            string map = @"[
                {""from"": ""firstname"", ""to"": ""givenname""}
            ]";

            Assert.AreEqual("Jane", (string)destObject.givenname);
            MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = destObject, Map = map }, null);
            Assert.AreEqual("John", (string)destObject.givenname);
        }

        [Test]
        public void ShouldNotOverwriteDestination()
        {
            dynamic destObject = JObject.Parse(@"{
                ""givenname"": ""Jane""
            }");

            string map = @"[
                {""from"": ""firstname"", ""to"": ""givenname*""}
            ]";
            Assert.AreEqual("Jane", (string)destObject.givenname);
            MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = destObject, Map = map }, null);
            Assert.AreEqual("Jane", (string)destObject.givenname);
        }

        [Test]
        public void UseSelectTokenToSelectSource()
        {
            string map = @"[
                {""from"": ""?$.nestedObject.prop"", ""to"": ""destProp""},
                {""from"": ""?$.lastname"", ""to"": ""surname""}
            ]";

            dynamic result = MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual("value", (string)result.destProp);
            Assert.AreEqual("Doe", (string)result.surname);
        }

        [Test]
        public void EscapeQuerstionMarknInSourceName()
        {
            string map = @"[
                {""from"": ""??propWithQuestionMark"", ""to"": ""destProp""}
            ]";

            dynamic result = MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, null);
            Assert.AreEqual("value", (string)result.destProp);
        }

        [Test]
        public void EscapeExclamationMarkInDestinationName()
        {
            dynamic destObject = JObject.Parse(@"{
                ""givenname!"": ""Jane""
            }");

            string map = @"[
                {""from"": ""firstname"", ""to"": ""givenname!!""}
            ]";
            Assert.AreEqual("Jane", (string)destObject["givenname!"]);
            MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = destObject, Map = map }, null);
            Assert.AreEqual("John", (string)destObject["givenname!"]);
        }

        [Test] 
        public void UseCustomTransformation()
        {
            string map = @"[
                {""from"": ""firstname"", ""to"": ""firstname"", ""trans"": [""UCase""]}
            ]";

            var options = new MapOptions()
            {
                Tranformations = new[]
                {
                    new MapTransformation()
                    {
                        TransformationName = "UCase",
                        TransformationAction = new Func<JToken, JToken>((input) =>
                        {
                            return input?.Value<string>()?.ToUpper();
                        })
                    }
                }
            };
            dynamic result = MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, options);
            Assert.AreEqual("JOHN", (string)result.firstname);
        }

        [Test]
        public void UnpackCdataSection()
        {
            string map = @"[
                {""from"": ""propWithCdata"", ""to"": ""dest""}
            ]";
            dynamic result = MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, new MapOptions { UnpackCdataSection = true });
            Assert.AreEqual("value", (string)result.dest);
        }

        [Test]
        public void DoNotUnpackCdataSection()
        {
            string map = @"[
                {""from"": ""propWithCdata"", ""to"": ""dest""}
            ]";
            dynamic result = MapTask.Map(new MapInput() { SourceObject = sourceObject, DestinationObject = null, Map = map }, new MapOptions { UnpackCdataSection = false });
            Assert.IsInstanceOf<JObject>(result.dest);
        }
    }
}
