using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Security;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using VRM.Integration.UDO.MVI.Messages;
using VRMRest;
using UDO.Model;
using System.Diagnostics;
using System.Text;
using MCSHelperClass;
using Microsoft.Crm.Sdk.Messages;
using MCSPlugins;
using UDO.LOB.Core;

namespace VRM.UDO.MVI.Plugin
{
    public class UDOPersonRetrieveMultiplePostStageRunner : MCSPlugins.PluginRunner
    {

        public UDOPersonRetrieveMultiplePostStageRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        private const int _searchTimeout = 100;
        private const string Query = "Query";
        string _fullName = string.Empty;
        bool _addperson = false;
        bool _MVICheck = false;
        bool _bypassMvi = false;
        bool _logSoap = false;
        bool _logTimer = false;
        string _uri = "";
        string _caddurl = "";
        string _letterurl = "";
        string mviSearchType = "";
        Guid _veteranId = new Guid();
        int _user_SL = 0;
        SecureString SSId = null;
        SecureString _vetSSN = null;
        SecureString _SSN = null;
        string _interactionId = "";
        // the output Entity Collection
        internal EntityCollection Output { get; set; }


		//CSDev Fields added from C:\Users\azureadmin\source\repos\UDOD365\UDO.Plugins\CustomActions.Plugins\Helpers\UDOActionRunner.cs
		internal CRMAuthTokenConfiguration _crmAuthTokenConfig;
		internal string _logTimerField = "udo_contactlogtimer";
		internal string _logSoapField = "udo_contactlogsoap";
		internal string _debugField = "udo_contact";
		internal string _vimtRestEndpointField = "crme_restendpointforvimt";
		internal string _vimtTimeoutField = "udo_contacttimeout";
		//internal string _method;
		internal string[] _validEntities = new string[] { "udo_contact" };
		internal Uri _uri2 = null;
		internal bool _debug;
		internal int _timeOutSetting;
		/// <summary>
		/// 
		/// </summary>
		internal void Execute()
        {

			//CSdev Init GetSettingValues();
			GetSettingValues();

			try
			{
                Trace("at the top - CSDev");
                Stopwatch txnTimer = Stopwatch.StartNew();
				Trace("at the top - CSDev - after stop watch");
				//Logger.WriteDebugMessage("UDOPersonRetrieveMultiplePostStageRunner.Execute");

				//CSDev REm 
				var qe = new QueryExpression();


				if (!PluginExecutionContext.InputParameters.Contains(Query) || !(PluginExecutionContext.InputParameters[Query] is QueryExpression))
				{
					#region CSDEv Cast FE AS QE
					//CSDev IS THIS A TEMPLATE FOR A UNITLIY CLASS TODO
					// Convert the FetchXML into a query expression.
					//TracingService.Trace("CSDEv Begin Cast");
					var conversionRequest = new FetchXmlToQueryExpressionRequest();
                    FetchExpression fe = (FetchExpression)PluginExecutionContext.InputParameters[Query];
                    if (fe.Query != null)
                    {
                        conversionRequest.FetchXml = fe.Query.ToString();
                    }
                    //TracingService.Trace("CSDEv Begin Cast Send");
                    //TracingService.Trace("CSDEv Begin Cast Send: " + conversionRequest.FetchXml);
                    var conversionResponse = (FetchXmlToQueryExpressionResponse)OrganizationService.Execute(conversionRequest);
					//TracingService.Trace("CSDEv Begin Cast Send End");
					// Use the newly converted query expression to make a retrieve multiple
					// request to Microsoft Dynamics CRM.
					qe = conversionResponse.Query;

					
					#endregion


					//#region CSDEv Working Debug
					//TracingService.Trace("Query Check");
					//TracingService.Trace("Query Input Contains: ! " + PluginExecutionContext.InputParameters.Contains(Query).ToString());
					//TracingService.Trace("Query Check: ! " + (PluginExecutionContext.InputParameters[Query] is QueryExpression).ToString());

					//foreach (var item in PluginExecutionContext.InputParameters)
					//{
					//	TracingService.Trace(". Parameter Key: " + item.Key.ToString());
					//	TracingService.Trace(". Parameter Valuey:" + item.Value.ToString());
					//	if (item.Key == Query)
					//		TracingService.Trace((PluginExecutionContext.InputParameters[Query] as FetchExpression).Query.ToString());
					//}
					//#endregion
					//CSDev Used to be a Return, but code path is now valid. 
					//return;

				}
					

				Trace("at the top; Past PluginExecutionContext Query Expression Check ");

				try
                {
					//Logger.WriteDebugMessage("UDOPersonRM: Before getting settings");
					Trace("at the top; Pre getSettingValues");
					getSettingValues();

					//CSdev  Removed b/c variable assigned above in error code path
					//var qe = (QueryExpression)PluginExecutionContext.InputParameters[Query];
					Trace("Past the initial bad QE Assignment");

					mviSearchType = FindAttributeValue(qe, "crme_SearchType");
                    //Logger.WriteDebugMessage("UDOPersonRM: mviSearchType:" + mviSearchType);
                    if (String.IsNullOrEmpty(mviSearchType))
                    {
                        //Logger.WriteDebugMessage("UDOPersonRM: SearchType is null or empty");
                        txnTimer.Stop();
                        //Logger.WriteTxnTimingMessage("PersonRM", txnTimer.ElapsedMilliseconds);
                        return;
                    }

                    Output = ((EntityCollection)PluginExecutionContext.OutputParameters["BusinessEntityCollection"]);
                    Output.Entities.Clear();

					Trace("Output.Entities.Clear();");

                    //  Logger.WriteGranularTimingMessage("Starting Retrieval of Person(s)");
                    //var RESTURL = McsSettings.GetSingleSetting("crme_restendpointforvimt", "string");

					//CSDev Set from Global
					//Uri uri = new Uri(_uri);
					//Uri uri = _uri2;
					//Uri uri = new Uri(@"http://mvilob-dev-udo.devtest.vaec.va.gov");
					//Uri uri = new Uri(@"https://dev.integration.d365.va.gov");
                    Uri uri = new Uri(McsSettings.GetSingleSetting("crme_restendpointforvimt", "string"));
                    


                    LogSettings _logSettings = new LogSettings()
					{
						Org = PluginExecutionContext.OrganizationName,
						ConfigFieldName = "RESTCALL",
						UserId = PluginExecutionContext.InitiatingUserId,
						callingMethod = "UDOPersonRM"
					};

					HeaderInfo HeaderInfo = GetHeaderInfo();
                    
					//we do the same thing regardlewss of what kind of search, let the VIMT message handle what that means.

					Trace("Pre Swtich RC Testing: " + mviSearchType);
					//Logger.WriteDebugMessage("UDOPersonRetrieveMultiplePostStageRunner.Execute.PreSwtich");
					switch (mviSearchType)
                    {
                        #region VBMSEFOLDERDOC
                        case "VBMSEFOLDERDOC":
                            //Logger.WriteDebugMessage("Doing VBMS eFolder Doc");


                            var vbmseFolderdocRequest = new UDOgetVBMSDocumentContentRequest();

                            Guid VBMSeFolderId;
                            var VBMSeFolderIdstr = FindAttributeValue(qe, "udo_vbmsefolderid");
                            //long docRefId;
                            var docRefIdstr = FindAttributeValue(qe, "udo_documentversionrefid");

                            if (Guid.TryParse(VBMSeFolderIdstr, out VBMSeFolderId))
                            {
                                //if (Int64.TryParse(docRefIdstr, out docRefId))
                                //{
                                vbmseFolderdocRequest.MessageId = PluginExecutionContext.CorrelationId.ToString(); // Guid.NewGuid().ToString();
                                vbmseFolderdocRequest.OrganizationName = PluginExecutionContext.OrganizationName;
                                vbmseFolderdocRequest.UserId = PluginExecutionContext.InitiatingUserId;
                                vbmseFolderdocRequest.Debug = McsSettings.getDebug;
                                vbmseFolderdocRequest.LogSoap = _logSoap;
                                vbmseFolderdocRequest.LogTiming = _logTimer;
                                vbmseFolderdocRequest.udo_DocumentVersionRefId = docRefIdstr;
                                vbmseFolderdocRequest.udo_VBMSeFolderId = VBMSeFolderId;
                                vbmseFolderdocRequest.LegacyServiceHeaderInfo = HeaderInfo;

                                var vbmsefolderdocResponse = Utility.SendReceive<UDOgetVBMSDocumentContentResponse>(uri, "UDOgetVBMSDocumentContentRequest", vbmseFolderdocRequest, _logSettings);
                                Map((UDOgetVBMSDocumentContentResponse)vbmsefolderdocResponse);
                                //}
                                //else
                                //{
                                //    Logger.WriteDebugMessage("No Document ID found - " + docIdstr);
                                //}
                            }

                            break;
						#endregion
						#region CTI
						case "CTI":
							//Logger.WriteDebugMessage("Doing " + mviSearchType);
							var CTIPersonSearchRequest = new UDOCTIPersonSearchRequest();
							CTIPersonSearchRequest.MessageId = PluginExecutionContext.CorrelationId.ToString(); // Guid.NewGuid().ToString();
                            CTIPersonSearchRequest.OrganizationName = PluginExecutionContext.OrganizationName;
							CTIPersonSearchRequest.UserId = PluginExecutionContext.InitiatingUserId;
							CTIPersonSearchRequest.Debug = McsSettings.getDebug;
							CTIPersonSearchRequest.LogSoap = _logSoap;
							CTIPersonSearchRequest.LogTiming = _logTimer;
							CTIPersonSearchRequest.LegacyServiceHeaderInfo = HeaderInfo;
							CTIPersonSearchRequest.noAddPerson = _addperson;
							CTIPersonSearchRequest.MVICheck = _MVICheck;
							CTIPersonSearchRequest.BypassMvi = _bypassMvi;
							CTIPersonSearchRequest.userSL = _user_SL;
							SetQueryString((UDOCTIPersonSearchRequest)CTIPersonSearchRequest, qe);
							//Logger.WriteDebugMessage("CTIPersonSearchRequest.OrganizationName  " + CTIPersonSearchRequest.OrganizationName);
							//Logger.WriteDebugMessage("CTIPersonSearchRequest.dob  " + CTIPersonSearchRequest.dob);
							//Logger.WriteDebugMessage("CTIPersonSearchRequest.SSIdString  " + CTIPersonSearchRequest.SSIdString);
							//Logger.WriteDebugMessage("CTIPersonSearchRequest.SSIdString  " + CTIPersonSearchRequest.SSIdString);
							//Logger.WriteDebugMessage("CTIPersonSearchRequest.edipi  " + CTIPersonSearchRequest.Edipi);
							if (string.IsNullOrEmpty(CTIPersonSearchRequest.interactionId))
							{
								Entity newInt = new Entity("udo_interaction");
								newInt["udo_title"] = _fullName + " " + System.DateTime.Now;
								newInt["udo_pcrsensitivitylevel"] = _user_SL;


								CTIPersonSearchRequest.interactionId = OrganizationService.Create(newInt).ToString();
								//Logger.WriteDebugMessage("CTIPersonSearchRequest: Created Interaction ");
								_interactionId = CTIPersonSearchRequest.interactionId;
							}
							var CTICombinedSearchResponse = new UDOCombinedPersonSearchResponse();


                           CTICombinedSearchResponse = Utility.SendReceive<UDOCombinedPersonSearchResponse>(uri, "UDOCTIPersonSearchRequest", CTIPersonSearchRequest, _logSettings, _searchTimeout, _crmAuthTokenConfig, TracingService);

                                                        //Logger.WriteDebugMessage("Got back results for  " + mviSearchType);
                            //var response = request.SendReceive<UDOPersonSearchResponse>(MessageProcessType.Remote);
                            Map((UDOCombinedPersonSearchResponse)CTICombinedSearchResponse);
							//Logger.WriteDebugMessage("finished mapping for   " + mviSearchType);
							break;
						#endregion
						#region Chat
						case "CHAT":
							//Logger.WriteDebugMessage("Doing " + mviSearchType);
							var CHATPersonSearchRequest = new UDOCHATPersonSearchRequest();
							CHATPersonSearchRequest.MessageId = PluginExecutionContext.CorrelationId.ToString(); // Guid.NewGuid().ToString();
                            CHATPersonSearchRequest.OrganizationName = PluginExecutionContext.OrganizationName;
							CHATPersonSearchRequest.UserId = PluginExecutionContext.InitiatingUserId;
							CHATPersonSearchRequest.Debug = McsSettings.getDebug;
							CHATPersonSearchRequest.LogSoap = _logSoap;
							CHATPersonSearchRequest.LogTiming = _logTimer;
							CHATPersonSearchRequest.LegacyServiceHeaderInfo = HeaderInfo;
							CHATPersonSearchRequest.noAddPerson = _addperson;
							CHATPersonSearchRequest.MVICheck = _MVICheck;
							CHATPersonSearchRequest.BypassMvi = _bypassMvi;
							CHATPersonSearchRequest.userSL = _user_SL;
							SetQueryString((UDOCHATPersonSearchRequest)CHATPersonSearchRequest, qe);
							if (string.IsNullOrEmpty(CHATPersonSearchRequest.interactionId))
							{
								CHATPersonSearchRequest.interactionId = "6DB421F5-B47E-E511-9411-00155D14E60C";
							}

							//Logger.WriteDebugMessage("as  CombinedSearchRequest.interactionId  " + CombinedSearchRequest.interactionId);
							var CHATCombinedSearchResponse = new UDOCombinedPersonSearchResponse();

							var gotoVIMT = false;

							if (!string.IsNullOrEmpty(CHATPersonSearchRequest.Edipi)) gotoVIMT = true;
							if (!string.IsNullOrEmpty(CHATPersonSearchRequest.SSIdString)) gotoVIMT = true;
							if (!string.IsNullOrEmpty(CHATPersonSearchRequest.ParticipantId)) gotoVIMT = true;


							if (gotoVIMT)
							{

                                CHATCombinedSearchResponse = Utility.SendReceive<UDOCombinedPersonSearchResponse>(uri, "UDOCHATPersonSearchRequest", CHATPersonSearchRequest, _logSettings, _searchTimeout, _crmAuthTokenConfig, TracingService);

                            }
                            else
							{
								CHATCombinedSearchResponse.MVIMessage = "An automated search using the information from the Chat Session resulted in no records being found.";
							}


							//Logger.WriteDebugMessage("Got back results for  " + mviSearchType);
							//var response = request.SendReceive<UDOPersonSearchResponse>(MessageProcessType.Remote);
							Map((UDOCombinedPersonSearchResponse)CHATCombinedSearchResponse);
							//Logger.WriteDebugMessage("finished mapping for   " + mviSearchType);
							break;
						//case "SearchByIdentifier":

						//case "SearchByFilter":
						//    //Logger.WriteDebugMessage("Doing SearchByFilter");
						//    var SearchRequest = new UDOPersonSearchRequest();
						//    SearchRequest.MessageId = Guid.NewGuid().ToString();
						//    SearchRequest.OrganizationName = PluginExecutionContext.OrganizationName;
						//    SearchRequest.UserId = PluginExecutionContext.InitiatingUserId;
						//    SearchRequest.Debug = McsSettings.getDebug;
						//    SearchRequest.LogSoap = _logSoap;
						//    SearchRequest.LogTiming = _logTimer;
						//    SearchRequest.LegacyServiceHeaderInfo = HeaderInfo;

						//    SetQueryString((UDOPersonSearchRequest)SearchRequest, qe);

						//    var SearchResponse = Utility.SendReceive<UDOPersonSearchResponse>(uri, "UDOPersonSearchRequest", SearchRequest, _logSettings, _searchTimeout);

						//    //var response = request.SendReceive<UDOPersonSearchResponse>(MessageProcessType.Remote);
						//    Map((UDOPersonSearchResponse)SearchResponse);
						//    break; 
						#endregion
						case "CombinedSearchByIdentifier":

                        case "CombinedSearchByFilter":
							//Logger.WriteDebugMessage("Doing " + mviSearchType);
							//Logger.WriteDebugMessage("UDOPersonRetrieveMultiplePostStageRunner.Execute.Switch.mviSearchType");
							Trace("Combined Person search Switch Enter RC was Here");

							var CombinedSearchRequest = new UDOCombinedPersonSearchRequest();
                            CombinedSearchRequest.MessageId = PluginExecutionContext.CorrelationId.ToString(); // Guid.NewGuid().ToString();
							//CSDev REm
							CombinedSearchRequest.OrganizationName = PluginExecutionContext.OrganizationName;
							//CombinedSearchRequest.OrganizationName = "UDODEV"; //This is the standard name for all UDO orgs to access MVI
                            //CombinedSearchRequest.OrganizationName = "UDO";
                            CombinedSearchRequest.UserId = PluginExecutionContext.InitiatingUserId;
                            CombinedSearchRequest.Debug = McsSettings.getDebug;
                            CombinedSearchRequest.LogSoap = _logSoap;
                            CombinedSearchRequest.LogTiming = _logTimer;
                            CombinedSearchRequest.LegacyServiceHeaderInfo = HeaderInfo;
                            CombinedSearchRequest.noAddPerson = _addperson;
                            CombinedSearchRequest.MVICheck = _MVICheck;
                            CombinedSearchRequest.BypassMvi = _bypassMvi;
                            CombinedSearchRequest.userSL = _user_SL;
                            //Logger.WriteDebugMessage("_user_SL:  " + _user_SL);
                            SetQueryString((UDOCombinedPersonSearchRequest)CombinedSearchRequest, qe);
							//if (string.IsNullOrEmpty(CombinedSearchRequest.interactionId))
							//{
							//    CombinedSearchRequest.interactionId = "6DB421F5-B47E-E511-9411-00155D14E60C";
							//}

							//Logger.WriteDebugMessage("0 - Pre CombinedSearchRequest.interactionId  " + CombinedSearchRequest.interactionId);
							//Logger.WriteDebugMessage("UDOPersonRetrieveMultiplePostStageRunner.Execute.Switch.mviSearchType CombinedSearchRequest:   " + JsonHelper.Serialize<UDOCombinedPersonSearchRequest>(CombinedSearchRequest));
                            //Logger.WriteDebugMessage("010 - FindAttributeValue(qe, crme_dobstring): " + FindAttributeValue(qe, "crme_dobstring"));
                            //Logger.WriteDebugMessage("UDOPersonRetrieveMultiplePostStageRunner.Execute.Switch.mviSearchType URI: " + uri.ToString());

                            Trace("CombinedSearchByFilter - before UDOCombinedPersonSearch begins");

                            var CombinedSearchResponse = Utility.SendReceive<UDOCombinedPersonSearchResponse>(uri
								, "UDOCombinedPersonSearchRequest", CombinedSearchRequest, _logSettings, _searchTimeout, _crmAuthTokenConfig, TracingService);

                            Trace("OUTSIDE - CombinedSearchByFilter - before UDOCombinedPersonSearch begins - Exception " + CombinedSearchResponse.ExceptionOccured.ToString());
                            
                            //Logger.WriteDebugMessage("9 Post Utility.SendRecseive!   Got back results for  " + mviSearchType);
                            //var response = request.SendReceive<UDOPersonSearchResponse>(MessageProcessType.Remote);

                            if (CombinedSearchResponse.ExceptionOccured)
                            {
                                Trace(CombinedSearchResponse.RawMviExceptionMessage);
                            }
                            //Logger.WriteDebugMessage("100 - Pre Mapping");
                            Trace("100 - Pre Mapping");

                            Map((UDOCombinedPersonSearchResponse)CombinedSearchResponse);
                            //Logger.WriteDebugMessage("101 - Post Mapping - FINAL");
                            Trace("finished mapping for   " + mviSearchType);
							break;

                        case "CombinedSelectedPerson":
							//Logger.WriteDebugMessage("Doing CombinedSelectedPersonSearch");
							//Logger.WriteDebugMessage("UDOPersonRetrieveMultiplePostStageRunner.Execute.Switch = CombinedSelectedPerson");
							var combinedSelectedRequest = new UDOCombinedSelectedPersonRequest();
                            combinedSelectedRequest.MessageId = PluginExecutionContext.CorrelationId.ToString();
                            SetQueryString((UDOCombinedSelectedPersonRequest)combinedSelectedRequest, qe);
                            combinedSelectedRequest.LegacyServiceHeaderInfo = HeaderInfo;
                            combinedSelectedRequest.OrganizationName = PluginExecutionContext.OrganizationName;
                            combinedSelectedRequest.UserId = PluginExecutionContext.InitiatingUserId;
                            combinedSelectedRequest.MessageId = PluginExecutionContext.CorrelationId.ToString(); // Guid.NewGuid().ToString();
                            combinedSelectedRequest.Debug = McsSettings.getDebug;
                            combinedSelectedRequest.LogSoap = _logSoap;
                            combinedSelectedRequest.LogTiming = _logTimer;
                            combinedSelectedRequest.noAddPerson = _addperson;
                            combinedSelectedRequest.MVICheck = _MVICheck;
                            combinedSelectedRequest.BypassMvi = _bypassMvi;
                            combinedSelectedRequest.userSL = _user_SL;
                            if (string.IsNullOrEmpty(combinedSelectedRequest.interactionId))
                            {
                                combinedSelectedRequest.interactionId = "6DB421F5-B47E-E511-9411-00155D14E60C";
                            }

                            var combinedSelectedResponse = Utility.SendReceive<UDOCombinedSelectedPersonResponse>(uri, "UDOCombinedSelectedPersonRequest", combinedSelectedRequest, _logSettings, _searchTimeout, _crmAuthTokenConfig, TracingService);

                            Map((UDOCombinedSelectedPersonResponse)combinedSelectedResponse);
                            break;
                        //case "SelectedPersonSearch":
                        //    //Logger.WriteDebugMessage("Doing SelectedPersonSearch");
                        //    var selectedRequest = new UDOSelectedPersonRequest();
                        //    SetQueryString((UDOSelectedPersonRequest)selectedRequest, qe);
                        //    selectedRequest.LegacyServiceHeaderInfo = HeaderInfo;
                        //    selectedRequest.OrganizationName = PluginExecutionContext.OrganizationName;
                        //    selectedRequest.UserId = PluginExecutionContext.InitiatingUserId;
                        //    selectedRequest.MessageId = Guid.NewGuid().ToString();
                        //    selectedRequest.Debug = McsSettings.getDebug;
                        //    selectedRequest.LogSoap = _logSoap;
                        //    selectedRequest.LogTiming = _logTimer;
                        //    selectedRequest.noAddPerson = _addperson;
                        //    selectedRequest.MVICheck = _MVICheck;
                        //    //var selectedResponse = request.SendReceive<UDOSelectedPersonResponse>(MessageProcessType.Remote);
                        //    var selectedResponse = Utility.SendReceive<UDOSelectedPersonResponse>(uri, "UDOSelectedPersonRequest", selectedRequest, _logSettings, _searchTimeout);

                        //    Map((UDOSelectedPersonResponse)selectedResponse);
                        //    break;
                        case "SearchByBirls":
                            ///Logger.WriteDebugMessage("Doing SearchByBirls");
                            var searchRequest = new UDOBIRLSandOtherSearchRequest();
                            searchRequest.MessageId = PluginExecutionContext.CorrelationId.ToString();
                            SetQueryString((UDOBIRLSandOtherSearchRequest)searchRequest, qe);
                            searchRequest.LegacyServiceHeaderInfo = HeaderInfo;
                            searchRequest.OrganizationName = PluginExecutionContext.OrganizationName;
                            searchRequest.UserId = PluginExecutionContext.InitiatingUserId;
                            searchRequest.MessageId = PluginExecutionContext.CorrelationId.ToString(); // Guid.NewGuid().ToString();
                            searchRequest.Debug = McsSettings.getDebug;
                            searchRequest.LogSoap = _logSoap;
                            searchRequest.LogTiming = _logTimer;
                            searchRequest.noAddPerson = _addperson;
                            searchRequest.userSL = _user_SL;
                            //var selectedResponse = request.SendReceive<UDOSelectedPersonResponse>(MessageProcessType.Remote);
                            var searchResponse = Utility.SendReceive<UDOCombinedPersonSearchResponse>(uri, "UDOBIRLSandOtherSearchRequest", searchRequest, _logSettings, _searchTimeout, _crmAuthTokenConfig, TracingService);

                            Map((UDOCombinedPersonSearchResponse)searchResponse);
                            break;
                        #region
                        case "CADD":
                            //Logger.WriteDebugMessage("Doing CADD");
                            var request = new UDOInitiateCADDRequest();
                            findParentId(request, qe);
                            if (didWeFindData(request))
                            {
                                request.MessageId = PluginExecutionContext.CorrelationId.ToString(); // Guid.NewGuid().ToString();
                                request.OrganizationName = PluginExecutionContext.OrganizationName;
                                request.UserId = PluginExecutionContext.InitiatingUserId;
                                request.Debug = McsSettings.getDebug;
                                request.LogSoap = _logSoap;
                                request.LogTiming = _logTimer;
                                request.LegacyServiceHeaderInfo = HeaderInfo;



                                if (McsSettings.getDebug)
                                {
                                    var requestData = MCSHelper.ConvertToSecureString_new(String.Format("UDOInitiateCADDRequest:{0}MessageId={1}" +
                                        "{0}OrganizationName={2}{0}UserId={3}{0}Debug={4}" +
                                        "{0}LogSoap={5}{0}LogTiming={6}{0}PayeeCode={7}" +
                                        "{0}ptcpntId={8}{0}RoutingNumber={9}",
                                        "\r\n  -  ", request.MessageId, request.OrganizationName
                                        , request.UserId, request.Debug, request.LogSoap, request.LogTiming
                                        , request.PayeeCode, request.ptcpntId, request.RoutingNumber));

                                    requestData = MCSHelper.AppendToSecureString(requestData, String.Format("{0}SSN={1}{0}udo_IDProofId={2}" +
                                        "{0}udo_personId={3}{0}udo_veteranId={4}{0}va_bankaccountId={5}" +
                                        "{0}vetfileNumber={6}{0}vetptcpntId{7}" +
                                        "{0}appealFirstName={8}{0}appealLastName={9}"
                                        , "\r\n  -  "
                                        , MCSHelper.ConvertToUnsecureString(SSId), request.udo_IDProofId, request.udo_personId,
                                        request.udo_veteranId, request.va_bankaccountId, request.vetfileNumber,
                                        request.vetptcpntId, request.appealFirstName, request.appealLastName));

                                    requestData = MCSHelper.AppendToSecureString(requestData, String.Format("{0}awardtypecode={1}",
                                        "\r\n  -  ", request.awardtypecode));

                                    //Logger.WriteDebugMessage(UDOHelper.ConvertToUnsecureString(requestData));
                                }
                                request.SSN = MCSHelper.ConvertToUnsecureString(SSId);
                                var response = Utility.SendReceive<UDOInitiateCADDResponse>(uri, "UDOInitiateCADDRequest", request, _logSettings);
                                Map((UDOInitiateCADDResponse)response);
                                //clear PII for Fortify
                                request.SSN = null;
                            }
                            else
                            {
                                //Logger.WriteDebugMessage("didnt get something, no call to LOB");
                            }
                            break;
                        #endregion
                        #region ITF
                        case "ITF":
                            //Logger.WriteDebugMessage("Doing ITF");
                            var ITFrequest = new UDOInitiateITFRequest();
                            findParentId(ITFrequest, qe);
                            if (didWeFindData(ITFrequest))
                            {
                                ITFrequest.MessageId = PluginExecutionContext.CorrelationId.ToString(); // Guid.NewGuid().ToString();
                                ITFrequest.OrganizationName = PluginExecutionContext.OrganizationName;
                                ITFrequest.UserId = PluginExecutionContext.InitiatingUserId;
                                ITFrequest.Debug = McsSettings.getDebug;
                                ITFrequest.LogSoap = _logSoap;
                                ITFrequest.LogTiming = _logTimer;
                                ITFrequest.LegacyServiceHeaderInfo = HeaderInfo;
                                ITFrequest.SSN = MCSHelper.ConvertToUnsecureString(_SSN);
                                ITFrequest.vetSSN = MCSHelper.ConvertToUnsecureString(_vetSSN);

                                var response = Utility.SendReceive<UDOInitiateITFResponse>(uri, "UDOInitiateITFRequest", ITFrequest, _logSettings);
                                Map((UDOInitiateITFResponse)response);
                                //clear PII for Fortify
                                ITFrequest.SSN = null;
                                ITFrequest.vetSSN = null;
                            }
                            else
                            {
                                //Logger.WriteDebugMessage("didnt get something, no call to LOB");
                            }
                            break;
                        #endregion
                        #region LETTERS
                        case "LETTERS":
                            //Logger.WriteDebugMessage("Doing Letters");
                            var LettersRrequest = new UDOInitiateLettersRequest();
                            findParentId(LettersRrequest, qe);
                            var letteridproofid = FindAttributeValue(qe, "crme_udoidproofid");
                            if (didWeFindData(LettersRrequest, letteridproofid))
                            {
                                LettersRrequest.MessageId = PluginExecutionContext.CorrelationId.ToString(); // Guid.NewGuid().ToString();
                                LettersRrequest.OrganizationName = PluginExecutionContext.OrganizationName;
                                LettersRrequest.UserId = PluginExecutionContext.InitiatingUserId;
                                LettersRrequest.Debug = McsSettings.getDebug;
                                LettersRrequest.LogSoap = _logSoap;
                                LettersRrequest.LogTiming = _logTimer;
                                LettersRrequest.LegacyServiceHeaderInfo = HeaderInfo;
                                LettersRrequest.SSN = MCSHelper.ConvertToUnsecureString(_SSN);
                                LettersRrequest.vetSSN = MCSHelper.ConvertToUnsecureString(_vetSSN);

                                var response = Utility.SendReceive<UDOInitiateLettersResponse>(uri, "UDOInitiateLettersRequest", LettersRrequest, _logSettings);
                                Map((UDOInitiateLettersResponse)response);
                                //clear PII for Fortify
                                LettersRrequest.SSN = null;
                                LettersRrequest.vetSSN = null;
                            }
                            else
                            {
                                //Logger.WriteDebugMessage("didnt get something, no call to LOB");
                            }
                            break;
                        #endregion
                        #region SERVICEREQUEST
                        case "SERVICEREQUEST":
                            //Logger.WriteDebugMessage("inServiceRequest");
                            var sr_VeteranId = FindAttributeValue(qe, "crme_udoveteranid");
                            var sr_PersonId = FindAttributeValue(qe, "crme_udopersonguid");
                            var sr_IdProofId = FindAttributeValue(qe, "crme_udoidproofid");

                            string ownerType = string.Empty;
                            Guid ownerId = Guid.Empty;

                            if (String.IsNullOrEmpty(sr_PersonId))
                            {
                                // Allow for alternate to personid (idproofid, veteranid, and pid)
                                var sr_ParticipantId = FindAttributeValue(qe, "crme_participantid");
                                if (!String.IsNullOrEmpty(sr_ParticipantId))
                                {
                                    var personFetch = "<fetch count='1'><entity name='udo_person'><attribute name='udo_personid'/>" +
                                                      "<attribute name='ownerid'/>" +
                                                      "<filter>" +
                                                      "<condition attribute='udo_ptcpntid' operator='eq' value='" + sr_ParticipantId + "'/>" +
                                                      "<condition attribute='udo_veteranid' operator='eq' value='" + sr_VeteranId + "'/>" +
                                                      "<condition attribute='udo_idproofid' operator='eq' value='" + sr_IdProofId + "'/>" +
                                                      "</filter></entity></fetch>";

                                    var sr_people = OrganizationService.RetrieveMultiple(new FetchExpression(personFetch));
                                    if (sr_people != null && sr_people.Entities.Count > 0)
                                    {
                                        sr_PersonId = sr_people.Entities[0].Id.ToString();
                                        if (sr_people[0].Contains("ownerid"))
                                        {
                                            var owner = (EntityReference)sr_people[0]["ownerid"];
                                            ownerId = owner.Id;
                                            ownerType = owner.LogicalName;
                                        }
                                    }
                                }
                            }

                            var sr_InteractionId = FindAttributeValue(qe, "crme_udointeractionid");

                            if (ownerId == Guid.Empty && !string.IsNullOrEmpty(sr_VeteranId))
                            {
                                var vet = OrganizationService.Retrieve("contact", new Guid(sr_VeteranId), new ColumnSet("ownerid"));
                                var owner = vet.GetAttributeValue<EntityReference>("ownerid");
                                if (owner != null)
                                {
                                    ownerId = owner.Id;
                                    ownerType = owner.LogicalName;
                                }
                            }

                            var sr_Request = new UDOInitiateSRRequest()
                            {
                                MessageId = PluginExecutionContext.CorrelationId.ToString(),
                                OrganizationName = PluginExecutionContext.OrganizationName,
                                //OrganizationName = "UDODEV", //TEMP FIX FOR DEV
                                //OrganizationName = "UDO",
                                UserId = PluginExecutionContext.InitiatingUserId,
                                Debug = McsSettings.getDebug,
                                LogSoap = _logSoap,
                                LogTiming = _logTimer,
                                LegacyServiceHeaderInfo = HeaderInfo,
                                udo_IDProofId = sr_IdProofId,
                                udo_InteractionId = sr_InteractionId,
                                udo_PersonId = sr_PersonId,
                                udo_VeteranId = sr_VeteranId,
                                OwnerId = ownerId,
                                OwnerType = ownerType
                            };

                            //var searchResponse = Utility.SendReceive<UDOCombinedPersonSearchResponse>(uri, "UDOBIRLSandOtherSearchRequest", searchRequest, _logSettings, _searchTimeout, _crmAuthTokenConfig, Logger);
                            var sr_Response = Utility.SendReceive<UDOInitiateSRResponse>(uri, "UDOInitiateSRRequest", sr_Request, _logSettings, _searchTimeout, _crmAuthTokenConfig, TracingService);

                            //var ref_SRResponse = new crme_person();
                            var ref_SRResponse = new Entity("crme_person");

                            // Shouldn't we do something here for friendly messages to the user?
                            if (!sr_Response.ExceptionOccured)
                            {
                                ref_SRResponse.Id = Guid.Parse(sr_Response.UDOServiceRequestId);
                                ref_SRResponse["crme_searchtype"] = "udo_servicerequest";
                                ref_SRResponse["crme_personid"] = Guid.Parse(sr_Response.UDOServiceRequestId);
                            }
                            else
                            {
                                ref_SRResponse.Id = Guid.NewGuid();
                                ref_SRResponse["crme_ReturnMessage"] = sr_Response.ExceptionMessage;
                                // if you return an object/entity in the output without an id, crm will return malformed json
                            }

                            Output.Entities.Add(ref_SRResponse);
                            break;
                        #endregion
                        #region FNOD
                        case "FNOD":
                            //Logger.WriteDebugMessage("Doing FNOD");                            
                            var FNODRequest = new UDOInitiateFNODRequest();
                            findParentId(FNODRequest, qe);
                            if (didWeFindData(FNODRequest))
                            {
                                FNODRequest.MessageId = PluginExecutionContext.CorrelationId.ToString(); // Guid.NewGuid().ToString();
                                FNODRequest.OrganizationName = PluginExecutionContext.OrganizationName;
                                FNODRequest.UserId = PluginExecutionContext.InitiatingUserId;
                                FNODRequest.Debug = McsSettings.getDebug;
                                FNODRequest.LogSoap = _logSoap;
                                FNODRequest.LogTiming = _logTimer;
                                FNODRequest.LegacyServiceHeaderInfo = HeaderInfo;
                                FNODRequest.SSN = MCSHelper.ConvertToUnsecureString(_SSN);
                                FNODRequest.vetSSN = MCSHelper.ConvertToUnsecureString(_vetSSN);

                                var response = Utility.SendReceive<UDOInitiateFNODResponse>(uri, "UDOInitiateFNODRequest", FNODRequest, _logSettings);
                                Map((UDOInitiateFNODResponse)response);
                                //clear PII for Fortify
                                FNODRequest.SSN = null;
                                FNODRequest.vetSSN = null;
                            }
                            else
                            {
                                //Logger.WriteDebugMessage("didnt get something, no call to LOB");
                            }
                            break;
                        #endregion
                        #region NOTES
                        case "NOTES":
                            //Logger.WriteDebugMessage("Loading NOTES");
                            var NotesPerson = FindPersonId(qe);

                            if (!NotesPerson.HasValue) return;
                            var NotesRequest = new UDORetrieveNotesRequest()
                            {
                                MessageId = PluginExecutionContext.CorrelationId.ToString(),
                                OrganizationName = PluginExecutionContext.OrganizationName,
                                UserId = PluginExecutionContext.InitiatingUserId,
                                Debug = McsSettings.getDebug,
                                LogSoap = _logSoap,
                                LogTiming = _logTimer,
                                LegacyServiceHeaderInfo = HeaderInfo,
                                LoadSize = 100
                                //udo_personId = NotesPerson.Value
                            };

                            if (didWeFindData(NotesRequest, NotesPerson.Value))
                            {
                                NotesRequest.RelatedParentEntityName = "udo_person";
                                NotesRequest.RelatedParentFieldName = "udo_personid";
                                NotesRequest.RelatedParentId = NotesPerson.Value;

                                var notesResponse = Utility.SendReceive<UDORetrieveNotesResponse>(uri, "UDORetrieveNotesRequest", NotesRequest, _logSettings, 120);
                                Map(NotesPerson, (UDORetrieveNotesResponse)notesResponse);
                            }
                            else
                            {
                                //Logger.WriteDebugMessage("didnt get something, no call to LOB");
                            }
                            break;
                        #endregion
                        #region CLAIMDOC
                        case "CLAIMDOC":
                            //Logger.WriteDebugMessage("Doing Claim Doc");
                            var claimdocRequest = new getUDOClaimDocumentsRequest();

                            Guid mapdletterId;
                            var mapdletterIdstr = FindAttributeValue(qe, "udo_mapdletterid");
                            long docId;
                            var docIdstr = FindAttributeValue(qe, "udo_documentid");

                            if (Guid.TryParse(mapdletterIdstr, out mapdletterId))
                            {
                                if (Int64.TryParse(docIdstr, out docId))
                                {
                                    claimdocRequest.MessageId = PluginExecutionContext.CorrelationId.ToString(); // Guid.NewGuid().ToString();
                                    claimdocRequest.OrganizationName = PluginExecutionContext.OrganizationName;
                                    claimdocRequest.UserId = PluginExecutionContext.InitiatingUserId;
                                    claimdocRequest.Debug = McsSettings.getDebug;
                                    claimdocRequest.LogSoap = _logSoap;
                                    claimdocRequest.LogTiming = _logTimer;
                                    claimdocRequest.documentId = docId;
                                    claimdocRequest.MAPDLetterId = mapdletterId;
                                    claimdocRequest.LegacyServiceHeaderInfo = HeaderInfo;

                                    var claimdocResponse = Utility.SendReceive<getUDOClaimDocumentsResponse>(uri, "getUDOClaimDocumentsRequest", claimdocRequest, _logSettings);
                                    Map((getUDOClaimDocumentsResponse)claimdocResponse);
                                }
                                else
                                {
                                    //Logger.WriteDebugMessage("No Document ID found - " + docIdstr);
                                }
                            }



                            break;
                            #endregion
                    }
                    //Logger.WriteDebugMessage("110 - Switch end");
                    Trace("Switch End");
				}

                catch (Exception ex)
                {
                    PluginError = true;
                    Trace("9999 - " + ex.Message + "\r\n" + ex.StackTrace);
                    Trace("Exception:" + ex.Message + " -- searchType:" + mviSearchType + "\r\n" + ex.StackTrace);
                    var newPerson = new crme_person();
                    newPerson.Id = Guid.NewGuid();
                    newPerson.crme_ReturnMessage = "An error occurred trying to process this request. Please try again.  If it continues to fail, please contact your administrator";
                    Output.Entities.Add(newPerson.ToEntity<Entity>());
                }
                //End the timing for the plugin
                //Logger.WriteTxnTimingMessage(String.Format("Ending : {0}", GetType()));
                txnTimer.Stop();
				Trace("Try end");
				//Logger.WriteTxnTimingMessage("PersonRM:" + mviSearchType, txnTimer.ElapsedMilliseconds);
            }

            catch (FaultException<OrganizationServiceFault> ex)
            {
                PluginError = true;
                Logger.WriteToFile(ex.Message);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.WriteToFile(ex.Message);
                Logger.WriteToFile(ex.InnerException.Message);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
            finally
            {
                Trace("Entered Finally");
                SetupLogger();
                Trace("Set up logger done.");
                ExecuteFinally(mviSearchType);
                Trace("Exit Finally");
            }
        }
        internal bool didWeFindData(UDOInitiateCADDRequest request)
        {
            try
            {
                Logger.setMethod = "didWeNeedData";
                //Logger.WriteDebugMessage("Starting didWeNeedData Method");

                using (var xrm = new UDOContext(OrganizationService))
                {
                    var getParent = from awd in xrm.udo_personSet
                                    join vet in xrm.ContactSet on awd.udo_veteranId.Id equals vet.ContactId.Value
                                    where awd.udo_personId.Value == request.udo_personId
                                    select new
                                    {
                                        awd.udo_ptcpntid,
                                        theId = MCSHelper.ConvertToSecureString_new(awd.udo_SSN),
                                        awd.udo_veteranId,
                                        snapshotid = awd.udo_vetsnapshotid,
                                        vetfileNumber = vet.udo_FileNumber,
                                        vetPID = vet.udo_ParticipantId,
                                        //vetSSN = vet.udo_SSN,
                                        awd.udo_first,
                                        awd.udo_last,
                                        awd.udo_payeeCode,
                                        awd.udo_awardtypecode,
                                        awd.udo_IDProofId
                                    };


                    foreach (var awd in getParent)
                    {
                        //if (awd.udo_CallComplete != null)
                        //{
                        //    if (awd.udo_CallComplete.Value) return false;
                        //}
                        //Logger.WriteDebugMessage("In For Each");
                        if (awd.udo_payeeCode != null)
                        {
                            request.PayeeCode = awd.udo_payeeCode;
                            //Logger.WriteDebugMessage("payeeCode --> " + request.PayeeCode);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no payee");
                            // Update 5/5/2016 - Payee code should not be required.
                            //return false;
                        }
                        if (awd.udo_awardtypecode != null)
                        {
                            request.awardtypecode = awd.udo_awardtypecode;
                            //Logger.WriteDebugMessage("awardtypecode --> " + request.awardtypecode);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no awardtypecode");

                            //return false;
                        }
                        if (awd.udo_ptcpntid != null)
                        {
                            request.ptcpntId = Convert.ToInt64(awd.udo_ptcpntid);
                            //Logger.WriteDebugMessage("ptcpntid --> " + request.ptcpntId.ToString());
                        }
                        else
                        {
                            return false;
                        }

                        if (!String.IsNullOrEmpty(awd.snapshotid))
                        {
                            Guid snapshotid;
                            if (Guid.TryParse(awd.snapshotid, out snapshotid))
                            {
                                request.udo_snapshotid = snapshotid;
                            }
                        }

                        if (awd.theId != null)
                        {
                            //request.SSN = awd.udo_SSN;
                            //Logger.WriteDebugMessage("ssn --> " + request.SSN);
                            SSId = awd.theId;
                            if (MCSHelper.ConvertToUnsecureString(awd.theId).Length < 9)
                            {
                                _bypassMvi = true;
                                //Logger.WriteDebugMessage("SSN < 9 - Bypass MVI set to true");
                            }
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no ssn");
                            //return false;
                        }
                        if (awd.vetfileNumber != null)
                        {
                            request.vetfileNumber = awd.vetfileNumber;
                            //Logger.WriteDebugMessage("vetfileNumber --> " + request.vetfileNumber);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no vetfileNumber");
                            return false;
                        }
                        if (awd.udo_veteranId != null)
                        {
                            _veteranId = awd.udo_veteranId.Id;
                            request.udo_veteranId = _veteranId;
                            //Logger.WriteDebugMessage("vet --> " + _veteranId.ToString());

                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no vet");
                            return false;
                        }
                        if (awd.vetPID != null)
                        {
                            request.vetptcpntId = Convert.ToInt64(awd.vetPID);
                            //Logger.WriteDebugMessage("vetptcpntId --> " + request.vetptcpntId.ToString());
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no vetptcpntId");
                            return false;
                        }
                        if (awd.udo_IDProofId != null)
                        {
                            request.udo_IDProofId = awd.udo_IDProofId.Id;
                            //Logger.WriteDebugMessage("awardtypecode --> " + request.awardtypecode);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no awardtypecode");

                            //return false;
                        }
                    }
                }
                //   Logger.WriteDebugMessage("Ending didWeNeedData Method");
                Logger.setMethod = "Execute";

                return true;
            }
            catch (Exception ex)
            {
                PluginError = true;
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
        internal bool didWeFindData(UDOInitiateITFRequest request)
        {
            try
            {
                Logger.setMethod = "didWeNeedData";
                //Logger.WriteDebugMessage("Starting didWeNeedData Method");

                using (var xrm = new UDOContext(OrganizationService))
                {
                    var getParent = from awd in xrm.udo_personSet
                                    join vet in xrm.ContactSet on awd.udo_veteranId.Id equals vet.ContactId.Value
                                    where awd.udo_personId.Value == request.udo_personId
                                    select new
                                    {
                                        awd.udo_ptcpntid,
                                        udoSSN = MCSHelper.ConvertToSecureString_new(awd.udo_SSN),
                                        awd.udo_veteranId,
                                        awd.udo_FileNumber,
                                        awd.udo_first,
                                        awd.udo_last,
                                        awd.udo_payeeCode,
                                        awd.udo_awardtypecode,
                                        vetFilenumber = vet.udo_FileNumber,
                                        vetPID = vet.udo_ParticipantId,
                                        vetSSN = MCSHelper.ConvertToSecureString_new(vet.udo_SSN),
                                        vet.FirstName,
                                        vet.LastName,
                                        vet.MiddleName,
                                        vet.udo_BirthDateString,
                                        vet.udo_Gender
                                    };


                    foreach (var awd in getParent)
                    {
                        //if (awd.udo_CallComplete != null)
                        //{
                        //    if (awd.udo_CallComplete.Value) return false;
                        //}
                        //Logger.WriteDebugMessage("In For Each");




                        if (awd.udoSSN != null)
                        {
                            _SSN = awd.udoSSN;

                        }
                        else
                        {

                            //return false;
                        }
                        if (awd.udo_FileNumber != null)
                        {
                            request.fileNumber = awd.udo_FileNumber;
                            //Logger.WriteDebugMessage("filenumber --> " + request.fileNumber);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no filenumber");
                            //return false;
                        }
                        if (awd.udo_ptcpntid != null)
                        {
                            request.ptcpntId = Convert.ToInt64(awd.udo_ptcpntid);
                            //Logger.WriteDebugMessage("udo_ParticipantId --> " + request.ptcpntId.ToString());
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no udo_ParticipantId");
                            return false;
                        }
                        if (awd.vetSSN != null)
                        {
                            _vetSSN = awd.vetSSN;
                            //Logger.WriteDebugMessage("vetSSN --> " + request.vetSSN);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no vetSSN");
                            return false;
                        }
                        if (awd.vetFilenumber != null)
                        {
                            request.vetfileNumber = awd.vetFilenumber;
                            //Logger.WriteDebugMessage("vetfileNumber --> " + request.vetfileNumber);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no vetfileNumber");
                            return false;
                        }
                        if (awd.vetPID != null)
                        {
                            request.vetptcpntId = Convert.ToInt64(awd.vetPID);
                            //Logger.WriteDebugMessage("vetptcpntId --> " + request.vetptcpntId.ToString());
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no udo_ParticipantId");
                            return false;
                        }
                        if (awd.FirstName != null)
                        {
                            request.vetFirstName = awd.FirstName;
                            //Logger.WriteDebugMessage("vetFirstName --> " + request.vetFirstName);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no vetFirstName");
                            //return false;
                        }
                        if (awd.LastName != null)
                        {
                            request.vetLastName = awd.LastName;
                            //Logger.WriteDebugMessage("vetLastName --> " + request.vetLastName);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no vetLastName");
                            //return false;
                        }
                        if (awd.MiddleName != null)
                        {
                            request.vetMiddleInitial = awd.MiddleName;
                            //Logger.WriteDebugMessage("vetMiddleInitial --> " + request.vetMiddleInitial);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no vetMiddleInitial");
                            //return false;
                        }
                        if (awd.udo_BirthDateString != null)
                        {
                            request.vetDOB = awd.udo_BirthDateString;
                            //Logger.WriteDebugMessage("vetDOB --> " + request.vetDOB);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no vetDOB");
                            //return false;
                        }
                        if (awd.udo_Gender != null)
                        {
                            request.vetGender = awd.udo_Gender;
                            //Logger.WriteDebugMessage("vetGender --> " + request.vetGender);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no vetGender");
                            //return false;
                        }
                    }
                }
                //   Logger.WriteDebugMessage("Ending didWeNeedData Method");
                Logger.setMethod = "Execute";

                return true;
            }
            catch (Exception ex)
            {
                PluginError = true;
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
        internal bool didWeFindData(UDORetrieveNotesRequest request, Guid personId)
        {
            try
            {
                Logger.setMethod = "didWeNeedData";
                //Logger.WriteDebugMessage("Starting didWeNeedData Method");
                Guid veteranId = Guid.Empty;
                Guid idproof = Guid.Empty;


                using (var xrm = new UDOContext(OrganizationService))
                {
                    var getParent = from awd in xrm.udo_personSet
                                    join vet in xrm.ContactSet on awd.udo_veteranId.Id equals vet.ContactId.Value
                                    where awd.udo_personId.Value == personId
                                    select new
                                    {
                                        awd.udo_ptcpntid,
                                        //awd.udo_SSN,
                                        awd.udo_IDProofId,
                                        awd.udo_veteranId,
                                        vet.OwnerId
                                        //awd.udo_FileNumber,
                                        //awd.udo_first,
                                        //awd.udo_last,
                                        //awd.udo_payeeCode,
                                        //awd.udo_awardtypecode,
                                        //vetFilenumber = vet.udo_FileNumber,
                                        //vetPID = vet.udo_ParticipantId,
                                        //vetSSN = vet.udo_SSN,
                                        //vet.FirstName,
                                        //vet.LastName,
                                        //vet.MiddleName,
                                        //vet.udo_BirthDateString,
                                        //vet.udo_Gender
                                    };



                    foreach (var awd in getParent)
                    {
                        //Logger.WriteDebugMessage("In For Each");

                        if (awd.udo_veteranId != null)
                        {
                            veteranId = awd.udo_veteranId.Id;
                            //Logger.WriteDebugMessage("veteranId --> " + veteranId);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no veteranId");
                            //return false;
                        }

                        if (awd.udo_IDProofId != null)
                        {
                            idproof = awd.udo_IDProofId.Id;
                            //Logger.WriteDebugMessage("udo_IDProofId --> " + idproof);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no udo_IDProofId");
                            //return false;
                        }

                        if (awd.OwnerId != null)
                        {
                            //Logger.WriteDebugMessage("Owner: {0} ({1})",
                            //   awd.OwnerId.Id, awd.OwnerId.LogicalName);
                            request.OwnerId = awd.OwnerId.Id;
                            request.OwnerType = awd.OwnerId.LogicalName;
                            //request.owner = awd.OwnerId;

                        }
                        else
                        {
                            //should never get called
                            //Logger.WriteDebugMessage("no ownerId");
                        }

                        if (awd.udo_ptcpntid != null)
                        {
                            request.ptcpntId = Convert.ToInt64(awd.udo_ptcpntid);
                            //Logger.WriteDebugMessage("udo_ParticipantId --> " + request.ptcpntId.ToString());
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no udo_ParticipantId");
                            return false;
                        }

                    }
                }

                var veteranReference = new UDORelatedEntity()
                {
                    RelatedEntityFieldName = "udo_veteranid",
                    RelatedEntityId = veteranId,
                    RelatedEntityName = "contact"
                };
                var udo_personReference = new UDORelatedEntity()
                {
                    RelatedEntityFieldName = "udo_personid",
                    RelatedEntityId = personId,
                    RelatedEntityName = "udo_person"
                };
                //var udo_idproofReference = new UDORetrieveNotesRelatedEntitiesMultipleRequest()
                //{
                //    RelatedEntityFieldName = "udo_idproofid",
                //    RelatedEntityId = idproof,
                //    RelatedEntityName = "udo_idproof"
                //};
                var references = new[] { veteranReference, udo_personReference }; //, udo_idproofReference };
                request.RelatedEntities = references;
                //   Logger.WriteDebugMessage("Ending didWeNeedData Method");
                Logger.setMethod = "Execute";

                return true;
            }
            catch (Exception ex)
            {
                PluginError = true;
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
        internal bool didWeFindData(UDOInitiateFNODRequest request)
        {
            try
            {
                Logger.setMethod = "didWeNeedData";
                //Logger.WriteDebugMessage("Starting didWeNeedData Method");

                using (var xrm = new UDOContext(OrganizationService))
                {
                    var getParent = from awd in xrm.udo_personSet
                                    join vet in xrm.ContactSet on awd.udo_veteranId.Id equals vet.ContactId.Value
                                    where awd.udo_personId.Value == request.udo_personId
                                    select new
                                    {
                                        awd.udo_ptcpntid,
                                        udoSSN = MCSHelper.ConvertToSecureString_new(awd.udo_SSN),
                                        awd.udo_IDProofId,
                                        awd.udo_veteranId,
                                        awd.udo_FileNumber,
                                        awd.udo_first,
                                        awd.udo_last,
                                        awd.udo_payeeCode,
                                        awd.udo_awardtypecode,
                                        vetFilenumber = vet.udo_FileNumber,
                                        vetPID = vet.udo_ParticipantId,
                                        vetSSN = MCSHelper.ConvertToSecureString_new(vet.udo_SSN),
                                        vet.FirstName,
                                        vet.LastName,
                                        vet.MiddleName,
                                        vet.udo_BirthDateString,
                                        vet.udo_Gender
                                    };



                    foreach (var awd in getParent)
                    {
                        //Logger.WriteDebugMessage("In For Each");

                        if (awd.udo_veteranId != null)
                        {
                            request.udo_veteranId = awd.udo_veteranId.Id;
                            //Logger.WriteDebugMessage("veteranId --> " + request.udo_veteranId);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no veteranId");
                            //return false;
                        }

                        request.udo_idproofId = Guid.Empty;
                        if (awd.udo_IDProofId != null)
                        {
                            request.udo_idproofId = awd.udo_IDProofId.Id;
                            //Logger.WriteDebugMessage("idproofid --> " + request.udo_idproofId);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no idproofid");
                            //return false;
                        }


                        if (awd.udoSSN != null)
                        {
                            _SSN = awd.udoSSN;
                            //Logger.WriteDebugMessage("ssn --> " + request.SSN);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no ssn");
                            //return false;
                        }
                        if (awd.udo_FileNumber != null)
                        {
                            request.fileNumber = awd.udo_FileNumber;
                            //Logger.WriteDebugMessage("filenumber --> " + request.fileNumber);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no filenumber");
                            //return false;
                        }
                        if (awd.udo_ptcpntid != null)
                        {
                            request.ptcpntId = Convert.ToInt64(awd.udo_ptcpntid);
                            //Logger.WriteDebugMessage("udo_ParticipantId --> " + request.ptcpntId.ToString());
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no udo_ParticipantId");
                            // PID IS NOT REQUIRED FOR FNOD
                            //return false;
                        }
                        if (awd.vetSSN != null)
                        {
                            _vetSSN = awd.vetSSN;
                            //Logger.WriteDebugMessage("vetSSN --> " + request.vetSSN);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no vetSSN");
                            return false;
                        }
                        if (awd.vetFilenumber != null)
                        {
                            request.vetfileNumber = awd.vetFilenumber;
                            //Logger.WriteDebugMessage("vetfileNumber --> " + request.vetfileNumber);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no vetfileNumber");
                            //return false;
                        }
                        if (awd.vetPID != null)
                        {
                            request.vetptcpntId = Convert.ToInt64(awd.vetPID);
                            //Logger.WriteDebugMessage("vetptcpntId --> " + request.vetptcpntId.ToString());
                        }
                        else
                        {
                            // PID NOT REQUIRED FOR FNOD
                            //Logger.WriteDebugMessage("no udo_ParticipantId");
                            //return false;
                        }
                        if (awd.udo_first != null)
                        {
                            request.FirstName = awd.udo_first;
                            //Logger.WriteDebugMessage("firstName --> " + request.FirstName);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no FirstName");
                            //return false;
                        }
                        if (awd.udo_last != null)
                        {
                            request.LastName = awd.udo_last;
                            //Logger.WriteDebugMessage("LastName --> " + request.LastName);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no LastName");
                            //return false;
                        }
                    }
                }
                //   Logger.WriteDebugMessage("Ending didWeNeedData Method");
                Logger.setMethod = "Execute";

                return true;
            }
            catch (Exception ex)
            {
                PluginError = true;
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }
        internal bool didWeFindData(UDOInitiateLettersRequest request, string idproofid)
        {
            try
            {
                Guid idp;
                if (!Guid.TryParse(idproofid, out idp)) idp = Guid.Empty;

                Logger.setMethod = "didWeNeedData";
                //Logger.WriteDebugMessage("Starting didWeNeedData Method");

                using (var xrm = new UDOContext(OrganizationService))
                {
                    var getParent = from awd in xrm.udo_personSet
                                    join vet in xrm.ContactSet on awd.udo_veteranId.Id equals vet.ContactId.Value
                                    where awd.udo_personId.Value == request.udo_personId ||
                                        (awd.udo_ptcpntid == request.ptcpntId.ToString() && awd.udo_IDProofId.Id.Equals(idp))
                                    select new
                                    {
                                        awd.udo_personId,
                                        awd.udo_ptcpntid,
                                        udoSSN = MCSHelper.ConvertToSecureString_new(awd.udo_SSN),
                                        awd.udo_veteranId,
                                        awd.udo_FileNumber,
                                        awd.udo_first,
                                        awd.udo_last,
                                        awd.udo_payeeCode,
                                        awd.udo_awardtypecode,
                                        vetFilenumber = vet.udo_FileNumber,
                                        vetPID = vet.udo_ParticipantId,
                                        vetSSN = MCSHelper.ConvertToSecureString_new(vet.udo_SSN),
                                        vet.FirstName,
                                        vet.LastName,
                                        vet.MiddleName,
                                        vet.udo_BirthDateString,
                                        vet.udo_Gender,
                                        awd.udo_vetsnapshotid
                                    };


                    foreach (var awd in getParent)
                    {

                        //Logger.WriteDebugMessage("In For Each");

                        if (awd.udo_personId != null && awd.udo_personId.HasValue)
                        {
                            request.udo_personId = awd.udo_personId.Value;
                        }

                        if (awd.udo_veteranId != null)
                        {
                            request.udo_veteranId = awd.udo_veteranId.Id;
                            //Logger.WriteDebugMessage("veteranId --> " + request.udo_veteranId.ToString());
                        }

                        if (awd.udoSSN != null)
                        {
                            _SSN = awd.udoSSN;
                            //Logger.WriteDebugMessage("ssn --> " + request.SSN);
                        }

                        if (awd.udo_FileNumber != null)
                        {
                            request.fileNumber = awd.udo_FileNumber;
                            //Logger.WriteDebugMessage("filenumber --> " + request.fileNumber);
                        }

                        if (awd.udo_ptcpntid != null)
                        {
                            request.ptcpntId = Convert.ToInt64(awd.udo_ptcpntid);
                            //Logger.WriteDebugMessage("udo_ParticipantId --> " + request.ptcpntId.ToString());
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no udo_ParticipantId");
                            return false;
                        }

                        if (awd.vetSSN != null)
                        {
                            _vetSSN = awd.vetSSN;
                            //Logger.WriteDebugMessage("vetSSN --> " + request.vetSSN);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no vetSSN");
                            return false;
                        }

                        if (awd.vetFilenumber != null)
                        {
                            request.vetfileNumber = awd.vetFilenumber;
                            //Logger.WriteDebugMessage("vetfileNumber --> " + request.vetfileNumber);
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no vetfileNumber");
                            return false;
                        }

                        if (awd.vetPID != null)
                        {
                            request.vetptcpntId = Convert.ToInt64(awd.vetPID);
                            //Logger.WriteDebugMessage("vetptcpntId --> " + request.vetptcpntId.ToString());
                        }
                        else
                        {
                            //Logger.WriteDebugMessage("no udo_ParticipantId");
                            return false;
                        }

                        if (awd.FirstName != null)
                        {
                            request.vetFirstName = awd.FirstName;
                            //Logger.WriteDebugMessage("vetFirstName --> " + request.vetFirstName);
                        }

                        if (awd.LastName != null)
                        {
                            request.vetLastName = awd.LastName;
                            //Logger.WriteDebugMessage("vetLastName --> " + request.vetLastName);
                        }

                        if (awd.MiddleName != null)
                        {
                            request.vetMiddleInitial = awd.MiddleName;
                            //Logger.WriteDebugMessage("vetMiddleInitial --> " + request.vetMiddleInitial);
                        }

                        if (awd.udo_BirthDateString != null)
                        {
                            request.vetDOB = awd.udo_BirthDateString;
                            //Logger.WriteDebugMessage("vetDOB --> " + request.vetDOB);
                        }

                        if (awd.udo_Gender != null)
                        {
                            request.vetGender = awd.udo_Gender;
                            //Logger.WriteDebugMessage("vetGender --> " + request.vetGender);
                        }

                        if (awd.udo_vetsnapshotid != null)
                        {
                            request.udo_vetsnapshotId = new Guid(awd.udo_vetsnapshotid);
                            //Logger.WriteDebugMessage("vetSnap --> " + awd.udo_vetsnapshotid);
                        }
                    }
                }
                //   Logger.WriteDebugMessage("Ending didWeNeedData Method");
                Logger.setMethod = "Execute";

                return true;
            }
            catch (Exception ex)
            {
                PluginError = true;
                throw new InvalidPluginExecutionException("Unable to didWeNeedData due to: {0}".Replace("{0}", ex.Message));
            }
        }

        internal Guid? FindPersonId(QueryExpression qe)
        {
            var personId = FindAttributeValue(qe, "crme_udopersonguid");
            if (String.IsNullOrEmpty(personId)) return null;
            Guid personGuid;
            if (Guid.TryParse(personId, out personGuid))
            {
                return personGuid;
            }
            return null;
        }

        internal string FindAttributeValue(FilterExpression fe, string attributeName, int depth = 0)
        {
            try
            {
                if (depth > 30) return null;
                var expression = fe.Conditions.FirstOrDefault(a => a.AttributeName.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
                if (expression != null)
                {
                    if (expression.Values.Count() == 1) return expression.Values[0].ToString();
                    return String.Join(",", expression.Values.Select(v => v.ToString()));
                }
                foreach (var filter in fe.Filters)
                {
                    var result = FindAttributeValue(filter, attributeName, depth + 1);
                    if (result != null) return result;
                }
                return null;
            }
            catch (Exception)
            {
                PluginError = true;
                return null;
            }
        }
        internal string FindAttributeValue(QueryExpression qe, string attributeName)
        {
            return FindAttributeValue(qe.Criteria, attributeName);
        }

        internal bool findParentId(UDOInitiateCADDRequest request, QueryExpression qe)
        {
            try
            {
                var personId = FindPersonId(qe);
                if (!personId.HasValue) return false;
                request.udo_personId = personId.Value;
                return true;
            }
            catch (Exception)
            {
                PluginError = true;
                return false;
            }
        }
        internal bool findParentId(UDOInitiateITFRequest request, QueryExpression qe)
        {
            try
            {
                var personId = FindPersonId(qe);
                if (!personId.HasValue) return false;
                request.udo_personId = personId.Value;
                return true;
            }
            catch (Exception)
            {
                PluginError = true;
                return false;
            }
        }
        internal bool findParentId(UDOInitiateFNODRequest request, QueryExpression qe)
        {
            try
            {
                var personId = FindPersonId(qe);
                if (!personId.HasValue) return false;
                request.udo_personId = personId.Value;
                return true;
            }
            catch (Exception)
            {
                PluginError = true;
                return false;
            }
        }
        internal bool findParentId(UDOInitiateLettersRequest request, QueryExpression qe)
        {
            try
            {
                request.ptcpntId = -1;
                var personId = FindPersonId(qe);
                if (!personId.HasValue)
                {
                    var pidstr = FindAttributeValue(qe, "crme_participantid");
                    if (!String.IsNullOrEmpty(pidstr))
                    {
                        long pid = -1;
                        if (long.TryParse(pidstr, out pid))
                        {
                            request.ptcpntId = pid;
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
                request.udo_personId = personId.Value;
                return true;
            }
            catch (Exception)
            {
                PluginError = true;
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        ///         
        private void Map(UDOInitiateITFResponse response)
        {
            try
            {
                //updated - we only care about the URL for CRMe, so that is all we need to map and return
                var newPerson = new crme_person();

                newPerson.Id = Guid.Empty;
                newPerson.crme_SearchType = "ITF";

                newPerson.Id = response.udo_veteranId;
                if (response.ExceptionOccured)
                {
                    newPerson.crme_ReturnMessage = response.ErrorMessage;
                }
                else
                {
                    newPerson.crme_itfparameters = response.parameter;

                }
                Output.Entities.Add(newPerson.ToEntity<Entity>());
            }
            catch
            {
                Logger.WriteToFile("Information: Error while mapping UDOInitiateCADDResponse.");
            }
        }
        private void Map(UDOInitiateCADDResponse response)
        {
            try
            {
                //updated - we only care about the URL for CRMe, so that is all we need to map and return
                var newPerson = new crme_person();

                newPerson.Id = Guid.Empty;
                newPerson.crme_SearchType = "CADD";

                newPerson.Id = response.CADDId;

                if (newPerson.Id == Guid.Empty)
                {
                    newPerson.Id = Guid.NewGuid();
                    //Logger.WriteDebugMessage("No ID retrieved");
                    var caddmessage = "";
                    if (response.ExceptionOccured)
                    {
                        foreach (var ex in response.InnerExceptions)
                        {
                            caddmessage += string.Format("{0}: {1} ", ex.ExceptionCategory, ex.ExceptionMessage);
                        }
                        caddmessage.Trim();
                    }
                    newPerson.crme_ReturnMessage = caddmessage;
                }
                else
                {
                    newPerson.crme_url = string.Format(_caddurl, newPerson.Id);

                }
                Output.Entities.Add(newPerson.ToEntity<Entity>());
            }
            catch
            {
                Logger.WriteToFile("Information: Error while mapping UDOInitiateCADDResponse.");
            }
        }
        private void Map(Guid? person, UDORetrieveNotesResponse response)
        {
            try
            {
                //updated - we only care about the URL for CRMe, so that is all we need to map and return
                var newPerson = new crme_person();

                newPerson.crme_SearchType = "Notes Loaded";

                if (person.HasValue)
                {
                    newPerson.Id = person.Value;
                }
                else
                {
                    newPerson.Id = Guid.NewGuid();
                }
                if (response.ExceptionOccured)
                {
                    newPerson.crme_ReturnMessage = response.ExceptionMessage;
                }

                Output.Entities.Add(newPerson.ToEntity<Entity>());
                Output.MoreRecords = false;
            }
            catch
            {
                Logger.WriteToFile("Information: Error while mapping UDOInitiateFNODResponse.");
            }
        }
        private void Map(UDOInitiateFNODResponse response)
        {
            try
            {
                //updated - we only care about the URL for CRMe, so that is all we need to map and return
                var newPerson = new crme_person();

                newPerson.Id = Guid.Empty;
                newPerson.crme_SearchType = "va_fnod";
                newPerson.Id = response.newUDOInitiateFNODId;
                if (response.UDOInitiateFNODExceptions != null)
                {
                    newPerson.crme_ReturnMessage = response.UDOInitiateFNODExceptions.ExceptionMessage;
                }

                if (newPerson.Id == Guid.Empty)
                {
                    newPerson.Id = Guid.NewGuid();
                    //Logger.WriteDebugMessage("No ID retrieved");
                }
                Output.Entities.Add(newPerson.ToEntity<Entity>());
            }
            catch
            {
                Logger.WriteToFile("Information: Error while mapping UDOInitiateFNODResponse.");
            }
        }/// <summary>
        private void Map(UDOInitiateLettersResponse response)
        {
            try
            {
                //updated - we only care about the URL for CRMe, so that is all we need to map and return
                var newPerson = new crme_person();

                newPerson.Id = Guid.Empty;
                newPerson.crme_SearchType = "udo_letters";

                newPerson.Id = response.newUDOInitiateLetterId;

                if (newPerson.Id == Guid.Empty)
                {
                    newPerson.Id = Guid.NewGuid();
                    //Logger.WriteDebugMessage("No ID retrieved");

                }
                else
                {
                    newPerson.crme_url = string.Format(_letterurl, newPerson.Id);

                }
                Output.Entities.Add(newPerson.ToEntity<Entity>());
            }
            catch
            {
                Logger.WriteToFile("Information: Error while mapping UDOInitiateLetterResponse.");
            }
        }/// <summary>
         /// 
         /// </summary>
         /// <param name="response"></param>
        private void Map(UDOSelectedPersonResponse response)
        {
            try
            {
                //updated - we only care about the URL for CRMe, so that is all we need to map and return
                var newPerson = new crme_person();
                if (response.contactId == null)
                {
                    newPerson.Id = Guid.NewGuid();
                }
                else
                {
                    newPerson.Id = response.contactId;
                }
                // newPerson.crme_FullName = string.Empty;

                if (response.URL == null)
                {
                    //Logger.WriteDebugMessage("No URL retrieved");
                    newPerson.crme_ReturnMessage = response.Message;

                    if (!string.IsNullOrEmpty(response.RawMviExceptionMessage))
                    {
                        //Logger.WriteDebugMessage(response.RawMviExceptionMessage);
                    }

                    Output.Entities.Add(newPerson.ToEntity<Entity>());
                    return;
                }

                // Logger.WriteDebugMessage("URL:" + response.URL);
                // Logger.WriteDebugMessage("contactId:" + response.contactId);
                newPerson.crme_url = response.URL;
                newPerson.crme_ReturnMessage = response.Message;
                //RC - added SL to return back to the HTML screen.
                newPerson.crme_VeteranSensitivityLevel = response.VeteranSensitivityLevel;
                Output.Entities.Add(newPerson.ToEntity<Entity>());
            }
            catch
            {
                Logger.WriteToFile("Information: Error while mapping PatientIdentifier.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        private void Map(UDOCombinedSelectedPersonResponse response)
        {
            if (response.Person == null || (response.Person != null && response.Person.Count() == 0))
            {

                var newPerson = new crme_person();

                newPerson.Id = Guid.NewGuid();
                //newPerson.crme_FullName = string.Empty;
                newPerson.crme_ReturnMessage = response.MVIMessage + response.CORPDbMessage;
                //Logger.WriteDebugMessage("Person has NOTHING:" + newPerson.crme_ReturnMessage);
                if (!string.IsNullOrEmpty(response.RawMviExceptionMessage))
                {
                    //Logger.WriteDebugMessage(response.RawMviExceptionMessage);
                }
                Output.Entities.Add(newPerson.ToEntity<Entity>());
                return;
            }
            else
            {
                //Logger.WriteDebugMessage("Person has something");
            }

            foreach (var person in response.Person)
            {
                var newPerson = new crme_person();
                // Logger.WriteDebugMessage("Starting mapping person");

                #region map the person with data
                if (!string.IsNullOrEmpty(person.SSIdString))
                {
                    var ssnGuid = new Guid(Regex.Replace(person.SSIdString, @"[\-]", "").PadRight(32, 'F'));
                    newPerson.Id = ssnGuid;
                }
                else
                {
                    newPerson.Id = Guid.NewGuid();
                }

                newPerson.Id = response.contactId;

                try
                {
                    if (person.NameList != null)
                    {
                        var legalName = person.NameList.FirstOrDefault(v => v.NameType.Equals("Legal", StringComparison.OrdinalIgnoreCase));
                        var alias = person.NameList.FirstOrDefault(v => v != null && v.NameType.Equals("Alias", StringComparison.OrdinalIgnoreCase));
                        //var legalName = person.NameList[0];
                        //var alias = person.NameList[1];
                        if (legalName != null)
                        {
                            newPerson.crme_FirstName = legalName.GivenName;
                            newPerson.crme_LastName = legalName.FamilyName;
                            newPerson.crme_MiddleName = legalName.MiddleName;
                            newPerson.crme_Suffix = legalName.NameSuffix;
                            newPerson.crme_Prefix = legalName.NamePrefix;
                        }
                        else
                        {
                            //legalName = person.NameList.FirstOrDefault();

                            if (legalName != null)
                            {
                                newPerson.crme_FirstName = legalName.GivenName;
                                newPerson.crme_LastName = legalName.FamilyName;
                                newPerson.crme_MiddleName = legalName.MiddleName;
                                newPerson.crme_Suffix = legalName.NameSuffix;
                                newPerson.crme_Prefix = legalName.NamePrefix;
                            }
                        }

                        newPerson.crme_Alias = TryGetAlias(alias);
                    }

                    newPerson.crme_DOBString = person.BirthDate;
                    newPerson.crme_BranchOfService = person.BranchOfService;
                    newPerson.crme_Rank = person.Rank;
                    newPerson.crme_FullName = person.FullName;
                    newPerson.crme_FullAddress = person.FullAddress;

                    newPerson.crme_PrimaryPhone = person.PhoneNumber;
                    newPerson.crme_RecordSource = person.RecordSource;
                    newPerson.crme_Gender = person.GenderCode;
                    newPerson.crme_DeceasedDate = person.DeceasedDate;
                    newPerson.crme_IdentityTheft = person.IdentifyTheft;
                    newPerson.crme_url = person.Url;
                    newPerson.crme_ReturnMessage = response.MVIMessage + " " + response.CORPDbMessage;
                    newPerson.crme_EDIPI = person.EdiPi;
                    newPerson.crme_FileNumber = person.FileNumber;
                    newPerson.crme_SSN = person.SSIdString;
                    newPerson.crme_ParticipantID = person.ParticipantId;
                    newPerson.crme_VeteranSensitivityLevel = person.VeteranSensitivityLevel;
                    TryGetMviQueryParams(person, newPerson);
                    newPerson.crme_ServiceNumber = person.ServiceNumber;
                    newPerson.crme_InsuranceNumber = person.InsuranceNumber;
                    newPerson.crme_EnteredOnDutyDate = person.EnteredOnDutyDate;
                    newPerson.crme_ReleasedActiveDutyDate = person.ReleasedActiveDutyDate;
                    newPerson.crme_PayeeNumber = person.PayeeNumber;
                    newPerson.crme_FolderLocation = person.FolderLocation;


                    newPerson.crme_udoidproofid = response.idProofId.ToString();


                    Output.Entities.Add(newPerson.ToEntity<Entity>());

                    //Logger.WriteDebugMessage("ending mapping person");
                }
                catch (Exception ex)
                {
                    PluginError = true;
                    //TODO Should this exception be handeled?
                    Logger.WriteToFile("Error: Unable to map: " + ex.Message);
                    Logger.WriteToFile("Error: Unable to map: " + ex.StackTrace);
                    Logger.WriteToFile("Error: Unable to map: " + ex.InnerException);
                    Logger.WriteToFile("Intormation: Error while mapping PatientIdentifier.");
                }
                #endregion

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        private void Map(UDOPersonSearchResponse response)
        {
            if (response.Person == null || (response.Person != null && response.Person.Count() == 0))
            {

                var newPerson = new crme_person();

                newPerson.Id = Guid.NewGuid();
                //newPerson.crme_FullName = string.Empty;
                newPerson.crme_ReturnMessage = response.MVIMessage + response.CORPDbMessage;
                //Logger.WriteDebugMessage("Person has NOTHING:" + newPerson.crme_ReturnMessage);
                if (!string.IsNullOrEmpty(response.RawMviExceptionMessage))
                {
                    //Logger.WriteDebugMessage(response.RawMviExceptionMessage);
                }
                Output.Entities.Add(newPerson.ToEntity<Entity>());
                return;
            }
            else
            {
                //Logger.WriteDebugMessage("Person has something");
            }

            foreach (var person in response.Person)
            {


                var newPerson = new crme_person();
                // Logger.WriteDebugMessage("Starting mapping person");

                #region map the person with data
                if (!string.IsNullOrEmpty(person.SSIdString))
                {
                    var ssnGuid = new Guid(Regex.Replace(person.SSIdString, @"[\-]", "").PadRight(32, 'F'));
                    newPerson.Id = ssnGuid;
                }
                else
                {
                    newPerson.Id = Guid.NewGuid();
                }
                try
                {
                    if (person.NameList != null)
                    {
                        var legalName = person.NameList.FirstOrDefault(v => v.NameType.Equals("Legal", StringComparison.OrdinalIgnoreCase));
                        var alias = person.NameList.FirstOrDefault(v => v.NameType.Equals("Alias", StringComparison.OrdinalIgnoreCase));

                        if (legalName != null)
                        {
                            newPerson.crme_FirstName = legalName.GivenName;
                            newPerson.crme_LastName = legalName.FamilyName;
                            newPerson.crme_MiddleName = legalName.MiddleName;
                        }
                        else
                        {
                            legalName = person.NameList.FirstOrDefault();

                            if (legalName != null)
                            {
                                newPerson.crme_FirstName = legalName.GivenName;
                                newPerson.crme_LastName = legalName.FamilyName;
                                newPerson.crme_MiddleName = legalName.MiddleName;
                            }
                        }

                        newPerson.crme_Alias = TryGetAlias(alias);
                    }

                    newPerson.crme_DOBString = person.BirthDate;
                    newPerson.crme_BranchOfService = person.BranchOfService;
                    newPerson.crme_Rank = person.Rank;
                    newPerson.crme_FullName = person.FullName;
                    newPerson.crme_FullAddress = person.FullAddress;

                    newPerson.crme_PrimaryPhone = person.PhoneNumber;
                    newPerson.crme_RecordSource = person.RecordSource;
                    newPerson.crme_Gender = person.GenderCode;
                    newPerson.crme_DeceasedDate = person.DeceasedDate;
                    newPerson.crme_IdentityTheft = person.IdentifyTheft;
                    newPerson.crme_url = person.Url;
                    newPerson.crme_ReturnMessage = response.MVIMessage + response.CORPDbMessage;
                    newPerson.crme_EDIPI = person.EdiPi;
                    newPerson.crme_SSN = person.SSIdString;
                    newPerson.crme_ParticipantID = person.ParticipantId;
                    newPerson.crme_VeteranSensitivityLevel = person.VeteranSensitivityLevel;
                    TryGetMviQueryParams(person, newPerson);
                    Output.Entities.Add(newPerson);

                    //Logger.WriteDebugMessage("ending mapping person");
                }
                catch
                {
                    //TODO Should this exception be handeled?
                    Logger.WriteToFile("Intormation: Error while mapping PatientIdentifier.");
                }
                #endregion

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        private void Map(UDOCombinedPersonSearchResponse response)
        {
            if (response.Person == null || (response.Person != null && response.Person.Count() == 0))
            {
                Trace("No Person");

                var newPerson = new crme_person();

                newPerson.Id = Guid.NewGuid();

                //newPerson.crme_FullName = string.Empty;
                //Logger.WriteDebugMessage("crme_ReturnMessage:" + response.MVIMessage + response.CORPDbMessage);
               


                newPerson.crme_ReturnMessage = (!string.IsNullOrEmpty(response.MVIMessage) ? response.MVIMessage + " " : "");
                Trace("crme_ReturnMessage" + newPerson.crme_ReturnMessage);
                newPerson.crme_ReturnMessage += (!string.IsNullOrEmpty(response.CORPDbMessage) ? response.CORPDbMessage : "");
                Trace("crme_ReturnMessage" + newPerson.crme_ReturnMessage);
                Output.Entities.Add(newPerson.ToEntity<Entity>());
                //Logger.WriteDebugMessage("100.5 - no person entity placed in output");
                return;
            }
            else
            {
                Trace("Person should exist");
                //Logger.WriteDebugMessage("100.9 - mapping if statement failed");
            }
            var personCount =0;
            foreach (var person in response.Person)
            {
                personCount += 1;

                Trace("Starting mapping Person:" + personCount);

                var newPerson = new crme_person();
                // Logger.WriteDebugMessage("Starting mapping person");

                #region map the person with data
                if (!string.IsNullOrEmpty(person.SSIdString))
                {
                    var ssnGuid = new Guid(Regex.Replace(person.SSIdString, @"[\-]", "").PadRight(32, 'F'));
                    newPerson.Id = ssnGuid;
                }
                else
                {
                    newPerson.Id = Guid.NewGuid();
                }

                newPerson.Id = response.contactId;
                
                try
                {
                    if (person.NameList != null)
                    {
                        var legalName = person.NameList.FirstOrDefault(v => v.NameType.Equals("Legal", StringComparison.OrdinalIgnoreCase));
                        var alias = person.NameList.FirstOrDefault(v => v.NameType.Equals("Alias", StringComparison.OrdinalIgnoreCase));

                        if (legalName != null)
                        {
                            newPerson.crme_FirstName = legalName.GivenName;
                            newPerson.crme_LastName = legalName.FamilyName;
                            newPerson.crme_MiddleName = legalName.MiddleName;
                            newPerson.crme_Suffix = legalName.NameSuffix;
                        }
                        else
                        {
                            legalName = person.NameList.FirstOrDefault();

                            if (legalName != null)
                            {
                                newPerson.crme_FirstName = legalName.GivenName;
                                newPerson.crme_LastName = legalName.FamilyName;
                                newPerson.crme_MiddleName = legalName.MiddleName;
                                newPerson.crme_Suffix = legalName.NameSuffix;
                            }
                        }

                        newPerson.crme_Alias = TryGetAlias(alias);
                    }

                    newPerson.crme_DOBString = person.BirthDate;
                    newPerson.crme_BranchOfService = person.BranchOfService;
                    newPerson.crme_Rank = person.Rank;
                    newPerson.crme_FullName = person.FullName;
                    //Logger.WriteDebugMessage("person.FullName:" + person.FullName);
                    newPerson.crme_FullAddress = person.FullAddress;

                    newPerson.crme_PrimaryPhone = person.PhoneNumber;
                    newPerson.crme_RecordSource = person.RecordSource;
                    newPerson.crme_Gender = person.GenderCode;
                    newPerson.crme_DeceasedDate = person.DeceasedDate;
                    newPerson.crme_IdentityTheft = person.IdentifyTheft;
                    newPerson.crme_url = person.Url;
                    newPerson.crme_ReturnMessage = response.MVIMessage + " " + response.CORPDbMessage;
                    newPerson.crme_EDIPI = person.EdiPi;
                    newPerson.crme_SSN = person.SSIdString;
                    newPerson.crme_FileNumber = person.FileNumber;
                    newPerson.crme_ParticipantID = person.ParticipantId;
                    newPerson.crme_VeteranSensitivityLevel = person.VeteranSensitivityLevel;
                    TryGetMviQueryParams(person, newPerson);
                    newPerson.crme_ServiceNumber = person.ServiceNumber;
                    newPerson.crme_InsuranceNumber = person.InsuranceNumber;
                    newPerson.crme_EnteredOnDutyDate = person.EnteredOnDutyDate;
                    newPerson.crme_ReleasedActiveDutyDate = person.ReleasedActiveDutyDate;
                    newPerson.crme_PayeeNumber = person.PayeeNumber;
                    newPerson.crme_FolderLocation = person.FolderLocation;
                    if (_interactionId != null)
                    {
                        newPerson.crme_udointeractionid = _interactionId;
                    }

                    newPerson.crme_udoidproofid = response.idProofId.ToString();
                    newPerson.crme_personId = response.contactId;

                    //Output.Entities.Add(newPerson);
                    Output.Entities.Add(newPerson.ToEntity<Entity>());

                    //Logger.WriteDebugMessage("ending mapping person");
                }
                catch
                {
                    //TODO Should this exception be handeled?
                    Logger.WriteToFile("Intormation: Error while mapping PatientIdentifier.");
                }
                #endregion

            }
        }
        private void Map(UDOgetVBMSDocumentContentResponse response)
        {
            try
            {
                //updated - we only care about the URL for CRMe, so that is all we need to map and return
                var newPerson = new crme_person();

                newPerson.Id = Guid.Empty;
                newPerson.crme_SearchType = "VBMSEFOLDERDOC";
                newPerson.udo_AttachmentId = response.udo_attachmentId.ToString();
                //Output.Entities.Add(newPerson);
                Output.Entities.Add(newPerson.ToEntity<Entity>());
                //Logger.WriteDebugMessage("passing this attachmentid back to matt:" + newPerson.udo_AttachmentId);
            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.WriteException(ex);
                Logger.WriteToFile("Information: Error while mapping UDOgetVBMSDocumentContentResponse.");
            }
        }
        private void Map(getUDOClaimDocumentsResponse response)
        {
            try
            {
                //updated - we only care about the URL for CRMe, so that is all we need to map and return
                var newPerson = new crme_person();

                newPerson.Id = Guid.Empty;
                newPerson.crme_SearchType = "CLAIMDOC";
                newPerson.udo_AttachmentId = response.getUDOClaimDocumentsResponseInfo[0].udo_attachmentId.ToString();
                //Logger.WriteDebugMessage("passing this attachementid back to matt:" + newPerson.udo_AttachmentId);
                //Output.Entities.Add(newPerson);
                Output.Entities.Add(newPerson.ToEntity<Entity>());
            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.WriteException(ex);
                Logger.WriteToFile("Information: Error while mapping getUDOClaimDocumentsResponse.");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aliasName"></param>
        /// <returns></returns>
        private static string TryGetAlias(Name aliasName)
        {
            try
            {
                if (aliasName == null)
                {
                    return string.Empty;
                }

                const string nameFormat = "{0} {1} {2}";
                var alias = string.Format(nameFormat, aliasName.GivenName, aliasName.MiddleName, aliasName.FamilyName);

                return alias;
            }
            catch (Exception)
            {                
                return string.Empty;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        private static void TryGetMviQueryParams(PatientPerson person, crme_person newPerson)
        {
            try
            {
                if (person.CorrespondingIdList == null || !person.CorrespondingIdList.Any())
                {
                    newPerson.crme_PatientMviIdentifier = string.Empty;
                    newPerson.crme_ICN = string.Empty;

                }
                else
                {

                    var patientNo = person.CorrespondingIdList.FirstOrDefault((v =>
                        v.AssigningAuthority != null &&
                        v.AssigningAuthority.Equals("USVHA", StringComparison.InvariantCultureIgnoreCase) &&
                        v.AssigningFacility != null && v.AssigningFacility == "200M" &&
                        v.IdentifierType != null &&
                        v.IdentifierType.Equals("NI", StringComparison.InvariantCultureIgnoreCase))) ??
                                    person.CorrespondingIdList.FirstOrDefault((v =>
                                        v.AssigningAuthority != null &&
                                        v.AssigningAuthority.Equals("USVHA", StringComparison.InvariantCultureIgnoreCase) &&
                                        v.IdentifierType != null &&
                                        v.IdentifierType.Equals("PI", StringComparison.InvariantCultureIgnoreCase)));

                    newPerson.crme_PatientMviIdentifier = patientNo != null ? patientNo.RawValueFromMvi : string.Empty;
                    newPerson.crme_ICN = patientNo != null ? patientNo.PatientIdentifier : string.Empty;
                }
            }
            catch (Exception)
            {
                
            }
        }
        internal void getSettingValues()
        {
            _logTimer = McsSettings.GetSingleSetting<bool>("udo_searchlogtiming");
            _logSoap = McsSettings.GetSingleSetting<bool>("udo_searchlogsoap");
            _addperson = McsSettings.GetSingleSetting<bool>("udo_addperson");
            _MVICheck = McsSettings.GetSingleSetting<bool>("udo_mvicheck");
            _bypassMvi = McsSettings.GetSingleSetting<bool>("udo_bypassmvi");
            _uri = McsSettings.GetSingleSetting<string>("crme_restendpointforvimt");
            _caddurl = McsSettings.GetSingleSetting<string>("udo_caddurl");
            _letterurl = McsSettings.GetSingleSetting<string>("udo_lettersurl");
        }
        public override string McsSettingsDebugField
        {
            get { return "udo_searchdebug"; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Entity GetPrimaryEntity()
        {
            return (Entity)PluginExecutionContext.InputParameters["Target"];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Entity GetSecondaryEntity()
        {
            return (Entity)PluginExecutionContext.InputParameters["Target"];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private User TryGetUserName(Guid userId)
        {
            using (var crm = new UDOContext(OrganizationService))
            {
                return (from u in crm.SystemUserSet
                        where u.Id == userId
                        select new User
                        {
                            FirstName = u.FirstName,
                            LastName = u.LastName
                        }).FirstOrDefault();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private BusinessUnit TryGetUserSensitivityLevel()
        {
            using (var crm = new UDOContext(OrganizationService))
            {
                return (from bu in crm.BusinessUnitSet
                        where bu.Id == PluginExecutionContext.BusinessUnitId
                        select new BusinessUnit
                        {
                            udo_VeteranSensitivityLevel = bu.udo_VeteranSensitivityLevel
                        }).FirstOrDefault();
            }

        }
        public void SetQueryString(UDOCHATPersonSearchRequest request, QueryExpression qe)
        {
            try
            {
                Logger.setMethod = "SetQueryString";
                //Logger.WriteDebugMessage("Starting SetQueryString Method");

                if (qe.Criteria != null)
                {
                    if (qe.Criteria.Filters.Any())
                    {
                        request.SSIdString = FindAttributeValue(qe, "crme_ssn");
                        request.Edipi = FindAttributeValue(qe, "crme_edipi");
                        if (!string.IsNullOrEmpty(request.Edipi))
                        {
                            if (request.Edipi.ToLower().Contains("not"))
                            {
                                request.Edipi = null;
                            }
                        }

                        request.ParticipantId = FindAttributeValue(qe, "crme_participantid");
                        if (string.IsNullOrEmpty(request.Edipi))
                        {
                            request.IsAttended = true;
                        }
                        request.OrganizationName = PluginExecutionContext.OrganizationName;
                        request.UserId = PluginExecutionContext.UserId;
                        var user = TryGetUserName(PluginExecutionContext.UserId);

                        if (user != null)
                        {
                            request.UserFirstName = user.FirstName;
                            request.UserLastName = user.LastName;
                        }

                        request.interactionId = FindAttributeValue(qe, "crme_udointeractionid");
                    }
                }

                //  Logger.WriteDebugMessage("Ending SetQueryString Method");
                Logger.setMethod = "Execute";

            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.WriteToFile("Unable to set Query String due to: {0}".Replace("{0}",
                    ex.Message));
                throw new InvalidPluginExecutionException("Unable to set Query String due to: {0}".Replace("{0}",
                    ex.Message));
            }
        }
        public void SetQueryString(UDOCTIPersonSearchRequest request, QueryExpression qe)
        {
            try
            {
                Logger.setMethod = "SetQueryString";
                //Logger.WriteDebugMessage("Starting SetQueryString Method");

                if (qe.Criteria != null)
                {
                    if (qe.Criteria.Filters.Any())
                    {
                        request.IsAttended = true;
                        request.SSIdString = FindAttributeValue(qe, "crme_ssn");
                        request.Edipi = FindAttributeValue(qe, "crme_edipi");
                        if (!string.IsNullOrEmpty(request.Edipi))
                        {
                            if (request.Edipi.ToLower().Contains("not"))
                            {
                                //Logger.WriteDebugMessage("Making EDIPI null");
                                request.Edipi = null;
                            }
                            else
                            {
                                if (request.Edipi.Length > 4)
                                {
                                    request.IsAttended = false;

                                    //Logger.WriteDebugMessage("Got real EDIPI:" + request.Edipi);
                                }
                                else
                                {
                                    //Logger.WriteDebugMessage("Making EDIPI null");
                                    request.Edipi = null;
                                }
                            }
                        }
                        request.dob = FindAttributeValue(qe, "crme_dobstring");


                        request.OrganizationName = PluginExecutionContext.OrganizationName;
                        request.UserId = PluginExecutionContext.UserId;
                        var user = TryGetUserName(PluginExecutionContext.UserId);

                        if (user != null)
                        {
                            request.UserFirstName = user.FirstName;
                            request.UserLastName = user.LastName;
                        }

                        request.interactionId = FindAttributeValue(qe, "crme_udointeractionid");
                    }
                }

                //  Logger.WriteDebugMessage("Ending SetQueryString Method");
                Logger.setMethod = "Execute";

            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.WriteToFile("Unable to set Query String due to: {0}".Replace("{0}",
                    ex.Message));
                throw new InvalidPluginExecutionException("Unable to set Query String due to: {0}".Replace("{0}",
                    ex.Message));
            }
        }
        /// <summary>
        /// JE: ALL GOOD
        /// </summary>
        /// <param name="request"></param>
        /// <param name="qe"></param>
        public void SetQueryString(UDOCombinedPersonSearchRequest request, QueryExpression qe)
        {
            try
            {
                Logger.setMethod = "SetQueryString";
                //Logger.WriteDebugMessage("Starting SetQueryString Method");

                if (qe.Criteria != null)
                {
                    if (qe.Criteria.Conditions.Any() || qe.Criteria.Filters.Any())
                    {
                        request.SSIdString = FindAttributeValue(qe, "crme_ssn");
                        request.Edipi = FindAttributeValue(qe, "crme_edipi");
                        request.FirstName = FindAttributeValue(qe, "crme_firstname");
                        request.BranchOfService = FindAttributeValue(qe, "crme_branchofservice");
                        request.MiddleName = FindAttributeValue(qe, "crme_middlename");
                        request.FamilyName = FindAttributeValue(qe, "crme_lastname");
                        request.BirthDate = FindAttributeValue(qe, "crme_dobstring");
                        request.PhoneNumber = FindAttributeValue(qe, "crme_primaryphone");
                        request.IdentifierClassCode = FindAttributeValue(qe, "crme_classcode");
                        var isAttended = FindAttributeValue(qe, "crme_isattended");
                        request.IsAttended = Convert.ToBoolean(string.IsNullOrEmpty(isAttended) ? "false" : isAttended);
						//CSDev
						request.OrganizationName = PluginExecutionContext.OrganizationName;
						//request.OrganizationName = "UDODEV"; //TEMP FIX FOR DEV
                        //request.OrganizationName = "UDO"; //This is the standard name for all UDO orgs to access MVI
                                                             //CSDev why is this in here twice TODO


                        //CSDev Rem
                        //request.FetchMessageProcessType = "remote";


                        request.UserId = PluginExecutionContext.UserId;
                        var user = TryGetUserName(PluginExecutionContext.UserId);

                        if (user != null)
                        {
                            request.UserFirstName = user.FirstName;
                            request.UserLastName = user.LastName;
                        }

                        request.interactionId = FindAttributeValue(qe, "crme_udointeractionid");
                    }
                }

                //  Logger.WriteDebugMessage("Ending SetQueryString Method");
                Logger.setMethod = "Execute";

            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.WriteToFile("Unable to set Query String due to: {0}".Replace("{0}",
                    ex.Message));
                throw new InvalidPluginExecutionException("Unable to set Query String due to: {0}".Replace("{0}",
                    ex.Message));
            }
        }
        /// <summary>
        /// JE: ALL GOOD
        /// </summary>
        /// <param name="request"></param>
        /// <param name="qe"></param>
        public void SetQueryString(UDOPersonSearchRequest request, QueryExpression qe)
        {
            try
            {
                Logger.setMethod = "SetQueryString";
                //Logger.WriteDebugMessage("Starting SetQueryString Method");

                if (qe.Criteria != null)
                {
                    if (qe.Criteria.Filters.Any())
                    {
                        request.SSIdString = FindAttributeValue(qe, "crme_ssn");
                        request.Edipi = FindAttributeValue(qe, "crme_edipi");
                        request.FirstName = FindAttributeValue(qe, "crme_firstname");
                        request.BranchOfService = FindAttributeValue(qe, "crme_branchofservice");
                        request.MiddleName = FindAttributeValue(qe, "crme_middlename");
                        request.FamilyName = FindAttributeValue(qe, "crme_lastname");
                        request.BirthDate = FindAttributeValue(qe, "crme_dobstring");
                        request.PhoneNumber = FindAttributeValue(qe, "crme_primaryphone");
                        request.IdentifierClassCode = FindAttributeValue(qe, "crme_classcode");
                        var isAttended = FindAttributeValue(qe, "crme_isattended");
                        request.IsAttended = Convert.ToBoolean(string.IsNullOrEmpty(isAttended) ? "false" : isAttended);
                        request.OrganizationName = PluginExecutionContext.OrganizationName;
                        request.FetchMessageProcessType = "remote";
                        request.UserId = PluginExecutionContext.UserId;
                        var user = TryGetUserName(PluginExecutionContext.UserId);

                        if (user != null)
                        {
                            request.UserFirstName = user.FirstName;
                            request.UserLastName = user.LastName;
                        }
                    }
                }

                //  Logger.WriteDebugMessage("Ending SetQueryString Method");
                Logger.setMethod = "Execute";

            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.WriteToFile("Unable to set Query String due to: {0}".Replace("{0}",
                    ex.Message));
                throw new InvalidPluginExecutionException("Unable to set Query String due to: {0}".Replace("{0}",
                    ex.Message));
            }
        }
        /// <summary>
        /// NO LONGER USED
        /// </summary>
        /// <param name="request"></param>
        /// <param name="qe"></param>
        public void SetQueryString(UDOSelectedPersonRequest request, QueryExpression qe)
        {
            try
            {
                Logger.setMethod = "SetQueryString";
                //Logger.WriteDebugMessage("Starting SetQueryString Method");

                if (qe.Criteria != null)
                {
                    if (qe.Criteria.Filters.Any())
                    {
                        request.SSIdString = FindAttributeValue(qe, "crme_ssn");
                        request.Edipi = FindAttributeValue(qe, "crme_edipi");
                        request.ICN = FindAttributeValue(qe, "crme_ICN");
                        request.FirstName = FindAttributeValue(qe, "crme_firstname");
                        request.MiddleName = FindAttributeValue(qe, "crme_middlename");
                        request.FamilyName = FindAttributeValue(qe, "crme_lastname");
                        request.RawValueFromMvi = FindAttributeValue(qe, "crme_PatientMviIdentifier");
                        request.RecordSource = FindAttributeValue(qe, "crme_RecordSource");

                        var vetSensLevel = FindAttributeValue(qe, "crme_VeteranSensitivityLevel");
                        if (vetSensLevel != string.Empty)
                        {
                            request.VeteranSensitivityLevel = Convert.ToInt32(vetSensLevel);
                        }

                        //if we got this data from MVI, then send it back in so we can put in in person in order for people to understand
                        //any potential difference in information.

                        if (request.RecordSource == "MVI")
                        {
                            request.DateofBirth = FindAttributeValue(qe, "crme_MVIDOBString");
                            request.FullAddress = FindAttributeValue(qe, "crme_MVIFullAddress");
                            request.FullName = FindAttributeValue(qe, "crme_MVIFullName");
                        }
                        request.OrganizationName = PluginExecutionContext.OrganizationName;
                        request.FetchMessageProcessType = "remote";
                        request.UserId = PluginExecutionContext.UserId;

                        if (!string.IsNullOrEmpty(request.Edipi))
                        {
                            request.IdentifierClassCode = "MIL";
                        }

                        var user = TryGetUserName(PluginExecutionContext.UserId);

                        if (user != null)
                        {
                            request.UserFirstName = user.FirstName;
                            request.UserLastName = user.LastName;
                        }

                    }
                }

                //Logger.WriteDebugMessage("Ending SetQueryString Method");
            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.WriteToFile("Unable to set Query String due to: {0}".Replace("{0}",
                      ex.Message));
                throw new InvalidPluginExecutionException("Unable to set Query String due to: {0}".Replace("{0}",
                    ex.Message));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="qe"></param>
        public void SetQueryString(UDOCombinedSelectedPersonRequest request, QueryExpression qe)
        {
            try
            {
                Logger.setMethod = "SetQueryString";
                //Logger.WriteDebugMessage("Starting SetQueryString Method");

                if (qe.Criteria != null)
                {
                    if (qe.Criteria.Filters.Any())
                    {
                        var criteria = qe.Criteria;
                        request.FileNumber = FindAttributeValue(criteria, "crme_filenumber");
                        request.SSIdString = FindAttributeValue(criteria, "crme_ssn");
                        request.Edipi = FindAttributeValue(criteria, "crme_edipi");
                        request.ICN = FindAttributeValue(criteria, "crme_icn");
                        request.FirstName = FindAttributeValue(criteria, "crme_firstname");
                        request.MiddleName = FindAttributeValue(criteria, "crme_middlename");
                        request.FamilyName = FindAttributeValue(criteria, "crme_lastname");
                        request.PatientSearchIdentifier = FindAttributeValue(criteria, "crme_patientmviidentifier");
                        request.RecordSource = FindAttributeValue(criteria, "crme_recordsource");
                        request.PrefixName = FindAttributeValue(criteria, "crme_prefix");
                        request.SuffixName = FindAttributeValue(criteria, "crme_suffix");
                        request.Gender = FindAttributeValue(criteria, "crme_gender");
                        request.AliasName = FindAttributeValue(criteria, "crme_alias");
                        request.FullAddress = FindAttributeValue(criteria, "crme_fulladdress");
                        request.FullName = FindAttributeValue(criteria, "crme_fullname");
                        request.DateofBirth = FindAttributeValue(criteria, "crme_dobstring");
                        request.DeceasedDate = FindAttributeValue(criteria, "crme_deceaseddate");
                        request.PhoneNumber = FindAttributeValue(criteria, "crme_primaryphone");
                        request.ParticipantId = FindAttributeValue(criteria, "crme_participantid");
                        request.IdentityTheft = FindAttributeValue(criteria, "crme_identitytheft");
                        request.interactionId = FindAttributeValue(criteria, "crme_udointeractionid");
                        var isAttended = FindAttributeValue(criteria, "crme_isattended");
                        request.IsAttended = Convert.ToBoolean(string.IsNullOrEmpty(isAttended) ? "false" : isAttended);
                        //var vetSensLevel = FindAttributeValue(criteria, "crme_veteransensitivitylevel");
                        //request.SocialSecurityNumber = FindAttributeValue(qe.Criteria, "crme_ssn");
                        //request.Edipi = FindAttributeValue(qe.Criteria, "crme_edipi");
                        //request.ICN = FindAttributeValue(qe.Criteria, "crme_icn");
                        //request.FirstName = FindAttributeValue(qe.Criteria, "crme_firstname");
                        //request.MiddleName = FindAttributeValue(qe.Criteria, "crme_middlename");
                        //request.FamilyName = FindAttributeValue(qe.Criteria, "crme_lastname");
                        //request.RawValueFromMvi = FindAttributeValue(qe.Criteria, "crme_patientmviidentifier");
                        //request.RecordSource = FindAttributeValue(qe.Criteria, "crme_recordsource");
                        //request.PrefixName = FindAttributeValue(qe.Criteria, "crme_prefix");
                        //request.SuffixName = FindAttributeValue(qe.Criteria, "crme_suffix");
                        //request.AliasName = FindAttributeValue(qe.Criteria, "crme_alias");
                        //request.FullAddress = FindAttributeValue(qe.Criteria, "crme_fulladdress");
                        //request.FullName = FindAttributeValue(qe.Criteria, "crme_fullname");
                        //request.DateofBirth = FindAttributeValue(qe.Criteria, "crme_dobstring");
                        //request.DeceasedDate = FindAttributeValue(qe.Criteria, "crme_deceaseddate");
                        //request.PhoneNumber = FindAttributeValue(qe.Criteria, "crme_primaryphone");
                        //request.ParticipantId = FindAttributeValue(qe.Criteria, "crme_participantid");
                        //request.IdentityTheft = FindAttributeValue(qe.Criteria, "crme_identitytheft");

                        var vetSensLevel = FindAttributeValue(qe.Criteria, "crme_veteransensitivitylevel");
                        if (vetSensLevel != string.Empty)
                        {
                            request.VeteranSensitivityLevel = Convert.ToInt32(vetSensLevel);
                        }

                        //if we got this data from MVI, then send it back in so we can put in in person in order for people to understand
                        //any potential difference in information.

                        //if (request.RecordSource == "MVI")
                        //{
                        //    request.DateofBirth = FindAttributeValue(qe.Criteria, "crme_MVIDOBString");
                        //    request.FullAddress = FindAttributeValue(qe.Criteria, "crme_MVIFullAddress");
                        //    request.FullName = FindAttributeValue(qe.Criteria, "crme_MVIFullName");
                        //}
                        request.OrganizationName = PluginExecutionContext.OrganizationName;
                        request.FetchMessageProcessType = "remote";
                        request.UserId = PluginExecutionContext.UserId;

                        if (!string.IsNullOrEmpty(request.Edipi))
                        {
                            request.IdentifierClassCode = "MIL";
                        }

                        var user = TryGetUserName(PluginExecutionContext.UserId);

                        if (user != null)
                        {
                            request.UserFirstName = user.FirstName;
                            request.UserLastName = user.LastName;
                        }
                    }
                }

                //Logger.WriteDebugMessage("Ending SetQueryString Method");
            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.WriteToFile("Unable to set Query String due to: {0}".Replace("{0}",
                      ex.Message));
                throw new InvalidPluginExecutionException("Unable to set Query String due to: {0}".Replace("{0}",
                    ex.Message));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="qe"></param>
        public void SetQueryString(UDOBIRLSandOtherSearchRequest request, QueryExpression qe)
        {
            try
            {
                Logger.setMethod = "SetQueryString";
                //Logger.WriteDebugMessage("Starting SetQueryString Method");

                if (qe.Criteria != null)
                {
                    if (qe.Criteria.Filters.Any())
                    {
                        var criteria = qe.Criteria;
                        request.SSIdString = FindAttributeValue(criteria, "crme_ssn");
                        //Logger.WriteDebugMessage("request.SSIdString:" + request.SSIdString);
                        request.FirstName = FindAttributeValue(criteria, "crme_firstname");
                        //Logger.WriteDebugMessage("request.FirstName:" + request.FirstName);
                        request.MiddleName = FindAttributeValue(qe, "crme_middlename");
                        // Logger.WriteDebugMessage("request.MiddleName:" + request.MiddleName);
                        request.FamilyName = FindAttributeValue(qe, "crme_lastname");
                        //Logger.WriteDebugMessage("request.FamilyName:" + request.FamilyName);

                        request.BirthDate = FindAttributeValue(qe, "crme_DOBString");
                        //Logger.WriteDebugMessage("request.BirthDate:" + request.BirthDate);
                        request.BranchofService = FindAttributeValue(qe, "crme_BranchOfService");
                        request.ServiceNumber = FindAttributeValue(qe, "crme_ServiceNumber");
                        request.DeceasedDate = FindAttributeValue(qe, "crme_DeceasedDate");
                        request.EnteredonDutyDate = FindAttributeValue(qe, "crme_EnteredOnDutyDate");
                        request.ReleasedActiveDutyDate = FindAttributeValue(qe, "crme_ReleasedActiveDutyDate");
                        request.Suffix = FindAttributeValue(qe, "crme_Suffix");
                        request.PayeeNumber = FindAttributeValue(qe, "crme_PayeeNumber");
                        request.FolderLocation = FindAttributeValue(qe, "crme_FolderLocation");
                        request.interactionId = FindAttributeValue(qe, "crme_udointeractionid");

                        //Logger.WriteDebugMessage("request.interactionId:" + request.interactionId);

                        request.OrganizationName = PluginExecutionContext.OrganizationName;
                        request.UserId = PluginExecutionContext.UserId;


                        var user = TryGetUserName(PluginExecutionContext.UserId);

                        if (user != null)
                        {
                            request.UserFirstName = user.FirstName;
                            request.UserLastName = user.LastName;
                        }
                    }
                    else
                    {
                        //Logger.WriteDebugMessage("No qe.Criteria.Filters.Any()");
                    }
                }
                else
                {
                    //Logger.WriteDebugMessage("qe.Criteria is null");
                }

                //Logger.WriteDebugMessage("Ending SetQueryString Method");
            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.WriteToFile("Unable to set Query String due to: {0}".Replace("{0}",
                      ex.Message));
                throw new InvalidPluginExecutionException("Unable to set Query String due to: {0}".Replace("{0}",
                    ex.Message));
            }
        }      /// <summary>
               /// /
               /// </summary>
               /// <param name="expression"></param>
               /// <param name="fieldName"></param>
               /// <returns></returns>
        //private static string GetStringValueOrDefault(FilterExpression expression, string fieldName)
        //{
        //    try
        //    {
        //        var conditions =
        //            expression
        //            .Filters[0]
        //            .Conditions.FirstOrDefault(
        //                 v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));

        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Conditions.FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }

        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Conditions.FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }

        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Conditions.FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }

        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Conditions.FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }

        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Conditions.FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }

        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Conditions
        //                .FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }

        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Conditions
        //                .FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }

        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Conditions
        //                .FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }
        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Conditions
        //                .FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }
        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Conditions
        //                .FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }
        //        //
        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                 .Filters[0]
        //                .Conditions
        //                .FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }
        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                 .Filters[0]
        //                  .Filters[0]
        //                .Conditions
        //                .FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }
        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                 .Filters[0]
        //                  .Filters[0]
        //                   .Filters[0]
        //                .Conditions
        //                .FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }
        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                 .Filters[0]
        //                  .Filters[0]
        //                   .Filters[0]
        //                    .Filters[0]
        //                .Conditions
        //                .FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }
        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                 .Filters[0]
        //                  .Filters[0]
        //                   .Filters[0]
        //                    .Filters[0]
        //                     .Filters[0]
        //                .Conditions
        //                .FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }
        //        if (conditions == null)
        //        {
        //            conditions =
        //                expression
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                .Filters[0]
        //                 .Filters[0]
        //                  .Filters[0]
        //                   .Filters[0]
        //                    .Filters[0]
        //                     .Filters[0]
        //                      .Filters[0]
        //                .Conditions
        //                .FirstOrDefault(
        //                    v => v.AttributeName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        //        }
        //        if (conditions != null)
        //        {
        //            return conditions.Values[0].ToString();
        //        }
        //        else
        //        {

        //        }

        //    }
        //    catch (Exception)
        //    {
        //        return string.Empty;
        //    }

        //    return string.Empty;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionSetString"></param>
        /// <param name="entityName"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public int GetOptionSetValue(string optionSetString, string entityName, string attributeName)
        {
            try
            {
                var attributeRequest = new RetrieveAttributeRequest
                {
                    EntityLogicalName = entityName,
                    LogicalName = attributeName,
                    RetrieveAsIfPublished = false
                };

                // Retrieve only the currently published changes, ignoring the changes that have
                // not been published.

                var attributeResponse = (RetrieveAttributeResponse)OrganizationService.Execute(attributeRequest);

                // Access the retrieved attribute.
                var retrievedAttributeMetadata = (PicklistAttributeMetadata)attributeResponse.AttributeMetadata;
                return (from t in retrievedAttributeMetadata.OptionSet.Options
                        where t.Label.LocalizedLabels[0].Label == optionSetString
                        let value = t.Value
                        where value != null
                        select value.Value).FirstOrDefault();
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                PluginError = true;
                Logger.setModule = "getOptionSetValue";
                Logger.WriteToFile(ex.Detail.Message);
                Logger.setModule = "execute";
                return 0;
            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.setModule = "getOptionSetValue";
                Logger.WriteToFile(ex.Message);
                Logger.setModule = "execute";
                return 0;
            }
        }
        internal HeaderInfo GetHeaderInfo()
        {
            ColumnSet userCols = new ColumnSet("fullname", "va_stationnumber", "va_wsloginname", "va_applicationname", "va_ipaddress", "va_pcrsensitivitylevel");
            Entity thisUser = OrganizationService.Retrieve("systemuser", PluginExecutionContext.InitiatingUserId, userCols);

            const string stationNumberIsNotAssignedForCrmUser = "Station Number is not assigned for CRM User.";
            const string vaStationnumber = "va_stationnumber";

            if (!thisUser.Attributes.ContainsKey(vaStationnumber))
                throw new Exception(stationNumberIsNotAssignedForCrmUser);

            const string wsLoginIsNotAssignedForCrmUser = "WS Login is not assigned for CRM User.";
            const string vaWsloginname = "va_wsloginname";

            if (!thisUser.Attributes.ContainsKey(vaWsloginname))
                throw new Exception(wsLoginIsNotAssignedForCrmUser);

            const string applicationNameIsNotAssignedForCrmUser = "Application Name is not assigned for CRM User.";
            const string vaApplicationname = "va_applicationname";

            if (!thisUser.Attributes.ContainsKey(vaApplicationname))
                throw new Exception(applicationNameIsNotAssignedForCrmUser);

            const string clientMachineIsNotAssignedForCrmUser = "Client Machine is not assigned for CRM User.";
            const string vaIpAddress = "va_ipaddress";

            const string userSLIsNotAssignedForCrmUser = "PCR Sensitivity is not assigned for CRM User.";
            const string va_pcrsensitivitylevel = "va_pcrsensitivitylevel";

            if (!thisUser.Attributes.ContainsKey(vaIpAddress))
                throw new Exception(clientMachineIsNotAssignedForCrmUser);

            if (!thisUser.Attributes.ContainsKey(va_pcrsensitivitylevel))
                throw new Exception(userSLIsNotAssignedForCrmUser);

            var stationNumber = (string)thisUser[vaStationnumber];

            var loginName = (string)thisUser[vaWsloginname];

            var applicationName = (string)thisUser[vaApplicationname];

            var clientMachine = (string)thisUser[vaIpAddress];

            _user_SL = ((OptionSetValue)thisUser[va_pcrsensitivitylevel]).Value;
            _fullName = (string)thisUser["fullname"];

            return new HeaderInfo
            {
                StationNumber = stationNumber,

                LoginName = loginName,

                ApplicationName = applicationName,

                ClientMachine = clientMachine,

            };
        }

		//private static void MapFields(Entity source, string[] sourceCols, Entity dst, string[] dstCols)
		//{
		//    for (int i = 0; i < dstCols.Length; i++)
		//    {
		//        var srckey = sourceCols[i];
		//        var dstkey = dstCols[i];
		//        // if the destination key is empty, there is nothing to map.
		//        if (!String.IsNullOrEmpty(dstkey) && source.Contains(srckey))
		//        {
		//            dst[dstkey] = source[srckey];
		//        }
		//    }
		//}

		//private static string DumpToString(Entity entity, string name = null)
		//{
		//    string nulltext = "<null>";
		//    if (String.IsNullOrEmpty(name))
		//    {
		//        name = "Entity";
		//        if (entity != null) name = entity.LogicalName;
		//    }

		//    if (entity == null) return name + ": " + nulltext;

		//    StringBuilder entityDump = new StringBuilder();
		//    entityDump.AppendFormat("{0} [entity:{1} id:{2}]",
		//        name, entity.LogicalName, entity.Id);

		//    foreach (var attributeName in entity.Attributes.Keys)
		//    {
		//        //Append Name
		//        entityDump.AppendFormat("{0}: ", attributeName);

		//        var attributeObj = entity[attributeName];
		//        var attributeValue = attributeObj.ToString();
		//        if (attributeObj == null) attributeValue = nulltext;

		//        if (attributeObj is AliasedValue)
		//        {
		//            if (((AliasedValue)attributeObj).Value == null)
		//            {
		//                attributeValue = nulltext;
		//            }
		//            else
		//            {
		//                attributeObj = ((AliasedValue)attributeObj).Value;
		//            }
		//        }

		//        if (attributeObj is OptionSetValue)
		//        {
		//            var attrOptionSet = (OptionSetValue)attributeObj;
		//            attributeValue = attrOptionSet.Value.ToString();
		//        }
		//        else if (attributeObj is EntityReference)
		//        {
		//            attributeValue = string.Empty;
		//            var attrLookup = (EntityReference)attributeObj;
		//            attributeValue = string.Format("{0}{1}[entity:{2} id:{3}]",
		//                attrLookup.Name, String.IsNullOrEmpty(attrLookup.Name) ? "" : " ",
		//                attrLookup.LogicalName, attrLookup.Id);
		//        }

		//        if (attributeValue.Length > 200 && !(attributeObj is EntityReference))
		//        {
		//            attributeValue = "\r\n" + attributeValue;
		//        }

		//        if (entity.FormattedValues.ContainsKey(attributeName))
		//        {
		//            attributeValue += String.Format(" ({0})", entity.FormattedValues[attributeName]);
		//        }

		//        // Append Value
		//        entityDump.AppendFormat("{0}\r\n", attributeValue);
		//    }

		//    return entityDump.ToString();
		//}

		protected void GetSettingValues()
		{
			Trace("getSettingValues started");

			_logTimer = McsSettings.GetSingleSetting<bool>(_logTimerField);
			_logSoap = McsSettings.GetSingleSetting<bool>(_logSoapField);

			var uri = McsSettings.GetSingleSetting<string>(_vimtRestEndpointField);

			if (string.IsNullOrEmpty(uri)) throw new NullReferenceException("NO URI FOUND, cannot call VIMT");

			_uri2 = new Uri(uri);
			_debug = McsSettings.GetSingleSetting<bool>(_debugField);
			_timeOutSetting = McsSettings.GetSingleSetting<int>(_vimtTimeoutField);
			_addperson = McsSettings.GetSingleSetting<bool>("udo_addperson");
			_MVICheck = McsSettings.GetSingleSetting<bool>("udo_mvicheck");
			_bypassMvi = McsSettings.GetSingleSetting<bool>("udo_bypassmvi");


			#region CRMAuthenticationToken
			//TODO: get settings for AuthToken from McsSettings

			//CSDev
			//OAuthResourceid
			//string parentAppId = McsSettings.GetSingleSetting<string>("udo_parentapplicationid");
			string parentAppId = McsSettings.GetSingleSetting<string>("udo_oauthresourceid");
			//OAuthClientId
			//string clientAppId = McsSettings.GetSingleSetting<string>("udo_clientapplicationid");
			string clientAppId = McsSettings.GetSingleSetting<string>("udo_oauthclientid");
			string clientSecret = McsSettings.GetSingleSetting<string>("udo_oauthclientsecret");
			//CSDev
			//string tenentId = McsSettings.GetSingleSetting<string>("udo_tenantId");
			string tenentId = McsSettings.GetSingleSetting<string>("udo_aadtenent");
			//CSDev
			//string apimsubscriptionkey = McsSettings.GetSingleSetting<string>("udo_apimsubscriptionkey");
			string apimsubscriptionkey = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkey");
            string apimsubscriptionkeyS = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeysouth");
            string apimsubscriptionkeyE = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeyeast");
			//TODO: Remove following values and uncomment above once settings McsSettings has been confirmed
			//string parentAppId = "309c45c5-4af4-4bc1-8f66-38ff1bf1f2dc";
			//string clientAppId = "1bfe8a8d-ba9e-459a-99e3-6e4057179f51";
			//string clientSecret = "SPogjAbtlBI7oJm9CN9Pu5iOfQoj4Yytmwm0AvNKLmg=";
			//string tenentId = "edeazclabs.va.gov";

			//Create the token from settings
			_crmAuthTokenConfig = new CRMAuthTokenConfiguration
			{
				ParentApplicationId = parentAppId,
				ClientApplicationId = clientAppId,
				ClientSecret = clientSecret,
				TenantId = tenentId,
				ApimSubscriptionKey = apimsubscriptionkey,
                ApimSubscriptionKeyE = apimsubscriptionkeyE,
                ApimSubscriptionKeyS = apimsubscriptionkeyS
			};
            try
            {

                if (_debug)
                {

                    Trace("CRMAuthTokenConfiguration : " + JsonHelper.Serialize<CRMAuthTokenConfiguration>(_crmAuthTokenConfig));

                }
            }
            catch(Exception e)
            {
                PluginError = true;
                Trace(e.Message);
            }
			#endregion
		}

	}



    /// <summary>
    /// 
    /// </summary>
    internal class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

}