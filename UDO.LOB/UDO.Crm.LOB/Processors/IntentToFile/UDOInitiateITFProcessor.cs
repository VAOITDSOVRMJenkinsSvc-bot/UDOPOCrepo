using Microsoft.Xrm.Sdk.Client;
using System;
using System.Security;
using VIMT.VeteranWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.IntentToFile.Messages;
using Logger = VRM.Integration.Servicebus.Core.Logger;

namespace VRM.Integration.UDO.IntentToFile.Processors
{
    class UDOInitiateITFProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOInitiateITFProcessor";
        private string LogBuffer { get; set; }
        SecureString claimantfirstname;
        SecureString claimantlastname;
        SecureString claimantmiddleinitial;
        SecureString claimantssn;
        string participantid;
        string claimantparticipantid;
        string filenumber;
        SecureString vetssn;
        SecureString vetfirstname;
        SecureString vetlastname;
        SecureString vetmiddleinitial;
        string vetdateofbirth;
        SecureString vetgender;
        string veteranaddressline1;
        string veteranaddressline2;
        string veteranaddressline3;
        string veterancity;
        string veteranzip;
        string veterancountry;
        string veteranstate;
        string veteranemail;
        string militarypostcode;
        string militarypostofficetypecode;
        string veteranphone;
        SecureString _ssn;
        SecureString _vetssn;
        SecureString _filenumber;

        public IMessageBase Execute(UDOInitiateITFRequest request)
        {
            //var request = message as createUDOLegacyPaymentDataRequest;
            UDOInitiateITFResponse response = new UDOInitiateITFResponse();
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            Logger.Instance.Info(string.Format("Message Id:{0}, Type={2}, Recieved diagnostics message: {1}",
                request.MessageId,
                request.MessageId,
                GetType().FullName));

            OrganizationServiceProxy OrgServiceProxy;

            #region connect to CRM

            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId,
                    "UDOfindZipCodeProcessor Processor, Connection Error", connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }

            #endregion

            progressString = "After Connection";

            try
            {

                var setClaimantInfo = true;
                _ssn = request.SSN.ToSecureString();
                _vetssn = request.vetSSN.ToSecureString();
                _filenumber = request.fileNumber.ToSecureString();


                //if we don't have a file number, we know we will have a PID, so go get filenumber and SSN from PID
                if (string.IsNullOrEmpty(_filenumber.ToUnsecureString()))
                {
                    #region findVeteranByPtcpntIdRequest

                    // prefix = vetPctfindVeteranByPtcpntIdRequest();
                    var findVeteranByPtcpntIdRequest = new VIMTvetPctfindVeteranByPtcpntIdRequest
                    {
                        LogTiming = request.LogTiming,
                        LogSoap = request.LogSoap,
                        Debug = request.Debug,
                        RelatedParentEntityName = request.RelatedParentEntityName,
                        RelatedParentFieldName = request.RelatedParentFieldName,
                        RelatedParentId = request.RelatedParentId,
                        UserId = request.UserId,
                        OrganizationName = request.OrganizationName,
                        mcs_ptcpntid = request.ptcpntId,
                        LegacyServiceHeaderInfo = new VIMT.VeteranWebService.Messages.HeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        }
                    };

                    var findVeteranByPtcpntIdResponse =
                        findVeteranByPtcpntIdRequest.SendReceive<VIMTvetPctfindVeteranByPtcpntIdResponse>(
                            MessageProcessType.Local);
                    progressString = "After findVeteranByPtcpntIdRequest EC Call";

                    if (findVeteranByPtcpntIdResponse != null)
                    {
                        if (findVeteranByPtcpntIdResponse.VIMTvetPctreturnInfo != null)
                        {
                            if (findVeteranByPtcpntIdResponse.VIMTvetPctreturnInfo.VIMTvetPctvetCorpRecordInfo != null)
                            {
                                var corpRecord =
                                    findVeteranByPtcpntIdResponse.VIMTvetPctreturnInfo.VIMTvetPctvetCorpRecordInfo;
                                if (!string.IsNullOrEmpty(corpRecord.mcs_fileNumber))
                                {
                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId,
                                        "UDOInitiateITF  Processor",
                                        "request file number was:" + _filenumber.ToUnsecureString());
                                    _filenumber = corpRecord.mcs_fileNumber.ToSecureString();
                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId,
                                        "UDOInitiateITF Processor",
                                        "request file number NOW is:" + _filenumber.ToUnsecureString());
                                }
                                if (String.IsNullOrEmpty(_filenumber.ToUnsecureString()) &&
                                    !string.IsNullOrEmpty(corpRecord.mcs_ssn))
                                {
                                    // Should this be setting the filenumber to the SSN?  Because the filenumber is the only thing used below.
                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId,
                                        "UDOInitiateITF Processor", "request file number was:" + _ssn.ToUnsecureString());
                                    _filenumber = corpRecord.mcs_ssn.ToSecureString();
                                    _ssn = corpRecord.mcs_ssn.ToSecureString();
                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId,
                                        "UDOInitiateITF Processor",
                                        "request file number NOW is:" + _ssn.ToUnsecureString());
                                }

                                claimantfirstname =
                                    populatefield("va_claimantfirstname", corpRecord.mcs_firstName).ToSecureString();
                                claimantlastname =
                                    populatefield("va_claimantlastname", corpRecord.mcs_lastName).ToSecureString();
                                claimantmiddleinitial =
                                    populatefield("va_claimantmiddleinitial", corpRecord.mcs_middleName)
                                        .ToSecureString();
                                if (!_ssn.IsNullOrEmpty())
                                {
                                    claimantssn = _ssn.Surround("<va_claimantssn>", "</va_claimantssn>");
                                    //claimantssn = ("<va_claimantssn>" + _ssn.ToUnsecureString() + "</va_claimantssn>").ToSecureString();
                                }
                                else if (!String.IsNullOrEmpty(corpRecord.mcs_ssn))
                                {
                                    claimantssn =
                                        String.Format("<va_claimantssn>{0}</va_claimantssn>", corpRecord.mcs_ssn)
                                            .ToSecureString();
                                    //claimantssn = MCSHelper.ConvertToSecureString("<va_claimantssn>" + corpRecord.mcs_ssn + "</va_claimantssn>");
                                }
                                setClaimantInfo = false;
                            }
                        }
                    }

                    #endregion
                }

                #region - process corporate data

                var findCorporateRecordByFileNumberRequest = new VIMTcrpFNfindCorporateRecordByFileNumberRequest();
                findCorporateRecordByFileNumberRequest.LogTiming = request.LogTiming;
                findCorporateRecordByFileNumberRequest.LogSoap = request.LogSoap;
                findCorporateRecordByFileNumberRequest.Debug = request.Debug;
                findCorporateRecordByFileNumberRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findCorporateRecordByFileNumberRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findCorporateRecordByFileNumberRequest.RelatedParentId = request.RelatedParentId;
                findCorporateRecordByFileNumberRequest.UserId = request.UserId;
                findCorporateRecordByFileNumberRequest.OrganizationName = request.OrganizationName;

                if (request.LegacyServiceHeaderInfo != null)
                {
                    findCorporateRecordByFileNumberRequest.LegacyServiceHeaderInfo = new VIMT.VeteranWebService.Messages
                        .HeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                }
                //non standard fields

                var findCorporateRecordByFileNumberResponse = GetCorporateRecord(request, response,
                    findCorporateRecordByFileNumberRequest, ref progressString);

                if (findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo != null)
                {

                    participantid = "<va_participantid>" + request.vetptcpntId.ToString() + "</va_participantid>";
                    claimantparticipantid = "<va_claimantparticipantid>" + request.ptcpntId.ToString() +
                                            "</va_claimantparticipantid>";
                    filenumber = "<va_filenumber>" + request.vetfileNumber + "</va_filenumber>";
                    vetssn = _vetssn.Surround("<va_veteranssn>", "</va_veteranssn>");
                    //vetssn = MCSHelper.ConvertToSecureString("<va_veteranssn>" + MCSHelper.ConvertToUnsecureString(_vetssn) + "</va_veteranssn>");

                    var shrinq2Person = findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo;

                    vetfirstname = populatefield("va_veteranfirstname", request.vetFirstName).ToSecureString();
                    vetlastname = populatefield("va_veteranlastname", request.vetLastName).ToSecureString();
                    vetmiddleinitial =
                        populatefield("va_veteranmiddleinitial", request.vetMiddleInitial).ToSecureString();

                    if (_ssn.Equals(_vetssn))
                        //if (MCSHelper.ConvertToUnsecureString(_ssn) == MCSHelper.ConvertToUnsecureString(_vetssn))
                    {
                        vetdateofbirth = populatefield("va_veterandateofbirth", shrinq2Person.mcs_dateOfBirth);
                    }
                    vetgender = populatefield("va_veterangender", request.vetGender).ToSecureString();

                    if (setClaimantInfo)
                    {
                        claimantssn = request.SSN.ToSecureString().Surround("<va_claimantssn>", "</va_claimantssn>");
                        //MCSHelper.ConvertToSecureString("<va_claimantssn>" + request.SSN + "</va_claimantssn>");
                        claimantfirstname =
                            populatefield("va_claimantfirstname", shrinq2Person.mcs_firstName).ToSecureString();
                        claimantlastname =
                            populatefield("va_claimantlastname", shrinq2Person.mcs_lastName).ToSecureString();
                        claimantmiddleinitial =
                            populatefield("va_claimantmiddleinitial", shrinq2Person.mcs_middleName).ToSecureString();
                    }



                    veteranaddressline1 = populatefield("va_veteranaddressline1", shrinq2Person.mcs_addressLine1);
                    veteranaddressline2 = populatefield("va_veteranaddressline2", shrinq2Person.mcs_addressLine2);
                    veteranaddressline3 = populatefield("va_veteranunitnumber", shrinq2Person.mcs_addressLine3);
                    veterancity = populatefield("va_veterancity", shrinq2Person.mcs_city);
                    veteranzip = populatefield("va_veteranzip", shrinq2Person.mcs_zipCode);
                    veterancountry = populatefield("va_veterancountry", shrinq2Person.mcs_country);
                    veteranstate = populatefield("va_veteranstate", shrinq2Person.mcs_state);
                    veteranemail = populatefield("va_veteranemail", shrinq2Person.mcs_emailAddress);
                    militarypostcode = populatefield("va_militarypostalcodevalue",
                        shrinq2Person.mcs_militaryPostalTypeCode);
                    militarypostofficetypecode = populatefield("va_militarypostofficetypecodevalue",
                        shrinq2Person.mcs_militaryPostOfficeTypeCode);
                    veteranphone = "<va_veteranphone>" + "(" +
                                   findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_areaNumberOne + ") " +
                                   findCorporateRecordByFileNumberResponse.VIMTcrpFNreturnInfo.mcs_phoneNumberOne +
                                   "</va_veteranphone>";
                }

                #endregion

                SecureString parameters = ("<record>" +
                                           participantid +
                                           claimantparticipantid +
                                           filenumber).ToSecureString()
                    .Append(vetssn)
                    .Append(vetfirstname)
                    .Append(vetlastname)
                    .Append(vetmiddleinitial)
                    .Append(claimantssn)
                    .Append(claimantfirstname)
                    .Append(claimantlastname)
                    .Append(claimantmiddleinitial)
                    .Append(veteranaddressline1 +
                            veteranaddressline2 +
                            veteranaddressline3 +
                            veterancity +
                            veteranzip +
                            veterancountry +
                            veteranstate +
                            veteranemail +
                            militarypostcode +
                            militarypostofficetypecode +
                            veteranphone +
                            "</record>");

                response.parameter = parameters.ToUnsecureString();

                return response;
            }


            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId,
                    request.RelatedParentEntityName, request.RelatedParentFieldName,
                    "UDOInitiateITFProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to process Initiate ITF";
                response.ExceptionOccured = true;
                return response;
            }
        }

        private VIMTcrpFNfindCorporateRecordByFileNumberResponse GetCorporateRecord(UDOInitiateITFRequest request,
            UDOInitiateITFResponse response,
            VIMTcrpFNfindCorporateRecordByFileNumberRequest findCorporateRecordByFileNumberRequest,
            ref string progressString, int level = 0)
        {
            if (level == 4) return null;
            if (level == 0)
            {
                if (_filenumber.IsNullOrEmpty())
                {
                    level = 1;
                    if (_ssn.IsNullOrEmpty())
                    {
                        level = 2;
                        if (String.IsNullOrEmpty(request.vetfileNumber))
                        {
                            level = 3;
                        }
                    }
                }
            }


            switch (level)
            {
                case 0:
                    findCorporateRecordByFileNumberRequest.mcs_filenumber = _filenumber.ToUnsecureString();
                    break;
                case 1:
                    findCorporateRecordByFileNumberRequest.mcs_filenumber = _ssn.ToUnsecureString();
                    break;
                case 2:
                    findCorporateRecordByFileNumberRequest.mcs_filenumber = request.vetfileNumber;
                    break;
                case 3:
                    findCorporateRecordByFileNumberRequest.mcs_filenumber = _vetssn.ToUnsecureString();
                    break;
            }

            var corpResponse =
                findCorporateRecordByFileNumberRequest.SendReceive<VIMTcrpFNfindCorporateRecordByFileNumberResponse>(
                    MessageProcessType.Local);
            progressString = "After VIMT EC Call";
            if (level > 0) progressString += String.Format(" Level {0}", level);

            response.ExceptionMessage = corpResponse.ExceptionMessage;
            response.ExceptionOccured = corpResponse.ExceptionOccured;

            if (corpResponse.VIMTcrpFNreturnInfo == null)
                return GetCorporateRecord(request, response, findCorporateRecordByFileNumberRequest, ref progressString,
                    level++);
            return corpResponse;
        }

        private string populatefield(string fieldName, string SourceField)
        {
            if (!string.IsNullOrEmpty(SourceField))
            {
                return "<" + fieldName + ">" + SourceField + "</" + fieldName + ">";
            }
            return null;
        }
    }
}