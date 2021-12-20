using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Kungsbacka.Json
{
    /// <summary>
    /// Reuired parameters for Query and QuerySingle
    /// </summary>
    public class QueryInput
    {
        /// <summary>
        /// Json input needs to be of type string or JToken
        /// </summary>
        [DefaultValue("{\"key\":\"value\"}")]
        [DisplayFormat(DataFormatString = "Json")]
        public dynamic Json { get; set; }

        /// <summary>
        /// The query is of type JSONPath. More details: http://goessner.net/articles/JsonPath/
        /// </summary>
        [DefaultValue("\"$.key\"")]
        public string Query { get; set; }
    }

    /// <summary>
    /// Optional parameters for Query and QuerySingle
    /// </summary>
    public class QueryOptions
    {
        /// <summary>
        /// A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.
        /// </summary>
        public bool ErrorWhenNotMatched { get; set; }
    }
}
