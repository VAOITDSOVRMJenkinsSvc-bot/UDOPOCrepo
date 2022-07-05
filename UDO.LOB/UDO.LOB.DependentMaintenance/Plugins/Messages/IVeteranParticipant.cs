﻿using System.Runtime.Serialization;

namespace VRM.Integration.Servicebus.AddDependent.Messages
{
    public interface IVeteranParticipant : IParticipant
    {
        [DataMember]
        long CorpParticipantId { get; set; }
		
		[DataMember]
        string MaritalStatus { get; set; }
		
		[DataMember]
        string EverMarriedInd { get; set; }

        [DataMember]
        string AllowPoaAccess { get; set; }

        [DataMember]
        string AllowPoaCadd { get; set; }
    }
}