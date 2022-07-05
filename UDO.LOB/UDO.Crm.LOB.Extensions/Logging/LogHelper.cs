using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UDO.LOB.Core;

namespace UDO.LOB.Extensions.Logging
{
    /// <summary>Log Helper Extension Class</summary>
    public class LogHelper
    {
        // Private static ILogger
        private static volatile ILog _logger;
        private static readonly object _lock = new object();

        //CSDev
        private static int _sequence = 1;

        // Logger property
        private static ILog logger
        {
            get
            {
                if (_logger != null)
                {
                    return _logger;
                }

                lock (_lock)
                {
                    if (_logger != null)
                    {
                        return _logger;
                    }

                    log4net.Config.XmlConfigurator.Configure();

                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                    var query = from assembly in assemblies.AsEnumerable()
                                where assembly.FullName.Split(',')[0].Split('.').ElementAtOrDefault(0) == "UDO" &&
                                assembly.FullName.Split(',')[0].Split('.').ElementAtOrDefault(3) != "Extensions" &&
                                assembly.FullName.Split(',')[0].Split('.').ElementAtOrDefault(2) != "Core" &&
                                assembly.FullName.Split(',')[0].Split('.').ElementAtOrDefault(2).Contains("Api")
                                select assembly;

                    var callingAssemby = query.FirstOrDefault();
                    string callerName = callingAssemby.FullName.Split(',')[0].Split('.')[2].Replace("Api", "");

                    string loggerName = callerName.ToLower() + "lob";
                    try
                    {
                        var virtualPath = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;

                        if (virtualPath != "/")
                        {
                            var folder = virtualPath.Replace("/", string.Empty);
                            loggerName += "-" + folder.ToLower();
                        }
                        else if (System.Web.Hosting.HostingEnvironment.SiteName.Contains("prod"))
                        {
                            loggerName += "_prod_";
                            if (System.Web.Hosting.HostingEnvironment.SiteName.Contains("south"))
                            {
                                loggerName += "south";
                            }
                            else
                            {
                                loggerName += "east";
                            }
                        }
                        else
                        {
                            loggerName += "-dev";
                        }
                    }
                    catch (Exception)
                    {
                        loggerName += "startup";
                    }

                    return _logger = LogManager.GetLogger(loggerName);
                }
            }
        }

        private enum LogMessageType
        {
            Debug = 935950000, Info = 935950001, Warn = 935950002, Error = 935950003, Timing = 935950005
        }

        private static readonly string TimestampFormat = "yyyy-MM-dd hh:mm:ss:fff";

        private static Guid LogToCrm(string callerName, string method, string message, LogMessageType messageType = LogMessageType.Debug)
        {
            Guid logId = Guid.Empty;
            try
            {
                if (message.Length > 999999)
                    message = message.Substring(0, 999998); // Prevent overflow exception
                Entity log = new Entity("mcs_log");
                log.Attributes["mcs_name"] = callerName;
                log.Attributes["mcs_errormessage"] = message;
                log.Attributes["crme_loglevel"] = new OptionSetValue((int)messageType);
                log.Attributes["mcs_method"] = method;
                log.Attributes["mcs_debugmessage"] = messageType.CompareTo(LogMessageType.Debug) == 0 ? true : false;
                //CSDev Add Sequence
                log.Attributes["mcs_sequence"] = _sequence;
                _sequence += 1;

                using (CrmServiceClient webProxyClient = ConnectionCache.GetProxy())
                {
                    logId = webProxyClient.Create(log);
                }

            }
            catch (Exception ex)
            {
                if (logger.IsErrorEnabled)
                    logger.Error($"ERROR: LogToCrm:: Message: {ex.Message} \r\n {WebApiUtility.StackTraceToString(ex)}");
            }

            return logId;

        }
        public static Guid StartTiming(string organizationName, string configFieldName, Guid systemUserId, Guid dependentMaintenanceId, string v1, string v2, string method, object p, out DateTime methodStartTime)
        {
            methodStartTime = DateTime.Now;
            return Guid.Empty;
        }

        public static void EndTiming(Guid wsLoggingId, string organizationName, string configFieldName, Guid systemUserId, DateTime wsStartTime)
        {
        }

        private static Guid LogToCrm(string method, string message)
        {
            try
            {
                string traceEntry = $"LOGTOCRM: [{DateTime.Now.ToString(TimestampFormat)}] {message}";

                if (logger.IsInfoEnabled)
                    logger.Info(traceEntry);

                return LogToCrm(System.Reflection.Assembly.GetCallingAssembly().FullName.Split(',')[0], method, traceEntry, LogMessageType.Debug);
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        public static void LogInfo(string orgName, bool debug, Guid userId, string method, string details)
        {
            try
            {
                if (logger.IsInfoEnabled)
                    logger.Info(Sanitizer($"LOGINFO: [{DateTime.Now.ToString(TimestampFormat)}] {orgName} {userId} {method} {details}"));
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        public static void LogInfo(string corelationId, string orgName, Guid userId, string method, params object[] details)
        {
            try
            {
                if (logger.IsInfoEnabled)
                    logger.Info(Sanitizer($"LOGINFO: [{DateTime.Now.ToString(TimestampFormat)}] {corelationId} {orgName} {userId} {method}"));
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        public static void LogInfo(string message, LogSettings logSettings)
        {
            try
            {
                if (logger.IsInfoEnabled)
                {
                    if (logSettings is null)
                    {
                        logger.Info(Sanitizer($"LOGINFO: [{DateTime.Now.ToString(TimestampFormat)}] {message}"));
                    }
                    else
                    {
                        logger.Info(Sanitizer($"LOGINFO: [{DateTime.Now.ToString(TimestampFormat)}] {logSettings.Org} {logSettings.UserId} {logSettings.CallingMethod} {message}"));
                    }
                }
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        public static void LogInfo(string message)
        {
            LogInfo(message, null);
        }

        public static void LogSoap(string messageId, string orgName, Guid userId, string method, string details, bool logSoap)
        {
            try
            {
                string callingAssembly = System.Reflection.Assembly.GetCallingAssembly().FullName.Split(',')[0];
                LogSoap(callingAssembly, messageId, orgName, userId, method, details, logSoap);
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        public static void LogSoap(string messageId, string orgName, Guid userId, string callingAssembly, string method, string details, bool logSoap)
        {
            LogSoap(callingAssembly, messageId, orgName, userId, method, details, logSoap);
        }

        public static void LogDebug(string messageId, string orgName, Guid userId, string method, string details, bool debug)
        {
            try
            {
                string callingAssembly = System.Reflection.Assembly.GetCallingAssembly().FullName.Split(',')[0];
                LogDebug(callingAssembly, messageId, orgName, userId, Guid.Empty, null, null, method, details, debug);
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        public static void LogDebug(string orgName, bool debug, Guid userId, string method, string details)
        {
            try
            {
                string callingAssembly = System.Reflection.Assembly.GetCallingAssembly().FullName.Split(',')[0];

                LogDebug(callingAssembly, orgName, userId, debug, Guid.Empty, null, null, method, details);
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        public static void LogDebug(string callingAssembly, string orgName, Guid userId, bool debug, Guid relatedParentId, string relatedParentName, string relatedParentFieldName, string callingMethod, string message)
        {
            try
            {
                if (debug)
                {
                    string traceEntry = $"LOGDEBUG: [{DateTime.Now.ToString(TimestampFormat)}] {orgName} {userId} {callingMethod} {relatedParentId} {relatedParentName} {relatedParentFieldName} {message}";

                    if (logger.IsDebugEnabled)
                        logger.Debug(Sanitizer(traceEntry));

                    LogToCrm(callingAssembly, callingMethod, traceEntry, LogMessageType.Debug);
                }
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        public static void LogDebug(string callingAssembly, string messageId, string orgName, Guid userId, Guid relatedParentId, string relatedParentName, string relatedParentFieldName, string callingMethod, string message, bool debug)
        {
            try
            {
                if (debug)
                {
                    string traceEntry = $"LOGDEBUG: [{DateTime.Now.ToString(TimestampFormat)}] {messageId} {orgName} {userId} {callingMethod} {relatedParentId} {relatedParentName} {relatedParentFieldName} {message}";

                    if (logger.IsDebugEnabled)
                        logger.Debug(Sanitizer(traceEntry));

                    LogToCrm(callingAssembly, callingMethod, traceEntry, LogMessageType.Debug);
                }
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        public static void LogSoap(string callingAssembly, string messageId, string orgName, Guid userId, string callingMethod, string message, bool logSoap)
        {
            try
            {
                if (logSoap)
                {
                    string traceEntry = $"LOGSOAP: [{DateTime.Now.ToString(TimestampFormat)}] {messageId} {orgName} {userId} {callingMethod} {message}";

                    if (logger.IsDebugEnabled)
                        logger.Debug(Sanitizer(traceEntry));

                    LogSoapToCrm(callingAssembly, callingMethod, message);
                }
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        public static void LogError(string orgName, Guid userId, string method, string details)
        {
            try
            {
                if (logger.IsErrorEnabled)
                {
                    string callingAssembly = System.Reflection.Assembly.GetCallingAssembly().FullName.Split(',')[0];
                    string traceEntry = $"LOGERROR: [{DateTime.Now.ToString(TimestampFormat)}] {orgName} {userId} {method} {details}";
                    logger.Error(Sanitizer(traceEntry));
                }
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        public static void LogError(string orgName, bool debug, Guid userId, string method, string details)
        {
            LogError(orgName, userId, method, details);
        }

        public static void LogError(string messageId, string orgName, Guid userId, string method, string details)
        {
            try
            {
                string callingAssembly = System.Reflection.Assembly.GetCallingAssembly().FullName.Split(',')[0];
                string traceEntry = $"LOGERROR: [{DateTime.Now.ToString(TimestampFormat)}] {messageId} {orgName} {userId} {method} {details}";

                if (logger.IsErrorEnabled)
                    logger.Error(Sanitizer(traceEntry));

                LogToCrm(callingAssembly, method, traceEntry, LogMessageType.Error);
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        public static void LogError(string orgName, Guid userId, string method, Exception exception)
        {
            LogError(orgName, userId, null, method, exception);
        }

        public static void LogError(string orgName, Guid userId, string messageId, string method, Exception exception)
        {
            LogError(messageId, orgName, userId, Guid.Empty, null, null, method, exception);
        }

        public static void LogError(string messageId, string orgName, Guid userId, string method, Exception exception)
        {
            LogError(messageId, orgName, userId, Guid.Empty, null, null, method, exception);
        }

        /// <summary>Logs the error.</summary>
        /// <param name="orgName">Org Name from the Request.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="relatedParentId">The related parent identifier.</param>
        /// <param name="relatedParentName">Name of the related parent.</param>
        /// <param name="relatedParentFieldName">Name of the related parent field.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="method">The method.</param>
        /// <param name="exception">The exception where the error occurred.</param>
        public static void LogError(string orgName, Guid userId, Guid relatedParentId, string relatedParentName, string relatedParentFieldName, string messageId, string method, Exception exception)
        {
            try
            {
                var exceptionDetails = $"ERROR: {exception.Message}\r\nSOURCE: {exception.Source}\r\n\r\n ExceptionType: {exception.GetType().Name}";
                if (exception.InnerException != null)
                {
                    exceptionDetails += $"\r\nInnerException: {exception.InnerException.Message}\r\n InnerExceptionType: {exception.InnerException.GetType().Name}";
                }
                exceptionDetails += $"\r\nCall Stack: \r\n {WebApiUtility.StackTraceToString(exception)}";

                messageId = string.IsNullOrEmpty(messageId) ? "-NA-" : messageId;
                string traceEntry = $"LOGERROR: [{DateTime.Now.ToString(TimestampFormat)}] OrgName: {orgName} UserId: {userId} Method: {method}  MessageId: {messageId} \r\n Exception Details: {exceptionDetails}";

                if (logger.IsErrorEnabled)
                    logger.Error(Sanitizer(traceEntry));

                LogToCrm(System.Reflection.Assembly.GetCallingAssembly().FullName.Split(',')[0], method, traceEntry, LogMessageType.Error);
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        public static void LogError(string messageId, string orgName, Guid userId, Guid relatedParentId, string relatedParentName, string relatedParentFieldName, string method, Exception exception)
        {
            try
            {
                var exceptionDetails = $"ERROR: {exception.Message}\r\nSOURCE: {exception.Source}\r\n\r\n ExceptionType: {exception.GetType().Name}";
                if (exception.InnerException != null)
                {
                    exceptionDetails += $"\r\nInnerException: {exception.InnerException.Message}\r\n InnerExceptionType: {exception.InnerException.GetType().Name}";
                }
                exceptionDetails += $"\r\nCall Stack: \r\n {WebApiUtility.StackTraceToString(exception)}";

                messageId = string.IsNullOrEmpty(messageId) ? "-NA-" : messageId;
                string traceEntry = $"LOGERROR: [{DateTime.Now.ToString(TimestampFormat)}] {messageId} {orgName} {userId} {method} \r\n Exception Details: {exceptionDetails}";
                
                if (logger.IsErrorEnabled)
                    logger.Error(Sanitizer(traceEntry));

                LogToCrm(System.Reflection.Assembly.GetCallingAssembly().FullName.Split(',')[0], method, traceEntry, LogMessageType.Error);
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        public static void LogTiming(string messageId, string orgName, bool logTiming, Guid userId, Guid relatedParentId, string relatedParentEntityName, string relatedParentFieldName, string message, object p, long elapsedMilliseconds)
        {
            try
            {
                string callerName = System.Reflection.Assembly.GetCallingAssembly().FullName.Split(',')[0];
                if (logTiming)
                {
                    string traceEntry = $"LOGTIMING: [{DateTime.Now.ToString(TimestampFormat)}] {messageId} {orgName} {userId} {relatedParentId} {relatedParentEntityName} {relatedParentFieldName} {message} {elapsedMilliseconds} ms.";

                    if (logger.IsInfoEnabled)
                        logger.Info(Sanitizer(traceEntry));

                    LogToCrm(callerName, message, traceEntry, LogMessageType.Timing);
                }
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        public static void LogTiming(string orgName, bool logTiming, Guid userId, Guid relatedParentId, string relatedParentEntityName, string relatedParentFieldName, string message, object p, long elapsedMilliseconds)
        {
            try
            {
                string callerName = System.Reflection.Assembly.GetCallingAssembly().FullName.Split(',')[0];
                if (logTiming)
                {
                    string traceEntry = $"LOGTIMING: [{DateTime.Now.ToString(TimestampFormat)}] {orgName} {userId} {relatedParentId} {relatedParentEntityName} {relatedParentFieldName} {message} {elapsedMilliseconds} ms.";

                    if (logger.IsInfoEnabled)
                        logger.Info(Sanitizer(traceEntry));

                    LogToCrm(callerName, message, traceEntry, LogMessageType.Timing);
                }
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        //CSDev DepMain methods for logging with enable proxy types
        public static void LogDebug(string orgName, bool debug, Guid userId, string method, string details, bool enableProxyTypes)
        {
            LogDebug(orgName, userId, debug, Guid.Empty, null, null, method, details, enableProxyTypes);
        }

        public static void LogDebug(string orgName, Guid userId, bool debug, Guid relatedParentId, string relatedParentName, string relatedParentFieldName, string callingMethod, string message, bool enableProxyTypes)
        {
            try
            {
                if (debug)
                {
                    string callingAssembly = System.Reflection.Assembly.GetCallingAssembly().FullName.Split(',')[0];
                    string traceEntry = $"LOGDEBUG: [{DateTime.Now.ToString(TimestampFormat)}] {orgName} {userId} {callingMethod} {relatedParentId} {relatedParentName} {relatedParentFieldName} {message}";

                    if (logger.IsDebugEnabled)
                        logger.Debug(Sanitizer(traceEntry));

                    LogToCrm(callingAssembly, callingMethod, traceEntry, enableProxyTypes, LogMessageType.Debug);
                }
            }
            catch (Exception)
            {
                // do nothing for log event failure
            }
        }

        private static Guid LogToCrm(string callerName, string method, string message, bool enableProxyTypes, LogMessageType messageType = LogMessageType.Debug)
        {
            Guid logId = Guid.Empty;
            try
            {
                Entity log = new Entity("mcs_log");
                log.Attributes["mcs_name"] = callerName;
                log.Attributes["mcs_errormessage"] = message;
                log.Attributes["crme_loglevel"] = new OptionSetValue((int)messageType);
                log.Attributes["mcs_method"] = method;
                log.Attributes["mcs_debugmessage"] = messageType.CompareTo(LogMessageType.Debug) == 0 ? true : false;
                //CSDev Add Sequence
                log.Attributes["mcs_sequence"] = _sequence;
                _sequence += 1;

                using (CrmServiceClient webProxyClient = ConnectionCache.GetProxy())
                {
                    logId = webProxyClient.Create(log);
                }
            }
            catch (Exception ex)
            {
                if (logger.IsErrorEnabled)
                    logger.Error($"ERROR: LogToCrm:: Message: {ex.Message} \r\n {WebApiUtility.StackTraceToString(ex)}");
            }

            return logId;
        }

        private static Guid LogSoapToCrm(string callerName, string method, string message)
        {
            Guid logId = Guid.Empty;
            try
            {
                Entity log = new Entity("mcs_log");
                log.Attributes["mcs_name"] = callerName;
                log.Attributes["mcs_errormessage"] = message;
                log.Attributes["mcs_method"] = method;
                log.Attributes["udo_soaplog"] = true;
                //CSDev Add Sequence
                log.Attributes["mcs_sequence"] = _sequence;
                _sequence += 1;

                using (CrmServiceClient webProxyClient = ConnectionCache.GetProxy())
                {
                    logId = webProxyClient.Create(log);
                }
            }
            catch (Exception ex)
            {
                if (logger.IsErrorEnabled)
                    logger.Error($"ERROR: LogSoapToCrm:: Message: {ex.Message} \r\n {WebApiUtility.StackTraceToString(ex)}");
            }

            return logId;
        }

        public static string Sanitizer(string potentiallyDirtyMsg)
        {
            try
            {
                // Fortify required to cleanse messages
                /*Regex r = new Regex(
                      "(?:[^a-zA-Z0-9 ]|(?<=['\"])s)",
                      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                return r.Replace(potentiallyDirtyMsg, String.Empty); */
                return potentiallyDirtyMsg;
            }
            catch (Exception ex)
            {
                return "Error sanitizing message: " + ex.Message;
            }
        }
    }

    public class LogSettings
    {
        /// <summary>
        /// CRM Organization
        /// </summary>
        public string Org { get; set; }
        /// <summary>
        /// Future Use On/Off Switch
        /// </summary>
        public string ConfigFieldName { get; set; }
        /// <summary>
        /// Calling User
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Calling Method
        /// </summary>
        public string CallingAssembly { get; set; }
        public string CallingMethod { get; set; }
        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public string MessageId { get; set; }

        public bool Debug { get; set; }

        public bool logSoap { get; set; }
    }
}
