using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
/// <summary>
/// VIMT LOB Component for UDOUDOcreateDeathRating,UDOcreateDeathRating method, Response.
/// Code Generated by IMS on: 6/15/2015 11:09:42 AM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace VRM.Integration.UDO.Ratings.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateDeathRatingResponse)]
    [DataContract]
    public class UDOcreateDeathRatingResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOUDOcreateDeathRatingMultipleResponse[] UDOUDOcreateDeathRatingInfo { get; set; }
    }
    [DataContract]
    public class UDOUDOcreateDeathRatingMultipleResponse
    {
        [DataMember]
        public Guid newUDOUDOcreateDeathRatingId { get; set; }
    }
}