using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.Plugins
{
    public class RestrictOutOfScopeAssignmentRunner : PluginRunner
    {
        private IServiceProvider serviceProvider;

        public RestrictOutOfScopeAssignmentRunner(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Execute(string teamName)
        {
            TracingService.Trace("Team : " + teamName);
            if (PluginExecutionContext.InputParameters.Contains("Target") && PluginExecutionContext.InputParameters["Target"] is Entity)
            {

                TracingService.Trace($"Usecure String: {teamName}");
                Entity entity = (Entity)PluginExecutionContext.InputParameters["Target"];

                Entity preImage = (Entity)PluginExecutionContext.PreEntityImages["InteractionPreEntityImage"];
                Entity postImage = (Entity)PluginExecutionContext.PostEntityImages["InteractionPostEntityImage"];
                TracingService.Trace("Pre Image Obtained");

                if (preImage.Attributes.Contains("udo_channel"))
                {
                    TracingService.Trace("Checking udo_channel");
                    OptionSetValue channelValue = new OptionSetValue();
                    channelValue = preImage.GetAttributeValue<OptionSetValue>("udo_channel");
                    TracingService.Trace($"Channel is {channelValue.Value}");
                    if (channelValue.Value != (int)Channel.VASS)
                    {
                        TracingService.Trace("No VASS Channel");
                        return;
                    }
                    else
                    {
                        var owner = ((EntityReference)postImage.Attributes["ownerid"]).Name;
                        TracingService.Trace("Post Owner is " + owner);

                        if (owner == teamName)
                        {

                            var isVASSAdmin = false;
                            EntityCollection userRoleRes;

                            TracingService.Trace($"Fetching security roles for user {PluginExecutionContext.InitiatingUserId}");
                            try
                            {
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

                                userRoleRes = OrganizationService.RetrieveMultiple(new FetchExpression(fetchUserRoles));
                                TracingService.Trace($"{userRoleRes.Entities.Count} roles found");
                            }
                            catch (Exception ex)
                            {
                                TracingService.Trace(ex.Message);
                                return;
                            }   
                            

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
                                throw new InvalidPluginExecutionException($"You do not have permission to assign to '{teamName}' team.");
                            }
                        }
                    }
                }
            }
        }
    }
}
