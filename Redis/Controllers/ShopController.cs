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
        private readonly IConnectionMultiplexer _multiplexer;
        private readonly IDatabase _redisDb;

        public ShopController(
            MysqlContext context, 
            UserManager<IdentityUser> userManager,
            IConnectionMultiplexer multiplexer)
        {
            _context = context;
            _userManager = userManager;
            _multiplexer = multiplexer;
            _redisDb = multiplexer.GetDatabase();
        }

        public async Task<IActionResult> Shop(ShopViewModel model)
        {
            if (ModelState.IsValid)
            {
                string username = _userManager.GetUserName(User);

                model.ItemsCounter = (int) await _redisDb.ListLengthAsync(username);
                model.Games = _context.Games.ToList();
            }

            return View(model);
        }

        public async Task<IActionResult> Cart(CartViewModel model)
        {
            if (ModelState.IsValid)
            {
                List<Game> itemsInCart = new List<Game>();

                string username = _userManager.GetUserName(User);
                var listLength = await _redisDb.ListLengthAsync(username);

                for (int i = 0; i < listLength; i++)
                {
                    int gameId = (int)await _redisDb.ListGetByIndexAsync(username, i);
                    var game = _context.Games.FirstOrDefault(game => game.Id == gameId);

                    if (game != null)
                    {
                        itemsInCart.Add(game);
                    }
                }

                model.CartItems = itemsInCart;
                model.ItemsCounter = itemsInCart.Count;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CreateUserGame userGame)
        {
            string username = _userManager.GetUserName(User);

            var res = await _redisDb.ListRightPushAsync(username, userGame.id);
            var listLength = await _redisDb.ListLengthAsync(username);

            return Ok(listLength);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveUserGame userGame)
        {
            string username = _userManager.GetUserName(User);

            var res = _redisDb.ListRemoveAsync(username, userGame.id, 1);
            var listLength = await _redisDb.ListLengthAsync(username);

            return Ok(listLength);
        }

        [HttpPost]
        public async Task<IActionResult> AddGame(ShopViewModel model)
        {
            if (ModelState.IsValid)
            {
                var game = new Game()
                {
                    Name = model.Name,
                    Genre = model.Genre,
                    Price = model.Price,
                    ImageUrl = model.ImageUrl
                };

                await _context.Games.AddAsync(game);
                await _context.SaveChangesAsync();
            }

            model.Games = _context.Games.ToList();
            model.ItemsCounter = model.Games.Count;

            return View("Shop", model);
        }
    }
}
