using System;

namespace VRM.Integration.Servicebus.AddDependent.Messages
{
    public interface IAddDependentOchestrationRequest
    {
        string OrganizationName { get; set; }
        Guid DependentMaintenanceId { get; set; }
        Guid UserId { get; set; }
    }
}