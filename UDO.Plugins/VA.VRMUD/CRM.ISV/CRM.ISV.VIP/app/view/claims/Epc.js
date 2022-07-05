Ext.define('VIP.view.claims.Epc', {
    extend: 'Ext.container.Container',
    alias: 'widget.claims.epc',
    layout: 'hbox',
    width: 250,
    initComponent: function () {
        var me = this;

        me.items = [
            {
                xtype: 'combobox',
                displayField: 'label',
                valueField: 'value',
                forceSelection: true,
                //multiSelect: true,
                store: 'Epc'
				, width: 127
            },
            {
                xtype: 'checkbox',
                boxLabel: 'By Code',
                submitValue: false,
                style: { fontSize: '8px' }
            },
            {
                xtype: 'button',
                text: 'Clear Filter',
                tooltip: 'Clears the Claims Filter',
                iconCls: 'icon-addrChange',
                action: 'clearclaimsfilter',
                style: { fontSize: '8px' }
            }
        ];

        me.callParent();
    }
});