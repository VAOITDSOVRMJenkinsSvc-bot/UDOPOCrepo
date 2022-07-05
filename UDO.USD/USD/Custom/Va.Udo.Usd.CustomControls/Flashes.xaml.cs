using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Va.Udo.Usd.CustomControls.Shared;

namespace Va.Udo.Usd.CustomControls
{

    public partial class Flashes : BaseHostedControlCommon
    {
        private readonly BackgroundWorker _worker = new BackgroundWorker();
        private List<string> _flashes = new List<string>();
        private string _contactid;
        private string _veteransnapshotid;
        private string saveFlashes;



        private static bool? _inDesignMode;
        public static bool IsInDesignMode
        {
            get
            {
                if (!_inDesignMode.HasValue)
                {
                    _inDesignMode = false;
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    if (prop != null)
                    {
                        _inDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
                        if (_inDesignMode.HasValue && _inDesignMode.Value) _inDesignMode = true;
                    }
                }
                return _inDesignMode.Value;
            }
        }
        private string _processstep;
        private bool _saveFlashes;
        //private readonly TraceLogger _logWriter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicationName"></param>
        /// <param name="initXml"></param>
        public Flashes(Guid id, string applicationName, string initXml) :
            base(id, applicationName, initXml)
        {
            InitializeComponent();
            //_logWriter = new TraceLogger();

            _worker.DoWork += worker_DoWork;
            _worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FlashPanel.Children.Clear();

            var parentWindow = Window.GetWindow(this);
            if (parentWindow != null) flashesScrollViewer.Width = parentWindow.ActualWidth;

            if (_flashes == null || _flashes.Count == 0)
            {
                AddFlash("No Flashes found at this time");

                SafeDispatcher.BeginInvoke(new Action(() =>
                {
                    FireEvent("FlashesLoaded");
                }));
                return;
            }

            SafeDispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var flash in _flashes)
                {
                    AddFlash(flash);
                }
            }));


            SafeDispatcher.BeginInvoke(new Action(() =>
            {
                FireEvent("FlashesLoaded");
            }));
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            base.UpdateContext("Flashes", "contactid", _contactid);
            base.UpdateContext("Flashes", "veteransnapshotid", _veteransnapshotid);
            base.UpdateContext("Flashes", "save", _saveFlashes.ToString());

            EntityReference target = null;

            if (!string.IsNullOrEmpty(_contactid))
            {
                target = new EntityReference("contact", new Guid(_contactid));
            }

            if (!string.IsNullOrEmpty(_veteransnapshotid))
            {
                target = new EntityReference("udo_veteransnapshot", new Guid(_veteransnapshotid));
            }

            _processstep = string.Format("Run GetFlashesFromCRM");
            _flashes = GetFlashesFromCRM(target);
        }

        public Flashes()
        {
            InitializeComponent();
        }

        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {

            _processstep = string.Format("{0} > started", args.Action);

            try
            {
                if (args != null && !String.IsNullOrWhiteSpace(args.Action))
                {
                    if (string.Equals(args.Action, "loadflashes", StringComparison.InvariantCultureIgnoreCase))
                    {

                        base.UpdateContext("Flashes", "ErrorOccurred", "");
                        base.UpdateContext("Flashes", "ErrorOccurredIn", "");
                        base.UpdateContext("Flashes", "ExceptionMessage", "");

                        var load_parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
                        _contactid = Utility.GetAndRemoveParameter(load_parms, "contactid");
                        _veteransnapshotid = Utility.GetAndRemoveParameter(load_parms, "veteransnapshotid");
                        saveFlashes = Utility.GetAndRemoveParameter(load_parms, "save");
                        _saveFlashes = saveFlashes == "Y";

                        _processstep = string.Format("Run LoadFlashes");

                        _worker.RunWorkerAsync();
                    }
                    else if (string.Equals(args.Action, "addflash", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var add_parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
                        var newflash = Utility.GetAndRemoveParameter(add_parms, "flash");
                        AddFlash(newflash);
                        //ShowFlash();
                    }
                    else if (string.Equals(args.Action, "clearFlash", StringComparison.InvariantCultureIgnoreCase))
                    {
                        FlashPanel.Children.Clear();
                    }
                }

                base.DoAction(args);

            }
            catch (Exception ex)
            {
                AddFlash("An unexpected exception occurred while retrieving Flashes");
                base.UpdateContext("Flashes", "Processing Step", _processstep);
                var exp = ExceptionManager.ReportException(ex);

                base.UpdateContext("Flashes", "ErrorOccurred", "Y");
                base.UpdateContext("Flashes", "ErrorOccurredIn", args.Action);
                base.UpdateContext("Flashes", "ExceptionMessage", ex.Message);
                base.UpdateContext("Flashes", "ExceptionReport", exp);
            }
            //}), new object[0]);
        }

        private void ShowFlash()
        {
            FlashPanel.Visibility = System.Windows.Visibility.Visible;
            flashesScrollViewer.Visibility = System.Windows.Visibility.Visible;

            SafeDispatcher.BeginInvoke(new Action(() =>
                {
                    FireEvent("showflash");
                }), new object[0]);
        }

        private void HideFlash()
        {
            FlashPanel.Visibility = System.Windows.Visibility.Collapsed;
            flashesScrollViewer.Visibility = System.Windows.Visibility.Collapsed;

            SafeDispatcher.BeginInvoke(new Action(() =>
            {
                FireEvent("hideflash");
            }), new object[0]);
        }

        private List<string> GetFlashesFromCRM(EntityReference target)
        {

            var list = new List<string>();
            if (target == null) return null;

            try
            {
                var req = new OrganizationRequest("udo_GetFlashes");

                req.Parameters.Add("ParentEntityReference", target);
                req.Parameters.Add("Save", _saveFlashes);

                _processstep = string.Format("Execute udo_GetFlashes Action");
                var resp = base.Execute(req);

                if (!resp.Results.ContainsKey("Flashes"))
                {
                    _processstep = string.Format("No Flashes Found");
                    AddFlash("No Flashes found");
                    return null;
                }

                var results = resp.Results["Flashes"] as EntityCollection;

                if (results == null || results.Entities.Count == 0) return null;

                foreach (var item in results.Entities)
                {
                    list.Add(item["udo_name"].ToString().Trim());
                }

                _processstep = string.Format("Return Flashes List");
                return list;
            }
            catch (Exception ex)
            {
                _processstep = string.Format("Unexpected error found - was not able to return Flashes");
                AddFlash("Unexpected error found - was not able to return Flashes");
                return null;
            }
        }

        //private void LoadFlashes(string contactid, string veteransnapshotid)
        //{

        //    //lock (_client.CrmInterface.ConnectionLockObject)
        //    //{
        //    //SafeDispatcher.BeginInvoke(new Action(() =>
        //    //{
        //    EntityReference target = null;

        //    if (!String.IsNullOrEmpty(contactid))
        //    {
        //        target = new EntityReference("contact", new Guid(contactid));
        //    }

        //    if (!String.IsNullOrEmpty(veteransnapshotid))
        //    {
        //        target = new EntityReference("udo_veteransnapshot", new Guid(veteransnapshotid));
        //    }

        //    _processstep = string.Format("Run GetFlashesFromCRM");
        //    var flashes = GetFlashesFromCRM(target);

        //    //FlashPanel.Children.Clear();
        //    //HideFlash();
        //    //FlashExpander.IsExpanded = false;

        //    if (flashes == null)
        //    {
        //        AddFlash("No Flashes found at this time");
        //        //ShowFlash();

        //        SafeDispatcher.BeginInvoke(new Action(() =>
        //        {
        //            FireEvent("FlashesLoaded");
        //        }), new object[0]);
        //        return;
        //    }

        //    foreach (var flash in flashes)
        //    {
        //        AddFlash(flash);
        //    }

        //    //ShowFlash();
        //    SafeDispatcher.BeginInvoke(new Action(() =>
        //    {
        //        FireEvent("FlashesLoaded");
        //    }), new object[0]);

        //    //}, DispatcherPriority.Background);
        //    //}), new object[0]);
        //    //}


        //}

        private void AddFlash(string flash)
        {
            var textBox = new System.Windows.Controls.TextBox
            {
                Style = FlashPanel.FindResource("Flash") as Style,
                Text = flash,
                IsReadOnly = true
            };
            FlashPanel.Children.Add(textBox);
        }

        protected override void SafeDispatcherUnhandledExceptionHandler(object sender, SafeDispatcherUnhandledExceptionEventArgs ex)
        {
            ex.Handled = true;
            base.SafeDispatcherUnhandledExceptionHandler(sender, ex);
        }
    }
}
