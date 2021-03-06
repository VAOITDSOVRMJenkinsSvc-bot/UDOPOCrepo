using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;


namespace VEIS.VideoVistService.Api.Messages
{
    [DataContract]
    public class EcTmpCreateAppointmentResponse : VEISEcResponseBase
    {
        public EcTmpCreateAppointmentResponse()
        {
            MessageId = Guid.NewGuid().ToString();
        }
         

        [DataMember]
        public string HttpStatusCode { get; set; }

        [DataMember]
        public EcTmpWriteResults EcTmpWriteResults { get; set; }
        
        [DataMember]
        public bool ExceptionOccured { get; set; }
         
    }
}