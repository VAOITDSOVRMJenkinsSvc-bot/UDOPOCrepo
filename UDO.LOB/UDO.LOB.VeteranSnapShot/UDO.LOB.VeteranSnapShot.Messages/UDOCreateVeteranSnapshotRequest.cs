using System;
using System.Security;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using VEIS.Core.Messages;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Interfaces;
//using VRM.Integration.UDO.Common.Messages;

namespace UDO.LOB.VeteranSnapShot.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOCreateVeteranSnapshotRequest)]
    [DataContract]
    public class UDOCreateVeteranSnapshotRequest : UDORequestBase
    {
        
        //[DataMember]
        //public string OrganizationName { get; set; }
        //[DataMember]
        //public Guid UserId { get; set; }
        //[DataMember]
        //public Guid RelatedParentId { get; set; }
        //[DataMember]
        //public string RelatedParentEntityName { get; set; }
        //[DataMember]
        //public string RelatedParentFieldName { get; set; }
        //[DataMember]
        //public bool LogTiming { get; set; }
        //[DataMember]
        //public bool LogSoap { get; set; }
        //[DataMember]
        //public bool Debug { get; set; }
         
        //[DataMember]
        //public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public string udo_participantid { get; set; }
        [DataMember]
        public string udo_filenumber { get; set; }
        [DataMember]
        public Guid udo_veteranid { get; set; }
        [DataMember]
        public string udo_name { get; set; }
        [DataMember]
        public Guid udo_idproofid { get; set; }
        [DataMember]
        public string udo_phonenumber { get; set; }
        [DataMember]
        public string udo_firstname { get; set; }
        [DataMember]
        public string udo_lastname { get; set; }
        [DataMember]
        public SecureString ssid { get; set; }
        [DataMember]
        public string udo_characterofdischarge { get; set; }
        [DataMember]
        public string udo_dateofdeath { get; set; }
        [DataMember]
        public string udo_soj { get; set; }
        [DataMember]
        public string udo_branchofservice { get; set; }
        [DataMember]
        public string udo_rank { get; set; }
        [DataMember]
        public string udo_gender { get; set; }
        [DataMember]
        public string udo_birthdatestring { get; set; }
    }
}