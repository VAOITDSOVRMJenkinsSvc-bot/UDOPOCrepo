using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Bgs.Services.AwardWebServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.DocOperationsReference;
using VRM.Integration.Servicebus.Bgs.Services.PdfServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.PersonWebServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.StandardDataWebServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.VetRecordWebServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.VnpBnftClaimServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.VnpPersonServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.VnpProcFormServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.VnpProcServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntAddrsServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntPhoneServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntRlnshpServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.VnpPtcpntServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.ClaimantWebServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.AddressWebServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.VnpChildSchoolServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.VnpChildStudentServiceReference;
using VRM.Integration.Servicebus.Bgs.Services.PhoneWebServiceReference;
using VEIS.Core.Configuration;
using VEIS.Core.Wcf;

namespace VRM.Integration.Servicebus.Bgs.Services
{
    public static class BgsServiceFactory
    {
        public static BenefitClaimService GetBenefitClaimService(string organizationName, IOrganizationService organizationService, Guid userId, OnAfterReceiveReplyDelegate onReplyCallBack)
        {
            var channel = new BgsCustomClientChannel<BenefitClaimService>("BenefitClaimWebServicePort", 
                ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            channel.OnBeforeSendRequest += OnBenefitClaimServiceBeforeSendRequestHandler;

            channel.OnAfterReceiveReply += onReplyCallBack;

            return channel.CreateChannel();
        }

        private static object OnBenefitClaimServiceBeforeSendRequestHandler(ref Message request, IClientChannel channel)
        {
            //Convert Request to Document
            var xmlDocument = request.ToXmlDocument();

            //Grab Root Node
            var rootNode = xmlDocument.DocumentElement;

            if (rootNode == null)
                return null;

            //Add Root Level Name Spaces
            rootNode.SetAttribute("xmlns:q0", "http://services.share.benefits.vba.va.gov/");
            rootNode.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            rootNode.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");

            //Get the Body Element
            var bodyElement = xmlDocument.GetElementsByTagName("s:Body")[0];

            //Get the Old BenefitClaimInput Element
            var rootElement = xmlDocument.GetElementsByTagName("benefitClaimInput")[0];

            //Get the Existing InsertBenefitClaim Element - We are going to delete it
            var deleteElement = xmlDocument.GetElementsByTagName("insertBenefitClaim")[0];

            //Delete the existing InsertBenefitClaim Element
            bodyElement.RemoveChild(deleteElement);

            //Create a new InsertBenefitClaim Element
            var insertBenefitClaimElement = xmlDocument.CreateElement("q0",
                "insertBenefitClaim",
                "http://services.share.benefits.vba.va.gov/");

            //Append the new InsertBenefitClaim Element to the Body Element
            bodyElement.AppendChild(insertBenefitClaimElement);

            //Create a new BenefitClaimInput Element
            var benefitClaimInputElement = xmlDocument.CreateElement("benefitClaimInput");

            //Append the new BenefitClaimInput Element to the new InsertBenefitClaim Element 
            insertBenefitClaimElement.AppendChild(benefitClaimInputElement);

            //Append all of the old children from the old BenefitClaimInput to the new BenefitClaimInput
            foreach (var childNode in rootElement.ChildNodes)
            {
                var xmlNode = (XmlNode)childNode;

                var newElement = xmlDocument.CreateElement(xmlNode.LocalName);

                benefitClaimInputElement.AppendChild(newElement).InnerText = xmlNode.InnerText;
            }

            //Convert the XmlDocument back to a message
            var message = xmlDocument.ToMessage(request.Version);

            //Return the message
            request = message;

            return null;
        }

        public static VetRecordService GetVetRecordService(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<VetRecordService>("VetRecordWebServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static VetRecordService GetVetRecordServiceProdTest(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<VetRecordService>("DansPort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static PersonWebService GetPersonWebService(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<PersonWebService>("PersonWebServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static VnpProcService GetVnpProcService(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<VnpProcService>("VnpProcServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static VnpProcFormService GetVnpProcFormService(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<VnpProcFormService>("VnpProcFormServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static VnpPtcpntService GetVnpPtcpntService(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<VnpPtcpntService>("VnpPtcpntServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static VnpPersonService GetVnpPersonService(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<VnpPersonService>("VnpPersonServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static VnpPtcpntAddrsService GetVnpPtcpntAddrsService(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<VnpPtcpntAddrsService>("VnpPtcpntAddrsServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static VnpPtcpntPhoneService GetVnpPtcpntPhoneService(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<VnpPtcpntPhoneService>("VnpPtcpntPhoneServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static VnpPtcpntRlnshpService GetVnpPtcpntRlnshpService(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<VnpPtcpntRlnshpService>("VnpPtcpntRlnshpServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static VnpChildSchoolService GetVnpChildSchoolService(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<VnpChildSchoolService>("VnpChildSchoolServicePort1", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static VnpChildStudentService GetVnpChildStudentService(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<VnpChildStudentService>("VnpChildSchoolServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }


        public static VnpBnftClaimService GetVnpBnftClaimService(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<VnpBnftClaimService>("VnpBnftClaimServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static StandardDataWebService GetStandardDataWebService(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<StandardDataWebService>("StandardDataWebServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static AwardWebService GetAwardWebService(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<AwardWebService>("AwardWebServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static ClaimantService GetClaimantWebServiceReference(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<ClaimantService>("ClaimantWebServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static AddressWebService GetAddressWebServiceReference(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<AddressWebService>("AddressWebServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static PhoneWebService GetPhoneWebServiceReference(string organizationName, IOrganizationService organizationService, Guid userId)
        {
            var channel = new BgsCustomClientChannel<PhoneWebService>("PhoneWebServicePort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddBgsSecurityHeader(organizationService, userId);

            return channel.CreateChannel();
        }

        public static DocOperations GetDocOperationsService(string organizationName)
        {
            var channel = new BgsCustomClientChannel<DocOperations>("DocOperationsImplPort", ConfigurationLocation.GetConfigFilePath(organizationName));

            channel.Endpoint.AddVvaSecurityHeader();

            return channel.CreateChannel();
        }

        public static IPdfService GetPdfService(string organizationName)
        {
            var channel = new BgsCustomClientChannel<IPdfService>("BasicHttpBinding_IPdfService", ConfigurationLocation.GetConfigFilePath(organizationName));

            return channel.CreateChannel();
        }
    }
}
