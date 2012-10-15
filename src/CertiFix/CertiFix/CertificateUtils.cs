using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CertiFix
{
    public class CertificateUtils
    {
        private X509Certificate2Collection ExpiringCertificates { get; set; }
        public int DaysBeforeEnd { get; set; }

        public X509Certificate2Collection Check()
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            var certificates = store.Certificates
                .Find(X509FindType.FindByTimeValid, DateTime.Now, false)
                .Find(X509FindType.FindByTimeExpired, DateTime.Now.AddDays(DaysBeforeEnd), false);
            store.Close();

            ExpiringCertificates = certificates;
            return ExpiringCertificates;
        }

        internal X509Certificate2Collection GetEncryptionCerts(DateTime newerThan)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            var certificates = store.Certificates
                .Find(X509FindType.FindByTimeValid, DateTime.Now, false)
                .Find(X509FindType.FindByTimeNotYetValid, newerThan, false)
                .Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.DataEncipherment, false);

            store.Close();
            return certificates;
        }
    }
}
