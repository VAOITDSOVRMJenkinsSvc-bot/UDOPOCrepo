//Place this code in the onload event of a form containing a sub-grid that you wish to filter  
  
window.attachEvent("onload", function() {  
  function locAssocObj_custom(iType, sSubType, sAssociationName, iRoleOrdinal, additionalParams, showNew, showProp, defaultViewId, customViews, allowFilterOff, disableQuickFind, disableViewPicker, viewsIds) {  
    var lookupItems = LookupObjects(null, "multi", iType, 0, null, additionalParams, showNew, showProp, null, null, null, null, defaultViewId, customViews, null, null, null, null, allowFilterOff, disableQuickFind, disableViewPicker, viewsIds, false);  
    if (lookupItems) lookupItems.items.length > 0 && AssociateObjects(crmFormSubmit.crmFormSubmitObjectType.value, crmFormSubmit.crmFormSubmitId.value, iType, lookupItems, iRoleOrdinal == 2, sSubType, sAssociationName)  
  }  
  
  if (Mscrm.GridRibbonActions.addExistingFromSubGridAssociated) {  
    Mscrm.GridRibbonActions.addExistingFromSubGridAssociated_org = window.Mscrm.GridRibbonActions.addExistingFromSubGridAssociated;  
  
    window.Mscrm.GridRibbonActions.addExistingFromSubGridAssociated = function(gridTypeCode, gridControl) {  
      if (IsNull(gridControl)) {  
        throw Error.argument("value", "gridControl is null or undefined")  
        return;  
      }  
  
   //Check if a filtered grid.  If not call default method        
      if (IsNull(gridControl._element.isFiltered) || !gridControl._element.isFiltered) {  
        Mscrm.GridRibbonActions.addExistingFromSubGridAssociated_org(gridTypeCode, gridControl)  
      } else {  
        var e = document.getElementById(gridControl._element.id);  
  
        var showNew = (IsNull(e.showNew) ? null : e.showNew);  
        var showProp = (IsNull(e.showProp) ? null : e.showProp);  
        var allowFilterOff = (IsNull(e.allowFilterOff) ? null : e.allowFilterOff);  
        var disableQuickFind = (IsNull(e.disableQuickFind) ? null : e.disableQuickFind);  
        var disableViewPicker = (IsNull(e.disableViewPicker) ? null : e.disableViewPicker);  
        var customViews = (IsNull(e.customViews) ? null : e.customViews);  
        var defaultViewId = (IsNull(e.defaultViewId) ? null : e.defaultViewId);  
        var viewsIds = null;  
  
        var $v_0 = gridControl.getParameter("relName"), $v_1 = gridControl.getParameter("roleOrd"), $v_2 = false;  
  
        switch (gridTypeCode) {  
          case Mscrm.EntityTypeCode.List:  
            switch ($v_0) {  
              case "campaignactivitylist_association":  
                window.parent.locAssocObjCampaignActivity(gridTypeCode, "", $v_0, $v_1);  
                break;  
              case "campaignlist_association":  
                locAssocObjCampaign(gridTypeCode, "subType=targetLists", $v_0, $v_1);  
                break;  
              case "listlead_association":  
                window.parent.locAssocObjLead(gridTypeCode, "", $v_0, $v_1);  
                break;  
              case "listcontact_association":  
                window.parent.locAssocObjContact(gridTypeCode, "", $v_0, $v_1);  
                break;  
              case "listaccount_association":  
                window.parent.locAssocObjAccount(gridTypeCode, "", $v_0, $v_1);  
                break;  
              default:  
                $v_2 = true;  
                break  
            }  
            break;  
          case Mscrm.EntityTypeCode.Campaign:  
            switch ($v_0) {  
              case "campaignlist_association":  
                locAssocObjList(gridTypeCode, "subType=targetLists", $v_0, $v_1);  
                break;  
              case "campaigncampaign_association":  
                locAssocObjCampaign(gridTypeCode, "", $v_0, $v_1);  
                break;  
              default:  
                $v_2 = true;  
                break  
            }  
            break;  
          case Mscrm.EntityTypeCode.Product:  
            switch ($v_0) {  
              case "productsubstitute_association":  
                locAssocObjProduct(gridTypeCode, "", $v_0, $v_1);  
                break;  
              case "productassociation_association":  
                locAssocObjProduct(gridTypeCode, "", $v_0, $v_1);  
                break;  
              case "campaignproduct_association":  
                locAssocObjCampaign(gridTypeCode, "", $v_0, $v_1);  
                break;  
              case "competitorproduct_association":  
                window.parent.locAssocObjCompetitor(gridTypeCode, "", $v_0, $v_1);  
                break;  
              default:  
                $v_2 = true; break  
            }  
            break;  
          default:  
            $v_2 = true;  
            break  
        }  
        if ($v_2) {  
          var $v_3 = locAssocObj_custom;  
          var additionalParams = null;  
          //$v_3(gridTypeCode,"",$v_0,$v_1, additionalParams)       
          $v_3(gridTypeCode, "", $v_0, $v_1, additionalParams, showNew, showProp, defaultViewId, customViews, allowFilterOff, disableQuickFind, disableViewPicker, viewsIds);  
        }  
      }  
    }  
  }  
});  
  
function addSubgridCustomView(subgridID, entityTypeCode, displayName, fetchXML, layoutXML, filterType, isDefault) {  
/// <summary>  
/// Adds a custom view to lookup page used by a subgrid  
/// </summary>  
/// <param name="subgridID" type="string">/// ID of sub-grid to add custom view to  
/// /// <param name="entityTypeCode" type="int">/// The entity type code returned by the fetch statement  
/// /// <param name="displayName" type="string">/// The name to use for the view  
/// /// <param name="fetchXML" type="string">/// The fetch XML to be used by the view  
/// /// <param name="layoutXML" type="string">/// The layout XML to be used by the view.  If layoutXML == null the layout from the default view of the entity will be used  
/// /// <param name="filterType" type="int">/// Type of custom view.  Default is 0  
/// /// <param name="isDefault" type="boolean">/// The layout XML to be used by the view.  If layoutXML == null the layout from the default view of the entity will be used  
/// /// <returns type="nothing">  
 if (IsNull(subgridID)) { throw Error.argument("value", "ID of sub-grid to filter not provided") ; return; }  
 if (IsNull(entityTypeCode)) { throw Error.argument("value", "Entype code of returned object by fetchXML not provided"); return; }  
 if (IsNull(fetchXML)) { throw Error.argument("value", "FetchXml not provide for custom view on sub-grid"); return; }  
 if (IsNull(layoutXML)) { throw Error.argument("value", "LayoutXml not provided for custom view on sub-grid"); return; }  
  
 var grd = document.getElementById(subgridID);  
  
 //Check if the sub-grid has any customViews already.  
 var customViews = (IsNull(grd.customViews) ? new Array() : grd.customViews);  
  
 var oScriptlet = new ActiveXObject("Scriptlet.TypeLib");  
 var viewId = oScriptlet.GUID.toString().substr(0, 38);  //Call substr to address issue of trailing white space  
   
 //Create an object to hold the customView's information  
 var customView = new Object();  
   customView.fetchXml = fetchXML;  
   customView.id = viewId;  
   customView.layoutXml = layoutXML;  
   customView.name = (IsNull(displayName) || displayName == "" ? "Filtered Lookup" : displayName);  
   customView.recordType = entityTypeCode;  
   customView.Type = (!IsNull(filterType) ? filterType : 0);  
  
 //Add the customView object to the array of customViews  
 customViews.push(customView);        
  
 //Add the array of custom views to the sub-grid  
 grd.customViews = customViews;  
  
 //Set this view as the default if desired  
 if (isDefault) { grd.defaultViewId = viewId; }  
}  
  
var fetchXML = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>";  
 fetchXML += "<entity name='account'>";  
 fetchXML += "<all-attributes>";  
 fetchXML += "<filter type='and'>";  
 fetchXML += "<condition attribute='name' operator='like' value='test%'>";  
 fetchXML += "</condition></filter>";  
 fetchXML += "</all-attributes></entity>";  
 fetchXML += "</fetch>";  
  
//Build the layout to use in the lookup.  Keep in mind that you will need to know the Object Type Code  
//and the primary key of object displayed  
var layoutXML = "<grid name='resultset' object='1' jump='name' select='1' icon='1' preview='1'>";  
 layoutXML += "<row name='result' id='accountid'>";  
 layoutXML += "<cell name='name' width='300'>";  
 layoutXML += "</cell></row>";  
 layoutXML += "</grid>";  
     
//Apply custom view / lookup filter to the grid  
addSubgridCustomView("#GRID_ID#", "#OBJECT_TYPE_CODE#", "Test", fetchXML, layoutXML, 0, true);
    