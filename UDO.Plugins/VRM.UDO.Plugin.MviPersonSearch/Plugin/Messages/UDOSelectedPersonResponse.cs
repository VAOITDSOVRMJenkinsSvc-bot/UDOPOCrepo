using System;
namespace VRM.Integration.UDO.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
  
    public class UDOSelectedPersonResponse 
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
    }
}
