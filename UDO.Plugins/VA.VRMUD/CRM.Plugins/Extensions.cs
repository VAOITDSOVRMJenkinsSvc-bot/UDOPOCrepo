using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRM.Plugins {
    public static class Extensions {
        public static string TrimEnd(this string buffer, string trim) {
            return buffer.TrimEnd(trim.ToCharArray());
        }

        public static string SafeReplace(this string input, string find, string replace) {
            return string.IsNullOrEmpty(input) ? input : input.Replace(find, replace);
        }

        public static string[] SafeSplit(this string input, string delimiter) {
            if (input.IsNullOrEmpty()) {
                return new string[0];
            } else {
                return input.Split(delimiter.ToCharArray());
            }
        }

        public static string Truncate(this string buffer, int maxLength) {
            if (buffer.IsNullOrEmpty()) {
                return string.Empty;
            }

            if (buffer.Length > maxLength) {
                return buffer.Substring(0, maxLength) + "...";
            } else {
                return buffer;
            }
        }

        public static string SafeTrim(this string value) {
            if (!value.IsNullOrEmpty()) {
                return value.Trim();
            }

            return string.Empty;
        }

        public static string NullIfEmpty(this string value) {
            if (!string.IsNullOrWhiteSpace(value)) {
                return value.Trim();
            }

            return null;
        }

        public static string EmptyIfNull(this string s) {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            else
                return s;
        }

        public static bool MatchesAnyString(this string container, params string[] values) {
            if (values == null || container.IsNullOrEmpty()) {
                return false;
            } else {
                container = container.Trim();
                return values.Any(t => string.Compare(container, t.Trim(), true) == 0);
            }
        }

        public static bool MatchesAny<T>(this T container, params T[] values) {
            if (values == null || container == null) {
                return false;
            }

            return values.Contains(container);
        }

        public static bool IsNullOrEmpty(this string buffer) {
            return string.IsNullOrEmpty(buffer);
        }

        public static bool IsNullOrWhiteSpace(this string buffer) {
            return string.IsNullOrWhiteSpace(buffer);
        }

        public static string FormatWith(this string format, params object[] args) {
            return string.Format(format, args);
        }

        public static T ShallowCopy<T, U>(this U entity) where T : new() {
            T clone = new T();

            // copy base class properties.
            foreach (var prop in typeof(U).GetProperties()) {
                var prop2 = typeof(T).GetProperty(prop.Name);
                if (prop2 != null && prop.CanWrite && prop.CanRead)
                    prop2.SetValue(clone, prop.GetValue(entity, null), null);
            }

            return clone;
        }

       
        public static string ToYesNo(this bool value) {
            return value ? "Yes" : "No";
        }

        public static T ConvertObject<T>(this object obj) {
            if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value)) {
                return default(T);
            }

            return (T)obj;
        }

        public static void ThrowOnNull(this object myself) {
            if (null == myself) {
                throw new ArgumentNullException();
            }
        }

        public static void ThrowOnNull(this object myself, string parameter = null, string message = null) {
            if (null == myself) {
                throw new ArgumentNullException(parameter ?? "N/A", message ?? "Argument cannot be null.");
            }
        }

        public static void ThrowOnNullOrWhitespace(this string myself, string message = null) {
            if (null == myself) {
                throw new ArgumentNullException(message ?? "Argument cannot be null.");
            } else if (myself.Trim().Length == 0) {
                throw new ArgumentException(message ?? "Argument cannot contain only whitespace.");
            }
        }

        public static bool IsNullOrWhitespace(this string myself) {
            return null == myself || myself.Trim().Length == 0;
        }

        public static string TrimOrEmptyOnNull(this string myString) {
            return (myString ?? string.Empty).Trim();
        }

        public static string RemoveWhitespace(this string thisString) {
            var whitespaceChars = "\n\r\t ";

            var newChars = thisString.ToCharArray().Where(c => whitespaceChars.IndexOf(c) < 0);

            return new String(newChars.ToArray());
        }

        public static string ToFirstLetterCapitalized(this string thisString) {
            var stringParts = (thisString ?? string.Empty).Split(' ');
            var builder = new StringBuilder();

            foreach (var stringPart in stringParts) {
                var trimmedString = stringPart.Trim();

                if (trimmedString.Length > 0) {
                    builder.Append(trimmedString.Substring(0, 1).ToUpper());

                    if (trimmedString.Length > 1) {
                        builder.Append(trimmedString.Substring(1).ToLower());
                    }

                    builder.Append(' ');
                }
            }

            return builder.ToString().Trim();
        }

    }
    
}
