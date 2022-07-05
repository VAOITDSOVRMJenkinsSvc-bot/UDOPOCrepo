using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.Workflows
{
    public class Add : CodeActivity
    {
        [RequiredArgument]
        [Input("Number 1")]
        public InArgument<int> Number1 { get; set; }
        [RequiredArgument]
        [Input("Number 2")]
        public InArgument<int> Number2 { get; set; }
        [RequiredArgument]
        [Output("Sum")]
        public OutArgument<int> Sum { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var number1 = Number1.Get<int>(context);
            var number2 = Number2.Get<int>(context);
            Sum.Set(context, number1 + number2);
        }
    }
}
