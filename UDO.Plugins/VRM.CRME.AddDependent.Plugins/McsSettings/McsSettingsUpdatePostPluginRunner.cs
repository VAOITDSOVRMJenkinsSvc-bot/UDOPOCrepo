using System;
using MCSPlugins;
using Microsoft.Xrm.Sdk;

namespace VRM.CRME.Plugin.DependentMaintenance.McsSettings
{
    public class McsSettingsUpdatePostPluginRunner : PluginRunner
    {
        public McsSettingsUpdatePostPluginRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        internal void Execute(IServiceProvider serviceProvider)
        {
            
                
            //End the timing for the plugin
            Logger.WriteTxnTimingMessage(String.Format("Ending :{0}", GetType()));
        }

        public override Entity GetPrimaryEntity()
        {
            throw new NotImplementedException();
        }

        public override Entity GetSecondaryEntity()
        {
            throw new NotImplementedException();
        }

        public override string McsSettingsDebugField
        {
            get { return "crme_dependent"; }
        }
    }
}
