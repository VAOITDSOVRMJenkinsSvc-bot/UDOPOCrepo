Ext.define('VIP.util.xml.Parser', {
    config: {
        xmlText: ''
    },
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        return me;
    },
    parse: function () {
        var me = this,
            xmlText = me.getXmlText(),
            xmlDoc,
            mod = xmlText.replace(/<[A-Z]*[a-z]*[0-9]*:/gm, "<"), //this removes namespaces
            mod = mod.replace(/<\/[A-Z]*[a-z]*[0-9]*:/gm, "</"); //this removes namespaces

        if (Ext.isIE) {
            xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
            xmlDoc.async = false;
            xmlDoc.loadXML(mod);
        }
        else {
            var parser = new DOMParser();
            xmlDoc = parser.parseFromString(mod, "text/xml");
        }

        return xmlDoc;
    },
    parseAsync: function (callback) {
        //debugger;
        var me = this,
            xmlText = me.getXmlText(),
            xmlDoc,
            mod = xmlText.replace(/<[A-Z]*[a-z]*[0-9]*:/gm, "<").replace(/<\/[A-Z]*[a-z]*[0-9]*:/gm, "</"); //this removes namespaces

        if (Ext.isIE) {
            xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
            xmlDoc.async = true;
            xmlDoc.onreadystatechange = function () {
                if (xmlDoc.readyState == 4) {
                    me.xmlDoc = xmlDoc;
                    if (callback) {
                        callback(xmlDoc);
                    }
                }
            };
            xmlDoc.loadXML(mod);
            if (Ext.isEmpty(me.xmlDoc)) {
                var id = setTimeout(me.waitForDoc, 2);
            }
        }
        else {
            var parser = new DOMParser();
            xmlDoc = parser.parseFromString(mod, "text/xml");
        }

        return xmlDoc;
    },
    waitForDoc: function () {
        var reps = 60000, n = 0;
        while (Ext.isEmpty(me.xmlDoc) && n < reps) {
            var id = setTimeout(me.noFunc, 10);
            n++;
        }
    },
    noFunc: function () { }
});