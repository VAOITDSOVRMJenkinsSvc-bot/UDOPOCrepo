using System.Web.Http;

namespace UDO.Crm.LOB.AddressApi.Controllers
{
    public class PingController : ApiController
    {
        public string Get()
        {
            const string pong = "Pong";
            return pong;
        }
    }
}