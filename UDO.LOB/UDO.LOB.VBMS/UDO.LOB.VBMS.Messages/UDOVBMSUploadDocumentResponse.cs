using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using VRM.Integration.Servicebus.Core;

namespace UDO.LOB.VBMS.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOVBMSUploadDocumentResponse)]
    [DataContract]
    public class UDOVBMSUploadDocumentResponse : MessageBase
    {
        [DataMember]
        public bool Processing { get; set; }
        
        [DataMember]
        public string ExceptionMessage { get; set; }

        [DataMember]
        public bool ExceptionOccurred { get; set; }
        
        [DataMember]
        public string udo_vbmsdcsid { get; set; }
        [DataMember] 
        public string udo_vbmscategory { get; set; } 
        [DataMember]
        public string udo_vbmsfilename { get; set; }
        [DataMember]
        public DateTime udo_vbmsuploaddate { get; set; }
        [DataMember]
        public bool udo_uploaded { get; set; }
        [DataMember]
        public string udo_uploadmessage { get; set; }

        [DataMember]
        public Guid? udo_doctypeid { get; set; }
        [DataMember]
        public string mcs_errorCode { get; set; }

        [DataMember]
        public string mcs_errorId { get; set; }

        [DataMember]
        public string mcs_errorTypeField { get; set; }


    }
}
