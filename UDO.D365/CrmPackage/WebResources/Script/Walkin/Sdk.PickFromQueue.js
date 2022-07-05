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
 this.PickFromQueueRequest = function (queueItemId, workerId, removeQueueItem)
{
///<summary>
 /// Contains the data that is needed to move an entity record from a source queue to a destination queue. 
///</summary>
///<param name="queueItemId" type="String">
 /// Sets the target queue item.
///</param>
///<param name="workerId" type="String">
 /// Sets the workerId.
///</param>
///<param name="removeQueueItem"  type="Boolean">
 /// Sets the removeQueueItem. 
///</param>
if (!(this instanceof Sdk.PickFromQueueRequest)) {
return new Sdk.PickFromQueueRequest(queueItemId, workerId, removeQueueItem);
}
Sdk.OrganizationRequest.call(this);

  // Internal properties
var _queueItemId = null;
var _removeQueueItem = false;
var _workerId = null;

// internal validation functions

function _setValidQueueItemId(value) {
 if (value != null && Sdk.Util.isGuid(value)) {
  _queueItemId = value;
 }
 else {
  throw new Error("Sdk.PickFromQueueRequest QueueItemId property is required and must be a String containing a Guid.")
 }
}

function _setValidWorkerId(value) {
 if (value != null && Sdk.Util.isGuid(value)) {
  _workerId = value;
 }
 else {
  throw new Error("Sdk.PickFromQueueRequest WorkerId property is required and must be a String containing a Guid.")
 }
}

function _setValidRemoveQueueItem(value) {
 if (value !== false && value != "false" && value!=0) {
  _removeQueueItem = true;
 } else {
  _removeQueueItem = false;
 }
}

//Set internal properties from constructor parameters
  if (typeof queueItemId != "undefined") {
   _setValidQueueItemId(queueItemId);
  }
  if (typeof workerId != "undefined") {
   _setValidWorkerId(workerId);
  }
  if (typeof removeQueueItem != "undefined") {
   _setValidRemoveQueueItem(removeQueueItem);
  }

  function getRequestXml() {
return ["<d:request>",
        "<a:Parameters>",

		  "<a:KeyValuePairOfstringanyType>",
            "<b:key>QueueItemId</b:key>",
           (_queueItemId == null) ? "<b:value i:nil=\"true\" />" :
           ["<b:value i:type=\"e:guid\">", _queueItemId, "</b:value>"].join(""),
          "</a:KeyValuePairOfstringanyType>",
		
		  "<a:KeyValuePairOfstringanyType>",
            "<b:key>WorkerId</b:key>",
           (_workerId == null) ? "<b:value i:nil=\"true\" />" :
           ["<b:value i:type=\"e:guid\">", _workerId, "</b:value>"].join(""),
          "</a:KeyValuePairOfstringanyType>",

          "<a:KeyValuePairOfstringanyType>",
            "<b:key>RemoveQueueItem</b:key>",
            (_removeQueueItem == null) ? "<b:value i:nil=\"true\" />" :
            ["<b:value i:type=\"c:boolean\">", _removeQueueItem, "</b:value>"].join(""),
          "</a:KeyValuePairOfstringanyType>",

        "</a:Parameters>",
        "<a:RequestId i:nil=\"true\" />",
        "<a:RequestName>PickFromQueue</a:RequestName>",
      "</d:request>"].join("");
  }

  this.setResponseType(Sdk.PickFromQueueResponse);
  this.setRequestXml(getRequestXml());

  // Public methods to set properties
  this.setQueueItemId = function (value) {
	  _setValidQueueItemId(value);
	  this.setRequestXml(getRequestXml());
  }	 

  this.setWorkerId = function (value) {
	  _setValidWorkerId(value);
	  this.setRquestXml(getRequestXml());
  }

  this.setRemoveQueueItem = function (value) {
	  _setValidRemoveQueueItem(true);
	  this.setRequestXml(getRequestXml());
  }
 }
 this.PickFromQueueRequest.__class = true;

this.PickFromQueueResponse = function (responseXml) {
  ///<summary>
 /// Contains the response from the PickFromQueueRequest class. 
  ///</summary>
  if (!(this instanceof Sdk.PickFromQueueResponse)) {
   return new Sdk.PickFromQueueResponse(responseXml);
  }
  Sdk.OrganizationResponse.call(this)

  // This message returns no values

 }
this.PickFromQueueResponse.__class = true;
}).call(Sdk)

Sdk.PickFromQueueRequest.prototype = new Sdk.OrganizationRequest();
Sdk.PickFromQueueResponse.prototype = new Sdk.OrganizationResponse();
