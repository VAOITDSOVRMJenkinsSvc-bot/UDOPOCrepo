﻿/************** udo_process.js ******************************************/
"use strict";

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};

Va.Udo.Crm.Scripts.Process = function (executionContext) {
	var _formContext = executionContext;
	var serverUrl = null;

	var DataTypes = {
		Bool: "boolean",
		Int: "int",
		String: "string",
		DateTime: "dateTime",
		EntityReference: "EntityReference",
		OptionSet: "OptionSetValue",
		Money: "Money",
		Guid: "guid"
	};

	var soapParams = function (paramArray, genericNSPrefix, schemaNSPrefix) {

		var xmlEncode = function (input) {
			var between = function (i, a, b) { return (i > a && i < b); };
			if (typeof input === "undefined" || input === null || input === '') return '';

			var output = '';
			for (var i = 0; i < input.length; i++) {
				var c = input.charCodeAt(i);
				if (between(c, 96, 123) || between(c, 64, 91) || between(c, 47, 58) || between(c, 43, 47) || c === 95 || c === 32) {
					output += String.fromCharCode(c);
				} else {
					output += "&#" + c + ";";
				}
			}
			return output;
		};

		var params = "";
		var value = "";

		if (paramArray) {
			// Add each input param
			for (var i = 0; i < paramArray.length; i++) {
				var param = paramArray[i];
				var includeNS = false;
				var type = ":" + param.Type;
				var typeNS = "http://www.w3.org/2001/XMLSchema";

				switch (param.Type) {
					case "dateTime":
						value = param.Value.toISOString();
						type = schemaNSPrefix + type;
						includeNS = true;
						break;
					case "EntityReference":
						type = "a" + type;
						value = "<a:Id>" + param.Value.id + "</a:Id><a:LogicalName>" + param.Value.entityType + "</a:LogicalName><a:Name i:nil='true' />";
						break;
					case "OptionSetValue":
					case "Money":
						type = "a" + type;
						value = "<a:Value>" + param.Value + "</a:Value>";
						break;
					case "guid":
						type = schemaNSPrefix + type;
						value = param.Value;
						includNS = true;
						typeNS = "http://schemas.microsoft.com/2003/10/Serialization/";
						break;
					case "string":
						type = schemaNSPrefix + type;
						value = xmlEncode(param.Value);
						includeNS = true;
						break;
					default:
						type = schemaNSPrefix + type;
						value = param.Value;
						includeNS = true;
						break;
				}

				params += "<a:KeyValuePairOfstringanyType>" +
					"<" + genericNSPrefix + ":key>" + param.Key + "</" + genericNSPrefix + ":key>" +
					"<" + genericNSPrefix + ":value i:type='" + type + "'";
				if (includeNS) params += " xmlns:" + schemaNSPrefix + "='" + typeNS + "'";
				params += ">" + value + "</" + genericNSPrefix + ":value>" +
					"</a:KeyValuePairOfstringanyType>";
			}
		}

		return "<a:Parameters xmlns:" + genericNSPrefix + "='http://schemas.datacontract.org/2004/07/System.Collections.Generic'>" +
			params + "</a:Parameters>";
	}

	var soapExecute = function (requestXml) {
		return "<Execute xmlns='http://schemas.microsoft.com/xrm/2011/Contracts/Services' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'>" +
			requestXml + "</Execute>";
	};

	var soapEnvelope = function (message) {
		return "<s:Envelope xmlns:s='http://schemas.xmlsoap.org/soap/envelope/'>" +
			"<s:Body>" + message + "</s:Body>" +
			"</s:Envelope>";
	};

	var soapActionRequest = function (action, inputParams) {
		return "<request xmlns:a='http://schemas.microsoft.com/xrm/2011/Contracts'>" +
			soapParams(inputParams, 'b', 'c') +
			"<a:RequestId i:nil='true' />" +
			"<a:RequestName>" + action + "</a:RequestName>" +
			"</request>";
	};

	var soapExecuteWorkflowRequest = function (workflowId, recordId) {
		return "<request i:type='b:ExecuteWorkflowRequest' xmlns:a='http://schemas.microsoft.com/xrm/2011/Contracts' xmlns:b='http://schemas.microsoft.com/crm/2011/Contracts'>" +
			soapParams([{ Key: "EntityId", Type: DataTypes.Guid, Value: recordId },
			{ Key: "WorkflowId", Type: DataTypes.Guid, Value: workflowId }], 'c', 'd') +
			"<a:RequestId i:nil='true' />" +
			"<a:RequestName>ExecuteWorkflow</a:RequestName>" +
			"</request>";
	};

	var execCrmSoapRequest = function (soapMessage) {
		if (serverUrl === null) {
			serverUrl = parent.Xrm.Page.context.getClientUrl();
			serverUrl += "/XRMServices/2011/Organization.svc/web";
			serverUrl = serverUrl.replace("//XRMServices", "/XRMServices");
		}

		var options = {
			url: serverUrl,
			type: "POST",
			dataType: "xml",
			data: soapMessage,
			processData: false,
			global: false,
			beforeSend: function (xhr) {
				xhr.setRequestHeader('SOAPAction', 'http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute');
				xhr.setRequestHeader("Accept", "application/xml, text/xml */*");
				xhr.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
			}
		};

		return $.ajax(options);
		return result;
	};

	var callAction = function (action, inputParams) {
		var dfd = $.Deferred();

		execCrmSoapRequest(soapEnvelope(soapExecute(soapActionRequest(action, inputParams))))
			.done(function (a, b, xhr) {
				var result = getValues(xhr.responseXML);
				dfd.resolve(result, b, xhr);
			})
			.fail(function (err) {
				dfd.reject(err);
			});

		return dfd.promise();
	};

	var getValues = function (xmlData) {
		var XmlToEntity = function (node) {
			try {
				//ToDo: This code needs to be validated
				var entity = {
					logicalName: node.getElementsByTagName("a:LogicalName")[0].text(),
					id: node.getElementsByTagName("a:Id")[0].text(),
					attributes: getValues(node.getElementsByTagName("a:Attributes")[0])
				};
			} catch (err) {
				return null;
			}
			try {
				var formattedValuesNode = node.getElementsByTagName("a:FormattedValues");
				if (formattedValuesNode !== null && formattedValuesNode.length > 0) {
					entity.formattedValues = getValues(formattedValuesNode);
				}
			} catch (err) { }
			return entity;
		};

		var XmlToEntities = function (node) {
			var xmlEntities = node.getElementsByTagName("a:Entity");
			var entities = [];
			for (var i = 0; i < xmlEntities.length; i++) {
				entities[i] = XmlToEntity(xmlEntities[i]);
			}
			return entities;
		};

		var kvps = xmlData.getElementsByTagName("a:KeyValuePairOfstringanyType");
		if (typeof kvps === "undefined" || kvps === null || kvps.length === 0) {
			kvps = xmlData.getElementsByTagName("KeyValuePairOfstringanyType");
		}
		if (typeof kvps === "undefined" || kvps === null || kvps.length === 0) {
			kvps = [];
		} else {
			kvps = kvps[0].parentNode.childNodes;
		}

		var result = {};
		for (var i = 0; i < kvps.length; i++) {
			var key = $(kvps[i].childNodes[0]).text();
			var valueObj = $(kvps[i].childNodes[1]);
			var typeNode = valueObj.attr("i:type");
			// continue if no type (like null values)
			if (typeof typeNode === undefined || typeNode === null) continue;
			// get the type from the node
			var type = valueObj.attr("i:type").toLowerCase();
			type = type.substring(type.indexOf(":") + 1);

			// setup value variable.
			var value = "";
			if (type === "aliasedvalue") {
				for (var i = 0; i < valueObj[0].childNodes.length; i++) {
					if (valueObj[0].childNodes[i].tagName === "a:Value") {
						valueObj = $(valueObj.childNodes[i]);
						break;
					}
				}
				// reset type using the aliasedvalue result
				type = valueObj.attr("i:type").toLowerCase();
				type = type.substring(type.indexOf(":") + 1);
			}
			switch (type) {
				case "entity":
					value = XmltoEntity(valueObj);
					break;
				case "entitycollection":
					value = XmlToEntities(valueObj[0]);
					break;
				case "entityreference":
					value = {
						id: $(valueObj[0].childNodes[0]).text(),
						entityType: $(valueObj[0].childNodes[1]).text()
					};
					if (valueObj[0].childNodes[2]) value.name = $(valueObj[0].childNodes[2]).text();
					break;
				case "datetime":
					value = new Date(valueObj.text());
					break;
				case "decimal":
				case "double":
				case "int":
				case "money":
				case "optionsetvalue":
					value = Number(valueObj.text());
					break;
				case "boolean":
					value = valueObj.text().toLowerCase() === "true";
					break;
				default: //string
					value = valueObj.text();
					break;
			}

			result[key] = value;
		}
		return result;
	}

	var callWorkflow = function (workflowId, recordId) {
		return execCrmSoapRequest(soapEnvelope(soapExecute(soapExecuteWorkflowRequest(workflowId, recordId))));
	};

	return {
		DataType: DataTypes,
		ExecuteAction: callAction,
		ExecuteWorkflow: callWorkflow
	};
}();