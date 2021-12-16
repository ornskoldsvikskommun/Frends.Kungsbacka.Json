using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Frends.Kungsbacka.Json.Tests
{
    [TestFixture]
    class TestClass
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

        [Test]
        public void HandlebarShouldGenerateTemplate()
        {
            const string json = @"{'title':'Mr.', 'name':'Andersson'}";
            const string template =
            @"<div><span>{{title}}</span> <strong>{{name}}</strong></div>";
            var result = JsonTasks.Handlebars(new HandlebarsInput() { Json = json, HandlebarsTemplate = template, HandlebarsPartials = new HandlebarsPartial[0] }, options: null);
            Assert.True(result.IContains("<span>Mr.</span> <strong>Andersson</strong>"));
        }

        [Test]
        public void HandlebarShouldGeneratePartials()
        {
            const string json = @"{'title':'Mr.', 'name':'Andersson'}";
            const string template =
            @"<div><span>{{title}}</span> {{> strongName}}</div>";
            var partials = new[] { new HandlebarsPartial { Template = "<strong>{{name}}</strong>", TemplateName = "strongName" } };
            var result = JsonTasks.Handlebars(new HandlebarsInput() { Json = json, HandlebarsTemplate = template, HandlebarsPartials = partials }, options: null);
            Assert.True(result.IContains("<span>Mr.</span> <strong>Andersson</strong>"));
        }

        [Test]
        public void HandlebarsWithHelperTest()
        {
            var helper = new HandlebarsHelper()
            {
                HelperName = "filterStore",
                HelperAction = new Action<System.IO.TextWriter, dynamic, object[]>((writer, context, parameters) =>
                {
                    if (parameters != null && parameters.Length == 1)
                    {
                        if (!((JToken)parameters[0]).Value<string>().Equals("Lambton Quay"))
                        {
                            writer.Write(parameters[0]);
                        }
                    }                    
                })
            };
            var input = new HandlebarsInput()
            {
                Json = jsonString,
                HandlebarsTemplate = "{{#each Stores}}{{filterStore .}}{{/each}}",
                HandlebarsHelpers = new HandlebarsHelper[] { helper }
            };
            var result = JsonTasks.Handlebars(input, null);
            Assert.AreEqual("Willis Street", result);
        }

        [Test]
        public void HandlebarsWithBuiltInExistsHelperTest()
        {
            var input = new HandlebarsInput()
            {
                Json = @"{
                    ""Products"": [
                        {
                            ""Name"": ""Anvil"",
                            ""Price"": 250
                        }
                    ]
                }",
                HandlebarsTemplate = "{{#each Products}}Product: {{Name}}{{#exists Quantity}}, Quantity: {{Quantity}}{{/exists}}, Price: {{Price}}{{/each}}"
            };
            var result = JsonTasks.Handlebars(input, null);
            Assert.AreEqual("Product: Anvil, Price: 250", result);
        }
    }
}
