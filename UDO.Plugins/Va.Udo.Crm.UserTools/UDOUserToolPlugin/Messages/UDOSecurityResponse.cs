using System;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;

namespace VRM.Integration.UDO.UserTool.Messages
{
    public class UDOSecurityResponse
    {
        public bool ExceptionOccured { get; set; }
        public string ExceptionMessage { get; set; }
    }   
}