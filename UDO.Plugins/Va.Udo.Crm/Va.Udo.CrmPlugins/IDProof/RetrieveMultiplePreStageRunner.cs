using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using System.Diagnostics;
using System.Threading;


namespace Va.Udo.Crm.Plugins.RetrieveMultiple
{
    internal class RetrieveMultiplePreStageRunner : PluginRunner
    {
        bool _logSoap = false;
        bool _logTimer = false;
        string _fileNumber = "";
        string _uri = "";
        const string Query = "Query";
        int sleepCount = 0;
        Guid _idproofid = new Guid();
        public RetrieveMultiplePreStageRunner(IServiceProvider serviceProvider)
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
            get { return "udo_idproof"; }
        }

        internal void Execute(string completeFlag)
        {

            Stopwatch txnTimer = Stopwatch.StartNew();

                if (!PluginExecutionContext.InputParameters.Contains(Query) ||
                     !(PluginExecutionContext.InputParameters[Query] is QueryExpression)) return;

            try
            {
               
                var qe = (QueryExpression)PluginExecutionContext.InputParameters[Query];
                if (findParentId(qe))
                {
                    isitdone(completeFlag);
                }
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
        internal bool isitdone(string completeFlag)
        {
            try
            {
                Logger.setMethod = "isitdone";
                if (sleepCount > 8)
                {
                    //Logger.WriteDebugMessage("Giving up on " + completeFlag + " creation");
                    TracingService.Trace("Giving up on " + completeFlag + " creation");
                    return false;
                }

                var idproof = OrganizationService.Retrieve("udo_idproof", _idproofid,
                    new ColumnSet("udo_claimintegration", "udo_legacypaymentintegration",
                        "udo_appealintegration", "udo_awardintegration"));

                switch (completeFlag)
                {
                    case "award":
                        var udo_awardIntegration = idproof.GetAttributeValue<OptionSetValue>("udo_awardintegration");
                        if (udo_awardIntegration != null)
                        {
                            var awardInt = udo_awardIntegration.Value;
                            if (awardInt == 752280002)
                            {
                                //complete, we're done
                                //Logger.WriteDebugMessage("Award Complete");
                                TracingService.Trace("Award Complete");
                                return true;
                            }
                            if (awardInt == 752280003)
                            {
                                //error - deal with it
                                //Logger.WriteDebugMessage("Award Error");
                                TracingService.Trace("Award Error");
                                return true;
                            }
                            Thread.Sleep(500);
                            sleepCount += 1;
                            //Logger.WriteDebugMessage("trying again after sleep");
                            TracingService.Trace("trying again after sleep");
                            if (isitdone(completeFlag))
                            {
                                return true;
                            }
                        }

                        break;
                    case "appeal":
                        var udo_appealIntegration = idproof.GetAttributeValue<OptionSetValue>("udo_appealintegration");
                        if (udo_appealIntegration != null)
                        {
                            var appealInt = udo_appealIntegration.Value;
                            if (appealInt == 752280002)
                            {
                                //complete, we're done
                                //Logger.WriteDebugMessage("Appeal Complete");
                                TracingService.Trace("Appeal Complete");
                                return true;
                            }
                            if (appealInt == 752280003)
                            {
                                //error - deal with it
                                //Logger.WriteDebugMessage("Appeal Error");
                                TracingService.Trace("Appeal Error");
                                return true;
                            }
                            Thread.Sleep(500);
                            sleepCount += 1;
                            //Logger.WriteDebugMessage("trying again after sleep");
                            TracingService.Trace("trying again after sleep");
                            if (isitdone(completeFlag))
                            {
                                return true;
                            }
                        }
                        break;
                    case "claim":
                        var udo_claimIntegration = idproof.GetAttributeValue<OptionSetValue>("udo_claimintegration");
                        if (udo_claimIntegration != null)
                        {
                            var claimInt = udo_claimIntegration.Value;
                            if (claimInt == 752280002)
                            {
                                //complete, we're done
                                //Logger.WriteDebugMessage("Claims Complete");
                                TracingService.Trace("Claims Complete");
                                return true;
                            }
                            if (claimInt == 752280003)
                            {
                                //error - deal with it
                                //Logger.WriteDebugMessage("Claims Error");
                                TracingService.Trace("Claims Error");
                                return true;
                            }
                            Thread.Sleep(500);
                            sleepCount += 1;
                            //Logger.WriteDebugMessage("trying again after sleep");
                            TracingService.Trace("trying again after sleep");
                            if (isitdone(completeFlag))
                            {
                                return true;
                            }
                        }

                        break;
                    case "legacypayment":
                        var udo_legacyPaymentIntegration = idproof.GetAttributeValue<OptionSetValue>("udo_legacypaymentintegration");
                        if (udo_legacyPaymentIntegration != null)
                        {
                            var legacyInt = udo_legacyPaymentIntegration.Value;
                            if (legacyInt == 752280002)
                            {
                                //complete, we're done
                                //Logger.WriteDebugMessage("legacy Complete");
                                TracingService.Trace("legacy Complete");
                                return true;
                            }
                            if (legacyInt == 752280003)
                            {
                                //error - deal with it
                                //Logger.WriteDebugMessage("legacy Error");
                                TracingService.Trace("legacy Error");
                                return true;
                            }
                            Thread.Sleep(500);
                            sleepCount += 1;
                            //Logger.WriteDebugMessage("trying again after sleep");
                            TracingService.Trace("trying again after sleep");
                            if (isitdone(completeFlag))
                            {
                                return true;
                            }
                        }
                        break;
                    default:
                        break;
                }
                
                Logger.setMethod = "Execute";
                return true;
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
                        if (filtCond.AttributeName.ToLower().Equals("udo_idproofid"))
                        {
                            _idproofid = Guid.Parse(filtCond.Values[0].ToString());
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