Ext.define('VIP.util.xml.Response', {

    prepareResponse: function (response) {
        var me = this,
            xml;

        if (response && response.responseXML) {
            if (typeof response.responseXML.text === 'undefined' && typeof response.responseXML.xml === 'undefined') {
                xml = (response.responseXML.childNodes && response.responseXML.childNodes.length > 0) ?
                response.responseXML.childNodes[0].textContent :
                response.responseText;

                response.responseXML['text'] = xml;
                response.responseXML['xml'] = response.responseText;
            }
        }
    }

});