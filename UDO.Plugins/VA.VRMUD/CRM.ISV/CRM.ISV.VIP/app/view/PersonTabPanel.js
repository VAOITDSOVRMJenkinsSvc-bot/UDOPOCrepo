/**
* @author Jonas Dawson
* @class VIP.view.PersonTabPanel
*
* The central tab panel to display the associated categories of person data
*/
Ext.define('VIP.view.PersonTabPanel', {
    extend: 'Ext.tab.Panel',
    alias: 'widget.persontabpanel',
    requires: [
        'VIP.view.PersonInfo',
        'VIP.view.PersonVadir',
        'VIP.view.Poa',
        'VIP.view.Fiduciary',
        'VIP.view.Birls',
        'VIP.view.Awards',
        'VIP.view.Claims',
        'VIP.view.MilitaryService',
        'VIP.view.PaymentInformation',
        'VIP.view.Ratings',
        'VIP.view.Denials',
        'VIP.view.Pathways',
        'VIP.view.Appeals',
        'VIP.view.PaymentHistory',
        'VIP.view.VirtualVA'
    ],

    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'personinfo' },
            { xtype: 'personvadir' },
            { xtype: 'poa' },
            { xtype: 'fiduciary' },
            { xtype: 'birls' },
            { xtype: 'awards' },
            { xtype: 'claims' },
            { xtype: 'militaryservice' },
            { xtype: 'paymentinformation' },
            { xtype: 'ratings' },
            { xtype: 'denials' },
            { xtype: 'appeals' },
            { xtype: 'pathways' },
            { xtype: 'virtualva' },
            { xtype: 'paymenthistory' }
        ];

        me.callParent();
    }
});
