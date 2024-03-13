using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HourglassAzure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HourglassAzure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public ApplicationDbContext()
        {
        }

        public DbSet<HourglassAzure.Models.Tramite> Tramite { get; set; }
        public DbSet<HourglassAzure.Models.TramiteDePara> TramiteDePara { get; internal set; }
    }
}
