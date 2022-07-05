using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using VIMT.AddressWebService.Messages;
using VIMT.BenefitClaimService.Messages;
using VIMT.ClaimantWebService.Messages;
using VIMT.RatingWebService.Messages;
using VIMT.VeteranWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.FNOD.Messages;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Query;
using VRM.Integration.UDO.Common;
using Microsoft.Xrm.Sdk.Messages;


namespace VRM.Integration.UDO.FNOD.Processors
{
    internal class UDOInitiateFNODProcessor
    {
        private TimeTracker timer { get; set; }
        private bool _debug { get; set; }
        private string LogBuffer { get; set; }
        private const string method = "UDOInitiateFNODProcessor";

        public UDOInitiateFNODProcessor()
        {
            timer = new TimeTracker();
        }

        public IMessageBase Execute(UDOInitiateFNODRequest request)
        {
            #region Start Timer
            timer.Restart();
            #endregion
            
            string startInit = timer.MarkStart("Initialize Process");

            var response = new UDOInitiateFNODResponse();

            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                {
                    ExceptionMessage = "Called with no message",
                    ExceptionOccured = true
                };
                return response;
            }

            Logger.Instance.Info(string.Format("Message Id:{0}, Type={2}, Recieved diagnostics message: {1}",
                request.MessageId,
                request.MessageId,
                GetType().FullName));

            #region connect to CRM
            OrganizationServiceProxy OrgServiceProxy;
            try
            {
                var CommonFunctions = new CRMConnect();
                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);
            }
            catch (Exception connectException)
            {
                var method = MethodInfo.GetThisMethod().ToString(false);
                LogHelper.LogError(request.OrganizationName, request.UserId, string.Format("{0} Processor, Connection Error", method), connectException.Message);                
                response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                {
                    ExceptionMessage = "Failed to get CRMConnection",
                    ExceptionOccured = true
                };
                return response;
            }

            #endregion

            progressString = "After Connection";

            timer.MarkStop("Initialize Process");

            string ptcpntBeneId = null, ptcpntVetId = null, ptpcntRecipId = null;

            try
            {

                //instantiate the new Entity
                Entity new_va_fnod_entity = new Entity("va_fnod");

                // Because FNOD pulls from the parent record, which is the contact record
                // the methods below follow the contact record's sources

                //TODO: These should use participantId instead of fileNumber whereever possible.

                #region If pid is null then findGeneralInformationByFileNumberRequest (generalInfoByFileNum)
                // This is to take the filenum and get the other ptcpnt id values.
                // Only do this if pid is empty.
                string startGenInfoByFileNum = timer.MarkStart("generalInfoByFileNum");
                var findGeneralInformationByFileNumberRequest = new VIMTfgenFNfindGeneralInformationByFileNumberRequest
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
                    mcs_filenumber = request.vetfileNumber,
                };

                var findGeneralInformationByFileNumberResponse =
                    findGeneralInformationByFileNumberRequest
                    .SendReceive<VIMTfgenFNfindGeneralInformationByFileNumberResponse>(MessageProcessType.Local);
                var generalInfoByFileNum = findGeneralInformationByFileNumberResponse.VIMTfgenFNreturnclmsInfo;
                timer.MarkStop("generalInfoByFileNum");
                long pid = 0;
                if (request.ptcpntId > 0) pid = request.ptcpntId;
                if (generalInfoByFileNum != null)
                {
                    ptcpntBeneId = generalInfoByFileNum.mcs_ptcpntBeneID;
                    ptcpntVetId = generalInfoByFileNum.mcs_ptcpntVetID;
                    ptpcntRecipId = generalInfoByFileNum.mcs_ptcpntRecipID;

                    
                    if (long.TryParse(generalInfoByFileNum.mcs_ptcpntVetID, out pid))
                    {
                        request.ptcpntId = pid;
                    }
                }

                if (ptcpntVetId == null && request.ptcpntId>0) ptcpntVetId = request.ptcpntId.ToString();
                if (ptcpntBeneId == null) ptcpntBeneId = ptcpntVetId;
                if (ptpcntRecipId == null) ptpcntRecipId = ptcpntVetId;
                
                progressString = "After VIMT EC Call - findGeneralInformationByFileNumber";
                #endregion

                #region findCorporateRecordByFileNumber (corpRecord)
                string startFindCorpRecord = timer.MarkStart("findCorporateRecordByFileNumber");
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
                    mcs_filenumber = request.vetfileNumber,
                };

                var findCorporateRecordByFileNumberResponse =
                    findCorporateRecordByFileNumberRequest.SendReceive<VIMTcrpFNfindCorporateRecordByFileNumberResponse>
                        (MessageProcessType.Local);
                progressString = "After VIMT EC Call - findCorporateRecord";
                timer.MarkStop("findCorporateRecordByFileNumber");
                VIMTcrpFNreturn corpRecord = findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo;
                
                // If there is no response, then return the response
                if (findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo == null)
                {
                    response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                    {
                        ExceptionMessage = findCorporateRecordByFileNumberResponse.ExceptionMessage,
                        ExceptionOccured = findCorporateRecordByFileNumberResponse.ExceptionOccured
                    };
                    return response;
                }

                #endregion

                #region findBirlsRecordByFileNumber (birlsRecord)
                string startFindBirls = timer.MarkStart("findBirlsRecordByFileNumber");
                var findBirlsRecordByFileNumberRequest = new VIMTbrlsFNfindBirlsRecordByFileNumberRequest
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    mcs_filenumber = request.vetfileNumber
                };

                var findBirlsRecordByFileNumberResponse =
                    findBirlsRecordByFileNumberRequest.SendReceive<VIMTbrlsFNfindBirlsRecordByFileNumberResponse>(
                        MessageProcessType.Local);
                progressString = "After VIMT EC Call - findBirlsRecord";

                var birlsRecord = findBirlsRecordByFileNumberResponse.VIMTbrlsFNreturnInfo;
                if (findBirlsRecordByFileNumberResponse.VIMTbrlsFNreturnInfo == null)
                {
                    response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                    {
                        ExceptionMessage = findBirlsRecordByFileNumberResponse.ExceptionMessage,
                        ExceptionOccured = findBirlsRecordByFileNumberResponse.ExceptionOccured
                    };
                    return response;
                }
                timer.MarkStop("findBirlsRecordByFileNumber");
                #endregion

                #region findGeneralInformationByPtcpntIds (generalInfo)
                VIMTfgenpidreturnclms generalInfo = null;
                if (!String.IsNullOrEmpty(ptcpntVetId) || (request.ptcpntId > 0))
                {
                    string startFindGeneralInfoByPtcpntId = timer.MarkStart("findGeneralInformationByPtcpntIds");

                    var findGeneralInformationByPtcpntIdsRequest = new VIMTfgenpidfindGeneralInformationByPtcpntIdsRequest
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
                        mcs_ptcpntvetid = String.IsNullOrEmpty(ptcpntVetId) ? request.ptcpntId.ToString() : ptcpntVetId,
                        mcs_ptcpntbeneid = ptcpntBeneId,

                    };
                    
                    var findGeneralInformationByPtcpntIdsResponse =
                        findGeneralInformationByPtcpntIdsRequest
                            .SendReceive<VIMTfgenpidfindGeneralInformationByPtcpntIdsResponse>(MessageProcessType.Local);
                    generalInfo = findGeneralInformationByPtcpntIdsResponse.VIMTfgenpidreturnclmsInfo;
                    progressString = "After VIMT EC Call - findGeneralInformationByPtcpntIds";

                    if (generalInfo == null)
                    {
                        response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                        {
                            ExceptionMessage = findGeneralInformationByPtcpntIdsResponse.ExceptionMessage,
                            ExceptionOccured = findGeneralInformationByPtcpntIdsResponse.ExceptionOccured
                        };
                        return response;
                    }

                    timer.MarkStop("findGeneralInformationByPtcpntIds");

                    ptcpntBeneId = generalInfo.mcs_ptcpntBeneID;
                    ptcpntVetId = generalInfo.mcs_ptcpntVetID;
                    ptpcntRecipId = generalInfo.mcs_ptcpntRecipID;
                }
                else
                {
                    // No pid...
                }
                #endregion
                                
                #region Set Entity Field Response Values
                

                #region udo_idproofid
                if (request.udo_idproofId == Guid.Empty)
                {
                    throw new InvalidPluginExecutionException("customIDProof not valid.  Please IDProof.");
                }
                else
                {
                    new_va_fnod_entity["udo_idproof"] = new EntityReference("udo_idproof", request.udo_idproofId);
                }
                #endregion

                #region va_veterancontactid
                if (request.udo_veteranId == Guid.Empty)
                {
                    throw new InvalidPluginExecutionException("customVeteran not valid.  Please use a valid veteran record.");
                }
                else
                {
                    new_va_fnod_entity["va_veterancontactid"] = new EntityReference("contact", request.udo_veteranId);
                }
                #endregion

                #region va_name [const]
                new_va_fnod_entity["va_name"] = "FNOD/MOD/PMC Request";
                #endregion

                #region udo_personid
                new_va_fnod_entity["udo_deceasedperson"] = new EntityReference("udo_person",request.udo_personId);
                #endregion

                #region va_filenumber [request]

                new_va_fnod_entity["va_filenumber"] = request.vetfileNumber;

                #endregion

                #region va_firstname, va_newpmcvetfirstname [corpRecord, birlsRecord] and va_birlsfirstname [birlsRecord]

                if (corpRecord!=null && !string.IsNullOrEmpty(corpRecord.mcs_firstName))
                {
                    new_va_fnod_entity["va_firstname"] = corpRecord.mcs_firstName;
                    new_va_fnod_entity["va_newpmcvetfirstname"] = corpRecord.mcs_firstName;
                }
                else if (birlsRecord!=null && !string.IsNullOrEmpty(birlsRecord.mcs_FIRST_NAME))
                {
                    new_va_fnod_entity["va_firstname"] = birlsRecord.mcs_FIRST_NAME;
                    new_va_fnod_entity["va_newpmcvetfirstname"] = birlsRecord.mcs_FIRST_NAME;
                }
                
                if (birlsRecord != null)
                {
                    new_va_fnod_entity["va_birlsfirstname"] = birlsRecord.mcs_FIRST_NAME;
                }
                #endregion

                #region newpmcvetmiddleinitial [coprRecord, birlsRecord]
                if (corpRecord!=null && !string.IsNullOrEmpty(corpRecord.mcs_middleName))
                {
                    new_va_fnod_entity["va_newpmcvetmiddleinitial"] = corpRecord.mcs_middleName;
                }
                else if (birlsRecord!=null && !string.IsNullOrEmpty(birlsRecord.mcs_MIDDLE_NAME))
                {
                    new_va_fnod_entity["va_newpmcvetmiddleinitial"] = birlsRecord.mcs_MIDDLE_NAME;
                }
                #endregion

                #region va_lastname, va_newpmcvetlastname [corpRecord, birlsRecord] and va_birlslastname [birlsRecord]

                if (corpRecord != null && !string.IsNullOrEmpty(corpRecord.mcs_lastName))
                {
                    new_va_fnod_entity["va_lastname"] = corpRecord.mcs_lastName;
                    new_va_fnod_entity["va_newpmcvetlastname"] = corpRecord.mcs_lastName;
                }
                else if (birlsRecord != null && !string.IsNullOrEmpty(birlsRecord.mcs_LAST_NAME))
                {
                    new_va_fnod_entity["va_lastname"] = birlsRecord.mcs_LAST_NAME;
                    new_va_fnod_entity["va_newpmcvetlastname"] = birlsRecord.mcs_LAST_NAME;
                }
                if (birlsRecord != null)
                {
                    new_va_fnod_entity["va_birlslastname"] = birlsRecord.mcs_LAST_NAME;
                }

                #endregion

                #region va_newpmcvetsuffix [corpRecord, birlsRecord]
                if (corpRecord!=null && !string.IsNullOrEmpty(corpRecord.mcs_suffixName))
                {
                    new_va_fnod_entity["va_newpmcvetsuffix"] = corpRecord.mcs_suffixName;
                }
                else if (birlsRecord!=null && !string.IsNullOrEmpty(birlsRecord.mcs_NAME_SUFFIX))
                {
                    new_va_fnod_entity["va_newpmcvetsuffix"] = birlsRecord.mcs_NAME_SUFFIX;
                }
                
                #endregion

                #region va_dateofbirth [corpRecord, birlsRecord]

                if (corpRecord != null || birlsRecord != null)
                {
                    string dateOfBirth = (corpRecord == null || String.IsNullOrEmpty(corpRecord.mcs_dateOfBirth)) ? birlsRecord.mcs_DATE_OF_BIRTH : corpRecord.mcs_dateOfBirth;
                    if (!string.IsNullOrEmpty(dateOfBirth))
                    {
                        new_va_fnod_entity["va_dateofbirthtext"] = dateOfBirth;  // If we need to store the date as text, shouldn't we also store the dateofdeath as text?
                        DateTime va_dateofbirth;
                        if (DateTime.TryParse(dateOfBirth, out va_dateofbirth))
                        {
                            new_va_fnod_entity["va_dateofbirth"] = va_dateofbirth.ToCRMDateTime();
                        }
                    }
                }
                #endregion

                #region va_dateofdeath [birlsRecord, generalinfo]

                
                string dod = (birlsRecord!=null) ? birlsRecord.mcs_DATE_OF_DEATH : string.Empty;
                if (String.IsNullOrEmpty(dod) && generalInfo != null) dod = generalInfo.mcs_vetDeathDate;
                if (!string.IsNullOrEmpty(dod))
                {
                    DateTime va_dateofdeath;
                    if (DateTime.TryParse(dod, out va_dateofdeath))
                    {
                        new_va_fnod_entity["va_dateofdeath"] = va_dateofdeath.ToCRMDateTime();
                    }
                }

                
                #endregion

                #region va_sex [generalInfo, birlsRecord]

                switch (generalInfo==null || string.IsNullOrEmpty(generalInfo.mcs_vetSex)
                    ? birlsRecord.mcs_SEX_CODE
                    : generalInfo.mcs_vetSex)
                {
                    case "M":
                        new_va_fnod_entity["va_sex"] = new OptionSetValue(953850000);
                        break;
                    case "F":
                        new_va_fnod_entity["va_sex"] = new OptionSetValue(953850001);
                        break;
                    default:
                        new_va_fnod_entity["va_sex"] = new OptionSetValue(953850002);
                        break;
                }

                #endregion

                #region va_soj [generalInfo, birlsRecord]

                if (generalInfo!=null && !string.IsNullOrEmpty(generalInfo.mcs_stationOfJurisdiction))
                {
                    new_va_fnod_entity["va_soj"] = generalInfo.mcs_stationOfJurisdiction;
                }
                else if (birlsRecord!=null && !string.IsNullOrEmpty(birlsRecord.mcs_CLAIM_FOLDER_LOCATION))
                {
                    new_va_fnod_entity["va_soj"] = birlsRecord.mcs_CLAIM_FOLDER_LOCATION;
                }

                #endregion

                #region va_poa [generalInfo]

                if (generalInfo!=null && !string.IsNullOrEmpty(generalInfo.mcs_powerOfAttorney))
                {
                    new_va_fnod_entity["va_poa"] = generalInfo.mcs_powerOfAttorney;
                }
                else
                {
                    var poa_exception = new UDOInitiateFNODException();
                    new_va_fnod_entity["va_poa"] = getPOA(OrgServiceProxy, request, out poa_exception);
                    if (poa_exception.ExceptionOccured)
                    {
                        //todo: add exception to response
                    }

                }



                #endregion

                #region va_birlsfileno [birlsRecord]

                if (birlsRecord!=null && !string.IsNullOrEmpty(birlsRecord.mcs_CLAIM_NUMBER))
                {
                    new_va_fnod_entity["va_birlsfileno"] = birlsRecord.mcs_CLAIM_NUMBER;
                }

                #endregion

                #region va_folderlocation [birlsRecord]

                if (birlsRecord != null && birlsRecord.VIMTbrlsFNFOLDERInfo != null)
                {
                    var claimFolder = birlsRecord.VIMTbrlsFNFOLDERInfo.Where(
                        folder =>
                            string.Equals(folder.mcs_FOLDER_TYPE, "CLAIM", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (claimFolder != null)
                    {
                        new_va_fnod_entity["va_folderlocation"] = claimFolder.mcs_FOLDER_CURRENT_LOCATION;
                    }
                }
                #endregion

                #region va_birlsmultiperiodservice [birlsRecord]

                if (birlsRecord != null)
                {
                    new_va_fnod_entity["va_birlsmultiperiodservice"] = (birlsRecord.VIMTbrlsFNSERVICEInfo.Length > 1);
                }
                #endregion

                #region va_birlbos and va_bilscharacterofservice [birlsRecord]

                // This gets the most recent service.
                // This should be able to replace the JScript reference that called va_findmilitaryrecordbyptcpntidresponse
                if (birlsRecord != null && birlsRecord.VIMTbrlsFNSERVICEInfo != null)
                {
                    var latestServiceInfo = birlsRecord.VIMTbrlsFNSERVICEInfo.OrderByDescending(x =>
                    {
                        DateTime d;
                        DateTime.TryParse(x.mcs_RELEASED_ACTIVE_DUTY_DATE, out d);
                        return d;
                    }).FirstOrDefault();

                    if (latestServiceInfo != null)
                    {
                        if (!string.IsNullOrEmpty(latestServiceInfo.mcs_BRANCH_OF_SERVICE))
                        {
                            new_va_fnod_entity["va_birlsbos"] = latestServiceInfo.mcs_BRANCH_OF_SERVICE;
                        }
                        if (!string.IsNullOrEmpty(latestServiceInfo.mcs_CHAR_OF_SVC_CODE))
                        {
                            new_va_fnod_entity["va_birlscharacterofservice"] = latestServiceInfo.mcs_CHAR_OF_SVC_CODE;
                        }
                    }
                }
                #endregion

                #region va_birlsservice1verified [birlsRecord]

                if (birlsRecord != null && !string.IsNullOrEmpty(birlsRecord.mcs_VERIFIED_SVC_DATA_IND))
                {
                    new_va_fnod_entity["va_birlsservice1verified"] = birlsRecord.mcs_VERIFIED_SVC_DATA_IND;
                }

                #endregion

                #region va_awardcode [generalInfoByFileNum]
                if (generalInfoByFileNum!=null && !string.IsNullOrEmpty(generalInfoByFileNum.mcs_awardTypeCode))
                {
                    new_va_fnod_entity["va_awardcode"] = generalInfoByFileNum.mcs_awardTypeCode;
                }
                else
                {
                    var awards = generalInfoByFileNum.VIMTfgenFNawardBenesclmsInfo;
                    if (awards != null && awards.Length > 1)
                    {
                        foreach (var award in awards)
                        {
                            if (!award.mcs_awardTypeCd.Trim().Equals("CPL", StringComparison.InvariantCultureIgnoreCase) ||
                                !award.mcs_payeeCd.Trim().Equals("00"))
                            {
                                continue;
                            }
                            new_va_fnod_entity["va_awardcode"] = award.mcs_awardTypeCd;
                            break;
                        }
                    }
                    else if (awards != null && awards.Length == 1)
                    {
                        new_va_fnod_entity["va_awardcode"] = awards[0].mcs_awardTypeCd;
                    }
                }
                #endregion

                #region va_awardstatus [generalInfoByFileNum]

                if (generalInfoByFileNum!=null && !string.IsNullOrEmpty(generalInfoByFileNum.mcs_payStatusTypeName))
                {
                    new_va_fnod_entity["va_awardatatus"] = generalInfoByFileNum.mcs_payStatusTypeName;
                }

                #endregion

                #region findDependents (va_listofdependents)
                string startFindDependents = timer.MarkStart("findDependents");
                var findDependentsRequest = new VIMTfedpfindDependentsRequest
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    mcs_filenumber = request.vetfileNumber
                };

                var findDependentsResponse =
                    findDependentsRequest.SendReceive<VIMTfedpfindDependentsResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call - findDependents";

                response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                {
                    ExceptionMessage = findDependentsResponse.ExceptionMessage,
                    ExceptionOccured = findDependentsResponse.ExceptionOccured
                };
                timer.MarkStop("findDependents");
                
                #endregion

                #region va_listofdependents [findDependents]
                
                if (findDependentsResponse.VIMTfedpreturnclmsInfo != null)
                {
                    var dependents = SecurityTools.ConvertToSecureString(string.Empty);
                    foreach (var dependent in findDependentsResponse.VIMTfedpreturnclmsInfo.VIMTfedppersonsclmsInfo)
                    {
                        dependents = SecurityTools.Append(dependents, string.Format("Name: {0} {1}     Relationship: {2}     DOB: {3}",
                            dependent.mcs_firstName, dependent.mcs_lastName,
                            dependent.mcs_relationship, dependent.mcs_dateOfBirth));
                        if (!string.IsNullOrEmpty(dependent.mcs_dateOfDeath))
                        {
                            dependents = SecurityTools.Append(dependents, string.Format("     DOD: {0}", dependent.mcs_dateOfDeath));
                        }
                        if (!string.IsNullOrEmpty(dependent.mcs_ssn))
                        {
                            dependents = SecurityTools.Append(dependents, string.Format("     SSN: {0}", dependent.mcs_ssn));
                        }
                        //// DEFECT 305719 asks to remove the Has Awards block.
                        //// If spouse, alive and has a SSN, display his/her Awards
                        //if (!string.IsNullOrEmpty(dependent.mcs_ssn) && string.IsNullOrEmpty(dependent.mcs_dateOfDeath) &&
                        //    string.Equals(dependent.mcs_relationship, "Spouse",
                        //        StringComparison.InvariantCultureIgnoreCase))
                        //{
                        //    var findSpouseRequest = new VIMTfgenFNfindGeneralInformationByFileNumberRequest
                        //    {
                        //        LogTiming = request.LogTiming,
                        //        LogSoap = request.LogSoap,
                        //        Debug = request.Debug,
                        //        RelatedParentEntityName = request.RelatedParentEntityName,
                        //        RelatedParentFieldName = request.RelatedParentFieldName,
                        //        RelatedParentId = request.RelatedParentId,
                        //        UserId = request.UserId,
                        //        OrganizationName = request.OrganizationName,
                        //        mcs_filenumber = dependent.mcs_ssn
                        //    };

                        //    var findSpouseResponse =
                        //        findSpouseRequest.SendReceive<VIMTfgenFNfindGeneralInformationByFileNumberResponse>(
                        //            MessageProcessType.Local);
                        //    var spouseInfo = findSpouseResponse.VIMTfgenFNreturnclmsInfo;
                        //    progressString = "After VIMT EC Call - findGeneralInformation (Spouse)";
                        //    response.UDOInitiateFNODExceptions = new UDOInitiateFNODException{
                        //        ExceptionMessage = findSpouseResponse.ExceptionMessage,
                        //        ExceptionOccured = findSpouseResponse.ExceptionOccured
                        //    };

                        //    if (spouseInfo != null)
                        //    {
                        //        var numberOfAwardBenes = 0;
                        //        if (!string.IsNullOrEmpty(spouseInfo.mcs_numberOfAwardBenes))
                        //        {
                        //            int.TryParse(spouseInfo.mcs_numberOfAwardBenes, out numberOfAwardBenes);
                        //        }
                        //        dependents += string.Format("     Has Awards: {0}",
                        //            (numberOfAwardBenes > 0) ? "Yes" : "No");
                        //    }
                        //}

                        dependents = SecurityTools.Append(dependents, "\r\n");
                    }
                    //dependents += "***************************************************************************************************\r\n";

                    new_va_fnod_entity["va_listofdependents"] = SecurityTools.ConvertToUnsecureString(dependents);
                }
                timer.MarkStop("Aggregating Dependents");
                
                #endregion

                #endregion

                #region Set PMC Caller Info
                var interactionFetch = "<fetch count='1'>" +
                    "<entity name='udo_interaction'>" +
                    "<attribute name='udo_firstname'/>" +
                    "<attribute name='udo_lastname'/>" +
                    "<attribute name='udo_relationship'/>" +
                    "<link-entity name='udo_idproof' to='udo_interactionid' from='udo_interaction'>" +
                    "<link-entity name='contact' to='udo_veteran' from='contactid' alias='contact'>" +
                    "<attribute name='ownerid'/>" +
                    "</link-entity>" +
                    "<attribute name='udo_idproofid'/>" +
                    "<filter type='and'>" +
                    "<condition attribute='udo_idproofid' operator='eq' value='" + request.udo_idproofId + "'/>" +
                    "</filter></link-entity></entity></fetch>";
                OrgServiceProxy.CallerId = request.UserId;
                var interactionResult = OrgServiceProxy.RetrieveMultiple(new FetchExpression(interactionFetch));

                if (interactionResult != null && interactionResult.Entities.Count() > 0)
                {
                    var interaction = interactionResult.Entities[0];
                    string caller = string.Empty;
                    if (interaction.Contains("udo_firstname")) caller = interaction["udo_firstname"] + " ";
                    if (interaction.Contains("udo_lastname")) caller += interaction["udo_lastname"];

                    new_va_fnod_entity["va_newpmcrecipname"] = caller;
                    if (interaction.Contains("udo_relationship"))
                    {
                        new_va_fnod_entity["va_newpmcreciprelationshiptovet"] = interaction.FormattedValues["udo_relationship"].ToString();
                    }

                    var aliasValue = interaction.GetAttributeValue<AliasedValue>("contact.ownerid");
                    if (aliasValue != null)
                    {
                        var owner = aliasValue.Value as EntityReference;
                        if (owner != null)
                        {
                            new_va_fnod_entity["ownerid"] = owner;
                        }
                    }
                }
                #endregion

                #region findPresidentialMemorialCertificate (pmcInfo)
                try
                {
                    progressString = "findPresidentialMemorialCertificate (pmcInfo)";
                    string startFindPresidentialMemorialCert = timer.MarkStart("findPresidentialMemorialCertificate");
                    // prefix = findPresidentialMemorialCertificateRequest();
                    var findPresidentialMemorialCertificateRequest = new VIMTfindPresidentialMemorialCertificateRequest
                    {
                        LogTiming = request.LogTiming,
                        LogSoap = request.LogSoap,
                        Debug = request.Debug,
                        RelatedParentEntityName = request.RelatedParentEntityName,
                        RelatedParentFieldName = request.RelatedParentFieldName,
                        RelatedParentId = request.RelatedParentId,
                        UserId = request.UserId,
                        OrganizationName = request.OrganizationName,
                        mcs_filenumber = request.vetfileNumber
                    };

                    if (request.LegacyServiceHeaderInfo != null)
                    {
                        findPresidentialMemorialCertificateRequest.LegacyServiceHeaderInfo = new VIMT.BenefitClaimService.Messages.HeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                    }

                    var findPresidentialMemorialCertificateResponse =
                        findPresidentialMemorialCertificateRequest
                            .SendReceive<VIMTfindPresidentialMemorialCertificateResponse>(
                                MessageProcessType.Local);
                    progressString = "After VIMT EC Call - findPresidentialMemorial";

                    VIMTFindPresidentialMemorialCertificateResponseReturnbclm pmcInfo = null;

                    if (findPresidentialMemorialCertificateResponse != null &&
                        findPresidentialMemorialCertificateResponse
                            .VIMTFindPresidentialMemorialCertificateResponseReturnbclmInfo != null)
                    {
                        pmcInfo =
                        findPresidentialMemorialCertificateResponse
                            .VIMTFindPresidentialMemorialCertificateResponseReturnbclmInfo;
                    }

                    response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                    {
                        ExceptionMessage = findPresidentialMemorialCertificateResponse.ExceptionMessage,
                        ExceptionOccured = findPresidentialMemorialCertificateResponse.ExceptionOccured
                    };
                    timer.MarkStop("findPresidentialMemorialCertificate");

                    #region pmc fields [pmcInfo]

                    if (pmcInfo != null)
                    {
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_veteranFirstName))
                        {
                            new_va_fnod_entity["va_existingpmcvetfirstname"] = pmcInfo.mcs_veteranFirstName;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_veteranLastName))
                        {
                            new_va_fnod_entity["va_existingpmcvetlastname"] = pmcInfo.mcs_veteranLastName;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_veteranMiddleInitial))
                        {
                            new_va_fnod_entity["va_existingpmcvetmiddleinitial"] = pmcInfo.mcs_veteranMiddleInitial;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_veteranSuffixName))
                        {
                            new_va_fnod_entity["va_existingpmcvetsuffix"] = pmcInfo.mcs_veteranSuffixName;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_station))
                        {
                            new_va_fnod_entity["va_existingpmcvetstation"] = pmcInfo.mcs_station;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_salutation))
                        {
                            new_va_fnod_entity["va_existingpmcrecipsalutation"] = pmcInfo.mcs_salutation;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_state))
                        {
                            new_va_fnod_entity["va_existingpmcrecipstate"] = pmcInfo.mcs_state;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_zipCode))
                        {
                            new_va_fnod_entity["va_existingpmcrecipzip"] = pmcInfo.mcs_zipCode;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_realtionshipToVeteran))
                        {
                            new_va_fnod_entity["va_existingpmcreciprelationshiptovet"] = pmcInfo.mcs_realtionshipToVeteran;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_city))
                        {
                            new_va_fnod_entity["va_existingpmcrecipcity"] = pmcInfo.mcs_city;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_title))
                        {
                            new_va_fnod_entity["va_existingpmcrecipname"] = pmcInfo.mcs_title;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_addressLine1))
                        {
                            new_va_fnod_entity["va_existingpmcrecipaddress1"] = pmcInfo.mcs_addressLine1;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_addressLine2))
                        {
                            new_va_fnod_entity["va_existingpmcrecipaddress2"] = pmcInfo.mcs_addressLine2;
                        }
                        // PMC Station
                        var pmc_station = (pmcInfo.mcs_station ?? string.Empty).Trim();

                        var vaFolder = string.Empty;

                        if (new_va_fnod_entity.Contains("va_folderlocation"))
                        {
                            vaFolder = (string)new_va_fnod_entity["va_folderlocation"];
                        }
                        
                        if (!String.IsNullOrEmpty(pmc_station) && !pmc_station.Equals("0"))
                        {
                            new_va_fnod_entity["va_existingpmcvetstation"] = pmc_station;
                        }
                        else if (String.IsNullOrEmpty(vaFolder))
                        {
                            new_va_fnod_entity["va_existingpmcvetstation"] = "10";
                        }
                        else
                        {
                            new_va_fnod_entity["va_existingpmcvetstation"] = vaFolder.Trim();
                        }
                        new_va_fnod_entity["va_newpmcvetstation"] = new_va_fnod_entity["va_existingpmcvetstation"];
                    }

                    #endregion
                }
                catch (Exception ExecutionException)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                    response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                    {
                        ExceptionMessage = "Failed to get CRMConnection",
                        ExceptionOccured = true
                    };
                    //TODO: UNCOMMENT RETURN - THIS BYPASSES ANY FAILS FOR PMC.
                    return response;
                }
                #endregion

                #region findAllPtcpntAddrsByPtcpntIdRequest (addresses)
                VIMTfallpidaddpidreturnMultipleResponse[] addresses = null;
                if (pid>0)
                {
                    string startFindAllPtcpntAddrs = timer.MarkStart("findAllPtcpntAddrsByPtcpntIdRequest");
                    var findAllPtcpntAddrsByPtcpntIdRequest = new VIMTfallpidaddpidfindAllPtcpntAddrsByPtcpntIdRequest
                    {
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
                        findAllPtcpntAddrsByPtcpntIdRequest.LegacyServiceHeaderInfo = new VIMT.AddressWebService.Messages.HeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                    }


                    var findAllPtcpntAddrsByPtcpntIdResponse =
                        findAllPtcpntAddrsByPtcpntIdRequest
                            .SendReceive<VIMTfallpidaddpidfindAllPtcpntAddrsByPtcpntIdResponse>(MessageProcessType.Local);
                    progressString = "After VIMT EC Call - findAllPtcpntAddrsByPtcpntIdRequest";

                    response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                    {
                        ExceptionMessage = findAllPtcpntAddrsByPtcpntIdResponse.ExceptionMessage,
                        ExceptionOccured = findAllPtcpntAddrsByPtcpntIdResponse.ExceptionOccured
                    };
                    
                    addresses = findAllPtcpntAddrsByPtcpntIdResponse.VIMTfallpidaddpidreturnInfo;

                    timer.MarkStop("findAllPtcpntAddrsByPtcpntIdRequest");

                }
                #endregion

                #region va_lastknownaddress [addresses]
                if (addresses != null && addresses.Length>0)
                {
                    bool updatePMC = !new_va_fnod_entity.Contains("va_newpmcrecipaddress1") ||
                        String.IsNullOrEmpty((string)new_va_fnod_entity["va_newpmcrecipaddress1"]);

                    var address = addresses.Where(a =>
                                  String.Equals(a.mcs_ptcpntAddrsTypeNm, "Mailing", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (address!=null) {
                        var addressBlock = address.mcs_addrsOneTxt;
                        new_va_fnod_entity["udo_lastaddress1"] = address.mcs_addrsOneTxt;
                        if (updatePMC)
                        {
                            new_va_fnod_entity["va_newpmcrecipaddress1"] = address.mcs_addrsOneTxt;
                        }
                        new_va_fnod_entity["udo_lastaddresstype"] = new OptionSetValue(953850000);
                        if (!string.IsNullOrWhiteSpace(address.mcs_addrsTwoTxt))
                        {
                            addressBlock += "\r\n" + address.mcs_addrsTwoTxt;
                            new_va_fnod_entity["udo_lastaddress2"] = address.mcs_addrsTwoTxt;
                            if (updatePMC)
                            {
                                new_va_fnod_entity["va_newpmcrecipaddress2"] = address.mcs_addrsTwoTxt;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(address.mcs_addrsThreeTxt))
                        {
                            new_va_fnod_entity["udo_lastaddress3"] = address.mcs_addrsThreeTxt;
                            addressBlock += "\r\n" + address.mcs_addrsThreeTxt;
                        }
                        if (!string.IsNullOrWhiteSpace(address.mcs_cityNm))
                        {
                            addressBlock += "\r\n" + address.mcs_cityNm;
                            new_va_fnod_entity["udo_lastcity"] = address.mcs_cityNm;
                            if (updatePMC)
                            {
                                new_va_fnod_entity["va_newpmcrecipcity"] = address.mcs_cityNm;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(address.mcs_postalCd))
                        {
                            addressBlock += ", " + address.mcs_postalCd;
                            new_va_fnod_entity["udo_laststate"] = address.mcs_postalCd;
                            if (updatePMC)
                            {
                                new_va_fnod_entity["va_newpmcrecipstate"] = address.mcs_postalCd;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(address.mcs_frgnPostalCd)) {
                            new_va_fnod_entity["udo_lastforeignpostalcode"]=address.mcs_frgnPostalCd;
                            new_va_fnod_entity["udo_lastaddresstype"] = new OptionSetValue(953850001);
                            if (updatePMC)
                            {
                                new_va_fnod_entity["va_newpmcrecipzip"] = new_va_fnod_entity["udo_lastforeignpostalcode"];
                            }
                        }
                        /*
            xrm.Page.getAttribute('va_spousestatelist').setValue(address.state);
            xrm.Page.getAttribute('va_spousezipcode').setValue(address.zip);
            xrm.Page.getAttribute('va_spousecountry').setValue(address.country);
            xrm.Page.getAttribute('va_spouseforeignpostalcode').setValue(address.forgeinPostalCode);
            xrm.Page.getAttribute('va_spouseoverseasmilitarypostalcode').setValue(address.mltyPostalType);
            xrm.Page.getAttribute('va_spouseoverseasmilitarypostofficetypecode').setValue(address.mltyPostOfficeType);
            xrm.Page.getAttribute('va_spouseaddresstype').setValue(address.spouseAddressType);
                         * */
                        if (!string.IsNullOrWhiteSpace(address.mcs_zipPrefixNbr))
                        {
                            addressBlock += " " + address.mcs_zipPrefixNbr;
                            new_va_fnod_entity["udo_lastzipcode"] = address.mcs_zipPrefixNbr;
                            if (updatePMC)
                            {
                                new_va_fnod_entity["va_newpmcrecipzip"] = address.mcs_zipPrefixNbr;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(address.mcs_cntryNm))
                        {
                            addressBlock += "\r\n" + address.mcs_cntryNm;
                            new_va_fnod_entity["udo_lastcountry"] = address.mcs_cntryNm;
                        }
                        if (!string.IsNullOrWhiteSpace(address.mcs_mltyPostOfficeTypeCd))
                        {
                            new_va_fnod_entity["udo_lastaddresstype"] = new OptionSetValue(953850002);
                            addressBlock += "\r\n" + address.mcs_mltyPostOfficeTypeCd;
                            
                            OptionSetValue mltyPostOfficeType = null;
                            switch (address.mcs_mltyPostalTypeCd.ToUpper())
                            {
                                case "APO": mltyPostOfficeType = new OptionSetValue(953850000); break;
                                case "DPO": mltyPostOfficeType = new OptionSetValue(953850001); break;
                                case "FPO": mltyPostOfficeType = new OptionSetValue(953850002); break;
                            }
                            new_va_fnod_entity["udo_lastoverseasmilitarypostofficetypeco"] = mltyPostOfficeType;
                            if (updatePMC) {
                                new_va_fnod_entity["va_newpmcrecipcity"] = address.mcs_mltyPostalTypeCd.ToUpper();
                            }
                            
                        }
                        if (!string.IsNullOrWhiteSpace(address.mcs_mltyPostalTypeCd))
                        {
                            addressBlock += " " + address.mcs_mltyPostalTypeCd;
                            OptionSetValue mltyPostalType = null;
                            switch (address.mcs_mltyPostalTypeCd.ToUpper())
                            {
                                case "AA": mltyPostalType=new OptionSetValue(953850000); break;
                                case "AE": mltyPostalType=new OptionSetValue(953850001); break;
                                case "AP": mltyPostalType=new OptionSetValue(953850002); break;
                            }
                            new_va_fnod_entity["udo_lastoverseasmilitarypostalcode"] = mltyPostalType;
                            if (updatePMC)
                            {
                                new_va_fnod_entity["va_newpmcrecipstate"] = address.mcs_mltyPostalTypeCd.ToUpper();
                            }
                        }

                        var lastAddressType = 0;
                        if (!String.IsNullOrWhiteSpace(address.mcs_mltyPostalTypeCd) || 
                            !String.IsNullOrWhiteSpace(address.mcs_mltyPostOfficeTypeCd))
                        {
                            // Military
                            lastAddressType = 953850002;
                        }
                        else
                        {
                            if (address.mcs_cntryNm != "USA" || String.IsNullOrWhiteSpace(address.mcs_cntryNm) ||
                                address.mcs_cntryNm == "US")
                            {
                                // Domestic
                                lastAddressType = 953850000;
                            }
                            else
                            {
                                // International
                                if (updatePMC) {
                                    new_va_fnod_entity["va_newpmcrecipstate"] =
                                    (new_va_fnod_entity["va_newpmcrecipstate"] + " " + address.mcs_cntryNm).Trim();
                                }
                                lastAddressType = 953850001;
                            }
                        }
                        if (lastAddressType != 0)
                        {
                            new_va_fnod_entity["udo_lastoverseasmilitarypostalcode"] = new OptionSetValue(lastAddressType);
                        }

                        new_va_fnod_entity["va_lastknownaddress"] = addressBlock;
                    }
                }
                #endregion
                timer.MarkStop("Processing Addresses");
                
                #region findRatingDatatRequest (ratingInfo)
                VIMTfnrtngdtdisabilityRatingRecord ratingInfo = null;
                string startFindRatingData = timer.MarkStart("findRatingDatatRequest");
                // prefix = fnrtngdtfindRatingDataRequest();
                var findRatingDataRequest = new VIMTfnrtngdtfindRatingDataRequest
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    mcs_filenumber = request.vetfileNumber
                };

                var findRatingDataResponse =
                    findRatingDataRequest.SendReceive<VIMTfnrtngdtfindRatingDataResponse>(
                        MessageProcessType.Local);
                progressString = "After VIMT EC Call";

                if (findRatingDataResponse != null && findRatingDataResponse.VIMTfnrtngdtInfo != null
                    && findRatingDataResponse.VIMTfnrtngdtInfo.VIMTfnrtngdtdisabilityRatingRecordInfo != null)
                {
                    ratingInfo = findRatingDataResponse.VIMTfnrtngdtInfo.VIMTfnrtngdtdisabilityRatingRecordInfo;
                }

                response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                {
                    ExceptionMessage = findRatingDataResponse.ExceptionMessage,
                    ExceptionOccured = findRatingDataResponse.ExceptionOccured
                };
                timer.MarkStop("findRatingDatatRequest");
                #endregion

                #region va_awardratings [ratingInfo]

                if (ratingInfo != null)
                {
                    if (!string.IsNullOrEmpty(ratingInfo.mcs_serviceConnectedCombinedDegree))
                    {
                        new_va_fnod_entity["va_awardratings"] = ratingInfo.mcs_serviceConnectedCombinedDegree;
                    }
                }

                #endregion
                
                #region findMilitaryRecordByPtcpntId [militaryTours]
                VIMTfmirecpicmilitaryPersonToursclmsMultipleResponse[] militaryTours = null;
                if (request.ptcpntId > 0)
                {
                    string startFindMilitaryRecord = timer.MarkStart("findMilitaryRecordByPtcpntId");
                    // prefix = fmirecpicfindMilitaryRecordByPtcpntIdRequest();
                    var findMilitaryRecordByPtcpntIdRequest = new VIMTfmirecpicfindMilitaryRecordByPtcpntIdRequest
                    {
                        LogTiming = request.LogTiming,
                        LogSoap = request.LogSoap,
                        Debug = request.Debug,
                        RelatedParentEntityName = request.RelatedParentEntityName,
                        RelatedParentFieldName = request.RelatedParentFieldName,
                        RelatedParentId = request.RelatedParentId,
                        UserId = request.UserId,
                        OrganizationName = request.OrganizationName,
                        mcs_ptcpntid = request.ptcpntId.ToString()
                    };

                    var findMilitaryRecordByPtcpntIdResponse =
                        findMilitaryRecordByPtcpntIdRequest.SendReceive<VIMTfmirecpicfindMilitaryRecordByPtcpntIdResponse>(
                            MessageProcessType.Local);
                    progressString = "After VIMT EC Call - findMilitaryRecordByPtcpntIdRequest";

                    militaryTours = findMilitaryRecordByPtcpntIdResponse.VIMTfmirecpicreturnclmsInfo == null ? null :
                        findMilitaryRecordByPtcpntIdResponse.VIMTfmirecpicreturnclmsInfo
                        .VIMTfmirecpicmilitaryPersonToursclmsInfo;

                    response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                    {
                        ExceptionMessage = findMilitaryRecordByPtcpntIdResponse.ExceptionMessage,
                        ExceptionOccured = findMilitaryRecordByPtcpntIdResponse.ExceptionOccured
                    };
                    timer.MarkStop("findMilitaryRecordByPtcpntId");
                }
                #endregion

                #region va_corpmilitarydischargetype, va_corpmilitaryseparationreason, va_corpverified [militaryTours]
                if (militaryTours != null)
                {
                    foreach (var tour in militaryTours)
                    {
                        if (!string.IsNullOrEmpty(tour.mcs_militarySeperationReasonTypeName))
                        {
                            new_va_fnod_entity["va_corpmilitaryseparationreason"] = tour.mcs_militarySeperationReasonTypeName;
                        }
                        if (
                            !string.IsNullOrEmpty(
                                tour.mcs_mpDischargeCharTypeName))
                        {
                            new_va_fnod_entity["va_corpmilitarydischargetype"] =
                                tour.mcs_mpDischargeCharTypeName;
                        }
                        if (!string.IsNullOrEmpty(tour.mcs_verifiedInd))
                        {
                            new_va_fnod_entity["va_corpverified"] =
                                tour.mcs_verifiedInd;
                        }
                    }
                }
                #endregion

                OrgServiceProxy.CallerId = Guid.Empty;
                var fnodid = OrgServiceProxy.Create(TruncateHelper.TruncateFields(new_va_fnod_entity, request.OrganizationName, request.UserId, request.LogTiming));
                response.newUDOInitiateFNODId = fnodid;

                #region Stop Timer
                LogHelper.LogTiming(request.OrganizationName, request.Debug, request.UserId,
                    request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method,
                    method,
                    Convert.ToDecimal(timer.ElapsedMilliseconds));

                //var o = request.OrganizationName;
                //var db = request.Debug;
                //var u = request.UserId;
                //var m = method;
                //var s = true;
                //timer.LogDurations(o, db, u, m, s);
                timer.LogDurations(request.OrganizationName, request.Debug, request.UserId, method, true);

                #endregion
                
                
                return response;
            }
            catch (Exception ExecutionException)
            {
                var method = MethodInfo.GetThisMethod().ToString(false);
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                timer.LogDurations(request.OrganizationName, request.Debug, request.UserId, method, true);
                response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                {
                    ExceptionMessage = "Failed to process FNOD Data",
                    ExceptionOccured = true
                };
                return response;
            }
        }

        private static string getPOA(OrganizationServiceProxy OrgServiceProxy, UDOInitiateFNODRequest request, out UDOInitiateFNODException response)
        {
            var progressString = "getPOAFIDData Start";
            response = new UDOInitiateFNODException { ExceptionMessage = String.Empty, ExceptionOccured = false };
            try
            {
                #region Get POA from person
                var fetch="<fetch count='1'><entity name='udo_person'>"+
                          "<attribute name='udo_poa'/>" + 
                          "<filter type='and'>" +
                          "<condition attribute='udo_personid' operator='eq' value='"+request.udo_personId.ToString()+"'/>"+
                          "</filter></entity></fetch>";

                OrgServiceProxy.CallerId = request.UserId;
                var people = OrgServiceProxy.RetrieveMultiple(new FetchExpression(fetch));
            
                if (people.Entities.Count==1 && people[0].Contains("udo_poa")) {
                
                    if (!String.IsNullOrEmpty((string)people[0]["udo_poa"])) return (string)people[0]["udo_poa"];
                }

                progressString = "After Fetch";
                #endregion
            
                #region findPOA
                 //if this doesn't contain anything, don't go asking for it!
                if (!string.IsNullOrEmpty(request.fileNumber))
                {

                    progressString = "Finding POA - findAllFiduciaryPoaRequest";
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.getPOAFIDData", progressString);

                    var findAllFiduciaryPoaRequest = new VIMTafidpoafindAllFiduciaryPoaRequest();
                    findAllFiduciaryPoaRequest.LogTiming = request.LogTiming;
                    findAllFiduciaryPoaRequest.LogSoap = request.LogSoap;
                    findAllFiduciaryPoaRequest.Debug = request.Debug;
                    findAllFiduciaryPoaRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                    findAllFiduciaryPoaRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                    findAllFiduciaryPoaRequest.RelatedParentId = request.RelatedParentId;
                    findAllFiduciaryPoaRequest.UserId = request.UserId;
                    findAllFiduciaryPoaRequest.OrganizationName = request.OrganizationName;

                    //non standard fields
                    findAllFiduciaryPoaRequest.mcs_filenumber = request.fileNumber;
                    if (request.LegacyServiceHeaderInfo != null)
                    {
                        findAllFiduciaryPoaRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                    }
                    var findAllFiduciaryPoaResponse = findAllFiduciaryPoaRequest.SendReceive<VIMTafidpoafindAllFiduciaryPoaResponse>(MessageProcessType.Local);

                    response.ExceptionMessage = findAllFiduciaryPoaResponse.ExceptionMessage;
                    response.ExceptionOccured = findAllFiduciaryPoaResponse.ExceptionOccured;
                    if (findAllFiduciaryPoaResponse.VIMTafidpoareturnclmsInfo != null)
                    {
                        if (findAllFiduciaryPoaResponse.VIMTafidpoareturnclmsInfo.VIMTafidpoacurrentPowerOfAttorneyclmsInfo != null)
                        {
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VIMTafidpoareturnclmsInfo.VIMTafidpoacurrentPowerOfAttorneyclmsInfo.mcs_personOrgName))
                            {
                                var poa = findAllFiduciaryPoaResponse.VIMTafidpoareturnclmsInfo.VIMTafidpoacurrentPowerOfAttorneyclmsInfo.mcs_personOrgName;

                                #region Update Person

                                var person = new Entity("udo_person");
                                person["udo_personid"] = request.udo_personId;
                                person["udo_poa"] = poa;
                                OrgServiceProxy.CallerId = request.UserId;
                                OrgServiceProxy.Update(TruncateHelper.TruncateFields(person, request.OrganizationName, request.UserId, request.LogTiming));

                                #endregion

                                return poa;
                            }

                        }
                    }

                }
                #endregion

                return string.Empty;
            }
            catch (Exception ExecutionException)
            {
                var method = MethodInfo.GetThisMethod().ToString(false);
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to get POAFIDDATA";
                response.ExceptionOccured = true;
                return string.Empty;
            }
        }
    }
}