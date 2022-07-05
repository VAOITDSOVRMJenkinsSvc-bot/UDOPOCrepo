using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Linq;

namespace MCSHelperClass
{
    public class MCSHelper
    {
        #region Private Fields
        private Entity _thisEntity;

        private Entity _preEntity;
        #endregion

        #region Constructor
        public MCSHelper(Entity thisEntity, Entity preEntity)
        {
            _thisEntity = thisEntity;

            _preEntity = preEntity;
        }

        public MCSHelper()
        {
        }
        #endregion

        #region Public Methods
        public Entity setThisEntity
        {
            set { _thisEntity = value; }
        }

        public Entity setPreEntity
        {
            set { _preEntity = value; }
        }

        public T GetAttributeValue<T>(string attribute)
        {
            if (_thisEntity.Contains(attribute)) return _thisEntity.GetAttributeValue<T>(attribute);
            if (_preEntity.Contains(attribute)) return _preEntity.GetAttributeValue<T>(attribute);

            return default(T);
        }

        public bool getBoolValue(string attributeLogicalName)
        {
            if (_thisEntity.Attributes.Contains(attributeLogicalName))
            {
                return (bool)_thisEntity[attributeLogicalName];
            }

            if (_preEntity.Attributes.Contains(attributeLogicalName))
            {
                return (bool)_preEntity[attributeLogicalName];
            }

            return new bool();
        }

        public int getIntValue(string attributeLogicalName)
        {
            if (_thisEntity.Attributes.Contains(attributeLogicalName))
            {
                return (int)_thisEntity[attributeLogicalName];
            }

            if (_preEntity.Attributes.Contains(attributeLogicalName))
            {
                return (int)_preEntity[attributeLogicalName];
            }

            return int.MinValue;
        }
        public string getStringValue(string attributeLogicalName)
        {
            if (_thisEntity.Attributes.Contains(attributeLogicalName))
            {
                return _thisEntity[attributeLogicalName].ToString();
            }

            if (_preEntity.Attributes.Contains(attributeLogicalName))
            {
                return _preEntity[attributeLogicalName].ToString();
            }

            return null;
        }
        public DateTime getDateTimeValue(string attributeLogicalName)
        {
            if (_thisEntity.Attributes.Contains(attributeLogicalName))
            {
                return (DateTime)_thisEntity[attributeLogicalName];
            }

            if (_preEntity.Attributes.Contains(attributeLogicalName))
            {
                return (DateTime)_preEntity[attributeLogicalName];
            }

            return DateTime.MinValue;
        }

        public object getObject(string attributeLogicalName)
        {
            if (_thisEntity.Attributes.Contains(attributeLogicalName))
            {
                return _thisEntity[attributeLogicalName];
            }

            if (_preEntity.Attributes.Contains(attributeLogicalName))
            {
                return _preEntity[attributeLogicalName];
            }

            return null;
        }

        public string getStringOptionSetValue(string attributeLogicalName)
        {

            if (_thisEntity.FormattedValues.Contains(attributeLogicalName))
            {
                return _thisEntity.FormattedValues[attributeLogicalName].ToString();
            }

            if (_preEntity.FormattedValues.Contains(attributeLogicalName))
            {
                return _preEntity.FormattedValues[attributeLogicalName].ToString();
            }

            return null;
        }

        public int getOptionSetValue(string attributeLogicalName)
        {
            OptionSetValue myOpt;

            if (_thisEntity.Attributes.Contains(attributeLogicalName))
            {
                myOpt = (OptionSetValue)_thisEntity[attributeLogicalName];

                return myOpt.Value;
            }

            if (_preEntity.Attributes.Contains(attributeLogicalName))
            {
                myOpt = (OptionSetValue)_preEntity[attributeLogicalName];

                return myOpt.Value;
            }

            return 0;
        }

        public string getEntRefName(string attributeLogicalName)
        {
            EntityReference myRef;

            if (_thisEntity.Attributes.Contains(attributeLogicalName))
            {
                myRef = (EntityReference)_thisEntity[attributeLogicalName];

                return myRef.Name;
            }
            if (_preEntity.Attributes.Contains(attributeLogicalName))
            {
                myRef = (EntityReference)_preEntity[attributeLogicalName];

                return myRef.Name;
            }

            return null;
        }

        public Guid getEntRefID(string attributeLogicalName)
        {
            EntityReference myRef;

            if (_thisEntity.Attributes.Contains(attributeLogicalName))
            {
                myRef = (EntityReference)_thisEntity[attributeLogicalName];

                return myRef.Id;
            }
            if (_preEntity.Attributes.Contains(attributeLogicalName))
            {
                myRef = (EntityReference)_preEntity[attributeLogicalName];

                return myRef.Id;
            }

            return Guid.Empty;
        }

        public string getEntRefType(string attributeLogicalName)
        {
            EntityReference myRef;

            if (_thisEntity.Attributes.Contains(attributeLogicalName))
            {
                myRef = (EntityReference)_thisEntity[attributeLogicalName];

                return myRef.LogicalName;
            }

            if (_preEntity.Attributes.Contains(attributeLogicalName))
            {
                myRef = (EntityReference)_preEntity[attributeLogicalName];

                return myRef.LogicalName;
            }

            return null;
        }

        public static bool HasInputColumns(IPluginExecutionContext context, string[] validColumns)
        {
            if (context == null)
                throw new ArgumentNullException("context", "Cannot find ColumnSet to verify validity.  PluginExecutionContext (context) is null.");

            if (context.InputParameters == null)
                throw new ArgumentNullException("InputParameters", "Cannot find InputParameters in PluginExecutionContext.  Unable to verify columns.");

            if (!context.InputParameters.Contains("ColumnSet")) return true;

            var colSet = context.InputParameters["ColumnSet"] as ColumnSet;
            return MCSHelperClass.MCSHelper.HasColumns(colSet, validColumns);
        }

        public static bool HasColumns(ColumnSet colSet, string[] validColumns)
        {
            // All Columns
            if (colSet == null || colSet.AllColumns) return true;

            // Nothing to look for
            if (validColumns == null || validColumns.Length == 0) return true;

            foreach (var col in colSet.Columns)
            {
                if (validColumns.Any(x => x.Equals(col, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return true;
                }
            }

            return false;
        }

        public static string DumpEntityToString(string name, Entity entity)
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

        public static byte[] ConvertToSecureString(string sensitiveString)
        {
            if (String.IsNullOrEmpty(sensitiveString)) return new byte[0];
            return Encoding.UTF8.GetBytes(sensitiveString);
        }

        public static SecureString ConvertToSecureString_new(string sensitiveString)
        {
            if (String.IsNullOrEmpty(sensitiveString)) return new SecureString();
            SecureString rv = new SecureString();
            foreach (char letter in sensitiveString)
                rv.AppendChar(letter);
            return rv;
        }

        public static string ConvertToUnsecureString(byte[] secureString)
        {
            if (secureString == null || secureString.Length == 0) return string.Empty;
            return Encoding.UTF8.GetString(secureString);
        }

        public static string ConvertToUnsecureString(SecureString secureString)
        {
            if (secureString == null || secureString.Length == 0) return string.Empty;
            IntPtr stringPointer = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(secureString);
            string normalString = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(stringPointer);
            System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(stringPointer);
            return normalString;
        }

        internal static byte[] AppendToSecureString(byte[] securedByte, string sensitiveString)
        {
            return ConvertToSecureString(String.Format("{0}{1}", ConvertToUnsecureString(securedByte), sensitiveString));
        }

        internal static SecureString AppendToSecureString(SecureString securedByte, string sensitiveString)
        {
            return ConvertToSecureString_new(String.Format("{0}{1}", ConvertToUnsecureString(securedByte), sensitiveString));
        }
        #endregion
    }
}
