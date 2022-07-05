/**
* @author Josh Oliver
* @class VIP.view.awards.details.Diaries
*
* View for Diaries associated with the person
*/
Ext.define('VIP.view.awards.details.Diaries', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.awards.details.diaries',
    store: 'awards.Diaries',
    title: 'Diaries',

    initComponent: function () {
        var me = this;

        me.columns = [
            { text: 'Date', flex: 1, dataIndex: 'date', xtype: 'datecolumn', format: 'm/d/Y' },
			{ text: 'Reason Cd', flex: 1, dataIndex: 'reasonCode' },
			{ text: 'Reason Name', flex: 1, dataIndex: 'reasonName' },
			{ text: 'Desc', flex: 1, dataIndex: 'description' }
        ];
        
        me.callParent();
    }
});