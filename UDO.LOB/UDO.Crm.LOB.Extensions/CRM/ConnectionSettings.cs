/*
The ServicePoint class handles connections to an Internet resource based
on the host information passed in the resource's Uniform Resource Identifier 
(URI). The initial connection to the resource determines the information that 
the ServicePoint object maintains, which is then shared by all subsequent 
requests to that resource.

ServicePoint objects are managed by the ServicePointManager class and are 
created, if necessary, by the System.Net.ServicePointManager.FindServicePoint 
method. ServicePoint objects are never created directly but are always created 
and managed by the ServicePointManager class. The maximum number of ServicePoint 
objects that can be created is set by the ServicePointManager.MaxServicePoints 
property.

Each ServicePoint object maintains its connection to an Internet resource until 
it has been idle longer than the time specified in the ServicePoint.MaxIdleTime 
property. When a ServicePoint exceeds the ServicePoint.MaxIdleTime value, it can 
be recycled to another connection. The default value of ServicePoint.MaxIdleTime 
is set by the ServicePointManager.MaxServicePointIdleTime property.

When the ServicePoint.ConnectionLeaseTimeout property is set to a value other 
than -1, and after the specified time elapses, an active ServicePoint connection 
is closed after it services the next request. This is useful for applications 
that do not require active connections that are opened indefinitely, as they 
are by default.
*/



using System;
using System.Net;
using System.Xml.Serialization;

namespace UDO.LOB.Extensions
{
    [Serializable]
    public sealed class ConnectionSettings
    {
        [XmlElement("connection-limit")]
        public int ConnectionLimit { get; set; }

        [XmlElement("max-idle-time")]
        public int MaxIdleTime { get; set; }

        [XmlElement("connection-lease-timeout")]
        public int ConnectionLeaseTimeout { get; set; }

        [XmlElement("expect-100-continue")]
        public bool? Expect100Continue { get; set; }

        [XmlElement("use-nagle-algorithm")]
        public bool? UseNagleAlgorithm { get; set; }

        public string UpdateServicePoint(ServicePoint sp)
        {
            if (sp == null) return "ServicePoint null.  No settings updated.";

            var result = "Service Endpoint Details:\r\n"
                         + string.Format("URI: {0}\r\n", sp.Address)
                         + string.Format("Connection Limit: {0}\r\n", sp.ConnectionLimit)
                         + string.Format("Expect 100 Continue: {0}\r\n", sp.Expect100Continue)
                         + string.Format("UseNagleAlgorithm: {0}\r\n\r\n", sp.UseNagleAlgorithm);

            if (ConnectionLimit !=0 && ConnectionLimit != sp.ConnectionLimit)
            {
                result += string.Format("* Updating Connection Limit to: {0}\r\n", ConnectionLimit);
                sp.ConnectionLimit = ConnectionLimit;
            }
            if (ConnectionLeaseTimeout != 0  && ConnectionLeaseTimeout != sp.ConnectionLeaseTimeout)
            {
                result += string.Format("* Updating Connection Lease Timeout to: {0}\r\n", ConnectionLeaseTimeout);
                sp.ConnectionLeaseTimeout = ConnectionLeaseTimeout;
            }
            if (MaxIdleTime != 0 && MaxIdleTime != sp.MaxIdleTime)
            {
                result += string.Format("* Updating Max Idle Time to: {0}\r\n", MaxIdleTime);
                sp.MaxIdleTime = MaxIdleTime;
            }
            if (Expect100Continue.HasValue)
            {
                sp.Expect100Continue = Expect100Continue.Value;
                result += string.Format("* Turning {0} Expect100Continue", Expect100Continue.Value ? "on" : "off");
            }
            if (UseNagleAlgorithm.HasValue)
            {
                sp.UseNagleAlgorithm = UseNagleAlgorithm.Value;
                result += string.Format("* Turning {0} UseNagleAlgorithm (connection reliability)",
                    UseNagleAlgorithm.Value ? "on" : "off");
            }

            return result;
        }

        public string UpdateServicePointManager()
        {
            var result = "";
            if (ConnectionLimit != 0)
            {
                ServicePointManager.DefaultConnectionLimit = ConnectionLimit;
                result += string.Format("* Increasing Default Connection Limit to: {0}\r\n", ConnectionLimit);
            }
            if (Expect100Continue.HasValue)
            {
                ServicePointManager.Expect100Continue = Expect100Continue.Value;
                result += string.Format("* Turning {0} Expect100Continue", Expect100Continue.Value ? "on" : "off");
            }
            if (UseNagleAlgorithm.HasValue)
            {
                ServicePointManager.UseNagleAlgorithm = UseNagleAlgorithm.Value;
                result += string.Format("* Turning {0} UseNagleAlgorithm (connection reliability)",
                    UseNagleAlgorithm.Value ? "on" : "off");
            }
            return result;
        }
    }
}