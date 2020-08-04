using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paypaycloud.BackgroundWorker.Models
{
    public class CertificateTwins
    {
        public CertificateClient Updater { get; set; }

        public CertificateClient App { get; set; }
    }

    public class CertificateClient
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientKey { get; set; }
        public string ClientIV { get; set; }
        public List<CertificateProtectedServers> ProtectedServers { get; set; }
    }

    public class CertificateProtectedServers
    {
        public string ServerId { get; set; }
        public string ServerName { get; set; }
        public string ServerURL { get; set; }
    }
}
