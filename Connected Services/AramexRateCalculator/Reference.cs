﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AramexRateCalculator
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;






    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ClientInfo", Namespace="http://ws.aramex.net/ShippingAPI/v1/")]
    public partial class ClientInfo : object
    {
        
        private string UserNameField;
        
        private string PasswordField;
        
        private string VersionField;
        
        private string AccountNumberField;
        
        private string AccountPinField;
        
        private string AccountEntityField;
        
        private string AccountCountryCodeField;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string UserName
        {
            get
            {
                return this.UserNameField;
            }
            set
            {
                this.UserNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=1)]
        public string Password
        {
            get
            {
                return this.PasswordField;
            }
            set
            {
                this.PasswordField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=2)]
        public string Version
        {
            get
            {
                return this.VersionField;
            }
            set
            {
                this.VersionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=3)]
        public string AccountNumber
        {
            get
            {
                return this.AccountNumberField;
            }
            set
            {
                this.AccountNumberField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=4)]
        public string AccountPin
        {
            get
            {
                return this.AccountPinField;
            }
            set
            {
                this.AccountPinField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=5)]
        public string AccountEntity
        {
            get
            {
                return this.AccountEntityField;
            }
            set
            {
                this.AccountEntityField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=6)]
        public string AccountCountryCode
        {
            get
            {
                return this.AccountCountryCodeField;
            }
            set
            {
                this.AccountCountryCodeField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Transaction", Namespace="http://ws.aramex.net/ShippingAPI/v1/")]
    public partial class Transaction : object
    {
        
        private string Reference1Field;
        
        private string Reference2Field;
        
        private string Reference3Field;
        
        private string Reference4Field;
        
        private string Reference5Field;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Reference1
        {
            get
            {
                return this.Reference1Field;
            }
            set
            {
                this.Reference1Field = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Reference2
        {
            get
            {
                return this.Reference2Field;
            }
            set
            {
                this.Reference2Field = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Reference3
        {
            get
            {
                return this.Reference3Field;
            }
            set
            {
                this.Reference3Field = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Reference4
        {
            get
            {
                return this.Reference4Field;
            }
            set
            {
                this.Reference4Field = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Reference5
        {
            get
            {
                return this.Reference5Field;
            }
            set
            {
                this.Reference5Field = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Address", Namespace="http://ws.aramex.net/ShippingAPI/v1/")]
    public partial class Address : object
    {
        
        private string Line1Field;
        
        private string Line2Field;
        
        private string Line3Field;
        
        private string CityField;
        
        private string StateOrProvinceCodeField;
        
        private string PostCodeField;
        
        private string CountryCodeField;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Line1
        {
            get
            {
                return this.Line1Field;
            }
            set
            {
                this.Line1Field = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Line2
        {
            get
            {
                return this.Line2Field;
            }
            set
            {
                this.Line2Field = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Line3
        {
            get
            {
                return this.Line3Field;
            }
            set
            {
                this.Line3Field = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=3)]
        public string City
        {
            get
            {
                return this.CityField;
            }
            set
            {
                this.CityField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=4)]
        public string StateOrProvinceCode
        {
            get
            {
                return this.StateOrProvinceCodeField;
            }
            set
            {
                this.StateOrProvinceCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=5)]
        public string PostCode
        {
            get
            {
                return this.PostCodeField;
            }
            set
            {
                this.PostCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=6)]
        public string CountryCode
        {
            get
            {
                return this.CountryCodeField;
            }
            set
            {
                this.CountryCodeField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ShipmentDetails", Namespace="http://ws.aramex.net/ShippingAPI/v1/")]
    public partial class ShipmentDetails : object
    {
        
        private AramexRateCalculator.Dimensions DimensionsField;
        
        private AramexRateCalculator.Weight ActualWeightField;
        
        private AramexRateCalculator.Weight ChargeableWeightField;
        
        private string DescriptionOfGoodsField;
        
        private string GoodsOriginCountryField;
        
        private int NumberOfPiecesField;
        
        private string ProductGroupField;
        
        private string ProductTypeField;
        
        private string PaymentTypeField;
        
        private string PaymentOptionsField;
        
        private AramexRateCalculator.Money CustomsValueAmountField;
        
        private AramexRateCalculator.Money CashOnDeliveryAmountField;
        
        private AramexRateCalculator.Money InsuranceAmountField;
        
        private AramexRateCalculator.Money CashAdditionalAmountField;
        
        private AramexRateCalculator.Money CollectAmountField;
        
        private string ServicesField;
        
        private AramexRateCalculator.ShipmentItem[] ItemsField;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public AramexRateCalculator.Dimensions Dimensions
        {
            get
            {
                return this.DimensionsField;
            }
            set
            {
                this.DimensionsField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=1)]
        public AramexRateCalculator.Weight ActualWeight
        {
            get
            {
                return this.ActualWeightField;
            }
            set
            {
                this.ActualWeightField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=2)]
        public AramexRateCalculator.Weight ChargeableWeight
        {
            get
            {
                return this.ChargeableWeightField;
            }
            set
            {
                this.ChargeableWeightField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=3)]
        public string DescriptionOfGoods
        {
            get
            {
                return this.DescriptionOfGoodsField;
            }
            set
            {
                this.DescriptionOfGoodsField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=4)]
        public string GoodsOriginCountry
        {
            get
            {
                return this.GoodsOriginCountryField;
            }
            set
            {
                this.GoodsOriginCountryField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=5)]
        public int NumberOfPieces
        {
            get
            {
                return this.NumberOfPiecesField;
            }
            set
            {
                this.NumberOfPiecesField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=6)]
        public string ProductGroup
        {
            get
            {
                return this.ProductGroupField;
            }
            set
            {
                this.ProductGroupField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=7)]
        public string ProductType
        {
            get
            {
                return this.ProductTypeField;
            }
            set
            {
                this.ProductTypeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=8)]
        public string PaymentType
        {
            get
            {
                return this.PaymentTypeField;
            }
            set
            {
                this.PaymentTypeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=9)]
        public string PaymentOptions
        {
            get
            {
                return this.PaymentOptionsField;
            }
            set
            {
                this.PaymentOptionsField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=10)]
        public AramexRateCalculator.Money CustomsValueAmount
        {
            get
            {
                return this.CustomsValueAmountField;
            }
            set
            {
                this.CustomsValueAmountField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=11)]
        public AramexRateCalculator.Money CashOnDeliveryAmount
        {
            get
            {
                return this.CashOnDeliveryAmountField;
            }
            set
            {
                this.CashOnDeliveryAmountField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=12)]
        public AramexRateCalculator.Money InsuranceAmount
        {
            get
            {
                return this.InsuranceAmountField;
            }
            set
            {
                this.InsuranceAmountField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=13)]
        public AramexRateCalculator.Money CashAdditionalAmount
        {
            get
            {
                return this.CashAdditionalAmountField;
            }
            set
            {
                this.CashAdditionalAmountField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=14)]
        public AramexRateCalculator.Money CollectAmount
        {
            get
            {
                return this.CollectAmountField;
            }
            set
            {
                this.CollectAmountField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=15)]
        public string Services
        {
            get
            {
                return this.ServicesField;
            }
            set
            {
                this.ServicesField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=16)]
        public AramexRateCalculator.ShipmentItem[] Items
        {
            get
            {
                return this.ItemsField;
            }
            set
            {
                this.ItemsField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Dimensions", Namespace="http://ws.aramex.net/ShippingAPI/v1/")]
    public partial class Dimensions : object
    {
        
        private int LengthField;
        
        private int WidthField;
        
        private int HeightField;
        
        private string UnitField;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Length
        {
            get
            {
                return this.LengthField;
            }
            set
            {
                this.LengthField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Width
        {
            get
            {
                return this.WidthField;
            }
            set
            {
                this.WidthField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=2)]
        public int Height
        {
            get
            {
                return this.HeightField;
            }
            set
            {
                this.HeightField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=3)]
        public string Unit
        {
            get
            {
                return this.UnitField;
            }
            set
            {
                this.UnitField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Weight", Namespace="http://ws.aramex.net/ShippingAPI/v1/")]
    public partial class Weight : object
    {
        
        private string UnitField;
        
        private double ValueField;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Unit
        {
            get
            {
                return this.UnitField;
            }
            set
            {
                this.UnitField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public double Value
        {
            get
            {
                return this.ValueField;
            }
            set
            {
                this.ValueField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Money", Namespace="http://ws.aramex.net/ShippingAPI/v1/")]
    public partial class Money : object
    {
        
        private string CurrencyCodeField;
        
        private double ValueField;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string CurrencyCode
        {
            get
            {
                return this.CurrencyCodeField;
            }
            set
            {
                this.CurrencyCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public double Value
        {
            get
            {
                return this.ValueField;
            }
            set
            {
                this.ValueField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ShipmentItem", Namespace="http://ws.aramex.net/ShippingAPI/v1/")]
    public partial class ShipmentItem : object
    {
        
        private string PackageTypeField;
        
        private int QuantityField;
        
        private AramexRateCalculator.Weight WeightField;
        
        private string CommentsField;
        
        private string ReferenceField;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string PackageType
        {
            get
            {
                return this.PackageTypeField;
            }
            set
            {
                this.PackageTypeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Quantity
        {
            get
            {
                return this.QuantityField;
            }
            set
            {
                this.QuantityField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public AramexRateCalculator.Weight Weight
        {
            get
            {
                return this.WeightField;
            }
            set
            {
                this.WeightField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=3)]
        public string Comments
        {
            get
            {
                return this.CommentsField;
            }
            set
            {
                this.CommentsField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=4)]
        public string Reference
        {
            get
            {
                return this.ReferenceField;
            }
            set
            {
                this.ReferenceField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Notification", Namespace="http://ws.aramex.net/ShippingAPI/v1/")]
    public partial class Notification : object
    {
        
        private string CodeField;
        
        private string MessageField;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Code
        {
            get
            {
                return this.CodeField;
            }
            set
            {
                this.CodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Message
        {
            get
            {
                return this.MessageField;
            }
            set
            {
                this.MessageField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://ws.aramex.net/ShippingAPI/v1/", ConfigurationName="AramexRateCalculator.Service_1_0")]
    public interface Service_1_0
    {
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://ws.aramex.net/ShippingAPI/v1/Service_1_0/CalculateRate", ReplyAction="http://ws.aramex.net/ShippingAPI/v1/Service_1_0/CalculateRateResponse")]
        System.Threading.Tasks.Task<AramexRateCalculator.RateCalculatorResponse> CalculateRateAsync(AramexRateCalculator.RateCalculatorRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RateCalculatorRequest", WrapperNamespace="http://ws.aramex.net/ShippingAPI/v1/", IsWrapped=true)]
    public partial class RateCalculatorRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.aramex.net/ShippingAPI/v1/", Order=0)]
        public AramexRateCalculator.ClientInfo ClientInfo;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.aramex.net/ShippingAPI/v1/", Order=1)]
        public AramexRateCalculator.Transaction Transaction;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.aramex.net/ShippingAPI/v1/", Order=2)]
        public AramexRateCalculator.Address OriginAddress;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.aramex.net/ShippingAPI/v1/", Order=3)]
        public AramexRateCalculator.Address DestinationAddress;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.aramex.net/ShippingAPI/v1/", Order=4)]
        public AramexRateCalculator.ShipmentDetails ShipmentDetails;
                
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.aramex.net/ShippingAPI/v1/", Order=5)]
        public AramexRateCalculator.Money Money;
        
        public RateCalculatorRequest()
        {
        }
        
        public RateCalculatorRequest(AramexRateCalculator.ClientInfo ClientInfo, AramexRateCalculator.Transaction Transaction, AramexRateCalculator.Address OriginAddress, AramexRateCalculator.Address DestinationAddress, AramexRateCalculator.ShipmentDetails ShipmentDetails, AramexRateCalculator.Money Money)
        {
            this.ClientInfo = ClientInfo;
            this.Transaction = Transaction;
            this.OriginAddress = OriginAddress;
            this.DestinationAddress = DestinationAddress;
            this.ShipmentDetails = ShipmentDetails;
            this.Money = Money;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RateCalculatorResponse", WrapperNamespace="http://ws.aramex.net/ShippingAPI/v1/", IsWrapped=true)]
    public partial class RateCalculatorResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.aramex.net/ShippingAPI/v1/", Order=0)]
        public AramexRateCalculator.Transaction Transaction;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.aramex.net/ShippingAPI/v1/", Order=1)]
        public AramexRateCalculator.Notification[] Notifications;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.aramex.net/ShippingAPI/v1/", Order=2)]
        public bool HasErrors;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.aramex.net/ShippingAPI/v1/", Order=3)]
        public AramexRateCalculator.Money TotalAmount;
        
        public RateCalculatorResponse()
        {
        }
        
        public RateCalculatorResponse(AramexRateCalculator.Transaction Transaction, AramexRateCalculator.Notification[] Notifications, bool HasErrors, AramexRateCalculator.Money TotalAmount)
        {
            this.Transaction = Transaction;
            this.Notifications = Notifications;
            this.HasErrors = HasErrors;
            this.TotalAmount = TotalAmount;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public interface Service_1_0Channel : AramexRateCalculator.Service_1_0, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public partial class Service_1_0Client : System.ServiceModel.ClientBase<AramexRateCalculator.Service_1_0>, AramexRateCalculator.Service_1_0
    {


        public Service_1_0Client() :
 base(Service_1_0Client.GetBindingForEndpoint(), Service_1_0Client.GetEndpointAddress())
        {
        }

        public Service_1_0Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
         base(binding, remoteAddress)
        {
        }


        
        public System.Threading.Tasks.Task<AramexRateCalculator.RateCalculatorResponse> CalculateRateAsync(AramexRateCalculator.RateCalculatorRequest request)
        {
            return base.Channel.CalculateRateAsync(request);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }



        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint()
        {
            var httpsBinding = new BasicHttpBinding();
          //  httpsBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
          // httpsBinding.Security.Mode = BasicHttpsSecurityMode.Transport;

            var integerMaxValue = 65536;
            httpsBinding.MaxBufferSize = integerMaxValue;
            httpsBinding.MaxReceivedMessageSize = integerMaxValue;
            httpsBinding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            httpsBinding.AllowCookies = true;

            httpsBinding.ReceiveTimeout = TimeSpan.Parse("00:10:00"); 
            httpsBinding.SendTimeout = TimeSpan.Parse("00:01:00"); 
            httpsBinding.OpenTimeout = TimeSpan.Parse("00:01:00"); 
            httpsBinding.CloseTimeout = TimeSpan.Parse("00:01:00");
            httpsBinding.Security.Mode = (BasicHttpSecurityMode)SecurityMode.None;
            httpsBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            httpsBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
            return httpsBinding;
        }

        private static System.ServiceModel.EndpointAddress GetEndpointAddress()
        {
            return new System.ServiceModel.EndpointAddress("http://ws.aramex.net/shippingapi/ratecalculator/service_1_0.svc");
        }









    }
}