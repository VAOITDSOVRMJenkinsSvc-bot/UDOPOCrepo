var xmlUtils = (function () {

    function parseXmlObject(xmlString) {
        if (!xmlString) {
            alert('XML Object contains a null value');
            return null;
        }
        var xmlObject = new ActiveXObject("Microsoft.XMLDOM");
        xmlObject.async = false;
        xmlObject.loadXML(xmlString);

        return xmlObject;
    }

    function parseXmlObjectFromFile(path) {
        var xmlObject = new ActiveXObject("Microsoft.XMLDOM");
        xmlObject.async = false;
        xmlObject.load(path);

        return xmlObject;
    }

    return {
        'parseXmlObject': parseXmlObject,
        'parseXmlObjectFromFile': parseXmlObjectFromFile,
    };

})();
