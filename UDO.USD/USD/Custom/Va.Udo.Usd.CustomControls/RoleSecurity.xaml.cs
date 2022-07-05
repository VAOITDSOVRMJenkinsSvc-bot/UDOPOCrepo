using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.Uii.Desktop.SessionManager;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Va.Udo.Usd.CustomControls.Shared;
using AuthenticationType = Microsoft.Xrm.Tooling.Connector.AuthenticationType;

namespace Va.Udo.Usd.CustomControls
{
    /// <summary>
    ///     Interaction logic for RoleSecurity.xaml
    /// </summary>
    public partial class RoleSecurity : BaseHostedControlCommon
    {
        private static readonly ColumnSet RoleColumns = new ColumnSet("roleid", "name", "businessunitid",
            "organizationid", "solutionid", "componentstate", "createdon", "modifiedon");

        /// <summary>
        ///     Log writer
        /// </summary>
        private readonly TraceLogger _logWriter;


        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicationName"></param>
        /// <param name="initXml"></param>
        public RoleSecurity(Guid id, string applicationName, string initXml) :
            base(id, applicationName, initXml)
        {
            InitializeComponent();
            _logWriter = new TraceLogger();
        }

        protected override void DesktopReady()
        {
            base.DesktopReady();
        }

        protected override void DoAction(RequestActionEventArgs args)
        {

            var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);

            //if (string.Compare(args.Action, "SetUserRoles", StringComparison.OrdinalIgnoreCase) == 0)
            //{
            //    var userid = Utility.GetAndRemoveParameter(parms, "userid");

            //    if (!string.IsNullOrEmpty(userid))
            //    {
            //        var userRoles = SetUserRoles(new Guid(userid));

            //        var lri = new LookupRequestItem
            //        {
            //            Key = "UserRoles",
            //            Value = userRoles
            //        };

            //        var lriList = new List<LookupRequestItem> { lri };

            //        var dcr =
            //            (DynamicsCustomerRecord)
            //                ((AgentDesktopSession)localSessionManager.ActiveSession).Customer.DesktopCustomer;
            //        //dcr.MergeReplacementParameter("$Return", lriList, true);

            //        //_logWriter.Log(string.Format("User Roles: {0}", userRoles));
            //    }
            //    else
            //    {
            //        _logWriter.Log("WARNING: No User ID found.  No User Roles Set");
            //    }
            //}
            //else
            if (string.Compare(args.Action, "SetAllUserRoles", StringComparison.OrdinalIgnoreCase) == 0)
            {
                var datanodename = Utility.GetAndRemoveParameter(parms, "DataNodeName");

                try
                {
                    base.UpdateContext(datanodename, "#RoleCount", "0");
                    base.UpdateContext(datanodename, "#UserRoleCount", "0");

                    // Added due to $Return issue
                    base.UpdateContext(datanodename, "#URJ", "");
                    var userid = Utility.GetAndRemoveParameter(parms, "userid");
                    if (!string.IsNullOrEmpty(userid))
                    {
                        var userRolesJson = SetUserRoles(new Guid(userid));
                        string userRolesJsonString = userRolesJson.ToString();
                        base.UpdateContext(datanodename, "#URJ", userRolesJsonString);
                    }

                    var roles = GetAllRoles();
                    var userRoles = GetAllUserRoles();

                    var roleCount = 0;
                    var userRoleCount = 0;

                    foreach (var role in roles.Entities)
                    {
                        roleCount++;
                        var roleFound = false;

                        if (userRoles.Entities.Any(userRole => role.GetAttributeValue<string>("name").Equals(userRole.GetAttributeValue<string>("name"))))
                        {
                            userRoleCount++;
                            roleFound = true;
                        }

                        base.UpdateContext(datanodename, role.GetAttributeValue<string>("name"), "string", roleFound ? "True" : "False");
                    }
                    base.UpdateContext(datanodename, "#RoleCount", roleCount.ToString());
                    base.UpdateContext(datanodename, "#UserRoleCount", userRoleCount.ToString());
                }
                catch (Exception ex)
                {
                    _logWriter.Log(ex);
                    base.UpdateContext(datanodename, "ErrorOccurred", "Y");
                    base.UpdateContext(datanodename, "ErrorOccurredIn", "SetAllUserRoles");
                    base.UpdateContext(datanodename, "ExceptionMessage", ex.Message);
                }
            }
            base.DoAction(args);
        }


        private string SetUserRoles(Guid userGuid)
        {
            var sb = new StringBuilder();
            var roleCount = 0;

            var userroles = new List<userrole>();

            var userRoleCollection = GetUserRoles(userGuid);
            foreach (var role in userRoleCollection.Entities)
            {
                var userrole = new userrole();
                userrole.roleid = role.GetAttributeValue<Guid>("roleid").ToString();
                userrole.name = role.GetAttributeValue<string>("name");
                userroles.Add(userrole);
                roleCount++;
            }

            sb.Append("{\"userroles\": ");
            sb.Append(ObjectToJSonString(userroles));
            sb.Append(string.Format(", \"rolecount\": {0} ", roleCount));
            sb.Append(" }");

            return sb.Replace("\"", "'").ToString();
        }

        /// <summary>
        ///     Get User Roles
        /// </summary>
        /// <param name="userGuid">User Guid</param>
        /// <returns>Returns an Entity Collection of User Assigned Roles</returns>
        private EntityCollection GetUserRoles(Guid userGuid)
        {
            //Create Query Expression to fetch Role Entity 
            var queryExpression = new QueryExpression
            {
                EntityName = "role",
                //Setting the link entity condition and filter condition criteria/ 
                LinkEntities =
                {
                    new LinkEntity
                    {
                        LinkFromEntityName = "role",
                        LinkFromAttributeName = "roleid",
                        LinkToEntityName = "systemuserroles",
                        LinkToAttributeName = "roleid",
                        LinkCriteria = new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression
                                {
                                    AttributeName = "systemuserid",
                                    Operator = ConditionOperator.Equal,
                                    Values = {userGuid}
                                }
                            }
                        }
                    }
                },
                ColumnSet = RoleColumns
            };

            // Obtain results from the query expression. 
            var userRoles = base.RetrieveMultiple(queryExpression);

            return userRoles;
        }

        private EntityCollection GetAllRoles()
        {
            var query = "<fetch version='1.0' distinct='true' > " +
                        "<entity name='role' > " +
                        "<attribute name='name' /> " +
                        "<order descending='false' attribute='name' /> " +
                        "</entity> " +
                        "</fetch>";

            var fetchExpression = new FetchExpression(query);
            var roles = base.RetrieveMultiple(fetchExpression);

            return roles;
        }

        private EntityCollection GetAllUserRoles()
        {
            var query = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' > " +
                        "<entity name='role' > " +
                        "<attribute name='name' /> " +
                        "<attribute name='businessunitid' /> " +
                        "<attribute name='roleid' /> " +
                        "<attribute name='roleidunique' /> " +
                        "<order descending='false' attribute='name' /> " +
                        "<link-entity name='systemuserroles' from='roleid' to='roleid' intersect='true' > " +
                        "<link-entity name='systemuser' to='systemuserid' from='systemuserid' alias='al' > " +
                        "<filter type='and' > " +
                        "<condition attribute='systemuserid' operator='eq-userid' /> " +
                        "</filter> " +
                        "</link-entity> " +
                        "</link-entity> " +
                        "</entity> " +
                        "</fetch>";

            var fetchExpression = new FetchExpression(query);
            var userRoles = base.RetrieveMultiple(fetchExpression);

            return userRoles;
        }

        /// <summary>
        ///     Convert object to MemoryStream in Json.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string ObjectToJSonString(object obj)
        {
            string jsonString;
            var memStream = new MemoryStream();

            var ser = new DataContractJsonSerializer(obj.GetType());

            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, obj);
                var sw = new StreamWriter(ms);
                sw.Flush();

                ms.Position = 0;
                var sr = new StreamReader(ms);

                jsonString = sr.ReadToEnd();
            }

            return jsonString;
        }
    }


    [DataContract]
    public class userrole
    {
        [DataMember(Name = "roleid")]
        public string roleid { get; set; }

        [DataMember(Name = "name")]
        public string name { get; set; }
    }
}