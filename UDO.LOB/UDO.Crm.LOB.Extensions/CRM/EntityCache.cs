using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Concurrent;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.Extensions
{
    public static class EntityCache
    {
        private static ConcurrentDictionary<string, EntityMetadata> _entityMetaDataCollection;

        private static readonly string orgName;
        public static string OrgName
        {
            get
            {
                return orgName;
            }
            private set
            {
                
            }
        }

        static EntityCache()
        {
            string crmUrl = ConnectionCache.ConnectManager.BaseUrl;
            if (!string.IsNullOrEmpty(crmUrl))
            {
                orgName = new Uri(crmUrl).Host.Split(new char[] { '.' })[0];
            }
        }

        public static bool LoadEntityCache()
        {
            try
            {
                bool loaded = false;

                if (OrgName != null)
                {
                    _entityMetaDataCollection = new ConcurrentDictionary<string, EntityMetadata>();
                }

                loaded = true;
                return loaded;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(OrgName, Guid.Empty, "LoadEntityCache", ex.Message);
                throw ex;
            }
        }

        private static ConcurrentDictionary<string, EntityMetadata> GetEntityMetaData(string org, Guid userId, bool forceRefresh = false)
        {
            try
            {
                if (forceRefresh)
                {
                    _entityMetaDataCollection = new ConcurrentDictionary<string, EntityMetadata>();
                }

                return _entityMetaDataCollection;
            } 
            catch (Exception ex)
            {
                LogHelper.LogError(OrgName, Guid.Empty, "GetEntityMetaData", ex.Message);
                throw ex;
            }
        }

        public static AttributeMetadata[] GetAttributes(string org, string entityName, Guid UserId)
        {
            return GetAttributes(org, entityName, UserId, false);
        }

        public static AttributeMetadata[] GetAttributes(string org, string entityName, Guid UserId, bool forceRefresh)
        {
            try
            {
                ConcurrentDictionary<string, EntityMetadata> metadataCollection = GetEntityMetaData(org, UserId, forceRefresh);
                EntityMetadata targetEntity = metadataCollection.GetOrAdd(entityName, (k) => RetrieveEntityMetadataFromCrm(org, entityName, UserId));

                return targetEntity.Attributes;
            } 
            catch (Exception ex)
            {
                LogHelper.LogError(org, UserId, "GetAttributes", ex.Message);
                throw ex;
            }
        }

        public static AttributeMetadata[] GetSingleAttributes(string org, string entityName, Guid UserId)
        {
            try
            {
                ConcurrentDictionary<string, EntityMetadata> metadataCollection = GetEntityMetaData(org, UserId);
                return metadataCollection.GetOrAdd(entityName, (k) => RetrieveEntityMetadataFromCrm(org, entityName, UserId)).Attributes;
            } 
            catch (Exception ex)
            {
                LogHelper.LogError(org, UserId, "GetSingleAttributes", ex.Message);
                throw ex;
            }
        }
        public static OptionSetMetadata GetOptionSet(string org, string entityName, Guid UserId, string attributeName)
        {
            var attributes = GetAttributes(org, entityName, UserId);
            foreach (var a in attributes)
            {
                if (a.LogicalName.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase))
                {
                    var picklist = a as PicklistAttributeMetadata;
                    if (picklist != null) return picklist.OptionSet;
                }
            }
            return null;
        }

        public static string GetOptionSetLabel(string org, string entityName, Guid UserId, string attributeName, int value)
        {
            var optionset = GetOptionSet(org, entityName, UserId, attributeName);

            foreach (var o in optionset.Options)
            {
                if (o.Value.Value == value) return o.Label.UserLocalizedLabel.Label;
            }
            return string.Empty;
        }

        private static EntityMetadata RetrieveEntityMetadataFromCrm(string org, string entityName, Guid UserId)
        {
            #region connect to CRM
            CrmServiceClient OrgServiceProxy;
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(org, UserId, "RetrieveEntityMetadataFromCrm", connectException.Message);
                throw connectException;
            }
            #endregion

            try
            {
                RetrieveEntityRequest request = new RetrieveEntityRequest()
                {
                    EntityFilters = EntityFilters.Attributes,
                    RetrieveAsIfPublished = true,
                    LogicalName = entityName
                };

                // Retrieve the MetaData.
                RetrieveEntityResponse response = (RetrieveEntityResponse)OrgServiceProxy.Execute(request);

                return response.EntityMetadata;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(org, UserId, "RetrieveEntityMetadataFromCrm", ex.Message);
                throw ex;
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }
        }
    }
}
