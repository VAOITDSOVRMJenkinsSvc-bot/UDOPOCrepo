using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using UDO.CustomActions.DepMaint.Plugins.Messages;
using UDO.LOB.Core;
using UDO.LOB.DependentMaintenance.Messages;
using VRMRest;

namespace UDO.CustomActions.DepMaint.Plugins.School
{
    public class FindSchoolRunner : UDOActionRunner
    {


        public FindSchoolRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_contactlogtimer";
            _logSoapField = "udo_contactlogsoap";
            _debugField = "crme_dependentmaintenance";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_contacttimeout";
            _validEntities = new string[] { "crme_dependentmaintenance" };
        }

        public override void DoAction()
        {
            _method = "DoAction";
            GetSettingValues();
            EntityReference entity = (EntityReference)PluginExecutionContext.InputParameters["ParentEntityReference"];

            string schoolName = (string)PluginExecutionContext.InputParameters["PartialSchoolName"];
            TracingService.Trace("School Name: " + schoolName);
            string state = (string)PluginExecutionContext.InputParameters["State"];
            TracingService.Trace("State is : " + state);
            string facilityID = (string)PluginExecutionContext.InputParameters["FacilityID"];
            TracingService.Trace("Facility id : " + facilityID);
            var schoolText = " ";
            //HeaderInfo HeaderInfo = GetHeaderInfo();
            UDOHeaderInfo HeaderInfo = new UDOHeaderInfo();
            LogSettings _logSettings = new LogSettings() { Org = PluginExecutionContext.OrganizationName, ConfigFieldName = "RESTCALL", UserId = PluginExecutionContext.InitiatingUserId };
            //if (facilityID == null) //this means name is provided
            //{
            //    TracingService.Trace("1");
            //}
            //else // this means facility IF 
            //{
            //    TracingService.Trace("2");
            //}
            #region SearchSchoolRequest
            var request = new SearchSchoolInfoRequest()
            {
                MessageId = PluginExecutionContext.CorrelationId.ToString(),
                OrganizationName = PluginExecutionContext.OrganizationName,
                UserId = PluginExecutionContext.UserId,
                Debug = McsSettings.getDebug,
                LogSoap = _logSoap,
                LogTiming = _logTimer,
                LegacyServiceHeaderInfo = HeaderInfo,
                searcheduinstitutessearchstringInfo = new SearchSchoolInfoRequest.VEISsrcheduinstsearcheduinstitutessearchstringPlugin
                {
                    mcs_instituteName = schoolName,
                    mcs_facilityCode = facilityID
                },
                edustateInfo = new SearchSchoolInfoRequest.VEISsrcheduinstedustatePlugin
                {
                    mcs_stateCodeORForeignCountry = state,
                    mcs_stateNumber = ""
                }
            };
            #endregion SearchSchoolRequest

            TracingService.Trace("SearchSchoolRequest: " + request);

            #region SearchSchoolResponse
            var response = Utility.SendReceive<SearchSchoolInfoResponse>(_uri, "Bgs#SearchSchoolInfoRequest", request, _logSettings, 0, _crmAuthTokenConfig, TracingService);
            #endregion SearchSchoolResponse
            TracingService.Trace("Search School Response: " + response);

            var exceptionOccured = response.ExceptionOccured;
            TracingService.Trace("Value of Exception Occured: " + exceptionOccured);
            if (exceptionOccured == true)
            {
                PluginExecutionContext.OutputParameters["ResponseMessage"] = "Search school failed.";
                PluginExecutionContext.OutputParameters["Exception"] = true;
                return;
            }
            if(response.facilityCode != null)
            {
                TracingService.Trace("Facility Code List: " + response.facilityCode);
                TracingService.Trace("Facility Code count: " + response.facilityCode.Count);
                #region one school
                if (response.facilityCode.Count == 1)
                {
                    TracingService.Trace("found one school.");
                    foreach (var schoolCode in response.facilityCode)
                    {
                        TracingService.Trace("School Code: " + schoolCode);
                        var getSchoolRequest = new GetSchoolInfoRequest()
                        {
                            MessageId = PluginExecutionContext.CorrelationId.ToString(),
                            OrganizationName = PluginExecutionContext.OrganizationName,
                            UserId = PluginExecutionContext.UserId,
                            Debug = McsSettings.getDebug,
                            LogSoap = _logSoap,
                            LogTiming = _logTimer,
                            LegacyServiceHeaderInfo = HeaderInfo,
                            mcs_fullFacilityCode = schoolCode
                        };

                        TracingService.Trace("Get School Request: " + getSchoolRequest);
                        var getEduInstitutesResponse = Utility.SendReceive<GetSchoolInfoResponse>(_uri, "Bgs#GetSchoolInfoRequest", getSchoolRequest, _logSettings, 0, _crmAuthTokenConfig, TracingService);
                        TracingService.Trace("Get School Response: " + getEduInstitutesResponse);
                        exceptionOccured = getEduInstitutesResponse.ExceptionOccured;
                        TracingService.Trace("Exception Occured: " + exceptionOccured);
                        if (exceptionOccured == true)
                        {
                            PluginExecutionContext.OutputParameters["ResponseMessage"] = "Get school info failed.";
                            PluginExecutionContext.OutputParameters["Exception"] = true;
                            return;
                        }
                        if (getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo != null)
                        {

                            if (getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_state != null)
                            {
                                TracingService.Trace("Executing Fetch");
                                var stateDetails = @"<fetch>
                                                  <entity name='crme_stateorprovincelookup'>
                                                    <attribute name = 'crme_stateorprovince' />
                                                     <attribute name = 'crme_stateorprovincelookupid' />
                                                      <filter type = 'and'>
                                                         <condition attribute = 'crme_stateorprovince' operator= 'eq' value = '" + getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_state + @"' />
                                                           </filter>
                                                         </entity>
                                                       </fetch> ";
                                TracingService.Trace("Done executing fetch");
                                var stateID = new Guid();
                                TracingService.Trace("1");
                                var stateName = string.Empty;
                                TracingService.Trace("2");
                                EntityCollection schoolState = OrganizationService.RetrieveMultiple(new FetchExpression(stateDetails));
                                TracingService.Trace("3");
                                if (schoolState.Entities[0].Contains("crme_stateorprovincelookupid"))
                                {
                                    stateID = schoolState.Entities[0].GetAttributeValue<Guid>("crme_stateorprovincelookupid");
                                    TracingService.Trace("5");
                                    stateName = schoolState.Entities[0].GetAttributeValue<string>("crme_stateorprovince");
                                }

                                TracingService.Trace("Setting output Parameters");
                                TracingService.Trace("School Name is: " + getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_instituteName);
                                TracingService.Trace("School participant ID is: " + getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_participantID);
                                PluginExecutionContext.OutputParameters["SchoolText"] = " ";
                                PluginExecutionContext.OutputParameters["SchoolCode"] = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_participantID;
                                PluginExecutionContext.OutputParameters["SchoolName"] = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_instituteName;
                                PluginExecutionContext.OutputParameters["SchoolAddressLine1"] = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_addressLine1;
                                PluginExecutionContext.OutputParameters["SchoolAddressLine2"] = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_addressLine2;
                                PluginExecutionContext.OutputParameters["SchoolAddressLine3"] = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_addressLine3;
                                PluginExecutionContext.OutputParameters["SchoolCity"] = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_city;
                                PluginExecutionContext.OutputParameters["SchoolStateName"] = stateName;
                                PluginExecutionContext.OutputParameters["SchoolStateID"] = stateID.ToString();
                                PluginExecutionContext.OutputParameters["SchoolZip"] = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_zipcode;
                                TracingService.Trace("Setting output parameters done.");
                                return;
                            }
                            else
                            {
                                TracingService.Trace("not executing fetch as State is blank");
                                TracingService.Trace("Setting output Parameters");
                                TracingService.Trace("School Name is: " + getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_instituteName);
                                TracingService.Trace("School participant ID is: " + getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_participantID);
                                PluginExecutionContext.OutputParameters["SchoolText"] = " ";
                                PluginExecutionContext.OutputParameters["SchoolCode"] = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_participantID;
                                PluginExecutionContext.OutputParameters["SchoolName"] = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_instituteName;
                                PluginExecutionContext.OutputParameters["SchoolAddressLine1"] = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_addressLine1;
                                PluginExecutionContext.OutputParameters["SchoolAddressLine2"] = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_addressLine2;
                                PluginExecutionContext.OutputParameters["SchoolAddressLine3"] = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_addressLine3;
                                PluginExecutionContext.OutputParameters["SchoolCity"] = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_city;
                                PluginExecutionContext.OutputParameters["SchoolStateName"] = " ";
                              //  PluginExecutionContext.OutputParameters["SchoolStateID"] = stateID.ToString();
                                PluginExecutionContext.OutputParameters["SchoolZip"] = getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_zipcode;
                                TracingService.Trace("Setting output parameters done.");
                                return;
                            }
                            
                        }
                        
                    }
                }
                #endregion

                #region multiple school in response

                schoolText = "Your search resulted in more than one record.\n" + "Please refine your search.\n" + "Here are some results for your reference: \n" + " \r----------------";

                TracingService.Trace("school text: " + schoolText);
                if (response.facilityCode != null && response.facilityCode.Count > 1)
                {
                    TracingService.Trace("Found multiple schools");
                    int iteration = 0;
                    foreach (var schoolCode in response.facilityCode)
                    {
                        TracingService.Trace("school Code: " + schoolCode);
                        TracingService.Trace("iteration: " + iteration);
                        if (iteration <= 2)
                        {
                            var getSchoolRequest = new GetSchoolInfoRequest()
                            {
                                MessageId = PluginExecutionContext.CorrelationId.ToString(),
                                OrganizationName = PluginExecutionContext.OrganizationName,
                                UserId = PluginExecutionContext.UserId,
                                Debug = McsSettings.getDebug,
                                LogSoap = _logSoap,
                                LogTiming = _logTimer,
                                LegacyServiceHeaderInfo = HeaderInfo,
                                mcs_fullFacilityCode = schoolCode
                            };

                            TracingService.Trace("get school request" + getSchoolRequest);

                            var getEduInstitutesResponse = Utility.SendReceive<GetSchoolInfoResponse>(_uri, "Bgs#GetSchoolInfoRequest", getSchoolRequest, _logSettings, 0, _crmAuthTokenConfig, TracingService);
                            TracingService.Trace("get school response" + getEduInstitutesResponse);

                            exceptionOccured = getEduInstitutesResponse.ExceptionOccured;

                            TracingService.Trace("Exception Occcured: " + exceptionOccured);
                            if (exceptionOccured == true)
                            {
                                PluginExecutionContext.OutputParameters["ResponseMessage"] = "Get school info failed.";
                                PluginExecutionContext.OutputParameters["Exception"] = true;
                                return;
                            }
                            TracingService.Trace("executing IF ");
                            if (getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo != null)
                            {
                                TracingService.Trace("entered if");
                                schoolText = schoolText + " \n" +  "School Name: " + getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_instituteName + "\n" + getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_addressLine1 + " ";
                                schoolText = schoolText + getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_addressLine2 + " " + getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_addressLine3 + "\n";
                                schoolText = schoolText + getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_city + " " + getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_state + " " + getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.VEISgteduinstdtladdressInfo.mcs_zipcode + "\n";
                                schoolText = schoolText + "Facility ID: " + getEduInstitutesResponse.VEISgteduinstdtleduInstituteInfo.mcs_facilityCode;
                                schoolText = schoolText + " \n" + " \r----------------";
                                TracingService.Trace("Value of School Text is: " + schoolText);
                            }
                        }
                        iteration++;
                    }
                    TracingService.Trace("Setting output Parameters");
                    PluginExecutionContext.OutputParameters["SchoolText"] = schoolText;
                    TracingService.Trace("Setting output Parameters done.");
                    return;
                }
                #endregion
            }
            TracingService.Trace("Setting output Parameters");
            //this means there was nothing returned and so set ouput parameters and send message to UI.
            PluginExecutionContext.OutputParameters["SchoolText"] = "Could not find a matching school. Please refine your search.";
            TracingService.Trace("Setting output Parameters done");
        }
        
    }
}
