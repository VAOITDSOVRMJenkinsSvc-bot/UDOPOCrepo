using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.ClaimManagementService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.ClaimManagementService,findClaimLevelSuspense method, Response.
	/// Code Generated by IMS on: 12/19/2018 10:47:20 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISfclmlvlSuspfindClaimLevelSuspenseResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISfclmlvlSuspfindClaimLevelSuspenseResponseData VEISfclmlvlSuspfindClaimLevelSuspenseResponseDataInfo { get; set; }
	}
	[DataContract]
	public class VEISfclmlvlSuspfindClaimLevelSuspenseResponseData
	{
		[DataMember]
		public VEISfclmlvlSuspClaimLevelSuspense VEISfclmlvlSuspClaimLevelSuspenseInfo { get; set; }
	}
	[DataContract]
	public class VEISfclmlvlSuspClaimLevelSuspense
	{
		[DataMember]
		public VEISfclmlvlSuspbenefitClaim VEISfclmlvlSuspbenefitClaimInfo { get; set; }
		[DataMember]
		public VEISfclmlvlSuspsend VEISfclmlvlSuspsendInfo { get; set; }
		[DataMember]
		public VEISfclmlvlSuspvnErrorsListMultipleResponse[] VEISfclmlvlSuspvnErrorsListInfo { get; set; }
	}
	[DataContract]
	public class VEISfclmlvlSuspbenefitClaim
	{
		[DataMember]
		public VEISfclmlvlSuspcontentionsMultipleResponse[] VEISfclmlvlSuspcontentionsInfo { get; set; }
		[DataMember]
		public VEISfclmlvlSuspdvlpmtItemsMultipleResponse[] VEISfclmlvlSuspdvlpmtItemsInfo { get; set; }
		[DataMember]
		public VEISfclmlvlSusplettersMultipleResponse[] VEISfclmlvlSusplettersInfo { get; set; }
		[DataMember]
		public VEISfclmlvlSuspstationProfile VEISfclmlvlSuspstationProfileInfo { get; set; }
		[DataMember]
		public VEISfclmlvlSuspstnSuspnsPrfilMultipleResponse[] VEISfclmlvlSuspstnSuspnsPrfilInfo { get; set; }
	}
	[DataContract]
	public class VEISfclmlvlSuspcontentionsMultipleResponse
	{
		[DataMember]
		public VEISfclmlvlSuspspecialIssuesMultipleResponse[] VEISfclmlvlSuspspecialIssuesInfo { get; set; }
	}
	[DataContract]
	public class VEISfclmlvlSuspspecialIssuesMultipleResponse
	{
	}
	[DataContract]
	public class VEISfclmlvlSuspdvlpmtItemsMultipleResponse
	{
		[DataMember]
		public VEISfclmlvlSuspdevlItemOutArgValueMultipleResponse[] VEISfclmlvlSuspdevlItemOutArgValueInfo { get; set; }
	}
	[DataContract]
	public class VEISfclmlvlSuspdevlItemOutArgValueMultipleResponse
	{
	}
	[DataContract]
	public class VEISfclmlvlSusplettersMultipleResponse
	{
		[DataMember]
		public VEISfclmlvlSuspcntctProfile VEISfclmlvlSuspcntctProfileInfo { get; set; }
		[DataMember]
		public VEISfclmlvlSuspdevelopmentActionsMultipleResponse[] VEISfclmlvlSuspdevelopmentActionsInfo { get; set; }
		[DataMember]
		public VEISfclmlvlSuspenclosuresMultipleResponse[] VEISfclmlvlSuspenclosuresInfo { get; set; }
		[DataMember]
		public VEISfclmlvlSuspletterRelationshipsMultipleResponse[] VEISfclmlvlSuspletterRelationshipsInfo { get; set; }
	}
	[DataContract]
	public class VEISfclmlvlSuspcntctProfile
	{
	}
	[DataContract]
	public class VEISfclmlvlSuspdevelopmentActionsMultipleResponse
	{
		[DataMember]
		public VEISfclmlvlSuspcustomParagraphsMultipleResponse[] VEISfclmlvlSuspcustomParagraphsInfo { get; set; }
		[DataMember]
		public VEISfclmlvlSuspdevelopmentItemsMultipleResponse[] VEISfclmlvlSuspdevelopmentItemsInfo { get; set; }
		[DataMember]
		public VEISfclmlvlSuspoutArgValuesMultipleResponse[] VEISfclmlvlSuspoutArgValuesInfo { get; set; }
	}
	[DataContract]
	public class VEISfclmlvlSuspcustomParagraphsMultipleResponse
	{
	}
	[DataContract]
	public class VEISfclmlvlSuspdevelopmentItemsMultipleResponse
	{
	}
	[DataContract]
	public class VEISfclmlvlSuspoutArgValuesMultipleResponse
	{
	}
	[DataContract]
	public class VEISfclmlvlSuspenclosuresMultipleResponse
	{
	}
	[DataContract]
	public class VEISfclmlvlSuspletterRelationshipsMultipleResponse
	{
	}
	[DataContract]
	public class VEISfclmlvlSuspstationProfile
	{
	}
	[DataContract]
	public class VEISfclmlvlSuspstnSuspnsPrfilMultipleResponse
	{
	}
	[DataContract]
	public class VEISfclmlvlSuspsend
	{
	}
	[DataContract]
	public class VEISfclmlvlSuspvnErrorsListMultipleResponse
	{
	}
}