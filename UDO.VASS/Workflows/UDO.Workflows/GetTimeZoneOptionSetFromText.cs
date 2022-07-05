using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.Workflows
{
    public class GetTimeZoneOptionSetFromText : CodeActivity
    {
        [Input("TimeZone")]
        public InArgument<string> TimeZone { get; set; }

        [Output("TimeZoneOptionSet")]
        [AttributeTarget("udo_interaction", "udo_timezonesuggested")]
        public OutArgument<OptionSetValue> TimeZoneOptionSet { get; set; }
        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            string timeZone = TimeZone.Get(executionContext);
            try
            {
                switch (timeZone)
                {
                    case "CST":
                        TimeZoneOptionSet.Set(executionContext, new OptionSetValue((int)TimeZones.CST));
                        break;
                    case "CDT":
                        TimeZoneOptionSet.Set(executionContext, new OptionSetValue((int)TimeZones.CDT));
                        break;
                    case "MST":
                        TimeZoneOptionSet.Set(executionContext, new OptionSetValue((int)TimeZones.MST));
                        break;
                    case "MDT":
                        TimeZoneOptionSet.Set(executionContext, new OptionSetValue((int)TimeZones.MDT));
                        break;
                    case "EST":
                        TimeZoneOptionSet.Set(executionContext, new OptionSetValue((int)TimeZones.EST));
                        break;
                    case "EDT":
                        TimeZoneOptionSet.Set(executionContext, new OptionSetValue((int)TimeZones.EDT));
                        break;
                    case "PST":
                        TimeZoneOptionSet.Set(executionContext, new OptionSetValue((int)TimeZones.PST));
                        break;
                    case "PDT":
                        TimeZoneOptionSet.Set(executionContext, new OptionSetValue((int)TimeZones.PDT));
                        break;
                    case "HST":
                        TimeZoneOptionSet.Set(executionContext, new OptionSetValue((int)TimeZones.HST));
                        break;
                    case "AKST":
                        TimeZoneOptionSet.Set(executionContext, new OptionSetValue((int)TimeZones.AKST));
                        break;
                    case "AKDT":
                        TimeZoneOptionSet.Set(executionContext, new OptionSetValue((int)TimeZones.AKDT));
                        break;

                    default:
                        TimeZoneOptionSet.Set(executionContext, new OptionSetValue((int)TimeZones.Other));
                        break;
                }
            }
            catch (Exception ex)
            {
                TimeZoneOptionSet.Set(executionContext, new OptionSetValue((int)TimeZones.Other));
                tracingService.Trace($" {ex.Message}");
            }
           
        }

        public enum TimeZones
        {
            CST = 752280000,
            CDT = 752280001,
            MST = 752280002,
            MDT = 752280003,
            EST = 752280004,
            EDT = 752280005,
            PST = 752280006,
            PDT = 752280007,
            HST = 752280008,
            AKST = 752280009,
            AKDT = 752280010,
            Other = 752280011
        }
    }
}
