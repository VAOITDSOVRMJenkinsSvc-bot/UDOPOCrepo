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

        //addition
        private static bool _verbosetracing;
        public bool getVerbose
        {
            get { return _verbosetracing; }
        }

        private static bool _logtoappinsights;
        public bool getLogtoAppInsights
        {
            get { return _logtoappinsights; }
        }

        private static bool _ebenefits_logtoai;
        public bool geteBenefitsLogToAI
        {
            get { return _ebenefits_logtoai; }
        }

        private static bool _veteransnapshot_logtoai;
        public bool getVeteranSnapshotLogToAI
        {
            get { return _veteransnapshot_logtoai; }
        }

        private static bool _idproof_logtoai;
        public bool getIDProofLogToAI
        {
            get { return _idproof_logtoai; }
        }

        private static bool _interaction_logtoai;
        public bool getInteractionLogToAI
        {
            get { return _interaction_logtoai; }
        }

        private static bool _request_logtoai;
        public bool getRequestLogToAI
        {
            get { return _request_logtoai; }
        }
        private static bool _ssrs_logtoai;
        public bool getSSRSLogToAI
        {
            get { return _ssrs_logtoai; }
        }

        private static bool _vbms_logtoai;
        public bool getVBMSLogToAI
        {
            get { return _vbms_logtoai; }
        }

        private static bool _mvisearch_logtoai;
        public bool getMVISearchLogToAI
        {
            get { return _mvisearch_logtoai; }
        }

        private static bool _awards_logtoai;
        public bool getAwardsLogToAI
        {
            get { return _awards_logtoai; }
        }

        private static bool _claims_logtoai;
        public bool getClaimsLogToAI
        {
            get { return _claims_logtoai; }
        }

        private static bool _contacts_logtoai;
        public bool getContactsLogToAI
        {
            get { return _contacts_logtoai; }
        }

        private static bool _fiduciary_logtoai;
        public bool getFiduciaryLogToAI
        {
            get { return _fiduciary_logtoai; }
        }

        private static bool _flash_logtoai;
        public bool getFlashLogToAI
        {
            get { return _flash_logtoai; }
        }

        private static bool _letter_logtoai;
        public bool getLetterLogToAI
        {
            get { return _letter_logtoai; }
        }

        private static bool _notes_logtoai;
        public bool getNotesLogToAI
        {
            get { return _notes_logtoai; }
        }

        private static bool _getratings_logtoai;
        public bool getRatingsLogToAI
        {
            get { return _getratings_logtoai; }
        }

        private static bool _vbms_efolder_logtoai;
        public bool getVBMSEFolderLogToAI
        {
            get { return _vbms_efolder_logtoai; }
        }

        private static bool _generatedocument_logtoai;
        public bool getGenerateDocumentLogToAI
        {
            get { return _generatedocument_logtoai; }
        }

        private static bool _servicerequest_logtoai;
        public bool getServiceRequestLogToAI
        {
            get { return _servicerequest_logtoai; }
        }
               
        private static string _appinsightskey;
        public string getAppInsightsKey
        {
            get { return _appinsightskey; }
        }
        private static string _appinsightsurl;
        public string getAppInsightsURL
        {
            get { return _appinsightsurl; }
        }
        //end addition

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
                    //addition
                    if(_settings.Contains("udo_verbosetracing") && !String.IsNullOrEmpty("udo_verbosetracing"))
                    {
                        _verbosetracing = _settings.GetAttributeValue<bool>("udo_verbosetracing");
                    }
                    
                    if (_settings.Contains("udo_logtoappinsights") && !String.IsNullOrEmpty("udo_logtoappinsights"))
                    {
                        _logtoappinsights = _settings.GetAttributeValue<bool>("udo_logtoappinsights");
                    }

                    if ((_logtoappinsights == true) && (_settings.Contains("udo_appinsightskey")) && (_settings.Contains("udo_appinsightsurl")))
                    {
                        _appinsightskey = _settings.GetAttributeValue<string>("udo_appinsightskey");
                        _appinsightsurl = _settings.GetAttributeValue<string>("udo_appinsightsurl");
                    }
                    else
                    {
                        _appinsightskey = String.Empty;
                        _appinsightsurl = String.Empty;
                    }

                    if (_logtoappinsights == true)
                    {
                        if (_settings.Contains("udo_ebenefits_logtoai"))                        
                            _ebenefits_logtoai = _settings.GetAttributeValue<bool>("udo_ebenefits_logtoai");
                        else
                            _ebenefits_logtoai = false;
                        
                        if (_settings.Contains("udo_veteransnapshot_logtoai"))
                            _veteransnapshot_logtoai = _settings.GetAttributeValue<bool>("udo_veteransnapshot_logtoai");
                        else
                            _veteransnapshot_logtoai = false;
                        if (_settings.Contains("udo_idproof_logtoai"))
                            _idproof_logtoai = _settings.GetAttributeValue<bool>("udo_idproof_logtoai");
                        else
                           _idproof_logtoai = false;
                        if(_settings.Contains("udo_interaction_logtoai"))
                            _interaction_logtoai = _settings.GetAttributeValue<bool>("udo_interaction_logtoai");
                        else
                            _interaction_logtoai = false;
                        if (_settings.Contains("udo_request_logtoai"))
                            _request_logtoai = _settings.GetAttributeValue<bool>("udo_request_logtoai");
                        else
                            _request_logtoai = false;
                        if (_settings.Contains("udo_ssrs_logtoai"))
                            _ssrs_logtoai = _settings.GetAttributeValue<bool>("udo_ssrs_logtoai");
                        else
                            _ssrs_logtoai = false;
                        if (_settings.Contains("udo_vbms_logtoai"))
                            _vbms_logtoai = _settings.GetAttributeValue<bool>("udo_vbms_logtoai");
                        else
                            _vbms_logtoai = false;
                        if (_settings.Contains("udo_mvisearch_logtoai"))
                            _mvisearch_logtoai = _settings.GetAttributeValue<bool>("udo_mvisearch_logtoai");
                        else
                            _mvisearch_logtoai = false;
                        if (_settings.Contains("udo_awards_logtoai"))
                            _awards_logtoai = _settings.GetAttributeValue<bool>("udo_awards_logtoai");
                        else
                            _awards_logtoai = false;
                        if (_settings.Contains("udo_claims_logtoai"))
                            _claims_logtoai = _settings.GetAttributeValue<bool>("udo_claims_logtoai");
                        else
                            _claims_logtoai = false;
                        if (_settings.Contains("udo_contacts_logtoai"))
                            _contacts_logtoai = _settings.GetAttributeValue<bool>("udo_contacts_logtoai");
                        else
                            _contacts_logtoai = false;
                        if (_settings.Contains("udo_fiduciary_logtoai"))
                            _fiduciary_logtoai = _settings.GetAttributeValue<bool>("udo_fiduciary_logtoai");
                        else
                            _fiduciary_logtoai = false;
                        if (_settings.Contains("udo_flash_logtoai"))
                            _flash_logtoai = _settings.GetAttributeValue<bool>("udo_flash_logtoai");
                        else
                            _flash_logtoai = false;
                        if (_settings.Contains("udo_letter_logtoai"))
                            _letter_logtoai = _settings.GetAttributeValue<bool>("udo_letter_logtoai");
                        else
                            _letter_logtoai = false;
                        if (_settings.Contains("udo_notes_logtoai"))
                            _notes_logtoai = _settings.GetAttributeValue<bool>("udo_notes_logtoai");
                        else
                            _notes_logtoai = false;
                        if (_settings.Contains("udo_getratings_logtoai"))
                            _getratings_logtoai = _settings.GetAttributeValue<bool>("udo_getratings_logtoai");
                        else
                            _getratings_logtoai = false;
                        if (_settings.Contains("udo_vbms_efolder_logtoai"))
                            _vbms_efolder_logtoai = _settings.GetAttributeValue<bool>("udo_vbms_efolder_logtoai");
                        else
                            _vbms_efolder_logtoai = false;
                        if (_settings.Contains("udo_generatedocument_logtoai"))
                            _generatedocument_logtoai = _settings.GetAttributeValue<bool>("udo_generatedocument_logtoai");
                        else
                            _generatedocument_logtoai = false;
                        if (_settings.Contains("udo_servicerequest_logtoai"))
                            _servicerequest_logtoai = _settings.GetAttributeValue<bool>("udo_servicerequest_logtoai");
                        else
                            _servicerequest_logtoai = false;
                    }
                    //end addition
                }
                //retrieve the value of the new setting fields here.
                #endregion
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                _Logger.setMethod = "GetStartupSettings";
                _Logger.WriteToFile(ex.Message);
                throw new InvalidPluginExecutionException(ex.Message);
            }
            catch (Exception ex)
            {
                _Logger.setMethod = "GetStartupSettings";
                _Logger.WriteToFile(ex.Message);
                throw new InvalidPluginExecutionException(ex.Message);
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
                throw new InvalidPluginExecutionException(ex.Message);
                return null;

            }
            catch (Exception ex)
            {
                _Logger.setMethod = "GetSingleSettings";
                _Logger.WriteToFile(ex.Message);
                throw new InvalidPluginExecutionException(ex.Message);
                return null;
            }
        }
    }
}
