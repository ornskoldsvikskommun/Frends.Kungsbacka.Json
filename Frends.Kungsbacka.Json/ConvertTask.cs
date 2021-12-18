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
    public static class ConvertTask
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
    }
}
