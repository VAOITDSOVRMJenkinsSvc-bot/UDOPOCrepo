using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Management;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;
using System.Text;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Uii.Csr;
using Va.Udo.Usd.CustomControls.Shared;

namespace Va.Udo.Usd.CustomControls
{
    /// <summary>
    ///     Interaction logic for RoleSecurity.xaml
    /// </summary>
    public partial class LogRetriever : BaseHostedControlCommon
    {
        /// <summary>
        ///     Log writer
        /// </summary>
        private readonly TraceLogger logWriter;


        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicationName"></param>
        /// <param name="initXml"></param>
        public LogRetriever(Guid id, string applicationName, string initXml) :
            base(id, applicationName, initXml)
        {
            InitializeComponent();
            logWriter = new TraceLogger();
        }

        protected override void DesktopReady()
        {
            base.DesktopReady();
        }

        protected override void DoAction(RequestActionEventArgs args)
        {
            if (string.Compare(args.Action, "UDO", StringComparison.OrdinalIgnoreCase) == 0)
            {
                //CheckChatActiveSessions(args);
            }

            base.DoAction(args);
        }

        private static void CreateSystemInfoFile(List<KeyValuePair<string, string>> list, string path)
        {
            StringBuilder info = new StringBuilder();
            string headerFormat = "{0}";
            string itemFormat = "{0}\t:\t{1}";
            foreach (KeyValuePair<string, string> item in list)
            {
                if ((item.Key == "SEPARATOR" ? false : !(item.Key == "NAME")))
                {
                    info.AppendFormat(itemFormat, item.Key, item.Value).AppendLine();
                }
                else
                {
                    info.AppendFormat(headerFormat, item.Value).AppendLine();
                }
            }
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                StreamWriter writer = File.CreateText(path);
                try
                {
                    writer.Write(info.ToString());
                }
                finally
                {
                    if (writer != null)
                    {
                        ((IDisposable)writer).Dispose();
                    }
                }
            }
            catch (IOException oException)
            {
                Trace.WriteLine(oException);
            }
        }
        
        private static void CaptureDetailedSystemInfo(string zipFile)
        {
           
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            list.AddRange(LogRetriever.GetSystemInfo("Win32_DiskDrive", "DISK DRIVE DETAILS"));
            list.AddRange(LogRetriever.GetSystemInfo("Win32_OperatingSystem", "OPERATING SYSTEM DETAILS"));
            list.AddRange(LogRetriever.GetSystemInfo("Win32_Processor", "PROCESSOR DETAILS"));
            list.AddRange(LogRetriever.GetSystemInfo("Win32_ComputerSystem", "COMPUTER SYSTEM DETAILS"));
            list.AddRange(LogRetriever.GetSystemInfo("Win32_StartupCommand", "START UP DETAILS"));
            list.AddRange(LogRetriever.GetSystemInfo("Win32_ProgramGroup", "PROGRAM GROUP DETAILS"));
            string file = Path.Combine(Path.GetDirectoryName(zipFile), "SysInfo.txt");
            LogRetriever.CreateSystemInfoFile(list, file);
            ZipArchive zip = ZipFile.Open(zipFile, ZipArchiveMode.Update);
            zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
            zip.Dispose();
        }

        private static void CopyFile(string zipFile, string destination)
        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            var context = WindowsIdentity.GetCurrent().Impersonate();
            try
            {
                destination = Path.Combine(destination, Path.GetFileName(zipFile));
                File.Copy(zipFile, destination, true);
            }
            catch
            {
                context.Undo();
            }
            finally
            {
                context.Dispose();
            }
        }

        private static List<KeyValuePair<string, string>> GetSystemInfo(string table, string displayText)
        {
            string str;
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            string line = "================================================================";
            list.Add(new KeyValuePair<string, string>("SEPARATOR", line));
            list.Add(new KeyValuePair<string, string>("NAME", displayText));
            try
            {
                foreach (var mgmtObj in (new ManagementObjectSearcher(string.Concat("SELECT * FROM ", table))).Get())
                {
                    foreach (var props in mgmtObj.Properties)
                    {
                        str = (props.Value == null ? "" : props.Value.ToString());
                        list.Add(new KeyValuePair<string, string>(props.Name, str));
                    }
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
            }
            list.Add(new KeyValuePair<string, string>("SEPARATOR", line));
            return list;
        }

        private static string LocateFiles()
        {
            string str;
            int i;
            string rootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "MSCRM", "UII");
            if (Directory.Exists(rootFolder))
            {
                string tmpFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "USD Logs");
                if (!Directory.Exists(tmpFilePath))
                {
                    Directory.CreateDirectory(tmpFilePath);
                }
                string machineName = Environment.MachineName;
                DateTime today = DateTime.Today;
                string zipFileName = Path.Combine(tmpFilePath, string.Concat(machineName, "_USDlogs_", today.ToString("MM_dd_yyyy"), ".zip"));
                if (File.Exists(zipFileName))
                {
                    File.Delete(zipFileName);
                }
                ZipArchive zip = ZipFile.Open(zipFileName, ZipArchiveMode.Create);
                string[] txtFiles = Directory.GetFiles(rootFolder, "*.txt");
                string[] logFiles = Directory.GetFiles(rootFolder, "*.log");
                string[] strArrays = txtFiles;
                for (i = 0; i < (int)strArrays.Length; i++)
                {
                    string txtFile = strArrays[i];
                    zip.CreateEntryFromFile(txtFile, Path.GetFileName(txtFile), CompressionLevel.Optimal);
                }
                strArrays = logFiles;
                for (i = 0; i < (int)strArrays.Length; i++)
                {
                    string logFile = strArrays[i];
                    zip.CreateEntryFromFile(logFile, Path.GetFileName(logFile), CompressionLevel.Optimal);
                }
                zip.Dispose();
                str = zipFileName;
            }
            else
            {
                str = "";
            }
            return str;
        }

        internal static void RetrieveLogFiles(string networkPath, string emailAddress)
        {
            Trace.WriteLine("Starting the retrieval process...");
            string zipFileName = LogRetriever.LocateFiles();
            LogRetriever.CaptureDetailedSystemInfo(zipFileName);
            if (!string.IsNullOrEmpty(networkPath))
            {
                LogRetriever.UploadLogFile(zipFileName);
            }
            if (!string.IsNullOrEmpty(emailAddress))
            {
                LogRetriever.SendEmail(zipFileName, emailAddress);
            }
            Trace.WriteLine("Logs retrieved successfully!");
        }

        private static void SendEmail(string fileName, string toAddress)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                //Credentials dialog = new Credentials();
                //bool? result = dialog.ShowDialog();
                string emailAddress = "";
                string smtpServer = "";
                string username = "";
                string password = "";
                //if ((!result.HasValue || !result.HasValue ? false : result.Value))
                //{
                //    emailAddress = dialog.txtFromEmail.Text;
                //    smtpServer = dialog.txtSmtpServer.Text;
                //    username = dialog.txtUsername.Text;
                //    password = dialog.txtPassword.Password;
                    try
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient client = new SmtpClient(smtpServer);
                        mail.From = new MailAddress(emailAddress);
                        mail.To.Add(toAddress);
                        string machineName = Environment.MachineName;
                        DateTime today = DateTime.Today;
                        mail.Subject = string.Concat("USD Logs - ", machineName, " - ", today.ToString("MM_dd_yyyy"));
                        mail.Body = "USD LOG";
                        Attachment attachment = new Attachment(fileName);
                        mail.Attachments.Add(attachment);

                        client.Credentials = new NetworkCredential().GetCredential();
                        client.EnableSsl = true;
                        client.Send(mail);
                        //client.Credentials = new NetworkCredential(username, password);
                        //client.EnableSsl = true;
                        //client.Send(mail);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.ToString());
                    }
                //}
            }
        }

        private static void UploadLogFile(string zipFile)
        {
            //string destination = Helper.GetConfigurationValue("NetworkPath");
            //if ((string.IsNullOrEmpty(destination) ? true : Directory.Exists(destination)))
            //{
            //    LogRetriever.CopyFile(zipFile, destination);
            //}
        }


    }
}