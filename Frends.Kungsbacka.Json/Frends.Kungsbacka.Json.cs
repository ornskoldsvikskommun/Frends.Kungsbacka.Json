using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Frends.Kungsbacka.Json
{
    /// <summary>
    /// JsonSchema Tasks
    /// </summary>
    public static class JsonTasks
    {
        private static readonly Regex anglebracketsRegex;
        private static readonly MatchEvaluator replaceAnglebracketsWithCurlyBraces;

        static JsonTasks()
        {
            // This is a crude attempt to support alternative template brackets.
            // You should never use regex to parse text. I still think it
            // will be useful, but consider it experimantal.
            anglebracketsRegex = new Regex(@"(^|[^\\])(\[\[)([^\]]*)(\]\])|(\\\[)", RegexOptions.Compiled);
            replaceAnglebracketsWithCurlyBraces = new MatchEvaluator((m) =>
            {
                if (m.Groups[0].Value == "\\[")
                {
                    return "[";
                }
                return m.Groups[1].Value + "{{" + m.Groups[3].Value + "}}";
            });
        }

        /// <summary>
        /// Validates Json against supplied schema
        /// Documentation: https://github.com/RicoSuter/NJsonSchema
        /// </summary>
        /// <param name="input">Required parameters</param>
        /// <param name="options">Optional parameters</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Collection with NJsonSchema.Validation.ValidationError</returns>
        public static async Task<ValidateResult> Validate([PropertyTab] ValidateInput input, [PropertyTab] ValidateOptions options, CancellationToken cancellationToken)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.Schema == null)
            {
                throw new ArgumentNullException(nameof(input.Schema));
            }

            if (input.Json == null)
            {
                throw new ArgumentNullException(nameof(input.Json));
            }

            var schema = await NJsonSchema.JsonSchema.FromJsonAsync(input.Schema, cancellationToken);
            
            SchemaType schemaType = SchemaType.JsonSchema;
            if (options != null)
            {
                schemaType = options.SchemaType;
            }

            ICollection<NJsonSchema.Validation.ValidationError> errors;
            try
            {
                errors = schema.Validate(input.Json, (NJsonSchema.SchemaType)schemaType);
            }
            catch (Exception exception)
            {
                if (options.ThrowOnInvalidJson)
                {
                    throw;  // re-throw
                }
                errors = new List<NJsonSchema.Validation.ValidationError>();
                while (exception != null)
                {
                    errors.Add(new NJsonSchema.Validation.ValidationError(NJsonSchema.Validation.ValidationErrorKind.Unknown, exception.Message, null, null, null));
                    exception = exception.InnerException;
                }
            }
            if (errors.Count > 0 && options.ThrowOnInvalidJson)
            {
                throw new JsonException($"Json is not valid. {string.Join(";", errors.Select(e => e.ToString()))}");
            }
            return new ValidateResult()
            {
                Errors = errors,
                IsValid = errors.Count == 0
            };
        }

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
                throw new ArgumentException("Handlebars template cannot be null or empty string", nameof(input.HandlebarsTemplate));
            }
            if (input.Json == null)
            {
                throw new ArgumentNullException(nameof(input.Json));
            }

            bool useAngleBrackets = options != null && options.UseAngleBrackets;
            if (useAngleBrackets)
            {
                input.HandlebarsTemplate = anglebracketsRegex.Replace(input.HandlebarsTemplate, replaceAnglebracketsWithCurlyBraces);
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
                        partial.Template = anglebracketsRegex.Replace(partial.Template, replaceAnglebracketsWithCurlyBraces);
                    }
                    using (var reader = new StringReader(partial.Template))
                    {
                        handlebars.RegisterTemplate(partial.TemplateName, partial.Template);
                    }
                }
            }

            JToken jToken = GetJTokenFromInput(input.Json);
            var template = handlebars.Compile(input.HandlebarsTemplate);
            return template(jToken);
        }

        /// <summary>
        /// Query a json string / json token for a single result. See https://github.com/FrendsPlatform/Frends.Json
        /// </summary>
        /// <returns>JToken</returns>
        public static object QuerySingle([PropertyTab] QueryInput input, [PropertyTab] QueryOptions options)
        {
            JToken jToken = GetJTokenFromInput(input.Json);

            return jToken.SelectToken(input.Query, options.ErrorWhenNotMatched);
        }

        /// <summary>
        /// Query a json string / json token. See https://github.com/FrendsPlatform/Frends.Json
        /// </summary>
        /// <returns>JToken[]</returns>
        public static IEnumerable<object> Query([PropertyTab] QueryInput input, [PropertyTab] QueryOptions options)
        {
            JToken jToken = GetJTokenFromInput(input.Json);

            return jToken.SelectTokens(input.Query, options.ErrorWhenNotMatched);
        }

        /// <summary>
        /// Convert a json string to JToken
        /// </summary>
        /// <returns>JToken</returns>
        public static object ConvertJsonStringToJToken(string json)
        {
            return JToken.Parse(json);
        }

        /// <summary>
        /// Convert XML string to JToken
        /// </summary>
        /// <returns>JToken</returns>
        public static object ConvertXmlStringToJToken(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var jsonString = JsonConvert.SerializeXmlNode(doc);
            return JToken.Parse(jsonString);

        }

        /// <summary>
        /// Convert XML byte array to JToken
        /// </summary>
        /// <returns>JToken</returns>
        public static object ConvertXmlBytesToJToken(byte[] xml)
        {
            using (var stream = new MemoryStream(xml))
            {
                using (var xmlreader = new XmlTextReader(stream))
                {
                    var doc = new XmlDocument();
                    doc.Load(xmlreader);
                    var jsonString = JsonConvert.SerializeXmlNode(doc);
                    return JToken.Parse(jsonString);
                }
            }
        }

        /// <summary>
        /// Maps properties from one JObject to another. A default value can be specified if the property
        /// is not found in the source object. Optionally simple transforms can be applied to the value.
        /// If destination object is null, a new empty JObject is created.
        /// </summary>
        /// <param name="input">Requred parameters (see MapInput class)</param>
        /// <param name="options">Optional parameters (see MapOptions class)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static object Map([PropertyTab] MapInput input, [PropertyTab] MapOptions options)
        {
            if (input.SourceObject == null)
            {
                throw new ArgumentNullException(nameof(input.SourceObject), "Source object cannot be null.");
            }
            if (input.DestinationObject == null)
            {
                input.DestinationObject = new JObject();
            }
            if (string.IsNullOrEmpty(input.Map))
            {
                throw new ArgumentException("Map cannot be null or an empty string.", nameof(input.Map));
            }
            var mappings = JsonConvert.DeserializeObject<Mapping[]>(input.Map);
            foreach (var mapping in mappings)
            {
                if (string.IsNullOrEmpty(mapping.From))
                {
                    throw new ArgumentNullException(nameof(mapping.From));
                }
                if (string.IsNullOrEmpty(mapping.To))
                {
                    mapping.To = mapping.From;
                }
                string from = mapping.From;
                string to = mapping.To;
                bool keepExistingValue = EndsWithChar(ref to, '*');
                bool useSelectToken = StartsWithChar(ref from, '$');
                if (keepExistingValue && input.DestinationObject.Properties().Any(p => p.Name.IEquals(to)))
                {
                    continue;
                }
                dynamic token;
                if (useSelectToken)
                {
                    token = input.SourceObject.SelectToken(from);
                }
                else
                {
                    token = input.SourceObject[from];
                }
                if (token == null)
                {
                    if (mapping.DefaultPresent)
                    {
                        input.DestinationObject.Add(new JProperty(to, mapping.Default));
                    }
                    continue;
                }
                if (options != null && options.UnpackCdataSection)
                {
                    if (token is JObject)
                    {
                        var cdata = token["#cdata-section"];
                        if (cdata != null)
                        {
                            token = cdata;
                        }
                    }
                }
                foreach (string transformation in mapping.Transformations)
                {
                    token = MapTransformations.Transform(transformation, token);
                }
                input.DestinationObject[to] = token;
            }
            return input.DestinationObject;
        }

        private static object GetJTokenFromInput(dynamic json)
        {
            if (json is string)
            {
                return JToken.Parse(json);
            }

            if (json is JToken)
            {
                return json;
            }

            try
            {
                return (JToken)JsonConvert.DeserializeObject(json);
            }
            catch
            {
                throw new InvalidDataException("The input data was not recognized. Supported formats are JSON string, JToken or a deserializable object.");
            }
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
                handlebars.RegisterHelper("ucase", (writer, context, arguments) =>
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
        }

        // Returns true if str ends with char c and removes c from end of str.
        // Escape char by doubling up. Examples (if c = '*'):
        // 1: "value*" returns true and str is changed to "value"
        // 2: "value**" returns false and str is changed to "value*"
        // 3: "value***" returns true and str is changed to "value*"
        // 4: "value****" returns false and str is changed to "value**"
        private static bool EndsWithChar(ref string str, char c)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            int len = str.Length;
            int pos = len - 1;
            while (pos >= 0 && str[pos] == c)
            {
                pos--;
            }
            bool b = ((len - pos) % 2) == 0;
            if (pos < len - 1)
            {
                str = str.Substring(0, len - (len - pos) / 2);
            }
            return b;
        }

        // Same as EndsWithChar above, but from the start of the string.
        private static bool StartsWithChar(ref string str, char c)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            int len = str.Length;
            int pos = 0;
            while (pos < len && str[pos] == c)
            {
                pos++;
            }
            bool b = (pos % 2) == 1;
            if (pos >= 0)
            {
                int cnt = pos == 1 ? 1 : pos / 2;
                str = str.Substring(cnt, len - cnt);
            }
            return b;
        }
    }
}
