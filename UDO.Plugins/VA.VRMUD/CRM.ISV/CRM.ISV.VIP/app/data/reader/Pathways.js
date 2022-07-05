Ext.define('VIP.data.reader.Pathways', {
    extend: 'Ext.data.reader.Xml',
    alias: 'reader.pathways',
    requires: [
        'VIP.util.xml.Parser'
    ],
    read: function (response) {
        var me = this,
            xmlParser = Ext.create('VIP.util.xml.Parser', {
                xmlText: response.text
            }),
            extractedResponse = xmlParser.parse();
        
        if (Ext.isEmpty(extractedResponse)) { return me.callParent([response]); }

        return me.callParent([extractedResponse]);
    }
});