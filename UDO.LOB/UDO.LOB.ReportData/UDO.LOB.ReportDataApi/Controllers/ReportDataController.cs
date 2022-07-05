
namespace UDO.LOB.ReportData.Controllers
{
    using Swashbuckle.Swagger.Annotations;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Web.Http;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions.Logging;
    using UDO.LOB.ReportData.Messages;
    using UDO.LOB.ReportData.Processors;

    public class ReportDataController : ApiController
    {
        // POST api/ratings
        [SwaggerOperation("populateReportData")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("populateReportData")]
        public IMessageBase Post([FromBody]UDOpopulateReportDataRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.populateReportData ");
            try
            {
                UDOpopulateReportDataProcessor processor = new UDOpopulateReportDataProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.populateReportData ");
            }
        }

       
    }
}