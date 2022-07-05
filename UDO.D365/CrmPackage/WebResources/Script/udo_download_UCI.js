"use strict";
var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Uci = Va.Udo.Crm.Uci || {};
Va.Udo.Crm.Uci.Scripts = Va.Udo.Crm.Uci.Scripts || {};

/// <summary>Downloader constructor function to download files using the client's browser</summary>
/// <param name="executionContext" type="Object">executionContext passed from the form</param>
/// <param name="paramObj" type="Object">parameter object containing values such as: webResourceName, fileName, contentType, b64Data</param>
/// <returns type="Object">Returns the Downloader object; example: var downloader = new Downloader(exCon, parObj);</returns>
Va.Udo.Crm.Uci.Scripts.Downloader = function (executionContext, paramsObj) {
	// Confirm required params have been provided
	if (arguments.length !== 2) {
		throw ("An error occurred in DownloaderUci constructor: Required number of parameters: 2");
	}

	// Private variables
	var _private = {
		// paramsObj variables
		webResourceName: paramsObj.webResourceName,
		fileName: paramsObj.fileName,
		contentType: paramsObj.contentType,
		b64Data: paramsObj.b64Data,

		// context variables
		globalContext: Xrm.Utility.getGlobalContext(),
		executionContext: executionContext,
		formContext: executionContext.getFormContext()
	}
	

	// Public methods

	/// <summary>Asynchronously download a file using the client's browser</summary>
	/// <returns type="undefined">does not return data; opens the file download dialog to download a file the client machine's file system</returns>
	this.DownloadAsync = function () {
		return new Promise(function (resolve, reject) {
			try {
				var contentWinPromise = _private.formContext.getControl(_private.webResourceName).getContentWindow();

				// Default mimeType to binary file
				if (_private.contentType === null || _private.contentType === "") {
					_private.contentType = "application/octet-stream";
				}

				// Create Blob from base64 fileContents
				var blob = _private.b64blob(_private.fileName, _private.contentType, _private.b64Data);

				/////
				// DOWNLOAD FILE
				/////

				contentWinPromise.then(function (contentWin) {

					// IE 10 and above - USD uses IE
					if (typeof contentWin.navigator !== "undefined") {
						if (typeof navigator.msSaveOrOpenBlob !== "undefined") {
							contentWin.navigator.msSaveOrOpenBlob(blob, _private.fileName);
							return resolve(["Downloaded Successfully", "IE10+"]);
						}
					}

					// Non IE
					if (typeof contentWin.URL !== "undefined" && typeof contentWin.URL.createObjectURL !== "undefined") {
						var blobURL = contentWin.URL.createObjectURL(blob);
						// Save using href link - newer browsers support this
						var linkEl = contentWin.document.createElementNS("http://www.w3.org/1999/xhtml", "a");
						if ("download" in linkEl) {
							linkEl.href = blobURL;
							linkEl.download = _private.fileName;
							linkEl.click();
							contentWin.URL.revokeObjectURL(blobURL);
							return resolve(["Downloaded Successfully", "Non-IE"]);
						}

						// Older browsers without href download
						contentWin.location.href = blobURL;
						setTimeout(function () {
							if (typeof contentWin.URL.revokeObjectUrl !== "undefined") contentWin.URL.revokeObjectUrl(blobURL);
						}, 10000);
						return resolve(["Downloaded Successfully", "Older Browser"]);
					}
				});
			} catch (err) {
				return reject(["Unable to Download File", err])
			}
		});
	}

	// Private methods
	_private.b64blob = function (fileName, contentType, b64Data) {
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
}