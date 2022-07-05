using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.CustomActions.DepMaint.Plugins.School
{
    public class FindSchool : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new FindSchoolRunner(serviceProvider);
            runner.Execute();
        }
    }
}
