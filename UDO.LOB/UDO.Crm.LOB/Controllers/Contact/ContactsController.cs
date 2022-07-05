
namespace UDO.Crm.LOB.Controllers.Contact
{
    using Swashbuckle.Swagger.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using VRM.Integration.Servicebus.Core;
    using VRM.Integration.UDO.Contact.Messages;
    using VRM.Integration.UDO.Contact.Processors;
    public class ContactsController : ApiController
    {
        // GET api/contacts
        public IMessageBase Get([FromBody]UDOgetContactRecordsRequest message)
        {
            try
            {
                // TODO:LogMessageReceipt(message);
                var processor = new UDOgetContactRecordsProcessor();
                return processor.Execute((UDOgetContactRecordsRequest)message);
            }
            catch (Exception ex)
            {
                UDOgetContactRecordsRequest msg = (UDOgetContactRecordsRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_getContactRecords", msg.UserId, "UDOgetContactRecordsMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOgetContactRecordsMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("createDependents")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createDependents")]
        // POST api/contacts
        public IMessageBase CreateDependents([FromBody]UDOcreateDependentsRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOcreateDependentsProcessor();
                return processor.Execute((UDOcreateDependentsRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreateDependentsRequest msg = (UDOcreateDependentsRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_createDependents", msg.UserId, "UDOcreateDependentsMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOcreateDependentsMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("createRelationships")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createRelationships")]
        // POST api/contacts
        public IMessageBase CreateRelationships([FromBody]UDOcreateDependentsRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOcreateDependentsProcessor();
                return processor.Execute((UDOcreateDependentsRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreateDependentsRequest msg = (UDOcreateDependentsRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_createDependents", msg.UserId, "UDOcreateDependentsMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOcreateDependentsMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("createFlashes")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createFlashes")]
        // POST api/contacts/createFlashes
        public IMessageBase CreateFlashes([FromBody]UDOcreateFlashesRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOcreateFlashesProcessor();
                return processor.Execute((UDOcreateFlashesRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreateFlashesRequest msg = (UDOcreateFlashesRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_createFlashes", msg.UserId, "UDOcreateFlashesMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOcreateFlashesMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("hasbenefits")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("hasbenefits")]
        // PUT api/contacts/hasbenefits
        public IMessageBase UpdateHasbenefits([FromBody]UDOupdateHasBenefitsRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOupdateHasBenefitsProcessor();
                return processor.Execute((UDOupdateHasBenefitsRequest)message);
            }
            catch (Exception ex)
            {
                UDOupdateHasBenefitsRequest msg = (UDOupdateHasBenefitsRequest)message;
                // LogHelper.LogError(msg.OrganizationName, msg.UserId, "UDOupdateHasBenefitsMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOupdateHasBenefitsMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("createPastFiduciaries")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createPastFiduciaries")]
        // POST api/contacts/hasbenefits
        public IMessageBase CreatePastFiduciaries([FromBody]UDOcreatePastFiduciariesRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOcreatePastFiduciariesProcessor();
                return processor.Execute((UDOcreatePastFiduciariesRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreatePastFiduciariesRequest msg = (UDOcreatePastFiduciariesRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_createPastFiduciaries", msg.UserId, "UDOcreatePastFiduciariesMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOcreatePastFiduciariesMessageHandler Error: {0}", ex.Message), ex);
            }
        }
    }
}