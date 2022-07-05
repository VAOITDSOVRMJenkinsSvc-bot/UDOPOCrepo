"use strict";

function setFocusFromUsd(context, tabName, displayTabFrameName) {
    try {
        var iframe = document.getElementById(displayTabFrameName);
        iframe.contentWindow.Va.Udo.Crm.Scripts.Code.activateTab(tabName, true);
    }
    catch (ex) {

    }
}
