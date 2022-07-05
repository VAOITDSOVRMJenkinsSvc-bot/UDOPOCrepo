using CRMUD;
using Microsoft.Xrm.Sdk;

namespace CRM.Plugins.QueueItem.Common {

    internal class Repository : IRepository {
        private readonly ServiceContext _context;
        private readonly ITracingService _log;

        public Repository(ServiceContext context, ITracingService log) {
            _context = context;
            _log = log;
        }

        public void CreateVaiAuditHistory(va_vaiaudithistory auditHistory) {
            _context.AddObject(auditHistory);
            _context.SaveChanges();
        }
    }
}