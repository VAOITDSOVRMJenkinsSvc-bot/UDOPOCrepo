using System;

namespace VRM.Integration.UDO.Claims.Messages
{
    public class UDOcreateUDOLifecyclesResponse
    {
        public bool ExceptionOccured { get; set; }
        public string ExceptionMessage { get; set; }
        public UDOcreateUDOLifecyclesMultipleResponse[] UDOcreateUDOLifecyclesInfo { get; set; }
    }
    public class UDOcreateUDOLifecyclesMultipleResponse
    {
        public Guid newUDOcreateUDOLifecyclesId { get; set; }
    }
}