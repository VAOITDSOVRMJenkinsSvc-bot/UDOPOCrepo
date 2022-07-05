using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

namespace VRM.Integration.UDO.PeoplelistPayeeeCode.Messages
{
        [Export(typeof(IMessageBase))]
        [ExportMetadata("MessageType", MessageRegistry.UDOFiduciaryExistsResponse)]
        [DataContract]
        public class UDOFiduciaryExistsResponse : MessageBase
        {
            [DataMember]
            public bool ExceptionOccured { get; set; }
            [DataMember]
            public string ExceptionMessage { get; set; }
            [DataMember]
            public bool FiduciaryExists { get; set; }
        }
    
}
