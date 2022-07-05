using System;

namespace UDO.LOB.DependentMaintenance
{
    public class ProcRequestState : IProcRequestState
    {
        public ProcRequestState()
        {
            CreatedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
        }

        public long VnpProcId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
