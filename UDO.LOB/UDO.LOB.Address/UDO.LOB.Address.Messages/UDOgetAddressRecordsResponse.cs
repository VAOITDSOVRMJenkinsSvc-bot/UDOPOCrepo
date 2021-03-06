using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateAddressRecords,createAddressRecords method, Response.
/// Code Generated by IMS on: 5/27/2015 11:21:44 AM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.Address.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOgetAddressRecordsResponse)]
    [DataContract]
    public class UDOgetAddressRecordsResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
       
    }
   
}