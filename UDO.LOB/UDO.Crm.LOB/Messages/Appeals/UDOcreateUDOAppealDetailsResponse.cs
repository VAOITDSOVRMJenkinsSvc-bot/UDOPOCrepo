﻿using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;
/// <summary>
/// VIMT LOB Component for UDOcreateUDOAppeals,createUDOAppeals method, Response.
/// Code Generated by IMS on: 7/8/2015 5:03:28 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace VRM.Integration.UDO.Appeals.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOAppealDetailsResponse)]
    [DataContract]
    public class UDOcreateUDOAppealDetailsResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateUDOAppealDetailsMultipleResponse[] UDOcreateUDOAppealsInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateUDOAppealDetailsMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateUDOAppealDetailsId { get; set; }
    }
}
