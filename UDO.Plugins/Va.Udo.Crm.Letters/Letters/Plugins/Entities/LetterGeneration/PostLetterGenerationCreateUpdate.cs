// <copyright file="PreLetterCreate.cs" company="">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author></author>
// <date>3/9/2016 12:56:18 PM</date>
// <summary>Implements the PreLetterCreate Plugin.</summary>
namespace Va.Udo.Crm.Letters.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// PostLetterUpdate Plugin.
    /// Fires when the following attributes are updated:
    /// All Attributes
    /// </summary>    
    public class PostLetterCreateUpdate : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostLetterUpdate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            //Extract the tracing service for use in debugging sandboxed plug-ins.
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            var runner = new PostLetterGenerationCreateUpdateRunner(serviceProvider);
            runner.Execute(tracingService);
        }
    }
}