using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.Workflows
{
    public class GetTimeZoneByAreaCode : CodeActivity
    {
       
        [Input("AreaCode")]
        public InArgument<string> AreaCode { get; set; }
        [Input("AreaCode2")]
        public InArgument<string> AreaCode2 { get; set; }
        [Output("TimeZone")]
        public OutArgument<string> timeZoneGenericName { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            string areaCode = AreaCode.Get(executionContext);
            string areaCode2 = AreaCode2.Get(executionContext);
            string code = string.Empty;
            if (string.IsNullOrEmpty(areaCode) && string.IsNullOrEmpty(areaCode2))
            {
                timeZoneGenericName.Set(executionContext, "Not Available");
            }
            string timeZone = string.Empty;
            if (!string.IsNullOrEmpty(areaCode))
            {
                areaCode = areaCode.Substring(0, 3);
            }
           if (!string.IsNullOrEmpty(areaCode2))
            {
                areaCode2 = areaCode2.Substring(0, 3);
            }
            if (!string.IsNullOrEmpty(areaCode))
            {
                code = areaCode;
            }
            else if (!string.IsNullOrEmpty(areaCode2))
            {
                code = areaCode2;
            }
            else
                return;
                
            
            try
            {
                string fetchXmlQuery = $@"<fetch top='1' no-lock='true' >
              <entity name='udo_vassareacode' >
                <attribute name='udo_timezone' />
            	<attribute name='udo_vassareacodeid' />
                <filter type='and' >
                  <condition attribute='udo_areacode' operator='eq' value='{code}' />
                </filter>
              </entity>
            </fetch>";
                FetchExpression fetchExpression = new FetchExpression(fetchXmlQuery);
                tracingService.Trace("fetch expression to retrieve timeZone");
                EntityCollection entityCollection = service.RetrieveMultiple(fetchExpression);
                if (entityCollection.Entities.Count > 0)
                {
                    tracingService.Trace($"Number of entities{entityCollection.Entities.Count}");
                    if (entityCollection.Entities[0].Contains("udo_timezone"))
                    {
                        timeZone = entityCollection.Entities[0].GetAttributeValue<string>("udo_timezone");
                        timeZoneGenericName.Set(executionContext, timeZone);
                    }
                    else
                        timeZoneGenericName.Set(executionContext, "Not Available");
                }
            }
            catch (Exception ex)
            {
                timeZoneGenericName.Set(executionContext, "Not Available");
                tracingService.Trace($"{ex.Message}");
            }
                    
            
        }
    }
}
