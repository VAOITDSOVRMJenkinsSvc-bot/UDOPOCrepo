using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Microsoft.Xrm.Sdk;
using Va.Udo.Usd.CustomControls.Shared;

namespace Va.Udo.Usd.CustomControls
{
    /// <summary>
    /// Interaction logic for RoleSecurity.xaml
    /// </summary>
    public partial class VeteranSnapShot : BaseHostedControlCommon, IComponentConnector
    {
        #region Private Members

        private bool _isTimerRunning;
        private Int32 m_Message;

        private static int _clicks = 0;

        /// <summary>
        /// Log writer
        /// </summary>
        private readonly TraceLogger _logWriter;

        private string _idproofid;
        private DispatcherTimer _timer;
        private int _actualattemps;
        private int _maxattemps;
        private int _interval;
        private bool _isLocked;
        private StringBuilder _sb;
        private Guid _veteranSnapshotId;
        private string _dispatch;

        private bool _dataIssue;
        private bool _timeOut;
        private bool _exception;
        private string _responseMessage;
        #endregion

        internal ProgressControl _ProgressIndicator;

        public VeteranSnapShot()
        {
            InitializeComponent();

            _logWriter = new TraceLogger();
            _isTimerRunning = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicationName"></param>
        /// <param name="initXml"></param>
        public VeteranSnapShot(Guid id, string applicationName, string initXml) :
            base(id, applicationName, initXml)
        {
            InitializeComponent();

            _logWriter = new TraceLogger();
            _isTimerRunning = false;
        }

        protected override void DesktopReady()
        {
            base.DesktopReady();
        }

        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            if (string.Compare(args.Action, "StartVeteranSnapShot", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region StartVeteranSnapShot

                _veteranSnapshotId = Guid.Empty;

                _sb = new StringBuilder();
                _sb.AppendLine("VeteranSnapShot Log:");

                var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
                _idproofid = Utility.GetAndRemoveParameter(parms, "idproofid");

                var findInterval = int.TryParse(Utility.GetAndRemoveParameter(parms, "interval"), out _interval);
                if (!findInterval)
                {
                    _interval = 1;
                }

                var finMaxAttemps = int.TryParse(Utility.GetAndRemoveParameter(parms, "maxattempts"), out _maxattemps);
                if (!finMaxAttemps)
                {
                    _maxattemps = 15;
                }

                _dispatch = Utility.GetAndRemoveParameter(parms, "DispatcherPriority");
                if (string.IsNullOrEmpty(_dispatch))
                {
                    _dispatch = "Background";
                }

                _actualattemps = 0;
                _isLocked = false;

                base.UpdateContext("VeteranSnapShot", "IdProofId", _idproofid);
                base.UpdateContext("VeteranSnapShot", "ActualAttemps", _actualattemps.ToString());
                base.UpdateContext("VeteranSnapShot", "MaxAttemps", _maxattemps.ToString());
                base.UpdateContext("VeteranSnapShot", "Interval", _interval.ToString());

                SearchProgress.Minimum = 1;
                SearchProgress.Maximum = _maxattemps;
                SearchProgress.Value = 1;
                SearchProgress.Foreground = System.Windows.Media.Brushes.Green;

                _sb.AppendLine("Interval: " + _interval.ToString());

                _timer = new DispatcherTimer(base.GetDispatchPrioroity(_dispatch));
                _timer.Tick += new EventHandler(timer_Tick);
                _timer.Interval = new TimeSpan(0, 0, 0, _interval);
                _timer.Start();
                _isTimerRunning = true;
                base.UpdateContext("VeteranSnapShot", "Timer", "Started");

                #endregion
            }
            else if (string.Compare(args.Action, "Refresh", StringComparison.OrdinalIgnoreCase) == 0)
            {

                var lockObj = new object();
                var timeout = 1;
                var lockTaken = false;

                #region Refresh

                try
                {

                    Monitor.TryEnter(lockObj, timeout, ref lockTaken);
                    if (lockTaken)
                    {
                        GetVeteranSnapShot();

                        //var parms = new Dictionary<string, string> { { "Mode", "Refresh" } };
                        //FireEvent("VeteranSnapShotLoadComplete", parms);

                        if (txtStatus.Text == "Success")
                        {
                            SearchProgress.Foreground = System.Windows.Media.Brushes.Green;
                        }

                    }
                }
                catch (Exception ex)
                {
                    var message = string.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                    if (ex.InnerException != null)
                    {
                        message += string.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                    }
                    message += string.Format("Call Stack:\r\n{0}", ex.StackTrace);
                    base.UpdateContext("VeteranSnapShot", "Message", message);
                    _logWriter.Log(message);
                }

                #endregion
            }
            else if (string.Compare(args.Action, "RefreshCADDAddress", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region RefreshCADDAddress

                var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
                _idproofid = Utility.GetAndRemoveParameter(parms, "idproofid");
                _interval = int.Parse(Utility.GetAndRemoveParameter(parms, "interval"));
                _maxattemps = int.Parse(Utility.GetAndRemoveParameter(parms, "maxattempts"));
                _actualattemps = 0;
                _isLocked = false;

                base.UpdateContext("VeteranSnapShot", "IdProofId", _idproofid);
                base.UpdateContext("VeteranSnapShot", "ActualAttemps", _actualattemps.ToString());
                base.UpdateContext("VeteranSnapShot", "MaxAttemps", _maxattemps.ToString());
                base.UpdateContext("VeteranSnapShot", "Interval", _interval.ToString());

                _timer = new DispatcherTimer();
                _timer.Tick += new EventHandler(timerMailingAddressChanged_Tick);
                _timer.Interval = new TimeSpan(0, 0, 0, _interval);
                _timer.Start();
                _isTimerRunning = true;
                base.UpdateContext("VeteranSnapShot", "Timer", "Started");

                try
                {
                    SafeDispatcher.BeginInvoke(new Action(() =>
                        {
                            GetMailingAddressChange();
                        }), new object[0]);
                }
                catch (Exception ex)
                {
                    var message = string.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                    if (ex.InnerException != null)
                    {
                        message += string.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                    }
                    message += string.Format("Call Stack:\r\n{0}", ex.StackTrace);
                    base.UpdateContext("VeteranSnapShot", "Message", message);
                    _logWriter.Log(message);
                }

                #endregion
            }
            else if (string.Compare(args.Action, "StopTimer", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region Stop Timer

                _veteranSnapshotId = Guid.Empty;
                if (_isTimerRunning)
                {
                    _timer.Stop();
                    _isTimerRunning = false;
                }

                #endregion
            }
            else if (string.Compare(args.Action, "ClearScreen", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region Clear Screen

                if (!_isTimerRunning)
                {
                    ClearScreen();
                }
                //clicks = 0;

                #endregion
            }
            else if (string.Compare(args.Action, "ResetClickCount", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region Reset Click Count

                btnRefresh.IsEnabled = true;
                _clicks = 0;

                #endregion
            }

            base.DoAction(args);
        }

        private delegate void UpdateProgressBarDelegate(
            System.Windows.DependencyProperty dp, object value);

        private void timer_Tick(object sender, EventArgs e)
        {
            SafeDispatcher.BeginInvoke(new Action(() =>
            {
                var updatePbDelegate = new UpdateProgressBarDelegate(SearchProgress.SetValue);

                _sb.AppendLine("Timer: Actual Attempts - " + _actualattemps.ToString() + " : " +
                               DateTime.Now.ToLongTimeString());

                if (_actualattemps >= _maxattemps)
                {
                    try
                    {
                        base.UpdateContext("VeteranSnapShot", "Timer", "Stopped");
                        _timer.Stop();
                        _isTimerRunning = false;
                        base.UpdateContext("VeteranSnapShot", "Message", "Veteran Snap Shot timed out");
                        _logWriter.Log("Veteran Snap Shot timed out");

                        var value = _maxattemps;

                        SearchProgress.Foreground = System.Windows.Media.Brushes.Red;
                        base.UpdateContext("VeteranSnapShot", "Log", _sb.ToString());

                        SafeDispatcher.BeginInvoke(new Action(() =>
                        {
                            FireEvent("VeteranSnapShotLoadComplete");
                        }), new object[0]);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    _sb.AppendLine("Is Locked : " + _isLocked.ToString());

                    if (_isLocked) return;
                    _isLocked = true;
                    _sb.AppendLine("Starts - " + DateTime.Now.ToLongTimeString());

                    try
                    {
                        var value = 0d;

                        var hasStopped = GetVeteranSnapShot();

                        if (hasStopped)
                        {
                            value = _maxattemps;
                        }
                        else
                        {
                            value = SearchProgress.Value;
                            value += 1;
                        }

                        //Dispatcher.Invoke(updatePbDelegate,
                        //    System.Windows.Threading.DispatcherPriority.Background,
                        //    new object[] { ProgressBar.ValueProperty, value });

                        SafeDispatcher.BeginInvoke(new Action(() =>
                        {
                            UpdateProgressBar(value);
                        }), new object[0]);

                        if (hasStopped)
                        {
                            SafeDispatcher.BeginInvoke(new Action(() =>
                            {
                                FireEvent("VeteranSnapShotLoadComplete");
                            }), new object[0]);
                        }
                    }
                    catch (Exception ex)
                    {
                        var message = string.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message,
                            ex.Source);
                        if (ex.InnerException != null)
                        {
                            message += string.Format("Inner Exception: {0}\r\n\r\n",
                                ex.InnerException.Message);
                        }
                        message += string.Format("Call Stack:\r\n{0}", ex.StackTrace);
                        base.UpdateContext("VeteranSnapShot", "Message", message);
                        _logWriter.Log(message);
                    }

                    _sb.AppendLine("Ends - " + DateTime.Now.ToLongTimeString());
                    _isLocked = false;
                }

            }), new object[0]);
        }

        private void timerMailingAddressChanged_Tick(object sender, EventArgs e)
        {
            SafeDispatcher.BeginInvoke(new Action(() =>
            {
                if (_actualattemps >= _maxattemps)
                {
                    base.UpdateContext("VeteranSnapShot", "Timer", "Stopped");
                    _timer.Stop();
                    _isTimerRunning = false;
                    base.UpdateContext("VeteranSnapShot", "Message", "Veteran Snap Shot timed out");
                    _logWriter.Log("Veteran Snap Shot timed out");
                    return;
                }

                if (_isLocked) return;

                _isLocked = true;

                try
                {
                    GetMailingAddressChange();
                }
                catch (Exception ex)
                {
                    var message = string.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                    if (ex.InnerException != null)
                    {
                        message += string.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                    }
                    message += string.Format("Call Stack:\r\n{0}", ex.StackTrace);
                    base.UpdateContext("VeteranSnapShot", "Message", message);
                    _logWriter.Log(message);
                }

                _isLocked = false;

            }), new object[0]);
        }

        private bool GetVeteranSnapShot()
        {
            var hasStopped = false;
            Entity result = null;

            _actualattemps++;
            base.UpdateContext("VeteranSnapShot", "ActualAttemps", _actualattemps.ToString());

            var resp = CallVeteranSnapShotAction();

            _dataIssue = Convert.ToBoolean(resp["DataIssue"]);
            _timeOut = Convert.ToBoolean(resp["Timeout"]);
            _exception = Convert.ToBoolean(resp["Exception"]);

            if (_exception || _dataIssue || _timeOut)
            {
                _responseMessage = resp["ResponseMessage"].ToString();
                base.UpdateContext("VeteranSnapShot", "ResponseException", _exception.ToString());
                base.UpdateContext("VeteranSnapShot", "ResponseDataIssue", _dataIssue.ToString());
                base.UpdateContext("VeteranSnapShot", "ResponseTimeOut", _timeOut.ToString());
                base.UpdateContext("VeteranSnapShot", "ResponseMessage", _responseMessage);

                return false;
            }

            _veteranSnapshotId = new Guid(resp["VeteranSnapshotId"].ToString());
            base.UpdateContext("VeteranSnapShot", "VeteranSnapshotId", _veteranSnapshotId.ToString());


            result = resp["SnapShot"] as Entity;

            //if (result != null && result.GetAttributeValue<string>("udo_integrationstatus") == "Success")
            //{
            //    base.UpdateContext("VeteranSnapShot", "Timer", "Stopped");
            //    _timer.Stop();
            //    _isTimerRunning = false;

            //    hasStopped = true;
            //    base.UpdateContext("VeteranSnapShot", "Log", _sb.ToString());

            //    FireEvent("VeteranSnapShotLoadComplete");
            //}

            if (result != null)
            {
                var parms = new Dictionary<string, string>();

                var lastName = result.GetAttributeValue<string>("udo_lastname");
                var firstName = result.GetAttributeValue<string>("udo_firstname");
                var phoneNumber = result.GetAttributeValue<string>("udo_phonenumber");

                parms.Add("udo_lastname", lastName);
                parms.Add("udo_firstname", firstName);
                parms.Add("udo_phonenumber", phoneNumber);


                txtName.Text = result.GetAttributeValue<string>("udo_lastname") + ", " +
                               result.GetAttributeValue<string>("udo_firstname");





                txtSSN.Text = result.GetAttributeValue<string>("udo_ssn");
                txtFileNumber.Text = result.GetAttributeValue<string>("udo_filenumber");
                txtBOS.Text = result.GetAttributeValue<string>("udo_branchofservice");
                txtRank.Text = result.GetAttributeValue<string>("udo_rank");

                if (result.Contains("udo_soj"))
                {
                    txtSOJ.Text = resp["SOJ"].ToString();
                }

                txtCOD.Text = result.GetAttributeValue<string>("udo_characterofdischarge");
                txtPOA.Text = result.GetAttributeValue<string>("udo_poa");
                txtDOB.Text = result.GetAttributeValue<string>("udo_birthdatestring");
                txtGender.Text = result.GetAttributeValue<string>("udo_gender");
                txtDOD.Text = result.GetAttributeValue<string>("udo_dateofdeath");
                //txtFlashes.Text = result.GetAttributeValue<string>("udo_flashes");
                txtFidicuary.Text = result.GetAttributeValue<string>("udo_cfidstatus");
                txtPersonOrgName.Text = result.GetAttributeValue<string>("udo_cfidpersonorgname");
                txtSCCombinedRating.Text = result.GetAttributeValue<string>("udo_sccombinedrating");
                txtSCCombinedDegree.Text = result.GetAttributeValue<string>("udo_nsccombineddegree");
                txtAwardType.Text = result.GetAttributeValue<string>("udo_awardtype");
                txtPayStatus.Text = result.GetAttributeValue<string>("udo_paymentstatus");
                txtLastPaidDate.Text = result.GetAttributeValue<string>("udo_lastpaiddate");
                txtNextScheduledPayDate.Text = result.GetAttributeValue<string>("udo_nextpaiddate");
                txtNextAmount.Text = result.GetAttributeValue<string>("udo_nextamount");
                txtAmount.Text = result.GetAttributeValue<string>("udo_amount");
                txtPendingClaims.Text = result.GetAttributeValue<string>("udo_pendingclaims");
                txtPendingAppeals.Text = result.GetAttributeValue<string>("udo_pendingappeals");
                txtMailingAddress.Text = result.GetAttributeValue<string>("udo_mailingaddress");

                var sb = new StringBuilder();

                if (result.Contains("udo_lastcalldate"))
                {
                    sb.Append(result.GetAttributeValue<string>("udo_lastcalldate") + " ");
                }

                if (result.Contains("udo_lastcalltime"))
                {
                    sb.Append(result.GetAttributeValue<string>("udo_lastcalltime") + " ");
                }

                if (result.Contains("udo_type") && result.GetAttributeValue<EntityReference>("udo_type") != null)
                {
                    sb.Append(result.GetAttributeValue<EntityReference>("udo_type").Name + " ");
                }

                if (result.Contains("udo_subtype") && result.GetAttributeValue<EntityReference>("udo_subtype") != null)
                {
                    sb.Append(result.GetAttributeValue<EntityReference>("udo_subtype").Name + " ");
                }
                txtLastPhoneCallHistory.Text = sb.ToString();
                txtStatus.Text = result.GetAttributeValue<string>("udo_integrationstatus");

                base.UpdateContext("Veteran", "udo_lastname", lastName);
                base.UpdateContext("Veteran", "udo_firstname", firstName);

                if (!string.IsNullOrEmpty(phoneNumber))
                    base.UpdateContext("Veteran", "udo_phonenumber", phoneNumber);
                else
                    base.UpdateContext("Veteran", "udo_phonenumber", "");

                base.UpdateContext("VeteranSnapShot", "FullName", txtName.Text);
                base.UpdateContext("VeteranSnapShot", "udo_phonenumber", phoneNumber);

                if (result.GetAttributeValue<string>("udo_integrationstatus") == "Success")
                {
                    base.UpdateContext("VeteranSnapShot", "Timer", "Stopped");
                    _timer.Stop();
                    _isTimerRunning = false;

                    hasStopped = true;
                    base.UpdateContext("VeteranSnapShot", "Log", _sb.ToString());
                }

                if (!hasStopped)
                {
                    SafeDispatcher.BeginInvoke(new Action(() =>
                    {
                        FireEvent("UpdateVeteranName", parms);
                    }), new object[0]);
                }

                //if (!hasStopped)
                //{
                //    SafeDispatcher.BeginInvoke(new Action(() =>
                //    {
                //        FireEvent("UpdateVeteranName", parms);
                //    }), new object[0]);
                //}
                //else
                //{
                //    SafeDispatcher.BeginInvoke(new Action(() =>
                //        {
                //            FireEvent("VeteranSnapShotLoadComplete");
                //        }), new object[0]);
                //}
            }

            return hasStopped;
        }

        private void GetMailingAddressChange()
        {
            _actualattemps++;
            base.UpdateContext("VeteranSnapShot", "ActualAttemps", _actualattemps.ToString());

            var resp = CallVeteranSnapShotAction();

            _dataIssue = Convert.ToBoolean(resp["DataIssue"]);
            _timeOut = Convert.ToBoolean(resp["Timeout"]);
            _exception = Convert.ToBoolean(resp["Exception"]);

            if (_exception || _dataIssue || _timeOut)
            {
                _responseMessage = resp["ResponseMessage"].ToString();
                base.UpdateContext("VeteranSnapShot", "ResponseException", _exception.ToString());
                base.UpdateContext("VeteranSnapShot", "ResponseDataIssue", _dataIssue.ToString());
                base.UpdateContext("VeteranSnapShot", "ResponseTimeOut", _timeOut.ToString());
                base.UpdateContext("VeteranSnapShot", "ResponseMessage", _responseMessage);

                return;
            }

            var veteranSnapSnot = resp["SnapShot"] as Entity;

            //var veteranSnapSnot = RetrieveMultipleVeteranSnapshot(MailingAddressColumns);
            if (veteranSnapSnot == null) return;

            if (veteranSnapSnot.GetAttributeValue<string>("udo_mailingaddress") != txtMailingAddress.Text)
            {
                base.UpdateContext("VeteranSnapShot", "Timer", "Stopped");
                _timer.Stop();
                _isTimerRunning = false;
            }

            txtMailingAddress.Text = veteranSnapSnot.GetAttributeValue<string>("udo_mailingaddress");
        }

        private OrganizationResponse CallVeteranSnapShotAction()
        {
            var req = new OrganizationRequest("udo_GetVeteranSnapshot");

            req["ParentEntityReference"] = new EntityReference("udo_idproof", new Guid(_idproofid));
            return base.Execute(req);
        }

        //private EntityCollection RetrieveMultipleVeteranSnapshot(ColumnSet columnSet)
        //{
        //    var qe = new QueryExpression
        //    {
        //        TopCount = 1,
        //        EntityName = "udo_veteransnapshot",
        //        ColumnSet = columnSet
        //    };

        //    var fe1 = new FilterExpression(LogicalOperator.And);
        //    fe1.AddCondition("udo_idproofid", ConditionOperator.Equal, new Guid(_idproofid));
        //    qe.Criteria.AddFilter(fe1);

        //    var veteranSnapSnot = base.RetrieveMultiple(qe);

        //    return veteranSnapSnot;
        //}

        private void BtnRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            var lockObj = new object();
            var timeout = 1;
            var lockTaken = false;

            try
            {
                Monitor.TryEnter(lockObj, timeout, ref lockTaken);
                if (lockTaken)
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    btnRefresh.IsEnabled = false;

                    GetVeteranSnapShot();

                    var parms = new Dictionary<string, string> { { "Mode", "Refresh" } };
                    FireEvent("VeteranSnapShotLoadComplete", parms);

                    if (txtStatus.Text == "Success")
                    {
                        SearchProgress.Foreground = System.Windows.Media.Brushes.Green;
                    }

                    btnRefresh.IsEnabled = true;
                }

            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(lockObj);
                }

                Mouse.OverrideCursor = Cursors.Arrow;
            }


            e.Handled = true;
        }

        //catch (Exception ex)
        //{
        //    var message = string.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
        //    if (ex.InnerException != null)
        //    {
        //        message += string.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
        //    }
        //    message += string.Format("Call Stack:\r\n{0}", ex.StackTrace);
        //    base.UpdateContext("VeteranSnapShot", "Message", message);
        //    _logWriter.Log(message);
        //}
        //private void RunRefresh(object stateInfo)
        //{
        //    try
        //    {
        //        var parms = new Dictionary<string, string> { { "Mode", "Refresh" } };


        //        btnRefresh.IsEnabled = false;
        //        GetVeteranSnapShot();
        //        FireEvent("VeteranSnapShotLoadComplete", parms);
        //        btnRefresh.IsEnabled = true;


        //        if (txtStatus.Text == "Success")
        //        {
        //            SearchProgress.Foreground = System.Windows.Media.Brushes.Green;
        //        }
        //    }
        //    //catch (Exception ex)
        //    //{
        //    //    var message = string.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
        //    //    if (ex.InnerException != null)
        //    //    {
        //    //        message += string.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
        //    //    }
        //    //    message += string.Format("Call Stack:\r\n{0}", ex.StackTrace);
        //    //    base.UpdateContext("VeteranSnapShot", "Message", message);
        //    //    _logWriter.Log(message);
        //    //}
        //    finally
        //    {
        //        lock (m_lock)
        //        {
        //            m_isRunning = false;

        //        }
        //    }
        //}


        private void UpdateControlBackground(string newValue, string oldValue, Control textBox)
        {
            textBox.Background = newValue != oldValue ? new SolidColorBrush(Colors.Khaki) : new SolidColorBrush(Colors.White);
        }

        private void ClearScreen()
        {
            _veteranSnapshotId = Guid.Empty;
            txtName.Text = string.Empty;
            txtSSN.Text = string.Empty;
            txtFileNumber.Text = string.Empty;
            txtBOS.Text = string.Empty;
            txtRank.Text = string.Empty;
            txtSOJ.Text = string.Empty;
            txtCOD.Text = string.Empty;
            txtPOA.Text = string.Empty;
            txtDOB.Text = string.Empty;
            txtGender.Text = string.Empty;
            txtDOD.Text = string.Empty;
            txtFidicuary.Text = string.Empty;
            txtPersonOrgName.Text = string.Empty;
            txtSCCombinedRating.Text = string.Empty;
            txtSCCombinedDegree.Text = string.Empty;
            txtAwardType.Text = string.Empty;
            txtPayStatus.Text = string.Empty;
            txtLastPaidDate.Text = string.Empty;
            txtAmount.Text = string.Empty;
            txtPendingClaims.Text = string.Empty;
            txtPendingAppeals.Text = string.Empty;
            txtMailingAddress.Text = string.Empty;
            txtLastPhoneCallHistory.Text = string.Empty;
            txtStatus.Text = string.Empty;

            SearchProgress.Foreground = System.Windows.Media.Brushes.Green;
            UpdateProgressBar(0);
        }

        private void UpdateProgressBar(double value)
        {
            var updatePbDelegate = new UpdateProgressBarDelegate(SearchProgress.SetValue);
            Dispatcher.BeginInvoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });
        }

        protected override void SafeDispatcherUnhandledExceptionHandler(object sender, SafeDispatcherUnhandledExceptionEventArgs ex)
        {
            ex.Handled = true;

            base.SafeDispatcherUnhandledExceptionHandler(sender, ex);
        }
    }
}