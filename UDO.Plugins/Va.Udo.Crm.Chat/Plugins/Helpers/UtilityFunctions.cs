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
        public int getOptionSetValue(string optionSetString, string entityName, string attributeName, ITracingService tracer)
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
                tracer.Trace(ex.Detail.Message);
                _Logger.setModule = "execute";
                return 0;
            }
            catch (Exception ex)
            {
                _Logger.setService = _service;
                _Logger.setModule = "getOptionSetValue";
                _Logger.WriteToFile(ex.Message);
                tracer.Trace(ex.Message);
                _Logger.setModule = "execute";
                return 0;
            }
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
    }
}
