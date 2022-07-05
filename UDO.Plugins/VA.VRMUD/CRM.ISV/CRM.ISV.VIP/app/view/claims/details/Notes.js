/**
* @author Jonas Dawson
* @class VIP.view.claims.details.Notes
*
* The view for the letters for the selected claim
*/
Ext.define('VIP.view.claims.details.Notes', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.claims.details.notes',
    title: 'Notes',
    store: 'claims.notes.All',
    initComponent: function () {
        var me = this;

        me.columns = [
            { header: 'Claim Id', dataIndex: 'claimId', width: 70 },
            { header: 'Date/Time', dataIndex: 'createDate', xtype: 'datecolumn', format: 'm/d/Y G:i' },
            { header: 'RO', dataIndex: 'journalLocationId', width: 40 },
            { header: 'User', dataIndex: 'fullUserID', width: 175 },
            { header: 'Type', dataIndex: 'noteOutTypeName', width: 150 },
            { header: 'Suspense Date', dataIndex: 'suspenseDate', xtype: 'datecolumn', format: 'm/d/Y' },
            { header: 'Note', flex: 1, dataIndex: 'text', width: 500 },
            { header: 'Participant ID', dataIndex: 'participantId', width: 80 }
        ];
        me.scroll = false;
        me.viewConfig = { style: { overflow: 'auto', overflowX: 'hidden' } };

        me.dockedItems = [{
            xtype: 'toolbar',
            labelStyle: 'font-size:11px',
            items: [
                {
                    xtype: 'button',
                    text: 'Refresh',
                    tooltip: 'Refresh all notes',
                    iconCls: 'icon-refresh',
                    action: 'refreshnotes'
                },
                {
                    xtype: 'checkbox',
                    id: 'includeVeteranNotes',
                    boxLabel: 'View Contact Notes',
                    checked: true,
                    style: {
                        fontSize: '11px',
                        color: '#333333'
                    }
                },
                { xtype: 'tbseparator', id: 'id_createNoteButton' },
                {
                    xtype: 'button',
                    id: 'createNoteButton',
                    text: 'Create Note',
                    tooltip: 'Create a new note for the veteran',
                    iconCls: 'icon-noteAdd',
                    action: 'createnote'
                },
                { xtype: 'tbseparator', id: 'id_editNoteButton' },
                {
                    id: 'editNoteButton',
                    xtype: 'button',
                    text: 'Edit Note',
                    tooltip: 'Edit the selected note',
                    iconCls: 'icon-noteEdit',
                    action: 'editnote'
                },
                { xtype: 'tbseparator', id: 'id_openSelectedNote' },
                {
                    id: 'openSelectedNote',
                    xtype: 'button',
                    text: 'Open Selected Note',
                    tooltip: 'Open the selected note',
                    iconCls: 'icon-note',
                    action: 'openselectednote'
                },
                { xtype: 'tbseparator', id: 'id_openDisplayedNotes' },
                {
                    id: 'openDisplayedNotes',
                    xtype: 'button',
                    text: 'Open All Displayed Notes',
                    tooltip: 'Open all notes displayed in the grid',
                    iconCls: 'icon-noteEdit',
                    action: 'viewallnotes'
                }
            ]
        }];

        me.callParent();
    }
});
