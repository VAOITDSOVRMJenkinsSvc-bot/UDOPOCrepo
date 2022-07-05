using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VIMT.VeteranWebService.Messages;
using System;
namespace VRM.Integration.UDO.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    [Export(typeof (IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOCombinedPersonSearchResponse)]
    [DataContract]
    public class UDOCombinedPersonSearchResponse : MessageBase
    {
        [DataMember]
        public VIMTfvetfindVeteranResponse VIMTFindVeteranResponse { get; set; }
        [DataMember]
        public string URL { get; set; }

        [DataMember]
        public Guid contactId { get; set; }

        [DataMember]
        public Guid idProofId { get; set; }
        
        [DataMember]
        public PatientPerson[] Person { get; set; }

        [DataMember]
        public bool ExceptionOccured { get; set; }

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

        [DataMember]
        public MessageProcessType FetchMessageProcessType { get; set; }
    }
   
}
