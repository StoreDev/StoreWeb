using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using StoreLib.Models;
using StoreLib.Services;

namespace StoreWeb.Models
{
    public class Search
    {
        public string query { get; set; }
        public DeviceFamily devicefamily { get; set; }
        public DCatEndpoint environment { get; set; }
        public Market market { get; set; }
        public Lang lang { get; set; }
        public string msatoken { get; set; }
    }

    public class Results
    {
        public string title { get; set; }
        public string type { get; set; }
        public string productid { get; set; }
    }
}
