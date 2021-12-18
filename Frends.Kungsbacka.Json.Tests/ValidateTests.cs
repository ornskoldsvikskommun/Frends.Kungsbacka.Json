using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Frends.Kungsbacka.Json.Tests
{
    [TestFixture]
    class ValidateTests
    {
        const string ValidUserJson = @"{
              'name': 'Arnie Admin',
              'roles': ['Developer', 'Administrator']
            }";

        const string ValidUserSchema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'array'}
              }
            }";

        [Test]
        public void JsonShouldValidate()
        {
            var result = ValidateTask.Validate(
                new ValidateInput() { Json = ValidUserJson, Schema = ValidUserSchema },
                new ValidateOptions(),
                CancellationToken.None)
                .GetAwaiter()
                .GetResult();
            Assert.True(result.IsValid);
            Assert.True(result.Errors.Count == 0);
        }

        [Test]
        public void JsonShouldNotValidate()
        {
            const string user = @"{
              'name': 'Arnie Admin',
              'roles': ['Developer', 'Administrator']
            }";

            const string schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'object'}
              }
            }";
            var result = ValidateTask.Validate(
                new ValidateInput() { Json = user, Schema = schema },
                new ValidateOptions(),
                CancellationToken.None)
                .GetAwaiter()
                .GetResult();
            Assert.False(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [Test]
        public void InvalidJsonShouldThrow()
        {
            const string user = @"{
              name: Arnie Admin,
              roles: [Developer, Administrator]
            }";

            const string schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'object'}
              }
            }";
            var ex = Assert.Throws<JsonReaderException>(() => ValidateTask.Validate(
                new ValidateInput() { Json = user, Schema = schema },
                new ValidateOptions() { ThrowOnInvalidJson = true },
                CancellationToken.None)
                .GetAwaiter()
                .GetResult()
            );
            Assert.True(ex.Message.IContains("Unexpected character encountered while parsing value: A. Path 'name', line 2, position 20."));
        }

        [Test]
        public void InvalidJsonShouldNotValidate()
        {
            const string user = @"{
              name: Arnie Admin,
              roles: [Developer, Administrator]
            }";

            const string schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'object'}
              }
            }";
            var result = ValidateTask.Validate(
                new ValidateInput() { Json = user, Schema = schema },
                new ValidateOptions(),
                CancellationToken.None)
                .GetAwaiter()
                .GetResult();
            Assert.False(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("Unexpected character encountered while parsing value: A. Path 'name', line 2, position 20.", result.Errors.First().Property);
        }

        [Test]
        public void JsonValidationShouldThrow()
        {
            const string user = @"{
              'name': 'Arnie Admin',
              'roles': ['Developer', 'Administrator']
            }";

            const string schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'object'}
              }
            }";
            var ex = Assert.Throws<JsonException>(() => ValidateTask.Validate(
                new ValidateInput() { Json = user, Schema = schema },
                new ValidateOptions() { ThrowOnInvalidJson = true },
                CancellationToken.None)
                .GetAwaiter()
                .GetResult()
            );
            Assert.True(ex.Message.IContains("Json is not valid. ObjectExpected: #/roles"));
        }
    }
}
