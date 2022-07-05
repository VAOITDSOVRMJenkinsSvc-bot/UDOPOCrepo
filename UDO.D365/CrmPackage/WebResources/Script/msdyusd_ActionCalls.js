function ClearAction()
{
  Xrm.Page.data.entity.attributes.get("msdyusd_action").setValue(null)
}

function GetServerUrl()
{
	return Xrm.Page.context.getClientUrl();
}

var req = null;
var exCon = null;
var formContext = null;
function ActionChanged(execCon)
{
    exCon = execCon;
    formContext = exCon.getFormContext();
  try
  {
    var actionId = Xrm.Page.data.entity.attributes.get("msdyusd_action");
    if (actionId != null)
    {
      // example:   https://crm.mspmtc.net/UnifiedServiceDesk/WebResources/msdyusd_Documentation/Actions/Navigate
      var url = GetServerUrl() + "/WebResources/msdyusd_Documentation/Actions/" + actionId.getValue()[0].name;
      req = _getXMLHttpRequest();
      req.open("GET", url, true);
      req.onreadystatechange = handleRequestStateChange;
      req.send();
    }
  }
  catch (ex)
  {
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
      var actionId = Xrm.Page.data.entity.attributes.get("msdyusd_action");
      if (actionId != null)
      {
        var url = GetServerUrl() + "/WebResources/msdyusd_Documentation/Actions/" + actionId.getValue()[0].name;
        Xrm.Page.ui.controls.get("WebResource_ParameterInput").setSrc(url);
      }
      else
      {
        var url = GetServerUrl() + "/WebResources/msdyusd_Documentation/Actions/undocumented";
        Xrm.Page.ui.controls.get("WebResource_ParameterInput").setSrc(url);
      }
    } 
    else
    {
      var url = GetServerUrl() + "/WebResources/msdyusd_Documentation/Actions/undocumented";
      Xrm.Page.ui.controls.get("WebResource_ParameterInput").setSrc(url);
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