using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;

namespace Frends.Kungsbacka.Json
{
    /// <summary>
    /// JsonSchema Tasks
    /// </summary>
    public static class MapTask
    {

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
                bool keepExistingValue = TaskHelper.EndsWithChar(ref to, '*');
                bool useSelectToken = TaskHelper.StartsWithChar(ref from, '$');
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
    }
}
