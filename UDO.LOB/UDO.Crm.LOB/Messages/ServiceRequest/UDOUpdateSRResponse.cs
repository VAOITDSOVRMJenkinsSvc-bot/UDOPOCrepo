﻿using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Common.Messages;
using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOInitiateSR,InitiateSR method, Response.
/// Code Generated by IMS on: 8/4/2015 10:18:39 AM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace VRM.Integration.UDO.ServiceRequest.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOUpdateSRResponse)]
    [DataContract]
    public class UDOUpdateSRRepsonse : UDOResponseBase
    {
    }
}