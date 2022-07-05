using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using VEIS.Messages.IntentToFileWebService;

namespace UDO.LOB.IntentToFile.Messages
{
    [DataContract]
    public class UDOSubmitITFResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public string request { get; set; }
        [DataMember]
        public string response { get; set; }
        [DataMember]
        public string jrnUserId { get; set; }
        [DataMember]
        public string jrnLctnId { get; set; }
        [DataMember]
        public string createDt { get; set; } 
        [DataMember]
        public string submtrApplcnTypeCd { get; set; }
        [DataMember]
        public string intentToFileId { get; set; }
        [DataMember]
        // Replaced: VIMTinsertInt2FileintentToFileDTO = VEISinsertInt2FileintentToFileDTO
        public VEISinsertInt2FileintentToFileDTO IntentToFileDto { get; set; }

    }


    // Below was already commented out.
    //[DataContract]
    //public class VIMTinsertInt2FileintentToFileDTO
    //{
    //    [DataMember]
    //    public string mcs_badAddrsInd { get; set; }
    //    [DataMember]
    //    public long mcs_bnftClaimId { get; set; }
    //    [DataMember]
    //    public bool mcs_bnftClaimIdSpecified { get; set; }
    //    [DataMember]
    //    public string mcs_clmantAddrsOneTxt { get; set; }
    //    [DataMember]
    //    public string mcs_clmantAddrsTwoTxt { get; set; }
    //    [DataMember]
    //    public string mcs_clmantAddrsUnitNbr { get; set; }
    //    [DataMember]
    //    public string mcs_clmantCityNm { get; set; }
    //    [DataMember]
    //    public string mcs_clmantCntryNm { get; set; }
    //    [DataMember]
    //    public string mcs_clmantEmailAddrsTxt { get; set; }
    //    [DataMember]
    //    public string mcs_clmantFirstNm { get; set; }
    //    [DataMember]
    //    public string mcs_clmantLastNm { get; set; }
    //    [DataMember]
    //    public string mcs_clmantMiddleNm { get; set; }
    //    [DataMember]
    //    public string mcs_clmantPhoneAreaNbr { get; set; }
    //    [DataMember]
    //    public string mcs_clmantPhoneNbr { get; set; }
    //    [DataMember]
    //    public string mcs_clmantSsn { get; set; }
    //    [DataMember]
    //    public string mcs_clmantStateCd { get; set; }
    //    [DataMember]
    //    public string mcs_clmantZipCd { get; set; }
    //    [DataMember]
    //    public DateTime mcs_cmpltdDt { get; set; }
    //    [DataMember]
    //    public bool mcs_cmpltdDtSpecified { get; set; }
    //    [DataMember]
    //    public DateTime mcs_createDt { get; set; }
    //    [DataMember]
    //    public bool mcs_createDtSpecified { get; set; }
    //    [DataMember]
    //    public string mcs_everFiledInd { get; set; }
    //    [DataMember]
    //    public DateTime mcs_exprtnDt { get; set; }
    //    [DataMember]
    //    public bool mcs_exprtnDtSpecified { get; set; }
    //    [DataMember]
    //    public string mcs_genderCd { get; set; }
    //    [DataMember]
    //    public string mcs_incompReasonTypeCd { get; set; }
    //    [DataMember]
    //    public long mcs_intentToFileId { get; set; }
    //    [DataMember]
    //    public bool mcs_intentToFileIdSpecified { get; set; }
    //    [DataMember]
    //    public string mcs_itfStatusTypeCd { get; set; }
    //    [DataMember]
    //    public string mcs_itfTypeCd { get; set; }
    //    [DataMember]
    //    public DateTime mcs_jrnDt { get; set; }
    //    [DataMember]
    //    public bool mcs_jrnDtSpecified { get; set; }
    //    [DataMember]
    //    public string mcs_jrnExtnlApplcnNm { get; set; }
    //    [DataMember]
    //    public string mcs_jrnExtnlKeyTxt { get; set; }
    //    [DataMember]
    //    public string mcs_jrnExtnlUserId { get; set; }
    //    [DataMember]
    //    public string mcs_jrnLctnId { get; set; }
    //    [DataMember]
    //    public string mcs_jrnObjId { get; set; }
    //    [DataMember]
    //    public string mcs_jrnStatusTypeCd { get; set; }
    //    [DataMember]
    //    public string mcs_jrnUserId { get; set; }
    //    [DataMember]
    //    public long mcs_ptcpntClmantId { get; set; }
    //    [DataMember]
    //    public bool mcs_ptcpntClmantIdSpecified { get; set; }
    //    [DataMember]
    //    public long mcs_ptcpntVetId { get; set; }
    //    [DataMember]
    //    public bool mcs_ptcpntVetIdSpecified { get; set; }
    //    [DataMember]
    //    public DateTime mcs_rcvdDt { get; set; }
    //    [DataMember]
    //    public bool mcs_rcvdDtSpecified { get; set; }
    //    [DataMember]
    //    public DateTime mcs_signedDt { get; set; }
    //    [DataMember]
    //    public bool mcs_signedDtSpecified { get; set; }
    //    [DataMember]
    //    public string mcs_signtrInd { get; set; }
    //    [DataMember]
    //    public DateTime mcs_submtdDt { get; set; }
    //    [DataMember]
    //    public bool mcs_submtdDtSpecified { get; set; }
    //    [DataMember]
    //    public string mcs_submtrApplcnTypeCd { get; set; }
    //    [DataMember]
    //    public long mcs_submtrPtcpntId { get; set; }
    //    [DataMember]
    //    public bool mcs_submtrPtcpntIdSpecified { get; set; }
    //    [DataMember]
    //    public string mcs_supresLetterInd { get; set; }
    //    [DataMember]
    //    public DateTime mcs_vetBrthdyDt { get; set; }
    //    [DataMember]
    //    public bool mcs_vetBrthdyDtSpecified { get; set; }
    //    [DataMember]
    //    public string mcs_vetFileNbr { get; set; }
    //    [DataMember]
    //    public string mcs_vetFirstNm { get; set; }
    //    [DataMember]
    //    public string mcs_vetLastNm { get; set; }
    //    [DataMember]
    //    public string mcs_vetMiddleNm { get; set; }
    //    [DataMember]
    //    public string mcs_vetSsnNbr { get; set; }
    //    [DataMember]
    //    public string mcs_vsoNm { get; set; }
    //}
}