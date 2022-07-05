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

namespace Va.Udo.Crm.UserTool.Plugins
{
    internal class SecurityRolePreRetrieveMultipleRunner : PluginRunner
    {
        public SecurityRolePreRetrieveMultipleRunner(IServiceProvider serviceProvider)
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
        internal void Execute()
        {

            try
            {
                // Don't run this inside other plugins
                if (PluginExecutionContext.Depth > 1) return;

                var qe = PluginExecutionContext.InputParameters["Query"] as QueryExpression;
                
                // Don't run this for fetches
                if (qe == null) return;
                
                // Don't run this when there is no paging info or the page is not 1.
                if (qe.PageInfo != null && qe.PageInfo.PageNumber != 1) return;

                Stopwatch txnTimer = Stopwatch.StartNew();

                #region Get Roles
                var roleFetch = "<fetch><entity name='role'><attribute name='name'/><attribute name='roleid'/>" +
                                "<filter type='and'><condition attribute='parentroleid' operator='null'/></filter>" +
                                "</entity></fetch>";
                var roleResults = OrganizationService.RetrieveMultiple(new FetchExpression(roleFetch));

                
                if (roleResults == null || roleResults.Entities.Count == 0) return;

                List<Entity> roles = roleResults.Entities.ToList();

                roles.RemoveAll(a => !a.Contains("name")); // remove noname entities
                #endregion

                #region Get Security Roles
                if (roles.Count > 0)
                {

                    var securityroleFetch = "<fetch distinct='true'><entity name='role'><attribute name='name'/>" +
                                        "<link-entity name='udo_securityrole' to='roleid' from='udo_role'>" +
                                        "<filter type='and'><condition attribute='udo_securityroleid' operator='not-null'/></filter>" +
                                        "</link-entity>" +
                                        "<filter type='and'><condition attribute='parentroleid' operator='null'/></filter>" +
                                        "</entity></fetch>";

                    var securityroleResult = OrganizationService.RetrieveMultiple(new FetchExpression(securityroleFetch));
                    if (securityroleResult != null && securityroleResult.Entities.Count > 0)
                    {
                        // Remove Security Roles
                        foreach (var secRole in securityroleResult.Entities)
                        {
                            if (secRole.Contains("name") && roles.Count>0)
                            {
                                roles.RemoveAll(a =>
                                    String.Equals(a["name"].ToString(), secRole["name"].ToString(), StringComparison.InvariantCultureIgnoreCase));
                            }
                        }
                    }
                }
                #endregion

                #region Build Create Requests
                var requests = new OrganizationRequestCollection();

                while (roles.Count > 0)
                {
                    var role = roles[0];
                    if (!role.Contains("name"))
                    {
                        roles.Remove(role);
                        continue;
                    }
                    var create = new CreateRequest();
                    create.Target = new Entity("udo_securityrole");
                    create.Target["udo_name"] = role["name"];
                    create.Target["udo_role"] = new EntityReference("role", (Guid)role["roleid"]);  // to EntityReference?
                    requests.Add(create);
                    roles.RemoveAll(a => a["name"] == role["name"]);
                } ;
                #endregion

                #region Execute Multiple
                var result = MCSRequestHelpers.ExecuteMultipleHelper.ExecuteMultiple(OrganizationService, requests, McsSettings.getDebug);

                if (result.IsFaulted)
                {
                    //Logger.WriteDebugMessage(result.ErrorDetail);
                    TracingService.Trace(result.ErrorDetail);
                    throw new Exception(result.FriendlyDetail);
                }
                #endregion

                txnTimer.Stop();
                //Logger.WriteTxnTimingMessage("Add Security Roles", txnTimer.ElapsedMilliseconds);

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

    }

}