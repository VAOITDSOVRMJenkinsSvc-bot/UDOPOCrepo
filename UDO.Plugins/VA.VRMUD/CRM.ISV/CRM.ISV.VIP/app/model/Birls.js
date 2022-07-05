/**
* @author Ivan Yurisevic
* @class VIP.model.Birls
*
* The model for BIRLS record details
*/
Ext.define('VIP.model.Birls', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.share.vetrecord.FindBirlsRecord',
        'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber',
        'VIP.data.reader.Birls',
        'VIP.model.birls.AlternateNames',
        'VIP.model.birls.Disclosures',
        'VIP.model.birls.Flashes',
        'VIP.model.birls.FolderLocations',
        'VIP.model.birls.InsurancePolicy',
        'VIP.model.birls.MilitaryService',
        'VIP.model.birls.ServiceDiagnostics'
    ],
    fields: [{
        name: 'ssn',
        mapping: 'SOC_SEC_NUMBER',
        type: 'string',
        ignoreMappingOnRequest: true
    }, {
        name: 'insurancePrefix',
        mapping: 'INS_PREFIX',
        type: 'string',
        ignoreMappingOnRequest: true
    }, {
        name: 'fileNumber',
        mapping: 'CLAIM_NUMBER',
        type: 'string',
        ignoreMappingOnRequest: true
    }, {
        name: 'insuranceFileNumber',
        mapping: 'INS_NUMBER',
        type: 'string',
        ignoreMappingOnRequest: true
    }, {
        name: 'lastName',
        mapping: 'LAST_NAME',
        type: 'string',
        ignoreMappingOnRequest: true
    }, {
        name: 'firstName',
        mapping: 'FIRST_NAME',
        type: 'string',
        ignoreMappingOnRequest: true
    }, {
        name: 'middleName',
        mapping: 'MIDDLE_NAME',
        type: 'string',
        ignoreMappingOnRequest: true
    }, {
        name: 'nameSuffix',
        mapping: 'NAME_SUFFIX',
        type: 'string'
    }, {
        name: 'dob',
        mapping: 'DATE_OF_BIRTH',
        type: 'string',
        ignoreMappingOnRequest: true
    }, {
        name: 'dod',
        mapping: 'DATE_OF_DEATH',
        type: 'string',
        ignoreMappingOnRequest: true
    }, {
        name: 'powNumberOfDays',
        mapping: 'POW_NUMBER_OF_DAYS',
        type: 'string'
    }, {
        name: 'activeServiceYears',
        mapping: 'TOTAL_ACTIVE_SERVICE_YEARS',
        type: 'string'
    }, {
        name: 'activeServiceMonths',
        mapping: 'TOTAL_ACTIVE_SERVICE_MONTHS',
        type: 'string'
    }, {
        name: 'activeServiceDays',
        mapping: 'TOTAL_ACTIVE_SERVICE_DAYS',
        type: 'string'
    }, {
        name: 'disabilitySeverancePay',
        mapping: 'DISABILITY_SEVERANCE_PAY',
        type: 'string'
    }, {
        name: 'lumpSumReadjustmentPay',
        mapping: 'LUMP_SUM_READJUSTMENT_PAY',
        type: 'string'
    }, {
        name: 'separationPay',
        mapping: 'SEPARATION_PAY',
        type: 'string'
    }, {
        name: 'claimFolderLocation',
        mapping: 'CLAIM_FOLDER_LOCATION',
        type: 'string'
    }, {
        name: 'vetHasBeneInd',
        mapping: 'VET_HAS_BENE_IND',
        type: 'string'
    }, {
        name: 'vetIsBeneInd',
        mapping: 'VET_IS_BENE_IND',
        type: 'string'
    }, {
        name: 'purpleHeartInd',
        mapping: 'PURPLE_HEART_IND',
        type: 'string'
    }, {
        name: 'verifiedSsnInd',
        mapping: 'VERIFIED_SOC_SEC_IND',
        type: 'string'
    }, {
        name: 'vaEmployeeInd',
        mapping: 'VA_EMPLOYEE_IND',
        type: 'string'
    }, {
        name: 'vietnamServiceInd',
        mapping: 'VIETNAM_SERVICE_IND',
        type: 'string'
    }, {
        name: 'disabilityInd',
        mapping: 'DISABILITY_IND',
        type: 'string'
    }, {
        name: 'medalOfHonorInd',
        mapping: 'MEDAL_OF_HONOR_IND',
        type: 'string'
    }, {
        name: 'transferToReservesInd',
        mapping: 'TRANSFER_TO_RESERVES_IND',
        type: 'string'
    }, {
        name: 'activeDutyTrainingInd',
        mapping: 'ACTIVE_DUTY_TRAINING_IND',
        type: 'string'
    }, {
        name: 'reenlistedInd',
        mapping: 'REENLISTED_IND',
        type: 'string'
    }, {
        name: 'burialFlagIssueInd',
        mapping: 'BURIAL_FLAG_ISSUE_IND',
        type: 'string'
    }, {
        name: 'gender',
        mapping: 'SEX_CODE',
        type: 'string'
    }, {
        name: 'contestedDataInd',
        mapping: 'CONTESTED_DATA_IND',
        type: 'string'
    }, {
        name: 'guardianshipCaseInd',
        mapping: 'GUARDIANSHIP_CASE_IND',
        type: 'string'
    }, {
        name: 'incompetentInd',
        mapping: 'INCOMPETENT_IND',
        type: 'string'
    }, {
        name: 'compensationPensionVeteranInd',
        mapping: 'CP_VET_CP_BENE_IND',
        type: 'string'
    }, {
        name: 'vadsInd',
        mapping: 'VADS_IND',
        type: 'string'
    }, {
        name: 'verifiedSvcDataInd',
        mapping: 'VERIFIED_SVC_DATA_IND',
        type: 'string'
    }, {
        name: 'ch30Ind',
        mapping: 'CH30_IND',
        type: 'string'
    }, {
        name: 'ch32BankInd',
        mapping: 'CH32_BANK_IND',
        type: 'string'
    }, {
        name: 'ch32BeneficiaryInd',
        mapping: 'CH32_BEN_IND',
        type: 'string'
    }, {
        name: 'ch34Ind',
        mapping: 'CH34_IND',
        type: 'string'
    }, {
        name: 'ch106Ind',
        mapping: 'CH106_IND',
        type: 'string'
    }, {
        name: 'ch31Ind',
        mapping: 'CH31_IND',
        type: 'string'
    }, {
        name: 'ch32_903_Ind',
        mapping: 'CH32_903_IND',
        type: 'string'
    }, {
        name: 'ind901',
        mapping: 'IND_901',
        type: 'string'
    }, {
        name: 'jobsInd',
        mapping: 'JOBS_IND',
        type: 'string'
    }, {
        name: 'varmsInd',
        mapping: 'VARMS_IND',
        type: 'string'
    }, {
        name: 'diagsVerifiedInd',
        mapping: 'DIAGS_VERIFIED_IND',
        type: 'string'
    }, {
        name: 'homelessVetInd',
        mapping: 'HOMELESS_VET_IND',
        type: 'string'
    }, {
        name: 'returnServiceInd',
        mapping: 'RET_SVR_IND',
        type: 'string'
    }, {
        name: 'persianGulfServiceInd',
        mapping: 'PERSIAN_GULF_SVC_IND',
        type: 'string'
    }, {
        name: 'serviceMedicalRecordInd',
        mapping: 'SVC_MED_RECORD_IND',
        type: 'string'
    }, {
        name: 'bankruptcyInd',
        mapping: 'BANKRUPTCY_IND',
        type: 'string'
    }, {
        name: 'causeOfDeath',
        mapping: 'CAUSE_OF_DEATH',
        type: 'string'
    }, {
        name: 'deathInService',
        mapping: 'DEATH_IN_SVC',
        type: 'string'
    }, {
        name: 'poaCode1',
        mapping: 'POWER_OF_ATTY_CODE1',
        type: 'string'
    }, {
        name: 'poaCode2',
        mapping: 'POWER_OF_ATTY_CODE2',
        type: 'string'
    }, {
        name: 'clothingAllowance',
        mapping: 'CLOTHING_ALLOWANCE',
        type: 'string'
    }, {
        name: 'numberOfServiceConnectedDiagnostics',
        mapping: 'NUM_OF_SVC_CON_DIS',
        type: 'string'
    }, {
        name: 'burialAwardPlot',
        mapping: 'BURIAL_AWARD_PLOT',
        type: 'string'
    }, {
        name: 'burialAwardTransport',
        mapping: 'BURIAL_AWARD_TRANSPORT',
        type: 'string'
    }, {
        name: 'headstone',
        mapping: 'HEADSTONE',
        type: 'string'
    }, {
        name: 'payment',
        mapping: 'PAYMENT',
        type: 'string'
    }, {
        name: 'applicationForPlot',
        mapping: 'APPLICATION_FOR_PLOT',
        type: 'string'
    }, {
        name: 'adaptiveEquipment',
        mapping: 'ADAPTIVE_EQUIPMENT',
        type: 'string'
    }, {
        name: 'specialAdaptiveHousing',
        mapping: 'SPECIAL_ADAPTIVE_HOUSING',
        type: 'string'
    }, {
        name: 'reasonForTermDisallow',
        mapping: 'REASON_FOR_TERM_DISALLOW',
        type: 'string'
    }, {
        name: 'entitlementCode',
        mapping: 'ENTITLEMENT_CODE',
        type: 'string'
    }, {
        name: 'specialLawCode',
        mapping: 'SPECIAL_LAW_CODE',
        type: 'string'
    }, {
        name: 'cpEffectiveDateOfTerm',
        mapping: 'CP_EFFCTVE_DATE_OF_TERM',
        type: 'string'
    }, {
        name: 'burialAwardServiceConnected',
        mapping: 'BURIAL_AWD_SVC_CONNECT',
        type: 'string'
    }, {
        name: 'burialAwardNonServiceConnected',
        mapping: 'BURIAL_AWD_NONSVC_CON',
        type: 'string'
    }, {
        name: 'automobileAllowance',
        mapping: 'AUTOMOBILE_ALLOWANCE',
        type: 'string'
    }, {
        name: 'combinedDegree',
        mapping: 'COMBINED_DEGREE',
        type: 'string'
    }, {
        name: 'additionalDiagnosticsInd',
        mapping: 'ADD_DIA_IND',
        type: 'string'
    }, {
        name: 'employeeNumber',
        mapping: 'EMPLOYEE_NUMBER',
        type: 'string'
    }, {
        name: 'employeeStationNumber',
        mapping: 'EMPLOYEE_STATION_NUMBER',
        type: 'string'
    }, {
        name: 'dateOfUpdate',
        mapping: 'DATE_OF_UPDATE',
        type: 'string'
    }, {
        name: 'numberOfDisclosures',
        mapping: 'NUMBER_OF_DISCLOSURES',
        type: 'string'
    }, {
        name: 'insuranceJuris',
        mapping: 'INSURANCE_JURIS',
        type: 'string'
    }, {
        name: 'dateOfInsLapsedPurge',
        mapping: 'DATE_OF_INS_LAPSED_PURGE',
        type: 'string'
    }, {
        name: 'ch30Overpayment',
        mapping: 'CH30_OVERPAYMENT',
        type: 'string'
    }, {
        name: 'currentRetirePayDateSBP',
        mapping: 'DATE_OF_DMDC_RETIRE_PAY_C',
        type: 'string'
    }, {
        name: 'priorPayDateSBP',
        mapping: 'DATE_OF_DMDC_RETIRE_PAY_P',
        type: 'string'
    }, {
        name: 'vadsInd2',
        mapping: 'VADS_IND2',
        type: 'string'
    }, {
        name: 'vadsInd3',
        mapping: 'VADS_IND3',
        type: 'string'
    }, {
        name: 'verifiedSvcDataInd2',
        mapping: 'VERIFIED_SVC_DATA_IND2',
        type: 'string'
    }, {
        name: 'verifiedSvcDataInd3',
        mapping: 'VERIFIED_SVC_DATA_IND3',
        type: 'string'
    }, {
        name: 'serviceNumberEditFiller',
        mapping: 'SVC_NUM_EDIT_FILLER',
        type: 'string'
    }, {
        name: 'pvrMonth',
        mapping: 'PVR_MONTH',
        type: 'string'
    }, {
        name: 'pvrDay',
        mapping: 'PVR_DAY',
        type: 'string'
    }, {
        name: 'pvrCentury',
        mapping: 'PVR_CENTURY',
        type: 'string'
    }, {
        name: 'pvrYear',
        mapping: 'PVR_YEAR',
        type: 'string'
    }, {
        name: 'pvrFiller1',
        mapping: 'PVR_FILLER1',
        type: 'string'
    }, {
        name: 'appealsInd',
        mapping: 'APPEALS_IND',
        type: 'string'
    }, {
        name: 'inTheaterStartDate',
        mapping: 'IN_THEATER_START_DATE',
        type: 'string'
    }, {
        name: 'inTheaterEndDate',
        mapping: 'IN_THEATER_END_DATE',
        type: 'string'
    }, {
        name: 'inTheaterDays',
        mapping: 'IN_THEATER_DAYS',
        type: 'string'
    }, {
        name: 'priorRetirePaySBP',
        mapping: 'DMDC_RETIRE_PAY_SBP_AMT_P',
        type: 'string'
    }, {
        name: 'currentRetirePaySBP',
        mapping: 'DMDC_RETIRE_PAY_SBP_AMT_C',
        type: 'string'
    }, {
        name: 'returnCode',
        mapping: 'RETURN_CODE',
        type: 'string'
    }, {
        name: 'returnMessage',
        mapping: 'RETURN_MESSAGE',
        type: 'string'
    }, {
        name: 'fullName',
        convert: function (v, record) {
            var firstName = record.get('firstName'),
                middleName = record.get('middleName'),
                lastName = record.get('lastName'),
                suffix = record.get('nameSuffix'),
                fullname = '';

            fullname = (!Ext.isEmpty(lastName) ? lastName : '')
                    + (!Ext.isEmpty(firstName) ? ', ' + firstName : '')
                    + (!Ext.isEmpty(middleName) ? ' ' + middleName : '')
                    + (!Ext.isEmpty(suffix) ? ' ' + suffix : '');
            return fullname;
        }
    }],

    hasMany: [
        {
            model: 'VIP.model.birls.AlternateNames',
            name: 'alternateNames',
            associationKey: 'alternateNames'
        },
        {
            model: 'VIP.model.birls.Disclosures',
            name: 'recurringDisclosure',
            associationKey: 'recurringDisclosure'
        },
        {
            model: 'VIP.model.birls.Flashes',
            name: 'flashes',
            associationKey: 'flashes'
        },
        {
            model: 'VIP.model.birls.FolderLocations',
            name: 'folders',
            associationKey: 'folders'
        },
        {
            model: 'VIP.model.birls.InsurancePolicy',
            name: 'insurancePolicies',
            associationKey: 'insurancePolicies'
        },
        {
            model: 'VIP.model.birls.MilitaryService',
            name: 'services',
            associationKey: 'services'
        },
        {
            model: 'VIP.model.birls.ServiceDiagnostics',
            name: 'serviceDiagnostic',
            associationKey: 'serviceDiagnostic'
        }
    ],    

    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'birls',
            record: 'return'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.vetrecord.FindBirlsRecord',//check Birls store.  This changes if we have have a fileNumber
            update: '',
            destroy: ''
        }
    }
}); 
