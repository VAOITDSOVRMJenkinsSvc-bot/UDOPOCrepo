using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.UserTool.Messages;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;

namespace VRM.Integration.UDO.UserTool.Processors
{
    public class UDOSecurityDisassocProcessor
    {

        private bool _debug { get; set; }

        private string progressString = string.Empty;
        private const string method = "UDOSecurityDisassocProcessor";
        private string LogBuffer { get; set; }

        private Dictionary<Guid, IEnumerable<Guid>> userRoles { get; set; }

        public IMessageBase Execute(UDOSecurityDisassocRequest request)
        {
            _debug = request.Debug;

            var response = new UDOSecurityDisassocResponse();
            userRoles = new Dictionary<Guid, IEnumerable<Guid>>();

            #region get OrgServiceProxy
            OrganizationServiceProxy OrgServiceProxy;
            progressString = "Getting CRM Conncection";
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);
                // Call on behalf of executing user not the service account.
                OrgServiceProxy.CallerId = request.UserId;
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, string.Format("{0} Processor, Connection Error", method), connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion


            // Clear log buffer
            LogBuffer = string.Empty;
            
            try
            {
                Task.Run(() => DoWork(OrgServiceProxy, request));
            }
            catch (Exception ex)
            {
                LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                response.ExceptionMessage = ex.Message;
                response.ExceptionOccured = true;
                return response;
            }

            return response;
        }

        private void DoWork(OrganizationServiceProxy orgService, UDOSecurityDisassocRequest request)
        {
            try
            {
                List<SecurityAssociation> associations = new List<SecurityAssociation>();

                #region udo_securitygroup, udo_securityrole association and disassociation
                progressString = "Handling User Add/Removes to groups, roles, queues";
                if (request.Relationship == "udo_udo_securityrole_systemuser" ||
                    request.Relationship == "udo_udo_securitygroup_systemuser")
                {

                    if (request.One.LogicalName == "systemuser")
                    {
                        var roles = GetAssignedRoles(orgService, request.One);
                        RemoveSecurityAccess(orgService, associations, request.One, request.Many, roles);
                    }
                    else
                    {
                        RemoveUsers(orgService, associations, request.One, request.Many);
                    }
                }
                #endregion

                #region disassociate from queue - remove membership for all users
                if (request.Relationship == "udo_udo_securitygroup_queue")
                {
                    List<EntityReference> Groups = new List<EntityReference>();
                    List<EntityReference> Queues = new List<EntityReference>();

                    if (request.One.LogicalName == "udo_securitygroup") 
                        { Groups.Add(request.One); Queues.AddRange(request.Many); }
                    else { Groups.AddRange(request.Many); Queues.Add(request.One); }

                    var Users = GetSecurityGroupUsers(orgService, request.One.Id);

                    if (_debug) Log("Related: QueueMembership");
                    if (_debug) Log("Users: {0}", Users.Count());

                    RemoveQueueMembership(orgService, Users, Queues, request);

                    //end LOB execution if relationship is to queue - no additional logic required
                    return;
                }
                #endregion

                #region disassociate from user - remove membership to all queues
                if (request.Relationship == "udo_udo_securitygroup_systemuser")
                {
                    List<EntityReference> Groups = new List<EntityReference>();
                    List<EntityReference> Queues = new List<EntityReference>();
                    List<EntityReference> Users = new List<EntityReference>();

                    if (request.One.LogicalName == "udo_securitygroup")
                        { Groups.Add(request.One); Users.AddRange(request.Many); }
                    else { Groups.AddRange(request.Many); Users.Add(request.One);}

                    Queues.AddRange(GetSecurityGroupQueues(Groups, orgService, request));

                    if (_debug) Log("Related: QueueMembership");
                    if (_debug) Log("Users: {0}", Users.Count());

                    RemoveQueueMembership(orgService, Users, Queues, request);
                }
                #endregion

                #region Handle security group changes- add/remove role, update attached users
                
                if (request.Relationship == "udo_udo_securitygroup_udo_securityrole")
                {
                    progressString = "Handling changes to Security Group configuration (add roles)";
                    var users = new List<EntityReference>();
                    var secGroups = new List<EntityReference>();
                    var secItems = new List<EntityReference>();

                    if (request.One.LogicalName == "udo_securitygroup")
                    {
                        Log("Target: SecurityGroup");
                        users.AddRange(GetSecurityGroupUsers(orgService, request.One.Id));
                        Log("Users: {0}", users.Count());
                        secGroups.Add(request.One);
                        secItems.AddRange(request.Many);
                    }
                    else
                    {
                        Log("Related: SecurityGroup");
                        secGroups.AddRange(request.Many);
                        foreach (var rel in request.Many)
                        {
                            users.AddRange(GetSecurityGroupUsers(orgService, rel.Id));
                        }
                        Log("Users: {0}", users.Count());
                        secItems.Add(request.One);
                    }
                    foreach (var secUser in users)
                    {
                        RemoveSecurityAccess(orgService, associations, secUser, secItems,
                            GetAssignedRoles(orgService, secUser));
                    }
                }
                #endregion

                #region Process best way to commit associations
                progressString = "Building RequestCollection";
                var requests = new OrganizationRequestCollection();
                AddDisassociations(requests, "systemuserroles_association", "role", associations);
                #endregion

                #region ExecuteMultiple
                progressString = "Before Execute Multiple";
                if (_debug) Log("Preparing to execute multiple... Performing {0} actions.", requests.Count());

                var result = ExecuteMultipleHelper.ExecuteMultiple(orgService, requests, request.OrganizationName,
                    request.UserId, _debug);

                if (_debug)
                {
                    LogBuffer += result.LogDetail;
                    LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                }

                if (result.IsFaulted)
                {
                    progressString = "After Execute Multiple";
                    LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                    return;
                    //response.ExceptionMessage = result.FriendlyDetail;
                    //response.ExceptionOccured = true;
                    //return response;
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, method, ex);
                LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
            }
        }

        private void AddDisassociations(OrganizationRequestCollection requests, string relationship, string entityName, IEnumerable<SecurityAssociation> associations)
        {
            var section = associations.Where(o => o.RelatedObject.LogicalName == entityName);
            var crmRelationship = new Relationship(relationship);
            switch (DetermineAssociationMethod(section))
            {
                case AssociationMethods.ObjectToUsers:
                    var objectIds = section.Select(o => o.RelatedObject.Id).Distinct();
                    foreach (var id in objectIds)
                    {
                        var target = new EntityReference(entityName, id);
                        var related = section.Where(o => o.RelatedObject.Id == id).Select(o => new EntityReference("systemuser", o.UserId));    
                        while (related.Count() > 0)
                        {
                            var ar = new DisassociateRequest();
                            ar.RelatedEntities = new EntityReferenceCollection(related.Take(500).ToList());
                            ar.Relationship = crmRelationship;
                            ar.Target = target;
                            requests.Add(ar);
                            related = related.Skip(500);
                        }
                    }
                    break;
                case AssociationMethods.UserToObjects:
                    var userIds = section.Select(o => o.UserId).Distinct();
                    foreach (var id in userIds)
                    {
                        var related = section.Where(o => o.UserId == id).Select(o => o.RelatedObject);
                        var target = new EntityReference("systemuser", id);
                        while (related.Count() > 0)
                        {
                            var ar = new DisassociateRequest();
                            ar.RelatedEntities = new EntityReferenceCollection(related.Take(500).ToList());
                            ar.Relationship = crmRelationship;
                            ar.Target = target;
                            requests.Add(ar);
                            related = related.Skip(500);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        internal enum AssociationMethods
        {
            ObjectToUsers,
            UserToObjects,
            NothingToAssociate
        };

        private static AssociationMethods DetermineAssociationMethod(IEnumerable<SecurityAssociation> associations)
        {
            if (associations == null || associations.Count() == 0) return AssociationMethods.NothingToAssociate;
            var objects = associations.Select(o => o.RelatedObject.Id).Distinct().Count();
            var users = associations.Select(o => o.UserId).Distinct().Count();
            if (objects < users) return AssociationMethods.ObjectToUsers;
            return AssociationMethods.UserToObjects;
        }

        internal IEnumerable<EntityReference> GetSecurityGroupUsers(OrganizationServiceProxy orgService, Guid securityGroupId)
        {
            var userFetch = "<fetch><entity name='udo_udo_securitygroup_systemuser'>" +
                            "<attribute name='systemuserid'/>" +
                            "<filter type='and'>" +
                            "<condition attribute='udo_securitygroupid' operator='eq' value='" + securityGroupId + "'/>" +
                            "</filter></entity></fetch>";

            var userResutls = Fetch(orgService, _debug, userFetch, "security group users");

            Log("Users: {0}", userResutls.Entities.Count());
            if (userResutls == null || userResutls.Entities.Count() == 0) return new List<EntityReference>();

            return userResutls.Entities.Select(u => new EntityReference("systemuser", (Guid)u["systemuserid"]));
        }


        internal void RemoveUsers(OrganizationServiceProxy orgService, List<SecurityAssociation> associations, EntityReference securityRecord, IEnumerable<EntityReference> users)
        {
            // It would be ideal to also traverse this from the user side...

            foreach (var user in users)
            {
                var roles = GetAssignedRoles(orgService, user);
                RemoveSecurityAccess(orgService, associations, user, new[] { securityRecord }, roles);
            }
        }

        internal IEnumerable<Guid> GetAssignedRoles(OrganizationServiceProxy orgService, EntityReference user)
        {
            if (userRoles.ContainsKey(user.Id)) return userRoles[user.Id];
            var rolenames = new List<String>();

            var fetch = "<fetch><entity name='role'><attribute name='roleid'/>" +
                "<link-entity name='role' from='name' to='name'>" +
                "<link-entity name='udo_securityrole' from='udo_role' to='roleid'>" +
                // Directly Assigned
                "<link-entity name='udo_udo_securityrole_systemuser' from='udo_securityroleid' to='udo_securityroleid' link-type='outer' alias='dir'>" +
                "<filter type='and'>" +
                "<condition attribute='systemuserid' operator='eq' value='" + user.Id + "' />" +
                "</filter></link-entity>" +
                // Indirectly Assigned
                "<link-entity name='udo_udo_securitygroup_udo_securityrole' from='udo_securityroleid' to='udo_securityroleid' link-type='outer'>" +
                "<link-entity name='udo_udo_securitygroup_systemuser' from='udo_securitygroupid' to='udo_securitygroupid' link-type='outer' alias='indir'>" +
                "<filter type='and'>" +
                "<condition attribute='systemuserid' operator='eq' value='" + user.Id + "' />" +
                "</filter></link-entity></link-entity>" +
                "</link-entity></link-entity>" +
                "<filter type='or'>" +
                "<condition entityname='dir' attribute='systemuserid' operator='not-null'/>" +
                "<condition entityname='indir' attribute='systemuserid' operator='not-null'/>" +
                "</filter>" +
                "</entity></fetch>";

            var results = Fetch(orgService, _debug, fetch, "user roles");

            if (results == null || results.Entities.Count() == 0) return null;

            userRoles.Add(user.Id, results.Entities.Select(r => r.Id));
            return userRoles[user.Id];
        }

        internal void RemoveSecurityAccess(OrganizationServiceProxy orgService, List<SecurityAssociation> associations, EntityReference user,
           IEnumerable<EntityReference> securityRecords, IEnumerable<Guid> userRoles)
        {
            if (securityRecords.Count() == 0) return;

            var fetch = string.Empty;
            EntityCollection results = new EntityCollection();

            #region Remove Security Group Roles
            var groups = securityRecords.Where(r => r.LogicalName == "udo_securitygroup");
            if (groups != null && groups.Count() > 0)
            {
                var securityRoles = new List<EntityReference>();

                #region Get Security Group Roles
                fetch = "<fetch><entity name='udo_udo_securitygroup_udo_securityrole'>" +
                        "<attribute name='udo_securityroleid'/>" +
                        "<filter type='and'>" +
                        "<condition attribute='udo_securitygroupid' operator='in'>" +
                        String.Join("", groups.Select(r => String.Format("<value>{0}</value>", r.Id))) +
                        "</condition>" +
                        "</filter></entity></fetch>";
                results = Fetch(orgService, _debug, fetch, "roles");

                if (results != null && results.Entities.Count() > 0)
                {
                    securityRoles.AddRange(results.Entities.Select(r => new EntityReference("udo_securityrole", (Guid)r["udo_securityroleid"])));
                    RemoveSecurityAccess(orgService, associations, user, securityRoles, userRoles);
                }
                #endregion
            }
            #endregion

            #region Remove Security Role
            var roles = securityRecords.Where(r => r.LogicalName == "udo_securityrole");
            if (roles != null && roles.Count() > 0)
            {
                fetch = "<fetch><entity name='udo_securityrole'><attribute name='udo_role'/><filter type='and'>" +
                        "<condition attribute='udo_securityroleid' operator='in'>" +
                        String.Join("", roles.Select(r => String.Format("<value>{0}</value>", r.Id))) +
                        "<condition attribute='statecode' operator='eq' value='0' />" +
                        "</condition></filter></entity></fetch>";

                results = Fetch(orgService, _debug, fetch, "security roles");

                if (results != null && results.Entities.Count > 0)
                {
                    var roleNames = results.Entities
                        .Where(r => r.Contains("udo_role") && r["udo_role"] != null)
                        .Select(r => String.Format("<value>{0}</value>", ((EntityReference)r["udo_role"]).Name));

                    fetch = "<fetch><entity name='role'><attribute name='roleid'/>" +
                                "<link-entity name='businessunit' from='businessunitid' to='businessunitid' alias='pbu'>" +
                                "<link-entity name='systemuser' from='businessunitid' to='businessunitid' alias='psu'>" +
                                "<filter type='and'>" +
                                    "<condition attribute='systemuserid' operator='eq' value='" + user.Id + "' />" +
                                "</filter>" +
                                "</link-entity></link-entity>" +
                                "<filter type='and'>" +
                                    "<condition attribute='name' operator='in'>" +
                                        String.Join("", roleNames) +
                                    "</condition>" +
                                "</filter></entity></fetch>";

                    results = Fetch(orgService, _debug, fetch, "security roles part 2");

                    if (results != null && results.Entities.Count() > 0)
                    {
                        associations.AddRange(results.Entities.Select(r =>
                            new SecurityAssociation()
                            {
                                RelatedObject = r.ToEntityReference(),
                                UserId = user.Id
                            }));
                    }
                }
            }
            #endregion

        }

        internal void RemoveQueueMembership(OrganizationServiceProxy orgService, IEnumerable<EntityReference> users,
            IEnumerable<EntityReference> queueRecords, UDOSecurityDisassocRequest request)
        {
            var _debug = request.Debug;
            progressString = "Before revokeAccessRequest";
            if (users == null || users.Count() == 0 || queueRecords == null || queueRecords.Count() == 0)
            {
                Log("Null or empty list of users or queues during queue membership revokation");
                return;
            }
            try
            {
                if (_debug) Log("Preparing to revoke Queue Memberships");

                var requests = new OrganizationRequestCollection();
                if (_debug) Log("{0} users in group", users.Count());
                if (_debug) Log("{0} queues in group", queueRecords.Count());
                var relatedEntities = new EntityReferenceCollection();
                var target = new EntityReference();
                if (users.Count() == 1)
                {
                    target = new EntityReference("systemuser", users.FirstOrDefault().Id);
                    
                    foreach (var queue in queueRecords)
                    {
                        relatedEntities.Add(new EntityReference("queue", queue.Id));
                    }
                }
                else if (queueRecords.Count() == 1)
                {
                    target = new EntityReference("queue", queueRecords.FirstOrDefault().Id);
                    foreach (var user in users)
                    {
                        relatedEntities.Add(new EntityReference("queue", user.Id));
                    }
                }
                else
                {
                    var exceptionMessage = "Disassociate contains an unexpected number of parameters.";
                    throw new FormatException(exceptionMessage);
                }
                var disassociateMembershipRequest = new DisassociateRequest()
                {
                    Relationship= new Relationship("queuemembership_association"),
                    RelatedEntities= relatedEntities,
                    Target=target
                };

                requests.Add(disassociateMembershipRequest);

                #region ExecuteMultiple
                progressString = "Before Execute Multiple";
                //LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, "Executing Multiple:  \r\n\r\n"+LogBuffer); 
                if (_debug) Log("Preparing to execute multiple... Performing {0} actions.", requests.Count());

                //string exLog;
                var result = ExecuteMultipleHelper.ExecuteMultiple(orgService, requests, request.OrganizationName,
                    request.UserId, _debug);

                if (_debug)
                {
                    LogBuffer += result.LogDetail;
                    LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                }

                if (result.IsFaulted)
                {
                    progressString = "After Execute Multiple";
                    LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                    return;
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, method + " Processor, Progess:" + progressString, ex);
                LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
            }
        }

        private IEnumerable<EntityReference> GetSecurityGroupQueues(IEnumerable<EntityReference> securityGroups, OrganizationServiceProxy orgService, UDOSecurityDisassocRequest request)
        {
            var _debug = request.Debug;
            try
            {
                EntityCollection queueResults;

                progressString = "before fetching queues";
                var queueFetch = "<fetch><entity name='udo_udo_securitygroup_queue'>" +
                                "<attribute name='queueid'/>" +
                                "<filter type='and'>" +
                                "<condition attribute='udo_securitygroupid' operator='in'>" +
                                String.Join("", securityGroups.Select(r => String.Format("<value>{0}</value>", r.Id))) +
                                "</condition>" +
                                "</filter></entity></fetch>";
                queueResults = Fetch(orgService, _debug, queueFetch, "security group queues");

                if (_debug) Log("{0} queues retrieved form association to security group", queueResults.Entities.Count());
                progressString = "after fetching queues";

                if (queueResults == null || queueResults.Entities.Count() == 0) { return new List<EntityReference>(); }
                else return queueResults.Entities.Select(i => new EntityReference("queue", (Guid)i["queueid"]));
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, method + " Processor, Progess:" + progressString, ex);
                LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                return new List<EntityReference>();
            }
        }

        private EntityCollection Fetch(OrganizationServiceProxy orgService, bool _debug, string fetch, string description = "")
        {
            if (_debug)
            {
                Log("Fetching {0}... \r\n", description);
                Log("Query: {0}\r\n", fetch);
            }
            return orgService.RetrieveMultiple(new FetchExpression(fetch));
        }

        private void Log(string format, params object[] vars)
        {
            if (_debug) LogBuffer += String.Format(format + "\r\n", vars);
        }
    }
}
