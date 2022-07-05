Ext.define('VIP.soap.envelopes.mapd.developmentnotes.CreateNote', {
    extend: 'VIP.soap.envelopes.mapd.DevelopmentNotesTemplate',
    alias: 'envelopes.CreateNote',

    config: {
        ptcpntNoteTc: 'CLMNTCONTACT',
        bnftClmNoteTc: 'CLMDVLNOTE',
        clmId: '',
        ptcpntId: '',
        txt: '',
        noteOutTn: '',
        createDt: '',
        modifdDt: '',
        jrnDt: '',
        suspnsDt: '',
        userId: '' // TODO: figure out what this should be set to
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {

        var me = this;

        me.initConfig(config);

        me.callParent();

        var today = Ext.Date.format(new Date(), "c"); //e.g., 2012-04-06T10:47:00-04:00

        me.setBody('createNote', {
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
                    value: '' //being set below
                },
                txt: {
                    namespace: '',
                    value: me.getTxt()
                },
                createDt: {
                    namespace: '',
                    value: today
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

        //Remove claim related nodes if clmId is empty
        if (config && config != undefined) {
            if (Ext.isEmpty(me.getClmId())) {
                delete me.request.Envelope.Body.createNote.note.bnftClmNoteTc;
                delete me.request.Envelope.Body.createNote.note.clmId;
                me.request.Envelope.Body.createNote.note.noteOutTn.value = 'Contact with Claimant';
            } else {
                delete me.request.Envelope.Body.createNote.note.ptcpntNoteTc;
                me.request.Envelope.Body.createNote.note.noteOutTn.value = 'Claim Development Note';
            }

        }
    }
});

