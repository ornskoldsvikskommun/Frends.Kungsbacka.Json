using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Kungsbacka.Json
{
	/// <summary>
	/// Required parameters for Map task
	/// </summary>
	public class MapInput
	{
		/// <summary>
		/// Source JObject (mandatory)
		/// </summary>
		[DisplayFormat(DataFormatString = "Expression")]
		public Newtonsoft.Json.Linq.JObject SourceObject { get; set; }

		/// <summary>
		/// Destination JObject. If destination object is not supplied (null),
		/// a new JObject is created.
		/// </summary>
		[DisplayFormat(DataFormatString = "Expression")]
		public Newtonsoft.Json.Linq.JObject DestinationObject { get; set; }

		/// <summary>
		/// JSON that describes the mappings. Array of:
		/// {
		///     "from": "from name",
		///     "to": "to name",
		///     "def": "Optional default",
		///     "trans": ["Array of transformations"]
		/// }
		/// 
		/// Def can be used to supply a default value if the source does not
		/// exist. Trans applies transformations to the source token before it
		/// is added to the destination object.
		/// 
		/// From property supports using Json.NETs SelectToken query syntax (
		/// https://www.newtonsoft.com/json/help/html/SelectToken.htm). 
		/// </summary>
		[DisplayFormat(DataFormatString = "Json")]
		[DefaultValue(@"[
    {""from"": ""Firstname"", ""to"": ""first""},
    {""from"": ""Lastname"", ""to"": ""last""},
    {""from"": ""NewUser"", ""to"": ""new"", ""def"": false},
    {""from"": ""Description"", ""to"": ""desc"", ""trans"": [""Trim""]}
]")]
		public string Map { get; set; }
	}

	/// <summary>
	/// Optional parameters for Map task
	/// </summary>
	public class MapOptions
	{
		/// <summary>
		/// If a source object contains a #cdata-section, get the
		/// value inside instead. #cdata-sections is a common
		/// result of XML to JSON conversions.
		/// </summary>
		[DefaultValue(false)]
		public bool UnpackCdataSection { get; set; }

		/// <summary>
		/// Array of custom transformations
		/// </summary>
		public MapTransformation[] Tranformations { get; set; }
	}

	/// <summary>
	/// Custom transformation
	/// </summary>
	public class MapTransformation
	{
		/// <summary>
		/// Transformation name
		/// </summary>
		public string TransformationName { get; set; }
        /// <summary>
        /// Transformation function. Must be a Func&lt;JToken,JToken&gt;
        /// Example function that takes the first element in an array.
        /// <code>
        /// return new Func&lt;JToken,JToken&gt;((input) =>
        /// {
        ///     if (input is JArray array)
        ///     {
        ///         if (array.Count > 0)
        ///         {
        ///             return array[0];
        ///         }
        ///     }
        ///     return null;
        /// });
        /// </code>
        /// </summary>
        public dynamic TransformationFunction { get; set; }
    }
	/// <summary>
	/// Inputparameter for converting string to jtoken
	/// </summary>
	public class XmlReaderInputString
	{
		/// <summary>
		/// Source xml as bytes 
		/// </summary>
		[DisplayFormat(DataFormatString = "Expression")]
		public string Xml { get; set; }
	}
	/// <summary>
	/// Inputparameter for converting bytes to jtoken
	/// </summary>
	public class XmlReaderInputBytes
	{
		/// <summary>
		/// Source xml as bytes 
		/// </summary>
		[DisplayFormat(DataFormatString = "Expression")]
		public byte[] Xml { get; set; }
	}
	/// <summary>
	/// Settings for converting xml to JToken using xmlreader.
	/// </summary>
	public class XmlReaderInputSettings
	{
		/// <summary>
		/// Will return the root element of the xml. Will ignore Elements-parameter
		/// </summary>
		[DefaultValue(true)]
		public bool SelectRootElement { get; set; }
		/// <summary>
		/// List of elements used to get specific values from xml
		/// </summary>
		[DisplayFormat(DataFormatString = "Expression")]
		[DefaultValue(@"new List<string> { """" }")]
		public List<string> Elements { get; set; }
		/// <summary>
		/// Used when the expected result is only one object, when the xmlnode is found the rest will be skipped.
		/// This can be used for getting a specific named element anywhere in the xml. 
		/// Example:
		///  <root>
		///   <animal>
		///    <name>Tom</name>
		///    <type>Cat</type>
		///  </animal>
		/// </root>
		/// Combining SingleElement with "animal" in Elements will return the "animal" together with the children "name" and "type".
		/// Combining SingleElement with "type" in Elements will return the "type" node.
		/// </summary>
		[DefaultValue(false)]
		public bool SingleElement { get; set; }
		/// <summary>
		/// Using this will return all the values and attributes on the found nodes as a flat JToken. All children will be ignored.
		/// Used for only getting specific values from an xml. Should probably no be used on a node with children.
		/// Example:
		/// <root>
		///  <animal>
		///    <name>Tom</name>
		///    <type id='1'>Cat</type>
		///  </animal>
		/// </root>
		/// Combining ResultAsJTokenOnlyValues with "type, name" in Elements will return a JToken with the following data:
		/// {
		///		{ "name": "Tom " },
		///		{ "type": "Cat " },
		///		{ "id": "1 " },
		/// }
		/// </summary>
		[DefaultValue(false)] 
		public bool AsSingleJtoken { get; set; }

	}
}
