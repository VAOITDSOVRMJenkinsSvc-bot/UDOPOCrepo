

namespace UDO.Crm.LOB.Controllers.Claims
{
    using Swashbuckle.Swagger.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using VRM.Integration.Servicebus.Core;
    using VRM.Integration.UDO.Claims.Messages;
    using VRM.Integration.UDO.Claims.Processors;

    public class ClaimsController : ApiController
    {
        [SwaggerOperation("createClaims")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createClaims")]
        // POST api/claims
        public IMessageBase Post([FromBody]UDOcreateUDOClaimsRequest message)
        {
            try
            {
                // TODO: LogMessageReceipt(message);
                var processor = new createUDOClaimsProcessor();
                return processor.Execute((UDOcreateUDOClaimsRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreateUDOClaimsRequest msg = (UDOcreateUDOClaimsRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_getUDOClaimData", msg.UserId, "getUDOClaimDataMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("Error occured in ClaimsController.createUDOClaimsProcessor: {0}", ex.Message), ex);
            }
        }

        // PUT api/claims
        public IMessageBase Put([FromBody]UDOUpdateUDOClaimsRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UpdateUDOClaimsProcessor();
                return processor.Execute((UDOUpdateUDOClaimsRequest)message);
            }
            catch (Exception ex)
            {
                UDOUpdateUDOClaimsRequest msg = (UDOUpdateUDOClaimsRequest)message;
                // LogHelper.LogError(msg.OrganizationName, msg.UserId, "UpdateUDOClaimsMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UpdateUDOClaimsMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("createUDOStatus")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createUDOStatus")]
        public IMessageBase CreateUDOStatus([FromBody]UDOcreateUDOStatusRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOcreateUDOStatusProcessor();
                return processor.Execute((UDOcreateUDOStatusRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreateUDOStatusRequest msg = (UDOcreateUDOStatusRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_createUDOStatus", msg.UserId, "UDOcreateUDOStatusMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOcreateUDOStatusMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("createTrackedItems")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createTrackedItems")]
        public IMessageBase CreateTrackedItems([FromBody]UDOcreateUDOTrackedItemsRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOcreateUDOTrackedItemsProcessor();
                return processor.Execute((UDOcreateUDOTrackedItemsRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreateUDOTrackedItemsRequest msg = (UDOcreateUDOTrackedItemsRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_createUDOTrackedItems", msg.UserId, "UDOcreateUDOTrackedItemsMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOcreateUDOTrackedItemsMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("createSuspense")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createSuspense")]
        public IMessageBase CreateSuspense([FromBody]UDOcreateUDOSuspenseRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOcreateUDOSuspenseProcessor();
                return processor.Execute((UDOcreateUDOSuspenseRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreateUDOSuspenseRequest msg = (UDOcreateUDOSuspenseRequest)message;
                //TODO: LogHelper.LogError(msg.OrganizationName, "mcs_createUDOSuspense", msg.UserId, "UDOcreateUDOSuspenseMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOcreateUDOSuspenseMessageHandler Error: {0}", ex.Message), ex);
            }
        }


        [SwaggerOperation("createLifecycles")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createLifecycles")]
        public IMessageBase CreateLifecycles([FromBody]UDOcreateUDOLifecyclesRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOcreateUDOLifecyclesProcessor();
                return processor.Execute((UDOcreateUDOLifecyclesRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreateUDOLifecyclesRequest msg = (UDOcreateUDOLifecyclesRequest)message;
                //TODO: LogHelper.LogError(msg.OrganizationName, "mcs_createUDOLifecycles", msg.UserId, "UDOcreateUDOLifecyclesMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOcreateUDOLifecyclesMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("createEvidence")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createEvidence")]
        public IMessageBase CreateEvidence([FromBody]UDOcreateUDOEvidenceRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOcreateUDOEvidenceProcessor();
                return processor.Execute((UDOcreateUDOEvidenceRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreateUDOEvidenceRequest msg = (UDOcreateUDOEvidenceRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_createUDOEvidence", msg.UserId, "UDOcreateUDOEvidenceMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOcreateUDOEvidenceMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("createContentions")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createContentions")]
        public IMessageBase CreateContentions([FromBody]UDOcreateUdoContentionsRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOcreateUdoContentionsProcessor();
                return processor.Execute((UDOcreateUdoContentionsRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreateUdoContentionsRequest msg = (UDOcreateUdoContentionsRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_createUdoContentions", msg.UserId, "UDOcreateUdoContentionsMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOcreateUdoContentionsMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("createUDOClaimsOrch")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createUDOClaimsOrch")]
        public IMessageBase CreateUDOClaimsOrch([FromBody]UDOcreateUDOClaimsRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new createUDOClaimsOrchProcessor();
                return processor.Execute((UDOcreateUDOClaimsRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreateUDOClaimsRequest msg = (UDOcreateUDOClaimsRequest)message;
                // LogHelper.LogError(msg.OrganizationName, msg.UserId, "createUDOClaimsSyncOrchMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("createUDOClaimsSyncOrchMessageHandler Error: {0}", ex.Message), ex);
            }
        }
    }
}