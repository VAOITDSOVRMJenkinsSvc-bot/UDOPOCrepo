using MCSHelperClass;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System;
namespace MCSPlugins
{
    public abstract class PluginRunner
    {
        #region Private Fields
        private readonly IServiceProvider _ServiceProvider;
        private IPluginExecutionContext _PluginExecutionContext;
        private IOrganizationServiceFactory _OrganizationServiceFactory;
        private IOrganizationService _OrganizationService;
        private ITracingService _TracingService;
        private MCSLogger _Logger;
        private UtilityFunctions _UtilityFunctions;
        private MCSSettings _McsSettings;
        private MCSHelper _McsHelper;
        #endregion

        #region Constructor
        protected PluginRunner(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            _ServiceProvider = serviceProvider;

            Logger.setDebug = McsSettings.getDebug;
            Logger.setTxnTiming = McsSettings.getTxnTiming;
            Logger.setGranularTiming = McsSettings.getGranular;

            McsSettings.setLogger = Logger;
        }
        #endregion

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
                        OrganizationServiceFactory.CreateOrganizationService(PluginExecutionContext.InitiatingUserId));
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

        internal MCSLogger Logger
        {
            get
            {
                return _Logger ??
                       (_Logger = new MCSLogger
                       {
                           setService = OrganizationService,
                           setTracingService = TracingService,
                           setModule = String.Format("{0}:", GetType())
                       });
            }
        }

        internal UtilityFunctions Utilities
        {
            get
            {
                return _UtilityFunctions ??
                    (_UtilityFunctions = new UtilityFunctions
                    {
                        setService = OrganizationService,
                        setLogger = Logger
                    });
            }
        }

        internal MCSSettings McsSettings
        {
            get
            {
                if (_McsSettings == null)
                {
                    _McsSettings = new MCSSettings
                    {
                        setService = OrganizationService,
                        setDebugField = McsSettingsDebugField,
                            systemSetting = "Active Settings"
                    };

                    _McsSettings.GetStartupSettings();
                }

                return _McsSettings;
            }
        }

        internal MCSHelper McsHelper
        {
            get
            {
                return _McsHelper ??  (_McsHelper = new MCSHelper(GetPrimaryEntity(), GetSecondaryEntity()));
            }
        }

        public abstract Entity GetPrimaryEntity();

        public abstract Entity GetSecondaryEntity();

        public abstract string McsSettingsDebugField { get; }
        #endregion
    }
}
