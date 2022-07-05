using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using UDO.LOB.Core.Interfaces;
using UDO.LOB.PersonSearch.Interfaces;
using UDO.LOB.PersonSearch.Models;
using VEIS.Messages.VeteranWebService;
//using VEIS.Messages.Mvi;


//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Interfaces;
//using VRM.Integration.UDO.PersonSearch.Interfaces;
//using VRM.Integration.UDO.PersonSearch.Models;

namespace UDO.LOB.PersonSearch.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    [DataContract]
    public class UDOpsFindPersonResponse : MessageBase, IPersonSearchResponse, IUDOException
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        // Below was added by Gokul but not needed, above item should be used.
        //[DataMember]
        //public bool ExceptionOccured { get; set; } 
        [DataMember]
        public string ExceptionMessage { get; set; }

        [DataMember]
        public VEISfvetfindVeteranResponse VEISFindVeteranResponse { get; set; }

        [DataMember]
        public string URL { get; set; }

        [DataMember]
        public Guid contactId { get; set; }

        [DataMember]
        public Guid idProofId { get; set; }
        
        [DataMember]
        public PatientPerson[] Person { get; set; }

        [DataMember]
        public string MVIMessage { get; set; }
        
        [DataMember]
        public string CORPDbMessage { get; set; }

        [DataMember]
        public string BIRLSMessage { get; set; }
        [DataMember]
        public int BIRLSRecordCount { get; set; }

        [DataMember]
        public string VADIRMessage { get; set; }
        [DataMember]
        public int VADIRRecordCount { get; set; }

        [DataMember]
        public int MVIRecordCount { get; set; }

        [DataMember]
        public int CORPDbRecordCount { get; set; }

        [DataMember]
        public string UDOMessage { get; set; }
        
        [DataMember]
        public string RawMviExceptionMessage { get; set; }

        [DataMember]
        public string OrganizationName { get; set; }

        //[DataMember]
        //public MessageProcessType FetchMessageProcessType { get; set; }
    }
}
