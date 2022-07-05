using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.BenefitClaimServiceV2.Api.Messages
{ 
	[DataContract] 
	public class VEISfindBenefitClaimRequest : VEISEcRequestBase
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
		public string mcs_filenumber
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
	}
}
