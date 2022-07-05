using Microsoft.Uii.Desktop.SessionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Va.Udo.Usd.CustomControls
{
    public interface ITask
    {
        bool Execute(TaskContext t);
    }

    public class TaskContext
    {
        #region Private Fields
        /// <summary>
        /// Active Session Id
        /// </summary>
        private Guid sessionId;
        #endregion

        #region Properties
        /// <summary>
        /// Access point to CRM Service
        /// </summary>
        public Microsoft.Crm.UnifiedServiceDesk.BaseControl.ICrmUtilityHostedControl Client { get; private set; }

        /// <summary>
        /// Session Manger
        /// </summary>
        public AgentDesktopSessions LocalSessionManager { get; private set; }

        /// <summary>
        /// Session 
        /// </summary>
        public AgentDesktopSession Session { get { return LocalSessionManager.GetSession(sessionId) as AgentDesktopSession; } }

        /// <summary>
        /// List of parameters passed into the action.
        /// </summary>
        public Dictionary<string, string> DataParameters { get; private set; }
        #endregion

        /// <summary>
        /// .CTOR
        /// </summary>
        /// <param name="crmUtilityHostedControl"></param>
        /// <param name="agentDesktopSessions"></param>
        /// <param name="sessionId"></param>
        public TaskContext(Microsoft.Crm.UnifiedServiceDesk.BaseControl.ICrmUtilityHostedControl crmUtilityHostedControl, Microsoft.Uii.Desktop.SessionManager.AgentDesktopSessions agentDesktopSessions, string sessionId)
        {
            // TODO: Complete member initialization
            this.DataParameters = new Dictionary<string, string>();
            this.Client = crmUtilityHostedControl;
            this.LocalSessionManager = agentDesktopSessions;
            this.sessionId = new Guid(sessionId);
        }

        
    }

}
