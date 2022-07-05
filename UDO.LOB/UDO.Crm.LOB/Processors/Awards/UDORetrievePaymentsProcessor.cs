
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
// using VIMT.AddressWebService.Messages;
using VIMT.ClaimantWebService.Messages;
using VIMT.VeteranWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Awards.Messages;
using VRM.Integration.UDO.Common;
using Logger = VRM.Integration.Servicebus.Core.Logger;

namespace VRM.Integration.UDO.Awards.Processors
{
    class UDORetrievePaymentsProcessor
    {

        OrganizationServiceProxy OrgServiceProxy;

        public IMessageBase Execute(UDORetrievePaymentsRequest request)
        {
            var progressString = "Top of Processor";

            if (request == null)
            {
                return null;
            }

            #region connect to CRM
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDORetrievePaymentsProcessor Processor, Connection Error", connectException.Message); 
                return null;
            }
            #endregion
            try
            {
            progressString = "After Connection";
            QueryExpression personQuery = new QueryExpression
            {
                EntityName = "udo_payment",
                ColumnSet = new ColumnSet("udo_paymentid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
 
                        new ConditionExpression
                        {
                            AttributeName = "udo_payeecodeid",
                            Operator = ConditionOperator.Equal,
                            Values = {request.payeeCodeId}
                        }
                    }
                }
            };
            OrgServiceProxy.CallerId = request.UserId;
            EntityCollection retrieved = OrgServiceProxy.RetrieveMultiple(personQuery);

            if (retrieved.Entities.Count == 0)
            {
                LogHelper.LogDebug(request.OrganizationName, request.Debug,request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDORetrievePaymentsProcessor","Did not get payments for payeecode :" + request.payeeCodeId.ToString());
            }
            return null;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDORetrievePaymentsProcessor Processor, Progess:" + progressString, 
                String.Format("{0}\r\n\r\nInner Exception: {1} \r\n\r\nStackTrace: {2}",ExecutionException.Message, 
                ExecutionException.InnerException!=null ? ExecutionException.InnerException.Message : "none",
                ExecutionException.StackTrace));
                //LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDORetrievePaymentsProcessor Processor, Progess:" + progressString, ExecutionException);
                return null;
            }
        }
    }

}

