using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Redis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.Context
{
    public partial class MysqlContext : IdentityDbContext
    {
        public MysqlContext(DbContextOptions<MysqlContext> options) : base(options)
        {

        }

        public DbSet<Game> Games { get; set; }
    }
}
