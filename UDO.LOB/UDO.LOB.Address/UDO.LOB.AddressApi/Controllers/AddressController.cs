
namespace UDO.LOB.Address.Controllers
{
    using Swashbuckle.Swagger.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions.Logging;
    using UDO.LOB.Address.Messages;
    using UDO.LOB.Address.Processors;

    //using VRM.Integration.Servicebus.Core;
    //using VRM.Integration.UDO.Contact.Messages;
    //using VRM.Integration.UDO.Contact.Processors;
    // using VRM.UDO.Integration.Contact.Processors;

    public class AddressController : ApiController
    {
        [SwaggerOperation("getAddress")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("getAddress")]
        // GET api/Address
        public IMessageBase GetAddress([FromBody]UDOgetAddressRecordsRequest message)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.getAddress ");
            try
            {
                var processor = new UDOgetAddressRecordsProcessor();
                return processor.Execute((UDOgetAddressRecordsRequest)message);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.getAddress ");
            }

        }

        [SwaggerOperation("createAddress")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createAddress")]
        // POST api/Address
        public IMessageBase CreateAddress([FromBody]UDOcreateAddressRecordsRequest message)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createAddress ");
            try
            {
                var processor = new UDOcreateAddressRecordsProcessor();
                return processor.Execute((UDOcreateAddressRecordsRequest)message);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createAddress ");
            }
        }

        [SwaggerOperation("validateAddress")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("validateAddress")]
        // PUT api/address/validateAddress
        public IMessageBase ValidateAddress([FromBody]UDOValidateAddressRequest message)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.validateAddress ");
            try
            {
                var processor = new UDOvalidateAddressRecordProcessor();
                return processor.Execute((UDOValidateAddressRequest)message);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.validateAddress ");
            }
        }
    }
}