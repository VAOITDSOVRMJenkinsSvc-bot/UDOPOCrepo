// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================
"use strict";
(function () {
 this.RouteToRequest = function (queueItemId, target)
{
///<summary>
 /// Contains the data that is needed to move an entity record from a source queue to a destination queue. 
///</summary>
///<param name="target" type="Sdk.EntityReference">
 /// Sets the target queue, user, or team.
///</param>
///<param name="queueItemId" type="String">
 /// Sets the queueItemId of the item to route to the target queue, user, or team.
///</param>
if (!(this instanceof Sdk.RouteToRequest)) {
return new Sdk.RouteToRequest(queueItemId, target);
}
Sdk.OrganizationRequest.call(this);

  // Internal properties
var _target = null;
var _queueItemId = null;

// internal validation functions

function _setValidTarget(value) {
 if (value instanceof Sdk.EntityReference) {
  _target = value;
 }
 else {
  throw new Error("Sdk.RouteToRequest Target property is required and must be a Sdk.EntityReference.")
 }
}

function _setValidQueueItemId(value) {
 if (value != null && Sdk.Util.isGuid(value)) {
  _queueItemId = value;
 }
 else {
  throw new Error("Sdk.ReleaseToQueueRequest QueueItemId property is required and must be a String containing a Guid.")
 }
}

//Set internal properties from constructor parameters
  if (typeof target != "undefined") {
   _setValidTarget(target);
  }
  if (typeof queueItemId != "undefined") {
   _setValidQueueItemId(queueItemId);
  }

  function getRequestXml() {
return ["<d:request>",
        "<a:Parameters>",

          "<a:KeyValuePairOfstringanyType>",
            "<b:key>Target</b:key>",
           (_target == null) ? "<b:value i:nil=\"true\" />" :
           ["<b:value i:type=\"a:EntityReference\">", _target.toValueXml(), "</b:value>"].join(""),
          "</a:KeyValuePairOfstringanyType>",

		  "<a:KeyValuePairOfstringanyType>",
            "<b:key>QueueItemId</b:key>",
           (_queueItemId == null) ? "<b:value i:nil=\"true\" />" :
           ["<b:value i:type=\"e:guid\">", _queueItemId, "</b:value>"].join(""),
          "</a:KeyValuePairOfstringanyType>",

        "</a:Parameters>",
        "<a:RequestId i:nil=\"true\" />",
        "<a:RequestName>RouteTo</a:RequestName>",
      "</d:request>"].join("");
  }

  this.setResponseType(Sdk.RouteToResponse);
  this.setRequestXml(getRequestXml());

  // Public methods to set properties
  this.setTarget = function (value) {
  ///<summary>
   /// Sets the target record to add to the destination queue. Required. 
  ///</summary>
  ///<param name="value" type="Sdk.EntityReference">
   /// The target record to add to the destination queue.
  ///</param>
   _setValidTarget(value);
   this.setRequestXml(getRequestXml());
  }

  this.setQueueItemId = function (value) {
	  _setValidQueueItemId(value);
	  this.setRequestXml(getRequestXml());
  }	
  
 }
 this.RouteToRequest.__class = true;

this.RouteToResponse = function (responseXml) {
  ///<summary>
 /// Contains the response from the RouteToRequest class. 
  ///</summary>
  if (!(this instanceof Sdk.RouteToResponse)) {
   return new Sdk.RouteToResponse(responseXml);
  }
  Sdk.OrganizationResponse.call(this)

  // This message returns no values
 }

 this.RouteToResponse.__class = true;
}).call(Sdk)

Sdk.RouteToRequest.prototype = new Sdk.OrganizationRequest();
Sdk.RouteToResponse.prototype = new Sdk.OrganizationResponse();
