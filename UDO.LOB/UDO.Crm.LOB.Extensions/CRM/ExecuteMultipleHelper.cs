using Microsoft.Pfe.Xrm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDO.LOB.Extensions.Logging;
using System.Diagnostics;

namespace UDO.LOB.Extensions
{
    public static class ExecuteMultipleHelper
    {
        //bool impersonate = false;

        /// <summary>
        ///     Reusable instance of OrganizationServiceManager
        /// </summary>
        public static ExecuteMultipleHelperResponse ExecuteMultiple(IOrganizationService service,
            IEnumerable<OrganizationRequest> requests, string orgName, Guid userId, bool debug,
            bool continueOnError, int batchSize)
        {

            bool impersonate = false;

            if (userId != Guid.Empty)
            {
                impersonate = true;
            }


            return ExecuteMultiple(service, requests, orgName, userId, debug, continueOnError, batchSize, false, impersonate);
        }

        public static ExecuteMultipleHelperResponse ExecuteMultiple(IOrganizationService service,
            IEnumerable<OrganizationRequest> requests, string orgName, Guid userId, bool debug,
            bool continueOnError, int batchSize, bool returnResponses, bool impersonate)
        {
            ConnectionManager manager = null;

            try
            {
                #region Initialize the connection, get the configuration, and set defaults

                TimeTracker timer = new TimeTracker();
                timer.Restart();
                timer.MarkStart("Start of Execute Multiple");

                string caller = MethodInfo.GetCallingMethod(true).GetClassCallerPath(false, true);
                string method = String.Format("{0} > ExecuteMultiple", caller);
                bool customBatchSize = false;

                // NP: 6/28 OrganizationWebProxyClient serviceProxy = service as Microsoft.Xrm.Sdk.WebServiceClient.OrganizationWebProxyClient;
                //var serviceProxy = service as OrganizationServiceProxy;
                var serviceProxy = service as Microsoft.Xrm.Tooling.Connector.CrmServiceClient;
                if (serviceProxy == null)
                    return new ExecuteMultipleHelperResponse
                    {
                        IsFaulted = true,
                        ErrorDetail = "Error: No Service Proxy",
                        FriendlyDetail = "There was an error connecting to CRM."
                    };

                #region Setup Responses

                ConcurrentQueue<ExecuteMultipleHelperResponse> responses = new ConcurrentQueue<ExecuteMultipleHelperResponse>();

                ExecuteMultipleHelperResponse rootResponse = new ExecuteMultipleHelperResponse
                {
                    LogDetail = "Execute Multiple: START\r\n"
                };

                #endregion
                manager = ConnectionCache.ConnectManager;
                if (batchSize <= 0)
                {
                    //batchSize = 10;
                    batchSize = manager.ExecuteMultipleHelperSettings.BatchSize;
                }
                else
                {
                    customBatchSize = true;
                }
                #endregion
                var gwatch = Stopwatch.StartNew();
                #region Truncate Requests
                timer.MarkStart("Starting to Truncate Requests.");
                if (debug) rootResponse.Log("Truncating Requests...");
                requests = TruncateHelper.TruncateRequests(requests, orgName, userId, debug, service);
                timer.MarkStop("Truncation Complete.");
                #endregion

                #region Create Batches
                IDictionary<string, ExecuteMultipleRequest> batches = requests.AsBatches(batchSize, continueOnError, returnResponses);
                string detail = String.Format("Batches Separated into {0} batches.", batches.Count());
                int batchid = 1;
                foreach (KeyValuePair<string, ExecuteMultipleRequest> batch in batches)
                {
                    detail += String.Format("\r\nBatch {0}: {1} Items", batchid, batch.Value.Requests.Count());
                    batchid++;
                }
                detail += "\r\nTime Elapsed: ";
                timer.MarkStop(detail);
                detail += gwatch.ElapsedMilliseconds.ToString();
                LogHelper.LogInfo($"{MethodInfo.GetThisMethod().Method} :: " + detail);
                #endregion

                #region Build OrganizationRequests List (orgRequests)

                List<OrganizationRequest> orgRequests = new List<OrganizationRequest>();
                //if (batches.Count() <= manager.ExecuteMultipleHelperSettings.MinBatches && !customBatchSize)
                if (batches.Count() <= 1 && !customBatchSize)
                {
                    // Regular Requests
                    orgRequests.AddRange(requests);
                }
                else
                {
                    orgRequests = batches.Select(o => (OrganizationRequest)o.Value).ToList();
                }
                timer.MarkStop("Requests Setup.");
                #endregion
                
                try
                {
                    IEnumerable<OrganizationResponse> orgResponses = null;
                    if (manager.ExecuteMultipleHelperSettings.ProcessMethod == ProcessMethodType.PFELibrary)
                    {

                        if (impersonate)
                            serviceProxy.CallerId = userId;
                        else
                            serviceProxy.CallerId = Guid.Empty;

                        orgResponses = ExecuteMultiplePFE(orgName, serviceProxy, orgRequests, responses, impersonate);
                    }
                    else if (manager.ExecuteMultipleHelperSettings.ProcessMethod == ProcessMethodType.Direct)
                    {

                        if (impersonate)
                            serviceProxy.CallerId = userId;
                        else
                            serviceProxy.CallerId = Guid.Empty;

                        orgResponses = ExecuteMultipleDirect(orgName, serviceProxy, orgRequests, responses, impersonate);
                    }

                    timer.MarkStop("Execute Multiple Complete.");

                    if (orgResponses != null)
                    {

                        responses.Enqueue(new ExecuteMultipleHelperResponse(orgResponses, orgRequests));
                    }
                    rootResponse.ChildResponses = responses.ToList();
                    rootResponse.CombineChildResponses();

                    timer.MarkStop("Responses Processed.");

                    #region Stop Timer
                    //if (debug)
                    //{
                    //    LogHelper.LogTiming(Guid.Empty.ToString(), orgName, debug, userId, Guid.Empty, null, null, timer.GetDurations(true), null, timer.ElapsedMilliseconds);
                    //    // TODO: Trace here instead of invoking API call??
                    //}

                    timer.Stop();
                    #endregion
                }
                catch (Exception processException)
                {
                    AggregateException ae = processException as AggregateException;
                    if (ae != null)
                        foreach (Exception ex in ae.InnerExceptions)
                            LogHelper.LogError(orgName, userId, caller + " In ExecuteMultipleHelper Main Try", ex);
                    LogHelper.LogError(orgName, userId, caller, processException);
                    return null;
                }
                finally
                {
                    impersonate = false;
                }
                return rootResponse;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(orgName, userId, "ExecuteMultiple", ex.Message);
                throw ex;
            }
        }

        public static ExecuteMultipleHelperResponse ExecuteMultiple(IOrganizationService service,
            IEnumerable<OrganizationRequest> requests, string orgName, Guid userId, string messageId, bool debug,
            bool continueOnError, int batchSize, bool returnResponses, bool impersonate)
        {
            ConnectionManager manager = null;

            try
            {
                #region Initialize the connection, get the configuration, and set defaults

                TimeTracker timer = new TimeTracker();
                timer.Restart();
                timer.MarkStart("Start of Execute Multiple");

                string caller = MethodInfo.GetCallingMethod(true).GetClassCallerPath(false, true);
                string method = String.Format("{0} > ExecuteMultiple", caller);
                bool customBatchSize = false;

                // NP: 6/28 OrganizationWebProxyClient serviceProxy = service as Microsoft.Xrm.Sdk.WebServiceClient.OrganizationWebProxyClient;
                //var serviceProxy = service as OrganizationServiceProxy;
                var serviceProxy = service as Microsoft.Xrm.Tooling.Connector.CrmServiceClient;
                if (serviceProxy == null)
                    return new ExecuteMultipleHelperResponse
                    {
                        IsFaulted = true,
                        ErrorDetail = "Error: No Service Proxy",
                        FriendlyDetail = "There was an error connecting to CRM."
                    };

                #region Setup Responses

                ConcurrentQueue<ExecuteMultipleHelperResponse> responses = new ConcurrentQueue<ExecuteMultipleHelperResponse>();

                ExecuteMultipleHelperResponse rootResponse = new ExecuteMultipleHelperResponse
                {
                    LogDetail = "Execute Multiple: START\r\n"
                };

                #endregion

                if (batchSize <= 0)
                {
                    batchSize = 50;
                }
                else
                {
                    customBatchSize = true;
                }
                //batchSize = 50;
                //batchSize = manager.ExecuteMultipleHelperSettings.BatchSize;
                //else
                //    customBatchSize = true;
                #endregion
                var gwatch = Stopwatch.StartNew();
                #region Truncate Requests
                timer.MarkStart("Starting to Truncate Requests.");
                if (debug) rootResponse.Log("Truncating Requests...");
                requests = TruncateHelper.TruncateRequests(requests, orgName, userId, debug, service);
                timer.MarkStop("Truncation Complete.");
                #endregion

                #region Create Batches
                IDictionary<string, ExecuteMultipleRequest> batches = requests.AsBatches(batchSize, continueOnError, returnResponses);
                string detail = String.Format("Batches Separated into {0} batches.", batches.Count());
                int batchid = 1;
                foreach (KeyValuePair<string, ExecuteMultipleRequest> batch in batches)
                {
                    detail += String.Format("\r\nBatch {0}: {1} Items", batchid, batch.Value.Requests.Count());
                    batchid++;
                }
                detail += "\r\nTime Elapsed: ";
                timer.MarkStop(detail);
                detail += gwatch.ElapsedMilliseconds.ToString();
                LogHelper.LogInfo($"{MethodInfo.GetThisMethod().Method} :: " + detail);
                #endregion



                #region Build OrganizationRequests List (orgRequests)

                List<OrganizationRequest> orgRequests = new List<OrganizationRequest>();
                //if (batches.Count() <= manager.ExecuteMultipleHelperSettings.MinBatches && !customBatchSize)
                if (batches.Count() <= 1 && !customBatchSize)
                {
                    // Regular Requests
                    orgRequests.AddRange(requests);
                }
                else
                {
                    orgRequests = batches.Select(o => (OrganizationRequest)o.Value).ToList();
                    // Execute Multiple Requeusts
                    /*var executeMultipleRequests = new ConcurrentBag<OrganizationRequest>();

                    #region Convert Batches into ExecuteMultipleRequest

                    Parallel.ForEach(batches, (batchCollection, loopstate, position) =>
                    {
                        var collection = new OrganizationRequestCollection();
                        collection.AddRange(batchCollection);

                        var request = new ExecuteMultipleRequest
                        {
                            Requests = collection,
                            Settings = new ExecuteMultipleSettings
                            {
                                ContinueOnError = continueOnError,
                                ReturnResponses = false //only error responses
                            }
                        };

                        executeMultipleRequests.Add(request);
                    });

                    #endregion

                    orgRequests = executeMultipleRequests.ToList();
                     */
                }
                timer.MarkStop("Requests Setup.");
                #endregion
                manager = ConnectionCache.ConnectManager;
                try
                {
                    IEnumerable<OrganizationResponse> orgResponses = null;
                    if (manager.ExecuteMultipleHelperSettings.ProcessMethod == ProcessMethodType.PFELibrary)
                    {
                        if (impersonate)
                            serviceProxy.CallerId = userId;
                        else
                            serviceProxy.CallerId = Guid.Empty;

                        orgResponses = ExecuteMultiplePFE(orgName, serviceProxy, orgRequests, responses, impersonate);
                    }
                    else if (manager.ExecuteMultipleHelperSettings.ProcessMethod == ProcessMethodType.Direct)
                    {
                        if (impersonate)
                            serviceProxy.CallerId = userId;
                        else
                            serviceProxy.CallerId = Guid.Empty;

                        orgResponses = ExecuteMultipleDirect(orgName, serviceProxy, orgRequests, responses, impersonate);
                    }

                    timer.MarkStop("Execute Multiple Complete.");

                    if (orgResponses != null)
                    {
                        responses.Enqueue(new ExecuteMultipleHelperResponse(orgResponses, orgRequests));
                    }
                    rootResponse.ChildResponses = responses.ToList();
                    rootResponse.CombineChildResponses();

                    timer.MarkStop("Responses Processed.");

                    #region Stop Timer
                    if (debug)
                    {
                        //LogHelper.LogTiming(messageId, orgName, debug, userId, Guid.Empty, null, null, timer.GetDurations(true), null, timer.ElapsedMilliseconds);
                        // TODO: Trace here instead of invoking API call??
                        #region Granularly Log Time
                        //IMessageBase log = new Servicebus.Logging.CRM.Messages.CreateCRMLogEntryAsyncRequest()
                        //{
                        //    OrganizationName = orgName,
                        //    UserId = userId,
                        //    crme_Method = method,
                        //    crme_GranularTiming = true,
                        //    crme_TransactionTiming = false,
                        //    crme_LogLevel = 935950005,
                        //    crme_ErrorMessage = timer.GetDurations(true),
                        //    crme_Sequence = 1,
                        //    crme_Duration = Convert.ToDecimal(timer.ElapsedMilliseconds),
                        //    crme_Debug = true,
                        //    crme_Name = "Execute Multiple Timing"
                        //};
                        //log.SendAsync(MessageProcessType.Local);

                        #endregion
                    }

                    timer.Stop();
                    #endregion
                }
                catch (Exception processException)
                {
                    AggregateException ae = processException as AggregateException;
                    if (ae != null)
                        foreach (Exception ex in ae.InnerExceptions)
                            LogHelper.LogError(orgName, userId, caller + " In ExecuteMultipleHelper Main Try", ex);
                    LogHelper.LogError(orgName, userId, caller, processException);
                    return null;
                }
                finally
                {
                    impersonate = false;
                }
                return rootResponse;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(orgName, userId, "ExecuteMultiple", ex.Message);
                throw ex;
            }
        }

        private static IEnumerable<OrganizationResponse> ExecuteMultiplePFE(string orgName,
            CrmServiceClient serviceProxy, List<OrganizationRequest> orgRequests,
            ConcurrentQueue<ExecuteMultipleHelperResponse> responses, bool impersonate)
        {
            ConnectionManager manager = null;

            try
            {
                #region Get OrganizationServiceManager
                manager = ConnectionCache.ConnectManager;
                if (manager.OrgServiceManager.ParallelProxy.MaxDegreeOfParallelism !=
                    manager.ExecuteMultipleHelperSettings.MaxDegreeOfParallelism)
                {
                    manager.OrgServiceManager.ParallelProxy.MaxDegreeOfParallelism =
                        manager.ExecuteMultipleHelperSettings.MaxDegreeOfParallelism;
                }

                #endregion

                #region Setup Proxy Options

                OrganizationServiceProxyOptions options = new OrganizationServiceProxyOptions();

                if (impersonate)
                    options.CallerId = serviceProxy.CallerId;

                #endregion

                ParallelOrganizationServiceProxy proxy = manager.OrgServiceManager.ParallelProxy;

                IEnumerable<OrganizationResponse> orgResponses = proxy.Execute<OrganizationRequest, OrganizationResponse>(orgRequests,
                    options,
                    (req, ex) =>
                    {
                        ExecuteMultipleHelperResponse response = new ExecuteMultipleHelperResponse();
                        response.Fault(string.Format("Error: {0}", ex.Message));

                        if (ex.InnerException != null)
                            response.Fault("Inner Exception: {0}", ex.InnerException.Message);
                        if (ex.StackTrace != null)
                            response.Fault("Stack Trace: {0}", ex.StackTrace);

                        responses.Enqueue(response);
                    });
                return orgResponses;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(orgName, Guid.Empty, "ExecuteMultiplePFE", ex.Message);
                throw ex;
            }
        }

        private static IEnumerable<OrganizationResponse> ExecuteMultiplePFEv2(string orgName,
            OrganizationWebProxyClient serviceProxy, List<OrganizationRequest> orgRequests,
            ConcurrentQueue<ExecuteMultipleHelperResponse> responses, bool impersonate)
        {
            ConnectionManager manager = null;

            try
            {
                #region Get OrganizationServiceManager
                manager = ConnectionCache.ConnectManager;
                if (manager.OrgServiceManager.ParallelProxy.MaxDegreeOfParallelism !=
                    manager.ExecuteMultipleHelperSettings.MaxDegreeOfParallelism)
                {
                    manager.OrgServiceManager.ParallelProxy.MaxDegreeOfParallelism =
                        manager.ExecuteMultipleHelperSettings.MaxDegreeOfParallelism;
                }

                #endregion

                #region Setup Proxy Options

                OrganizationServiceProxyOptions options = new OrganizationServiceProxyOptions();
                //{
                //    CallerId = serviceProxy.CallerId
                //};

                if (impersonate)
                    options.CallerId = serviceProxy.CallerId;

                #endregion

                ParallelOrganizationServiceProxy proxy = manager.OrgServiceManager.ParallelProxy;

                IEnumerable<OrganizationResponse> orgResponses = proxy.Execute<OrganizationRequest, OrganizationResponse>(orgRequests,
                    options,
                    (req, ex) =>
                    {
                        ExecuteMultipleHelperResponse response = new ExecuteMultipleHelperResponse();
                        response.Fault(string.Format("Error: {0}", ex.Message));

                        if (ex.InnerException != null)
                            response.Fault("Inner Exception: {0}", ex.InnerException.Message);
                        if (ex.StackTrace != null)
                            response.Fault("Stack Trace: {0}", ex.StackTrace);

                        responses.Enqueue(response);
                    });
                return orgResponses;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(orgName, Guid.Empty, "ExecuteMultiplePFEv2", ex.Message);
                throw ex;
            }
        }

        private static IEnumerable<OrganizationResponse> ExecuteMultipleDirect(string orgName,
            CrmServiceClient serviceProxy, IEnumerable<OrganizationRequest> orgRequests,
            ConcurrentQueue<ExecuteMultipleHelperResponse> responses, bool impersonate)
        {
            ConcurrentQueue<OrganizationResponse> orgResponses = new ConcurrentQueue<OrganizationResponse>();

            Parallel.ForEach(orgRequests,
                            new ParallelOptions { MaxDegreeOfParallelism = 100 },
                            () =>
                            {
                                #region connect to CRM

                                // OrganizationServiceProxy threadService;
                                CrmServiceClient threadService;
                                try
                                {
                                    // var commonFunctions = new CRMConnect();
                                    // threadService = commonFunctions.ConnectToCrm(orgName);

                                    threadService = ConnectionCache.GetProxy();
                                    // Commented as no references found
                                    // IOrganizationService organizationService = threadService as IOrganizationService; 

                                    if (impersonate)
                                        threadService.CallerId = serviceProxy.CallerId;
                                    else
                                        threadService.CallerId = Guid.Empty;

                                }
                                catch (Exception connectException)
                                {
                                    throw new Exception("Could not connect to CRM on this thread.", connectException);
                                }

                                #endregion

                                return threadService;
                            },
                            (request, loopState, threadService) =>
                            {
                                try
                                {
                                    orgResponses.Enqueue(threadService.Execute(request));
                                }
                                catch (Exception processException)
                                {
                                    #region Append Errors to Logs

                                    ExecuteMultipleHelperResponse response = new ExecuteMultipleHelperResponse();
                                    response.Fault("Error: {0}", processException.Message);
                                    if (processException.InnerException != null)
                                        response.Fault("Inner Exception: {0}", processException.InnerException.Message);
                                    if (processException.StackTrace != null)
                                        response.Fault("Stack Trace: {0}", processException.StackTrace);
                                    responses.Enqueue(response);

                                    #endregion
                                }
                                return threadService;
                            },
                            threadService =>
                            {
                                threadService.Dispose();
                            });
            return orgResponses;
        }

        /// <summary>
        ///     ExecuteMultipleInParallel: Public method that executes multiple OrganizationRequests
        ///     in parallel, allowing for higher throughput, and bulk message passing to the CRM Service
        ///     in parallel.
        /// </summary>
        /// <param name="service">CRM OrganizationService</param>
        /// <param name="requests">OrganizationRequestCollection</param>
        /// <param name="userId">User ID</param>
        /// <param name="debug">Debug: Default is True</param>
        /// <param name="continueOnError">Continue On Error: Default is True</param>
        /// <param name="orgName">Org Name</param>
        /// <returns>ExecuteMultipleHelperResponse</returns>
        public static ExecuteMultipleHelperResponse ExecuteMultiple(IOrganizationService service,
            IEnumerable<OrganizationRequest> requests, string orgName, Guid userId, bool debug = false,
            bool continueOnError = true)
        {
            // Most LOBs use this method.
            // BatchSize is the default batch size

            //timer start
            //if(debug)
            //{
            //    try
            //    {
            //        LogHelper.LogInfo($"Total Requests: {requests.Count<OrganizationRequest>()} \r\n UserId: {userId}");
            //        //Parallel.ForEach<OrganizationRequest>(requests, (req) =>
            //        foreach (var req in requests)
            //        {
            //            LogHelper.LogDebug(orgName, debug, userId, "ExecuteMultiple", 
            //                $"Request: {req.RequestName} Entity: [logicalname :{((Entity)req.Parameters["Target"]).LogicalName}, id:{((Entity)req.Parameters["Target"]).Id}]");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        LogHelper.LogError(orgName, userId, MethodInfo.GetCallingMethod().Method, ex);
            //    }
            //}
            
            //timer end
            return ExecuteMultiple(service, requests, orgName, userId, debug,
                continueOnError, 0);
        }

        /// <summary>
        ///     ExecuteMultiple: Executes multiple OrganizationRequests allowing for higher throughput,
        ///     and bulk message passing to the CRM Service
        /// </summary>
        /// <param name="service">CRM OrganizationService</param>
        /// <param name="multipleRequest">ExecuteMultipleRequest</param>
        /// <param name="userId">User ID</param>
        /// <param name="debug">Debug: Default is True</param>
        /// <param name="continueOnError">Continue On Error: Default is True</param>
        /// <param name="orgName">Organization Name</param>
        /// <returns>ExecuteMultipleHelperResponse</returns>
        public static ExecuteMultipleHelperResponse ExecuteMultiple(IOrganizationService service,
            ExecuteMultipleRequest multipleRequest, string orgName, Guid userId, bool debug = true,
            bool continueOnError = true)
        {
            return ExecuteMultiple(service, multipleRequest.Requests, orgName, userId, debug, continueOnError);
        }

        public static ExecuteMultipleHelperResponse ExecuteMultipleImpersonate(IOrganizationService service,
            IEnumerable<OrganizationRequest> requests, string orgName, Guid userId, bool debug = true,
            bool continueOnError = true)
        {
            return ExecuteMultiple(service, requests, orgName, userId, debug,
               continueOnError, 0, false, true);
        }
    }

    public class ExecuteMultipleHelperResponse
    {
        public ExecuteMultipleHelperResponse()
        {
            Requests = new List<OrganizationRequest>();
            Responses = new List<OrganizationResponse>();

            ErrorDetail = string.Empty;
            LogDetail = string.Empty;
            FriendlyDetail = string.Empty;
            IsFaulted = false;
        }

        public ExecuteMultipleHelperResponse(IEnumerable<OrganizationResponse> orgResponses, List<OrganizationRequest> orgRequests)
        {
            this.Requests = new List<OrganizationRequest>();
            this.Responses = new List<OrganizationResponse>();
            this.Requests.AddRange(orgRequests.Where((a) => (a as ExecuteMultipleRequest) == null));
            this.Responses.AddRange(orgResponses.Where((a) => (a as ExecuteMultipleResponse) == null));

            if (orgResponses == null) return;
            ConcurrentBag<ExecuteMultipleHelperResponse> responses = new ConcurrentBag<ExecuteMultipleHelperResponse>();
            for (int index = 0; index < orgResponses.Count(); index++)
            {
                OrganizationResponse orgResponse = orgResponses.ElementAt(index);

                //}
                //Parallel.ForEach(orgResponses, (orgResponse, loopState, index) =>
                //{
                StringBuilder log = new StringBuilder();
                StringBuilder friendly = new StringBuilder();


                ExecuteMultipleResponse emResponse = orgResponse as ExecuteMultipleResponse;
                if (emResponse != null)
                {
                    int position = Convert.ToInt32(index);
                    ExecuteMultipleRequest emRequest = (ExecuteMultipleRequest)orgRequests[position];
                    IEnumerable<OrganizationResponse> _orgresponses = emResponse.Responses.Select((r) => r.Response);
                    if (_orgresponses.Count() > 0) this.Responses.AddRange(_orgresponses);
                    if (emRequest.Requests != null && emRequest.Requests.Count > 0)
                    {
                        IEnumerable<OrganizationRequest> _orgrequests = emResponse.Responses.Select((r) => emRequest.Requests[r.RequestIndex]);
                        if (_orgrequests.Count() > 0) this.Requests.AddRange(_orgrequests);
                    }

                    if (emResponse.IsFaulted)
                    {


                        ProcessFault(position, emResponse, log, friendly, emRequest.Requests);
                        responses.Add(new ExecuteMultipleHelperResponse()
                        {
                            IsFaulted = true,
                            LogDetail = log.ToString(),
                            FriendlyDetail = friendly.ToString(),
                            Request = Request,
                            Response = emResponse
                        });
                    }
                }
            }
            //);

            this.IsFaulted = responses.Any(o => o.IsFaulted);
            this.ChildResponses = responses.ToList();

            CombineChildResponses();
        }

        public void CombineChildResponses()
        {
            CombineChildResponses(this);
        }

        public static void CombineChildResponses(ExecuteMultipleHelperResponse response)
        {
            int faultedChildren = 0;
            foreach (ExecuteMultipleHelperResponse child in response.ChildResponses)
            {
                // roll up requests and responses
                if (child.Requests != null && child.Requests.Count > 0)
                {
                    response.Requests.AddRange(child.Requests);
                }
                if (child.Responses != null && child.Responses.Count > 0)
                {
                    response.Responses.AddRange(child.Responses);
                }

                response.LogDetail += child.LogDetail + "\r\n\r\n";
                if (child.IsFaulted)
                {
                    faultedChildren++;
                    response.FriendlyDetail += child.FriendlyDetail + "\r\n";
                }
            }
            if (faultedChildren == 0) return;
            if (faultedChildren == 1)
            {
                ExecuteMultipleHelperResponse faultChild = response.ChildResponses.Where(a => a.IsFaulted).First();
                response.ErrorDetail = faultChild.ErrorDetail;
                response.Response = faultChild.Response;
                response.FriendlyDetail = faultChild.FriendlyDetail;
            }
            else if (faultedChildren > 1)
            {
                response.FriendlyDetail = "Multiple Errors Occurred:\r\n\r\n" + response.FriendlyDetail;
            }
        }

        public string ErrorDetail { get; set; }
        public string LogDetail { get; set; }
        public string FriendlyDetail { get; set; }
        public bool IsFaulted { get; set; }

        public List<OrganizationResponse> Responses { get; set; }
        public List<OrganizationRequest> Requests { get; set; }

        public ExecuteMultipleResponse Response { get; set; }
        public ExecuteMultipleRequest Request { get; set; }
        public List<ExecuteMultipleHelperResponse> ChildResponses { get; set; }

        public void Log(string format, params object[] args)
        {
            LogDetail += string.Format(format, args) + "\r\n";
        }

        public void Fault(string format, params object[] args)
        {
            IsFaulted = true;
            Log(format, args);
        }

        private static void ProcessFault(int position, ExecuteMultipleResponse emResponse, StringBuilder log, StringBuilder friendlyMsg, OrganizationRequestCollection requests)
        {
            StringBuilder faultlog = new StringBuilder();
            position++;
            ExecuteMultipleResponseItemCollection responses = emResponse.Responses;
            faultlog.AppendFormat("Execute Multiple {0}: FAULTED\r\n", position);

            int faults = 0;
            try
            {
                foreach (ExecuteMultipleResponseItem faultResponse in responses)
                {
                    OrganizationRequest request = requests[faultResponse.RequestIndex];
                    faults = ProcessFault(faults, faultResponse, faultlog, friendlyMsg, request);
                }

                if (faults > 0)
                {
                    faultlog.Insert(0, string.Format("Execute Multiple: {0} Faults\r\n\r\n", faults));

                    if (faults > 1)
                        friendlyMsg.Insert(0,
                            string.Format("There were {0} errors performing the operation.\r\n", faults));

                }
            }
            catch (Exception e)
            {
                faultlog.AppendFormat("Processing Error in ProcessFaults: {0}", e.Message);
                faultlog.AppendFormat("\r\n\r\nSTACK TRACE: \r\n{0}", e.StackTrace);
            }
            log.Append(faultlog);
        }

        private static int ProcessFault(int position, ExecuteMultipleResponseItem responseItem, StringBuilder log, StringBuilder friendlyMsg, OrganizationRequest request)
        {
            if (responseItem.Fault == null) return position; //no fault
            OrganizationServiceFault fault = responseItem.Fault;

            if (request == null)
            {
                log.AppendFormat("\r\nRequest {0} is null.\r\n", responseItem.RequestIndex);
                log.AppendFormat("Fault Error Details: {0}\r\n", fault.ErrorDetails);
                log.AppendFormat("Fault Error Message: {0}\\r\n", fault.Message);
                return position + 1;
            }

            return ProcessFault(position, fault, log, friendlyMsg, request);
        }

        private static int ProcessFault(int position, OrganizationServiceFault orgFault, StringBuilder log, StringBuilder friendlyMsg, OrganizationRequest request)
        {

            string requestName = request.RequestName.ToUpperInvariant();
            if (requestName.Equals("ASSOCIATE"))
                if (orgFault.Message.Contains("duplicate key")) return 0;

            position++;

            // Separate Faults
            if (log.Length > 0)
                log.Append("\r\n\r\n\r\n");

            log.AppendFormat("ERROR #{0}:  ", position);

            friendlyMsg.AppendFormat("{0}. There was an error performing {1} {2}: {3}\r\n", position,
                request.RequestName.FirstLetterIsVowel() ? "an" : "a", requestName.ToLowerInvariant(),
                orgFault.Message);

            string pad = string.Empty;
            do
            {
                log.AppendFormat("{3}Code: {0}\r\n{3}Message: {1}\r\n{3}Trace:{2}",
                    orgFault.ErrorCode, orgFault.Message, orgFault.TraceText, pad);
                orgFault = orgFault.InnerFault;
                if (orgFault != null)
                {
                    log.AppendFormat("\r\n{0}Inner Fault:\r\n", pad);
                    pad += "  ";
                }
            } while (orgFault != null);

            log.Append("\r\n\r\n");

            switch (requestName)
            {
                case "ASSOCIATE":
                    AssociateRequest assocRequest = (AssociateRequest)request;
                    try
                    {
                        log.AppendFormat(
                            "Associate: Using Relationship [{0}] to append to {1}{2}{3} ({4})",
                            assocRequest.Relationship,
                            assocRequest.Target.LogicalName,
                            !string.IsNullOrEmpty(assocRequest.Target.Name) ? " " : string.Empty,
                            assocRequest.Target.Name,
                            assocRequest.Target.Id);
                        log.AppendFormat(":\r\n{0}",
                            string.Join("\r\n", assocRequest.RelatedEntities.Select(r =>
                                string.Format("{0}{1}{2}{3} [{4}]", r.Name,
                                    !string.IsNullOrEmpty(r.Name) ? " (" : string.Empty,
                                    r.Id, !string.IsNullOrEmpty(r.Name) ? ")" : string.Empty, r.LogicalName)))
                        );
                    }
                    catch
                    {
                        // Ignore erros creating error log entry
                    }
                    break;

                case "DISASSOCIATE":
                    DisassociateRequest dissocRequest = (DisassociateRequest)request;
                    try
                    {
                        log.AppendFormat(
                            "Disassociate: Using Relationship [{0}] to remove association to {1}{2}{3} ({4})",
                            dissocRequest.Relationship,
                            dissocRequest.Target.LogicalName,
                            !string.IsNullOrEmpty(dissocRequest.Target.Name) ? " " : string.Empty,
                            dissocRequest.Target.Name,
                            dissocRequest.Target.Id);
                        log.AppendFormat("\r\n{0}",
                            string.Join("\r\n", dissocRequest.RelatedEntities.Select(r =>
                                string.Format("{0}{1}{2}{3} [{4}]", r.Name,
                                    !string.IsNullOrEmpty(r.Name) ? " (" : string.Empty,
                                    r.Id, !string.IsNullOrEmpty(r.Name) ? ")" : string.Empty, r.LogicalName)))
                        );
                    }
                    catch
                    {
                        // Ignore erros creating error log entry
                    }
                    break;
                default:
                    if (request.Parameters.ContainsKey("Target"))
                    {
                        object oTarget = request.Parameters["Target"];
                        if (oTarget is Entity)
                        {
                            Entity eTarget = (Entity)oTarget;
                            log.Append(DumpEntityToString(request.RequestName, eTarget));
                        }
                        else if (oTarget is EntityReference)
                        {
                            EntityReference erTarget = (EntityReference)oTarget;
                            log.AppendFormat("{0}: {1} ({2})", request.RequestName,
                                erTarget.LogicalName +
                                (string.IsNullOrEmpty(erTarget.Name) ? string.Empty : ":" + erTarget.Name),
                                erTarget.Id);
                        }
                    }

                    break;
            }
            return position;
        }

        /// <summary>
        ///     DumpEntityToString: This dumps an entity to a string, which can then be logged.
        /// </summary>
        /// <param name="name">The name or type of the entity</param>
        /// <param name="entity">The entity to dump</param>
        /// <returns></returns>
        private static string DumpEntityToString(string name, Entity entity)
        {
            try
            {
                return entity.DumpToString(name);
            }
            catch (Exception processException)
            {
                return "Unable to dump entity to string:   " + processException.Message;
            }
        }
    }
}