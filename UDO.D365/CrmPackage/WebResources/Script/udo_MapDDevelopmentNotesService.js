"use strict";

var ws = ws || {};
ws.mapDDevelopmentNotes = {};

// Function will be called by form onLoad event
ws.mapDDevelopmentNotes.initalize = function () {
    //=====================================================================================================
    // START MapDDevelopmentNotesService
    //=====================================================================================================
    var mapDDevelopmentNotesService = function (context) {
        this.context = context;
        this.webserviceRequestUrl = WebServiceURLRoot + 'DevelopmentNotesService/DevelopmentNotesService';
        this.prefix = 'ser';
        this.prefixUrl = 'http://services.mapd.benefits.vba.va.gov/';
    };
    mapDDevelopmentNotesService.prototype = new webservice;
    mapDDevelopmentNotesService.prototype.constructor = mapDDevelopmentNotesService;
    window.mapDDevelopmentNotesService = mapDDevelopmentNotesService;
    //=====================================================================================================
    // START Individual MapDDevelopmentNotesService Methods
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findDevelopmentNotes
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.mapd.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findDevelopmentNotes>
    //         <!--Optional:-->
    //         <ptcpntId>?</ptcpntId>
    //         <!--Optional:-->
    //         <claimid>?</claimid>
    //      </ser:findDevelopmentNotes>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    var findDevelopmentNotes = function (context) {
        this.context = context;

        this.serviceName = 'findDevelopmentNotes';
        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'MapDDevelopmentNotesService.findDevelopmentNotes';
        this.wsMessage.serviceName = 'findDevelopmentNotes';
        this.wsMessage.friendlyServiceName = 'Development Notes';
        this.responseFieldSchema = 'va_finddevelopmentnotesresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['ptcpntId'] = null;
    };
    findDevelopmentNotes.prototype = new mapDDevelopmentNotesService;
    findDevelopmentNotes.prototype.constructor = findDevelopmentNotes;
    findDevelopmentNotes.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
        var innerXml;

        var claimId = this.context.parameters['claimId'] === null ? "" : this.context.parameters['claimId'];
        var ptcpntId = this.context.parameters['ptcpntId'];

        if (ptcpntId && ptcpntId !== undefined && ptcpntId !== '') {
            innerXml = '<ser:findDevelopmentNotes>'
                + '<claimId>' + claimId + '</claimId>'
                + '<ptcpntId>' + ptcpntId + '</ptcpntId></ser:findDevelopmentNotes>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'You must provide a Participant Id to perform the search';
            return null;
        }

        return innerXml;
    };
    window.findDevelopmentNotes = findDevelopmentNotes;
    //END findDevelopmentNotes
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START createNote
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.mapd.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:createNote>
    //         <!--Optional:-->
    //         <note>
    //            <!--Optional:-->
    //            <callId>?</callId>
    //            <!--Optional:-->
    //            <jrnDt>?</jrnDt>
    //            <!--Optional:-->
    //            <jrnLctnId>?</jrnLctnId>
    //            <!--Optional:-->
    //            <jrnObjId>?</jrnObjId>
    //            <!--Optional:-->
    //            <jrnSttTc>?</jrnSttTc>
    //            <!--Optional:-->
    //            <jrnUserId>?</jrnUserId>
    //            <!--Optional:-->
    //            <parentId>?</parentId>
    //            <!--Optional:-->
    //            <parentName>?</parentName>
    //            <!--Optional:-->
    //            <rowCnt>?</rowCnt>
    //            <!--Optional:-->
    //            <rowId>?</rowId>
    //            <!--Optional:-->
    //            <bnftClmNoteTc>?</bnftClmNoteTc>
    //            <!--Optional:-->
    //            <clmId>?</clmId>
    //            <!--Optional:-->
    //            <createDt>?</createDt>
    //            <!--Optional:-->
    //            <modifdDt>?</modifdDt>
    //            <!--Optional:-->
    //            <noteId>?</noteId>
    //            <!--Optional:-->
    //            <noteOutTn>?</noteOutTn>
    //            <!--Optional:-->
    //            <ptcpntId>?</ptcpntId>
    //            <!--Optional:-->
    //            <ptcpntNoteTc>?</ptcpntNoteTc>
    //            <!--Optional:-->
    //            <stdNoteId>?</stdNoteId>
    //            <!--Optional:-->
    //            <suspnsDt>?</suspnsDt>
    //            <!--Optional:-->
    //            <txt>?</txt>
    //            <!--Optional:-->
    //            <userId>?</userId>
    //            <!--Optional:-->
    //            <userNm>?</userNm>
    //         </note>
    //      </ser:createNote>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //=====================================================================================================
    var createNote = function (context) {
        this.context = context;

        this.serviceName = 'createNote';
        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'MapDDevelopmentNotesService.createNote';
        this.wsMessage.serviceName = 'createNote';
    };
    createNote.prototype = new mapDDevelopmentNotesService;
    createNote.prototype.constructor = createNote;
    createNote.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml = '';
        var clmId = this.context.parameters['claimId'];
        var ptcpntId = this.context.parameters['ptcpntId'];
        var noteText = this.context.parameters['noteText'];

        var timezone = '';
        var tzOffset = -new Date().getTimezoneOffset() / 60;
        if (Math.abs(tzOffset) < 10) { timezone += '-0' + Math.abs(tzOffset); }
        else { timezone += tzOffset; }
        timezone += ':00';

        var today = new Date().format('yyyy-MM-dd\'T\'HH:mm:ss') + timezone;
        var claimNote = true;
        var userId = null;

        if (!this.context.user) { this.context.user = GetUserSettingsForWebservice(); }

        if (this.context.user && this.context.user.pcrId) userId = this.context.user.pcrId;
        if ((this.context.isContactNote && this.context.isContactNote === true) || !clmId || clmId.length === 0) claimNote = false;

        innerXml =
            '<ser:createNote><note>' +
            (claimNote ? '<bnftClmNoteTc>CLMDVLNOTE</bnftClmNoteTc>' : '<ptcpntNoteTc>CLMNTCONTACT</ptcpntNoteTc>') +
            (clmId && clmId !== '' && claimNote ? '<clmId>' + clmId + '</clmId>' : '') +
            (ptcpntId && ptcpntId !== '' ? '<ptcpntId>' + ptcpntId + '</ptcpntId>' : '') +
            '<noteOutTn>' + (claimNote ? 'Claim Development Note' : 'Contact with Claimant') + '</noteOutTn>' +
            '<txt>' + noteText + '</txt>' +
            '<createDt>' + today + '</createDt>' +
            '<modifdDt>' + today + '</modifdDt>' +
            '<jrnDt>' + today + '</jrnDt>' +
            '<suspnsDt>' + today + '</suspnsDt>' +
            (userId ? '<userId>' + userId + '</userId>' : '') +
            '</note></ser:' + this.serviceName + '>';


        return innerXml;
    };
    window.createNote = createNote;
    // END createNote
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START updateNote
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.mapd.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:updateNote>
    //         <!--Optional:-->
    //         <note>
    //            <!--Optional:-->
    //            <callId>?</callId>
    //            <!--Optional:-->
    //            <jrnDt>?</jrnDt>
    //            <!--Optional:-->
    //            <jrnLctnId>?</jrnLctnId>
    //            <!--Optional:-->
    //            <jrnObjId>?</jrnObjId>
    //            <!--Optional:-->
    //            <jrnSttTc>?</jrnSttTc>
    //            <!--Optional:-->
    //            <jrnUserId>?</jrnUserId>
    //            <!--Optional:-->
    //            <parentId>?</parentId>
    //            <!--Optional:-->
    //            <parentName>?</parentName>
    //            <!--Optional:-->
    //            <rowCnt>?</rowCnt>
    //            <!--Optional:-->
    //            <rowId>?</rowId>
    //            <!--Optional:-->
    //            <bnftClmNoteTc>?</bnftClmNoteTc>
    //            <!--Optional:-->
    //            <clmId>?</clmId>
    //            <!--Optional:-->
    //            <createDt>?</createDt>
    //            <!--Optional:-->
    //            <modifdDt>?</modifdDt>
    //            <!--Optional:-->
    //            <noteId>?</noteId>
    //            <!--Optional:-->
    //            <noteOutTn>?</noteOutTn>
    //            <!--Optional:-->
    //            <ptcpntId>?</ptcpntId>
    //            <!--Optional:-->
    //            <ptcpntNoteTc>?</ptcpntNoteTc>
    //            <!--Optional:-->
    //            <stdNoteId>?</stdNoteId>
    //            <!--Optional:-->
    //            <suspnsDt>?</suspnsDt>
    //            <!--Optional:-->
    //            <txt>?</txt>
    //            <!--Optional:-->
    //            <userId>?</userId>
    //            <!--Optional:-->
    //            <userNm>?</userNm>
    //         </note>
    //      </ser:updateNote>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //=====================================================================================================
    var updateNote = function (context) {
        this.context = context;

        this.serviceName = 'updateNote';
        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'MapDDevelopmentNotesService.updateNote';
        this.wsMessage.serviceName = 'updateNote';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['noteId'] = null;
    };
    updateNote.prototype = new mapDDevelopmentNotesService;
    updateNote.prototype.constructor = updateNote;
    updateNote.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
        var innerXml = '';
        var clmId = this.context.parameters['claimId'];
        var ptcpntId = this.context.parameters['ptcpntId'];
        var noteText = this.context.parameters['noteText'];
        var noteId = this.context.parameters['noteId'];

        var timezone = '';
        var tzOffset = -new Date().getTimezoneOffset() / 60;
        if (Math.abs(tzOffset) < 10) { timezone += '-0' + Math.abs(tzOffset); }
        else { timezone += tzOffset; }
        timezone += ':00';

        var today = new Date().format('yyyy-MM-dd\'T\'HH:mm:ss') + timezone;
        var userId = null;

        if (!this.context.user) { this.context.user = GetUserSettingsForWebservice(); }
        if (this.context.user && this.context.user.pcrId) userId = this.context.user.pcrId;

        var createDt = this.context.parameters['createDt'];

        var claimNote = true;
        if ((this.context.isContactNote && this.context.isContactNote === true) || !clmId || clmId.length === 0) claimNote = false;

        if ((noteId && noteId !== '') || createDt) {
            if (noteId && noteId !== '') createDt = null;    // createDt only needed if ID is missing

            innerXml =
                '<ser:updateNote><note>' +
                '<jrnDt>' + today + '</jrnDt>' +
                (claimNote ? '<bnftClmNoteTc>CLMDVLNOTE</bnftClmNoteTc>' : '<ptcpntNoteTc>CLMNTCONTACT</ptcpntNoteTc>') +
                (clmId && clmId !== '' && claimNote ? '<clmId>' + clmId + '</clmId>' : '') +
                (noteId && noteId.length > 0 ? '<noteId>' + noteId + '</noteId>' : '') +
                (createDt ? '<createDt>' + createDt + '</createDt>' : '') +
                '<noteOutTn>' + (claimNote ? 'Claim Development Note' : 'Contact with Claimant') + '</noteOutTn>' +
                (ptcpntId && ptcpntId.length > 0 ? '<ptcpntId>' + ptcpntId + '</ptcpntId>' : '') +
                '<txt>' + noteText + '</txt>' +
                '<modifdDt>' + today + '</modifdDt>' +
                '<suspnsDt>' + today + '</suspnsDt>' +
                (userId ? '<userId>' + userId + '</userId>' : '') +
                '</note></ser:updateNote>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a note id or note Create Date present for the request';
            return null;
        }

        return innerXml;
    };
    window.updateNote = updateNote;
    // END updateNote
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    // END Individual MapDDevelopmentNotesService Methods
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    // END MapDDevelopmentNotesService
    //=====================================================================================================
};