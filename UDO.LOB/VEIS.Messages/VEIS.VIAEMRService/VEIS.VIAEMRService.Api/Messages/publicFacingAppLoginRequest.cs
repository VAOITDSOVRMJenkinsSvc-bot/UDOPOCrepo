using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VIAEMRService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.VIAEMRService,publicFacingAppLogin method, Request.
	/// Code Generated by IMS on: 12/28/2018 9:37:43 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISEMRpfalpublicFacingAppLoginRequest : VEISEcRequestBase
	{
		[DataMember]
		public Guid RelatedParentId { get; set; }
		[DataMember]
		public string RelatedParentEntityName { get; set; }
		[DataMember]
		public string RelatedParentFieldName { get; set; }
		[DataMember]
		public bool LogTiming { get; set; }
		[DataMember]
		public bool LogSoap { get; set; }
		[DataMember]
		public bool Debug { get; set; }
		[DataMember]
		public string mcs_sitecode { get; set; }
		[DataMember]
		public string mcs_veteranid { get; set; }
		[DataMember]
		public VEISEMRpfalReqqueryBean VEISEMRpfalReqqueryBeanInfo { get; set; }
	}
	[DataContract]
	public class VEISEMRpfalReqqueryBean
	{
		[DataMember]
		public string mcs_recordSiteCode { get; set; }
		[DataMember]
		public string mcs_requestingApp { get; set; }
		[DataMember]
		public string mcs_active { get; set; }
		[DataMember]
		public string mcs_endDate { get; set; }
		[DataMember]
		public string mcs_startDate { get; set; }
		[DataMember]
		public string mcs_maxRecords { get; set; }
		[DataMember]
		public string mcs_status { get; set; }
		[DataMember]
		public string mcs_supplementalParameters { get; set; }
		[DataMember]
		public string mcs_itemId { get; set; }
		[DataMember]
		public string mcs_target { get; set; }
		[DataMember]
		public string mcs_criteria { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
		[DataMember]
		public string mcs_direction { get; set; }
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public bool mcs_multiSiteQuery { get; set; }
		[DataMember]
		public string mcs_consumingAppToken { get; set; }
		[DataMember]
		public string mcs_consumingAppPassword { get; set; }
		[DataMember]
		public VIMTEMRpfalReqprovider VIMTEMRpfalReqproviderInfo { get; set; }
		[DataMember]
		public VIMTEMRpfalReqpatient VIMTEMRpfalReqpatientInfo { get; set; }
	}
	[DataContract]
	public class VIMTEMRpfalReqprovider
	{
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public string mcs_userId { get; set; }
		[DataMember]
		public string mcs_loginSiteCode { get; set; }
		[DataMember]
		public string mcs_contextMenu { get; set; }
		[DataMember]
		public string mcs_vistaLocations { get; set; }
	}
	[DataContract]
	public class VIMTEMRpfalReqpatient
	{
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public string mcs_localPid { get; set; }
		[DataMember]
		public string mcs_localSiteId { get; set; }
		[DataMember]
		public string mcs_mpiPid { get; set; }
		[DataMember]
		public string mcs_vistaLocations { get; set; }
		[DataMember]
		public string mcs_ssn { get; set; }
		[DataMember]
		public string mcs_inPatient { get; set; }
		[DataMember]
		public string mcs_age { get; set; }
		[DataMember]
		public string mcs_gender { get; set; }
		[DataMember]
		public string mcs_scPercentage { get; set; }
	}
}
