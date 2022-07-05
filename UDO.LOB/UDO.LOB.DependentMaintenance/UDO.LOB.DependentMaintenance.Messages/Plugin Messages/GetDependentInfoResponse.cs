using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using UDO.LOB.DependentMaintenance.Messages;
//using VRM.Integration.Servicebus.Core;

//CSDEv
//namespace VRM.Integration.Servicebus.Bgs.Messages
namespace UDO.LOB.DependentMaintenance.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.GetBGSDependentInfoResponse)]
    [DataContract]
    public class GetDependentInfoResponse : MessageBase
    {
        [DataMember]
        public GetDependentInfoMultipleResponse[] GetDependentInfo { get; set; }
        [DataMember]
        public string Fault { get; set; }
        [DataMember]
        public string SoapLog { get; set; }

    }
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.GetBGSDependentInfoResponse)]
    [DataContract]
    public class GetDependentInfoMultipleResponse : MessageBase
    {
        [DataMember]
        public string crme_Zip { get; set; }
        [DataMember]
        public string crme_State { get; set; }
        [DataMember]
        public string crme_SSN { get; set; }
        [DataMember]
        public string crme_SpouseVAFileNumber { get; set; }
        [DataMember]
        public string crme_SpousePrimaryPhone { get; set; }
        [DataMember]
        public bool crme_SpouseisVeteran { get; set; }
        [DataMember]
        public int crme_MonthlyContributiontoSpouseSupport { get; set; }
        [DataMember]
        public string crme_MiddleName { get; set; }
        [DataMember]
        public string crme_MarriageState { get; set; }
        [DataMember]
        public string crme_MarriageDate { get; set; }
        [DataMember]
        public string crme_MarriageCountry { get; set; }
        [DataMember]
        public string crme_MarriageCity { get; set; }
        [DataMember]
        public bool crme_LiveswithSpouse { get; set; }
        [DataMember]
        public string crme_LastName { get; set; }
        [DataMember]
        public string crme_FirstName { get; set; }
        [DataMember]
        public string crme_DOB { get; set; }
        [DataMember]
        public int crme_DependentStatus { get; set; }
        [DataMember]
        public int crme_DependentRelationship { get; set; }
        [DataMember]
        public string crme_County { get; set; }
        [DataMember]
        public string crme_Country { get; set; }
        [DataMember]
        public string crme_City { get; set; }
        [DataMember]
        public bool crme_ChildSeriouslyDisabled { get; set; }
        [DataMember]
        public int crme_ChildRelationship { get; set; }
        [DataMember]
        public string crme_ChildPrimaryPhone { get; set; }
        [DataMember]
        public bool crme_ChildPreviouslyMarried { get; set; }
        [DataMember]
        public string crme_ChildPlaceofBirthState { get; set; }
        [DataMember]
        public string crme_ChildPlaceofBirthCountry { get; set; }
        [DataMember]
        public string crme_ChildPlaceofBirthCity { get; set; }
        [DataMember]
        public string crme_ChildLiveswithMiddleName { get; set; }
        [DataMember]
        public string crme_ChildLiveswithLastName { get; set; }
        [DataMember]
        public string crme_ChildLiveswithFirstName { get; set; }
        [DataMember]
        public bool crme_ChildLiveswithVet { get; set; }
        [DataMember]
        public bool crme_ChildAge1823InSchool { get; set; }
        [DataMember]
        public string crme_Address3 { get; set; }
        [DataMember]
        public string crme_Address2 { get; set; }
        [DataMember]
        public string crme_Address1 { get; set; }
        [DataMember]
        public string crme_name { get; set; }
        [DataMember]
        public string crme_SuffixName { get; set; }
        [DataMember]
        public string crme_ChildLIveswithSuffixName { get; set; }
        [DataMember]
        public string crme_TermnlDigitNbr { get; set; }
        [DataMember]
        public string crme_RelationshipBeginDate { get; set; }
        [DataMember]
        public string crme_RelationshipEndDate { get; set; }
        [DataMember]
        public string crme_AwardInd { get; set; }
    }
}
