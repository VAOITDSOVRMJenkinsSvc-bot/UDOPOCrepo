using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VIAEMRService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.VIAEMRService,getAppointments method, Response.
	/// Code Generated by IMS on: 12/28/2018 8:56:13 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISEMRgagetAppointmentsResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISEMRgataggedAppointmentArrays VEISEMRgataggedAppointmentArraysInfo { get; set; }
	}
	[DataContract]
	public class VEISEMRgataggedAppointmentArrays
	{
		[DataMember]
		public Int32 mcs_count { get; set; }
		[DataMember]
		public bool mcs_countSpecified { get; set; }
		[DataMember]
		public VEISEMRgaarraysMultipleResponse[] VEISEMRgaarraysInfo { get; set; }
		[DataMember]
		public VEISEMRgafault19 VEISEMRgafault19Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgaarraysMultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public Int32 mcs_count { get; set; }
		[DataMember]
		public bool mcs_countSpecified { get; set; }
		[DataMember]
		public VEISEMRgaapptsMultipleResponse[] VEISEMRgaapptsInfo { get; set; }
		[DataMember]
		public VEISEMRgafault18 VEISEMRgafault18Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgaapptsMultipleResponse
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
		public VEISEMRgafacility VEISEMRgafacilityInfo { get; set; }
		[DataMember]
		public VEISEMRgaclinic VEISEMRgaclinicInfo { get; set; }
		[DataMember]
		public VEISEMRgafault17 VEISEMRgafault17Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgafacility
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISEMRgataggedResultsMultipleResponse[] VEISEMRgataggedResultsInfo { get; set; }
		[DataMember]
		public VEISEMRgafault VEISEMRgafaultInfo { get; set; }
	}
	[DataContract]
	public class VEISEMRgataggedResultsMultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISEMRgafault
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
	public class VEISEMRgaclinic
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
		public VEISEMRgadepartment VEISEMRgadepartmentInfo { get; set; }
		[DataMember]
		public VEISEMRgaservice VEISEMRgaserviceInfo { get; set; }
		[DataMember]
		public VEISEMRgaspecialty VEISEMRgaspecialtyInfo { get; set; }
		[DataMember]
		public VEISEMRgastopCode VEISEMRgastopCodeInfo { get; set; }
		[DataMember]
		public VEISEMRgacreditStopCode VEISEMRgacreditStopCodeInfo { get; set; }
		[DataMember]
		public VEISEMRgafacility1 VEISEMRgafacility1Info { get; set; }
		[DataMember]
		public VEISEMRgaavailability VEISEMRgaavailabilityInfo { get; set; }
		[DataMember]
		public VEISEMRgafault16 VEISEMRgafault16Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgadepartment
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISEMRgataggedResults1MultipleResponse[] VEISEMRgataggedResults1Info { get; set; }
		[DataMember]
		public VEISEMRgafault1 VEISEMRgafault1Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgataggedResults1MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISEMRgafault1
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
	public class VEISEMRgaservice
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISEMRgataggedResults2MultipleResponse[] VEISEMRgataggedResults2Info { get; set; }
		[DataMember]
		public VEISEMRgafault2 VEISEMRgafault2Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgataggedResults2MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISEMRgafault2
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
	public class VEISEMRgaspecialty
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
		[DataMember]
		public VEISEMRgataggedResults3MultipleResponse[] VEISEMRgataggedResults3Info { get; set; }
		[DataMember]
		public VEISEMRgafault3 VEISEMRgafault3Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgataggedResults3MultipleResponse
	{
		[DataMember]
		public string mcs_tag { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public String[] mcs_textArray { get; set; }
	}
	[DataContract]
	public class VEISEMRgafault3
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
	public class VEISEMRgastopCode
	{
		[DataMember]
		public VEISEMRgataggedResults4MultipleResponse[] VEISEMRgataggedResults4Info { get; set; }
		[DataMember]
		public VEISEMRgafault4 VEISEMRgafault4Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgataggedResults4MultipleResponse
	{
	}
	[DataContract]
	public class VEISEMRgafault4
	{
	}
	[DataContract]
	public class VEISEMRgacreditStopCode
	{
		[DataMember]
		public VEISEMRgataggedResults5MultipleResponse[] VEISEMRgataggedResults5Info { get; set; }
		[DataMember]
		public VEISEMRgafault5 VEISEMRgafault5Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgataggedResults5MultipleResponse
	{
	}
	[DataContract]
	public class VEISEMRgafault5
	{
	}
	[DataContract]
	public class VEISEMRgafacility1
	{
		[DataMember]
		public VEISEMRgadataSources VEISEMRgadataSourcesInfo { get; set; }
		[DataMember]
		public VEISEMRgachildSites VEISEMRgachildSitesInfo { get; set; }
		[DataMember]
		public VEISEMRgafault13 VEISEMRgafault13Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgadataSources
	{
		[DataMember]
		public VEISEMRgaitemsMultipleResponse[] VEISEMRgaitemsInfo { get; set; }
		[DataMember]
		public VEISEMRgafault8 VEISEMRgafault8Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgaitemsMultipleResponse
	{
		[DataMember]
		public VEISEMRgasiteId VEISEMRgasiteIdInfo { get; set; }
		[DataMember]
		public VEISEMRgafault7 VEISEMRgafault7Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgasiteId
	{
		[DataMember]
		public VEISEMRgataggedResults6MultipleResponse[] VEISEMRgataggedResults6Info { get; set; }
		[DataMember]
		public VEISEMRgafault6 VEISEMRgafault6Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgataggedResults6MultipleResponse
	{
	}
	[DataContract]
	public class VEISEMRgafault6
	{
	}
	[DataContract]
	public class VEISEMRgafault7
	{
	}
	[DataContract]
	public class VEISEMRgafault8
	{
	}
	[DataContract]
	public class VEISEMRgachildSites
	{
		[DataMember]
		public VEISEMRgasitesMultipleResponse[] VEISEMRgasitesInfo { get; set; }
		[DataMember]
		public VEISEMRgafault12 VEISEMRgafault12Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgasitesMultipleResponse
	{
		[DataMember]
		public VEISEMRgadataSources1 VEISEMRgadataSources1Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgadataSources1
	{
		[DataMember]
		public VEISEMRgaitems1MultipleResponse[] VEISEMRgaitems1Info { get; set; }
		[DataMember]
		public VEISEMRgafault11 VEISEMRgafault11Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgaitems1MultipleResponse
	{
		[DataMember]
		public VEISEMRgasiteId1 VEISEMRgasiteId1Info { get; set; }
		[DataMember]
		public VEISEMRgafault10 VEISEMRgafault10Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgasiteId1
	{
		[DataMember]
		public VEISEMRgataggedResults7MultipleResponse[] VEISEMRgataggedResults7Info { get; set; }
		[DataMember]
		public VEISEMRgafault9 VEISEMRgafault9Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgataggedResults7MultipleResponse
	{
	}
	[DataContract]
	public class VEISEMRgafault9
	{
	}
	[DataContract]
	public class VEISEMRgafault10
	{
	}
	[DataContract]
	public class VEISEMRgafault11
	{
	}
	[DataContract]
	public class VEISEMRgafault12
	{
	}
	[DataContract]
	public class VEISEMRgafault13
	{
	}
	[DataContract]
	public class VEISEMRgaavailability
	{
		[DataMember]
		public VEISEMRgaslotsMultipleResponse[] VEISEMRgaslotsInfo { get; set; }
		[DataMember]
		public VEISEMRgafault15 VEISEMRgafault15Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgaslotsMultipleResponse
	{
		[DataMember]
		public VEISEMRgafault14 VEISEMRgafault14Info { get; set; }
	}
	[DataContract]
	public class VEISEMRgafault14
	{
	}
	[DataContract]
	public class VEISEMRgafault15
	{
	}
	[DataContract]
	public class VEISEMRgafault16
	{
	}
	[DataContract]
	public class VEISEMRgafault17
	{
	}
	[DataContract]
	public class VEISEMRgafault18
	{
	}
	[DataContract]
	public class VEISEMRgafault19
	{
	}
}
