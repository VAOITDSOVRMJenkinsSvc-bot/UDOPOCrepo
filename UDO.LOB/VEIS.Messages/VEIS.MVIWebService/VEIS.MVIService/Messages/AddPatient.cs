using System.Runtime.Serialization;
using VEIS.Mvi.Messages;

namespace VEIS.Mvi.Messages
{
    [DataContract]
    public class AddPatient
    {
        [DataMember]
        public string PatientId { get; set; }

        [DataMember]
        public string[] ConfidentialityCode { get; set; }

        [DataMember]
        public AddPatientPerson PatientPerson { get; set; }
    }
}
