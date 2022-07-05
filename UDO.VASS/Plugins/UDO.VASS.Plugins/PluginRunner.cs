using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.Plugins
{
    public abstract class PluginRunner
    {
        private readonly IServiceProvider _ServiceProvider;
        private IPluginExecutionContext _PluginExecutionContext;
        private IOrganizationServiceFactory _OrganizationServiceFactory;
        private IOrganizationService _OrganizationService;
        private IOrganizationService _ElevatedOrganizationService;
        private ITracingService _TracingService;

        protected PluginRunner(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            _ServiceProvider = serviceProvider;
        }
        #region Internal Methods/Properties
        internal IServiceProvider ServiceProvider
        {
            get
            {
                return _ServiceProvider;
            }
        }
        internal IPluginExecutionContext PluginExecutionContext
        {
            get
            {
                return _PluginExecutionContext ??
                       (_PluginExecutionContext =
                        (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext)));
            }
        }

        internal IOrganizationServiceFactory OrganizationServiceFactory
        {
            get
            {
                return _OrganizationServiceFactory ??
                       (_OrganizationServiceFactory =
                        (IOrganizationServiceFactory)ServiceProvider.GetService(typeof(IOrganizationServiceFactory)));
            }
        }

        internal IOrganizationService OrganizationService
        {
            get
            {
                return _OrganizationService ??
                       (_OrganizationService =
                        OrganizationServiceFactory.CreateOrganizationService(PluginExecutionContext.UserId));
            }
        }

        internal IOrganizationService ElevatedOrganizationService
        {
            get
            {
                return _ElevatedOrganizationService ??
                       (_ElevatedOrganizationService =
                        OrganizationServiceFactory.CreateOrganizationService(null));
            }
        }

        internal OrganizationServiceContext OrganizationServiceContext
        {

            get
            {
                var OrgService = _OrganizationService ??
                       (_OrganizationService =
                        OrganizationServiceFactory.CreateOrganizationService(PluginExecutionContext.UserId));
                return new OrganizationServiceContext(OrgService);

            }
        }

        internal ITracingService TracingService
        {
            get
            {
                return _TracingService ??
                       (_TracingService =
                        (ITracingService)ServiceProvider.GetService(typeof(ITracingService)));
            }
        }
        #endregion
    }
}
