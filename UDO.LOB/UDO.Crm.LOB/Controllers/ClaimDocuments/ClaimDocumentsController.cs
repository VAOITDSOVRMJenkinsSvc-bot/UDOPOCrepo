namespace UDO.Crm.LOB.Controllers.ClaimDocuments
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using VRM.Integration.Servicebus.Core;
    using VRM.Integration.UDO.ClaimDocuments.Messages;
    using VRM.Integration.UDO.ClaimDocuments.Processors;

    public class ClaimDocumentsController : ApiController
    {
        // GET api/ClaimDocuments
        public IMessageBase Get([FromBody] getUDOClaimDocumentsRequest message)
        {
            try
            {
                // TODO: LogMessageReceipt(message);
                var processor = new getUDOClaimDocumentsProcessor();
                return processor.Execute((getUDOClaimDocumentsRequest)message);
            }
            catch (Exception ex)
            {
                getUDOClaimDocumentsRequest msg = (getUDOClaimDocumentsRequest)message;
                // LogHelper.LogError(msg.OrganizationName, msg.UserId, "getUDOClaimDocumentMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("getUDOClaimDocumentMessageHandler Error: {0}", ex.Message), ex);
            }
        }
    }
}