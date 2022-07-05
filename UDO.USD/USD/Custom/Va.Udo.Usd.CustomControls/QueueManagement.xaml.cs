using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using System;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Va.Udo.Usd.CustomControls.Shared;
using AuthenticationType = Microsoft.Xrm.Tooling.Connector.AuthenticationType;

namespace Va.Udo.Usd.CustomControls
{
    /// <summary>
    /// Interaction logic for RoleSecurity.xaml
    /// </summary>
    public partial class QueueManagement : BaseHostedControlCommon
    {
        private static readonly ColumnSet QueueItemColumns = new ColumnSet("queueitemid", "workerid", "objectid", "udo_sensitivitylevel");
        /// <summary>
        /// Log writer
        /// </summary>
        private readonly TraceLogger _logWriter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicationName"></param>
        /// <param name="initXml"></param>
        public QueueManagement(Guid id, string applicationName, string initXml) :
            base(id, applicationName, initXml)
        {
            InitializeComponent();
            _logWriter = new TraceLogger("QueueManagement");
        }

        #region USD Methods

        protected override void DesktopReady()
        {
            base.DesktopReady();
        }

        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {

            var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
            var queueItemId = "";
            var datanodename = "";

            datanodename = Utility.GetAndRemoveParameter(parms, "datanodename");
            if (string.IsNullOrEmpty(datanodename))
            {
                datanodename = "SelectedQueueItem";
            }

            queueItemId = Utility.GetAndRemoveParameter(parms, "queueitemid");

            switch (args.Action.ToLower())
            {
                case "pickfromqueue":

                    #region Pick From Queue

                    var workerId = Utility.GetAndRemoveParameter(parms, "workerid");

                    var removeQueueItemValue = Utility.GetAndRemoveParameter(parms, "removequeueitem");
                    if (string.IsNullOrEmpty(removeQueueItemValue))
                    {
                        removeQueueItemValue = "N";
                    }

                    try
                    {
                        if (!string.IsNullOrEmpty(queueItemId) && !string.IsNullOrEmpty(workerId))
                        {
                            PickFromQueue(datanodename, queueItemId, workerId, removeQueueItemValue);
                        }
                        else
                        {
                            UpdateContext(datanodename, "Selected", "N");
                            UpdateContext(datanodename, "Released", "N");
                            UpdateContext(datanodename, "Removed", "N");
                            UpdateContext(datanodename, "ErrorOccurred", "Y");
                            UpdateContext(datanodename, "ErrorOccurredIn", "PickFromQueue");
                            UpdateContext(datanodename, "ExceptionMessage", "Missing required information to pick queue item");
                        }

                    }
                    catch (Exception ex)
                    {
                        _logWriter.Log(ex);
                        UpdateContext(datanodename, "Selected", "N");
                        UpdateContext(datanodename, "ErrorOccurred", "Y");
                        UpdateContext(datanodename, "ErrorOccurredIn", "PickFromQueue");
                        UpdateContext(datanodename, "ExceptionMessage", ex.Message);
                    }

                    #endregion

                    break;
                case "releasebacktoqueue":

                    #region Release Back To Queue

                    try
                    {
                        if (!string.IsNullOrEmpty(queueItemId))
                        {
                            ReleaseBackToQueue(datanodename, queueItemId);
                        }
                        else
                        {
                            UpdateContext(datanodename, "ErrorOccurred", "Y");
                            UpdateContext(datanodename, "ErrorOccurredIn", "ReleaseBackToQueue");
                            UpdateContext(datanodename, "ExceptionMessage", "Missing required information to pick queue item");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logWriter.Log(ex);
                        UpdateContext(datanodename, "ErrorOccurred", "Y");
                        UpdateContext(datanodename, "ErrorOccurredIn", "ReleaseBackToQueue");
                        UpdateContext(datanodename, "ExceptionMessage", ex.Message);
                    }

                    #endregion

                    break;
                case "removefromqueue":

                    #region Remove From Queueu

                    workerId = Utility.GetAndRemoveParameter(parms, "workerid");

                    try
                    {
                        if (!string.IsNullOrEmpty(queueItemId) && !string.IsNullOrEmpty(workerId))
                        {
                            RemoveFromQueue(datanodename, queueItemId, workerId);
                        }
                        else
                        {
                            UpdateContext(datanodename, "ErrorOccurred", "Y");
                            UpdateContext(datanodename, "ErrorOccurredIn", "RemoveFromQueue");
                            UpdateContext(datanodename, "ExceptionMessage", "Missing required information to pick queue item");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logWriter.Log(ex);
                        UpdateContext(datanodename, "ErrorOccurred", "Y");
                        UpdateContext(datanodename, "ErrorOccurredIn", "RemoveFromQueue");
                        UpdateContext(datanodename, "ExceptionMessage", ex.Message);
                    }

                    #endregion

                    break;
                case "updatequeueitemselection":

                    #region Update Queue Item Selection

                    try
                    {
                        var queueItemPre = RetrieveQueueItemId(new Guid(queueItemId));
                        if (queueItemPre.Contains("workerid"))
                        {
                            UpdateContext(datanodename, "Selected", "Y");
                        }
                        else
                        {
                            UpdateContext(datanodename, "Selected", "N");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logWriter.Log(ex);
                        UpdateContext(datanodename, "ErrorOccurred", "Y");
                        UpdateContext(datanodename, "ErrorOccurredIn", "UpdateQueueItemSelection");
                        UpdateContext(datanodename, "ExceptionMessage", ex.Message);
                    }

                    #endregion

                    break;
                default:

                    #region Run Base Actions

                    base.DoAction(args);

                    #endregion

                    break;
            }
        }

        #endregion

        #region Queue Features

        public void PickFromQueue(string datanodename, string queueItemId, string userId, string removeQueueItemValue)
        {
            var removeQueueItem = false;
            var queueItemSelection = false;

            var queueItemPre = RetrieveQueueItemId(new Guid(queueItemId));

            OptionSetValue sensitivityLevel = null;
            if (queueItemPre.Contains("udo_sensitivitylevel")) // QueueItem Sensitivity Level
            {
                sensitivityLevel = queueItemPre.GetAttributeValue<OptionSetValue>("udo_sensitivitylevel");
            }

            if (queueItemPre.Contains("workerid")) // QueueItem already picked and assigned to worker
            {
                if (string.Equals(userId.ToLower(),
                    queueItemPre.GetAttributeValue<EntityReference>("workerid").Id.ToString().ToLower(),
                    StringComparison.Ordinal))
                {
                    queueItemSelection = true;
                }
            }
            else
            {
                if (removeQueueItemValue == "Y")
                    removeQueueItem = true;

                var pickFromQueue = new PickFromQueueRequest
                {
                    QueueItemId = new Guid(queueItemId),
                    WorkerId = new Guid(userId),
                    RemoveQueueItem = removeQueueItem
                };

                base.PickFromQueue(pickFromQueue);
                //if (_client.CrmInterface.ActiveAuthenticationType == AuthenticationType.OAuth)
                //    _client.CrmInterface.OrganizationWebProxyClient.Execute(pickFromQueue);
                //else
                //    _client.CrmInterface.OrganizationServiceProxy.Execute(pickFromQueue);
            }

            var queueItemPost = RetrieveQueueItemId(new Guid(queueItemId));

            if (queueItemPost.Contains("workerid")) // QueueItem already picked and assigned to worker
            {
                if (string.Equals(userId.ToLower(),
                    queueItemPost.GetAttributeValue<EntityReference>("workerid").Id.ToString().ToLower(),
                    StringComparison.Ordinal))
                {
                    queueItemSelection = true;
                }
            }

            EntityReference interactionId = null;
            if (queueItemPost.Contains("objectid")) // Should be the Interaction Id
            {
                interactionId = queueItemPost.GetAttributeValue<EntityReference>("objectid");
            }

            UpdateContext(datanodename, "Selected", queueItemSelection ? "Y" : "N");
            UpdateContext(datanodename, "QueueItemId", queueItemId);

            if (interactionId != null && interactionId.LogicalName == "udo_interaction")
                UpdateContext(datanodename, "InteractionId", interactionId.Id.ToString());

            if (sensitivityLevel != null)
                UpdateContext(datanodename, "SensitivityLevel", sensitivityLevel.Value.ToString());

            UpdateContext(datanodename, "WorkerId", userId);
            UpdateContext(datanodename, "Released", "N");
            UpdateContext(datanodename, "Removed", "N");
            UpdateContext(datanodename, "ErrorOccurred", "N");
            UpdateContext(datanodename, "ErrorOccurredIn", "");
            UpdateContext(datanodename, "ExceptionMessage", "");

        }

        public void ReleaseBackToQueue(string datanodename, string queueItemId)
        {
            try
            {
                var queueItemPre = RetrieveQueueItemId(new Guid(queueItemId));

                if (!queueItemPre.Contains("workerid")) return;

                var releaseBackToQueue = new ReleaseToQueueRequest
                {
                    QueueItemId = new Guid(queueItemId)
                };

                base.Execute(releaseBackToQueue);

                UpdateContext(datanodename, "Selected", "N");
                UpdateContext(datanodename, "Released", "Y");
                UpdateContext(datanodename, "Removed", "N");
                UpdateContext(datanodename, "ErrorOccurred", "N");
                UpdateContext(datanodename, "ErrorOccurredIn", "");
                UpdateContext(datanodename, "ExceptionMessage", "");
            }
            catch (Exception ex)
            {
                _logWriter.Log(ex);
                UpdateContext(datanodename, "ErrorOccurred", "Y");
                UpdateContext(datanodename, "ErrorOccurredIn", "ReleaseBackToQueue");
                UpdateContext(datanodename, "ExceptionMessage", ex.Message);
            }
        }

        public void RemoveFromQueue(string datanodename, string queueItemId, string userId)
        {

            var queueItem = RetrieveQueueItemId(new Guid(queueItemId));

            if (queueItem.Contains("workerid")) // QueueItem already picked and assigned to worker
            {
                if (string.Equals(userId.ToLower(),
                    queueItem.GetAttributeValue<EntityReference>("workerid").Id.ToString().ToLower(),
                    StringComparison.Ordinal))
                {
                    var removeFromQueueRequest = new RemoveFromQueueRequest
                    {
                        QueueItemId = new Guid(queueItemId)
                    };

                    if (_client.CrmInterface.ActiveAuthenticationType == AuthenticationType.OAuth)
                        _client.CrmInterface.OrganizationWebProxyClient.Execute(removeFromQueueRequest);
                    else
                        _client.CrmInterface.OrganizationServiceProxy.Execute(removeFromQueueRequest);

                }
            }

            UpdateContext(datanodename, "Selected", "N");
            UpdateContext(datanodename, "Released", "N");
            UpdateContext(datanodename, "Removed", "Y");
            UpdateContext(datanodename, "ErrorOccurred", "N");
            UpdateContext(datanodename, "ErrorOccurredIn", "");
            UpdateContext(datanodename, "ExceptionMessage", "");
        }

        #endregion

        #region CRM Retrieve Features

        public Entity RetrieveQueueItemId(Guid queueitemid)
        {
            return RetrieveQueueItemId(queueitemid, QueueItemColumns);
        }

        public Entity RetrieveQueueItemId(Guid queueitemid, ColumnSet columnSet)
        {
            return Retrieve("queueitem", queueitemid, columnSet);
        }

        #endregion
    }
}
