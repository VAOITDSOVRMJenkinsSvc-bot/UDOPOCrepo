using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using UDO.VASS.POC.Plugins.Entities;
using UDO.VASS.POC.Plugins.NotesApi;

namespace UDO.VASS.POC.Plugins
{
    public class CreateNotesRunner : PluginRunner
    {
        private IServiceProvider serviceProvider;
        string apimUrl = "";
        //private static bool _Debug;
        //public bool getDebug
        //{
        //    get { return _Debug; }
        //}

        public CreateNotesRunner(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Execute()
        {
            TracingService.Trace("Fetching Settings");
            var settingsFetch = @"<fetch>
                                      <entity name='mcs_setting' >
                                        <attribute name='udo_aadtenent' />
                                        <attribute name='udo_oauthclientsecret' />
                                        <attribute name='udo_ocpapimsubscriptionkeysouth' />
                                        <attribute name='udo_ocpapimsubscriptionkeyeast' />
                                        <attribute name='udo_oauthresourceid' />
                                        <attribute name='udo_oauthclientid' />
                                        <attribute name='udo_ocpapimsubscriptionkey' />
                                        <attribute name='crme_restendpointforvimt' />
                                      </entity>
                                    </fetch>";

            EntityCollection result = OrganizationService.RetrieveMultiple(new FetchExpression(settingsFetch));
            UDOSettings settings = new UDOSettings();
            foreach (var c in result.Entities)
            {
                settings.TenantId = c.Attributes["udo_aadtenent"].ToString();
                settings.ClientSecret = c.Attributes["udo_oauthclientsecret"].ToString();
                settings.ApimSubscriptionKey = c.Attributes["udo_ocpapimsubscriptionkey"].ToString();
                settings.ApimSubscriptionKeyS = c.Attributes["udo_ocpapimsubscriptionkeysouth"].ToString();
                settings.ApimSubscriptionKeyE = c.Attributes["udo_ocpapimsubscriptionkeyeast"].ToString();
                settings.ParentApplicationId = c.Attributes["udo_oauthresourceid"].ToString();
                settings.ClientApplicationId = c.Attributes["udo_oauthclientid"].ToString();
                apimUrl = c.Attributes["crme_restendpointforvimt"].ToString();

                TracingService.Trace("Printing Settings");
                foreach (var prop in c.Attributes)
                {
                    TracingService.Trace(prop.Key + " - " + prop.Value);
                }
            }
            
            TracingService.Trace("Fetching Header Info");
            var headerInfoFetch = @"<fetch>
                                            <entity name='systemuser' >
                                            <attribute name='va_stationnumber' />
                                            <attribute name='va_applicationname' />
                                            <attribute name='va_pcrssn' />
                                            <attribute name='va_ipaddress' />
                                            <attribute name='va_wsloginname' />
                                            <filter>
                                                <condition attribute='systemuserid' operator='eq' value='" + PluginExecutionContext.UserId + @"' />
                                            </filter>
                                            </entity>
                                        </fetch>";

            EntityCollection headerRes = OrganizationService.RetrieveMultiple(new FetchExpression(headerInfoFetch));
            TracingService.Trace("Executed Fetch");
            TracingService.Trace("Header Result Count - " + headerRes.Entities.Count);

            UDOHeader headerObj = new UDOHeader();
            foreach (var obj in headerRes.Entities)
            {

                TracingService.Trace("udo_StationNumber" + obj.Attributes["va_stationnumber"].ToString());
                TracingService.Trace("udo_ApplicationName" + obj.Attributes["va_applicationname"].ToString());
                TracingService.Trace("udo_PcrSSN" + obj.Attributes["va_pcrssn"].ToString());
                TracingService.Trace("udo_IPAddress" + obj.Attributes["va_ipaddress"].ToString());
                TracingService.Trace("udo_WSLoginName" + obj.Attributes["va_wsloginname"].ToString());

                headerObj.StationNumber = obj.Attributes["va_stationnumber"].ToString();
                headerObj.ApplicationName = obj.Attributes["va_applicationname"].ToString();
                headerObj.udo_PcrSsn = obj.Attributes["va_pcrssn"].ToString();
                headerObj.ClientMachine = obj.Attributes["va_ipaddress"].ToString();
                headerObj.LoginName = obj.Attributes["va_wsloginname"].ToString();

                TracingService.Trace("Printing Header Info");
                foreach (var prop in obj.Attributes)
                {
                    TracingService.Trace(prop.Key + " - " + prop.Value);
                }
            }

            TracingService.Trace("APIM URL - " + apimUrl);
            Uri uri = new Uri(apimUrl);
            TracingService.Trace("Uri - " + uri);

            if (string.IsNullOrEmpty(apimUrl))
            {
                TracingService.Trace("No Uri Found!");
            }

            if (PluginExecutionContext.InputParameters.Contains("Target") && PluginExecutionContext.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)PluginExecutionContext.InputParameters["Target"];
                AttributeCollection coll = entity.Attributes;

                Guid VeteranId = ((EntityReference)coll["udo_contactid"]).Id;
                var fetchParticipant =@"<fetch>
                                          <entity name='contact' >
                                            <attribute name='udo_participantid' />
                                            <filter>
                                              <condition attribute='contactid' operator='eq' value='" + VeteranId + @"' />
                                            </filter>
                                          </entity>
                                        </fetch>";

                TracingService.Trace("Fetching participant id for the veteran - " + VeteranId);
                EntityCollection participantRes = OrganizationService.RetrieveMultiple(new FetchExpression(fetchParticipant));
                string partipantIdContact = string.Empty;
                foreach(var res in participantRes.Entities)
                {
                    partipantIdContact = res.Attributes["udo_participantid"].ToString();
                }

                TracingService.Trace("Participant Id is " + partipantIdContact);

                TracingService.Trace("Create the request object for creating note");

                TracingService.Trace("Fetching agent name from systemuser");
                Guid AgentId = ((EntityReference)coll["createdby"]).Id;
                var fetchAgentName = @"<fetch>
                                          <entity name='systemuser' >
                                            <attribute name='fullname' />
                                            <filter>
                                              <condition attribute='systemuserid' operator='eq' value='" + AgentId + @"' />
                                            </filter>
                                          </entity>
                                        </fetch>";

                EntityCollection userRes = OrganizationService.RetrieveMultiple(new FetchExpression(fetchAgentName));
                string agentName = string.Empty;
                foreach (var res in userRes.Entities)
                {
                    agentName = res.Attributes["fullname"].ToString();
                }

                var disposition = entity.FormattedValues["udo_disposition"].ToString();
                //var createdOn = coll["createdon"].ToString();

                UDOCreateNoteRequest request = new UDOCreateNoteRequest()
                {
                    udo_ParticipantID = partipantIdContact,
                    LegacyServiceHeaderInfo = headerObj,
                    OrganizationName = PluginExecutionContext.OrganizationName,
                    UserId = PluginExecutionContext.UserId,
                    MessageId = Guid.NewGuid().ToString(),
                    udo_DateTime = string.Empty,
                    udo_RO = headerObj.StationNumber,
                    udo_Note = String.Format("A call attempt with disposition as '{0}' has been created by {1}", disposition, agentName),
                    Debug = true
                };

                TracingService.Trace("Sending the request to api");
                var response = Utility.SendReceive<UDOCreateNoteResponse>(uri, "UDOCreateNoteRequest", request, 0, settings, TracingService);
            }
        }
    }
}
