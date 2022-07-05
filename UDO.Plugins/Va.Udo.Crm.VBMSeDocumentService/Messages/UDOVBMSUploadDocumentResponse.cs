using System;
using System.Runtime.Serialization;

namespace VRM.Integration.UDO.VBMS.Messages
{
    public class UDOVBMSUploadDocumentResponse 
    {
        public bool Processing { get; set; }
        public string ExceptionMessage { get; set; }
        public bool ExceptionOccured { get; set; }
        /*public string udo_vbmsdcsid { get; set; }
        public string udo_vbmscategory { get; set; }
        public string udo_vbmsfilename { get; set; }
        public DateTime udo_vbmsuploaddate { get; set; }
        public bool udo_uploaded { get; set; }
        public string udo_uploadmessage { get; set; }
         */
    }
}
