﻿using System;
using Microsoft.Xrm.Sdk;

namespace CustomActions.Plugins.Entities.Evidence
{
    public class UDOGetEvidence : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostIdProofCreate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOGetEvidenceRunner(serviceProvider);
            runner.Execute();
        }
    }
}
