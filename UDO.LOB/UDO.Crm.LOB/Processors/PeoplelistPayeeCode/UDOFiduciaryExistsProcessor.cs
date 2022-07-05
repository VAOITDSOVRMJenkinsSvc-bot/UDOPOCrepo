using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using VIMT.ClaimantWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.PeoplelistPayeeeCode.Messages;

namespace VRM.Integration.UDO.PeoplelistPayeeeCode.Processors
{
    internal class UDOFiduciaryExistsProcessor
    {
        public StringBuilder sr_log { get; set; }

        public IMessageBase Execute(UDOFiduciaryExistsRequest request)
        {
            sr_log = new StringBuilder("UDO Fiduciary Exists Log:");
            var response = new UDOFiduciaryExistsResponse();

            #region Check for request

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            #endregion

            try
            {
                if (request.LogSoap)
                {
                    var method = MethodInfo.GetThisMethod().ToString(false);
                    var requestMessage = "Request: \r\n\r\n" + request.SerializeToString();
                    LogHelper.LogInfo(request.OrganizationName, request.LogSoap, request.UserId, method, requestMessage);
                }

                var fidexist = false;

                var findFiduciaryRequest = new VIMTfidfindFiduciaryRequest()
                {
                    MessageId = request.MessageId,
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName
                };

                var searchfor = request.fileNumber;
                //if (String.IsNullOrEmpty(searchfor))
                //{
                //    searchfor = request.udo_ssn;
                //}

                if (String.IsNullOrEmpty(searchfor))
                {
                    response.ExceptionOccured = true;
                    response.ExceptionMessage = "No search identifier provided in the request";
                    return response;
                }

                findFiduciaryRequest.mcs_filenumber = searchfor;

                if (request.LegacyServiceHeaderInfo != null)
                {
                    findFiduciaryRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                var findFiduciaryResponse = findFiduciaryRequest.SendReceive<VIMTfidfindFiduciaryResponse>(MessageProcessType.Local);


                if (findFiduciaryResponse.VIMTfidreturnclmsInfo != null)
                {
                    var fidInfo = findFiduciaryResponse.VIMTfidreturnclmsInfo;

                    DateTime fidEndDate = DateTime.Today.AddDays(5);

                    if (!String.IsNullOrEmpty(fidInfo.mcs_endDate))
                       fidEndDate = DateTime.ParseExact(fidInfo.mcs_endDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                    var currentDate = DateTime.Today;

                    if (!String.IsNullOrEmpty(fidInfo.mcs_personOrgName) && fidEndDate > currentDate) fidexist = true;
                }

                response.FiduciaryExists = fidexist;

                return response;
            }
            catch (Exception ex)
            {
                var method = String.Format("{0}:Find Fiduciary", MethodInfo.GetThisMethod().ToString(true));
                var message = String.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                if (ex.InnerException != null)
                {
                    message += String.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                }
                message += String.Format("Call Stack:\r\n{0}", ex.StackTrace);
                sr_log.Insert(0, message);
                message = sr_log.ToString();
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId,
                    request.RelatedParentEntityName, request.RelatedParentFieldName, method, message);
                response.ExceptionMessage = "Failed to Find Fiduciary info";
                response.ExceptionOccured = true;
                return response;
            }
        }
    }
}
