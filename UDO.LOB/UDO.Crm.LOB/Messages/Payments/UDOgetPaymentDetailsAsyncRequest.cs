﻿using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOgetPaymentDetails,getPaymentDetails method, Request.
/// Code Generated by IMS on: 6/4/2015 1:39:18 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.Payments.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOgetPaymentDetailsAsyncRequest)]
    [DataContract]
    public class UDOgetPaymentDetailsAsyncRequest : UDOgetPaymentDetailsRequest
    {
    }
}