Ext.define('VIP.util.xml.FragmentBuilder', {
    requires: [
        'VIP.util.xml.Serializer',
        'VIP.util.xml.Parser'
    ],

    config: {
        xmlFragment: undefined,
        rootNodeName: ''
    },

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        return me;
    },

    toString: function () {
        var me = this,
            xmlFragment = me.getXmlFragment(),
            rootNodeName = me.getRootNodeName(),
            xmlSerializer = Ext.create('VIP.util.xml.Serializer'),
            xmlString = '';

        if (!Ext.isEmpty(xmlFragment) && !Ext.isEmpty(rootNodeName)) {
            xmlString = xmlSerializer.serialize(xmlFragment, rootNodeName);
        }

        return xmlString;
    },

    parseXml: function () {
        var me = this,
            xmlParser = Ext.create('VIP.util.xml.Parser', {
                xmlText: me.toString()
            }),
            xmlObject = xmlParser.parse();

        return xmlObject;
    }

});