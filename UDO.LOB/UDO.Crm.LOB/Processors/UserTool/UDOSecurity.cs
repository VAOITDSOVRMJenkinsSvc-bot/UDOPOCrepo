using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VRM.Integration.UDO.USerTool.Processors
{
    internal class UDOSecurity
    {

        private bool Debug {get;set;}
        private ITracingService TracingService { get; set; }
        private IOrganizationService OrganizationService { get; set; }

        internal UDOSecurity(IOrganizationService orgService, ITracingService tracer, bool debug)
        {
            Debug = debug;
            OrganizationService = orgService;
            TracingService = tracer;
        }

        internal IEnumerable<EntityReference> GetSecurityGroupUsers(Guid securityGroupId)
        {
            var userFetch = "<fetch><entity name='udo_udo_securitygroup_systemuser'>" +
                            "<attribute name='systemuserid'/>" +
                            "<filter type='and'>" +
                            "<condition attribute='udo_securitygroupid' operator='eq' value='" + securityGroupId + "'/>" +
                            "</filter></entity></fetch>";

            var userResutls = Fetch(userFetch);

            if (Debug) TracingService.Trace("Users: {0}", userResutls.Entities.Count());
            if (userResutls == null || userResutls.Entities.Count() == 0) return new List<EntityReference>();

            return userResutls.Entities.Select(u => new EntityReference("systemuser", (Guid)u["systemuserid"]));
        }

        internal void RemoveSecurityAccess(OrganizationRequestCollection requests, EntityReference user, IEnumerable<EntityReference> securityRecords, List<Guid> userRoles)
        {
            if (securityRecords.Count() == 0) return;

            #region Remove Security Group Roles
            var groups = securityRecords.Where(r => r.LogicalName == "udo_securitygroup");
            if (groups != null && groups.Count() > 0)
            {
                var securityRoles = new List<EntityReference>();

                #region Get Security Group Roles
                var groupRolesFetch = "<fetch><entity name='udo_udo_securitygroup_udo_securityrole'>" +
                                "<attribute name='udo_securityroleid'/>" +
                                "<filter type='and'>" +
                                "<condition attribute='udo_securitygroupid' operator='in'>" +
                                String.Join("", groups.Select(r => String.Format("<value>{0}</value>", r.Id))) +
                                "</condition>" +
                                "</filter></entity></fetch>";
                var groupRolesResults = Fetch(groupRolesFetch);

                if (groupRolesResults != null && groupRolesResults.Entities.Count() > 0)
                {
                    securityRoles.AddRange(groupRolesResults.Entities.Select(r => new EntityReference("udo_securityrole", (Guid)r["udo_securityroleid"])));
                    RemoveSecurityAccess(requests, user, securityRoles, userRoles);
                }
                #endregion
            }
            #endregion

            #region Remove Security Role
            var roles = securityRecords.Where(r => r.LogicalName == "udo_securityrole");
            if (roles != null && roles.Count() > 0)
            {
                var roleFetch = "<fetch><entity name='udo_securityrole'><attribute name='udo_role'/><filter type='and'>" +
                                "<condition attribute='udo_securityroleid' operator='in'>" +
                                String.Join("", roles.Select(r => String.Format("<value>{0}</value>", r.Id))) +
                                "<condition attribute='statecode' operator='eq' value='0' />" +
                                "</condition></filter></entity></fetch>";

                var roleResults = Fetch(roleFetch);

                if (roleResults != null && roleResults.Entities.Count > 0)
                {
                    var roleNames = roleResults.Entities
                        .Where(r => r.Contains("udo_role") && r["udo_role"] != null)
                        .Select(r => String.Format("<value>{0}</value>", ((EntityReference)r["udo_role"]).Name));

                    roleFetch = "<fetch><entity name='role'><attribute name='roleid'/>" +
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

                    roleResults = Fetch(roleFetch);

                    if (roleResults != null && roleResults.Entities.Count() > 0)
                    {
                        requests.Add(Disassociate(user, "systemuserroles_association",
                            roleResults.Entities.Select(r => r.ToEntityReference()),
                            userRoles));
                    }
                }
            }
            #endregion
        }

        internal void AddSecurityAccess(OrganizationRequestCollection requests, EntityReference user,
            IEnumerable<EntityReference> securityRecords, List<Guid> userRoles)
        {
            if (securityRecords.Count() == 0) return;

            #region Assign Security Role
            var roles = securityRecords.Where(r => r.LogicalName == "udo_securityrole");
            if (roles != null && roles.Count() > 0)
            {
                var roleFetch = "<fetch><entity name='udo_securityrole'>" +
                                "<attribute name='udo_role'/>" +
                                "<filter type='and'>" +
                                    "<condition attribute='udo_securityroleid' operator='in'>" +
                                        String.Join("", roles.Select(r => String.Format("<value>{0}</value>", r.Id))) +
                                    "</condition>" +
                                    "<condition attribute='statecode' operator='eq' value='0' />" +
                                "</filter></entity></fetch>";

                var roleResults = Fetch(roleFetch);
                if (roleResults != null && roleResults.Entities.Count > 0)
                {
                    var roleNames = roleResults.Entities
                        .Where(r => r.Contains("udo_role") && r["udo_role"] != null)
                        .Select(r => String.Format("<value>{0}</value>", ((EntityReference)r["udo_role"]).Name));

                    roleFetch = "<fetch><entity name='role'><attribute name='roleid'/>" +
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

                    roleResults = Fetch(roleFetch);
                    if (Debug) TracingService.Trace("Assign {0} Security Roles.", roleResults.Entities.Count());

                    if (roleResults != null && roleResults.Entities.Count() > 0)
                    {
                        requests.Add(Associate(user, "systemuserroles_association",
                            roleResults.Entities.Select(r => r.ToEntityReference()),
                            userRoles));

                    }
                }
            }
            #endregion

            #region Assign Security Group Roles
            var groups = securityRecords.Where(r => r.LogicalName == "udo_securitygroup");
            if (groups != null && groups.Count() > 0)
            {
                var securityRoles = new List<EntityReference>();

                #region Get Security Group Roles
                var groupRolesFetch = "<fetch><entity name='udo_udo_securitygroup_udo_securityrole'>" +
                                "<attribute name='udo_securityroleid'/>" +
                                "<filter type='and'>" +
                                "<condition attribute='udo_securitygroupid' operator='in'>" +
                                String.Join("", groups.Select(r => String.Format("<value>{0}</value>", r.Id))) +
                                "</condition>" +
                                "</filter></entity></fetch>";
                var groupRolesResults = Fetch(groupRolesFetch);

                if (groupRolesResults != null && groupRolesResults.Entities.Count() > 0)
                {
                    securityRoles.AddRange(groupRolesResults.Entities.Select(r => new EntityReference("udo_securityrole", (Guid)r["udo_securityroleid"])));
                    AddSecurityAccess(requests, user, securityRoles, userRoles);
                }
                //throw new Exception("Debugging...");
                #endregion
            }
            #endregion
        }

        internal List<Guid> GetUserRoles(EntityReference user)
        {
            var userroles = new List<Guid>();

            #region Add User's Roles
            var userrolesFetch = "<fetch><entity name='systemuserroles'><attribute name='roleid'/>" +
                "<filter type='and'><condition attribute='systemuserid' operator='eq' value='" + user.Id + "'/></filter>" +
                "</entity></fetch>";

            var userrolesResults = Fetch(userrolesFetch);
            if (Debug) TracingService.Trace("Getting Roles Assigned to User:");

            if (userrolesResults != null && userrolesResults.Entities.Count > 0)
            {
                userroles.AddRange(userrolesResults.Entities.Select(t =>
                {
                    if (Debug)
                    {
                        var role = OrganizationService.Retrieve("role", (Guid)t["roleid"], new ColumnSet(new string[] { "name", "businessunitid" }));
                        TracingService.Trace("Role: {2}:{0} [{1}]", role["name"], t["roleid"], ((EntityReference)role["businessunitid"]).Name);
                    }
                    return (Guid)t["roleid"];
                }));
            }
            #endregion

            return userroles;
        }

        internal List<Guid> GetAssignedRoles(EntityReference user)
        {
            var rolenames = new List<String>();

            #region Add User Roles (directly assigned)
            var userrolesFetch = "<fetch><entity name='udo_securityrole'><attribute name='udo_role'/>" +
                                 "<link-entity name='udo_udo_securityrole_systemuser' " +
                                              "from='udo_securityroleid' " +
                                              "to='udo_securityroleid' " +
                                              "visible='false' " +
                                              "intersect='true'>" +
                                 "<filter type='and'><condition attribute='systemuserid' operator='eq' value='" + user.Id + "'/></filter>" +
                                 "</link-entity>" +
                                 "<filter type='and'><condition attribute='statecode' operator='eq' value='0' /></filter>" +
                                 "</entity></fetch>";

            var userrolesResults = Fetch(userrolesFetch);

            if (userrolesResults != null && userrolesResults.Entities.Count > 0)
            {
                if (Debug) TracingService.Trace("Directly Assigned User Roles:");
                rolenames.AddRange(userrolesResults.Entities
                    .Where(r => r.Contains("udo_role"))
                    .Select(r =>
                    {
                        if (Debug) TracingService.Trace("Role: {0} [{1}]",
                            ((EntityReference)r["udo_role"]).Name,
                            ((EntityReference)r["udo_role"]).Id);
                        return ((EntityReference)r["udo_role"]).Name;
                    }));
            }
            #endregion

            #region Add User Roles from UDO Security Groups
            userrolesFetch = "<fetch><entity name='udo_securityrole'><attribute name='udo_role'/>" +
                             "<link-entity name='udo_udo_securitygroup_udo_securityrole' " +
                                             "from='udo_securityroleid' " +
                                             "to='udo_securityroleid' " +
                                             "visible='false' " +
                                             "intersect='true'>" +
                             "<link-entity name='udo_udo_securitygroup_systemuser' from='udo_securitygroupid' to='udo_securitygroupid' intersect='true'>" +
                             "<filter type='and'>" +
                             "<condition attribute='systemuserid' operator='eq' value='" + user.Id + "' />" +
                             "</filter></link-entity></link-entity>" +
                             "<filter type='and'><condition attribute='statecode' operator='eq' value='0' /></filter>" +
                             "</entity></fetch>";

            userrolesResults = Fetch(userrolesFetch);

            if (userrolesResults != null && userrolesResults.Entities.Count > 0)
            {
                rolenames.AddRange(userrolesResults.Entities.Select(r => ((EntityReference)r["udo_role"]).Name));
            }

            if (rolenames == null || rolenames.Count() == 0) return null;


            userrolesFetch = "<fetch><entity name='role'><attribute name='roleid'/>" +
                              "<link-entity name='businessunit' from='businessunitid' to='businessunitid' alias='pbu'>" +
                              "<link-entity name='systemuser' from='businessunitid' to='businessunitid' alias='psu'>" +
                              "<filter type='and'>" +
                                "<condition attribute='systemuserid' operator='eq' value='" + user.Id + "' />" +
                              "</filter>" +
                              "</link-entity></link-entity>" +
                              "<filter type='and'>" +
                              "<condition attribute='name' operator='in'>" +
                                String.Join("", rolenames.Select(r => String.Format("<value>{0}</value>", r))) +
                              "</condition>" +
                              "</filter></entity></fetch>";

            userrolesResults = Fetch(userrolesFetch);

            var userroles = new List<Guid>();
            if (userrolesResults != null && userrolesResults.Entities.Count() > 0)
            {
                userroles.AddRange(userrolesResults.Entities.Select(r => (Guid)r["roleid"]));
            }

            #endregion

            if (Debug) TracingService.Trace("Assigned Roles: {0}", userroles.Count());

            return userroles;
        }


        internal OrganizationRequest Associate(EntityReference one, string relationship, IEnumerable<EntityReference> many, List<Guid> excluding = null)
        {
            if (many == null || many.Count() == 0) return null;

            TracingService.Trace("Associate:");
            //"systemuserroles_association"
            var assoc = new AssociateRequest();

            assoc.Target = one;
            assoc.RelatedEntities = new EntityReferenceCollection();
            if (excluding == null || excluding.Count() == 0)
            {
                if (Debug) TracingService.Trace("Nothing to exclude");
                assoc.RelatedEntities.AddRange(many);
            }
            else
            {
                assoc.RelatedEntities.AddRange(many.Where(r =>
                {
                    if (Debug) TracingService.Trace("rID: {0}, in exlcluding: {1}", r.Id, excluding.Contains(r.Id));
                    return !excluding.Contains(r.Id);
                }));
            }
            if (assoc.RelatedEntities.Count() == 0)
            {
                if (Debug) TracingService.Trace("Nothing to relate to, association cancelled.");
                return null;
            }

            assoc.Relationship = new Relationship(relationship);
            if (Debug) TracingService.Trace("Target: {0}\r\nRelationship: {1} RelatedEntity Count: {2}",
                one, relationship, assoc.RelatedEntities.Count());

            return assoc;
        }

        internal OrganizationRequest Disassociate(EntityReference one, string relationship, IEnumerable<EntityReference> many, List<Guid> excluding = null)
        {
            if (many == null || many.Count() == 0) return null;

            //"systemuserroles_association"
            var disassoc = new DisassociateRequest();
            disassoc.Target = one;
            if (Debug)
            {
                TracingService.Trace("Disassociate:");
                TracingService.Trace("Target: {0} [{1}]", one.LogicalName, one.Id);
            }
            disassoc.RelatedEntities = new EntityReferenceCollection();
            if (excluding == null || excluding.Count() == 0)
            {
                if (Debug) TracingService.Trace("Nothing to exclude");
                disassoc.RelatedEntities.AddRange(many);
            }
            else
            {

                disassoc.RelatedEntities.AddRange(many.Where(r =>
                {
                    if (Debug) TracingService.Trace("rID: {0}, in exlcluding: {1}", r.Id, excluding.Contains(r.Id));
                    return !excluding.Contains(r.Id);
                }));
            }
            if (disassoc.RelatedEntities.Count() == 0)
            {
                if (Debug) TracingService.Trace("Nothing to relate to, disassociation cancelled.");
                return null;
            }
            else
            {
                if (Debug)
                {
                    foreach (var rel in disassoc.RelatedEntities)
                    {
                        TracingService.Trace("Related Item: {0} [{1}]", rel.LogicalName, rel.Id);
                    }
                }
            }
            if (Debug) TracingService.Trace("Relationship: {0}", relationship);
            disassoc.Relationship = new Relationship(relationship);

            return disassoc;
        }

        private EntityCollection Fetch(string fetch, string description = "")
        {
            if (Debug)
            {
                TracingService.Trace("Fetching {0}... \r\n", description);
                TracingService.Trace("Query: {0}\r\n", fetch);
            }
            return OrganizationService.RetrieveMultiple(new FetchExpression(fetch));
        }

        internal List<EntityReference> GetAllUsers()
        {
            var usersFetch = "<fetch><entity name='systemuser'><attribute name='systemuserid'/>" +
                "<filter type='and'><condition attribute='isdisabled' operator='eq' value='false'/></filter>" +
                "</entity></fetch>";

            var usersResult = Fetch(usersFetch, "All Users");

            if (usersResult == null) return null;

            if (usersResult.Entities.Count() == 0) return null;

            return usersResult.Entities.Select(u => u.ToEntityReference()).ToList();
        }
    }
     
}
