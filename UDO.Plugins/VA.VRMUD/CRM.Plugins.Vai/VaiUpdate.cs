using System;
using System.ServiceModel;
using CRM.Plugins.Vai.Common;
using Microsoft.Xrm.Sdk;
using CRMUD;

namespace CRM.Plugins.Vai {

    public class VaiUpdate : IPlugin {
        private ITracingService _log;
        private IPluginExecutionContext _pluginContext;
        private IOrganizationServiceFactory _serviceFactory;
        private IOrganizationService _serviceProxy;
        private ServiceContext _serviceContext;
        private IRepository _repository;

        public void Execute(IServiceProvider serviceProvider) {
            _log = (ITracingService) serviceProvider.GetService(typeof (ITracingService));
            _pluginContext = (IPluginExecutionContext) serviceProvider.GetService(typeof (IPluginExecutionContext));
            _serviceFactory = (IOrganizationServiceFactory) serviceProvider.GetService(typeof (IOrganizationServiceFactory));
            _serviceProxy = _serviceFactory.CreateOrganizationService(_pluginContext.InitiatingUserId);

            _log.Trace("VaiUpdate: context init");
                
            if (_pluginContext.InputParameters.Contains("Target") && _pluginContext.InputParameters["Target"] is Entity) {
                try {
                    va_vai preVaiImage = null;
                    if (_pluginContext.PreEntityImages.Contains("PreVaiImage")) {
                        preVaiImage = _pluginContext.PreEntityImages["PreVaiImage"].ToEntity<va_vai>();
                    }
                    preVaiImage.ThrowOnNull();

                    _log.Trace("VaiUpdate: got pre Vai image");

                    va_vai postVaiImage = null;
                    if (_pluginContext.PostEntityImages.Contains("PostVaiImage")) {
                        postVaiImage = _pluginContext.PostEntityImages["PostVaiImage"].ToEntity<va_vai>();
                    }
                    postVaiImage.ThrowOnNull();

                    _log.Trace("VaiUpdate: got post Vai image");

                    try {
                        // VAI Assaing to NCC supervisors queue
                        if (preVaiImage.statuscode.Value == Constants.StatusReasonCodeValueNotSet && postVaiImage.statuscode.Value != Constants.StatusReasonCodeValueNotSet) {

                            AssignVaiToQueue(preVaiImage, _pluginContext.InitiatingUserId);

                            _log.Trace("VaiUpdate: assign to supervisors queue complete");
                        } else {
                            _log.Trace("VaiUpdate: pre and post statuscode are not the ones desired");
                        }

                        // VAI audit history
                        if (preVaiImage.statuscode.Value != postVaiImage.statuscode.Value) {

                            CreateAuditRecord(postVaiImage);

                            _log.Trace("VaiUpdate: Creted audit record complete");
                        } else {
                            _log.Trace("VaiUpdate: pre and post statuscode are the same");
                        }

                    } finally {
                        if (_serviceContext != null)
                            _serviceContext.Dispose();
                    }


                } catch (FaultException<OrganizationServiceFault> ex) {
                    _log.Trace(ex.ToString());
                    throw new InvalidPluginExecutionException("An error occurred in the Vai Update plug-in.", ex);
                } catch (Exception ex) {
                    _log.Trace("An error occurred in the Vai Update plug-in: {0}", ex.ToString());
                    throw;
                }
            }
        }

        private void AssignVaiToQueue(va_vai preVaiImage, Guid userId) {
           _serviceContext = _serviceContext ?? new ServiceContext(_serviceProxy);
            _log.Trace("VaiUpdate: got service context");

            _repository =_repository ?? new Repository(_serviceContext, _log);
            _log.Trace("VaiUpdate: got repository");

            var queueId = _repository.GetUserNccVaiSupervisorQueueId(userId);
            queueId.ThrowOnNull();
            _log.Trace("VaiUpdate: got NCC supervisors queue");

            var queueItem = _repository.GetVaiCurrentQueueItem(preVaiImage.Id);
            queueItem.ThrowOnNull();
            _log.Trace("VaiUpdate: got current VAI queue item");

            _repository.AssignEntityToQueue(queueItem.QueueId.Id, queueId, preVaiImage);
            _log.Trace("VaiUpdate: assigned VAI to ncc supervisors queue");
        }

        private void CreateAuditRecord(va_vai postVaiImage) {
            _serviceContext = _serviceContext ?? new ServiceContext(_serviceProxy);
            _log.Trace("VaiUpdate: got service context");

            _repository =_repository ?? new Repository(_serviceContext, _log);
            _log.Trace("VaiUpdate: got repository");

            var vaiAuditHistory = new va_vaiaudithistory() {
                va_ChangedField = Constants.StatusReasonChangeName,
                va_ChangedValue = postVaiImage.FormattedValues[Constants.StatusReasonFieldName],
                va_VAI = new EntityReference(postVaiImage.LogicalName, postVaiImage.Id)
            };

            _repository.CreateVaiAuditHistory(vaiAuditHistory);
            _log.Trace("VaiUpdate: VAI Audit History created");
        }
    }
}