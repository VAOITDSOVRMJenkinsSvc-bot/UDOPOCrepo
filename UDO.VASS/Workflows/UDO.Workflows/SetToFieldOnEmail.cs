using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

namespace UDO.Workflows
{
    public class SetToFieldOnEmail : CodeActivity
    {
        [RequiredArgument]
        [Input("Email")]
        [ReferenceTarget("email")]
        public InArgument<EntityReference> sourceEmailInput { get; set; }

        [RequiredArgument]
        [Input("Email Address")]
        public InArgument<string> emailAddressInput { get; set; }

        [Output("EmailOutput")]
        [ReferenceTarget("email")]
        public OutArgument<EntityReference> emailReferenceOutput { get; set; }

        [RequiredArgument]
        [Input("Send Email")]
        public InArgument<bool> sendEmail { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            tracingService.Trace("Get the input fields.");

            EntityReference emailRef = sourceEmailInput.Get(executionContext);
            string emailAddress = emailAddressInput.Get(executionContext);

            tracingService.Trace("Set the to field on the email to " + emailAddress);

            Entity emailEntity = new Entity("email", emailRef.Id);
            EntityCollection colAP = new EntityCollection();
            Entity activityParty = new Entity("activityparty");
            activityParty["addressused"] = emailAddress;
            colAP.Entities.Add(activityParty);

            emailEntity["to"] = colAP;

            service.Update(emailEntity);            

            emailReferenceOutput.Set(executionContext, emailEntity.ToEntityReference());

            if(sendEmail.Get(executionContext))
            {
                tracingService.Trace("Sending the email");
                SendEmailRequest reqSendEmail = new SendEmailRequest();
                reqSendEmail.EmailId = emailRef.Id;
                reqSendEmail.IssueSend = true;
                reqSendEmail.TrackingToken = "";
                service.Execute(reqSendEmail);

            }

        }
    }
}
