/**
* @author Josh Oliver
* @class VIP.view.awards.details.Evrs
*
* View for Evrs associated with the person
*/
Ext.define('VIP.view.awards.details.Evrs', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.awards.details.evrs',
    store: 'awards.Evrs',
    title: 'EVRs',

    initComponent: function () {
        var me = this;

        me.columns = [
            { text: 'Type', flex: 1, dataIndex: 'type' },
			{ text: 'Status', flex: 1, dataIndex: 'status' },
			{ text: 'Control', flex: 1, dataIndex: 'control' },
			{ text: 'Exempt', flex: 1, dataIndex: 'exempt' },
			{ text: 'Last Reported', flex: 1, dataIndex: 'lastReported' }
        ];

        me.callParent();
    }
});
