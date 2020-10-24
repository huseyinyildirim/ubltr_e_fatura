﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ubltr.INGUBL {
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gib.gov.tr/vedop3/eFatura")]
    public partial class EFaturaFaultType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int codeField;
        
        private bool codeFieldSpecified;
        
        private string msgField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public int code {
            get {
                return this.codeField;
            }
            set {
                this.codeField = value;
                this.RaisePropertyChanged("code");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool codeSpecified {
            get {
                return this.codeFieldSpecified;
            }
            set {
                this.codeFieldSpecified = value;
                this.RaisePropertyChanged("codeSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string msg {
            get {
                return this.msgField;
            }
            set {
                this.msgField = value;
                this.RaisePropertyChanged("msg");
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
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://gib.gov.tr/vedop3/eFatura", ConfigurationName="INGUBL.EFaturaPortType")]
    public interface EFaturaPortType {
        
        // CODEGEN: Generating message contract since the wrapper name (getAddressInfoRequest) of message getAddressInfo does not match the default value (getAddressInfo)
        [System.ServiceModel.OperationContractAttribute(Action="getAddressInfo", ReplyAction="*")]
        [System.ServiceModel.FaultContractAttribute(typeof(ubltr.INGUBL.EFaturaFaultType), Action="getAddressInfo", Name="EFaturaFault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        ubltr.INGUBL.getAddressInfoResponse getAddressInfo(ubltr.INGUBL.getAddressInfo request);
        
        // CODEGEN: Generating message contract since the operation getApplicationResponse is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="getApplicationResponse", ReplyAction="*")]
        [System.ServiceModel.FaultContractAttribute(typeof(ubltr.INGUBL.EFaturaFaultType), Action="getApplicationResponse", Name="EFaturaFault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        ubltr.INGUBL.getApplicationResponseResponse getApplicationResponse(ubltr.INGUBL.getApplicationResponse request);
        
        // CODEGEN: Generating message contract since the operation sendDocument is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="sendDocument", ReplyAction="*")]
        [System.ServiceModel.FaultContractAttribute(typeof(ubltr.INGUBL.EFaturaFaultType), Action="sendDocument", Name="EFaturaFault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        ubltr.INGUBL.sendDocumentResponse sendDocument(ubltr.INGUBL.sendDocument request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="getAddressInfoRequest", WrapperNamespace="http://gib.gov.tr/vedop3/eFatura", IsWrapped=true)]
    public partial class getAddressInfo {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gib.gov.tr/vedop3/eFatura", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string identifier;
        
        public getAddressInfo() {
        }
        
        public getAddressInfo(string identifier) {
            this.identifier = identifier;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="getAddressInfoResponse", WrapperNamespace="http://gib.gov.tr/vedop3/eFatura", IsWrapped=true)]
    public partial class getAddressInfoResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gib.gov.tr/vedop3/eFatura", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool addressFound;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gib.gov.tr/vedop3/eFatura", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute("aliasList", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string[] aliasList;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gib.gov.tr/vedop3/eFatura", Order=2)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string identifier;
        
        public getAddressInfoResponse() {
        }
        
        public getAddressInfoResponse(bool addressFound, string[] aliasList, string identifier) {
            this.addressFound = addressFound;
            this.aliasList = aliasList;
            this.identifier = identifier;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gib.gov.tr/vedop3/eFatura")]
    public partial class getAppRespRequestType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string instanceIdentifierField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string instanceIdentifier {
            get {
                return this.instanceIdentifierField;
            }
            set {
                this.instanceIdentifierField = value;
                this.RaisePropertyChanged("instanceIdentifier");
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gib.gov.tr/vedop3/eFatura")]
    public partial class getAppRespResponseType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string applicationResponseField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string applicationResponse {
            get {
                return this.applicationResponseField;
            }
            set {
                this.applicationResponseField = value;
                this.RaisePropertyChanged("applicationResponse");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class getApplicationResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gib.gov.tr/vedop3/eFatura", Order=0)]
        public ubltr.INGUBL.getAppRespRequestType getAppRespRequest;
        
        public getApplicationResponse() {
        }
        
        public getApplicationResponse(ubltr.INGUBL.getAppRespRequestType getAppRespRequest) {
            this.getAppRespRequest = getAppRespRequest;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class getApplicationResponseResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gib.gov.tr/vedop3/eFatura", Order=0)]
        public ubltr.INGUBL.getAppRespResponseType getAppRespResponse;
        
        public getApplicationResponseResponse() {
        }
        
        public getApplicationResponseResponse(ubltr.INGUBL.getAppRespResponseType getAppRespResponse) {
            this.getAppRespResponse = getAppRespResponse;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gib.gov.tr/vedop3/eFatura")]
    public partial class documentType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string fileNameField;
        
        private base64Binary binaryDataField;
        
        private string hashField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string fileName {
            get {
                return this.fileNameField;
            }
            set {
                this.fileNameField = value;
                this.RaisePropertyChanged("fileName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public base64Binary binaryData {
            get {
                return this.binaryDataField;
            }
            set {
                this.binaryDataField = value;
                this.RaisePropertyChanged("binaryData");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string hash {
            get {
                return this.hashField;
            }
            set {
                this.hashField = value;
                this.RaisePropertyChanged("hash");
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2005/05/xmlmime")]
    public partial class base64Binary : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string contentTypeField;
        
        private byte[] valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string contentType {
            get {
                return this.contentTypeField;
            }
            set {
                this.contentTypeField = value;
                this.RaisePropertyChanged("contentType");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute(DataType="base64Binary")]
        public byte[] Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
                this.RaisePropertyChanged("Value");
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://gib.gov.tr/vedop3/eFatura")]
    public partial class documentReturnType : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string msgField;
        
        private string hashField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string msg {
            get {
                return this.msgField;
            }
            set {
                this.msgField = value;
                this.RaisePropertyChanged("msg");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string hash {
            get {
                return this.hashField;
            }
            set {
                this.hashField = value;
                this.RaisePropertyChanged("hash");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class sendDocument {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gib.gov.tr/vedop3/eFatura", Order=0)]
        public ubltr.INGUBL.documentType documentRequest;
        
        public sendDocument() {
        }
        
        public sendDocument(ubltr.INGUBL.documentType documentRequest) {
            this.documentRequest = documentRequest;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class sendDocumentResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://gib.gov.tr/vedop3/eFatura", Order=0)]
        public ubltr.INGUBL.documentReturnType documentResponse;
        
        public sendDocumentResponse() {
        }
        
        public sendDocumentResponse(ubltr.INGUBL.documentReturnType documentResponse) {
            this.documentResponse = documentResponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface EFaturaPortTypeChannel : ubltr.INGUBL.EFaturaPortType, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class EFaturaPortTypeClient : System.ServiceModel.ClientBase<ubltr.INGUBL.EFaturaPortType>, ubltr.INGUBL.EFaturaPortType {
        
        public EFaturaPortTypeClient() {
            
        }
        
        public EFaturaPortTypeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public EFaturaPortTypeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public EFaturaPortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public EFaturaPortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ubltr.INGUBL.getAddressInfoResponse ubltr.INGUBL.EFaturaPortType.getAddressInfo(ubltr.INGUBL.getAddressInfo request) {
            return base.Channel.getAddressInfo(request);
        }
        
        public bool getAddressInfo(ref string identifier, out string[] aliasList) {
            ubltr.INGUBL.getAddressInfo inValue = new ubltr.INGUBL.getAddressInfo();
            inValue.identifier = identifier;
            ubltr.INGUBL.getAddressInfoResponse retVal = ((ubltr.INGUBL.EFaturaPortType)(this)).getAddressInfo(inValue);
            aliasList = retVal.aliasList;
            identifier = retVal.identifier;
            return retVal.addressFound;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ubltr.INGUBL.getApplicationResponseResponse ubltr.INGUBL.EFaturaPortType.getApplicationResponse(ubltr.INGUBL.getApplicationResponse request) {
            return base.Channel.getApplicationResponse(request);
        }
        
        public ubltr.INGUBL.getAppRespResponseType getApplicationResponse(ubltr.INGUBL.getAppRespRequestType getAppRespRequest) {
            ubltr.INGUBL.getApplicationResponse inValue = new ubltr.INGUBL.getApplicationResponse();
            inValue.getAppRespRequest = getAppRespRequest;
            ubltr.INGUBL.getApplicationResponseResponse retVal = ((ubltr.INGUBL.EFaturaPortType)(this)).getApplicationResponse(inValue);
            return retVal.getAppRespResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ubltr.INGUBL.sendDocumentResponse ubltr.INGUBL.EFaturaPortType.sendDocument(ubltr.INGUBL.sendDocument request) {
            return base.Channel.sendDocument(request);
        }
        
        public ubltr.INGUBL.documentReturnType sendDocument(ubltr.INGUBL.documentType documentRequest) {
            ubltr.INGUBL.sendDocument inValue = new ubltr.INGUBL.sendDocument();
            inValue.documentRequest = documentRequest;
            ubltr.INGUBL.sendDocumentResponse retVal = ((ubltr.INGUBL.EFaturaPortType)(this)).sendDocument(inValue);
            return retVal.documentResponse;
        }
    }
}
