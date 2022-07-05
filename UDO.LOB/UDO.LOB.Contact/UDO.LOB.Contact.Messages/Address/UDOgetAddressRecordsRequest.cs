﻿using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using VEIS.Core.Messages;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;

/// <summary>
/// VIMT LOB Component for UDOcreateAddressRecords,createAddressRecords method, Request.
/// Code Generated by IMS on: 5/27/2015 11:21:44 AM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.Contact.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOgetAddressRecordsRequest)]
    [DataContract]
    public class UDOgetAddressRecordsRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        //public Guid RelatedParentId { get; set; }
        //[DataMember]
        //public string RelatedParentEntityName { get; set; }
        //[DataMember]
        //public string RelatedParentFieldName { get; set; }
        //[DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public Int64 ptcpntId { get; set; }
        [DataMember]
        public Guid udo_personid { get; set; }
        [DataMember]
        public string streetAddress1 { get; set; }
        [DataMember]
        public string streetAddress2 { get; set; }
        [DataMember]
        public string streetAddress3 { get; set; }
        [DataMember]
        public string streetAddress4 { get; set; }
        [DataMember]
        public string country { get; set; }
        [DataMember]
        public string postalCode { get; set; }
        [DataMember]
        public string state { get; set; }

    }
}