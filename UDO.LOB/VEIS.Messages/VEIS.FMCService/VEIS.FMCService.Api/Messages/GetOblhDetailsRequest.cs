using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.FMCService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.FMCService,GetOblhDetails method, Request.
	/// Code Generated by IMS on: 1/7/2019 8:00:30 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISFMCgpbdGetOblhDetailsRequest : VEISEcRequestBase
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
		public VEISFMCgpbdReqOblh VEISFMCgpbdReqOblhInfo { get; set; }
	}
	[DataContract]
	public class VEISFMCgpbdReqOblh
	{
		[DataMember]
		public Int32 mcs_id { get; set; }
		[DataMember]
		public string mcs_tc { get; set; }
		[DataMember]
		public string mcs_tnum { get; set; }
		[DataMember]
		public string mcs_vndid { get; set; }
		[DataMember]
		public string mcs_dt { get; set; }
		[DataMember]
		public string mcs_vname { get; set; }
		[DataMember]
		public string mcs_podte { get; set; }
		[DataMember]
		public string mcs_actdte { get; set; }
		[DataMember]
		public string mcs_effdte { get; set; }
		[DataMember]
		public string mcs_enddte { get; set; }
		[DataMember]
		public string mcs_lnum { get; set; }
		[DataMember]
		public string mcs_bfy { get; set; }
		[DataMember]
		public string mcs_fund { get; set; }
		[DataMember]
		public string mcs_xdiv { get; set; }
		[DataMember]
		public string mcs_xsta { get; set; }
		[DataMember]
		public string mcs_substa { get; set; }
		[DataMember]
		public string mcs_cc { get; set; }
		[DataMember]
		public string mcs_fcpacc { get; set; }
		[DataMember]
		public string mcs_boc { get; set; }
		[DataMember]
		public string mcs_jobnum { get; set; }
		[DataMember]
		public string mcs_orgamt { get; set; }
		[DataMember]
		public string mcs_clsamt { get; set; }
		[DataMember]
		public string mcs_otsdamt { get; set; }
		[DataMember]
		public string mcs_acamt { get; set; }
		[DataMember]
		public string mcs_hlbamt { get; set; }
		[DataMember]
		public string mcs_examt { get; set; }
		[DataMember]
		public string mcs_contract { get; set; }
		[DataMember]
		public string mcs_autoaccrue { get; set; }
		[DataMember]
		public string mcs_discpercent { get; set; }
		[DataMember]
		public string mcs_discdays { get; set; }
		[DataMember]
		public string mcs_discterm { get; set; }
		[DataMember]
		public string mcs_fob { get; set; }
		[DataMember]
		public string mcs_bebfy { get; set; }
	}
}
