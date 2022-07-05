using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRM.EmailReport
{
    class EntityData
    {
        public string EntityName { get; set; }
        public Guid guid { get; set; }
        public Dictionary<string, string> data { get; set; }
        public List<Tuple<string, byte[]>> reports { get; set; }

        public EntityData(string entityName, Guid id, Dictionary<string, string> data)
        {
            EntityName = entityName;
            guid = id;
            this.data = data;
            reports = new List<Tuple<string,byte[]>>();
        }

        public void AddReport(string reportname, byte[] report){
            var reportentry = new Tuple<string, byte[]>(reportname, report);
            reports.Add(reportentry);
        }
    }
}
