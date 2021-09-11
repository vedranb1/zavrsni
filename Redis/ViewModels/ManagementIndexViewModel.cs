using Microsoft.AspNetCore.Identity;
using Redis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.ViewModels
{
    public class ManagementIndexViewModel : BaseViewModel
    {
        public List<Game> Games { get; set; }
        public List<IdentityUser> Users { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
