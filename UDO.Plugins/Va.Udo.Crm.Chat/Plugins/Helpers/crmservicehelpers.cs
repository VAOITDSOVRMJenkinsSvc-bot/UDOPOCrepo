// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================

//<snippetCrmServiceHelper>
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;

namespace MCSUtilities2011
{
    /// <summary>
    /// Provides server connection information.
    /// </summary>
    public class ServerConnection
    {
        /// <summary>
        /// Stores CRM server configuration information.
        /// </summary>
        public class Configuration
        {
            public String ServerAddress;
            public String OrganizationName;
            public Uri DiscoveryUri;
            public Uri OrganizationUri;
            public Uri HomeRealmUri = null;
            public ClientCredentials DeviceCredentials = null;
            public ClientCredentials Credentials = null;
            public AuthenticationProviderType EndpointType;

        }

        public List<Configuration> configurations = null;


        private Configuration config = new Configuration();
        /// <summary>
        /// Obtains the server connection information including the target organization's
        /// Uri and user login credentials from the user.
        /// </summary>
        public virtual Configuration GetServerConfiguration(string orgName, string serverAddress)
        {
            // CRM Online in the North American data center.
            config.ServerAddress = serverAddress;
            config.DiscoveryUri = new Uri(String.Format("http://{0}/XRMServices/2011/Discovery.svc", config.ServerAddress));

            // Get the user's logon credentials.
            config.Credentials = GetUserLogonCredentials();

            config.OrganizationUri = GetOrganizationAddress(config.DiscoveryUri, orgName);

            return config;
        }

        /// <summary>
        /// Discovers the organizations that the calling user belongs to.
        /// </summary>
        /// <param name="service">A Discovery service proxy instance.</param>
        /// <returns>Array containing detailed information on each organization that 
        /// the user belongs to.</returns>
        public OrganizationDetailCollection DiscoverOrganizations(IDiscoveryService service)
        {
            try
            {

                RetrieveOrganizationsRequest orgRequest = new RetrieveOrganizationsRequest();
                RetrieveOrganizationsResponse orgResponse =
                    (RetrieveOrganizationsResponse)service.Execute(orgRequest);


                return orgResponse.Details;
            }
            catch (FaultException<OrganizationServiceFault>)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
      
        /// <summary>
        /// Obtains the name and port of the server running the Microsoft Dynamics CRM
        /// Discovery service.
        /// </summary>
        /// <returns>The server's network name and optional TCP/IP port.</returns>
        protected virtual String GetServerAddress()
        {   

            var server = System.Configuration.ConfigurationManager.AppSettings["CRM2011Server"].ToString();
         

            return server;
        }

        /// <summary>
        /// Obtains the Web address (Uri) of the target organization.
        /// </summary>
        /// <param name="discoveryServiceUri">The Uri of the CRM Discovery service.</param>
        /// <returns>Uri of the organization service or an empty string.</returns>
        protected virtual Uri GetOrganizationAddress(Uri discoveryServiceUri, string orgName)
        {
            using (DiscoveryServiceProxy serviceProxy = new DiscoveryServiceProxy(discoveryServiceUri, null, config.Credentials, config.DeviceCredentials))
            {
                // Obtain organization information from the Discovery service. 
                if (serviceProxy != null)
                {
                    // Obtain information about the organizations that the system user belongs to.
                    OrganizationDetailCollection orgs = DiscoverOrganizations(serviceProxy);

                    if (orgs.Count > 0)
                    {
                        for (int n = 0; n < orgs.Count; n++)
                            if (orgs[n].FriendlyName.Equals(orgName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                config.OrganizationName = orgs[n].FriendlyName;
                                // Return the organization Uri.
                                return new System.Uri(orgs[n].Endpoints[EndpointType.OrganizationService]);
                            }
                        return new System.Uri(String.Empty);
                    }
                    else
                    {
                        return new System.Uri(String.Empty);
                    }
                }
                else
                {
                    throw new Exception("An invalid server name was specified.");
                }
            }
            
        }

        /// <summary>
        /// Obtains the user's logon credentials for the target server.
        /// </summary>
        /// <returns>Logon credentials of the user.</returns>
        protected virtual ClientCredentials GetUserLogonCredentials()
        {
            ClientCredentials credentials = new ClientCredentials(); 
            return credentials;
        }
        
    }

    #region Internal Classes

    //<snippetCrmServiceHelper1>
    /// <summary>
    /// The SolutionComponentType defines the type of solution component.
    /// </summary>
    public static class SolutionComponentType
    {
        public const int Attachment = 35;
        public const int Attribute = 2;
        public const int AttributeLookupValue = 5;
        public const int AttributeMap = 47;
        public const int AttributePicklistValue = 4;
        public const int ConnectionRole = 63;
        public const int ContractTemplate = 37;
        public const int DisplayString = 22;
        public const int DisplayStringMap = 23;
        public const int DuplicateRule = 44;
        public const int DuplicateRuleCondition = 45;
        public const int EmailTemplate = 36;
        public const int Entity = 1;
        public const int EntityMap = 46;
        public const int EntityRelationship = 10;
        public const int EntityRelationshipRelationships = 12;
        public const int EntityRelationshipRole = 11;
        public const int FieldPermission = 71;
        public const int FieldSecurityProfile = 70;
        public const int Form = 24;
        public const int KBArticleTemplate = 38;
        public const int LocalizedLabel = 7;
        public const int MailMergeTemplate = 39;
        public const int ManagedProperty = 13;
        public const int OptionSet = 9;
        public const int Organization = 25;
        public const int PluginAssembly = 91;
        public const int PluginType = 90;
        public const int Relationship = 3;
        public const int RelationshipExtraCondition = 8;
        public const int Report = 31;
        public const int ReportCategory = 33;
        public const int ReportEntity = 32;
        public const int ReportVisibility = 34;
        public const int RibbonCommand = 48;
        public const int RibbonContextGroup = 49;
        public const int RibbonCustomization = 50;
        public const int RibbonDiff = 55;
        public const int RibbonRule = 52;
        public const int RibbonTabToCommandMap = 53;
        public const int Role = 20;
        public const int RolePrivilege = 21;
        public const int SavedQuery = 26;
        public const int SavedQueryVisualization = 59;
        public const int SDKMessageProcessingStep = 92;
        public const int SDKMessageProcessingStepImage = 93;
        public const int SDKMessageProcessingStepSecureConfig = 94;
        public const int ServiceEndpoint = 95;
        public const int SiteMap = 62;
        public const int SystemForm = 60;
        public const int ViewAttribute = 6;
        public const int WebResource = 61;
        public const int Workflow = 29;
    }
    //</snippetCrmServiceHelper1>
    #endregion
}
//</snippetCrmServiceHelper>
