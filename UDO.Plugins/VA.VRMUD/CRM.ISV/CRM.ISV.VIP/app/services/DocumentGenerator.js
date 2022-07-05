/**
* @author Josh Oliver
* @class VIP.services.DocumentGenerator
*
* Document operations
*/
Ext.define('VIP.services.DocumentGenerator', {
    config: {
        content: undefined,
        decodedContent: undefined,
        title: undefined,
        mimeType: undefined,
        fileExtension: undefined,
        downloadTargetUrl: undefined,
        downloadTargetPage: 'vip.web/download.aspx',
        requestType: 'vva'
    },

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.setInitialDownloadTargetUrl();

        return me;
    },

    decompressContent: function () {
        var me = this,
            content = JXG.decompress(me.getContent());

        if (!Ext.isEmpty(content)) {
            me.setDecodedContent(content);
        }

        return content;
    },

    decodeContent: function () {
        var me = this,
            content = base64.decode(me.getContent());

        if (!Ext.isEmpty(content)) {
            me.setDecodedContent(content);
        }

        return content;
    },

    openDocument: function () {
        var me = this,
            pageParameters = {
                letter: me.getContent(),
                title: me.getTitle(),
                mimeType: me.getMimeType(),
                fileExtension: me.getFileExtension(),
                requestType: me.getRequestType()
            };

        post(me.getDownloadTargetFullUrl(), pageParameters);

        function post(downloadTarget, pageParameters) {
            var downloadWindow = window,
                myForm = downloadWindow.document.createElement("form");

            myForm.method = 'post';
            myForm.action = downloadTarget;
            myForm.target = '_blank';
            for (var k in pageParameters) {
                var myInput = downloadWindow.document.createElement("input");
                myInput.setAttribute("name", k);
                myInput.setAttribute("value", pageParameters[k]);
                myForm.appendChild(myInput);
            }
            downloadWindow.document.body.appendChild(myForm);
            myForm.submit();
            downloadWindow.document.body.removeChild(myForm);
        }
    },

    getDownloadTargetFullUrl: function () {
        var me = this;
        return me.getDownloadTargetUrl() + me.getDownloadTargetPage();
    },

    setInitialDownloadTargetUrl: function () {
        var me = this;

        if (parent && parent.Xrm) {
            me.setDownloadTargetUrl(parent.Xrm.Page.context.getServerUrl().replace(parent.Xrm.Page.context.getOrgUniqueName(), '') + 'isv/');
        }
        else {
            me.setDownloadTargetUrl('http://localhost/va.vrm/');
        }

    }
});