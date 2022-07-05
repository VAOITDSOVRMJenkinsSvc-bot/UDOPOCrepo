var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.Request = Va.Udo.Crm.Scripts.Request || {};
Va.Udo.Crm.Scripts.Request.Attributes = Va.Udo.Crm.Scripts.Request.Attributes || {};
var exCon = null;
var formContext = null;
//On load event of request
Va.Udo.Crm.Scripts.Request.onLoad = function (execCon) {
    exCon = execCon;
    formContext = exCon.getFormContext();
    Va.Udo.Crm.Scripts.Request.refreshRequestSubGrid();
    Va.Udo.Crm.Scripts.Request.toggleCreatedByOnload();
}

//Initiate the refresh of the sub grid
Va.Udo.Crm.Scripts.Request.refreshRequestSubGrid = function () {
    var requestType = Xrm.Page.getAttribute("udo_type");
    var requestSubType = Xrm.Page.getAttribute("udo_subtype");
    var veteran = Xrm.Page.getAttribute("udo_veteran");
    if (requestType != null && requestSubType != null && veteran != null) {
        var requestTypeEntityReference = null;
        var requestSubTypeEntityReference = null;
        var veteranEntityReference = null;
        if (requestType.getValue() != null)
            requestTypeEntityReference = requestType.getValue()[0];
        if (requestSubType.getValue() != null)
            requestSubTypeEntityReference = requestSubType.getValue()[0];
        if (veteran.getValue() != null)
            veteranEntityReference = veteran.getValue()[0];
        var currentRecordId = Xrm.Page.data.entity.getId();
        if (requestTypeEntityReference != null && requestSubTypeEntityReference != null && veteranEntityReference != null && currentRecordId != null)
            Va.Udo.Crm.Scripts.Request.updateRequestSubgrid(requestTypeEntityReference, requestSubTypeEntityReference, veteranEntityReference, currentRecordId);
    }
}
//Recursive function to refresh the subgrid.
Va.Udo.Crm.Scripts.Request.updateRequestSubgrid = function (requestTypeEntityReference, requestSubTypeEntityReference, veteranEntityReference, currentRecordId) {
    debugger;
    if (requestTypeEntityReference != null && requestSubTypeEntityReference != null && veteranEntityReference != null && currentRecordId != null) {
        var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>"
      + "<entity name='udo_request'><attribute name='udo_requestid' /><attribute name='createdon' /><attribute name='udo_regarding' /><attribute name='udo_isrepeatcall' /><attribute name='udo_duration' />"
      + "<order attribute='createdon' descending='false' />"
        + "<filter type='and'>"
          + "<condition attribute='udo_type' operator='eq' uitype='udo_requesttype' value='" + requestTypeEntityReference.id + "' />"
          + "<condition attribute='udo_subtype' operator='eq' uitype='udo_requestsubtype' value='" + requestSubTypeEntityReference.id + "' />"
          + "<condition attribute='udo_veteran' operator='eq' uitype='contact' value='" + veteranEntityReference.id + "' />"
             + "<condition attribute='udo_requestid' operator='ne' uitype='udo_request' value='" + currentRecordId + "' />"
        + "</filter></entity></fetch>"

        var subGrid = document.getElementById("relatedRequests");
        if (subGrid != null) {
            var subGridControl = subGrid.control;
            if (subGridControl != null) {
                subGridControl.SetParameter("fetchXml", fetchXml);
                var xrmGrid = Xrm.Page.getControl("relatedRequests");
                if (xrmGrid != null) {
                    xrmGrid.refresh();
                }
            }
        }
        else {
            ///Wait for the control to load
            setTimeout(Va.Udo.Crm.Scripts.Request.updateRequestSubgrid, 1000, requestTypeEntityReference, requestSubTypeEntityReference, veteranEntityReference, currentRecordId);
        }
    }
}


Va.Udo.Crm.Scripts.Request.toggleCreatedByOnload = function() {
    try {
        var createdBy = Xrm.Page.getControl("createdby");
        var udCreatedBy = Xrm.Page.getControl("udo_udcreatedby");
        var udCreatedByAttr = Xrm.Page.getAttribute("udo_udcreatedby");
        if (udCreatedByAttr.getValue()) {
            createdBy.setVisible(false);
            udCreatedBy.setVisible(true);
        }
        else {
            createdBy.setVisible(true);
            udCreatedBy.setVisible(false);
        }

    }
    catch (e) {
        alert("Encountered an error: " + e);
    }
}
