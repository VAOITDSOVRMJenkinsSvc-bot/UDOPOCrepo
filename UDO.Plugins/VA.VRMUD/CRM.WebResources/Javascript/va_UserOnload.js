function Onload() {
    var isCreateForm = Xrm.Page.ui.getFormType() == 1;
    var field = Xrm.Page.getAttribute("va_applicationname");
    if (isCreateForm) {
        field.setValue("CRMUD");
    }
} 