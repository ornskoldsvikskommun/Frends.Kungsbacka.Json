using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Kungsbacka.Json
{
    /// <summary>
    /// JsonSchema Tasks
    /// </summary>
    public static class ValidateTask
    {
        /// <summary>
        /// Validates Json against supplied schema
        /// Documentation: https://github.com/RicoSuter/NJsonSchema
        /// </summary>
        /// <param name="input">Required parameters</param>
        /// <param name="options">Optional parameters</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Collection with NJsonSchema.Validation.ValidationError</returns>
        public static async Task<ValidateResult> Validate([PropertyTab] ValidateInput input, [PropertyTab] ValidateOptions options, CancellationToken cancellationToken)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.Schema == null)
            {
                throw new ArgumentNullException(nameof(input.Schema));
            }

            if (input.Json == null)
            {
                throw new ArgumentNullException(nameof(input.Json));
            }

            var schema = await NJsonSchema.JsonSchema.FromJsonAsync(input.Schema, cancellationToken);
            
            SchemaType schemaType = SchemaType.JsonSchema;
            if (options != null)
            {
                schemaType = options.SchemaType;
            }

            ICollection<NJsonSchema.Validation.ValidationError> errors;
            try
            {
                errors = schema.Validate(input.Json, (NJsonSchema.SchemaType)schemaType);
            }
            catch (Exception exception)
            {
                if (options.ThrowOnInvalidJson)
                {
                    throw;  // re-throw
                }
                errors = new List<NJsonSchema.Validation.ValidationError>();
                while (exception != null)
                {
                    errors.Add(new NJsonSchema.Validation.ValidationError(NJsonSchema.Validation.ValidationErrorKind.Unknown, exception.Message, null, null, null));
                    exception = exception.InnerException;
                }
            }
            if (errors.Count > 0 && options.ThrowOnInvalidJson)
            {
                throw new JsonException($"Json is not valid. {string.Join(";", errors.Select(e => e.ToString()))}");
            }
            return new ValidateResult()
            {
                Errors = errors,
                IsValid = errors.Count == 0
            };
        }
    }
}
