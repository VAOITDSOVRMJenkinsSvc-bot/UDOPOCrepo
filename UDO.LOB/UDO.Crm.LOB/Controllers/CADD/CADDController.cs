
namespace UDO.Crm.LOB.Controllers.CADD
{
    using Swashbuckle.Swagger.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using VRM.Integration.Servicebus.Core;
    using VRM.Integration.UDO.CADD.Messages;
    using VRM.Integration.UDO.CADD.Processors;

    public class CADDController : ApiController
    {
        [SwaggerOperation("FindBank")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        // GET api/CADD
        public IMessageBase Get([FromBody]UDOFindBankRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                UDOFindBankProcessor processor = new UDOFindBankProcessor();
                return processor.Execute(message);
            }
            catch (Exception ex)
            {
                UDOFindBankRequest msg = message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_updateCADD", msg.UserId, "UDOupdateCADDAddressMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOupdateCADDAddressMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        // POST api/CADD
        public IMessageBase Post([FromBody]UDOInitiateCADDRequest message)
        {
            try
            {
                UDOInitiateCADDProcessor processor = new UDOInitiateCADDProcessor();
                return processor.Execute(message);
            }
            catch (Exception ex)
            {
                // UDOInitiateCADDRequest msg = message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_InitiateCADD", msg.UserId, "UDOInitiateCADDMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOInitiateCADDMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        // PUT api/CADD
        public IMessageBase Put([FromBody]UDOupdateCADDAddressRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                UDOupdateCADDAddressProcessor processor = new UDOupdateCADDAddressProcessor();
                return processor.Execute(message);
            }
            catch (Exception ex)
            {
                UDOupdateCADDAddressRequest msg = message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_updateCADD", msg.UserId, "UDOupdateCADDAddressMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOupdateCADDAddressMessageHandler Error: {0}", ex.Message), ex);
            }
        }

    }
}