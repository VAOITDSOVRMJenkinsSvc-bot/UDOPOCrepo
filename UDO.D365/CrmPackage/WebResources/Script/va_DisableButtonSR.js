
function disableServiceRequest() {
    var cancelledStatus = 953850005;

    Xrm.Page.getAttribute('va_requeststatus').setValue(cancelledStatus);

    Xrm.Page.data.entity.save('saveandclose');
}
