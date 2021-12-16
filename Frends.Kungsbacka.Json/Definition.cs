using HandlebarsDotNet;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Kungsbacka.Json
{
    /// <summary>
    /// A copy of NJsonSchema.SchemaType
    /// The enum must live in the task assembly or else it is not found.
    /// </summary>
    public enum SchemaType
    {
        /// <summary>
        /// Uses oneOf with null schema and null type to express the nullability of a property
        /// (valid JSON Schema draft v4).
        /// </summary>
        JsonSchema = 0,

        /// <summary>
        /// Uses required to express the nullability of a property
        /// (not valid in JSON Schema draft v4).
        /// </summary>
        Swagger2 = 1,

        /// <summary>
        /// Uses null handling of Open API 3.
        /// </summary>
        OpenApi3 = 2
    }

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

    /// <summary>
    /// Required parameters
    /// </summary>
    public class ValidateInput
    {
        /// <summary>
        /// Schema to validate against.
        /// </summary>
        [DisplayFormat(DataFormatString = "Json")]
        public string Schema { get; set; }

        /// <summary>
        /// Json to validate
        /// </summary>
        [DisplayFormat(DataFormatString = "Json")]
        public string Json { get; set; }
    }

    /// <summary>
    /// Optional parameters
    /// </summary>
    public class ValidateOptions
    {
        /// <summary>
        /// Defines how to express the nullability of a property.
        /// * JsonSchema: Uses oneOf with null schema and null type to express the nullability of a property (valid JSON Schema draft v4).
        /// * Swagger2: Uses required to express the nullability of a property (not valid in JSON Schema draft v4).
        /// * OpenApi3: Uses null handling of Open API 3.
        /// </summary>
        [DefaultValue(SchemaType.JsonSchema)]
        public SchemaType SchemaType { get; set; }

        /// <summary>
        /// Throw exception if Json is invalid
        /// </summary>
        public bool ThrowOnInvalidJson { get; set; }
    }

    /// <summary>
    /// Validation result
    /// </summary>
    public class ValidateResult
    {
        /// <summary>
        /// Idicates if JSON is valid
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Validation error collection
        /// </summary>
        public ICollection<NJsonSchema.Validation.ValidationError> Errors { get; set; }
    }

    /// <summary>
    /// Required input for Handlebars task
    /// </summary>
    public class HandlebarsInput
    {
        /// <summary>
        /// Json input needs to be of type string or JToken
        /// </summary>
        [DefaultValue("{\"title\":\"Mr.\", \"name\":\"Foo\" }")]
        [DisplayFormat(DataFormatString = "Json")]
        public dynamic Json { get; set; }

        /// <summary>
        /// Template for Handlebars. > indicates a partial. If you use curly brackets in the template,
        /// then it needs to be in expression mode. Using {{ }} in other modes breaks the task.
        /// To avoid this you can enable angle brackets in options and use [[ ]] instead.
        /// </summary>
        [DefaultValue("\"<xml> {{title}} {{> strongName}} </xml>\"")]
        [DisplayFormat(DataFormatString = "Expression")]
        public string HandlebarsTemplate { get; set; }

        /// <summary>
        /// Optional partials for template.
        /// </summary>
        public HandlebarsPartial[] HandlebarsPartials { get; set; }

        /// <summary>
        /// Optional handlebars helpers
        /// </summary>
        public HandlebarsHelper[] HandlebarsHelpers { get; set; }

        /// <summary>
        /// Optional handlebars block helpers
        /// </summary>
        public HandlebarsBlockHelper[] HandlebarsBlockHelpers { get; set; }
    }

    /// <summary>
    /// Optional parameters for Handlebars task
    /// </summary>
    public class HandlebarsOptions
    {
        /// <summary>
        /// Use [[angle brackets]] instead of {{curly braces}} in Handlebars template
        /// </summary>
        [DefaultValue(false)]
        public bool UseAngleBrackets { get; set; }
    }

    /// <summary>
    /// Handlebars partial
    /// </summary>
    public class HandlebarsPartial
    {
        /// <summary>
        /// Template name that exists in the HandlebarsTemplate.
        /// </summary>
        [DefaultValue("\"strongName\"")]
        public string TemplateName { get; set; }

        /// <summary>
        /// Partial template. This needs to be in expression mode. Using {{ }} in other modes breaks the task.
        /// </summary>
        [DefaultValue("\"<strong>{{name}}</strong>\"")]
        [DisplayFormat(DataFormatString = "Expression")]
        public string Template { get; set; }
    }

    /// <summary>
    /// Handlebars helper
    /// </summary>
    public class HandlebarsHelper
    {
        /// <summary>
        /// Helper name. This is the name you use to reference
        /// the helper in the template.
        /// </summary>
        public string HelperName { get; set; }

        /// <summary>
        /// Helper action. Must be an Action&lt;String.IO.TextWriter, dynamic, object[]&gt;
        /// Create a helper using a C# statement i FRENDS.
        /// <code>
        /// return new Action&lt;System.IO.TextWriter, dynamic, object[]&gt;((writer, context, parameters) => {
        ///     foreach (object param in parameters)
        ///     {
        ///         if (param is JToken)
        ///         {
        ///             writer.Write(((JToken) param).Value&lt;string&gt;()?.ToUpper());
        ///         }
        ///     }
        /// });
        /// </code>
        /// </summary>
        public dynamic HelperAction { get; set; }
    }

    /// <summary>
    /// Handlebars block helper
    /// </summary>
    public class HandlebarsBlockHelper
    {
        /// <summary>
        /// Helper name. This is the name you use to reference
        /// the helper in the template.
        /// </summary>
        public string HelperName { get; set; }

        /// <summary>
        /// Helper action. Must be an Action&lt;String.IO.TextWriter, dynamic, dynamic, object[]&gt;
        /// Create a helper using a C# statement i FRENDS.
        /// <code>
        /// return new Action&lt;System.IO.TextWriter, dynamic, object[]&gt;((writer, options, context, parameters) => {
        ///     if (some condition)
        ///     {
        ///         options.Template(writer, (object)context);
        ///     }
        ///     else
        ///     {
        ///         options.Inverse(writer, (object)context);
        ///     }
        /// });
        /// </code>
        /// </summary>
        public dynamic HelperAction { get; set; }
    }

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
    }

}
