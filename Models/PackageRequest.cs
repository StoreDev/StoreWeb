using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using StoreLib.Models;
using StoreLib.Services;

namespace StoreWeb.Models
{
    public class PackageRequest
    {
        public IdentiferType type { get; set; }
        public string id { get; set; }
        public DCatEndpoint environment { get; set; }
        public Market market { get; set; }
        public Lang lang { get; set; }
        public string msatoken { get; set; }
    }

    public class Packages
    {
        public string packagemoniker { get; set; }
        public string packagedownloadurl { get; set; }
    }
}
