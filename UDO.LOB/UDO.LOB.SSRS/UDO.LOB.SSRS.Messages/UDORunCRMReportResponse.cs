#region Using Directives

using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using VRM.Integration.Servicebus.Core;

#endregion

namespace UDO.LOB.SSRS.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDORunCRMReportResponse)]
    [DataContract]
    public class UDORunCRMReportResponse : MessageBase
    {
        [DataMember]
        public string ExceptionMessage { get; set; }

        [DataMember]
        public bool ExceptionOccurred { get; set; }

        [DataMember]
        public string udo_Base64FileContents { get; set; }
        [DataMember]
        public Guid? udo_doctypeId { get; set; }
        [DataMember]
        public string udo_Extension { get; set; }

        [DataMember]
        public string udo_FileName { get; set; }

        [DataMember]
        public string udo_MimeType { get; set; }

        [DataMember]
        public string udo_ReportName { get; set; }

        [DataMember]
        public bool udo_Uploaded { get; set; }

        [DataMember]
        public string udo_UploadMessage { get; set; }
    }
}