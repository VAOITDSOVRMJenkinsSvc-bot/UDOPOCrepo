using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Va.Udo.Usd.CustomControls.Shared;

namespace Va.Udo.Usd.CustomControls
{
    /// <summary>
    ///     Interaction logic for RoleSecurity.xaml
    /// </summary>
    public partial class TestTest : BaseHostedControlCommon
    {
        //private CRMGlobalManager _globalManager;
        private static readonly ColumnSet reportColumns = new ColumnSet(
            "name"
            );
        private static readonly ColumnSet letterColumns = new ColumnSet(
            "udo_sourcetype",
            "udo_source",
            "udo_ssrsreportname"
            );


        private readonly TraceLogger _logWriter;
        private string _processstep;
        private string _dispatch;
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicationName"></param>
        /// <param name="initXml"></param>
        public TestTest(Guid id, string applicationName, string initXml) :
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
            _processstep = string.Format("{0} > started", args.Action);

            var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
            var datanodename = Utility.GetAndRemoveParameter(parms, "DataNodeName");
;

            if (string.Compare(args.Action, "Test", StringComparison.OrdinalIgnoreCase) == 0)
            {
                _processstep = "Testing";
            }


            base.DoAction(args);
            base.UpdateContext(datanodename, "Processing Step", _processstep);
        }

    }
}