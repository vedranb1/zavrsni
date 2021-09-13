using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Redis.Context;
using Redis.DTOs;
using Redis.Models;
using Redis.ViewModels;
using ServiceStack.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.Controllers
{
    public class ShopController : Controller
    {
        private readonly MysqlContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IDatabase _redisDb;

        public ShopController(
            MysqlContext context, 
            UserManager<IdentityUser> userManager,
            IConnectionMultiplexer multiplexer)
        {
            _context = context;
            _userManager = userManager;
            _redisDb = multiplexer.GetDatabase();
        }

        public async Task<IActionResult> Index(ShopIndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                var games = _context.Games.ToList();
                string username = _userManager.GetUserName(User);
                string key = username + "dm";

                int listLength = (int) await _redisDb.ListLengthAsync(username);
                int darkMode = (int)await _redisDb.StringGetAsync(key);

                for (int i = 0; i < listLength; i++)
                {
                    var game = (int) await _redisDb.ListGetByIndexAsync(username, i);

                    games.Remove(_context.Games.First(c => c.Id == game));
                }

                model.Games = games;
                model.DarkTheme = (darkMode == 1) ? true : false;
                model.ItemsCounter = listLength;
            }

            return View(model);
        }

        public async Task<IActionResult> Cart(ShopCartViewModel model)
        {
            if (ModelState.IsValid)
            {
                List<Game> itemsInCart = new List<Game>();

                string username = _userManager.GetUserName(User);
                string key = username + "dm";

                int listLength = (int) await _redisDb.ListLengthAsync(username);
                int darkMode = (int)await _redisDb.StringGetAsync(key);

                for (int i = 0; i < listLength; i++)
                {
                    int gameId = (int) await _redisDb.ListGetByIndexAsync(username, i);
                    var game = _context.Games.FirstOrDefault(game => game.Id == gameId);

                    if (game != null)
                    {
                        itemsInCart.Add(game);
                    }
                }

                model.CartItems = itemsInCart;
                model.DarkTheme = (darkMode == 1) ? true : false;
                model.ItemsCounter = itemsInCart.Count;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CreateUserGame userGame)
        {
            string username = _userManager.GetUserName(User);

            var listLength = await _redisDb.ListRightPushAsync(username, userGame.id);
            
            return Ok(listLength);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveUserGame userGame)
        {
            string username = _userManager.GetUserName(User);

            var listLength = await _redisDb.ListRemoveAsync(username, userGame.id, 1);

            return Ok(listLength);
        }
    }
}
