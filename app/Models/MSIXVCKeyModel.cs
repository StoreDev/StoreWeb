using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreWeb.Models
{
    public class MSIXVCKeyModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ProductId { get; set; }
        public string PackageName { get; set; }
        public Guid CikGuid { get; set; }
        public string Base64Key { get; set; }
    }
}
