using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.Workflows
{
    public class FormatNames : CodeActivity
    {
        [RequiredArgument]
        [Input("Name")]
        public InArgument<string> Name { get; set; }

        [Output("Formatted Name")]
        public OutArgument<string> FormattedName { get; set; }
        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            try
            {
                tracingService.Trace("Get the input fields.");
                string name = Name.Get(executionContext);

                string formattedName = string.Empty;

                if (name.Contains("'"))
                {
                    tracingService.Trace("Name contains an apostrophe");
                    var words = name.Split('\'');
                    tracingService.Trace("Splitted name by an apostrophe!");
                    for (int i = 0; i < words.Length; i++)
                    {
                        tracingService.Trace(words[i]);
                        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                        words[i] = textInfo.ToTitleCase(words[i].ToLower());
                    }

                    formattedName = string.Join("'", words);

                    tracingService.Trace($"Full formatted name: {formattedName}");
                }
                else
                {
                    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                    formattedName = textInfo.ToTitleCase(name.ToLower());
                }

                tracingService.Trace($"Formatted name is {formattedName}");

                tracingService.Trace("Setting the output variable");
                FormattedName.Set(executionContext, formattedName);
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
