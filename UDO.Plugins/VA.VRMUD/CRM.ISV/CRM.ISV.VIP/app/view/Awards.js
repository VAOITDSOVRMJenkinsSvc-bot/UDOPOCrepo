/**
* @author Jonas Dawson
* @class VIP.view.Awards
*
* The main panel for the Awards tab
*/
Ext.define('VIP.view.Awards', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.awards',
    title: 'Awards',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: ['VIP.view.awards.Benefits', 'VIP.view.awards.Details'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'awards.benefits',
            flex: 1
        }, {
            xtype: 'awards.details',
            flex: 1
        }];

        me.callParent();
    }
});