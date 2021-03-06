using System.Runtime.Serialization;
using UDO.LOB.Core;

/// <summary>
/// VIMT LOB Component for UDOcreateFlashes,createFlashes method, Request.
/// Code Generated by IMS on: 5/19/2015 2:33:53 PM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace UDO.LOB.Flashes.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOgetFlashesRequest)]
    [DataContract]
    public class UDOgetFlashesRequest : UDORequestBase
    {
        // Covered by UDORequestBase
        //[DataMember]
        //public string OrganizationName { get; set; }
        //[DataMember]
        //public Guid UserId { get; set; }
        //[DataMember]
        //public bool LogTiming { get; set; }
        //[DataMember]
        //public bool LogSoap { get; set; }
        //[DataMember]
        //public bool Debug { get; set; }
        //[DataMember]
        //public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public string ptcpntVetId { get; set; }
        [DataMember]
        public string ptcpntBeneId { get; set; }
        [DataMember]
        public string ptpcntRecipId { get; set; }
        [DataMember]
        public string fileNumber { get; set; }
        [DataMember]
        public string depFileNumber { get; set; }
        [DataMember]
        public string awardTypeCd { get; set; }
    }
}