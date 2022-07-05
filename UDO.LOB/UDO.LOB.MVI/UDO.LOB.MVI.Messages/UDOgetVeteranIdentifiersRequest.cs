using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

/// <summary>
/// VIMT LOB Component for UDOgetVeteranIdentifiers,getVeteranIdentifiers method, Request.
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace UDO.LOB.MVI.Messages
{
    [DataContract]
    public class UDOgetVeteranIdentifiersRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public Int64 ptcpntId { get; set; }
        [DataMember]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
    }
}
