using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Redis.Context;
using Redis.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RedisContext _context;
        public HomeController(ILogger<HomeController> logger, RedisContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var igra = new Igra();
            igra.Cijena = 5.2m;
            igra.Naziv = "Kurac";
            igra.Zanr = "asdasd";

            _context.Add(igra);
            await _context.SaveChangesAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
