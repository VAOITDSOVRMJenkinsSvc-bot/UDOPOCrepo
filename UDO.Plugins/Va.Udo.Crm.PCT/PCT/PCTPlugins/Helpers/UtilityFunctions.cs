using System;
using System.IO;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
//using VRM.Integration.Servicebus.CRM.SDK;

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

        public string getOptionSetString(Entity entity, string attributeName)
        {
            var label = string.Empty;
            if (entity == null) return string.Empty;
            if (entity.FormattedValues!=null && entity.FormattedValues.ContainsKey(attributeName))
            {
                label = entity.FormattedValues[attributeName];
            }

            if (String.IsNullOrEmpty(label))
            {
                var value = entity.GetAttributeValue<OptionSetValue>(attributeName);
                if (value==null) return string.Empty;

                label = getOptionSetString(value.Value, entity.LogicalName, attributeName);
            }
            return label;
        }

        public string getOptionSetString(int optionSetValue, string entityName, string attributeName)
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
                _Logger.setModule = "execute";
                return null;
            }
            catch (Exception ex)
            {
                _Logger.setService = _service;
                _Logger.setModule = "getOptionSetString";
               _Logger.WriteToFile(ex.Message);
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
            var emRequest = new ExecuteMultipleRequest();
            emRequest.Requests = new OrganizationRequestCollection();
            emRequest.Requests.Add(request);
            var emResponse = _service.Execute(emRequest) as ExecuteMultipleResponse;
            if (emResponse == null || emResponse.Responses.Count == 0) return null;
            return emResponse.Responses[0].Response;
        }
    }
}
