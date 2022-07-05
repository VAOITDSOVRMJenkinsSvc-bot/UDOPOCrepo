/**
* @author Josh Oliver
* @class VIP.view.personinfo.Corp
*
* The view for person corporate detail information
*/
Ext.define('VIP.view.personinfo.Corp', {
    extend: 'Ext.form.Panel',
    mixins: {
        sec: 'VIP.mixin.CRMRoleSecurity'
    },
    alias: 'widget.personinfo.corp',
    title: 'Corporate Details',
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
    //listeners: {
    //    afterrender: function () {
    //        var me = this;
    
    //        if (me.UserHasRole('VR%26E') || me.UserHasRole('DMC') || me.UserHasRole('IPC')) {
    //            if (Ext.get('id_personinfo_Corp_01')) Ext.get('id_personinfo_Corp_01').hide();
    //            if (Ext.get('id_personinfo_Corp_02')) Ext.get('id_personinfo_Corp_02').hide();
    //            if (Ext.get('id_personinfo_Corp_03')) Ext.get('id_personinfo_Corp_03').hide();
    //            if (Ext.get('id_personinfo_Corp_04')) Ext.get('id_personinfo_Corp_04').hide();

    //            //if (Ext.get('id_personinfo_details_Dependents_01')) Ext.get('id_personinfo_details_Dependents_01').hide();
    //            if (Ext.get('id_personinfo_details_Dependents_02')) Ext.get('id_personinfo_details_Dependents_02').hide();
    //            if (Ext.get('id_personinfo_details_Dependents_03')) Ext.get('id_personinfo_details_Dependents_03').hide();
    //            if (Ext.get('id_personinfo_details_Dependents_04')) Ext.get('id_personinfo_details_Dependents_04').hide();
    //            if (Ext.get('id_personinfo_details_Dependents_05')) Ext.get('id_personinfo_details_Dependents_05').hide();
    //            if (Ext.get('id_personinfo_details_Dependents_06')) Ext.get('id_personinfo_details_Dependents_06').hide();

    //            if (Ext.get('id_personinfo_details_AllRelationships_01')) Ext.get('id_personinfo_details_AllRelationships_01').hide();
    //        }
    //    }
    //},
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'Name',
            name: 'fullName',
            id: 'fullName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'SSN',
            name: 'ssn',
            id: 'ssn'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'File Number',
            name: 'fileNumber',
            id: 'fileNumber'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'DOB',
            name: 'dob',
            id: 'dateOfBirth'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Phone # 1',
            name: 'fullPhone1',
            id: 'fullPhone1'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Phone 1 Type',
            name: 'phoneTypeNameOne'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Phone # 2',
            name: 'fullPhone2',
            id: 'fullPhone2'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Phone 2 Type',
            name: 'phoneTypeNameTwo'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'DOD',
            name: 'dod',
            id: 'dateOfDeath'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Mil Branch',
            name: 'branchOfService1',
            id: 'branchOfService1'
            // assuming here that Branch of service is the same as Mil Branch
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Mil PO Type',
            name: 'militaryPostOfficeTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Mil Post Tp',
            name: 'militaryPostalTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Org Name',
            name: 'orgName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Org Title',
            name: 'orgTitle'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Org Type',
            name: 'orgType'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Ptcpnt Rel',
            name: 'participantRelationship'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'FID Decision Category Code',
            name: 'fiduciaryDecisionCategoryTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'FID Folder Loc',
            name: 'fiduciaryFolderLocation'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Temp Cust',
            name: 'temporaryCustodianInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Prep Phrase',
            name: 'prepPhraseType'
        }],

        me.dockedItems = [{
            xtype: 'toolbar',
            items: [
            {
                xtype: 'button',
                text: 'Create CORP Service Request',
                tooltip: 'Creates Service Request from Person Info Tab',
                iconCls: 'icon-request',
                action: 'createpersonservicerequest',
                id: 'id_personinfo_Corp_01'
            },
            {
                xtype: 'tbseparator',
                id: 'id_personinfo_Corp_02'
            },
            {
                xtype: 'button',
                text: 'Create VAI',
                tooltip: 'Creates VAI for CORP Record',
                iconCls: 'icon-vai',
                action: 'createvai',
                hidden: true,
                id: 'id_personinfo_Corp_03'
            },
            {
                xtype: 'tbfill',
                id: 'id_personinfo_Corp_04'
            }]
        }];

        me.callParent();
    }
});