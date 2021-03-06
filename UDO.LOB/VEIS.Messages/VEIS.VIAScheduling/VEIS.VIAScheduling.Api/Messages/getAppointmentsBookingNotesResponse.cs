using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VIAScheduling.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.VIAScheduling,getAppointmentsBookingNotes method, Response.
	/// Code Generated by IMS on: 1/24/2019 5:33:24 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISVIASchedgabngetAppointmentsBookingNotesResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISVIASchedgabntaggedAppointmentArrays VEISVIASchedgabntaggedAppointmentArraysInfo { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabntaggedAppointmentArrays
	{
		[DataMember]
		public Int32 mcs_count { get; set; }
		[DataMember]
		public bool mcs_countSpecified { get; set; }
		[DataMember]
		public VEISVIASchedgabnarraysMultipleResponse[] VEISVIASchedgabnarraysInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault19 VEISVIASchedgabnfault19Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnarraysMultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public Int32 mcs_count { get; set; }
		[DataMember]
		public bool mcs_countSpecified { get; set; }
		[DataMember]
		public VEISVIASchedgabnapptsMultipleResponse[] VEISVIASchedgabnapptsInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault18 VEISVIASchedgabnfault18Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnapptsMultipleResponse
	{
		[DataMember]
		public string mcs_id { get; set; }
		[DataMember]
		public string mcs_timestamp { get; set; }
		[DataMember]
		public string mcs_title { get; set; }
		[DataMember]
		public string mcs_status { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public string mcs_labDateTime { get; set; }
		[DataMember]
		public string mcs_xrayDateTime { get; set; }
		[DataMember]
		public string mcs_ekgDateTime { get; set; }
		[DataMember]
		public string mcs_purpose { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
		[DataMember]
		public string mcs_currentStatus { get; set; }
		[DataMember]
		public string mcs_clinicId { get; set; }
		[DataMember]
		public string mcs_purposeSubcategory { get; set; }
		[DataMember]
		public string mcs_appointmentLength { get; set; }
		[DataMember]
		public string mcs_appointmentType { get; set; }
		[DataMember]
		public string mcs_bookNotes { get; set; }
		[DataMember]
		public string mcs_consultIen { get; set; }
		[DataMember]
		public string mcs_lvl { get; set; }
		[DataMember]
		public string mcs_desiredDateTime { get; set; }
		[DataMember]
		public string mcs_visitId { get; set; }
		[DataMember]
		public string mcs_providerName { get; set; }
		[DataMember]
		public VEISVIASchedgabnfacility VEISVIASchedgabnfacilityInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnclinic VEISVIASchedgabnclinicInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault17 VEISVIASchedgabnfault17Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnfacility
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchedgabntaggedResultsMultipleResponse[] VEISVIASchedgabntaggedResultsInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault VEISVIASchedgabnfaultInfo { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabntaggedResultsMultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnfault
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
	public class VEISVIASchedgabnclinic
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
		public VEISVIASchedgabndepartment VEISVIASchedgabndepartmentInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnservice VEISVIASchedgabnserviceInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnspecialty VEISVIASchedgabnspecialtyInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnstopCode VEISVIASchedgabnstopCodeInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabncreditStopCode VEISVIASchedgabncreditStopCodeInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnfacility1 VEISVIASchedgabnfacility1Info { get; set; }
		[DataMember]
		public VEISVIASchedgabnavailability VEISVIASchedgabnavailabilityInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault16 VEISVIASchedgabnfault16Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabndepartment
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchedgabntaggedResults1MultipleResponse[] VEISVIASchedgabntaggedResults1Info { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault1 VEISVIASchedgabnfault1Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabntaggedResults1MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnfault1
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
	public class VEISVIASchedgabnservice
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchedgabntaggedResults2MultipleResponse[] VEISVIASchedgabntaggedResults2Info { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault2 VEISVIASchedgabnfault2Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabntaggedResults2MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnfault2
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
	public class VEISVIASchedgabnspecialty
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchedgabntaggedResults3MultipleResponse[] VEISVIASchedgabntaggedResults3Info { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault3 VEISVIASchedgabnfault3Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabntaggedResults3MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnfault3
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
	public class VEISVIASchedgabnstopCode
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchedgabntaggedResults4MultipleResponse[] VEISVIASchedgabntaggedResults4Info { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault4 VEISVIASchedgabnfault4Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabntaggedResults4MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnfault4
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
	public class VEISVIASchedgabncreditStopCode
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchedgabntaggedResults5MultipleResponse[] VEISVIASchedgabntaggedResults5Info { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault5 VEISVIASchedgabnfault5Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabntaggedResults5MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnfault5
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
	public class VEISVIASchedgabnfacility1
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
		public VEISVIASchedgabndataSources VEISVIASchedgabndataSourcesInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnchildSites VEISVIASchedgabnchildSitesInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault13 VEISVIASchedgabnfault13Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabndataSources
	{
		[DataMember]
		public Int32 mcs_count { get; set; }
		[DataMember]
		public bool mcs_countSpecified { get; set; }
		[DataMember]
		public VEISVIASchedgabnitemsMultipleResponse[] VEISVIASchedgabnitemsInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault8 VEISVIASchedgabnfault8Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnitemsMultipleResponse
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
		public VEISVIASchedgabnsiteId VEISVIASchedgabnsiteIdInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault7 VEISVIASchedgabnfault7Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnsiteId
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchedgabntaggedResults6MultipleResponse[] VEISVIASchedgabntaggedResults6Info { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault6 VEISVIASchedgabnfault6Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabntaggedResults6MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnfault6
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
	public class VEISVIASchedgabnfault7
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
	public class VEISVIASchedgabnfault8
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
	public class VEISVIASchedgabnchildSites
	{
		[DataMember]
		public Int32 mcs_count { get; set; }
		[DataMember]
		public bool mcs_countSpecified { get; set; }
		[DataMember]
		public VEISVIASchedgabnsitesMultipleResponse[] VEISVIASchedgabnsitesInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault12 VEISVIASchedgabnfault12Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnsitesMultipleResponse
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
		public VEISVIASchedgabndataSources1 VEISVIASchedgabndataSources1Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabndataSources1
	{
		[DataMember]
		public Int32 mcs_count { get; set; }
		[DataMember]
		public bool mcs_countSpecified { get; set; }
		[DataMember]
		public VEISVIASchedgabnitems1MultipleResponse[] VEISVIASchedgabnitems1Info { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault11 VEISVIASchedgabnfault11Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnitems1MultipleResponse
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
		public VEISVIASchedgabnsiteId1 VEISVIASchedgabnsiteId1Info { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault10 VEISVIASchedgabnfault10Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnsiteId1
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISVIASchedgabntaggedResults7MultipleResponse[] VEISVIASchedgabntaggedResults7Info { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault9 VEISVIASchedgabnfault9Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabntaggedResults7MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnfault9
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
	public class VEISVIASchedgabnfault10
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
	public class VEISVIASchedgabnfault11
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
	public class VEISVIASchedgabnfault12
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
	public class VEISVIASchedgabnfault13
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
	public class VEISVIASchedgabnavailability
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public Int32 mcs_count { get; set; }
		[DataMember]
		public bool mcs_countSpecified { get; set; }
		[DataMember]
		public VEISVIASchedgabnslotsMultipleResponse[] VEISVIASchedgabnslotsInfo { get; set; }
		[DataMember]
		public VEISVIASchedgabnfault15 VEISVIASchedgabnfault15Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnslotsMultipleResponse
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
		public VEISVIASchedgabnfault14 VEISVIASchedgabnfault14Info { get; set; }
	}
	[DataContract]
	public class VEISVIASchedgabnfault14
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
	public class VEISVIASchedgabnfault15
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
	public class VEISVIASchedgabnfault16
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
	public class VEISVIASchedgabnfault17
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
	public class VEISVIASchedgabnfault18
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
	public class VEISVIASchedgabnfault19
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
