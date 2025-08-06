using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PovVoyage.Data.Entities;

namespace PovVoyage.Data.Context
{
    public class PovVoyageContext : DbContext
    {
        public PovVoyageContext(DbContextOptions<PovVoyageContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<VideoMetadata> VideoMetadata { get; set; }
    }
}
