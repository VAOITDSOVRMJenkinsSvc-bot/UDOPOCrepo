using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.Plugins
{
    public class CallAttemptValidationRunner : PluginRunner
    {
        private IServiceProvider serviceProvider;

        public CallAttemptValidationRunner(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Execute()
        {
            #region Commented Tuple Implementation
            //var outboundMessage = "You cannot create more than one call attempt for Left Voicemail, Do Not Contact Request, Unable to Contact/No Voicemail Required and Bad Phone Number in one day.";
            //var outinMessageSuccessful = "You cannot create more than one Successful Outbound and one Successful Inbound call attempt";
            //var inboundMessage = "No Inbound call attempts are allowed for disposition type Left Voicemail, Do Not Contact Request, Unable to Contact/No Voicemail Required and Bad Phone Number";

            //var dict = new Dictionary<Tuple<int, int>, Tuple<int, string>>()
            //{
            //    //https://docs.microsoft.com/en-us/dotnet/api/system.tuple-2.equals?view=netframework-4.8

            //    //Successful Contact, Outbound
            //    { new Tuple<int, int>(752280000, 752280000), new Tuple<int, string>(1, outinMessageSuccessful) },
            //    //Successful Contact, Inbound
            //    { new Tuple<int, int>(752280000, 752280001), new Tuple<int, string>(1, outinMessageSuccessful) },
            //    //Left Voicemail, Outbound
            //    { new Tuple<int, int>(752280002, 752280000), new Tuple<int, string>(1 , outboundMessage) },
            //    //Left Voicemail, Inbound
            //    { new Tuple<int, int>(752280002, 752280001), new Tuple<int, string>(0 , inboundMessage) },
            //    //Do Not Contact Request, Outbound
            //    { new Tuple<int, int>(752280003, 752280000), new Tuple<int, string>(1 , outboundMessage) },
            //    //Do Not Contact Request, Inbound
            //    { new Tuple<int, int>(752280003, 752280001), new Tuple<int, string>(0 , inboundMessage) },
            //    //Unable to Contact/No Voicemail Required, Outbound
            //    { new Tuple<int, int>(752280001, 752280000), new Tuple<int, string>(1 , outboundMessage) },
            //    //Unable to Contact/No Voicemail Required, Inbound
            //    { new Tuple<int, int>(752280001, 752280001), new Tuple<int, string>(0 , inboundMessage) },
            //    //Bad Phone Number, Outbound
            //    { new Tuple<int, int>(752280005, 752280000), new Tuple<int, string>(1 , outboundMessage) },
            //    //Bad Phone Number, Inbound
            //    { new Tuple<int, int>(752280005, 752280001), new Tuple<int, string>(0 , inboundMessage) }
            //};
            #endregion

            var isVASSAdmin = false;

            TracingService.Trace($"Fetching security roles for user {PluginExecutionContext.InitiatingUserId}");
            var fetchUserRoles = @"<fetch>
                                      <entity name='systemuser' >
                                        <filter>
                                          <condition attribute='systemuserid' operator='eq' value='" + PluginExecutionContext.InitiatingUserId + @"' />
                                        </filter>
                                        <link-entity name='systemuserroles' from='systemuserid' to='systemuserid' intersect='true' >
                                          <link-entity name='role' from='roleid' to='roleid' >
                                            <attribute name='name' alias='roleName' />
                                          </link-entity>
                                        </link-entity>
                                      </entity>
                                    </fetch>";

            EntityCollection userRoleRes = OrganizationService.RetrieveMultiple(new FetchExpression(fetchUserRoles));
            TracingService.Trace($"{userRoleRes.Entities.Count} roles found");

            if (userRoleRes.Entities.Count >= 1)
            {
                foreach (var role in userRoleRes.Entities)
                {
                    TracingService.Trace("Printing Roles");
                    var roleName = (role.GetAttributeValue<AliasedValue>("roleName")).Value.ToString();
                    TracingService.Trace(roleName);

                    if (roleName == "VASS Administrator")
                    {
                        TracingService.Trace("VASS Admin role found");
                        isVASSAdmin = true;
                        break;
                    }
                }
            }


            if (!isVASSAdmin)
            {
                if (PluginExecutionContext.InputParameters.Contains("Target") && PluginExecutionContext.InputParameters["Target"] is Entity)
                {
                    Entity entity = (Entity)PluginExecutionContext.InputParameters["Target"];
                    AttributeCollection coll = entity.Attributes;

                    Guid createdBy = ((EntityReference)coll["createdby"]).Id;
                    Guid interactionId = ((EntityReference)coll["udo_interactionid"]).Id;
                    var disposition = ((OptionSetValue)coll["udo_disposition"]).Value;
                    var direction = ((OptionSetValue)coll["udo_direction"]).Value;
                    DateTime createdOn = (DateTime)entity["createdon"];

                    TracingService.Trace($"Current call attempt createdon value - {createdOn}");

                    //unlimited call attempts when disposition is CallBackRequest
                    if (disposition != 752280004)
                    {
                        TracingService.Trace("Fetching Call Attempts");

                        var fetchCallAttempts = @"<fetch aggregate='true' >
                                              <entity name='udo_outboundcallattempt' >
                                                <attribute name='udo_interactionid' alias='interaction_count' aggregate='count' />
                                                <filter type='and' >
                                                  <condition attribute='createdby' operator='eq' value='" + createdBy + @"' />
                                                  <condition attribute='udo_interactionid' operator='eq' value='" + interactionId + @"' />
                                                  <condition attribute='udo_disposition' operator='eq' value='" + disposition + @"' />
                                                  <condition attribute='udo_direction' operator='eq' value='" + direction + @"' />
                                                  <condition attribute='createdon' operator='today' />
                                                  <condition attribute='statecode' operator='eq' value='0' />
                                                </filter>
                                              </entity>
                                            </fetch>";

                        EntityCollection callAttemptsRes = OrganizationService.RetrieveMultiple(new FetchExpression(fetchCallAttempts));
                        int interactionCountFetch = 0;

                        foreach (var c in callAttemptsRes.Entities)
                        {
                            interactionCountFetch = (int)((AliasedValue)c["interaction_count"]).Value;
                        }

                        TracingService.Trace($"Aggregate column 'interaction_count' value is {interactionCountFetch}");

                        //var tuple = new Tuple<int, int>(disposition, direction);

                        //var noOfAllowedAttempts = dict[tuple].Item1;
                        //var message = dict[tuple].Item2;

                        //Getting Number of allowed attempts and Messages from the Combination class
                        CombinationOutput output = new CombinationOutput();
                        var result = output.GetAttemptsMessages(disposition, direction);
                        var noOfAllowedAttempts = result.AllowedCallAttempt;
                        var message = result.Message;


                        if (interactionCountFetch >= noOfAllowedAttempts)
                        {
                            TracingService.Trace($"Found {callAttemptsRes.Entities.Count} call attempts for today's date");
                            throw new InvalidPluginExecutionException(message);
                        }
                        else
                        {
                            TracingService.Trace("Found less than 1 call attempt created today for same disposition on same interaction by same agent");
                            //create call attempt
                        }

                    }
                }
            }
        }
    }
}
