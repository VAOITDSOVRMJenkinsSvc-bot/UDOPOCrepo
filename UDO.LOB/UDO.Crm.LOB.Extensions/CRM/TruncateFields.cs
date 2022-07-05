namespace UDO.LOB.Extensions
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.WebServiceClient;
    using Microsoft.Xrm.Tooling.Connector;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using UDO.LOB.Extensions.Logging;

    /// <summary>
    /// TruncateHelper
    /// </summary>
    public class TruncateHelper
    {
        private static TruncHelperSettings Settings { get; set; }
        private static IOrganizationService OrgServiceProxy { get; set; }

        [Obsolete("Deprecated method. Use the overloaded method instead.", true)]
        public static Entity TruncateFields(Entity thisEntity, string OrgName, Guid userId, bool logTiming)
        {
            return TruncateFields(thisEntity, OrgName, userId, logTiming, string.Empty);
        }

        [Obsolete("Deprecated method. Use the overloaded method instead.", true)]
        public static Entity TruncateFields(Entity thisEntity, string OrgName, Guid userId, bool logTiming, string requestName)
        {
            CrmServiceClient OrgServiceProxy = null;

            var method = MethodInfo.GetCallingMethod(true).GetClassCallerPath(false, true);

            try
            {
                if (Settings == null || DateTime.UtcNow >= Settings.ExpiresOn)
                {
                    #region connect to CRM
                    try
                    {
                        OrgServiceProxy = ConnectionCache.GetProxy();
                        LogHelper.LogInfo($"{MethodInfo.GetThisMethod().Method} :: Invoked CRM2: ConnectionManager.GetConnectionManager(OrgName).GetProxy()");
                    }
                    catch (Exception connectException)
                    {
                        LogHelper.LogError(OrgName, userId, "Truncate Fields Process", connectException.Message);
                        return null;
                    }
                    #endregion

                    Settings = new TruncHelperSettings();
                    Settings = Settings.Load(OrgServiceProxy);

                    var next = DateTime.UtcNow;
                    var mintoadd = 15 - (next.Minute % 15);
                    if (mintoadd < 1) mintoadd = 15;
                    next.AddMinutes(mintoadd);
                    Settings.ExpiresOn = next;
                }

                try
                {
                    Stopwatch txnTimer = Stopwatch.StartNew();
                    AttributeMetadata[] attributes = EntityCache.GetAttributes(OrgName, thisEntity.LogicalName, userId, Settings.ForceMetadataRefresh);
                    //whjen testing, comment out the above and use this
                    //AttributeMetadata[] attributes = EntityCache.GetSingleAttributes(OrgName, thisEntity.LogicalName, userId);


                    if (Settings.ForceMetadataRefresh)
                    {
                        Settings.ForceMetadataRefresh = false;
                        if (OrgServiceProxy != null) Settings.Update(OrgServiceProxy);
                    }
                    txnTimer.Stop();
                    LogHelper.LogTiming(Guid.Empty.ToString(), OrgName, logTiming, userId, Guid.Empty, null, null, "truncate fields, get metadata in MS:" + thisEntity.LogicalName, null, txnTimer.ElapsedMilliseconds);

                    #region truncate my fields
                    txnTimer.Restart();

                    // Get keys (fields) on the Entity
                    var keys = new List<string>();
                    keys.AddRange(thisEntity.Attributes.Keys);

                    //walk through the keys - keys has to be a separate array otherwise we would be modifying what
                    //we are iterating through, which would cause fatal errors.
                    foreach (var key in keys)
                    {
                        var attrib = Array.Find(attributes, s => s.LogicalName.Equals(key));
                        if (attrib == null)
                        {
                            var notFoundMessage = string.Format("Field doesn't exist in CRM  - Entity:{0} ; Field: {1}",
                                  thisEntity.LogicalName,
                                  key);

                            LogHelper.LogError(OrgName, userId, method, notFoundMessage);
                            thisEntity.Attributes.Remove(key);
                            continue;
                        }

                        #region Remove invalid fields for request type
                        if (!String.IsNullOrEmpty(requestName))
                        {
                            if (requestName.Equals("Create", StringComparison.OrdinalIgnoreCase) && (attrib.IsValidForCreate.HasValue && !attrib.IsValidForCreate.Value))
                            {
                                thisEntity.Attributes.Remove(key);
                                continue;
                            }
                            if (requestName.Equals("Update", StringComparison.OrdinalIgnoreCase) && (attrib.IsValidForUpdate.HasValue && !attrib.IsValidForUpdate.Value))
                            {
                                thisEntity.Attributes.Remove(key);
                                continue;
                            }
                        }
                        #endregion

                        switch (attrib.AttributeType)
                        {
                            case AttributeTypeCode.String:
                                #region TruncateString
                                var fieldString = (string)thisEntity[key];
                                if (!String.IsNullOrEmpty(fieldString))
                                {
                                    var maxLength = ((Microsoft.Xrm.Sdk.Metadata.StringAttributeMetadata)(attrib)).MaxLength;
                                    if (fieldString.Length > maxLength.Value)
                                    {
                                        var message = string.Format("Entity:{0} ; Field: {1} ;  CRM Length {2} ; Actual Data Length {3} ; value: {4}",
                                            thisEntity.LogicalName,
                                            attrib.SchemaName.ToLower(),
                                            maxLength.Value,
                                            fieldString.Length,
                                            fieldString);

                                        LogHelper.LogError(OrgName, userId, method, message);
                                        thisEntity[key] = fieldString.Substring(0, maxLength.Value);
                                    }
                                    else
                                    {
                                        // LogHelper.LogInfo(OrgName, logTiming, userId, "truncate", "field was not truncated:" + thisField.Key);
                                    }
                                }
                                else
                                {
                                    // Remove null or empty string fields for create - decrease size of request sent to CrmService
                                    if (requestName.Equals("Create", StringComparison.OrdinalIgnoreCase)) thisEntity.Attributes.Remove(key);
                                    // LogHelper.LogInfo(OrgName, logTiming, userId, "truncate", "field was null:" + thisField.Key);
                                }
                                #endregion
                                break;

                            case AttributeTypeCode.DateTime:
                                #region Process Date using ToCRMDateTime
                                // This ensures all Date Time values used in the execute multiple helper are passed through the ToCRMDateTime method
                                // ensuring dates are valid and usable.
                                var fieldDate = thisEntity[key] as DateTime?;

                                if (!fieldDate.HasValue) continue; // no value in date field

                                var dateAttribute = (DateTimeAttributeMetadata)attrib;
                                var crmDate = fieldDate.Value.ToCRMDateTime(dateAttribute);
                                thisEntity[key] = crmDate;

                                if (dateAttribute.Format == DateTimeFormat.DateAndTime && !crmDate.Equals(fieldDate.Value) && logTiming)
                                {
                                    var message = String.Format("DateTime {0} {1} {2} was out of range and had to be adjusted.",
                                        dateAttribute.EntityLogicalName, dateAttribute.DisplayName.UserLocalizedLabel.Label, dateAttribute.LogicalName);
                                    LogHelper.LogInfo(OrgName, logTiming, userId, method, message);
                                }

                                #endregion
                                break;
                            default:
                                // not a string or date...
                                break;
                        }
                    }
                    txnTimer.Stop();
                    //LogHelper.LogTiming(OrgName, logTiming, userId, Guid.Empty, null, null, "truncate fields in MS:" + thisEntity.LogicalName, null, txnTimer.ElapsedMilliseconds);
                    #endregion

                    if (thisEntity.RelatedEntities.Count > 0)
                    {
                        foreach (var relationship in thisEntity.RelatedEntities)
                        {
                            foreach (var relatedEntity in relationship.Value.Entities)
                            {
                                TruncateFields(relatedEntity, OrgName, userId, logTiming, requestName);
                            }
                        }
                    }

                    return thisEntity;
                }
                catch (Exception processException)
                {
                    var message = processException.Message + "\r\n" + processException.StackTrace;
                    LogHelper.LogError(OrgName, userId, method, message);
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(OrgName, userId, method, ex.Message);
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

        [Obsolete("Deprecated method. Use the overloaded method instead.", true)]
        public static IEnumerable<OrganizationRequest> TruncateRequests(IEnumerable<OrganizationRequest> thisCollection, string OrgName, Guid userId, bool logTiming)
        {
            if (thisCollection == null) return null;
            Stopwatch txnTimer = Stopwatch.StartNew();
            Parallel.ForEach(thisCollection, (item) =>
            {
                if ((item.RequestName.Equals("Create", StringComparison.OrdinalIgnoreCase) ||
                     item.RequestName.Equals("Update", StringComparison.OrdinalIgnoreCase))
                    && item.Parameters.ContainsKey("Target"))
                {
                    var target = item.Parameters["Target"] as Entity;
                    if (target != null)
                    {
                        TruncateFields(target, OrgName, userId, logTiming, item.RequestName);
                    }
                }
            });
            txnTimer.Stop();
            //LogHelper.LogTiming(OrgName, logTiming, userId, Guid.Empty, null, null, "TruncateRequestCollection fields in MS", null, txnTimer.ElapsedMilliseconds);
            return thisCollection;
        }

        [Obsolete("Deprecated method. Use the overloaded method instead.", true)]
        public static OrganizationRequestCollection TruncateRequestCollection(OrganizationRequestCollection thisCollection, string OrgName, Guid userId, bool logTiming)
        {
            Stopwatch txnTimer = Stopwatch.StartNew();
            Parallel.ForEach(thisCollection, (item) =>
            {
                //            foreach (var item in thisCollection)
                //          {
                if ((item.RequestName.Equals("Create", StringComparison.OrdinalIgnoreCase) ||
                     item.RequestName.Equals("Update", StringComparison.OrdinalIgnoreCase))
                    && item.Parameters.ContainsKey("Target"))
                {
                    var target = item.Parameters["Target"] as Entity;
                    if (target != null)
                    {
                        TruncateFields(target, OrgName, userId, logTiming, item.RequestName);
                    }
                }
            });
            txnTimer.Stop();
            //LogHelper.LogTiming(OrgName, logTiming, userId, Guid.Empty, null, null, "TruncateRequestCollection fields in MS", null, txnTimer.ElapsedMilliseconds);
            return thisCollection;
        }

        public static Entity TruncateFields(string messageId, Entity thisEntity, string OrgName, Guid userId, bool logTiming, IOrganizationService OrgServiceProxy)
        {
            return TruncateFields(messageId, thisEntity, OrgName, userId, logTiming, string.Empty, OrgServiceProxy);
        }

        public static Entity TruncateFields(Entity thisEntity, string OrgName, Guid userId, bool logTiming, string requestName, IOrganizationService OrgServiceProxy)
        {
            // IOrganizationService OrgServiceProxy = null;
            var method = MethodInfo.GetCallingMethod(true).GetClassCallerPath(false, true);

            if (Settings == null || DateTime.UtcNow >= Settings.ExpiresOn)
            {
                Settings = new TruncHelperSettings();
                Settings = Settings.Load(OrgServiceProxy);

                var next = DateTime.UtcNow;
                var mintoadd = 15 - (next.Minute % 15);
                if (mintoadd < 1) mintoadd = 15;
                next.AddMinutes(mintoadd);
                Settings.ExpiresOn = next;
            }

            try
            {
                Stopwatch txnTimer = Stopwatch.StartNew();
                AttributeMetadata[] attributes = EntityCache.GetAttributes(OrgName, thisEntity.LogicalName, userId, Settings.ForceMetadataRefresh);
                if (Settings.ForceMetadataRefresh)
                {
                    Settings.ForceMetadataRefresh = false;
                    if (OrgServiceProxy != null) Settings.Update(OrgServiceProxy);
                }
                txnTimer.Stop();
                LogHelper.LogTiming(Guid.Empty.ToString(), OrgName, logTiming, userId, Guid.Empty, null, null, "truncate fields, get metadata in MS:" + thisEntity.LogicalName, null, txnTimer.ElapsedMilliseconds);

                #region truncate my fields
                txnTimer.Restart();

                // Get keys (fields) on the Entity
                var keys = new List<string>();
                keys.AddRange(thisEntity.Attributes.Keys);

                //walk through the keys - keys has to be a separate array otherwise we would be modifying what
                //we are iterating through, which would cause fatal errors.
                foreach (var key in keys)
                {
                    var attrib = Array.Find(attributes, s => s.LogicalName.Equals(key));
                    if (attrib == null)
                    {
                        var notFoundMessage = string.Format("Field doesn't exist in CRM  - Entity:{0} ; Field: {1}",
                              thisEntity.LogicalName,
                              key);

                        LogHelper.LogError(OrgName, userId, method, notFoundMessage);
                        thisEntity.Attributes.Remove(key);
                        continue;
                    }

                    #region Remove invalid fields for request type
                    if (!String.IsNullOrEmpty(requestName))
                    {
                        if (requestName.Equals("Create", StringComparison.OrdinalIgnoreCase) && (attrib.IsValidForCreate.HasValue && !attrib.IsValidForCreate.Value))
                        {
                            thisEntity.Attributes.Remove(key);
                            continue;
                        }
                        if (requestName.Equals("Update", StringComparison.OrdinalIgnoreCase) && (attrib.IsValidForUpdate.HasValue && !attrib.IsValidForUpdate.Value))
                        {
                            thisEntity.Attributes.Remove(key);
                            continue;
                        }
                    }
                    #endregion

                    switch (attrib.AttributeType)
                    {
                        case AttributeTypeCode.String:
                            #region TruncateString
                            var fieldString = (string)thisEntity[key];
                            if (!String.IsNullOrEmpty(fieldString))
                            {
                                var maxLength = ((Microsoft.Xrm.Sdk.Metadata.StringAttributeMetadata)(attrib)).MaxLength;
                                if (fieldString.Length > maxLength.Value)
                                {
                                    var message = string.Format("Entity:{0} ; Field: {1} ;  CRM Length {2} ; Actual Data Length {3} ; value: {4}",
                                        thisEntity.LogicalName,
                                        attrib.SchemaName.ToLower(),
                                        maxLength.Value,
                                        fieldString.Length,
                                        fieldString);

                                    LogHelper.LogError(OrgName, userId, method, message);
                                    thisEntity[key] = fieldString.Substring(0, maxLength.Value);
                                }
                                else
                                {
                                    // LogHelper.LogInfo(OrgName, logTiming, userId, "truncate", "field was not truncated:" + thisField.Key);
                                }
                            }
                            else
                            {
                                // Remove null or empty string fields for create - decrease size of request sent to CrmService
                                if (requestName.Equals("Create", StringComparison.OrdinalIgnoreCase)) thisEntity.Attributes.Remove(key);
                                // LogHelper.LogInfo(OrgName, logTiming, userId, "truncate", "field was null:" + thisField.Key);
                            }
                            #endregion
                            break;

                        case AttributeTypeCode.DateTime:
                            #region Process Date using ToCRMDateTime
                            // This ensures all Date Time values used in the execute multiple helper are passed through the ToCRMDateTime method
                            // ensuring dates are valid and usable.
                            var fieldDate = thisEntity[key] as DateTime?;

                            if (!fieldDate.HasValue) continue; // no value in date field

                            var dateAttribute = (DateTimeAttributeMetadata)attrib;
                            var crmDate = fieldDate.Value.ToCRMDateTime(dateAttribute);
                            thisEntity[key] = crmDate;

                            if (dateAttribute.Format == DateTimeFormat.DateAndTime && !crmDate.Equals(fieldDate.Value) && logTiming)
                            {
                                var message = String.Format("DateTime {0} {1} {2} was out of range and had to be adjusted.",
                                    dateAttribute.EntityLogicalName, dateAttribute.DisplayName.UserLocalizedLabel.Label, dateAttribute.LogicalName);
                                LogHelper.LogInfo(OrgName, logTiming, userId, method, message);
                            }

                            #endregion
                            break;
                        default:
                            // not a string or date...
                            break;
                    }
                }
                txnTimer.Stop();
                //LogHelper.LogTiming(OrgName, logTiming, userId, Guid.Empty, null, null, "truncate fields in MS:" + thisEntity.LogicalName, null, txnTimer.ElapsedMilliseconds);
                #endregion

                if (thisEntity.RelatedEntities.Count > 0)
                {
                    foreach (var relationship in thisEntity.RelatedEntities)
                    {
                        foreach (var relatedEntity in relationship.Value.Entities)
                        {
                            TruncateFields(relatedEntity, OrgName, userId, logTiming, requestName, OrgServiceProxy);
                        }
                    }
                }

                return thisEntity;
            }
            catch (Exception processException)
            {
                var message = processException.Message + "\r\n" + processException.StackTrace;
                LogHelper.LogError(OrgName, userId, method, message);
                return null;
            }
        }

        public static Entity TruncateFields(string messageId, Entity thisEntity, string OrgName, Guid userId, bool logTiming, string requestName, IOrganizationService OrgServiceProxy)
        {
            // IOrganizationService OrgServiceProxy = null;
            var method = MethodInfo.GetCallingMethod(true).GetClassCallerPath(false, true);

            if (Settings == null || DateTime.UtcNow >= Settings.ExpiresOn)
            {
                Settings = new TruncHelperSettings();
                Settings = Settings.Load(OrgServiceProxy);

                var next = DateTime.UtcNow;
                var mintoadd = 15 - (next.Minute % 15);
                if (mintoadd < 1) mintoadd = 15;
                next.AddMinutes(mintoadd);
                Settings.ExpiresOn = next;
            }

            try
            {
                Stopwatch txnTimer = Stopwatch.StartNew();
                AttributeMetadata[] attributes = EntityCache.GetAttributes(OrgName, thisEntity.LogicalName, userId, Settings.ForceMetadataRefresh);
                if (Settings.ForceMetadataRefresh)
                {
                    Settings.ForceMetadataRefresh = false;
                    if (OrgServiceProxy != null) Settings.Update(OrgServiceProxy);
                }
                txnTimer.Stop();
                LogHelper.LogTiming(messageId, OrgName, logTiming, userId, Guid.Empty, null, null, "truncate fields, get metadata in MS:" + thisEntity.LogicalName, null, txnTimer.ElapsedMilliseconds);

                #region truncate my fields
                txnTimer.Restart();

                // Get keys (fields) on the Entity
                var keys = new List<string>();
                keys.AddRange(thisEntity.Attributes.Keys);

                //walk through the keys - keys has to be a separate array otherwise we would be modifying what
                //we are iterating through, which would cause fatal errors.
                foreach (var key in keys)
                {
                    var attrib = Array.Find(attributes, s => s.LogicalName.Equals(key));
                    if (attrib == null)
                    {
                        var notFoundMessage = string.Format("Field doesn't exist in CRM  - Entity:{0} ; Field: {1}",
                              thisEntity.LogicalName,
                              key);

                        LogHelper.LogError(OrgName, userId, method, notFoundMessage);
                        thisEntity.Attributes.Remove(key);
                        continue;
                    }

                    #region Remove invalid fields for request type
                    if (!String.IsNullOrEmpty(requestName))
                    {
                        if (requestName.Equals("Create", StringComparison.OrdinalIgnoreCase) && (attrib.IsValidForCreate.HasValue && !attrib.IsValidForCreate.Value))
                        {
                            thisEntity.Attributes.Remove(key);
                            continue;
                        }
                        if (requestName.Equals("Update", StringComparison.OrdinalIgnoreCase) && (attrib.IsValidForUpdate.HasValue && !attrib.IsValidForUpdate.Value))
                        {
                            thisEntity.Attributes.Remove(key);
                            continue;
                        }
                    }
                    #endregion

                    switch (attrib.AttributeType)
                    {
                        case AttributeTypeCode.String:
                            #region TruncateString
                            var fieldString = (string)thisEntity[key];
                            if (!String.IsNullOrEmpty(fieldString))
                            {
                                var maxLength = ((Microsoft.Xrm.Sdk.Metadata.StringAttributeMetadata)(attrib)).MaxLength;
                                if (fieldString.Length > maxLength.Value)
                                {
                                    var message = string.Format("Entity:{0} ; Field: {1} ;  CRM Length {2} ; Actual Data Length {3} ; value: {4}",
                                        thisEntity.LogicalName,
                                        attrib.SchemaName.ToLower(),
                                        maxLength.Value,
                                        fieldString.Length,
                                        fieldString);

                                    LogHelper.LogError(OrgName, userId, method, message);
                                    thisEntity[key] = fieldString.Substring(0, maxLength.Value);
                                }
                                else
                                {
                                    // LogHelper.LogInfo(OrgName, logTiming, userId, "truncate", "field was not truncated:" + thisField.Key);
                                }
                            }
                            else
                            {
                                // Remove null or empty string fields for create - decrease size of request sent to CrmService
                                if (requestName.Equals("Create", StringComparison.OrdinalIgnoreCase)) thisEntity.Attributes.Remove(key);
                                // LogHelper.LogInfo(OrgName, logTiming, userId, "truncate", "field was null:" + thisField.Key);
                            }
                            #endregion
                            break;

                        case AttributeTypeCode.DateTime:
                            #region Process Date using ToCRMDateTime
                            // This ensures all Date Time values used in the execute multiple helper are passed through the ToCRMDateTime method
                            // ensuring dates are valid and usable.
                            var fieldDate = thisEntity[key] as DateTime?;

                            if (!fieldDate.HasValue) continue; // no value in date field

                            var dateAttribute = (DateTimeAttributeMetadata)attrib;
                            var crmDate = fieldDate.Value.ToCRMDateTime(dateAttribute);
                            thisEntity[key] = crmDate;

                            if (dateAttribute.Format == DateTimeFormat.DateAndTime && !crmDate.Equals(fieldDate.Value) && logTiming)
                            {
                                var message = String.Format("DateTime {0} {1} {2} was out of range and had to be adjusted.",
                                    dateAttribute.EntityLogicalName, dateAttribute.DisplayName.UserLocalizedLabel.Label, dateAttribute.LogicalName);
                                LogHelper.LogInfo(OrgName, logTiming, userId, method, message);
                            }

                            #endregion
                            break;
                        default:
                            // not a string or date...
                            break;
                    }
                }
                txnTimer.Stop();
                //LogHelper.LogTiming(OrgName, logTiming, userId, Guid.Empty, null, null, "truncate fields in MS:" + thisEntity.LogicalName, null, txnTimer.ElapsedMilliseconds);
                #endregion

                if (thisEntity.RelatedEntities.Count > 0)
                {
                    foreach (var relationship in thisEntity.RelatedEntities)
                    {
                        foreach (var relatedEntity in relationship.Value.Entities)
                        {
                            TruncateFields(relatedEntity, OrgName, userId, logTiming, requestName, OrgServiceProxy);
                        }
                    }
                }

                return thisEntity;
            }
            catch (Exception processException)
            {
                var message = processException.Message + "\r\n" + processException.StackTrace;
                LogHelper.LogError(OrgName, userId, method, message);
                return null;
            }
        }


        public static IEnumerable<OrganizationRequest> TruncateRequests(IEnumerable<OrganizationRequest> thisCollection, string OrgName, Guid userId, bool logTiming, IOrganizationService OrgServiceProxy)
        {
            if (thisCollection == null) return null;
            Stopwatch txnTimer = Stopwatch.StartNew();
            foreach(var item in thisCollection)
            //Parallel.ForEach(thisCollection, (item) =>
            {
                if ((item.RequestName.Equals("Create", StringComparison.OrdinalIgnoreCase) ||
                     item.RequestName.Equals("Update", StringComparison.OrdinalIgnoreCase))
                    && item.Parameters.ContainsKey("Target"))
                {
                    var target = item.Parameters["Target"] as Entity;
                    if (target != null)
                    {
                        TruncateFields(target, OrgName, userId, logTiming, item.RequestName, OrgServiceProxy);
                    }
                }
            }
            txnTimer.Stop();
            //LogHelper.LogTiming(OrgName, logTiming, userId, Guid.Empty, null, null, "TruncateRequestCollection fields in MS", null, txnTimer.ElapsedMilliseconds);
            return thisCollection;
        }
    }
}
