
using System;
using CRMUD;

namespace CRM.Plugins.Vai.Common  {
    internal interface IRepository {
        Guid GetUserNccVaiSupervisorQueueId(Guid userId);
        CRMUD.QueueItem GetVaiCurrentQueueItem(Guid vaiId);
        void AssignEntityToQueue(Guid sourceQueueId, Guid destinationQueueId, va_vai preVaiImage);
        void CreateVaiAuditHistory(va_vaiaudithistory auditHistory);
    }
}