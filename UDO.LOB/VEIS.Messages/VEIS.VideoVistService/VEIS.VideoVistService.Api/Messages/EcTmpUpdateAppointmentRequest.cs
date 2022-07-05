using VEIS.VideoVistService.Api.Messages;
using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VideoVistService.Api.Messages
{
    [DataContract]
    public class EcTmpUpdateAppointmentRequest : VEISEcRequestBase
    {
        public EcTmpUpdateAppointmentRequest()
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
        public EcTmpPatients[] Patients { get; set; }

        [DataMember]
        public int Duration { get; set; }

        [DataMember]
        public string DateTime { get; set; }

        [DataMember]
        public EcTmpStatus Status { get; set; }

        [DataMember]
        public EcTmpSchedulingRequestType SchedulingRequestType { get; set; }

        [DataMember]
        public bool SchedulingRequestTypeSpecified { get; set; }

        [DataMember]
        public EcTmpAppointmentKind AppointmentKind { get; set; }

        [DataMember]
        public EcTmpAppointmentType Type { get; set; }

        [DataMember]
        public bool TypeSpecified { get; set; }

        [DataMember]
        public string BookingNotes { get; set; }

        [DataMember]
        public string DesiredDate { get; set; }

        [DataMember]
        public bool DesiredDateSpecified { get; set; }

        [DataMember]
        public EcTmpProviders[] Providers { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string SamlToken { get; set; }
        [DataMember]
        public LegacyHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
    }
}