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
    public class UDOSecurityAssocProcessor
    {
        
        private bool _debug { get; set; }
        private string progressString = string.Empty;
        private const string method = "UDOSecurityAssocProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOSecurityAssocRequest request)
        {
            _debug = request.Debug;
            var response = new UDOSecurityAssocResponse();
            
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
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                response.ExceptionMessage = "Failed to process Security Association";
                response.ExceptionOccured = true;
                return response;
            }

            return response;
        }

        private void DoWork(OrganizationServiceProxy orgService, UDOSecurityAssocRequest request)
        {
            try
            {
                List<SecurityAssociation> associations = new List<SecurityAssociation>();

                #region udo_securitygroup, udo_securityrole association and disassociation
                progressString = "Handling User Add/Removes to groups, roles";
                if (request.Relationship == "udo_udo_securityrole_systemuser" ||
                    request.Relationship == "udo_udo_securitygroup_systemuser")
                {
                    if (request.One.LogicalName == "systemuser")
                    {
                        AddSecurityAccess(orgService, associations, request.One, request.Many);
                    }
                    else
                    {
                        AddUsers(orgService, associations, request.One, request.Many);
                    }
                }
                #endregion

                #region Handle security group changes- add/remove role, update attached users
                progressString = "Handling changes to Security Group configuration (add roles)";
                if (request.Relationship == "udo_udo_securitygroup_udo_securityrole")
                {
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
                        AddSecurityAccess(orgService, associations, secUser, secItems);
                    }
                }
                #endregion

                #region Handle association security group and queue
                if (request.Relationship == "udo_udo_securitygroup_queue" ||
                    request.Relationship == "udo_udo_securitygroup_systemuser")
                {
                    List<EntityReference> Groups = new List<EntityReference>();
                    List<EntityReference> Queues = new List<EntityReference>();

                    if (request.One.LogicalName == "udo_securitygroup") { Groups.Add(request.One); }
                    else { Groups.AddRange(request.Many); }

                    var Users = GetSecurityGroupUsers(orgService, request.One.Id);

                    Queues.AddRange(GetSecurityGroupQueues(Groups, orgService, request));
                                        
                    if (_debug) Log("Related: QueueMembership");
                    if (_debug) Log("Users: {0}", Users.Count());

                    AddQueueMembership(orgService, Users, Queues, request);

                    //end LOB execution if relationship is to queue
                    if (request.Relationship == "udo_udo_securitygroup_queue") return;
                }
                #endregion
                
                #region Process best way to commit associations
                progressString = "Building RequestCollection";
                var requests = new OrganizationRequestCollection();
                AddAssociations(requests, "systemuserroles_association", "role", associations);
                #endregion

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
                    //response.ExceptionMessage = result.FriendlyDetail;
                    //response.ExceptionOccured = true;
                    //return response;
                    //throw new Exception(result.FriendlyDetail);
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ex);
                LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
            }
        }

        private void AddAssociations(OrganizationRequestCollection requests, string relationship, string entityName, IEnumerable<SecurityAssociation> associations)
        {
            var section = associations.Where(o => o.RelatedObject.LogicalName == entityName);
            var crmRelationship = new Relationship(relationship);
            switch (DetermineAssociationMethod(section))
            {
                case AssociationMethods.ObjectToUsers:
                    var objectIds = section.Select(o => o.RelatedObject.Id).Distinct();
                    foreach (var id in objectIds)
                    {
                        var related = section.Where(o => o.RelatedObject.Id == id).Select(o => new EntityReference("systemuser", o.UserId));
                        var target = new EntityReference(entityName, id);
                        while (related.Count() > 0)
                        {
                            var ar = new AssociateRequest();
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
                            var ar = new AssociateRequest();
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


        internal void AddUsers(OrganizationServiceProxy orgService, List<SecurityAssociation> associations, EntityReference securityRecord, IEnumerable<EntityReference> users)
        {
            // It would be ideal to also traverse this from the user side...

            foreach (var user in users)
            {
                AddSecurityAccess(orgService, associations, user, new[] {securityRecord});
            }
        }

        internal void AddSecurityAccess(OrganizationServiceProxy orgService, List<SecurityAssociation> associations, EntityReference user,
           IEnumerable<EntityReference> securityRecords)
        {
            string fetch = string.Empty;
            EntityCollection results = null;
            if (securityRecords.Count() == 0) return;

            #region Assign Security Role
            var roles = securityRecords.Where(r => r.LogicalName == "udo_securityrole");
            if (roles != null && roles.Count() > 0)
            {
                // While complex, this fetch expression does a couple of things... commented as Parts A B and C
                fetch = "<fetch><entity name='role'><attribute name='roleid'/>" +
                        // Part A - Match any role that has the same name as the role linked to the securityrole
                        "<link-entity name='role' from='name' to='name'>" +
                        "<link-entity name='udo_securityrole' from='udo_role' to='roleid'>" +
                        "<filter type='and'>" +
                            "<condition attribute='udo_securityroleid' operator='in'>" +
                                String.Join("", roles.Select(r => String.Format("<value>{0}</value>", r.Id))) +
                            "</condition>" +
                        "</filter></link-entity></link-entity>" +
                        // Part B - Filter it by roles that are in the users business unit only..
                        "<link-entity name='businessunit' from='businessunitid' to='businessunitid' alias='pbu'>" +
                        "<link-entity name='systemuser' from='businessunitid' to='businessunitid' alias='psu'>" +
                        "<filter type='and'>" +
                            "<condition attribute='systemuserid' operator='eq' value='" + user.Id + "' />" +
                        "</filter>" +
                        "</link-entity></link-entity>" +
                        // Part C - Exclude all roles the user already has...
                        "<link-entity name='systemuserroles' from='roleid' to='roleid' link-type='outer' alias='sur'><filter type='and'>"+
                            "<condition attribute='systemuserid' operator='eq' value='" + user.Id + "' />" +
                        "</filter></link-entity><filter type='and'>" +
                            "<condition entityname='sur' attribute='roleid' operator='null'/>"+
                        "</filter>" +
                        "</entity></fetch>";

                results = Fetch(orgService, _debug, fetch, "UDO_SecurityRoles"); 

                if (results!=null && results.Entities.Count()>0) {

                    associations.AddRange(results.Entities.Select(a=>new SecurityAssociation(){ 
                            RelatedObject = a.ToEntityReference(),
                            UserId = user.Id
                        }));
                }

                if (_debug)
                {
                    Log("Assign {0} Security Roles.", results.Entities.Count());
                    results.Entities.ToList().ForEach(a=>Log("Role: {0}  User: {1}",a.Id,user.Id));
                }
            }
            #endregion
            
            #region Assign Security Group Roles
            var groups = securityRecords.Where(r => r.LogicalName == "udo_securitygroup");
            if (groups != null && groups.Count() > 0)
            {
                var securityroles = new List<EntityReference>();

                #region Get Security Group Roles
                fetch = "<fetch><entity name='udo_udo_securitygroup_udo_securityrole'>" +
                        "<attribute name='udo_securityroleid'/>" +
                        "<filter type='and'>" +
                        "<condition attribute='udo_securitygroupid' operator='in'>" +
                        String.Join("", groups.Select(r => String.Format("<value>{0}</value>", r.Id))) +
                        "</condition>" +
                        "</filter></entity></fetch>";
                results = Fetch(orgService, _debug, fetch, "group security roles");

                if (results != null && results.Entities.Count() > 0)
                {
                    securityroles.AddRange(results.Entities.Select(r => new EntityReference("udo_securityrole", (Guid)r["udo_securityroleid"])));
                }
                #endregion

                if (securityroles.Count() > 0)
                {
                    AddSecurityAccess(orgService, associations, user, securityroles);
                }
            }
            #endregion
        }
        
        internal void AddQueueMembership(OrganizationServiceProxy orgService, IEnumerable<EntityReference> users, 
            IEnumerable<EntityReference> queueRecords, UDOSecurityAssocRequest request)
        {
            var _debug = request.Debug;
            progressString = "Before addPrincipalToQueueRequest";
            if (users == null || users.Count() == 0 || queueRecords == null || queueRecords.Count() == 0) 
            {
                Log("Null or empty list of users or queues during queue membership assignment"); 
                return;
            }
            try
            {
                if (_debug) Log("Preparing to create Queue Memberships");

                var requests = new OrganizationRequestCollection();
                if (_debug) Log("{0} users in group", users.Count());
                if (_debug) Log("{0} queues in group", queueRecords.Count());
                foreach (var user in users)
                {
                    var thisUser = orgService.Retrieve("systemuser", user.Id, new ColumnSet());
                    foreach (var queue in queueRecords)
                    {
                        var addPrincipalToQueueRequest = new OrganizationRequest();

                        addPrincipalToQueueRequest.RequestName = "AddPrincipalToQueue";
                        addPrincipalToQueueRequest.Parameters.Add("Principal", thisUser);
                        addPrincipalToQueueRequest.Parameters.Add("QueueId", queue.Id);

                        requests.Add(addPrincipalToQueueRequest);
                    }
                }

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
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ex);
                LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
            }
        }

        private IEnumerable<EntityReference> GetSecurityGroupQueues(IEnumerable<EntityReference> securityGroups, 
            OrganizationServiceProxy orgService, UDOSecurityAssocRequest request)
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

                if(_debug) Log("{0} queues retrieved form association to security group", queueResults.Entities.Count());
                progressString = "after fetching queues";

                if (queueResults == null || queueResults.Entities.Count() == 0) { return new List<EntityReference>(); }
                else return queueResults.Entities.Select(i => new EntityReference("queue", (Guid)i["queueid"]));
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ex);
                LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                return new List<EntityReference>();
            }
        }

        private EntityCollection Fetch(OrganizationServiceProxy orgService, bool debug, string fetch, string description = "")
        {
            if (debug)
            {
                Log("Fetching {0}... \r\n", description);
                Log("Query: {0}\r\n", fetch);
            }
            return orgService.RetrieveMultiple(new FetchExpression(fetch));
        }

        private void Log(string format, params object[] vars)
        {
            if (_debug) LogBuffer += String.Format(format+"\r\n", vars);
        }
    }
}
