﻿using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
//using UDO.LOB.Extensions;
//using VEIS.Core.Messages;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;

/// <summary>
/// VIMT LOB Component for UDOUDOcreateSMCRatings,UDOcreateSMCRatings method, Response.
/// Code Generated by IMS on: 6/12/2015 3:18:51 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.MilitaryService.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOfindMilitaryServiceResponse)]
    [DataContract]
    public class UDOfindMilitaryServiceResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateDecorationMultipleResponse[] UDOcreateDecorationInfo { get; set; }
        [DataMember]
        public UDOcreateMilitaryTheaterMultipleResponse[] UDOcreateMilitaryTheatreInfo { get; set; }
        [DataMember]
        public UDOcreatePOWInformationMultipleResponse[] UDOcreatePOWInformationInfo { get; set; }
        [DataMember]
        public UDOcreateRetirementPayMultipleResponse[] UDOcreateRetirementPayInfo { get; set; }
        [DataMember]
        public UDOcreateTourHistoryMultipleResponse[] UDOcreateTourHistoryDetailsInfo { get; set; }


    }
    [DataContract]
    public class UDOcreateDecorationMultipleResponse
    {
        [DataMember]
        public Guid newUDOUDOcreateSMCRatingsId { get; set; }
    }
    [DataContract]
    public class UDOcreateMilitaryTheaterMultipleResponse
    {
        [DataMember]
        public Guid newUDOUDOcreateSMCParagraphRatingsId { get; set; }
    }
    [DataContract]
    public class UDOcreatePOWInformationMultipleResponse
    {
        [DataMember]
        public Guid newUDOUDOcreateDisabilityRatingsId { get; set; }
    }
    [DataContract]
    public class UDOcreateRetirementPayMultipleResponse
    {
        [DataMember]
        public Guid newUDOUDOcreateDisabilityDetailsId { get; set; }
    }
    [DataContract]
    public class UDOcreateTourHistoryMultipleResponse
    {
        [DataMember]
        public Guid newUDOUDOcreateOtherRatingsId { get; set; }
    }
}
