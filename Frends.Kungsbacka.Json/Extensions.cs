using System;

namespace Frends.Kungsbacka.Json
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {

        /// <summary>
        /// Case insensitive method for finding a string in another string.
        /// </summary>
        /// <param name="str">String to search</param>
        /// <param name="value">String to find</param>
        /// <returns></returns>
        public static bool IContains(this string str, string value)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            return str.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Case insensitive check if two strings contains the same characters.
        /// </summary>
        /// <param name="str">String to compare</param>
        /// <param name="value">String to compare with</param>
        /// <returns></returns>
        public static bool IEquals(this string str, string value)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            return str.Equals(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
