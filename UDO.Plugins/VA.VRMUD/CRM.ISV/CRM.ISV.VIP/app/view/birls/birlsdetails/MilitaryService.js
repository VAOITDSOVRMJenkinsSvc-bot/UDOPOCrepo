/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.MilitaryService
*
* The view for MilitaryService associated with the person
*/
Ext.define('VIP.view.birls.birlsdetails.MilitaryService', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.birls.birlsdetails.militaryservice',
    title: 'Military Service',

    requires: ['VIP.view.birls.birlsdetails.militaryservice.ServiceList', 'VIP.view.birls.birlsdetails.militaryservice.VerificationInfo'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'birls.birlsdetails.militaryservice.servicelist',
            flex: 1
        }, {
            xtype: 'birls.birlsdetails.militaryservice.verificationinfo',
            flex: 1
        }];

        me.callParent();
    }
});