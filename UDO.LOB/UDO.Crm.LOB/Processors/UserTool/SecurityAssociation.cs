using Microsoft.Xrm.Sdk;
using System;

namespace VRM.Integration.UDO.UserTool.Processors
{
    internal class SecurityAssociation
    {
        internal EntityReference RelatedObject {get;set;}
        internal Guid UserId { get; set; }
    }
}
