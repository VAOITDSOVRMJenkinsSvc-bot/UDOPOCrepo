using System;
namespace VRM.Integration.UDO.SSRS.Messages
{
    public class UDORunCRMReportResponse 
    {
        public string ExceptionMessage { get; set; }
        public bool ExceptionOccured { get; set; }
        public string udo_Base64FileContents { get; set; }
        public string udo_Extension { get; set; }
        public string udo_FileName { get; set; }
        public string udo_MimeType { get; set; }
        public string udo_ReportName { get; set; }
        public bool udo_Uploaded { get; set; }
        public string udo_UploadMessage { get; set; }
        public Guid? udo_vbmsdocumentid { get; set; }
    }
}