using Microsoft.Xrm.Sdk;
using System;

namespace UDO.LOB.UserTool.Processors
{
    internal class SecurityAssociation
    {
        internal EntityReference RelatedObject {get;set;}
        internal Guid UserId { get; set; }
    }
}
