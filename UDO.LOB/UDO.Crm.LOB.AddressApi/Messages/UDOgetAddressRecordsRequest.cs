﻿using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

/// <summary>
/// VIMT LOB Component for UDOcreateAddressRecords,createAddressRecords method, Request.
/// Code Generated by IMS on: 5/27/2015 11:21:44 AM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace UDO.Crm.LOB.Messages.Address
{
    [DataContract]
    public class UDOgetAddressRecordsRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public new Guid RelatedParentId { get; set; }
        [DataMember]
        public new string RelatedParentEntityName { get; set; }
        [DataMember]
        public new string RelatedParentFieldName { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public ILegacyHeaderInfo LegacyServiceHeaderInfo { get; set; }
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