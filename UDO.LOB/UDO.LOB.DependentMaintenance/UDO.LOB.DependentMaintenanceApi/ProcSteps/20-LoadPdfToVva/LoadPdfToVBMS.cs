using CuttingEdge.Conditions;
//using VRM.Integration.Servicebus.Bgs.Services;
using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Bgs.Services.DocOperationsReference;
using System;
using System.Net;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Security.Cryptography.X509Certificates;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//using VRM.Integration.UDO.VBMS.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
//using VRMRest;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
//using CRM007.CRM.SDK.Core;
//using VRM.IntegrationServicebus.AddDependent.CrmModel;
using UDO.LOB.Extensions.Logging;
using VRM.IntegrationServicebus.AddDependent.CrmModel;

namespace UDO.LOB.DependentMaintenance.ProcSteps
{
    public class LoadPdfToVbms : FilterBase<IAddDependentPdfState>
    {
        private const string _MimeType = "application/pdf";
        private const string _CPdfDocument = "686C PDF Document";

        IOrganizationService OrgService;

        public override void Execute(IAddDependentPdfState msg)
        {
            //var _onPremDACUri = new Uri("");

            try
            {
				//CSDEv REm 
				//Logger.Instance.Debug("LoadPdfToVbms - Init LoadPdfToVbms");
				LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
					, msg.AddDependentMaintenanceRequestState.SystemUserId, "LoadPdfToVbms.Execute", "LoadPdfToVbms - Init LoadPdfToVbms");

				#region init
				Condition.Requires(msg.AddDependentMaintenanceRequestState,
                    "msg.AddDependentMaintenanceRequestState").IsNotNull();
                Condition.Requires(msg.AddDependentMaintenanceRequestState.Context,
                    "msg.AddDependentMaintenanceRequestState.Context").IsNotNull();

                var addDependentConfiguration = AddDependentConfiguration.GetCurrent(msg.AddDependentMaintenanceRequestState.Context);

                if (!addDependentConfiguration.LoadPdfToVbms || msg.HasOrchestrationError)
                {
                    LogHelper.LogInfo(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, "LoadPdfToVBMS Execute", "Configuration not set or Orchestration Error encountered. Skipping VBMS Upload.");
                    return;
                }

                DateTime methodStartTime;
                string method = "LoadPdfToVbms";

				//CSDEv Need Timings 
                //Guid methodLoggingId = LogHelper.StartTiming(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId,
                    //msg.AddDependentMaintenanceRequestState.DependentMaintenanceId, "crme_dependentmaintenance", "crme_relateddependentmaintenance", method, null, out methodStartTime);

                //Logger.Instance.Debug("Timing started.");

                Condition.Requires(msg, "msg")
                   .IsNotNull();

                Condition.Requires(msg.PdfFileBytes, "msg.PdfFileBytes").IsNotNull().IsNotEmpty();
                Condition.Requires(msg.PdfFileName, "msg.PdfFileName").IsNotNull().IsNotEmpty();
                #endregion

                #region connect to CRM
                try
                {
                    //var CommonFunctions = new CRMConnect();
                    //OrgServiceProxy = CommonFunctions.ConnectToCrm(msg.AddDependentMaintenanceRequestState.OrganizationName);
                    //Logger.Instance.Debug("Creating Org Svc");
                    OrgService = ConnectToCrmHelper.ConnectToCrm(msg.AddDependentMaintenanceRequestState.OrganizationName);

                }
                catch (Exception connectException)
                {
                    //Logger.Instance.Debug("Exception creating org svc");
                    LogHelper.LogError(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.SystemUserId, "LoadPdfToVBMS Step of AddDependent Orchestration, Connection Error", connectException.Message);
                    if (connectException.StackTrace != null)
                        LogHelper.LogError(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.SystemUserId, "LoadPdfToVBMS Execute", connectException.StackTrace);
                }
                #endregion

                #region fetch the correct doctype record
                var doctypeFetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                   @"<entity name='udo_vbmsdoctype'>" +
                                   @"<filter type='and'>" +
                                   @"<condition attribute='udo_name' operator='eq' value='VA 21-686c Declaration of Status of Dependents' />" +
                                   @"</filter>" +
                                   @"</entity>" +
                                   @"</fetch>";

                //Logger.Instance.Debug("Fetching DocType");

                var docTypes = OrgService.RetrieveMultiple(new FetchExpression(doctypeFetch));
                var docType = new Entity();
                if (docTypes.Entities.Count > 0)
                    docType = docTypes.Entities[0];
                else
                {
                    //Logger.Instance.Debug("No DocTypes Found");
                    LogHelper.LogError(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.SystemUserId, "LoadPdfToVBMS, Execute", "Failed to identify correct VBMS Doc Type for Dependent Maintenance (type 148)");
                    return;
                }
                #endregion

                #region Retrieve user Upload Role
                var userUploadRole = new OptionSetValue();
                var uploadRoleFetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                   @"<entity name='systemuser'>" +
                                   @"<attribute name='udo_vbmsuploadrole' />" +
                                   @"<filter>" +
                                   @"<condition attribute='systemuserid' operator='eq' value='" + msg.AddDependentMaintenanceRequestState.SystemUserId + "' />" +
                                   @"</filter>" +
                                   @"</entity>" +
                                   @"</fetch>";

                //Logger.Instance.Debug("Fetching User Role");

                var users = OrgService.RetrieveMultiple(new FetchExpression(uploadRoleFetch));
                if (users.Entities.Count > 0)
                {
                    userUploadRole = users.Entities[0].GetAttributeValue<OptionSetValue>("udo_vbmsuploadrole");
                }
                if (userUploadRole == null)
                {
                    //Logger.Instance.Debug("User Role Not Found");
                    LogHelper.LogDebug(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.Debug
						, msg.AddDependentMaintenanceRequestState.SystemUserId, "UploadPdfToVBMS", "User Upload Role not set. Defaulting to PCR - NCC");
                    userUploadRole = new OptionSetValue(752280000);
                }

                #endregion

                #region Construct/Create udo_vbmsdocument record

                Entity vbmsDoc = new Entity("udo_vbmsdocument");
                vbmsDoc.Attributes["udo_vbmsuploaddate"] = DateTime.Now;
                vbmsDoc.Attributes["udo_vbmsfilename"] = "Dependent Maintenance";
                vbmsDoc.Attributes["udo_vbmsdocumenttype"] = new EntityReference(docType.LogicalName, docType.Id);
                vbmsDoc.Attributes["udo_vbmsuploadrole"] = userUploadRole;
                vbmsDoc.Attributes["udo_firstname"] = msg.VeteranRequestState.VeteranParticipant.FirstName;
                vbmsDoc.Attributes["udo_lastname"] = msg.VeteranRequestState.VeteranParticipant.LastName;
                vbmsDoc.Attributes["udo_middlename"] = msg.VeteranRequestState.VeteranParticipant.MiddleName;
                vbmsDoc.Attributes["udo_filenumber"] = msg.VeteranRequestState.VeteranParticipant.FileNumber;

                //Logger.Instance.Debug("Creating VBMSDoc Record");

                var createdDoc = OrgService.Create(vbmsDoc);
                #endregion

                #region Create Annotation

                var annotation = new Annotation
                {
                    Subject = "686c Dependent Maintenance File Attachment",
                    ObjectTypeCode = vbmsDoc.LogicalName,
                    MimeType = _MimeType,
                    NoteText = _CPdfDocument,
                    FileName = string.Concat(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss"), ".pdf"),
                    DocumentBody = Convert.ToBase64String(msg.PdfFileBytes),
                    ObjectId = new EntityReference
                    {
                        Id = createdDoc,
                        LogicalName = vbmsDoc.LogicalName
                    }
                };
                var newAnnotation = OrgService.Create(annotation);

                #endregion

                //#region fetch VIMTRestEndpoint
                //string VIMTRestEndpoint = "";
                //var settingsFetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                //                   @"<entity name='mcs_setting'>" +
                //                   @"<attribute name='crme_restendpointforvimt' />" +
                //                   @"<filter />" +
                //                   @"</entity>" +
                //                   @"</fetch>";

                //var settings = OrgService.RetrieveMultiple(new FetchExpression(settingsFetch));
                //if (settings.Entities.Count > 0)
                //    VIMTRestEndpoint = settings.Entities[0].GetAttributeValue<string>("crme_restendpointforvimt");
                //else
                //{
                //    Logger.Instance.Error("Unable to retrieve VIMT Rest Endpoint from UDO Settings. VBMS Document will not be uploaded.");
                //    return;
                //}

                //#endregion

                //#region build LOB request and call UDOVBMSUploadDocumentRequest
                //string userRoleString = null;
                //try
                //{
                //    //Logger.Instance.Debug("Metadata Call for Role String");
                //    userRoleString = getOptionSetString(OrgService, userUploadRole, "udo_vbmsdocument", "udo_vbmsuploadrole");
                //}
                //catch (Exception ex)
                //{
                //    //Logger.Instance.Debug("Metadata Call Failed");
                //    LogHelper.LogInfo(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.debug, msg.AddDependentMaintenanceRequestState.SystemUserId, "LoadPdfToVBMS, getOptionSetString", "Failed to retrieve user role string in metadata call");
                //    if (ex.StackTrace != null)
                //        LogHelper.LogError(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.SystemUserId, "LoadPdfToVBMS Execute", ex.StackTrace);
                //    userRoleString = "PCR - NCC";
                //}
                //Logger.Instance.Debug("Building LOB Request");
                //var request = new UDOVBMSUploadDocumentRequest()
                //{
                //    udo_claimnumber = string.Empty,
                //    OrganizationId = new Guid(),
                //    RelatedParentEntityName = msg.AddDependentMaintenanceRequestState.DependentMaintenance.LogicalName,
                //    RelatedParentFieldName = string.Empty,
                //    RelatedParentId = msg.AddDependentMaintenanceRequestState.DependentMaintenance.Id,
                //    udo_relatedentity = new EntityReference(msg.AddDependentMaintenanceRequestState.DependentMaintenance.LogicalName, Guid.NewGuid()),
                //    OrganizationName = msg.AddDependentMaintenanceRequestState.OrganizationName,
                //    UserId = msg.AddDependentMaintenanceRequestState.SystemUserId,
                //    LogTiming = true,
                //    LogSoap = true,
                //    Debug = true,
                //    LegacyServiceHeaderInfo = new HeaderInfo()
                //    {
                //        StationNumber = msg.AddDependentMaintenanceRequestState.BgsHeaderInfo.StationNumber,
                //        LoginName = msg.AddDependentMaintenanceRequestState.BgsHeaderInfo.LoginName,
                //        ApplicationName = msg.AddDependentMaintenanceRequestState.BgsHeaderInfo.ApplicationName,
                //        ClientMachine = msg.AddDependentMaintenanceRequestState.BgsHeaderInfo.ClientMachine
                //    },
                //    MessageId = Guid.NewGuid().ToString(),
                //    udo_filename = msg.PdfFileName,
                //    udo_base64filecontents = Convert.ToBase64String(msg.PdfFileBytes),
                //    udo_filenumber = msg.VeteranRequestState.VeteranParticipant.FileNumber,
                //    udo_vet_firstname = msg.VeteranRequestState.VeteranParticipant.FirstName,
                //    udo_vet_middlename = msg.VeteranRequestState.VeteranParticipant.MiddleName,
                //    udo_vet_lastname = msg.VeteranRequestState.VeteranParticipant.LastName,
                //    udo_vbmsdocument = new EntityReference("udo_vbmsdocument", createdDoc),
                //    udo_doctypeid = docType.Id,
                //    udo_source = "CRM",
                //    udo_subject = string.Empty,
                //    udo_userRole = userRoleString,
                //    udo_userName = msg.AddDependentMaintenanceRequestState.SystemUser.FullName
                //};

                //Logger.Instance.Debug("Sending LOB Request");

                //var response = request.SendReceive<UDOVBMSUploadDocumentResponse>(MessageProcessType.Remote);

                //_onPremDACUri = new Uri(VIMTRestEndpoint);
                //var _OnPremDACwsd = new VRMRest.WebApi.WebServiceDetails()
                //{
                //    TargetURL = VIMTRestEndpoint,
                //    WSUserName = "apione",
                //    Password = "v@p@ss0rd12123434"
                //};

                //Logger.Instance.Debug("Dac Connnection initialized");

                //var response = VRMRest.WebApi.WebApiUtility.SendReceive<UDOVBMSUploadDocumentResponse>(_onPremDACUri, "CreateInteractionRequest", request, null, _OnPremDACwsd);

                //Logger.Instance.Debug("Response Received. Ending Logging");
                //LogHelper.EndTiming(methodLoggingId, msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.ConfigFieldName, msg.AddDependentMaintenanceRequestState.SystemUserId, methodStartTime);
                //#endregion
            }
            catch (Exception ex)
            {
                var errMsg = string.Empty;

                //if ((_onPremDACUri != null) && !string.IsNullOrEmpty(_onPremDACUri.AbsolutePath))
                //{
                //    errMsg = "SOAP URL: " + _onPremDACUri.AbsolutePath.ToString() + Environment.NewLine + Environment.NewLine;
                //}

                errMsg += "Error Message: " + ex.Message.ToString();

                if (ex.StackTrace != null)
                {
                    errMsg += Environment.NewLine + Environment.NewLine;
                    errMsg += "Stack Trace: " + ex.StackTrace.ToString();
                }

                //Logger.Instance.Debug("Parent-Level Catch Statement");
                LogHelper.LogError(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.SystemUserId, "LoadPdfToVBMS Execute", errMsg);
                //if(ex.StackTrace != null)
                //LogHelper.LogError(msg.AddDependentMaintenanceRequestState.OrganizationName, msg.AddDependentMaintenanceRequestState.SystemUserId, "LoadPdfToVBMS Execute", ex.StackTrace);
            }
        }

        public string getOptionSetString(IOrganizationService OrgService, OptionSetValue optionSetValue, string entityName, string attributeName)
        {
            string optionSetString = string.Empty;

            RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest();
            attributeRequest.EntityLogicalName = entityName;
            attributeRequest.LogicalName = attributeName;
            // Retrieve only the currently published changes, ignoring the changes that have
            // not been published.
            attributeRequest.RetrieveAsIfPublished = true;

            RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)OrgService.Execute(attributeRequest);

            // Access the retrieved attribute.
            PicklistAttributeMetadata retrievedAttributeMetadata = (PicklistAttributeMetadata)attributeResponse.AttributeMetadata;
            for (int i = 0; i < retrievedAttributeMetadata.OptionSet.Options.Count; i++)
            {
                if (retrievedAttributeMetadata.OptionSet.Options[i].Value == optionSetValue.Value)
                {
                    optionSetString = retrievedAttributeMetadata.OptionSet.Options[i].Label.LocalizedLabels[0].Label;
                    break;
                }

            }
            return optionSetString;
        }
    }
}