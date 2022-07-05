window.ReportExportType = "PDF";

function SetExportType(t) {
    window.ReportExportType = t;
}
debugger;
var context = Xrm.Utility.getGlobalContext();
var version = context.getVersion();
var webApi = new CrmCommonJS.WebApi(version);
var formContext = null;
//get logged on user
function Getinfo() {
    var fullname;   
    //var serverUrl;
    var UserID;
    var ODataPath;
    //TODO: convert to form context
    //context = Xrm.Page.context;
    //serverUrl = context.getClientUrl();
    UserID = context.userSettings.userId;
   /* if (serverUrl[serverUrl.length - 1] != "/") { serverUrl = serverUrl + "/"; }
    ODataPath = serverUrl + "XRMServices/2011/OrganizationData.svc";
    var retrieveUserReq = new XMLHttpRequest();
    retrieveUserReq.open("GET", ODataPath + "/SystemUserSet(guid'" + UserID + "')", false);
    retrieveUserReq.setRequestHeader("Accept", "application/json");
    retrieveUserReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    retrieveUserReq.onreadystatechange = function () {
        fullname = retrieveUserReqCallBack(this);
    };*/
    return new Promise(function (resolve, reject) {
        webApi.RetrieveRecord(UserID, "systemuser", ["fullname"], null)
            .then(
                function (data) {
                    if (data != null) {
                        fullname = data.fullname;
                        return resolve(fullname);
                    }
                    else {
                        return reject( new Error("Error in Fetching User data"));
                    }
                },
                function (error) {
                return reject(error);
            });
        //TODO: convert to WebAPI
        //retrieveUserReq.send();
    });   
}

/*function retrieveUserReqCallBack(retrieveUserReq) {
    if (retrieveUserReq.readyState == 4 /* complete ) {
        if (retrieveUserReq.status == 200) {
            var retrievedUser = (this.parent.JSON) ? this.parent.JSON.parse(retrieveUserReq.responseText).d : this.JSON.parse(retrieveUserReq.responseText).d;
            if (retrievedUser.FullName != null)
                return retrievedUser.FullName;

            else {
                alert("Error in Fetching User data");
            }
        }
    }
}*/



//get reports
var reports = {
    list: [],

    getReports: function () {
        var len;        
        var me = this;
            //TODO: convert to form context
        //    url = Xrm.Page.context.getClientUrl();

        //VTRIGILI - 2015-01-21 code not working when there is an organization on the url, needs trialing /
        //if (url[url.length - 1] != "/") { url = url + "/"; }
        //url = url + 'xrmservices/2011/OrganizationData.svc/ReportSet?$select=ReportId,Name';
        //TODO: convert to WebAPI
        var cols = ["reportid", "name"];
        webApi.RetrieveMultiple("reports", cols, null)
            .then(function (data) {
                if (data && data.length > 0) {
                    len = data.length;

                    for (var i = 0; i < len; i++) {
                        me.list.push({ name: data[i].name, reportid: data[i].reportid });
                    }

                }
            })
            .catch((error) => { alert("fail"); });
        /*doRequest = function (url) {
            $.ajax({
                type: 'GET',
                url: url,
                dataType: 'json'
            }).done(function (data) {
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

        doRequest(url);*/
    }
}


reports.getReports();

function Letters(exCon, CommandProperties) {
    debugger;
    formContext = exCon.getFormContext();
  
    //TODO: convert to form context
    var action = formContext.getAttribute("udo_action");
    var menuXmlECCAction = '<Menu Id=\"ISV.DynamicMenu\">' +
         '<MenuSection Id=\"ISV.Dynamic.MenuSection\" Sequence=\"10\">' +
           '<Controls Id=\"ISV.Dynamic.Controls\">' +
                '<Button Id=\"ISV.Dynamic.119\" Command=\"ISV.SearchCommand\" Sequence=\"10\" LabelText=\"119- Report of Contact\" Alt=\"119- Report of Contact\"  ToolTipTitle=\"119- Report of Contact\"  ToolTipDescription=\"119- Report of Contact\"  />' +
                '<Button Id=\"ISV.Dynamic.AmountLetterGross\" Command=\"ISV.SearchCommand\" Sequence=\"60\" LabelText=\"Amount Letter Gross\" Alt=\"Amount Letter Gross\"  ToolTipTitle=\"Amount Letter Gross\"  ToolTipDescription=\"Amount Letter Gross\" />' +
                '<Button Id=\"ISV.Dynamic.BlankLetter\" Command=\"ISV.SearchCommand\" Sequence=\"70\" LabelText=\"Blank Letter\" Alt=\"Blank Letter\"  ToolTipTitle=\"Blank Letter\"  ToolTipDescription=\"Blank Letter\" />' +
                '<Button Id=\"ISV.Dynamic.CommissaryLetter\" Command=\"ISV.SearchCommand\" Sequence=\"80\" LabelText=\"Commissary Letter\" Alt=\"Commissary Letter\"  ToolTipTitle=\"Commissary Letter\"  ToolTipDescription=\"Commissary Letter\" />' +
                '<Button Id=\"ISV.Dynamic.CommissaryLetterSurvivingSpouse\" Command=\"ISV.SearchCommand\" Sequence=\"82\" LabelText=\"Commissary Letter Surviving Spouse\" Alt=\"Commissary Letter Surviving Spouse\"  ToolTipTitle=\"Commissary Letter Surviving Spouse\"  ToolTipDescription=\"Commissary Letter Surviving Spouse\" />' +
                '<Button Id=\"ISV.Dynamic.CommissaryLetterwithFutureExam\" Command=\"ISV.SearchCommand\" Sequence=\"83\" LabelText=\"Commissary Letter with Future Exam\" Alt=\"Commissary Letter with Future Exam\"  ToolTipTitle=\"Commissary Letter with Future Exam\" ToolTipDescription=\"Commissary Letter with Future Exam\"/>' +
                '<Button Id=\"ISV.Dynamic.DeathBenefitsLetter\" Command=\"ISV.SearchCommand\" Sequence=\"84\" LabelText=\"Death Benefits Letter\" Alt=\"Death Benefits Letter\"  ToolTipTitle=\"Death Benefits Letter\"  ToolTipDescription=\"Death Benefits Letter\" />' +
                '<Button Id=\"ISV.Dynamic.DisabilityBreakdownLetter\" Command=\"ISV.SearchCommand\" Sequence=\"85\" LabelText=\"Disability Breakdown Letter\" Alt=\"Disability Breakdown Letter\"  ToolTipTitle=\"Disability Breakdown Letter\"  ToolTipDescription=\"Disability Breakdown Letter\" />' +
                '<Button Id=\"ISV.Dynamic.FaxCoverSheet\" Command=\"ISV.SearchCommand\" Sequence=\"86\" LabelText=\"Fax Cover Sheet\" Alt=\"Fax Cover Sheet\"  ToolTipTitle=\"Fax Cover Sheet\"  ToolTipDescription=\"Fax Cover Sheet\" />' +
                '<Button Id=\"ISV.Dynamic.IncomeAmountLetter\" Command=\"ISV.SearchCommand\" Sequence=\"87\" LabelText=\"Income Amount Letter\" Alt=\"Income Amount Letter\"  ToolTipTitle=\"Income Amount Letter\"  ToolTipDescription=\"Income Amount Letter\" />' +
                //'<Button Id=\"ISV.Dynamic.InformalClaimLetter\" Command=\"ISV.SearchCommand\" Sequence=\"88\" LabelText=\"Informal Claim Letter - AB10\" Alt=\"Informal Claim Letter - AB10\" />' +
                '<Button Id=\"ISV.Dynamic.MODMemorandum\" Command=\"ISV.SearchCommand\" Sequence=\"89\" LabelText=\"MOD Memorandum\" Alt=\"MOD Memorandum\"  ToolTipTitle=\"MOD Memorandum\"  ToolTipDescription=\"MOD Memorandum\" />' +
                '<Button Id=\"ISV.Dynamic.MultiplePaymentLetter\" Command=\"ISV.SearchCommand\" Sequence=\"90\" LabelText=\"Multiple Payment Letter\" Alt=\"Multiple Payment Letter\"  ToolTipTitle=\"Multiple Payment Letter\"  ToolTipDescription=\"Multiple Payment Letter\" />' +
                '<Button Id=\"ISV.Dynamic.NoBenefitsLetter\" Command=\"ISV.SearchCommand\" Sequence=\"91\" LabelText=\"No Benefits Letter\" Alt=\"No Benefits Letter\"  ToolTipTitle=\"No Benefits Letter\"  ToolTipDescription=\"No Benefits Letter\" />' +
                '<Button Id=\"ISV.Dynamic.NOKLetter\" Command=\"ISV.SearchCommand\" Sequence=\"92\" LabelText=\"NOK Letter\" Alt=\"NOK Letter\"  ToolTipTitle=\"NOK Letter\"  ToolTipDescription=\"NOK Letter\" />' +
                '<Button Id=\"ISV.Dynamic.PreferenceLetters\" Command=\"ISV.SearchCommand\" Sequence=\"93\" LabelText=\"Preference Letters\" Alt=\"Preference Letters\"  ToolTipTitle=\"Preference Letters\"  ToolTipDescription=\"Preference Letters\" />' +
                '<Button Id=\"ISV.Dynamic.PropertyTaxDMVRegistration\" Command=\"ISV.SearchCommand\" Sequence=\"94\" LabelText=\"Property Tax DMV Registration - MT\" Alt=\"Property Tax DMV Registration - MT\"  ToolTipTitle=\"Property Tax DMV Registration - MT\"  ToolTipDescription=\"Property Tax DMV Registration - MT\" />' +
                '<Button Id=\"ISV.Dynamic.PropertyTaxExemption\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Property Tax Exemption - CA - LA\" Alt=\"Property Tax Exemption - CA - LA\"  ToolTipTitle=\"Property Tax Exemption - CA - LA\"  ToolTipDescription=\"Property Tax Exemption - CA - LA\" />' +
                '<Button Id=\"ISV.Dynamic.RealEstate\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Real Estate - License Fee Tax Exemption\" Alt=\"Real Estate - License Fee Tax Exemption\"  ToolTipTitle=\"Real Estate - License Fee Tax Exemption\"  ToolTipDescription=\"Real Estate - License Fee Tax Exemption\" />' +
                '<Button Id=\"ISV.Dynamic.ServiceVerification\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Service Verification\" Alt=\"Service Verification\"  ToolTipTitle=\"Service Verification\"  ToolTipDescription=\"Service Verification\" />' +
                '<Button Id=\"ISV.Dynamic.SummaryofBenefitsSurvivingSpouseLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Summary of Benefits Surviving Spouse Letter\" Alt=\"Summary of Benefits Surviving Spouse Letter\"  ToolTipTitle=\"Summary of Benefits Surviving Spouse Letter\"  ToolTipDescription=\"Summary of Benefits Surviving Spouse Letter\" />' +
                '<Button Id=\"ISV.Dynamic.SummaryofBenefitsVeteransLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Summary of Benefits Veterans Letter\" Alt=\"Summary of Benefits Veterans Letter\"  ToolTipTitle=\"Summary of Benefits Veterans Letter\"  ToolTipDescription=\"Summary of Benefits Veterans Letter\" />' +
                '<Button Id=\"ISV.Dynamic.VeteranPercentageLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Veteran Percentage Letter\" Alt=\"Veteran Percentage Letter\"  ToolTipTitle=\"Veteran Percentage Letter\"  ToolTipDescription=\"Veteran Percentage Letter\" />' +
                '<Button Id=\"ISV.Dynamic.VeteranPTLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Veteran PT Letter\" Alt=\"Veteran PT Letter\"  ToolTipTitle=\"Veteran PT Letter\"  ToolTipDescription=\"Veteran PT Letter\" />' +

            '</Controls>' +
          '</MenuSection></Menu>';
    var menuXmlAction = '<Menu Id=\"ISV.DynamicMenu\">' +
         '<MenuSection Id=\"ISV.Dynamic.MenuSection\" Sequence=\"10\">' +
           '<Controls Id=\"ISV.Dynamic.Controls\">' +
                '<Button Id=\"ISV.Dynamic.0820\" Command=\"ISV.SearchCommand\" Sequence=\"20\" LabelText=\"27-0820- Report of General Information\" Alt=\"27-0820- Report of General Information\" ToolTipTitle=\"27-0820- Report of General Information\" ToolTipDescription=\"27-0820- Report of General Information\" />' +
                '<Button Id=\"ISV.Dynamic.0820a\" Command=\"ISV.SearchCommand\" Sequence=\"30\" LabelText=\"27- 0820a-Report of First Notice of Death\" Alt=\"27-0820a-Report of First Notice of Death\"  ToolTipTitle=\"27-0820a-Report of First Notice of Death\"  ToolTipDescription=\"27-0820a-Report of First Notice of Death\" />' +
                '<Button Id=\"ISV.Dynamic.0820d\" Command=\"ISV.SearchCommand\" Sequence=\"40\" LabelText=\"27- 0820d-Report of Non Reciept of Payment\" Alt=\"27-0820d-Report of Non Reciept of Payment\"  ToolTipTitle=\"27-0820d-Report of Non Reciept of Payment\"  ToolTipDescription=\"27-0820d-Report of Non Reciept of Payment\" />' +
                '<Button Id=\"ISV.Dynamic.0820f\" Command=\"ISV.SearchCommand\" Sequence=\"50\" LabelText=\"27- 0820f-Report of Month of Death\" Alt=\"27-0820f-Report of Month of Death\"  ToolTipTitle=\"27-0820f-Report of Month of Death\"  ToolTipDescription=\"27-0820f-Report of Month of Death\" />' +
                '<Button Id=\"ISV.Dynamic.AmountLetterGross\" Command=\"ISV.SearchCommand\" Sequence=\"60\" LabelText=\"Amount Letter Gross\" Alt=\"Amount Letter Gross\"  ToolTipTitle=\"Amount Letter Gross\"  ToolTipDescription=\"Amount Letter Gross\" />' +
                '<Button Id=\"ISV.Dynamic.BlankLetter\" Command=\"ISV.SearchCommand\" Sequence=\"70\" LabelText=\"Blank Letter\" Alt=\"Blank Letter\" ToolTipTitle=\"Blank Letter\" ToolTipDescription=\"Blank Letter\" />' +
                '<Button Id=\"ISV.Dynamic.CommissaryLetter\" Command=\"ISV.SearchCommand\" Sequence=\"80\" LabelText=\"Commissary Letter\" Alt=\"Commissary Letter\"  ToolTipTitle=\"Commissary Letter\"  ToolTipDescription=\"Commissary Letter\" />' +
                '<Button Id=\"ISV.Dynamic.CommissaryLetterSurvivingSpouse\" Command=\"ISV.SearchCommand\" Sequence=\"82\" LabelText=\"Commissary Letter Surviving Spouse\" Alt=\"Commissary Letter Surviving Spouse\" ToolTipTitle=\"Commissary Letter Surviving Spouse\" ToolTipDescription=\"Commissary Letter Surviving Spouse\" />' +
                '<Button Id=\"ISV.Dynamic.CommissaryLetterwithFutureExam\" Command=\"ISV.SearchCommand\" Sequence=\"83\" LabelText=\"Commissary Letter with Future Exam\" Alt=\"Commissary Letter with Future Exam\"  ToolTipTitle=\"Commissary Letter with Future Exam\"  ToolTipDescription=\"Commissary Letter with Future Exam\" />' +
                '<Button Id=\"ISV.Dynamic.DeathBenefitsLetter\" Command=\"ISV.SearchCommand\" Sequence=\"84\" LabelText=\"Death Benefits Letter\" Alt=\"Death Benefits Letter\"  ToolTipTitle=\"Death Benefits Letter\"  ToolTipDescription=\"Death Benefits Letter\" />' +
                '<Button Id=\"ISV.Dynamic.DisabilityBreakdownLetter\" Command=\"ISV.SearchCommand\" Sequence=\"85\" LabelText=\"Disability Breakdown Letter\" Alt=\"Disability Breakdown Letter\"  ToolTipTitle=\"Disability Breakdown Letter\"  ToolTipDescription=\"Disability Breakdown Letter\" />' +
                '<Button Id=\"ISV.Dynamic.FaxCoverSheet\" Command=\"ISV.SearchCommand\" Sequence=\"86\" LabelText=\"Fax Cover Sheet\" Alt=\"Fax Cover Sheet\"  ToolTipTitle=\"Fax Cover Sheet\"  ToolTipDescription=\"Fax Cover Sheet\"/>' +
                '<Button Id=\"ISV.Dynamic.IncomeAmountLetter\" Command=\"ISV.SearchCommand\" Sequence=\"87\" LabelText=\"Income Amount Letter\" Alt=\"Income Amount Letter\"  ToolTipTitle=\"Income Amount Letter\"  ToolTipDescription=\"Income Amount Letter\" />' +
                //'<Button Id=\"ISV.Dynamic.InformalClaimLetter\" Command=\"ISV.SearchCommand\" Sequence=\"88\" LabelText=\"Informal Claim Letter - AB10\" Alt=\"Informal Claim Letter - AB10\" />' +
                '<Button Id=\"ISV.Dynamic.MODMemorandum\" Command=\"ISV.SearchCommand\" Sequence=\"89\" LabelText=\"MOD Memorandum\" Alt=\"MOD Memorandum\"  ToolTipTitle=\"MOD Memorandum\"  ToolTipDescription=\"MOD Memorandum\" />' +
                '<Button Id=\"ISV.Dynamic.MultiplePaymentLetter\" Command=\"ISV.SearchCommand\" Sequence=\"90\" LabelText=\"Multiple Payment Letter\" Alt=\"Multiple Payment Letter\"  ToolTipTitle=\"Multiple Payment Letter\"  ToolTipDescription=\"Multiple Payment Letter\" />' +
                '<Button Id=\"ISV.Dynamic.NoBenefitsLetter\" Command=\"ISV.SearchCommand\" Sequence=\"91\" LabelText=\"No Benefits Letter\" Alt=\"No Benefits Letter\"  ToolTipTitle=\"No Benefits Letter\"  ToolTipDescription=\"No Benefits Letter\" />' +
                '<Button Id=\"ISV.Dynamic.NOKLetter\" Command=\"ISV.SearchCommand\" Sequence=\"92\" LabelText=\"NOK Letter\" Alt=\"NOK Letter\"  ToolTipTitle=\"NOK Letter\"  ToolTipDescription=\"NOK Letter\" />' +
                '<Button Id=\"ISV.Dynamic.PreferenceLetters\" Command=\"ISV.SearchCommand\" Sequence=\"93\" LabelText=\"Preference Letters\" Alt=\"Preference Letters\"  ToolTipTitle=\"Preference Letters\"  ToolTipDescription=\"Preference Letters\" />' +
                '<Button Id=\"ISV.Dynamic.PropertyTaxDMVRegistration\" Command=\"ISV.SearchCommand\" Sequence=\"94\" LabelText=\"Property Tax DMV Registration - MT\" Alt=\"Property Tax DMV Registration - MT\"  ToolTipTitle=\"Property Tax DMV Registration - MT\"  ToolTipDescription=\"Property Tax DMV Registration - MT\" />' +
                '<Button Id=\"ISV.Dynamic.PropertyTaxExemption\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Property Tax Exemption - CA - LA\" Alt=\"Property Tax Exemption - CA - LA\"  ToolTipTitle=\"Property Tax Exemption - CA - LA\"  ToolTipDescription=\"Property Tax Exemption - CA - LA\" />' +
                '<Button Id=\"ISV.Dynamic.RealEstate\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Real Estate - License Fee Tax Exemption\" Alt=\"Real Estate - License Fee Tax Exemption\"  ToolTipTitle=\"Real Estate - License Fee Tax Exemption\"  ToolTipDescription=\"Real Estate - License Fee Tax Exemption\" />' +
                '<Button Id=\"ISV.Dynamic.ServiceVerification\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Service Verification\" Alt=\"Service Verification\"  ToolTipTitle=\"Service Verification\"  ToolTipDescription=\"Service Verification\" />' +
                '<Button Id=\"ISV.Dynamic.SummaryofBenefitsSurvivingSpouseLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Summary of Benefits Surviving Spouse Letter\" Alt=\"Summary of Benefits Surviving Spouse Letter\"  ToolTipTitle=\"Summary of Benefits Surviving Spouse Letter\"  ToolTipDescription=\"Summary of Benefits Surviving Spouse Letter\" />' +
                '<Button Id=\"ISV.Dynamic.SummaryofBenefitsVeteransLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Summary of Benefits Veterans Letter\" Alt=\"Summary of Benefits Veterans Letter\" ToolTipTitle=\"Summary of Benefits Veterans Letter\" ToolTipDescription=\"Summary of Benefits Veterans Letter\" />' +
                '<Button Id=\"ISV.Dynamic.VeteranPercentageLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Veteran Percentage Letter\" Alt=\"Veteran Percentage Letter\"  ToolTipTitle=\"Veteran Percentage Letter\"  ToolTipDescription=\"Veteran Percentage Letter\" />' +
                '<Button Id=\"ISV.Dynamic.VeteranPTLetter\" Command=\"ISV.SearchCommand\" Sequence=\"95\" LabelText=\"Veteran PT Letter\" Alt=\"Veteran PT Letter\"  ToolTipTitle=\"Veteran PT Letter\"  ToolTipDescription=\"Veteran PT Letter\" />' +

            '</Controls>' +
          '</MenuSection></Menu>';

    var menuXmlActionBlankLetter = '<Menu Id=\"ISV.DynamicMenu\">' +
         '<MenuSection Id=\"ISV.Dynamic.MenuSection\" Sequence=\"10\">' +
           '<Controls Id=\"ISV.Dynamic.Controls\">' +
                '<Button Id=\"ISV.Dynamic.BlankLetter\" Command=\"ISV.SearchCommand\" Sequence=\"70\" LabelText=\"Blank Letter\" Alt=\"Blank Letter\"  ToolTipTitle=\"Blank Letter\"  ToolTipDescription=\"Blank Letter\" />' +
                '<Button Id=\"ISV.Dynamic.FaxCoverSheet\" Command=\"ISV.SearchCommand\" Sequence=\"86\" LabelText=\"Fax Cover Sheet\" Alt=\"Fax Cover Sheet\"  ToolTipTitle=\"Fax Cover Sheet\"  ToolTipDescription=\"Fax Cover Sheet\" />' +

            '</Controls>' +
          '</MenuSection></Menu>';

    if (action != null) {
        //TODO: convert to form context
        var actionvalue = formContext.getAttribute("udo_action").getValue();
        var controlId = actionvalue;

        switch (controlId) {
            //ECC
            case 953850000:
                CommandProperties.PopulationXML = menuXmlECCAction;
                break;
            //0820
            case 1:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
            //0820a 
            case 953850001:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
            //0820d  
            case 953850002:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
            //0820f  
            case 953850003:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
            //VAI  
            case 2:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
            //0820 & VAI    
            case 953850005:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
            //Letter      
            case 953850006:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
            //Non Emergency Email        
            case 953850007:
                CommandProperties.PopulationXML = menuXmlAction;
                break;
            //Email Forms          
            case 953850004:
                CommandProperties.PopulationXML = menuXmlActionBlankLetter;
                break;
            default:
                CommandProperties.PopulationXML = menuXmlActionBlankLetter;

        }

    }   

}


function getReportID(reportName) {
    var len = reports.list.length;
    var reportName = reportName + " - UDO";
    for (var i = 0; i < len; i++) {
        if (reports.list[i].name === reportName) {
            return reports.list[i].reportid;
        }
    }

    return '';
}

function getReportUrl(action, reportName, serviceRequestId){
    var orgUrl = Xrm.Utility.getGlobalContext().getClientUrl();
    var reportUrl = null;
    var reportId = getReportID(reportName);
    if (reportId.length > 0) {
        reportUrl = orgUrl +
            "crmreports/viewer/viewer.aspx ? action =" +
            encodeURIComponent(action) +
            "&id=%7b" +
            encodeURIComponent(reportId) +
            "%7d&p:ServiceRequestGUID=" +
            encodeURIComponent(serviceRequestId);
    }
    return reportUrl;
}

//Call Report based on seletion
function Search(exCon, CommandProperties) {
    var controlId = CommandProperties.SourceControlId;
    //TODO: convert to form context
    formContext = exCon.getFormContext();
    var GUIDvalue = formContext.data.entity.getId();
    var letterCreatedMsg=null;
    var reportUrl = null;
    var reportName = null;
    var action = "run";
    //VTRIGILI - 2015-01-21 code not working when there is an organization on the url, needs trialing /
    //if (serverUrl[serverUrl.length - 1] != "/") { serverUrl = serverUrl + "/"; }
    //TODO: convert to form context
    debugger;
    var lettersCreated = formContext.getAttribute("udo_letterscreated");
    Getinfo()
        .then(function (pcr) {
            var now = new Date();
            var today = (now.getMonth() + 1) + '/' + now.getDate() + '/' + now.getFullYear() + ' ' + now.getHours() + ':' + ((now.getMinutes() < 10 ? '0' : '') + now.getMinutes());

            switch (controlId) {


                case 'ISV.Dynamic.0820':
                    reportName = "27-0820 - Report of General Information";
                    letterCreatedMsg = "Letter 0820 was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.0820a':
                    reportName = "27-0820a - Report of First Notice of Death";
                    letterCreatedMsg = "Letter 0820a was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.0820d':
                    reportName = "27-0820d - Report of Non-Receipt of Payment";
                    letterCreatedMsg = "Letter 0820d was created by " + pcr + ' on ' + today;
                    break;

                case 'ISV.Dynamic.0820f':
                    reportName = "27-0820f - Report of Month of Death";
                    letterCreatedMsg = "Letter 0820f was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.119':
                    reportName = "119 - Report of Contact";
                    letterCreatedMsg = "Letter 0820 was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.AmountLetterGross':
                    reportName = "Amount Letter Gross";
                    letterCreatedMsg = "Amount Letter Gross was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.BlankLetter':
                    reportName = "Blank Letter";
                    letterCreatedMsg = "Blank Letter was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.CommissaryLetter':
                    reportName = "Commissary Letter";
                    letterCreatedMsg = "Commissary Letter was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.CommissaryLetterSurvivingSpouse':
                    reportName = "'Commissary Letter Surviving Spouse";
                    letterCreatedMsg = "Commissary Letter Surviving Spouse was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.CommissaryLetterwithFutureExam':
                    reportName = "Commissary Letter with Future Exam";
                    letterCreatedMsg = "Commissary Letter with Future Exam was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.DeathBenefitsLetter':
                    reportName = "Death Benefits Letter";
                    letterCreatedMsg = "Death Benefits Letter was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.DisabilityBreakdownLetter':
                    reportName = "Disability Breakdown Letter";
                    letterCreatedMsg = "Disability Breakdown Letter was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.FaxCoverSheet':
                    reportName = "Fax Cover Sheet";
                    letterCreatedMsg = "Fax Cover Sheet was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.IncomeAmountLetter':
                    reportName = "Income Amount Letter";
                    letterCreatedMsg = "Income Amount Letter was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.MODMemorandum':
                    reportName = "MOD Memorandum"
                    letterCreatedMsg = "MOD Memorandum was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.MultiplePaymentLetter':
                    reportName = "Multiple Payment Letter";
                    letterCreatedMsg = "Multiple Payment Letter was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.NoBenefitsLetter':
                    reportName = "No Benefits Letter";
                    letterCreatedMsg = "No Benefits Letter was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.NOKLetter':
                    reportName = "NOK Letter";
                    letterCreatedMsg = "NOK Letter was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.PreferenceLetters':
                    reportName = "Preference Letters";
                    letterCreatedMsg = "Preference Letters was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.PropertyTaxDMVRegistration':
                    reportName = "Property Tax DMV Registration - MT";
                    letterCreatedMsg = "Property Tax DMV Registration was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.PropertyTaxExemption':
                    reportName = "Property Tax Exemption - CA - LA";
                    letterCreatedMsg = "Property Tax Exemption was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.RealEstate':
                    reportName = "Real Estate - License Fee Tax Exemption";
                    letterCreatedMsg = "Real Estate was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.ServiceVerification':
                    reportName = "Service Verification";
                    letterCreatedMsg = "Service Verification was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.SummaryofBenefitsSurvivingSpouseLetter': 
                    reportName = "Summary of Benefits Surviving Spouse Letter";
                    letterCreatedMsg = "Summary of Benefits Surviving Spouse Letter was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.SummaryofBenefitsVeteransLetter':
                    reportName = "Summary of Benefits Veterans Letter";
                    letterCreatedMsg = "Summary of Benefits Veterans Letter was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.VeteranPercentageLetter':
                    reportName = "Veteran Percentage Letter";
                    letterCreatedMsg = "Veteran Percentage Letter was created by " + pcr + " on " + today;
                    break;

                case 'ISV.Dynamic.VeteranPTLetter':
                    reportName = "Veteran PT Letter";
                    letterCreatedMsg = "Veteran PT Letter was created by " + pcr + " on " + today;
                    break;

                default:
                    alert('Button Unknown');
            }
            //TODO: add error handling
            if (reportName != null) {
                reportUrl = getReportUrl(action, reportName, GUIDvalue);
                if (reportUrl != null) {
                    Xrm.Navigation.openUrl(reportUrl);
                    if (lettersCreated.getValue() == null) {
                        lettersCreated.setValue(letterCreatedMsg);
                    }
                    else {
                        lettersCreated.setValue(lettersCreated.getValue() + ';\n' + letterCreatedMsg);
                    }
                }
                else {
                    Xrm.Navigation.openErrorDialog({ message: "Could not find report specified" });
                }
            }
        }).catch((error) => { Xrm.Navigation.openErrorDialog({ message: error.message }); });
}