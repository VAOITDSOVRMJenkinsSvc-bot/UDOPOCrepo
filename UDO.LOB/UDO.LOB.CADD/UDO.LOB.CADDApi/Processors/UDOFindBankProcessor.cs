
namespace UDO.LOB.CADD.Processors
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.WebServiceClient;
    using System;
    using UDO.LOB.CADD.Messages;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions;
    using UDO.LOB.Extensions.Logging;
    using VEIS.Core.Messages;
    using VEIS.Messages.DdeftWebService;

    class UDOFindBankProcessor
    {
        private IOrganizationService OrgServiceProxy;
        private bool _debug { get; set; }

        private const string method = "UDOFindBankProcessor";

        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOFindBankRequest request)
        {
            UDOFindBankResponse response = new UDOFindBankResponse { MessageId = request?.MessageId };
            //set multiple message exception response
            response.UDOFindBankExceptions = new UDOFindBankExceptions();

            var progressString = $">> Entered {method} ";

            if (request == null)
            {
                response.UDOFindBankExceptions.AppealExceptionMessage = "Called with no message";
                response.UDOFindBankExceptions.AppealExceptionOccurred = true;

                response.UDOFindBankExceptions.ClaimantExceptionMessage = "Called with no message";
                response.UDOFindBankExceptions.ClaimantExceptionOccurred = true;

                response.UDOFindBankExceptions.PaymentExceptionMessage = "Called with no message";
                response.UDOFindBankExceptions.PaymentExceptionOccurred = true;

                response.UDOFindBankExceptions.VeteranExceptionMessage = "Called with no message";
                response.UDOFindBankExceptions.VeteranExceptionOccurred = true;
                return response;
            }

            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }

            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion

            progressString += " >> CRM Connection Successful";

            try
            {
                #region findBankNameByRoutingNumber
                if (!string.IsNullOrEmpty(request.RoutingNumber)  && ValidABARoutingNumber(request.RoutingNumber))
                {
                    var findBankNameByRoutingNumberRequest = new VEISbyRoutingTransitionNumberfindBankNameByRoutngTrnsitNbrRequest
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
                        LegacyServiceHeaderInfo = new LegacyHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        }
                    };

                    // REM: Invoke VEIS Endpoint
                    // var findBankNameByRoutingNumberResponse = findBankNameByRoutingNumberRequest.SendReceive<VIMT.DdeftWebService.Messages.VIMTbyRoutingTransitionNumberfindBankNameByRoutngTrnsitNbrResponse>(MessageProcessType.Local);
                    var findBankNameByRoutingNumberResponse = WebApiUtility.SendReceive<VEISbyRoutingTransitionNumberfindBankNameByRoutngTrnsitNbrResponse>(findBankNameByRoutingNumberRequest, WebApiType.VEIS);
                    progressString+= " >> After findBankNameByRoutingNumber EC Call";

                    if (findBankNameByRoutingNumberRequest.LogSoap || findBankNameByRoutingNumberResponse.ExceptionOccurred)
                    {
                        if (findBankNameByRoutingNumberResponse.SerializedSOAPRequest != null || findBankNameByRoutingNumberResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = findBankNameByRoutingNumberResponse.SerializedSOAPRequest + findBankNameByRoutingNumberResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISbyRoutingTransitionNumberfindBankNameByRoutngTrnsitNbrRequest Request/Response {requestResponse}", true);
                        }
                    }

                    response.UDOFindBankExceptions.DdeftExceptionMessage = findBankNameByRoutingNumberResponse.ExceptionMessage;
                    response.UDOFindBankExceptions.DdeftExceptionOccurred = findBankNameByRoutingNumberResponse.ExceptionOccurred;
                    // Replaced: VIMTbyRoutingTransitionNumberInfo = VEISbyRoutingTransitionNumberreturnInfo
                    if (findBankNameByRoutingNumberResponse.VEISbyRoutingTransitionNumberreturnInfo != null)
                    {
                        var BankName = findBankNameByRoutingNumberResponse.VEISbyRoutingTransitionNumberreturnInfo;

                        // Replaced? VEIS missing attribute mcs_bankName for VEISbyRoutingTransitionNumberreturnInfo 
                        if (!string.IsNullOrEmpty(BankName.mcs_bankName))
                        {

                            response.bankName = BankName.mcs_bankName;
                        }
                    }
                }
                else
                {
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId,  method, $"{progressString} >> No routing number", request.Debug);
                }
                #endregion
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, 
                    method, ExecutionException);
                response.ExceptionMessage = ExecutionException.Message;
                response.ExceptionOccurred = true;
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
