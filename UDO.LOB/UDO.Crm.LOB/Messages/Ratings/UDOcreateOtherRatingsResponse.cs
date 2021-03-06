using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
/// <summary>
/// VIMT LOB Component for UDOUDOcreateOtherRatings,UDOcreateOtherRatings method, Response.
/// Code Generated by IMS on: 6/12/2015 3:09:21 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace VRM.Integration.UDO.Ratings.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateOtherRatingsResponse)]
    [DataContract]
    public class UDOcreateOtherRatingsResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOUDOcreateOtherRatingsMultipleResponse[] UDOUDOcreateOtherRatingsInfo { get; set; }
    }
    [DataContract]
    public class UDOUDOcreateOtherRatingsMultipleResponse
    {
        [DataMember]
        public Guid newUDOUDOcreateOtherRatingsId { get; set; }
    }
}
