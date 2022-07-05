using System; 
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.BenefitClaimServiceV2.Api.Messages
{ 
	[DataContract] 
	public class VEISclearBenefitClaimRequest : VEISEcRequestBase
    {
		[DataMember]
		public bool Debug
		{
			get;
			set;
		}

		[DataMember]
		public LegacyHeaderInfo LegacyServiceHeaderInfo
		{
			get;
			set;
		}

		[DataMember]
		public bool LogSoap
		{
			get;
			set;
		}

		[DataMember]
		public bool LogTiming
		{
			get;
			set;
		} 

		[DataMember]
		public string RelatedParentEntityName
		{
			get;
			set;
		}

		[DataMember]
		public string RelatedParentFieldName
		{
			get;
			set;
		}

		[DataMember]
		public Guid RelatedParentId
		{
			get;
			set;
		} 

		[DataMember]
		public VEISReqclearBenefitClaimInputBCS2 VEISReqclearBenefitClaimInputBCS2Info
		{
			get;
			set;
		}
    }
    [DataContract]
    public class VEISReqclearBenefitClaimInputBCS2
    {
        [DataMember]
        public string mcs_fileNumber { get; set; }
        [DataMember]
        public string mcs_payeeCode { get; set; }
        [DataMember]
        public string mcs_benefitClaimType { get; set; }
        [DataMember]
        public string mcs_endProductCode { get; set; }
        [DataMember]
        public string mcs_incremental { get; set; }
        [DataMember]
        public string mcs_pclrReasonCode { get; set; }
        [DataMember]
        public string mcs_pclrReasonText { get; set; }
        [DataMember]
        public string mcs_bypassIndicator { get; set; }
    }

}
