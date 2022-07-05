using Microsoft.Xrm.Sdk;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

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
            return _preEntity.GetAttributeValue<T>(attribute);
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

        public static string DumpEntityToString(string name, Entity entity)
        {
            string nulltext = "<null>";

            string entityDump = String.Format("{0} [entity:{1} id:{2}]",
                name,entity.LogicalName, entity.Id);

            foreach (var attributeName in entity.Attributes.Keys)
            {
                var attributeObj = entity[attributeName];
                var attributeValue = attributeObj.ToString();
                if (attributeObj == null) attributeValue = nulltext;

                if (attributeObj is AliasedValue)
                {
                    if (((AliasedValue)attributeObj).Value == null)
                    {
                        attributeValue = "<null>";
                    }
                    else
                    {
                        attributeObj = ((AliasedValue)attributeObj).Value;
                    }
                }

                if (attributeObj is OptionSetValue)
                {
                    var attrOptionSet = (OptionSetValue)attributeObj;
                    attributeValue = attrOptionSet.Value.ToString();
                }
                else if (attributeObj is EntityReference)
                {
                    attributeValue = string.Empty;
                    var attrLookup = (EntityReference)attributeObj;
                    attributeValue = string.Format("{0}{1}[entity:{2} id:{3}]",
                        attrLookup.Name, String.IsNullOrEmpty(attrLookup.Name) ? "" : " ",
                        attrLookup.LogicalName, attrLookup.Id);
                }

                if (attributeValue.Length > 200 && !(attributeObj is EntityReference))
                {
                    attributeValue = "\r\n" + attributeValue;
                }

                if (entity.FormattedValues.ContainsKey(attributeName))
                {
                    attributeValue += String.Format(" ({0})", entity.FormattedValues[attributeName]);
                }

                entityDump += String.Format("{0}{1}: {2}",
                    (String.IsNullOrEmpty(entityDump) ? "" : "\r\n"),
                    attributeName, attributeValue);
            }
            return entityDump;
        }

        public static byte[] ConvertToSecureString(string sensitiveString)
        {
            if (String.IsNullOrEmpty(sensitiveString)) sensitiveString = string.Empty;
            return Encoding.UTF8.GetBytes(sensitiveString);
        }

        public static string ConvertToUnsecureString(byte[] secureString)
        {
            return Encoding.UTF8.GetString(secureString);
        }

        internal static byte[] AppendToSecureString(byte[] securedByte, string sensitiveString)
        {
            return ConvertToSecureString(String.Format("{0}{1}",ConvertToUnsecureString(securedByte), sensitiveString));
        }
        #endregion
    }
}
