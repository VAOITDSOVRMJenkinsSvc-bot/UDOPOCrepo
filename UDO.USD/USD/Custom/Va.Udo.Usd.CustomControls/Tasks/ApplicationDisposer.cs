using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Desktop.SessionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Va.Udo.Usd.CustomControls.Tasks
{
    public class ApplicationDisposer:ITask
    {
        public DynamicsBaseHostedControl Control { get; set; }
        public ApplicationDisposer(DynamicsBaseHostedControl control)
        {
            this.Control = control;
        }
        public bool Execute(TaskContext t)
        {
            var applicationList = t.Session.AppHost.GetApplications();
            var openApplications = new List<Guid>();

            foreach (var app in applicationList)
            {
                DynamicsBaseHostedControl control = app as DynamicsBaseHostedControl;
                if (control != null && !control.IsGlobal && control is BrowserWindowEx)
                {
                    openApplications.Add(app.ApplicationID);
                }
            }
           
            var results = Parallel.ForEach<Guid>(openApplications, (guid) => { CloseApplication(guid, t.Session); });

            return results.IsCompleted;
        }

        public void CloseApplication(Guid appId, Session session)
        {
            var appName = session.AppHost.GetApplicationName(appId);
            this.Control.Dispatcher.Invoke(() =>
            {
                this.Control.FireRequestAction(new RequestActionEventArgs(appName, "Close", null));
            });           
        }
    }
}
