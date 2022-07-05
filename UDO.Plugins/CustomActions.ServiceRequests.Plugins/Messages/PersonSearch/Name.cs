using System.Runtime.Serialization;

namespace UDO.LOB.PersonSearch.Models
{
    [DataContract]
    public class Name
    {
        [DataMember]
        public string GivenName { get; set; }

        [DataMember]
        public string FamilyName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string NamePrefix { get; set; }

        [DataMember]
        public string NameSuffix { get; set; }

        /// <summary>
        /// Legal, Alias, Maiden
        /// </summary>
        /// 
        [DataMember]
        public string NameType { get; set; }
    }
}