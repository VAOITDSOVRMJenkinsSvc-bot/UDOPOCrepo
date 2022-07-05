using System;
using System.Security;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using VEIS.Core.Messages;
using VEIS.Messages.VeteranWebService;

//using VRM.Integration.Servicebus.Core;
//using VIMT.VeteranWebService.Messages;

namespace UDO.LOB.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    //[Export(typeof (IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOAddPersonRequest)]
    [DataContract]
    public class UDOAddPersonRequest : MessageBase
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
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
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
