"use strict";

var ws = ws || {};
ws.addressValidation = {};
// Function will be called by form code

ws.addressValidation.initalize = function () {
    //=====================================================================================================
    // START AddressWebService
    //=====================================================================================================
    var addressValidationWebService = function (context) {
        this.context = context;
        this.webserviceRequestUrl = _currentEnv.AddressVal; // WebServiceURLRoot;

        this.prefix = 'val';
        this.prefixUrl = 'http://www.g1.com/services/ValidateAddress';
        this.contextParameters = (context && context.contextParameters) ? context.contextParameters : [];
        this.optionsParameters = (context && context.optionsParameters) ? context.optionsParameters : [];
        this.addressParameters = (context && context.addressParameters) ? context.addressParameters : [];
    };
    addressValidationWebService.prototype = new webservice;
    addressValidationWebService.prototype.constructor = addressValidationWebService;
    addressValidationWebService.prototype.contextParameters = [
        { name: 'account.id', value: 'guest' },
        { name: 'account.password', value: '' }
    ];
    addressValidationWebService.prototype.optionsParameters = [
        { name: 'PerformUSProcessing', value: null },
        { name: 'PerformCanadianProcessing', value: null },
        { name: 'PerformInternationalProcessing', value: null }
    ];
    addressValidationWebService.prototype.addressParameters = [
        { name: 'AddressLine1', value: null },
        { name: 'AddressLine2', value: null },
        { name: 'AddressLine3', value: null },
        { name: 'AddressLine4', value: null },
        { name: 'City', value: null },
        { name: 'StateProvince', value: null },
        { name: 'PostalCode', value: null },
        { name: 'Country', value: null }
    ];
    window.addressValidationWebService = addressValidationWebService;

    //=====================================================================================================
    // START Individual addressValidationWebService Methods
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    // START validateAddress
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:val="http://www.g1.com/services/ValidateAddress">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <val:ValidateAddressRequest>
    //         <val:context>
    //            <val:account.id>guest</val:account.id>
    //            <!--Optional:-->
    //            <val:account.password></val:account.password>
    //         </val:context>
    //         <val:rows>
    //            <!--1 or more repetitions:-->
    //            <val:row>
    //               <!--You may enter the following 12 items in any order-->
    //               <!--Optional:-->
    //               <val:AddressLine1>4301 Connecticut Avenue N.W.</val:AddressLine1>
    //               <!--Optional:-->
    //               <val:AddressLine2>Suite 3200</val:AddressLine2>
    //               <!--Optional:-->
    //               <val:AddressLine3></val:AddressLine3>
    //               <!--Optional:-->
    //               <val:AddressLine4></val:AddressLine4>
    //               <!--Optional:-->
    //               <val:City>Washington</val:City>
    //               <!--Optional:-->
    //               <val:StateProvince>DC</val:StateProvince>
    //               <!--Optional:-->
    //               <val:PostalCode>20008</val:PostalCode>
    //               <!--Optional:-->
    //               <val:Country></val:Country>
    //               <!--Optional:-->
    //               <val:FirmName></val:FirmName>
    //               <!--Optional:-->
    //               <val:USUrbanName></val:USUrbanName>
    //               <!--Optional:-->
    //               <val:CanLanguage></val:CanLanguage>
    //               <!--Optional:-->
    //               <val:user_fields>
    //                  <!--Zero or more repetitions:-->
    //                  <val:user_field>
    //                     <val:name></val:name>
    //                     <val:value></val:value>
    //                  </val:user_field>
    //               </val:user_fields>
    //            </val:row>
    //         </val:rows>
    //      </val:ValidateAddressRequest>
    //   </soapenv:Body>
    //</soapenv:Envelope>

    var validateAddress = function (context) {
        this.context = context;
        this.webserviceRequestUrl = _currentEnv.AddressVal;

        this.serviceName = 'ValidateAddress';
        this.responseFieldSchema = 'va_validateaddressresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        //this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'addressValidationWebService.ValidateAddress';
        this.wsMessage.serviceName = 'ValidateAddress';
        this.wsMessage.friendlyServiceName = 'Addresses';

        this.requiredSearchParameters = new Array();
    };

    validateAddress.prototype = new addressValidationWebService;
    validateAddress.prototype.constructor = validateAddress;

    validateAddress.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
        var innerXml = '';
        var i = 0;

        var contextParams = this.context.contextParameters;
        var optionsParams = this.context.optionsParameters;
        var addressParams = this.context.addressParameters;

        if (contextParams && contextParams.length > 0) {
            innerXml += '<val:context>';
            for (i = 0; i < contextParams.length; i++) {
                innerXml += '<val:' + contextParams[i].name + '>' +
                    contextParams[i].value +
                    '</val:' + contextParams[i].name + '>';
            }
            innerXml += '</val:context>';
        }

        if (optionsParams && optionsParams.length > 0) {
            innerXml += '<val:options>';
            for (i = 0; i < optionsParams.length; i++) {
                innerXml += '<val:' + optionsParams[i].name + '>' +
                    optionsParams[i].value +
                    '</val:' + optionsParams[i].name + '>';
            }
            innerXml += '</val:options>';
        }

        if (addressParams && addressParams.length > 0) {
            innerXml += '<val:rows><val:row>';

            for (i = 0; i < addressParams.length; i++) {
                innerXml += '<val:' + addressParams[i].name + '>' +
                    addressParams[i].value +
                    '</val:' + addressParams[i].name + '>';
            }
            innerXml += '</val:row></val:rows>';
        } else {
            // No Address Data, return and display error Message
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'Address fields are empty.  There is nothing to validate.';
            return null;
        }

        innerXml = '<val:ValidateAddressRequest><val:context><val:account.id>' + _currentEnv.AddressValAccountId +
            '</val:account.id><val:account.password>' + _currentEnv.AddressValAccountPwd +
            '</val:account.password></val:context>' +
            innerXml + '</val:ValidateAddressRequest>';

        return innerXml;
    };
    window.validateAddress = validateAddress;
    // END validateAddress
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    // END Individual AddressWebService Methods
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    // END AddressWebService
    //=====================================================================================================
};