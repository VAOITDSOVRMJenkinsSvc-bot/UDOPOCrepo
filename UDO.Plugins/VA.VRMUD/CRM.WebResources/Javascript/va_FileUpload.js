/**
 * Created by VHAISDBLANCW on 7/19/2014.
 */
window.FileUpload = {};
window.FileUpload.url = '';

function OpenFileUploadDialog() {
    var recordId = Xrm.Page.data.entity.getId();
    var etc = Xrm.Page.context.getQueryStringParameters().etc;
    window.FileUpload.url = Xrm.Page.context.getServerUrl() + '/Notes/edit.aspx?hideDesc=1&pId=' + recordId + '&pType=' + etc;
    CreateFileUploadObject(etc, recordId);
}

function CreateFileUploadObject(etc, recordId) {
    var fileUpload = new Object();
    fileUpload.va_EntityName = Xrm.Page.data.entity.getEntityName();
    fileUpload.va_RecordId = recordId;
    fileUpload.va_name = 'VBMS Upload';
    CreateFileUploadRecord(fileUpload, 'va_fileuploadSet', CreateFileUploadRecordCompleted, null);
}

// This callback method executes on succesful account creation

function CreateFileUploadRecordCompleted(data, textStatus, XmlHttpRequest) {
    var fileUpload = data;
    //alert('FileUpload record created' + fileUpload.va_RecordId.toString());
    //window.open(window.FileUpload.url, "", "width=500, height=180, toolbar=no");
    ShowFilePopupWindow(500, 180);
}

function ShowFilePopupWindow(w, h) {
    var left = (screen.width / 2) - (w / 2);
    var top = (screen.height / 2) - (h / 2);
    window.open(window.FileUpload.url, "", "width=" + w + ", height=" + h + ", top=" + top + ", left=" + left + ", toolbar=no");
}

function CreateFileUploadRecord(entityObject, odataSetName, successCallback, errorCallback) {
    //Parse the entity object into JSON
    var jsonEntity = window.JSON.stringify(entityObject);
    // Get Server URL
    var serverUrl = Xrm.Page.context.getServerUrl();
    //The OData end-point
    var ODATA_ENDPOINT = '/XRMServices/2011/OrganizationData.svc';
    //Asynchronous AJAX function to Create a CRM record using OData
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        url: serverUrl + ODATA_ENDPOINT + '/' + odataSetName,
        data: jsonEntity,
        beforeSend: function (XMLHttpRequest) {
            //Specifying this header ensures that the results will be returned as JSON.
            XMLHttpRequest.setRequestHeader('Accept', 'application/json');
        },
        success: function (data, textStatus, XmlHttpRequest) {
            if (successCallback) {
                successCallback(data.d, textStatus, XmlHttpRequest);
            }
        },
        error: function (XmlHttpRequest, textStatus, errorThrown) {
            if (errorCallback)
                errorCallback(XmlHttpRequest, textStatus, errorThrown);
            else
                alert('Error on the creation of record; Error – ' + errorThrown);
        }
    });
}
