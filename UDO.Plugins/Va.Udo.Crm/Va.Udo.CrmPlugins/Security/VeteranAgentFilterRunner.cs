using MCSPlugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Va.Udo.Crm.Plugins.Security
{
    internal class VeteranAgentFilterRunner : PluginRunner
    {

        public VeteranAgentFilterRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        /// <summary>
        /// Retrieves the primary entity
        /// </summary>
        /// <returns>Primary entity</returns>
        public override Microsoft.Xrm.Sdk.Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        /// <summary>
        /// Retrieves the secondary entity
        /// </summary>
        /// <returns></returns>
        public override Microsoft.Xrm.Sdk.Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        /// <summary>
        /// Debug field
        /// </summary>
        public override string McsSettingsDebugField
        {
            get { return "udo_security"; }
        }

        public void Execute(string VeteranFieldName)
        {


            var context = this.PluginExecutionContext;



            if (context.Depth > 2) return;

            if (context.Mode == 0 && context.Stage == 20 && (context.MessageName == "RetrieveMultiple" || context.MessageName == "Retrieve"))
            {
                var user = OrganizationService.Retrieve("systemuser", context.InitiatingUserId, new ColumnSet(new[] { "va_filenumber", "domainname" }));

                //  Logger.WriteDebugMessage(string.Format("in VeteranAgentFilterRunner code, VeteranFieldName: {0}; entity Name: {1}", VeteranFieldName, context.PrimaryEntityName));
                if (!user.Attributes.Contains("domainname") || String.IsNullOrEmpty(user.GetAttributeValue<string>("domainname")))
                {
                    return;
                }

                if (user.Attributes["va_filenumber"] == null)
                {
                    //Log the issue.
                    //Logger.WriteDebugMessage("no user va_filenumber");
                    TracingService.Trace("no user va_filenumber");

                }

                var userSsn = user.GetAttributeValue<string>("va_filenumber");


                switch (context.MessageName)
                {

                    case "RetrieveMultiple":
                        // Logger.WriteDebugMessage("RetrieveMultiple");
                        var query = new QueryExpression();

                        if (context.InputParameters["Query"] is FetchExpression)
                        {
                            var conversionRequest = new FetchXmlToQueryExpressionRequest()
                                {
                                    FetchXml = ((FetchExpression)context.InputParameters["Query"]).Query
                                };

                            var qResponse = (FetchXmlToQueryExpressionResponse)OrganizationService.Execute(conversionRequest);
                            query = qResponse.Query;
                            // Logger.WriteDebugMessage("Converted Fetch to Query");
                        }
                        else
                        {
                            query = context.InputParameters["Query"] as QueryExpression;
                            //var conversionRequest = new QueryExpressionToFetchXmlRequest()
                            //{
                            //    Query = query
                            //};
                            //var qResponse = (QueryExpressionToFetchXmlResponse)OrganizationService.Execute(conversionRequest);
                            //Logger.WriteDebugMessage("QueryExpression - fetchxml = " + qResponse.FetchXml);
                        }
                        if (query != null)
                        {
                            if (!query.EntityName.Equals("contact", StringComparison.InvariantCultureIgnoreCase))
                            {
                                LinkEntity vetLink = new LinkEntity()
                                {
                                    LinkFromEntityName = query.EntityName,
                                    LinkFromAttributeName = VeteranFieldName,
                                    LinkToEntityName = "contact",
                                    LinkToAttributeName = "contactid",

                                    LinkCriteria = new FilterExpression()
                                    {
                                        FilterOperator = LogicalOperator.And,
                                        Conditions = {
                                                new ConditionExpression()
                                                    {
                                                        AttributeName = "udo_ssn",
                                                        Operator = ConditionOperator.NotEqual,
                                                        Values = {userSsn}
                                                    }
                                            }
                                    }

                                };

                                query.LinkEntities.Add(vetLink);
                            }
                            else
                            {
                                query.Criteria.AddCondition(new ConditionExpression()
                                            {
                                                AttributeName = "udo_ssn",
                                                Operator = ConditionOperator.NotEqual,
                                                Values = { userSsn }
                                            });

                            }
                        }
                        else
                        {
                            // Logger.WriteDebugMessage("Query was null");
                        }
                        break;

                    case "Retrieve":

                        //   Logger.WriteDebugMessage("Retrieve");   
                        var targetEntity = context.InputParameters["Target"] as EntityReference;
                        if (targetEntity == null)
                            throw new InvalidPluginExecutionException("Invalid Target Entity");


                        Guid vetID = targetEntity.Id;
                        if (targetEntity.LogicalName != "contact")
                        {
                            var relatedEntity = OrganizationService.Retrieve(targetEntity.LogicalName, targetEntity.Id, new ColumnSet(new[] { VeteranFieldName }));
                            vetID = relatedEntity.GetAttributeValue<EntityReference>(VeteranFieldName).Id;
                            // Logger.WriteDebugMessage("Got the ID");
                        }

                        var veteran = OrganizationService.Retrieve("contact", vetID, new ColumnSet(new[] { "udo_ssn" }));

                        if (veteran.Contains("udo_ssn"))
                        {
                            var vetSsn = veteran.GetAttributeValue<string>("udo_ssn");
                            if (vetSsn.Equals(userSsn))
                            {
                                throw new InvalidPluginExecutionException("Security does not allow you to view your own records.");
                            }
                            //   Logger.WriteDebugMessage("SSN is not the same as the user");

                        }
                        break;
                }
            }
        }
    }
}
