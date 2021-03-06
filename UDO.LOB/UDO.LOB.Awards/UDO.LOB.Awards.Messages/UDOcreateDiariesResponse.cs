using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
/// <summary>
/// VIMT LOB Component for UDOcreateDiaries,createDiaries method, Response.
/// Code Generated by IMS on: 5/14/2015 4:00:05 PM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.Awards.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateDiariesResponse)]
    [DataContract]
    public class UDOcreateDiariesResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateDiariesMultipleResponse[] UDOcreateDiariesInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateDiariesMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateDiariesId { get; set; }
    }
}
