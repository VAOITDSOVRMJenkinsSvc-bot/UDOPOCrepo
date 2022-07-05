// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAiLogger.cs" company="Microsoft">
// © 2017 Microsoft Corporation. All rights reserved
// </copyright>
// <author></author>
// <date>13/07/2017 10:16:37 AM</date>
// <summary>
//   Defines the App Insights logger interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UDO.LOB.Core.Logger
{
    using System;

    /// <summary>
    /// The AiLogger interface.
    /// </summary>
    public interface IAiLogger
    {
        /// <summary>
        /// The information.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void Information(string message);

        /// <summary>
        /// The information.
        /// </summary>
        /// <param name="message">The Message</param>
        /// <param name="parameters">The parameters</param>
        void Information(string message, params object[] parameters);

        /// <summary>
        /// The information.
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <param name="message">The Message</param>
        /// <param name="parameters">The parameters</param>
        void Information(Exception exception, string message, params object[] parameters);

        /// <summary>
        /// The warning.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void Warning(string message);

        /// <summary>
        /// The warning.
        /// </summary>
        /// <param name="message">The Message</param>
        /// <param name="parameters">The parameters</param>
        void Warning(string message, params object[] parameters);

        /// <summary>
        /// The warning.
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <param name="message">The Message</param>
        /// <param name="parameters">The parameters</param>
        void Warning(Exception exception, string message, params object[] parameters);

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void Error(string message);

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">The Message</param>
        /// <param name="parameters">The parameters</param>
        void Error(string message, params object[] parameters);

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <param name="message">The Message</param>
        /// <param name="parameters">The parameters</param>
        void Error(Exception exception, string message, params object[] parameters);
    }
}