/**
* @author Jonas Dawson
* @class VIP.view.SearchOverview
*
* The sole header tool bar for the VIP application.
*/
Ext.define('VIP.view.SearchOverview', {
    extend: 'Ext.toolbar.Toolbar',
    alias: 'widget.searchoverview',
    requires: ['VIP.view.WebServiceRequestHistory'],

    currentIconCls: undefined,
    statusText: undefined,

    levelClasses: {
        'nominal': 'icon-status-ok',
        'error': 'icon-status-error',
        'busy': 'icon-status-busy',
        'warning': 'icon-status-warning'
    },

    initComponent: function () {
        var me = this;

        me.currentIconCls = me.levelClasses['nominal'];

        me.items = [{
            xtype: 'tbtext',
            itemId: 'searchresults',
            text: 'VIP',
            style: {
                fontWeight: 'bold'
            }
        }, '-', {
            xtype: 'tbtext',
            text: 'No Processes Running',
            cls: 'status-text ' + me.currentIconCls,
            itemId: 'statusText',
            flex: 1
        }, {
            xtype: 'tbtext',
            itemId: 'executiontime',
            text: 'Execution Time: 0 seconds'
        }, '-', {
            xtype: 'tbtext',
            itemId: 'calltimer',
            text: 'Call Time: 0 seconds'
        }];

        me.callParent();
        me.statusText = me.down("#statusText");
    },


    setStatus: function (status) {
        var me = this;

        if (Ext.isString(status)) {
            status = {
                text: status
            };
        }

        if (status.text) {
            me.statusText.setText(status.text);
        }

        if (status.level) {
            var levelCls = me.levelClasses[status.level];

            if (me.currentIconCls && me.currentIconCls != levelCls) {
                me.statusText.removeCls(me.currentIconCls);
            }

            me.statusText.addCls(levelCls);
            me.currentIconCls = levelCls;
        }
    }
});