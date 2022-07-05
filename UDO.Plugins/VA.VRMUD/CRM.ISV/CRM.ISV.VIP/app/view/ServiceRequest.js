/**
* @author Jonas Dawson
* @class VIP.view.ServiceRequest
*
* The component for selecting and requesting a service request
*             //{ value: '953850008', text: 'Other' }
*/
Ext.define('VIP.view.ServiceRequest', {
    extend: 'Ext.button.Split',
    alias: 'widget.servicerequest',
    text: 'Create Service Request',
    action: 'startservicerequest',
    tooltip: 'Creates a new Service Request for the selected Dependent',
    iconCls: 'icon-request',
    menu: {
        xtype: 'menu',
        defaults: {
            iconCls: 'icon-doc'
        },
        items: [
            { value: '1', text: '0820' },
            { value: '953850001', text: '0820a' },
            { value: '953850002', text: '0820d' },
            { value: '953850003', text: '0820f' },
            //{ value: '2', text: 'VAI' },
            //{ value: '953850005', text: '0820 & VAI' },
            { value: '953850006', text: 'Letter' },
            { value: '953850007', text: 'Non Emergency Email' }
        ]
    },
    defaultMenuSelection: { value: '1', text: '0820' }
});