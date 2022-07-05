﻿using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

/// <summary>
/// VIMT LOB Component for UDOcreateAddressRecords,createAddressRecords method, Response.
/// Code Generated by IMS on: 5/27/2015 11:21:44 AM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.Crm.LOB.Messages.Address
{
    [DataContract]
    public class UDOcreateAddressRecordsResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateAddressRecordsMultipleResponse[] UDOcreateAddressRecordsInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateAddressRecordsMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateAddressRecordsId { get; set; }
    }
}