using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VIAScheduling.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.VIAScheduling,makeAppointment method, Request.
	/// Code Generated by IMS on: 1/24/2019 5:43:22 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISVIASchdMakeApptmakeAppointmentRequest : VEISEcRequestBase
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
		public LegacyHeaderInfo LegacyServiceHeaderInfo { get; set; }
		[DataMember]
		public VEISVIASchdMakeApptReqqueryBean VEISVIASchdMakeApptReqqueryBeanInfo { get; set; }
		[DataMember]
		public VEISVIASchdMakeApptReqappointment VEISVIASchdMakeApptReqappointmentInfo { get; set; }
	}
	[DataContract]
	public class VEISVIASchdMakeApptReqqueryBean
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
		public VIMTVIASchdMakeApptReqprovider VIMTVIASchdMakeApptReqproviderInfo { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqpatient VIMTVIASchdMakeApptReqpatientInfo { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqprovider
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
	public class VIMTVIASchdMakeApptReqpatient
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
	[DataContract]
	public class VEISVIASchdMakeApptReqappointment
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
		public string mcs_appointmentTimestamp { get; set; }
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
		public string mcs_message { get; set; }
		[DataMember]
		public string mcs_cancelCode { get; set; }
		[DataMember]
		public string mcs_cancelReason { get; set; }
		[DataMember]
		public string mcs_remarks { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqfacility VIMTVIASchdMakeApptReqfacilityInfo { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqclinic VIMTVIASchdMakeApptReqclinicInfo { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqfacility
	{
		[DataMember]
		public string mcs_id { get; set; }
		[DataMember]
		public string mcs_lastEvent { get; set; }
		[DataMember]
		public string mcs_lastSeenDate { get; set; }
		[DataMember]
		public string mcs_name { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqclinic
	{
		[DataMember]
		public string mcs_abbr { get; set; }
		[DataMember]
		public string mcs_appointmentLength { get; set; }
		[DataMember]
		public string mcs_appointmentTimestamp { get; set; }
		[DataMember]
		public bool mcs_askForCheckIn { get; set; }
		[DataMember]
		public bool mcs_askForCheckInSpecified { get; set; }
		[DataMember]
		public string mcs_bed { get; set; }
		[DataMember]
		public string mcs_building { get; set; }
		[DataMember]
		public string mcs_displayIncrements { get; set; }
		[DataMember]
		public string mcs_dispositionAction { get; set; }
		[DataMember]
		public string mcs_floor { get; set; }
		[DataMember]
		public string mcs_id { get; set; }
		[DataMember]
		public string mcs_maxOverbooksPerDay { get; set; }
		[DataMember]
		public string mcs_name { get; set; }
		[DataMember]
		public string mcs_phone { get; set; }
		[DataMember]
		public string mcs_physicalLocation { get; set; }
		[DataMember]
		public string mcs_prohibitAccessToClinic { get; set; }
		[DataMember]
		public string mcs_room { get; set; }
		[DataMember]
		public string mcs_status { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
		[DataMember]
		public string mcs_visitLocation { get; set; }
		[DataMember]
		public string mcs_siteId { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqavailabilityMultipleResponse[] VIMTVIASchdMakeApptReqavailabilityInfo { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqcreditStopCode VIMTVIASchdMakeApptReqcreditStopCodeInfo { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqdepartment VIMTVIASchdMakeApptReqdepartmentInfo { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqdivision VIMTVIASchdMakeApptReqdivisionInfo { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqfacility1 VIMTVIASchdMakeApptReqfacility1Info { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqinstitution VIMTVIASchdMakeApptReqinstitutionInfo { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqmodule VIMTVIASchdMakeApptReqmoduleInfo { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqprincipalClinic VIMTVIASchdMakeApptReqprincipalClinicInfo { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqservice VIMTVIASchdMakeApptReqserviceInfo { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqspecialty VIMTVIASchdMakeApptReqspecialtyInfo { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqstopCode VIMTVIASchdMakeApptReqstopCodeInfo { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqtypeExtension VIMTVIASchdMakeApptReqtypeExtensionInfo { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqwardLocation VIMTVIASchdMakeApptReqwardLocationInfo { get; set; }
		[DataMember]
		public VIMTVIASchdMakeApptReqfault VIMTVIASchdMakeApptReqfaultInfo { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqavailabilityMultipleResponse
	{
		[DataMember]
		public bool mcs_available { get; set; }
		[DataMember]
		public string mcs_end { get; set; }
		[DataMember]
		public string mcs_start { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqcreditStopCode
	{
		[DataMember]
		public Object mcs_key { get; set; }
		[DataMember]
		public Object mcs_value { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqdepartment
	{
		[DataMember]
		public Object mcs_key { get; set; }
		[DataMember]
		public Object mcs_value { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqdivision
	{
		[DataMember]
		public Object mcs_key { get; set; }
		[DataMember]
		public Object mcs_value { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqfacility1
	{
		[DataMember]
		public string mcs_id { get; set; }
		[DataMember]
		public string mcs_lastEvent { get; set; }
		[DataMember]
		public string mcs_lastSeenDate { get; set; }
		[DataMember]
		public string mcs_name { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqinstitution
	{
		[DataMember]
		public Object mcs_key { get; set; }
		[DataMember]
		public Object mcs_value { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqmodule
	{
		[DataMember]
		public Object mcs_key { get; set; }
		[DataMember]
		public Object mcs_value { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqprincipalClinic
	{
		[DataMember]
		public Object mcs_key { get; set; }
		[DataMember]
		public Object mcs_value { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqservice
	{
		[DataMember]
		public Object mcs_key { get; set; }
		[DataMember]
		public Object mcs_value { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqspecialty
	{
		[DataMember]
		public Object mcs_key { get; set; }
		[DataMember]
		public Object mcs_value { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqstopCode
	{
		[DataMember]
		public Object mcs_key { get; set; }
		[DataMember]
		public Object mcs_value { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqtypeExtension
	{
		[DataMember]
		public Object mcs_key { get; set; }
		[DataMember]
		public Object mcs_value { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqwardLocation
	{
		[DataMember]
		public Object mcs_key { get; set; }
		[DataMember]
		public Object mcs_value { get; set; }
	}
	[DataContract]
	public class VIMTVIASchdMakeApptReqfault
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
