using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using UDO.LOB.DependentMaintenance.Messages;
using VRM.Integration.Servicebus.Core;

//using VRM.Integration.Servicebus.AddDependent.Messages;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.IntegrationServicebus.AddDependent.CrmModel;
using UDO.LOB.Core;
using VRM.Integration.Servicebus.Bgs.Services;

namespace UDO.LOB.DependentMaintenance
{
    public class AddDependentMaintenanceRequestState : PipeState, IAddDependentMaintenanceRequestState
    {
        public AddDependentMaintenanceRequestState(string organizationName, 
            Guid dependentMaintenanceId, 
            Guid systemUserId,
			bool debug)
        {
            OrganizationName = organizationName;

            DependentMaintenanceId = dependentMaintenanceId;

            SystemUserId = systemUserId;

			Debug = debug;
	}
        
        public string OrganizationName { get; set; }
        public Guid DependentMaintenanceId { get; set; }
        public Guid SystemUserId { get; set; }
		public bool Debug { get; set; }
		public crme_dependentmaintenance DependentMaintenance { get; set; }

        public AddDependentRequest AddDependentRequest { get; set; }
        public IOrganizationService OrganizationService { get; set; }
        public OrganizationServiceContext Context { get; set; }
        public SystemUser SystemUser { get; set; }
        public Exception Exception { get; set; }
		
		public BgsHeaderInfo BgsHeaderInfo { get; set; }
		//public UDOHeaderInfo BgsHeaderInfo { get; set; }
        public string ConfigFieldName { get { return "crme_adddependenttiming"; } }
    }
}