/**
* @author Ivan Yurisevic
* @class VIP.view.birls.IdentificationData
*
* The view for the BIRLS fieldset at the top section
*/
Ext.define('VIP.view.birls.IdentificationData', {
    extend: 'Ext.form.Panel',
    alias: 'widget.birls.identificationdata',
    title: 'Identification Data',
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
        labelWidth: 120,
        width: 250
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [{ //ROW 1
            xtype: 'displayfield',
            fieldLabel: 'Name',
            name: 'fullName',
            colspan: 2,
            width: 400
        }, {
            xtype: 'displayfield',
            fieldLabel: 'DOB',
            name: 'dob'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'SSN',
            name: 'ssn'
        }, { //ROW 2
            xtype: 'displayfield',
            fieldLabel: 'Verified SSN',
            name: 'verifiedSsnInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Insurance Prefix',
            name: 'insurancePrefix'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Insurance File Nbr',
            name: 'insuranceFileNumber'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Sex',
            name: 'gender'
        }, { //ROW 3
            xtype: 'displayfield',
            fieldLabel: 'POA Code 1',
            name: 'poaCode1'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'POA Code 2',
            name: 'poaCode2'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Date of Death',
            name: 'dod'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Cause of Death',
            name: 'causeOfDeath'
        }, { //ROW 4
            xtype: 'displayfield',
            fieldLabel: 'Death in Service',
            name: 'deathInService'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'In-Theater Start',
            name: 'inTheaterStartDate'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'In-Theater End',
            name: 'inTheaterEndDate'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'In-Theater Days',
            name: 'inTheaterDays'
        }, { //ROW 5
            xtype: 'displayfield',
            fieldLabel: 'Contested Data',
            name: 'contestedDataInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Active Svc Years',
            name: 'activeServiceYears'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Active Svc Months',
            name: 'activeServiceMonths'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Active Svc Days',
            name: 'activeServiceDays'
        }, { //ROW 6
            xtype: 'displayfield',
            fieldLabel: 'POW # of Days',
            name: 'powNumberOfDays'
        },
        {
            xtype: 'fieldcontainer',
            fieldLabel: 'Claim Folder Loc',
            items: {
                xtype: 'displayfield',
                //text: 'Open Processing Times',
                //action: 'displayclaimprocessingtimes',
                name: 'claimFolderLocation'
            },
            colspan: 1
        },
        {
            xtype: 'displayfield',
            fieldLabel: 'Employee Station #',
            name: 'employeeStationNumber'
        },
        {
            xtype: 'displayfield',
            fieldLabel: 'Claim #',
            name: 'fileNumber'
        }],

        me.dockedItems = [{
            xtype: 'toolbar',
            items: [
            {
                xtype: 'button',
                text: 'Create BIRLS Service Request',
                tooltip: 'Creates Service Request from BIRLS Tab',
                iconCls: 'icon-request',
                action: 'createpersonservicerequest',
                id: 'id_birls_IdentificationData_01'
            },
            {
                xtype: 'tbseparator',
                id: 'id_birls_IdentificationData_02'
            },
            {
                xtype: 'button',
                text: 'Create VAI',
                tooltip: 'Creates VAI for BIRLS Record',
                iconCls: 'icon-vai',
                action: 'createvai',
                hidden: true,
                id: 'id_birls_IdentificationData_03'
            },
            {
                xtype: 'tbfill',
                id: 'id_birls_IdentificationData_04'
            }]
        }];

        me.callParent();
    }
});