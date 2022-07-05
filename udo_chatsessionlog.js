function OnLoad(exCon) {
    var formContext = exCon.getFormContext();
    try {
        var regarding = formContext.getControl("regardingobjectid");
        var phonecall = formContext.getControl("crme_phonecallid");
        var interaction = formContext.getControl("udo_interactionid");
        var veteran = formContext.getControl("udo_veteranid");
        var udCreatedBy = formContext.getControl("udo_udcreatedby");
        var udCreatedByAttr = formContext.getAttribute("udo_udcreatedby");
        if (udCreatedByAttr.getValue()) {
            //from UD
            regarding.setVisible(true);
            phonecall.setVisible(true);
            interaction.setVisible(false);
            veteran.setVisible(false);
            //udCreatedBy.setVisible(true);
        }
        else {
            //UDO only
            regarding.setVisible(false);
            phonecall.setVisible(false);
            interaction.setVisible(true);
            veteran.setVisible(true);
            //udCreatedBy.setVisible(false);
        }

    }
    catch (e) {
        alert("Encountered an error: " + e);
    }
}