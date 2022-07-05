// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AiLogger.cs" company="Microsoft">
// © 2017 Microsoft Corporation. All rights reserved
// </copyright>
// <author></author>
// <date>13/07/2017 10:16:37 AM</date>
// <summary>
//   Defines the App Insights logger type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UDO.LOB.Core.Logger
{
    using System;

    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    /// <summary>
    /// The ai logger.
    /// </summary>
    internal class AiLogger : IAiLogger
    {
        /// <summary>
        /// The telemetry client.
        /// </summary>
        private readonly TelemetryClient telemetryClient = new TelemetryClient();

        /// <summary>
        /// The information.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Information(string message)
        {
            this.telemetryClient.TrackTrace(message, SeverityLevel.Information);
        }

        /// <summary>
        /// The information.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void Information(string message, params object[] parameters)
        {
            this.telemetryClient.TrackTrace(string.Format(message, parameters), SeverityLevel.Information);
        }

        /// <summary>
        /// The information.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void Information(Exception exception, string message, params object[] parameters)
        {
            var telemetry = new TraceTelemetry(string.Format(message, parameters), SeverityLevel.Information);
            telemetry.Properties.Add("Exception", exception.Message);
            this.telemetryClient.TrackTrace(telemetry);
        }

        /// <summary>
        /// The warning.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Warning(string message)
        {
            this.telemetryClient.TrackTrace(message, SeverityLevel.Warning);
        }

        /// <summary>
        /// The warning.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void Warning(string message, params object[] parameters)
        {
            this.telemetryClient.TrackTrace(string.Format(message, parameters), SeverityLevel.Warning);
        }

        /// <summary>
        /// The warning.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void Warning(Exception exception, string message, params object[] parameters)
        {
            var telemetry = new TraceTelemetry(string.Format(message, parameters), SeverityLevel.Warning);
            telemetry.Properties.Add("Exception", exception.Message);
            this.telemetryClient.TrackTrace(telemetry);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Error(string message)
        {
            this.telemetryClient.TrackTrace(message, SeverityLevel.Error);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void Error(string message, params object[] parameters)
        {
            this.telemetryClient.TrackTrace(string.Format(message, parameters), SeverityLevel.Error);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void Error(Exception exception, string message, params object[] parameters)
        {
            var telemetry = new ExceptionTelemetry(exception);
            telemetry.Properties.Add("message", string.Format(message, parameters));
            ////telemetry.Properties.Add("trace", exception.StackTrace);
            this.telemetryClient.TrackException(telemetry);
        }
    }
}