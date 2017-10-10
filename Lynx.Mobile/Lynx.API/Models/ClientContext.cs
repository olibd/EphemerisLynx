using System;
using Microsoft.EntityFrameworkCore;

namespace Lynx.API.Models
{
    public class ClientContext : DbContext
    {
        public ClientContext(DbContextOptions<ClientContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                        .HasIndex(c => c.APIKey)
                        .IsUnique();
        }

        public DbSet<Client> Clients { get; set; }
    }
}
