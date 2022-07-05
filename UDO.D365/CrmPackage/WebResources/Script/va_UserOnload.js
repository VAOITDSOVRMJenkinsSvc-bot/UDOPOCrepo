
function Onload(exCon) {
    var formContext = exCon.getFormContext();
    var isCreateForm = formContext.ui.getFormType() === 1;
    var field = formContext.getAttribute("va_applicationname");
    if (isCreateForm) {
        field.setValue("CRMUD");
    }
} 