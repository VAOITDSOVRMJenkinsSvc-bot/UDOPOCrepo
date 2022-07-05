using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using Microsoft.Azure;
namespace UDO.Crm.LOB.Controllers
{

    public class ValuesController : ApiController
    {
        // GET api/values
        [SwaggerOperation("GetAll")]
        public IEnumerable<string> Get()
        {
            
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public string Get(int id)
        {
            var returnValue = "Not defined";
            if(id==1)
            {
                if (CloudConfigurationManager.GetSetting("CrmConnectionString") != null)
                   return CloudConfigurationManager.GetSetting("CrmConnectionString");
            }
            else
            {
                throw new WebException("CrmConnectionString not a valid AppSetting.", WebExceptionStatus.UnknownError);
            }

            if (id == 5)
            {
                throw new WebException("Not a valid option.", WebExceptionStatus.UnknownError);
                // returnValue = ConfigurationManager.AppSettings["CrmConnectionString"].ToString();
            }

            if (ConfigurationManager.ConnectionStrings.Count > 0)
            {
                returnValue = ConfigurationManager.ConnectionStrings[0].ConnectionString;
            }

            return returnValue;
        }

        // POST api/values
        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [SwaggerOperation("Update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [SwaggerOperation("Delete")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Delete(int id)
        {
        }
    }
}
