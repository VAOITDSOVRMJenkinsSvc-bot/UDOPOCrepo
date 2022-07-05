/**
* @author Ivan Yurisevic
* @class VIP.services.WebServiceMessageHandler
*
* Analyzer and processor for service errors
*/
Ext.define('VIP.services.WebServiceMessageHandler', {
    config: {
        messages: null
    },
    requires: [
        'VIP.model.WebServiceMessage',
        'VIP.model.WebServiceFault',
        'VIP.model.WebServiceHtml'
        ],

    constructor: function () {
        var me = this;
        me.setMessages(new Array());
        me.faultCodes = [
            'S:SYS',      //Web Services Down
            'S:INV_STN',  //Invalid Station Number
            'S:INV_USR',  //Invalid User
            'S:INV_USER', //Invalid User
            'S:AUTH_APL', //AUTH failed
            'S:AUTH_EXP'  //Password has expired
        ];
        me.errorCodes = [
            'COV081',   //BIRLS is down
            'BPNQ9900', //BIRLS System Error - Contact Help Desk to fix error.
            'SENS10009',
            'SENS10008',
            'SENS10007',
            'SENS10006',
            'SENS10005',
            'SENS10004',
            'SENS10003',
            'SENS10002',
            'SENS10001',
            'SENS10000'  //Insufficient Sensitivity Level 0
        ];
        return me;
    },

    /* @retrieveWebServiceMessages -Ivan
    * The purpose of this function is to handle web service messages returned, whether they are error messages or not.
    * It will first remove namespaces from the XML using REGEX because the reader doesn't support them. Then it will 
    * read the MessageModel and the FaultModel so that we don't have to deal directly with XML. It returns a message
    * JSON object with a success flag set to false if the message code matches one of the messages defined in the constructor.
    * --EDIT-- It now also handles HTML responses.  Uses REGEX to turn HTML into XML and load it into DOM.  Reads HTML model.
    * @return message (JSON Object)
    * @returnProperties = success(boolean) returnCode(string) returnMessage(string)
    */
    retrieveWebServiceMessages: function (response, isLoaded) {
        var xml;

        if (!Ext.isEmpty(response.xml)) {
            xml = response.xml;
        }
        else if (!Ext.isEmpty(response.responseText)) {
            xml = response.responseText;
        }

        var me = this,
            message, 
            modx = xml.replace(/<[A-Z]*[a-z]*[0-9]*:/gm, "<"),
            mod = modx.replace(/<\/[A-Z]*[a-z]*[0-9]*:/gm, "</");
        if (isLoaded == undefined) {
            isLoaded = false;
        }

        if (!isLoaded) {
            response.loadXML(mod);
        }

        var webServiceMessageModel = Ext.create('VIP.model.WebServiceMessage'),
            messageResults = webServiceMessageModel.getProxy().getReader().read(response);

        var webServiceFaultModel = Ext.create('VIP.model.WebServiceFault'),
            faultResults = webServiceFaultModel.getProxy().getReader().read(response);

        mod = me.parseHTMLResponseToXML(mod);
        var xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
        xmlDoc.loadXML(mod);
        var webServiceHtmlModel = Ext.create('VIP.model.WebServiceHtml'),
            htmlResults = webServiceHtmlModel.getProxy().getReader().read(xmlDoc);
        xmlDoc = null;
        
        if (!Ext.isEmpty(messageResults) && messageResults.count) {
            var returnCode = messageResults.records[0].get('corpReturnCode') ?
                messageResults.records[0].get('corpReturnCode') : messageResults.records[0].get('birlsReturnCode');
            var returnMessage = messageResults.records[0].get('corpReturnMessage') ?
				messageResults.records[0].get('corpReturnMessage') : messageResults.records[0].get('birlsReturnMessage');
            message = {
                success: true,
                returnCode: returnCode ? returnCode : 'Unknown',
                returnMessage: returnMessage ? returnMessage : 'N/A'
            };

            //If return code is in the error code array, set flag. If indexOf not available, iterate through array
            if (me.errorCodes.indexOf == undefined) {
                for (var i = 0; i < me.errorCodes.length; i++) {
                    if (me.errorCodes[i] == returnCode) {
                        message.success = false;
                        break;
                    }
                }
            }
            else if (me.errorCodes.indexOf(returnCode) != -1) {
                message.success = false;
            }

            return message;
        }
        //If any of the nodes in the Fault model are found, success if false.
        else if (!Ext.isEmpty(faultResults) && faultResults.count) {
            message = {
                success: false,
                returnCode: faultResults.records[0].get('faultCode') ? faultResults.records[0].get('faultCode') : 'Fault',
                returnMessage: faultResults.records[0].get('faultMessage') ? faultResults.records[0].get('faultMessage') : faultResults.records[0].get('exceptionMessage')
            };
            return message;
        }
        //If any html responses found, success is false
        else if (!Ext.isEmpty(htmlResults) && htmlResults.count) {
            message = {
                success: false,
                returnCode: 'HTML Fault',
                returnMessage: htmlResults.records[0].get('title') + '-' + htmlResults.records[0].get('header2')
            };
            return message;
        }
        //Text only error responses
        else if (mod == "An error has occured (133).") {
            message = {
                success: false,
                returnCode: 'Error',
                returnMessage: mod
            };
        }

        //Default Message
        message = {
            success: true,
            returnCode: 'N/A',
            returnMessage: 'N/A'
        };
        return message;
    },

    parseHTMLResponseToXML: function (text) {
        var xmlText = '';

        //Remove these tags completely.
        xmlText = text.replace(/<HR>/gm, '').replace(/%/gm, '').replace(/<\/FONT>/gm, '').replace(/<FONT.*?>/gm, '');
        //Remove these tags completely - checking for upper and lower cases
        xmlText = xmlText.replace(/<[Pp]>/gm, '').replace(/<\/[Pp]>/gm, '');
        //Remove Meta and DocType tags. No closing tags here
        xmlText = xmlText.replace(/<META.*?>/gm, '').replace(/<BR.*?>/gm, '').replace(/<!DOCTYPE.*?>/gm, '');
        //Keep these tags, but remove extra options from them
        xmlText = xmlText.replace(/<BODY.*?>/gm, '<BODY>').replace(/<TABLE.*?>/gm, '<TABLE>').replace(/<TD.*?>/gm, '<TD>');

        return xmlText;
    }

});
