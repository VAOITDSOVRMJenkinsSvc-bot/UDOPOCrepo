﻿using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using VEIS.Core.Messages;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateUdoContentions,createUdoContentions method, Response.
/// Code Generated by IMS on: 5/29/2015 3:12:46 PM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.Claims.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateUdoContentionsResponse)]
    [DataContract]
    public class UDOcreateUdoContentionsResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateUdoContentionsMultipleResponse[] UDOcreateUdoContentionsInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateUdoContentionsMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateUdoContentionsId { get; set; }
    }
}