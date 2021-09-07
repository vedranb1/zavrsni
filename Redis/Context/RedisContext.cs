using Microsoft.EntityFrameworkCore;
using Redis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.Context
{
    public partial class RedisContext : DbContext
    {
        public RedisContext(DbContextOptions<RedisContext> options) : base(options)
        {

        }

        public DbSet<Korisnik> Korisniks { get; set; }
        public DbSet<Igra> Igras { get; set; }
        public DbSet<KorisnikIgra> KorisnikIgras { get; set; }
    }
}
