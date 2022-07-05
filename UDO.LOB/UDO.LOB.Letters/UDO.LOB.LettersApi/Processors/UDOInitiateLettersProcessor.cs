using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security;
using System.Text;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Configuration;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Letters.Messages;
using UDO.LOB.Ratings.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.AddressWebService;
using VEIS.Messages.ClaimantService;
using VEIS.Messages.PaymentInformationService;
using VEIS.Messages.RatingService;
using VEIS.Messages.VeteranWebService;


namespace UDO.LOB.Letters.Processors
{
    public class UDOInitiateLettersProcessor
    {
        private CrmServiceClient OrgServiceProxy = null;
        private bool _debug { get; set; }
        private string LogBuffer { get; set; }
        string _progressString = "Top of Processor";
        private const string method = "UDOInitiateLettersProcessor";
        public StringBuilder SrLog { get; set; }
        private Entity _thisNewEntity;
        public bool IsVeteranDeceased { get; set; }

        public IMessageBase Execute(UDOInitiateLettersRequest request)
        {

            #region Initialize the following variables

            LogBuffer = string.Empty;

            SrLog = new StringBuilder();
            var response = new UDOInitiateLettersResponse() { MessageId = request?.MessageId };
            _debug = request.Debug;
            if (request != null && request.DiagnosticsContext == null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : string.Empty,
                    VeteranId = request.udo_veteranId,
                };
            }
            TraceLogger aiLogger = new TraceLogger(method, request);
            #endregion

            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }

            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOgetMilitaryInformationProcessor", connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion

            #region Update Progress

            UpdateProgress(GetMethod(MethodInfo.GetThisMethod().ToString(true)), "Letter Processor started - {0}",
                DateTime.Now.ToLongDateString());

            #endregion


            #region Handle Debug & Log Soap

            if (request.Debug)
            {
                var requestMessage = string.Format("Process Type: {0} \r\n\r\nRequest: \r\n\r\n{1}", "UDOInitiateLettersRequest", JsonHelper.Serialize<UDOInitiateLettersRequest>(request));
                LogHelper.LogDebug(request.OrganizationName, request.UserId, _debug, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, GetMethod(MethodInfo.GetThisMethod().ToString(true)), requestMessage, false);
            }

            if (request.LogSoap || response.ExceptionOccurred)
            {
                var requestMessage = "Request: \r\n\r\n" + JsonHelper.Serialize<UDOInitiateLettersRequest>(request);
                LogHelper.LogInfo(request.OrganizationName, request.LogSoap, request.UserId, GetMethod(MethodInfo.GetThisMethod().ToString(true)), requestMessage);
            }

            #endregion

            try
            {
                #region Request Info Requirements Check

                #region Update Progress

                UpdateProgress(GetMethod(MethodInfo.GetThisMethod().ToString(true)), "Letter Processor verify Request Information - {0}", DateTime.Now.ToLongDateString());

                #endregion

                if (request.udo_vetsnapshotId == Guid.Empty)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId,
                        "UDOInitiateLettersProcessor.Execute - {0} /r/n/r/n" + SrLog.ToString(), "Veteran Snapshot is required");
                    response.ExceptionOccurred = true;
                    response.ExceptionMessage = "Veteran Snapshot is required";
                    return response;
                }

                if (request.udo_personId == Guid.Empty)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, "UDOInitiateLettersProcessor.Execute - " + _progressString, "Person Id is required");
                    response.ExceptionOccurred = true;
                    response.ExceptionMessage = "Person Id is required";
                    return response;
                }

                if (request.udo_veteranId == Guid.Empty)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, "UDOInitiateLettersProcessor.Execute - " + _progressString, "Veteran Id is required");
                    response.ExceptionOccurred = true;
                    response.ExceptionMessage = "Veteran Id is required";
                    return response;
                }
                #endregion

                #region Create Letter Generation record
                _thisNewEntity = CreateLetterGeneration(request, response);
                #endregion

                #region Get Veteran Snapshot Information
                var snapShot = GetVeteranSnapShotInfo(request, response, _thisNewEntity);
                #endregion

                #region Get Person Selected Information
                var people = GetPersonInfo(request, response, _thisNewEntity, snapShot);
                #endregion

                #region payments info

                if (people.Contains("udo_payeecodeid"))
                {
                    GetPayments(request, _thisNewEntity, people);
                }

                #endregion

                #region Military Service Data

                var serviceDates = "";
                var discharge = "";
                var bos = "";

                GetMilitaryHistory(request, out serviceDates, out discharge, out bos);
                // This is the latest Military Status goes
                _thisNewEntity["udo_discharge"] = discharge;
                _thisNewEntity["udo_servicedates"] = serviceDates;
                _thisNewEntity["udo_branchofservice"] = bos;

                #endregion

                #region Retrieve BIRLS data

                var claimfolderloc = "";
                var dob = "";
                var dateofdeath = "";
                var TourHist = "";
                var EODDate = "";
                var RADDate = "";
                var charofDis = "";

                GetBirls(request, out claimfolderloc, out dob, out dateofdeath, out TourHist, out EODDate, out RADDate, out charofDis);

                if (!string.IsNullOrEmpty(claimfolderloc))
                {
                    var udoFolderLocation = claimfolderloc;

                    if (request.Debug)
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor, Progess:" + _progressString, "Station of Jurisdiction :" + udoFolderLocation);

                    //REM: CallerId used throughout the processor to set context for security based on UserId which = CallerId
                    //OrgServiceProxy.CallerId = request.UserId;
                    var soj = GetSojId(udoFolderLocation);

                    if (soj != null)
                    {
                        if (request.Debug)
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor, Progess:" + _progressString, "SOJ Found");
                        _thisNewEntity["udo_regionalofficeid"] = (EntityReference)soj;
                        _thisNewEntity["udo_regionalofficeidname"] = soj.Name.ToString();
                        _thisNewEntity["udo_rpotext"] = soj.Name.ToString();
                    }
                    else
                    {
                        if (request.Debug)
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateAwardsProcessor Processor, Progess:" + _progressString, "SOJ Not Found");
                    }
                }

                _thisNewEntity["udo_srdobtext"] = dob;
                //Tools.PopulateDateField(_thisNewEntity, "udo_dateofdeath", dateofdeath);
                _thisNewEntity["udo_militaryservicebranch"] = TourHist;
                _thisNewEntity["udo_militaryserviceeoddate"] = EODDate;
                _thisNewEntity["udo_militaryserviceraddate"] = RADDate;
                _thisNewEntity["udo_characterofdischarge"] = charofDis;

                #endregion

                #region Retrieve Ratings data

                var latestBeginDate = "";
                var combinedRatingDegree = 0;
                var svcConnected = false;
                var otherRatingFound = false;

                var retrievedDisabilityRatings = GetRatings(request, out latestBeginDate, out otherRatingFound, out combinedRatingDegree, out svcConnected);

                _thisNewEntity["udo_ratingdegree"] = combinedRatingDegree.ToString();
                _thisNewEntity["udo_serviceconnecteddisability"] = svcConnected;

                if (otherRatingFound)
                {
                    _thisNewEntity["udo_ratingeffectivedate"] = latestBeginDate;
                }

                #endregion

                #region Get POA FI Data

                GetPoafidData(request, _thisNewEntity);

                #endregion

                #region Perform Final Update to Letter Generation Record

                _thisNewEntity["udo_exceptionoccurred"] = false;
                _thisNewEntity["udo_exceptionmessage"] = "";

                UpdateLetterGeneration(request);

                #endregion

                #region Create Disability records in Child table

                var requestDisabilityCollection = new OrganizationRequestCollection();

                if (retrievedDisabilityRatings != null)
                {

                    foreach (var disabilityRatings in retrievedDisabilityRatings.Entities)
                    {
                        if (disabilityRatings.GetAttributeValue<string>("udo_decisiontypecode") == "SVCCONNCTED" && !disabilityRatings.Attributes.Contains("udo_enddate"))
                        {
                            var thisNewDisabilityEntity = new Entity { LogicalName = "udo_lettergenerationdisability" };
                            thisNewDisabilityEntity["udo_name"] = "Letter Generation Disability";
                            thisNewDisabilityEntity["udo_lettergenerationid"] = new EntityReference("udo_lettergeneration", response.newUDOInitiateLetterId);
                            thisNewDisabilityEntity["udo_percentage"] = disabilityRatings.GetAttributeValue<string>("udo_diagnosticpercent");
                            thisNewDisabilityEntity["udo_disability"] = disabilityRatings.GetAttributeValue<string>("udo_diagnostic");
                            thisNewDisabilityEntity["udo_diagnosticcode"] = disabilityRatings.GetAttributeValue<string>("udo_diagnostictypecode");
                            thisNewDisabilityEntity["udo_effectivedate"] = disabilityRatings.GetAttributeValue<string>("udo_effectivedate");
                            thisNewDisabilityEntity["udo_decisiontypecode"] = disabilityRatings.GetAttributeValue<string>("udo_decisiontypecode");

                            if (!string.IsNullOrEmpty(request.OwnerType) && request.OwnerId.HasValue)
                            {
                                thisNewDisabilityEntity["ownerid"] = new EntityReference(request.OwnerType, request.OwnerId.Value);
                            }
                            else
                            {
                                thisNewDisabilityEntity["ownerid"] = new EntityReference("systemuser", request.UserId);
                            }

                            CreateRequest createDisabilityData = new CreateRequest
                            {
                                Target = thisNewDisabilityEntity
                            };
                            requestDisabilityCollection.Add(createDisabilityData);
                        }
                    }

                    if (requestDisabilityCollection.Count > 0)
                    {
                        //OrgServiceProxy.CallerId = request.UserId;
                        var resultDisabilities = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestDisabilityCollection, request.OrganizationName, Guid.Empty, request.Debug);

                        if (request.Debug)
                        {
                            LogBuffer += resultDisabilities.LogDetail;
                            LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                        }

                        if (resultDisabilities.IsFaulted)
                        {
                            LogHelper.LogError(request.OrganizationName, request.UserId, method, resultDisabilities.ErrorDetail);
                            response.ExceptionMessage = resultDisabilities.FriendlyDetail;
                            response.ExceptionOccurred = true;
                            return response;
                        }
                    }
                }

                #endregion

                #region Create Note
                // REM: (NP) Removed the reference to MapDNote class and  excluded class from project.
                // (NP:Existing Code Commented) Will need to comment out this section to generate a note.  The note will be created within the form.
                //try
                //{
                //    _progressString = "Create Note";
                //    _orgServiceProxy.CallerId = request.UserId;
                //    var message = MapDNote.GenerateMapdNotes(request.OrganizationName, request.UserId, _thisNewEntity, "Create");
                //    var noteid = MapDNote.Create(request, _thisNewEntity, "New Letter", message, _orgServiceProxy, request.ProcessType);
                //    response.NewUdoNoteId = noteid;
                //}
                //catch (Exception ex)
                //{
                //    var method = MethodInfo.GetThisMethod().ToString();
                //    var message = string.Format("Error: {0}\r\n{1}\r\n\r\nCALL STACK:{2}",
                //        "Could not create note for letter generation.", ex.Message, ex.StackTrace);

                //    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + ", Progess:" + _progressString, message);
                //}

                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Exception handler

                var message = string.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                if (ex.InnerException != null)
                {
                    message += string.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                }

                if (ex.StackTrace != null)
                {
                    message += string.Format("Call Stack:\r\n{0}", ex.StackTrace);
                }

                SrLog.Insert(0, message);
                message = SrLog.ToString();

                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, GetMethod(MethodInfo.GetThisMethod().ToString(true)), ex);

                try
                {
                    if (_thisNewEntity.Id != Guid.Empty)
                    {
                        var exceptionEntity = new Entity
                        {
                            LogicalName = _thisNewEntity.LogicalName,
                            Id = _thisNewEntity.Id
                        };

                        exceptionEntity["udo_exceptionoccurred"] = true;
                        exceptionEntity["udo_exceptionmessage"] = string.Format("Unexpected Error Occurred - {0}", ex.Message);
                        exceptionEntity["udo_exceptiondebug"] = message;

                        //OrgServiceProxy.CallerId = Guid.Empty;
                        OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, exceptionEntity, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    }
                }
                catch (Exception ex2)
                {
                    var tmpMessage = ex2.Message;
                    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName,
                        request.MessageId, GetMethod(MethodInfo.GetThisMethod().ToString(true)), ex2);

                }

                response.ExceptionOccurred = true;
                response.ExceptionMessage = ex.Message;
                return response;

                #endregion
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }
        }

        #region Methods

        /*
        public bool ConnectToCrm(UDOInitiateLettersRequest request, UDOInitiateLettersResponse response)
        {
            #region connect to CRM

            try
            {
                var webProxyClient = CrmConnection.Connect<OrganizationWebProxyClient>();
                if (request.UserId != Guid.Empty)
                {
                    webProxyClient.CallerId = request.UserId;
                }
                OrgServiceProxy = webProxyClient as IOrganizationService;
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, string.Format("{0} Processor, Connection Error", method), connectException.Message);
                response.ExceptionOccurred = true;
                response.ExceptionMessage = connectException.Message;

                return false;
            }

            #region Update Progress

            UpdateProgress(GetMethod(MethodInfo.GetThisMethod().ToString(true)), "After Connection - {0}",
                DateTime.Now.ToLongDateString());

            #endregion

            #endregion

            return true;
        }
        */

        public Entity CreateLetterGeneration(UDOInitiateLettersRequest request, UDOInitiateLettersResponse response)
        {
            //***********************************************
            // Start populating the Letter Generation record
            //***********************************************
            _thisNewEntity = new Entity();
            _thisNewEntity.LogicalName = "udo_lettergeneration";
            _thisNewEntity["udo_dateopened"] = DateTime.Now;
            _thisNewEntity["udo_issue"] = new OptionSetValue(953850023);
            _thisNewEntity["udo_action"] = new OptionSetValue(953850006);
            _thisNewEntity["udo_personid"] = new EntityReference("udo_person", request.udo_personId);
            _thisNewEntity["udo_isveteran"] = false;
            if (request.ptcpntId == request.vetptcpntId)
            {
                _thisNewEntity["udo_isveteran"] = true;
            }
            _thisNewEntity["udo_relatedveteranid"] = new EntityReference("contact", request.udo_veteranId);
            _thisNewEntity["udo_exceptionoccurred"] = true;
            _thisNewEntity["udo_exceptionmessage"] = "Letter Generation Iniital Creation";

            if (!string.IsNullOrEmpty(request.OwnerType) && request.OwnerId.HasValue)
            {
                _thisNewEntity["ownerid"] = new EntityReference(request.OwnerType, request.OwnerId.Value);
            }
            else
            {
                _thisNewEntity["ownerid"] = new EntityReference("systemuser", request.UserId);
            }

            _progressString = "Before Create of Letter Generation";
            if (request.Debug)
                LogHelper.LogDebug(request.OrganizationName, request.UserId, _debug, request.RelatedParentId,
                    request.RelatedParentEntityName, request.RelatedParentFieldName,
                    GetMethod(MethodInfo.GetThisMethod().ToString(true)), Tools.DumpToString(_thisNewEntity, "Letter Generation"), false);

            response.newUDOInitiateLetterId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(request.MessageId, _thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
            //OrgServiceProxy.Create(TruncateHelper.TruncateFields(_thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming));

            _thisNewEntity.Id = response.newUDOInitiateLetterId;
            _progressString = "After Create of Letter Generation";

            return _thisNewEntity;
        }

        public Entity GetVeteranSnapShotInfo(UDOInitiateLettersRequest request, UDOInitiateLettersResponse response, Entity newEntity)
        {
            Entity snapShot = null;

            #region Retrieve Veteran Snapshot record

            #region Update Progress

            UpdateProgress(GetMethod(MethodInfo.GetThisMethod().ToString(true)), "Starting Retrieve Veteran Snapshot - {0}",
                DateTime.Now.ToLongDateString());

            #endregion

            var vetSnapCols = new ColumnSet("udo_ratingsguid", "udo_birlsguid", "udo_filenumber", "udo_awardtype", "udo_idproofid", "udo_veteranid", "udo_dateofdeath");
            //OrgServiceProxy.CallerId = request.UserId;
            snapShot = OrgServiceProxy.Retrieve("udo_veteransnapshot", request.udo_vetsnapshotId, vetSnapCols);

            #region Update Progress

            UpdateProgress(GetMethod(MethodInfo.GetThisMethod().ToString(true)), "After Retrieve Veteran Snapshot - {0}",
                DateTime.Now.ToLongDateString());

            #endregion

            IsVeteranDeceased = false;
            if (snapShot.Attributes.Contains("udo_dateofdeath"))
            {
                if (!string.IsNullOrEmpty(snapShot.GetAttributeValue<string>("udo_dateofdeath")))
                {
                    IsVeteranDeceased = true;
                }
            }

            #region Handle _debug

            if (request.Debug)
                LogHelper.LogDebug(request.OrganizationName, request.UserId, _debug, request.RelatedParentId,
                    request.RelatedParentEntityName, request.RelatedParentFieldName,
                    GetMethod(MethodInfo.GetThisMethod().ToString(true)), snapShot.DumpToString("Veteran Snapshot"), false);

            #endregion

            #endregion

            #region Get the Veteran ID from the Veteran Snapshot record

            _progressString = "Validation: Must have a Veteran ID on the Veteran Snapshot record";
            if (!snapShot.Contains("udo_veteranid"))
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOInitiateLettersProcessor.Execute - " + _progressString, "Veteran Id is required on Veteran Snapshot");
                response.ExceptionOccurred = true;
                response.ExceptionMessage = "Veteran Id is required on Veteran Snapshot";
                return snapShot;
            }


            #endregion

            #region Get the IDProof ID from the Veteran Snapshot record

            _progressString = "Validation: Must have a IDProof ID on the Veteran Snapshot record";
            if (!snapShot.Contains("udo_idproofid"))
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOInitiateLettersProcessor.Execute - " + _progressString, "IDProof Id is required on Veteran Snapshot");
                response.ExceptionOccurred = true;
                response.ExceptionMessage = "IDProof Id is required on Veteran Snapshot";
                return snapShot;
            }

            #endregion

            newEntity["udo_relatedveteranid"] = (EntityReference)snapShot["udo_veteranid"];
            newEntity["udo_relatedveteranidname"] = ((EntityReference)snapShot["udo_veteranid"]).Name;
            newEntity["udo_vetcontactid"] = ((EntityReference)snapShot["udo_veteranid"]).Id.ToString();
            newEntity["udo_idproofid"] = (EntityReference)snapShot["udo_idproofid"];
            Tools.PopulateFieldfromEntity(newEntity, "udo_filenumber", snapShot, "udo_filenumber");

            ///THIS IS WRONG - It should not be pulling from the snapshot; it should be pulling from udo_person.udo_awardtypecode (string)
            //Tools.PopulateFieldfromEntity(newEntity, "udo_awardbenefittype", snapShot, "udo_awardtype");

            if (!string.IsNullOrEmpty(snapShot.GetAttributeValue<string>("udo_dateofdeath")))
            {
                DateTime newDateTime;
                if (DateTime.TryParse(snapShot.GetAttributeValue<string>("udo_dateofdeath"), out newDateTime))
                {
                    if (newDateTime != System.DateTime.MinValue)
                    {
                        newEntity["udo_dateofdeath"] = newDateTime;
                    }
                }
            }

            if (request.Debug)
                LogHelper.LogDebug(request.OrganizationName, request.UserId, _debug, request.RelatedParentId,
                    request.RelatedParentEntityName, request.RelatedParentFieldName,
                    GetMethod(MethodInfo.GetThisMethod().ToString(true)), Tools.DumpToString(_thisNewEntity, "Prior to UpdateLetterGeneration"), false);


            UpdateLetterGeneration(request);

            return snapShot;
        }

        public Entity GetPersonInfo(UDOInitiateLettersRequest request, UDOInitiateLettersResponse response, Entity newEntity, Entity snapShot)
        {
            Entity people = null;

            #region Retrieve Person record

            //OrgServiceProxy.CallerId = request.UserId;
            var peopleCols = new ColumnSet("udo_payeecodeid", "udo_awardeffectivedate", "udo_payeecode", "udo_first", "udo_last", "udo_email", "udo_vetssn", "udo_ptcpntid", "udo_address1", "udo_address2", "udo_address3", "udo_city", "udo_state", "udo_zip", "udo_country", "udo_dayphone", "udo_eveningphone", "udo_vetfirstname", "udo_vetlastname", "udo_grossamount", "udo_netamount", "udo_ssn", "udo_dobstr", "udo_filenumber", "udo_awardtypecode");
            people = OrgServiceProxy.Retrieve("udo_person", request.udo_personId, peopleCols);
            UpdateAddress(request, people);
            _progressString = "After Retrieve Person";

            if (request.Debug)
                LogHelper.LogDebug(request.OrganizationName, request.UserId, _debug, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, GetMethod(MethodInfo.GetThisMethod().ToString(true)), people.DumpToString(), false);

            #endregion

            #region Update Progress

            UpdateProgress(GetMethod(MethodInfo.GetThisMethod().ToString(true)), "Starting Get Person Info - {0}",
                DateTime.Now.ToLongDateString());

            #endregion

            //Person address comes from the veteran as well = need to figure out where it comes from for 10, 11, 12 payee codes
            Tools.PopulateFieldfromEntity(newEntity, "udo_participantid", people, "udo_ptcpntid");
            Tools.PopulateFieldfromEntity(newEntity, "udo_mailing_address1", people, "udo_address1");
            Tools.PopulateFieldfromEntity(newEntity, "udo_mailing_address2", people, "udo_address2");
            Tools.PopulateFieldfromEntity(newEntity, "udo_mailing_address3", people, "udo_address3");
            Tools.PopulateFieldfromEntity(newEntity, "udo_mailing_city", people, "udo_city");
            Tools.PopulateFieldfromEntity(newEntity, "udo_mailing_state", people, "udo_state");
            Tools.PopulateFieldfromEntity(newEntity, "udo_mailingcountry", people, "udo_country");
            Tools.PopulateFieldfromEntity(newEntity, "udo_mailing_zip", people, "udo_zip");
            Tools.PopulateFieldfromEntity(newEntity, "udo_srfirstname", people, "udo_first");
            Tools.PopulateFieldfromEntity(newEntity, "udo_srlastname", people, "udo_last");
            Tools.PopulateFieldfromEntity(newEntity, "udo_email", people, "udo_email");
            Tools.PopulateFieldfromEntity(newEntity, "udo_sremail", people, "udo_email");
            Tools.PopulateFieldfromEntity(newEntity, "udo_ssn", people, "udo_ssn");
            Tools.PopulateFieldfromEntity(newEntity, "udo_srssn", people, "udo_ssn");
            Tools.PopulateFieldfromEntity(newEntity, "udo_firstname", people, "udo_first");
            Tools.PopulateFieldfromEntity(newEntity, "udo_lastname", people, "udo_last");
            Tools.PopulateFieldfromEntity(newEntity, "udo_srfirstname", people, "udo_first");
            Tools.PopulateFieldfromEntity(newEntity, "udo_srlastname", people, "udo_last");
            Tools.PopulateFieldfromEntity(newEntity, "udo_address1", people, "udo_address1");
            Tools.PopulateFieldfromEntity(newEntity, "udo_address2", people, "udo_address2");
            Tools.PopulateFieldfromEntity(newEntity, "udo_address3", people, "udo_address3");
            Tools.PopulateFieldfromEntity(newEntity, "udo_city", people, "udo_city");
            Tools.PopulateFieldfromEntity(newEntity, "udo_state", people, "udo_state");
            Tools.PopulateFieldfromEntity(newEntity, "udo_country", people, "udo_country");
            Tools.PopulateFieldfromEntity(newEntity, "udo_zipcode", people, "udo_zip");
            Tools.PopulateFieldfromEntity(newEntity, "udo_awardbenefittype", people, "udo_awardtypecode");

            if (!string.IsNullOrEmpty(people["udo_dobstr"].ToString()))
            {
                DateTime dateofBirth;
                if (DateTime.TryParse(people["udo_dobstr"].ToString(), out dateofBirth))
                {
                    newEntity["udo_dateofbirth"] = dateofBirth;
                }
            }

            if (people.Contains("udo_dayphone"))
            {
                newEntity["udo_dayphone"] = people.GetAttributeValue<string>("udo_dayphone").ToTelephoneFormat();
            }

            if (people.Contains("udo_eveningphone"))
            {
                newEntity["udo_eveningphone"] = people.GetAttributeValue<string>("udo_eveningphone").ToTelephoneFormat();
            }

            Tools.PopulateFieldfromEntity(newEntity, "udo_vetfirstname", people, "udo_vetfirstname");
            Tools.PopulateFieldfromEntity(newEntity, "udo_vetlastname", people, "udo_vetlastname");

            #region Get Veteran People Values

            if (request.ptcpntId != request.vetptcpntId) // If the person is not the veteran, then get veteran people record
            {
                var vetPeople = GetVeteranPeopleRecord(request, (EntityReference)snapShot["udo_idproofid"]);
                UpdateAddress(request, vetPeople);

                Tools.PopulateFieldfromEntity(newEntity, "udo_mailing_address1", vetPeople, "udo_address1");
                Tools.PopulateFieldfromEntity(newEntity, "udo_mailing_address2", vetPeople, "udo_address2");
                Tools.PopulateFieldfromEntity(newEntity, "udo_mailing_address3", vetPeople, "udo_address3");
                Tools.PopulateFieldfromEntity(newEntity, "udo_mailing_city", vetPeople, "udo_city");
                Tools.PopulateFieldfromEntity(newEntity, "udo_mailing_state", vetPeople, "udo_state");
                Tools.PopulateFieldfromEntity(newEntity, "udo_mailingcountry", vetPeople, "udo_country");
                Tools.PopulateFieldfromEntity(newEntity, "udo_mailing_zip", vetPeople, "udo_zip");
                Tools.PopulateFieldfromEntity(newEntity, "udo_ssn", vetPeople, "udo_ssn");


                Tools.PopulateFieldfromEntity(newEntity, "udo_depfirstname", people, "udo_first");
                Tools.PopulateFieldfromEntity(newEntity, "udo_deplastname", people, "udo_last");
                Tools.PopulateFieldfromEntity(newEntity, "udo_depssn", people, "udo_ssn");
                Tools.PopulateFieldfromEntity(newEntity, "udo_depcity", people, "udo_city");
                Tools.PopulateFieldfromEntity(newEntity, "udo_depstate", people, "udo_state");
                Tools.PopulateFieldfromEntity(newEntity, "udo_depdob", people, "udo_dobstr");


                if (!string.IsNullOrEmpty(people["udo_dobstr"].ToString()))
                {
                    DateTime depDateofBirth;
                    if (DateTime.TryParse(people["udo_dobstr"].ToString(), out depDateofBirth))
                    {
                        newEntity["udo_depdateofbirth"] = depDateofBirth;
                    }
                }

                GetCurrentMonthlyRate(request, newEntity, people.GetAttributeValue<string>("udo_ptcpntid"), vetPeople);
            }
            else
            {
                GetCurrentMonthlyRate(request, newEntity, people.GetAttributeValue<string>("udo_ptcpntid"), people);
            }

            #endregion

            UpdateLetterGeneration(request);

            return people;
        }

        public void UpdateLetterGeneration(UDOInitiateLettersRequest request)
        {
            if (_thisNewEntity == null) return;
            if (_thisNewEntity.Id == Guid.Empty) return;

            _progressString = "Before Update of Letter Generation";
            if (request.Debug)
                LogHelper.LogDebug(request.OrganizationName, request.UserId, _debug,
                    request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName,
                    GetMethod(MethodInfo.GetThisMethod().ToString(true)), Tools.DumpToString(_thisNewEntity, "Letter Generation"), false);
            // OrgServiceProxy.CallerId = Guid.Empty;
            OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, _thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
            _progressString = "After Update of Letter Generation";

        }

        /// <summary>
        /// Get Method - Will return the Method Value that contains [Method Name]
        /// </summary>
        /// <returns></returns>
        public string GetMethod(string method)
        {
            var returnMethod = string.Format("{0}_Letters_{1}", method, _progressString);
            return returnMethod;
        }

        /// <summary>
        /// This routine will update the address fields on the Person Entity
        /// </summary>
        /// <param name="request">Letter LOB Request</param>
        /// <param name="person">Person Entity</param>
        public void UpdateAddress(UDOInitiateLettersRequest request, Entity person)
        {
            long pid = 0;
            if (!long.TryParse(person["udo_ptcpntid"].ToString(), out pid)) return;

            // Replaced: VIMTfallpidaddpidfindAllPtcpntAddrsByPtcpntIdRequest
            var findAllPtcpntAddrsByPtcpntIdRequest = new VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdRequest
            {
                MessageId = request.MessageId,
                LogTiming = request.LogTiming,
                LogSoap = request.LogSoap,
                Debug = request.Debug,
                RelatedParentEntityName = request.RelatedParentEntityName,
                RelatedParentFieldName = request.RelatedParentFieldName,
                RelatedParentId = request.RelatedParentId,
                UserId = request.UserId,
                OrganizationName = request.OrganizationName,
                mcs_ptcpntid = pid
            };

            if (request.LegacyServiceHeaderInfo != null)
            {
                findAllPtcpntAddrsByPtcpntIdRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }
            // REM: Invoke VEIS Endpoint
            // Replaced: var findAllPtcpntAddrsByPtcpntIdResponse = findAllPtcpntAddrsByPtcpntIdRequest.SendReceive<VIMTfallpidaddpidfindAllPtcpntAddrsByPtcpntIdResponse>(request.ProcessType);
            //var findAllPtcpntAddrsByPtcpntIdResponse = WebApiUtility.SendReceive<VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdResponse>(veisBaseUri, findAllPtcpntAddrsByPtcpntIdRequest.MessageId, findAllPtcpntAddrsByPtcpntIdRequest, logSettings);
            var findAllPtcpntAddrsByPtcpntIdResponse = WebApiUtility.SendReceive<VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdResponse>(findAllPtcpntAddrsByPtcpntIdRequest, WebApiType.VEIS);
            _progressString = "After VEIS EC Call";

            if (request.LogSoap || findAllPtcpntAddrsByPtcpntIdResponse.ExceptionOccurred)
            {
                if (findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPRequest != null || findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPRequest + findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdRequest Request/Response {requestResponse}", true);
                }
            }

            if (findAllPtcpntAddrsByPtcpntIdResponse.ExceptionOccurred)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().ToString(), findAllPtcpntAddrsByPtcpntIdResponse.ExceptionMessage);
                return;
            }

            var mailingaddress = "";
            // Replaced: VIMTfallpidaddpidreturnInfo
            if (findAllPtcpntAddrsByPtcpntIdResponse.VEISfallpidaddpidfreturnInfo != null)
            {
                var ptcpntAddrsDto = findAllPtcpntAddrsByPtcpntIdResponse.VEISfallpidaddpidfreturnInfo;
                foreach (var ptcpntAddrsDtoItem in ptcpntAddrsDto)
                {
                    var isMailingAddress = false;

                    if (ptcpntAddrsDtoItem.mcs_ptcpntAddrsTypeNm != string.Empty)
                    {

                        if (ptcpntAddrsDtoItem.mcs_ptcpntAddrsTypeNm.Equals("mailing", StringComparison.InvariantCultureIgnoreCase))
                        {
                            isMailingAddress = true;
                            person["udo_address1"] = string.Empty;
                            person["udo_address2"] = string.Empty;
                            person["udo_address3"] = string.Empty;
                            person["udo_city"] = string.Empty;
                            person["udo_state"] = string.Empty;
                            person["udo_country"] = string.Empty;
                            person["udo_zip"] = string.Empty;

                        }
                    }
                    if (ptcpntAddrsDtoItem.mcs_addrsOneTxt != string.Empty)
                    {
                        if (isMailingAddress)
                        {
                            mailingaddress = ptcpntAddrsDtoItem.mcs_addrsOneTxt;
                            person["udo_address1"] = ptcpntAddrsDtoItem.mcs_addrsOneTxt;
                        }
                    }
                    if (ptcpntAddrsDtoItem.mcs_addrsTwoTxt != string.Empty)
                    {
                        if (isMailingAddress)
                        {
                            mailingaddress += " " + ptcpntAddrsDtoItem.mcs_addrsTwoTxt;
                            person["udo_address2"] = ptcpntAddrsDtoItem.mcs_addrsTwoTxt;
                        }
                    }
                    if (ptcpntAddrsDtoItem.mcs_addrsThreeTxt != string.Empty)
                    {
                        if (isMailingAddress)
                        {
                            mailingaddress += " " + ptcpntAddrsDtoItem.mcs_addrsThreeTxt;
                            person["udo_address3"] = ptcpntAddrsDtoItem.mcs_addrsThreeTxt;
                        }
                    }

                    if (!string.IsNullOrEmpty(ptcpntAddrsDtoItem.mcs_cityNm))
                    {
                        if (isMailingAddress)
                        {
                            mailingaddress += " " + ptcpntAddrsDtoItem.mcs_cityNm;
                            person["udo_city"] = ptcpntAddrsDtoItem.mcs_cityNm;
                        }
                    }
                    else if (!string.IsNullOrEmpty(ptcpntAddrsDtoItem.mcs_mltyPostOfficeTypeCd))
                    {
                        if (isMailingAddress)
                        {
                            mailingaddress += " " + ptcpntAddrsDtoItem.mcs_mltyPostOfficeTypeCd;
                            person["udo_city"] = ptcpntAddrsDtoItem.mcs_mltyPostOfficeTypeCd;
                        }
                    }
                    if (!string.IsNullOrEmpty(ptcpntAddrsDtoItem.mcs_postalCd))
                    {
                        if (isMailingAddress)
                        {
                            mailingaddress += " " + ptcpntAddrsDtoItem.mcs_postalCd;
                            person["udo_state"] = ptcpntAddrsDtoItem.mcs_postalCd;
                        }
                    }
                    else if (!string.IsNullOrEmpty(ptcpntAddrsDtoItem.mcs_mltyPostalTypeCd))
                    {
                        if (isMailingAddress)
                        {
                            mailingaddress += " " + ptcpntAddrsDtoItem.mcs_mltyPostalTypeCd;
                            person["udo_state"] = ptcpntAddrsDtoItem.mcs_mltyPostalTypeCd;
                        }
                    }

                    if (ptcpntAddrsDtoItem.mcs_cntryNm != string.Empty)
                    {
                        if (isMailingAddress)
                        {
                            mailingaddress += ", " + ptcpntAddrsDtoItem.mcs_countyNm;
                            person["udo_country"] = ptcpntAddrsDtoItem.mcs_cntryNm;
                        }
                    }

                    if (ptcpntAddrsDtoItem.mcs_emailAddrsTxt != string.Empty)
                    {
                        person["udo_email"] = ptcpntAddrsDtoItem.mcs_emailAddrsTxt;
                    }

                    if (ptcpntAddrsDtoItem.mcs_zipPrefixNbr != string.Empty)
                    {
                        if (isMailingAddress)
                        {
                            person["udo_zip"] = ptcpntAddrsDtoItem.mcs_zipPrefixNbr;
                        }
                    }
                }
            }
        }

        public void GetPoafidData(UDOInitiateLettersRequest request, Entity newEntity)
        {
            var myNow = DateTime.Now;
            var extendedTiwEnd = myNow.Hour.ToString("00") + ":" + myNow.Minute.ToString("00") + ":" + myNow.Second.ToString("00") + ":" + myNow.Millisecond.ToString("000");

            newEntity["udo_hasfiduciary"] = false;
            newEntity["udo_fiduciarydata"] = string.Empty;
            newEntity["udo_haspoa"] = false;
            newEntity["udo_poadata"] = string.Empty;

            //if this doesn't contain anything, don't go asking for it!
            if (!string.IsNullOrEmpty(request.fileNumber))
            {
                if (request.Debug)
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "getPOAFIDData ", extendedTiwEnd + " starting");

                var findAllFiduciaryPoaRequest = new VEISafidpoafindAllFiduciaryPoaRequest
                {
                    MessageId = request.MessageId,
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    mcs_filenumber = request.fileNumber
                };

                //non standard fields
                if (request.LegacyServiceHeaderInfo != null)
                {
                    findAllFiduciaryPoaRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }
                // REM: Invoke VEIS Endpoint
                // Replaced: var findAllFiduciaryPoaResponse = findAllFiduciaryPoaRequest.SendReceive<VIMTafidpoafindAllFiduciaryPoaResponse>(request.ProcessType);
                //var findAllFiduciaryPoaResponse = WebApiUtility.SendReceive<VEISafidpoafindAllFiduciaryPoaResponse>(veisBaseUri, findAllFiduciaryPoaRequest.MessageId, findAllFiduciaryPoaRequest, logSettings);
                var findAllFiduciaryPoaResponse = WebApiUtility.SendReceive<VEISafidpoafindAllFiduciaryPoaResponse>(findAllFiduciaryPoaRequest, WebApiType.VEIS);

                if (request.LogSoap || findAllFiduciaryPoaResponse.ExceptionOccurred)
                {
                    if (findAllFiduciaryPoaResponse.SerializedSOAPRequest != null || findAllFiduciaryPoaResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findAllFiduciaryPoaResponse.SerializedSOAPRequest + findAllFiduciaryPoaResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISafidpoafindAllFiduciaryPoaRequest Request/Response {requestResponse}", true);
                    }
                }

                if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo != null)
                {
                    // Replaced: VIMTafidpoareturnclmsInfo.VIMTafidpoacurrentFiduciaryclmsInfo
                    if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentFiduciaryInfo != null)
                    {
                        #region map current FID data
                        newEntity["udo_hasfiduciary"] = true;
                        var fidData = new StringBuilder();
                        newEntity["udo_fiduciarydata"] = fidData.ToString();
                        #endregion

                    }
                    if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo != null)
                    {
                        #region map current POA data
                        newEntity["udo_haspoa"] = true;
                        var poaData = new StringBuilder();
                        poaData.AppendLine("Name: " + findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrgName);
                        poaData.AppendLine("From: " + findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_beginDate);
                        poaData.AppendLine("To: " + findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_endDate);
                        poaData.AppendLine("Relation: " + findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_relationshipName);
                        poaData.AppendLine("Person or Org: " + findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrOrganizationIndicator);
                        poaData.AppendLine("Temp Custodian: " + findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_temporaryCustodianIndicator);
                        newEntity["udo_poadata"] = poaData.ToString();

                        #endregion
                    }
                }
            }
            UpdateLetterGeneration(request);
        }

        public EntityReference GetSojId(string stationCode)
        {
            EntityReference thisEntRef = new EntityReference();

            QueryExpression expression = new QueryExpression()
            {
                ColumnSet = new ColumnSet("va_regionalofficeid", "va_name"),
                EntityName = "va_regionaloffice",
                Criteria =
                {
                    Filters =
                    {

                        new FilterExpression()
                        {
                          Conditions =
                            {

                                new ConditionExpression("va_code", ConditionOperator.Equal, stationCode)
                            }
                        }
                    }
                }
            };

            EntityCollection results = OrgServiceProxy.RetrieveMultiple(expression);
            if (results.Entities.Count > 0)
            {
                if (results.Entities[0].Attributes.Contains("va_regionalofficeid"))
                {
                    Entity soj = results[0];
                    thisEntRef.Id = soj.Id;
                    thisEntRef.LogicalName = expression.EntityName;
                    thisEntRef.Name = soj.GetAttributeValue<string>("va_name");
                }
            }
            else
            {
                return null;
            }
            return thisEntRef;
        }

        public void GetPayments(UDOInitiateLettersRequest request, Entity newEntity, Entity person)
        {

            var blnContinue = true;
            _progressString = "Top of GetPayments";
            long PID = 0;
            SecureString fileNumber = new SecureString();
            var payeeCode = "";
            if (person.Contains("udo_ptcpntid"))
            {
                PID = Convert.ToInt64(person["udo_ptcpntid"].ToString());
            }
            else
            {
                blnContinue = false;
            }
            if (person.Contains("udo_payeecode"))
            {
                payeeCode = person["udo_payeecode"].ToString();
            }
            else
            {
                blnContinue = false;
            }
            if (person.Contains("udo_ssn"))
            {
                fileNumber = person["udo_ssn"].ToString().ToSecureString();
            }
            else
            {
                blnContinue = false;
            }

            if (blnContinue)
            {
                #region get payments, and build the entity collection, just like the RM would have

                // Replaced: VIMTrtvpmtsumbdnretrievePaymentSummaryWithBDNRequest
                var retrievePaymentSummaryWithBDNRequest = new VEISrtvpmtsumbdnretrievePaymentSummaryWithBDNRequest
                {
                    MessageId = request.MessageId,
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    mcs_participantid = PID,
                    mcs_filenumber = fileNumber.ToUnsecureString(),
                    mcs_payeecode = payeeCode,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    }
                };

                // REM: Invoke VEIS Endpoint
                // var retrievePaymentSummaryWithBDNResponse = retrievePaymentSummaryWithBDNRequest.SendReceive<VIMTrtvpmtsumbdnretrievePaymentSummaryWithBDNResponse>(request.ProcessType);
                var retrievePaymentSummaryWithBDNResponse = WebApiUtility.SendReceive<VEISrtvpmtsumbdnretrievePaymentSummaryWithBDNResponse>(retrievePaymentSummaryWithBDNRequest, WebApiType.VEIS);

                if (request.LogSoap || retrievePaymentSummaryWithBDNResponse.ExceptionOccurred)
                {
                    if (retrievePaymentSummaryWithBDNResponse.SerializedSOAPRequest != null || retrievePaymentSummaryWithBDNResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = retrievePaymentSummaryWithBDNResponse.SerializedSOAPRequest + retrievePaymentSummaryWithBDNResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISrtvpmtsumbdnretrievePaymentSummaryWithBDNRequest Request/Response {requestResponse}", true);
                    }
                }

                _progressString = "After VEIS EC Call";
                var comerica = string.Empty;

                var udOcreatePaymentsArray = new List<Entity>();

                #region process payments
                if (retrievePaymentSummaryWithBDNResponse != null &&
                    // Replaced: VIMTrtvpmtsumbdnPaymentSummaryResponseInfo
                    retrievePaymentSummaryWithBDNResponse.VEISrtvpmtsumbdnrpaymentSummaryResponseVOInfo != null &&
                    retrievePaymentSummaryWithBDNResponse.VEISrtvpmtsumbdnrpaymentSummaryResponseVOInfo.VEISrtvpmtsumbdnrpaymentsInfo != null)
                {
                    var paymentsInfo = retrievePaymentSummaryWithBDNResponse.VEISrtvpmtsumbdnrpaymentSummaryResponseVOInfo.VEISrtvpmtsumbdnrpaymentsInfo;
                    var scheduledDateExists = false;
                    var paymentsSorted = paymentsInfo.OrderByDescending(h => h.mcs_paymentDate);

                    foreach (var hPaymentVOItem in paymentsSorted)
                    {
                        if (!string.IsNullOrEmpty(hPaymentVOItem.mcs_scheduledDate))
                        {
                            DateTime testDate;
                            if (DateTime.TryParse(hPaymentVOItem.mcs_scheduledDate, out testDate))
                            {
                                if (testDate != System.DateTime.MinValue)
                                {
                                    scheduledDateExists = true;
                                }
                            }

                            #region foreach payment record

                            Entity peopleEntity = new Entity();
                            peopleEntity.LogicalName = "udo_person";
                            peopleEntity.Id = request.udo_personId;

                            //instantiate the new Entity
                            Entity thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_payment";
                            thisNewEntity["udo_name"] = "Payment Summary";
                            if (hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo != null)
                            {
                                if (hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_paymentID > 0)
                                {

                                    thisNewEntity["udo_paymentidentifier"] = hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_paymentID.ToString();
                                }

                                if (hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_transactionID > 0)
                                {
                                    thisNewEntity["udo_transactionid"] = hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_transactionID.ToString();
                                }
                            }
                            //if (hPaymentVOItem.mcs_authorizedDate != System.DateTime.MinValue)
                            //{
                            //    thisNewEntity["udo_authorizeddate"] = hPaymentVOItem.mcs_authorizedDate;
                            //}
                            if (!string.IsNullOrEmpty(hPaymentVOItem.mcs_authorizedDate))
                            {
                                var authDate = new DateTime();
                                if (DateTime.TryParse(hPaymentVOItem.mcs_authorizedDate, out authDate))
                                {
                                    thisNewEntity["udo_authorizeddate"] = authDate;
                                }

                            }
                            //if (hPaymentVOItem.mcs_paymentDate != System.DateTime.MinValue)
                            //{
                            //    thisNewEntity["udo_paydate"] = hPaymentVOItem.mcs_paymentDate;
                            //}
                            if (!string.IsNullOrEmpty(hPaymentVOItem.mcs_paymentDate))
                            {
                                DateTime payDate;
                                if (DateTime.TryParse(hPaymentVOItem.mcs_paymentDate, out payDate))
                                {
                                    thisNewEntity["udo_paydate"] = payDate;
                                }
                            }
                            if (hPaymentVOItem.mcs_paymentType != string.Empty)
                            {
                                thisNewEntity["udo_paymenttype"] = hPaymentVOItem.mcs_paymentType;
                            }
                            if (hPaymentVOItem.mcs_programType != string.Empty)
                            {
                                thisNewEntity["udo_programtype"] = hPaymentVOItem.mcs_programType;
                            }
                            if (hPaymentVOItem.mcs_recipientName != string.Empty)
                            {
                                thisNewEntity["udo_recipient"] = hPaymentVOItem.mcs_recipientName;
                            }
                            //if (hPaymentVOItem.mcs_scheduledDate != System.DateTime.MinValue)
                            //{
                            //    thisNewEntity["udo_scheduleddate"] = hPaymentVOItem.mcs_scheduledDate;
                            //}
                            if (!string.IsNullOrEmpty(hPaymentVOItem.mcs_scheduledDate))
                            {
                                DateTime scheduleDate;
                                if (DateTime.TryParse(hPaymentVOItem.mcs_scheduledDate, out scheduleDate))
                                {
                                    thisNewEntity["udo_scheduleddate"] = scheduleDate;
                                }

                            }
                            if (hPaymentVOItem.mcs_payeeType != string.Empty)
                            {
                                thisNewEntity["udo_payeetype"] = hPaymentVOItem.mcs_payeeType;
                            }

                            if (!string.IsNullOrEmpty(hPaymentVOItem.mcs_bdnRecordType))
                            {
                                //   LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Create Payment", "Found BDNRecord");
                                thisNewEntity["udo_programtype"] = hPaymentVOItem.mcs_bdnRecordType;
                                thisNewEntity["udo_recipient"] = hPaymentVOItem.mcs_beneficiaryName;
                            }

                            thisNewEntity["udo_amount"] = hPaymentVOItem.mcs_paymentAmount.ToString().ToMoneyFormat();
                            var addressField = "";
                            #region address info
                            var accountNumber = string.Empty;
                            var routingNumber = string.Empty;

                            if (hPaymentVOItem.VEISrtvpmtsumbdnraddressEFTInfo != null)
                            {
                                var addressEFTVO = hPaymentVOItem.VEISrtvpmtsumbdnraddressEFTInfo;

                                if (addressEFTVO.mcs_accountType != string.Empty)
                                {
                                    thisNewEntity["udo_accounttype"] = addressEFTVO.mcs_accountType;
                                    addressField += " " + addressEFTVO.mcs_accountType;
                                }

                                if (addressEFTVO.mcs_bankName != string.Empty)
                                {
                                    thisNewEntity["udo_bankname"] = addressEFTVO.mcs_bankName;
                                    addressField += " " + addressEFTVO.mcs_bankName;
                                }
                                if (addressEFTVO.mcs_routingNumber != string.Empty)
                                {
                                    addressField += " " + routingNumber;
                                }
                            }
                            #endregion

                            if (hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo != null)
                            {
                                if (hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_paymentID > 0)
                                {

                                    thisNewEntity["udo_paymentidentifier"] = hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_paymentID.ToString();
                                    //responseIds.paymentId = hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo.mcs_paymentID;
                                    //oktoloadPaymentDetails = true;
                                }

                                if (hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_transactionID > 0)
                                {
                                    thisNewEntity["udo_transactionid"] = hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_transactionID.ToString();
                                    // thisNewEntity["udo_ftbid" = Convert.ToInt32(hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo.mcs_transactionID);
                                    //responseIds.ftbid = hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo.mcs_transactionID;
                                    //oktoloadPaymentDetails = true;
                                }
                            }

                            // 5/8/17 - cchano - addToArray used to block addition of payments where returnreason contains value, per defect 487958

                            bool addToArray = true;
                            if (hPaymentVOItem.VEISrtvpmtsumbdnrreturnPaymentInfo != null)
                            {
                                if (hPaymentVOItem.VEISrtvpmtsumbdnrreturnPaymentInfo.mcs_returnReason != null && hPaymentVOItem.VEISrtvpmtsumbdnrreturnPaymentInfo.mcs_returnReason != "")
                                {
                                    //thisNewEntity["udo_returnpayment"] = hPaymentVOItem.VIMTrtvpmtsumbdnreturnPaymentInfo.mcs_returnReason;
                                    addToArray = false;
                                }
                            }

                            #region checkinfo
                            if (hPaymentVOItem.VEISrtvpmtsumbdnrcheckAddressInfo != null)
                            {
                                var addressCheckVO = hPaymentVOItem.VEISrtvpmtsumbdnrcheckAddressInfo;

                                var zipCode = addressCheckVO.mcs_zipCode;
                                peopleEntity["udo_zip"] = zipCode;
                                if (addressCheckVO.mcs_addressLine1 != string.Empty)
                                {
                                    addressField = addressCheckVO.mcs_addressLine1;

                                }
                                if (addressCheckVO.mcs_addressLine2 != string.Empty)
                                {
                                    addressField += " " + addressCheckVO.mcs_addressLine2;

                                }
                                if (addressCheckVO.mcs_addressLine3 != string.Empty)
                                {
                                    addressField += " " + addressCheckVO.mcs_addressLine3;

                                }
                                if (addressCheckVO.mcs_addressLine4 != string.Empty)
                                {
                                    addressField += " " + addressCheckVO.mcs_addressLine4;
                                }
                                if (addressCheckVO.mcs_addressLine5 != string.Empty)
                                {
                                    addressField += " " + addressCheckVO.mcs_addressLine5;

                                }
                                if (addressCheckVO.mcs_addressLine6 != string.Empty)
                                {
                                    addressField += " " + addressCheckVO.mcs_addressLine6;
                                }
                                if (addressCheckVO.mcs_addressLine7 != string.Empty)
                                {
                                    addressField += " " + addressCheckVO.mcs_addressLine7;
                                }
                                if (addressCheckVO.mcs_zipCode != string.Empty)
                                {
                                    addressField += " " + addressCheckVO.mcs_zipCode;

                                }
                            }
                            #endregion

                            thisNewEntity["udo_address"] = addressField;
                            #endregion

                            if (addToArray) udOcreatePaymentsArray.Add(thisNewEntity);
                        }

                    }
                    #endregion

                    #endregion

                    #region payments info

                    var payments = "";
                    var firstPayDate = new DateTime();
                    var firstPaymentAmount = "";
                    var firstPaymenttype = "";
                    var paymentCount = 0;
                    if (udOcreatePaymentsArray.Count() > 0)
                    {
                        if (request.Debug)
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateLettersRequest Processor", "Got Payments:" + udOcreatePaymentsArray.Count());

                        #region create payments
                        foreach (var paymentrecs in udOcreatePaymentsArray)
                        {
                            var paymentdate = "";
                            var paymentAmount = "";

                            var firstRec = false;
                            var includeRecord = false;
                            //don't show education benefits in letters
                            if (paymentrecs.Contains("udo_programtype"))
                            {
                                var pgramType = paymentrecs["udo_programtype"].ToString();

                                if ((pgramType.Equals("compensation", StringComparison.InvariantCultureIgnoreCase)) || (pgramType.Equals("pension", StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    includeRecord = true;
                                }
                            }
                            if (includeRecord)
                            {
                                if (paymentrecs.Contains("udo_paydate"))
                                {
                                    paymentdate = ((DateTime)paymentrecs["udo_paydate"]).ToShortDateString();
                                    if (firstPayDate == new DateTime())
                                    {
                                        firstPayDate = (DateTime)paymentrecs["udo_paydate"];
                                        firstRec = true;
                                    }

                                    if (paymentrecs.Contains("udo_amount"))
                                    {
                                        paymentAmount = paymentrecs["udo_amount"].ToString();
                                        if (firstRec)
                                        {
                                            firstPaymentAmount = paymentrecs["udo_amount"].ToString();
                                        }
                                    }
                                    if (paymentrecs.Contains("udo_paymenttype"))
                                    {
                                        if (firstRec)
                                        {
                                            firstPaymenttype = paymentrecs["udo_paymenttype"].ToString();
                                        }
                                    }
                                    if (paymentCount == 0)
                                    {
                                        payments = paymentdate + "_" + paymentAmount.Replace("$", "");
                                    }
                                    else
                                    {
                                        payments += ";" + paymentdate + "_" + paymentAmount.Replace("$", "");
                                    }

                                    if (firstRec)
                                    {
                                        var paymentDetails = GetPaymentDetailInfo(request, newEntity, paymentrecs);

                                        //Tools.PopulateFieldfromEntityToCurrency(newEntity, "udo_currentmonthlyrate", paymentDetails, "udo_grossamount");
                                        Tools.PopulateFieldfromEntityToCurrency(newEntity, "udo_netamountpaid", paymentDetails, "udo_netamount");

                                        if (paymentDetails.Contains("udo_awardeffectivedate"))
                                        {

                                            var tempDate = paymentDetails["udo_awardeffectivedate"];

                                            DateTime newDateTime;
                                            if (DateTime.TryParse(tempDate.ToString(), out newDateTime))
                                            {
                                                if (newDateTime != System.DateTime.MinValue)
                                                {
                                                    newEntity["udo_effectivedate"] = newDateTime;
                                                }
                                            }


                                            //var effDate = ((DateTime)paymentDetails["udo_awardeffectivedate"]).ToShortDateString();
                                            //Tools.PopulateDateField(newEntity, "udo_effectivedate", tempDate.ToShortDateString());
                                        }


                                    }


                                    paymentCount += 1;
                                }
                            }
                        }
                        #endregion

                        #region Retrieve Veteran's Current Monthly Rate

                        //if (!string.IsNullOrEmpty(request.vetfileNumber))
                        //{
                        //    var findGeneralInformationByFileNumberRequest = new VIMTfgenFNfindGeneralInformationByFileNumberRequest();
                        //    findGeneralInformationByFileNumberRequest.LogTiming = request.LogTiming;
                        //    findGeneralInformationByFileNumberRequest.LogSoap = request.LogSoap;
                        //    findGeneralInformationByFileNumberRequest.Debug = request.Debug;
                        //    findGeneralInformationByFileNumberRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                        //    findGeneralInformationByFileNumberRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                        //    findGeneralInformationByFileNumberRequest.RelatedParentId = request.RelatedParentId;
                        //    findGeneralInformationByFileNumberRequest.UserId = request.UserId;
                        //    findGeneralInformationByFileNumberRequest.OrganizationName = request.OrganizationName;

                        //    findGeneralInformationByFileNumberRequest.mcs_filenumber = request.vetfileNumber;

                        //    if (request.LegacyServiceHeaderInfo != null)
                        //    {
                        //        findGeneralInformationByFileNumberRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                        //        {
                        //            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        //            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        //            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        //            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        //        };
                        //    }

                        //    var findGeneralInformationByFileNumberResponse = findGeneralInformationByFileNumberRequest.SendReceive<VIMTfgenFNfindGeneralInformationByFileNumberResponse>(MessageProcessType.Local);

                        //    if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VIMTfgenFNreturnclmsInfo.mcs_currentMonthlyRate))
                        //    {
                        //        newEntity["udo_currentmonthlyrate"] = moneyStringFormat(findGeneralInformationByFileNumberResponse.VIMTfgenFNreturnclmsInfo.mcs_currentMonthlyRate).ToCurrencyFormat();
                        //    }
                        //}

                        #endregion

                        newEntity["udo_benefittype"] = firstPaymenttype;
                        newEntity["udo_paymentamount"] = firstPaymentAmount.ToCurrencyFormat();
                        newEntity["udo_paydate"] = firstPayDate;
                        newEntity["udo_mpraw"] = payments;

                        UpdateLetterGeneration(request);
                    }

                    #endregion
                }
            }
        }

        public Entity GetPaymentDetailInfo(UDOInitiateLettersRequest request, Entity newEntity, Entity paymentrecs)
        {

            long paymentid = 0;
            long fbtid = 0;

            if (paymentrecs.Contains("udo_paymentidentifier"))
                paymentid = long.Parse(paymentrecs.GetAttributeValue<string>("udo_paymentidentifier"));

            if (paymentrecs.Contains("udo_transactionid"))
                fbtid = long.Parse(paymentrecs.GetAttributeValue<string>("udo_transactionid"));

            var person = new Entity
            {
                LogicalName = "udo_person"
            };


            var retrievePaymentDetailRequest = new VEISrtrpmtdtlretrievePaymentDetailRequest
            {
                MessageId = request.MessageId,
                LogTiming = request.LogTiming,
                LogSoap = request.LogSoap,
                Debug = request.Debug,
                RelatedParentEntityName = request.RelatedParentEntityName,
                RelatedParentFieldName = request.RelatedParentFieldName,
                RelatedParentId = request.RelatedParentId,
                UserId = request.UserId,
                OrganizationName = request.OrganizationName,
                mcs_paymentid = paymentid,
                mcs_fbtid = fbtid,
                LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                },
            };

            // REM: Invoke VEIS Endpoint
            // var retrievePaymentDetailResponse = retrievePaymentDetailRequest.SendReceive<VIMTrtrpmtdtlretrievePaymentDetailResponse>(request.ProcessType);
            var retrievePaymentDetailResponse = WebApiUtility.SendReceive<VEISrtrpmtdtlretrievePaymentDetailResponse>(retrievePaymentDetailRequest, WebApiType.VEIS);
            _progressString = "After VEIS EC Call";

            if (request.LogSoap || retrievePaymentDetailResponse.ExceptionOccurred)
            {
                if (retrievePaymentDetailResponse.SerializedSOAPRequest != null || retrievePaymentDetailResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = retrievePaymentDetailResponse.SerializedSOAPRequest + retrievePaymentDetailResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISrtrpmtdtlretrievePaymentDetailRequest Request/Response {requestResponse}", true);
                }
            }

            var paymentAdjustmentCount = 0;
            var awardadjustmentCount = 0;
            var requestCollection = new OrganizationRequestCollection();

            if (retrievePaymentDetailResponse != null)
            {
                // Replaced: VIMTrtrpmtdtlPaymentDetailResponseInfo = VEISrtrpmtdtlpaymentDetailResponseVOInfo
                if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo != null)
                {
                    #region PaymentDetailsPaymentAdjustment

                    if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlpaymentAdjustmentsInfo != null)
                    {
                        //Map Details: Payment Adjustment Information
                        _progressString = "Beginning Mapping Payment Details: Payment Adjustment";

                        var paymentAdjustmentResponse = retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlpaymentAdjustmentsInfo;

                        if (request.udo_personId != Guid.Empty)
                        {
                            person.Id = request.udo_personId;
                            person["udo_grossamount"] = paymentAdjustmentResponse.mcs_grossPaymentAmount.ToString().ToMoneyFormat();
                            person["udo_netamount"] = paymentAdjustmentResponse.mcs_netPaymentAmount.ToString().ToMoneyFormat();
                            if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo != null)
                            {
                                if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo.mcs_awardEffectiveDate != null)
                                {
                                    person["udo_awardeffectivedate"] = retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo.mcs_awardEffectiveDate;
                                }
                            }
                            person["udo_paymentcomplete"] = true;
                        }
                        _progressString = "Completed Mapping Payment Details: Payment Adjustment";
                    }

                    #endregion
                }
            }

            return person;
        }

        public void GetMilitaryHistory(UDOInitiateLettersRequest request, out string serviceDates, out string discharge, out string bos)
        {
            var findMilitaryServiceDataRequest = new VEISfmirecpicfindMilitaryRecordByPtcpntIdRequest
            {
                MessageId = request.MessageId,
                LogTiming = request.LogTiming,
                LogSoap = request.LogSoap,
                Debug = request.Debug,
                RelatedParentEntityName = request.RelatedParentEntityName,
                RelatedParentFieldName = request.RelatedParentFieldName,
                RelatedParentId = request.RelatedParentId,
                UserId = request.UserId,
                OrganizationName = request.OrganizationName,
                mcs_ptcpntid = request.vetptcpntId.ToString(),
                LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                }
            };
            _progressString = "Request PTCPNT id is: " + findMilitaryServiceDataRequest.mcs_ptcpntid + ", Request Org is: " + findMilitaryServiceDataRequest.OrganizationName + ", Request User ID is: " + findMilitaryServiceDataRequest.UserId;

            // REM: Invoke VEIS Endpoint
            // var findMilitaryServiceDataResponse = findMiliatryServiceDataRequest.SendReceive<VIMTfmirecpicfindMilitaryRecordByPtcpntIdResponse>(request.ProcessType);
            //var findMilitaryServiceDataResponse = WebApiUtility.SendReceive<VEISfmirecpicfindMilitaryRecordByPtcpntIdResponse>(veisBaseUri, findMilitaryServiceDataRequest.MessageId, findMilitaryServiceDataRequest, logSettings);
            var findMilitaryServiceDataResponse = WebApiUtility.SendReceive<VEISfmirecpicfindMilitaryRecordByPtcpntIdResponse>(findMilitaryServiceDataRequest, WebApiType.VEIS);
            _progressString = "After VEIS EC Call";

            if (request.LogSoap || findMilitaryServiceDataResponse.ExceptionOccurred)
            {
                if (findMilitaryServiceDataResponse.SerializedSOAPRequest != null || findMilitaryServiceDataResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findMilitaryServiceDataResponse.SerializedSOAPRequest + findMilitaryServiceDataResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfmirecpicfindMilitaryRecordByPtcpntIdRequest Request/Response {requestResponse}", true);
                }
            }
            
            _progressString = "Beginning Creation of Child Records";

            var enteredactiveduty = "";
            var releasedactiveduty = "";
            serviceDates = "";
            discharge = "";
            bos = "";
            var savedTourNumber = 0;
            if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo == null) return;

            #region Tour History

            if (findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryPersonToursInfo != null)
            {
                var shrinq1By2MilitaryPersonTour = findMilitaryServiceDataResponse.VEISfmirecpicreturnInfo.VEISfmirecpicmilitaryPersonToursInfo;
                foreach (var shrinq1By2MilitaryPersonTourItem in shrinq1By2MilitaryPersonTour)
                {
                    var currentTourNumber = 0;
                    if (int.TryParse(shrinq1By2MilitaryPersonTourItem.mcs_militaryPersonTourNbr, out currentTourNumber))
                    {
                        if (currentTourNumber > savedTourNumber)
                        {
                            savedTourNumber = currentTourNumber;

                            if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_eodDate))
                            {
                                enteredactiveduty = shrinq1By2MilitaryPersonTourItem.mcs_eodDate.ToDateStringFormat();
                            }
                            if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_radDate))
                            {
                                releasedactiveduty = shrinq1By2MilitaryPersonTourItem.mcs_radDate.ToDateStringFormat();
                            }
                            serviceDates = enteredactiveduty.ToDateStringFormat() + "-" + releasedactiveduty.ToDateStringFormat();

                            if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_mpDischargeCharTypeName))
                            {
                                discharge = shrinq1By2MilitaryPersonTourItem.mcs_mpDischargeCharTypeName;
                            }
                            if (!string.IsNullOrEmpty(shrinq1By2MilitaryPersonTourItem.mcs_militarySvcBranchTypeName))
                            {
                                bos = shrinq1By2MilitaryPersonTourItem.mcs_militarySvcBranchTypeName;
                            }
                        }
                    }
                }
            }
            #endregion
        }

        public void GetCurrentMonthlyRate(UDOInitiateLettersRequest request, Entity newEntity, string ptcpntRecipId, Entity vetperson)
        {
            #region Retrieve Veteran's Current Monthly Rate

            #region Update Progress

            UpdateProgress(GetMethod(MethodInfo.GetThisMethod().ToString(true)), "Starting Get Current Monthly Rate - {0}",
                DateTime.Now.ToLongDateString());

            #endregion

            if (request.Debug)
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateLettersProcessor Processor, Progess:" + "GetCurrentMonthlyRate", "Start Retrieve Veteran's Current Monthly Rate - " + vetperson.GetAttributeValue<string>("udo_filenumber"));

            if (!string.IsNullOrEmpty(vetperson.GetAttributeValue<string>("udo_filenumber")))
            {
                var findGeneralInformationByFileNumberRequest =
                    new VEISfgenFNfindGeneralInformationByFileNumberRequest
                    {
                        MessageId = request.MessageId,
                        LogTiming = request.LogTiming,
                        LogSoap = request.LogSoap,
                        Debug = true,
                        RelatedParentEntityName = request.RelatedParentEntityName,
                        RelatedParentFieldName = request.RelatedParentFieldName,
                        RelatedParentId = request.RelatedParentId,
                        UserId = request.UserId,
                        OrganizationName = request.OrganizationName,
                        mcs_filenumber = vetperson.GetAttributeValue<string>("udo_filenumber")
                    };

                if (request.LegacyServiceHeaderInfo != null)
                {
                    findGeneralInformationByFileNumberRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                // REM: Invoke VEIS Endpoint
                // var findGeneralInformationByFileNumberResponse = findGeneralInformationByFileNumberRequest.SendReceive<VIMTfgenFNfindGeneralInformationByFileNumberResponse>(request.ProcessType);
                //var findGeneralInformationByFileNumberResponse = WebApiUtility.SendReceive<VEISfgenFNfindGeneralInformationByFileNumberResponse>(veisBaseUri, findGeneralInformationByFileNumberRequest.MessageId, findGeneralInformationByFileNumberRequest, logSettings);
                var findGeneralInformationByFileNumberResponse = WebApiUtility.SendReceive<VEISfgenFNfindGeneralInformationByFileNumberResponse>(findGeneralInformationByFileNumberRequest, WebApiType.VEIS);

                if (request.LogSoap || findGeneralInformationByFileNumberResponse.ExceptionOccurred)
                {
                    if (findGeneralInformationByFileNumberResponse.SerializedSOAPRequest != null || findGeneralInformationByFileNumberResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findGeneralInformationByFileNumberResponse.SerializedSOAPRequest + findGeneralInformationByFileNumberResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfgenFNfindGeneralInformationByFileNumberRequest Request/Response {requestResponse}", true);
                    }
                }

                if (request.Debug)
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateLettersProcessor Processor, Progess:" + "GetCurrentMonthlyRate", "Retrieve Veteran's Current Monthly Rate - " + findGeneralInformationByFileNumberResponse.ExceptionOccurred.ToString() + " - " + findGeneralInformationByFileNumberResponse.ExceptionMessage);

                if (request.Debug)
                {
                    if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_currentMonthlyRate))
                    {
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateLettersProcessor Processor, Progess:" + "GetCurrentMonthlyRate", "Retrieve Veteran's Current Monthly Rate - " + findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_currentMonthlyRate);
                    }
                    else
                    {
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateLettersProcessor Processor, Progess:" + "GetCurrentMonthlyRate", "Retrieve Veteran's Current Monthly Rate - " + "Current Monthly Rate is NULL or Empty");
                    }
                }

                if (!string.IsNullOrEmpty(findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_currentMonthlyRate))
                {
                    decimal output;
                    if (
                        decimal.TryParse(
                            findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_currentMonthlyRate,
                            out output))
                    {
                        var currentRate = new Money();
                        currentRate.Value =
                            Convert.ToDecimal(
                                findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.mcs_currentMonthlyRate);
                        newEntity["udo_currentmonthlyrate"] = currentRate;
                    }


                    //newEntity["udo_currentmonthlyrate"] = moneyStringFormat(findGeneralInformationByFileNumberResponse.VIMTfgenFNreturnclmsInfo.mcs_currentMonthlyRate).ToCurrencyFormat();
                }
                if (findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.VEISfgenFNawardBenesInfo != null)
                {
                    foreach (var beneClaimsInfo in findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.VEISfgenFNawardBenesInfo)
                    {
                        if (ptcpntRecipId == beneClaimsInfo.mcs_ptcpntRecipId)
                        {
                            var findGeneralInformationByPtcpntIdsRequest = new VEISfgenpidfindGeneralInformationByPtcpntIdsRequest();
                            findGeneralInformationByPtcpntIdsRequest.MessageId = request.MessageId;
                            findGeneralInformationByPtcpntIdsRequest.LogTiming = request.LogTiming;
                            findGeneralInformationByPtcpntIdsRequest.LogSoap = request.LogSoap;
                            findGeneralInformationByPtcpntIdsRequest.Debug = request.Debug;
                            findGeneralInformationByPtcpntIdsRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                            findGeneralInformationByPtcpntIdsRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                            findGeneralInformationByPtcpntIdsRequest.RelatedParentId = request.RelatedParentId;
                            findGeneralInformationByPtcpntIdsRequest.UserId = request.UserId;
                            findGeneralInformationByPtcpntIdsRequest.OrganizationName = request.OrganizationName;
                            if (request.LegacyServiceHeaderInfo != null)
                            {
                                findGeneralInformationByPtcpntIdsRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                                {
                                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                };
                            }
                            findGeneralInformationByPtcpntIdsRequest.mcs_ptcpntvetid = beneClaimsInfo.mcs_ptcpntVetId;
                            findGeneralInformationByPtcpntIdsRequest.mcs_ptcpntbeneid = beneClaimsInfo.mcs_ptcpntBeneId;
                            findGeneralInformationByPtcpntIdsRequest.mcs_ptpcntrecipid = beneClaimsInfo.mcs_ptcpntRecipId;
                            // TODO: VEIS Dependency missing definition for mcs_awardtypecd, but has been added by UDO Mig team as interim fix
                            findGeneralInformationByPtcpntIdsRequest.mcs_awardtypecd = "CPL";

                            // REM: Invoke VEIS Endpoint
                            // var findGeneralInformationByPtcpntIdsResponse = findGeneralInformationByPtcpntIdsRequest.SendReceive<VIMTfgenpidfindGeneralInformationByPtcpntIdsResponse>(request.ProcessType);
                            //var findGeneralInformationByPtcpntIdsResponse = WebApiUtility.SendReceive<VEISfgenpidfindGeneralInformationByPtcpntIdsResponse>(veisBaseUri, findGeneralInformationByPtcpntIdsRequest.MessageId, findGeneralInformationByPtcpntIdsRequest, logSettings);
                            var findGeneralInformationByPtcpntIdsResponse = WebApiUtility.SendReceive<VEISfgenpidfindGeneralInformationByPtcpntIdsResponse>(findGeneralInformationByPtcpntIdsRequest, WebApiType.VEIS);

                            if (request.LogSoap || findGeneralInformationByPtcpntIdsResponse.ExceptionOccurred)
                            {
                                if (findGeneralInformationByPtcpntIdsResponse.SerializedSOAPRequest != null || findGeneralInformationByPtcpntIdsResponse.SerializedSOAPResponse != null)
                                {
                                    var requestResponse = findGeneralInformationByPtcpntIdsResponse.SerializedSOAPRequest + findGeneralInformationByPtcpntIdsResponse.SerializedSOAPResponse;
                                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfgenpidfindGeneralInformationByPtcpntIdsRequest Request/Response {requestResponse}", true);
                                }
                            }

                            if (findGeneralInformationByPtcpntIdsResponse.VEISfgenpidreturnInfo != null)
                            {
                                var generalInfo = findGeneralInformationByPtcpntIdsResponse.VEISfgenpidreturnInfo;

                                if (!string.IsNullOrEmpty(generalInfo.mcs_currentMonthlyRate))
                                {
                                    decimal output;
                                    if (
                                        decimal.TryParse(
                                            generalInfo.mcs_currentMonthlyRate,
                                            out output))
                                    {
                                        var currentRate = new Money();
                                        currentRate.Value = Convert.ToDecimal(generalInfo.mcs_currentMonthlyRate);
                                        newEntity["udo_currentmonthlyrate"] = currentRate;
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }
            }

            #endregion
        }

        public void GetBirls(UDOInitiateLettersRequest request, out string claimfolderloc, out string dob, out string dateofdeath, out string TourHist, out string EODDate, out string RADDate, out string charofDis)
        {
            claimfolderloc = "";
            dob = "";
            dateofdeath = "";
            TourHist = "";
            EODDate = "";
            RADDate = "";
            charofDis = "";

            var findBirlsRecordByFileNumberRequest = new VEISbrlsFNfindBirlsRecordByFileNumberRequest
            {
                MessageId = request.MessageId,
                LogTiming = request.LogTiming,
                LogSoap = request.LogSoap,
                Debug = request.Debug,
                UserId = request.UserId,
                OrganizationName = request.OrganizationName,
                mcs_filenumber = request.vetfileNumber,
                LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                }
            };

            // REM: Invoke VEIS Endpoint
            // var findBirlsRecordByFileNumberResponse = findBirlsRecordByFileNumberRequest.SendReceive<VIMTbrlsFNfindBirlsRecordByFileNumberResponse>(request.ProcessType);
            //var findBirlsRecordByFileNumberResponse = WebApiUtility.SendReceive<VEISbrlsFNfindBirlsRecordByFileNumberResponse>(veisBaseUri, findBirlsRecordByFileNumberRequest.MessageId, findBirlsRecordByFileNumberRequest, logSettings);
            var findBirlsRecordByFileNumberResponse = WebApiUtility.SendReceive<VEISbrlsFNfindBirlsRecordByFileNumberResponse>(findBirlsRecordByFileNumberRequest, WebApiType.VEIS);
            _progressString = "After VEIS EC Call";

            if (request.LogSoap || findBirlsRecordByFileNumberResponse.ExceptionOccurred)
            {
                if (findBirlsRecordByFileNumberResponse.SerializedSOAPRequest != null || findBirlsRecordByFileNumberResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findBirlsRecordByFileNumberResponse.SerializedSOAPRequest + findBirlsRecordByFileNumberResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISbrlsFNfindBirlsRecordByFileNumberRequest Request/Response {requestResponse}", true);
                }
            }

            if (findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo == null) return;

            #region main BIRLS Update

            if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_BIRTH))
            {
                dob = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_BIRTH;
            }
            if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_DEATH))
            {
                dateofdeath = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_DEATH;
            }
            if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CLAIM_FOLDER_LOCATION))
            {
                claimfolderloc = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CLAIM_FOLDER_LOCATION;
            }

            #endregion

            #region milHistory

            var tourHist = new StringBuilder();
            var eodDate = new StringBuilder();
            var radDate = new StringBuilder();
            var _charofDis = new StringBuilder();

            if (findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNSERVICEInfo != null)
            {
                var birlsRecordService = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNSERVICEInfo;

                foreach (var birlsRecordServiceItem in birlsRecordService)
                {

                    if (!string.IsNullOrEmpty(birlsRecordServiceItem.mcs_RELEASED_ACTIVE_DUTY_DATE) && !string.IsNullOrWhiteSpace(birlsRecordServiceItem.mcs_RELEASED_ACTIVE_DUTY_DATE))
                    {
                        radDate.AppendLine(birlsRecordServiceItem.mcs_RELEASED_ACTIVE_DUTY_DATE.ToDateStringFormat());
                    }
                    if (!string.IsNullOrEmpty(birlsRecordServiceItem.mcs_ENTERED_ON_DUTY_DATE) && !string.IsNullOrWhiteSpace(birlsRecordServiceItem.mcs_ENTERED_ON_DUTY_DATE))
                    {
                        eodDate.AppendLine(birlsRecordServiceItem.mcs_ENTERED_ON_DUTY_DATE.ToDateStringFormat());
                    }
                    if (!string.IsNullOrEmpty(birlsRecordServiceItem.mcs_CHAR_OF_SVC_CODE))
                    {
                        _charofDis.AppendLine(birlsRecordServiceItem.mcs_CHAR_OF_SVC_CODE);
                    }
                    if (!string.IsNullOrEmpty(birlsRecordServiceItem.mcs_BRANCH_OF_SERVICE))
                    {
                        tourHist.AppendLine(birlsRecordServiceItem.mcs_BRANCH_OF_SERVICE.ToLongBranchOfService());
                    }
                }
            }

            TourHist = tourHist.ToString();
            EODDate = eodDate.ToString();
            RADDate = radDate.ToString();
            charofDis = _charofDis.ToString();

            #endregion
        }

        /// <summary>
        /// UpdateProgress: This method is simple, it appends the log with information passed to it
        /// </summary>
        /// <param name="method">Method Name</param>
        /// <param name="progress">A composite format string with a progress update.</param>
        /// <param name="args">The object(s) to format.</param>
        public void UpdateProgress(string method, string progress, params object[] args)
        {
            try
            {
                //var method = MethodInfo.GetCallingMethod(false).ToString(true);
                _progressString = progress;
                if (args.Length > 0) _progressString = string.Format(progress, args);
                if (SrLog == null) SrLog = new StringBuilder();
                SrLog.AppendFormat("Progress:[{0}]: {1}\r\n", method, _progressString);
            }
            catch (Exception ex)
            {
                // This should not happen - if it does, then the log is not updated.
            }
        }

        /// <summary>
        /// Get Veteran Rating information
        /// </summary>
        /// <param name="request">Letters LOB Request</param>
        /// <param name="ratingeffectivedate">returns Rate Effective Date</param>
        /// <param name="otherRatingFound">returns Other Ratings Found (true/false)</param>
        /// <param name="combinedRatingDegree">returns Combined Rating Degree</param>
        /// <param name="svcConnected">returns Service Connected Disabilities (true/false)</param>
        /// <returns></returns>
        public EntityCollection GetRatings(UDOInitiateLettersRequest request, out string ratingeffectivedate, out bool otherRatingFound, out int combinedRatingDegree, out bool svcConnected)
        {
            ratingeffectivedate = "";
            var requestCollection = new EntityCollection();
            var otherRatingsCount = 0;
            var disabilityRatingsCount = 0;
            combinedRatingDegree = 0;
            svcConnected = false;
            var latestBeginDate = DateTime.MinValue;
            otherRatingFound = false;

            // Replaced: VIMTfnrtngdtfindRatingDataRequest = VEISVIMTfnrtngdtfindRatingDataRequest
            var findRatingDataRequest = new VEISfnrtngdtfindRatingDataRequest
            {
                MessageId = request.MessageId,
                LogTiming = request.LogTiming,
                LogSoap = request.LogSoap,
                Debug = request.Debug,
                RelatedParentEntityName = request.RelatedParentEntityName,
                RelatedParentFieldName = request.RelatedParentFieldName,
                RelatedParentId = request.RelatedParentId,
                UserId = request.UserId,
                OrganizationName = request.OrganizationName,
                mcs_filenumber = request.vetfileNumber,
                LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                }
            };

            _progressString = "Request FN# is: " + findRatingDataRequest.mcs_filenumber + ", Request Org is: " + findRatingDataRequest.OrganizationName + ", Request User ID is: " + findRatingDataRequest.UserId;
            // REM: Invoke VEIS Endpoint
            // Replaced: var findRatingDataResponse = findRatingDataRequest.SendReceive<VIMTfnrtngdtfindRatingDataResponse>(request.ProcessType);
            //var findRatingDataResponse = WebApiUtility.SendReceive<VEISfnrtngdtfindRatingDataResponse>(veisBaseUri, findRatingDataRequest.MessageId, findRatingDataRequest, logSettings);
            var findRatingDataResponse = WebApiUtility.SendReceive<VEISfnrtngdtfindRatingDataResponse>(findRatingDataRequest, WebApiType.VEIS);
            _progressString = "After VEIS EC Call";

            if (request.LogSoap || findRatingDataResponse.ExceptionOccurred)
            {
                if (findRatingDataResponse.SerializedSOAPRequest != null || findRatingDataResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findRatingDataResponse.SerializedSOAPRequest + findRatingDataResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfnrtngdtfindRatingDataRequest Request/Response {requestResponse}", true);
                }
            }

            _progressString = "Beginning Creation of Child Records";

            if (findRatingDataResponse == null) return requestCollection;
            // Replaced: VIMTfnrtngdtInfo = VEISVIMTfnrtngdtreturnInfo
            if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo == null) return requestCollection;

            #region DisabilityRatings

            if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo != null)
            {
                if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo != null)
                {
                    if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo.VEISVIMTfnrtngdtratings1Info != null)
                    {
                        var disabilityDetails = findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo;

                        if (!string.IsNullOrEmpty(disabilityDetails.mcs_serviceConnectedCombinedDegree))
                        {
                            var ratingDegree = 0;
                            if (int.TryParse(disabilityDetails.mcs_serviceConnectedCombinedDegree, out ratingDegree))
                            {
                                //Calculated Degree Value
                                combinedRatingDegree = ratingDegree;
                                svcConnected = true;
                            }
                        }
                    }
                }
            }

            if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo != null)
            {
                if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo.VEISVIMTfnrtngdtratings1Info != null)
                {
                    // REM: Added Dependency project (UDO.LOB.Ratings.Messages) to solution and referenced it APi 
                    var udOcreateDisabilityRatingsArray = new List<UDOUDOcreateDisabilityRatingsMultipleResponse>();
                    var disabilityRatings = findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo.VEISVIMTfnrtngdtratings1Info;
                    var disabilityDetails = findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo;

                    foreach (var rating in disabilityRatings)
                    {
                        var thisNewEntity = new Entity { LogicalName = "udo_disabilityrating" };

                        thisNewEntity["udo_name"] = "Disability Rating Summary";
                        if (!string.IsNullOrEmpty(disabilityDetails.mcs_serviceConnectedCombinedDegree))
                        {
                            thisNewEntity["udo_svcconncombineddegree"] = disabilityDetails.mcs_serviceConnectedCombinedDegree;
                        }
                        if (!string.IsNullOrEmpty(rating.mcs_beginDate))
                        {
                            thisNewEntity["udo_begindate"] = rating.mcs_beginDate.ToDateStringFormat();
                        }
                        if (!string.IsNullOrEmpty(rating.mcs_endDate))
                        {
                            thisNewEntity["udo_enddate"] = rating.mcs_endDate.ToDateStringFormat();
                        }
                        if (!string.IsNullOrEmpty(disabilityDetails.mcs_legalEffectiveDate))
                        {
                            thisNewEntity["udo_effectivedate"] = disabilityDetails.mcs_legalEffectiveDate.ToDateStringFormat();
                        }
                        if (!string.IsNullOrEmpty(rating.mcs_disabilityDecisionTypeCode))
                        {
                            thisNewEntity["udo_decisiontypecode"] = rating.mcs_disabilityDecisionTypeCode;
                        }
                        if (!string.IsNullOrEmpty(rating.mcs_diagnosticText))
                        {
                            thisNewEntity["udo_diagnostic"] = rating.mcs_diagnosticText;
                        }
                        if (!string.IsNullOrEmpty(rating.mcs_diagnosticPercent))
                        {
                            thisNewEntity["udo_diagnosticpercent"] = rating.mcs_diagnosticPercent;
                        }
                        if (!string.IsNullOrEmpty(rating.mcs_diagnosticTypeCode))
                        {
                            thisNewEntity["udo_diagnostictypecode"] = rating.mcs_diagnosticTypeCode;
                        }
                        requestCollection.Entities.Add(thisNewEntity);
                        disabilityRatingsCount += 1;

                        //if (rating.mcs_disabilityDecisionTypeCode == "SVCCONNCTED" && string.IsNullOrEmpty(rating.mcs_endDate))
                        //{
                        //    var ratingDegree = 0;
                        //    if (int.TryParse(rating.mcs_diagnosticPercent, out ratingDegree))
                        //    {
                        //        combinedRatingDegree += ratingDegree;
                        //        combinedRatingDegree = (combinedRatingDegree > 100) ? 100 : combinedRatingDegree;
                        //    }
                        //}
                    }
                }
            }
            #endregion

            #region Other Ratings

            if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtotherRatingRecordInfo == null) return requestCollection;

            if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtotherRatingRecordInfo.VEISVIMTfnrtngdtratings3Info != null)
            {
                var otherRatings = findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtotherRatingRecordInfo.VEISVIMTfnrtngdtratings3Info;

                foreach (var rating in otherRatings)
                {
                    if (!String.IsNullOrEmpty(rating.mcs_disabilityTypeName))
                    {
                        if (rating.mcs_disabilityTypeName.ToLower().Contains("ch 35") || rating.mcs_disabilityTypeName.ToLower().Contains("permanent and total"))
                        {
                            DateTime otherBeginDate;
                            if (DateTime.TryParseExact(rating.mcs_beginDate, "MMddyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out otherBeginDate))
                            {
                                if (otherBeginDate > latestBeginDate)
                                {
                                    latestBeginDate = otherBeginDate;
                                    otherRatingFound = true;
                                }
                            }
                        }
                    }
                    otherRatingsCount += 1;
                }

                if (otherRatingFound)
                {
                    ratingeffectivedate = latestBeginDate.ToShortDateString();
                }
            }

            #endregion

            return requestCollection;
        }


        /// <summary>
        /// Get Veteran Person Entity values
        /// </summary>
        /// <param name="request">Letters LOB Request</param>
        /// <param name="idProofId">Entity Reference to Id Proof record</param>
        /// <returns></returns>
        public Entity GetVeteranPeopleRecord(UDOInitiateLettersRequest request, EntityReference idProofId)
        {
            QueryExpression qe = new QueryExpression
            {
                EntityName = "udo_person",
                ColumnSet = new ColumnSet()
            };

            qe.ColumnSet.Columns.Add("udo_payeecodeid");
            qe.ColumnSet.Columns.Add("udo_payeecode");
            qe.ColumnSet.Columns.Add("udo_first");
            qe.ColumnSet.Columns.Add("udo_last");
            qe.ColumnSet.Columns.Add("udo_email");
            qe.ColumnSet.Columns.Add("udo_vetssn");
            qe.ColumnSet.Columns.Add("udo_ptcpntid");
            qe.ColumnSet.Columns.Add("udo_dayphone");
            qe.ColumnSet.Columns.Add("udo_eveningphone");
            qe.ColumnSet.Columns.Add("udo_vetfirstname");
            qe.ColumnSet.Columns.Add("udo_vetlastname");
            qe.ColumnSet.Columns.Add("udo_ssn");
            qe.ColumnSet.Columns.Add("udo_address1");
            qe.ColumnSet.Columns.Add("udo_address2");
            qe.ColumnSet.Columns.Add("udo_address3");
            qe.ColumnSet.Columns.Add("udo_city");
            qe.ColumnSet.Columns.Add("udo_state");
            qe.ColumnSet.Columns.Add("udo_country");
            qe.ColumnSet.Columns.Add("udo_zip");
            qe.ColumnSet.Columns.Add("udo_grossamount");
            qe.ColumnSet.Columns.Add("udo_netamount");
            qe.ColumnSet.Columns.Add("udo_awardeffectivedate");
            qe.ColumnSet.Columns.Add("udo_filenumber");

            var fe1 = new FilterExpression(LogicalOperator.And);
            fe1.AddCondition("udo_idproofid", ConditionOperator.Equal, idProofId.Id);

            var fe2 = new FilterExpression(LogicalOperator.And);
            fe2.AddCondition("udo_ptcpntid", ConditionOperator.Equal, request.vetptcpntId.ToString());

            qe.Criteria.AddFilter(fe1);
            qe.Criteria.AddFilter(fe2);

            var ec = OrgServiceProxy.RetrieveMultiple(qe);

            if (ec != null || ec.TotalRecordCount > 0)
                return ec[0];

            return null;
        }

        #endregion

        private static string moneyStringFormat(string thisField)
        {
            var returnField = "";
            try
            {
                Double newValue = 0;
                if (Double.TryParse(thisField, out newValue))
                {
                    returnField = string.Format("{0:C}", newValue);
                }
                else
                {
                    returnField = "$0.00";
                }
            }
            catch (Exception ex)
            {
                returnField = ex.Message;
            }
            return returnField;

        }
    }
}