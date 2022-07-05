using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.MVI.Messages
{
    /// <summary>
    /// UDOOpenIDProofResponse
    /// </summary>
    /// <remarks>
    /// </remarks>

    [DataContract]
    public class UDOOpenIDProofResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
      
    }
}
