using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using MCSHelperClass;

namespace MCSUtilities2011
{
    public interface IMCSLogger
    {
        ITracingService setTracingService { set; }
        IOrganizationService setService { set; }
        string setMethod { get; set; }
        string setEntityName { get; set; }
        Guid setEntityId { get; set; }
        string setModule { get; set; }
        bool setDebug { set; }
        bool setTxnTiming { set; }
        bool setGranularTiming { set; }
        void WriteToFile(string message);
        void WriteTxnTimingMessage(string message);
        void WriteGranularTimingMessage(string message);
        void WriteDebugMessage(string message);
    }

    [Serializable]
    public class MCSLogger : IMCSLogger
    {
        private int _sequence = 1;

        public Guid CallingUserId { get; set; }

        public Guid CorrelationId { get; set; }

        public string OrganizationName { get; set; }

        private ITracingService _tracingService;
        public ITracingService setTracingService
        {
            set { _tracingService = value; }
        }
        private IOrganizationService _service;
        public IOrganizationService setService
        {
            set { _service = value; }
        }
        private string _method;
        public string setMethod
        {
            get { return _method; }
            set { _method = value; }
        }

        private string _relatedEntityName;
        public string setEntityName
        {
            get { return _relatedEntityName; }
            set { _relatedEntityName = value; }
        }
        private Guid _relatedEntityId;
        public Guid setEntityId
        {
            get { return _relatedEntityId; }
            set { _relatedEntityId = value; }
        }
        private string _module;
        public string setModule
        {
            get { return _module; }
            set { _module = value; }
        }

        private bool _debug;
        public bool setDebug
        {
            set { _debug = value; }
        }
        private bool _txnTiming;
        public bool setTxnTiming
        {
            set { _txnTiming = value; }
        } private bool _granularTiming;
        public bool setGranularTiming
        {
            set { _granularTiming = value; }
        }
        private void writeOutMessage(string message, bool debugMessage, bool granularTiming, bool TxnTiming)
        {
            try
            {

                Entity logCreate = new Entity("mcs_log");

                logCreate["mcs_name"] = _module;
                //DateTime myNow = DateTime.Now;
                //var extendedTIWEnd = myNow.Hour.ToString("00") + ":" + myNow.Minute.ToString("00") + ":" + myNow.Second.ToString("00") + ":" + myNow.Millisecond.ToString("000");

                string traceEntry = $"[{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff")}] {CorrelationId} {OrganizationName} {CallingUserId} {_method} {_relatedEntityId} {_relatedEntityName} {message}";

                // message = extendedTIWEnd + " --" + message;
                logCreate["mcs_errormessage"] = traceEntry;
                if (debugMessage)
                {
                    logCreate["mcs_debugmessage"] = true;
                }
                if (granularTiming)
                {
                    logCreate["mcs_grantiming"] = true;
                }
                if (TxnTiming)
                {
                    logCreate["mcs_txntiming"] = true;
                }

                if (!_relatedEntityId.ToString().StartsWith("000"))
                {
                    logCreate["mcs_entityid"] = _relatedEntityId.ToString();
                }
                if (_relatedEntityName != null)
                {
                    logCreate["mcs_entityname"] = _relatedEntityName;
                }
                logCreate["mcs_method"] = _method;
                logCreate["mcs_sequence"] = _sequence;
                _sequence += 1;
                if (_tracingService != null)
                {
                    _tracingService.Trace(message);
                }
               
                //replace with Don's log
                _service.Create(logCreate);

            }
            catch (FaultException<OrganizationServiceFault>)
            {
               // WriteToFile(ex.Message);
                return;
            }
            catch (Exception)
            {
               // WriteToFile(ex.Message);
            }
        }
        private void writeOutMessage(string message, bool debugMessage, bool granularTiming, bool TxnTiming, Decimal duration)
        {
            try
            {

                Entity logCreate = new Entity("mcs_log");

                logCreate["mcs_name"] = _module;
                DateTime myNow = DateTime.Now;
                // var extendedTIWEnd = myNow.Hour.ToString("00") + ":" + myNow.Minute.ToString("00") + ":" + myNow.Second.ToString("00") + ":" + myNow.Millisecond.ToString("000");
                //message = extendedTIWEnd + " --" + message;

                string traceEntry = $"[{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff")}] {CorrelationId} {OrganizationName} {CallingUserId} {_method} {_relatedEntityId} {_relatedEntityName} {message}";
                logCreate["mcs_errormessage"] = traceEntry;
                if (debugMessage)
                {
                    logCreate["mcs_debugmessage"] = true;
                }
                if (granularTiming)
                {
                    logCreate["mcs_grantiming"] = true;
                }
                if (TxnTiming)
                {
                    logCreate["mcs_txntiming"] = true;
                }

                if (!_relatedEntityId.ToString().StartsWith("000"))
                {
                    logCreate["mcs_entityid"] = _relatedEntityId.ToString();
                }
                if (_relatedEntityName != null)
                {
                    logCreate["mcs_entityname"] = _relatedEntityName;
                }
                if (duration != 0)
                {
                    logCreate["crme_duration"] = duration;
                    logCreate["crme_loglevel"] = new OptionSetValue(935950005);
                }
               
                logCreate["mcs_method"] = _method;
                logCreate["mcs_sequence"] = _sequence;
                _sequence += 1;
                if (_tracingService != null)
                {
                    _tracingService.Trace(message);
                }

                //replace with Don's log
                _service.Create(logCreate);

            }
            catch (FaultException<OrganizationServiceFault>)
            {
                // WriteToFile(ex.Message);
                return;
            }
            catch (Exception)
            {
                // WriteToFile(ex.Message);
            }
        }
        public void WriteToFile(string message)
        {
            writeOutMessage(message,false,false,false);

        }
        public void WriteToFile(string message, params object[] args)
        {
            WriteToFile(String.Format(message, args));
        }
        
        //public void WriteException(FaultException<OrganizationServiceFault> ex)
        //{
        //    Exception fault = ex;
        //    string message = "";
        //    do
        //    {
        //        message += String.Format("Error Message: {0}\r\n\r\nStack Trace: \r\n{1}", fault.Message, fault.StackTrace);
        //        fault = fault.InnerException;
        //    } while (fault != null);

        //    WriteToFile("{0}\r\n\r\n{1}", ex.Message, ex.StackTrace);
            
        //}
        public void WriteException(Exception ex)
        {
            Exception exception = ex;
            string message = "";
            do
            {
                message += String.Format("{0}\r\n\r\nStack Trace: \r\n{1}", exception.Message, exception.StackTrace);
                if ((exception = exception.InnerException) != null)
                {
                    message += "\r\n\r\n\r\nInner Exception:\r\n";
                }
            } while (exception != null);

            WriteToFile(message);
            //WriteToFile("{0}\r\n\r\n{1}", ex.Message, ex.StackTrace);
        }
        public void WriteTxnTimingMessage(string message)
        {
            if (_txnTiming)
            {
                writeOutMessage(message,false, false,true);
            }
        }
        public void WriteTxnTimingMessage(string message, Decimal duration)
        {
            if (_txnTiming)
            {
                writeOutMessage(message, false, false, true, duration);
            }
        }
        public void WriteGranularTimingMessage(string message)
        {
            if (_granularTiming)
            {
                writeOutMessage(message,false,true,false);
            }
        }
        public void WriteDebugMessage(Entity entity, string entityName)
        {
            if (_debug)
            {
                var message = MCSHelper.DumpEntityToString(entityName,entity);
                writeOutMessage(message, true, false, false);
            }
        }
        public void WriteDebugMessage(string message)
        {
            if (_debug)
            {
                writeOutMessage(message,true,false,false);

            }
        }
        public void WriteDebugMessage(string message, params object[] args)
        {
            if (_debug)
            {
                writeOutMessage(String.Format(message, args), true, false, false);

            }
        }
    }
}
