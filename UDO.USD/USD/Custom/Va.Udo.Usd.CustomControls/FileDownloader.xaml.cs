using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Windows.Threading;
using System.Xml;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Va.Udo.Usd.CustomControls.Shared;

namespace Va.Udo.Usd.CustomControls
{
    /// <summary>
    ///     Interaction logic for RoleSecurity.xaml
    /// </summary>
    public partial class FileDownLoader : BaseHostedControlCommon
    {
        //private CRMGlobalManager _globalManager;
        private static readonly ColumnSet reportColumns = new ColumnSet(
            "name"
            );
        private static readonly ColumnSet letterColumns = new ColumnSet(
            "udo_sourcetype",
            "udo_source",
            "udo_ssrsreportname"
            );
        private static readonly ColumnSet virtualvaColumns = new ColumnSet(
            "udo_documentformat",
            "udo_documentid",
            "udo_documentsource",
            "udo_regionaloffice"
            );
        private static readonly ColumnSet systemsettingColumns = new ColumnSet(
            "va_description",
            "va_name"
            );
        private static readonly ColumnSet systemuserColumns = new ColumnSet(
            "va_wsloginname",
            "domainname"
            );
        private static readonly ColumnSet noteColumns = new ColumnSet(
            "filename",
            "filesize",
            "mimetype",
            "documentbody"
            );

        private readonly TraceLogger _logWriter;
        private string _processstep;
        private string _dispatch;
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicationName"></param>
        /// <param name="initXml"></param>
        public FileDownLoader(Guid id, string applicationName, string initXml) :
            base(id, applicationName, initXml)
        {
            InitializeComponent();
            _logWriter = new TraceLogger();
        }

        protected override void DesktopReady()
        {
            base.DesktopReady();
        }

        protected override void DoAction(RequestActionEventArgs args)
        {
            _processstep = string.Format("{0} > started", args.Action);

            var parms = Utility.SplitLines(args.Data, CurrentContext, localSession);
            var datanodename = Utility.GetAndRemoveParameter(parms, "DataNodeName");
            _dispatch = Utility.GetAndRemoveParameter(parms, "DispatcherPriority");

            if (string.Compare(args.Action, "FileDownload", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region FileDownload

                UpdateStatus("File Download Started...");

                SafeDispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        #region Action Parameters

                        var hasPerson = Utility.GetAndRemoveParameter(parms, "HasPerson");
                        var target = Utility.GetAndRemoveParameter(parms, "Target").ToEntityReference();

                        var person = new EntityReference();
                        if (hasPerson.ToLower() != "false")
                        {
                            person = Utility.GetAndRemoveParameter(parms, "Person").ToEntityReference();
                        }


                        var specialFolder = Utility.GetAndRemoveParameter(parms, "SpecialFolder");
                        var reportName = Utility.GetAndRemoveParameter(parms, "ReportName");
                        var formatType = Utility.GetAndRemoveParameter(parms, "FormatType");
                        var sourceUrl = Utility.GetAndRemoveParameter(parms, "SourceUrl");
                        var uploadToVbms = Utility.GetAndRemoveParameter(parms, "UploadToVBMS");

                        #endregion

                        base.UpdateContext(datanodename, "ErrorOccurred", "");
                        base.UpdateContext(datanodename, "ErrorOccurredIn", "");
                        base.UpdateContext(datanodename, "ExceptionMessage", "");
                        base.UpdateContext(datanodename, "FilePath", "");
                        base.UpdateContext(datanodename, "DownloadedFile", "");
                        base.UpdateContext(datanodename, "DownloadedFileJS", "");

                        #region Check for Missing Action Parameters

                        if (target.Id == Guid.Empty)
                        {
                            throw new ArgumentException("Missing Target Reference");
                        }

                        if (hasPerson.ToLower() != "false")
                        {
                            if (person.Id == Guid.Empty)
                            {
                                throw new ArgumentException("Missing Person Reference");
                            }
                        }

                        if (string.IsNullOrEmpty(specialFolder))
                        {
                            throw new ArgumentException("Missing Download Folder");
                        }

                        if (string.IsNullOrEmpty(reportName))
                        {
                            throw new ArgumentException("Missing Report Name");
                        }

                        if (string.IsNullOrEmpty(formatType))
                        {
                            throw new ArgumentException("Missing Format Type");
                        }
                        #endregion

                        //
                        if ((target.LogicalName == "udo_servicerequest") && (reportName == "Non+Emergency+Email" || reportName == "Email+Forms"))
                        {
                            _processstep = "No Report generated";
                            base.UpdateContext(datanodename, "Processing Step", _processstep);
                            base.UpdateContext(datanodename, "AttachmentRequired", "N");
                            return;
                        }

                        base.UpdateContextFormat(datanodename, "Target", "EntityReferenace({0}, {1})", new object[] { target.LogicalName, target.Id.ToString() });

                        UpdateContext(datanodename, "SpecialFolder", specialFolder);
                        _processstep = "Get Special Folder";
                        var fileLocation = base.GetSpecialFolder(specialFolder);

                        // Check if Downloads folder exists, else try to get it through backup method
                        if (!Directory.Exists(fileLocation))
                        {
                            fileLocation = BackupGetDownloadsFolder();
                            if (!Directory.Exists(fileLocation))
                            {
                                fileLocation = base.GetSpecialFolder("CurrentDirectory");
                            }
                        }
                        base.UpdateContext(datanodename, "FilePath", fileLocation);

                        if (target.LogicalName == "udo_servicerequest")
                        {
                            reportName = GetSRReportName(reportName);
                        }

                        base.UpdateContext(datanodename, "ReportName", reportName);
                        base.UpdateContext(datanodename, "FormatType", formatType);

                        OrganizationResponse response = null;
                        if (target.LogicalName == "udo_servicerequest")
                        {
                            _processstep = "Start Execute Generate Service Request";
                            response = ExecuteGenerateServiceRequest(datanodename, target, person, reportName,
                                formatType, sourceUrl,
                                uploadToVbms == "Y", hasPerson);
                        }
                        else if (target.LogicalName == "udo_lettergeneration")
                        {
                            _processstep = "Start Execute Generate Letter";
                            response = ExecuteGenerateLetter(datanodename, target, person, reportName, formatType, sourceUrl,
                                uploadToVbms == "Y");
                        }

                        if (response != null)
                        {
                            UpdateStatus("File being saved on local device");
                            _processstep = "Create Download File";

                            var overrideFileName = Utility.GetAndRemoveParameter(parms, "OverrideFileName");
                            var fileName = CreateDownloadFile(response, fileLocation, overrideFileName);

                            UpdateStatus("File Saved Locally");
                            base.UpdateContext(datanodename, "DownloadedFile", fileName);
                            base.UpdateContext(datanodename, "DownloadedFileJS", fileName.Replace(@"\", @"\\"));
                            base.UpdateContext(datanodename, "AttachmentRequired", "Y");

                            // Open Email if required
                            var openEmail = Utility.GetAndRemoveParameter(parms, "OpenEmail");
                            if (!string.IsNullOrEmpty(openEmail))
                            {
                                if (openEmail.ToLower() == "true")
                                {
                                    datanodename = "OutlookEmail";
                                    _processstep = "Open Outlook Email - top";
                                    UpdateStatus("Open Outlook Email Started");
                                    base.UpdateContext(datanodename, "Processing Step", _processstep);

                                    var emailTo = Utility.GetAndRemoveParameter(parms, "EmailTo");
                                    var emailSubject = Utility.GetAndRemoveParameter(parms, "EmailSubject");
                                    var emailBody = Utility.GetAndRemoveParameter(parms, "EmailBody");
                                    var emailAttachmentPath = fileName;

                                    OpenEmail(emailTo, emailSubject, emailBody, emailAttachmentPath);

                                    _processstep = "Open Outlook Email - complete";
                                    base.UpdateContext(datanodename, "Processing Step", _processstep);
                                    UpdateStatus("Open Outlook Email Completed");
                                }
                            }
                        }
                        else
                        {
                            base.UpdateContext(datanodename, "ErrorOccurred", "Y");
                            base.UpdateContext(datanodename, "ErrorOccurredIn", "FileDownloader");
                            base.UpdateContext(datanodename, "ExceptionMessage", "Was not able to successfully able to retrieve your report.  Please try again.");
                        }

                    }
                    catch (System.ServiceModel.FaultException ex)
                    {
                        UpdateStatus("UNEXPECTED ERROR OCCURRED WHILE DOWNLOADING YOUR FILE");
                        base.UpdateContext(datanodename, "Processing Step", _processstep);

                        var exp = ExceptionManager.ReportException(ex);

                        base.UpdateContext(datanodename, "ErrorOccurred", "Y");
                        base.UpdateContext(datanodename, "ErrorOccurredIn", args.Action);
                        base.UpdateContext(datanodename, "ExceptionMessage", "Timeout occurred while retrieving report.  Please try again!");
                        base.UpdateContext(datanodename, "ExceptionReport", exp);
                    }
                    catch (System.Exception ex)
                    {
                        UpdateStatus("UNEXPECTED ERROR OCCURRED WHILE DOWNLOADING YOUR FILE");
                        base.UpdateContext(datanodename, "Processing Step", _processstep);

                        var exp = ExceptionManager.ReportException(ex);

                        base.UpdateContext(datanodename, "ErrorOccurred", "Y");
                        base.UpdateContext(datanodename, "ErrorOccurredIn", args.Action);
                        base.UpdateContext(datanodename, "ExceptionMessage", ex.Message);
                        base.UpdateContext(datanodename, "ExceptionReport", exp);
                    }

                }, base.GetDispatchPrioroity(_dispatch));

                #endregion
            }
            else if (string.Compare(args.Action, "OpenDownloadFile", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region OpenDownloadFile

                var downloadedFile = Utility.GetAndRemoveParameter(parms, "DownloadedFile");

                try
                {
                    _processstep = "Start Open Downloaded File";
                    if (File.Exists(downloadedFile))
                    {
                        Process.Start(downloadedFile);
                        _processstep = "Opened Downloaded File";
                    }
                }
                catch (Exception ex)
                {
                    base.UpdateContext(datanodename, "Processing Step", _processstep);

                    var exp = ExceptionManager.ReportException(ex);

                    base.UpdateContext(datanodename, "ErrorOccurred", "Y");
                    base.UpdateContext(datanodename, "ErrorOccurredIn", args.Action);
                    base.UpdateContext(datanodename, "ExceptionMessage", ex.Message);
                    base.UpdateContext(datanodename, "ExceptionReport", exp);
                }

                #endregion
            }
            else if (string.Compare(args.Action, "DeleteDownloadFile", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region DeleteDownloadFile

                var downloadedFile = Utility.GetAndRemoveParameter(parms, "DownloadedFile");

                try
                {
                    _processstep = "Start Delete Downloaded File";
                    if (File.Exists(downloadedFile))
                    {
                        File.Delete(downloadedFile);
                        _processstep = "Deleted Downloaded File";
                    }
                }
                catch (Exception ex)
                {
                    base.UpdateContext(datanodename, "Processing Step", _processstep);

                    var exp = ExceptionManager.ReportException(ex);

                    base.UpdateContext(datanodename, "ErrorOccurred", "Y");
                    base.UpdateContext(datanodename, "ErrorOccurredIn", args.Action);
                    base.UpdateContext(datanodename, "ExceptionMessage", ex.Message);
                    base.UpdateContext(datanodename, "ExceptionReport", exp);
                }

                #endregion
            }
            else if (string.Compare(args.Action, "ViewLetterContextUpdate", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region ViewLetterContextUpdate

                try
                {
                    _processstep = "View Letter Context Update - top";
                    base.UpdateContext(datanodename, "Processing Step", _processstep);
                    _processstep = "View Letter Context Update - update process step";

                    var sourceURL = Utility.GetAndRemoveParameter(parms, "sourceUrl");
                    _processstep = "View Letter Context Update - source URL";

                    var reportId = Utility.GetAndRemoveParameter(parms, "reportid");
                    _processstep = "View Letter Context Update - source URL";

                    var letterGuid = Utility.GetAndRemoveParameter(parms, "letterguid");

                    var contextURL = string.Format(sourceURL, reportId);
                    contextURL = contextURL + "&p:LetterGenerationGUID=" + letterGuid;

                    base.UpdateContext(datanodename, "RunViewLetter", contextURL);
                }
                catch (Exception ex)
                {
                    base.UpdateContext(datanodename, "Processing Step", _processstep);

                    var exp = ExceptionManager.ReportException(ex);

                    base.UpdateContext(datanodename, "ErrorOccurred", "Y");
                    base.UpdateContext(datanodename, "ErrorOccurredIn", args.Action);
                    base.UpdateContext(datanodename, "ExceptionMessage", ex.Message);
                    base.UpdateContext(datanodename, "ExceptionReport", exp);
                }

                #endregion
            }
            else if (string.Compare(args.Action, "GetVBMSDocument", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region GetVBMSDocument

                try
                {
                    _processstep = "Get VBMS Document - top";
                    UpdateStatus("VBMS Document Download Started");
                    base.UpdateContext(datanodename, "Processing Step", _processstep);

                    var specialFolder = Utility.GetAndRemoveParameter(parms, "SpecialFolder");
                    UpdateContext(datanodename, "VBMS-SpecialFolder", specialFolder);
                    _processstep = "Get Special Folder";
                    var fileLocation = base.GetSpecialFolder(specialFolder);
                    base.UpdateContext(datanodename, "VBMS-FilePath", fileLocation);

                    var vbmsguid = Utility.GetAndRemoveParameter(parms, "Id");

                    getVBMSDocument(datanodename, vbmsguid, fileLocation);

                    _processstep = "Get VBMS Document - complete";
                    base.UpdateContext(datanodename, "Processing Step", _processstep);
                    UpdateStatus("VBMS Document Download Completed");
                }
                catch (Exception ex)
                {
                    base.UpdateContext(datanodename, "Processing Step", _processstep);

                    var exp = ExceptionManager.ReportException(ex);

                    base.UpdateContext(datanodename, "ErrorOccurred", "Y");
                    base.UpdateContext(datanodename, "ErrorOccurredIn", args.Action);
                    base.UpdateContext(datanodename, "ExceptionMessage", ex.Message);
                    base.UpdateContext(datanodename, "ExceptionReport", exp);

                    UpdateStatus("Error Occurred while VBMS Document Download");
                }

                #endregion
            }
            else if (string.Compare(args.Action, "GetVirtualVADocument", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region GetVirtualVADocument

                /*
                 * Currently not used by VA and not working due to redirect service
                try
                {
                    _processstep = "Get Virtual VA Document - top";
                    UpdateStatus("Virtual VA Document Download Started");
                    base.UpdateContext(datanodename, "Processing Step", _processstep);

                    var virtualvaGuid = Utility.GetAndRemoveParameter(parms, "Id");

                    getVirualVADoc(datanodename, virtualvaGuid);

                    _processstep = "Get Virtual VA Document - complete";
                    base.UpdateContext(datanodename, "Processing Step", _processstep);
                    UpdateStatus("Virtual VA Document Download Completed");
                }
                catch (Exception ex)
                {
                    base.UpdateContext(datanodename, "Processing Step", _processstep);

                    var exp = ExceptionManager.ReportException(ex);

                    base.UpdateContext(datanodename, "ErrorOccurred", "Y");
                    base.UpdateContext(datanodename, "ErrorOccurredIn", args.Action);
                    base.UpdateContext(datanodename, "ExceptionMessage", ex.Message);
                    base.UpdateContext(datanodename, "ExceptionReport", exp);

                    UpdateStatus("Error Occurred while Virtual VA Document Download");
                }
                */

                #endregion
            }
            else if (string.Compare(args.Action, "OpenOutlookEmail", StringComparison.OrdinalIgnoreCase) == 0)
            {
                #region OpenOutlookEmail

                try
                {
                    _processstep = "Open Outlook Email - top";
                    UpdateStatus("Open Outlook Email Started");
                    base.UpdateContext(datanodename, "Processing Step", _processstep);

                    var emailTo = Utility.GetAndRemoveParameter(parms, "EmailTo");
                    var emailSubject = Utility.GetAndRemoveParameter(parms, "EmailSubject");
                    var emailBody = Utility.GetAndRemoveParameter(parms, "EmailBody");
                    var emailAttachmentPath = Utility.GetAndRemoveParameter(parms, "EmailAttachmentPath");

                    OpenEmail(emailTo, emailSubject, emailBody, emailAttachmentPath);

                    _processstep = "Open Outlook Email - complete";
                    base.UpdateContext(datanodename, "Processing Step", _processstep);
                    UpdateStatus("Open Outlook Email Completed");
                }
                catch (Exception ex)
                {
                    var exp = ExceptionManager.ReportException(ex);

                    base.UpdateContext(datanodename, "ErrorOccurred", "Y");
                    base.UpdateContext(datanodename, "ErrorOccurredIn", args.Action);
                    base.UpdateContext(datanodename, "ExceptionMessage", ex.Message);
                    base.UpdateContext(datanodename, "ExceptionReport", exp);

                    UpdateStatus("Error Occurred in OpenOutlookEmail");
                }

                #endregion
            }
            base.DoAction(args);
            base.UpdateContext(datanodename, "Processing Step", _processstep);
        }

        private void OpenEmail(string emailTo, string emailSubject, string emailBody, string emailAttachmentPath)
        {
            Microsoft.Office.Interop.Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
            Microsoft.Office.Interop.Outlook.MailItem oMsg = (Microsoft.Office.Interop.Outlook.MailItem)oApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);

            oMsg.To = emailTo;
            oMsg.Subject = emailSubject;
            oMsg.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
            oMsg.HTMLBody = emailBody;

            if (!string.IsNullOrEmpty(emailAttachmentPath))
            {
                string[] attachments = emailAttachmentPath.Split(',');
                foreach (var attachment in attachments)
                {
                    oMsg.Attachments.Add(attachment, Microsoft.Office.Interop.Outlook.OlAttachmentType.olByValue, Type.Missing, Type.Missing);
                }
            }
            oMsg.Display(false);
        }

        private string BackupGetDownloadsFolder()
        {
            string local = GetSpecialFolder("LocalApplicationData");
            string DownloadsPath = local.Replace(@"\AppData", "");
            return Path.Combine(DownloadsPath, "Downloads");
        }

        private string GetSRReportName(string action)
        {
            string reportName;

            switch (action)
            {
                case "0820":
                    reportName = "27-0820 - Report of General Information";
                    break;
                case "0820a":
                    reportName = "27-0820a - Report of First Notice of Death";
                    break;
                case "0820d":
                    reportName = "27-0820d - Report of Non-Receipt of Payment";
                    break;
                case "0820f":
                    reportName = "27-0820f - Report of Month of Death";
                    break;
                default:
                    reportName = action;
                    break;
            }

            return reportName;
        }

        private Entity RetrieveAnnotation(Guid id)
        {
            var cols = new ColumnSet("filename", "documentbody");
            var annotation = base.Retrieve("annotation", id, cols);

            return annotation;
        }

        private string CreateDownloadFile(OrganizationResponse response, string filePath, string overrideFileName)
        {

            var fileName = !string.IsNullOrEmpty(overrideFileName) ? overrideFileName : response["FileName"].ToString();

            var fileLocation = Path.Combine(filePath, fileName);

            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }

            using (var fileStream = new FileStream(fileLocation, FileMode.OpenOrCreate))
            {
                byte[] fileContent = Convert.FromBase64String(response["Base64FileContents"].ToString());
                fileStream.Write(fileContent, 0, fileContent.Length);
            }

            return fileLocation;
        }

        private OrganizationResponse ExecuteGenerateLetter(string datanodename, EntityReference target, EntityReference person, string reportName, string formatType, string sourceUrl, bool uploadToVbms)
        {

            var report = GetReportByReportName(reportName);
            reportName = reportName.Replace("&", "&amp;");

            sourceUrl = string.Format(sourceUrl, report.Id);

            base.UpdateContext(datanodename, "ReportName", reportName);
            base.UpdateContext(datanodename, "Report", report.Id.ToString());
            base.UpdateContext(datanodename, "SourceUrl", sourceUrl);

            var req = new OrganizationRequest("udo_GenerateLetter");
            req["Target"] = target;
            req["Person"] = person;
            req["ReportName"] = reportName;
            req["Report"] = report;
            req["FormatType"] = formatType;
            req["SourceUrl"] = sourceUrl;
            req["UploadToVBMS"] = uploadToVbms;
            req["ClaimNumber"] = "";

            return base.Execute(req);
        }

        private OrganizationResponse ExecuteGenerateServiceRequest(string datanodename, EntityReference target, EntityReference person, string reportName, string formatType, string sourceUrl, bool uploadToVbms, string hasPerson)
        {
            UpdateStatus("Service Request Report Requested");

            if (target.LogicalName == "udo_servicerequest")
            {
                reportName = reportName + " - UDO";
            }

            var report = GetReportByReportName(reportName);
            reportName = reportName.Replace("&", "&amp;");


            sourceUrl = string.Format(sourceUrl, report.Id);

            base.UpdateContext(datanodename, "ReportName", reportName);
            base.UpdateContext(datanodename, "Report", report.Id.ToString());
            base.UpdateContext(datanodename, "SourceUrl", sourceUrl);

            var req = new OrganizationRequest("udo_GenerateServiceRequest");
            req["Target"] = target;
            if (hasPerson.ToLower() != "false")
            {
                req["Person"] = person;
            }
            req["ReportName"] = reportName;
            req["Report"] = report;
            req["FormatType"] = formatType;
            req["SourceUrl"] = sourceUrl;
            req["UploadToVBMS"] = uploadToVbms;
            req["ClaimNumber"] = "";

            UpdateStatus("Service Request Report Returned");

            return base.Execute(req);
        }

        private EntityReference GetReportByReportName(string reportName)
        {
            var expression = new QueryExpression()
            {
                TopCount = 1,
                EntityName = "report",
                ColumnSet = reportColumns,
                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression()
                        {
                          Conditions =
                            {
                                new ConditionExpression("name", ConditionOperator.Equal, reportName)
                            }
                        }
                    }
                }
            };

            var results = RetrieveMultiple(expression);
            return results.Entities.Count > 0 ? results.Entities[0].ToEntityReference() : null;
        }

        private Entity GetLetterByReportName(string reportName)
        {
            var expression = new QueryExpression()
            {
                TopCount = 1,
                EntityName = "udo_letter",
                ColumnSet = letterColumns,
                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression()
                        {
                          Conditions =
                            {
                                new ConditionExpression("udo_name", ConditionOperator.Equal, reportName)
                            }
                        }
                    }
                }
            };

            var results = RetrieveMultiple(expression);
            return results.Entities.Count > 0 ? results.Entities[0] : null;
        }

        private void UpdateStatus(string input)
        {
            TxtStatus.Text = input;
            //TxtStatus.Dispatcher.Invoke(null, DispatcherPriority.Render);
        }

        private void getVBMSDocument(string datanodename, string vbmsGuid, string filePath)
        {
            base.UpdateContext(datanodename, "VBMS-guid", vbmsGuid);

            var target = new EntityReference("udo_vbmsefolder", new Guid(vbmsGuid));
            var req = new OrganizationRequest("udo_GetVBMSeFolderDocuments")
            {
                ["ParentEntityReference"] = new EntityReference("udo_vbmsefolder", new Guid(vbmsGuid))
            };

            //req.Parameters.Add("ParentEntityReference", target);
            var actionResult = base.Execute(req);

            // must find an Annoutation Id
            if (actionResult.Results.Contains("result"))
            {

                var er = (EntityReference)actionResult.Results["result"];
                base.UpdateContext(datanodename, "VBMS-annotationid", er.Id.ToString());

                var note = base.Retrieve("annotation", er.Id, noteColumns);
                if (note != null)
                {
                    base.UpdateContext(datanodename, "VBMS-filename", note["filename"].ToString());
                    var fileName = note["filename"].ToString();

                    var fileLocation = Path.Combine(filePath, fileName);
                    base.UpdateContext(datanodename, "VBMS-filelocation", fileLocation);

                    if (File.Exists(fileLocation))
                    {
                        File.Delete(fileLocation);
                    }

                    using (var fileStream = new FileStream(fileLocation, FileMode.OpenOrCreate))
                    {
                        byte[] fileContent = Convert.FromBase64String(note["documentbody"].ToString());
                        base.UpdateContext(datanodename, "VBMS-filecontent", fileContent.ToString());
                        fileStream.Write(fileContent, 0, fileContent.Length);
                    }

                    Process.Start(fileLocation);

                }
            }
        }

        private void getVirualVADoc(string datanodename, string virtualvaGuid)
        {
            var documentformat = string.Empty;
            var documentid = string.Empty;
            var documentsource = string.Empty;
            var regionaloffice = string.Empty;

            var vvausername = string.Empty;
            var vvapass = string.Empty;
            var vvabase = string.Empty;
            var isprod = string.Empty;
            var globaldac = string.Empty;

            var wsloginname = string.Empty;

            base.UpdateContext(datanodename, "VVA-guid", virtualvaGuid);

            var virtualva = base.Retrieve("udo_virtualva", new Guid(virtualvaGuid), virtualvaColumns);
            if (virtualva != null)
            {
                documentformat = virtualva["udo_documentformat"].ToString();
                base.UpdateContext(datanodename, "VVA-documentformat", documentformat);
                documentid = virtualva["udo_documentid"].ToString();
                base.UpdateContext(datanodename, "VVA-documentid", documentid);
                documentsource = virtualva["udo_documentsource"].ToString();
                base.UpdateContext(datanodename, "VVA-documentsource", documentsource);
                regionaloffice = virtualva["udo_regionaloffice"].ToString();
                base.UpdateContext(datanodename, "VVA-regionaloffice", regionaloffice);

                var expression = new QueryExpression()
                {
                    EntityName = "va_systemsettings",
                    ColumnSet = systemsettingColumns,
                    Criteria =
                    {
                        Filters =
                        {
                            new FilterExpression(LogicalOperator.Or)
                            {
                              Conditions =
                                {
                                    new ConditionExpression("va_name", ConditionOperator.Equal, "VVAUser"),
                                    new ConditionExpression("va_name", ConditionOperator.Equal, "VVAPassword"),
                                    new ConditionExpression("va_name", ConditionOperator.Equal, "VVABase"),
                                    new ConditionExpression("va_name", ConditionOperator.Equal, "isProd"),
                                    new ConditionExpression("va_name", ConditionOperator.Equal, "globalDAC")
                                }
                            }
                        }
                    }
                };

                var results = RetrieveMultiple(expression);

                if (results != null)
                {
                    foreach (var result in results.Entities)
                    {
                        switch (result["va_name"])
                        {
                            case "VVAUser":
                                vvausername = result["va_description"].ToString();
                                base.UpdateContext(datanodename, "VVA-User", vvausername);
                                break;
                            case "VVAPassword":
                                vvapass = result["va_description"].ToString();
                                base.UpdateContext(datanodename, "VVA-Password", vvapass);
                                break;
                            case "VVABase":
                                vvabase = result["va_description"].ToString();
                                base.UpdateContext(datanodename, "VVA-Base", vvabase);
                                break;
                            case "isProd":
                                isprod = result["va_description"].ToString();
                                base.UpdateContext(datanodename, "VVA-IsProd", isprod);
                                break;
                            case "globalDAC":
                                globaldac = result["va_description"].ToString();
                                base.UpdateContext(datanodename, "VVA-GlobalDAC", globaldac);
                                break;
                        }
                    }
                }

                var systemUserRequest = new WhoAmIRequest();
                var systemUserResponse = (WhoAmIResponse)_client.CrmInterface.Execute(systemUserRequest);
                Guid systemuserguid = systemUserResponse.UserId;

                var systemuser = base.Retrieve("systemuser", systemuserguid, systemuserColumns);
                if (systemuser != null)
                {
                    wsloginname = systemuser["va_wsloginname"].ToString();
                    base.UpdateContext(datanodename, "VVA-WSLogin", wsloginname);

                    var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
                            @"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" +
                            @"<soap:Body><DownloadVVADocument xmlns=""http://tempuri.org/"">" +
                            @"<vvaServiceUri>" + vvabase + "VABFI/services/vva" + @"</vvaServiceUri>" +
                            @"<userName>" + vvausername + @"</userName>" +
                            @"<password>" + vvapass + @"</password>" +
                            @"<fnDocId>" + documentid + @"</fnDocId>" +
                            @"<fnDocSource>" + documentsource + @"</fnDocSource>" +
                            @"<docFormatCode>" + documentformat + @"</docFormatCode>" +
                            @"<jro>" + regionaloffice + @"</jro>" +
                            @"<userId>" + wsloginname + @"</userId>" +
                            @"</DownloadVVADocument></soap:Body></soap:Envelope>";

                    // Create SOAP Envelope
                    XmlDocument soapEnvelopeDocument = new XmlDocument();
                    soapEnvelopeDocument.LoadXml(xml);

                    base.UpdateContext(datanodename, "VVA-SOAPDoc", xml);

                    string soapResult;
                    HttpWebResponse webResponse = null;

                    try
                    {
                        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(globaldac);
                        webRequest.Headers.Add("SOAPAction", "");
                        webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                        webRequest.Accept = "text/xml";
                        webRequest.Method = "POST";
                        //webRequest.ContentLength = soapEnvelopeDocument.ToString().Length;
                        webRequest.KeepAlive = false;
                        //webRequest.Timeout = System.Threading.Timeout.Infinite;
                        //webRequest.ProtocolVersion = HttpVersion.Version10;
                        webRequest.GetRequestStream().Write(Encoding.UTF8.GetBytes(soapEnvelopeDocument.InnerXml), 0, xml.Length);

                        webResponse = (HttpWebResponse)webRequest.GetResponse();
                        StreamReader rd = new StreamReader(webResponse.GetResponseStream());
                        soapResult = rd.ReadToEnd();

                        base.UpdateContext(datanodename, "VVA-Result", soapResult);
                    }
                    catch (WebException e)
                    {
                        base.UpdateContext(datanodename, "VVA-WebSatus", e.Status.ToString());
                        base.UpdateContext(datanodename, "VVA-WebErrorMessage", e.Message + " / " + e.InnerException.ToString() + " / " + e.StackTrace,ToString());

                        if (e.Status == WebExceptionStatus.ProtocolError)
                        {
                            webResponse = (HttpWebResponse)e.Response;
                            base.UpdateContext(datanodename, "VVA-WebSatusCode", webResponse.StatusCode.ToString() + " - " + webResponse.StatusDescription.ToString());
                        }
                    }
                    finally
                    {
                        if (webResponse != null)
                        {
                            webResponse.Close();
                        }
                    }





                }
            }
        }
    }
}