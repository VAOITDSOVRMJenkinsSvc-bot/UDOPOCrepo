using System;
using System.Security;
using UDO.LOB.Core;
using UDO.LOB.Core.Interfaces;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Interfaces;
//using VRM.Integration.UDO.Common.Messages;

namespace UDO.LOB.PersonSearch.Interfaces
{
    public interface IPersonSearchRequest: IUDORequest, IMessageBase
    {
        string UserFirstName { get; set; }
        string UserLastName { get; set; }

        string MiddleName { get; set; }
        string PhoneNumber { get; set; }
        string FamilyName { get; set; }
        string FirstName { get; set; }
        string BirthDate { get; set; }
        
        SecureString SSId { get; set; }
        int SSIdLength { get; set; }

        string Edipi { get; set; }
        
        bool IsAttended { get; set; }
        UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        Int64 ParticipantID { get; set; }
        bool BypassMvi { get; set; }
    }
}