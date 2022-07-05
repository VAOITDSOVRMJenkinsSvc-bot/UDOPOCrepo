/**
* @author Ivan Yurisevic
* @class VIP.view.appeals.details.Appellant
*
* The view for the current POA fieldset
*/
Ext.define('VIP.view.appeals.details.Appellant', {
    extend: 'Ext.form.Panel',
    alias: 'widget.appeals.details.appellant',
    title: 'Appellant Info',
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
        width: 250
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [
        //Per RTC158386, Row 1:  Name, SSN, DOB, Gender
        {
        xtype: 'displayfield',
        fieldLabel: 'Appellant Name',
        name: 'appellantFullName'
        }, 
        {
        xtype: 'displayfield',
        fieldLabel: 'SSN',
        name: 'veteranSsn'
        },
        {
        xtype: 'displayfield',
        fieldLabel: 'Birth Date',
        name: 'birthDate'
        }, 
        {
        xtype: 'displayfield',
        fieldLabel: 'Gender',
        name: 'veteranGender'
        },
        //Row 2:  Vet Desc, Title, Vet Name, FNOD Date   
        {
            xtype: 'displayfield',
            fieldLabel: 'Relationship to Vet Desc',
            name: 'appellantRelationshipToVeteranDescription'
        }, 
        {
            xtype: 'displayfield',
            fieldLabel: 'Title',
            name: 'appellantTitle'
        },
        {
            xtype: 'displayfield',
            fieldLabel: 'Veteran Name',
            name: 'veteranFullName'
        },
        {
            xtype: 'displayfield',
            fieldLabel: 'FNOD Date',
            name: 'finalNoticeOfDeathDate_f'
        },
        //Row 3:  Address 1 & 2
        {
            xtype: 'displayfield',
            fieldLabel: 'Address 1',
            name: 'appellantAddressLine1',
            width: 400
        }, 
        {
            xtype: 'displayfield',
            fieldLabel: 'Address 2',
            name: 'appellantAddressLine2',
            colspan: 3,
            width: 400
        }, 
        //Row 4:  City, State, ZIP, Country
        {
            xtype: 'displayfield',
            fieldLabel: 'City',
            name: 'appellantAddressCityName'
        }, 
        {
            xtype: 'displayfield',
            fieldLabel: 'State',
            name: 'appellantAddressStateCode'
        }, 
        {
            xtype: 'displayfield',
            fieldLabel: 'Zip Code',
            name: 'appellantAddressZipCode'
        }, 
        {
            xtype: 'displayfield',
            fieldLabel: 'Country',
            name: 'appellantAddressCountryName'
        },
        //Row 5: Address Mod By, Address Mod Date, Address Notes
        {
            xtype: 'displayfield',
            fieldLabel: 'Address Mod By',
            name: 'appellantAddressLastModifiedByROName'
        }, 
        {
            xtype: 'displayfield',
            fieldLabel: 'Address Mod Date',
            name: 'appellantAddressLastModifiedDate_f'
        }, 
        {
            xtype: 'displayfield',
            fieldLabel: 'Address Notes',
            name: 'appellantAddressNotes',
            colspan: 2
        },
        //Row 6:  Home Phone, Work Phone 
        {
            xtype: 'displayfield',
            fieldLabel: 'Home Phone',
            name: 'appellantHomePhone'
        },
        {
            xtype: 'displayfield',
            fieldLabel: 'Work Phone',
            name: 'appellantWorkPhone'
        }];

        me.callParent();
    }
});
