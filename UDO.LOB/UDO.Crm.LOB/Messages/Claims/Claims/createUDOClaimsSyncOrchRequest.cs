using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

namespace VRM.Integration.UDO.Claims.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOClaimsSyncOrchRequest)]
    [DataContract]
    public class UDOcreateUDOClaimsSyncOrchRequest : UDOcreateUDOClaimsRequest
    {

    }
}