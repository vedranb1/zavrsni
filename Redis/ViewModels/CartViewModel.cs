using Redis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.ViewModels
{
    public class CartViewModel : BaseViewModel
    {
        public List<Game> CartItems { get; set; }

        public decimal TotalPrice()
        {
            return CartItems.Aggregate((decimal) 0, (acc, item) => acc + item.Price);
        }
    }
}
