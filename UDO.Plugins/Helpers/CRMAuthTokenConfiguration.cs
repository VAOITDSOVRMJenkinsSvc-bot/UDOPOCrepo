using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MCSPlugins
{
    public class CRMAuthTokenConfiguration
    {
        public string ClientApplicationId { get; set; }
        public string ClientSecret { get; set; }
        public string ParentApplicationId { get; set; }
        public string TenantId { get; set; }
        public string ApimSubscriptionKey { get; set; }
        public string ApimSubscriptionKeyS { get; set;}
        public string ApimSubscriptionKeyE {get; set;}
    }

    
}
