﻿using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateAwards,createAwards method, Request.
/// Code Generated by IMS on: 6/9/2015 11:40:43 AM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.Awards.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateAwardsSyncRequest)]
    [DataContract]
    public class UDOcreateAwardsSyncRequest : UDOcreateAwardsRequest
    {
    }
}