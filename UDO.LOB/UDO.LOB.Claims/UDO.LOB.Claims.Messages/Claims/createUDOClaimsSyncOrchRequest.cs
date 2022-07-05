using System.Runtime.Serialization;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;

namespace UDO.LOB.Claims.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOClaimsSyncOrchRequest)]
    [DataContract]
    public class UDOcreateUDOClaimsSyncOrchRequest : UDOcreateUDOClaimsRequest
    {

    }
}