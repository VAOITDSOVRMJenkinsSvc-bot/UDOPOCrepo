// runReportToPrint_v3.js
// Description: 
//      Reports are run from udo_lettergeneration or udo_servicerequest
//      Which Report to run is selected by user from specific field on the entity
//      User will select a button to indicate WORD or PDF
// End result:
//      User is prompted to save the document
function printReport() {
    try {
        var reportid = null;
        // TODO: Determine FORMAT 
        //      Create parameter and logic to handle if printing to WORD or PDF 
        //      Replace "pdf" with input parameter from button OnSelect passing a parameter to indicate if WORD or PDF button was selected
        var buttonSelected = "pdf";
        if buttonSelected == "pdf" || buttonSelected == null {
            var formatSelected = "PDF" // or WORDOPENXML
        } else {
            var formatSelected = "WORDOPENXML";
        }

        var req = new XMLHttpRequest();

        // TODO: Determine REPORT NAME 
        //      Convert reportName to using logic based on entity from which invoked
        var invokeRecord = Xrm.Page.context.getEntityName(); // psuedo code here, replace with real call for context to get current entity name
        

        // TODO: SET Report PARAMETER
        //      Depending on above entity invoking the report
        //      There is custom field on entity where user has selected the Name of the report
        //      Lookup report name from field on entity
        var reportName = "Account Name PoC";


        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.2/reports?$select=reportid&$filter=filename eq '" + reportName + ".rdl'", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var results = JSON.parse(this.response);
                    if (results != null) {
                        reportid = results.value[0]["reportid"];
                        var params = getReportingSession(reportName, reportid);
                        // TODO: Replace end of URL with variable to indicate format
                        var newPth = Xrm.Page.context.getClientUrl() + "/Reserved.ReportViewerWebControl.axd?ReportSession=" + params[0] + "&Culture=1033&CultureOverrides=True&UICulture=1033&UICultureOverrides=True&ReportStack=1&ControlID=" + params[1] + "&OpType=Export&FileName=" + reportName + "&ContentDisposition=OnlyHtmlInline&Format=PDF";

                        window.open(newPth, "_self");
                    } else {
                        Xrm.Utility.alertDialog(this.statusText);
                    }
                }
            }
        };
        req.send();
    } catch (ex) {
        throw ex;
    }
}


function getReportingSession(reportName, reportGuid) {

    try {

        // TODO: Determine recordId 
        //      Reports are created from an entity in USD, that entity has required fields that are part of the report being built.
        var recordId = Xrm.Page.data.entity.getId();;
        // TODO: Create correct fetch
        var strParameterXML = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'><entity name='account'><all-attributes /><filter type='and'><condition attribute='accountid' operator='eq' value='" + recordId + "' /> </filter></entity></fetch>";

        var pth = Xrm.Page.context.getClientUrl() + "/CRMReports/rsviewer/ReportViewer.aspx";
        var retrieveEntityReq = new XMLHttpRequest();
        retrieveEntityReq.open("POST", pth, false);
        retrieveEntityReq.setRequestHeader("Accept", "*/*");
        retrieveEntityReq.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        retrieveEntityReq.send("id=%7B" + reportGuid + "%7D&uniquename=" + Xrm.Page.context.getOrgUniqueName() + "&iscustomreport=true&reportnameonsrs=&reportName=" + reportName + "&isScheduledReport=false&p:CRM_FilteredAccount=" + strParameterXML);

        var x = retrieveEntityReq.responseText.lastIndexOf("ReportSession=");

        var y = retrieveEntityReq.responseText.lastIndexOf("ControlID=");

        var ret = new Array();

        ret[0] = retrieveEntityReq.responseText.substr(x + 14, 24);

        ret[1] = retrieveEntityReq.responseText.substr(x + 10, 32);

        return ret;
    }
    catch (ex) {
        throw ex;
    }
}

function getPDFBase64String(requestPath) {
    var retrieveEntityReq = new XMLHttpRequest();
    retrieveEntityReq.open("GET", requestPath, true);
    retrieveEntityReq.setRequestHeader("Accept", "*/*");
    retrieveEntityReq.responseType = "arraybuffer";
    retrieveEntityReq.onreadystatechange = function () { // This is the callback function.
        if (retrieveEntityReq.readyState == 4 && retrieveEntityReq.status == 200) {
            var binary = "";
            var bytes = new Uint8Array(this.response);
            for (var i = 0; i < bytes.byteLength; i++) {
                binary += String.fromCharCode(bytes[i]);
            }
            //This is the base 64 PDF formatted string.
            var base64PDFString = btoa(binary);
            return base64PDFString;
        }
    };
}


// sendEmailWithAttachment 
// params:
//  reportName: Name of report
//  attachmentType: "PDF" or "WORD"
//  recipientAddress: valid email address. Seperate with ; for multiple
//  emailSubject, emailBody
//  reportParams: Array of KeyValuePair <string, object> => string=parameterName, object=paramValue
//      {[Variance:1], [ServiceRequestGUID:<Guid>]}
function sendEmailWithAttachment(reportName,  attachmentType, recipientAddress, emailSubject, emailBody, reportParams) {
    try {
        if (attachmentType.toUpper() == "pdf" || attachmentType == null) {
            var formatSelected = "PDF" // or WORDOPENXML
        } else {
            var formatSelected = "WORDOPENXML";
        }

        //TODO add reportParams to this call. We will need to dynamically add these params into the report path
        //var paramString = "";
        //for (var i = 0; i < reportParams.length; i++) {
        //    paramString = paramString + "&" + reportParams[i.parameterName] + "=" + reportParams[i.parameterValue];
        //}
        //var reportPath = getPathToReport(reportName, formatSelected, paramString);
        var reportPath = getPathToReport(reportParams, formatSelected);
        var attachment = getPDFBase64String(reportPath); 

        /* Will this work??
        var axApp = new ActiveXObject("Outlook.Application");
        var objNS = axApp.GetNameSpace('MAPI');
        var mailItem = axApp.CreateItem(0); // value 0 = MailItem
        mailItem.to = (recipientAddress);
        mailItem.Subject = (emailSubject);
        mailItem.Body = (emailBody);
        mailItem.Attachments.Add(attachment);
        mailItem.display();
        */
    }
    catch (err) {
        alert(err.message);
    }
}

function getPathToReport(reportName, reportFormat) {
    try {
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.2/reports?$select=reportid&$filter=filename eq '" + reportName + ".rdl'", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var results = JSON.parse(this.response);
                    if (results != null) {
                        reportid = results.value[0]["reportid"];
                        var params = getReportingSession(reportName, reportid);                        
                        var newPth = Xrm.Page.context.getClientUrl() + "/Reserved.ReportViewerWebControl.axd?ReportSession=" + params[0] + "&Culture=1033&CultureOverrides=True&UICulture=1033&UICultureOverrides=True&ReportStack=1&ControlID=" + params[1] + "&OpType=Export&FileName=" + reportName + "&ContentDisposition=OnlyHtmlInline&Format=" + reportFormat;

                        return newPth;
                    } else {
                        Xrm.Utility.alertDialog(this.statusText);
                    }
                }
            }
        };
        req.send();
    } catch (e) {
        throw e;
    }
}
