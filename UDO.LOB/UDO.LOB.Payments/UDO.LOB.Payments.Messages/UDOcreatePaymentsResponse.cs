﻿using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
//using UDO.LOB.Extensions;
using VEIS.Core.Messages;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;

/// <summary>
/// VIMT LOB Component for UDOcreatePayments,createPayments method, Response.
/// Code Generated by IMS on: 6/4/2015 9:18:47 AM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.Payments.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreatePaymentsResponse)]
    [DataContract]
    public class UDOcreatePaymentsResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreatePaymentsMultipleResponse[] UDOcreatePaymentsInfo { get; set; }
        [DataMember]
        public string NextScheduledPayDate { get; set; }
        [DataMember]
        public string NextAmount { get; set; }
    }
    [DataContract]
    public class UDOcreatePaymentsMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreatePaymentsId { get; set; }
        [DataMember]
        public long ftbid { get; set; }
        [DataMember]
        public long paymentId { get; set; }
    }
}