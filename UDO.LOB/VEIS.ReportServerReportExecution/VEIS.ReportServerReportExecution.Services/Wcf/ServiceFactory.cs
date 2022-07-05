using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel.Channels;
using VEIS.Core.Configuration;
using VEIS.Core.Messages;
using VEIS.Core.Wcf;
using VEIS.ReportServerReportExecution.Services.ReportServerReportExecution; 

namespace VEIS.ReportServerReportExecution.Services
{
    public static class ServiceFactory
    {

        public static ReportServerReportExecution.ReportExecutionServiceSoap GetReportServerReportExecutionServiceReference(IOrganizationService organizationService, Guid userId)
        {
            var channel = new ServiceCustomClientChannel<ReportServerReportExecution.ReportExecutionServiceSoap>("ReportExecutionServiceSoap", ConfigurationLocation.GetConfigFilePath("EC"));



            SoapLog.Current.Active = ReportServerReportExecutionSecurityConfiguration.Current.EnableLogging;

            return channel.CreateChannel();
        }

        public static ReportServerReportExecution.ReportExecutionServiceSoap GetReportServerReportExecutionServiceReference(LegacyHeaderInfo headerInfo)
        {
            var channel = new ServiceCustomClientChannel<ReportServerReportExecution.ReportExecutionServiceSoap>("ReportExecutionServiceSoap", ConfigurationLocation.GetConfigFilePath("EC"));

            SoapLog.Current.Active = ReportServerReportExecutionSecurityConfiguration.Current.EnableLogging;

            return channel.CreateChannel();
        }

    }
}
