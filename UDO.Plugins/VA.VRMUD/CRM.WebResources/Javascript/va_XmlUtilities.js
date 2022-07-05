var XmlUtilities = function () {
    this.concatenateDocs = function (xmlDoms, newParentNodeName) {
        var concatenatedXmlDom = new ActiveXObject("MSXML2.DOMDocument");
        concatenatedXmlDom.async = false;
        concatenatedXmlDom.loadXML('<' + newParentNodeName + '></' + newParentNodeName + '>');

        for (var idx in xmlDoms) {
            var elementRoot = xmlDoms[idx].documentElement;
            if (elementRoot) concatenatedXmlDom.documentElement.appendChild(elementRoot);
        }

        return concatenatedXmlDom;
    };

    this.parseXmlObject = function (xmlString) {
        if (xmlString == null) {
            alert('XML Object contains a null value');
            return null;
        }
        var xmlObject = new ActiveXObject("Microsoft.XMLDOM");
        xmlObject.async = false;
        xmlObject.loadXML(xmlString);

        return xmlObject;
    };
}