//Web Resource for Letters on VAI Entity
//get logged on user
function Getinfo() {
    var fullname;
    var context;
    var serverUrl;
    var UserID;
    var ODataPath;
    context = Xrm.Page.context;
    serverUrl = context.getServerUrl();
    UserID = context.getUserId();
    ODataPath = serverUrl + "XRMServices/2011/OrganizationData.svc";
    var retrieveUserReq = new XMLHttpRequest();
    retrieveUserReq.open("GET", ODataPath + "/SystemUserSet(guid'" + UserID + "')", false);
    retrieveUserReq.setRequestHeader("Accept", "application/json");
    retrieveUserReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    retrieveUserReq.onreadystatechange = function () {
        fullname = retrieveUserReqCallBack(this);
    };
    retrieveUserReq.send();
    return fullname;
}

function retrieveUserReqCallBack(retrieveUserReq) {
    if (retrieveUserReq.readyState == 4 /* complete */) {
        if (retrieveUserReq.status == 200) {
            var retrievedUser = (this.parent.JSON) ? this.parent.JSON.parse(retrieveUserReq.responseText).d : this.JSON.parse(retrieveUserReq.responseText).d;
            if (retrievedUser.FullName != null)
                return retrievedUser.FullName;

            else {
                alert("Error in Fetching User data");
            }
        }
    }
}



//get reports
var reports = {
    list: [],

    getReports: function () {
        var len, doRequest, me = this,
            url = Xrm.Page.context.getServerUrl() + 'XRMServices/2011/OrganizationData.svc/ReportSet?$select=ReportId,Name';

        doRequest = function (url) {
            $.ajax({
                type: 'GET',
                url: url,
                dataType: 'json'
            }).done(function (data) {
                //debugger
                if (data && data.d && data.d.results && data.d.results.length > 0) {
                    len = data.d.results.length;

                    for (var i = 0; i < len; i++) {
                        me.list.push({ name: data.d.results[i].Name, reportId: data.d.results[i].ReportId });
                    }

                    if (data.d.__next) {
                        doRequest(data.d.__next)
                    }
                }
            }).fail(function () {
                // console.log("fail")
            });
        };

        doRequest(url);
    }
};


reports.getReports();


function Letters(CommandProperties) {

    var menuXmlActionBlankLetter = '<Menu Id=\"ISV.DynamicMenu\">' +
         '<MenuSection Id=\"ISV.Dynamic.MenuSection\" Sequence=\"10\">' +
           '<Controls Id=\"ISV.Dynamic.Controls\">' +
                '<Button Id=\"ISV.Dynamic.VAIBlankLetter\" Command=\"ISV.SearchCommand\" Sequence=\"70\" LabelText=\"VAI Blank Letter\" Alt=\"VAI Blank Letter\" />' +

            '</Controls>' +
          '</MenuSection></Menu>';

    CommandProperties.PopulationXML = menuXmlActionBlankLetter;

}


function getReportID(reportName) {
    var len = reports.list.length;

    for (var i = 0; i < len; i++) {
        if (reports.list[i].name === reportName) {
            return reports.list[i].reportId;
        }
    }

    return '';
}

//Call Report based on seletion

function Search(CommandProperties) {
    var controlId = CommandProperties.SourceControlId;
    var GUIDvalue = Xrm.Page.data.entity.getId();
    var serverUrl = Xrm.Page.context.getServerUrl();
    var pcr = Getinfo();
    var now = new Date();

    switch (controlId) {

        case 'ISV.Dynamic.VAIBlankLetter':
            window.open(serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=report.rdl&id=%7b" + getReportID('VAI Blank Letter') + "%7d&p:VAIGUID=" + GUIDvalue);
            break;
        default:
            alert('Button Unknown');
    }
}