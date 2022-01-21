using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Frends.Kungsbacka.Json.Tests
{
    [TestFixture]
    class QueryTests
    {
        private const string jsonString = @"{
            'Stores': [
            'Lambton Quay',
            'Willis Street'
            ],
            'Manufacturers': [
            {
                'Name': 'Acme Co',
                'Products': [
                {
                    'Name': 'Anvil',
                    'Price': 50
                }
                ]
            },
            {
                'Name': 'Contoso',
                'Products': [
                {
                    'Name': 'Elbow Grease',
                    'Price': 99.95
                },
                {
                    'Name': 'Headlight Fluid',
                    'Price': 4
                }
                ]
            }
            ]
        }";

        public class DeserializableObject
        {
            public override string ToString()
            {
                return jsonString;
            }
        }

        [Test]
        public void QueryShouldWorkWithStringInput()
        {
            const string query = "$..Products[?(@.Price >= 50)].Name";
            var result = (IEnumerable<JToken>)QueryTask.Query(new QueryInput() { Json = jsonString, Query = query }, new QueryOptions());
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Anvil", result.First().Value<string>());
        }

        [Test]
        public void QueryShouldWorkWithJTokenInput()
        {
            const string query = "$..Products[?(@.Price >= 50)].Name";
            var jtoken = JToken.Parse(jsonString);
            var result = (IEnumerable<JToken>)QueryTask.Query(new QueryInput() { Json = jtoken, Query = query }, new QueryOptions());
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Anvil", result.First().Value<string>());
        }

        [Test]
        public void QueryShouldWorkWithDeserializableObjectInput()
        {
            const string query = "$..Products[?(@.Price >= 50)].Name";
            var result = (IEnumerable<JToken>)QueryTask.Query(new QueryInput() { Json = new DeserializableObject(), Query = query }, new QueryOptions());
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Anvil", result.First().Value<string>());
        }

        [Test]
        public void TestQuerySingle()
        {
            const string query = "$.Manufacturers[?(@.Name == 'Acme Co')]";
            var result = (JToken)QueryTask.QuerySingle(new QueryInput() { Json = jsonString, Query = query }, new QueryOptions());
            Assert.AreEqual("Acme Co", result["Name"].Value<string>());
        }

        [Test]
        public void QueryShouldThrowIfOptionSetAndNothingIsFound()
        {
            const string query = "$.Manufacturer[?(@.Name == 'Acme Co')]";
            var ex = Assert.Throws<JsonException>(() => QueryTask.QuerySingle(new QueryInput() { Json = jsonString, Query = query }, new QueryOptions() { ErrorWhenNotMatched = true }));
            Assert.True(ex.Message.IContains("Property 'Manufacturer' does not exist on JObject."));
        }

        [Test]
        public void QuerySingleShouldNotThrowIfOptionNotSetAndNothingIsFound()
        {
            const string query = "$.Manufacturer[?(@.Name == 'Acme Co')]";
            var result = QueryTask.QuerySingle(new QueryInput() { Json = jsonString, Query = query }, new QueryOptions() { ErrorWhenNotMatched = false });
            Assert.Null(result);
        }

        [Test]
        public void QueryShouldNotThrowIfOptionNotSetAndNothingIsFound()
        {
            const string query = "$..Product[?(@.Price >= 50)].Name";
            var result = QueryTask.Query(new QueryInput() { Json = jsonString, Query = query }, new QueryOptions() { ErrorWhenNotMatched = false });
            Assert.AreEqual(result, Enumerable.Empty<object>());
        }
    }
}
