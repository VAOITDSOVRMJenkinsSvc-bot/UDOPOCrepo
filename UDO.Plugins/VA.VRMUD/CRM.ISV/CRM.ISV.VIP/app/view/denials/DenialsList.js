/**
* @author Ivan Yurisevic
* @class VIP.view.denials.DenialsList
*
* The view for the Denials
*/
Ext.define('VIP.view.denials.DenialsList', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.denials.denialslist',
    store: 'Denial',

    initComponent: function () {
        var me = this;

        me.store = 'Denial';

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { xtype: 'rownumberer' },
                { header: 'Award Type', dataIndex: 'awardTypeName' },
			    { header: 'Claim Date', dataIndex: 'claimDate', xtype: 'datecolumn', format: 'm/d/Y' },
			    { header: 'Claim Type', dataIndex: 'claimTypeName' },
			    { header: 'Decision Date', dataIndex: 'decisionDate', xtype: 'datecolumn', format: 'm/d/Y' }
            ]
        };

        me.callParent();
    }
});
