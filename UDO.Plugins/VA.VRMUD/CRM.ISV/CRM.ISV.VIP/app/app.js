/**
* @class VIP.app.Application
*
* The top-level controller of the application.
*/
_extApp = null;
_cvipSearchContext = null;
_ccrmEnvironment = null;
_cpcrContext = null;
_cuseCache = null;
_cxmlCachedData = null;
_startOperation = null;
_appLaunched = false;
_crmEnvironment = null;

function _appSearch(vipSearchContext) {
    if (Ext.isEmpty(_extApp)) {
        // todo start search when launch is done
        _cvipSearchContext = vipSearchContext;
        if (Ext.isEmpty(_startOperation)) { _startOperation = Ext.create('Ext.util.HashMap'); }
        _startOperation.add('search', null);
        return;
    }
    _extApp.search(vipSearchContext);
}

function _appOnCallStarted(crmEnvironment, pcrContext, useCache, xmlCachedData) {
    if (Ext.isEmpty(_extApp)) {
        // todo start call when launch is done
        _ccrmEnvironment = crmEnvironment; _cpcrContext = pcrContext; _cuseCache = useCache; _cxmlCachedData = xmlCachedData;
        if (Ext.isEmpty(_startOperation)) { _startOperation = Ext.create('Ext.util.HashMap'); }
        _startOperation.add('loadcache', null);
        return;
    }
    _extApp.onCallStart(crmEnvironment, pcrContext, useCache, xmlCachedData);
}

function _appFireEvent(eventName, objParam) {
    if (Ext.isEmpty(_extApp)) {
        return;
    }
    _extApp.fireEvent(eventName, objParam);
}

function _appStartTimer(timerData) {
    if (Ext.isEmpty(_extApp)) {
        if (Ext.isEmpty(_startOperation)) { _startOperation = Ext.create('Ext.util.HashMap'); }
        _startOperation.add('starttimer', timerData); return;
    }
    _extApp.fireEvent('calltimerstarted', { 'timerData': timerData });
}

function _appStopTimer() {
    if (!Ext.isEmpty(_extApp)) {
        _extApp.fireevent('calltimerstopped');
    }
}
//Remove zone to ensure dates are the same across local zones
function _dtZoneless(dt) {
    var output = new Date();
    if ((dt !== null) && (dt !== undefined)) {
        switch (dt.length) {
            case 25:
                { //2013-07-01T00:00:00-05:00
                    var strZoneless = dt.slice(0, 10);
                    var dtArr = strZoneless.split('-');
                    strZoneless = dtArr[1] + '-' + dtArr[2] + '-' + dtArr[0]; //07-01-2013
                    //output = Ext.Date.format(new Date(strZoneless), 'm/d/Y');
                    output = new Date(strZoneless); //Return Date
                    break;
                }
            case 28: //Thu, 1 Dec 2011 06:00:00 UTC
            case 29:
                {
                    var dtArr = dt.split(' ');
                    var strZoneless = dtArr[2] + ' ' + dtArr[1] + ' ' + dtArr[3]; //Dec 1 2011
                    output = Ext.Date.format(new Date(strZoneless), 'm/d/Y'); //Return String m/d/Y
                    break;
                }
            default:
                {
                    output = null;
                    break;
                }
        }
    }
    else {
        output = null;
    }
    return output;
}
/* EXTjs -------------------------------------------------------
*/

Ext.Loader.setConfig({
    enabled: true,
    disableCaching: false,
    paths: {
        'Ext': '.',
        'VIP': 'app'
    }
});
Ext.require([
	'Ext.container.Viewport',
	'VIP.data.proxy.Soap',
	'VIP.services.CrmEnvironment',
    'VIP.model.Environment',
    'VIP.util.soap.Envelope',
    'VIP.store.debug.WebServiceRequestHistory',
    'VIP.util.soap.WebserviceResponseAnalyzer',
    'VIP.soap.analyzers.mvi.PersonSearch',
    'VIP.soap.analyzers.mvi.GetCorrespondingIds',
	'VIP.services.WebServiceMessageHandler',
	'VIP.soap.envelopes.VARequestBuilder',
	'VIP.model.Pcr',
	'VIP.model.PersonInquiry',
	'VIP.services.CacheProcessor',
	'VIP.model.Environment'
]);

Ext.BLANK_IMAGE_URL = 'resources/images/blank.gif';
Ext.application({
    name: 'VIP',

    controllers: [
		'Appeals',
		'AppStatus',
		'Awards',
		'Birls',
		'Claims',
		'claims.Benefits',
		'claims.Notes',
		'claims.Details',
		'debug.WebServiceRequestHistory',
		'services.ServiceRequest',
        'services.VAI',
		'ContentPanel',
		'Denials',
		'PaymentInformation',
		'FiduciaryPoa',
		'Launch',
		'MilitaryService',
		'PersonInfo',
		'PersonSelection',
		'PersonTabPanel',
		'Ratings',
		'SearchOverview',
		'Viewport',
		'PaymentHistory',
		'Mvi',
		'Pathways',
		'VirtualVA',
        'PersonVadir'
    ],

    autoCreateViewport: true,

    // Execution context
    responseCacheMap: null,        //  HashMap   for Store names to xml field names map
    crmEnvironment: null,           // 'VIP.model.Environment' Environment data, URLs, DAC etc
    pcrContext: null,               // 'VIP.model.Pcr' holding user login name, password, app name etc.
    useCachedResponseData: false,   // true to load from cached data instead of running search
    xmlCachedData: null,
    personInquiryModel: null,       // instance of Ext.create('VIP.model.PersonInquiry' containing both search parameters and some responses such as ssn, dob, file number   
    serviceRequest: { va_SelectedPayeeCode: '', va_ServiceRequestType: '', va_SelectedSSN: '', va_SelectedPID: '', va_DepAdd: [], va_DepFiduciaryPoa: [], va_DepPayment: [], va_DepCurrFiduciary: [], va_DepCorp: [] },
    // End of Execution context
    // ***********************

    launch: function () {
        var me = this;
        _crmEnvironment = Ext.create('VIP.model.Environment');
        me.fireEvent('hidealerticon'); //app status listens to this

        Ext.override(Ext.selection.RowModel, {
            onLastFocusChanged: function (oldFocused, newFocused, supressFocus) {
                if (this.views && this.views.length) {
                    this.callOverridden(arguments);
                }
            },
            onSelectChange: function (record, isSelected, suppressEvent, commitFn) {
                if (this.views && this.views.length) {
                    this.callOverridden(arguments);
                }
            }
        });

        if (Ext.isEmpty(parent.Xrm)) {
            me.fireEvent('parentformnotdetected');
        }

        me.responseCacheMap = me.initializeResponseCacheMap();
        _extApp = me;
        Ext.log('The viewport has been created.');

        // global ws message analyzer and handler
        me.webServiceMessageHandler = Ext.create('VIP.services.WebServiceMessageHandler');

        //debugger;

        // If container has passed cached data, start displaying it now
        // detect if container want to load cache
        if ((Ext.isEmpty(_appLaunched) || _appLaunched == false) && parent && parent._cachedData && parent._cachedData['Environment']) {
            me.onCallStart(parent._cachedData['Environment'], parent._cachedData['UserSettings'], parent._cachedData['UseCache'], parent._cachedData['Cache']);
            return;
        }
        // if client called extjs op before it loaded, process here
        if (!Ext.isEmpty(_startOperation)) {

            if (_startOperation.containsKey('search')) { _extApp.search(_cvipSearchContext); }
            if (_startOperation.containsKey('loadcache')) { _extApp.onCallStart(_ccrmEnvironment, _cpcrContext, _cuseCache, _cxmlCachedData); }
            if (_startOperation.containsKey('starttimer')) { _extApp.fireEvent('calltimerstarted', { 'timerData': _startOperation['starttimer'] }); }

            _startOperation = null;
        }

        // Setup the env variables
        if (parent && parent._currentEnv) {
            _crmEnvironment = me.initializeEnvironment(parent._currentEnv);
            me.crmEnvironment = _crmEnvironment;
        }
    },

    onCallStart: function (crmEnvironment, pcrContext, useCache, xmlCachedData) {
        var me = this;

        Ext.require('VIP.soap.envelopes.VARequestBuilder');
        var timerData = null, stopTimer = false;

        if (parent._cachedData) {
            timerData = parent._cachedData['TimerData'];
            stopTimer = parent._cachedData['StopTimer'];
        }

        _extApp.fireEvent('calltimerstarted', { 'timerData': timerData, 'stopTimer': stopTimer });

        Ext.apply(VIP.soap.envelopes.VARequestBuilder, {
            namespaces: {
                'q0': 'http://services.share.benefits.vba.va.gov/',
                'ser': 'http://services.share.benefits.vba.va.gov/',
                'ws': 'http://ws.vrm.benefits.vba.va.gov/',
                'pay': 'http://paymenthistory.services.ebenefits.vba.va.gov/',
                'app': 'http://www.va.gov/schema/AppealService'
            },
            isDacRequest: !Ext.isEmpty(_crmEnvironment.globalDAC),
            baseUrl: crmEnvironment.CORP,
            serviceUrl: '',
            dacUrl: crmEnvironment.globalDAC,
            isPROD: crmEnvironment.isPROD,
            securityContext: {
                username: pcrContext.loginName,
                password: '',
                clientMachine: pcrContext.clientMachine,
                stationId: pcrContext.stationId,
                applicationName: pcrContext.applicationName
            }
        });

        var pcrModel = Ext.create('VIP.model.Pcr', {
            userName: pcrContext.userName,
            password: pcrContext.password,
            clientMachine: pcrContext.clientMachine,
            stationId: pcrContext.stationId,
            applicationName: pcrContext.applicationName,
            pcrSensitivityLevel: pcrContext.pcrSensitivityLevel,
            loginName: pcrContext.loginName,
            ssn: pcrContext.userSsn,
            fileNumber: pcrContext.userFileNumber,
            fullName: pcrContext.userFullName,
            email: pcrContext.email,
            pcrId: pcrContext.pcrId,
            site: pcrContext.site
        });

        var theApp = me.application;
        if (Ext.isEmpty(theApp)) { theApp = _extApp; }

        // Set execution context
        theApp.crmEnvironment = me.initializeEnvironment(crmEnvironment);
        theApp.pcrContext = pcrModel;
        theApp.useCachedResponseData = useCache;
        theApp.xmlCachedData = xmlCachedData;
        theApp.personInquiryModel = Ext.create('VIP.model.PersonInquiry', {
            firstName: null,
            lastName: null,
            middleName: null,
            dob: null,
            dod: null,
            fileNumber: null,
            ssn: null,
            participantId: null,

            // MVI/Pathways
            gender: null,
            appointementFromDate: null,
            appointementToDate: null,
            doSearchPathways: true,
            doSearchVadir: false
        });

        // cached data is collection using values specified in  app.responseCacheMap

        me.fireEvent('callstarted');
        _appLaunched = true;
        //check if the data is cached. if so, tell the mux proxies to use cache instead

        if (useCache) {
            // use cache processor
            var cacheProcessor = Ext.create('VIP.services.CacheProcessor', {
                cacheMap: theApp.responseCacheMap,
                cacheDataCollection: xmlCachedData
            });

            cacheProcessor.loadAll();
            me.fireEvent('cacheddataloaded');
        }
    },

    search: function (vipSearchContext, vipSearchPcrModel, vipSearchPersonInquiryModel) {
        var me = this,
			persistenceRecord;

        Ext.data.StoreManager.each(function (item, index, length) {
            item.removeAll();
        });
        //me.getFiduciaryInfo().getForm().reset(); use this to clear Panels of data.
        //me.getPersistenceStore().load();
        //persistenceRecord = me.getPersistenceStore().getAt(0);

        if (vipSearchContext != null) {
            if (Ext.isEmpty(vipSearchContext)) {
                alert('Application Search function requires search context.');
                return;
            }

            if (Ext.isEmpty(_crmEnvironment.get('envName'))) {
                _crmEnvironment = me.initializeEnvironment(vipSearchContext.environment);
            }

            // user data from parent screen
            var userName = vipSearchContext.user.userName,
				password = '',
				clientMachine = vipSearchContext.user.clientMachine,
				stationId = vipSearchContext.user.stationId,
				applicationName = vipSearchContext.user.applicationName,
				pcrSensitivityLevel = vipSearchContext.user.pcrSensitivityLevel,
				loginName = vipSearchContext.user.loginName,
				userSsn = vipSearchContext.user.ssn,
				userFileNumber = vipSearchContext.user.fileNumber,
				userFullName = vipSearchContext.user.fullName,
				email = vipSearchContext.user.email,
				pcrId = vipSearchContext.user.pcrId,
				site = vipSearchContext.user.site;

            // search parameters
            var firstName = vipSearchContext.firstName,
				lastName = vipSearchContext.lastName,
				middleName = vipSearchContext.middleName,

				dob = vipSearchContext.dob,
				city = vipSearchContext.city,
				state = vipSearchContext.state,
				zipCode = vipSearchContext.zipCode,

				branchOfService = vipSearchContext.branchOfService,
				serviceNumber = vipSearchContext.serviceNumber,
				insuranceNumber = vipSearchContext.insuranceNumber,
				dod = vipSearchContext.dod,
				eod = vipSearchContext.eod,
				rad = vipSearchContext.rad,
				suffix = vipSearchContext.suffix,
				folderLocation = vipSearchContext.folderLocation,
				payeeNumber = vipSearchContext.payeeNumber,

				fileNumber = vipSearchContext.fileNumber,
				participantId = vipSearchContext.participantId,

            //MVI/Pathways
				gender = vipSearchContext.gender,
				appointementFromDate = vipSearchContext.appointementFromDate,
				appointementToDate = vipSearchContext.appointementToDate,
				doSearchPathways = vipSearchContext.doSearchPathways;

            // Vadir
            doSearchVadir = vipSearchContext.doSearchVadir;

            pcrModel = Ext.create('VIP.model.Pcr', {
                userName: userName,
                password: password,
                clientMachine: clientMachine,
                stationId: stationId,
                applicationName: applicationName,
                pcrSensitivityLevel: pcrSensitivityLevel,
                loginName: loginName,
                ssn: userSsn,
                fileNumber: userFileNumber,
                fullName: userFullName,
                email: email,
                pcrId: pcrId,
                site: site
            });

            personInquiryModel = Ext.create('VIP.model.PersonInquiry', {
                searchCORPBy: vipSearchContext.searchCORPBy,  //SSN or PARTICIPANTID
                firstName: firstName,                      //!!!Update from search results
                lastName: lastName,                          //!!!Update from search results
                middleName: middleName,                      //!!!Update from search results
                dob: dob,                                     //!!!Update from search results
                city: city,
                state: state,
                zipCode: zipCode,
                branchOfService: branchOfService,
                serviceNumber: serviceNumber,
                insuranceNumber: insuranceNumber,
                dod: dod,                                    //!!!Update from search results
                eod: eod,
                rad: rad,
                suffix: suffix,
                folderLocation: folderLocation,
                fileNumber: fileNumber,                 //!!!Update from search results
                ssn: vipSearchContext.ssn,              //!!!Update from search results
                participantId: participantId,           //!!!Update from search results
                payeeNumber: payeeNumber,

                appealsSearchValue: vipSearchContext.appealsSearchValue,
                appealsSsn: vipSearchContext.appealsSsn,
                appealsFirstName: vipSearchContext.appealsFirstName,
                appealsLastName: vipSearchContext.appealsLastName,
                appealsDateOfBirth: vipSearchContext.appealsDateOfBirth,
                appealsCity: vipSearchContext.appealsCity,
                appealsState: vipSearchContext.appealsState,

                // MVI/Pathways
                gender: gender,
                appointementFromDate: appointementFromDate,
                appointementToDate: appointementToDate,
                doSearchPathways: doSearchPathways,
                doSearchVadir: doSearchVadir
            });
        }
        else if (vipSearchPcrModel != null && vipSearchPersonInquiryModel != null) {
            pcrModel = vipSearchPcrModel;
            personInquiryModel = vipSearchPersonInquiryModel;
        }
        else {
            alert('Application Search function requires search context or search parameters passed through URL string');
            return;
        }

        // redefine request builder
        Ext.require('VIP.soap.envelopes.VARequestBuilder');
        Ext.apply(VIP.soap.envelopes.VARequestBuilder, {
            namespaces: {
                'q0': 'http://services.share.benefits.vba.va.gov/',
                'ser': 'http://services.share.benefits.vba.va.gov/',
                'ws': 'http://ws.vrm.benefits.vba.va.gov/',
                'pay': 'http://paymenthistory.services.ebenefits.vba.va.gov/',
                'app': 'http://www.va.gov/schema/AppealService'
            },
            isDacRequest: !Ext.isEmpty(_crmEnvironment.get('globalDAC')),
            baseUrl: '',
            serviceUrl: '',
            dacUrl: _crmEnvironment.get('globalDAC'),
            isPROD: _crmEnvironment.get('isPROD'),
            securityContext: {
                username: loginName,
                password: '',
                clientMachine: clientMachine,
                stationId: stationId,
                applicationName: applicationName
            }
        });

        var theApp = me.application;
        if (Ext.isEmpty(theApp)) { theApp = _extApp; }

        // Set execution context
        if (Ext.isEmpty(theApp.crmEnvironment)) {
            theApp.crmEnvironment = _crmEnvironment;
        }
        theApp.pcrContext = pcrModel;
        theApp.useCachedResponseData = false;
        theApp.personInquiryModel = personInquiryModel;
        //var cacheMap = Ext.create('VIP.services.CacheMap');
        //theApp.responseCacheMap = cacheMap;
        theApp.webServiceMessageHandler = Ext.create('VIP.services.WebServiceMessageHandler');

        theApp.fireEvent('personinquirystarted', personInquiryModel);
    },

    initializeEnvironment: function (environment) {
        return Ext.create('VIP.model.Environment', {
            envName: environment.name,
            isPROD: environment.isPROD,
            globalDAC: environment.globalDAC,
            CORP: environment.CORP,
            VADIR: environment.VADIR,
            MVI: environment.MVI,
            MVIDAC: environment.MVIDAC,
            MVIBase: environment.MVIBase,
            Pathways: environment.Pathways,
            PathwaysBase: environment.PathwaysBase,
            Vacols: environment.Vacols,
            VacolsDAC: environment.VacolsDAC,
            VacolsBase: environment.VacolsBase,
            RepWS: environment.RepWS,
            UseMSXML: environment.UseMSXML,
            EbenefitsBase: environment.EbenefitsBase,
            VVABase: environment.VVABase,
            VVAUser: environment.VVAUser,
            VVAPassword: environment.VVAPassword
        });
    },

    initializeResponseCacheMap: function () {
        var map = new Ext.util.MixedCollection();

        // !!! Order is important

        //Person Info
        map.add('Corp', 'va_findcorprecordresponse');
        map.add('personinfo.Addresses', 'va_findaddressresponse');
        map.add('personinfo.Dependents', 'va_finddependentsresponse');
        map.add('personinfo.AllRelationships', 'va_findallrelationshipsresponse');
        map.add('personinfo.GeneralDetails', 'va_generalinformationresponsebypid');
        map.add('personinfo.Flashes', 'va_generalinformationresponsebypid');

        //BIRLS
        map.add('Birls', 'va_findbirlsresponse');

        //Awards
        map.add('Awards', 'va_generalinformationresponse');
        map.add('awards.SingleAward', 'va_generalinformationresponsebypid');
        map.add('awards.Fiduciary', 'va_awardfiduciaryresponse');
        map.add('awards.AwardInfo', 'va_findotherawardinformationresponse');
        map.add('awards.IncomeExpenseInfo', 'va_findincomeexpenseresponse');

        //Fiduciary/Poa
        map.add('FiduciaryPoa', 'va_findfiduciarypoaresponse');

        //Claims
        map.add('claims.Evidence', 'va_findunsolvedevidenceresponse');
        map.add('claims.notes.All', 'va_finddevelopmentnotesresponse');
        map.add('Claims', 'va_benefitclaimresponse');
        map.add('claims.Contentions', 'va_findcontentionsresponse');
        map.add('claims.ClaimDetail', 'va_findbenefitdetailresponse');
        map.add('claims.LifeCycle', 'va_findbenefitdetailresponse');
        map.add('claims.Suspense', 'va_findbenefitdetailresponse');
        map.add('claims.TrackedItems', 'va_findtrackeditemsresponse');
        map.add('claims.Letters', 'va_findtrackeditemsresponse');
        map.add('claims.Status', 'va_findclaimstatusresponse');
        map.add('claims.BenefitClaimDetailsByBnftClaimId', 'va_findbenefitclaimdetailsbybnftclaimidrespo');

        //Military Service
        map.add('MilitaryService', 'va_findmilitaryrecordbyptcpntidresponse');

        //Payments
        map.add('PaymentHistory', 'va_findpaymenthistoryresponse');

        //PaymentInformation
        map.add('Payment', 'va_retrievepaymentsummaryresponse');
        map.add('paymentinformation.AwardAdjustment', 'va_retrievepaymentdetailresponse');
        map.add('paymentinformation.AwardAdjustmentVo', 'va_retrievepaymentdetailresponse');
        map.add('paymentinformation.AwardReason', 'va_retrievepaymentdetailresponse');
        map.add('paymentinformation.PaymentAdjustment', 'va_retrievepaymentdetailresponse');
        map.add('paymentinformation.PaymentAdjustmentVo', 'va_retrievepaymentdetailresponse');
        map.add('paymentinformation.PaymentDetail', 'va_retrievepaymentdetailresponse');

        //Ratings
        map.add('Ratings', 'va_findratingdataresponse');

        //Denials
        map.add('Denial', 'va_finddenialsresponse');
        map.add('denials.FullDenialReason', 'va_finddenialsresponse');
        //map.add('denials.Inquiry', 'va_finddenialsresponse');

        //Pathways
        map.add('mvi.Patient', 'va_mviresponse');
        map.add('pathways.Appointment', 'va_readdataappointmentresponse');
        map.add('pathways.Patient', 'va_readdataexamresponse');

        //Appeals
        map.add('Appeal', 'va_findappealsresponse');
        map.add('appeals.Detail', 'va_findindividualappealsresponse');

        //eBenefits
        map.add('ebenefits.Ebenefits', 'va_getregistrationstatus');
        map.add('virtualva.DocumentRecord', 'va_findgetdocumentlist');

        // Vadir
        map.add('PersonVadir', 'va_findpersonresponsevadir');
        map.add('personVadir.ContactInfo', 'va_getcontactinfovadir');

        return map;
    }
});

