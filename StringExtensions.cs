namespace CountryCodeKit
{
    /// <summary>
    /// Extensions for string operations
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Checks if a string contains another string using the specified comparison type
        /// </summary>
        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            return source?.IndexOf(value, comparisonType) >= 0;
        }
    }
}