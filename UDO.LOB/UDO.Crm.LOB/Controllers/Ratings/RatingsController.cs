using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Ratings.Messages;
using VRM.Integration.UDO.Ratings.Processors;

namespace UDO.Crm.LOB.Controllers.Ratings
{
    public class RatingsController : ApiController
    {
        // GET api/ratings
        [SwaggerOperation("findRatings")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("findratings")]
        public IMessageBase Get([FromBody]UDOfindRatingsRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOfindRatingsProcessor();
                return processor.Execute((UDOfindRatingsRequest)message);
            }
            catch (Exception ex)
            {
                UDOfindRatingsRequest msg = (UDOfindRatingsRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_UDOfindRatingsRequest", msg.UserId, "UDOfindRatingsRequest.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOfindRatingsRequest Error: {0}", ex.Message), ex);
            }
        }

        // GET api/ratings
        [SwaggerOperation("getRatings")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("getratings")]
        public IMessageBase Get([FromBody] UDOgetRatingDataRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOgetRatingDataProcessor();
                return processor.Execute((UDOgetRatingDataRequest)message);
            }
            catch (Exception ex)
            {
                UDOgetRatingDataRequest msg = (UDOgetRatingDataRequest)message;
                // LogHelper.LogError(msg.OrganizationName, msg.UserId, "UDOgetRatingDataRequest.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOgetRatingDataRequest Error: {0}", ex.Message), ex);
            }
        }
    }
}