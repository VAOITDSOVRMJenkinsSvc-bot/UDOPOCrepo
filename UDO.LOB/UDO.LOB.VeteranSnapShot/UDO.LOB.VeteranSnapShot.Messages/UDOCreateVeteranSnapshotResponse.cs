using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using UDO.LOB.Core.Interfaces;
using UDO.LOB.Extensions;
using VEIS.Core.Messages;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Interfaces;

namespace UDO.LOB.VeteranSnapShot.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOCreateVeteranSnapshotResponse)]
    [DataContract]
    public class UDOCreateVeteranSnapshotResponse : MessageBase, IUDOException
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }

        [DataMember]
        public Guid udo_veteransnapshotId { get; set; }
    }
}