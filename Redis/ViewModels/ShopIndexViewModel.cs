using Redis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.ViewModels
{
    public class ShopIndexViewModel : BaseViewModel
    {
        public List<Game> Games { get; set; }
    }
}
