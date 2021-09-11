using Microsoft.AspNetCore.Mvc;
using Redis.Context;
using Redis.Models;
using Redis.ViewModels;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.Controllers
{
    public class ManagementController : Controller
    {
        private readonly MysqlContext _context;
        private readonly IDatabase _redisDb;
        public ManagementController(MysqlContext context, IConnectionMultiplexer multiplexer)
        {
            _context = context;
            _redisDb = multiplexer.GetDatabase();
        }

        public IActionResult Index(ManagementIndexViewModel model)
        {
            model.Games = _context.Games.ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateGame(ManagementIndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == -1)
                {
                    var game = new Game()
                    {
                        Name = model.Name,
                        Genre = model.Genre,
                        Price = model.Price,
                        ImageUrl = model.ImageUrl
                    };

                    await _context.Games.AddAsync(game);
                }
                else
                {
                    var existingGame = _context.Games.FirstOrDefault(g => g.Id == model.Id);

                    existingGame.Name = model.Name;
                    existingGame.Genre = model.Genre;
                    existingGame.Price = model.Price;
                    existingGame.ImageUrl = model.ImageUrl;

                    _context.Games.Update(existingGame);
                }

                await _context.SaveChangesAsync();
            }

            model.Games = _context.Games.ToList();

            return View("Index", model);
        }
    }
}
