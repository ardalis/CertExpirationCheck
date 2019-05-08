using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace CertCheck
{
    public class ProductionServerCertificates
    {
        [Theory]
        [InlineData("ardalis.com")] // team mentoring site
        [InlineData("devbetter.com")] // individual career mentoring site
        public void MustHaveAtLeast30DaysLeftBeforeExpiring(string domain)
        {
            HttpWebRequest request = WebRequest.CreateHttp($"https://{domain}");
            request.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
            }

        }

        private static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var expirationDate = DateTime.Parse(certificate.GetExpirationDateString());
            if (expirationDate - DateTime.Today < TimeSpan.FromDays(30))
            {
                throw new Exception("Time to renew the certificate!");
            }
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }
            else
            {
                throw new Exception("Cert policy errors: " + sslPolicyErrors.ToString());
            }
        }
    }
}
