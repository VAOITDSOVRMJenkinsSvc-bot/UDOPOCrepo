﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VRM.Integration.Servicebus.Egain.UnitTest.ServicebusServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServicebusServiceReference.IServicebusWcf")]
    public interface IServicebusWcf {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicebusWcf/Send", ReplyAction="http://tempuri.org/IServicebusWcf/SendResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(VRM.Integration.Servicebus.Core.ServicebusMessageWcf))]
        void Send(object messageWcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicebusWcf/Send", ReplyAction="http://tempuri.org/IServicebusWcf/SendResponse")]
        System.Threading.Tasks.Task SendAsync(object messageWcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicebusWcf/SendReceive", ReplyAction="http://tempuri.org/IServicebusWcf/SendReceiveResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(VRM.Integration.Servicebus.Core.ServicebusMessageWcf))]
        object SendReceive(object messageWcf);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicebusWcf/SendReceive", ReplyAction="http://tempuri.org/IServicebusWcf/SendReceiveResponse")]
        System.Threading.Tasks.Task<object> SendReceiveAsync(object messageWcf);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicebusWcfChannel : VRM.Integration.Servicebus.Egain.UnitTest.ServicebusServiceReference.IServicebusWcf, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServicebusWcfClient : System.ServiceModel.ClientBase<VRM.Integration.Servicebus.Egain.UnitTest.ServicebusServiceReference.IServicebusWcf>, VRM.Integration.Servicebus.Egain.UnitTest.ServicebusServiceReference.IServicebusWcf {
        
        public ServicebusWcfClient() {
        }
        
        public ServicebusWcfClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServicebusWcfClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicebusWcfClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServicebusWcfClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void Send(object messageWcf) {
            base.Channel.Send(messageWcf);
        }
        
        public System.Threading.Tasks.Task SendAsync(object messageWcf) {
            return base.Channel.SendAsync(messageWcf);
        }
        
        public object SendReceive(object messageWcf) {
            return base.Channel.SendReceive(messageWcf);
        }
        
        public System.Threading.Tasks.Task<object> SendReceiveAsync(object messageWcf) {
            return base.Channel.SendReceiveAsync(messageWcf);
        }
    }
}
