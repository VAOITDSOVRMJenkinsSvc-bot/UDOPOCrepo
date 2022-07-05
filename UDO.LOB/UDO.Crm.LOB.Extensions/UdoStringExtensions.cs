namespace UDO.LOB.Extensions
{
    /// <summary>
    /// UDO extension methods for <see cref="string"/>.
    /// </summary>
    public static class UdoStringExtensions
    {
        /// <summary>
        /// Returns the value as a not null boolean.
        /// If the value can't be converted, then false is returned.
        /// </summary>
        /// <param name="value">The value. NOTE: can be null or empty string.</param>
        /// <returns>The value as a not null boolean.
        /// If the value can't be converted, then false is returned.</returns>
        public static bool AsNotNullBoolean(this string value)
        {
            if (bool.TryParse(value, out var result))
            {
                return result;
            }

            return false;
        }

        /// <summary>
        /// Returns the value as a not null string.
        /// If the value is null, then an empty string "" is returned.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A not null string.</returns>
        public static string AsNotNullString(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value;
        }

        /// <summary>
        /// Returns null or the value.
        /// If the value is null, then a null is returned.
        /// If the value is an empty string "", then a null is returned.
        /// If the value is a non empty string, then the non empty string is returned.
        /// </summary>
        /// <param name="value">The value. NOTE: can be null or empty string ""</param>
        /// <returns>null or the value.
        /// If the value is null, then a null is returned.
        /// If the value is an empty string "", then a null is returned.
        /// If the value is a non empty string, then the non empty string is returned.
        /// </returns>
        public static string AsNullableString(this string value) =>
            string.IsNullOrEmpty(value) ? null : value;

        /// <summary>
        /// Returns null or a new string that is the result of trimming the specified string.
        /// If the value is null, then null is returned.
        /// If the result of the trimming the value is an empty string "", then null is returned.
        /// </summary>
        /// <param name="value">The value. NOTE: can be null or empty string ""</param>
        /// <returns>
        /// null or a new string that is the result of trimming the specified string.
        /// If the value is null, then null is returned.
        /// If the result of the trimming the value is an empty string "", then null is returned.</returns>
        public static string AsTrimmedNullableString(this string value)
        {
            // if input value is null
            // set trimmedString to null
            // otherwise, set trimmedString to the trimmed string
            var trimmedString = value?.Trim();
            return trimmedString.AsNullableString();
        }
    }
}
