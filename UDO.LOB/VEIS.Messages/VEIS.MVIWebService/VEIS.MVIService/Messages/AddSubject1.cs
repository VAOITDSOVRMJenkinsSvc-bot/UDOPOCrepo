using System.Runtime.Serialization;

namespace VEIS.Mvi.Messages
{
    [DataContract]
    public class AddSubject1
    {
        [DataMember]
        public AddPatient Patient { get; set; }
    }
}
