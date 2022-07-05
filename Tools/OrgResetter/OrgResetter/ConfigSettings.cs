using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgResetter
{
    public class ConfigSettings
    {
        public string ConnectionString { get; set; }
        public List<KeyValuePair<string,string>> BahKvps { get; set; }
        public List<KeyValuePair<string, string>> UsdOptions { get; set; }
        public List<KeyValuePair<string, string>> McsSettings { get; set; }
        public string SiteMapFilePath { get; set; }
    }
}
