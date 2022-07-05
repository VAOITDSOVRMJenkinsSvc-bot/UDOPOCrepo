using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;


//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;

namespace UDO.LOB.PeoplelistPayeeCode.Messages
{
        //[Export(typeof(IMessageBase))]
        //[ExportMetadata("MessageType", MessageRegistry.UDOFiduciaryExistsResponse)]
        [DataContract]
        public class UDOFiduciaryExistsResponse : MessageBase
        {
            [DataMember]
            public bool ExceptionOccurred { get; set; }
            [DataMember]
            public string ExceptionMessage { get; set; }
            [DataMember]
            public bool FiduciaryExists { get; set; }
        }
    
}
