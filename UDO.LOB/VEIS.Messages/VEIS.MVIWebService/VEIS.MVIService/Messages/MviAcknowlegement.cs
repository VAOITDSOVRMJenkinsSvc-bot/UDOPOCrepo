using System.Runtime.Serialization;

namespace VEIS.Mvi.Messages
{
    [DataContract]
    public class Acknowledgement
    {
        [DataMember]
        public string TypeCode { get; set; }

        [DataMember]
        public string TargetMessage { get; set; }

        [DataMember]
        public AcknowledgementDetail[] AcknowledgementDetails { get; set; }
    }

    [DataContract]
    public class AcknowledgementDetail
    {
        [DataMember]
        public AcknowledgementDetailCode Code { get; set; }

        [DataMember]
        public string Text { get; set; }
    }

    [DataContract]
    public class AcknowledgementDetailCode
    {
        [DataMember]
        public string CodeSystemName { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string DisplayName { get; set; }
    }

    [DataContract]
    public class QueryAcknowledgement
    {
        [DataMember]
        public string QueryResponseCode { get; set; }

        [DataMember]
        public string ResultCurrentQuantity { get; set; }
    }
}
