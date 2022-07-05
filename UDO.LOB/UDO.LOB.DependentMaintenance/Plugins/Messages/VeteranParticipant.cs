using System.Runtime.Serialization;

namespace VRM.Integration.Servicebus.AddDependent.Messages
{
    [DataContract]
    public class VeteranParticipant : Participant, IVeteranParticipant
    {
        [DataMember]
        public long CorpParticipantId { get; set; }

		[DataMember]
        public string MaritalStatus { get; set; }
		
		[DataMember]
        public string EverMarriedInd { get; set; }

        [DataMember]
        public string AllowPoaAccess { get; set; }

        [DataMember]
        public string AllowPoaCadd { get; set; }
    }
}