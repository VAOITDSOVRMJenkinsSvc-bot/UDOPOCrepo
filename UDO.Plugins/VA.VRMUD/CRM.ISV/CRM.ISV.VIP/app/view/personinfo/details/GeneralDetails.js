/**
* @class VIP.view.personinfo.details.GeneralDetails
* The view for person general detail information
*/
Ext.define('VIP.view.personinfo.details.GeneralDetails', {
    extend: 'Ext.form.Panel',
    alias: 'widget.personinfo.details.generaldetails',
    store: 'personinfo.GeneralDetails',
    title: 'General Details',
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
        labelWidth: 130,
        width: 250
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'Ptcpnt Id',
            name: 'participantId',
            id: 'participantId'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Sex',
            name: 'vetSex'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'EOD',
            name: 'enteredOnDutyDate_F'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'RAD',
            name: 'releasedActiveDutyDate_F'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Add\'l Service',
            name: 'additionalServiceInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'GW Registry',
            name: 'gulfWarRegistryIndicator'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Desert Shield',
            name: 'desertShieldInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Paraplegic Housing #',
            name: 'paraplegicHousingNumber'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Competency Dec Cd',
            name: 'competencyDecisionTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Competency Decision',
            name: 'competencyDecisionTypeName'
        },
        //        { THIS DOESNT POPULATE UNLESS AN AWARD IS SELECTED AND AWARD TYPE CODE IS SPECIFIED
        //            xtype: 'displayfield',
        //            fieldLabel: 'mailingAddressID',
        //            name: 'mailingAddressID'
        //        }, 
        {
            xtype: 'displayfield',
            fieldLabel: 'Nursing Home',
            name: 'nursingHomeInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Nursing Home Name',
            name: 'nursingHomeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'FID Category',
            name: 'fiduciaryCategory'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Guardian Folder Loc',
            name: 'guardianFolderLocation'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Converted Case',
            name: 'convertedCaseInd'
        }, {
            xtype: 'fieldcontainer',
            fieldLabel: 'Claim Folder Loc',
            items: {
                xtype: 'displayfield',
                //action: 'claimfolderlocation'
                name: 'claimFolderLocation'
            }
        }, {
            xtype: 'displayfield',
            fieldLabel: 'eBenefits Status',
            name: 'registrationStatus'
        }];

        me.callParent();
    }
});