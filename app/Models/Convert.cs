using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using StoreLib.Models;
using StoreLib.Services;

namespace StoreWeb.Models
{
    public class AlternateAppIDs
    {
        public string IDType { get; set; }
        public string Value { get; set; }
    }
}
