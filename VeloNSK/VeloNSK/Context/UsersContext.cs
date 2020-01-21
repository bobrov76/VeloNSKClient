using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VeloNSK.APIServise.Model;

namespace VeloNSK.Context
{
    class UsersContext : DbContext
    {
        public DbSet<InfoUser> Users { get; set; }
        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
