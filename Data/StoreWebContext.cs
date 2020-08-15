using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreWeb.Models;

namespace StoreWeb.Data
{
    public class StoreWebContext : DbContext
    {
        public StoreWebContext (DbContextOptions<StoreWebContext> options)
            : base(options)
        {
        }

        public DbSet<StoreWeb.Models.PackageRequest> PackageRequest { get; set; }
    }
}
