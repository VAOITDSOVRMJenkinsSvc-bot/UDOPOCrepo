using System.Runtime.Serialization;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;

namespace UDO.LOB.Claims.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOClaimsSyncRequest)]
    [DataContract]
    public class UDOcreateUDOClaimsSyncRequest : UDOcreateUDOClaimsRequest
    {

    }
}