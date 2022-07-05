/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.militaryservice.VerificationInfo
*
* The view for the BIRLS fieldset at the top section
*/
Ext.define('VIP.view.birls.birlsdetails.militaryservice.VerificationInfo', {
    extend: 'Ext.form.Panel',
    alias: 'widget.birls.birlsdetails.militaryservice.verificationinfo',
    store: 'Birls',
    title: 'Verification Info',
    layout: {
        type: 'table',
        columns: 3,
        tableAttrs: {
            style: {
                width: '100%'
            }
        }
    },
    defaults: {
        labelStyle: 'font-weight:bold;font-size:11px;',
        fieldStyle: 'font-size:11px;',
        labelWidth: 200
        //,width: 200
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'Service 1 Verified',
            name: 'verifiedSvcDataInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Service 2 Verified',
            name: 'verifiedSvcDataInd2'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Service 3 Verified',
            name: 'verifiedSvcDataInd3'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Service 1 VADS',
            name: 'vadsInd'
        },  {
            xtype: 'displayfield',
            fieldLabel: 'Service 2 VADS',
            name: 'vadsInd2'
        },  {
            xtype: 'displayfield',
            fieldLabel: 'Service 3 VADS',
            name: 'vadsInd3'
        }];

        me.callParent();
    }
});