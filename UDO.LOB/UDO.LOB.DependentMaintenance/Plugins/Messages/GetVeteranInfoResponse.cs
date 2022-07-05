using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRM.CRME.Plugin.DependentMaintenance.Messages
{
    public class GetVeteranInfoResponse 
    {
        public string MessageId { get; set; }
        public GetVeteranInfoMultipleResponse[] GetVeteranInfo { get; set; }
        public string Fault { get; set; }
        public string SoapLog { get; set; }
    }

    public class GetVeteranInfoMultipleResponse
    {
        public string MessageId { get; set; }

        public string crme_ZIP { get; set; }
        
        public string crme_VAFileNumber { get; set; }
        
        public string crme_StoredSSN { get; set; }
        
        public string crme_State { get; set; }
        
        public string crme_SSN { get; set; }
        
        public string crme_SecondaryPhone { get; set; }
        
        public string crme_PrimaryPhone { get; set; }
        
        public string crme_ParticipantID { get; set; }
        
        public string crme_MiddleName { get; set; }
        
        public string crme_LastName { get; set; }
        
        public string crme_FirstName { get; set; }
        
        public string crme_Email { get; set; }
        
        public string crme_EDIP { get; set; }
        
        public string crme_DOB { get; set; }
        
        public bool crme_DataFromApplication { get; set; }
        
        public string crme_Country { get; set; }
        
        public string crme_City { get; set; }
        
        public string crme_Address3 { get; set; }
        
        public string crme_Address2 { get; set; }
        
        public string crme_Address1 { get; set; }
        
        public string crme_AddressType { get; set; }
        
        public string crme_AllowPOAAccess { get; set; }
        
        public string crme_AllowPOACADD { get; set; }
        
        public string crme_beneficiarydateofbirth { get; set; }
        
        public string crme_DayTimeAreaCode { get; set; }
        
        public string crme_NightTimeAreaCode { get; set; }
        
        public string crme_ZipPlus4 { get; set; }
        
        public string crme_SuffixName { get; set; }
        
        public string crme_Title { get; set; }
    }

}
