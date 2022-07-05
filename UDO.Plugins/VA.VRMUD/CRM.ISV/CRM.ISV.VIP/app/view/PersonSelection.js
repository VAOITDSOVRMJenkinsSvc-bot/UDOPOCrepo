/**
* @author Josh Oliver
* @class VIP.view.PersonSelection
*
* The sole header tool bar for the VIP application.
*/
Ext.define('VIP.view.PersonSelection', {
    extend: 'Ext.tab.Panel',
    alias: 'widget.personselection',
    title: 'Person Selection',
    requires: [
        'VIP.view.personselection.Birls',
        'VIP.view.personselection.Corp'
    ],
    initComponent: function () {
        var me = this;

        me.items = [
            {
                xtype: 'personselection.corp'
            },
            {
                xtype: 'personselection.birls'
            }
        ];

        me.callParent();
    }
});


