using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.Plugins
{
    public class TeamAssignmentUsingUpdateRunner : PluginRunner
    {
        private IServiceProvider serviceProvider;

        public TeamAssignmentUsingUpdateRunner(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Execute()
        {
            try
            {
                if (PluginExecutionContext.InputParameters.Contains("Target") && PluginExecutionContext.InputParameters["Target"] is Entity)
                {

                    //Get Entity Information 
                    Entity entity = (Entity)PluginExecutionContext.InputParameters["Target"];
                    //get PreImage 
                    Entity preImageofInteraction = (Entity)PluginExecutionContext.PreEntityImages["InteractionPreEntityImage"];

                    Entity PostImageofInteraction = (Entity)PluginExecutionContext.PostEntityImages["InteractionPostEntityImage"];
                    TracingService.Trace("Images Obtained");

                    if (preImageofInteraction.Attributes.Contains("udo_channel"))
	                {
                        OptionSetValue channelValue = new OptionSetValue();
                        channelValue = preImageofInteraction.GetAttributeValue<OptionSetValue>("udo_channel");
                        if (channelValue.Value != (int)Channel.VASS)
	                    {
                            return;
	                    }
	                }
                    EntityReference assignedFromPersonorTeam = new EntityReference();
                    if (preImageofInteraction.Attributes.Contains("ownerid"))
                    {
                        assignedFromPersonorTeam = preImageofInteraction.GetAttributeValue<EntityReference>("ownerid");
                    }
                    EntityReference assignedToPersonorTeam = new EntityReference();
                    if (PostImageofInteraction.Attributes.Contains("ownerid"))
                    {
                        assignedToPersonorTeam = PostImageofInteraction.GetAttributeValue<EntityReference>("ownerid");
                    }

                    EntityReference assignedBy = new EntityReference();
                    if (PostImageofInteraction.Attributes.Contains("modifiedby"))
                    {
                        assignedBy = preImageofInteraction.GetAttributeValue<EntityReference>("modifiedby");
                    }
                    OptionSetValue reassignmentReason = new OptionSetValue();
                    if (preImageofInteraction.Attributes.Contains("udo_reassignmentreasonnew"))
                    {
                        reassignmentReason = preImageofInteraction.GetAttributeValue<OptionSetValue>("udo_reassignmentreasonnew");
                        TracingService.Trace($"Reached here ---- values assigned reasignment reason is {reassignmentReason.Value}");
                    }
                    TracingService.Trace("Reached here ---- values assigned");

                    DateTime reassignedTime = DateTime.Now;
                    if (PostImageofInteraction.Attributes.Contains("modifiedon"))
                    {
                        reassignedTime = PostImageofInteraction.GetAttributeValue<DateTime>("modifiedon");
                    }

                    Entity assignmentLog = new Entity("udo_assignmentlog");
                    TracingService.Trace("Object Created");
                    if (assignedToPersonorTeam.LogicalName == "team")
                    {
                        assignmentLog["udo_assignedtoteam"] = assignedToPersonorTeam;
                        assignmentLog["udo_touserorteam"] = assignedToPersonorTeam.Name + " (Team)";
                       
                    }
                    else
                    {
                        assignmentLog["udo_assignedto"] = assignedToPersonorTeam;
                        assignmentLog["udo_touserorteam"] = assignedToPersonorTeam.Name + " (User)";
                    }
                    if (assignedFromPersonorTeam.LogicalName == "team")
                    {
                        assignmentLog["udo_assignedfromteam"] = assignedFromPersonorTeam;
                        assignmentLog["udo_fromuserorteam"] = assignedFromPersonorTeam.Name + " (Team)";
                        assignmentLog["ownerid"] = new EntityReference("systemuser",PluginExecutionContext.InitiatingUserId);
                        assignmentLog["udo_assignedby"] = new EntityReference("systemuser", PluginExecutionContext.InitiatingUserId);

                    }
                    else
                    {
                        assignmentLog["udo_assignedfrom"] = assignedFromPersonorTeam;
                        assignmentLog["udo_fromuserorteam"] = assignedFromPersonorTeam.Name + " (User)";
                        assignmentLog["ownerid"] = new EntityReference("systemuser", assignedFromPersonorTeam.Id);
                        assignmentLog["udo_assignedby"] = new EntityReference("systemuser", assignedFromPersonorTeam.Id);

                    }
                    assignmentLog["udo_assignedon"] = reassignedTime;
                    TracingService.Trace($"reasignment reason is {reassignmentReason.Value}");
                    if (!(reassignmentReason.Value == 0))
                    {
                        assignmentLog["udo_reassignmentreason"] = reassignmentReason;
                    }
                    if (PostImageofInteraction.Attributes.Contains("udo_interactionid"))
                    {
                        var interactionLu = PostImageofInteraction.GetAttributeValue<Guid>("udo_interactionid");
                        assignmentLog["udo_interaction"] = new EntityReference("udo_interaction", interactionLu);
                    }

                   
                    TracingService.Trace($"User ID : {PluginExecutionContext.InitiatingUserId}");
                    TracingService.Trace($"Initiating User ID : {PluginExecutionContext.UserId}");
                    TracingService.Trace($" {assignedFromPersonorTeam.LogicalName}");
                    var auditTrailId = OrganizationService.Create(assignmentLog); // handle the exception to tackle the async 
                    TracingService.Trace("Audit Trail Record Created");

                }
            }
            catch (Exception ex)
            {
                TracingService.Trace("Exception:" + ex.Message);
            }
        }
    }

    public enum Channel 
    {
        Phone=752280000,
        Chat=752280001,
        walkin = 752280002,
        InquiryCorrespondance = 752280003,
        VASS=752280004
    }
}
