using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.LOB.PeoplelistPayeeCode.CRM
{
    public static class XrmSetAttributeValue
    {
        public static object SetAttributeValue(this Entity e, Type T, string attribute, params object[] values)
        {
            bool setIfEmpty = false;
            if (values.Length < 1) return null;
            var value = values[0];

            if (!setIfEmpty && value == null) return null;

            switch (T.ToString())
            {
                case "System.Nullable`1[System.Double]":
                case "System.Double?":
                case "System.Double":
                    double d = 0;
                    if (Double.TryParse(value.ToString(), out d))
                    {
                        e[attribute] = d;
                        return e[attribute];
                    }
                    return null;
                case "System.Nullable`1[System.Int32]":
                case "System.Nullable`1[System.Int16]":
                case "System.Nullable`1[System.Int64]":
                case "System.Int32?":
                case "System.Int16?":
                case "System.Int64?":
                case "System.Int32":
                case "System.Int16":
                case "System.Int64":
                    int ival = 0;
                    if (int.TryParse(value.ToString(), out ival))
                    {
                        e[attribute] = ival;
                        return e[attribute];
                    }
                    return null;
                case "System.Nullable`1[System.DateTime]":
                case "System.DateTime":
                case "System.DateTime?":
                    // This should never clean for CRM Date and Time.  Allow EM to do that.
                    if (values.Length == 2) setIfEmpty = (bool)values[1];
                    if (value is string)
                    {
                        var dateString = value.ToString();
                        if (String.IsNullOrEmpty(dateString) && !setIfEmpty) return null;
                        DateTime newDateTime;
                        if (!dateString.Contains('/') && dateString.Length > 7 && dateString.Length < 11)
                        {
                            dateString = dateString.Insert(2, "/").Insert(5, "/");
                        }

                        if (DateTime.TryParse(dateString, out newDateTime))
                        {
                            if (newDateTime.Year == 1) return null;
                            value = newDateTime;
                        }
                    }
                    if (value == null) return null;
                    if (value is DateTime)
                    {
                        e[attribute] = value;
                        return e[attribute];
                    }
                    return null;
                case "System.String":
                    if (values.Length == 2) setIfEmpty = (bool)values[1];
                    var result = value.ToString();
                    if (String.IsNullOrEmpty(result) && !setIfEmpty) return null;
                    e[attribute] = result;
                    return e[attribute];
                case "Microsoft.Xrm.Sdk.Money":
                    if (value is Money)
                    {
                        e[attribute] = (Money)value;
                    }
                    decimal money = 0;
                    if (Decimal.TryParse(value.ToString(), out money))
                    {
                        e[attribute] = new Money(money);
                        return e[attribute];
                    }
                    return null;
                case "Microsoft.Xrm.Sdk.OptionSetValue":
                    int selected = -1;
                    if (value is OptionSetValue)
                    {
                        selected = ((OptionSetValue)value).Value;
                    }
                    else
                    {
                        if (!Int32.TryParse(value.ToString(), out selected)) return null;
                    }
                    if (values.Length == 2) setIfEmpty = (bool)values[1];
                    if (selected == -1)
                    {
                        if (setIfEmpty)
                        {
                            e[attribute] = null;
                            return null;
                        }
                        return null;
                    }
                    e[attribute] = new OptionSetValue(selected);
                    return e[attribute];
                case "Microsoft.Xrm.Sdk.EntityReference":
                    EntityReference lookup = null;
                    if (value is string)
                    {
                        Guid id = Guid.Empty;
                        if (values.Length > 1) id = (Guid)values[1];
                        if (values.Length == 3) setIfEmpty = (bool)values[2];
                        if (!setIfEmpty && id == Guid.Empty) return null;
                        lookup = new EntityReference(value.ToString(), id);
                    }
                    else if (value is EntityReference)
                    {
                        lookup = (EntityReference)value;
                        if (values.Length == 2) setIfEmpty = (bool)values[1];
                    }
                    if (!setIfEmpty && lookup != null && lookup.Id == Guid.Empty) return null;
                    e[attribute] = lookup;
                    return e[attribute];
                default:
                    throw new ArgumentException("Invlaid Type Passed to SetAttributeValue");
            }
            return null;
        }

        public static T SetAttributeValue<T>(this Entity e, string attribute, params object[] values)
        {
            var value = SetAttributeValue(e, typeof(T), attribute, values);
            if (value == null) return default(T);
            return (T)value;
/*            bool setIfEmpty = false;
            if (values.Length < 1) return default(T);
            var value = values[0];

            if (!setIfEmpty && value == null) return default(T);

            switch (typeof(T).ToString())
            {
                case "System.Double":
                    double d = 0;
                    if (Double.TryParse(value.ToString(), out d))
                    {
                        e[attribute] = d;
                        return (T)e[attribute];
                    }
                    return default(T);
                case "System.Int32":
                case "System.Int16":
                case "System.Int64":
                    int ival = 0;
                    if (int.TryParse(value.ToString(), out ival))
                    {
                        e[attribute] = ival;
                        return (T)e[attribute];
                    }
                    return default(T);
                case "System.DateTime":
                case "System.DateTime?":
                    // This should never clean for CRM Date and Time.  Allow EM to do that.
                    if (values.Length == 2) setIfEmpty = (bool)values[1];
                    if (value is string)
                    {
                        var dateString = value.ToString();
                        if (String.IsNullOrEmpty(dateString) && !setIfEmpty) return default(T);
                        DateTime newDateTime;
                        if (dateString.Length > 7 && dateString.Length < 11)
                        {
                            dateString = dateString.Insert(2, "/").Insert(5, "/");
                        }

                        if (DateTime.TryParse(dateString, out newDateTime))
                        {
                            value = newDateTime;
                        }
                    }
                    if (value == null) return default(T);
                    if (value is DateTime)
                    {
                        e[attribute] = value;
                        return (T)e[attribute];
                    }
                    return default(T);
                case "System.String":
                    if (values.Length == 2) setIfEmpty = (bool)values[1];
                    var result = value.ToString();
                    if (String.IsNullOrEmpty(result) && !setIfEmpty) return default(T);
                    e[attribute] = result;
                    return (T)e[attribute];
                case "Microsoft.Xrm.Sdk.Money":
                    if (value is Money)
                    {
                        e[attribute] = (Money)value;
                    }
                    decimal money = 0;
                    if (Decimal.TryParse(value.ToString(), out money))
                    {
                        e[attribute] = new Money(money);
                        return (T)e[attribute];
                    }
                    return default(T);
                case "Microsoft.Xrm.Sdk.OptionSetValue":
                    int selected = -1;
                    if (value is OptionSetValue)
                    {
                        selected = ((OptionSetValue)value).Value;
                    }
                    else
                    {
                        if (!Int32.TryParse(value.ToString(), out selected)) return default(T);
                    }
                    if (values.Length == 2) setIfEmpty = (bool)values[1];
                    if (selected == -1)
                    {
                        if (setIfEmpty)
                        {
                            e[attribute] = null;
                            return default(T);
                        }
                        return default(T);
                    }
                    e[attribute] = new OptionSetValue(selected);
                    return (T)e[attribute];
                case "Microsoft.Xrm.Sdk.EntityReference":
                    EntityReference lookup = null;
                    if (value is string)
                    {
                        Guid id = Guid.Empty;
                        if (values.Length > 1) id = (Guid)values[1];
                        if (values.Length == 3) setIfEmpty = (bool)values[2];
                        if (!setIfEmpty && id == Guid.Empty) return default(T);
                        lookup = new EntityReference(value.ToString(), id);
                    }
                    else if (value is EntityReference)
                    {
                        lookup = (EntityReference)value;
                        if (values.Length == 2) setIfEmpty = (bool)values[1];
                    }
                    if (!setIfEmpty && lookup.Id == Guid.Empty) return default(T);
                    e[attribute] = lookup;
                    return (T)e[attribute];
                default:
                    throw new ArgumentException("Invlaid Type Passed to SetAttributeValue");
            }
            return default(T);
            */
        }
    }
}
