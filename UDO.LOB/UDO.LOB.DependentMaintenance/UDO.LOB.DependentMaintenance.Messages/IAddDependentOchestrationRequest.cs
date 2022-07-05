using System;

namespace UDO.LOB.DependentMaintenance.Messages
{
    public interface IAddDependentOchestrationRequest
    {
        string OrganizationName { get; set; }
        Guid DependentMaintenanceId { get; set; }
        Guid UserId { get; set; }
    }
}