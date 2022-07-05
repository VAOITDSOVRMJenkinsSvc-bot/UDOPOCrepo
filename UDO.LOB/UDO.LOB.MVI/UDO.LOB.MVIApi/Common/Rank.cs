using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDO.LOB.eMIS.Messages;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Messages;
using UDO.LOB.Core;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//using VRM.Integration.UDO.eMIS.Messages;
//using VRM.Integration.UDO.MVI.Messages;

namespace UDO.LOB.MVI.Common
{
    public static class Rank
    {
        public static void GetPersonsRank(bool debug, bool logSoap, bool logTiming, string orgName, Guid userId, PatientPerson person)
        {
            if (person == null)
            {
                return;
            } 
            else
            {
                LogHelper.LogDebug(orgName, debug, userId, "GetPersonsRank", "EDIPI: " + person.EdiPi ?? "<null>");
                person.Rank = string.Empty;
                if (String.IsNullOrEmpty(person.EdiPi)) return;
            }

            #region Get Rank

            var miRequest = new UDOgetMilitaryInformationRequest()
            {                
                Debug = debug,
                LogSoap = logSoap,
                LogTiming = logTiming,
                MessageId = Guid.NewGuid().ToString(),
                OrganizationName = orgName,
                UserId = userId,
                udo_MostRecentServiceOnly = true,
                udo_EDIPI = person.EdiPi
            };

            var miResponse = WebApiUtility.SendReceive<UDOgetMilitaryInformationResponse>(miRequest, WebApiType.LOB);
                       
            // Ignore errors - just won't have a rank.
            //if (miResponse.ExceptionOccurred) return;

            if (miResponse != null)
            {
                if (miResponse.ExceptionOccurred) return;
                var serviceInfo = miResponse.udo_MostRecentService;
                if (serviceInfo != null)
                {
                    person.Rank = serviceInfo.RankName;
                }
            }

            return;
            #endregion
        }
    }
}
