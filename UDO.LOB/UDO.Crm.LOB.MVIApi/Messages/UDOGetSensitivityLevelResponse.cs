using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.UDO.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOGetSensitivityLevelResponse)]
    [DataContract]
    public class UDOGetSensitivityLevelResponse : MessageBase
    {

        [DataMember]
        public string SensitivityLevel { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public bool ExceptionOccured { get; set; }

    }
}
