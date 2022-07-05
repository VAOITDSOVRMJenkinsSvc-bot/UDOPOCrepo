var ws = ws || {};
ws.intentToFile = {};
// Function will be called by form onLoad event
ws.intentToFile.initalize = function () {
    var AddressWebServiceBeanITF = function (context) {
        this.context = context;
        this.webserviceRequestUrl = WebServiceURLRoot + 'AddressWebServiceBean/AddressWebService';
        this.prefix = 'add';
        this.prefixUrl = 'http://address.services.vetsnet.vba.va.gov/';
    };
    AddressWebServiceBeanITF.prototype = new webservice;
    AddressWebServiceBeanITF.prototype.constructor = AddressWebServiceBeanITF;
    window.AddressWebServiceBeanITF = AddressWebServiceBeanITF;

    var addressValidationITF = function (context) {
        this.context = context;

        this.serviceName = 'AddressWebService';
        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'AddressWebServiceBean.AddressWebService';
        this.wsMessage.serviceName = 'AddressWebService';
        this.wsMessage.friendlyServiceName = 'Address Validation Web Service';
        //this.responseFieldSchema = 'va_updateaddressresponse';
        //this.responseTimestamp = 'va_webserviceresponse';
    };
    addressValidationITF.prototype = new AddressWebServiceBeanITF;
    addressValidationITF.prototype.constructor = addressValidationITF;
    addressValidationITF.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml = '<add:validateAddress>';

        innerXml +=
                '<addressLine1>' + NN(Xrm.Page.getAttribute('va_veteranaddressline1').getValue()) + '</addressLine1>' +
                '<addressLine2>' + NN(Xrm.Page.getAttribute('va_veteranaddressline2').getValue()) + '</addressLine2>' +
                '<addressLine3>' + NN(Xrm.Page.getAttribute('va_veteranunitnumber').getValue()) + '</addressLine3>' +
                '<city>' + NN(Xrm.Page.getAttribute('va_veterancity').getValue()) + '</city>' +
                '<state>' + NN(Xrm.Page.getAttribute('va_veteranstate').getValue()) + '</state>' +
                '<postalCode>' + NN(Xrm.Page.getAttribute('va_veteranzip').getValue()) + '</postalCode>' +
                '<country>' + NN(Xrm.Page.getAttribute('va_veterancountry').getValue()) + '</country>';

        innerXml += '</add:validateAddress>';

        return innerXml;
    };
    window.addressValidationITF = addressValidationITF;
};

function NN(s) { return (s == null ? '' : s); }