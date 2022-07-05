using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using MCSPlugins;
using UDO.Model;
using VRMRest;
using UDO.LOB.Contact.Messages;
using UDO.LOB.Core;

namespace CustomActions.Plugins.Entities.Address
{
    internal class ValidateAddressRunner : UDOActionRunner
    {
        protected Guid _bankId = new Guid();
        
        public ValidateAddressRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_contactlogtimer";
            _logSoapField = "udo_contactlogsoap";
            _debugField = "va_bankaccount";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_contacttimeout";
            _validEntities = new string[] { "va_bankaccount" };
        }

        public override void DoAction()
        {
            Entity bankent = new Entity("va_bankaccount");

            _method = "DoAction";

            if (Parent.LogicalName == va_bankaccount.EntityLogicalName)
            {
                _bankId = Parent.Id;
            }

            GetSettingValues();

            EntityCollection validatedAddresses = new EntityCollection();

            LogSettings _logSettings = new LogSettings()
            {
                Org = PluginExecutionContext.OrganizationName,
                ConfigFieldName = "RESTCALL",
                UserId = PluginExecutionContext.InitiatingUserId,
                callingMethod = Logger.setModule
            };

            UDOValidateAddressRequest messageValues = new UDOValidateAddressRequest();
                        
            UDOHeaderInfo _headerInfo = MCSUtilities2011.UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);
            var inputParamaters = PluginExecutionContext.InputParameters;


            var addressCollection = inputParamaters["Addresses"] as EntityCollection;

            messageValues.MessageId = PluginExecutionContext.CorrelationId.ToString();
            
            messageValues.LegacyServiceHeaderInfo = _headerInfo;
            messageValues.LogSoap = _logSoap;
            messageValues.LogTiming = _logTimer;
            messageValues.RelatedParentEntityName = "va_bankaccount";
            messageValues.RelatedParentFieldName = "va_bankaccountid";
            messageValues.UserId = PluginExecutionContext.InitiatingUserId;
            messageValues.OrganizationName = PluginExecutionContext.OrganizationName;

            if (addressCollection != null)
            {
                foreach (var address in addressCollection.Entities)
                {
                    messageValues.mcs_addressLine1 = address.GetAttributeValue<string>("udo_addressline1");
                    messageValues.mcs_addressLine2 = address.GetAttributeValue<string>("udo_addressline2");
                    messageValues.mcs_addressLine3 = address.GetAttributeValue<string>("udo_addressline3");
                    messageValues.mcs_city = address.GetAttributeValue<string>("udo_city");

                    var stateString = address.GetAttributeValue<string>("udo_stateprovince");

                    int stateIndex = 0;
                    if (!string.IsNullOrEmpty(stateString))
                        stateIndex = Int32.Parse(stateString);

                    if (stateIndex != 0)
                        messageValues.mcs_stateProvince = GetOptionsetText(bankent, OrganizationService, "va_state", stateIndex);

                    messageValues.mcs_postalCode = address.GetAttributeValue<string>("udo_postalcode");

                    var response = Utility.SendReceive<UDOValidateAddressResponse>(_uri, "UDOValidateAddressRequest", messageValues, _logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);

                    if (response.ExceptionOccured)
                    {
                        //ExceptionOccurred = true;
                        //_responseMessage = "An error occurred while validating addresses.";

                        Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));
                        tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", _responseMessage, PluginExecutionContext.CorrelationId.ToString()));

                        var validatedAddress = new Entity("udo_validateaddress");
                        validatedAddress.Id = address.Id;
                        validatedAddress["udo_addresstype"] = address.GetAttributeValue<OptionSetValue>("udo_addresstype");
                        validatedAddress["udo_exceptionoccurred"] = true;
                        validatedAddress["udo_exceptionmessage"] = response.ExceptionMessage;

                        validatedAddresses.Entities.Add(validatedAddress);
                    }
                    else
                    {
                        ///Map Response
                        var validatedAddress = new Entity("udo_validateaddress");
                        validatedAddress.Id = address.Id;
                        validatedAddress["udo_addresstype"] = address.GetAttributeValue<OptionSetValue>("udo_addresstype");
                        validatedAddress["udo_addressblock1"] = response.mcs_addressBlock1;
                        validatedAddress["udo_addressblock2"] = response.mcs_addressBlock2;
                        validatedAddress["udo_addressblock3"] = response.mcs_addressBlock3;
                        validatedAddress["udo_addressline1"] = response.mcs_addressLine1;
                        validatedAddress["udo_addressline2"] = response.mcs_addressLine2;
                        validatedAddress["udo_addressline3"] = response.mcs_addressLine3;
                        validatedAddress["udo_addressline4"] = response.mcs_addressLine4;
                        validatedAddress["udo_city"] = response.mcs_city;
                        validatedAddress["udo_country"] = response.mcs_country;
                        validatedAddress["udo_pobox"] = response.mcs_POBox;
                        validatedAddress["udo_postalcode"] = response.mcs_postalCode;
                        validatedAddress["udo_privatemailbox"] = response.mcs_privateMailbox;
                        validatedAddress["udo_stateprovince"] = response.mcs_stateProvince;
                        validatedAddress["udo_streetname"] = response.mcs_streetName;
                        validatedAddress["udo_streetsuffix"] = response.mcs_streetSuffix;
                        validatedAddress["udo_mcsltaddr"] = response.mcs_USAltAddr;
                        validatedAddress["udo_uscountyname"] = response.mcs_USCountyName;
                        validatedAddress["udo_status"] = response.mcs_status;
                        validatedAddress["udo_statuscode"] = response.mcs_statusCode;
                        validatedAddress["udo_confidence"] = response.mcs_confidence;
                        validatedAddress["udo_postalcodeaddon"] = response.mcs_postalCodeAddOn;
                        validatedAddress["udo_postalcodebase"] = response.mcs_postalCodeBase;
                        validatedAddress["udo_stateprovinceresult"] = response.mcs_stateProvinceResult;

                        validatedAddresses.Entities.Add(validatedAddress);
                    }
                }
            }

            PluginExecutionContext.OutputParameters["ValidatedAddresses"] = validatedAddresses;
        }

        public string GetOptionsetText(Entity entity, IOrganizationService service, string optionsetName, int optionsetValue)
        {
            string optionsetSelectedText = string.Empty;
            try
            {
                RetrieveOptionSetRequest retrieveOptionSetRequest = new RetrieveOptionSetRequest
                {
                    Name = optionsetName
                };

                // Execute the request.
                RetrieveOptionSetResponse retrieveOptionSetResponse =
                (RetrieveOptionSetResponse)service.Execute(retrieveOptionSetRequest);

                // Access the retrieved OptionSetMetadata.
                OptionSetMetadata retrievedOptionSetMetadata = (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;

                // Get the current options list for the retrieved attribute.
                OptionMetadata[] optionList = retrievedOptionSetMetadata.Options.ToArray();
                foreach (OptionMetadata optionMetadata in optionList)
                {
                    if (optionMetadata.Value == optionsetValue)
                    {
                        optionsetSelectedText = optionMetadata.Label.UserLocalizedLabel.Label.ToString();
                        break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return optionsetSelectedText;
        }
    }   
}
