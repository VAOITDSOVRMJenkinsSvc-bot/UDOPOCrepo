/**
* @author Jonas Dawson
* @class VIP.view.awards.details.AwardDetails
*
* Award details for the selected award line
*/
Ext.define('VIP.view.awards.details.AwardDetails', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.awards.details.awarddetails',
    title: 'Award Details',
    requires: ['VIP.view.awards.details.awarddetails.OtherAwardInformation'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'awards.details.awarddetails.otherawardinformation',
            flex: 1
        }];

        me.callParent();
    }
});