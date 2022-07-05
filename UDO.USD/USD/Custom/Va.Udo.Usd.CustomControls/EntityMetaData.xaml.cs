using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.Uii.Desktop.SessionManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using Va.Udo.Usd.CustomControls.Shared;

namespace Va.Udo.Usd.CustomControls
{
    /// <summary>
    /// Interaction logic for RoleSecurity.xaml
    /// </summary>
    public partial class EntityMetaData : BaseHostedControlCommon
    {
        /// <summary>
        /// Log writer
        /// </summary>
        private TraceLogger logWriter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicationName"></param>
        /// <param name="initXml"></param>
        public EntityMetaData(Guid id, string applicationName, string initXml) :
            base(id, applicationName, initXml)
        {
            InitializeComponent();
            logWriter = new TraceLogger();
        }

        //protected override void DesktopReady()
        //{
        //    base.DesktopReady();
        //}

        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            if (string.Compare(args.Action, "CacheEntityMetaData", StringComparison.OrdinalIgnoreCase) == 0)
            {
                var userRoles = GetEtc();

                var lri = new LookupRequestItem
                {
                    Key = "EntityMetaData",
                    Value = userRoles
                };

                var lriList = new List<LookupRequestItem> { lri };

                var dcr = (DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.ActiveSession).Customer.DesktopCustomer;
                dcr.MergeReplacementParameter("$Return", lriList, true);

                logWriter.Log(string.Format("Entity Meta Data: {0}", userRoles), TraceEventType.Information);
            }
            base.DoAction(args);
        }

        public string GetEtc()
        {
            var sb = new StringBuilder();
            var entityMetaDataCol = new List<Emd>();
            var emdCount = 0;

            if (_client.CrmInterface.ActiveAuthenticationType != AuthenticationType.OAuth)
                _client.CrmInterface.OrganizationServiceProxy.EnableProxyTypes();

            var request = new RetrieveAllEntitiesRequest()
            {
                EntityFilters = EntityFilters.Entity,
                RetrieveAsIfPublished = true
            };

            var response = base.RetrieveAllEntities(request);

            foreach (var currentEntity in response.EntityMetadata)
            {

                if (currentEntity.IsCustomEntity != null && currentEntity.IsCustomEntity.Value == false) continue;
                if (!currentEntity.ObjectTypeCode.HasValue) continue;
                var myEmd = new Emd
                {
                    Entityname = currentEntity.LogicalName,
                    ObjectTypeCode = currentEntity.ObjectTypeCode.Value
                };
                entityMetaDataCol.Add(myEmd);
                emdCount++;
            }

            sb.Append("{\"emd\": ");
            sb.Append(ObjectToJSonString(entityMetaDataCol));
            sb.Append(string.Format(", \"emdcount\": {0} ", emdCount.ToString()));
            sb.Append(" }");

            return sb.Replace("\"", "'").ToString();
        }

        /// <summary>
        /// Convert object to MemoryStream in Json.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string ObjectToJSonString(object obj)
        {
            string jsonString;
            //var memStream = new MemoryStream();

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
    public class Emd
    {
        [DataMember(Name = "entityname")]
        public string Entityname
        {
            get;
            set;
        }
        [DataMember(Name = "objecttypecode")]
        public int ObjectTypeCode
        {
            get;
            set;
        }
    }

}
