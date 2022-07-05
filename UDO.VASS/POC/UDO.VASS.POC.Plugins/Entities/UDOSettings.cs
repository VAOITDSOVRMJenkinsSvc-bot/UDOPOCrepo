using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.POC.Plugins.Entities
{
    public class UDOSettings
    {
        public string ClientApplicationId { get; set; }
        public string ClientSecret { get; set; }
        public string ParentApplicationId { get; set; }
        public string TenantId { get; set; }
        public string ApimSubscriptionKey { get; set; }
        public string ApimSubscriptionKeyS { get; set; }
        public string ApimSubscriptionKeyE { get; set; }
    }
}
