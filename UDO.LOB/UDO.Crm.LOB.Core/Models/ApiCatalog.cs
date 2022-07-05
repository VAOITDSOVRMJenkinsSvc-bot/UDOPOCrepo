using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UDO.LOB.Core
{
    [DataContract]
    public class ApiCatalog
    {
        [DataMember]
        public List<Api> ApiCollection { get; private set; }

        public string this[string requestName]
        {
            get
            {
                return this.ApiCollection.Where<Api>(t => t.RequestName == requestName).FirstOrDefault().ApiRoute;
            }
        }

        public string this[Type responseType]
        {
            get
            {
                var name = responseType.Name;

                return this.ApiCollection
                    .Where<Api>(t => t.ResponseName == name).FirstOrDefault().ApiRoute;
            }
        }
    }

    [DataContract]
    public class Api
    {
        [DataMember]
        public string RequestName { get; set; }
        [DataMember]
        public string ResponseName { get; set; }
        [DataMember]
        public string ApiRoute { get; set; }

    }

}
