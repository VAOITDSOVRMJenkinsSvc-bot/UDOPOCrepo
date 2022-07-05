using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Va.Udo.Usd.CustomControls.Tasks
{
    public class UpdateInteraction : ITask
    {

        const string INTERACTIONIDKEY = "InteractionId";
        private Dictionary<string, CrmDataTypeWrapper> fields;
        public bool Execute(TaskContext t)
        {
            if (t == null || !t.DataParameters.ContainsKey(INTERACTIONIDKEY))
                return false;
            var interactionId = t.DataParameters[INTERACTIONIDKEY];
            fields = new Dictionary<string, CrmDataTypeWrapper>();
            fields.Add("udo_endtime", new CrmDataTypeWrapper(DateTime.Now, CrmFieldType.CrmDateTime));
            fields.Add("udo_status",new CrmDataTypeWrapper(false,CrmFieldType.CrmBoolean));
            return t.Client.CrmInterface.UpdateEntity("udo_interaction", "udo_interactionid", new Guid(interactionId), fields);
        }
    }
}

