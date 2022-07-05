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
 this.ReleaseToQueueRequest = function (queueItemId)
{
///<summary>
 /// Contains the data that is needed to move an entity record from a source queue to a destination queue. 
///</summary>
///<param name="queueItemId" type="String">
 /// Sets the target queue item.
///</param>
if (!(this instanceof Sdk.ReleaseToQueueRequest)) {
return new Sdk.ReleaseToQueueRequest(queueItemId, workerId, removeQueueItem);
}
Sdk.OrganizationRequest.call(this);

  // Internal properties
var _queueItemId = null;

// internal validation functions

function _setValidQueueItemId(value) {
 if (value != null && Sdk.Util.isGuid(value)) {
  _queueItemId = value;
 }
 else {
  throw new Error("Sdk.ReleaseToQueueRequest QueueItemId property is required and must be a String containing a Guid.")
 }
}

//Set internal properties from constructor parameters
  if (typeof queueItemId != "undefined") {
   _setValidQueueItemId(queueItemId);
  }
	 
  function getRequestXml() {
return ["<d:request>",
        "<a:Parameters>",

		  "<a:KeyValuePairOfstringanyType>",
            "<b:key>QueueItemId</b:key>",
           (_queueItemId == null) ? "<b:value i:nil=\"true\" />" :
           ["<b:value i:type=\"e:guid\">", _queueItemId, "</b:value>"].join(""),
          "</a:KeyValuePairOfstringanyType>",

        "</a:Parameters>",
        "<a:RequestId i:nil=\"true\" />",
        "<a:RequestName>ReleaseToQueue</a:RequestName>",
      "</d:request>"].join("");
  }

  this.setResponseType(Sdk.ReleaseToQueueResponse);
  this.setRequestXml(getRequestXml());

  // Public methods to set properties
  this.setQueueItemId = function (value) {
	  _setValidQueueItemId(value);
	  this.setRequestXml(getRequestXml());
  }	 
 }
 this.ReleaseToQueueRequest.__class = true;

this.ReleaseToQueueResponse = function (responseXml) {
  ///<summary>
 /// Contains the response from the ReleaseToQueueRequest class. 
  ///</summary>
  if (!(this instanceof Sdk.ReleaseToQueueResponse)) {
   return new Sdk.ReleaseToQueueResponse(responseXml);
  }
  Sdk.OrganizationResponse.call(this)

  // This message returns no values

 }
this.ReleaseToQueueResponse.__class = true;
}).call(Sdk)

Sdk.ReleaseToQueueRequest.prototype = new Sdk.OrganizationRequest();
Sdk.ReleaseToQueueResponse.prototype = new Sdk.OrganizationResponse();
