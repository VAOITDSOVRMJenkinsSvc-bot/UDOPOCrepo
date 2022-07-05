using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;


namespace VEIS.VideoVistService.Api.Messages
{
    [DataContract(Name = "Appointment", Namespace = "gov.va.vamf.videoconnect/1.0")]
    public class EcTmpCancelAppointmentRequest : VEISEcRequestBase
    {
        public EcTmpCancelAppointmentRequest()
        {
            MessageId = Guid.NewGuid().ToString();
        }
         

        [DataMember]
        public Guid RelatedParentId { get; set; }

        [DataMember]
        public string RelatedParentEntityName { get; set; }

        [DataMember]
        public string RelatedParentFieldName { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string SourceSystem { get; set; }

        [DataMember]
        public EcTmpPersonBookingStatuses PatientBookingStatuses { get; set; }

        [DataMember]
        public string SamlToken { get; set; }
        [DataMember]
        public LegacyHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
    }        

    [DataContract]
    public class EcTmpPersonBookingStatus
    {
        [DataMember]
        public EcTmpPersonIdentifier Id { get; set; }

        [DataMember]
        public EcTmpStatus Status { get; set; }
    }

    [DataContract]
    public class EcTmpPersonBookingStatuses
    {
        [DataMember]
        public EcTmpPersonBookingStatus[] PersonBookingStatus { get; set; }
    }
}