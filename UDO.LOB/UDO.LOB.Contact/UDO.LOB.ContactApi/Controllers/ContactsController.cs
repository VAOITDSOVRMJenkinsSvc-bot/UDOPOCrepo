using Swashbuckle.Swagger.Annotations;
using System;
using System.Diagnostics;
using System.Net;
using System.Web.Http;
using UDO.LOB.Contact.Messages;
using UDO.LOB.Contact.Processors;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.ContactApi.Controllers
{
    public class ContactsController : ApiController
    {
        // POST api/contacts
        [SwaggerOperation("getContactRecords")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("getContactRecords")]
        public IMessageBase Post([FromBody]UDOgetContactRecordsRequest request)
        {
            UDOgetContactRecordsProcessor processor = new UDOgetContactRecordsProcessor();
            return processor.Execute(request);
        }

        [SwaggerOperation("createDependents")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createDependents")]
        // POST api/contacts
        public IMessageBase createDependents([FromBody]UDOcreateDependentsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createDependents ");
            try
            {
                UDOcreateDependentsProcessor processor = new UDOcreateDependentsProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "createDependents", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createDependents ");
            }
        }

        [SwaggerOperation("createRelationships")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createRelationships")]
        // POST api/contacts
        public IMessageBase createRelationships([FromBody]UDOcreateRelationshipsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createRelationships ");
            try
            {
                UDOcreateRelationshipsProcessor processor = new UDOcreateRelationshipsProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "createRelationships", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createRelationships ");
            }
        }

        [SwaggerOperation("createFlashes")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createFlashes")]
        // POST api/contacts/createFlashes
        public IMessageBase Post([FromBody]UDOcreateFlashesRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createFlashes ");
            try
            {
                UDOcreateFlashesProcessor processor = new UDOcreateFlashesProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "createFlashes", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createFlashes ");
            }
        }

        [SwaggerOperation("hasbenefits")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("hasbenefits")]
        // PUT api/contacts/hasbenefits
        public IMessageBase Post([FromBody]UDOupdateHasBenefitsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.hasbenefits ");
            try
            {
                UDOupdateHasBenefitsProcessor processor = new UDOupdateHasBenefitsProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "hasbenefits", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.hasbenefits ");
            }
        }

        [SwaggerOperation("createPastFiduciaries")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createPastFiduciaries")]
        // POST api/contacts/createPastFiduciaries
        public IMessageBase Post([FromBody]UDOcreatePastFiduciariesRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createPastFiduciaries ");
            try
            {
                UDOcreatePastFiduciariesProcessor processor = new UDOcreatePastFiduciariesProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "createPastFiduciaries", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createPastFiduciaries ");
            }

        }

        [SwaggerOperation("getAddress")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("getAddress")]
        // GET api/<controller>
        public IMessageBase Post([FromBody]UDOgetAddressRecordsRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOgetAddressRecordsProcessor();
                return processor.Execute((UDOgetAddressRecordsRequest)message);
            }
            catch (Exception ex)
            {
                UDOgetAddressRecordsRequest msg = (UDOgetAddressRecordsRequest)message;
                // LogHelper.LogError(msg.OrganizationName, msg.UserId, "UDOgetAddressRecordsMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOgetAddressRecordsMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("createAddress")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createAddress")]
        // POST api/<controller>
        public IMessageBase Post([FromBody]UDOcreateAddressRecordsRequest message)
        {
            try
            {
                //TODO:  LogMessageReceipt(message);
                var processor = new UDOcreateAddressRecordsProcessor();
                return processor.Execute((UDOcreateAddressRecordsRequest)message);
            }
            catch (Exception ex)
            {
                UDOcreateAddressRecordsRequest msg = (UDOcreateAddressRecordsRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_createAddressRecords", msg.UserId, "UDOcreateAddressRecordsMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOcreateAddressRecordsMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("validateAddress")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        // PUT api/address/validate
        [ActionName("validateAddress")]
        public IMessageBase ValidateAddress([FromBody]UDOValidateAddressRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOvalidateAddressRecordProcessor();
                return processor.Execute((UDOValidateAddressRequest)message);
            }
            catch (Exception ex)
            {
                UDOValidateAddressRequest msg = (UDOValidateAddressRequest)message;
                // LogHelper.LogError(msg.OrganizationName, msg.UserId, "UDOvalidateAddressMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOvalidateAddressMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [HttpGet]
        [HttpPost]
        [Route("api/ping")]
        [SwaggerOperation("ping")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult ping()
        {
            return Ok("pong");
        }
    }
}