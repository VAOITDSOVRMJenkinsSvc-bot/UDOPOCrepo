// NOTE : to run, requires Windows Identity Foundation http://www.microsoft.com/download/en/details.aspx?displaylang=en&id=17331

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceModel.Description;

using Microsoft.Xrm.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Client;

using Xrm;

//using CommunicatorAPI;
//using CommunicatorPrivate;

namespace VA.VRMUD.Web
{
    public partial class Emergency : System.Web.UI.Page
    {
        private OrganizationDetail CurrentOrganizationDetail { get; set; }
        private IDiscoveryService DiscoveryService { get; set; }
        private IOrganizationService OrgService { get; set; }
        private String TeamName { get; set; }
        private String CRMServerURL { get; set; }
        private String OrgServiceURL { get; set; }
        private String SMTPServer { get; set; }
        private String CRMUniqueName { get; set; }
        private String FromName { get; set; }
        private String FormType { get; set; }
        private String OwnerId { get; set; }
        private String UserName { get; set; }
        private String UserDomain { get; set; }
        private String Pwd { get; set; }
        private String PCRName { get; set; }
        private int EmailsSent = 0;
        XRMVAContext context = null;
        private bool declaringEmergency;
        //CommunicatorAPI.Messenger communicator= null;
        //IMessengerConversationWndAdvanced imWindow = null;
        //long imWindowHandle = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
                TeamName = ConfigurationManager.AppSettings["TeamName"];
                FromName = ConfigurationManager.AppSettings["FromName"];
                declaringEmergency = (this.Request["cancel"] == null || (this.Request["cancel"].ToUpper() != "Y" && this.Request["cancel"] != "1") );
                EmployeeEmergency_Button.Text = String.Format("{0} Emergency", declaringEmergency ? "Declare" : "Cancel");
                if (this.Request["user"] != null) PCRName = this.Request["user"];
                if (!String.IsNullOrEmpty(this.Request["formtype"])) FormType = this.Request["formtype"];
                if (!String.IsNullOrEmpty(this.Request["ownerid"])) OwnerId = this.Request["ownerid"];
                if (!String.IsNullOrEmpty(this.Request["from"])) FromName = this.Request["from"];

                if (!this.IsPostBack)
                {
                    // load emails for members of Supervisor group; send smtp email with emergency text to each email
                    CRMServerURL = ConfigurationManager.AppSettings["CrmServer"];
                    OrgServiceURL = ConfigurationManager.AppSettings["CrmOrgServiceUrl"];
                    CRMUniqueName = ConfigurationManager.AppSettings["CrmOrgUniqueName"];
                    UserName = ConfigurationManager.AppSettings["UserName"];
                    UserDomain = ConfigurationManager.AppSettings["UserDomain"];
                    Pwd = ConfigurationManager.AppSettings["Pwd"];


                    ConnectToCRM();
                    GetTeamMemberEmails(OwnerId != null ? new Guid(OwnerId) : new Guid());
                    txtEmergencyText.Text = "Emergency Declared by PCR";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = String.Format("Error detected: {0}\n{1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty);
            }
        }
 
        protected void EmployeeEmergency_Button_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtRecepients.Text.Trim().Length == 0) return;
                String subject = String.Format("Emergency {0} by PCR {1}", declaringEmergency ? "Declared" : "Cancelled", PCRName);
                if (txtEmergencyText.Text.Trim().Length == 0) txtEmergencyText.Text = subject;

                String[] arEmails = txtRecepients.Text.Split(new char[] { ';' });
                arEmails.ToList().ForEach(s => SendMessage(s, subject));
                EmailsSent = arEmails.Length;

                lblMessage.Text = String.Format("{0} message(s) sent", EmailsSent);
                EmployeeEmergency_Button.Visible = false;
            }
            catch (Exception ex)
            {
                lblMessage.Text = String.Format("Error detected: {0}\n{1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty);
            } 
            

        }

        protected void GetTeamMemberEmails(Guid id)
        {
            Entity user = this.OrgService.Retrieve("systemuser", id, new ColumnSet(new[]{"siteid"}));
            Guid siteid = new Guid();
            if (user.Contains("siteid"))
            {
                siteid = user.GetAttributeValue<EntityReference>("siteid").Id;
            }
            // get all members of the team TeamName
            QueryExpression qe = new QueryExpression();
            qe.EntityName = "team";
            qe.ColumnSet = new ColumnSet();
            qe.ColumnSet.Columns.Add("name");

            qe.LinkEntities.Add(new LinkEntity("team", "teammembership", "teamid", "teamid", JoinOperator.Inner));
            qe.LinkEntities[0].Columns.AddColumns("systemuserid");
            qe.LinkEntities[0].EntityAlias = "tm";
            qe.LinkEntities[0].AddLink("systemuser", "systemuserid", "systemuserid", JoinOperator.Inner);
            qe.LinkEntities[0].LinkEntities[0].Columns.AddColumns("siteid", "mobilealertemail", "internalemailaddress", "fullname");
            qe.LinkEntities[0].LinkEntities[0].EntityAlias = "teammember";
            //qe.LinkEntities[0].LinkEntities[0].AddLink("systemuser", "systemuserid", "systemuserid", JoinOperator.Inner);
            //qe.LinkEntities[0].LinkEntities[0].LinkEntities[0].Columns.AddColumns("mobilealertemail", "internalemailaddress", "fullname");
            //qe.LinkEntities[0].LinkEntities[0].LinkEntities[0].EntityAlias = "teammember";
            
            FilterExpression filter = new FilterExpression();
            ConditionExpression condition = new ConditionExpression("name", ConditionOperator.Equal, this.TeamName);
            filter.Conditions.Add(condition);
            qe.Criteria.AddFilter(filter);

            FilterExpression filter2 = new FilterExpression();
            ConditionExpression condition2 = new ConditionExpression("siteid", ConditionOperator.Equal, siteid);
            filter2.Conditions.Add(condition2);
            qe.LinkEntities[0].LinkEntities[0].LinkCriteria.AddFilter(filter2);
            EntityCollection ec = OrgService.RetrieveMultiple(qe);

            List<string> emails = new List<string>();
            foreach (var u in ec.Entities)
            {
                if(u.GetAttributeValue<AliasedValue>("teammember.mobilealertemail") != null)
                    emails.Add(u.GetAttributeValue<AliasedValue>("teammember.mobilealertemail").Value.ToString());
                else if (u.GetAttributeValue<AliasedValue>("teammember.internalemailaddress") != null)
                    emails.Add(u.GetAttributeValue<AliasedValue>("teammember.internalemailaddress").Value.ToString());
            }


            string s = String.Join(";", emails);
            
            /*
            var members =
                from m in context.TeamMembershipSet
                join t in context.TeamSet on m.TeamId equals t.TeamId
                join user in context.SystemUserSet on m.SystemUserId equals user.SystemUserId
                where t.Name == TeamName 
                //&& (!String.IsNullOrEmpty(user.MobileAlertEMail.ToString()) || !String.IsNullOrEmpty(user.InternalEMailAddress.Length.ToString()) )
                //&& ((string)m["InternalEMailAddress"]).Length > 0
                select user.MobileAlertEMail != null && user.MobileAlertEMail.Length > 0 ? user.MobileAlertEMail : user.InternalEMailAddress;

            members.ToList().ForEach(m => System.Diagnostics.Debug.WriteLine(m));

            string s = String.Join(";", members.ToArray());*/

            if (s == null || s.Length == 0)
                lblMessage.Text = String.Format("None of the members of {0} team have e-mails specified.", TeamName);
            else
            {
                lblMessage.Text = String.Format("Auto-populated e-mails for the members of {0} team.", TeamName);
                txtRecepients.Text = s;
            }
        }

        protected void ConnectToCRM()
        {
            //var connection = new CrmConnection(String.Format("Url={0}; Domain={1}; Username={2}; Password={3};", OrgServiceURL, UserDomain, UserName, Pwd));
            //var service = new OrganizationService(connection);
            //var cont = new CrmOrganizationServiceContext(connection);

            IServiceConfiguration<IDiscoveryService> dinfo = null;
            // give 3 tries to account for timeout
            int tryCount = 0;
            ConnectionStart:
            try
            {
                tryCount++;
                dinfo = ServiceConfigurationFactory.CreateConfiguration<IDiscoveryService>(GetDiscoveryServiceUri(CRMServerURL));
                tryCount = 4;
            }
            catch (Exception cex)
            {
                if (tryCount >= 3) throw;
            }
            if (tryCount < 4) goto ConnectionStart;


            var creds = new ClientCredentials();

            //If IFD/claims-based authentication, use stored credentials to log on to CRM.
            if (!String.IsNullOrEmpty(UserName))
            {
                creds.UserName.UserName = UserName;
                creds.UserName.Password = Pwd;
                //creds.Windows.ClientCredential.UserName = UserName;
                //creds.Windows.ClientCredential.Password = Pwd; 
            }

            /*
            Uri organizationUriIFD = new Uri(CRMServerURL);
            IServiceConfiguration<IOrganizationService> config = ServiceConfigurationFactory.CreateConfiguration<IOrganizationService>(organizationUriIFD);
            OrganizationServiceProxy _serviceProxy = new OrganizationServiceProxy(config, creds);
            _serviceProxy.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(new ProxyTypesBehavior());
            */
            Uri orgServiceUri = null;

            if (String.IsNullOrEmpty(OrgServiceURL))
            {
                DiscoveryServiceProxy dsp = new DiscoveryServiceProxy(dinfo, creds);
                try
                {
                    dsp.Authenticate();
                }
                catch (Exception e)
                {
                    throw new Exception("Error Authenticating User. \nMessage: " + e.Message + "\nStackTrace:\n" + e.StackTrace);
                }
                RetrieveOrganizationsRequest orgRequest = new RetrieveOrganizationsRequest();
                RetrieveOrganizationsResponse orgResponse = dsp.Execute(orgRequest) as RetrieveOrganizationsResponse;
                foreach (var o in orgResponse.Details)
                {
                    if (o.UniqueName == this.CRMUniqueName)
                    {
                        this.CurrentOrganizationDetail = o;
                        break;
                    }
                }
                if (this.CurrentOrganizationDetail == null)
                {
                    throw new Exception("Could not connect to specified CRM organization.");
                }
                orgServiceUri = new Uri(CurrentOrganizationDetail.Endpoints[EndpointType.OrganizationService]);
            }
            else
                orgServiceUri = new Uri(OrgServiceURL);

            IServiceConfiguration<IOrganizationService> orgConfigInfo =
                                        ServiceConfigurationFactory.CreateConfiguration<IOrganizationService>(orgServiceUri);

            using (var serviceProxy = new OrganizationServiceProxy(orgConfigInfo, creds))
            {
                OrgService = serviceProxy;
                serviceProxy.EnableProxyTypes();
                context = new XRMVAContext(serviceProxy);

                Microsoft.Crm.Sdk.Messages.WhoAmIResponse response = (Microsoft.Crm.Sdk.Messages.WhoAmIResponse)serviceProxy.Execute(new Microsoft.Crm.Sdk.Messages.WhoAmIRequest());
            }

        }

        private Uri GetDiscoveryServiceUri(string serverName)
        {
            string discoSuffix = @"/XRMServices/2011/Discovery.svc";

            return new Uri(string.Format("{0}{1}", serverName, discoSuffix));
        }

        protected void SendMessage(string to, String subject)
        {
            if (String.IsNullOrEmpty(to)) return;
            SendSMTPMail(SMTPServer, FromName, to, subject, txtEmergencyText.Text.Trim());
        }

        protected void SendSMTPMail(string server, string from, string to, string subject, string body)
        {
            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = body;
            SmtpClient client = new SmtpClient(server);
            // Credentials are necessary if the server requires the client 
            // to authenticate before it will send e-mail on the client's behalf.
            client.UseDefaultCredentials = true;
            var creds = new NetworkCredential(UserName, Pwd);
            client.Credentials = creds;

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Exception caught in SendSMTPMail(): {0}", ex.Message));
            }
        }
    }
}