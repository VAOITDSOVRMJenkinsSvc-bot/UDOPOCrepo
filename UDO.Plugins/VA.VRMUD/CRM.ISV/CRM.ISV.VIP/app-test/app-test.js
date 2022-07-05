
Ext.Loader.setConfig({
    enabled: true,
    paths: {
        'VIP': '../app'
    }
});

Ext.Loader.setConfig('disableCaching', false);

Ext.Loader.require([
    'Ext.app.Application',
    'VIP.util.xml.Parser',
	'VIP.data.proxy.Soap',
    'VIP.util.soap.WebserviceResponseAnalyzer',
    'VIP.controller.ContentPanel',
    'VIP.controller.PaymentInformation',
    'VIP.store.Payment',

    'VIP.model.PersonVadir',
    'VIP.model.personVadir.Alias',
    'VIP.model.personVadir.Address',
    'VIP.model.personVadir.Email',
    'VIP.model.personVadir.Phone',
    'VIP.model.personVadir.ContactInfo',

    'VIP.store.PersonVadir',
    'VIP.store.personVadir.Alias',
    'VIP.store.personVadir.Address',
    'VIP.store.personVadir.Email',
    'VIP.store.personVadir.Phone',
    'VIP.store.personVadir.ContactInfo',

    'VIP.soap.envelopes.vadir.personSearch.FindPersonByEdipi',
    'VIP.soap.envelopes.vadir.personSearch.FindPersonByFnameLname',
    'VIP.soap.envelopes.vadir.personSearch.FindPersonByFnameLnameDob',
    'VIP.soap.envelopes.vadir.personSearch.FindPersonByLnameDob',
    'VIP.soap.envelopes.vadir.personSearch.FindPersonBySsn',
    'VIP.soap.envelopes.vadir.personSearch.GetContactInfo',

    'VIP.controller.services.ServiceRequest',
    'VIP.store.personinfo.Addresses',
    'VIP.store.FiduciaryPoa',
    'VIP.store.fiduciary.CurrentFiduciary',
    'VIP.store.Payment'
]);

var Application = null;
var _extApp = null;

Ext.onReady(function () {
    Application = Ext.create('Ext.app.Application', {
        name: 'VIP',
        appFolder: 'app',

        //controllers: ['ContentPanel', 'PaymentInformation'],

        pcrContext: Ext.create('VIP.model.Pcr', {
            userName: 'VACOGROSSJ',
            password: '',
            clientMachine: '10.224.104.174',
            stationId: '317',
            applicationName: 'VBMS',
            pcrSensitivityLevel: '9',
            loginName: 'aide\\jdawson',
            ssn: '45678',
            fileNumber: '45678',
            fullName: 'VACOGROSSJ',
            email: 'jonasd@infostrat.com',
            pcrId: '600022589',
            site: 'VA Headquarters'
        }),

        crmEnvironment: Ext.create('VIP.model.Environment', {
            envName: 'INTI',
            isPROD: false,
            globalDAC: 'http://10.153.95.73/RedirectSvc.asmx',
            CORP: 'http://vbmscert.vba.va.gov/',
            VADIR: 'http://vavdrapp80.aac.va.gov/',
            MVI: 'http://ps-esr.commserv.healthevet.va.gov:7957/psim_webservice/IdMWebService',
            MVIDAC: 'http://10.153.95.73/RedirectSvc.asmx',
            MVIBase: 'http://ps-esr.commserv.healthevet.va.gov:7957/',
            Pathways: 'http://vahdrtvapp02.aac.va.gov:7251/repositories.med.va.gov/pathways',
            PathwaysBase: 'http://vahdrtvapp02.aac.va.gov:7251/',
            Vacols: 'http://vaausvrsapp80.aac.va.gov/VIERSService/AppealService/Appeal',
            VacolsDAC: 'http://10.153.95.73/RedirectSvc.asmx',
            VacolsBase: 'http://vaausvrsapp80.aac.va.gov/',
            RepWS: 'http://10.153.96.36/ReportGen/ReportGen.asmx',
            UseMSXML: false,
            EbenefitsBase: 'http://vaebnweb2.aac.va.gov/',
            VVABase: 'https://vbaphi5dopp.vba.va.gov:7002/',
            VVAUser: 'TEST',
            VVAPassword: 'YYYYY'
        }),

    });

    _extApp = Application;

    jasmine.getEnv().addReporter(new jasmine.HtmlReporter());
    jasmine.getEnv().execute();

});