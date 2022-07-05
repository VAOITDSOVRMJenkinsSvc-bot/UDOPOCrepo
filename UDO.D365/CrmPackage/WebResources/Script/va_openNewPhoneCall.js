function createPhoneCall() {
    Xrm.Utility.openEntityForm("phonecall");
}

function SaveCompleteClose() {
    var crmFormCtrl = $find("crmForm");
    if (!crmFormCtrl.IsValid())
        return;
    crmFormCtrl.SubmitCrmForm(58, true, true, true)
}