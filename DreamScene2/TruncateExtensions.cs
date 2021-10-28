using System;

namespace Humanizer
{
    /// <summary>
    /// Allow strings to be truncated
    /// </summary>
    public static class TruncateExtensions
    {
        /// <summary>
        /// Truncate the string
        /// </summary>
        /// <param name="input">The string to be truncated</param>
        /// <param name="length">The length to truncate to</param>
        /// <param name="truncationString">The string used to truncate with</param>
        /// <param name="from">The enum value used to determine from where to truncate the string</param>
        /// <returns>The truncated string</returns>
        public static string Truncate(this string input, int length, string truncationString = "…", TruncateFrom from = TruncateFrom.Right)
        {
            if (input.Length > length)
            {
                if (from== TruncateFrom.Left)
                {
                    string v = input.Substring(0, length - truncationString.Length);
                    return v + truncationString;
                }
                else if (from == 0)
                {
                    int v = (length - truncationString.Length) / 2;
                    string v1 = input.Substring(0, v);
                    string v2 = input.Substring(input.Length - v, v);
                    return v1 + truncationString + v2;
                }
                else if (from== TruncateFrom.Right)
                {
                    string v = input.Substring(input.Length - length + truncationString.Length, length - truncationString.Length);
                    return truncationString + v;
                }
            }
            return input;
        }
    }

    /// <summary>
    /// Truncation location for humanizer
    /// </summary>
    public enum TruncateFrom
    {
        /// <summary>
        /// Truncate letters from the left (start) of the string
        /// </summary>
        Left,
        /// <summary>
        /// Truncate letters from the right (end) of the string
        /// </summary>
        Right
    }
}
