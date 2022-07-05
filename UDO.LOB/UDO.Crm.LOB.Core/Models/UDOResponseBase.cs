using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core.Interfaces;

namespace UDO.LOB.Core
{
    [Export(typeof(IMessageBase))]
    [DataContract]
    public abstract class UDOResponseBase : MessageBase
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOException[] InnerExceptions { get; set; }
    }

    [DataContract]
    public class UDOException : IUDOException
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public string ExceptionCategory { get; set; }
    }
}
