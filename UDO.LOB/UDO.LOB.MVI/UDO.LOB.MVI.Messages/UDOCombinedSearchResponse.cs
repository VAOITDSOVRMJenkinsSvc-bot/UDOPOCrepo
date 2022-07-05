using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using VEIS.Messages.VeteranWebService;
namespace UDO.LOB.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>

    [DataContract]
    public class UDOCombinedPersonSearchResponse : MessageBase
    {
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
        public bool ExceptionOccurred { get; set; }

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

        // TODO: Commented to check the need in new remediated code
        //[DataMember]
        //public MessageProcessType FetchMessageProcessType { get; set; }
    }
   
}
