"use strict";
/************** udo_download.js ******************************************/
var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};

// download: download a base64 file
//
// fileName: the default name of the file
// mimeType: the mime type of the file
// fileContents: base64 string containing the base64 file contents (without the base64; prefix)
Va.Udo.Crm.Scripts.Download = function (fileName, mimeType, fileContents) {
    function b64blob(fileName, contentType, b64Data) {
        // Convert to Char Array
        var byteChars = atob(b64Data);

        // Convert to Number Array
        var byteNums = new Array(byteChars.length);
        for (var i = 0; i < byteChars.length; i++) {
            byteNums[i] = byteChars.charCodeAt(i);
        }

        // Convert to byteArray
        var byteArray = new Uint8Array(byteNums);
        var blob = new Blob([byteArray], { type: contentType, name: fileName || "download" });
        return blob;
    }

    var dfd = $.Deferred();

    try {
        // Default mimeType to binary file
        if (mimeType === null || mimeType === "") mimeType = "application/octet-stream";

        // Create Blob from base64 fileContents
        var blob = b64blob(fileName, mimeType, fileContents);

        // DOWNLOAD FILE
        // IE 10 and above - USD uses IE
        if (typeof navigator !== "undefined") {
            if (typeof navigator.msSaveOrOpenBlob !== "undefined") {
                navigator.msSaveOrOpenBlob(blob, fileName);
                dfd.resolveWith(this, ["Downloaded Successfully", "IE10+"]);
                return dfd.promise();
            }
        }


        // Firefox & Chrome
        if (typeof URL !== "undefined" && typeof URL.createObjectURL !== "undefined") {
            var blobURL = URL.createObjectURL(blob);
            // Save using href link - newer browsers support this
            var linkEl = document.createElementNS("http://www.w3.org/1999/xhtml", "a");
            if ("download" in linkEl) {
                linkEl.href = blobURL;
                linkEl.download = fileName;
                linkEl.click();
                URL.revokeObjectUrl(blobURL);
                dfd.resolveWith(this, ["Downloaded Successfully", "Firefox/Chrome"]);
                return dfd.promise();
            }

            // Older browsers without href download
            window.location.href = blobURL;
            setTimeout(function () {
                if (typeof URL.revokeObjectUrl !== "undefined") URL.revokeObjectUrl(blobURL);
            }, 10000);
            dfd.resolveWith(this, ["Downloaded Successfully", "Older Browser"]);
            return dfd.promise();
        }
    } catch (err) {
        dfd.rejectWith(this, [err.message]);
        return dfd.promise();
    }
    // File was not able to be downloaded
    dfd.rejectWith(this, ["Unable to download file."]);
    return dfd.promise();
}
