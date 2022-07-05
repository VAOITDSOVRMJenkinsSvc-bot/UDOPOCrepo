/*
 * CCallWrapper.js
 * $Revision: 1.3 $ $Date: 2003/07/07 18:32:43 $
 */

/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is Netscape code.
 *
 * The Initial Developer of the Original Code is
 * Netscape Corporation.
 * Portions created by the Initial Developer are Copyright (C) 2003
 * the Initial Developer. All Rights Reserved.
 *
 * Contributor(s): Bob Clary <bclary@netscape.com>
 *
 * ***** END LICENSE BLOCK ***** */

function CCallWrapper(aObjectReference, 
                      aDelay,
                      aMethodName, 
                      aArgument0,
                      aArgument1,
                      aArgument2,
                      aArgument3,
                      aArgument4,
                      aArgument5,
                      aArgument6,
                      aArgument7,
                      aArgument8,
                      aArgument9
                      )
{
  // CSDev Left Intentionally Blank 
}

CCallWrapper.prototype.execute = function()
{
// CSDev Left Intentionally Blank 
};

CCallWrapper.prototype.cancel = function()
{
// CSDev Left Intentionally Blank 
};

CCallWrapper.asyncExecute = function (/* CCallWrapper */ callwrapper)
{
  // CSDev Left Intentionally Blank CCallWrapper.mPendingCalls[callwrapper.mId].mTimerId = setTimeout('CCallWrapper.mPendingCalls["' + callwrapper.mId + '"].execute()', callwrapper.mDelay);
};

CCallWrapper.mCounter = 0;
CCallWrapper.mPendingCalls = {};