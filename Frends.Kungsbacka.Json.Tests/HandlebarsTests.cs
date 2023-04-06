using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;

namespace Frends.Kungsbacka.Json.Tests
{
    [TestFixture]
    class HandlebarsTests
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
        public void HandlebarsShouldUseCustomHelper()
        {
            var helper = new HandlebarsHelper()
            {
                HelperName = "filterStore",
                HelperAction = new Action<System.IO.TextWriter, dynamic, object[]>((writer, context, arguments) =>
                {
                    if (arguments != null && arguments.Length == 1)
                    {
                        if (!((JToken)arguments[0]).Value<string>().Equals("Lambton Quay"))
                        {
                            writer.Write(arguments[0]);
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
        public void HandlebarsShouldUseCustomBlockHelper()
        {
            var helper = new HandlebarsBlockHelper()
            {
                HelperName = "htmlList",
                HelperAction = new Action<System.IO.TextWriter, dynamic, dynamic, object[]>((writer, options, context, arguments) =>
                {
                    if (arguments != null && arguments.Length == 1)
                    {
                        if (arguments[0] is JArray arr)
                        {
                            foreach (var item in arr)
                            {
                                writer.WriteLine($"<li>{item}</li>");
                            }
                        }
                    }
                })
            };
            var input = new HandlebarsInput()
            {
                Json = jsonString,
                HandlebarsTemplate = "{{#htmlList Stores}}{{.}}{{/htmlList}}",
                HandlebarsBlockHelpers = new HandlebarsBlockHelper[] { helper }
            };
            var result = JsonTasks.Handlebars(input, null);
            Assert.AreEqual($"&lt;li&gt;Lambton Quay&lt;/li&gt;{Environment.NewLine}&lt;li&gt;Willis Street&lt;/li&gt;{Environment.NewLine}", result);
        }

        [Test]
        public void HandlebarsAngleBracketsInsteadOfCurlyBraces()
        {
            var input = new HandlebarsInput()
            {
                Json = jsonString,
                HandlebarsTemplate = "[[Manufacturers.0.Products.0.Name]]"
            };
            var options = new HandlebarsOptions()
            {
                UseAngleBrackets = true
            };
            var result = JsonTasks.Handlebars(input, options);
            Assert.AreEqual("Anvil", result);
        }

        [Test]
        public void HandlebarsWithEscapedAngleBrackets()
        {
            var input = new HandlebarsInput()
            {
                Json = jsonString,
                HandlebarsTemplate = @"\[[Escaped]]"
            };
            var options = new HandlebarsOptions()
            {
                UseAngleBrackets = true
            };
            var result = JsonTasks.Handlebars(input, options);
            Assert.AreEqual("[[Escaped]]", result);
        }

        [Test]
        public void HandlebarsWithEscapedEscape()
        {
            var input = new HandlebarsInput()
            {
                Json = jsonString,
                HandlebarsTemplate = @"\\[[Escaped]]"
            };
            var options = new HandlebarsOptions()
            {
                UseAngleBrackets = true
            };
            var result = JsonTasks.Handlebars(input, options);
            Assert.AreEqual(@"\[[Escaped]]", result);
        }

        [Test]
        public void HandlebarsWithNestedAngleBrackets()
        {
            var input = new HandlebarsInput()
            {
                Json = jsonString,
                HandlebarsTemplate = @"[[[[Manufacturers.0.Products.0.Name]]]]"
            };
            var options = new HandlebarsOptions()
            {
                UseAngleBrackets = true
            };
            var result = JsonTasks.Handlebars(input, options);
            Assert.AreEqual("[[Anvil]]", result);
        }

        [Test]
        public void HandlebarsWithMixedAngleBracketsAndCurlyBraces()
        {
            var input = new HandlebarsInput()
            {
                Json = jsonString,
                HandlebarsTemplate = @"[[Manufacturers.0.Products.0.Name]].{{Manufacturers.0.Products.0.Name}}"
            };
            var options = new HandlebarsOptions()
            {
                UseAngleBrackets = true
            };
            var result = JsonTasks.Handlebars(input, options);
            Assert.AreEqual("Anvil.Anvil", result);
        }

        [Test]
        public void HandlebarsShouldUseBuiltInExistsHelper()
        {
            var input = new HandlebarsInput()
            {
                Json = jsonString,
                HandlebarsTemplate = "{{#each Manufacturers.0.Products}}Product: {{Name}}{{#exists Quantity}}, Quantity: {{Quantity}}{{/exists}}, Price: {{Price}}{{/each}}"
            };
            var result = JsonTasks.Handlebars(input, null);
            Assert.AreEqual("Product: Anvil, Price: 50", result);
        }

        [Test]
        public void HandlebarsBuiltInExistsHelperShouldTakeMultipleArguments()
        {
            var input = new HandlebarsInput()
            {
                Json = jsonString,
                HandlebarsTemplate = "{{#each Manufacturers.0.Products}}{{#exists Name Price}}Product: {{Name}}, Price: {{Price}}{{/exists}}{{/each}}"
            };
            var result = JsonTasks.Handlebars(input, null);
            Assert.AreEqual("Product: Anvil, Price: 50", result);
        }

        [Test]
        public void HandlebarsShouldUseBuiltInTrimHelper()
        {
            var input = new HandlebarsInput()
            {
                Json = jsonString,
                HandlebarsTemplate = "{{trim Manufacturers.0.Products.0.Name}}"
            };
            var result = JsonTasks.Handlebars(input, null);
            Assert.AreEqual("Anvil", result);
        }

        [Test]
        public void HandlebarsShouldUseBuiltInUCaseHelper()
        {
            var input = new HandlebarsInput()
            {
                Json = jsonString,
                HandlebarsTemplate = "{{ucase Manufacturers.0.Products.0.Name}}"
            };
            var result = JsonTasks.Handlebars(input, null);
            Assert.AreEqual("ANVIL", result);
        }

        [Test]
        public void HandlebarsShouldUseBuiltInLCaseHelper()
        {
            var input = new HandlebarsInput()
            {
                Json = jsonString,
                HandlebarsTemplate = "{{lcase Manufacturers.0.Products.0.Name}}"
            };
            var result = JsonTasks.Handlebars(input, null);
            Assert.AreEqual("anvil", result);
        }
    }
}
