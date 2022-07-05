using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using MCSUtilities2011;
using UDO.LOB.Core;
using UDO.LOB.IntentToFile.Messages;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.ITF.Messages;
using VRMRest;

namespace CustomActions.Plugins.Entities.ITF
{
    public class UDOSubmitITFRunner : UDOActionRunner
    {

        #region Members

        protected UDOHeaderInfo _headerInfo;
        protected EntityReference _idproof;
        protected EntityReference _owner;
        protected Entity _response;

        #endregion

        public UDOSubmitITFRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_itflogtimer";
            _logSoapField = "udo_itflogsoap";
            _debugField = "udo_itf";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_itftimeout";
            _validEntities = new string[] { "va_intenttofile" };
        }
        
        public override void DoAction()
        {

            try
            {
                _method = "DoAction";
                tracer.Trace("Start UDOSubmitITFRunner - " + _method);

                GetSettingValues();
                _headerInfo = UtilityFunctions.GetHeaderInfo(OrganizationService, PluginExecutionContext.InitiatingUserId);

                _responseMessage = string.Empty;

                tracer.Trace("Calling DidWeFindData - " + _method);
                if (DataIssue = !DidWeFindData()) return;
                tracer.Trace("No DataIssues found in DidWeFindData - " + _method);

                var fromLegacy = _response.GetAttributeValue<bool>("udo_fromlegacy");

                if (fromLegacy)
                {
                    tracer.Trace("Record is from Legacy system. Aborting from SubmitITFRunner");
                    Complete = true;
                    return;
                }


                #region Build Request

                tracer.Trace("Starting to build Request class - " + _method);
                var compensationType = "";
                if (_response.Contains("va_generalbenefittype"))
                {
                    tracer.Trace("Request contains va_generalbenefittype - " + _method);
                    var generalBenefitType = (OptionSetValue)_response["va_generalbenefittype"];
                    switch (generalBenefitType.Value)
                    {
                        case (int)GeneralBenefitType.Compensation:
                            compensationType = "C";
                            break;
                        case (int)GeneralBenefitType.Pension:
                            compensationType = "P";
                            break;
                        case (int)GeneralBenefitType.Survivor:
                            compensationType = "S";
                            break;
                        default:
                            break;
                    }
                }


                var gender = string.Empty;
                if (_response.Contains("va_veterangender"))
                {
                    var veteranGender = (bool)_response["va_veterangender"];
                    gender = veteranGender ? "F" : "M";
                }

                var fullPhone = _response.GetAttributeValue<string>("va_veteranphone");
                var phoneareacode = string.Empty;
                var phone = string.Empty;
                if (!String.IsNullOrEmpty(fullPhone))
                {
                    fullPhone = RemoveNonNumber(_response.GetAttributeValue<string>("va_veteranphone"));

                    if (fullPhone.Length == 10)
                    {
                        phoneareacode = fullPhone.Substring(0, 3);
                        phone = fullPhone.Substring(3, 7);
                    }
                }

                tracer.Trace("Starting to build Claimant class - " + _method);
                var claimant = new Claimant
                {
                    ClaimantParticipantId = _response.Contains("va_claimantparticipantid")
                        ? _response["va_claimantparticipantid"].ToString()
                        : string.Empty,
                    VeteranParticipantId = _response.Contains("va_participantid")
                        ? _response["va_participantid"].ToString()
                        : string.Empty,
                    CompensationType = compensationType,
                    VeteranFirstName = _response.Contains("va_veteranfirstname")
                        ? _response["va_veteranfirstname"].ToString()
                        : string.Empty,
                    VeteranLastName = _response.Contains("va_veteranlastname")
                        ? _response["va_veteranlastname"].ToString()
                        : string.Empty,
                    VeteranMiddleInitial = _response.Contains("va_veteranmiddleinitial")
                        ? _response["va_veteranmiddleinitial"].ToString()
                        : string.Empty,
                    VeteranSsn = _response.Contains("va_veteranssn")
                        ? _response["va_veteranssn"].ToString()
                        : string.Empty,
                    VeteranFileNumber = _response.Contains("va_veteranfilenumber")
                        ? _response["va_veteranfilenumber"].ToString()
                        : string.Empty,
                    VeteranBirthDate = _response.Contains("va_veterandateofbirth")
                        ? _response["va_veterandateofbirth"].ToString()
                        : string.Empty,
                    VeteranGender = _response.Contains("va_veterangender")
                        ? gender
                        : string.Empty,
                    ClaimantFirstName = _response.Contains("va_claimantfirstname")
                        ? _response["va_claimantfirstname"].ToString()
                        : string.Empty,
                    ClaimantLastName = _response.Contains("va_claimantlastname")
                        ? _response["va_claimantlastname"].ToString()
                        : string.Empty,
                    ClaimantMiddleInitial = _response.Contains("va_claimantmiddleinitial")
                        ? _response["va_claimantmiddleinitial"].ToString()
                        : string.Empty,
                    ClaimantSsn = _response.Contains("va_claimantssn")
                        ? _response["va_claimantssn"].ToString()
                        : string.Empty,
                    PhoneAreaCode = phoneareacode,
                    Phone = phone,
                    Email = _response.Contains("va_veteranemail")
                        ? _response["va_veteranemail"].ToString()
                        : string.Empty,
                    AddressLine1 = _response.Contains("va_veteranaddressline1")
                        ? _response["va_veteranaddressline1"].ToString()
                        : string.Empty,
                    AddressLine2 = _response.Contains("va_veteranaddressline2")
                        ? _response["va_veteranaddressline2"].ToString()
                        : string.Empty,
                    AddressLine3 = string.Empty,
                    City = _response.Contains("va_veterancity")
                        ? _response["va_veterancity"].ToString()
                        : string.Empty,
                    State = _response.Contains("va_veteranstate")
                        ? _response["va_veteranstate"].ToString()
                        : string.Empty,
                    Zip = _response.Contains("va_veteranzip") ? _response["va_veteranzip"].ToString() : string.Empty,
                    Country = _response.Contains("va_veterancountry")
                        ? _response["va_veterancountry"].ToString()
                        : string.Empty,
                    StationLocation = _response.Contains("va_stationlocation")
                        ? _response["va_stationlocation"].ToString()
                        : string.Empty
                };

                tracer.Trace("Starting to build UDOSubmitITFRequest class - " + _method);
                var request = new UDOSubmitITFRequest
                {
                    MessageId = PluginExecutionContext.CorrelationId.ToString(),
                    OrganizationName = PluginExecutionContext.OrganizationName,
                    UserId = PluginExecutionContext.InitiatingUserId,
                    Debug = _debug,
                    LogSoap = _logSoap,
                    LogTiming = _logTimer,
                    LegacyServiceHeaderInfo = _headerInfo,
                    Claimant = claimant,
                    va_intenttofileId = Parent.Id
                };

                #endregion

                #region Setup Log Settings

                tracer.Trace("Starting to build LogSettings class - " + _method);
                var logSettings = new LogSettings()
                {
                    Org = PluginExecutionContext.OrganizationName,
                    ConfigFieldName = "RESTCALL",
                    UserId = PluginExecutionContext.InitiatingUserId,
                    callingMethod = Logger.setModule
                };

                #endregion

                tracer.Trace("Calling UDOSubmitRequest LOB - " + _method);
                var submitResponse = Utility.SendReceive<UDOSubmitITFResponse>(_uri, "UDOSubmitITFRequest", request, logSettings, _timeOutSetting, _crmAuthTokenConfig, tracer);
                tracer.Trace("Returned from UDOSubmitRequest LOB - " + _method);

                if (submitResponse.ExceptionOccurred)
                {
                    _responseMessage = submitResponse.ExceptionMessage;
                    ExceptionOccurred = true;

                    Logger.WriteToFile(string.Format("Error message - {0}", _responseMessage));
                    tracer.Trace(string.Format("Error message - {0}", _responseMessage));
                    throw new InvalidPluginExecutionException("An unexpected error calling Submit ITF due to: {0}. CorrelationId: {1}".Replace("{0}",
                        submitResponse.ExceptionMessage).Replace("{1}", PluginExecutionContext.CorrelationId.ToString()));
                }

                PluginExecutionContext.OutputParameters["result"] = new EntityReference("va_intenttofile", Parent.Id);

                tracer.Trace("Start to Update ITF record - " + _method);

                var updateItf = new Entity("va_intenttofile");
                updateItf.Id = Parent.Id;


                if (!string.IsNullOrEmpty(submitResponse.request))
                {
                    updateItf.Attributes.Add("va_corpdbrequest", submitResponse.request);
                }

                if (!string.IsNullOrEmpty(submitResponse.response))
                {
                    updateItf.Attributes.Add("va_corpdbresponse", submitResponse.response);
                }

                if (submitResponse.IntentToFileDto != null && submitResponse.IntentToFileDto.mcs_createDtSpecified)
                {
                    //CSDEv REm
                    //PluginExecutionContext.OutputParameters["createDt"] = submitResponse.IntentToFileDto.mcs_createDt.ToShortDateString();
                    PluginExecutionContext.OutputParameters["createDt"] = submitResponse.IntentToFileDto.mcs_createDt.ToString();

                    DateTime checkDate;
                    if (DateTime.TryParse(submitResponse.IntentToFileDto.mcs_createDt, out checkDate))
                    {
                        updateItf["va_createddate"] = checkDate;
                    }

                }

                if (submitResponse.IntentToFileDto != null && submitResponse.IntentToFileDto.mcs_rcvdDtSpecified)
                {
                    //updateItf["va_intenttofiledate"] = submitResponse.IntentToFileDto.mcs_rcvdDt;

                    DateTime checkDate;
                    if (DateTime.TryParse(submitResponse.IntentToFileDto.mcs_rcvdDt, out checkDate))
                    {
                        updateItf["va_intenttofiledate"] = checkDate;
                    }
                }

                if (!string.IsNullOrEmpty(submitResponse.jrnLctnId))
                {
                    PluginExecutionContext.OutputParameters["jrnLctnId"] = submitResponse.jrnLctnId;
                    updateItf.Attributes.Add("va_stationlocation", submitResponse.jrnLctnId);
                }

                if (submitResponse.IntentToFileDto != null && !string.IsNullOrEmpty(submitResponse.IntentToFileDto.mcs_jrnUserId))
                {
                    PluginExecutionContext.OutputParameters["jrnUserId"] = submitResponse.IntentToFileDto.mcs_jrnUserId;
                    updateItf.Attributes.Add("va_userid", submitResponse.IntentToFileDto.mcs_jrnUserId);
                }

                if (!string.IsNullOrEmpty(submitResponse.submtrApplcnTypeCd))
                {
                    PluginExecutionContext.OutputParameters["submtrApplcnTypeCd"] = submitResponse.submtrApplcnTypeCd;
                    updateItf.Attributes.Add("va_sourceapplicationname", submitResponse.submtrApplcnTypeCd);
                }

                if (submitResponse.IntentToFileDto != null && submitResponse.IntentToFileDto.mcs_intentToFileIdSpecified)
                {
                    updateItf.Attributes.Add("udo_intenttofileexternalid", submitResponse.IntentToFileDto.mcs_intentToFileId.ToString());
                }

                if (submitResponse.IntentToFileDto != null && !string.IsNullOrEmpty(submitResponse.IntentToFileDto.mcs_itfStatusTypeCd))
                {
                    PluginExecutionContext.OutputParameters["ITFStatusCode"] = submitResponse.IntentToFileDto.mcs_itfStatusTypeCd;
                    updateItf.Attributes.Add("va_intenttofilestatus", submitResponse.IntentToFileDto.mcs_itfStatusTypeCd);
                }

                updateItf.Attributes.Add("statecode", new OptionSetValue(1)); // inactive
                updateItf.Attributes.Add("statuscode", new OptionSetValue(2)); // inactive

                OrganizationService.Update(updateItf);
                tracer.Trace("Updated the ITF record - " + _method);
            }
            catch (Exception ex)
            {
                _responseMessage = ex.Message;
                ExceptionOccurred = true;
                return; // don't set result if exception occurred
            }
        }

        private const string FetchIntenttofile =
            "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' count='1'>" +
            "<entity name='va_intenttofile' >" +
            "<attribute name='udo_fromlegacy' />" +
            "<attribute name='va_claimantparticipantid' />" +
            "<attribute name='va_participantid' />" +
            "<attribute name='va_generalbenefittype' />" +
            "<attribute name='va_veteranfirstname' />" +
            "<attribute name='va_veteranlastname' />" +
            "<attribute name='va_veteranmiddleinitial' />" +
            "<attribute name='va_veteranssn' />" +
            "<attribute name='va_veteranfilenumber' />" +
            "<attribute name='va_veterandateofbirth' />" +
            "<attribute name='va_veterangender' />" +
            "<attribute name='va_veterangendername' />" +
            "<attribute name='va_claimantfirstname' />" +
            "<attribute name='va_claimantlastname' />" +
            "<attribute name='va_claimantmiddleinitial' />" +
            "<attribute name='va_claimantssn' />" +
            "<attribute name='va_veteranphone' />" +
            "<attribute name='va_veteranemail' />" +
            "<attribute name='va_veteranaddressline1' />" +
            "<attribute name='va_veteranaddressline2' />" +
            "<attribute name='va_veterancity' />" +
            "<attribute name='va_veteranstate' />" +
            "<attribute name='va_veteranzip' />" +
            "<attribute name='va_veterancountry' />" +
            "<attribute name='va_stationlocation' />" +
            "<attribute name='ownerid' />" +
            "<filter>" +
            "<condition attribute='va_intenttofileid' operator='eq' value='{0}' />" +
            "</filter>" +
            "</entity>" +
            "</fetch>";

        internal bool DidWeFindData()
        {
            try
            {
                Logger.setMethod = "DidWeFindData";

                var fetchxml = string.Format(FetchIntenttofile, Parent.Id.ToString());
                var response = OrganizationService.RetrieveMultiple(new FetchExpression(fetchxml));

                if (response.Entities.Count > 0)
                {
                    _response = response.Entities.FirstOrDefault();
                    if (_response == null) return false;
                    _owner = _response.GetAttributeValue<EntityReference>("ownerid");

                    if (!_response.Contains("va_participantid"))
                    {
                        _responseMessage = "Veteran Participant Id is missing";
                        return false;
                    }
                }
                else
                {
                    _responseMessage = "ITF record not found";
                    Logger.setMethod = "Execute";
                    return false;
                }

                tracer.Trace("DidWeFindData: Fields have been retrieved and set.");
                Logger.setMethod = "Execute";

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to execute didWeFindData due to: {0}".Replace("{0}",
                    ex.Message));
            }
        }

        private string RemoveNonNumber(string input)
        {
            var rgx = new Regex("[^0-9]");
            return rgx.Replace(input, "");
        }
    }

    public enum GeneralBenefitType
    {
        Compensation = 953850000,
        Pension = 953850001,
        Survivor = 953850002,
    }
}
