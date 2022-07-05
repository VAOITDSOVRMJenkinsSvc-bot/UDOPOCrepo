using Microsoft.Xrm.Sdk;
using System;

namespace Va.Udo.Crm.Plugins.Security
{
    /// <summary>
    /// Assign records to the default team.
    /// </summary>
    public class SecurityPreCreate:IPlugin
    {
        private string _unSecure;
        public SecurityPreCreate(string unSecure, string Secure)
        {
            _unSecure = unSecure;
        }


        /// <summary>
        /// Execution logic of the plugin: Assign the record to the default team.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void Execute(IServiceProvider serviceProvider)
        {


            
                AssignmentRunner runner = new AssignmentRunner(serviceProvider);
            
            if (String.IsNullOrEmpty(_unSecure))
            {
                throw new InvalidPluginExecutionException("Unsecure Configuration not set for PreUpdate step.");
            }
           runner.Execute(_unSecure);
            
        }
    }
}
