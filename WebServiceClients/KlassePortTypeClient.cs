using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace KlasseWebService
{
    public partial class KlassePortTypeClient : System.ServiceModel.ClientBase<KlasseWebService.KlassePortType>, KlasseWebService.KlassePortType
    {
        public KlassePortTypeClient(string endpointUrl, string certPath, string certPassword)
           : base(KlassePortTypeClient.GetBindingForEndpoint(), KlassePortTypeClient.GetEndpointAddress(endpointUrl))
        {
            this.ClientCredentials.ClientCertificate.Certificate = new X509Certificate2(fileName: certPath, password: certPassword);

            // Disable revocation checking
            this.ClientCredentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.OpenTimeout = new TimeSpan(0, 3, 0);
            binding.CloseTimeout = new TimeSpan(0, 3, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 3, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 3, 0);
            binding.SendTimeout = new TimeSpan(0, 3, 0);

            return binding;
        }

        private static System.ServiceModel.EndpointAddress GetEndpointAddress(string endpointUrl)
        {
            return new System.ServiceModel.EndpointAddress(endpointUrl);
        }
    }
}
