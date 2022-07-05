using UDO.LOB.Core.Interfaces;
using UDO.LOB.PersonSearch.Models;
//using VRM.Integration.UDO.Common.Interfaces;
//using VRM.Integration.UDO.PersonSearch.Models;

namespace UDO.LOB.PersonSearch.Interfaces
{
    public interface IPersonSearchResponse : IUDOException
    {
        PatientPerson[] Person { get; set; }
        string MVIMessage { get; set; }
        string RawMviExceptionMessage { get; set; }
        string CORPDbMessage { get; set; }
        int CORPDbRecordCount { get; set; }
    }
}