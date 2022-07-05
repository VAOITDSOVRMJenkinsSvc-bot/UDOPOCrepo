using System;
using CRMUD;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace CRM.Plugins.Vai.Common {
    internal class Repository : IRepository {
        private readonly ServiceContext _context;
        private readonly ITracingService _log;

        public Repository(ServiceContext context, ITracingService log) {
            _context = context;
            _log = log;
        }

        public Guid GetUserNccVaiSupervisorQueueId(Guid userId) {
            var siteId = _context.SystemUserSet.SingleOrDefault(u => u.Id == userId).SiteId.Id;
            var teamId = _context.va_sitesteamsSet.SingleOrDefault(s => s.va_Site.Id == siteId && s.va_name == Constants.NCCVaiSiteToTeamKey).va_Team.Id;
            return _context.TeamSet.SingleOrDefault(t => t.Id == teamId).QueueId.Id;
        }

        public QueueItem GetVaiCurrentQueueItem(Guid vaiId) {
            return _context.QueueItemSet.SingleOrDefault(q => q.ObjectId.Id == vaiId);
        }

        public void AssignEntityToQueue(Guid sourceQueueId, Guid destinationQueueId, va_vai preVaiImage) {
            var queueRequest = new AddToQueueRequest {
                SourceQueueId = sourceQueueId,
                Target = new EntityReference(preVaiImage.LogicalName, preVaiImage.Id),
                DestinationQueueId = destinationQueueId
            };

            _context.Execute(queueRequest);
        }

        public void CreateVaiAuditHistory(va_vaiaudithistory auditHistory) {
            _context.AddObject(auditHistory);
            _context.SaveChanges();
        }
    }
}