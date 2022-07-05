try {

    var attribute = Xrm.Page.getAttribute("udo_relationship");
    attribute.setValue(752280001);
    Xrm.Page.data.save();
}
catch (e) {
    window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
    Va.Udo.Crm.Scripts.Popup
                .MsgBox("Could not set the relationship to Dependent.\nTry updating the relationship Interaction field and saving directly.",
                    Va.Udo.Crm.Scripts.Popup.PopupStyles.Critical,
                    "Interaction Update Error",
                { height: 200, width: 350 });
}