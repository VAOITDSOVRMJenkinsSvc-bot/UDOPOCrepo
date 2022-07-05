using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Desktop.SessionManager;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using AuthenticationType = Microsoft.Xrm.Tooling.Connector.AuthenticationType;

namespace Va.Udo.Usd.CustomControls
{
    /// <summary>
    ///     Interaction logic for RoleSecurity.xaml
    /// </summary>
    public partial class XrmData : DynamicsBaseHostedControl
    {
        /// <summary>
        ///     Log writer
        /// </summary>
        private readonly TraceLogger logWriter;


        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicationName"></param>
        /// <param name="initXml"></param>
        public XrmData(Guid id, string applicationName, string initXml) :
            base(id, applicationName, initXml)
        {
            InitializeComponent();
            logWriter = new TraceLogger();
        }

        protected override void DesktopReady()
        {
            base.DesktopReady();
        }

        protected void DoFetch(RequestActionEventArgs args)
        {
            var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);

            var name = Utility.GetAndRemoveParameter(parms, "name");
            var fetchXml = Utility.GetAndRemoveParameter(parms, "fetchxml");

            if (String.IsNullOrEmpty(fetchXml)) fetchXml = Utility.RemainderParameter(parms);
            
            //var service = _client.CrmInterface.OrganizationServiceProxy;

            var fetch = new FetchExpression(fetchXml);

            EntityCollection results = null;

            if (_client.CrmInterface.ActiveAuthenticationType == AuthenticationType.OAuth)
                results = _client.CrmInterface.OrganizationWebProxyClient.RetrieveMultiple(fetch);
            else
                results = _client.CrmInterface.OrganizationServiceProxy.RetrieveMultiple(fetch);
            
            if (results.Entities.Count > 0)
            {
                var entity = results.Entities[0];
                if (!name.Equals(entity.LogicalName)) {
                    name = String.Format("{0}#{1}", name, entity.LogicalName);
                }
                
                var sessionMgr = localSessionManager;
                var session = sessionMgr.ActiveSession as AgentDesktopSession;
                if (session != null)
                {
                    var customerRecord = (DynamicsCustomerRecord)session.Customer.DesktopCustomer;
                    var ed = EntityDescription.FromEntity(results.Entities[0]);
                    var data = ed.data;
                    foreach (var fld in results.Entities[0].Attributes)
                    {
                        var av = fld.Value as AliasedValue;
                        if (av != null)
                        {
                            var flddata = new CRMApplicationData();
                            flddata.name = fld.Key + "_value";
                            if (av.Value is EntityReference)
                            {
                                data.Add(fld.Key+"_value", new CRMApplicationData {value =  ((EntityReference) av.Value).Id.ToString(), type = "Guid", name = fld.Key+"_value"});
                                data.Add(fld.Key+"_name", new CRMApplicationData {value =  ((EntityReference) av.Value).Name, type = "string", name = fld.Key+"_name"});
                            }
                            else if (av.Value is OptionSetValue)
                            {
                                data.Add(fld.Key + "_value",
                                    new CRMApplicationData
                                    {
                                        value = ((OptionSetValue) av.Value).Value.ToString(),
                                        type = "Guid",
                                        name = fld.Key + "_value"
                                    });
                            }
                            else
                            {
                                data.Add(fld.Key+"_value", new CRMApplicationData {value =  av.Value.ToString(), type = "string", name = fld.Key+"_value"});
                            }
                        }
                    }
                    customerRecord.MergeReplacementParameter(name, data,false);
                }
                results = null;
            }
        }

        protected override void DoAction(RequestActionEventArgs args)
        {
            switch (args.Action.ToUpper())
            {
                case "FETCH":
                    DoFetch(args);
                    break;
                default:
                    //no action
                    break;
            }
            base.DoAction(args);
        }
    }
}