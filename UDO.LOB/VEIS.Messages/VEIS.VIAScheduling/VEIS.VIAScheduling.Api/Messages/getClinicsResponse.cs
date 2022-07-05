using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VIAScheduling.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.VIAScheduling,getClinics method, Response.
	/// Code Generated by IMS on: 1/24/2019 5:38:31 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISVIASchGetClgetClinicsResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISVIASchGetCltaggedHospitalLocationArray VEISVIASchGetCltaggedHospitalLocationArrayInfo { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetCltaggedHospitalLocationArray
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public Int32 mcs_count { get; set; }
		[DataMember]
		public bool mcs_countSpecified { get; set; }
		[DataMember]
		public VEISVIASchGetCllocationsMultipleResponse[] VEISVIASchGetCllocationsInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClfault16 VEISVIASchGetClfault16Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetCllocationsMultipleResponse
	{
		[DataMember]
		public string mcs_id { get; set; }
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public string mcs_building { get; set; }
		[DataMember]
		public string mcs_floor { get; set; }
		[DataMember]
		public string mcs_room { get; set; }
		[DataMember]
		public string mcs_bed { get; set; }
		[DataMember]
		public string mcs_status { get; set; }
		[DataMember]
		public string mcs_phone { get; set; }
		[DataMember]
		public string mcs_appointmentTimestamp { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
		[DataMember]
		public string mcs_physicalLocation { get; set; }
		[DataMember]
		public bool mcs_askForCheckIn { get; set; }
		[DataMember]
		public bool mcs_askForCheckInSpecified { get; set; }
		[DataMember]
		public string mcs_appointmentLength { get; set; }
		[DataMember]
		public string mcs_clinicDisplayStartTime { get; set; }
		[DataMember]
		public string mcs_displayIncrements { get; set; }
		[DataMember]
		public string mcs_maxOverbooksPerDay { get; set; }
		[DataMember]
		public string mcs_prohibitAccessToClinic { get; set; }
		[DataMember]
		public VEISVIASchGetCldepartment VEISVIASchGetCldepartmentInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClservice VEISVIASchGetClserviceInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClspecialty VEISVIASchGetClspecialtyInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClstopCode VEISVIASchGetClstopCodeInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClcreditStopCode VEISVIASchGetClcreditStopCodeInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClfacility VEISVIASchGetClfacilityInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClavailability VEISVIASchGetClavailabilityInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClfault15 VEISVIASchGetClfault15Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetCldepartment
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchGetCltaggedResultsMultipleResponse[] VEISVIASchGetCltaggedResultsInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClfault VEISVIASchGetClfaultInfo { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetCltaggedResultsMultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClservice
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchGetCltaggedResults1MultipleResponse[] VEISVIASchGetCltaggedResults1Info { get; set; }
		[DataMember]
		public VEISVIASchGetClfault1 VEISVIASchGetClfault1Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetCltaggedResults1MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault1
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClspecialty
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchGetCltaggedResults2MultipleResponse[] VEISVIASchGetCltaggedResults2Info { get; set; }
		[DataMember]
		public VEISVIASchGetClfault2 VEISVIASchGetClfault2Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetCltaggedResults2MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault2
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClstopCode
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchGetCltaggedResults3MultipleResponse[] VEISVIASchGetCltaggedResults3Info { get; set; }
		[DataMember]
		public VEISVIASchGetClfault3 VEISVIASchGetClfault3Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetCltaggedResults3MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault3
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClcreditStopCode
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchGetCltaggedResults4MultipleResponse[] VEISVIASchGetCltaggedResults4Info { get; set; }
		[DataMember]
		public VEISVIASchGetClfault4 VEISVIASchGetClfault4Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetCltaggedResults4MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault4
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfacility
	{
		[DataMember]
		public string mcs_sitecode { get; set; }
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public string mcs_displayName { get; set; }
		[DataMember]
		public string mcs_moniker { get; set; }
		[DataMember]
		public string mcs_regionID { get; set; }
		[DataMember]
		public string mcs_lastEventTimestamp { get; set; }
		[DataMember]
		public string mcs_lastEventReason { get; set; }
		[DataMember]
		public string mcs_uid { get; set; }
		[DataMember]
		public string mcs_pid { get; set; }
		[DataMember]
		public string mcs_parentSiteId { get; set; }
		[DataMember]
		public string mcs_address { get; set; }
		[DataMember]
		public string mcs_city { get; set; }
		[DataMember]
		public string mcs_state { get; set; }
		[DataMember]
		public string mcs_systemName { get; set; }
		[DataMember]
		public string mcs_siteType { get; set; }
		[DataMember]
		public VEISVIASchGetCldataSources VEISVIASchGetCldataSourcesInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClchildSites VEISVIASchGetClchildSitesInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClfault12 VEISVIASchGetClfault12Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetCldataSources
	{
		[DataMember]
		public Int32 mcs_count { get; set; }
		[DataMember]
		public bool mcs_countSpecified { get; set; }
		[DataMember]
		public VEISVIASchGetClitemsMultipleResponse[] VEISVIASchGetClitemsInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClfault7 VEISVIASchGetClfault7Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClitemsMultipleResponse
	{
		[DataMember]
		public string mcs_protocol { get; set; }
		[DataMember]
		public string mcs_modality { get; set; }
		[DataMember]
		public Int32 mcs_timeout { get; set; }
		[DataMember]
		public Int32 mcs_port { get; set; }
		[DataMember]
		public string mcs_provider { get; set; }
		[DataMember]
		public string mcs_status { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
		[DataMember]
		public string mcs_context { get; set; }
		[DataMember]
		public bool mcs_testSource { get; set; }
		[DataMember]
		public string mcs_vendor { get; set; }
		[DataMember]
		public string mcs_version { get; set; }
		[DataMember]
		public string mcs_welcomeMessage { get; set; }
		[DataMember]
		public VEISVIASchGetClsiteId VEISVIASchGetClsiteIdInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClfault6 VEISVIASchGetClfault6Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClsiteId
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchGetCltaggedResults5MultipleResponse[] VEISVIASchGetCltaggedResults5Info { get; set; }
		[DataMember]
		public VEISVIASchGetClfault5 VEISVIASchGetClfault5Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetCltaggedResults5MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault5
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault6
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault7
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClchildSites
	{
		[DataMember]
		public Int32 mcs_count { get; set; }
		[DataMember]
		public bool mcs_countSpecified { get; set; }
		[DataMember]
		public VEISVIASchGetClsitesMultipleResponse[] VEISVIASchGetClsitesInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClfault11 VEISVIASchGetClfault11Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClsitesMultipleResponse
	{
		[DataMember]
		public string mcs_sitecode { get; set; }
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public string mcs_displayName { get; set; }
		[DataMember]
		public string mcs_moniker { get; set; }
		[DataMember]
		public string mcs_regionID { get; set; }
		[DataMember]
		public string mcs_lastEventTimestamp { get; set; }
		[DataMember]
		public string mcs_lastEventReason { get; set; }
		[DataMember]
		public string mcs_uid { get; set; }
		[DataMember]
		public string mcs_pid { get; set; }
		[DataMember]
		public string mcs_parentSiteId { get; set; }
		[DataMember]
		public string mcs_address { get; set; }
		[DataMember]
		public string mcs_city { get; set; }
		[DataMember]
		public string mcs_state { get; set; }
		[DataMember]
		public string mcs_systemName { get; set; }
		[DataMember]
		public string mcs_siteType { get; set; }
		[DataMember]
		public VEISVIASchGetCldataSources1 VEISVIASchGetCldataSources1Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetCldataSources1
	{
		[DataMember]
		public Int32 mcs_count { get; set; }
		[DataMember]
		public bool mcs_countSpecified { get; set; }
		[DataMember]
		public VEISVIASchGetClitems1MultipleResponse[] VEISVIASchGetClitems1Info { get; set; }
		[DataMember]
		public VEISVIASchGetClfault10 VEISVIASchGetClfault10Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClitems1MultipleResponse
	{
		[DataMember]
		public string mcs_protocol { get; set; }
		[DataMember]
		public string mcs_modality { get; set; }
		[DataMember]
		public Int32 mcs_timeout { get; set; }
		[DataMember]
		public Int32 mcs_port { get; set; }
		[DataMember]
		public string mcs_provider { get; set; }
		[DataMember]
		public string mcs_status { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
		[DataMember]
		public string mcs_context { get; set; }
		[DataMember]
		public bool mcs_testSource { get; set; }
		[DataMember]
		public string mcs_vendor { get; set; }
		[DataMember]
		public string mcs_version { get; set; }
		[DataMember]
		public string mcs_welcomeMessage { get; set; }
		[DataMember]
		public VEISVIASchGetClsiteId1 VEISVIASchGetClsiteId1Info { get; set; }
		[DataMember]
		public VEISVIASchGetClfault9 VEISVIASchGetClfault9Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClsiteId1
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchGetCltaggedResults6MultipleResponse[] VEISVIASchGetCltaggedResults6Info { get; set; }
		[DataMember]
		public VEISVIASchGetClfault8 VEISVIASchGetClfault8Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetCltaggedResults6MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault8
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault9
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault10
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault11
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault12
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClavailability
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public Int32 mcs_count { get; set; }
		[DataMember]
		public bool mcs_countSpecified { get; set; }
		[DataMember]
		public VEISVIASchGetClslotsMultipleResponse[] VEISVIASchGetClslotsInfo { get; set; }
		[DataMember]
		public VEISVIASchGetClfault14 VEISVIASchGetClfault14Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClslotsMultipleResponse
	{
		[DataMember]
		public string mcs_start { get; set; }
		[DataMember]
		public string mcs_end { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public bool mcs_available { get; set; }
		[DataMember]
		public VEISVIASchGetClfault13 VEISVIASchGetClfault13Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault13
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault14
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault15
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISVIASchGetClfault16
	{
		[DataMember]
		public string mcs_innerMessage { get; set; }
		[DataMember]
		public string mcs_innerStackTrace { get; set; }
		[DataMember]
		public string mcs_innerType { get; set; }
		[DataMember]
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_stackTrace { get; set; }
		[DataMember]
		public string mcs_suggestion { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
}
