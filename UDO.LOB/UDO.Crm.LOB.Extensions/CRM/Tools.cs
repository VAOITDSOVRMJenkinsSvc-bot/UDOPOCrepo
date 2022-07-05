using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using UDO.LOB.Core.Interfaces;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.Extensions
{
    public static class Tools
    {
        /// <summary>
        /// Creates a new exception response with the message using the IUDOException interface
        /// </summary>
        /// <typeparam name="T">IUDOException response message</typeparam>
        /// <param name="message">message</param>
        /// <param name="args">paramters for message</param>
        /// <returns></returns>
        public static T Exception<T>(string message, params object[] args) where T : IUDOException, new()
        {
            var response = new T();
            response.ExceptionOccurred = true;
            response.ExceptionMessage = string.Format(message, args);
            return response;
        }

        /// <summary>
        /// Truncate a string to the specified length
        /// </summary>
        /// <param name="value">The string to truncate</param>
        /// <param name="maxLength">The maximum length of the string</param>
        /// <returns></returns>
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        /// Convert Text Date to Date Parsable string
        /// </summary>
        /// <param name="date">The string date</param>
        /// <returns></returns>
        public static string ToDateStringFormat(this string date)
        {
            if (date == null) return null;
            if (date.Length == 10) return date;
            if (date.Length < 8) return date;

            date = date.Insert(2, "/");
            date = date.Insert(5, "/");
            return date;
        }

        /// <summary>
        /// Tries to format a given string to mm/dd/yyyy. If the conversion fails, the original string is passed back.
        /// </summary>
        /// <param name="date">String to be converted to mm/dd/yyyy</param>
        /// <param name="format">The format in which the date string is passed, such as yyyyMMdd or MMddyyyy</param>
        /// <returns>Returns a given date string to MM/dd/yyyy format.</returns>
        public static string ToDateStringFormat(this string date, string format)
        {
            try
            {
                var dateTime = DateTime.ParseExact(date, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
                return dateTime.ToString("MM/dd/yyyy");
            }
            catch (FormatException dateFormatException)
            {
                LogHelper.LogError("UDO", Guid.Empty, "ToDateStringFormat", dateFormatException.Message);
                //If date cannot be reformatted return the date present in the system.
                return date;
            }
            catch (ArgumentException dateArgumentException)
            {
                LogHelper.LogError("UDO", Guid.Empty, "ToDateStringFormat", dateArgumentException.Message);
                //If date cannot be reformatted return the date present in the system.
                return date;
            }
        }

        /// <summary>
        /// Convert a DateTime to a valid CRM Date.  If less than 1/1/1900, converts date to 1/1/1900.
        /// 
        /// DateOnly fields have the time set to 11:59:00 and the timezone set the UTC, so that regardless of the time
        /// zone the date will always be the same.
        /// </summary>
        /// <param name="date">The date</param>
        /// <param name="attr">The Date Time Attribute Metadata</param>
        /// <returns></returns>
        public static DateTime ToCRMDateTime(this DateTime date, DateTimeAttributeMetadata attr)
        {
            if (attr != null && attr.Format.HasValue) return date.ToCRMDateTime(attr.Format.Value);
            return date.ToCRMDateTime();
        }

        /// <summary>
        /// Convert a DateTime to a valid CRM Date.  If less than 1/1/1900, converts date to 1/1/1900.
        /// </summary>
        /// <param name="date">The date</param>
        /// <param name="format">The Format (ex:DateOnly)</param>
        /// <returns></returns>
        public static DateTime ToCRMDateTime(this DateTime date, DateTimeFormat format)
        {
            if (format == DateTimeFormat.DateOnly)
            {
                date = date.ToUniversalTime(); //assume DateOnly fields were passed as UTC values
                date = new DateTime(date.Year, date.Month, date.Day, 11, 59, 00, DateTimeKind.Utc);
                return date.ToCRMDateTime();
            }
            return date.ToCRMDateTime();
        }

        /// <summary>
        /// Convert a DateTime to a valid CRM Date.  If less than 1/1/1900, converts date to 1/1/1900.
        /// </summary>
        /// <param name="date">The date</param>
        /// <returns></returns>
        public static DateTime ToCRMDateTime(this DateTime date)
        {
            var minDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return date > minDate ? date : minDate;
        }

        /// <summary>
        /// Replace part of a string with a single character
        /// </summary>
        /// <param name="input"></param>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="replaceWithCharacter"></param>
        /// <returns></returns>
        public static String ReplaceSegment(this string input, int startPosition = 0, int endPosition = 0, char replaceWithCharacter = '*')
        {
            // Replace(input,3) will result in Replace(input,0,3)
            // Replace(input,-3) will result in Replace(input,0,-3) 
            // because it will calculate the end, then see that it is greater
            if (startPosition == 0 && endPosition == 0) endPosition = input.Length;
            if (endPosition < 0)
            {
                endPosition = endPosition + input.Length;
            }
            if (startPosition < 0)
            {
                startPosition = startPosition + input.Length;
            }
            if (endPosition < startPosition)
            {
                var tmp = startPosition;
                startPosition = endPosition;
                endPosition = tmp;
            }
            if (endPosition == 0) endPosition = input.Length;
            if (endPosition < startPosition)
            {
                var tmp = startPosition;
                startPosition = endPosition;
                endPosition = tmp;
            }
            //Replace("ABCDEFGHIJKLMNOP",1,3,'-');
            //Result: "A--DEFGHIJKLMNOP"
            return input.Substring(0, startPosition) +
                   new String(replaceWithCharacter, endPosition - startPosition) +
                   ((endPosition < input.Length) ? input.Substring(endPosition) : "");
        }

        /// <summary>
        /// DumpToString: This dumps an entity to a string, which can then be logged.
        /// </summary>
        /// <param name="name">The name or type of the entity</param>
        /// <param name="entity">The entity to dump</param>
        /// <returns></returns>
        public static string DumpToString(this Entity entity, string name = null)
        {
            string nulltext = "<null>";
            if (String.IsNullOrEmpty(name))
            {
                name = "Entity";
                if (entity != null) name = entity.LogicalName;
            }

            if (entity == null) return name + ": " + nulltext;

            StringBuilder entityDump = new StringBuilder();
            entityDump.AppendFormat("{0} [entity:{1} id:{2}]",
                name, entity.LogicalName, entity.Id);

            foreach (var key in entity.Attributes.Keys)
            {
                //Append Name
                entityDump.AppendFormat("{0}: ", key);

                var attributeObj = entity[key];

                if (attributeObj == null)
                {
                    entityDump.AppendFormat("{0}\r\n", nulltext);
                    continue;
                }

                if (attributeObj is AliasedValue)
                {
                    if (((AliasedValue)attributeObj).Value == null)
                    {
                        entityDump.AppendFormat("{0}\r\n", nulltext);
                        continue;
                    }
                    else
                    {
                        entityDump.AppendFormat("{0}: ", ((AliasedValue)attributeObj).AttributeLogicalName);
                        attributeObj = ((AliasedValue)attributeObj).Value;
                    }
                }

                var attributeValue = attributeObj.ToString();
                
                if (attributeObj is Money)
                {
                    var attrMoney = (Money)attributeObj;
                    attributeValue = attrMoney.Value.ToString();
                }
                else if (attributeObj is OptionSetValue)
                {
                    var attrOptionSet = (OptionSetValue)attributeObj;
                    attributeValue = attrOptionSet.Value.ToString();
                }
                else if (attributeObj is EntityReference)
                {
                    attributeValue = string.Empty;
                    var attrLookup = (EntityReference)attributeObj;
                    attributeValue = string.Format("{0}[entity:{1} id:{2}]",
                        String.IsNullOrEmpty(attrLookup.Name) ? "" : attrLookup.Name + " ",
                        attrLookup.LogicalName, attrLookup.Id);
                }

                if (attributeValue.Length > 200 && !(attributeObj is EntityReference))
                {
                    attributeValue = "\r\n" + attributeValue;
                }

                if (entity.FormattedValues.ContainsKey(key))
                {
                    attributeValue += String.Format(" ({0})", entity.FormattedValues[key]);
                }

                // Append Value
                entityDump.AppendFormat("{0}\r\n", attributeValue);
            }

            return entityDump.ToString();
        }

        /// <summary>
        /// Returns whether or not the first letter in a string is a vowel (a e i o u) but not y
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool FirstLetterIsVowel(this string text)
        {
            var letter = text[0].ToString().ToLowerInvariant()[0];
            return (letter == 'a' || letter == 'e' || letter == 'i' || letter == 'o' || letter == 'u');
        }

        public static string Mask(string input, int pos = 0, char maskchar = '*')
        {
            if (pos == 0) return new String(maskchar, input.Length);
            var maskpos = pos;

            if (pos < 0)
            {
                maskpos = input.Length + pos;
            }

            return new String(maskchar, maskpos) + input.Substring(maskpos);
        }
        
        /// <summary>
        /// Format Telephone 
        /// </summary>
        /// <param name="telephoneNumber">Input Telephone Number</param>
        /// <returns>Formatted Telephone Number</returns>
        public static string ToTelephoneFormat(this string telephoneNumber)
        {
            var phone = telephoneNumber;
            var ext = "";
            var result = "";

            if (0 != phone.IndexOf('+'))
            {
                if (1 < phone.LastIndexOf('x'))
                {
                    ext = phone.Substring(phone.LastIndexOf('x'));
                    phone = phone.Substring(0, phone.LastIndexOf('x'));
                }
            }
            //Phone = Phone.Replace(/[^\d]/gi, "");
            result = phone;
            if (7 == phone.Length)
            {
                result = phone.Substring(0, 3) + "-" + phone.Substring(3);
            }
            if (10 == phone.Length)
            {
                result = "(" + phone.Substring(0, 3) + ") " + phone.Substring(3, 3) + "-" + phone.Substring(6);
            }
            if (0 < ext.Length)
            {
                result = result + " " + ext;
            }
            return result;

        }

        public static Money ToCurrencyFormat(this string fieldValue)
        {
            Decimal thisValue;
            var fieldData = fieldValue;
            if (fieldData.StartsWith("$"))
            {
                fieldData = fieldData.Substring(1);
            }
            Decimal.TryParse(fieldData, out thisValue);
            return new Money(thisValue);
        }

        /// <summary>
        /// Populate Field from Entity Field
        /// </summary>
        /// <param name="thisNewEntity">Populating Entity</param>
        /// <param name="fieldName">Populating Entity Field</param>
        /// <param name="sourceEntity">Source Entity</param>
        /// <param name="fieldValue">Source Entity Field</param>
        public static void PopulateFieldfromEntity(Entity thisNewEntity, string fieldName, Entity sourceEntity, string fieldValue)
        {
            if (sourceEntity.Attributes.Contains(fieldValue))
            {
                thisNewEntity[fieldName] = sourceEntity[fieldValue];
            }
        }

        public static void PopulateFieldfromEntityToCurrency(Entity thisNewEntity, string fieldName, Entity sourceEntity, string fieldValue)
        {
            if (sourceEntity.Attributes.Contains(fieldValue))
            {
                var thisValue = new Decimal();
                var fieldData = sourceEntity[fieldValue].ToString();
                if (fieldData.StartsWith("$"))
                {
                    fieldData = fieldData.Substring(1);
                }
                decimal.TryParse(fieldData, out thisValue);
                thisNewEntity[fieldName] = new Money(thisValue);
            }
        }
        
        public static void PopulateDateField(Entity thisNewEntity, string fieldName, string fieldValue)
        {
            if (!string.IsNullOrEmpty(fieldValue))
            {
                DateTime newDateTime;
                if (DateTime.TryParse(fieldValue, out newDateTime))
                {
                    if (newDateTime != System.DateTime.MinValue)
                    {

                        thisNewEntity[fieldName] = newDateTime;
                    }
                }
            }
        }

        public static string ToMoneyFormat(this string thisField)
        {
            var returnField = "";
            try
            {
                double newValue = 0;
                if (double.TryParse(thisField, out newValue))
                {
                    returnField = string.Format("{0:C}", newValue);
                }
                else
                {
                    returnField = "$0.00";
                }
            }
            catch (Exception ex)
            {
                returnField = ex.Message;
            }
            return returnField;

        }

        public static string TrimWhiteSpace(this string text)
        {
            if (text == null) return null;
            if (String.IsNullOrWhiteSpace(text)) return string.Empty;
            return Regex.Replace(text, @"\s+", " ");
        }

        public static string ToLongBranchOfService(this string branchcode)
        {
            //JS switch (branchcode.trim())
            switch (branchcode.Trim())
            {
                case "AF": return "AIR FORCE (AF)";
                case "A": return "ARMY (ARMY)";
                //ARMY AIR CORPS
                case "CG": return "COAST GUARD (CG)";
                case "CA": return "COMMONWEALTH ARMY (CA)";
                case "GCS": return "GUERRILLA AND COMBINATION SVC (GCS)";
                case "M": return "MARINES (M)";
                case "MM": return "MERCHANT MARINES (MM)";
                case "NOAA": return "NATIONAL OCEANIC & ATMOSPHERIC ADMINISTRATION (NOAA)";
                //NAVY (NAVY)
                case "PHS": return "PUBLIC HEALTH SVC (PHS)";
                case "RSS": return "REGULAR PHILIPPINE SCOUT (RSS)";
                //REGULAR PHILIPPINE SCOUT COMBINED WITH SPECIAL
                case "RPS": return "PHILIPPINE SCOUT OR COMMONWEALTH ARMY SVC (RPS)";
                case "SPS": return "SPECIAL PHILIPPINE SCOUTS (SPS)";
                case "WAC": return "WOMEN'S ARMY CORPS (WAC)";
            }
            return branchcode;
        }

    }
}
