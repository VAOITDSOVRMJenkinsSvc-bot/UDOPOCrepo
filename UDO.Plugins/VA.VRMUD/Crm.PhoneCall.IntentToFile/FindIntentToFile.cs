using System;
using System.IO;
using ConsoleApplication1.IntentToFileServiceReference2;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Crm.WebServices
{
    public class FindIntentToFile
    {
        public intentToFileDTO[] FindIntentToFileByPtcpntId(BgsConfig bgsConfig, string participantId, out string request,  out string response)
        {
            try
            {
                string uri = bgsConfig.BepServiceUrl;

                string messageBody = string.Format("<q0:findIntentToFileByPtcpntId><ptcpntId>{0}</ptcpntId></q0:findIntentToFileByPtcpntId>",
                    participantId);

                string endpoint = "http://intenttofile.services.vetsnet.vba.va.gov/";
                Common webServiceInvocation = new Common();
                response = webServiceInvocation.InvokeWebService(bgsConfig, new Uri(uri), messageBody, endpoint);
                request = webServiceInvocation.SoapRequestBody;

                XDocument xDoc = XDocument.Parse(response);
                List<intentToFileDTO> intentToFileDtoList = new List<intentToFileDTO>();
                foreach (var envelope in xDoc.Elements())
                {
                    foreach (var body in envelope.Elements())
                    {
                        foreach (var findIntentToFileByPtcpntIdResponse in body.Elements())
                        {
                            foreach (var intentToFileDto in findIntentToFileByPtcpntIdResponse.Elements())
                            {
                                intentToFileDTO intentToFileDTOItem = new intentToFileDTO()
                                {
                                    ptcpntVetId = (intentToFileDto.Element("ptcpntVetId") != null) ? Convert.ToInt64(intentToFileDto.Element("ptcpntVetId").Value) : 0,
                                    exprtnDt = (intentToFileDto.Element("exprtnDt") != null) ? DateTime.Parse(intentToFileDto.Element("exprtnDt").Value) : DateTime.MinValue,
                                    intentToFileId = (intentToFileDto.Element("intentToFileId") != null) ? Convert.ToInt64(intentToFileDto.Element("intentToFileId").Value) : 0,
                                    itfStatusTypeCd = (intentToFileDto.Element("itfStatusTypeCd") != null) ? intentToFileDto.Element("itfStatusTypeCd").Value : string.Empty,
                                    itfTypeCd = (intentToFileDto.Element("itfTypeCd") != null) ? intentToFileDto.Element("itfTypeCd").Value : string.Empty,
                                    jrnDt = (intentToFileDto.Element("jrnDt") != null) ? DateTime.Parse(intentToFileDto.Element("jrnDt").Value) : DateTime.MinValue,
                                    jrnLctnId = (intentToFileDto.Element("jrnLctnId") != null) ? intentToFileDto.Element("jrnLctnId").Value : string.Empty,
                                    jrnExtnlApplcnNm = (intentToFileDto.Element("jrnExtnlApplcnNm") != null) ? intentToFileDto.Element("jrnExtnlApplcnNm").Value : string.Empty,
                                    jrnObjId = (intentToFileDto.Element("jrnObjId") != null) ? intentToFileDto.Element("jrnObjId").Value : string.Empty,
                                    jrnStatusTypeCd = (intentToFileDto.Element("jrnStatusTypeCd") != null) ? intentToFileDto.Element("jrnStatusTypeCd").Value : string.Empty,
                                    jrnUserId = (intentToFileDto.Element("jrnUserId") != null) ? intentToFileDto.Element("jrnUserId").Value : string.Empty,
                                    ptcpntClmantId = (intentToFileDto.Element("ptcpntClmantId") != null) ? Convert.ToInt64(intentToFileDto.Element("ptcpntClmantId").Value) : 0,
                                    rcvdDt = (intentToFileDto.Element("rcvdDt") != null) ? DateTime.Parse(intentToFileDto.Element("rcvdDt").Value) : DateTime.MinValue,
                                    createDt = (intentToFileDto.Element("createDt") != null) ? DateTime.Parse(intentToFileDto.Element("createDt").Value) : DateTime.MinValue,
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
                                };
                                intentToFileDtoList.Add(intentToFileDTOItem);
                            }
                        }
                    }
                }

                intentToFileDTO[] intentToFileDTOItems = intentToFileDtoList.ToArray();
                return intentToFileDTOItems;
            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message);
            }
        }
    }
}