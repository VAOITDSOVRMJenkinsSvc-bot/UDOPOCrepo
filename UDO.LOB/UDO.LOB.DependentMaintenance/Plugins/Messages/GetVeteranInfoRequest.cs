using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRM.CRME.Plugin.DependentMaintenance.Messages
{
    public class GetVeteranInfoRequest 
    {
        public string MessageId { get; set; }
        public string crme_OrganizationName { get; set; }
        public Guid crme_UserId { get; set; }
        public string crme_SSN { get; set; }
		public bool crme_debug { get; set; }
	}
}
