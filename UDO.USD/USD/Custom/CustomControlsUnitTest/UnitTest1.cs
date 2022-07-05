using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using VRM.Integration.UDO.Common.UnitTest;
using System.Web;


namespace CustomControlsUnitTest
{
    [TestClass]
    public class UnitTest1 : UdoUnitTestBase
    {


        [TestMethod]
        public void TestMethod1()
        {
            //Guid _crmeUserId = Guid.Parse("A80729AA-199E-E311-9410-02BF0A191463");
            //Uri organizationUri = new Uri("https://internalcrm.crmo15.dev.crm.vrm.vba.va.gov/UDODEV/XRMServices/2011/Organization.svc");
            //Uri homeRealmUri = null;
            //ClientCredentials credentials = new ClientCredentials();

            //credentials.Windows.ClientCredential = (NetworkCredential)CredentialCache.DefaultCredentials;
            //// or
            ////credentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
            //OrganizationServiceProxy orgProxy = new OrganizationServiceProxy(organizationUri, homeRealmUri, credentials, null);
            //orgProxy.EnableProxyTypes();

            //_service = (IOrganizationService)orgProxy;

            //var testString = SetUserRoles(_crmeUserId);


        }

        [TestMethod]
        public void GetUrlParameterValue()
        {
            var url = "https://internalcrm.crmo15.dev.crm.vrm.vba.va.gov/UDODEV/main.aspx?etc=10166&extraqs=%3f_gridType%3d2029%26etc%3d10166%26id%3d%257b5CCCD3EA-51BD-E611-9451-0050568DF261%257d%26rskey%3d%257b81484ACC-C30C-4869-B40C-2D1154FC1E5F%257d&histKey=175936093&newWindow=true&pagetype=entityrecord&rskey=%7b81484ACC-C30C-4869-B40C-2D1154FC1E5F%7d";
            var parm = "id";

            var returnValue = GetUrlParameters(url, parm);

        }

        private string GetUrlParameters(string url, string section)
        {
            var decodedUrl = HttpUtility.UrlDecode(url);

            decodedUrl = decodedUrl.Replace("%7b", "{");
            decodedUrl = decodedUrl.Replace("%7d", "}");

            var returnValue = "";
            var pArray = decodedUrl.Split('&');
            foreach (var t in pArray)
            {
                var keyValue = t.Split('=');
                if (keyValue[0] == section)
                {
                    returnValue = keyValue[1];
                }
            }

            returnValue = returnValue.Replace("{", "");
            returnValue = returnValue.Replace("}", "");

            return returnValue;
        }

        private string GetExtraQsParameter(string extraqs, string parameter)
        {
            var returnValue = "";


            var decodedExtraqs = extraqs.Replace("?", "");
            var eqsArray = decodedExtraqs.Split('&');
            foreach (var t in eqsArray)
            {
                var keyValue = t.Split('=');
                if (keyValue[0] != parameter) continue;

                returnValue = keyValue[1];
            }

            returnValue = returnValue.Replace("{", "");
            returnValue = returnValue.Replace("}", "");
            return returnValue;
        }





        //private string SetUserRoles(Guid userGuid)
        //{
        //    //StringBuilder sb = new StringBuilder();
        //    //int roleCount = 0;

        //    var userroles = new List<userrole>();

        //    var userRoleCollection = GetUserRoles(userGuid);
        //    foreach (var role in userRoleCollection.Entities)
        //    {
        //        var userrole = new userrole();
        //        userrole.roleid = role.GetAttributeValue<Guid>("roleid").ToString();
        //        userrole.name = role.GetAttributeValue<string>("name");
        //        userroles.Add(userrole);
        //        //roleCount++;
        //    }


        //    //sb.Append("{ userroles: [ ");            
        //    //foreach (var role in userRoleCollection.Entities)
        //    //{
        //    //    if (roleCount > 0)
        //    //    {
        //    //        sb.Append(", ");
        //    //    }

        //    //    sb.Append("{ ");
        //    //    sb.Append(string.Format("roleid: '{0}', name: '{1}'", role.GetAttributeValue<Guid>("roleid").ToString(), role.GetAttributeValue<string>("name")));
        //    //    sb.Append(" }");
        //    //    roleCount++;
        //    //}

        //    //sb.Append(string.Format(" ] rolecount: {0}", roleCount.ToString()));
        //    //sb.Append(" }");

        //    return ObjectToJSonString(userroles);
        //}

        /// <summary>
        /// Get User Roles
        /// </summary>
        /// <param name="userGuid">User Guid</param>
        /// <returns>Returns an Entity Collection of User Assigned Roles</returns>
        //private EntityCollection GetUserRoles(Guid userGuid)
        //{
            ////Create Query Expression to fetch Role Entity 
            //var queryExpression = new QueryExpression
            //{
            //    EntityName = "role",
            //    //Setting the link entity condition and filter condition criteria/ 
            //    LinkEntities =
            //        {
            //            new LinkEntity
            //            {
            //                LinkFromEntityName = "role",
            //                LinkFromAttributeName = "roleid",
            //                LinkToEntityName = "systemuserroles",
            //                LinkToAttributeName = "roleid",
            //                LinkCriteria = new FilterExpression
            //                {
            //                    FilterOperator = LogicalOperator.And,
            //                    Conditions =
            //                    {
            //                        new ConditionExpression
            //                        {
            //                            AttributeName = "systemuserid",
            //                            Operator = ConditionOperator.Equal,
            //                            Values = {userGuid}
            //                        }
            //                    }
            //                }
            //            }
            //        },
            //    ColumnSet = RoleColumns
            //};

            //// Obtain results from the query expression. 
            //var userRoles = OrgServiceProxy.RetrieveMultiple(queryExpression);

            //return userRoles;
        //}

        /// <summary>
        /// Convert object to MemoryStream in Json.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string ObjectToJSonString(object obj)
        {
            string jsonString;
            MemoryStream memStream = new MemoryStream();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
  
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





        [TestCategory("CustomControls"), TestMethod]
        public void EntityMetaData_UnitTest()
        {


            #region connect to CRM

            string connectionExceptionMessage;

            OrgServiceProxy = ConnectToCrm(UdoOrg, out connectionExceptionMessage);
            if (OrgServiceProxy == null)
            {
                Assert.Fail(connectionExceptionMessage);
            }

            #endregion


            var request = new RetrieveAllEntitiesRequest()
            {
                EntityFilters = EntityFilters.Entity,
                RetrieveAsIfPublished = true
            };

            var response = (RetrieveAllEntitiesResponse)OrgServiceProxy.Execute(request);

            foreach (var currentEntity in response.EntityMetadata)
            {

                if (currentEntity.IsCustomEntity != null && currentEntity.IsCustomEntity.Value == false) continue;
                if (!currentEntity.ObjectTypeCode.HasValue) continue;

                if (currentEntity.LogicalName == "udo_mapdletter")
                {

                    var test = currentEntity.DisplayName;
                }
            }


        }


        [TestCategory("CustomControls"), TestMethod]
        public void RetrieveAvailableItemFromQueue_UnitTest()
        {

            #region connect to CRM

            string connectionExceptionMessage;

            OrgServiceProxy = ConnectToCrm(UdoOrg, out connectionExceptionMessage);
            if (OrgServiceProxy == null)
            {
                Assert.Fail(connectionExceptionMessage);
            }

            #endregion

            try
            {

                var objectId = "18EE4355-95B5-E611-9451-0050568DF261";
                //UpdateContext("SelectedQueueItem", "ObjectId", objectId);

                var fetchXmlString = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' >
    <entity name='queueitem' >
        <filter type='and' >
            <condition attribute='objectid' operator='eq' value='{0}' />
            <condition attribute='statecode' operator='eq' value='0' />
        </filter>
    </entity>
</fetch>";

                var fetchExpression = new FetchExpression(string.Format(fetchXmlString, objectId));

                // Obtain results from the query expression. 
                var queueitems = OrgServiceProxy.RetrieveMultiple(fetchExpression);

                if (queueitems != null)
                {
                    if (queueitems.Entities.Count > 0)
                    {
                        var queueItem = queueitems.Entities[0];
                        //UpdateContext("SelectedQueueItem", "QueueItemRecordFound", "Y");
                        //UpdateContext("SelectedQueueItem", "QueueItemId", queueItem.Id.ToString());
                        //UpdateContext("SelectedQueueItem", "WorkerId", queueItem.GetAttributeValue<EntityReference>("workerid").Id.ToString());
                        //UpdateContext("SelectedQueueItem", "WorkerName", queueItem.GetAttributeValue<EntityReference>("workerid").Name);
                    }
                    else
                    {
                        //UpdateContext("SelectedQueueItem", "QueueItemRecordFound", "N");
                    }
                }
                else
                {
                    //UpdateContext("SelectedQueueItem", "QueueItemRecordFound", "N");
                }
            }
            catch (Exception ex)
            {
                //UpdateContext("SelectedQueueItem", "ExceptionMessage", ex.Message);
            }


        }



    //    [TestCategory("Letters"), TestMethod]
    //    public void PickFromQueue_UnitTest()
    //    {


    //        #region connect to CRM

    //        string connectionExceptionMessage;

    //        OrgServiceProxy = ConnectToCrm(UdoOrg, out connectionExceptionMessage);
    //        if (OrgServiceProxy == null)
    //        {
    //            Assert.Fail(connectionExceptionMessage);
    //        }

    //        #endregion

    //        var queueItemId = "25E01B53-6CB6-E611-9451-0050568DF261";
    //        var userid = "0A90C445-88A2-E511-9418-00155D14F3D0";

    //        var pickFromQueue = new PickFromQueueRequest
    //        {
    //            QueueItemId = new Guid(queueItemId),
    //            WorkerId = new Guid(userid)
    //        };
           
    //        var queueResponse = (PickFromQueueResponse)OrgServiceProxy.Execute(pickFromQueue);

    //        if (queueResponse.Results.Count > 0)
    //        {

    //        }


    //    }
    }

    [DataContract]
    public class userrole
    {
        [DataMember(Name = "roleid")]
        public string roleid
        {
            get;
            set;
        }
        [DataMember(Name = "name")]
        public string name
        {
            get;
            set;
        }
    }
}
