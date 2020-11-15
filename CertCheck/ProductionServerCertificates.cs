using System;
using System.Globalization;
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
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
            }
        }

        private static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //see: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.x509certificate.getexpirationdatestring?view=netcore-3.1#remarks
            //Make sure we parse the DateTime.Parse(expirationdate) the same as GetExpirationDateString() does.
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var expirationDate = DateTime.Parse(certificate.GetExpirationDateString(), CultureInfo.InvariantCulture);
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
