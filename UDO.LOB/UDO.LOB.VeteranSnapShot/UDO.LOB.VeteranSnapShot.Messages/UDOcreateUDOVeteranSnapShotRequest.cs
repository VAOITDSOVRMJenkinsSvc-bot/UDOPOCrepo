﻿using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using VEIS.Core.Messages;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Interfaces;
//using VRM.Integration.UDO.Common.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateUDOVeteranSnapShot,createUDOVeteranSnapShot method, Request.
/// Code Generated by IMS on: 7/22/2015 4:09:38 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.VeteranSnapShot.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOVeteranSnapShotRequest)]
    [DataContract]
    public class UDOcreateUDOVeteranSnapShotRequest : MessageBase
    {
       
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        //[DataMember]
        //public Guid RelatedParentId { get; set; }
        //[DataMember]
        //public string RelatedParentEntityName { get; set; }
        //[DataMember]
        //public string RelatedParentFieldName { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public Guid udo_veteransnapshotid { get; set; }
        [DataMember]
        public Guid udo_veteranid { get; set; }
        [DataMember]
        public string PID { get; set; }
        [DataMember]
        public string fileNumber { get; set; }
    }
   
}