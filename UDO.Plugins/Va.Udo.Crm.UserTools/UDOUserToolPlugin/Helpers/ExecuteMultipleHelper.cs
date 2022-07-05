using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSRequestHelpers
{
    public static class ExecuteMultipleHelper
    {
        // This is the Max Batch Size (All Batches will be shrunk that are bigger than this)
        private static int MaxBatchSize = 1000;

        /// <summary>
        /// ExecuteMultiple: Executes multiple OrganizationRequests allowing for higher throughput, 
        /// and bulk message passing to the CRM Service
        /// </summary>
        /// <param name="service">CRM OrganizationService</param>
        /// <param name="multipleRequest">ExecuteMultipleRequest</param>
        /// <param name="debug">Debug: Default is True</param>
        /// <param name="continueOnError">Continue On Error: Default is True</param>
        /// <returns>ExecuteMultipleHelperResponse</returns>
        public static ExecuteMultipleHelperResponse ExecuteMultiple(IOrganizationService service,
            ExecuteMultipleRequest multipleRequest, bool debug = true, bool continueOnError = true)
        {
            return ExecuteMultiple(service, multipleRequest.Requests, debug, continueOnError, null);
        }

        /// <summary>
        /// ExecuteMultiple: Executes multiple OrganizationRequests allowing for higher throughput, 
        /// and bulk message passing to the CRM Service
        /// </summary>
        /// <param name="service">CRM OrganizationService</param>
        /// <param name="requests">OrganizationRequestCollection</param>
        /// <param name="debug">Debug: Default is True</param>
        /// <param name="continueOnError">Continue On Error: Default is True</param>
        /// <returns>ExecuteMultipleHelperResponse</returns>
        public static ExecuteMultipleHelperResponse ExecuteMultiple(IOrganizationService service,
            IEnumerable<OrganizationRequest> requests, bool debug = true, bool continueOnError = true)
        {
            return ExecuteMultiple(service, requests, debug, continueOnError, null);
        }

        /// <summary>
        /// ExecuteMultiple: Private (Internal) method that executes multiple OrganizationRequests allowing 
        /// for higher throughput, andd bulk message passing to the CRM Service.  This method passes the
        /// rootResponse, allowing for deep EM requests due to execution for larger sets.
        /// </summary>
        /// <param name="service">CRM OrganizationService</param>
        /// <param name="requests">OrganizationRequestCollection</param>
        /// <param name="debug">Debug: Default is True</param>
        /// <param name="continueOnError">Continue On Error: Default is True</param>
        /// <param name="rootResponse">A reference to the root response record</param>
        /// <returns>ExecuteMultipleHelperResponse</returns>
        private static ExecuteMultipleHelperResponse ExecuteMultiple(IOrganizationService service,
            IEnumerable<OrganizationRequest> requests, bool debug, bool continueOnError = true, ExecuteMultipleHelperResponse rootResponse = null)
        {
            // Filter out null requests
            var requestCollection = requests.Where(r => r != null);

            var response = new ExecuteMultipleHelperResponse
            {
                LogDetail = "Execute Multiple: START\r\n",
                ErrorDetail = string.Empty,
                FriendlyDetail = string.Empty,
                IsFaulted = false
            };

            // Set Root Response if there is not one.
            if (rootResponse == null)
            {
                response.ChildReponses = new List<ExecuteMultipleHelperResponse>();
                rootResponse = response;
            }

            if (debug) response.LogDetail += String.Format("Checking Batch Limit: {0}\r\n", requestCollection.Count());


            var executeCollection = new OrganizationRequestCollection();
            if (requestCollection.Count() > MaxBatchSize)
            {
                SplitRequests(service, requestCollection, debug, continueOnError, rootResponse, response);
            }
            else
            {
                executeCollection.AddRange(requestCollection);
                DoExecuteMultiple(service, executeCollection, debug, continueOnError, response);
            }

            CombineChildResults(rootResponse, response);

            return response;
        }

        /// <summary>
        /// CombineChildResults: combines the child results with the parent result
        /// </summary>
        /// <param name="rootResponse">rootResponse</param>
        /// <param name="response">thisResponse</param>
        private static void CombineChildResults(ExecuteMultipleHelperResponse rootResponse, ExecuteMultipleHelperResponse response)
        {
            if (response != rootResponse || response.ChildReponses == null) return;

            var faultedChildren = 0;
            foreach (var child in rootResponse.ChildReponses)
            {
                response.LogDetail += child.LogDetail + "\r\n\r\n";
                if (child.IsFaulted) faultedChildren++;
            }
            if (faultedChildren == 0) return;
            if (faultedChildren == 1)
            {
                var faultChild = rootResponse.ChildReponses.Where(a => a.IsFaulted).First();
                response.ErrorDetail = faultChild.ErrorDetail;
                response.Response = faultChild.Response;
                response.FriendlyDetail = faultChild.FriendlyDetail;
                return;
            }
            else if (faultedChildren > 1)
            {
                response.FriendlyDetail = "Multiple Errors Occurred:\r\n\r\n";
                rootResponse.ChildReponses.Where(a => a.IsFaulted)
                    .ToList().ForEach(a => response.FriendlyDetail += a.FriendlyDetail + "\r\n");
            }
        }

        /// <summary>
        /// DoExecuteMultiple: Executes the EM Request
        /// </summary>
        /// <param name="service">OrganizationService</param>
        /// <param name="requests">OrganizationRequestCollection: Must be the collection</param>
        /// <param name="debug">Debug</param>
        /// <param name="continueOnError">Whether or not to continue on error</param>
        /// <param name="response">The initialized response that information will be appended to</param>
        private static void DoExecuteMultiple(IOrganizationService service, OrganizationRequestCollection requests, bool debug, bool continueOnError, ExecuteMultipleHelperResponse response)
        {
            if (debug) response.LogDetail += "Execute Multiple: Sending Execute...\r\n";
            response.Request = new ExecuteMultipleRequest()
            {
                Requests = requests,
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = continueOnError,
                    ReturnResponses = false // only get error responses
                }
            };
            response.Response = (ExecuteMultipleResponse)service.Execute(response.Request);

            // Build Initial Response
            if (debug) response.LogDetail += "Execute Multiple: EM Complete, building result\r\n";

            if (response.Response.IsFaulted || response.Response.Responses.Count > 0)
            {
                ProcessFaults(requests, debug, response);
            }

            if (debug) response.LogDetail += "Execute Multiple: END\r\n";
        }

        /// <summary>
        /// SplitRequests: Takes a large request and splits it into multiple smaller requests.
        /// </summary>
        /// <param name="service">OrganzationService</param>
        /// <param name="requests">An enurable collection/list of OrganizationRequest</param>
        /// <param name="debug"></param>
        /// <param name="continueOnError"></param>
        /// <param name="rootResponse"></param>
        /// <param name="response"></param>
        private static void SplitRequests(IOrganizationService service, IEnumerable<OrganizationRequest> requests, bool debug, bool continueOnError, ExecuteMultipleHelperResponse rootResponse, ExecuteMultipleHelperResponse response)
        {
            var pages = Convert.ToInt32(Math.Ceiling(((double)requests.Count()) / MaxBatchSize));
            if (debug) response.LogDetail += String.Format("Executing with {0} smaller batches...\r\n", pages);

            for (int page = 0; page < pages; page++)
            {
                if (debug) response.LogDetail += String.Format("Executing Batch {0}:\r\n", page);
                var smallBatchResponse = ExecuteMultiple(service,
                    requests.Skip((page + 1) * MaxBatchSize).Take(MaxBatchSize),
                    debug, continueOnError, rootResponse);
                if (smallBatchResponse.IsFaulted)
                {
                    rootResponse.IsFaulted = true;
                    rootResponse.ChildReponses.Add(smallBatchResponse);
                }
            }
        }

        /// <summary>
        /// ProcessFaults:  Handle the errors from any ExecuteMultipleRequests and convert the result into
        /// a friendly response as well as a detailed response for the LogDetail and ErrorDetail.
        /// </summary>
        /// <param name="requests"></param>
        /// <param name="debug"></param>
        /// <param name="response"></param>
        private static void ProcessFaults(OrganizationRequestCollection requests, bool debug, ExecuteMultipleHelperResponse response)
        {
            var responses = response.Response.Responses;
            if (debug) response.LogDetail += "Execute Multiple: FAULTED\r\n";
            response.IsFaulted = true;
            if (debug)
            {
                StringBuilder faultMsg = new StringBuilder();
                StringBuilder friendlyMsg = new StringBuilder();
                int totalFaults = 0;

                foreach (var faultResponse in responses)
                {
                    if (faultResponse.Fault == null) continue; //no fault

                    totalFaults++;

                    // Separate Faults
                    if (faultMsg.Length > 0)
                    {
                        faultMsg.Append("\r\n\r\n\r\n");
                    }

                    faultMsg.AppendFormat("ERROR #{0}:  ", totalFaults);

                    var request = requests[faultResponse.RequestIndex];
                    var fault = faultResponse.Fault;


                    friendlyMsg.AppendFormat("{0}. There was an error performing {1} {2}: {3}", totalFaults,
                        IsFirstLetterVowel(request.RequestName) ? "an" : "a", request.RequestName.ToLowerInvariant(),
                        fault.Message);

                    var pad = string.Empty;
                    do
                    {
                        faultMsg.AppendFormat("{3}Code: {0}\r\n{3}Message: {1}\r\n{3}Trace:{2}",
                        fault.ErrorCode, fault.Message, fault.TraceText, pad);
                        fault = fault.InnerFault;
                        if (fault != null)
                        {
                            faultMsg.AppendFormat("\r\n{0}Inner Fault:\r\n", pad);
                            pad += "  ";
                        }
                    } while (fault != null);

                    faultMsg.Append("\r\n\r\n");

                    switch (request.RequestName.ToUpperInvariant())
                    {
                        case "ASSOCIATE":
                            var assocRequest = (AssociateRequest)request;
                            try
                            {
                                faultMsg.AppendFormat("Associate: Using Relationship [{0}] to append to {1}{2}{3} ({4})",
                                                        assocRequest.Relationship,
                                                        assocRequest.Target.LogicalName,
                                                        !String.IsNullOrEmpty(assocRequest.Target.Name) ? " " : string.Empty,
                                                        assocRequest.Target.Name,
                                                        assocRequest.Target.Id);
                                faultMsg.AppendFormat(":\r\n{0}", String.Join("\r\n", assocRequest.RelatedEntities.Select(r =>
                                                            String.Format("{0}{1}{2}{3} [{4}]", r.Name, !String.IsNullOrEmpty(r.Name) ? " (" : string.Empty,
                                                            r.Id, !String.IsNullOrEmpty(r.Name) ? ")" : string.Empty, r.LogicalName)))
                                                    );
                            }
                            catch { }
                            break;

                        case "DISASSOCIATE":
                            var dissocRequest = (DisassociateRequest)request;
                            try
                            {
                                faultMsg.AppendFormat("Associate: Using Relationship [{0}] to append to {1}{2}{3} ({4})",
                                                        dissocRequest.Relationship,
                                                        dissocRequest.Target.LogicalName,
                                                        !String.IsNullOrEmpty(dissocRequest.Target.Name) ? " " : string.Empty,
                                                        dissocRequest.Target.Name,
                                                        dissocRequest.Target.Id);
                                faultMsg.AppendFormat("\r\n{0}", String.Join("\r\n", dissocRequest.RelatedEntities.Select(r =>
                                                            String.Format("{0}{1}{2}{3} [{4}]", r.Name, !String.IsNullOrEmpty(r.Name) ? " (" : string.Empty,
                                                            r.Id, !String.IsNullOrEmpty(r.Name) ? ")" : string.Empty, r.LogicalName)))
                                                    );
                            }
                            catch { }
                            break;
                        default:
                            if (request.Parameters.ContainsKey("Target"))
                            {
                                object oTarget = request.Parameters["Target"];
                                if (oTarget is Entity)
                                {
                                    var eTarget = (Entity)oTarget;
                                    faultMsg.Append(DumpEntityToString(request.RequestName, eTarget));

                                }
                                else if (oTarget is EntityReference)
                                {
                                    var erTarget = (EntityReference)oTarget;
                                    faultMsg.AppendFormat("{0}: {1} ({2})", request.RequestName,
                                        erTarget.LogicalName + (String.IsNullOrEmpty(erTarget.Name) ? string.Empty : ":" + erTarget.Name),
                                        erTarget.Id);
                                }
                            }

                            break;
                    }

                }

                if (totalFaults > 0)
                {
                    faultMsg.Insert(0, String.Format("Execute Multiple: {0} Faults\r\n\r\n", totalFaults));


                    if (totalFaults > 1)
                    {
                        friendlyMsg.Insert(0, String.Format("There were {0} errors performing the operation.\r\n", totalFaults));
                    }

                    if (!String.IsNullOrEmpty(response.FriendlyDetail)) response.FriendlyDetail += "\r\n\r\n\r\n";
                    response.FriendlyDetail += friendlyMsg.ToString();

                    response.IsFaulted = true;
                    if (String.IsNullOrEmpty(response.ErrorDetail)) response.ErrorDetail = response.FriendlyDetail;

                    response.LogDetail += faultMsg.ToString();
                }
            }
            else
            {
                StringBuilder friendlyMsg = new StringBuilder();
                int totalFaults = 0;

                foreach (var faultResponse in responses)
                {
                    if (faultResponse.Fault == null) continue; //no fault

                    totalFaults++;

                    var request = requests[faultResponse.RequestIndex];
                    var fault = faultResponse.Fault;


                    friendlyMsg.AppendFormat("{0}. There was an error performing {1} {2}: {3}\r\n", totalFaults,
                        IsFirstLetterVowel(request.RequestName) ? "an" : "a", request.RequestName.ToLowerInvariant(),
                        fault.Message);
                }
                if (totalFaults > 0)
                {
                    if (totalFaults > 1)
                    {
                        friendlyMsg.Insert(0, String.Format("There were {0} errors performing the operation.\r\n", totalFaults));
                    }

                    if (!String.IsNullOrEmpty(response.ErrorDetail)) response.ErrorDetail += "\r\n\r\n\r\n";
                    response.ErrorDetail += "Debug is not enabled.  To get detailed errors for ExecuteMultiple, enable debug.\r\n";

                    if (!String.IsNullOrEmpty(response.FriendlyDetail)) response.FriendlyDetail += "\r\n\r\n\r\n";
                    response.FriendlyDetail += friendlyMsg.ToString();

                    response.IsFaulted = true;
                }
            }
        }


        /// <summary>
        /// DumpEntityToString: This dumps an entity to a string, which can then be logged.
        /// </summary>
        /// <param name="name">The name or type of the entity</param>
        /// <param name="entity">The entity to dump</param>
        /// <returns></returns>
        public static string DumpEntityToString(string name, Entity entity)
        {
            string nulltext = "<null>";

            StringBuilder entityDump = new StringBuilder();
            entityDump.AppendFormat("{0} [entity:{1} id:{2}]",
                name, entity.LogicalName, entity.Id);

            foreach (var attributeName in entity.Attributes.Keys)
            {
                //Append Name
                entityDump.AppendFormat("{0}: ", attributeName);

                var attributeObj = entity[attributeName];
                var attributeValue = attributeObj.ToString();
                if (attributeObj == null) attributeValue = nulltext;

                if (attributeObj is AliasedValue)
                {
                    if (((AliasedValue)attributeObj).Value == null)
                    {
                        attributeValue = nulltext;
                    }
                    else
                    {
                        attributeObj = ((AliasedValue)attributeObj).Value;
                    }
                }

                if (attributeObj is OptionSetValue)
                {
                    var attrOptionSet = (OptionSetValue)attributeObj;
                    attributeValue = attrOptionSet.Value.ToString();
                }
                else if (attributeObj is EntityReference)
                {
                    attributeValue = string.Empty;
                    var attrLookup = (EntityReference)attributeObj;
                    attributeValue = string.Format("{0}{1}[entity:{2} id:{3}]",
                        attrLookup.Name, String.IsNullOrEmpty(attrLookup.Name) ? "" : " ",
                        attrLookup.LogicalName, attrLookup.Id);
                }

                if (attributeValue.Length > 200 && !(attributeObj is EntityReference))
                {
                    attributeValue = "\r\n" + attributeValue;
                }

                if (entity.FormattedValues.ContainsKey(attributeName))
                {
                    attributeValue += String.Format(" ({0})", entity.FormattedValues[attributeName]);
                }

                // Append Value
                entityDump.AppendFormat("{0}\r\n", attributeValue);
            }

            return entityDump.ToString();
        }

        private static bool IsFirstLetterVowel(string text)
        {
            var letter = text[0].ToString().ToLowerInvariant()[0];
            return (letter == 'a' || letter == 'e' || letter == 'i' || letter == 'o' || letter == 'u');
        }
    }
    public class ExecuteMultipleHelperResponse
    {
        public string ErrorDetail { get; set; }
        public string LogDetail { get; set; }
        public string FriendlyDetail { get; set; }
        public bool IsFaulted { get; set; }
        public ExecuteMultipleResponse Response { get; set; }
        public ExecuteMultipleRequest Request { get; set; }
        public List<ExecuteMultipleHelperResponse> ChildReponses { get; set; }
    }
}
