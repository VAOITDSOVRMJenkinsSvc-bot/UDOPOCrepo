using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.Workflows
{
    public class UpdateFutureAdditionalInteractions : CodeActivity
    {
        [RequiredArgument]
        [Input("Interaction")]
        [ReferenceTarget("udo_interaction")]
        public InArgument<EntityReference> Interaction { get; set; }
        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                tracingService.Trace("Get the input fields.");
                Guid interactionGuid = Interaction.Get(executionContext).Id;

                tracingService.Trace($"Interaction Guid is - {interactionGuid}");

                tracingService.Trace("Fetching Triggering Interaction");

                var fetchTriggeringInteraction = $@"<fetch>
                                                      <entity name='udo_interaction' >
                                                        <attribute name='udo_country' />
                                                        <attribute name='udo_state' />
                                                        <attribute name='udo_vlcreferral' />
                                                        <attribute name='udo_timezonesuggested' />
                                                        <attribute name='udo_addressline1' />
                                                        <attribute name='udo_city' />
                                                        <attribute name='udo_addressline2' />
                                                        <attribute name='udo_veteranemail' />
                                                        <attribute name='udo_phonenumber2' />
                                                        <attribute name='udo_zipcode' />
                                                        <attribute name='udo_phonenumber' />
                                                        <attribute name='udo_vassadditionalintervalnumber' />
                                                        <attribute name='udo_interactionmheointerval' />
                                                        <attribute name='udo_mheointeractionassignmentid' />
                                                        <filter>
                                                          <condition attribute='udo_interactionid' operator='eq' value='{interactionGuid}' />
                                                        </filter>
                                                      </entity>
                                                    </fetch>";
                
                FetchExpression fetchExpression = new FetchExpression(fetchTriggeringInteraction);
                EntityCollection interactionColl = service.RetrieveMultiple(fetchExpression);

                string country = string.Empty;
                string state = string.Empty;
                bool vlcReferral = false;
                int timezoneSuggested = 0;
                string addressLine1 = string.Empty;
                string city = string.Empty;
                string addressLine2 = string.Empty;
                string veteranEmail = string.Empty;
                string phoneNumber = string.Empty;
                string phoneNumber2 = string.Empty;
                string zipcode = string.Empty;
                string additionalIntervalNumber = string.Empty;
                string interval = string.Empty;
                Guid interactionAssignmentId = new Guid();

                if (interactionColl.Entities.Count >= 1)
                {
                    tracingService.Trace("Found Triggering Interaction");
                    tracingService.Trace("Retrieving the updated values");

                    if(interactionColl.Entities[0].Contains("udo_country"))
                    {
                        country = interactionColl.Entities[0].Attributes["udo_country"].ToString();
                        tracingService.Trace($"Country is : {country}");
                    }
                    if (interactionColl.Entities[0].Contains("udo_state"))
                    {
                        state = interactionColl.Entities[0].Attributes["udo_state"].ToString();
                        tracingService.Trace($"State is : {state}");
                    }
                    if (interactionColl.Entities[0].Contains("udo_vlcreferral"))
                    {
                        vlcReferral = interactionColl.Entities[0].GetAttributeValue<bool>("udo_vlcreferral");
                        tracingService.Trace($"VCL Referral is : {vlcReferral}");
                    }
                    if (interactionColl.Entities[0].Contains("udo_timezonesuggested"))
                    {
                        timezoneSuggested = ((OptionSetValue)interactionColl.Entities[0]["udo_timezonesuggested"]).Value;
                        tracingService.Trace($"Timezone Suggested is : {timezoneSuggested}");
                    }
                    if (interactionColl.Entities[0].Contains("udo_addressline1"))
                    {
                        addressLine1 = interactionColl.Entities[0].Attributes["udo_addressline1"].ToString();
                        tracingService.Trace($"Address Line1 is : {addressLine1}");
                    }
                    if (interactionColl.Entities[0].Contains("udo_city"))
                    {
                        city = interactionColl.Entities[0].Attributes["udo_city"].ToString();
                        tracingService.Trace($"City is : {city}");
                    }
                    if (interactionColl.Entities[0].Contains("udo_addressline2"))
                    {
                        addressLine2 = interactionColl.Entities[0].Attributes["udo_addressline2"].ToString();
                        tracingService.Trace($"Address Line 2 is : {addressLine2}");
                    }
                    if (interactionColl.Entities[0].Contains("udo_veteranemail"))
                    {
                        veteranEmail = interactionColl.Entities[0].Attributes["udo_veteranemail"].ToString();
                        tracingService.Trace($"Veteran Email is : {veteranEmail}");
                    }
                    if (interactionColl.Entities[0].Contains("udo_phonenumber2"))
                    {
                        phoneNumber2 = interactionColl.Entities[0].Attributes["udo_phonenumber2"].ToString();
                        tracingService.Trace($"Phone Number 2 is : {phoneNumber2}");
                    }
                    if (interactionColl.Entities[0].Contains("udo_zipcode"))
                    {
                        zipcode = interactionColl.Entities[0].Attributes["udo_zipcode"].ToString();
                        tracingService.Trace($"Zipcode is : {zipcode}");
                    }
                    if (interactionColl.Entities[0].Contains("udo_phonenumber"))
                    {
                        phoneNumber = interactionColl.Entities[0].Attributes["udo_phonenumber"].ToString();
                        tracingService.Trace($"Phone Number is : {phoneNumber}");
                    }
                    if (interactionColl.Entities[0].Contains("udo_vassadditionalintervalnumber"))
                    {
                        additionalIntervalNumber = interactionColl.Entities[0].Attributes["udo_vassadditionalintervalnumber"].ToString();
                        tracingService.Trace($"Additional Interval Number is : {additionalIntervalNumber}");
                    }
                    if (interactionColl.Entities[0].Contains("udo_interactionmheointerval"))
                    {
                        interval = interactionColl.Entities[0].FormattedValues["udo_interactionmheointerval"].ToString();
                        tracingService.Trace($"Interval is : {interval}");
                    }
                    if (interactionColl.Entities[0].Contains("udo_mheointeractionassignmentid"))
                    {
                        interactionAssignmentId = ((EntityReference)interactionColl.Entities[0].Attributes["udo_mheointeractionassignmentid"]).Id;
                        tracingService.Trace($"Interaction Assignment Id is : {interactionAssignmentId}");
                    }
                }

                var fetchFutureAdditionalInteractions = string.Empty;
                if (interval == "VASS Additional")
                {
                    tracingService.Trace("Fetching all future additional interactions if workflow triggered from an additional interaction");

                    fetchFutureAdditionalInteractions = $@"<fetch top='50' >
                                                              <entity name='udo_interaction' >
                                                                <filter type='and' >
                                                                  <condition attribute='statecode' operator='eq' value='0' />
                                                                  <condition attribute='udo_interactionmheointerval' operator='eq' value='752280003' />
                                                                  <condition attribute='udo_mheointeractionassignmentid' operator='eq' value='{interactionAssignmentId}' />
                                                                  <condition attribute='udo_vassadditionalintervalnumber' operator='gt' value='{additionalIntervalNumber}' />
                                                                </filter>
                                                              </entity>
                                                            </fetch>";
                }
                else
                {
                    tracingService.Trace("Fetching all future additional interactions");
                    fetchFutureAdditionalInteractions = $@"<fetch top='50' >
                                                              <entity name='udo_interaction' >
                                                                <filter type='and' >
                                                                  <condition attribute='statecode' operator='eq' value='0' />
                                                                  <condition attribute='udo_interactionmheointerval' operator='eq' value='752280003' />
                                                                  <condition attribute='udo_mheointeractionassignmentid' operator='eq' value='{interactionAssignmentId}' />
                                                                </filter>
                                                              </entity>
                                                            </fetch>";
                }

                FetchExpression fetchExpressionAddl = new FetchExpression(fetchFutureAdditionalInteractions);
                EntityCollection additionalColl = service.RetrieveMultiple(fetchExpressionAddl);

                tracingService.Trace($"Found {additionalColl.Entities.Count} additional future interactions");

                if (additionalColl.Entities.Count >= 1)
                {
                    foreach (var additional in additionalColl.Entities)
                    {
                        tracingService.Trace("Updating fetched interactions with the updated values");
                        tracingService.Trace($"Guid of the additional interaction to be updated - {additional.GetAttributeValue<Guid>("udo_interactionid")}");
                        Entity updateAdditional = new Entity("udo_interaction", additional.GetAttributeValue<Guid>("udo_interactionid"));
                        
                        updateAdditional["udo_country"] = country;
                        updateAdditional["udo_state"] = state;
                        updateAdditional["udo_vlcreferral"] = vlcReferral;
                        updateAdditional["udo_timezonesuggested"] = new OptionSetValue(timezoneSuggested);
                        updateAdditional["udo_addressline1"] = addressLine1;
                        updateAdditional["udo_city"] = city;
                        updateAdditional["udo_addressline2"] = addressLine2;
                        updateAdditional["udo_veteranemail"] = veteranEmail;
                        updateAdditional["udo_phonenumber2"] = phoneNumber2;
                        updateAdditional["udo_zipcode"] = zipcode;
                        updateAdditional["udo_phonenumber"] = phoneNumber;
                        service.Update(updateAdditional);                        
                        
                    }
                    tracingService.Trace("Updated Interactions!");

                    tracingService.Trace("Updating interaction assignment with the updated values");
                    Entity updateInteractionAssignment = new Entity("udo_mheointeractionassignment", interactionAssignmentId);
                    updateInteractionAssignment["udo_state"] = state;
                    updateInteractionAssignment["udo_city"] = city;
                    updateInteractionAssignment["udo_phone1"] = phoneNumber2;
                    updateInteractionAssignment["udo_phone2"] = phoneNumber;
                    service.Update(updateInteractionAssignment);

                    tracingService.Trace("Updated Interaction Assignment!");
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace("Exception Occurred!");
                tracingService.Trace("Message: " + ex.Message);
                if (ex.InnerException != null)
                {
                    tracingService.Trace("Inner Exception: " + ex.InnerException);
                }
            }
        }
    }
}
