using MCSPlugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using Xrm;

namespace Va.Udo.Crm.VetHistory.Plugins
{
    internal class FNODRetrieveMultiplePreRunner : PluginRunner
    {
        Guid _veteranId = new Guid();
        string _ssn = "";
        const string Query = "Query";
        public FNODRetrieveMultiplePreRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }
        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }
        public override Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }
        public override string McsSettingsDebugField
        {
            get { return "udo_vethistory"; }
        }
        internal void Execute()
        {
            Stopwatch txnTimer = Stopwatch.StartNew();

            try
            {
                var query = new QueryExpression();
                if (PluginExecutionContext.InputParameters["Query"] is QueryExpression)
                {
                    var qe = (QueryExpression)PluginExecutionContext.InputParameters[Query];

                    if (qe != null && qe.PageInfo != null && qe.PageInfo.PageNumber > 1)
                    {
                        //Logger.WriteDebugMessage("Captured the next page request");
                        //return;
                    }
                    query = PluginExecutionContext.InputParameters["Query"] as QueryExpression;
                    //Logger.WriteDebugMessage("query was a queryexpression");

                    if (McsSettings.getDebug)
                    {
                        var conversionRequest = new QueryExpressionToFetchXmlRequest()
                        {
                            Query = query
                        };
                        var qResponse = (QueryExpressionToFetchXmlResponse)OrganizationService.Execute(conversionRequest);
                        Logger.WriteDebugMessage("QueryExpression before I mess with it - fetchxml = " + qResponse.FetchXml);
                    }
                }
                if (PluginExecutionContext.InputParameters["Query"] is FetchExpression)
                {
                    var conversionRequest = new FetchXmlToQueryExpressionRequest()
                    {
                        FetchXml = ((FetchExpression)PluginExecutionContext.InputParameters["Query"]).Query
                    };
                    Logger.WriteDebugMessage("query was a fetch");
                    var qResponse = (FetchXmlToQueryExpressionResponse)OrganizationService.Execute(conversionRequest);
                    query = qResponse.Query;
                }

                var criteria = query.Criteria.Conditions;
                var blnContinue = false;
                var blnBefore = false;

                if (PluginExecutionContext.InputParameters["Query"] is QueryExpression)
                {
                    foreach (var item in criteria)
                    {
                        Logger.WriteDebugMessage(" item.AttributeName = " + item.AttributeName);
                        if (item.AttributeName.Equals("va_name"))
                        {
                            var condValues = item.Values;
                            foreach (var item2 in condValues)
                            {
                                if (item2.ToString() == "N/ABEFORE")
                                {
                                    blnContinue = true;
                                    blnBefore = true;
                                    Logger.WriteDebugMessage("Found N/A Before");
                                }
                                if (item2.ToString() == "N/AAFTER")
                                {
                                    blnContinue = true;
                                    Logger.WriteDebugMessage("Found N/A After");
                                }
                            }
                        }
                        if (item.AttributeName.Equals("va_veterancontactid"))
                        {
                            var condValues = item.Values;
                            foreach (var item2 in condValues)
                            {
                                _veteranId = Guid.Parse(item2.ToString());
                                // Logger.WriteDebugMessage("_veteranId:" + _veteranId.ToString());
                            }
                        }
                    }
                }

                if (PluginExecutionContext.InputParameters["Query"] is FetchExpression)
                {
                    foreach (var item in criteria)
                    {
                        Logger.WriteDebugMessage(" item.AttributeName = " + item.AttributeName);
                        if (item.AttributeName.Equals("va_name"))
                        {
                            var condValues = item.Values;
                            foreach (var item2 in condValues)
                            {
                                if (item2.ToString() == "N/ABEFORE")
                                {
                                    blnContinue = true;
                                    blnBefore = true;
                                    Logger.WriteDebugMessage("Found N/A Before");
                                }
                                if (item2.ToString() == "N/AAFTER")
                                {
                                    blnContinue = true;
                                    Logger.WriteDebugMessage("Found N/A After");
                                }
                            }
                        }
                    }

                    // Linked Entities
                    foreach (var linkedEntity in query.LinkEntities)
                    {
                        if (linkedEntity.LinkToEntityName == "contact")
                        {
                            foreach (var condition in linkedEntity.LinkCriteria.Conditions)
                            {
                                if (condition.AttributeName == "contactid")
                                {
                                    _veteranId = Guid.Parse(condition.Values[0].ToString());
                                }
                            }
                        }
                    }
                }

                if (blnContinue)
                {
                    if (didWeNeedData())
                    {
                        if (blnBefore)
                        {
                            #region for before 2/1/
                            query.Criteria.Conditions.Clear();
                            query.Criteria.Filters.Clear();
                            query.LinkEntities.Clear();

                            Logger.WriteDebugMessage("on to criteria");
                            ConditionExpression condition1 = new ConditionExpression
                            {
                                AttributeName = "createdon",
                                Operator = ConditionOperator.OnOrBefore,
                                Values = { "2016-02-01" }
                            };

                            FilterExpression filter1 = new FilterExpression();
                            filter1.Conditions.Add(condition1);

                            query.Criteria.AddFilter(filter1);


                            //Logger.WriteDebugMessage("on to linked entity");

                            query.LinkEntities.Add(new LinkEntity
                            {
                                JoinOperator = Microsoft.Xrm.Sdk.Query.JoinOperator.Inner,
                                LinkFromAttributeName = "va_phonecallid",
                                LinkFromEntityName = "va_fnod",
                                LinkToAttributeName = "activityid",
                                LinkToEntityName = "phonecall",
                                LinkCriteria = new FilterExpression
                                {

                                    Conditions = 
                                    { 
                                   
                                        new ConditionExpression
                                        {
                                            AttributeName = "va_ssn",
                                            Operator = ConditionOperator.Equal,
                                            Values = { _ssn }
                                        }
                                    }
                                }
                            });

                            var conversionRequest = new QueryExpressionToFetchXmlRequest()
                            {
                                Query = query
                            };
                            var qResponse = (QueryExpressionToFetchXmlResponse)OrganizationService.Execute(conversionRequest);
                            Logger.WriteDebugMessage("QueryExpression < 2/1 after I mess with it - fetchxml = " + qResponse.FetchXml);
                            #endregion
                        }
                        else
                        {
                            #region for after 2/1/
                            query.Criteria.Conditions.Clear();
                            query.Criteria.Filters.Clear();
                            query.LinkEntities.Clear();

                            Logger.WriteDebugMessage("on to criteria");
                            ConditionExpression condition1 = new ConditionExpression
                            {
                                AttributeName = "createdon",
                                Operator = ConditionOperator.OnOrAfter,
                                Values = { "2016-02-01" }
                            };

                            FilterExpression filter1 = new FilterExpression();
                            filter1.Conditions.Add(condition1);

                            query.Criteria.AddFilter(filter1);


                           // Logger.WriteDebugMessage("on to linked entity");

                            query.LinkEntities.Add(new LinkEntity
                            {
                                JoinOperator = Microsoft.Xrm.Sdk.Query.JoinOperator.Inner,
                                LinkFromAttributeName = "va_veterancontactid",
                                LinkFromEntityName = "va_fnod",
                                LinkToAttributeName = "contactid",
                                LinkToEntityName = "contact",
                                LinkCriteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.Or,
                                    Conditions = 
                                { 
                                    new ConditionExpression
                                    {
                                        AttributeName = "udo_ssn",
                                        Operator = ConditionOperator.Equal,
                                        Values = { _ssn }
                                    },
                                    new ConditionExpression
                                    {
                                        AttributeName = "va_ssn",
                                        Operator = ConditionOperator.Equal,
                                        Values = { _ssn }
                                    }
                                }
                                }
                            });

                            var conversionRequest = new QueryExpressionToFetchXmlRequest()
                            {
                                Query = query
                            };
                            var qResponse = (QueryExpressionToFetchXmlResponse)OrganizationService.Execute(conversionRequest);
                            Logger.WriteDebugMessage("QueryExpression > 2/1 after I mess with it - fetchxml = " + qResponse.FetchXml);
                            #endregion
                        }

                        PluginExecutionContext.InputParameters["Query"] = query;
                    }
                    else
                    {
                        Logger.WriteDebugMessage("Didn't update query");
                    }

                }

                txnTimer.Stop();
                Logger.setMethod = "requestRetreiveMultiple";
                Logger.WriteTxnTimingMessage("requestRetreiveMultiple", txnTimer.ElapsedMilliseconds);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Logger.WriteToFile(ex.Message);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
            catch (Exception ex)
            {
                Logger.WriteToFile(ex.Message);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
        }
        internal bool didWeNeedData()
        {
            try
            {
                Logger.setMethod = "didWeNeedData";
                var gotData = false;

                using (var xrm = new XrmServiceContext(OrganizationService))
                {
                    var getParent2 = from vet in xrm.ContactSet
                                     where vet.ContactId == _veteranId
                                     select new
                                     {
                                         vet.udo_SSN,
                                         vet.va_SSN
                                     };

                    foreach (var vet in getParent2)
                    {
                        gotData = true;

                        if (vet.udo_SSN != null)
                        {
                            _ssn = vet.udo_SSN;
                            Logger.WriteDebugMessage("udo_SSN:" + _ssn);
                            
                        }
                        else
                        {
                            _ssn = vet.va_SSN;
                            Logger.WriteDebugMessage("va_SSN:" + _ssn);
                        }

                    }
                }
               
                Logger.setMethod = "Execute";
                //Logger.WriteDebugMessage("did not get data the 2nd time");
                return gotData;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="qe"></param>
        internal bool findParentId(QueryExpression qe)
        {
            try
            {
                Logger.setMethod = "SetQueryString";
               

                if (qe.Criteria != null)
                {
                    if (qe.Criteria.Filters.Any())
                    {
                    }
                    var theseFilterExpression = qe.Criteria;

                    foreach (var filtCond in theseFilterExpression.Conditions)
                    {
                        if (filtCond.AttributeName.ToLower().Equals("udo_veteranid"))
                        {
                            _veteranId = Guid.Parse(filtCond.Values[0].ToString());
                            Logger.setMethod = "Execute";
                            return true;
                            
                        }
                    }
                }
               
                Logger.setMethod = "Execute";
                return false;

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Unable to set Query String due to: {0}".Replace("{0}", ex.Message));
            }
        }
    }
}