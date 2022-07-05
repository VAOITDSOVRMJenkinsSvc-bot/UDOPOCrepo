// using CRM007.CRM.SDK.Core;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using System;
using System.Collections.Generic;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.CADD.Messages;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using Logger = VRM.Integration.Servicebus.Core.Logger;
using VIMT.PaymentInformationService.Messages;
using VIMT.VeteranWebService.Messages;
using VIMT.ClaimantWebService.Messages;
using VIMT.AppealService.Messages;
using System.Linq;
using VRM.Integration.UDO.Common;

namespace VRM.Integration.UDO.CADD.Processors
{
    class UDOFindBankProcessor
    {
        public IMessageBase Execute(UDOFindBankRequest request)
        {
            //var request = message as InitiateCADDRequest;
            UDOFindBankResponse response = new UDOFindBankResponse();
            //set multiple message exception response
            response.UDOFindBankExceptions = new UDOFindBankExceptions();

            var progressString = "Top of Process or";

            if (request == null)
            {
                response.UDOFindBankExceptions.AppealExceptionMessage = "Called with no message";
                response.UDOFindBankExceptions.AppealExceptionOccured = true;

                response.UDOFindBankExceptions.ClaimantExceptionMessage = "Called with no message";
                response.UDOFindBankExceptions.ClaimantExceptionOccured = true;

                response.UDOFindBankExceptions.PaymentExceptionMessage = "Called with no message";
                response.UDOFindBankExceptions.PaymentExceptionOccured = true;

                response.UDOFindBankExceptions.VeteranExceptionMessage = "Called with no message";
                response.UDOFindBankExceptions.VeteranExceptionOccured = true;
                return response;
            }

            OrganizationServiceProxy OrgServiceProxy;

            #region connect to CRM
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOFindBankProcessor Processor, Connection Error", connectException.Message);
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                #region findBankNameByRoutingNumber
                if (!string.IsNullOrEmpty(request.RoutingNumber)  && ValidABARoutingNumber(request.RoutingNumber))
                {
                    var findBankNameByRoutingNumberRequest = new VIMT.DdeftWebService.Messages.VIMTbyRoutingTransitionNumberfindBankNameByRoutngTrnsitNbrRequest
                    {
                        LogTiming = request.LogTiming,
                        LogSoap = request.LogSoap,
                        Debug = request.Debug,
                        RelatedParentEntityName = request.RelatedParentEntityName,
                        RelatedParentFieldName = request.RelatedParentFieldName,
                        RelatedParentId = request.RelatedParentId,
                        UserId = request.UserId,
                        OrganizationName = request.OrganizationName,
                        mcs_routngtrnsitnbr = request.RoutingNumber,
                        LegacyServiceHeaderInfo = new VIMT.DdeftWebService.Messages.HeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        }
                    };

                    // TODO(TN): Comment to remediate
                    var findBankNameByRoutingNumberResponse = new VIMT.DdeftWebService.Messages.VIMTbyRoutingTransitionNumberfindBankNameByRoutngTrnsitNbrResponse();
                    // var findBankNameByRoutingNumberResponse = findBankNameByRoutingNumberRequest.SendReceive<VIMT.DdeftWebService.Messages.VIMTbyRoutingTransitionNumberfindBankNameByRoutngTrnsitNbrResponse>(MessageProcessType.Local);
                    progressString = "After findBankNameByRoutingNumber EC Call";

                    response.UDOFindBankExceptions.DdeftExceptionMessage = findBankNameByRoutingNumberResponse.ExceptionMessage;
                    response.UDOFindBankExceptions.DdeftExceptionOccured = findBankNameByRoutingNumberResponse.ExceptionOccured;
                    if (findBankNameByRoutingNumberResponse.VIMTbyRoutingTransitionNumberInfo != null)
                    {
                        var BankName = findBankNameByRoutingNumberResponse.VIMTbyRoutingTransitionNumberInfo;

                        if (!string.IsNullOrEmpty(BankName.mcs_bankName))
                        {

                            response.bankName = BankName.mcs_bankName;
                        }
                    }
                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOFindBankProcessor Processor, Progess:" + progressString, "No routing number");
                }
                #endregion
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOFindBankProcessor Processor, Progess:" + progressString, ExecutionException);
                return response;
            }
        }

        private bool ValidABARoutingNumber(string routingnumber)
        {
            int[] d = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            if (routingnumber.Length != 9) return false;

            int num = 0;
            if (!Int32.TryParse(routingnumber, out num)) return false;

            var i = 8; // max position
            while (num > 0)
            {
                d[i] = num % 10; // get digit
                num = num / 10; // remove digit
                i--; //decrease position;
            }

            long checksum = 3 * (d[0] + d[3] + d[6]);
            checksum += 7 * (d[1] + d[4] + d[7]);
            checksum += d[2] + d[5] + d[8];

            return (checksum % 10) == 0;
        }

        private string FormatTelephone(string telephoneNumber)
        {
            var Phone = telephoneNumber;
            var ext = "";
            var result = "";

            if (0 != Phone.IndexOf('+'))
            {
                if (1 < Phone.LastIndexOf('x'))
                {
                    ext = Phone.Substring(Phone.LastIndexOf('x'));
                    Phone = Phone.Substring(0, Phone.LastIndexOf('x'));
                }
            }
            //Phone = Phone.Replace(/[^\d]/gi, "");
            result = Phone;
            if (7 == Phone.Length)
            {
                result = Phone.Substring(0, 3) + "-" + Phone.Substring(3);
            }
            if (10 == Phone.Length)
            {
                result = "(" + Phone.Substring(0, 3) + ") " + Phone.Substring(3, 3) + "-" + Phone.Substring(6);
            }
            if (0 < ext.Length)
            {
                result = result + " " + ext;
            }
            return result;

        }
        private void PopulateField(Entity thisNewEntity, string fieldName, string fieldValue)
        {
            if (!string.IsNullOrEmpty(fieldValue))
            {
                thisNewEntity[fieldName] = fieldValue;
            }

        }
        private void PopulateFieldfromEntity(Entity thisNewEntity, string fieldName, Entity sourceEntity, string fieldValue)
        {
            if (sourceEntity.Attributes.Contains(fieldValue))
            {
                thisNewEntity[fieldName] = sourceEntity[fieldValue];
            }

        }

        private void PopulateDateField(Entity thisNewEntity, string fieldName, string fieldValue)
        {
            if (!string.IsNullOrEmpty(fieldValue))
            {
                DateTime newDateTime;
                if (DateTime.TryParse(fieldValue, out newDateTime))
                {
                    thisNewEntity[fieldName] = newDateTime;
                }
            }
        }

    }
}
