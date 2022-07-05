﻿using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;

/// <summary>
/// VIMT LOB Component for UDOgetVBMSDocumentContent,getVBMSDocumentContent method, Response.
/// Code Generated by IMS on: 6/1/2016 11:51:24 AM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.VBMSeFolder.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOgetVBMSDocumentContentResponse)]
    [DataContract]
    public class UDOgetVBMSDocumentContentResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public Guid udo_attachmentId { get; set; }
        
    }
}
