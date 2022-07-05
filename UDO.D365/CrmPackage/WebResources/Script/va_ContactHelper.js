var contact = function () {
    this.parseCorpRecord = function (xmlObject, xmlGeneralInfo, xmlPoaFid) {
        // check that the mandatory nodes exist
        this.isaveteran = false;
        if (!xmlObject || !xmlObject.selectSingleNode('//lastName')) return false;
        this.firstName = xmlObject.selectSingleNode('//firstName') ? xmlObject.selectSingleNode('//firstName').text : null;
        this.lastName = xmlObject.selectSingleNode('//lastName').text;
        this.middleName = xmlObject.selectSingleNode('//middleName') ? xmlObject.selectSingleNode('//middleName').text : null;
        this.ssn = xmlObject.selectSingleNode('//ssn') ? xmlObject.selectSingleNode('//ssn').text : null;
        this.email = xmlObject.selectSingleNode('//emailAddress') ? xmlObject.selectSingleNode('//emailAddress').text : null;
        this.fileNumber = xmlObject.selectSingleNode('//fileNumber') ? xmlObject.selectSingleNode('//fileNumber').text : null;
        this.participantId = xmlObject.selectSingleNode('//ptcpntId') ? xmlObject.selectSingleNode('//ptcpntId').text : null;
        this.sensitiveLevelOfRecord = xmlObject.selectSingleNode('//sensitiveLevelOfRecord') ? xmlObject.selectSingleNode('//sensitiveLevelOfRecord').text : null;
        this.phoneNumberOne = xmlObject.selectSingleNode('//phoneNumberOne') ? xmlObject.selectSingleNode('//phoneNumberOne').text : null;
        this.areaNumberOne = xmlObject.selectSingleNode('//areaNumberOne') ? xmlObject.selectSingleNode('//areaNumberOne').text : null;
        this.phoneTypeNameOne = xmlObject.selectSingleNode('//phoneTypeNameOne') ? xmlObject.selectSingleNode('//phoneTypeNameOne').text : null;
        this.phoneNumberTwo = xmlObject.selectSingleNode('//phoneNumberTwo') ? xmlObject.selectSingleNode('//phoneNumberTwo').text : null;
        this.areaNumberTwo = xmlObject.selectSingleNode('//areaNumberTwo') ? xmlObject.selectSingleNode('//areaNumberTwo').text : null;
        this.phoneTypeNameTwo = xmlObject.selectSingleNode('//phoneTypeNameTwo') ? xmlObject.selectSingleNode('//phoneTypeNameTwo').text : null;
        var birth = xmlObject.selectSingleNode('//dateOfBirth') ? xmlObject.selectSingleNode('//dateOfBirth').text :
            (xmlObject.selectSingleNode('//DATE_OF_BIRTH') ? xmlObject.selectSingleNode('//DATE_OF_BIRTH').text : null);
        //Addition of new DOB Text Field:  RTC 108417 - Handle Dates before 1900.
        this.dobtext = birth != null && birth.length > 0 ? birth : null;
        var death = xmlGeneralInfo && xmlGeneralInfo.selectSingleNode('//vetDeathDate') ?
            xmlGeneralInfo.selectSingleNode('//vetDeathDate').text :
            (xmlObject.selectSingleNode('//DATE_OF_DEATH') ? xmlObject.selectSingleNode('//DATE_OF_DEATH').text : null);
        var dodFormatted = death && death.length > 0 ? FormatExtjsDate(death) : null;
        this.dod = death && death.length > 0 && dodFormatted ? new Date(dodFormatted) : null;
        this.causeOfDeath = xmlObject.selectSingleNode('//CAUSE_OF_DEATH') ?
            xmlObject.selectSingleNode('//CAUSE_OF_DEATH').text : null; //TODO: Corp source?
        var update = xmlObject.selectSingleNode('//DATE_OF_UPDATE') ? xmlObject.selectSingleNode('//DATE_OF_UPDATE').text : null;
        this.dou = update != null && update.length > 0 ? new Date(update) : null;
        this.poaCode1 = xmlPoaFid && xmlPoaFid.selectSingleNode('//currentPowerOfAttorney/personOrgName') ?
            xmlPoaFid.selectSingleNode('//currentPowerOfAttorney/personOrgName').text :
            (xmlObject.selectSingleNode('//POWER_OF_ATTY_CODE1') ? xmlObject.selectSingleNode('//POWER_OF_ATTY_CODE1').text : null);
        this.poaCode2 = xmlObject.selectSingleNode('//POWER_OF_ATTY_CODE2') ? xmlObject.selectSingleNode('//POWER_OF_ATTY_CODE2').text : null;
        this.addressLine1 = (xmlObject && xmlObject.selectSingleNode('//addressLine1') ? xmlObject.selectSingleNode('//addressLine1').text : null);
        this.addressLine2 = (xmlObject && xmlObject.selectSingleNode('//addressLine2') ? xmlObject.selectSingleNode('//addressLine2').text : null);
        this.addressLine3 = (xmlObject && xmlObject.selectSingleNode('//addressLine3') ? xmlObject.selectSingleNode('//addressLine3').text : null);
        this.city = (xmlObject && xmlObject.selectSingleNode('//city') ? xmlObject.selectSingleNode('//city').text : null);
        this.state = (xmlObject && xmlObject.selectSingleNode('//state') ? xmlObject.selectSingleNode('//state').text : null);
        this.zipCode = (xmlObject && xmlObject.selectSingleNode('//zipCode') ? xmlObject.selectSingleNode('//zipCode').text : null);
        this.country = (xmlObject && xmlObject.selectSingleNode('//country') ? xmlObject.selectSingleNode('//country').text : null);
        this.branchOfService = xmlGeneralInfo && xmlGeneralInfo.selectSingleNode('//militaryBranch') ?
            xmlGeneralInfo.selectSingleNode('//militaryBranch').text :
            (xmlObject.selectSingleNode('//SERVICE/BRANCH_OF_SERVICE') ? xmlObject.selectSingleNode('//SERVICE/BRANCH_OF_SERVICE').text : null);
        this.gender = xmlGeneralInfo && xmlGeneralInfo.selectSingleNode('//vetSex') ?
            xmlGeneralInfo.selectSingleNode('//vetSex').text :
            (xmlObject.selectSingleNode('//SEX_CODE') ? xmlObject.selectSingleNode('//SEX_CODE').text : null);
        this.competencyFlag = xmlObject.selectSingleNode('//competencyDecisionTypeCode') ? xmlObject.selectSingleNode('//competencyDecisionTypeCode').text :
            (xmlObject.selectSingleNode('//INCOMPETENT_IND') ? xmlObject.selectSingleNode('//INCOMPETENT_IND').text : null);
        this.flash = xmlGeneralInfo && xmlGeneralInfo.selectSingleNode('//flashes/flashName') ?
            xmlGeneralInfo.selectSingleNode('//flashes/flashName').text :
            (xmlObject.selectSingleNode('//FLASH/FLASH_CODE') && xmlObject.selectSingleNode('//FLASH/FLASH_CODE').text ?
                xmlObject.selectSingleNode('//FLASH/FLASH_CODE').text + ': ' + xmlObject.selectSingleNode('//FLASH/FLASH_STATION').text + ', ' + xmlObject.selectSingleNode('//FLASH/FLASH_ROUTING_SYMBOL').text : null);
        this.flashes = this.getFlashes(xmlObject, xmlGeneralInfo);
        var characterOfService = xmlObject.selectSingleNode('//SERVICE/CHAR_OF_SVC_CODE') ? xmlObject.selectSingleNode('//SERVICE/CHAR_OF_SVC_CODE').text : '';
        // person is a vet if there's a record in birls for them
        this.isaveteran = (this.lastName && this.lastName.length > 0 /*&& characterOfService == 'OTH'*/);
        if (xmlPoaFid != null) {
            this.poaEndDate = xmlPoaFid.selectSingleNode('//endDate') ? xmlPoaFid.selectSingleNode('//endDate').text : '';
        }
        else {
            this.poaEndDate = '';
        }
        this.CorpFlags = this.getCorpFlags(1);
        return true;
    };
    this.getFidPOAData = function (xmlPoaFid) {
        // detailed analysis of FID/POA
        if (xmlPoaFid && xmlPoaFid.selectSingleNode('//currentFiduciary/personOrgName') && xmlPoaFid.selectSingleNode('//currentFiduciary/personOrgName').text) {
            this.currentFiduciary = new Array();
            this.currentFiduciary['Name'] = xmlPoaFid.selectSingleNode('//currentFiduciary/personOrgName');
            this.currentFiduciary['Fm'] = xmlPoaFid.selectSingleNode('//currentFiduciary/beginDate');
            this.currentFiduciary['To'] = xmlPoaFid.selectSingleNode('//currentFiduciary/endDate');
            this.currentFiduciary['Relation'] = xmlPoaFid.selectSingleNode('//currentFiduciary/relationshipName');
            this.currentFiduciary['Person or Org'] = xmlPoaFid.selectSingleNode('//currentFiduciary/personOrOrganizationIndicator');
            this.currentFiduciary['Temp Custodian'] = xmlPoaFid.selectSingleNode('//currentFiduciary/temporaryCustodianIndicator');
        }
        if (xmlPoaFid && xmlPoaFid.selectSingleNode('//currentPowerOfAttorney/personOrgName') && xmlPoaFid.selectSingleNode('//currentPowerOfAttorney/personOrgName').text) {
            this.currentPowerOfAttorney = new Array();
            this.currentPowerOfAttorney['Name'] = xmlPoaFid.selectSingleNode('//currentPowerOfAttorney/personOrgName');
            this.currentPowerOfAttorney['Fm'] = xmlPoaFid.selectSingleNode('//currentPowerOfAttorney/beginDate');
            this.currentPowerOfAttorney['To'] = xmlPoaFid.selectSingleNode('//currentPowerOfAttorney/endDate');
            this.currentPowerOfAttorney['Relation'] = xmlPoaFid.selectSingleNode('//currentPowerOfAttorney/relationshipName');
            this.currentPowerOfAttorney['Person or Org'] = xmlPoaFid.selectSingleNode('//currentPowerOfAttorney/personOrOrganizationIndicator');
            this.currentPowerOfAttorney['Temp Custodian'] = xmlPoaFid.selectSingleNode('//currentPowerOfAttorney/temporaryCustodianIndicator');
        }
    };
    this.getFolderInfo = function (xmlObject) {
        if (xmlObject && xmlObject.selectSingleNode('//FOLDER/FOLDER_CURRENT_LOCATION') && xmlObject.selectSingleNode('//FOLDER/FOLDER_CURRENT_LOCATION').text) {
            this.folderInfo = new Array();
            this.folderInfo['Location'] = xmlObject.selectSingleNode('//FOLDER/FOLDER_CURRENT_LOCATION');
            this.folderInfo['Type'] = xmlObject.selectSingleNode('//FOLDER/FOLDER_TYPE');
            this.folderInfo['Transfer Date'] = xmlObject.selectSingleNode('//FOLDER/DATE_OF_TRANSFER');
            this.folderInfo['Prior Location'] = xmlObject.selectSingleNode('//FOLDER/FOLDER_PRIOR_LOCATION');
            this.folderInfo['Retire Date'] = xmlObject.selectSingleNode('//FOLDER/DATE_OF_FLDR_RETIRE');
            this.folderInfo['Transit Date'] = xmlObject.selectSingleNode('//FOLDER/DATE_OF_TRANSIT');
            this.folderInfo['Insurance Folder Type'] = xmlObject.selectSingleNode('//FOLDER/INSURANCE_FOLDER_TYPE');
        }
    };
    this.parseVadirRecord = function (vadirPersonXmlObject, vadirContactXmlObject) {
        // check that the mandatory nodes exist
        var selectedVadir = {};
        var selectedAddress = {};
        this.isaveteran = false;
        if ((!vadirContactXmlObject) || (!vadirPersonXmlObject) || (!vadirContactXmlObject.selectSingleNode('//ContactInfo/edipi'))) return false;

        //Get the correct Person record
        if (vadirPersonXmlObject.selectSingleNode('//availableCount').text >= 1) {
            //loop through Persons for correct edipi = vaid
            for (var i = 0; i < vadirPersonXmlObject.selectSingleNode('//Persons').childNodes.length; i++) {
                if (vadirPersonXmlObject.selectSingleNode('//Persons').childNodes[i].selectSingleNode('//vaId').text == vadirContactXmlObject.selectSingleNode('//ContactInfo/edipi').text) {
                    selectedVadir = vadirPersonXmlObject.selectSingleNode('//Persons').childNodes[i];
                }
            }
        }

        //Get the correct Contact Address
        if (vadirContactXmlObject.selectSingleNode('//ContactInfo/Addresses').childNodes.length >= 1) {
            //loop through ContactInfo for correct address = M
            for (var i = 0; i < vadirContactXmlObject.selectSingleNode('//ContactInfo/Addresses').childNodes.length; i++) {
                if (vadirContactXmlObject.selectSingleNode('//ContactInfo/Addresses').childNodes[i].selectSingleNode('//addressType').text == 'M') {
                    selectedAddress = vadirContactXmlObject.selectSingleNode('//ContactInfo/Addresses').childNodes[i];
                    break;
                }
            }
        }

        //get the Phone
        if (vadirContactXmlObject.selectSingleNode('//ContactInfo/Phones').childNodes.length >= 1) {
            //loop through ContactInfo for correct address = M
            for (var i = 0; i < vadirContactXmlObject.selectSingleNode('//ContactInfo/Phones').childNodes.length; i++) {
                if (vadirContactXmlObject.selectSingleNode('//ContactInfo/Phones').childNodes[i].selectSingleNode('//phoneType').text == 'H') {
                    if (vadirContactXmlObject.selectSingleNode('//ContactInfo/Phones/').childNodes[i].selectSingleNode('//phoneNumber')) {
                        this.phoneNumberOne = vadirContactXmlObject.selectSingleNode('//ContactInfo/Phones/').childNodes[i].selectSingleNode('//phoneNumber').text;
                        if (this.phoneNumberOne.length == 10) {
                            this.phoneNumberOne = '(' + this.phoneNumberOne.slice(0, 3) + ') ' + this.phoneNumberOne.slice(3, 6) + '-' + this.phoneNumberOne.slice(6);
                            break;
                        }
                    }
                }
            }
        }

        //Set Flags
        this.flashes = 'Record from VADIR.';

        this.firstName = selectedVadir.selectSingleNode('//firstName') ? selectedVadir.selectSingleNode('//firstName').text : null;
        this.middleName = selectedVadir.selectSingleNode('//middleName') ? selectedVadir.selectSingleNode('//middleName').text : null;
        this.lastName = selectedVadir.selectSingleNode('//lastName').text;
        this.ssn = selectedVadir.selectSingleNode('//socialSecurityNumber') ? selectedVadir.selectSingleNode('//socialSecurityNumber').text : null;

        this.addressLine1 = ((vadirContactXmlObject && selectedAddress.hasChildNodes && selectedAddress.selectSingleNode('//addressLine1')) ? selectedAddress.selectSingleNode('//addressLine1').text : null);
        this.addressLine2 = ((vadirContactXmlObject && selectedAddress.hasChildNodes && selectedAddress.selectSingleNode('//addressLine2')) ? selectedAddress.selectSingleNode('//addressLine2').text : null);
        this.city = ((vadirContactXmlObject && selectedAddress.hasChildNodes && selectedAddress.selectSingleNode('//city')) ? selectedAddress.selectSingleNode('//city').text : null);
        this.state = ((vadirContactXmlObject && selectedAddress.hasChildNodes && selectedAddress.selectSingleNode('//state')) ? selectedAddress.selectSingleNode('//state').text : null);
        this.zipCode = ((vadirContactXmlObject && selectedAddress.hasChildNodes && selectedAddress.selectSingleNode('//zipcode')) ? selectedAddress.selectSingleNode('//zipcode').text : null);
        this.country = ((vadirContactXmlObject && selectedAddress.hasChildNodes && selectedAddress.selectSingleNode('//countryCode')) ? selectedAddress.selectSingleNode('//countryCode').text : null);
        return true;
    }
    this.parseBIRLSRecord = function (xmlObject) {
        // check that the mandatory nodes exist
        this.isaveteran = false;
        if (!xmlObject.selectSingleNode('//LAST_NAME')) return false;
        this.firstName = xmlObject.selectSingleNode('//FIRST_NAME') ? xmlObject.selectSingleNode('//FIRST_NAME').text : null;
        this.lastName = xmlObject.selectSingleNode('//LAST_NAME') ? xmlObject.selectSingleNode('//LAST_NAME').text : null;
        this.middleName = xmlObject.selectSingleNode('//MIDDLE_NAME') ? xmlObject.selectSingleNode('//MIDDLE_NAME').text : null;
        this.ssn = xmlObject.selectSingleNode('//SOC_SEC_NUMBER') ? xmlObject.selectSingleNode('//SOC_SEC_NUMBER').text : null;
        this.fileNumber = xmlObject.selectSingleNode('//CLAIM_NUMBER') ? xmlObject.selectSingleNode('//CLAIM_NUMBER').text : null;
        this.sensitiveLevelOfRecord = null; //? where from xmlObject.selectSingleNode('//sensitiveLevelOfRecord').text;
        var birth = xmlObject.selectSingleNode('//DATE_OF_BIRTH') ? xmlObject.selectSingleNode('//DATE_OF_BIRTH').text : null;
        this.dob = birth != null && birth.length > 0 ? new Date(birth) : null;
        var death = xmlObject.selectSingleNode('//DATE_OF_DEATH') ? xmlObject.selectSingleNode('//DATE_OF_DEATH').text : null;

        var dodFormatted = death && death.length > 0 ? FormatExtjsDate(death) : null;
        this.dod = death && death.length > 0 && dodFormatted ? new Date(dodFormatted) : null;
        this.causeOfDeath = xmlObject.selectSingleNode('//CAUSE_OF_DEATH') ? xmlObject.selectSingleNode('//CAUSE_OF_DEATH').text : null;
        var update = xmlObject.selectSingleNode('//DATE_OF_UPDATE') ? xmlObject.selectSingleNode('//DATE_OF_UPDATE').text : null;
        this.dou = update != null && update.length > 0 ? new Date(update) : null;
        this.poaCode1 = xmlObject.selectSingleNode('//POWER_OF_ATTY_CODE1') ? xmlObject.selectSingleNode('//POWER_OF_ATTY_CODE1').text : null;
        this.poaCode2 = xmlObject.selectSingleNode('//POWER_OF_ATTY_CODE2') ? xmlObject.selectSingleNode('//POWER_OF_ATTY_CODE2').text : null;
        this.addressLine1 = '';
        this.addressLine2 = '';
        this.addressLine3 = '';
        this.city = '';
        this.state = '';
        this.zipCode = '';
        this.country = '';
        this.branchOfService = xmlObject.selectSingleNode('//SERVICE/BRANCH_OF_SERVICE') ? xmlObject.selectSingleNode('//SERVICE/BRANCH_OF_SERVICE').text : null;
        this.gender = xmlObject.selectSingleNode('//SEX_CODE').text;
        this.competencyFlag = xmlObject.selectSingleNode('//competencyDecisionTypeCode') ? xmlObject.selectSingleNode('//competencyDecisionTypeCode').text :
            (xmlObject.selectSingleNode('//INCOMPETENT_IND') ? xmlObject.selectSingleNode('//INCOMPETENT_IND').text : null);
        this.flash = xmlObject.selectSingleNode('//FLASH/FLASH_CODE') && xmlObject.selectSingleNode('//FLASH/FLASH_CODE').text.length > 0 ?
            xmlObject.selectSingleNode('//FLASH/FLASH_CODE').text + ': ' + xmlObject.selectSingleNode('//FLASH/FLASH_STATION').text + ', ' + xmlObject.selectSingleNode('//FLASH/FLASH_ROUTING_SYMBOL').text : null;
        this.flashes = this.getFlashes();
        this.BIRLSEmployeeNumber = xmlObject.selectSingleNode('//EMPLOYEE_NUMBER')
            ? xmlObject.selectSingleNode('//EMPLOYEE_NUMBER').text : null;
        this.BIRLSEmployeeStation = xmlObject.selectSingleNode('//EMPLOYEE_STATION_NUMBER')
            ? xmlObject.selectSingleNode('//EMPLOYEE_STATION_NUMBER').text : null;
        //var characterOfService = xmlObject.selectSingleNode('//SERVICE/CHAR_OF_SVC_CODE') ? xmlObject.selectSingleNode('//SERVICE/CHAR_OF_SVC_CODE').text : '';
        // person is a vet if there's a record in birls for them
        this.isaveteran = (this.lastName && this.lastName.length > 0 /*&& characterOfService == 'OTH'*/);
        this.BIRLSFlags = this.getBIRLSFlags(1);
        this.va_StaionofJurisdictionId = xmlObject.selectSingleNode('//CLAIM_FOLDER_LOCATION')
            ? xmlObject.selectSingleNode('//CLAIM_FOLDER_LOCATION').text : null;

        //Get the RPO if it is EDU
        if ((xmlObject.selectSingleNode('//folders')) && (xmlObject.selectSingleNode('//folders').childNodes.length >= 1)) {
            //loop through FOLDERS for correct Type
            for (var i = 0; i < xmlObject.selectSingleNode('//folders').childNodes.length; i++) {
                var stagedXML = xmlObject.selectSingleNode('//folders').childNodes[i].xml;
                var stagedObj = _XML_UTIL.parseXmlObject(stagedXML);
                var stagedChild = stagedObj.selectSingleNode('//FOLDER/FOLDER_TYPE').text;
                if (stagedChild == 'EDU') {
                    this.rpo = stagedObj.selectSingleNode('//FOLDER/FOLDER_CURRENT_LOCATION').text;
                    break;
                }
            }
        }
        return true;
    };
    this.IsIncompetent = function () {
        return (this.competencyFlag == 'IR' || this.competencyFlag == 'ICC' || this.competencyFlag == 'M');
    };
    this.create = function () {
        var sensitivityLevel = parseInt(this.sensitiveLevelOfRecord);
        //todo: fix updating pick lists
        sensitivityLevel = null;
        var contact = CrmRestKit2011.Create('Contact', {
            FirstName: this.firstName,
            LastName: this.lastName,
            MiddleName: this.middleName,
            va_SSN: this.ssn,
            va_FileNumber: this.fileNumber,
            va_ParticipantID: this.participantId,
            Address1_Line1: this.addressLine1,
            Address1_Line2: this.addressLine2,
            Address1_Line3: this.addressLine3,
            Address1_City: this.city,
            Address1_StateOrProvince: this.state,
            Address1_PostalCode: this.zipCode,
            Address1_Country: this.country,
            va_SensitivityLevelValue: this.sensitivityLevelValue,
            va_VeteranSensitivityLevel: this.sensitiveLevelOfRecord ?
                {
                    "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                    "Value": parseInt(this.sensitiveLevelOfRecord)
                } : null,
            va_DateofDeath: (this.dod ? (this.dod < new Date('1/1/1901') ? new Date('1/1/1901') : this.dod) : null),
            va_SMSPaymentNotices: this.SMSPaymentNotices,
            va_EmailPaymentNotices: this.EmailPaymentNotices,
            va_EmailClaimNotices: this.EmailClaimNotices,
            PreferredContactMethodCode: !this.PreferredMethodofContact ? null :
                {
                    "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                    "Value": parseInt(this.PreferredMethodofContact)
                },
            PreferredAppointmentDayCode: !this.PreferredDay ? null :
                {
                    "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                    "Value": parseInt(this.PreferredDay)
                },
            PreferredAppointmentTimeCode: !this.PreferredTime ? null :
                {
                    "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                    "Value": parseInt(this.PreferredTime)
                },
            va_SelfServiceNotifications: !this.SelfServiceNotifications ? null :
                {
                    "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                    "Value": parseInt(this.SelfServiceNotifications)
                },
            va_TimeZone: !this.TimeZone ? null :
                {
                    "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                    "Value": parseInt(this.TimeZone)
                },
            EMailAddress1: this.email,
            va_PrimaryPhone: !this.PreferredPhoneType ? null :
                {
                    "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                    "Value": parseInt(this.PreferredPhoneType)
                },
            va_CallerRelationtoVeteran: this.callerRelationToVeteran ?
                {
                    "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                    "Value": parseInt(this.callerRelationToVeteran)
                } : null,
            Address1_Telephone1: this.PreferredPhone,
            va_FindCorpRecordResponse: this.CorpResponse,
            va_FindBIRLSResponse: this.BirlsResponse,
            va_FindVeteranResponse: this.VeteranResponse,
            va_GeneralInformationResponse: this.GeneralInformationResponse,
            va_GeneralInformationResponsebyPID: this.GeneralInformationResponsePK,
            va_findaddressresponse: this.AddressResponse,
            va_BenefitClaimResponse: this.BenefitClaimResponse,
            va_FindBenefitDetailResponse: this.BenefitDetailResponse,
            va_FindClaimStatusResponse: this.ClaimStatusResponse,
            va_FindClaimantLettersResponse: this.ClaimantLettersResponse,
            va_FindContentionsResponse: this.ContentionsResponse,
            va_FindDependentsResponse: this.DependentsResponse,
            va_FindAllRelationshipsResponse: this.AllRelationsResponse,
            va_FindDevelopmentNotesResponse: this.DevNotesResponse,
            va_FindFiduciaryPOAResponse: this.FiduciaryPOAResponse,
            va_FindMilitaryRecordbyptcpntidResponse: this.MilitaryRecordResponse,
            va_FindPaymentHistoryResponse: this.PayHistoryResponse,
            va_FindTrackedItemsResponse: this.TrackedItemsResponse,
            va_findunsolvedevidenceresponse: this.UnsolvEvidenceResponse,
            va_FindDenialsResponse: this.DenialsResponse,
            va_FindAwardCompensationResponse: this.AwardCompResponse,
            va_FindOtherAwardInformationResponse: this.OtherAwardInfoResponse,
            va_FindMonthOfDeathResponse: this.MonthofDeathResponse,
            va_FindIncomeExpenseResponse: this.IncomeExpenseResponse,
            va_FindRatingDataResponse: this.RatingDataResponse,
            va_FindAppealsResponse: this.AppealsResponse,
            va_FindIndividualAppealsResponse: this.IndividualAppealsResponse,
            va_AppellantAddressResponse: this.AppellantAddressResponse,
            va_UpdateAppellantAddressResponse: this.UpdateAppellantAddressResponse,
            va_CreateNoteResponse: this.CreateNoteResponse,
            va_findReasonsByRbaIssueIdResponse: this.ReasonsByRbaIssueIdResponse,
            va_IsAliveResponse: this.IsAliveResponse,
            va_MVIResponse: this.MVIResponse,
            va_ReadDataExamResponse: this.ReadDataExamResponse,
            va_ReadDataAppointmentResponse: this.ReadDataAppointmentResponse,
            va_AwardFiduciaryResponse: this.AwardFiduciaryResponse,
            va_RetrievePaymentSummaryResponse: this.RetrievePaymentSummaryResponse,
            va_RetrievePaymentDetailResponse: this.RetrievePaymentDetailResponse,
            va_Flags: this.Flags,
            va_IsaVeteran: this.isaveteran,
            va_WebServiceResponseDate: this.WSUpdateDate,
            va_MorningPhone: this.MorningPhone,
            va_AfternoonPhone: this.AfternoonPhone,
            va_EveningPhone: this.EveningPhone,
            va_AutoSearch: this.contactAutoSearch,
            va_BranchofService: this.branchOfService,
            va_ICN: this.ICN,
            va_SearchIn:
                {
                    "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                    "Value": parseInt(this.va_SearchIn)
                },
            va_searchAppeals: this.va_searchAppeals,
            va_searchBIRLS: this.va_searchBIRLS,
            va_searchCorpAll: this.va_searchCorpAll,
            va_searchCorpAwards: this.va_searchCorpAwards,
            va_searchCorpClaims: this.va_searchCorpClaims,
            va_searchCorpMin: this.va_searchCorpMin,
            va_searchPathways: this.va_searchPathways,
            va_AppointmentFromDate: (this.va_AppointmentFromDate && this.va_AppointmentFromDate != undefined)
                ? this.va_AppointmentFromDate : null,
            va_AppointmentToDate: (this.va_AppointmentToDate && this.va_AppointmentToDate != undefined)
                ? this.va_AppointmentToDate : null,
            va_GenderSet:
                {
                    "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                    "Value": parseInt(this.va_GenderSet)
                },
            va_WebServiceExecutionStatus: this.WebServiceExecutionStatus,
            va_SearchOptionsObject: this.SearchOptionsObject,
            va_SearchActionsCompleted: this.SearchActionsCompleted,
            va_LastExecutedSearch: this.LastExecutedSearch
        }, false);
        contact.done(function (data) {
            contact = data.d;
        }).fail(function (err) {
            UTIL.restKitError(err, 'Failed to create dev note');
        })
        return contact;
    };
    this.update = function () {
        var sensitivityLevel = parseInt(this.sensitiveLevelOfRecord);
        var contactid = CrmRestKit2011.Update('Contact', this.id, {
            FirstName: this.firstName,
            LastName: this.lastName,
            MiddleName: this.middleName,
            va_SSN: this.ssn,
            va_FileNumber: this.fileNumber,
            va_ParticipantID: this.participantId,
            Address1_Line1: this.addressLine1,
            Address1_Line2: this.addressLine2,
            Address1_Line3: this.addressLine3,
            Address1_City: this.city,
            Address1_StateOrProvince: this.state,
            Address1_PostalCode: this.zipCode,
            Address1_Country: this.country,
            va_SensitivityLevelValue: this.sensitivityLevelValue,
            va_VeteranSensitivityLevel: this.sensitiveLevelOfRecord ?
                {
                    "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                    "Value": parseInt(this.sensitiveLevelOfRecord)
                } : null,
            va_DateofDeath: (this.dod ? (this.dod < new Date('1/1/1901') ? new Date('1/1/1901') : this.dod) : null),
            va_FindCorpRecordResponse: this.CorpResponse,
            va_FindBIRLSResponse: this.BirlsResponse,
            va_FindVeteranResponse: this.VeteranResponse,
            va_GeneralInformationResponse: this.GeneralInformationResponse,
            va_GeneralInformationResponsebyPID: this.GeneralInformationResponsePK,
            va_findaddressresponse: this.AddressResponse,
            va_BenefitClaimResponse: this.BenefitClaimResponse,
            va_FindBenefitDetailResponse: this.BenefitDetailResponse,
            va_FindClaimStatusResponse: this.ClaimStatusResponse,
            va_FindClaimantLettersResponse: this.ClaimantLettersResponse,
            va_FindContentionsResponse: this.ContentionsResponse,
            va_FindDependentsResponse: this.DependentsResponse,
            va_FindAllRelationshipsResponse: this.AllRelationsResponse,
            va_FindDevelopmentNotesResponse: this.DevNotesResponse,
            va_FindFiduciaryPOAResponse: this.FiduciaryPOAResponse,
            va_FindMilitaryRecordbyptcpntidResponse: this.MilitaryRecordResponse,
            va_FindPaymentHistoryResponse: this.PayHistoryResponse,
            va_FindTrackedItemsResponse: this.TrackedItemsResponse,
            va_findunsolvedevidenceresponse: this.UnsolvEvidenceResponse,
            va_FindDenialsResponse: this.DenialsResponse,
            va_FindAwardCompensationResponse: this.AwardCompResponse,
            va_FindOtherAwardInformationResponse: this.OtherAwardInfoResponse,
            va_FindMonthOfDeathResponse: this.MonthofDeathResponse,
            va_FindIncomeExpenseResponse: this.IncomeExpenseResponse,
            va_FindRatingDataResponse: this.RatingDataResponse,
            va_FindAppealsResponse: this.AppealsResponse,
            va_FindIndividualAppealsResponse: this.IndividualAppealsResponse,
            va_AppellantAddressResponse: this.AppellantAddressResponse,
            va_UpdateAppellantAddressResponse: this.UpdateAppellantAddressResponse,
            va_CreateNoteResponse: this.CreateNoteResponse,
            va_findReasonsByRbaIssueIdResponse: this.ReasonsByRbaIssueIdResponse,
            va_IsAliveResponse: this.IsAliveResponse,
            va_MVIResponse: this.MVIResponse,
            va_ReadDataExamResponse: this.ReadDataExamResponse,
            va_ReadDataAppointmentResponse: this.ReadDataAppointmentResponse,
            va_AwardFiduciaryResponse: this.AwardFiduciaryResponse,
            va_RetrievePaymentSummaryResponse: this.RetrievePaymentSummaryResponse,
            va_RetrievePaymentDetailResponse: this.RetrievePaymentDetailResponse,
            va_Flags: this.Flags,
            va_IsaVeteran: this.isaveteran,
            va_WebServiceResponseDate: this.WSUpdateDate,
            va_AutoSearch: this.contactAutoSearch,
            va_BranchofService: this.branchOfService,
            va_ICN: this.ICN,
            va_SearchIn:
                {
                    "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                    "Value": parseInt(this.va_SearchIn)
                },
            va_searchAppeals: this.va_searchAppeals,
            va_searchBIRLS: this.va_searchBIRLS,
            va_searchCorpAll: this.va_searchCorpAll,
            va_searchCorpAwards: this.va_searchCorpAwards,
            va_searchCorpClaims: this.va_searchCorpClaims,
            va_searchCorpMin: this.va_searchCorpMin,
            va_searchPathways: this.va_searchPathways,
            va_AppointmentFromDate: this.va_AppointmentFromDate,
            va_AppointmentToDate: this.va_AppointmentToDate,
            va_GenderSet:
                {
                    "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                    "Value": parseInt(this.va_GenderSet)
                },
            va_WebServiceExecutionStatus: this.WebServiceExecutionStatus,
            va_SearchOptionsObject: this.SearchOptionsObject,
            va_SearchActionsCompleted: this.SearchActionsCompleted,
            va_LastExecutedSearch: this.LastExecutedSearch,
            va_CallerRelationtoVeteran: this.callerRelationToVeteran ?
                {
                    "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                    "Value": parseInt(this.callerRelationToVeteran)
                } : null
        }, false);
        return contactid;
    };
    this.updateOutreachFields = function () {
        var contactid = CrmRestKit2011.Update('Contact', this.id,
            {
                va_SMSPaymentNotices: this.SMSPaymentNotices,
                va_EmailPaymentNotices: this.EmailPaymentNotices,
                va_EmailClaimNotices: this.EmailClaimNotices,
                EMailAddress1: this.email,
                va_PrimaryPhone: !this.PreferredPhoneType ? null :
                    {
                        "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                        "Value": parseInt(this.PreferredPhoneType)
                    },
                Address1_Telephone1: this.PreferredPhone,
                PreferredContactMethodCode: !this.PreferredMethodofContact ? null :
                    {
                        "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                        "Value": parseInt(this.PreferredMethodofContact)
                    },
                PreferredAppointmentDayCode: !this.PreferredDay ? null :
                    {
                        "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                        "Value": parseInt(this.PreferredDay)
                    },
                PreferredAppointmentTimeCode: !this.PreferredTime ? null :
                    {
                        "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                        "Value": parseInt(this.PreferredTime)
                    },
                va_SelfServiceNotifications: !this.SelfServiceNotifications ? null :
                    {
                        "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                        "Value": parseInt(this.SelfServiceNotifications)
                    },
                va_TimeZone: !this.TimeZone ? null :
                    {
                        "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.OptionSetValue" },
                        "Value": parseInt(this.TimeZone)
                    },
                va_MorningPhone: this.MorningPhone,
                va_AfternoonPhone: this.AfternoonPhone,
                va_EveningPhone: this.EveningPhone
            });
        return contactid;
    };
    this.toShortDateString = function (dval) {
        if (!dval) return '';
        return (dval.getMonth() + 1).toString() + '/' + dval.getDate().toString() + '/' + dval.getFullYear().toString();
    };
    this.getCorpFlags = function (flagType) {
        var flags = '';
        var sep = flagType == 1 ? '; ' : '\n';
        var dod = '';
        var flash = (this.flash && this.flash.length > 0 ? 'Flash:' + this.flash : null);
        var comp = this.IsIncompetent() ? 'Incompetent: ' + this.competencyFlag : null;
        var branch = (this.branchOfService ? 'Branch: ' + this.branchOfService : null);

        var poa = null;
        // if (this.poaCode2 && this.poaCode2.length > 0) {
        // if (!POA || POA.length == 0)
        // POA = 'POA:';
        // else
        // POA += '; ';
        // POA += this.poaCode2;
        // }

        var xxx = new Date();
        var yyy = (xxx.getMonth() + 1) + '/' + xxx.getDate() + '/' + xxx.getFullYear();

        if (this.poaEndDate && new Date(this.poaEndDate) < new Date(yyy)) {
            poa = '';
        }
        else {
            poa = (this.poaCode1 && this.poaCode1.length > 0 ? 'POA: ' + this.poaCode1 : null);
        }

        var openClaims = '';
        flags = comp ? comp + sep : '';
        //flags += flash ? flash + sep : '';
        flags += branch ? branch + sep : '';
        flags += openClaims;
        if (this.dod && !isNaN(this.dod)) dod = "DOD: " + this.toShortDateString(this.dod) + sep;
        if (this.causeOfDeath) dod += "COD: " + this.causeOfDeath + sep;
        flags = dod + flags;
        if (this.hasOpenAppeals()) flags += "Open Appeals" + sep;
        if (this.claimStatus && this.claimStatus.OpenClaimCount) {
            flags += this.claimStatus.OpenClaimCount.toString() + " Open Claims" + sep;
        }
        //if (flags && flags.length > 0 && flags[flags.length - 1] == ';') flags = flags.substr(flags.length - 1);
        flags += poa ? poa : '';
        return flags;
    };
    this.getBIRLSFlags = function (flagType) {
        var flags = '';
        var sep = flagType == 1 ? '; ' : '\n';
        var dod = '';
        var comp = this.IsIncompetent() ? 'Incompetent: ' + this.competencyFlag : null;
        var branch = (this.branchOfService ? 'Branch: ' + this.branchOfService : null);
        var poa = (this.poaCode1 && this.poaCode1.length > 0 ? 'POA: ' + this.poaCode1 : null);
        if (this.poaCode2 && this.poaCode2.length > 0) {
            if (!poa || poa.length == 0)
                poa = 'POA:';
            else
                poa += '; ';
            poa += this.poaCode2;
        }
        var flash = (this.flash && this.flash.length > 0 ? 'Flash:' + this.flash : null);
        var openClaims = '';
        flags = comp ? comp + sep : '';
        //flags += flash ? flash + sep : '';
        flags += branch ? branch + sep : '';
        flags += openClaims;
        if (this.dod && !isNaN(this.dod)) dod = "DOD: " + this.toShortDateString(this.dod) + sep;
        if (this.causeOfDeath) dod += "COD: " + this.causeOfDeath + sep;
        flags = dod + flags;
        if (this.claimStatus && this.claimStatus.OpenClaimCount) {
            flags += this.claimStatus.OpenClaimCount.toString() + " Open Claims" + sep;
        }
        flags += poa ? poa : '';
        return flags;
    };
    this.getFlashes = function (xmlBirlsInfo, xmlGeneralInfo) {
        var flashes = '';
        var nodes = null;
        if (xmlGeneralInfo)
            nodes = xmlGeneralInfo.selectNodes('//flashes');
        else if (xmlBirlsInfo)
            nodes = xmlBirlsInfo.selectNodes('//FLASH');
        if (nodes) {
            for (var i = 0; i < nodes.length; i++) {
                var subNode = nodes[i];
                if (subNode) {
                    var txt = subNode.text;
                    if (txt && txt.length > 0) flashes += (flashes.length > 0 ? '; ' : '') + txt;
                }
            }
        }
        return flashes;
    };
    this.hasOpenAppeals = function () {
        if (!Xrm.Page.getAttribute('va_findindividualappealsresponse')) return false;
        var val = Xrm.Page.getAttribute('va_findindividualappealsresponse').getValue();
        if (!val) return false;
        var xmlObject = parseXmlObject(val);
        var node = xmlObject.selectSingleNode('//AppealStatusCode');
        if (!node) return false;
        var txt = node.text;
        return (txt != 'HIS');
    };
    this.crmDateNotSupported = function (dateVal) {
        if (!dateVal || dateVal == undefined) {
            return true;
        }
        try {
            var dt = new Date(dateVal);
        }
        catch (ex) {
            return true;
        }
        return (dt < new Date('1/1/1901'));
    };
};
contact.prototype.constructor = contact;
contact.prototype.id;
contact.prototype.firstName;
contact.prototype.lastName;
contact.prototype.middleName;
contact.prototype.ssn;
contact.prototype.fileNumber;
contact.prototype.participantId;
contact.prototype.dob;
contact.prototype.dod;
contact.prototype.causeOfDeath;
contact.prototype.dou;
contact.prototype.poaCode1;
contact.prototype.poaCode2;
contact.prototype.addressLine1;
contact.prototype.addressLine2;
contact.prototype.addressLine3;
contact.prototype.city;
contact.prototype.state;
contact.prototype.zipCode;
contact.prototype.country;
contact.prototype.sensitiveLevelOfRecord;
contact.prototype.sensitivityLevelValue;
contact.prototype.SMSPaymentNotices;
contact.prototype.EmailPaymentNotices;
contact.prototype.EmailClaimNotices;
contact.prototype.PreferredMethodofContact;
contact.prototype.PreferredDay;
contact.prototype.PreferredTime;
contact.prototype.SelfServiceNotifications;
contact.prototype.email;
contact.prototype.PreferredPhone;
contact.prototype.MorningPhone;
contact.prototype.AfternoonPhone;
contact.prototype.EveningPhone;
contact.prototype.PreferredPhoneType;
contact.prototype.HasEbenefitsAccount;
contact.prototype.TimeZone;
contact.prototype.CorpFlags;
contact.prototype.BIRLSFlags;
contact.prototype.flashes;
contact.prototype.gender;
contact.prototype.competencyFlag;
contact.prototype.flash;
contact.prototype.branchOfService;
contact.prototype.isaveteran;
contact.prototype.WSUpdateDate;
contact.prototype.claimStatus;
contact.prototype.contactAutoSearch;
contact.prototype.currentFiduciary;
contact.prototype.currentPowerOfAttorney;
contact.prototype.folderInfo;
contact.prototype.ICN;
contact.prototype.callerRelationToVeteran;
contact.prototype.va_StaionofJurisdictionId;
//
// Search In Fields
//
contact.prototype.va_SearchIn;
contact.prototype.va_searchAppeals;
contact.prototype.va_searchBIRLS;
contact.prototype.va_searchCorpAll;
contact.prototype.va_searchCorpAwards;
contact.prototype.va_searchCorpClaims;
contact.prototype.va_searchCorpMin;
contact.prototype.va_searchPathways;
contact.prototype.va_GenderSet;
contact.prototype.va_AppointmentFromDate;
contact.prototype.va_AppointmentToDate;
//
// Response fields 
//
contact.prototype.CorpResponse;
contact.prototype.BirlsResponse;
contact.prototype.VeteranResponse;
contact.prototype.GeneralInformationResponse;
contact.prototype.GeneralInformationResponsePK;
contact.prototype.AddressResponse;
contact.prototype.BenefitClaimResponse;
contact.prototype.BenefitDetailResponse;
contact.prototype.ClaimStatusResponse;
contact.prototype.ClaimantLettersResponse;
contact.prototype.ContentionsResponse;
contact.prototype.DependentsResponse;
contact.prototype.AllRelationsResponse;
contact.prototype.DevNotesResponse;
contact.prototype.FiduciaryPOAResponse;
contact.prototype.MilitaryRecordResponse;
contact.prototype.PayHistoryResponse;
contact.prototype.TrackedItemsResponse;
contact.prototype.UnsolvEvidenceResponse;
contact.prototype.DenialsResponse;
contact.prototype.AwardCompResponse;
contact.prototype.OtherAwardInfoResponse;
contact.prototype.MonthofDeathResponse;
contact.prototype.IncomeExpenseResponse;
contact.prototype.RatingDataResponse;
contact.prototype.AppealsResponse;
contact.prototype.IndividualAppealsResponse;
contact.prototype.AppellantAddressResponse;
contact.prototype.UpdateAppellantAddressResponse;
contact.prototype.CreateNoteResponse;
contact.prototype.ReasonsByRbaIssueIdResponse;
contact.prototype.IsAliveResponse;
contact.prototype.MVIResponse;
contact.prototype.ReadDataExamResponse;
contact.prototype.ReadDataAppointmentResponse;
contact.prototype.BIRLSEmployeeNumber;
contact.prototype.BIRLSEmployeeStation;
contact.prototype.AwardFiduciaryResponse;
contact.prototype.RetrievePaymentDetailResponse;
contact.prototype.RetrievePaymentSummaryResponse;

contact.prototype.WebServiceExecutionStatus;
contact.prototype.SearchOptionsObject;
contact.prototype.SearchActionsCompleted;
contact.prototype.LastExecutedSearch;

//////////////////////////////////
// ClaimStatus
/////////////////////////////////
var ClaimStatus = function () {
    this.AnalyzeClaimRecordset = function (xml) {
        this.OpenClaimCount = 0;
        if (xml) {
            var xmlObject = parseXmlObject(xml);
            var nodes = xmlObject.selectNodes('//selection');
            if (nodes) {
                for (var i = 0; i < nodes.length; i++) {
                    var subNode = nodes[i].selectSingleNode('statusTypeCode'), s = '';
                    if (subNode && subNode.text) {
                        s = subNode.text.toUpperCase();
                        if (s == 'PEND' || s == 'RFD' || s == 'SRFD' || s == 'RI') {
                            this.OpenClaimCount++;
                        }
                    }
                }
            }
        }
    };
    this.AnalyzeTrackedItems = function (xml) {
        // open if no Err or Received receiveDt or Closed dates
        // Per Medha and Ravi, item is open if it is NOT Suspended, Received or Closed
        this.TrackedItemStatusList = null;
        if (xml) {
            var XmlObject = parseXmlObject(xml);
            var nodes = XmlObject.selectNodes('//dvlpmtItems');
            if (nodes) {
                this.TrackedItemStatusList = new Array();
                for (var i = 0; i < nodes.length; i++) {
                    // check if current claim is already in the list
                    var claim = nodes[i].selectSingleNode('claimId');
                    if (!claim || !claim.text) {
                        continue;
                    } // blank claim id, should never happen
                    var claimId = claim.text;
                    var currentTrackedItemStatus = null;
                    if (this.TrackedItemStatusList[claimId])
                        currentTrackedItemStatus = this.TrackedItemStatusList[claimId];
                    else {
                        currentTrackedItemStatus = new TrackedItemStatus();
                        currentTrackedItemStatus.Count = 0;
                        currentTrackedItemStatus.Count_Open = 0;
                        currentTrackedItemStatus.Count_Closed = 0;
                        currentTrackedItemStatus.Count_Suspended = 0;
                        currentTrackedItemStatus.Count_Received = 0;
                        currentTrackedItemStatus.Summary = '';
                    }
                    var susp = nodes[i].selectSingleNode('suspnsDt');
                    var rec = nodes[i].selectSingleNode('receiveDt');
                    var closed = nodes[i].selectSingleNode('acceptDt');
                    var isSusp = false;
                    var isRec = false;
                    var isClosed = false;
                    if (susp && susp.text) {
                        currentTrackedItemStatus.Count_Suspended++;
                        isSusp = true;
                    }
                    if (rec && rec.text) {
                        currentTrackedItemStatus.Count_Received++;
                        isRec = true;
                    }
                    if (closed && closed.text) {
                        currentTrackedItemStatus.Count_Closed++;
                        isClosed = true;
                    }
                    if (!isSusp && !isRec && !isClosed) {
                        currentTrackedItemStatus.Count_Open++;
                    }
                    currentTrackedItemStatus.Count++;
                    this.TrackedItemStatusList[claimId] = currentTrackedItemStatus;
                }
                // go through results and format summary lines
                for (var item in this.TrackedItemStatusList) {
                    this.TrackedItemStatusList[item].Summary = this.TrackedItemStatusList[item].GetSummary('; ');
                }
            }
        }
    };
};
ClaimStatus.prototype.OpenClaimCount;
ClaimStatus.prototype.TrackedItemStatusList;
//////////////////////////////////
// TrackedItemStatus
/////////////////////////////////
var TrackedItemStatus = function () {
    this.GetSummary = function (sep) {
        this.Summary =
            "Trckd Items: " + this.Count +
                (this.Count_Open ? sep + "Open: " + this.Count_Open : "") +
                    (this.Count_Closed ? sep + "Closed:" + this.Count_Closed : "") +
                        (this.Count_Suspended ? sep + "Susp: " + this.Count_Suspended : "") +
                            (this.Count_Received ? sep + "Rcd: " + this.Count_Received : "");
        return this.Summary;
    };
};
TrackedItemStatus.prototype.Count;
TrackedItemStatus.prototype.Count_Closed;
TrackedItemStatus.prototype.Count_Suspended;
TrackedItemStatus.prototype.Count_Received;
TrackedItemStatus.prototype.Count_Open;
TrackedItemStatus.prototype.Summary;
//////////////////////////////////
// PaymentHistoryStatus
/////////////////////////////////
var PaymentHistoryStatus = function () {
    this.AnalyzePaymentHistory = function (xml) {
        this.PaymentsList = null;
        //this.ReturnPaymentsList = null;
        if (xml) {
            var XmlObject = parseXmlObject(xml);
            var nodes = XmlObject.selectNodes('//payments');
            if (nodes) {
                this.PaymentsList = new Array();
                //this.ReturnPaymentsList = new Array();
                for (var i = 0; i < nodes.length; i++) {
                    var PayCheckType = nodes[i].selectSingleNode('payCheckType');
                    var PayCheckAmount = nodes[i].selectSingleNode('payCheckAmount');
                    if (!PayCheckType || !PayCheckAmount) {
                        continue;
                    }
                    if (!PayCheckType.text && !PayCheckAmount.text) {
                        continue;
                    }
                    var type = PayCheckType.text;
                    var amount = PayCheckAmount.text ? parseFloat(PayCheckAmount.text.replace('$', '').replace(',', '')) : 0;
                    var currentItem = null;
                    if (this.PaymentsList[type]) {
                        currentItem = this.PaymentsList[type];
                        currentItem += amount;
                        this.PaymentsList[type] = currentItem;
                    } else {
                        this.PaymentsList[type] = amount;
                    }
                }
                // now go through ReturnPayments
                var amount = 0;
                for (var i = 0; i < nodes.length; i++) {
                    var PayCheckAmount = nodes[i].selectSingleNode('returnedCheckAmount');
                    if (!PayCheckAmount || !PayCheckAmount.text) {
                        continue;
                    }
                    amount += (PayCheckAmount.text.toString().trim() ? parseFloat(PayCheckAmount.text.replace('$', '')) : 0);
                }
                this.PaymentsList["returned"] = amount;
                // go through results and format summary lines 
                this.Summary = '';
                for (var item in this.PaymentsList) {
                    this.Summary += (this.Summary.length > 0 ? '; ' : '') + item + ': ' + formatCurrency(this.PaymentsList[item]);
                }
            }
        }
    };
};
PaymentHistoryStatus.prototype.PaymentsList;
//PaymentHistoryStatus.prototype.ReturnPaymentsList;
PaymentHistoryStatus.prototype.Count;
PaymentHistoryStatus.prototype.Count_Returned;
PaymentHistoryStatus.prototype.Total;
PaymentHistoryStatus.prototype.Total_Returned;
PaymentHistoryStatus.prototype.Summary;