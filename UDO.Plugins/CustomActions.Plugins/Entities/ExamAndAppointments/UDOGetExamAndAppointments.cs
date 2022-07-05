using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.ExamAndAppointments
{
    public class UDOGetExamAndAppointments : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExamAndAppointments"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetExamAndAppointmentsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
