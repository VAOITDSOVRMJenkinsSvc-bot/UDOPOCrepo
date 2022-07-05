using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using UDO.VASS.Plugins.Models;
using UDO.VASS.Plugins.Helpers;

namespace UDO.VASS.Plugins
{
    public class CreateNotesRunner : PluginRunner
    {
        private IServiceProvider serviceProvider;
        string apimUrl = "";

        public CreateNotesRunner(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Execute()
        {
            try
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
                        TracingService.Trace($"{prop.Key} - {prop.Value}");
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
                TracingService.Trace($"Header Result Count - {headerRes.Entities.Count}");

                UDOHeader headerObj = new UDOHeader();
                foreach (var obj in headerRes.Entities)
                {
                    headerObj.StationNumber = obj.Attributes["va_stationnumber"].ToString();
                    headerObj.ApplicationName = obj.Attributes["va_applicationname"].ToString();
                    //headerObj.udo_PcrSsn = obj.Attributes["va_pcrssn"].ToString();
                    headerObj.ClientMachine = obj.Attributes["va_ipaddress"].ToString();
                    headerObj.LoginName = obj.Attributes["va_wsloginname"].ToString();

                    TracingService.Trace("Printing Header Info");
                    foreach (var prop in obj.Attributes)
                    {
                        TracingService.Trace($"{prop.Key} - {prop.Value}");
                    }
                }

                TracingService.Trace($"APIM URL - {apimUrl}");
                Uri uri = new Uri(apimUrl);

                if (string.IsNullOrEmpty(apimUrl))
                {
                    TracingService.Trace("No Uri Found!");
                }

                if (PluginExecutionContext.InputParameters.Contains("Target") && PluginExecutionContext.InputParameters["Target"] is Entity)
                {
                    Entity entity = (Entity)PluginExecutionContext.InputParameters["Target"];
                    AttributeCollection coll = entity.Attributes;

                    Guid veteranId = ((EntityReference)coll["udo_contactid"]).Id;
                    var fetchParticipant = @"<fetch>
                                          <entity name='contact' >
                                            <attribute name='udo_participantid' />
                                            <filter>
                                              <condition attribute='contactid' operator='eq' value='" + veteranId + @"' />
                                            </filter>
                                          </entity>
                                        </fetch>";

                    TracingService.Trace("Fetching participant id for the veteran");
                    EntityCollection participantRes = OrganizationService.RetrieveMultiple(new FetchExpression(fetchParticipant));
                    string partipantIdContact = string.Empty;
                    foreach (var res in participantRes.Entities)
                    {
                        partipantIdContact = res.Attributes["udo_participantid"].ToString();
                    }

                    TracingService.Trace($"Participant Id - {partipantIdContact}");


                    TracingService.Trace("Create the request object for creating note");

                    TracingService.Trace("Fetching agent name from systemuser");
                    Guid agentId = ((EntityReference)coll["createdby"]).Id;
                    var fetchAgentName = @"<fetch>
                                          <entity name='systemuser' >
                                            <attribute name='fullname' />
                                            <filter>
                                              <condition attribute='systemuserid' operator='eq' value='" + agentId + @"' />
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

                    UDOCreateNoteRequest request = new UDOCreateNoteRequest()
                    {
                        udo_ParticipantID = partipantIdContact,
                        LegacyServiceHeaderInfo = headerObj,
                        OrganizationName = PluginExecutionContext.OrganizationName,
                        UserId = PluginExecutionContext.UserId,
                        MessageId = Guid.NewGuid().ToString(),
                        udo_DateTime = DateTime.UtcNow.ToString(),
                        udo_RO = headerObj.StationNumber,
                        udo_Note = $"VA Solid Start - A call attempt with disposition as '{disposition}' has been created by {agentName}",
                        Debug = false
                    };

                    TracingService.Trace("Sending the request to api");
                    var response = Utility.SendReceive<UDOCreateNoteResponse>(uri, "UDOCreateNoteRequest", request, 0, settings, TracingService);

                    TracingService.Trace($"Value of ExceptionOccured is '{response.ExceptionOccurred}'");

                    if (!response.ExceptionOccurred)
                    {
                        TracingService.Trace("Updating 'NoteCreatedInMapD' field to 'Yes'");
                        entity["udo_notecreatedinmapd"] = true;
                        OrganizationService.Update(entity);
                        TracingService.Trace("Updated!");
                    }
                    else
                    {
                        TracingService.Trace("Updating 'NoteCreatedInMapD' field to 'No'");
                        entity["udo_notecreatedinmapd"] = false;
                        OrganizationService.Update(entity);
                        TracingService.Trace("Updated!");
                    }
                }
            }
            catch (Exception ex)
            {
                TracingService.Trace("Exception Occurred : " + ex.Message);
            }
        }
    }
}
