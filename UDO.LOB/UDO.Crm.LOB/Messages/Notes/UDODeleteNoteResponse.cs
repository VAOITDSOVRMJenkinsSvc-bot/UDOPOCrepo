using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Common.Messages;
using VRM.Integration.UDO.Messages;
/// <summary>
/// VIMT LOB Component for UDOCreateNote,CreateNote method, Response.
/// Code Generated by IMS on: 6/27/2015 2:48:16 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace VRM.Integration.UDO.Notes.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDODeleteNoteResponse)]
    [DataContract]
    public class UDODeleteNoteResponse : UDOResponseBase
    {
        [DataMember]
        public UDODeleteNoteResponseInfo UDODeleteNoteInfo { get; set; }
    }
    [DataContract]
    public class UDODeleteNoteResponseInfo
    {
        [DataMember]
        public string udo_ClaimId { get; set; }
        [DataMember]
        public string udo_DateTime { get; set; }
        [DataMember]
        public string udo_Note { get; set; }
        [DataMember]
        public string udo_ParticipantID { get; set; }
        [DataMember]
        public string udo_RO { get; set; }
        [DataMember]
        public DateTime udo_SuspenseDate { get; set; }
        [DataMember]
        public string udo_Type { get; set; }
        [DataMember]
        public string udo_User { get; set; }
    }
}