using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;


namespace UDO.LOB.UserTool.Messages
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