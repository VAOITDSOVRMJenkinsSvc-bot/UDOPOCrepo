
namespace UDO.Crm.LOB.Controllers.BIRLS
{
    using Swashbuckle.Swagger.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using VRM.Integration.Servicebus.Core;
    using VRM.Integration.UDO.BIRLS.Messages;
    using VRM.Integration.UDO.BIRLS.Processors;

    public class BIRLSController : ApiController
    {
        [SwaggerOperation("GetBIRLS")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        // GET api/<controller>
        public IMessageBase Get([FromBody] UDOgetBIRLSDataRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOgetBIRLSDataProcessor();
                return processor.Execute((UDOgetBIRLSDataRequest)message);
            }
            catch (Exception ex)
            {
                UDOgetBIRLSDataRequest msg = (UDOgetBIRLSDataRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_getBIRLSData", msg.UserId, "UDOgetBIRLSDataMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOgetBIRLSDataMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}