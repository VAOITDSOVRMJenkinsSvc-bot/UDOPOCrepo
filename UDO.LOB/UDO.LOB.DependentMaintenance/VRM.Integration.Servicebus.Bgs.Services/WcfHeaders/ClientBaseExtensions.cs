using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
//using VRM.Integration.Servicebus.Core;
using ServiceEndpoint = System.ServiceModel.Description.ServiceEndpoint;

namespace VRM.Integration.Servicebus.Bgs.Services
{
    public static class ClientBaseExtensions
    {
        public static void AddBgsSecurityHeader(this ServiceEndpoint serviceEndpoint, string userName,
            string password,
            string clientMachine,
            string stnId,
            string applicationId) 
        {
            var customEndpointBehavior =
                new AddWseSecurityHeaderEndpointBehavior(userName, 
                    password,
                    clientMachine,
                    stnId,
                    applicationId);

            serviceEndpoint.Behaviors.Add(customEndpointBehavior);
        }

        public static BgsHeaderInfo GetBgsHeaderInfo(IOrganizationService organizationService, Guid userId)
        {
            var context = new OrganizationServiceContext(organizationService);

            //Using late bound here because cannot guarantee that the passed OrgService will have 
            //the correct type of SystemUser 
            var systemUserLateBound = (from d in context.CreateQuery("systemuser")
                                       where (Guid)d["systemuserid"] == userId
                                       select d).FirstOrDefault();

            if (systemUserLateBound == null)
                throw new Exception(string.Format("Crm User [{0}] was not found", userId));

            const string stationNumberIsNotAssignedForCrmUser = "Station Number is not assigned for CRM User.";
            const string vaStationnumber = "va_stationnumber";

            if(!systemUserLateBound.Attributes.ContainsKey(vaStationnumber))
                throw new Exception(stationNumberIsNotAssignedForCrmUser);

            const string wsLoginIsNotAssignedForCrmUser = "WS Login is not assigned for CRM User.";
            const string vaWsloginname = "va_wsloginname";

            if (!systemUserLateBound.Attributes.ContainsKey(vaWsloginname))
                throw new Exception(wsLoginIsNotAssignedForCrmUser);

            const string applicationNameIsNotAssignedForCrmUser = "Application Name is not assigned for CRM User.";
            const string vaApplicationname = "va_applicationname";

            if (!systemUserLateBound.Attributes.ContainsKey(vaApplicationname))
                throw new Exception(applicationNameIsNotAssignedForCrmUser);

            const string clientMachineIsNotAssignedForCrmUser = "Client Machine is not assigned for CRM User.";
            const string vaIpAddress = "va_ipaddress";

            if (!systemUserLateBound.Attributes.ContainsKey(vaIpAddress))
                throw new Exception(clientMachineIsNotAssignedForCrmUser);

            var stationNumber = (string)systemUserLateBound[vaStationnumber];

            var loginName = (string)systemUserLateBound[vaWsloginname];

            var applicationName = (string)systemUserLateBound[vaApplicationname];

            var clientMachine = (string)systemUserLateBound[vaIpAddress];

            var password = string.Empty;

            if (string.IsNullOrEmpty(stationNumber))
                throw new Exception(stationNumberIsNotAssignedForCrmUser);

            if (string.IsNullOrEmpty(loginName))
                throw new Exception(wsLoginIsNotAssignedForCrmUser);

            if (string.IsNullOrEmpty(applicationName))
                throw new Exception(applicationNameIsNotAssignedForCrmUser);

            if (string.IsNullOrEmpty(clientMachine))
                throw new Exception(clientMachineIsNotAssignedForCrmUser);

            return new BgsHeaderInfo
            {
                StationNumber = stationNumber,

                LoginName = loginName,

                ApplicationName = applicationName,

                ClientMachine = clientMachine,

                Password = password
            };
        }

        public static void AddBgsSecurityHeader(this ServiceEndpoint serviceEndpoint, IOrganizationService organizationService, Guid userId)
        {
            var bgsHeaderInfo = GetBgsHeaderInfo(organizationService, userId);

            AddBgsSecurityHeader(serviceEndpoint,
                bgsHeaderInfo.LoginName,
                bgsHeaderInfo.Password,
                bgsHeaderInfo.ClientMachine,
                bgsHeaderInfo.StationNumber,
                bgsHeaderInfo.ApplicationName);
        }
        
        public static void AddVvaSecurityHeader(this ServiceEndpoint serviceEndpoint)
        {
            var configuration = VvaSecurityConfiguration.Current;

            AddVvaSecurityHeader(serviceEndpoint,
                configuration.UserName,
                configuration.Password);
        }

        public static void AddVvaSecurityHeader(this ServiceEndpoint serviceEndpoint, string userName,
            string password)
        {
            var customEndpointBehavior =
                new AddVvaSecurityHeaderEndpointBehavior(userName,
                    password);

            serviceEndpoint.Behaviors.Add(customEndpointBehavior);
        }

        public static string HttpPostFile(string uri, string fileName)
        {
            var text = System.IO.File.ReadAllText(fileName);

            return HttpPost(uri, text);
        }

        public static string HttpPost(string uri, string parameters)
        {
            var req = System.Net.WebRequest.Create(uri);

            req.ContentType = "text/xml";

            req.Method = "POST";

            var bytes = System.Text.Encoding.ASCII.GetBytes(parameters);

            req.ContentLength = bytes.Length;

            var os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);
            os.Close();

            var resp = req.GetResponse();

            var sr = new System.IO.StreamReader(resp.GetResponseStream());

            return sr.ReadToEnd().Trim();
        }
    }
}