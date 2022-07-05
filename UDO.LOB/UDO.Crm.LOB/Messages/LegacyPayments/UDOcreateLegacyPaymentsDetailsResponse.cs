﻿using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateUDOLegacyPaymentData,createUDOLegacyPaymentData method, Response.
/// Code Generated by IMS on: 7/13/2015 6:24:18 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace VRM.Integration.UDO.LegacyPayments.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateLegacyPaymentsDetailsResponse)]
    [DataContract]
    public class UDOcreateLegacyPaymentsDetailsResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateUDOLegacyPaymentsDetailsMultipleResponse[] UDOcreateUDOLegacyPaymentDataInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateUDOLegacyPaymentsDetailsMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateUDOLegacyPaymentDataId { get; set; }
    }
}