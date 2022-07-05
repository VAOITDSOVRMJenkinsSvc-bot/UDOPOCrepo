using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using UDO.LOB.Core;
//using UDO.LOB.Core;
//using VRM.Integration.Servicebus.CRM.SDK;
//using VRM.Integration.UDO.MVI.Messages;

namespace MCSUtilities2011
{
    public class UtilityFunctions
    {
        private IOrganizationService _service;
        public IOrganizationService setService
        {
            set { _service = value; }
        }
        private MCSLogger _Logger;
        public MCSLogger setLogger
        {
            set { _Logger = value; }
        }
        public int getOptionSetValue(string optionSetString, string entityName, string attributeName)
        {
            try
            {
                int returnInt = 0;
                RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest();
                attributeRequest.EntityLogicalName = entityName;
                attributeRequest.LogicalName = attributeName;
                // Retrieve only the currently published changes, ignoring the changes that have
                // not been published.
                attributeRequest.RetrieveAsIfPublished = false;

                RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)_service.Execute(attributeRequest);

                // Access the retrieved attribute.
                PicklistAttributeMetadata retrievedAttributeMetadata = (PicklistAttributeMetadata)attributeResponse.AttributeMetadata;
                for (int i = 0; i < retrievedAttributeMetadata.OptionSet.Options.Count; i++)
                {
                    if (retrievedAttributeMetadata.OptionSet.Options[i].Label.LocalizedLabels[0].Label == optionSetString)
                    {
                        returnInt = retrievedAttributeMetadata.OptionSet.Options[i].Value.Value;
                        break;
                    }

                }
                return returnInt;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                _Logger.setService = _service;
                _Logger.setModule = "getOptionSetValue";
                _Logger.WriteToFile(ex.Detail.Message);
                _Logger.setModule = "execute";
                return 0;
            }
            catch (Exception ex)
            {
                _Logger.setService = _service;
                _Logger.setModule = "getOptionSetValue";
                _Logger.WriteToFile(ex.Message);
                _Logger.setModule = "execute";
                return 0;
            }
        }

        public string getOptionSetString(Entity entity, string attributeName, ITracingService tracer)
        {
            string label = string.Empty;
            if (entity == null) return string.Empty;
            if (entity.FormattedValues != null && entity.FormattedValues.ContainsKey(attributeName))
            {
                label = entity.FormattedValues[attributeName];
            }

            if (String.IsNullOrEmpty(label))
            {
                OptionSetValue value = entity.GetAttributeValue<OptionSetValue>(attributeName);
                if (value == null) return string.Empty;

                label = getOptionSetString(value.Value, entity.LogicalName, attributeName, tracer);
            }
            return label;
        }

        public string getOptionSetString(int optionSetValue, string entityName, string attributeName, ITracingService tracer)
        {
            try
            {

                string optionSetString = string.Empty;

                RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest();
                attributeRequest.EntityLogicalName = entityName;
                attributeRequest.LogicalName = attributeName;
                // Retrieve only the currently published changes, ignoring the changes that have
                // not been published.
                attributeRequest.RetrieveAsIfPublished = true;

                RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)_service.Execute(attributeRequest);

                // Access the retrieved attribute.
                PicklistAttributeMetadata retrievedAttributeMetadata = (PicklistAttributeMetadata)attributeResponse.AttributeMetadata;
                for (int i = 0; i < retrievedAttributeMetadata.OptionSet.Options.Count; i++)
                {
                    if (retrievedAttributeMetadata.OptionSet.Options[i].Value == optionSetValue)
                    {
                        optionSetString = retrievedAttributeMetadata.OptionSet.Options[i].Label.LocalizedLabels[0].Label;
                        break;
                    }

                }
                return optionSetString;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                _Logger.setService = _service;
                _Logger.setModule = "getOptionSetString";
                _Logger.WriteToFile(ex.Detail.Message);
                tracer.Trace(ex.Detail.Message);
                _Logger.setModule = "execute";
                return null;
            }
            catch (Exception ex)
            {
                _Logger.setService = _service;
                _Logger.setModule = "getOptionSetString";
                _Logger.WriteToFile(ex.Message);
                tracer.Trace(ex.Message);
    
                _Logger.setModule = "execute";
                return null;
            }
        }

        /// <summary>
        /// ExecuteRequestOutsideCurrentTransaction: Wrap the request inside an Execute Multiple Request,
        /// causing the request to be executed in a separate database transaction.
        /// 
        /// This allow errors thrown inside child plugins to be handled and the parent plugin to continue.
        /// </summary>
        /// <param name="request">The Organization Request (CreateRequest, UpdateRequest, etc)</param>
        /// <returns></returns>
        public OrganizationResponse ExecuteRequestOutsideCurrentTransaction(OrganizationRequest request)
        {
            ExecuteMultipleRequest emRequest = new ExecuteMultipleRequest();
            emRequest.Requests = new OrganizationRequestCollection();
            emRequest.Requests.Add(request);
            ExecuteMultipleResponse emResponse = _service.Execute(emRequest) as ExecuteMultipleResponse;
            if (emResponse == null || emResponse.Responses.Count == 0) return null;
            return emResponse.Responses[0].Response;
        }

        public static UDOHeaderInfo GetHeaderInfo(IOrganizationService _service, Guid _initiatinguser)
        {
            ColumnSet userCols = new ColumnSet("va_stationnumber", "va_wsloginname", "va_applicationname", "va_ipaddress", "fullname", "va_pcrsensitivitylevel");
            Entity thisUser = _service.Retrieve("systemuser", _initiatinguser, userCols);

            const string stationNumberIsNotAssignedForCrmUser = "Station Number is not assigned for CRM User.";
            const string vaStationnumber = "va_stationnumber";

            if (!thisUser.Attributes.ContainsKey(vaStationnumber))
                throw new Exception(stationNumberIsNotAssignedForCrmUser);

            const string wsLoginIsNotAssignedForCrmUser = "WS Login is not assigned for CRM User.";
            const string vaWsloginname = "va_wsloginname";

            if (!thisUser.Attributes.ContainsKey(vaWsloginname))
                throw new Exception(wsLoginIsNotAssignedForCrmUser);

            const string applicationNameIsNotAssignedForCrmUser = "Application Name is not assigned for CRM User.";
            const string vaApplicationname = "va_applicationname";

            if (!thisUser.Attributes.ContainsKey(vaApplicationname))
                throw new Exception(applicationNameIsNotAssignedForCrmUser);

            const string clientMachineIsNotAssignedForCrmUser = "Client Machine is not assigned for CRM User.";
            const string vaIpAddress = "va_ipaddress";

            if (!thisUser.Attributes.ContainsKey(vaIpAddress))
                throw new Exception(clientMachineIsNotAssignedForCrmUser);

            string stationNumber = (string)thisUser[vaStationnumber];

            string loginName = (string)thisUser[vaWsloginname];

            string applicationName = (string)thisUser[vaApplicationname];

            string clientMachine = (string)thisUser[vaIpAddress];

            string fullName = thisUser.GetAttributeValue<string>("fullname");

            OptionSetValue userSL = thisUser.GetAttributeValue<OptionSetValue>("va_pcrsensitivitylevel");

            return new UDOHeaderInfo
            {
                StationNumber = stationNumber,

                LoginName = loginName,

                ApplicationName = applicationName,

                ClientMachine = clientMachine,
            };
        }
    }



    //public class UDOHeaderInfo : ILegacyHeaderInfo
    //{
    //    public string StationNumber { get; set; }
    //    public string LoginName { get; set; }
    //    public string ApplicationName { get; set; }
    //    public string ClientMachine { get; set; }
    //}

    //public interface ILegacyHeaderInfo
    //{
    //    string StationNumber { get; set; }
    //    string LoginName { get; set; }
    //    string ApplicationName { get; set; }
    //    string ClientMachine { get; set; }
    //}
}
