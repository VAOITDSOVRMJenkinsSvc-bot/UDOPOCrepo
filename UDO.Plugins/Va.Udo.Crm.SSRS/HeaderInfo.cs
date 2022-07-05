using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.LOB.Core
{
    public class HeaderInfo
    {
        public string StationNumber { get; set; }
        public string LoginName { get; set; }
        public string ApplicationName { get; set; }
        public string ClientMachine { get; set; }
    }

    public class UDOHeaderInfo
    {
        public string StationNumber { get; set; }
        public string LoginName { get; set; }
        public string ApplicationName { get; set; }
        public string ClientMachine { get; set; }
        public static UDOHeaderInfo GetHeaderInfo(IOrganizationService _service, Guid _initiatinguser)
        {
            ColumnSet userCols = new ColumnSet("va_stationnumber", "va_wsloginname", "va_applicationname", "va_ipaddress", "fullname", "va_pcrsensitivitylevel");
            Entity thisUser = _service.Retrieve("systemuser", _initiatinguser, userCols);

            const string stationNumberIsNotAssignedForCrmUser = "Station Number is not assigned for CRM User.";
            const string vaStationnumber = "va_stationnumber";

            if (!thisUser.Attributes.ContainsKey(vaStationnumber))
                throw new Exception(stationNumberIsNotAssignedForCrmUser);

            const string wsLoginIsNotAssignedForCrmUser = "WS Login is not assigned for CRM User.";
            const string vaWsloginname = "va_wsloginname";

            if (!thisUser.Attributes.ContainsKey(vaWsloginname))
                throw new Exception(wsLoginIsNotAssignedForCrmUser);

            const string applicationNameIsNotAssignedForCrmUser = "Application Name is not assigned for CRM User.";
            const string vaApplicationname = "va_applicationname";

            if (!thisUser.Attributes.ContainsKey(vaApplicationname))
                throw new Exception(applicationNameIsNotAssignedForCrmUser);

            const string clientMachineIsNotAssignedForCrmUser = "Client Machine is not assigned for CRM User.";
            const string vaIpAddress = "va_ipaddress";

            if (!thisUser.Attributes.ContainsKey(vaIpAddress))
                throw new Exception(clientMachineIsNotAssignedForCrmUser);

            var stationNumber = (string)thisUser[vaStationnumber];

            var loginName = (string)thisUser[vaWsloginname];

            var applicationName = (string)thisUser[vaApplicationname];

            var clientMachine = (string)thisUser[vaIpAddress];

            var fullName = thisUser.GetAttributeValue<string>("fullname");

            var userSL = thisUser.GetAttributeValue<OptionSetValue>("va_pcrsensitivitylevel");

            return new UDOHeaderInfo
            {
                StationNumber = stationNumber,

                LoginName = loginName,

                ApplicationName = applicationName,

                ClientMachine = clientMachine,
            };
        }
    }
}
