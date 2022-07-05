﻿using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using System.ComponentModel.Composition;
//using UDO.LOB.Extensions;
//using VEIS.Core.Messages;

//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOInitiateCADD,InitiateCADD method, Response.
/// Code Generated by IMS on: 8/4/2015 10:18:39 AM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.Letters.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOInitiateLettersResponse)]
    [DataContract]
    public class UDOInitiateLettersResponse : UDOResponseBase
    {
        [DataMember]
        public Guid newUDOInitiateLetterId { get; set; }
        [DataMember]
        public Guid NewUdoNoteId { get; set; }

    }
}