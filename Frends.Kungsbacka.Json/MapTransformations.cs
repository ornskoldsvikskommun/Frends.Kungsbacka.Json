using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Frends.Kungsbacka.Json
{
    internal static class MapTransformations
    {
        private static readonly Dictionary<string, Func<JToken, JToken>> _transformations =
            new Dictionary<string, Func<JToken, JToken>>(StringComparer.OrdinalIgnoreCase);

        private static readonly Regex SwedishSsnRegex = new Regex(
            @"(?<short>^[\d]{10}$)|(?<long>^[\d]{12}$)|(?<dshort>^[\d]{6}-[\d]{4}$)|(?<dlong>^[\d]{8}-[\d]{4}$)",
            RegexOptions.Compiled);

        public static JToken Transform(string name, JToken input)
        {
            if (_transformations.TryGetValue(name, out Func<JToken, JToken> trans))
            {
                return trans(input);
            }
            return input;
        }


        public static void RegisterTransformation(MapTransformation transformation)
        {
            RegisterTransformation(transformation.TransformationName, transformation.TransformationFunction);
        }

        private static void RegisterTransformation(string name, dynamic transformation)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (transformation == null)
            {
                throw new ArgumentNullException(nameof(transformation));
            }
            _transformations[name] = transformation;
        }

        public static void RegisterBuiltInTransformations()
        {
            RegisterTransformation("Trim", new Func<JToken, JToken>((input) =>
            {
                if (input is JValue jv)
                {
                    string str = jv.Value<string>();
                    return new JValue(str?.Trim());
                }
                return input;
            }));

            RegisterTransformation("LCase", new Func<JToken, JToken>((input) =>
            {
                if (input is JValue jv)
                {
                    string str = jv.Value<string>();
                    return new JValue(str?.ToLower());
                }
                return input;
            }));

            RegisterTransformation("UCase", new Func<JToken, JToken>((input) =>
            {
                if (input is JValue jv)
                {
                    string str = jv.Value<string>();
                    return new JValue(str?.ToUpper());
                }
                return input;
            }));

            RegisterTransformation("SweSsn", new Func<JToken, JToken>((input) =>
            {
                if (input is JValue jv)
                {
                    string str = jv.Value<string>();
                    var matches = SwedishSsnRegex.Match(str);
                    if (matches.Groups["short"].Success)
                    {
                        return str.Substring(0, 6) + "-" + str.Substring(6, 4);
                    }
                    else if (matches.Groups["long"].Success)
                    {
                        return str.Substring(0, 8) + "-" + str.Substring(8, 4);
                    }
                }
                return input;
            }));

            RegisterTransformation("SweOrgNum", new Func<JToken, JToken>((input) =>
            {
                if (input is JValue jv)
                {
                    string str = jv.Value<string>();
                    var matches = SwedishSsnRegex.Match(str);
                    if (matches.Groups["short"].Success)
                    {
                        return "16" + str.Substring(0, 6) + "-" + str.Substring(6, 4);
                    }
                    else if (matches.Groups["long"].Success)
                    {
                        return str.Substring(0, 8) + "-" + str.Substring(8, 4);
                    }
                    else if (matches.Groups["dshort"].Success)
                    {
                        return "16" + input;
                    }
                }
                return input;
            }));
        }
    }
}
