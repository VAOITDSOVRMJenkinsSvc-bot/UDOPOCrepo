﻿using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Dependents
{
    public class UDOGetDependentAwards : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetDependentAwardsRunner(serviceProvider);
            runner.Execute();
        }
    }
}
