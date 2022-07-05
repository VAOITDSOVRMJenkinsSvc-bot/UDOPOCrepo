/**
* @author Josh Oliver
* @class VIP.view.paymentinformation.details.AwardReasons
*
* View for paymentinformation award reasons
*/
Ext.define('VIP.view.paymentinformation.details.AwardReasons', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.paymentinformation.details.awardreasons',
    store: 'paymentinformation.AwardReason',
    title: 'Award Reasons',

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
			    { header: 'Reason', dataIndex: 'awardReasonText', flex: 1 }
            ]
        };

        me.callParent();
    }
});