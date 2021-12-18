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
}
