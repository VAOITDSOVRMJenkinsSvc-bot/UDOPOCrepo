/**
* @author Dmitri Riz
* @class VIP.view.Viewport
*
* The top level container for the viewable application environment.
*/
Ext.define('VIP.view.Viewport', {
    extend: 'Ext.container.Viewport',
    layout: 'border',
    requires: [
        'VIP.view.AppStatus',
        'VIP.view.ContentPanel',
        'VIP.view.SearchOverview'
    ],

    initComponent: function () {
        var me = this;

        me.items = [
            {
                xtype: 'searchoverview',
                cls: 'appStatus',
                region: 'north'
            },
            {
                xtype: 'contentpanel',
                region: 'center'
            },
            {
                xtype: 'appstatus',
                cls: 'appStatus',
                region: 'south'
            }
        ];

        me.callParent();
    }
});