using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace UDO.LOB.Core
{
    public class HeaderInfo
    {
        public string StationNumber { get; set; }
        public string LoginName { get; set; }
        public string ApplicationName { get; set; }
        public string ClientMachine { get; set; }

        public static HeaderInfo GetHeaderInfo(IOrganizationService _service, Guid _initiatinguser)
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

            return new HeaderInfo
            {
                StationNumber = stationNumber,

                LoginName = loginName,

                ApplicationName = applicationName,

                ClientMachine = clientMachine,
            };
        }
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

    public class PersonSearchHeaderInfo
    {
        public UDOHeaderInfo HeaderInfo { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserSL { get; set; }

        public static PersonSearchHeaderInfo GetHeaderInfo(IOrganizationService _service, Guid _initiatinguser)
        {

            ColumnSet userCols = new ColumnSet("va_stationnumber", "va_wsloginname", "va_applicationname", "va_ipaddress", "fullname", "va_pcrsensitivitylevel", "firstname", "lastname");
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

            const string userSLIsNotAssignedForCrmUser = "PCR Sensitivity is not assigned for CRM User.";
            const string va_pcrsensitivitylevel = "va_pcrsensitivitylevel";

            if (!thisUser.Attributes.ContainsKey(vaIpAddress))
                throw new Exception(clientMachineIsNotAssignedForCrmUser);

            if (!thisUser.Attributes.ContainsKey(va_pcrsensitivitylevel))
                throw new Exception(userSLIsNotAssignedForCrmUser);

            var stationNumber = (string)thisUser[vaStationnumber];

            var loginName = (string)thisUser[vaWsloginname];

            var applicationName = (string)thisUser[vaApplicationname];

            var clientMachine = (string)thisUser[vaIpAddress];

            var fullName = thisUser.GetAttributeValue<string>("fullname");

            var firstName = thisUser.GetAttributeValue<string>("firstname");
            var lastName = thisUser.GetAttributeValue<string>("lastname");

            var userSL = thisUser.GetAttributeValue<OptionSetValue>(va_pcrsensitivitylevel);

            var headerInfo = new UDOHeaderInfo();
            headerInfo.StationNumber = stationNumber;
            headerInfo.LoginName = loginName;
            headerInfo.ApplicationName = applicationName;
            headerInfo.ClientMachine = clientMachine;


            return new PersonSearchHeaderInfo
            {
                HeaderInfo = headerInfo,

                FullName = fullName,

                FirstName = firstName,

                LastName = lastName,

                UserSL = userSL.Value,
            };
        }
    }
}
