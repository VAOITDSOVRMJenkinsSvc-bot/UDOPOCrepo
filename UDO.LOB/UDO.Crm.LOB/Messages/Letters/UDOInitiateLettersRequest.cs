﻿using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Common.Messages;
using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOInitiateCADD,InitiateCADD method, Request.
/// Code Generated by IMS on: 8/4/2015 10:18:38 AM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.Letters.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOInitiateLettersRequest)]
    [DataContract]
    public class UDOInitiateLettersRequest : UDORequestBase
    {
        [DataMember]
        public Guid udo_veteranId { get; set; }
        [DataMember]
        public Guid udo_personId { get; set; }
        [DataMember]
        public string fileNumber { get; set; }
        [DataMember]
        public string SSN { get; set; }
        [DataMember]
        public Int64 ptcpntId { get; set; }
        [DataMember]
        public string vetfileNumber { get; set; }
        [DataMember]
        public Guid udo_vetsnapshotId { get; set; }
        [DataMember]
        public string vetSSN { get; set; }
        [DataMember]
        public Int64 vetptcpntId { get; set; }
        [DataMember]
        public string vetFirstName { get; set; }
        [DataMember]
        public string vetLastName { get; set; }
        [DataMember]
        public string vetMiddleInitial { get; set; }
        [DataMember]
        public string vetDOB { get; set; }
        [DataMember]
        public string vetGender { get; set; }
    }
}