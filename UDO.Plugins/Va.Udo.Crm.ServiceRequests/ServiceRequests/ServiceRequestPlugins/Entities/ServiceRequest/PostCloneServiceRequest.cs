// <copyright file="PostCloneServiceRequest.cs" company="">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author></author>
// <date>8/25/2015 12:56:18 PM</date>
// <summary>Implements the PreServiceRequestCreate Plugin.</summary>
namespace Va.Udo.Crm.ServiceRequests.Plugins
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// PostCloneServiceRequest Plugin.
    /// Fires when the following attributes are updated:
    /// All Attributes
    /// </summary>    
    public class PostCloneServiceRequest : IPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostServiceRequestUpdate"/> class.
        /// </summary>
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new PostCloneServiceRequestRunner(serviceProvider);
            runner.Execute();
        }
    }
}
