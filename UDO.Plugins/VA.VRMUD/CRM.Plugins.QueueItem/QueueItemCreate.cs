using System;
using System.ServiceModel;
using CRM.Plugins.QueueItem.Common;
using Microsoft.Xrm.Sdk;
using CRMUD;

namespace CRM.Plugins.QueueItem {

    public class QueueItemCreate : IPlugin {

        public void Execute(IServiceProvider serviceProvider) {
            var log = (ITracingService) serviceProvider.GetService(typeof (ITracingService));
            var pluginContext = (IPluginExecutionContext) serviceProvider.GetService(typeof (IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var serviceProxy = serviceFactory.CreateOrganizationService(pluginContext.InitiatingUserId);
            
            log.Trace("QueueItemCreate: context initalize");

            if (pluginContext.InputParameters.Contains("Target") && pluginContext.InputParameters["Target"] is Entity) {
                try {
                    CRMUD.QueueItem postQueueItemImage = null;
                    if (pluginContext.PostEntityImages.Contains("PostQueueItemImage")) {
                        postQueueItemImage = pluginContext.PostEntityImages["PostQueueItemImage"].ToEntity<CRMUD.QueueItem>();
                    }
                    postQueueItemImage.ThrowOnNull();

                    log.Trace("QueueItemCreate: got post Queue item image");

                    if (postQueueItemImage.ObjectId.LogicalName != Constants.VaiEntityLogicalName) {
                        log.Trace("QueueItemCreate: ObjectId is not a va_vai entity");
                        return;
                    }

                    CreateAuditRecord(serviceProxy, log, postQueueItemImage);

                } catch (FaultException<OrganizationServiceFault> ex) {
                    log.Trace(ex.ToString());
                    throw new InvalidPluginExecutionException("An error occurred in the Queue Item Create plug-in.", ex);
                } catch (Exception ex) {
                    log.Trace("An error occurred in the Queue Item Create plug-in: {0}", ex.ToString());
                    throw;
                }
            }
        }

        private void CreateAuditRecord(IOrganizationService serviceProxy, ITracingService log, CRMUD.QueueItem postQueueItemImage) {
            using (var serviceContext = new ServiceContext(serviceProxy)) {
                log.Trace("QueueItemCreate: got service context");

                IRepository repository = new Repository(serviceContext, log);
                log.Trace("QueueItemCreate: got repository");

                var vaiAuditHistory = new va_vaiaudithistory() {
                    va_ChangedField = Constants.FieldChangeType,
                    va_ChangedValue = string.Format(Constants.FieldChangeValue, postQueueItemImage.QueueId.Name.Replace("<", "").Replace(">", "")),
                    va_VAI = new EntityReference(va_vai.EntityLogicalName, postQueueItemImage.ObjectId.Id)
                };

                repository.CreateVaiAuditHistory(vaiAuditHistory);

                log.Trace("QueueItemCreate: VAI Audit History created");
            }
        }
    }
}