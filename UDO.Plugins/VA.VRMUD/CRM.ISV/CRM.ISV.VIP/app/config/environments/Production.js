/**
* @class VIP.config.environments.Production
*
* Production environment object containing urls for DAC, CORP, 
* MVI, PATHWAYS, VACOLS and Report Generation web service.
* Developed by: Josh Oliver
*/
Ext.define('VIP.config.environments.Production', {
    
    /**
    * @property {String} shortName
    * Short Name of environment variable
    */
    shortName: 'PROD',

    /**
    * @property {String} dacUrl
    * URL to DAC redirect web service
    */
    dacUrl: 'http://crmdac.xrm.va.gov/RedirectSvc.asmx',

    /**
    * @property {String} corpUrl
    * URL to production corporate web services
    */
    corpUrl: 'https://bepprod.vba.va.gov/',

    /**
    * @property {String} mviUrl
    * URL to production MVI web service
    */
    mviUrl: 'https://services.eauth.va.gov:9193/psim_webservice/IdMWebService',

    /**
    * @property {String} pathwaysUrl
    * URL to production Pathways web service
    */
    pathwaysUrl: 'http://152.132.35.134:5035/repositories.med.va.gov/pathways',

    /**
    * @property {String} vacolsUrl
    * URL to production VACOLS web service
    */
    vacolsUrl: 'https://vavdrapp1.aac.va.gov:443/VIERSService/AppealService/Appeal',

    /**
    * @property {String} reportWebServiceUrl
    * URL to production Report Generation web service
    */
    reportWebServiceUrl: 'https://reports.xrm.va.gov/ReportGen/ReportGen.asmx'
});