namespace UDO.LOB.Extensions
{
    using System;
    using System.Globalization;
    using System.Text.Json;

    public static class JsonElementExtensions
    {
        /// <summary>
        /// Gets the nullable string.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static string GetNullableString(this JsonElement jsonElement, string attributeName)
        {
            if (jsonElement.TryGetProperty(attributeName, out var rawValue))
            {
                return GetNullableStringInternal(rawValue);
            }

            return null;
        }

        /// <summary>
        /// Gets the nullable int64.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <returns></returns>
        public static long? GetNullableInt64(this JsonElement jsonElement)
        {
            try
            {
                return jsonElement.GetInt64();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the nullable int64.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static long? GetNullableInt64(this JsonElement jsonElement, string attributeName)
        {
            if (jsonElement.TryGetProperty(attributeName, out var rawValue))
            {
                return rawValue.GetNullableInt64();
            }

            return null;
        }

        /// <summary>
        /// Gets the nullable boolean.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static bool? GetNullableBoolean(this JsonElement jsonElement, string attributeName)
        {
            if (jsonElement.TryGetProperty(attributeName, out var rawValue))
            {
                return rawValue.GetNullableBoolean();
            }

            return null;
        }

        /// <summary>
        /// Gets the nullable int32.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static int? GetNullableInt32(this JsonElement jsonElement, string attributeName)
        {
            if (jsonElement.TryGetProperty(attributeName, out var rawValue))
            {
                return rawValue.GetNullableInt32();
            }

            return null;
        }

        /// <summary>
        /// Gets the trimmed nullable string.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static string GetTrimmedNullableString(this JsonElement jsonElement, string attributeName)
        {
            if (jsonElement.TryGetProperty(attributeName, out var rawValue))
            {
                return rawValue.GetTrimmedNullableString();
            }

            return null;
        }

        /// <summary>
        /// Gets the nullable boolean.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <returns></returns>
        public static bool? GetNullableBoolean(this JsonElement jsonElement)
        {
            try
            {
                return jsonElement.GetBoolean();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the nullable date time.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <returns></returns>
        public static DateTime? GetNullableDateTime(this JsonElement jsonElement)
        {
            try
            {
                return jsonElement.GetDateTime();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the nullable date time.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static DateTime? GetNullableDateTime(this JsonElement jsonElement, string attributeName)
        {
            try
            {
                if (jsonElement.TryGetProperty(attributeName, out var rawValue))
                {
                    return rawValue.GetDateTime();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the nullable decimal.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <returns></returns>
        public static decimal? GetNullableDecimal(this JsonElement jsonElement)
        {
            try
            {
                return jsonElement.GetDecimal();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the nullable double.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static double? GetNullableDouble(this JsonElement jsonElement, string attributeName)
        {
            if (jsonElement.TryGetProperty(attributeName, out var rawValue))
            {
                return rawValue.GetNullableDouble();
            }

            return null;
        }

        /// <summary>
        /// Gets the nullable double.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <returns></returns>
        public static double? GetNullableDouble(this JsonElement jsonElement)
        {
            try
            {
                return jsonElement.GetDouble();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the nullable string.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <returns></returns>
        public static string GetNullableString(this JsonElement jsonElement)
        {
            return jsonElement.GetString().AsNotNullString();
        }

        /// <summary>
        /// Gets the trimmed nullable string.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <returns></returns>
        public static string GetTrimmedNullableString(this JsonElement jsonElement)
        {
            return jsonElement.GetString().AsTrimmedNullableString();
        }

        /// <summary>
        /// Gets the nullable int32.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <returns></returns>
        public static int? GetNullableInt32(this JsonElement jsonElement)
        {
            try
            {
                return jsonElement.GetInt32();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the nullable unique identifier.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <returns></returns>
        public static Guid? GetNullableGuid(this JsonElement jsonElement)
        {
            var stringToParse = GetNullableString(jsonElement);
            if (stringToParse is null)
            {
                return null;
            }

            if (Guid.TryParse(stringToParse, out var convertedValue) && convertedValue != Guid.Empty)
            {
                return convertedValue;
            }

            return null;
        }

        /// <summary>
        /// Gets the nullable object.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <returns></returns>
        public static object GetNullableObject(this JsonElement jsonElement)
        {
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.False:
                    return (bool?)false;
                case JsonValueKind.Number:
                    if (jsonElement.TryGetInt32(out var _))
                    {
                        return GetNullableInt32(jsonElement);
                    }
                    else
                    {
                        return GetNullableDouble(jsonElement);
                    }
                case JsonValueKind.String:
                    return GetNullableString(jsonElement);
                case JsonValueKind.True:
                    return (bool?)true;
            }

            return null;
        }

        /// <summary>
        /// Gets the property value as not null string.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static string GetPropertyValueAsNotNullString(this JsonElement jsonElement, string propertyName)
        {
            if (jsonElement.TryGetProperty(propertyName, out var value))
            {
                return value.GetString().AsNotNullString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the property array enumerator.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static JsonElement.ArrayEnumerator GetPropertyArrayEnumerator(this JsonElement jsonElement, string propertyName)
        {
            if (jsonElement.TryGetProperty(propertyName, out var value) &&
                value.ValueKind == JsonValueKind.Array)
            {
                return value.EnumerateArray();
            }

            // if property doesn't exist, or is not an array, return an empty array
            return new JsonElement.ArrayEnumerator();
        }

        /// <summary>
        /// Gets the property array enumerator.
        /// </summary>
        /// <param name="jsonElement">The json element.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="childPropertyName">Name of the child property.</param>
        /// <returns></returns>
        public static JsonElement.ArrayEnumerator GetPropertyArrayEnumerator(this JsonElement jsonElement, string propertyName, string childPropertyName)
        {
            if (jsonElement.TryGetProperty(propertyName, out var value) &&
                value.TryGetProperty(childPropertyName, out var childValue) &&
                childValue.ValueKind == JsonValueKind.Array)
            {
                return childValue.EnumerateArray();
            }

            // if child property doesn't exist, or is not an array, return an empty array
            return new JsonElement.ArrayEnumerator();
        }

        private static string GetNullableStringInternal(JsonElement jsonValue)
        {
            switch (jsonValue.ValueKind)
            {
                case JsonValueKind.False:
                    return "0";

                case JsonValueKind.Number:
                    var parsedInt = jsonValue.GetNullableInt32();
                    if (parsedInt.HasValue)
                    {
                        return parsedInt.Value.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        var parsedDouble = jsonValue.GetNullableDouble();
                        if (parsedDouble.HasValue)
                        {
                            return parsedDouble.Value.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    break;

                case JsonValueKind.String:
                    return jsonValue.GetString().AsNullableString();

                case JsonValueKind.True:
                    return "1";
            }

            return null;
        }
    }
}
