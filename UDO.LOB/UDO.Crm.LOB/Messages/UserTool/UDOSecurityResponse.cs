using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;
namespace VRM.Integration.UDO.UserTool.Messages
{
    [Serializable]
    [DataContract]
    public class UDOSecurityResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        //[DataMember]
        //public UDOSecurityErrorMessage[] ErrorMessages { get; set; }
    }
    //[DataContract]
    //public class UDOSecurityErrorMessage
    //{
    //    [DataMember]
    //    public string CallStack { get; set; }
    //    [DataMember]
    //    public string Message { get; set; }
    //    [DataMember]
    //    public string Source { get; set; }
    //}
    
}