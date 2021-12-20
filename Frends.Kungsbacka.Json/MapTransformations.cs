using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Frends.Kungsbacka.Json
{
    internal static class MapTransformations
    {
        private static readonly Dictionary<string, Func<JToken, JToken>> _trans;
        private static readonly Regex SwedishSsnRegex = new Regex(
            @"(?<short>^[\d]{10}$)|(?<long>^[\d]{12}$)|(?<dshort>^[\d]{6}-[\d]{4}$)|(?<dlong>^[\d]{8}-[\d]{4}$)",
            RegexOptions.Compiled);


        public static JToken Transform(string name, JToken input)
        {
            if (_trans.TryGetValue(name, out Func<JToken, JToken> trans))
            {
                return trans(input);
            }
            return input;
        }

        static MapTransformations()
        {
            _trans = new Dictionary<string, Func<JToken, JToken>>(StringComparer.OrdinalIgnoreCase);

            _trans.Add("Trim", new Func<JToken, JToken>((input) =>
            {
                if (input is JValue jv)
                {
                    string str = jv.Value<string>();
                    return new JValue(str?.Trim());
                }
                return input;
            }));

            _trans.Add("LCase", new Func<JToken, JToken>((input) =>
            {
                if (input is JValue jv)
                {
                    string str = jv.Value<string>();
                    return new JValue(str?.ToLower());
                }
                return input;
            }));

            _trans.Add("UCase", new Func<JToken, JToken>((input) =>
            {
                if (input is JValue jv)
                {
                    string str = jv.Value<string>();
                    return new JValue(str?.ToUpper());
                }
                return input;
            }));

            _trans.Add("SweSsn", new Func<JToken, JToken>((input) =>
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

            _trans.Add("SweOrgNum", new Func<JToken, JToken>((input) =>
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
