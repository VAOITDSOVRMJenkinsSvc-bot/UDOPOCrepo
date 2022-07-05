﻿using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Messages;

/// <summary>
/// VIMT LOB Component for UDOCreateNote,CreateNote method, Request.
/// Code Generated by IMS on: 6/27/2015 2:48:16 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace UDO.LOB.Notes.Messages
{
	//[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOUpdateNoteRequest)]
	[DataContract]
    public class UDOUpdateNoteRequest : UDORequestBase
	{
        [DataMember]
        public string udo_ClaimId { get; set; }
        [DataMember]
        public string udo_LegacyNoteId { get; set; }
        [DataMember]
        public string udo_dtTime { get; set; }
        [DataMember]
        public string udo_Note { get; set; }
        [DataMember]
        public string udo_ParticipantID { get; set; }
        [DataMember]
        public string udo_RO { get; set; }
        [DataMember]
        public string udo_Type { get; set; }
        [DataMember]
        public string udo_pctptnttc { get; set; }
        [DataMember]
        public string udo_User { get; set; }
	}
}