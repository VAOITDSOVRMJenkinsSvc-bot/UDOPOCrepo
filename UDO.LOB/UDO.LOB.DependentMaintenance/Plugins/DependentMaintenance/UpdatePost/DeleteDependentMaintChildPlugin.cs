using System;
using System.Threading;
using Microsoft.Xrm.Sdk;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class DeleteDependentMaintChildPlugin
    {
        public IOrganizationService OrganizationService { get; private set; }
        public Guid Id { get; private set; }

        public DeleteDependentMaintChildPlugin(IOrganizationService organizationService, Guid id)
        {
            OrganizationService = organizationService;
            Id = id;
        }

        public void Exec()
        {
            try
            {
                Thread.Sleep(5000);

                OrganizationService.Delete("crme_dependentmaintenance", Id);

            }
            catch (Exception ex)
            {
                Entity thisLog = new Entity();
                thisLog.LogicalName = "mcs_log";
                thisLog["mcs_name"] = "Error in Cancel";
                thisLog["mcs_errormessage"] = ex.StackTrace;
                OrganizationService.Create(thisLog);
            }     
        }
    }
}