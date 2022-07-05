using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.BenefitClaimServiceV2.Api.Messages
{ 
	[DataContract] 
	public class VEISfindBenefitClaimResponse : VEISEcResponseBase
    { 

		[DataMember]
		public bool ExceptionOccured
		{
			get;
			set;
		}

		[DataMember]
		public VEISbenefitClaimRecordBCS2 VEISbenefitClaimRecordBCS2Info
		{
			get;
			set;
		}
         
	}
}
