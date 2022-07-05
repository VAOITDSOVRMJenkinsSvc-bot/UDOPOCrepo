using System;
using System.Security;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Interfaces;
//using VRM.Integration.UDO.PersonSearch.Interfaces;
//using VRM.Integration.UDO.PersonSearch.Models;
using UDO.LOB.Core;
using UDO.LOB.Core.Interfaces;
using UDO.LOB.PersonSearch.Interfaces;
using UDO.LOB.PersonSearch.Models;

namespace UDO.LOB.PersonSearch.Messages
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class UDOpsSelectPersonResponse : MessageBase, IPersonSearchResponse, IUDOException
    {
        /// <summary>
        /// For CMRe, all we want back is the URL of the cmre_person that we either found, or that we created.
        /// </summary>
        [DataMember]
        public Guid OwningTeamId { get; set; }
        [DataMember]
        public string URL { get; set; }
        [DataMember]
        public string SSIdString { get; set; }
        [DataMember]
        public SecureString SSId { get; set; }
        [DataMember]
        public string FileNumber { get; set; }
        [DataMember]
        public string Edipi { get; set; }
        [DataMember]
        public Guid contactId { get; set; }
        [DataMember]
        public string ParticipantId { get; set; }
        [DataMember]
        public string BranchOfService { get; set; }
        [DataMember]
        public string phonenumber { get; set; }
        [DataMember]
        public string VeteranSensitivityLevel { get; set; }
        [DataMember]
        public string GenderCode { get; set; }
        [DataMember]
        public string BirthDate { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string FamilyName { get; set; }
        [DataMember]
        public string DateofBirth { get; set; }
        [DataMember]
        public string Gender { get; set; }
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string crme_CauseOfDeath { get; set; }
        [DataMember]
        public string crme_DateofDeath { get; set; }
        [DataMember]
        public string crme_stationofJurisdiction { get; set; }
        [DataMember]
        public string crme_CharacterofDishcarge { get; set; }
        [DataMember]
        public string RawMviExceptionMessage { get; set; }
        [DataMember]
        public string Rank { get; set; }
        
        
        // For search
        [DataMember]
        public PatientPerson[] Person { get; set; }
        [DataMember]
        public string MVIMessage { get; set; }
        [DataMember]
        public string CORPDbMessage { get; set; }
        [DataMember]
        public int CORPDbRecordCount { get; set; }
    }
}
