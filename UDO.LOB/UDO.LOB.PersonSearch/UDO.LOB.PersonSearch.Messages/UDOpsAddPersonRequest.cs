using System;
using System.Security;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Interfaces;
//using VRM.Integration.UDO.Common.Messages;
using UDO.LOB.Core;
using VEIS.Messages.VeteranWebService;
using UDO.LOB.Core.Interfaces;

namespace UDO.LOB.PersonSearch.Messages
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class UDOpsAddPersonRequest : MessageBase, IUDORequest
    {
        [DataMember]
        public VEISfvetfindVeteranResponse VEISResponse { get; set; }
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
        public bool MVICheck { get; set; }
        [DataMember]
        public bool noAddPerson { get; set; }
        [DataMember]
        public ILegacyHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public string SSIdString { get; set; }
        [DataMember]
        public string FileNumber { get; set; }
        [DataMember]
        public Int64 participantID { get; set; }
        [DataMember]
        public string userfirstname { get; set; }
        [DataMember]
        public string userlastname { get; set; }
        [DataMember]
        public string BirthDate { get; set; }
        [DataMember]
        public string FamilyName { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public Guid ContactId { get; set; }
        [DataMember]
        public SecureString SSId { get; set; }
       
      
    }
}
