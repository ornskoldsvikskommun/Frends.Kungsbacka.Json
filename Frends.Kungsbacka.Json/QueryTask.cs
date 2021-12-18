using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace Frends.Kungsbacka.Json
{
    /// <summary>
    /// JsonSchema Tasks
    /// </summary>
    public static class QueryTask
    {
        /// <summary>
        /// Query a json string / json token for a single result. See https://github.com/FrendsPlatform/Frends.Json
        /// </summary>
        /// <returns>JToken</returns>
        public static object QuerySingle([PropertyTab] QueryInput input, [PropertyTab] QueryOptions options)
        {
            JToken jToken = TaskHelper.GetJTokenFromInput(input.Json);

            return jToken.SelectToken(input.Query, options.ErrorWhenNotMatched);
        }

        /// <summary>
        /// Query a json string / json token. See https://github.com/FrendsPlatform/Frends.Json
        /// </summary>
        /// <returns>JToken[]</returns>
        public static IEnumerable<object> Query([PropertyTab] QueryInput input, [PropertyTab] QueryOptions options)
        {
            JToken jToken = TaskHelper.GetJTokenFromInput(input.Json);

            return jToken.SelectTokens(input.Query, options.ErrorWhenNotMatched);
        }
    }
}
