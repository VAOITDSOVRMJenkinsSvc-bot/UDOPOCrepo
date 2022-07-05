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
    public class CalculateFollowUpInterval : CodeActivity
    {

        [RequiredArgument]
        [Input("Released from Active Duty Date")]

        public InArgument<DateTime> ReleasedActiveDutyDate { get; set; }
        [RequiredArgument]
        [Input("FollowUpCount")]
        public InArgument<int> FollowUpCount { get; set; }
        [RequiredArgument]
        [Input("FollowUpDuration(Months)")]
        public InArgument<int> FollowUpDuration { get; set; }

        [Output("Interval Start Date")]
        public OutArgument<DateTime> IntervalStartDate { get; set; }
        [Output("Interval End Date")]
        public OutArgument<DateTime> IntervalEndDate { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            int followUpCount = FollowUpCount.Get(context);
            DateTime radDate = ReleasedActiveDutyDate.Get(context);
            int followUpDuration = FollowUpDuration.Get(context);
            ITracingService trace = context.GetExtension<ITracingService>();

            #region Basic Input Validation
            if (followUpCount < 0)
            {
                throw new InvalidPluginExecutionException("Invalid Input : FollowUpCount");
            }
            if (radDate == DateTime.MinValue || radDate == DateTime.MaxValue)
            {
                throw new InvalidPluginExecutionException("Invalid Input : RAD Date");
            }
            if (followUpDuration < 1)
            {
                throw new InvalidPluginExecutionException("Invalid input: Follow Up Duration");
            }
            #endregion

            trace.Trace($"All input validation commplete.RAD : {radDate.ToUniversalTime().ToString()}");
            var vassEndDate = radDate.AddMonths(12);
            trace.Trace($"VASS End Date: {vassEndDate.ToUniversalTime().ToString()}");
            DateTime intervalStart = vassEndDate.AddMonths(followUpCount * 3).AddDays(1);
            DateTime intervalEnd = vassEndDate.AddMonths(followUpCount * 3 + 3);
            trace.Trace($"Interval Start: {intervalStart.ToUniversalTime().ToString()}");
            trace.Trace($"Interval End: {intervalEnd.ToUniversalTime().ToString()}");
            IntervalStartDate.Set(context, intervalStart);
            IntervalEndDate.Set(context, intervalEnd);
        }
    }
}
