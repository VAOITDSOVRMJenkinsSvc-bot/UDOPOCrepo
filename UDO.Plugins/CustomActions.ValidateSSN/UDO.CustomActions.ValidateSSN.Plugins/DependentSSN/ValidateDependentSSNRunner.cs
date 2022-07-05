using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.CustomActions.ValidateSSN.Plugins.DependentSSN
{
    public class ValidateDependentSSNRunner : UDOActionRunner
    {
        public ValidateDependentSSNRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _logTimerField = "udo_contactlogtimer";
            _logSoapField = "udo_contactlogsoap";
            _debugField = "crme_dependentmaintenance";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_contacttimeout";
            _validEntities = new string[] { "crme_dependentmaintenance" };
        }

        public override void DoAction()
        {
            _method = "DoAction";
            GetSettingValues();
            EntityReference entity = (EntityReference)PluginExecutionContext.InputParameters["ParentEntityReference"];
            TracingService.Trace("Dep Maintenance ID is: " + entity.Id);
            var validateResponse = false;
            TracingService.Trace("Fetching Dependent Records Detail");
            var dependentDetails = @"<fetch>
                                      <entity name='crme_dependent' >
                                        <attribute name='crme_ssn' />
                                        <attribute name='crme_dependentrelationship' />
                                        <attribute name='udo_marriageenddate' />
                                        <filter type='and' >
                                          <condition attribute='crme_dependentmaintenance' operator='eq' value = '" + entity.Id + @"' />
                                          <filter type='or' >
                                            <condition attribute='crme_maintenancetype' operator='eq' value='935950000' />
                                            <condition attribute='crme_maintenancetype' operator='eq' value='752280000' />
                                          </filter>
                                        </filter>
                                      </entity>
                                    </fetch>";
            EntityCollection dependents = OrganizationService.RetrieveMultiple(new FetchExpression(dependentDetails));
            TracingService.Trace("Finished Fetching Dependent Records Detail");
            TracingService.Trace("Dependent Result Count - " + dependents.Entities.Count);

            if(dependents.Entities.Count == 0)//this means ONLY remove or nothing
            {
                validateResponse = true;
            }


            if (dependents.Entities.Count >= 1)
            {
                foreach(var dep in dependents.Entities)
                {
                    OptionSetValue depRelationship = new OptionSetValue();
                    if(dep.Attributes.Contains("crme_dependentrelationship"))
                    {
                        depRelationship = dep.GetAttributeValue<OptionSetValue>("crme_dependentrelationship");
                    }

                    TracingService.Trace("Value of Dep Relationship is:  " + depRelationship.Value);

                    if (depRelationship.Value == 935950000)//child
                    {
                        var ssn = dep.Attributes["crme_ssn"].ToString();
                        TracingService.Trace("Value of Child SSN is:  " + ssn);
                        if (ssn == null)
                        {
                            validateResponse = false;
                            break;
                        }
                        else
                        {
                            validateResponse = true;
                        }
                    }
                    else//spouse
                    {
                        if (dep.Attributes.Contains("udo_marriageenddate"))
                        {
                            var marEndDate = dep.GetAttributeValue<DateTime>("udo_marriageenddate");
                            TracingService.Trace("Marriage end date is:  " + marEndDate);
                            validateResponse = true;
                            //this means they have an end date and so they are past spouse and we dont have to do SSN validation

                        }
                        else
                        {
                            var ssn = dep.Attributes["crme_ssn"].ToString();
                            TracingService.Trace("Spouse SSN is:  " + ssn);
                            if (ssn == null)
                            {
                                validateResponse = false;
                                break;
                            }
                            else
                            {
                                validateResponse = true;
                            }
                        }

                    }
                }
            }

            TracingService.Trace("Dependent Record Count is: " + dependents.Entities.Count);
            TracingService.Trace("Validation Response is: " + validateResponse);
            PluginExecutionContext.OutputParameters["validationResponse"] = validateResponse;


        }
    }
}
