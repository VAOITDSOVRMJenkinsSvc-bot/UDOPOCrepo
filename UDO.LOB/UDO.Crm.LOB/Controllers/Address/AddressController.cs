﻿
namespace UDO.Crm.LOB.Controllers.Address
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
    // using VRM.UDO.Integration.Contact.Processors;

    public class AddressController : ApiController
    {
        [SwaggerOperation("getAddress")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        // GET api/<controller>
        public IMessageBase Get([FromBody]UDOgetAddressRecordsRequest message)
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
        [ActionName("validate")]
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
    }
}