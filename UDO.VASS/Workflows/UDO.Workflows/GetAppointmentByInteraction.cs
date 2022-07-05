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
    public class GetAppointmentByInteraction : CodeActivity
    {
        [RequiredArgument]
        [Input("Interaction")]
        [ReferenceTarget("udo_interaction")]
        public InArgument<EntityReference> Interaction { get; set; }

        [Output("Appointment")]
        [ReferenceTarget("appointment")]
        public OutArgument<EntityReference> AppointmentOut { get; set; }

        [Output("CountRecordsFound")]
        public OutArgument<int> CountRecordsOutput { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {                
                tracingService.Trace("Get the input fields.");
                Guid interactionGuid = Interaction.Get(executionContext).Id;

                var fetchAppointment = $@"<fetch>
                                          <entity name='appointment' >
                                            <attribute name='statecode' />                                            
                                            <filter type='and'>
                                              <condition attribute='regardingobjectid' operator='eq' value='{interactionGuid}' />
                                              <condition attribute='statecode' operator='neq' value='2' />
                                            </filter>
                                            <order attribute='modifiedon' descending='true' />
                                          </entity>
                                        </fetch>";

                FetchExpression fetchExpression = new FetchExpression(fetchAppointment);
                EntityCollection appointmentColl = service.RetrieveMultiple(fetchExpression);

                tracingService.Trace($"Fetched {appointmentColl.Entities.Count} appointments.");                

                CountRecordsOutput.Set(executionContext, appointmentColl.Entities.Count);
                tracingService.Trace($"CountRecordOutput value set to {appointmentColl.Entities.Count}");

                if (appointmentColl.Entities.Count >= 1)
                {
                    EntityReference appointmentRef = appointmentColl.Entities[0].ToEntityReference();                    
                    AppointmentOut.Set(executionContext, appointmentRef);
                }
                
            }
            catch (Exception ex)
            {
                tracingService.Trace("Exception Occurred!");
                tracingService.Trace("Message: " + ex.Message);
                if (ex.InnerException != null)
                {
                    tracingService.Trace("Inner Exception: " + ex.InnerException);
                }
            }
        }
    }
}
