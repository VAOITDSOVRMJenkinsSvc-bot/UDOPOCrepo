using System;
using System.ServiceModel;
using CRM.Plugins.QueueItem.Common;
using Microsoft.Xrm.Sdk;
using CRMUD;

namespace CRM.Plugins.QueueItem {

    public class QueueItemDelete : IPlugin {

        public void Execute(IServiceProvider serviceProvider) {
            var log = (ITracingService) serviceProvider.GetService(typeof (ITracingService));
            var pluginContext = (IPluginExecutionContext) serviceProvider.GetService(typeof (IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory) serviceProvider.GetService(typeof (IOrganizationServiceFactory));
            var serviceProxy = serviceFactory.CreateOrganizationService(pluginContext.InitiatingUserId);

            log.Trace("QueueItemDelete: context init");

            if (pluginContext.InputParameters.Contains("Target") && pluginContext.InputParameters["Target"] is EntityReference) {
                try {
                    CRMUD.QueueItem preQueueItemImage = null;
                    if (pluginContext.PreEntityImages.Contains("PreQueueItemImage")) {
                        preQueueItemImage = pluginContext.PreEntityImages["PreQueueItemImage"].ToEntity<CRMUD.QueueItem>();
                    }
                    preQueueItemImage.ThrowOnNull();

                    log.Trace("QueueItemDelete: got Pre QueueItem Image");

                    if (preQueueItemImage.ObjectId.LogicalName != Constants.VaiEntityLogicalName) {
                        log.Trace("QueueItemDelete: ObjectId is not a va_vai entity");
                        return;
                    }

                    CreateAuditRecord(serviceProxy, log, preQueueItemImage);

                } catch (FaultException<OrganizationServiceFault> ex) {
                    log.Trace(ex.ToString());
                    throw new InvalidPluginExecutionException("An error occurred in the Queue Item Delete plug-in.", ex);
                } catch (Exception ex) {
                    log.Trace("An error occurred in the Queue Item Delete plug-in: {0}", ex.ToString());
                    throw;
                }
            }
        }

        private void CreateAuditRecord(IOrganizationService serviceProxy, ITracingService log, CRMUD.QueueItem preQueueItemImage) {
            using (var serviceContext = new ServiceContext(serviceProxy)) {
                log.Trace("QueueItemDelete: got service context");

                IRepository repository = new Repository(serviceContext, log);
                log.Trace("QueueItemDelete: got repository");

                 var vaiAuditHistory = new va_vaiaudithistory() {
                    va_ChangedField = Constants.FieldChangeType,
                    va_ChangedValue = string.Format(Constants.FieldChangeValue, Constants.RemovedFromQueueValue),
                    va_VAI = new EntityReference(va_vai.EntityLogicalName, preQueueItemImage.ObjectId.Id)
                };

                repository.CreateVaiAuditHistory(vaiAuditHistory);

                log.Trace("QueueItemDelete: VAI Audit History created");
            }
        }
    }
}