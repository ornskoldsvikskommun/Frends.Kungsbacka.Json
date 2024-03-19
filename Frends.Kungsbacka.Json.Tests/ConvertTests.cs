using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frends.Kungsbacka.Json.Tests
{
	[TestFixture]
	class ConvertTests
	{
		const string xml = @"<?xml version='1.0' standalone='no'?>
             <root>
               <person id='1'>
                 <name>Alan</name>
                 <url>http://www.google.com</url>
               </person>
               <person id='2'>
                <name>Louis</name>
                 <url>http://www.yahoo.com</url>
              </person>
            </root>";

		const string xmlReaderXml = @"<bookstore>
									  <book category='COOKING'>
										<title>Everyday Italian</title>
										<author>Giada De Laurentiis</author>										
										<location country='US'>New York</location>
										<year>2005</year>
										<price>30.00</price>
									  </book>
									  <book category='CHILDREN'>
										<title>Harry Potter</title>
										<author>J K. Rowling</author>
										<location country='US'>Washington</location>
										<year>2005</year>
										<price>29.99</price>
									  </book>
									  <book category='WEB'>
										<title>Learning XML</title>
										<location country='US'>California</location>
										<author>Erik T. Ray</author>
										<year>2003</year>
										<price>39.95</price>
									  </book>
									</bookstore>";

		[Test]
		public void ShouldConvertXmlStringToJToken()
		{
			var result = JsonTasks.ConvertXmlStringToJToken(xml);
			Assert.IsInstanceOf<JObject>(result);
		}

		[Test]
		public void ShouldConvertStringToJToken()
		{
			string json = @"{
                ""FirstName"": ""John"",
                ""LastName"": ""Doe""
            }";
			JToken token = JToken.Parse(json);
			var result = JsonTasks.ConvertJsonStringToJToken(json);
			Assert.AreEqual(token, result);
		}

		[Test]
		public void ShouldConvertXmlBytesToJToken()
		{
			var bytes = Encoding.UTF8.GetBytes(xml);
			var result = JsonTasks.ConvertXmlBytesToJToken(bytes);
			Assert.IsInstanceOf<JObject>(result);
		}
		[Test]
		public void ConvertXmlBytesToJTokenXmlReaderResultIsJObject()
		{

			var input = new XmlReaderInputBytes
			{
				Xml = Encoding.UTF8.GetBytes(xmlReaderXml)
			};

			var settings = new XmlReaderInputSettings
			{
				Elements = new List<string> { "bookstore" },
				SingleElement = true,
				AsSingleJtoken = false
			};

			var result = JsonTasks.ConvertXmlBytesToJTokenXmlReader(input, settings);

			Assert.IsInstanceOf<JObject>(result);
		}
		[Test]
		public void ConvertXmlBytesToJTokenXmlReaderResultIsSingleObject()
		{
			var settings = new XmlReaderInputSettings
			{
				Elements = new List<string> { "bookstore" },
				SingleElement = true,
				AsSingleJtoken = false
			};
			var input = new XmlReaderInputBytes
			{
				Xml = Encoding.UTF8.GetBytes(xmlReaderXml)
			};

			var result = JsonTasks.ConvertXmlBytesToJTokenXmlReader(input, settings);

			Assert.AreEqual(result.Count(), 1);
		}
		[Test]
		public void ConvertXmlBytesToJTokenXmlReaderResultIsJArrayWithObjects()
		{
			var settings = new XmlReaderInputSettings
			{
				Elements = new List<string> { "book" },
				SingleElement = false,
				AsSingleJtoken = false
			};

			var input = new XmlReaderInputBytes
			{
				Xml = Encoding.UTF8.GetBytes(xmlReaderXml)
			};

			var result = JsonTasks.ConvertXmlBytesToJTokenXmlReader(input, settings);

			Assert.IsInstanceOf<JArray>(result);
			Assert.AreEqual(result.Count(), 3);
		}
		[Test]
		public void ConvertXmlBytesToJTokenXmlReaderResultIsFirstJObjectInJToken()
		{
			var settings = new XmlReaderInputSettings
			{
				Elements = new List<string> { "book" },
				SingleElement = true,
				AsSingleJtoken = false
			};

			var input = new XmlReaderInputBytes
			{
				Xml = Encoding.UTF8.GetBytes(xmlReaderXml)
			};

			var result = JsonTasks.ConvertXmlBytesToJTokenXmlReader(input, settings);

			Assert.IsInstanceOf<JObject>(result);
			Assert.AreEqual(result.Count(), 1);

			Assert.AreEqual(result["book"]["title"].ToString(), "Everyday Italian");
		}
		[Test]
		public void ConvertXmlStringToJTokenXmlReaderResultIsJObject()
		{
			var settings = new XmlReaderInputSettings
			{
				Elements = new List<string> { "bookstore" },
				SingleElement = true,
				AsSingleJtoken = false
			};

			var input = new XmlReaderInputString
			{
				Xml = xmlReaderXml
			};

			var result = JsonTasks.ConvertXmlStringToJTokenXmlReader(input, settings);
			Assert.IsInstanceOf<JObject>(result);
		}
		[Test]
		public void ConvertXmlStringToJTokenXmlReaderResultIsSingleObject()
		{
			var settings = new XmlReaderInputSettings
			{
				Elements = new List<string> { "bookstore" },
				SingleElement = true,
				AsSingleJtoken = false
			};

			var input = new XmlReaderInputString
			{
				Xml = xmlReaderXml
			};

			var result = JsonTasks.ConvertXmlStringToJTokenXmlReader(input, settings);
			Assert.AreEqual(result.Count(), 1);
		}
		[Test]
		public void ConvertXmlStringToJTokenXmlReaderResultIsJArrayWithObjects()
		{
			var settings = new XmlReaderInputSettings
			{
				Elements = new List<string> { "book" },
				SingleElement = false,
				AsSingleJtoken = false
			};

			var input = new XmlReaderInputString
			{
				Xml = xmlReaderXml
			};

			var result = JsonTasks.ConvertXmlStringToJTokenXmlReader(input, settings);

			Assert.IsInstanceOf<JArray>(result);
			Assert.AreEqual(result.Count(), 3);
		}

		[Test]
		public void ConvertXmlStringToJTokenXmlReaderResultMultipleElements()
		{
			var settings = new XmlReaderInputSettings
			{
				Elements = new List<string> { "author", "location" },
				SingleElement = false,
				AsSingleJtoken = true
			};

			var input = new XmlReaderInputString
			{
				Xml = xmlReaderXml
			};

			var result = JsonTasks.ConvertXmlStringToJTokenXmlReader(input, settings);

			Assert.IsInstanceOf<JObject>(result);
			Assert.AreEqual(result["author"].ToString(), "Giada De Laurentiis");
			Assert.AreEqual(result["location"].ToString(), "New York");
		}
		[Test]
		public void ConvertXmlStringToJTokenXmlReaderResultMultipleElementsAndAttributes()
		{
			var settings = new XmlReaderInputSettings
			{
				Elements = new List<string> { "name", "location" },
				SingleElement = true,
				AsSingleJtoken = true
			};

			var input = new XmlReaderInputString
			{
				Xml = xmlReaderXml
			};

			var result = JsonTasks.ConvertXmlStringToJTokenXmlReader(input, settings);

			Assert.IsInstanceOf<JObject>(result);
			Assert.AreEqual(result["@country"].ToString(), "US");
			Assert.AreEqual(result["location"].ToString(), "New York");
		}
		[Test]
		public void ConvertXmlStringToJTokenXmlReaderSelectRootElementShouldReturnRootElement()
		{
			var settings = new XmlReaderInputSettings
			{
				Elements = new List<string> { "" },
				SingleElement = false,
				AsSingleJtoken = false,
				SelectRootElement = true
			};

			var input = new XmlReaderInputString
			{
				Xml = xmlReaderXml
			};

			var result = JsonTasks.ConvertXmlStringToJTokenXmlReader(input, settings);

			Assert.IsInstanceOf<JObject>(result);
			Assert.AreEqual(result.Count(), 1);
		}
	}
}
