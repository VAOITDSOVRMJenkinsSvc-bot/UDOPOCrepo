//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VRM.Integration.Servicebus.Bgs.Services.PdfServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PdfServiceResponse", Namespace="http://schemas.datacontract.org/2004/07/VRM.CRME.DocGen.PDFConversion.Service")]
    [System.SerializableAttribute()]
    public partial class PdfServiceResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] OutputFileBytesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool SucceededField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] OutputFileBytes {
            get {
                return this.OutputFileBytesField;
            }
            set {
                if ((object.ReferenceEquals(this.OutputFileBytesField, value) != true)) {
                    this.OutputFileBytesField = value;
                    this.RaisePropertyChanged("OutputFileBytes");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Succeeded {
            get {
                return this.SucceededField;
            }
            set {
                if ((this.SucceededField.Equals(value) != true)) {
                    this.SucceededField = value;
                    this.RaisePropertyChanged("Succeeded");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PdfServiceReference.IPdfService")]
    public interface IPdfService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPdfService/ConvertWordToPdf", ReplyAction="http://tempuri.org/IPdfService/ConvertWordToPdfResponse")]
        VRM.Integration.Servicebus.Bgs.Services.PdfServiceReference.PdfServiceResponse ConvertWordToPdf(byte[] wordBytes, string fileName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPdfService/ConvertWordToPdf", ReplyAction="http://tempuri.org/IPdfService/ConvertWordToPdfResponse")]
        System.Threading.Tasks.Task<VRM.Integration.Servicebus.Bgs.Services.PdfServiceReference.PdfServiceResponse> ConvertWordToPdfAsync(byte[] wordBytes, string fileName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPdfServiceChannel : VRM.Integration.Servicebus.Bgs.Services.PdfServiceReference.IPdfService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PdfServiceClient : System.ServiceModel.ClientBase<VRM.Integration.Servicebus.Bgs.Services.PdfServiceReference.IPdfService>, VRM.Integration.Servicebus.Bgs.Services.PdfServiceReference.IPdfService {
        
        public PdfServiceClient() {
        }
        
        public PdfServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PdfServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PdfServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PdfServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public VRM.Integration.Servicebus.Bgs.Services.PdfServiceReference.PdfServiceResponse ConvertWordToPdf(byte[] wordBytes, string fileName) {
            return base.Channel.ConvertWordToPdf(wordBytes, fileName);
        }
        
        public System.Threading.Tasks.Task<VRM.Integration.Servicebus.Bgs.Services.PdfServiceReference.PdfServiceResponse> ConvertWordToPdfAsync(byte[] wordBytes, string fileName) {
            return base.Channel.ConvertWordToPdfAsync(wordBytes, fileName);
        }
    }
}
