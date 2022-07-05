/**
* @class VIP.config.environments.Integration
*
* Integration environment object containing urls for DAC, CORP, 
* MVI, PATHWAYS, VACOLS and Report Generation web service.
* Developed by: Josh Oliver
*/
Ext.define('VIP.config.environments.Integration', {
    /**
    * @property {String} shortName
    * Short Name of environment variable
    */
    shortName: 'INTI',

    /**
    * @property {String} dacUrl
    * URL to development DAC redirect web service
    */
    dacUrl: 'http://10.153.95.73/RedirectSvc.asmx',

    /**
    * @property {String} corpUrl
    * URL to development CERT corporate web services
    */
    corpUrl: 'http://vbmscert.vba.va.gov/',

    /**
    * @property {String} mviUrl
    * URL to SQA1 MVI web service
    */
    mviUrl: 'https://int.services.eauth.va.gov:9193/psim_webservice/stage1a/IdMWebService',

    /**
    * @property {String} pathwaysUrl
    * URL to development Pathways web service
    */
    pathwaysUrl: 'http://vahdrtvapp02.aac.va.gov:7251/repositories.med.va.gov/pathways',

    /**
    * @property {String} vacolsUrl
    * URL to development VACOLS web service
    */
    vacolsUrl: 'http://vaausvrsapp81.aac.va.gov/VIERSService/v1/AppealService/Appeal',

    /**
    * @property {String} reportWebServiceUrl
    * URL to development Report Generation web service
    */
    reportWebServiceUrl: 'http://10.153.96.36/ReportGen/ReportGen.asmx'
});