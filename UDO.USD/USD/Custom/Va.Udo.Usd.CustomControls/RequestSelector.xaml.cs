using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Va.Udo.Usd.CustomControls.Shared;

namespace Va.Udo.Usd.CustomControls
{

    public partial class RequestSelector : BaseHostedControlCommon, IComponentConnector
    {

        private readonly TraceLogger _logWriter;
        private string _processstep;
        private string _dispatch;

        public RequestSelector()
        {
            InitializeComponent();
        }

        public RequestSelector(Guid id, string applicationName, string initXml) :
            base(id, applicationName, initXml)
        {
            InitializeComponent();
            _logWriter = new TraceLogger("Va.Udo.Usd.CustomControls.RequestSelector");
        }

        protected override void DesktopReady()
        {
            base.PopulateToolbars(this.ProgrammableToolbarTray);
            base.DesktopReady();
        }

        protected override void DoAction(RequestActionEventArgs args)
        {
            _processstep = string.Format("{0} > started", args.Action);

            var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
            var datanodename = Utility.GetAndRemoveParameter(parms, "DataNodeName");


            if (string.Compare(args.Action, "FileDownload", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region FileDownload



                

                #endregion
            }


            base.DoAction(args);

        }

        private void LoadRequestTypes()
        {
            LbRequestType.Items.Clear();

            var fetchXml = "";
            var uiiOption = base.RetrieveUiiOptionByName("RoleSecurityEnabled");

            if (uiiOption.GetAttributeValue<string>("RoleSecurityEnabled") == "Y")
            {
                fetchXml = "<fetch><entity name='udo_requesttype'>" +
                   "<attribute name='udo_name'/>" +
                   "<attribute name='udo_requesttypeid'/>" +
                   "<attribute name='udo_order'/>" +
                   "<link-entity name='udo_securityrole' from='udo_securityroleid' to='udo_securityroleid' alias='secRole' link-type='outer'>" +
                   "<link-entity name='role' from='name' to='udo_name' alias='userrole' link-type='outer'>" +
                   "<link-entity name='systemuserroles' from='roleid' to='roleid' alias='r' link-type='outer'>" +
                   "<attribute name='roleid'/>" +
                   "<filter type='and'>" +
                   "<condition attribute='systemuserid' operator='eq' value='" + _client.CrmInterface.GetMyCrmUserId() + "'/>" +
                   "</filter>" +
                   "</link-entity></link-entity>" +
                   "</link-entity>" +
                   "<filter type='and'>" +
                   "<condition attribute='udo_order' operator='not-null'/>" +
                   "<filter type='or'>" +
                   "<condition attribute='udo_securityroleid' operator='null'/>" +
                   "<filter type='and'>" +
                   "<condition attribute='udo_securityroleid' operator='not-null'/>" +
                   "<condition entityname='r' attribute='roleid' operator='not-null'/>" +
                   "</filter></filter>" +
                   "</filter>" +
                   "<order attribute='udo_order' descending='false'/>" +
                   "</entity></fetch>";
            }
            else
            {
                fetchXml = "<fetch><entity name='udo_requesttype'>" +
                    "<attribute name='udo_name'/>" +
                    "<attribute name='udo_requesttypeid'/>" +
                    "<attribute name='udo_order'/>" +
                    "<filter type='and'>" +
                    "<condition attribute='udo_order' operator='not-null'/>" +
                    "</filter>" +
                    "<order attribute='udo_order' descending='false'/>" +
                    "</entity></fetch>";
            }

            var execFetch = new FetchExpression(fetchXml);
            var ec = base.RetrieveMultiple(execFetch);


            foreach (var item in ec.Entities)
            {
                
            }

        }

        private void LbRequestType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void LbRequestSubType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        
        public void InitializationComponent()
        {
            if (this._contentLoaded)
            {
                return;
            }
            this._contentLoaded = true;

            LoadRequestTypes();
            //Application.LoadComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    public class RequestItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

}