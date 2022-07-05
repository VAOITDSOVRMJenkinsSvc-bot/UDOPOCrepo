using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Va.Udo.Usd.CustomControls.Tasks
{
    public class UpdateRequest:ITask
    {
        const string REQUESTIDKEY = "RequestId";
        private Dictionary<string, CrmDataTypeWrapper> fields;
        public bool Execute(TaskContext t)
        {
            if (t == null || !t.DataParameters.ContainsKey(REQUESTIDKEY))
                return false;
            var requestId = t.DataParameters[REQUESTIDKEY];
            fields = new Dictionary<string, CrmDataTypeWrapper>();
            fields.Add("udo_endtime", new CrmDataTypeWrapper(DateTime.Now, CrmFieldType.CrmDateTime));
            return t.Client.CrmInterface.UpdateEntity("udo_request", "udo_requestid", new Guid(requestId), fields);         
        }
    }
}
