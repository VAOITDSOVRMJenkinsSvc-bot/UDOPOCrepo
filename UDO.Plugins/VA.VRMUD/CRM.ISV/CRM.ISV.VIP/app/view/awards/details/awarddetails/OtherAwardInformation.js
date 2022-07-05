/**
* @author Jonas Dawson
* @class VIP.view.awards.details.awarddetails.OtherAwardInformation
*
* Other award details
*/
Ext.define('VIP.view.awards.details.awarddetails.OtherAwardInformation', {
    extend: 'Ext.form.Panel',
    alias: 'widget.awards.details.awarddetails.otherawardinformation',
    title: 'Other Award Information (Select Award to Populate)',
    layout: {
        type: 'table',
        columns: 4,
        tableAttrs: {
            style: {
                width: '100%'
            }
        },
        tdAttrs: {
            width: '25%'
        }
    },
    defaults: {
        labelStyle: 'font-weight:bold;font-size:11px;',
        fieldStyle: 'font-size:11px;',
        labelWidth: 160,
        width: 300
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [
        // Fields from GeneralDetails model
        {
        xtype: 'displayfield',
        fieldLabel: 'Award Type Code',
        name: 'awardTypeCode'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Benefit Type Code',
        name: 'benefitTypeCode'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Ptcpnt Recip Id',
        name: 'participantRecipientId'
    },  {
        xtype: 'displayfield',
        fieldLabel: 'Direct Deposit Id',
        name: 'directDepositAccountID'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Clothing Allowance Code',
        name: 'clothingAllowanceTypeCode'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Clothing Allowance Name',
        name: 'clothingAllowanceTypeName'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Ptcpnt Bene Id',
        name: 'participantBeneId'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Status Reason Code',
        name: 'statusReasonTypeCode'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Paid Through',
        name: 'paidThroughDate_F'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Pay Status Code',
        name: 'payStatusCd'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Pay Status Name',
        name: 'payStatusName'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Status Reason Name',
        name: 'statusReasonTypeName'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'POA',
        name: 'powerOfAttorney',
        width: 450,
        colspan: 2
    }, {
        xtype: 'displayfield',
        fieldLabel: 'SOJ',
        name: 'stationOfJurisdiction',
        id: 'stationOfJurisdiction'
    },  {
        xtype: 'displayfield',
        fieldLabel: 'Status Reason Date',
        name: 'statusReasonDate_F'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Funds Incompetent',
        name: 'fundsDueIncompetentBalance' // TODO: format fundsDueIncompetentBalance
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Funds of Patient',
        name: 'personalFundsOfPatientBalance' // TODO: format personalFundsOfPatientBalance
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Payment Address Id',
        name: 'paymentAddressID'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Curr Monthly Rate',
        name: 'currentMonthlyRate',
        id: 'currentMonthlyRate'
    },
    // Fields from AwardInfo model
        {
        xtype: 'displayfield',
        fieldLabel: 'IG Reference Num',
        name: 'igReferenceNbr'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Requested Frequency',
        name: 'requestedFrequency'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Frequency',
        name: 'frequencyName'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Retroactive Date',
        name: 'retroactiveDate_F' 
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Last Paid Date',
        name: 'lastPaidDate_F' 
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Benefit Type',
        name: 'bnftName',
        width: 450,
        colspan: 2
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Beneficiary Full Name',
        name: 'beneficiaryFullName',//custom field
        width: 450,
        colspan: 2
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Beneficiary Type',
        name: 'beneName'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Audit Related A/R',
        name: 'auditRelatedAr'
    }];

    me.callParent();
}
});