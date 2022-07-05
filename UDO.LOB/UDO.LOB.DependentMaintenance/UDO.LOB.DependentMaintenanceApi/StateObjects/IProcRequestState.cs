using System;

namespace UDO.LOB.DependentMaintenance
{
    public interface IProcRequestState
    {
        long VnpProcId { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime LastModifiedDate { get; set; }
    }
}