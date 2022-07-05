/**
* @author Dmitri Riz
* @class VIP.view.CrmLaunch
*
* The launch panel for the application. Precedes any searches.
*/
Ext.define('VIP.view.CrmLaunch', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.crmlaunch',
    title: 'Welcome to CRM UD',
    initComponent: function ()
    {
        var me = this;
        me.items = [
            {
                        xtype: 'container',
                        padding: '0 5 0 0',
                        layout: {
                            type: 'hbox',
                            align: 'middle'
                        },
                        //style: { height: '100%', borderColor: '#000000', borderStyle: 'solid', borderWidth: '1px', backgroundColor: 'lightblue', font: 'font:size: 24px' },
                        bodyStyle: 'font-size: 66px',
                        items: [{
                            xtype: 'label',
                            text: 'VRMUD VIP. Please use controls above to provide search criteria.',
                            margins: '0 0 0 10'
                        }]       
            }
        ];

        me.callParent();
    }
});
