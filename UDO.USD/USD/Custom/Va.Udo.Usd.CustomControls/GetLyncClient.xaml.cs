using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Va.Udo.Usd.CustomControls.Shared;
using Microsoft.Lync.Model;

namespace Va.Udo.Usd.CustomControls
{

    public partial class GetLyncClient : BaseHostedControlCommon
    {
        public GetLyncClient(Guid id, string applicationName, string initXml) :
            base(id, applicationName, initXml)
        {
            InitializeComponent();
        }

        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            if (string.Compare(args.Action, "GetLyncClient-CheckRunning", StringComparison.OrdinalIgnoreCase) == 0)
            {
                var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
                var _message = Utility.GetAndRemoveParameter(parms, "message");
                var recipients = Utility.GetAndRemoveParameter(parms, "recipients");
                Initiate(recipients, args);
            }
        }

        public void Initiate(string recipients, Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            try
            {
                //Start the conversation
                base.UpdateContext("EmergencyProcess", "Processing Step", "Initiate");
                var client = LyncClient.GetClient();
                args.ActionReturnValue = "sfb";
            }
            catch (LyncClientException lyncClientException)
            {
                args.ActionReturnValue = "2010";
            }
            catch (SystemException systemException)
            {
                args.ActionReturnValue = "unknown";
            }
        }
    }
}
