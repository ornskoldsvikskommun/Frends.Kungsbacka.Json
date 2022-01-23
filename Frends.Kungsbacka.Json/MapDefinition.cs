using Newtonsoft.Json.Linq;
using System;
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
        /// Transformation function. Must be a Func&lt;JToken&gt;
        /// Example function that takes the first element in an array.
        /// <code>
        /// return new Func&lt;JToken&gt;((input) =>
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
}
