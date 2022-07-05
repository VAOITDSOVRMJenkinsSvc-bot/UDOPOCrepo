using System;
using System.Linq;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.WebServiceClient;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;


//using CRM007.CRM.SDK.Core;
using VRM.Integration.Servicebus.Core;
using VRM.IntegrationServicebus.AddDependent.CrmModel;

namespace UDO.LOB.DependentMaintenance
{
    [Serializable]
    public class AddDependentConfiguration
    {
        private static mcs_setting GetMcsSetting(OrganizationServiceContext context)
        {
            var settings = (from d in context.CreateQuery<mcs_setting>()
                            select d).FirstOrDefault();

            return settings;
        }

        private static AddDependentConfiguration CreateNewAddDependentConfiguration(mcs_setting settings)
        {
            return new AddDependentConfiguration
            {
                LoadPdfToVva = settings.crme_Load686CPDFtoVVA.GetValueOrDefault(),
                LoadPdfToVbms = settings.GetAttributeValue<bool>("udo_load686cpdftovbms"),
                AttachPdfToAdRecord = settings.crme_Attach686CPDFtoAddDependentTransaction.GetValueOrDefault(),
                AttachWordDocToAdRecord = settings.crme_Attach686CMSWordtoAddDependentTransaction.GetValueOrDefault(),
                VvaCallerApplicationName = settings.crme_VVACallerApplicationName,
                OrchestrationSetReadyState = settings.crme_OrchestrationSetReadyState.GetValueOrDefault(),

                AttachPdfToAdRecordError = settings.crme_Attach686CPDFtoAddDependentTransError.GetValueOrDefault(),
                AttachWordDocToAdRecordError = settings.crme_Attach686CMSWordtoAddDependentTransError.GetValueOrDefault(),
                LoadPdfToVvaError = settings.crme_Load686CPDFtoVVAError.GetValueOrDefault(),
                SubmtrApplcnTypeCd = settings.crme_SubmtrApplcnTypeCd,
                SubmtrRoleTypeCd = settings.crme_SubmtrRoleTypeCd
            };
        }

        public static AddDependentConfiguration GetCurrent()
        {
            return GetCurrent(null);
        }

        public static AddDependentConfiguration GetCurrent(OrganizationServiceContext context)
        {
            if (context == null)
                context = new OrganizationServiceContext(ConnectionCache.GetProxy());

            var settings = GetMcsSetting(context);

            if (settings == null)
                throw new Exception("The Add Dependent configuration record has not been specified");

            return CreateNewAddDependentConfiguration(settings);
        }


        public bool LoadPdfToVva { get; private set; }
        public bool LoadPdfToVbms { get; private set; }
        public bool AttachPdfToAdRecord { get; private set; }
        public bool AttachWordDocToAdRecord { get; private set; }
        public bool OrchestrationSetReadyState { get; private set; }
        public string VvaCallerApplicationName { get; private set; }
        public bool AttachWordDocToAdRecordError { get; set; }
        public bool AttachPdfToAdRecordError { get; set; }
        public bool LoadPdfToVvaError { get; set; }
        public string SubmtrApplcnTypeCd { get; set; }
        public string SubmtrRoleTypeCd { get; set; }
    }
}
