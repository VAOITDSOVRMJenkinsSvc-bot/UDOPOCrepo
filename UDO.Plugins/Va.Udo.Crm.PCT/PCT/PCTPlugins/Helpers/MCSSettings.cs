﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.Runtime.Caching;
using System.Runtime.Serialization.Json;

namespace MCSUtilities2011
{
    /// <summary>
    /// 
    /// Instructions on adding to your plugin,
    /// 1) Add a an existing Item to your project, navigate to this cs file, down in the bottom right corner where it says "Add", change to "Add as Link"
    /// 2) Add a Reference to System.Runtime.Caching to the plugin project this is needed by the settings caching mechanism.
    /// 
    /// </summary>
    public class MCSSettings
    {
        private const string settingskey = "mcs_settings";
        private const int cacheMinutes = 15;

        private static MemoryCache _cache = new MemoryCache(settingskey);
        public static T AddOrGetFromCache<T>(string key, Func<T> init)
        {
            var newValue = new Lazy<T>(init);
            var oldValue = _cache.AddOrGetExisting(key, newValue, DateTime.Now.AddMinutes(cacheMinutes)) as Lazy<T>;
            try
            {
                return (oldValue ?? newValue).Value;
            }
            catch
            {
                _cache.Remove(key);
                throw;
            }
        }

        private Entity _settings = null;

        private IOrganizationService _Service;
        public IOrganizationService setService
        {
            set { _Service = value; }
        }
        private MCSLogger _Logger;
        public MCSLogger setLogger
        {
            set { _Logger = value; }
        }
        private static bool _Debug;
        public bool getDebug
        {
            get { return _Debug; }
        }

        public object this[string key]
        {
            get
            {
                if (_settings == null || !_settings.Contains(key)) return null;
                return _settings[key];
            }
        }

        private static string _restendpointforvimt;
        public string getVIMTRESTEndPoint
        {
            get { return _restendpointforvimt; }
        }

        private static bool _GranularTxnTiming;
        public bool getGranular
        {
            get { return _GranularTxnTiming; }
        }

        private static bool _Transactionaltimings;
        public bool getTxnTiming
        {
            get { return _Transactionaltimings; }
        }

        private string _DebugField;
        public string setDebugField
        {
            set { _DebugField = value; }
        }

        private string _SystemSetting;
        public string systemSetting
        {
            get { return _SystemSetting; }
            set { _SystemSetting = value; }
        }

        private string _UnexpectedMessage;
        public string getUnexpectedErrorMessage
        {
            get { return _UnexpectedMessage; }
        }

        private Guid _SettingId;
        public Guid getSettingsId
        {
            get { return _SettingId; }
        }

        /// <summary>
        /// Load the initial settings.  
        /// It will look in shared variables, then System.Runtime.Cache, then it will retrieve from CRM.
        /// </summary>
        /// <param name="context">Plugin Execution Context</param>
        public void GetStartupSettings(IPluginExecutionContext context = null)
        {
            try
            {
                _UnexpectedMessage = string.Empty;

                #region Load Settings [_settings]
                #region 1. Attempt to read from SharedVariables
                if (context != null && context.IsolationMode == 1 && _settings == null) GetFromSharedVariables(context);
                #endregion

                #region 2. Attempt to read from .Net Global Cache and CRM
                try
                {
                    if (context != null /*&& context.IsolationMode== 1 None */ && _settings == null)
                    {
                        _settings = AddOrGetFromCache<Entity>(settingskey + "_" + context.OrganizationName, GetSettings);

                        if (context.IsolationMode == 1)
                        {
                            SaveToSharedVariables(context);
                        }
                    }
                }
                catch
                {
                    if (_settings == null)
                    {
                        _settings = GetSettings();
                        if (context.IsolationMode == 1) SaveToSharedVariables(context);
                    }
                }
                #endregion
                #endregion

                #region Use Settings to setup MCSSettings object.
                if (_settings == null)
                {
                    _Logger.WriteToFile("Active Settings not Found");
                    _GranularTxnTiming = false;
                    _Transactionaltimings = false;
                    _Debug = false;
                }
                else
                {
                    _SettingId = _settings.Id;

                    if (_settings.Contains("mcs_granulartimings"))
                    {
                        _GranularTxnTiming = _settings.GetAttributeValue<bool>("mcs_granulartimings");
                    }
                    else
                    {
                        _GranularTxnTiming = false;
                    }

                    if (_settings.Contains("mcs_granulartimings"))
                    {
                        _Transactionaltimings = _settings.GetAttributeValue<bool>("mcs_transactionaltiming");
                    }
                    else
                    {
                        _Transactionaltimings = false;
                    }

                    if (!String.IsNullOrEmpty(_DebugField) && _settings.Contains(_DebugField))
                    {
                        _Debug = _settings.GetAttributeValue<bool>(_DebugField);
                    }
                    else
                    {
                        _Debug = false;
                    }

                    _restendpointforvimt = _settings.GetAttributeValue<string>("crme_restendpointforvimt");
                }
                #endregion
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                _Logger.setMethod = "GetStartupSettings";
                _Logger.WriteToFile(ex.Message);
            }
            catch (Exception ex)
            {
                _Logger.setMethod = "GetStartupSettings";
                _Logger.WriteToFile(ex.Message);
            }
        }

        private Entity GetSettings()
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = "mcs_setting",
                ColumnSet = new ColumnSet(true),
                NoLock = true,
                TopCount = 1,
                Criteria = new FilterExpression()
            };
            query.Criteria.AddCondition("mcs_name", ConditionOperator.Equal, _SystemSetting);

            EntityCollection results = _Service.RetrieveMultiple(query);

            if (results.Entities.Count > 0)
            {
                return results.Entities[0];
            }
            return null;
        }

        private static Entity JsonToEntity(string json)
        {
            Entity e = new Entity();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(e.GetType());
                e = ser.ReadObject(ms) as Entity;
            }
            return e;
        }

        private static string EntityToJson(Entity entity)
        {
            byte[] jsonArray = new byte[0];
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(entity.GetType());
                ser.WriteObject(ms, entity);
                jsonArray = ms.ToArray();
            }
            if (jsonArray.Length==0) return string.Empty;
            return Encoding.UTF8.GetString(jsonArray, 0, jsonArray.Length);
        }

        private void GetFromSharedVariables(IPluginExecutionContext context)
        {
            var key = settingskey;
            if (context != null && context.SharedVariables.ContainsKey(key))
            {
                _settings = context.SharedVariables[key] as Entity;
            }


            //var key = "json" + settingskey;
            //if (context != null && context.SharedVariables != null && context.SharedVariables.ContainsKey(key))
            //{
            //    var json = context.SharedVariables[key] as string;
            //    if (String.IsNullOrEmpty(json))
            //    {
            //        _settings = null;
            //        return;
            //    }
            //    _settings = JsonToEntity(json);
            //}
        }

        private void SaveToSharedVariables(IPluginExecutionContext context)
        {
            if (context == null || context.SharedVariables == null) return;
            var key = settingskey;
            if (context.SharedVariables.ContainsKey(key)) return;
            context.SharedVariables.Add(key, _settings);

            //if (_settings != null)
            //{
            //    key = "json" + settingskey;
            //    if (context.SharedVariables.ContainsKey(key)) return;
            //    var json = EntityToJson(_settings);
            //    if (!String.IsNullOrEmpty(json)) context.SharedVariables.Add(key, json);
            //}
            
        }


        /// <summary>
        /// Get a single setting.
        /// If the attribute does not exists, the method will return the following based on type
        /// String: null, OptionSetValue: null, byte[]: null, Money: null, EntityReference: null
        /// bool:false, int:0, double:0.0, decimal:0, datetime: DateTime.MinValue
        /// </summary>
        /// <typeparam name="T">Attribute Type, ex: String, OptionSetValue, int</typeparam>
        /// <param name="field">Attribute Name</param>
        /// <returns>Attribute Value</returns>
        public T GetSingleSetting<T>(string field)
        {
            return (T)_settings.GetAttributeValue<T>(field);
        }

        /// <summary>
        /// Get a single setting as a string
        /// </summary>
        /// <param name="field">The attribute name on the settings entity</param>
        /// <param name="fieldType">The type of CRM attribute being retrieved</param>
        /// <param name="useCache">True [Default] to use the settings cache, false to make a separate call to CRM</param>
        /// <returns>String version of the setting</returns>
        public string GetSingleSetting(string field, string fieldType, bool useCache = true)
        {
            try
            {
                #region Get setting
                Entity settings = _settings;

                if (!useCache)
                {
                    #region Get settings for this attribute
                    QueryExpression query = new QueryExpression()
                    {
                        EntityName = "mcs_setting",
                        ColumnSet = new ColumnSet(field),
                        NoLock = true,
                        TopCount = 1,
                        Criteria = new FilterExpression()
                    };
                    query.Criteria.AddCondition("mcs_name", ConditionOperator.Equal, _SystemSetting);


                    EntityCollection results = _Service.RetrieveMultiple(query);
                    settings = results.Entities[0];
                    #endregion
                }
                #endregion

                if (settings == null || !settings.Contains(field))
                    return null;

                switch (fieldType.ToLower())
                {
                    case "entityreference":
                        return ((EntityReference)settings[field]).Id.ToString();
                    case "optionsetvalue":
                        return ((OptionSetValue)settings[field]).Value.ToString();
                    case "bool":
                        return ((bool)settings[field]).ToString();
                    case "string":
                        return settings[field].ToString();
                    default:
                        return null;
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                _Logger.setMethod = "GetSingleSettings";
                _Logger.WriteToFile(ex.Message);
                return null;

            }
            catch (Exception ex)
            {
                _Logger.setMethod = "GetSingleSettings";
                _Logger.WriteToFile(ex.Message);
                return null;
            }
        }
    }
}
