﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AirMonit_DU.CommunicationInfastructure {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Entry", Namespace="http://schemas.datacontract.org/2004/07/Communication_Infrastructure")]
    [System.SerializableAttribute()]
    public partial class Entry : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private AirMonit_DU.CommunicationInfastructure.City cityField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime dateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int idField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private AirMonit_DU.CommunicationInfastructure.Parameter parameterField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal valueField;
        
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
        public AirMonit_DU.CommunicationInfastructure.City city {
            get {
                return this.cityField;
            }
            set {
                if ((this.cityField.Equals(value) != true)) {
                    this.cityField = value;
                    this.RaisePropertyChanged("city");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime date {
            get {
                return this.dateField;
            }
            set {
                if ((this.dateField.Equals(value) != true)) {
                    this.dateField = value;
                    this.RaisePropertyChanged("date");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int id {
            get {
                return this.idField;
            }
            set {
                if ((this.idField.Equals(value) != true)) {
                    this.idField = value;
                    this.RaisePropertyChanged("id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public AirMonit_DU.CommunicationInfastructure.Parameter parameter {
            get {
                return this.parameterField;
            }
            set {
                if ((this.parameterField.Equals(value) != true)) {
                    this.parameterField = value;
                    this.RaisePropertyChanged("parameter");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal value {
            get {
                return this.valueField;
            }
            set {
                if ((this.valueField.Equals(value) != true)) {
                    this.valueField = value;
                    this.RaisePropertyChanged("value");
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="City", Namespace="http://schemas.datacontract.org/2004/07/Communication_Infrastructure")]
    public enum City : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        LEIRIA = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        COIMBRA = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        LISBOA = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        PORTO = 3,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Parameter", Namespace="http://schemas.datacontract.org/2004/07/Communication_Infrastructure")]
    public enum Parameter : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        NITROGEN_DIOXIDE = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        CARBON_MONOXIDE = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OZONE = 2,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CommunicationInfastructure.ServiceCommunication")]
    public interface ServiceCommunication {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ServiceCommunication/TesteDoservice", ReplyAction="http://tempuri.org/ServiceCommunication/TesteDoserviceResponse")]
        string TesteDoservice();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ServiceCommunication/TesteDoservice", ReplyAction="http://tempuri.org/ServiceCommunication/TesteDoserviceResponse")]
        System.Threading.Tasks.Task<string> TesteDoserviceAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ServiceCommunication/addEntry", ReplyAction="http://tempuri.org/ServiceCommunication/addEntryResponse")]
        void addEntry(AirMonit_DU.CommunicationInfastructure.Entry entry);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ServiceCommunication/addEntry", ReplyAction="http://tempuri.org/ServiceCommunication/addEntryResponse")]
        System.Threading.Tasks.Task addEntryAsync(AirMonit_DU.CommunicationInfastructure.Entry entry);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ServiceCommunication/getEntries", ReplyAction="http://tempuri.org/ServiceCommunication/getEntriesResponse")]
        AirMonit_DU.CommunicationInfastructure.Entry[] getEntries();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ServiceCommunication/getEntries", ReplyAction="http://tempuri.org/ServiceCommunication/getEntriesResponse")]
        System.Threading.Tasks.Task<AirMonit_DU.CommunicationInfastructure.Entry[]> getEntriesAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ServiceCommunication/getRecentEntries", ReplyAction="http://tempuri.org/ServiceCommunication/getRecentEntriesResponse")]
        AirMonit_DU.CommunicationInfastructure.Entry[] getRecentEntries(int id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ServiceCommunication/getRecentEntries", ReplyAction="http://tempuri.org/ServiceCommunication/getRecentEntriesResponse")]
        System.Threading.Tasks.Task<AirMonit_DU.CommunicationInfastructure.Entry[]> getRecentEntriesAsync(int id);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ServiceCommunicationChannel : AirMonit_DU.CommunicationInfastructure.ServiceCommunication, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceCommunicationClient : System.ServiceModel.ClientBase<AirMonit_DU.CommunicationInfastructure.ServiceCommunication>, AirMonit_DU.CommunicationInfastructure.ServiceCommunication {
        
        public ServiceCommunicationClient() {
        }
        
        public ServiceCommunicationClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceCommunicationClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceCommunicationClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceCommunicationClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string TesteDoservice() {
            return base.Channel.TesteDoservice();
        }
        
        public System.Threading.Tasks.Task<string> TesteDoserviceAsync() {
            return base.Channel.TesteDoserviceAsync();
        }
        
        public void addEntry(AirMonit_DU.CommunicationInfastructure.Entry entry) {
            base.Channel.addEntry(entry);
        }
        
        public System.Threading.Tasks.Task addEntryAsync(AirMonit_DU.CommunicationInfastructure.Entry entry) {
            return base.Channel.addEntryAsync(entry);
        }
        
        public AirMonit_DU.CommunicationInfastructure.Entry[] getEntries() {
            return base.Channel.getEntries();
        }
        
        public System.Threading.Tasks.Task<AirMonit_DU.CommunicationInfastructure.Entry[]> getEntriesAsync() {
            return base.Channel.getEntriesAsync();
        }
        
        public AirMonit_DU.CommunicationInfastructure.Entry[] getRecentEntries(int id) {
            return base.Channel.getRecentEntries(id);
        }
        
        public System.Threading.Tasks.Task<AirMonit_DU.CommunicationInfastructure.Entry[]> getRecentEntriesAsync(int id) {
            return base.Channel.getRecentEntriesAsync(id);
        }
    }
}
