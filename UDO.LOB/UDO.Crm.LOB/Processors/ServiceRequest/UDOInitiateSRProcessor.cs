// using CRM007.CRM.SDK.Core;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.ServiceRequest.Messages;
using VRM.Integration.UDO.Common;
using Logger = VRM.Integration.Servicebus.Core.Logger;
using Microsoft.Xrm.Sdk.Query;
using System.Text;
using VIMT.VeteranWebService.Messages;
using System.Security;
using VRM.Integration.UDO.Helpers;
using VIMT.AddressWebService.Messages;

namespace VRM.Integration.UDO.ServiceRequest.Processors
{
    class UDOInitiateSRProcessor
    {
        public string errorString = "";


        public delegate void ProgressSetter(string progress, params object[] args);
        /// <summary>
        /// UpdateProgress: This method is simple, it appends the log with information passed to it
        /// </summary>
        /// <param name="progress">A composite format string with a progress update.</param>
        /// <param name="args">The object(s) to format.</param>
        internal void UpdateProgress(string progress, params object[] args)
        {
            try
            {

                var method = MethodInfo.GetCallingMethod(false).ToString(true);
                string progressString = progress;
                if (args.Length > 0) progressString = string.Format(progress, args);
                if (sr_log == null) sr_log = new StringBuilder();
                sr_log.AppendFormat("Progress:[{0}]: {1}\r\n", method, progressString);
            }
            catch (Exception ex)
            {
                // This should not happen - if it does, then the log is not updated.
            }
        }
        public StringBuilder sr_log { get; set; }

        public IMessageBase Execute(UDOInitiateSRRequest request)
        {
            sr_log = new StringBuilder("SERVICEREQUEST Creation Details:\r\n");
            UpdateProgress("Top of Process");

            //var request = message as InitiateSRRequest;
            UDOInitiateSRResponse response = new UDOInitiateSRResponse();
            //set multiple message exception response

            string vetFileNumber = string.Empty;
            Guid? VetSnapshotId = null;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            #region connect to CRM
            OrganizationServiceProxy OrgServiceProxy;
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);
                OrgServiceProxy.CallerId = request.UserId;
            }
            catch (Exception connectException)
            {
                var method = MethodInfo.GetThisMethod().ToString();
                LogHelper.LogError(request.OrganizationName, request.UserId, method + ", Connection Error", connectException.Message);
                response.ExceptionOccured = true;
                response.ExceptionMessage = "Failed to get CRMConnection";

                return response;
            }
            #endregion

            UpdateProgress("After Connection to CRM");

            try
            {
                // Initialize flags
                var mapSnapshotAddress = false;
                var mapSnapshotContactInfo = false;

                // This variates from the others because we are creating the serviceRequest entity
                // by processing it through it's Create Plugin.

                UpdateProgress("START: Build of udo_servicerequest");

                var svcCaller = OrgServiceProxy.CallerId;
                OrgServiceProxy.CallerId = request.UserId;
                UpdateProgress("Set Caller to {0}", request.UserId);

                #region Set pcrofrecord and dateopened
                var serviceRequest = new Entity("udo_servicerequest");
                serviceRequest["udo_pcrofrecordid"] = new EntityReference("systemuser", request.UserId);
                serviceRequest["udo_dateopened"] = DateTime.Now;
                #endregion

                #region Set transactioncurencyid
                var defaultCurrency = GetDefaultCurrency(OrgServiceProxy, request);
                if (defaultCurrency != null) serviceRequest["transactioncurrencyid"] = defaultCurrency;
                #endregion

                #region Get PersonId, IdProofId, InteractionId, and VeteranId
                Guid? sr_PersonId = String.IsNullOrEmpty(request.udo_PersonId) ? null : (Guid?)Guid.Parse(request.udo_PersonId);
                if (sr_PersonId.HasValue && sr_PersonId.Value == Guid.Empty) sr_PersonId = null;
                sr_log.AppendFormat("PersonId: {0}\r\n", sr_PersonId);
                Guid? sr_IdProofId = String.IsNullOrEmpty(request.udo_IDProofId) ? null : (Guid?)Guid.Parse(request.udo_IDProofId);
                sr_log.AppendFormat("IdProofId: {0}\r\n", sr_IdProofId);
                Guid? sr_InteractionId = String.IsNullOrEmpty(request.udo_InteractionId) ? null : (Guid?)Guid.Parse(request.udo_InteractionId);
                sr_log.AppendFormat("InteractionId: {0}\r\n", sr_InteractionId);
                Guid? sr_VeteranId = String.IsNullOrEmpty(request.udo_VeteranId) ? null : (Guid?)Guid.Parse(request.udo_VeteranId);
                sr_log.AppendFormat("VeteranId: {0}\r\n", sr_VeteranId);
                string sr_RequestType = String.IsNullOrEmpty(request.udo_RequestType) ? null : request.udo_RequestType;
                sr_log.AppendFormat("RequestType: {0}\r\n", sr_RequestType);
                string sr_RequestSubType = String.IsNullOrEmpty(request.udo_RequestSubType) ? null : request.udo_RequestSubType;
                sr_log.AppendFormat("RequestSubType: {0}\r\n", sr_RequestSubType);
                #endregion

                #region Get Address and VetSnapshotId (and idPRoofId if missing) from Person
                if (!sr_PersonId.HasValue)
                {
                    sr_log.AppendLine("*** Is Veteran (without PersonId) ***");
                    serviceRequest["udo_isveteran"] = true;
                    mapSnapshotAddress = true;
                    mapSnapshotContactInfo = true;
                }
                else
                {
                    serviceRequest["udo_personid"] = new EntityReference("udo_person", sr_PersonId.Value);

                    var src_person_cols = new string[] {"udo_idproofid", "udo_vetsnapshotid", "udo_ptcpntid",
                                    "udo_address1","udo_address2","udo_address3",
                                    "udo_city","udo_state","udo_country","udo_zip","udo_ssn",
                                    "udo_dayphone","udo_eveningphone","udo_gender","udo_dob","udo_dobstr","udo_first","udo_last","udo_email"};

                    var dst_person_cols = new string[] {"","", "",
                                    "udo_mailing_address1","udo_mailing_address2","udo_mailing_address3",
                                    "udo_mailing_city","udo_mailing_state","udo_mailingcountry","udo_mailing_zip","udo_srssn",
                                    "udo_dayphone","udo_eveningphone","udo_srgender","udo_srdob","udo_srdobtext","udo_srfirstname","udo_srlastname","udo_sremail"};

                    OrgServiceProxy.CallerId = request.UserId;
                    //**** - Potential Error
                    var person = OrgServiceProxy.Retrieve("udo_person", sr_PersonId.Value, new ColumnSet(src_person_cols));
                    //sr_log.AppendLine(person.DumpToString("person"));
                    sr_log.AppendLine();

                    UpdateAddress(request, person);
                    MapFields(person, src_person_cols, serviceRequest, dst_person_cols);

                    if (person.Contains("udo_vetsnapshotid"))
                    {
                        var idstring = person["udo_vetsnapshotid"].ToString();
                        if (!String.IsNullOrEmpty(idstring))
                        {
                            VetSnapshotId = Guid.Parse(idstring);
                        }
                        sr_log.AppendFormat("VetSnapshotId: {0}\r\n", VetSnapshotId);
                    }

                    if (String.IsNullOrEmpty(person.GetAttributeValue<string>("udo_address1")))
                    {
                        // If there is no address from person, then pull it from the veteran
                        mapSnapshotAddress = true;
                    }
                    if (!sr_IdProofId.HasValue)
                    {
                        if (person.Contains("udo_idproofid")) sr_IdProofId = ((EntityReference)person["udo_idproofid"]).Id;
                        sr_log.AppendLine("*** Could not find idproofid in variables passed, setting idproofid: " + sr_IdProofId.ToString());
                    }
                }
                #endregion

                #region Set idproof on SR and get interaction and veteran from idproof if not found in the request.
                if (sr_IdProofId.HasValue)
                {
                    serviceRequest["udo_servicerequestsid"] = new EntityReference("udo_idproof", sr_IdProofId.Value);
                    if (!sr_InteractionId.HasValue || !sr_VeteranId.HasValue)
                    {
                        OrgServiceProxy.CallerId = request.UserId;
                        var idproof = OrgServiceProxy.Retrieve("udo_idproof", sr_IdProofId.Value, new ColumnSet(
                            "udo_interaction", "udo_veteran"));
                        //sr_log.AppendLine(idproof.DumpToString("idproof"));
                        sr_log.AppendLine();

                        if (idproof.Contains("udo_interaction")) sr_InteractionId = ((EntityReference)idproof["udo_interaction"]).Id;
                        if (idproof.Contains("udo_veteran")) sr_VeteranId = ((EntityReference)idproof["udo_veteran"]).Id;

                        sr_log.AppendLine("*** Could not find either interactionid or veteranid... resetting using idproof.");
                        sr_log.AppendFormat("InteractionId: {0}\r\n", sr_InteractionId);
                        sr_log.AppendFormat("VeteranId: {0}\r\n", sr_VeteranId);
                    }
                }
                #endregion

                #region Get firstname and lastname and relation to vet from the interaction
                if (sr_InteractionId.HasValue)
                {
                    var src_int_cols = new string[] { "udo_firstname", "udo_lastname", "udo_relationship" };

                    var dst_int_cols = new string[] { "udo_firstname", "udo_lastname", "udo_relationtoveteran" };

                    OrgServiceProxy.CallerId = request.UserId;
                    var interaction = OrgServiceProxy.Retrieve("udo_interaction", sr_InteractionId.Value, new ColumnSet(src_int_cols));
                    //sr_log.AppendLine(interaction.DumpToString("interaction"));
                    sr_log.AppendLine();

                    MapFields(interaction, src_int_cols, serviceRequest, dst_int_cols);

                    var reportedby_first = (interaction.Contains("udo_firstname")) ? interaction["udo_firstname"] : string.Empty;
                    var reportedby_last = (interaction.Contains("udo_lastname")) ? interaction["udo_lastname"] : string.Empty;
                    var reportedby_name = (reportedby_first + " " + reportedby_last).Trim();
                    serviceRequest["udo_nameofreportingindividual"] = reportedby_name;
                    serviceRequest["udo_originatinginteractionid"] = interaction.ToEntityReference();
                }
                #endregion

                #region Set udo_relatedveteranid
                if (sr_VeteranId.HasValue)
                {
                    serviceRequest["udo_relatedveteranid"] = new EntityReference("contact", sr_VeteranId.Value);
                }
                #endregion

                #region Get VetSnapshot data and BIRLS data
                try
                {
                    if (VetSnapshotId.HasValue)
                    {
                        #region load veteran snapshot, get birls, copy udo_participantid, udo_vetfirstname, udo_vetlastname, udo_ssn, udo_emailofveteran, udo_filenumber
                        var snap_src_cols = new List<string>(new string[] {
                            "udo_birlsguid", 
                            "udo_participantid",
                            "udo_firstname",
                            "udo_lastname",
                            "udo_ssn",
                            "udo_email",
                            "udo_filenumber", 
                            "udo_poa",
                            "udo_soj", 
                            "udo_dateofdeath", 
                            "udo_branchofservice"
                        });
                        var snap_dst_cols = new List<string>(new string[] {
                            "", 
                            "udo_participantid", 
                            "udo_vetfirstname", 
                            "udo_vetlastname", 
                            "udo_ssn", 
                            "udo_emailofveteran",
                            "udo_filenumber",
                            "udo_poadata",
                            "","","" // udo_soj, udo_dateofdeath, udo_branchofservice
                        });

                        // without the above "","","" if any of the additional mappings were added, it would cause mismatches.
                        if (mapSnapshotAddress)
                        {
                            #region Add Address Mapping
                            snap_src_cols.AddRange(new[] { "udo_address1", "udo_address2", "udo_address3", 
                                                           "udo_mailingcity", "udo_mailingstate", 
                                                           "udo_mailingcountry", "udo_mailingzip"});

                            snap_dst_cols.AddRange(new[] { "udo_mailing_address1", "udo_mailing_address2", "udo_mailing_address3",
                                                           "udo_mailing_city", "udo_mailing_state",
                                                           "udo_mailingcountry", "udo_mailing_zip"});
                            #endregion
                        }

                        if (mapSnapshotContactInfo)
                        {
                            #region Other SR fields
                            snap_src_cols.Add("udo_ssn");
                            snap_dst_cols.Add("udo_srssn");

                            snap_src_cols.Add("udo_dayphone");
                            snap_dst_cols.Add("udo_dayphone");

                            snap_src_cols.Add("udo_eveningphone");
                            snap_dst_cols.Add("udo_eveningphone");

                            snap_src_cols.Add("udo_firstname");
                            snap_dst_cols.Add("udo_srfirstname");

                            snap_src_cols.Add("udo_lastname");
                            snap_dst_cols.Add("udo_srlastname");

                            snap_src_cols.Add("udo_email");
                            snap_dst_cols.Add("udo_sremail");
                            #endregion

                            snap_src_cols.Add("udo_dob");
                            snap_dst_cols.Add("");
                        }

                        UpdateProgress("Retrieving Veteran Snapshot");
                        OrgServiceProxy.CallerId = request.UserId;
                        var snapShot = OrgServiceProxy.Retrieve("udo_veteransnapshot", VetSnapshotId.Value, new ColumnSet(snap_src_cols.Distinct().ToArray()));
                        //sr_log.AppendLine(snapShot.DumpToString());

                        MapFields(snapShot, snap_src_cols.ToArray(), serviceRequest, snap_dst_cols.ToArray());
                        if (snapShot.Contains("udo_filenumber")) vetFileNumber = snapShot["udo_filenumber"].ToString();
                        if (snapShot.Contains("udo_poa"))
                        {
                            serviceRequest["udo_haspoa"] = true;
                            serviceRequest["udo_poadata"] = snapShot["udo_poa"];
                        }

                        sr_log.AppendLine();
                        #endregion

                        #region udo_ssn
                        if (!serviceRequest.Contains("udo_ssn") && snapShot.Contains("udo_ssn"))
                        {
                            serviceRequest["udo_ssn"] = snapShot["udo_ssn"];
                        }
                        #endregion

                        #region udo_regionalofficeid

                        if (snapShot.Contains("udo_soj"))
                        {
                            var udo_FolderLocation = snapShot.GetAttributeValue<string>("udo_soj");

                            if (string.IsNullOrEmpty(udo_FolderLocation))
                            {
                                sr_log.AppendLine("SOJ - Unable to determine.  SnapShot claim folder was blank");
                            }
                            else
                            {
                                sr_log.AppendFormat("SOJ - Folder Location: {0}\r\n", udo_FolderLocation);
                                OrgServiceProxy.CallerId = request.UserId;
                                var soj = getSOJId(OrgServiceProxy, udo_FolderLocation);
                                if (soj != null)
                                {
                                    UpdateProgress("SOJ Found");
                                    sr_log.AppendFormat("SOJ: {0}\r\n", soj);
                                    serviceRequest["udo_regionalofficeid"] = soj;
                                    serviceRequest["udo_regionalofficeidname"] = soj.Name.ToString();
                                }
                                else
                                {
                                    UpdateProgress("SOJ Not Found");
                                }
                            }
                        }
                        #endregion

                        PopulateDateField(serviceRequest, "udo_dateofbirth", snapShot.GetAttributeValue<string>("udo_dob"));
                        PopulateDateField(serviceRequest, "udo_srdob", snapShot.GetAttributeValue<string>("udo_dob"));

                        if (!string.IsNullOrEmpty(snapShot.GetAttributeValue<string>("udo_dob")))
                        {
                            serviceRequest["udo_srdobtext"] = snapShot.GetAttributeValue<string>("udo_dob");
                        }
                        //PopulateFieldfromEntity(serviceRequest, "udo_srdobtext", snapShot, snapShot.GetAttributeValue<string>("udo_dob"));

                        UpdateProgress("Date of Death - " + snapShot.GetAttributeValue<string>("udo_dateofdeath"));
                        if (!string.IsNullOrEmpty(snapShot.GetAttributeValue<string>("udo_dateofdeath")))
                        {
                            DateTime newDateTime;
                            if (DateTime.TryParse(snapShot.GetAttributeValue<string>("udo_dateofdeath"), out newDateTime))
                            {
                                if (newDateTime != System.DateTime.MinValue)
                                {
                                    UpdateProgress("Converted Date of Death - " + newDateTime.ToShortDateString());
                                    serviceRequest["udo_dateofdeath"] = newDateTime;
                                }
                            }
                        }

                        /////PopulateDateField(serviceRequest, "udo_dateofdeath", snapShot.GetAttributeValue<string>("udo_dateofdeath"));
                        PopulateFieldfromEntity(serviceRequest, "udo_branchofservice", snapShot, "udo_branchofservice");
                    }
                }
                catch (Exception ex)
                {
                    sr_log.AppendLine("Exception Error in Getting Birls Data:");
                    sr_log.AppendLine("");
                    sr_log.AppendLine(errorString);

                    sr_log.AppendLine(ex.Message);
                }
                #endregion

                #region Get email of veteran
                if (!serviceRequest.Contains("udo_emailofveteran") || String.IsNullOrEmpty(serviceRequest["udo_emailofveteran"].ToString()))
                {
                    UpdateProgress("Before VIMT EC Call: findCorporateRecordByFileNumberRequest");
                    var findCorporateRecordByFileNumberRequest = new VIMTcrpFNfindCorporateRecordByFileNumberRequest
                    {
                        LogTiming = request.LogTiming,
                        LogSoap = request.LogSoap,
                        Debug = request.Debug,
                        RelatedParentEntityName = request.RelatedParentEntityName,
                        RelatedParentFieldName = request.RelatedParentFieldName,
                        RelatedParentId = request.RelatedParentId,
                        UserId = request.UserId,
                        OrganizationName = request.OrganizationName,
                        //non standard fields
                        mcs_filenumber = vetFileNumber
                    };
                    if (request.LegacyServiceHeaderInfo != null)
                    {
                        findCorporateRecordByFileNumberRequest.LegacyServiceHeaderInfo = new VIMT.VeteranWebService.Messages.HeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                    }
                    UpdateProgress("Executing findCorporateRecordByFileNumberRequest");
                    //TODO(NP): Update the VIMT call to VEIS
                    var findCorporateRecordByFileNumberResponse = findCorporateRecordByFileNumberRequest.SendReceive<VIMTcrpFNfindCorporateRecordByFileNumberResponse>(request.ProcessType);
                    UpdateProgress("After VIMT EC Call: findCorporateRecordByFileNumberRequest");

                    if (findCorporateRecordByFileNumberResponse != null &&
                        findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo != null)
                    {
                        var corpRecord = findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo;
                        PopulateField(serviceRequest, "udo_emailofveteran", corpRecord.mcs_emailAddress);
                        if (!serviceRequest.Attributes.Contains("udo_sremail"))
                        {
                            PopulateField(serviceRequest, "udo_sremail", corpRecord.mcs_emailAddress);
                        }
                    }
                }
                #endregion

                #region Set udo_reqnumber
                OrgServiceProxy.CallerId = request.UserId;
                serviceRequest["udo_reqnumber"] = GenerateRequestNumber(OrgServiceProxy, serviceRequest).ToUnsecureString();
                #endregion

                #region Set Request Type & Sub Type

                serviceRequest["udo_requesttype"] = sr_RequestType;
                serviceRequest["udo_requestsubtype"] = sr_RequestSubType;

                if (sr_RequestType == "FNOD" && sr_RequestSubType == "Death of a Non-Veteran Beneficiary")
                {
                    serviceRequest["udo_deceasedisnvb"] = true;
                    serviceRequest["udo_namenvb"] = serviceRequest["udo_srfirstname"] + " " + serviceRequest["udo_srlastname"];
                }
                else
                {
                    serviceRequest["udo_deceasedisnvb"] = false;
                    serviceRequest["udo_namenvb"] = serviceRequest["udo_vetfirstname"] + " " + serviceRequest["udo_vetlastname"];
                }

                #endregion

                //sr_log.Insert(0, serviceRequest.DumpToString("serviceRequest") + "\r\n\r\n\r\n");

                if (request.Debug)
                {
                    var method = MethodInfo.GetThisMethod().ToString();
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, sr_log.ToString());
                }

                // ******* 
                if (!String.IsNullOrEmpty(request.OwnerType) && request.OwnerId.HasValue)
                {
                    serviceRequest["ownerid"] = new EntityReference(request.OwnerType, request.OwnerId.Value);
                }
                else
                {
                    serviceRequest["ownerid"] = new EntityReference("systemuser", request.UserId);
                }

                #region get Dependents
                if (VetSnapshotId.HasValue)
                {
                    QueryByAttribute getDeps = new QueryByAttribute("udo_person");

                    getDeps.AddAttributeValue("udo_veteransnapshotid", VetSnapshotId.Value);
                    //getDeps.AddAttributeValue("udo_type", 752280001); 
                    var getDep_cols = new string[] { "udo_first", "udo_last", "udo_type" };

                    getDeps.ColumnSet = new ColumnSet(getDep_cols.Distinct().ToArray());
                    var deps = OrgServiceProxy.RetrieveMultiple(getDeps);

                    var _deps = new StringBuilder();
                    if (deps != null && deps.Entities.Count > 0)
                    {
                        foreach (var item in deps.Entities)
                        {
                            var type = item.GetAttributeValue<OptionSetValue>("udo_type");
                            if (type.Value != 752280000)
                            {
                                _deps.AppendLine(
                                    String.Format("{0} {1}",
                                    item.GetAttributeValue<string>("udo_first"),
                                    item.GetAttributeValue<string>("udo_last")).Trim());
                            }
                        }
                        serviceRequest["udo_dependentnames"] = _deps.ToString();
                    }
                }
                //752,280,000
                #endregion

                OrgServiceProxy.CallerId = Guid.Empty;
                var sr_id = OrgServiceProxy.Create(serviceRequest); // ERROR POINT - at Microsoft.Xrm.Sdk.Entity.get_Item(String attributeName)
                response.UDOServiceRequestId = sr_id.ToString();


                //TODO: Will need to comment out this section to generate a note.  The note will be created within the form.
                //try
                //{
                //    if (serviceRequest.GetAttributeValue<EntityReference>("udo_personid") != null)
                //    {
                //        UpdateProgress("Create Note");
                //        OrgServiceProxy.CallerId = request.UserId;
                //        var message = MapDNote.GenerateMapdNotes(UpdateProgress, request.OrganizationName, request.UserId, serviceRequest, "Create");
                //        var noteid = MapDNote.Create(UpdateProgress, request, serviceRequest, "New Service Request", message, OrgServiceProxy);
                //    }
                //    else
                //    {
                //        UpdateProgress("Unable to create note, there is no person attached to the service request.");
                //    }
                //}
                //catch (Exception ex)
                //{
                //    var method = MethodInfo.GetThisMethod().ToString();
                //    var errormessage = String.Format("Error: {0}\r\n{1}\r\n\r\nCALL STACK:{2}",
                //        "Could not create note for service request.", ex.Message, ex.StackTrace);

                //    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, errormessage);
                //}
                //var noteid = MapDNote.Create(serviceRequest, "Copy Service Request", message, OrgServiceProxy, request.UserId);

                return response;
            }
            catch (Exception ExecutionException)
            {
                var method = MethodInfo.GetThisMethod().ToString();
                sr_log.Insert(0, "\r\n\r\nLog Details:");
                sr_log.Insert(0, ExecutionException.Message);
                sr_log.Insert(0, "EXECUTION EXCEPTION:\r\n");
                sr_log.AppendFormat("\r\nEXECUTION EXCEPTION: ");
                sr_log.AppendLine(ExecutionException.Message);
                sr_log.AppendLine("\r\nCALL STACK: ");
                sr_log.AppendLine(ExecutionException.StackTrace.ToString());

                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, sr_log.ToString());
                response.ExceptionOccured = true;
                response.ExceptionMessage = "There was an error creating the Service Request.";
                return response;
            }
        }

        private void UpdateAddress(UDOInitiateSRRequest request, Entity person)
        {
            long pid = 0;
            if (person == null ||
                !person.Contains("udo_ptcpntid") ||
                !long.TryParse(person["udo_ptcpntid"].ToString(), out pid)) return;

            var findAllPtcpntAddrsByPtcpntIdRequest = new VIMTfallpidaddpidfindAllPtcpntAddrsByPtcpntIdRequest();
            findAllPtcpntAddrsByPtcpntIdRequest.LogTiming = request.LogTiming;
            findAllPtcpntAddrsByPtcpntIdRequest.LogSoap = request.LogSoap;
            findAllPtcpntAddrsByPtcpntIdRequest.Debug = request.Debug;
            findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findAllPtcpntAddrsByPtcpntIdRequest.RelatedParentId = request.RelatedParentId;
            findAllPtcpntAddrsByPtcpntIdRequest.UserId = request.UserId;
            findAllPtcpntAddrsByPtcpntIdRequest.OrganizationName = request.OrganizationName;

            findAllPtcpntAddrsByPtcpntIdRequest.mcs_ptcpntid = pid;
            if (request.LegacyServiceHeaderInfo != null)
            {
                findAllPtcpntAddrsByPtcpntIdRequest.LegacyServiceHeaderInfo = new VIMT.AddressWebService.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }
            //TODO(NP): Update the VIMT call to VEIS
            var findAllPtcpntAddrsByPtcpntIdResponse = findAllPtcpntAddrsByPtcpntIdRequest.SendReceive<VIMTfallpidaddpidfindAllPtcpntAddrsByPtcpntIdResponse>(request.ProcessType);
            UpdateProgress("After VIMT EC Call");

            if (findAllPtcpntAddrsByPtcpntIdResponse.ExceptionOccured)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().ToString(), findAllPtcpntAddrsByPtcpntIdResponse.ExceptionMessage);
                return;
            }

            var mailingaddress = "";
            if (findAllPtcpntAddrsByPtcpntIdResponse.VIMTfallpidaddpidreturnInfo != null)
            {
                var ptcpntAddrsDTO = findAllPtcpntAddrsByPtcpntIdResponse.VIMTfallpidaddpidreturnInfo;
                foreach (var ptcpntAddrsDTOItem in ptcpntAddrsDTO)
                {
                    var isMailingAddress = false;

                    if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_ptcpntAddrsTypeNm))
                    {
                        if (ptcpntAddrsDTOItem.mcs_ptcpntAddrsTypeNm.Equals("mailing", StringComparison.InvariantCultureIgnoreCase))
                        {
                            isMailingAddress = true;
                        }
                    }

                    #region Is Mailing Address
                    if (isMailingAddress)
                    {
                        person["udo_address1"] = string.Empty;
                        person["udo_address2"] = string.Empty;
                        person["udo_address3"] = string.Empty;
                        person["udo_city"] = string.Empty;
                        person["udo_state"] = string.Empty;
                        person["udo_country"] = string.Empty;
                        person["udo_zip"] = string.Empty;

                        if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_addrsOneTxt))
                        {
                            mailingaddress = ptcpntAddrsDTOItem.mcs_addrsOneTxt;
                            person["udo_address1"] = ptcpntAddrsDTOItem.mcs_addrsOneTxt;
                        }

                        if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_addrsTwoTxt))
                        {
                            mailingaddress += " " + ptcpntAddrsDTOItem.mcs_addrsTwoTxt;
                            person["udo_address2"] = ptcpntAddrsDTOItem.mcs_addrsTwoTxt;
                        }

                        if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_addrsThreeTxt))
                        {
                            person["udo_address3"] = ptcpntAddrsDTOItem.mcs_addrsThreeTxt;
                        }

                        if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_cityNm))
                        {
                            mailingaddress += " " + ptcpntAddrsDTOItem.mcs_cityNm;
                            person["udo_city"] = ptcpntAddrsDTOItem.mcs_cityNm;
                        }

                        if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd))
                        {
                            mailingaddress += " " + ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd;
                            person["udo_city"] = ptcpntAddrsDTOItem.mcs_mltyPostOfficeTypeCd;
                        }

                        if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_postalCd))
                        {
                            mailingaddress += " " + ptcpntAddrsDTOItem.mcs_postalCd;
                            person["udo_state"] = ptcpntAddrsDTOItem.mcs_postalCd;
                        }

                        if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd))
                        {
                            mailingaddress += " " + ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd;
                            person["udo_state"] = ptcpntAddrsDTOItem.mcs_mltyPostalTypeCd;
                        }

                        if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_cntryNm))
                        {
                            mailingaddress += ", " + ptcpntAddrsDTOItem.mcs_countyNm;
                            person["udo_country"] = ptcpntAddrsDTOItem.mcs_cntryNm;
                        }

                        if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_emailAddrsTxt))
                        {
                            person["udo_email"] = ptcpntAddrsDTOItem.mcs_emailAddrsTxt;
                        }

                        if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_zipPrefixNbr))
                        {
                            person["udo_zip"] = ptcpntAddrsDTOItem.mcs_zipPrefixNbr;
                        }

                        if (!string.IsNullOrEmpty(ptcpntAddrsDTOItem.mcs_frgnPostalCd))
                        {
                            person["udo_zip"] = ptcpntAddrsDTOItem.mcs_frgnPostalCd;
                        }

                        // Exit loop
                        break;
                    }
                    #endregion
                }
            }
        }

        private static void PopulateField(Entity thisNewEntity, string fieldName, string fieldValue)
        {
            if (!string.IsNullOrEmpty(fieldValue))
            {
                thisNewEntity[fieldName] = fieldValue;
            }
        }

        private static void PopulateFieldfromEntity(Entity thisNewEntity, string fieldName, Entity sourceEntity, string fieldValue)
        {
            if (sourceEntity.Attributes.Contains(fieldValue))
            {
                thisNewEntity[fieldName] = sourceEntity[fieldValue];
            }

        }

        private void MapFields(Entity source, string[] sourceCols, Entity dst, string[] dstCols)
        {
            if (sourceCols == null || dstCols == null) return;
            errorString += string.Format("\n columns in source: {0} \n columns in dest: {1}", sourceCols.Length, dstCols.Length);
            for (int i = 0; i < dstCols.Length; i++)
            {
                errorString += string.Format("\n mapping column index {0}", i);
                var srckey = sourceCols[i];
                var dstkey = dstCols[i];
                errorString += "\n srckey: " + srckey + " dstkey: " + dstkey;
                // if the destination key is empty, there is nothing to map.
                if (!String.IsNullOrEmpty(dstkey) && source.Contains(srckey))
                {
                    dst[dstkey] = source[srckey];
                    errorString += "\n mapped " + source.LogicalName + "." + srckey + " : " + source[srckey] + " : " + dst.LogicalName + "." + dstkey + " : " + dst[dstkey];
                }
            }
        }

        private static EntityReference getSOJId(OrganizationServiceProxy OrgServiceProxy, string stationCode)
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

        private static void PopulateDateField(Entity thisNewEntity, string fieldName, string fieldValue)
        {
            if (!string.IsNullOrEmpty(fieldValue))
            {
                DateTime newDateTime;
                if (DateTime.TryParse(fieldValue, out newDateTime))
                {
                    if (newDateTime != System.DateTime.MinValue)
                    {
                        if (newDateTime == Tools.ToCRMDateTime(newDateTime))
                        {
                            thisNewEntity[fieldName] = newDateTime;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Generates the request number for the service request record
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        private SecureString GenerateRequestNumber(OrganizationServiceProxy orgProxy, Entity serviceRequest)
        {
            var today = DateTime.Now.ToString("MM/dd/yy H:mm:ss");
            var num = 1;

            var relatedVeteran = serviceRequest.GetAttributeValue<EntityReference>("udo_relatedveteranid");
            var secureSRSSN = serviceRequest.GetAttributeValue<string>("udo_srssn").ToSecureString();
            var secureSSN = serviceRequest.GetAttributeValue<string>("udo_ssn").ToSecureString();

            //UpdateProgress("Debug SR data:\nudo_srssn: {0}\nudo_ssn:{1}",serviceRequest.GetAttributeValue<string>("udo_srssn")
            //    ,serviceRequest.GetAttributeValue<string>("udo_ssn"));

            StringBuilder fetch = new StringBuilder();

            if (relatedVeteran != null)
            {
                fetch.Append("<condition attribute='udo_relatedveteranid' operator='eq' value='" + relatedVeteran.Id.ToString() + "'/>");
            }
            else if (secureSRSSN.Length != 0)
            {
                fetch.Append("<condition attribute='udo_srssn' operator='eq' value='" + secureSRSSN.ToUnsecureString() + "'/>");
            }
            else if (secureSSN.Length != 0)
            {
                fetch.Append("<condition attribute='udo_ssn' operator='eq' value='" + secureSSN.ToUnsecureString() + "'/>");
            }

            if (fetch.Length > 0)
            {
                fetch.Insert(0, "<fetch count='1'><entity name='udo_servicerequest'><attribute name='udo_reqnumber'/>" +
                                "<filter type='and'>");
                fetch.Append("</filter><order attribute='createdon' descending='true' /></entity></fetch>");

                var results = orgProxy.RetrieveMultiple(new FetchExpression(fetch.ToString()));
                if (results != null && results.Entities.Count > 0)
                {
                    var sr = results.Entities[0];
                    var number = sr.GetAttributeValue<string>("udo_reqnumber");
                    if (!String.IsNullOrEmpty(number))
                    {
                        var reqparts = number.Split(':');
                        if (reqparts.Length > 1)
                        {
                            int lastnum = 0;
                            if (int.TryParse(reqparts[1].Trim(), out lastnum))
                            {
                                num = lastnum + 1;
                            }
                        }
                    }
                }
            }

            SecureString requestNumber;
            if (secureSRSSN.Length != 0)
            {
                requestNumber = secureSRSSN.Copy();
            }
            else if (secureSSN.Length != 0)
            {
                requestNumber = secureSSN.Copy();
            }
            else
            {
                requestNumber = "SR".ToSecureString();
            }
            //UpdateProgress("req-num:1={0}", requestNumber.ToUnsecureString());
            requestNumber = requestNumber.Append(" : ");
            //UpdateProgress("req-num:2={0}", requestNumber.ToUnsecureString());
            requestNumber = requestNumber.Append(num.ToString());
            //UpdateProgress("req-num:3={0}", requestNumber.ToUnsecureString());

            return requestNumber;
        }

        /// <summary>
        /// Gets the default currency for service request from the organization settings
        /// </summary>
        /// <param name="serviceRequest"></param>
        private static EntityReference GetDefaultCurrency(OrganizationServiceProxy orgService, UDOInitiateSRRequest request)
        {
            var fetch = @"<fetch count='1'><entity name='organization'><attribute name='basecurrencyid'/>" +
                        @"<link-entity name='systemuser' to='organizationid' from='organizationid'><filter>" +
                        @"<condition attribute='systemuserid' operator='eq' value='" + request.UserId.ToString() + "' />" +
                        @"</filter></link-entity></entity></fetch>";

            orgService.CallerId = request.UserId;
            var results = orgService.RetrieveMultiple(new FetchExpression(fetch));

            if (results != null && results.Entities.Count > 0)
            {
                var org = results.Entities[0];
                return org.GetAttributeValue<EntityReference>("transactioncurrencyid");
            }
            return null;
        }

        private static string LongBranchOfService(string branchcode)
        {
            //JS switch (branchcode.trim())
            switch (branchcode.Trim())
            {
                case "AF": return "AIR FORCE (AF)";
                case "A": return "ARMY (ARMY)";
                //ARMY AIR CORPS
                case "CG": return "COAST GUARD (CG)";
                case "CA": return "COMMONWEALTH ARMY (CA)";
                case "GCS": return "GUERRILLA AND COMBINATION SVC (GCS)";
                case "M": return "MARINES (M)";
                case "MM": return "MERCHANT MARINES (MM)";
                case "NOAA": return "NATIONAL OCEANIC & ATMOSPHERIC ADMINISTRATION (NOAA)";
                //NAVY (NAVY)
                case "PHS": return "PUBLIC HEALTH SVC (PHS)";
                case "RSS": return "REGULAR PHILIPPINE SCOUT (RSS)";
                //REGULAR PHILIPPINE SCOUT COMBINED WITH SPECIAL
                case "RPS": return "PHILIPPINE SCOUT OR COMMONWEALTH ARMY SVC (RPS)";
                case "SPS": return "SPECIAL PHILIPPINE SCOUTS (SPS)";
                case "WAC": return "WOMEN'S ARMY CORPS (WAC)";
            }
            return branchcode;
        }

    }
}
