/**
* @author Josh Oliver
* @class VIP.view.claims.details.notes.CreateNote
*
* The component for selecting to create claimant or claim development
*/
Ext.define('VIP.view.claims.details.notes.CreateNote', {
    extend: 'Ext.button.Split',
    alias: 'widget.createnote',
    text: 'Create Note',
    id: 'createNoteButton',
    action: 'createnote',
    tooltip: 'Create a new note for the claimant or selected claim',
    iconCls: 'icon-noteAdd',
    menu: {
        xtype: 'menu',
        defaults: {
            iconCls: 'icon-note'
        },
        items: [
            { value: 'claimant', text: 'For Claimant (All Claims)' },
            { value: 'selectedclaim', text: 'For Selected Claim' }
        ]
    },
    defaultMenuSelection: { value: 'claimant', text: 'For Claimant (All Claims)' }
});