using System;
using Microsoft.Xrm.Sdk;

namespace UDO.CustomActions.FNOD.Plugins.SsaInquiry
{
    public class UDOSsaDeathMatchInquiry : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var runner = new UDOSsaDeathMatchInquiryRunner(serviceProvider);
            runner.Execute();
        }
    }
}
