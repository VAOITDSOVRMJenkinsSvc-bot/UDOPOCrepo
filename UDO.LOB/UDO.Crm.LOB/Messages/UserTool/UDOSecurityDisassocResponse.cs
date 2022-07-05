using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

namespace VRM.Integration.UDO.UserTool.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOSecurityDisassocResponse)]
    [DataContract]
    public class UDOSecurityDisassocResponse : UDOSecurityResponse
    {
      
    }
}