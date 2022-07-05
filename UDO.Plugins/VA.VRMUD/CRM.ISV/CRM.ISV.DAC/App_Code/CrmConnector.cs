using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Services;
using System.ServiceModel.Description;

using Microsoft.Xrm.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Client;

using Xrm;

/// <summary>
/// Summary description for CrmConnector
/// </summary>
public class CrmConnector {
    private OrganizationDetail CurrentOrganizationDetail { get; set; }
    private IDiscoveryService DiscoveryService { get; set; }

    private String CRMServerURL { get; set; }
    private String CRMUniqueName { get; set; }
    private Dictionary<string, SystemUser> users = null;

    XRMVAContext context = null;

    public bool ValidateUser(string userName, ref string message, string allowedUsers) {
        message = String.Empty;
        bool res = true;
        //string doValidate = ConfigurationManager.AppSettings["ValidateUser"];
        //if (String.IsNullOrEmpty(doValidate) || doValidate != "1") return true; // validation is not requried

        if (String.IsNullOrEmpty(userName)) {
            res = false;
            message = "User name is not supplied.";
        }

        // see if configured through web.config
        if (!String.IsNullOrEmpty(allowedUsers)) {
            if (!allowedUsers.ToUpper().Contains(userName.ToUpper() + ";")) {
                res = false;
                message = "Current user is not allowed to access VRM Data Access Component.";
            }
            return res;
        }

        // validate through CRM
        if (users == null || users.Count == 0) Connect();
        if (users == null || users.Count == 0) throw new Exception("Failed to retrieve user list.");

        if (!users.ContainsKey(userName.ToUpper())) {
            res = false;
            message = "Current user is not detected as enabled CRM user.";
        }

        return res;
    }

    protected void Connect() {
        try {
            CRMServerURL = ConfigurationManager.AppSettings["CrmServer"];
            CRMUniqueName = ConfigurationManager.AppSettings["CrmOrgUniqueName"];

            ConnectToCRM();
            GetUsers();
        } catch (Exception ex) {
            throw new Exception(String.Format("Error detected: {0}\n{1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty), ex);
        }
    }


    protected void GetUsers() {
        // get all members of the team TeamName
        users = new Dictionary<string, SystemUser>();
        var userSet =
            from m in context.SystemUserSet
            where m.IsDisabled == false
            select m;

        userSet.ToList().ForEach(m => users.Add(m.DomainName.ToUpper(), m));

        //users = String.Join(";", userSet.ToArray()).ToUpper();
    }

    protected void ConnectToCRM() {
        IServiceConfiguration<IDiscoveryService> dinfo = null;
        // give 3 tries to account for timeout
        int tryCount = 0;
    ConnectionStart:
        try {
            tryCount++;
            dinfo = ServiceConfigurationFactory.CreateConfiguration<IDiscoveryService>(GetDiscoveryServiceUri(CRMServerURL));
            tryCount = 4;
        } catch (Exception cex) {
            if (tryCount >= 3) throw;
        }
        if (tryCount < 4) goto ConnectionStart;


        var creds = new ClientCredentials();
        //if (!String.IsNullOrEmpty(UserName))
        //{
        //    creds.Windows.ClientCredential.UserName = UserName;
        //    creds.Windows.ClientCredential.Password = Pwd;
        //}

        DiscoveryServiceProxy dsp = new DiscoveryServiceProxy(dinfo, creds);
        try {
            dsp.Authenticate();
        } catch (Exception e) {
            throw new Exception("Error Authenticating User. \nMessage: " + e.Message + "\nStackTrace:\n" + e.StackTrace);
        }
        RetrieveOrganizationsRequest orgRequest = new RetrieveOrganizationsRequest();
        RetrieveOrganizationsResponse orgResponse = dsp.Execute(orgRequest) as RetrieveOrganizationsResponse;
        if (orgResponse != null)
        {
            foreach (var o in orgResponse.Details)
            {
                if (o.UniqueName == this.CRMUniqueName)
                {
                    this.CurrentOrganizationDetail = o;
                    break;
                }
            }
        }
        if (this.CurrentOrganizationDetail == null) {
            throw new Exception("Could not connect to specified CRM organization.");
        }
        Uri orgServiceUri = new Uri(CurrentOrganizationDetail.Endpoints[EndpointType.OrganizationService]);

        IServiceConfiguration<IOrganizationService> orgConfigInfo =
                                    ServiceConfigurationFactory.CreateConfiguration<IOrganizationService>(orgServiceUri);

        //OrgService = new OrganizationServiceProxy(orgConfigInfo, creds);

        using (var serviceProxy = new OrganizationServiceProxy(orgConfigInfo, creds)) {
            serviceProxy.EnableProxyTypes();
            context = new XRMVAContext(serviceProxy);
        }
    }

    private Uri GetDiscoveryServiceUri(string serverName) {
        string discoSuffix = @"/XRMServices/2011/Discovery.svc";

        return new Uri(string.Format("{0}{1}", serverName, discoSuffix));
    }

}