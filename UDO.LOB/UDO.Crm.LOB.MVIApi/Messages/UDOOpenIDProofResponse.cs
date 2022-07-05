using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.UDO.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    [Export(typeof (IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOOpenIDProofResponse)]
    [DataContract]
    public class UDOOpenIDProofResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
      
    }
}
