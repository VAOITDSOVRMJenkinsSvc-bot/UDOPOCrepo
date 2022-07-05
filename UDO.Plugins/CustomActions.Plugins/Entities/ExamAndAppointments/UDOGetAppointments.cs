using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.ExamAndAppointments
{
    public class UDOGetAppointments : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExamAndAppointments"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetAppointmentsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
