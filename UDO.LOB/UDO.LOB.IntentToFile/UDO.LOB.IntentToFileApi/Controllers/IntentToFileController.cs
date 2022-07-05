

namespace UDO.LOB.IntentToFile.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Swashbuckle.Swagger.Annotations;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions;
    using UDO.LOB.Extensions.Logging;
    using UDO.LOB.IntentToFile.Messages;
    using UDO.LOB.IntentToFile.Processors;

    public class IntentToFileController : ApiController
    {
        // POST api/intenttofile
        [SwaggerOperation("createIntentToFile")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createIntentToFile")]
        public IMessageBase CreateIntentToFile([FromBody]  UDOcreateIntentToFileRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createIntentToFile ");
            try
            {
                UDOcreateIntentToFileProcessor processor = new UDOcreateIntentToFileProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createIntentToFile ");
            }
        }

        // POST api/intenttofile
        [SwaggerOperation("initiateITF")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("initiateITF")]
        public IMessageBase Post([FromBody]  UDOInitiateITFRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.initiateITF ");
            try
            {
                UDOInitiateITFProcessor processor = new UDOInitiateITFProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.initiateITF ");
            }
        }

        // POST api/intenttofile
        [SwaggerOperation("submitITF")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("submitITF")]
        public IMessageBase Post([FromBody]  UDOSubmitITFRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.submitITF ");
            try
            {
                UDOSubmitITFProcessor processor = new UDOSubmitITFProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.submitITF ");
            }
        }

        // POST api/intenttofile/findZipCode
        [SwaggerOperation("findZipCode")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("findZipCode")]
        public IMessageBase FindZipCode([FromBody]  UDOfindZipCodeRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.findZipCode ");
            try
            {
                UDOfindZipCodeProcessor processor = new UDOfindZipCodeProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.findZipCode ");
            }
        }

        // POST api/intenttofile
        [SwaggerOperation("validateAddress")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("validateAddress")]
        public IMessageBase Post([FromBody]  UDOvalidateAddressRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.validateAddress ");
            try
            {
                UDOvalidateAddressProcessor processor = new UDOvalidateAddressProcessor();
                return processor.Execute(request);

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

        #region Ping Controller
        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/ping")]
        [SwaggerOperation("ping")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult ping()
        {
            return Ok("pong");
        }

        #endregion
    }
}
