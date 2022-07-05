﻿using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreatePayments,createPayments method, Request.
/// Code Generated by IMS on: 6/4/2015 9:18:47 AM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.Payments.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreatePaymentsAsyncRequest)]
    [DataContract]
    public class UDOcreatePaymentsAsyncRequest : UDOcreatePaymentsRequest
    {
    }
}