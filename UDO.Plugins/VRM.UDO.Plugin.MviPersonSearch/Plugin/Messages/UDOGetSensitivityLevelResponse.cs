using System;

namespace VRM.Integration.UDO.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    public class UDOGetSensitivityLevelResponse
    {

        
        public string SensitivityLevel { get; set; }
        
        public string ErrorMessage { get; set; }
        
        public bool ExceptionOccured { get; set; }

    }
}
