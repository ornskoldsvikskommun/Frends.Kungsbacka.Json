using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Xml;

namespace Frends.Kungsbacka.Json
{
	/// <summary>
	/// JsonSchema Tasks
	/// </summary>
	public static partial class JsonTasks
	{
		/// <summary>
		/// Convert a json string to JToken
		/// </summary>
		/// <returns>JToken</returns>
		public static JToken ConvertJsonStringToJToken(string json)
		{
			return JToken.Parse(json);
		}

		/// <summary>
		/// Convert XML string to JToken
		/// </summary>
		/// <returns>JToken</returns>
		public static JToken ConvertXmlStringToJToken(string xml)
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
		public static JToken ConvertXmlBytesToJToken(byte[] xml)
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
		/// Convert XML string to JToken using xmlreader and using specific elementnames. 
		/// Using singleElement will return the first object.
		/// Using asJToken will all objects in one single jToken.
		/// </summary>
		/// <returns>JToken</returns>
		public static JToken ConvertXmlStringToJTokenXmlReader([PropertyTab] XmlReaderInputString input, [PropertyTab] XmlReaderInputSettings settings)
		{
			IEnumerable<XmlNode> result;

			using (var stringReader = new StringReader(input.Xml))
			{
				using (var xmlReader = XmlReader.Create(stringReader, XmlReaderSettings(settings.SingleElement, settings.SelectRootElement)))
				{
					result = ReadXmlNodes(xmlReader, settings.Elements, settings.SelectRootElement, settings.SingleElement);
				}
			}

			if(settings.AsSingleJtoken)
			{
				return XmlNodeListToJTokenOnlyValues(result);
			}

			if (!result.Any()) return new JArray();

			return settings.SingleElement || settings.SelectRootElement ? JToken.FromObject(result.FirstOrDefault()) : JToken.FromObject(result);
		}

		/// <summary>
		/// Convert XML byte array to JToken using xmlreader and using specific elementnames
		/// </summary>
		/// <returns>JToken</returns>
		public static JToken ConvertXmlBytesToJTokenXmlReader([PropertyTab] XmlReaderInputBytes input, [PropertyTab] XmlReaderInputSettings settings)
		{
			IEnumerable<XmlNode> result;

			using (var stream = new MemoryStream(input.Xml))
			{
				using (var xmlReader = XmlReader.Create(stream, XmlReaderSettings(settings.SingleElement, settings.SelectRootElement)))
				{
					result = ReadXmlNodes(xmlReader, settings.Elements, settings.SelectRootElement, settings.SingleElement);
				}
			}

			if (settings.AsSingleJtoken)
			{
				return XmlNodeListToJTokenOnlyValues(result);
			}

			if(!result.Any()) return new JArray();

			return settings.SingleElement || settings.SelectRootElement ? JToken.FromObject(result.FirstOrDefault()) : JToken.FromObject(result);
		}

		private static IEnumerable<XmlNode> ReadXmlNodes(XmlReader xmlReader, List<string> elements, bool selectRootElement, bool singleElement)
		{
			var doc = new XmlDocument();
			var nodeList = new List<XmlNode>();

			while (xmlReader.Read())
			{
				if (xmlReader.NodeType == XmlNodeType.Element)
				{
					if(selectRootElement)
					{
						nodeList.Add(doc.ReadNode(xmlReader));

						break;
					}

					if (elements.Contains(xmlReader.Name, StringComparer.OrdinalIgnoreCase))
					{
						nodeList.Add(doc.ReadNode(xmlReader));
						if (singleElement) break;
					}
				}
			}

			if(!singleElement || selectRootElement && nodeList.Any())
			{
				RemoveWhitespaceNodes(nodeList);
			}

			return nodeList;
		}

		private static void RemoveWhitespaceNodes(IEnumerable<XmlNode> nodeList)
		{
			foreach (XmlNode node in nodeList)
			{
				for (int i = 0; i < node.ChildNodes.Count; i++)
				{
					if (node.ChildNodes[i].NodeType == XmlNodeType.Whitespace)
					{
						node.RemoveChild(node.ChildNodes[i]);
					}
				}
			}
		}

		private static readonly string[] AttributesToIgnore = { "xmlns" };	

		private static JToken XmlNodeListToJTokenOnlyValues(IEnumerable<XmlNode> xmlNodes)
		{
			var result = new JObject();
			
			foreach (XmlNode node in xmlNodes)
			{
				if (!result.ContainsKey(node.Name))
				{
					result.Add(node.Name, node.InnerXml);

					if (node.Attributes.Count > 0)
					{
                        foreach (XmlAttribute nodeAttribute in node.Attributes)
                        {
							if(!AttributesToIgnore.Contains($"@{nodeAttribute.Name}") && !result.ContainsKey($"@{nodeAttribute.Name}"))
							{
								result.Add($"@{nodeAttribute.Name}", nodeAttribute.InnerText);
							}
						}
					}
				}
			}

			return result;
		}

		private static XmlReaderSettings XmlReaderSettings(bool singleElement, bool selectRootElement)
		{
			return new XmlReaderSettings
			{
				IgnoreWhitespace = singleElement || selectRootElement
			};
		}
	}
}
