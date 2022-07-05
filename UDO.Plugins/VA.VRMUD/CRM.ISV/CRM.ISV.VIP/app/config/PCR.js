/**
* @class VIP.config.PCR
* @singleton
*
* A singleton context object to encapsulate any user specific configurations. 
* All of these should be defined at application instantiation.
* 
* Developed by: Jonas Dawson
*
* To Do: Mark required configs with the required tag.
*/
Ext.define('VIP.config.PCR', {
    singleton: true,
    config: {
        /**
        * @cfg {String} applicationName
        * @accessor
        */
        applicationName: undefined,
        /**
        * @cfg {String} domainName
        * @accessor
        */
        domainName: undefined,
        /**
        * @cfg {Number} fileNumber
        * @accessor
        */
        fileNumber: undefined,
        /**
        * @cfg {String} fullName
        * @accessor
        */
        fullName: undefined,
        /**
        * @cfg {String} internalEmailAddress
        * @accessor
        */
        internalEmailAddress: undefined,
        /**
        * @cfg {String} ipAddress
        * @accessor
        */
        ipAddress: undefined,
        /**
        * @cfg {String} mobileAlertEmailAddress
        * @accessor
        */
        mobileAlertEmailAddress: undefined,
        /**
        * @cfg {String} sensitivityLevel
        * @accessor
        */
        sensitivityLevel: undefined,
        /**
        * @cfg {Number} ssn
        * @accessor
        */
        ssn: undefined,
        /**
        * @cfg {Number} stationNumber
        * @accessor
        */
        stationNumber: undefined,
        /**
        * @cfg {String} systemUserId
        * @accessor
        */
        systemUserId: undefined,
        /**
        * @cfg {String} webServiceLoginName
        * @accessor
        */
        webServiceLoginName: undefined
    }
});