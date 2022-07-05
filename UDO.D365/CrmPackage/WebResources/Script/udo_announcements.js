var focusAnnouncement;

window["ENTITY_SET_NAMES"] = window["ENTITY_SET_NAMES"] || JSON.stringify({
    "udo_announcement": "udo_announcements"
});

function RetrieveAnnouncements() {
    var ExpirationDate = new Date();
    var docMode = document.documentMode;
    if (docMode < 9) {
        alert("The web browser version is incompatible and cannot query for Announcements.");
        console.log("Document mode is: " + document.documentMode);
        return;
    }
    ExpirationDate = ExpirationDate.toISOString();
    //formulate OData query sorted by Priority for all those with expiration dates today or in the future, or with null expiration dates
    var filter = "?$select=udo_body,udo_expirationdate,udo_name,udo_priority,statecode&$filter=statecode eq 0 and udo_expirationdate ge (" + ExpirationDate + ") or statecode eq 0 and udo_expirationdate eq null&$orderby=udo_priority asc";
    ExecuteQuery(filter);
}

function ExecuteQuery(filter) {    
    Xrm.WebApi.retrieveMultipleRecords("udo_announcement",filter)
        .then(
            function success(response) {
                var tableBody = document.getElementById("announcementsTableBody");
                if (response === null || response.value === null || response.value.length === 0) {
                    var row = tableBody.insertRow(0);
                    var cell1 = row.insertCell(0);
                    cell1.setAttribute("SCOPE", "row");
                    var cell2 = row.insertCell(1);
                    cell2.setAttribute("SCOPE", "row");

                    var announcementName = "There are currently no Announcements.";
                    var inputAnnouncement = "inputAnnouncement_0";
                    focusAnnouncement = inputAnnouncement;

                    cell1.innerHTML = "<img alt=\"Announcement Icon\" src=\"udo_/Images/Announcement.png\" />";
                    cell2.innerHTML =
                        "<div class=\"header\"><input type=\"input\" name=\"" +
                        inputAnnouncement +
                        "\" style=\"font-size:14px;font-weight: bold;border: none;\" readonly=\"readonly\" size=\"" +
                        announcementName.length +
                        "\" value=\"" +
                        announcementName +
                        "\" aria-label=\"" +
                        announcementName +
                        "\" /></div>";
                } else {
                    var announcementRowCnt = 0;
                    var data = response.value;
                    for (var i = data.length - 1; i >= 0; i--) {
                        if (data[i]["udo_body"] == null) {
                            data[i]["udo_body"] = "";
                        }

                        if (data[i]["udo_body"] != null) {
                            var row = tableBody.insertRow(0);
                            var cell1 = row.insertCell(0);
                            cell1.setAttribute("SCOPE", "row");
                            var cell2 = row.insertCell(1);
                            cell2.setAttribute("SCOPE", "row");
                            var announcementName = data[i]["udo_name"];
                            var announcementBody = data[i]["udo_body"];
                            //could be multiline with LF characters, or could be generic HTML
                            if (announcementBody.toLowerCase().indexOf("</html>") === -1)
                                announcementBody = announcementBody.replace(/(\r\n|\n|\r)/gm, "<br>");

                            var inputAnnouncement = "inputAnnouncement_" + i.toString();

                            if (i === 0) {
                                focusAnnouncement = inputAnnouncement;
                            }

                            cell1.innerHTML = "<img alt=\"Announcement Icon\" src=\"udo_/Images/Announcement.png\" />";
                            cell2.innerHTML =
                                "<div class=\"header\"><input type=\"input\" name=\"" +
                                inputAnnouncement +
                                "\" style=\"font-size:14px;font-weight: bold;border: none;\" readonly=\"readonly\" size=\"" +
                                announcementName.length +
                                "\" value=\"" +
                                announcementName +
                                "\" aria-label=\"" +
                                "Announcement - " +
                                announcementName +
                                announcementBody +
                                "\" /></div>" +
                                "<div class=\"body\">" +
                                announcementBody +
                                "</div>";

                            announcementRowCnt++;
                        } else {
                            console.log("Announcements record has no body, please add a body to add announcements records.");
                        }
                    }
                }
                getFocus();
            }, function (error) { console.log(error); });
}

function getFocus() {
    (document.forms['announcementForm'].elements[focusAnnouncement]).focus();
}

//make OData call onload
window.onload = RetrieveAnnouncements;
