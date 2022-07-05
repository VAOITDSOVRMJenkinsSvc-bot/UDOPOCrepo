using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using UDO.LOB.Core;
using UDO.LOB.DependentMaintenance.Messages;
using VRM.Integration.Servicebus.Bgs.Services;
//using VRM.Integration.Servicebus.AddDependent.Messages;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.IntegrationServicebus.AddDependent.CrmModel;

namespace UDO.LOB.DependentMaintenance
{
    public interface IAddDependentMaintenanceRequestState
    {
        string OrganizationName { get; set; }
        Guid DependentMaintenanceId { get; set; }
        Guid SystemUserId { get; set; }
		bool Debug { get; set; }
		crme_dependentmaintenance DependentMaintenance { get; set; }
        AddDependentRequest AddDependentRequest { get; set; }
        IOrganizationService OrganizationService { get; set; }
        OrganizationServiceContext Context { get; set; }
        SystemUser SystemUser { get; set; }
        Exception Exception { get; set; }
		//CSDev Rem
        BgsHeaderInfo BgsHeaderInfo { get; set; }
        //UDOHeaderInfo BgsHeaderInfo { get; set; }
        string ConfigFieldName { get; }
    }
}