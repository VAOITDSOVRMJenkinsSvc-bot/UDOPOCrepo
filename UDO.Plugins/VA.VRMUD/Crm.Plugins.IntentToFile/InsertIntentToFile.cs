using System;
using System.IO;
using ConsoleApplication1.IntentToFileServiceReference2;
using System.Xml.Linq;
using Crm.Plugins.IntentToFile;
using System.Security;
using System.Runtime.InteropServices;

namespace Crm.WebServices
{
    public class InsertIntentToFile
    {
        public intentToFileDTO InsertItf(BgsConfig bgsConfig, Claimant claimant, out string request, out string response)
        {
            try
            {
                string uri = bgsConfig.BepServiceUrl;
                string veteranParticipantId = !String.IsNullOrEmpty(claimant.VeteranParticipantId) ? string.Format("<ptcpntVetId>{0}</ptcpntVetId>", claimant.VeteranParticipantId) : string.Empty;
                string claimantFirstName = !String.IsNullOrEmpty(claimant.ClaimantFirstName) ? string.Format("<clmantFirstNm>{0}</clmantFirstNm>", claimant.ClaimantFirstName) : string.Empty;
                string claimantLastName = !String.IsNullOrEmpty(claimant.ClaimantLastName) ? string.Format("<clmantLastNm>{0}</clmantLastNm>", claimant.ClaimantLastName) : string.Empty;
                string claimantMiddleName = !String.IsNullOrEmpty(claimant.ClaimantMiddleInitial) ? string.Format("<clmantMiddleNm>{0}</clmantMiddleNm>", claimant.ClaimantMiddleInitial) : string.Empty;
                string claimantSsn = claimant.ClaimantSsn != null ? string.Format("<clmantSsn>{0}</clmantSsn>", ConvertToUnsecureString(claimant.ClaimantSsn)) : string.Empty;
                string veteranFirstName = !String.IsNullOrEmpty(claimant.VeteranFirstName) ? string.Format("<vetFirstNm>{0}</vetFirstNm>", claimant.VeteranFirstName) : string.Empty;
                string veteranLastName = !String.IsNullOrEmpty(claimant.VeteranLastName) ? string.Format("<vetLastNm>{0}</vetLastNm>", claimant.VeteranLastName) : string.Empty;
                string veteranMiddleName = !String.IsNullOrEmpty(claimant.VeteranMiddleInitial) ? string.Format("<vetMiddleNm>{0}</vetMiddleNm>", claimant.VeteranMiddleInitial) : string.Empty;
                string veteranSsn = claimant.VeteranSsn != null ? string.Format("<vetSsnNbr>{0}</vetSsnNbr>", ConvertToUnsecureString(claimant.VeteranSsn)) : string.Empty;
                string veteranFileNumber = !String.IsNullOrEmpty(claimant.VeteranFileNumber) ? string.Format("<vetFileNbr>{0}</vetFileNbr>", claimant.VeteranFileNumber) : string.Empty;
                string veteranGender = !String.IsNullOrEmpty(claimant.VeteranGender) ? string.Format("<genderCd>{0}</genderCd>", claimant.VeteranGender) : string.Empty;
                string veteranDateOfbirth = claimant.VeteranBirthDate != null ? string.Format("<vetBrthdyDt>{0}</vetBrthdyDt>", ConvertToUtc(claimant.VeteranBirthDate.GetValueOrDefault())) : string.Empty;//.ToString("yyyy'-'MM'-'dd'Z'")) : string.Empty;
                string phoneAreaCode = !String.IsNullOrEmpty(claimant.PhoneAreaCode) ? string.Format("<clmantPhoneAreaNbr>{0}</clmantPhoneAreaNbr>", claimant.PhoneAreaCode) : string.Empty;
                string phoneNumber = !String.IsNullOrEmpty(claimant.Phone) ? string.Format("<clmantPhoneNbr>{0}</clmantPhoneNbr>", claimant.Phone) : string.Empty;
                string email = !String.IsNullOrEmpty(claimant.Email) ? string.Format("<clmantEmailAddrsTxt>{0}</clmantEmailAddrsTxt>", claimant.Email) : string.Empty;
                string addressline1 = !String.IsNullOrEmpty(claimant.AddressLine1) ? string.Format("<clmantAddrsOneTxt>{0}</clmantAddrsOneTxt>", claimant.AddressLine1) : string.Empty;
                string addressLine2 = !String.IsNullOrEmpty(claimant.AddressLine2) ? string.Format("<clmantAddrsTwoTxt>{0}</clmantAddrsTwoTxt>", claimant.AddressLine2) : string.Empty;
                string addressLine3 = !String.IsNullOrEmpty(claimant.AddressLine3) ? string.Format("<clmantAddrsUnitNbr>{0}</clmantAddrsUnitNbr>", claimant.AddressLine3) : string.Empty;
                string city = !String.IsNullOrEmpty(claimant.Country) ? string.Format("<clmantCityNm>{0}</clmantCityNm>", claimant.City) : string.Empty;
                string state = !String.IsNullOrEmpty(claimant.State) ? string.Format("<clmantStateCd>{0}</clmantStateCd>", claimant.State) : string.Empty;
                string zip = !String.IsNullOrEmpty(claimant.Zip) ? string.Format("<clmantZipCd>{0}</clmantZipCd>", claimant.Zip) : string.Empty;
                string country = !String.IsNullOrEmpty(claimant.Country) ? string.Format("<clmantCntryNm>{0}</clmantCntryNm>", claimant.Country) : string.Empty;
                string userId = !String.IsNullOrEmpty(claimant.UserId) ? string.Format("<jrnUserId>{0}</jrnUserId>", claimant.UserId) : string.Empty;
                string stationLocation = !String.IsNullOrEmpty(claimant.StationLocation) ? string.Format("<jrnLctId>{0}</jrnLctId>", claimant.StationLocation) : string.Empty;
                //string createDate = string.Format("<createDt>{0}</createDt>", ConvertToUtc(DateTime.Now));

                string messageBody = string.Format("<q0:insertIntentToFile>" +
                    "<intentToFileDTO>" +
                      "<itfTypeCd>{0}</itfTypeCd>" +
                      "<ptcpntClmantId>{1}</ptcpntClmantId>" +
                      veteranParticipantId +
                      claimantFirstName +
                      claimantLastName +
                      claimantMiddleName +
                      claimantSsn +
                      veteranFirstName +
                      veteranLastName +
                      veteranMiddleName +
                      veteranSsn +
                      veteranFileNumber +
                      veteranGender +
                      veteranDateOfbirth +
                      phoneAreaCode +
                      phoneNumber +
                      email +
                      addressline1 +
                      addressLine2 +
                      addressLine3 +
                      city +
                      state +
                      zip +
                      country +
                      userId +
                      stationLocation +
                    //createDate + 
                      "<rcvdDt>{2}</rcvdDt>" + //2015-01-05T17:42:12.058Z
                      "<signtrInd>Y</signtrInd>" +
                      "<submtrApplcnTypeCd>CRM</submtrApplcnTypeCd>" +
                    "</intentToFileDTO>" +
                    "</q0:insertIntentToFile>", claimant.CompensationType, claimant.ClaimantParticipantId, ConvertToUtc(DateTime.Now));//DateTime.Now.ToString("yyyy'-'MM'-'dd'Z'"));

                string endpoint = "http://intenttofile.services.vetsnet.vba.va.gov/";
                Common webServiceInvocation = new Common();
                response = webServiceInvocation.InvokeWebService(bgsConfig, new Uri(uri), messageBody, endpoint);
                request = webServiceInvocation.SoapRequestBody;
                claimantSsn = string.Empty;
                veteranSsn = string.Empty;

                XDocument xDoc = XDocument.Parse(response);
                intentToFileDTO intentToFileDTOItem = null;
                foreach (var envelope in xDoc.Elements())
                {
                    foreach (var body in envelope.Elements())
                    {
                        foreach (var findIntentToFileByPtcpntIdResponse in body.Elements())
                        {
                            foreach (var intentToFileDto in findIntentToFileByPtcpntIdResponse.Elements())
                            {
                                intentToFileDTOItem = new intentToFileDTO()
                                {
                                    ptcpntVetId = (intentToFileDto.Element("ptcpntVetId") != null) ? Convert.ToInt64(intentToFileDto.Element("ptcpntVetId").Value) : 0,
                                    exprtnDt = (intentToFileDto.Element("exprtnDt") != null) ? DateTime.Parse(intentToFileDto.Element("exprtnDt").Value) : DateTime.MinValue,
                                    intentToFileId = (intentToFileDto.Element("intentToFileId") != null) ? Convert.ToInt64(intentToFileDto.Element("intentToFileId").Value) : 0,
                                    itfStatusTypeCd = (intentToFileDto.Element("itfStatusTypeCd") != null) ? intentToFileDto.Element("itfStatusTypeCd").Value : string.Empty,
                                    itfTypeCd = (intentToFileDto.Element("itfTypeCd") != null) ? intentToFileDto.Element("itfTypeCd").Value : string.Empty,
                                    jrnDt = (intentToFileDto.Element("jrnDt") != null) ? DateTime.Parse(intentToFileDto.Element("jrnDt").Value) : DateTime.MinValue,
                                    jrnLctnId = (intentToFileDto.Element("jrnLctnId") != null) ? intentToFileDto.Element("jrnLctnId").Value : string.Empty,
                                    jrnObjId = (intentToFileDto.Element("jrnObjId") != null) ? intentToFileDto.Element("jrnObjId").Value : string.Empty,
                                    jrnStatusTypeCd = (intentToFileDto.Element("jrnStatusTypeCd") != null) ? intentToFileDto.Element("jrnStatusTypeCd").Value : string.Empty,
                                    jrnUserId = (intentToFileDto.Element("jrnUserId") != null) ? intentToFileDto.Element("jrnUserId").Value : string.Empty,
                                    ptcpntClmantId = (intentToFileDto.Element("ptcpntClmantId") != null) ? Convert.ToInt64(intentToFileDto.Element("ptcpntClmantId").Value) : 0,
                                    signtrInd = (intentToFileDto.Element("signtrInd") != null) ? intentToFileDto.Element("signtrInd").Value : string.Empty,
                                    submtrApplcnTypeCd = (intentToFileDto.Element("submtrApplcnTypeCd") != null) ? intentToFileDto.Element("submtrApplcnTypeCd").Value : string.Empty,
                                    clmantFirstNm = (intentToFileDto.Element("clmantFirstNm") != null) ? intentToFileDto.Element("clmantFirstNm").Value : string.Empty,
                                    clmantLastNm = (intentToFileDto.Element("clmantLastNm") != null) ? intentToFileDto.Element("clmantLastNm").Value : string.Empty,
                                    clmantMiddleNm = (intentToFileDto.Element("clmantMiddleNm") != null) ? intentToFileDto.Element("clmantMiddleNm").Value : string.Empty,
                                    clmantSsn = (intentToFileDto.Element("clmantSsn") != null) ? intentToFileDto.Element("clmantSsn").Value : string.Empty,
                                    vetFirstNm = (intentToFileDto.Element("vetFirstNm") != null) ? intentToFileDto.Element("vetFirstNm").Value : string.Empty,
                                    vetLastNm = (intentToFileDto.Element("vetLastNm") != null) ? intentToFileDto.Element("vetLastNm").Value : string.Empty,
                                    vetMiddleNm = (intentToFileDto.Element("vetMiddleNm") != null) ? intentToFileDto.Element("vetMiddleNm").Value : string.Empty,
                                    vetSsnNbr = (intentToFileDto.Element("vetSsnNbr") != null) ? intentToFileDto.Element("vetSsnNbr").Value : string.Empty,
                                    vetFileNbr = (intentToFileDto.Element("vetFileNbr") != null) ? intentToFileDto.Element("vetFileNbr").Value : string.Empty,
                                    genderCd = (intentToFileDto.Element("genderCd") != null) ? intentToFileDto.Element("genderCd").Value : string.Empty,
                                    vetBrthdyDt = (intentToFileDto.Element("vetBrthdyDt") != null) ? DateTime.Parse(intentToFileDto.Element("vetBrthdyDt").Value) : DateTime.MinValue,
                                    clmantEmailAddrsTxt = (intentToFileDto.Element("clmantEmailAddrsTxt") != null) ? intentToFileDto.Element("clmantEmailAddrsTxt").Value : string.Empty,
                                    clmantAddrsOneTxt = (intentToFileDto.Element("clmantAddrsOneTxt") != null) ? intentToFileDto.Element("clmantAddrsOneTxt").Value : string.Empty,
                                    clmantAddrsTwoTxt = (intentToFileDto.Element("clmantAddrsTwoTxt") != null) ? intentToFileDto.Element("clmantAddrsTwoTxt").Value : string.Empty,
                                    clmantAddrsUnitNbr = (intentToFileDto.Element("clmantAddrsUnitNbr") != null) ? intentToFileDto.Element("clmantAddrsUnitNbr").Value : string.Empty,
                                    clmantCityNm = (intentToFileDto.Element("clmantCityNm") != null) ? intentToFileDto.Element("clmantCityNm").Value : string.Empty,
                                    clmantZipCd = (intentToFileDto.Element("clmantZipCd") != null) ? intentToFileDto.Element("clmantZipCd").Value : string.Empty,
                                    clmantCntryNm = (intentToFileDto.Element("clmantCntryNm") != null) ? intentToFileDto.Element("clmantCntryNm").Value : string.Empty,
                                    clmantStateCd = (intentToFileDto.Element("clmantStateCd") != null) ? intentToFileDto.Element("clmantStateCd").Value : string.Empty,
                                    clmantPhoneAreaNbr = (intentToFileDto.Element("clmantPhoneAreaNbr") != null) ? intentToFileDto.Element("clmantPhoneAreaNbr").Value : string.Empty,
                                    clmantPhoneNbr = (intentToFileDto.Element("clmantPhoneNbr") != null) ? intentToFileDto.Element("clmantPhoneNbr").Value : string.Empty,
                                    createDt = (intentToFileDto.Element("createDt") != null) ? DateTime.Parse(intentToFileDto.Element("createDt").Value) : DateTime.MinValue,
                                };
                            }
                        }
                    }
                }

                return intentToFileDTOItem;
            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message);
            }
        }

        public static SecureString ConvertToSecureString(string sensitiveString)
        {
            var secureString = new SecureString();
            if (sensitiveString.Length > 0)
            {
                foreach (var s in sensitiveString.ToCharArray())
                {
                    secureString.AppendChar(s);
                }
            }
            return secureString;
        }

        public static string ConvertToUnsecureString(SecureString secureString)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        private string ConvertToUtc(DateTime date)
        {
            // DateTime.SpecifyKind(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 0, 0), DateTimeKind.Utc).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            string dateOutput = DateTime.SpecifyKind(new DateTime(date.Year, date.Month, date.Day, 11, 0, 0), DateTimeKind.Utc).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            return dateOutput;
        }
    }
}