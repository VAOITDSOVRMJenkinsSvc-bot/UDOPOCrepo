using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using System.Diagnostics;
using Microsoft.Xrm.Sdk.Messages;
using MCSHelperClass;
//using UDOUserToolPlugin.Helpers;

namespace Va.Udo.Crm.UserTool.Plugins
{
    internal class UDOSecurityPostUpdateRunner : PluginRunner
    {
        private bool _debug { get; set; }
        public UDOSecurityPostUpdateRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }
        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.PreEntityImages["pre"] as Entity;
        }
        public override Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }
        public override string McsSettingsDebugField
        {
            get { return "udo_usertool"; }
        }
        internal void Execute(IServiceProvider serviceProvider)
        {
            _debug = false;
            try
            {
                //Extract the tracing service for use in plug-in debugging.
                ITracingService tracingService =
                    (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                tracingService.Trace("Execute UDOSecurityPostUpdateRunner");

                // Don't run this inside other plugins
                if (PluginExecutionContext.Depth > 1) return;

                #region Debug
                var debug = McsSettings.getDebug;
                #endregion

                Stopwatch txnTimer = Stopwatch.StartNew();

                var target = GetSecondaryEntity();
                if (!target.Contains("udo_action")) return;
                
                var targetRef = target.ToEntityReference();
                var request = new OrganizationRequest();

                var action = (OptionSetValue)target["udo_action"];
                if (action.Value == 752280010 || action.Value==752280020) {

                    var update = new Entity(target.LogicalName);
                    update["udo_action"] = new OptionSetValue(action.Value+1);
                    update.Id = target.Id;
                    OrganizationService.Update(update);

                    var users = GetAllUsers();
                    var relationshipName = String.Format("udo_{0}_systemuser", target.LogicalName.ToLowerInvariant());
                    var relationship = new Relationship(relationshipName);
                    if (action.Value == 752280010) {
                        var fetchRelatedUsers = @"<fetch><entity name='" + relationshipName + "'><attribute name='systemuserid'/>" +
                                                 "<filter><condition attribute='" + target.LogicalName.ToLowerInvariant() + "id' operator='eq' value='" + target.Id + "'/></filter>" +
                                                 "</entity></fetch>";
                        var relatedUsersResponse = OrganizationService.RetrieveMultiple(new FetchExpression(fetchRelatedUsers));
                        if (relatedUsersResponse != null && relatedUsersResponse.Entities.Count > 0)
                        {
                            foreach (var relatedUser in relatedUsersResponse.Entities)
                            {
                                users.Remove(new EntityReference("systemuser", (Guid)relatedUser["systemuserid"]));
                            }
                        }

                        request = new AssociateRequest
                        {
                            RelatedEntities = users,
                            Relationship = relationship,
                            Target = targetRef
                        };
                    } else {
                        request = new DisassociateRequest
                        {
                            RelatedEntities = users,
                            Relationship = relationship,
                            Target = targetRef
                        };
                    }

                    #region Execute & Update Status
                    var result = OrganizationService.Execute(request);

                    update["udo_action"] = new OptionSetValue(752280999);
                    OrganizationService.Update(update);
                    #endregion
                }
                
                txnTimer.Stop();
                //Logger.WriteTxnTimingMessage("UDO_SecurityGroup.Update", txnTimer.ElapsedMilliseconds);

            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                //Logger.WriteException(ex);
                TracingService.Trace(ex.Message + ex.StackTrace);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
            catch (Exception ex)
            {
                //Logger.WriteException(ex);
                TracingService.Trace(ex.Message + ex.StackTrace);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }

        }

        private EntityReferenceCollection GetAllUsers()
        {
            var usersFetch = "<fetch><entity name='systemuser'><attribute name='systemuserid'/>" +
                "<filter type='and'><condition attribute='isdisabled' operator='eq' value='false'/></filter>" +
                "</entity></fetch>";

            var usersResult = Fetch(usersFetch, "All Users");

            if (usersResult == null) return null;

            if (usersResult.Entities.Count() == 0) return null;

            return new EntityReferenceCollection(usersResult.Entities.Select(u => u.ToEntityReference()).ToList());
        }

        private EntityCollection Fetch(string fetch, string description = "")
        {
            if (_debug)
            {
                TracingService.Trace("Fetching {0}... \r\n", description);
                TracingService.Trace("Query: {0}\r\n", fetch);
            }
            return OrganizationService.RetrieveMultiple(new FetchExpression(fetch));
        }


    }

}