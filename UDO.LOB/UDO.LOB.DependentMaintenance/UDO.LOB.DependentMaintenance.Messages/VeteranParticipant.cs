using System.Runtime.Serialization;

namespace UDO.LOB.DependentMaintenance.Messages
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