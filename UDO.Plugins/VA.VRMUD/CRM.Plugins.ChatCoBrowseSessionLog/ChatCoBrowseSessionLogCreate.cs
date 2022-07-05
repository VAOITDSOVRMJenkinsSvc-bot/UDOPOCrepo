using System;
using CRMUD;
using Microsoft.Xrm.Sdk;
using CRM.Plugins;

namespace CRM.Plugins.ChatCoBrowseSessionLog
{
    public class ChatCoBrowseSessionLogCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var log = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var pluginContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var serviceProxy = serviceFactory.CreateOrganizationService(pluginContext.InitiatingUserId);

            crme_chatcobrowsesessionlog chatCoBrowse = ((Entity)pluginContext.InputParameters["Target"]).ToEntity<crme_chatcobrowsesessionlog>();
            
            if (chatCoBrowse != null)
            {
                string subject = null;

                if (!string.IsNullOrEmpty(chatCoBrowse.crme_ChatSessionId))
                {
                    subject = string.Format("Chat Id: {0}", chatCoBrowse.crme_ChatSessionId);
                }
                else if (!string.IsNullOrEmpty(chatCoBrowse.crme_CoBrowseSessionId))
                {
                    subject = string.Format("CoBrowse Id: {0}", chatCoBrowse.crme_CoBrowseSessionId);
                }
                
                var phoneCallId = CreatePhoneCall(serviceProxy, chatCoBrowse, subject);
                
                chatCoBrowse.RegardingObjectId = new EntityReference()
                {
                    Name = subject,
                    LogicalName = PhoneCall.EntityLogicalName,
                    Id = phoneCallId
                };
                
                chatCoBrowse.crme_PhoneCallId = new EntityReference()
                {
                    Name = subject,
                    LogicalName = PhoneCall.EntityLogicalName,
                    Id = phoneCallId
                };
                
                string path = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                path = path.Substring(0, path.IndexOf(".gov/") + 5);
                chatCoBrowse.crme_LaunchUrl = string.Format("{0}main.aspx?etn=phonecall&id={1}&pagetype=entityrecord", path, chatCoBrowse.RegardingObjectId.Id.ToString());
                
            }
        }

        private Guid CreatePhoneCall(IOrganizationService serviceProxy, crme_chatcobrowsesessionlog chatCoBrowse, string subject)
        {
            /*SystemUser user = (SystemUser)serviceProxy.Retrieve
                (SystemUser.EntityLogicalName,
                    chatCoBrowse.OwnerId.Id,
                    new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                if (user == null)
                    throw new InvalidPluginExecutionException("Could not find user record using: " + chatCoBrowse.OwnerId.Id.ToString());
            */
            /*user.va_PCRSensitivityLevel != null ? (int?)user.va_PCRSensitivityLevel.Value : null,*/
            

            PhoneCall phoneCall = new PhoneCall()
            {
                Subject = subject,
                va_SessionType = chatCoBrowse.crme_SessionType,
                va_ChatSessionId = chatCoBrowse.crme_ChatSessionId,
                va_CoBrowseSessionId = chatCoBrowse.crme_CoBrowseSessionId,
                va_CallAgentId = chatCoBrowse.crme_CallAgentId,
                va_VSOId = chatCoBrowse.crme_VSOId,
                va_VSOAgentId = chatCoBrowse.crme_VSOAgentId,
                va_EDIPI = chatCoBrowse.crme_EDIPI,
                va_CoBrowseSessionIndicator = !string.IsNullOrEmpty(chatCoBrowse.crme_CoBrowseSessionId),
                va_SensitivityLevelValue = getSensitivityLevel(chatCoBrowse.crme_VeteranSensitivityLevel)
                //OwnerId = chatCoBrowse.OwnerId
            };
            
            //update Recipient of the Phone Call
            Entity toParty = new Entity("activityparty");

            toParty.Attributes.Add("partyid", new EntityReference("systemuser", chatCoBrowse.OwnerId.Id));

            //remove the to field to avoid duplicate key issues
            phoneCall.Attributes.Remove("to");

            //now re-add the to field
            phoneCall.Attributes.Add("to", new Entity[] { toParty });
            
            //If SEP, set Participant ID, else set SSN.  If we don't have either, then leave them blank
            if (chatCoBrowse.crme_VSOOrgId != null)
            {
                if (chatCoBrowse.crme_ParticipantId != null)
                {
                    phoneCall.va_ParticipantID = chatCoBrowse.crme_ParticipantId;
                }
                else
                {
                    phoneCall.va_ParticipantID = "";
                }
            }
            else
            {
                if (chatCoBrowse.crme_SSN != null)
                {
                    phoneCall.va_SSN = chatCoBrowse.crme_SSN;
                }
                else
                {
                    phoneCall.va_SSN = "";
                }
            }
            
            parseCallTypes(phoneCall, chatCoBrowse);
            
            return serviceProxy.Create(phoneCall);
        }

        private int? getSensitivityLevel(string str)
        {
            int i;
            if (int.TryParse(str, out i))
                return i;
            return null;
        }

        private void parseCallTypes(PhoneCall phoneCall, crme_chatcobrowsesessionlog chatCoBrowse)
        {
            char[] resolutionDelimiter = { '#' };
            char[] callTypeDelimeter = { ',' };
            char[] callSubtypeDelimeter = { '|' };

            string temp1 = chatCoBrowse.crme_Category;
            string res = chatCoBrowse.crme_Resolution;

            if (temp1 != null)
            {

                string temp2 = temp1.Replace("Department|Service|Classifications|Categories|", "");//strip out the beginning to get to the Call Type
                string cat = temp2.Replace("SEP|", "");//strip out SEP to get to additional Call Types

                if (cat != null)
                {
                    string resolution = null;

                    if (res != null)
                    {
                        resolution = res;
                    }
                    else
                    {
                        resolution = "";
                    }

                    string[] callTypes = cat.Split(callTypeDelimeter);
                    int callTypeCounter = 0;

                    if (callTypes != null)
                    {
                        foreach (string s in callTypes)
                        {
                            //s.Replace("DepartmentServiceClassificationsCategories", ""); //get to the Call Types
                            //s.Replace("SEP|", ""); //remove SEP if it's part of the Call Types string
                            string[] callSubtypes = s.Split(callSubtypeDelimeter);
                            if (callSubtypes != null)
                            {
                                if (callTypeCounter == 0)
                                {

                                    if (callSubtypes.Length > 1)
                                    {
                                        phoneCall.va_Disposition = setCallType(callSubtypes[0].Trim());
                                        phoneCall.va_DispositionSubtype = setCallSubType(callSubtypes[0].Trim(), callSubtypes[1].Trim());
                                    }
                                    else
                                    {
                                        phoneCall.va_Disposition = setCallType(callSubtypes[0].Trim());
                                    }
                                }

                                if (callTypeCounter == 1)
                                {
                                    phoneCall.va_Disposition2 = setCallType(callSubtypes[0].Trim());
                                }
                                if (callTypeCounter == 2)
                                {
                                    phoneCall.va_Disposition3 = setCallType(callSubtypes[0].Trim());
                                }
                                if (callTypeCounter == 3)
                                {
                                    phoneCall.va_Disposition4 = setCallType(callSubtypes[0].Trim());
                                }
                                if (callTypeCounter == 4)
                                {
                                    phoneCall.va_Disposition5 = setCallType(callSubtypes[0].Trim());
                                }
                                callTypeCounter++;
                            }
                        }
                    }
                }
            }
        }

        public OptionSetValue setCallType(string callType)
        {
            int iCallType = 0;
            switch (callType)
            {
                case "Appeals":
                    iCallType = 953850000;
                    break;
                case "Claim":
                    iCallType = 953850001;
                    break;
                case "Claim Inquiry":
                    iCallType = 953850018;
                    break;
                case "Correspondence and Forms":
                    iCallType = 953850002;
                    break;
                case "Debt Inquiry":
                    iCallType = 953850022;
                    break;
                case "Dependent Maintenance":
                    iCallType = 953850032;
                    break;
                case "eBenefits":
                    iCallType = 953850011;
                    break;
                case "Email Forms":
                    iCallType = 953850027;
                    break;
                case "Emergency/Priority":
                    iCallType = 953850025;
                    break;
                case "Fiduciary":
                    iCallType = 953850003;
                    break;
                case "FNOD":
                    iCallType = 953850004;
                    break;
                case "FOIA/Privacy Act":
                    iCallType = 953850034;
                    break;
                case "General Inquiry":
                    iCallType = 953850017;
                    break;
                case "General Questions":
                    iCallType = 953850005;
                    break;
                case "General Request for VA phone number/Va address/VA fax":
                    iCallType = 953850036;
                    break;
                case "Ghost Call/Disconnected Call":
                    iCallType = 953850033;
                    break;
                case "Letters/Documents Request":
                    iCallType = 953850023;
                    break;
                case "Major Event - Call Tracker":
                    iCallType = 953850024;
                    break;
                case "Medical":
                    iCallType = 953850006;
                    break;
                case "Monthly Certification":
                    iCallType = 953850019;
                    break;
                case "News media inquiry":
                    iCallType = 953850035;
                    break;
                case "Other":
                    iCallType = 953850007;
                    break;
                case "Other (EDU)":
                    iCallType = 953850026;
                    break;
                case "Other Benefits - Comp or Pension":
                    iCallType = 953850009;
                    break;
                case "Other Business Lines":
                    iCallType = 953850008;
                    break;
                case "Payment Inquiry":
                    iCallType = 953850021;
                    break;
                case "Payment/Debts":
                    iCallType = 953850010;
                    break;
                case "SEP VSO":
                    iCallType = 953850016;
                    break;
                case "Special Issues":
                    iCallType = 953850012;
                    break;
                case "Suicide Call":
                    iCallType = 953850013;
                    break;
                case "Threat Call":
                    iCallType = 953850014;
                    break;
                case "Update Information":
                    iCallType = 953850015;
                    break;
                case "Update Information (EDU)":
                    iCallType = 953850020;
                    break;
                case "VA Media Campaign 2013-Online":
                    iCallType = 953850031;
                    break;
                case "VA Media Campaign 2013-Radio":
                    iCallType = 953850028;
                    break;
                case "VA Media Campaign 2013-Social Media":
                    iCallType = 953850030;
                    break;
                case "VA Media Campaign 2013-TV":
                    iCallType = 953850029;
                    break;
                default:
                    //iCallType = 953850000;
                    iCallType = 000000000;
                    break;
            }
            return new OptionSetValue(iCallType);
        }

        public OptionSetValue setCallSubType(string callType, string callSubType)
        {
            int iCallSubType = 0;
            switch (callType)
            {
                case "Appeals":
                    switch (callSubType)
                    {
                        case "General Status":
                            iCallSubType = 953850035;
                            break;
                        case "SOC/SSOC Question":
                            iCallSubType = 953850025;
                            break;
                    }
                    break;
                case "Claim Inquiry":
                    switch (callSubType)
                    {
                        case "Advance Pay":
                            iCallSubType = 953850130;
                            break;
                        case "Application Inquiry":
                            iCallSubType = 953850125;
                            break;
                        case "Audit":
                            iCallSubType = 953850131;
                            break;
                        case "Correspondence Inquiry":
                            iCallSubType = 953850122;
                            break;
                        case "Delimiting Date":
                            iCallSubType = 953850128;
                            break;
                        case "Extension Request of Delimiting Date":
                            iCallSubType = 953850133;
                            break;
                        case "Extension Request for Entitlement":
                            iCallSubType = 953850134;
                            break;
                        case "Kickers - Buy ups":
                            iCallSubType = 953850132;
                            break;
                        case "Notification of Widrawal/Drop":
                            iCallSubType = 953850129;
                            break;
                        case "Other":
                            iCallSubType = 953850135;
                            break;
                        case "Percent of Eligibility":
                            iCallSubType = 953850126;
                            break;
                        case "Processing Time":
                            iCallSubType = 953850123;
                            break;
                        case "Reliquish/Election Date":
                            iCallSubType = 953850124;
                            break;
                        case "Remaining Entitlement":
                            iCallSubType = 953850127;
                            break;
                        case "Status of Enrollment Certification":
                            iCallSubType = 953850120;
                            break;
                        case "VONAPP":
                            iCallSubType = 953850121;
                            break;
                        default:
                            iCallSubType = 953850130;
                            break;
                    }
                    break;
                case "Claim":
                    switch (callSubType)
                    {
                        case "Aid & Attendance":
                            iCallSubType = 953850060;
                            break;
                        case "Appointment Claim":
                            iCallSubType = 953850088;
                            break;
                        case "Burial Plot and Transportation Benefits":
                            iCallSubType = 953850087;
                            break;
                        case "Death Pension/A&A/Housebound":
                            iCallSubType = 953850086;
                            break;
                        case "DIC(Dependency and Indemnity compensation)/Accrued":
                            iCallSubType = 953850038;
                            break;
                        case "Document Verification":
                            iCallSubType = 953850019;
                            break;
                        case "Establish a Claim":
                            iCallSubType = 953850024;
                            break;
                        case "Exam":
                            iCallSubType = 953850026;
                            break;
                        case "Fully Developed":
                            iCallSubType = 953850074;
                            break;
                        case "General Status":
                            iCallSubType = 953850082;
                            break;
                        case "Income Adjustment":
                            iCallSubType = 953850040;
                            break;
                        case "Increase a Claim":
                            iCallSubType = 953850032;
                            break;
                        case "Informal - AB-10 Letter":
                            iCallSubType = 953850085;
                            break;
                        case "IVM":
                            iCallSubType = 953850071;
                            break;
                        case "MAP-D not updated;VAI necessary":
                            iCallSubType = 953850047;
                            break;
                        case "Reopen Claim":
                            iCallSubType = 953850012;
                            break;
                        case "Status of EVRs":
                            iCallSubType = 953850069;
                            break;
                        case "Verifying VA Income":
                            iCallSubType = 953850001;
                            break;
                        case "Withdraw a Claim/Contention":
                            iCallSubType = 953850072;
                            break;
                        default:
                            iCallSubType = 953850060;
                            break;
                    }
                    break;
                case "Correspondence and Forms":
                    switch (callSubType)
                    {
                        case "Explanation of Letter":
                            iCallSubType = 953850027;
                            break;
                        case "MAP-D Letters":
                            iCallSubType = 953850046;
                            break;
                        case "Request for Benefit Letter":
                            iCallSubType = 953850079;
                            break;
                        case "Request for Forms":
                            iCallSubType = 953850067;
                            break;
                        default:
                            iCallSubType = 953850027;
                            break;
                    }
                    break;
                case "Debt Inquiry":
                    switch (callSubType)
                    {
                        case "Dispute/Request Waiver Request":
                            iCallSubType = 953850155;
                            break;
                        case "Explanation":
                            iCallSubType = 953850152;
                            break;
                        case "Other":
                            iCallSubType = 953850160;
                            break;
                        case "Payment Arrangements":
                            iCallSubType = 953850154;
                            break;
                        case "Refund Request":
                            iCallSubType = 953850159;
                            break;
                        case "Refund Status":
                            iCallSubType = 953850158;
                            break;
                        case "School Debt Recoup":
                            iCallSubType = 953850157;
                            break;
                        case "Status - Existing Waiver Request":
                            iCallSubType = 953850153;
                            break;
                        case "Transfer Debt from School/Student":
                            iCallSubType = 953850156;
                            break;
                        default:
                            iCallSubType = 953850155;
                            break;
                    }
                    break;
                case "Dependent Maintenance":
                    switch (callSubType)
                    {
                        case "Add Adopted Children":
                            iCallSubType = 953850204;
                            break;
                        case "Add Minor Children":
                            iCallSubType = 953850202;
                            break;
                        case "Add School Aged Children":
                            iCallSubType = 953850206;
                            break;
                        case "Add Spouse":
                            iCallSubType = 953850201;
                            break;
                        case "Add Spouse and Minor Children":
                            iCallSubType = 953850203;
                            break;
                        case "Add Stepchildren":
                            iCallSubType = 953850205;
                            break;
                        case "Other Dependency Related Call":
                            iCallSubType = 953850209;
                            break;
                        case "Remove Dependents":
                            iCallSubType = 953850208;
                            break;
                        case "Update Dependents":
                            iCallSubType = 953850207;
                            break;
                        default:
                            iCallSubType = 953850204;
                            break;
                    }
                    break;
                case "eBenefits":
                    switch (callSubType)
                    {
                        case "Conflicting/Non-matching information":
                            iCallSubType = 953850216;
                            break;
                        case "General Question":
                            iCallSubType = 953850011;
                            break;
                        case "N/A":
                            iCallSubType = 953850095;
                            break;
                        case "Remote Proofing":
                            iCallSubType = 953850005;
                            break;
                        case "Website Issue":
                            iCallSubType = 953850008;
                            break;
                        default:
                            iCallSubType = 953850216;
                            break;
                    }
                    break;
                case "Email Forms":
                    switch (callSubType)
                    {
                        case "Email Blank Forms":
                            iCallSubType = 953850176;
                            break;
                        default:
                            iCallSubType = 953850176;
                            break;
                    }
                    break;
                case "Emergency/Priority":
                    switch (callSubType)
                    {
                        case "Bomb/Violence Threat":
                            iCallSubType = 953850173;
                            break;
                        case "Other":
                            iCallSubType = 953850174;
                            break;
                        case "Suicidal Caller":
                            iCallSubType = 953850172;
                            break;
                        default:
                            iCallSubType = 953850173;
                            break;
                    }
                    break;
                case "Fiduciary":
                    switch (callSubType)
                    {
                        case "CADD":
                            iCallSubType = 953850009;
                            break;
                        case "Fiduciary Contact Request":
                            iCallSubType = 953850030;
                            break;
                        case "Fiduciary Issues requiring Transfer/VA":
                            iCallSubType = 953850210;
                            break;
                        case "Fiduciary Issues requiring VAI":
                            iCallSubType = 953850031;
                            break;
                        case "Misuse allegation":
                            iCallSubType = 953850015;
                            break;
                        default:
                            iCallSubType = 953850009;
                            break;
                    }
                    break;
                case "FNOD":
                    switch (callSubType)
                    {
                        case "Death of a Dependent":
                            iCallSubType = 953850020;
                            break;
                        case "Death of a Non-Veteran Beneficiary":
                            iCallSubType = 953850017;
                            break;
                        case "Death of a Veteran":
                            iCallSubType = 953850034;
                            break;
                        case "MOD Payment":
                            iCallSubType = 953850033;
                            break;
                        default:
                            iCallSubType = 953850020;
                            break;
                    }

                    break;
                case "FOIA/Privacy Act":
                    switch (callSubType)
                    {
                        case "How to file":
                            iCallSubType = 953850211;
                            break;
                        case "Status Update":
                            iCallSubType = 953850212;
                            break;
                        default:
                            iCallSubType = 953850211;
                            break;
                    }
                    break;
                case "General Inquiry":
                    switch (callSubType)
                    {
                        case "Ebenefits Questions":
                            iCallSubType = 953850106;
                            break;
                        case "Forms Request":
                            iCallSubType = 953850112;
                            break;
                        case "GIBILL Website Questions":
                            iCallSubType = 953850109;
                            break;
                        case "Homeless":
                            iCallSubType = 953850111;
                            break;
                        case "How to Appeal":
                            iCallSubType = 953850117;
                            break;
                        case "How to Apply":
                            iCallSubType = 953850113;
                            break;
                        case "How to change schools":
                            iCallSubType = 953850114;
                            break;
                        case "How to file a complaint":
                            iCallSubType = 953850118;
                            break;
                        case "How to report hours OTJ 22-6653d-1":
                            iCallSubType = 953850115;
                            break;
                        case "How to transfer benefits to Dependents-TOE":
                            iCallSubType = 953850116;
                            break;
                        case "Other":
                            iCallSubType = 953850119;
                            break;
                        case "Other VA Benefits":
                            iCallSubType = 953850104;
                            break;
                        case "VA Office Contact Information":
                            iCallSubType = 953850105;
                            break;
                        case "VONAPP Questions":
                            iCallSubType = 953850108;
                            break;
                        case "Wave Questions":
                            iCallSubType = 953850107;
                            break;
                        case "Wrong Department/Number":
                            iCallSubType = 953850107;
                            break;
                        default:
                            iCallSubType = 953850106;
                            break;
                    }

                    break;
                case "General Questions":
                    switch (callSubType)
                    {
                        case "Ebenefits Questions":
                            iCallSubType = 953850083;
                            break;
                        case "Forms Request":
                            iCallSubType = 953850057;
                            break;
                        case "GIBILL Website Questions":
                            iCallSubType = 953850014;
                            break;
                        case "Homeless":
                            iCallSubType = 953850065;
                            break;
                        default:
                            iCallSubType = 953850083;
                            break;
                    }

                    break;
                case "General Request for VA phone number/Va address/VA fax":
                    switch (callSubType)
                    {
                        case "N/A":
                            iCallSubType = 953850215;
                            break;
                        default:
                            iCallSubType = 953850215;
                            break;
                    }
                    break;
                case "Ghost Call/Disconnected Call":
                    switch (callSubType)
                    {
                        case "N/A":
                            iCallSubType = 953850213;
                            break;
                        default:
                            iCallSubType = 953850213;
                            break;
                    }
                    break;
                case "Letters/Documents Request":
                    switch (callSubType)
                    {
                        case "Copy of Award Ltr/Debt Ltr/Relinq Ltr":
                            iCallSubType = 953850165;
                            break;
                        case "Copy of COE":
                            iCallSubType = 953850161;
                            break;
                        case "Copy of DD214":
                            iCallSubType = 953850167;
                            break;
                        case "Copy of VONAPP":
                            iCallSubType = 953850166;
                            break;
                        case "Exhaust Entitlement Letter":
                            iCallSubType = 953850163;
                            break;
                        case "Financial Aid Letter":
                            iCallSubType = 953850162;
                            break;
                        case "Hazelwood Letter":
                            iCallSubType = 953850164;
                            break;
                        case "Other":
                            iCallSubType = 953850168;
                            break;
                        default:
                            iCallSubType = 953850165;
                            break;
                    }

                    break;
                case "Major Event - Call Tracker":
                    switch (callSubType)
                    {
                        case "Billing Payment Failure":
                            iCallSubType = 953850170;
                            break;
                        case "Hurricane":
                            iCallSubType = 953850169;
                            break;
                        case "Other":
                            iCallSubType = 953850171;
                            break;
                        default:
                            iCallSubType = 953850170;
                            break;
                    }

                    break;
                case "Medical":
                    switch (callSubType)
                    {
                        case "Eligibility":
                            iCallSubType = 953850023;
                            break;
                        case "Fee Basis":
                            iCallSubType = 953850029;
                            break;
                        case "Medical Treatment":
                            iCallSubType = 953850049;
                            break;
                        case "Other VHA issues":
                            iCallSubType = 953850058;
                            break;
                        case "Patient Advocate":
                            iCallSubType = 953850061;
                            break;
                        case "Prescriptions":
                            iCallSubType = 953850064;
                            break;
                        default:
                            iCallSubType = 953850023;
                            break;
                    }
                    break;
                case "Monthly Certification":
                    switch (callSubType)
                    {
                        case "Confirmation of self certification on IVR":
                            iCallSubType = 953850136;
                            break;
                        case "Confirmation of self certification on WAVE":
                            iCallSubType = 953850137;
                            break;
                        case "Request to certify":
                            iCallSubType = 953850138;
                            break;
                        default:
                            iCallSubType = 953850136;
                            break;
                    }
                    break;
                case "News media inquiry":
                    switch (callSubType)
                    {
                        case "N/A":
                            iCallSubType = 953850214;
                            break;
                        default:
                            iCallSubType = 953850214;
                            break;
                    }
                    break;
                case "Other":
                    switch (callSubType)
                    {
                        case "N/A":
                            iCallSubType = 953850051;
                            break;
                        default:
                            iCallSubType = 953850051;
                            break;
                    }
                    break;
                case "Other (EDU)":
                    switch (callSubType)
                    {
                        case "N/A":
                            iCallSubType = 953850175;
                            break;
                        default:
                            iCallSubType = 953850175;
                            break;
                    }
                    break;
                case "Other Benefits - Comp or Pension":
                    switch (callSubType)
                    {
                        case "Adaptive Housing":
                            iCallSubType = 953850000;
                            break;
                        case "Clothing Allowance":
                            iCallSubType = 953850013;
                            break;
                        case "Vehicle Allowance/Adaptation":
                            iCallSubType = 953850076;
                            break;
                        default:
                            iCallSubType = 953850000;
                            break;
                    }
                    break;
                case "Other Business Lines":
                    switch (callSubType)
                    {
                        case "CHAMP VA/TRICARE":
                            iCallSubType = 953850010;
                            break;
                        case "Education Issues":
                            iCallSubType = 953850021;
                            break;
                        case "Insurance":
                            iCallSubType = 953850043;
                            break;
                        case "Loan Guaranty Issues":
                            iCallSubType = 953850045;
                            break;
                        case "NCA":
                            iCallSubType = 953850053;
                            break;
                        case "Non VA Calls":
                            iCallSubType = 953850055;
                            break;
                        case "Vocational Rehabilitation":
                            iCallSubType = 953850078;
                            break;
                        default:
                            iCallSubType = 953850010;
                            break;
                    }
                    break;
                case "Payment Inquiry":
                    switch (callSubType)
                    {
                        case "Advance Payment/Accelerated Payment":
                            iCallSubType = 953850151;
                            break;
                        case "Duplicate Payment":
                            iCallSubType = 953850148;
                            break;
                        case "Explanation of Amount":
                            iCallSubType = 953850146;
                            break;
                        case "Missing Payment":
                            iCallSubType = 953850147;
                            break;
                        case "Next payment date/Amount":
                            iCallSubType = 953850145;
                            break;
                        case "Return Payment":
                            iCallSubType = 953850149;
                            break;
                        case "Status of Refund/Reissued Payment":
                            iCallSubType = 953850150;
                            break;
                        default:
                            iCallSubType = 953850151;
                            break;
                    }
                    break;

                case "Payment/Debts":
                     switch (callSubType)
                    {  
                        case "Address Change/Account Suspended":
                            iCallSubType = 953850070;
                            break;
                        case "Amount of payment":
                            iCallSubType = 953850004;
                            break;
                        case "COLA(Cost of Living Adjustment)":
                            iCallSubType = 953850090;
                            break;
                        case "Date of Payment":
                            iCallSubType = 953850092;
                            break;
                        case "DMC":
                            iCallSubType = 953850018;
                            break;
                        case "General Status":
                            iCallSubType = 953850084;
                            break;
                        case "Go direct Master Cards":
                            iCallSubType = 953850036;
                            break;
                        case "Incorrect check amount":
                            iCallSubType = 953850041;
                            break;
                        case "Medical Center Debts":
                            iCallSubType = 953850089;
                            break;
                        case "Non Receipt of Check":
                            iCallSubType = 953850054;
                            break;
                        case "Payment Deductions":
                            iCallSubType = 953850062;
                            break;
                        case "Payment Lost/Stolen":
                            iCallSubType = 953850091;
                            break;
                        case "TRACER":
                            iCallSubType = 953850075;
                            break;
                        default:
                            iCallSubType = 953850151;
                            break;
                    }
                    break;
                case "SEP VSO":
                    switch (callSubType)
                    {
                        case "CADD Issues":
                            iCallSubType = 953850102;
                            break;
                        case "Claim Form Issues":
                            iCallSubType = 953850099;
                            break;
                        case "Claim Status Issues":
                            iCallSubType = 953850098;
                            break;
                        case "Claim Submission Issues":
                            iCallSubType = 953850100;
                            break;
                        case "General Questions":
                            iCallSubType = 953850016;
                            break;
                        case "Login Issues":
                            iCallSubType = 953850068;
                            break;
                        case "OGC Database Issues":
                            iCallSubType = 953850048;
                            break;
                        case "Payment History Issues":
                            iCallSubType = 953850097;
                            break;
                        case "PIV Card Issues":
                            iCallSubType = 953850037;
                            break;
                        case "POA Issues":
                            iCallSubType = 953850080;
                            break;
                        case "Portal Outage":
                            iCallSubType = 953850103;
                            break;
                        case "Remote Proofing Issues":
                            iCallSubType = 953850044;
                            break;
                        case "Search Issues":
                            iCallSubType = 953850096;
                            break;
                        case "Uploading forms":
                            iCallSubType = 953850101;
                            break;
                        default:
                            iCallSubType = 953850102;
                            break;
                    }

                    break;
                case "Special Issues":
                    switch (callSubType)
                    {
                        case "Agent Orange":
                            iCallSubType = 953850003;
                            break;
                        case "Elderly Veterans":
                            iCallSubType = 953850022;
                            break;
                        case "Former POW":
                            iCallSubType = 953850028;
                            break;
                        case "Homeless":
                            iCallSubType = 953850039;
                            break;
                        case "Indigent Burial":
                            iCallSubType = 953850042;
                            break;
                        case "Medical Foster Home Coordinator":
                            iCallSubType = 953850094;
                            break;
                        case "Minority Veterans":
                            iCallSubType = 953850050;
                            break;
                        case "Native American Veterans":
                            iCallSubType = 953850052;
                            break;
                        case "OEF/OIF (Operations Enduring Freedom/Operation Iraqi Freedom)":
                            iCallSubType = 953850056;
                            break;
                        case "Outreach":
                            iCallSubType = 953850059;
                            break;
                        case "PTSD":
                            iCallSubType = 953850066;
                            break;
                        case "Women Veterans":
                            iCallSubType = 953850081;
                            break;
                        default:
                            iCallSubType = 953850003;
                            break;
                    }

                    break;
                case "Suicide Call":
                    switch (callSubType)
                    {
                        case "Suicide Call":
                            iCallSubType = 953850073;
                            break;
                         default:
                            iCallSubType = 953850073;
                            break;
                    }
                   break;
                case "Threat Call":
                   switch (callSubType)
                   {
                       case "Bomb Threat":
                           iCallSubType = 953850007;
                           break;
                       case "Facility Threat":
                           iCallSubType = 953850093;
                           break;
                       case "Physical Threat on Individual":
                           iCallSubType = 953850063;
                           break;
                       default:
                           iCallSubType = 953850007;
                           break;
                   }
                    break;
                case "Update Information":
                   switch (callSubType)
                   {
                       case "Address (CADD)/Phone Number":
                           iCallSubType = 953850002;
                           break;
                       case "Bank Account/EFT":
                           iCallSubType = 953850006;
                           break;
                       default:
                           iCallSubType = 953850002;
                           break;
                   }
                    break;
                case "Update Information (EDU)":
                    switch (callSubType)
                    {
                        case "Address Change":
                            iCallSubType = 953850140;
                            break;
                        case "Direct Deposit - Set up, Change, Cancel":
                            iCallSubType = 953850139;
                            break;
                        case "Email Change":
                            iCallSubType = 953850141;
                            break;
                        case "Name Change":
                            iCallSubType = 953850143;
                            break;
                        case "Phone Number Change":
                            iCallSubType = 953850142;
                            break;
                        case "Status Change - No longer Active Duty":
                            iCallSubType = 953850144;
                            break;
                        default:
                            iCallSubType = 953850140;
                            break;
                    }
                    break;
                case "VA Media Campaign 2013-Online":
                    switch (callSubType)
                    {
                        case "Compensation":
                            iCallSubType = 953850195;
                            break;
                        case "Education":
                            iCallSubType = 953850197;
                            break;
                        case "Insurance":
                            iCallSubType = 953850199;
                            break;
                        case "Loan Guarantee":
                            iCallSubType = 953850198;
                            break;
                        case "Pension":
                            iCallSubType = 953850196;
                            break;
                        case "VHA":
                            iCallSubType = 953850200;
                            break;
                        default:
                            iCallSubType = 953850195;
                            break;
                    }
                    break;
                case "VA Media Campaign 2013-Radio":
                    switch (callSubType)
                    {
                        case "Compensation":
                            iCallSubType = 953850177;
                            break;
                        case "Education":
                            iCallSubType = 953850179;
                            break;
                        case "Insurance":
                            iCallSubType = 953850181;
                            break;
                        case "Loan Guarantee":
                            iCallSubType = 953850180;
                            break;
                        case "Pension":
                            iCallSubType = 953850178;
                            break;
                        case "VHA":
                            iCallSubType = 953850182;
                            break;
                        default:
                            iCallSubType = 953850177;
                            break;
                    }
                    break;
                case "VA Media Campaign 2013-Social Media":
                    switch (callSubType)
                    {
                        case "Compensation":
                            iCallSubType = 953850189;
                            break;
                        case "Education":
                            iCallSubType = 953850191;
                            break;
                        case "Insurance":
                            iCallSubType = 953850193;
                            break;
                        case "Loan Guarantee":
                            iCallSubType = 953850192;
                            break;
                        case "Pension":
                            iCallSubType = 953850190;
                            break;
                        case "VHA":
                            iCallSubType = 953850194;
                            break;
                        default:
                            iCallSubType = 953850189;
                            break;
                    }
                    break;
                case "VA Media Campaign 2013-TV":
                    switch (callSubType)
                    {
                        case "Compensation":
                            iCallSubType = 953850183;
                            break;
                        case "Education":
                            iCallSubType = 953850185;
                            break;
                        case "Insurance":
                            iCallSubType = 953850187;
                            break;
                        case "Loan Guarantee":
                            iCallSubType = 953850186;
                            break;
                        case "Pension":
                            iCallSubType = 953850184;
                            break;
                        case "VHA":
                            iCallSubType = 953850188;
                            break;
                        default:
                            iCallSubType = 953850183;
                            break;
                    }
                    break;
                default:
                    //iCallSubType = 953850035;
                    iCallSubType = 000000000;
                    break;
            }

            return new OptionSetValue(iCallSubType);
        }
    }
}
