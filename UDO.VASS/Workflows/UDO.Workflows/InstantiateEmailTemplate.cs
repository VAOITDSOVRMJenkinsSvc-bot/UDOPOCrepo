using Microsoft.Crm.Sdk.Messages;
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
    /// <summary>
    /// This Custom workflow activity does not send out the email , we are just Instatiating the email body and email subject with instantiateTemplateRequest object.
    /// </summary>
    public class InstantiateEmailTemplate : CodeActivity
    {
        [RequiredArgument]
        [Input("EmailTemplateTitle")]
        public InArgument<string> emailTemplateTitle { get; set; }

        [RequiredArgument]
        [Input("Email")]
        [ReferenceTarget("email")]
        public InArgument<EntityReference> sourceEmailInput { get; set; }

        [RequiredArgument]
        [Input("Contact")]
        [ReferenceTarget("contact")]
        public InArgument<EntityReference> contactReference { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {



                tracingService.Trace("Execution context ----Get Input fields");
                string emailTemmplateName = emailTemplateTitle.Get(executionContext);
                EntityReference contactInfo = contactReference.Get(executionContext);
                if (string.IsNullOrWhiteSpace(emailTemmplateName)&& contactInfo ==null)
                {
                    tracingService.Trace("email template or contact info not found");
                    return;
                }
                tracingService.Trace($"Email Template retrieved {emailTemmplateName} and contact --{contactInfo.LogicalName}");

                string fetchXmltoGetEmailTemplateInfo = $@"<fetch top='10' no-lock='true' >
  <entity name='template' >
    <attribute name='templateid' />
	<attribute name='templatetypecode' />
    <filter>
      <condition attribute='title' operator='eq' value='{emailTemmplateName}' />
    </filter>
  </entity>
</fetch>";

                EntityCollection templateInformation = service.RetrieveMultiple(new FetchExpression(fetchXmltoGetEmailTemplateInfo));
                tracingService.Trace("no problem with xml query for templates");
                string templatetypecode = string.Empty;
                Guid templateId = new Guid();
                if (templateInformation.Entities.Count > 0 && templateInformation.Entities[0].Attributes.Contains("templateid"))
                {
                    tracingService.Trace($"Retireved template Information -- {templateInformation.Entities[0].GetAttributeValue<Guid>("templateid")}");
                    templateId = templateInformation.Entities[0].GetAttributeValue<Guid>("templateid");
                    templatetypecode = templateInformation.Entities[0].GetAttributeValue<string>("templatetypecode");
                }
                else
                {
                    tracingService.Trace(" Email template or template id not found");
                    return;
                }

                InstantiateTemplateRequest instantiateTemplateRequest = new InstantiateTemplateRequest
                {
                    ObjectId = contactInfo.Id,
                    TemplateId = templateId,
                    ObjectType = templatetypecode
                };
                //execute message

                InstantiateTemplateResponse instantiateTemplateResponse = (InstantiateTemplateResponse)service.Execute(instantiateTemplateRequest);
                tracingService.Trace("InstantiateTemplateResponse data retrieved");
                Entity emailInformation = new Entity();
                //Store response in collection
                var emailInfoCollection = (EntityCollection)instantiateTemplateResponse["EntityCollection"];
                if (emailInfoCollection.Entities.Count > 0)
                {
                    emailInformation = emailInfoCollection[0];
                    foreach (var item in emailInformation.Attributes)
                    {
                        tracingService.Trace($"Key -- {item.Key} --- Value -- {item.Value} ");
                    }
                    tracingService.Trace(" Tempalte information received");
                    if (emailInformation.Attributes.Contains("description") && emailInformation.Attributes.Contains("subject"))
                    {

                        EntityReference emailRef = sourceEmailInput.Get(executionContext);
                        if (emailRef == null)
                        {
                            tracingService.Trace("Email reference not available");
                            return;
                        }
                        Entity emailEntity = new Entity("email", emailRef.Id);
                        emailEntity["description"] = emailInformation.GetAttributeValue<string>("description");
                        emailEntity["subject"] = emailInformation.GetAttributeValue<string>("subject");

                        service.Update(emailEntity);
                        tracingService.Trace("Email Entity Updated");
                    }
                    else
                    {
                        tracingService.Trace("Email Description or Subject not found in the body");
                        return;
                    }
                }
                else
                {
                    tracingService.Trace("No email Information Obtained ");
                    return;
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace($"Exception ----{ex.InnerException} {ex.Message}");

            }
        }
    }
}
