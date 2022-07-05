var exCon = null;
var formContext = null;

function GetServerUrl()
{
	return Xrm.Page.context.getClientUrl();
}

var req = null;
function LoadForm(execCon)
{
    exCon = execCon;
    formContext = exCon.getFormContext();
  var eventname= Xrm.Page.data.entity.attributes.get("msdyusd_name");
  if (eventname!= null)
  {
    // example:   https://crm.mspmtc.net/UnifiedServiceDesk/WebResources/msdyusd_Documentation/Actions/Navigate
    var url = GetServerUrl() + "/WebResources/msdyusd_Documentation/Events/" + eventname.getValue();
    req = _getXMLHttpRequest();
    req.open("GET", url, true);
    req.onreadystatechange = handleRequestStateChange;
    req.send();
  }
}

function handleRequestStateChange() 
{
  // continue if the process is completed
  if (req.readyState == 4) 
  {
    // continue only if HTTP status is "OK"
    if (req.status == 200) 
    {
      var eventname= Xrm.Page.data.entity.attributes.get("msdyusd_name");
      if (eventname!= null)
      {
        var url = GetServerUrl() + "/WebResources/msdyusd_Documentation/Events/" + eventname.getValue();
        Xrm.Page.ui.controls.get("WebResource_EventDocumentation").setSrc(url);
      }
      else
      {
        var url = GetServerUrl() + "/WebResources/msdyusd_Documentation/Events/undocumented";
        Xrm.Page.ui.controls.get("WebResource_EventDocumentation").setSrc(url);
      }
    } 
    else
    {
      var url = GetServerUrl() + "/WebResources/msdyusd_Documentation/Events/undocumented";
      Xrm.Page.ui.controls.get("WebResource_EventDocumentation").setSrc(url);
    }
  }
}

function _getXMLHttpRequest() {
        try {
            if (navigator.appVersion.indexOf("MSIE 7.") != -1) {
                try { return new ActiveXObject("Msxml2.XMLHTTP.6.0"); }
                catch (e) { }
                try { return new ActiveXObject("Msxml2.XMLHTTP.3.0"); }
                catch (e) { }
                try { return new ActiveXObject("Microsoft.XMLHTTP"); }
                catch (e) { }
            }
        }
        catch (e) { /* do nothing */ }

        if (window.XMLHttpRequest) {
            return new window.XMLHttpRequest;
        }
        else {
            try { return new ActiveXObject("Msxml2.XMLHTTP.6.0"); }
            catch (e) { }
            try { return new ActiveXObject("Msxml2.XMLHTTP.3.0"); }
            catch (e) { }
            try { return new ActiveXObject("Microsoft.XMLHTTP"); }
            catch (e) { }
        }
}