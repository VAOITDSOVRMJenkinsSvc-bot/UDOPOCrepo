/**
* @author Josh Oliver
* @class VIP.controller.claims.Notes
*
* Controller for the claims tabs under the claims tab
*/
Ext.define('VIP.controller.claims.Notes', {
    extend: 'Ext.app.Controller',
    requires: [
        'VIP.model.Claims',
        'VIP.model.claims.Notes'
    ],
    uses: [
        'Ext.util.MixedCollection',
        'Ext.util.Filter',
        'Ext.window.Window'
    ],
    refs: [{
        ref: 'notesGrid',
        selector: '[xtype="claims.details.notes"]'
    }, {
        ref: 'includeVeteranNotes',
        selector: '[id="includeVeteranNotes"]'
    }, {
        ref: 'claimsGrid',
        selector: '[xtype="claims.benefits"]'
    }, {
        ref: 'claimDetails',
        selector: '[xtype="claims.details.claimdetails"]'
    }, {
        ref: 'createNote',
        selector: '[xtype="createnote"]'
    }, {
        ref: 'refreshButton',
        selector: '[xtype="claims.details.notes"] > toolbar > button[action="refreshnotes"]'
    }],

    stores: [
        'claims.notes.Claims',
        'claims.notes.Veteran',
        'claims.notes.All'
    ],

    init: function () {
        var me = this;

        me.application.on({
            claimdetailsloaded: me.loadNotes,
            claimscacheddataloaded: me.onClaimsCachedDataLoaded,
            claimselected: me.onClaimSelected,
            individualidentified: me.onIndividualIdentified,
            servicerequestnotecreated: me.onServiceRequestNoteCreated,
            claimsrefreshed: me.onClaimsRefreshed,
            scope: me
        });

        me.control({
            '[xtype="claims.details.notes"] > toolbar > checkbox': {
                change: me.refreshNotes
            },
            '[xtype="claims.details.notes"] > toolbar > button[action="createnote"]': {
                click: me.createVeteranNote
            },
            '[xtype="claims.details.notes"] > toolbar > button[action="editnote"]': {
                click: me.editNote
            },
            '[xtype="claims.details.notes"] > toolbar > button[action="openselectednote"]': {
                click: me.openSelectedNote
            },
            '[xtype="claims.details.notes"] > toolbar > button[action="viewallnotes"]': {
                click: me.openDisplayedNotes
            },
            '[xtype="claims.details.notes"] > toolbar > button[action="refreshnotes"]': {
                click: me.refreshNotes
            }
        });


        Ext.log('The claims.Details controller has been initialized');
    },

    onClaimsRefreshed: function () {
        this.getNotesGrid().setLoading(true, true);
    },

    loadNotes: function (claim) {
        var me = this,
            includeVeteranNotes = me.getIncludeVeteranNotes().getValue();

        me.getRefreshButton().setDisabled(true);
        me.clearClaimsNotesStores();

        if (includeVeteranNotes) {
            var veteranParticipantId = !Ext.isEmpty(me.application.personInquiryModel) ? me.application.personInquiryModel.get('participantId') : null;
            if (!Ext.isEmpty(veteranParticipantId)) {
                me.loadNotesStore(null, veteranParticipantId, me.getClaimsNotesVeteranStore());
            }
        }

        if (!Ext.isEmpty(claim) && !Ext.isEmpty(claim.get('claimId'))) {
            me.loadNotesStore(claim.get('claimId'), claim.get('participantId'), me.getClaimsNotesClaimsStore());
        }
    },

    clearClaimsNotesStores: function () {
        var me = this;

        me.getClaimsNotesAllStore().removeAll();
        me.getClaimsNotesClaimsStore().removeAll();
        me.getClaimsNotesVeteranStore().removeAll();
    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this;
        me.clearClaimsNotesStores();
        me.getNotesGrid().setLoading(true, true);
    },

    onClaimSelected: function () {
        var me = this;
        me.clearClaimsNotesStores();
        me.getNotesGrid().setLoading(true, true);
    },

    onServiceRequestNoteCreated: function (claimId, participantId) {
        var me = this,
            claim = Ext.create('VIP.model.Claims');

        me.clearClaimsNotesStores();
        me.getNotesGrid().setLoading(true, true);

        claim.set('claimId', claimId);
        claim.set('participantId', participantId);
        claim.commit();

        me.loadNotes(claim);
    },

    onClaimsCachedDataLoaded: function (claimsStore) {
        var me = this,
            claimsNotesStore = me.getClaimsNotesAllStore();

        if (!Ext.isEmpty(claimsNotesStore)) {
            var developmentNotes = [],
                claimNotes = [],
                claim = me.getSelectedOrSingleClaim();

            if (!Ext.isEmpty(claim)) {
                claimNotes = me.filterReturnedNotesRecords(claimsNotesStore.getRange(), claim.get('claimId'));
            }

            developmentNotes = me.filterReturnedNotesRecords(claimsNotesStore.getRange(), null);

            claimsNotesStore.loadRecords(developmentNotes, {
                addRecords: false
            });
            claimsNotesStore.loadRecords(claimNotes, {
                addRecords: true
            });
        }

        me.getNotesGrid().reconfigure(claimsNotesStore);
    },

    createVeteranNote: function () {
        var me = this;

        me.showCreateNoteDialog(createVeteranNoteCallback);

        function createVeteranNoteCallback(buttonResult, noteText, config) {
            if (buttonResult == 'cancel') return;

            me.clearClaimsNotesStores();

            var participantId = me.application.personInquiryModel.get('participantId');

            me.createNote('', participantId, noteText);
        }
    },

    showCreateNoteDialog: function (createNoteCallback) {
        var me = this;

        Ext.Msg.show({
            animateTarget: 'createNoteButton',
            buttons: Ext.Msg.OKCANCEL,
            closable: false,
            fn: createNoteCallback,
            height: 1000,
            icon: Ext.window.MessageBox.INFO,
            msg: 'Please enter text for the new note:',
            multiline: true,
            title: 'Create Note',
            value: '',
            width: 1000,
            scope: me
        });
    },

    createNote: function (claimId, participantId, noteText) {
        var me = this,
            newNote = Ext.create('VIP.model.claims.Notes'),
            includeVeteranNotes = me.getIncludeVeteranNotes();

        if (!Ext.isEmpty(includeVeteranNotes)) {
            includeVeteranNotes.setValue(true);
        }

        noteText = me.replaceInvalidXmlCharacters(noteText);

        if (!Ext.isEmpty(participantId)) {
            me.getNotesGrid().setLoading(true, true);
            newNote.save({
                filters: [{
                    property: 'clmId',
                    value: claimId
                }, {
                    property: 'ptcpntId',
                    value: participantId
                }, {
                    property: 'txt',
                    value: noteText
                }],
                callback: function (records, operation, success) {
                    //refresh the notes after the create
                    if (!Ext.isEmpty(operation) && operation.success) {
                        var claim = me.getSelectedOrSingleClaim();

                        me.loadNotes(claim);
                    }
                    else { me.getNotesGrid().setLoading(false); }

                    if (parent && parent._CreateDevNoteLogEntry) {
                        parent._CreateDevNoteLogEntry(operation.response.xml, noteText);
                    }
                },
                scope: me
            });
        }
    },

    editNote: function () {
        var me = this,
            notesGrid = me.getNotesGrid();

        if (!notesGrid.getSelectionModel().hasSelection()) {
            Ext.Msg.alert('Select Development Note', 'Please select a Note record from the grid.');
            return;
        }

        var note = me.getNotesGrid().getSelectionModel().getSelection()[0];

        /*
        users can edit only if created in CRM
        PCR role can only change records they create within 3 days of date created
        Management roles (Coach, Supervisor) can only change notes from their station within 3 days.
        to validate, get the record about the note from the QueryLog
        */
        if (!Ext.isEmpty(note)) {
            var noteId = note.get('noteId'),
                crmNote = false,
                noteUserId = note.get('journalUserId'),
                currentUserId = me.application.pcrContext.get('userName'),
                createdOnDate = new Date(Date.parse(note.get('createDate'))),
                msg = '';


            if (!Ext.isEmpty(noteId)) {
                var filter = "va_NoteId eq '" + note.get('noteId') + "'",
                    columns = ['CreatedOn', 'OwnerId'],
                    b = parent._QueryDevNotes(columns, filter);

                crmNote = (b && b.results && b.results.length > 0);
            } else {
                if (!confirm('Please confirm that this Development Note was created within CRM tool.\n\nOnly Notes created by CRM can be edited.')) {
                    return;
                }
                crmNote = true;
            }

            if (crmNote) {
                //JournalID compare
                if (noteUserId !== currentUserId) {
                    msg = 'this note was created by another user.';
                }
                //Can Only update Note if created TODAY
                var varToday = new Date();

                varToday.setHours(3);
                if (createdOnDate < varToday) {
                    msg = 'this note was created before today (Created on ' + createdOnDate.toLocaleDateString() + ').';
                }
            } else {
                msg = 'this Note was not created in CRM tool.';
            }



            if (msg.length > 0) {
                Ext.Msg.alert('Unable to edit', 'Cannot update this Development Note because ' + msg);
                return;
            }

            Ext.Msg.show({
                animateTarget: 'editNoteButton',
                buttons: Ext.Msg.OKCANCEL,
                closable: false,
                fn: me.finishEditNote,
                height: 1000,
                icon: Ext.window.MessageBox.INFO,
                msg: 'Please enter the new note text:',
                multiline: true,
                title: 'Update Note Text',
                value: note.get('text'),
                width: 1000,
                scope: me
            });
        }
    },

    finishEditNote: function (buttonResult, noteText, config) {

        if (buttonResult == 'cancel') {
            return;
        }

        var me = config.scope, //get "this" of the parent
            notesStore = me.getClaimsNotesClaimsStore(),
            selectedNote = me.getNotesGrid().getSelectionModel().getSelection()[0],
            today = Ext.Date.format(new Date(), "c"),
            newNoteText = me.replaceInvalidXmlCharacters(noteText),
            includeVeteranNotes = me.getIncludeVeteranNotes();

        me.clearClaimsNotesStores();

        if (newNoteText == '') {
            Ext.Msg.alert('No Input', 'Text must be entered to update the note. Process will terminate.');
            return;
        }

        if (!Ext.isEmpty(includeVeteranNotes)) {
            includeVeteranNotes.setValue(true);
        }

        if (!Ext.isEmpty(selectedNote)) {
            var claimId = selectedNote.get('claimId'),
                participantId = selectedNote.get('participantId');

            if (!Ext.isEmpty(participantId)) {
                me.clearClaimsNotesStores();
                me.getNotesGrid().setLoading(true, true);
                notesStore.load({
                    action: 'update',
                    filters: [
                        {
                            property: 'clmId',
                            value: claimId
                        },
                        {
                            property: 'ptcpntId',
                            value: participantId
                        },
                        {
                            property: 'txt',
                            value: newNoteText
                        },
                        {
                            property: 'noteId',
                            value: selectedNote.get('noteId')
                        },
                        {
                            property: 'noteOutTn',
                            value: selectedNote.get('noteOutTypeName')
                        },
                        {
                            property: 'createDt',
                            value: Ext.Date.format(new Date(selectedNote.get('createDate')), "c")
                        },
                        {
                            property: 'modifdDt',
                            value: today
                        },
                        {
                            property: 'jrnDt',
                            value: today
                        },
                        {
                            property: 'suspnsDt',
                            value: today
                        },
                        {
                            property: 'userId',
                            value: me.application.pcrContext.get('pcrId')
                        }
                    ],
                    callback: function (records, operation, success) {
                        if (success) {
                            var claim = me.getSelectedOrSingleClaim();

                            me.loadNotes(claim);
                        }
                        else { me.getNotesGrid().setLoading(false); }
                    },
                    scope: me
                });
            }
        }
    },

    openSelectedNote: function () {
        var me = this;

        if (!me.getNotesGrid().getSelectionModel().hasSelection()) {
            Ext.Msg.alert('Select Development Note', 'Please select a Note record from the grid.');
            return;
        }
        var developmentNote = me.getNotesGrid().getSelectionModel().getSelection()[0];

        Ext.Msg.show({
            animateTarget: 'openSelectedNote',
            buttons: Ext.Msg.CANCEL,
            closable: false,
            icon: Ext.window.MessageBox.INFO,
            msg: 'Please view the note text:',
            multiline: 400,
            title: 'View Note Text',
            value: developmentNote.get('text'),
            height: 1000,
            width: 1000
        });
    },

    refreshNotes: function (button) {
        var me = this,
            claim = me.getSelectedOrSingleClaim(),
            includeVeteranNotes = me.getIncludeVeteranNotes().getValue();

        me.clearClaimsNotesStores();

        if (!Ext.isEmpty(claim) || includeVeteranNotes) {

            me.getNotesGrid().setLoading(true, true);

            me.loadNotes(claim, button);
        }
    },

    openDisplayedNotes: function () {
        var me = this;

        var tempStore = me.getNotesGrid().getStore();

        if (Ext.isEmpty(tempStore)) {
            return;
        }

        tempStore.sort('createDate', 'DESC');
        var notesText = '';

        tempStore.each(function (note) {
            if (notesText.length > 0) {
                notesText += '\n************************************************************\n\n';
            }

            if (note.data && note.data != undefined) {
                notesText += 'User:  ' + note.get('fullUserID') + ';  Date:  ' + (!Ext.isEmpty(note.get('createDate')) ? Ext.Date.format(note.get('createDate'), 'm/d/y-h:i:s A') : '') + '\n';
                notesText += 'Text:  ' + note.get('text');
            }
        });

        Ext.create('Ext.window.Window', {
            title: 'Notes View',
            height: 400,
            width: 800,
            layout: 'fit',
            items: {
                xtype: 'panel',
                title: 'View All Notes Text',
                bodyPadding: 10,
                width: 800,
                renderTo: Ext.getBody(),
                items: [{
                    xtype: 'textareafield',
                    width: 765,
                    height: 300,
                    grow: true,
                    growMin: 300,
                    autoScroll: true,
                    value: notesText
                }]
            }
        }).show();
    },

    loadNotesStore: function (claimId, participantId, notesStore) {
        var me = this,
            filters = getNotesFilters(claimId, participantId);

        if (!Ext.isEmpty(filters)) {
            notesStore.load({
                filters: Ext.clone(filters),
                callback: function (records, operation, success) {
                    var filteredNotesRecords = [];

                    if (success && !Ext.isEmpty(records)) {
                        filteredNotesRecords = me.filterReturnedNotesRecords(records, claimId);

                        me.getClaimsNotesAllStore().loadRecords(filteredNotesRecords, {
                            addRecords: true
                        });

                        me.getNotesGrid().reconfigure(me.getClaimsNotesAllStore());
                    }

                    me.getNotesGrid().setLoading(false);
                    me.application.fireEvent('webservicecallcomplete', operation, 'claims.notes.All');
                    //                    if (typeof (button.setDisabled) != "undefined") {
                    //                        button.setDisabled(false);
                    //                    }
                    me.getRefreshButton().setDisabled(false);
                },
                scope: me
            });
        }

        function getNotesFilters(claimId, participantId) {
            var filters = Ext.create('Ext.util.MixedCollection');
            if (!Ext.isEmpty(claimId)) {
                filters.add({
                    property: 'claimId',
                    value: claimId
                });
            }

            if (!Ext.isEmpty(participantId)) {
                filters.add({
                    property: 'ptcpntId',
                    value: participantId
                });
            }

            filters.each(function (filter) {
                if (Ext.isEmpty(filter.value)) {
                    filters.remove(filter);
                }
            });

            return filters.getRange();
        }
    },

    filterReturnedNotesRecords: function (notesRecords, filterClaimId) {
        var notes = [];

        for (var i in notesRecords) {
            var note = notesRecords[i];

            if (Ext.isEmpty(filterClaimId) && Ext.isEmpty(note.get('claimId'))) {
                notes.push(note);
            }
            else if (note.get('claimId').localeCompare(filterClaimId) == 0) {
                notes.push(note);
            }
        }

        return notes;
    },

    replaceInvalidXmlCharacters: function (noteText) {
        var text = noteText;

        text = text.replace(new RegExp('<', 'g'), '&lt;').replace(
            new RegExp('>', 'g'), '&gt;').replace(
                new RegExp('&', 'g'), '&amp;').replace(
                    new RegExp("'", 'g'), '&quot;').replace(
                        new RegExp("’", 'g'), '&quot;');

        return text;
    },

    getSelectedOrSingleClaim: function () {
        var me = this,
            claimsGrid = me.getClaimsGrid(),
            claim = claimsGrid.getSelectionModel().hasSelection() ? claimsGrid.getSelectionModel().getSelection()[0] : null;

        if (Ext.isEmpty(claim) && claimsGrid.getStore().getCount() == 1) {
            claim = claimsGrid.getStore().getAt(0);
        }

        return claim;
    }
});