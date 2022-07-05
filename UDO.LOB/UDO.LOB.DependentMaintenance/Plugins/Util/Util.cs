using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRM.Integration.Servicebus.AddDependent.Util
{
    public class Utils
    {

        //public static int getOptionSetValue(IOrganizationService service, string optionSetString, string entityName, string attributeName)
        public static int getOptionSetValue(OrganizationServiceContext service, string optionSetString, string entityName, string attributeName)
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

                RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)service.Execute(attributeRequest);

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
            catch (Exception ex)
            {
                throw ex;
            }
            //catch (FaultException<OrganizationServiceFault> ex)
            //{
            //    _logger.setService = _service;
            //    _logger.setModule = "getOptionSetValue";
            //    _TracingService.Trace(ex.Detail.Message);
            //    _logger.setModule = "execute";
            //    return 0;
            //}
            //catch (Exception ex)
            //{
            //    _logger.setService = _service;
            //    _logger.setModule = "getOptionSetValue";
            //    _TracingService.Trace(ex.Message);
            //    _logger.setModule = "execute";
            //    return 0;
            //}
        }

        public static string getOptionSetString(OrganizationServiceContext service, OptionSetValue optionSetValue, string entityName, string attributeName)
        {
            try
            {

                string optionSetString = string.Empty;
                if (optionSetValue == null) return null;

                RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest();
                attributeRequest.EntityLogicalName = entityName;
                attributeRequest.LogicalName = attributeName;
                // Retrieve only the currently published changes, ignoring the changes that have
                // not been published.
                attributeRequest.RetrieveAsIfPublished = true;

                RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)service.Execute(attributeRequest);

                // Access the retrieved attribute.
                PicklistAttributeMetadata retrievedAttributeMetadata = (PicklistAttributeMetadata)attributeResponse.AttributeMetadata;
                for (int i = 0; i < retrievedAttributeMetadata.OptionSet.Options.Count; i++)
                {
                    if (retrievedAttributeMetadata.OptionSet.Options[i].Value == optionSetValue.Value)
                    {
                        optionSetString = retrievedAttributeMetadata.OptionSet.Options[i].Label.LocalizedLabels[0].Label;
                        break;
                    }

                }
                return optionSetString;
            }
            catch (Exception ex) {
                throw ex;
            }
            //catch (FaultException<OrganizationServiceFault> ex)
            //{
            //    _logger.setService = _service;
            //    _logger.setModule = "getOptionSetString";
            //    _TracingService.Trace(ex.Detail.Message);
            //    _logger.setModule = "execute";
            //    return null;
            //}
            //catch (Exception ex)
            //{
            //    _logger.setService = _service;
            //    _logger.setModule = "getOptionSetString";
            //    _TracingService.Trace(ex.Message);
            //    _logger.setModule = "execute";
            //    return null;
            }
       

    }
}
