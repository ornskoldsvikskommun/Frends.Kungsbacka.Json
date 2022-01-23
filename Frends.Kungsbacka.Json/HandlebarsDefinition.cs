using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Kungsbacka.Json
{
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
}
