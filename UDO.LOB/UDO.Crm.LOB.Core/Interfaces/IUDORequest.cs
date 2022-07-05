using System;

namespace UDO.LOB.Core.Interfaces
{
    public interface IUDORequest
    {
        string OrganizationName { get; set; }
        Guid UserId { get; set; }
        bool LogTiming { get; set; }
        bool LogSoap { get; set; }
        bool Debug { get; set; }

        Guid RelatedParentId { get; set; }
        string RelatedParentEntityName { get; set; }
        string RelatedParentFieldName { get; set; }
    }
}