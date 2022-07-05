using System;
namespace VRM.Integration.UDO.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
  
    public class UDOCombinedSelectedPersonResponse 
    {
        /// <summary>
        /// For CMRe, all we want back is the URL of the cmre_person that we either found, or that we created.
        /// </summary>
        
        public string URL { get; set; }

        
        public Guid contactId { get; set; }
        
        
        public bool ExceptionOccured { get; set; }

        public string Message { get; set; }

        public string RawMviExceptionMessage { get; set; }

        public string VeteranSensitivityLevel { get; set; }

        public Guid idProofId { get; set; }

        public PatientPerson[] Person { get; set; }

        public string MVIMessage { get; set; }

        public string CORPDbMessage { get; set; }

        public int MVIRecordCount { get; set; }

        public int CORPDbRecordCount { get; set; }

        public string UDOMessage { get; set; }

        public string OrganizationName { get; set; }

        public string FetchMessageProcessType { get; set; }
    }
}
