Ext.define('VIP.soap.envelopes.mapd.developmentnotes.UpdateNote', {
    extend: 'VIP.soap.envelopes.mapd.DevelopmentNotesTemplate',

    config: {
        ptcpntNoteTc: 'CLMNTCONTACT',
        bnftClmNoteTc: 'CLMDVLNOTE',
        clmId: '',
        ptcpntId: '',
        noteId: '',
        txt: '',
        noteOutTn: '',
        createDt: '',
        modifdDt: '',
        jrnDt: '',
        suspnsDt: '',
        userId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {

        var me = this;

        me.initConfig(config);

        me.callParent();

        var today = Ext.Date.format(new Date(), "c"); //e.g., 2012-04-06T10:47:00-04:00

        me.setBody('updateNote', {
            namespace: 'ser',
            note: {
                namespace: '',
                bnftClmNoteTc: {
                    namespace: '',
                    value: me.getBnftClmNoteTc()
                },
                ptcpntNoteTc: {
                    namespace: '',
                    value: me.getPtcpntNoteTc()
                },
                clmId: {
                    namespace: '',
                    value: me.getClmId()
                },
                ptcpntId: {
                    namespace: '',
                    value: me.getPtcpntId()
                },
                noteOutTn: {
                    namespace: '',
                    value: me.getNoteOutTn()
                },
                noteId: {
                    namespace: '',
                    value: me.getNoteId()
                },
                txt: {
                    namespace: '',
                    value: me.getTxt()
                },
                createDt: {
                    namespace: '',
                    value: me.getCreateDt()
                },
                modifdDt: {
                    namespace: '',
                    value: today
                },
                jrnDt: {
                    namespace: '',
                    value: today
                },
                suspnsDt: {
                    namespace: '',
                    value: today
                },
                userId: {
                    namespace: '',
                    value: !Ext.isEmpty(_extApp.pcrContext) ? _extApp.pcrContext.data.pcrId : null
                }
            }
        });

        me.addNamespace('ser', 'http://services.mapd.benefits.vba.va.gov/');
        
        //tweaking xml envelope depending on the data

        if (config && config != undefined) {
            if (Ext.isEmpty(me.getClmId())) {
                delete me.request.Envelope.Body.updateNote.note.bnftClmNoteTc;
                delete me.request.Envelope.Body.updateNote.note.clmId;
                me.request.Envelope.Body.updateNote.note.noteOutTn.value = 'Contact with Claimant';
            } else {
                delete me.request.Envelope.Body.updateNote.note.ptcpntNoteTc;
                me.request.Envelope.Body.updateNote.note.noteOutTn.value = 'Claim Development Note';
            }

            if (Ext.isEmpty(me.getNoteId())) {
                delete me.request.Envelope.Body.updateNote.note.nodeId;
            } else {
                delete me.request.Envelope.Body.updateNote.note.createDt;
            }
        }
    }
});

