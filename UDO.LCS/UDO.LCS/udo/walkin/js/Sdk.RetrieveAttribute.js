// =====================================================================
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================
"use strict";
(function () {
	
function processResponse(node,includeNulls)
{
	function isNumeric(v)
	{
		return !isNaN(parseFloat(v)) && isFinite(v);
	}

	function textToValue(v)
	{
		var r =
			v === 'true' ? true :
			v === 'false' ? false :
			v === 'TRUE' ? true :
			v === 'FALSE' ? false :
			isNumeric(v) ? Number(v) :
			v;
		//console.log(r);
		return r;
	}
	
	function special(e,k,n)
	{
		var label = null;
		for(var i=0; i<n.childNodes.length; i++)
		{
			if (n.childNodes[i].nodeName=="a:UserLocalizedLabel") {
				label = n.childNodes[i];
				break;
			}
		}
		
		if (label!=null) {
		    if (Sdk.Xml.selectSingleNode(label, "a:Label") != null)
		        e[k].text = Sdk.Xml.selectSingleNode(label, "a:Label").text;
		    else e[k].text = "";
		}
	}

	function nodeValue(n,an)
	{
		if (typeof an=="undefined") an=true;
		var r={};
		if ((typeof n==="undefined") || n==null || (typeof n.childNodes === "undefined") || n.childNodes.length==0) return null;
		if (n.childNodes.length==1 && n.childNodes[0].nodeName=="#text") return textToValue(n.text);
		for (var i=0; i<n.childNodes.length;i++) 
		{
			var c = n.childNodes[i], 
				v = nodeValue(c,an), 
				k = c.nodeName.substring(c.nodeName.indexOf(":")+1);
			
			if (!an && v==null) continue;
				
			if (typeof r[k] !== "undefined") 
			{
				if (r[k].push) r[k].push(v);
				else r[k] = [r[k],v];
			} else r[k] = v;
			
			special(r,k,c);
		}
		return r;
	}
	return nodeValue(node,includeNulls)
}

	
this.RetrieveOptionSetRequest = function(entity,attribute) {
	if (!(this instanceof Sdk.RetrieveOptionSetRequest)) {
		return new Sdk.RetrieveOptionSetRequest(entity, attribute);
	}
	
	var request = new Sdk.RetrieveAttributeRequest(entity, attribute, null, true);
	request.setRequestXml = this.setRequestXml;
	
	
	this.setEntity = request.setEntity;
	this.setAttribute = request.setAttribute;
	this.setMetadataId = request.setMetadataId;
	this.setRetrieveAsIfPublished = request.setRetrieveAsIfPublished;
    this.setRetrieveAsIfPublished(true); // force setRequestXml to be executed
	this.setResponseType(Sdk.RetrieveOptionSetResponse);
}
this.RetrieveOptionSetRequest.__class = true;

this.RetrieveOptionSetResponse = function (responseXml) {
 if (!(this instanceof Sdk.RetrieveOptionSetResponse)) {
   return new Sdk.RetrieveOptionSetResponse(responseXml);
  } 
  Sdk.OrganizationResponse.call(this);
  
  this.AttributeMetadata = processResponse(Sdk.Xml.selectSingleNode(responseXml, "//b:value"),false);
  
  var am = this.AttributeMetadata;
  
  var os = {
	  Name:"",
	  IsGlobal:false,
	  DisplayName:"",
	  Options: []
  };
  
  if (am!=null) {
	os.Name = am.OptionSet.Name;
	os.DisplayName = am.OptionSet.DisplayName.text;
	os.IsGlobal = am.OptionSet.IsGlobal;
	os.DefaultFormValue = am.DefaultFormValue;
	os.EntityLogicalName = am.EntityLogicalName;
	os.LogicalName = am.LogicalName;
	
	var options = am.OptionSet.Options.OptionMetadata;
	if (options && options.length>0) {
		for(var i=0; i<options.length;i++)
		{
			var option = options[i];
			os.Options.push({Name:option.Label.text, Value: option.Value});
		}
	}
  }
  
  this.OptionSet = os;
};
this.RetrieveOptionSetResponse.__class = true;


this.RetrieveAttributeRequest = function (entity, attribute, metadataid, retrieveAsIfPublished)
{
///<summary>
 /// Contains the data that is needed to move an entity record from a source queue to a destination queue. 
///</summary>
///<param name="entity"  type="String">
 /// Sets the target record to the logicalname of the target Entity
///</param>
///<param name="attribute" optional="true" type="String">
 /// Sets the attribute to retrieve
///</param>
///<param name="metadataid" optional="true" type="String">
 /// Sets the metadataid of the attribute to retrieve (optional)
///</param>
///<param name="retrieveAsIfPublished"  type="Boolean">
 /// Retrieve the published attribute
///</param>
if (!(this instanceof Sdk.RetrieveAttributeRequest)) {
return new Sdk.RetrieveAttributeRequest(entity, attribute, metadataid, retrieveAsIfPublished);
}
Sdk.OrganizationRequest.call(this);

  // Internal properties
var _entityName = null;
var _attrbiuteName = null;
var _published = null;
var _metadataId = null;

// internal validation functions

function _setValidEntityName(value) {
 if (value == null) {
   throw new Error("Sdk.RetrieveAttribute Entity is required to execute this request.");
 } 
 else if (typeof value == "string")
 {
  if (value=="")
  {
	throw new Error("Sdk.RetrieveAttribute Entity cannot be empty, it is required to execute this request.");
  }	else {
    _entityName = value;
  }
 }
 else if (value instanceof Sdk.Entity) {
  _entityName = value.getType();
 }
 else {
  throw new Error("Sdk.RetrieveAttribute Entity must be a string containing the logical name of an entity or of type Sdk.Entity.");
 }
 if (_entityName==null || _entityName=="")
 {
   throw new Error("Sdk.RetrieveAttribute Entity is required to execute this request."); 
 }
}

function _setValidAttributeName(value) {
 if (value == null) {
   throw new Error("Sdk.RetrieveAttribute Attribute is required to execute this request.");
 } 
 else if (typeof value == "string")
 {
  if (value=="")
  {
	throw new Error("Sdk.RetrieveAttribute Attribute cannot be empty, it is required to execute this request.");
  }	else {
    _attrbiuteName = value;
  }
 }
 else if (value instanceof Sdk.AttributeBase) {
  _attrbiuteName = value.getName();
 }
 else {
  throw new Error("Sdk.RetrieveAttribute Attribute must be a string containing the logical name of an attribute or of type Sdk.AttributeBase.");
 }
 if (_attrbiuteName==null || _attrbiuteName=="")
 {
   throw new Error("Sdk.RetrieveAttribute Attribute is required to execute this request."); 
 }
}

function _setValidMetadataId(value) {
 if (typeof value == "undefined" || value==null) {
  _metadataId = null;
 } else if (Sdk.Util.isGuid(value)) {
  _metadataId = value;
 }
 else {
  throw new Error("Sdk.RetrieveAttribute MetadataId property must be a String GUID or null.");
 }
}

function _setValidRetrieveAsIfPublished(value) {
 if (typeof value == "undefined" || value == null) { _published = true; }
 else if (typeof value == "string")
 {
   if (value="") {
     _published = true;
   }
   _published = (!(value[0]=='f' || value[0]=='F' || value[0]=='n' || value[0]=='N' || value[0]=='0'));
 }
 else if (typeof value == "boolean")
 {
   _published = value;
 }
}

//Set internal properties from constructor parameters
  if (typeof entity != "undefined") {
   _setValidEntityName(entity);
  }
  if (typeof attribute != "undefined") {
   _setValidAttributeName(attribute);
  }
  _setValidMetadataId(metadataid);
  _setValidRetrieveAsIfPublished(retrieveAsIfPublished);

  function getRequestXml() {
return ["<d:request>",
        "<a:Parameters>",

			"<a:KeyValuePairOfstringanyType>",
			"<b:key>EntityLogicalName</b:key>",
			(_entityName == null) ? "<b:value i:nil=\"true\" />" :
			["<b:value i:type=\"c:string\">", _entityName, "</b:value>"].join(""),
			"</a:KeyValuePairOfstringanyType>",

			"<a:KeyValuePairOfstringanyType>",
			"<b:key>LogicalName</b:key>",
			(_attrbiuteName == null) ? "<b:value i:nil=\"true\" />" :
			["<b:value i:type=\"c:string\">", _attrbiuteName, "</b:value>"].join(""),
			"</a:KeyValuePairOfstringanyType>",
			
            "<a:KeyValuePairOfstringanyType>",
            "<b:key>MetadataId</b:key>",
            ["<b:value i:type=\"e:guid\">", 
			   (_metadataId == null) ? "00000000-0000-0000-0000-000000000000" 
			   : _metadataId, 
			   "</b:value>"].join(""),
            "</a:KeyValuePairOfstringanyType>",

            "<a:KeyValuePairOfstringanyType>",
			"<b:key>RetrieveAsIfPublished</b:key>",
			["<b:value i:type=\"c:boolean\">", _published, "</b:value>"].join(""),
			"</a:KeyValuePairOfstringanyType>",

        "</a:Parameters>",
        "<a:RequestId i:nil=\"true\" />",
        "<a:RequestName>RetrieveAttribute</a:RequestName>",
      "</d:request>"].join("");
  }

  this.setResponseType(Sdk.RetrieveAttributeResponse);
  this.setRequestXml(getRequestXml());

  // Public methods to set properties
  this.setEntity = function (value) {
  ///<summary>
   /// Sets the logical name of the entity. Required. 
  ///</summary>
  ///<param name="value" type="String">
   /// Logical name of the entity.
  ///</param>
   _setValidEntityName(value);
   this.setRequestXml(getRequestXml());
  }

  this.setAttribute = function (value) {
  ///<summary>
   /// Sets the logical name of the attribute. Required.
  ///</summary>
  ///<param name="value" type="String">
   /// Logical name of the attribute.
  ///</param>
   _setValidAttributeName(value);
   this.setRequestXml(getRequestXml());
  }

  this.setMetadataId = function (value) {
  ///<summary>
   /// Sets the ID of the metadata attribute. Optional. 
  ///</summary>
  ///<param name="value" type="String">
   /// The MetadataId of the attribute.
  ///</param>
   _setValidMetadataId(value);
   this.setRequestXml(getRequestXml());
  }

  this.setRetrieveAsIfPublished = function (value) {
  ///<summary>
   /// Retrieve the metadata as if published, default is true. Optional. 
  ///</summary>
  ///<param name="value" type="Sdk.Entity">
   /// true to use published, false to use unpublished.
  ///</param>
   _setValidRetrieveAsIfPublished(value);
   this.setRequestXml(getRequestXml());
  }

 }
 this.RetrieveAttributeRequest.__class = true;

 this.RetrieveAttributeResponse = function (responseXml) {
  ///<summary>
 /// Contains the response from the RetrieveAttributeRequest class. 
  ///</summary>
  if (!(this instanceof Sdk.RetrieveAttributeResponse)) {
   return new Sdk.RetrieveAttributeResponse(responseXml);
  }
  Sdk.OrganizationResponse.call(this);


  this.AttributeMetadata = processResponse(Sdk.Xml.selectSingleNode(responseXml, "//b:value"),false);
 }
 this.RetrieveAttributeResponse.__class = true;
}).call(Sdk);

Sdk.RetrieveAttributeRequest.prototype = new Sdk.OrganizationRequest();
Sdk.RetrieveAttributeResponse.prototype = new Sdk.OrganizationResponse();

Sdk.RetrieveOptionSetRequest.prototype = new Sdk.OrganizationRequest();
Sdk.RetrieveOptionSetResponse.prototype = new Sdk.OrganizationResponse();
