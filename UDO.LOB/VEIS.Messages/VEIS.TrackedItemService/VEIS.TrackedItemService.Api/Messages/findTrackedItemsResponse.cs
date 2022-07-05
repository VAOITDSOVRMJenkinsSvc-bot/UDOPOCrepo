using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.TrackedItemService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.TrackedItemService,findTrackedItems method, Response.
	/// Code Generated by IMS on: 12/27/2018 4:23:41 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISfindTrackedItemsResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISbenefitClaim VEISbenefitClaimInfo { get; set; }
	}
	[DataContract]
	public class VEISbenefitClaim
	{
		[DataMember]
		public string mcs_bnftClmTc { get; set; }
		[DataMember]
		public string mcs_bnftClmTn { get; set; }
		[DataMember]
		public DateTime mcs_brokeredInDt { get; set; }
		[DataMember]
		public bool mcs_brokeredInDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_brokeredOutDt { get; set; }
		[DataMember]
		public bool mcs_brokeredOutDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_cancldDt { get; set; }
		[DataMember]
		public bool mcs_cancldDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_claimRcvdDt { get; set; }
		[DataMember]
		public bool mcs_claimRcvdDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_claimSuspnsDt { get; set; }
		[DataMember]
		public bool mcs_claimSuspnsDtSpecified { get; set; }
		[DataMember]
		public string mcs_clmId { get; set; }
		[DataMember]
		public string mcs_clmSuspnsCd { get; set; }
		[DataMember]
		public DateTime mcs_closedDt { get; set; }
		[DataMember]
		public bool mcs_closedDtSpecified { get; set; }
		[DataMember]
		public string mcs_comntTxt { get; set; }
		[DataMember]
		public DateTime mcs_continuedDt { get; set; }
		[DataMember]
		public bool mcs_continuedDtSpecified { get; set; }
		[DataMember]
		public string mcs_dvlpmtItemId { get; set; }
		[DataMember]
		public DateTime mcs_lastCntctDt { get; set; }
		[DataMember]
		public bool mcs_lastCntctDtSpecified { get; set; }
		[DataMember]
		public string mcs_lcSttRsnTc { get; set; }
		[DataMember]
		public string mcs_lcSttRsnTn { get; set; }
		[DataMember]
		public string mcs_lctnId { get; set; }
		[DataMember]
		public string mcs_nonMedClmDesc { get; set; }
		[DataMember]
		public string mcs_notesInd { get; set; }
		[DataMember]
		public string mcs_notfcnPrflTc { get; set; }
		[DataMember]
		public string mcs_prirty { get; set; }
		[DataMember]
		public string mcs_ptcpntIdClmnt { get; set; }
		[DataMember]
		public string mcs_ptcpntIdRvsr { get; set; }
		[DataMember]
		public string mcs_ptcpntIdVet { get; set; }
		[DataMember]
		public string mcs_ptcpntIdVsr { get; set; }
		[DataMember]
		public string mcs_ptcpntSuspnsId { get; set; }
		[DataMember]
		public DateTime mcs_ratdecCmpltDt { get; set; }
		[DataMember]
		public bool mcs_ratdecCmpltDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_ratingCorrctnDt { get; set; }
		[DataMember]
		public bool mcs_ratingCorrctnDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_ratingIncmpltDt { get; set; }
		[DataMember]
		public bool mcs_ratingIncmpltDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_rdyDecnDt { get; set; }
		[DataMember]
		public bool mcs_rdyDecnDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_readyToWrkDt { get; set; }
		[DataMember]
		public bool mcs_readyToWrkDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_secRdyDecnDt { get; set; }
		[DataMember]
		public bool mcs_secRdyDecnDtSpecified { get; set; }
		[DataMember]
		public string mcs_sojLctnId { get; set; }
		[DataMember]
		public DateTime mcs_suspnsActnDt { get; set; }
		[DataMember]
		public bool mcs_suspnsActnDtSpecified { get; set; }
		[DataMember]
		public string mcs_suspnsRsnTxt { get; set; }
		[DataMember]
		public DateTime mcs_transferredInDt { get; set; }
		[DataMember]
		public bool mcs_transferredInDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_transferredOutDt { get; set; }
		[DataMember]
		public bool mcs_transferredOutDtSpecified { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
		[DataMember]
		public VEIScontentionsMultipleResponse[] VEIScontentionsInfo { get; set; }
		[DataMember]
		public VEISdvlpmtItemsMultipleResponse[] VEISdvlpmtItemsInfo { get; set; }
		[DataMember]
		public VEISlettersMultipleResponse[] VEISlettersInfo { get; set; }
		[DataMember]
		public VEISstationProfile VEISstationProfileInfo { get; set; }
		[DataMember]
		public VEISstnSuspnsPrfilMultipleResponse[] VEISstnSuspnsPrfilInfo { get; set; }
	}
	[DataContract]
	public class VEIScontentionsMultipleResponse
	{
		[DataMember]
		public DateTime mcs_beginDt { get; set; }
		[DataMember]
		public bool mcs_beginDtSpecified { get; set; }
		[DataMember]
		public string mcs_clmId { get; set; }
		[DataMember]
		public string mcs_clmntTxt { get; set; }
		[DataMember]
		public string mcs_clsfcnId { get; set; }
		[DataMember]
		public string mcs_clsfcnTxt { get; set; }
		[DataMember]
		public DateTime mcs_cmpltdDt { get; set; }
		[DataMember]
		public bool mcs_cmpltdDtSpecified { get; set; }
		[DataMember]
		public string mcs_cntntnId { get; set; }
		[DataMember]
		public string mcs_cntntnStatusTc { get; set; }
		[DataMember]
		public string mcs_cntntnTypeCd { get; set; }
		[DataMember]
		public string mcs_cntntnTypeNm { get; set; }
		[DataMember]
		public string mcs_dgnstcTc { get; set; }
		[DataMember]
		public string mcs_dgnstcTn { get; set; }
		[DataMember]
		public string mcs_medInd { get; set; }
		[DataMember]
		public DateTime mcs_notfcnDt { get; set; }
		[DataMember]
		public bool mcs_notfcnDtSpecified { get; set; }
		[DataMember]
		public string mcs_origSourceTypeCd { get; set; }
		[DataMember]
		public string mcs_wgAplcblInd { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
		[DataMember]
		public VEISspecialIssuesMultipleResponse[] VEISspecialIssuesInfo { get; set; }
	}
	[DataContract]
	public class VEISspecialIssuesMultipleResponse
	{
		[DataMember]
		public string mcs_clmId { get; set; }
		[DataMember]
		public string mcs_cntntnId { get; set; }
		[DataMember]
		public string mcs_cntntnSpisId { get; set; }
		[DataMember]
		public string mcs_spisTc { get; set; }
		[DataMember]
		public string mcs_spisTn { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
	}
	[DataContract]
	public class VEISdvlpmtItemsMultipleResponse
	{
		[DataMember]
		public DateTime mcs_acceptDt { get; set; }
		[DataMember]
		public bool mcs_acceptDtSpecified { get; set; }
		[DataMember]
		public string mcs_claimId { get; set; }
		[DataMember]
		public DateTime mcs_createDt { get; set; }
		[DataMember]
		public bool mcs_createDtSpecified { get; set; }
		[DataMember]
		public Int64 mcs_createPtcpntId { get; set; }
		[DataMember]
		public bool mcs_createPtcpntIdSpecified { get; set; }
		[DataMember]
		public string mcs_createStnNum { get; set; }
		[DataMember]
		public string mcs_devactnId { get; set; }
		[DataMember]
		public string mcs_docid { get; set; }
		[DataMember]
		public string mcs_dvlpmtItemId { get; set; }
		[DataMember]
		public string mcs_dvlpmtTc { get; set; }
		[DataMember]
		public DateTime mcs_followDt { get; set; }
		[DataMember]
		public bool mcs_followDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_inErrorDt { get; set; }
		[DataMember]
		public bool mcs_inErrorDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_receiveDt { get; set; }
		[DataMember]
		public bool mcs_receiveDtSpecified { get; set; }
		[DataMember]
		public string mcs_recipient { get; set; }
		[DataMember]
		public string mcs_relatdDvlpmtItemId { get; set; }
		[DataMember]
		public string mcs_relatdOutgngDcmntId { get; set; }
		[DataMember]
		public DateTime mcs_reqDt { get; set; }
		[DataMember]
		public bool mcs_reqDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_scanDt { get; set; }
		[DataMember]
		public bool mcs_scanDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_scndFlwDt { get; set; }
		[DataMember]
		public bool mcs_scndFlwDtSpecified { get; set; }
		[DataMember]
		public string mcs_shortNm { get; set; }
		[DataMember]
		public string mcs_stdDevactnId { get; set; }
		[DataMember]
		public DateTime mcs_suspnsDt { get; set; }
		[DataMember]
		public bool mcs_suspnsDtSpecified { get; set; }
		[DataMember]
		public string mcs_vbmsDevactionId { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
		[DataMember]
		public VEISdevlItemOutArgValueMultipleResponse[] VEISdevlItemOutArgValueInfo { get; set; }
	}
	[DataContract]
	public class VEISdevlItemOutArgValueMultipleResponse
	{
		[DataMember]
		public string mcs_cntntnId { get; set; }
		[DataMember]
		public Int64 mcs_colNo { get; set; }
		[DataMember]
		public bool mcs_colNoSpecified { get; set; }
		[DataMember]
		public string mcs_devactnId { get; set; }
		[DataMember]
		public string mcs_keyWordTxt { get; set; }
		[DataMember]
		public string mcs_outgngDcmntId { get; set; }
		[DataMember]
		public string mcs_ptcOutgngDocTn { get; set; }
		[DataMember]
		public string mcs_ptcpntId { get; set; }
		[DataMember]
		public Int64 mcs_rowNo { get; set; }
		[DataMember]
		public bool mcs_rowNoSpecified { get; set; }
		[DataMember]
		public Byte[] mcs_valueTxt { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
	}
	[DataContract]
	public class VEISlettersMultipleResponse
	{
		[DataMember]
		public string mcs_barCd { get; set; }
		[DataMember]
		public string mcs_clmId { get; set; }
		[DataMember]
		public DateTime mcs_dcmntDt { get; set; }
		[DataMember]
		public bool mcs_dcmntDtSpecified { get; set; }
		[DataMember]
		public string mcs_docid { get; set; }
		[DataMember]
		public string mcs_dvlpmtTc { get; set; }
		[DataMember]
		public DateTime mcs_estabdDt { get; set; }
		[DataMember]
		public bool mcs_estabdDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_fileSttDt { get; set; }
		[DataMember]
		public bool mcs_fileSttDtSpecified { get; set; }
		[DataMember]
		public string mcs_fileSttTc { get; set; }
		[DataMember]
		public string mcs_incldEnclsrInd { get; set; }
		[DataMember]
		public string mcs_lttrTmplatId { get; set; }
		[DataMember]
		public Byte[] mcs_lttrTxt { get; set; }
		[DataMember]
		public Byte[] mcs_lttrTxt2 { get; set; }
		[DataMember]
		public string mcs_nm { get; set; }
		[DataMember]
		public string mcs_outdcmtTc { get; set; }
		[DataMember]
		public DateTime mcs_printDt { get; set; }
		[DataMember]
		public bool mcs_printDtSpecified { get; set; }
		[DataMember]
		public string mcs_ptcpntDcmntTn { get; set; }
		[DataMember]
		public string mcs_ptcpntId { get; set; }
		[DataMember]
		public string mcs_templatTc { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
		[DataMember]
		public VEIScntctProfile VEIScntctProfileInfo { get; set; }
		[DataMember]
		public VEISdevelopmentActionsMultipleResponse[] VEISdevelopmentActionsInfo { get; set; }
		[DataMember]
		public VEISenclosuresMultipleResponse[] VEISenclosuresInfo { get; set; }
		[DataMember]
		public VEISletterRelationshipsMultipleResponse[] VEISletterRelationshipsInfo { get; set; }
	}
	[DataContract]
	public class VEIScntctProfile
	{
		[DataMember]
		public string mcs_adrOneTxt { get; set; }
		[DataMember]
		public string mcs_adrThreeTxt { get; set; }
		[DataMember]
		public string mcs_adrTwoTxt { get; set; }
		[DataMember]
		public string mcs_cityNm { get; set; }
		[DataMember]
		public string mcs_cntryTn { get; set; }
		[DataMember]
		public string mcs_cntyNm { get; set; }
		[DataMember]
		public string mcs_dvlpmtCntctTc { get; set; }
		[DataMember]
		public string mcs_firstNm { get; set; }
		[DataMember]
		public string mcs_frgnPostalCd { get; set; }
		[DataMember]
		public string mcs_lastNm { get; set; }
		[DataMember]
		public string mcs_middleNm { get; set; }
		[DataMember]
		public string mcs_orgNm { get; set; }
		[DataMember]
		public string mcs_postalCd { get; set; }
		[DataMember]
		public string mcs_prvncNm { get; set; }
		[DataMember]
		public string mcs_ptcpntAdrTn { get; set; }
		[DataMember]
		public string mcs_ptcpntId { get; set; }
		[DataMember]
		public string mcs_sfxNm { get; set; }
		[DataMember]
		public string mcs_slttnTn { get; set; }
		[DataMember]
		public string mcs_trtryNm { get; set; }
		[DataMember]
		public string mcs_zip1stSfxNo { get; set; }
		[DataMember]
		public string mcs_zip2ndSfxNo { get; set; }
		[DataMember]
		public string mcs_zipPrefixNo { get; set; }
		[DataMember]
		public string mcs_mPostalTc { get; set; }
		[DataMember]
		public string mcs_mPstOfficeTc { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
	}
	[DataContract]
	public class VEISdevelopmentActionsMultipleResponse
	{
		[DataMember]
		public DateTime mcs_createDt { get; set; }
		[DataMember]
		public bool mcs_createDtSpecified { get; set; }
		[DataMember]
		public string mcs_cstHeadngId { get; set; }
		[DataMember]
		public Byte[] mcs_cstPrgrphTxt { get; set; }
		[DataMember]
		public string mcs_devactnId { get; set; }
		[DataMember]
		public string mcs_docid { get; set; }
		[DataMember]
		public string mcs_fedAgencyInd { get; set; }
		[DataMember]
		public string mcs_headngId { get; set; }
		[DataMember]
		public string mcs_lctnId { get; set; }
		[DataMember]
		public string mcs_nm { get; set; }
		[DataMember]
		public string mcs_pgmTc { get; set; }
		[DataMember]
		public string mcs_piesReqstCd { get; set; }
		[DataMember]
		public string mcs_prgrphId { get; set; }
		[DataMember]
		public string mcs_ptcpntId { get; set; }
		[DataMember]
		public string mcs_rulesBasedInd { get; set; }
		[DataMember]
		public string mcs_stdDevactnCd { get; set; }
		[DataMember]
		public string mcs_stdDevactnId { get; set; }
		[DataMember]
		public string mcs_stdactnDescp { get; set; }
		[DataMember]
		public DateTime mcs_suspnsDt { get; set; }
		[DataMember]
		public bool mcs_suspnsDtSpecified { get; set; }
		[DataMember]
		public Int64 mcs_suspnsDuratn { get; set; }
		[DataMember]
		public bool mcs_suspnsDuratnSpecified { get; set; }
		[DataMember]
		public Int64 mcs_suspnsDys { get; set; }
		[DataMember]
		public bool mcs_suspnsDysSpecified { get; set; }
		[DataMember]
		public string mcs_suspnsUnit { get; set; }
		[DataMember]
		public Byte[] mcs_txt { get; set; }
		[DataMember]
		public string mcs_vbmsDevactionId { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
		[DataMember]
		public VEIScustomParagraphsMultipleResponse[] VEIScustomParagraphsInfo { get; set; }
		[DataMember]
		public VEISdevelopmentItemsMultipleResponse[] VEISdevelopmentItemsInfo { get; set; }
		[DataMember]
		public VEISoutArgValuesMultipleResponse[] VEISoutArgValuesInfo { get; set; }
	}
	[DataContract]
	public class VEIScustomParagraphsMultipleResponse
	{
		[DataMember]
		public Byte[] mcs_customPrgrphTxt { get; set; }
		[DataMember]
		public string mcs_devactnId { get; set; }
		[DataMember]
		public string mcs_headngId { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
	}
	[DataContract]
	public class VEISdevelopmentItemsMultipleResponse
	{
		[DataMember]
		public DateTime mcs_acceptDt { get; set; }
		[DataMember]
		public bool mcs_acceptDtSpecified { get; set; }
		[DataMember]
		public string mcs_claimId { get; set; }
		[DataMember]
		public DateTime mcs_createDt { get; set; }
		[DataMember]
		public bool mcs_createDtSpecified { get; set; }
		[DataMember]
		public Int64 mcs_createPtcpntId { get; set; }
		[DataMember]
		public bool mcs_createPtcpntIdSpecified { get; set; }
		[DataMember]
		public string mcs_createStnNum { get; set; }
		[DataMember]
		public string mcs_devactnId { get; set; }
		[DataMember]
		public string mcs_docid { get; set; }
		[DataMember]
		public string mcs_dvlpmtItemId { get; set; }
		[DataMember]
		public string mcs_dvlpmtTc { get; set; }
		[DataMember]
		public DateTime mcs_followDt { get; set; }
		[DataMember]
		public bool mcs_followDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_inErrorDt { get; set; }
		[DataMember]
		public bool mcs_inErrorDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_receiveDt { get; set; }
		[DataMember]
		public bool mcs_receiveDtSpecified { get; set; }
		[DataMember]
		public string mcs_recipient { get; set; }
		[DataMember]
		public string mcs_relatdDvlpmtItemId { get; set; }
		[DataMember]
		public string mcs_relatdOutgngDcmntId { get; set; }
		[DataMember]
		public DateTime mcs_reqDt { get; set; }
		[DataMember]
		public bool mcs_reqDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_scanDt { get; set; }
		[DataMember]
		public bool mcs_scanDtSpecified { get; set; }
		[DataMember]
		public DateTime mcs_scndFlwDt { get; set; }
		[DataMember]
		public bool mcs_scndFlwDtSpecified { get; set; }
		[DataMember]
		public string mcs_shortNm { get; set; }
		[DataMember]
		public string mcs_stdDevactnId { get; set; }
		[DataMember]
		public DateTime mcs_suspnsDt { get; set; }
		[DataMember]
		public bool mcs_suspnsDtSpecified { get; set; }
		[DataMember]
		public string mcs_vbmsDevactionId { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
		[DataMember]
		public VEISdevlItemOutArgValue1MultipleResponse[] VEISdevlItemOutArgValue1Info { get; set; }
	}
	[DataContract]
	public class VEISdevlItemOutArgValue1MultipleResponse
	{
		[DataMember]
		public string mcs_cntntnId { get; set; }
		[DataMember]
		public Int64 mcs_colNo { get; set; }
		[DataMember]
		public bool mcs_colNoSpecified { get; set; }
		[DataMember]
		public string mcs_devactnId { get; set; }
		[DataMember]
		public string mcs_keyWordTxt { get; set; }
		[DataMember]
		public string mcs_outgngDcmntId { get; set; }
		[DataMember]
		public string mcs_ptcOutgngDocTn { get; set; }
		[DataMember]
		public string mcs_ptcpntId { get; set; }
		[DataMember]
		public Int64 mcs_rowNo { get; set; }
		[DataMember]
		public bool mcs_rowNoSpecified { get; set; }
		[DataMember]
		public Byte[] mcs_valueTxt { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
	}
	[DataContract]
	public class VEISoutArgValuesMultipleResponse
	{
		[DataMember]
		public string mcs_cntntnId { get; set; }
		[DataMember]
		public Int64 mcs_colNo { get; set; }
		[DataMember]
		public bool mcs_colNoSpecified { get; set; }
		[DataMember]
		public string mcs_devactnId { get; set; }
		[DataMember]
		public string mcs_keyWordTxt { get; set; }
		[DataMember]
		public string mcs_outgngDcmntId { get; set; }
		[DataMember]
		public string mcs_ptcOutgngDocTn { get; set; }
		[DataMember]
		public string mcs_ptcpntId { get; set; }
		[DataMember]
		public Int64 mcs_rowNo { get; set; }
		[DataMember]
		public bool mcs_rowNoSpecified { get; set; }
		[DataMember]
		public Byte[] mcs_valueTxt { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
	}
	[DataContract]
	public class VEISenclosuresMultipleResponse
	{
		[DataMember]
		public Int64 mcs_copiesQty { get; set; }
		[DataMember]
		public bool mcs_copiesQtySpecified { get; set; }
		[DataMember]
		public string mcs_docid { get; set; }
		[DataMember]
		public string mcs_jetFormNm { get; set; }
		[DataMember]
		public string mcs_oneOcrncInd { get; set; }
		[DataMember]
		public string mcs_outdcmtStencid { get; set; }
		[DataMember]
		public string mcs_stdEnclsrId { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
	}
	[DataContract]
	public class VEISletterRelationshipsMultipleResponse
	{
		[DataMember]
		public string mcs_followUpDcmntId { get; set; }
		[DataMember]
		public string mcs_letterRlnshpId { get; set; }
		[DataMember]
		public string mcs_relatdDcmntId { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
	}
	[DataContract]
	public class VEISstationProfile
	{
		[DataMember]
		public Int64 mcs_clmntNtfiv { get; set; }
		[DataMember]
		public bool mcs_clmntNtfivSpecified { get; set; }
		[DataMember]
		public string mcs_cutOffTm { get; set; }
		[DataMember]
		public Int64 mcs_dftAllItmNtfIv { get; set; }
		[DataMember]
		public bool mcs_dftAllItmNtfIvSpecified { get; set; }
		[DataMember]
		public string mcs_lctnId { get; set; }
		[DataMember]
		public string mcs_lttrSig { get; set; }
		[DataMember]
		public string mcs_lttrSigTtl { get; set; }
		[DataMember]
		public string mcs_lttrStnEmail { get; set; }
		[DataMember]
		public string mcs_notfcnPrflTc { get; set; }
		[DataMember]
		public string mcs_snstvFileUserId { get; set; }
		[DataMember]
		public string mcs_stnNm { get; set; }
		[DataMember]
		public string mcs_suspnsInd { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
	}
	[DataContract]
	public class VEISstnSuspnsPrfilMultipleResponse
	{
		[DataMember]
		public string mcs_claimSuspnsCd { get; set; }
		[DataMember]
		public string mcs_lctnId { get; set; }
		[DataMember]
		public string mcs_pgmTypeCd { get; set; }
		[DataMember]
		public Int64 mcs_row_cnt { get; set; }
		[DataMember]
		public bool mcs_row_cntSpecified { get; set; }
		[DataMember]
		public string mcs_stdDvlpmtActionCd { get; set; }
		[DataMember]
		public string mcs_stdDvlpmtActionId { get; set; }
		[DataMember]
		public string mcs_stnSuspnsPrfilId { get; set; }
		[DataMember]
		public Int64 mcs_suspnsDuratnQty { get; set; }
		[DataMember]
		public bool mcs_suspnsDuratnQtySpecified { get; set; }
		[DataMember]
		public Int64 mcs_callId { get; set; }
		[DataMember]
		public bool mcs_callIdSpecified { get; set; }
		[DataMember]
		public DateTime mcs_jrnDt { get; set; }
		[DataMember]
		public bool mcs_jrnDtSpecified { get; set; }
		[DataMember]
		public string mcs_jrnLctnId { get; set; }
		[DataMember]
		public string mcs_jrnObjId { get; set; }
		[DataMember]
		public string mcs_jrnSttTc { get; set; }
		[DataMember]
		public string mcs_jrnUserId { get; set; }
		[DataMember]
		public Int64 mcs_parentId { get; set; }
		[DataMember]
		public bool mcs_parentIdSpecified { get; set; }
		[DataMember]
		public string mcs_parentName { get; set; }
		[DataMember]
		public Int64 mcs_rowCnt { get; set; }
		[DataMember]
		public bool mcs_rowCntSpecified { get; set; }
		[DataMember]
		public Int64 mcs_rowId { get; set; }
		[DataMember]
		public bool mcs_rowIdSpecified { get; set; }
	}
}
