using System;
using System.Globalization;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace Va.Udo.Usd.CustomControls.Shared
{
    public static class ExceptionManager
    {

        public static string ReportException(Exception exception)
        {
            return ReportException(exception, Assembly.GetCallingAssembly().GetName().Name, null, null);
        }

        public static string ReportException(Exception exception, string format, params object[] args)
        {
            return ReportException(exception, Assembly.GetCallingAssembly().GetName().Name, format, args);
        }

        public static string ReportException(Exception exception, string assemblyName, string format, params object[] args)
        {
            var sb = new StringBuilder();

            sb.AppendLine(string.Format("Exception generated at: {0}", DateTime.Now.ToString(CultureInfo.InvariantCulture)));
            sb.AppendLine(string.Format("Error Type: {0}", exception.GetType()));
            sb.AppendLine(string.Format("Error Message: {0}", exception.Message));
            sb.AppendLine(string.Format("Error Stack Trace: {0}", StackTraceToString(new StackTrace(exception))));

            var myAssembly = Assembly.GetCallingAssembly();
            sb.AppendLine(string.Format("Calling Assembly Name: {0}", myAssembly.GetName().Name));
            if (myAssembly.Location != null)
                sb.AppendLine(string.Format("Calling Assembly Version: {0}", FileVersionInfo.GetVersionInfo(myAssembly.Location).FileVersion));

            for (var i = exception.InnerException; i != null; i = i.InnerException)
            {
                sb.AppendLine(Environment.NewLine);
                sb.AppendLine("Inner Exception:");
                sb.AppendLine(string.Format("Error Type: {0}", i.GetType()));
                sb.AppendLine(string.Format("Error Message: {0}", i.Message));
                sb.AppendLine(string.Format("Error Stack Trace: {0}", StackTraceToString(new StackTrace(i))));
            }

            if (exception.Data.Count > 0)
            {
                sb.AppendLine(string.Concat(Environment.NewLine, "Exception Data:"));
                var num = 0;

                foreach (var key in exception.Data.Keys)
                {
                    num++;
                    sb.AppendLine(string.Format("\t{0}: Key type: {1}: key: {2}", num, key.GetType(), key));
                    var item = exception.Data[key];
                    if (item == null)
                    {
                        continue;
                    }
                    sb.AppendLine(string.Format("\t{0}: Value type: {1}: value: {2}", num, item.GetType(), item));
                }
            }

            if (!string.IsNullOrEmpty(format))
            {
                sb.AppendLine(string.Concat(Environment.NewLine, string.Format("Custom Message: {0}", string.Format(CultureInfo.InvariantCulture, format, args))));
            }

            return sb.ToString();
        }

        public static string StackTraceToString(StackTrace trace)
        {
            var sb = new StringBuilder(0x200);
            for (var i = 0; i <= trace.FrameCount; i++)
            {
                sb.Append(string.Concat(Environment.NewLine, "\tat "));
                StackFrameToStringBuilder(sb, trace.GetFrame(i), true);
            }

            return sb.ToString();
        }

        private static void StackFrameToStringBuilder(StringBuilder output, StackFrame frame, bool includeParameters)
        {
            if (frame == null) return;

            var method = frame.GetMethod();
            if (method != null && method.ReflectedType != null)
            {
                output.Append(method.ReflectedType.Name);
                output.Append(".");
                output.Append(method.Name);
                if (includeParameters)
                {
                    output.Append("(");
                    var parameters = method.GetParameters();
                    for (var i = 0; i < (int)parameters.Length; i++)
                    {
                        var parameterInfo = parameters[i];
                        if (i > 0)
                        {
                            output.Append(", ");
                        }
                        output.Append(parameterInfo.ParameterType.Name);
                        output.Append(" ");
                        output.Append(parameterInfo.Name);
                    }
                    output.Append(")");
                }

                var invariantCulture = CultureInfo.InvariantCulture;
                var lOffset = new object[] { frame.GetILOffset() };
                output.AppendFormat(invariantCulture, " ilOffset = 0x{0:X}", lOffset);
                var fileName = frame.GetFileName();
                if (fileName != null)
                {
                    output.Append(" ");
                    output.Append(fileName);
                }

                var fileLineNumber = frame.GetFileLineNumber();
                if (fileLineNumber > 0)
                {
                    output.Append("(");
                    output.Append(fileLineNumber.ToString(CultureInfo.InvariantCulture));
                    output.Append(")");
                }
            }
        }
    }
}
