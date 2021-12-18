using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Frends.Kungsbacka.Json
{
    /// <summary>
    /// JsonSchema Tasks
    /// </summary>
    public static class HandlebarsTask
    {
        /// <summary>
        /// Handlebars provides the power necessary to let you build semantic templates effectively with no frustration.
        /// See https://github.com/Handlebars-Net/Handlebars.Net and https://github.com/FrendsPlatform/Frends.Json
        /// </summary>
        /// <returns>string</returns>
        public static string Handlebars([PropertyTab] HandlebarsInput input, [PropertyTab] HandlebarsOptions options)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (string.IsNullOrEmpty(input.HandlebarsTemplate))
            {
                throw new ArgumentException("Handlebars template cannot be null or an empty string", nameof(input.HandlebarsTemplate));
            }
            if (input.Json == null)
            {
                throw new ArgumentNullException(nameof(input.Json));
            }

            bool useAngleBrackets = options != null && options.UseAngleBrackets;
            if (useAngleBrackets)
            {
                input.HandlebarsTemplate = TaskHelper.ReplaceAngleBracketsWithCurlyBraces(input.HandlebarsTemplate);
            }

            var handlebars = HandlebarsDotNet.Handlebars.Create();

            // Helpers must be registrerd before partials to be available inside partials
            if (input.HandlebarsHelpers != null)
            {
                foreach (var helper in input.HandlebarsHelpers)
                {
                    if (helper.HelperAction is Action<TextWriter, dynamic, object[]> action)
                    {
                        handlebars.RegisterHelper(helper.HelperName,
                            new HandlebarsDotNet.HandlebarsHelper(action)
                        );
                    }
                }
            }

            if (input.HandlebarsBlockHelpers != null)
            {
                foreach (var helper in input.HandlebarsBlockHelpers)
                {
                    if (helper.HelperAction is Action<TextWriter, dynamic, dynamic, object[]> action)
                    {
                        handlebars.RegisterHelper(helper.HelperName,
                            new HandlebarsDotNet.HandlebarsBlockHelper(action)
                        );
                    }
                }
            }

            // Do this after registring user supplied helpers to allow overriding built-in helpers.
            RegisterBuiltInHandlebarsHelpers(handlebars);

            if (input.HandlebarsPartials != null)
            {
                foreach (var partial in input.HandlebarsPartials)
                {
                    if (useAngleBrackets)
                    {
                        partial.Template = TaskHelper.ReplaceAngleBracketsWithCurlyBraces(partial.Template);
                    }
                    using (var reader = new StringReader(partial.Template))
                    {
                        handlebars.RegisterTemplate(partial.TemplateName, partial.Template);
                    }
                }
            }

            JToken jToken = TaskHelper.GetJTokenFromInput(input.Json);
            var template = handlebars.Compile(input.HandlebarsTemplate);
            return template(jToken);
        }

        private static void RegisterBuiltInHandlebarsHelpers(HandlebarsDotNet.IHandlebars handlebars)
        {
            if (!handlebars.Configuration.BlockHelpers.ContainsKey("exists"))
            {
                handlebars.RegisterHelper("exists", (writer, options, context, arguments) =>
                {
                    // Using reflection because UndefinedBindingResult is internal
                    if (arguments[0].GetType().Name == "UndefinedBindingResult")
                    {
                        options.Inverse(writer, (object)context);
                    }
                    else
                    {
                        options.Template(writer, (object)context);
                    }
                });
            }
            if (!handlebars.Configuration.Helpers.ContainsKey("ucase"))
            {
                handlebars.RegisterHelper("ucase", (writer, context, arguments) =>
                {
                    foreach (object arg in arguments)
                    {
                        if (arg is JToken token)
                        {
                            writer.Write(token.Value<string>()?.ToUpper());
                        }
                    }
                });
            }
            if (!handlebars.Configuration.Helpers.ContainsKey("lcase"))
            {
                handlebars.RegisterHelper("lcase", (writer, context, arguments) =>
                {
                    foreach (object arg in arguments)
                    {
                        if (arg is JToken token)
                        {
                            writer.Write(token.Value<string>()?.ToLower());
                        }
                    }
                });
            }
            if (!handlebars.Configuration.Helpers.ContainsKey("trim"))
            {
                handlebars.RegisterHelper("trim", (writer, context, arguments) =>
                {
                    foreach (object arg in arguments)
                    {
                        if (arg is JToken token)
                        {
                            writer.Write(token.Value<string>()?.Trim());
                        }
                    }
                });
            }
        }
    }
}
