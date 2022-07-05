using System;
using System.IO;
using ConsoleApplication1.IntentToFileServiceReference2;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Crm.WebServices
{
    public static class FindIntentToFile
    {
        public static intentToFileDTO[] FindIntentToFileByPtcpntId(BgsConfig bgsConfig, string participantId, out string response)
        {
            try
            {
                string uri = bgsConfig.BepServiceUrl;

                string messageBody = string.Format("<q0:findIntentToFileByPtcpntId><ptcpntId>{0}</ptcpntId></q0:findIntentToFileByPtcpntId>",
                    participantId);

                string endpoint = "http://intenttofile.services.vetsnet.vba.va.gov/";
                Common webServiceInvocation = new Common();
                response = webServiceInvocation.InvokeWebService(bgsConfig, new Uri(uri), messageBody, endpoint);

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
                                    rcvdDt = (intentToFileDto.Element("rcvdDt") != null) ? DateTime.Parse(intentToFileDto.Element("rcvdDt").Value) : DateTime.MinValue,
                                    signtrInd = (intentToFileDto.Element("signtrInd") != null) ? intentToFileDto.Element("signtrInd").Value : string.Empty,
                                    submtrApplcnTypeCd = (intentToFileDto.Element("submtrApplcnTypeCd") != null) ? intentToFileDto.Element("submtrApplcnTypeCd").Value : string.Empty,
                                    jrnExtnlApplcnNm = (intentToFileDto.Element("jrnExtnlApplcnNm") != null) ? intentToFileDto.Element("jrnExtnlApplcnNm").Value : string.Empty,
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