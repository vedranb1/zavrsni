using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Redis.Context;
using Redis.Models;
using Redis.ViewModels;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.Controllers
{
    public class HomeController : Controller
    {
        private readonly MysqlContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IDatabase _redisDb;

        public HomeController(
            MysqlContext context, 
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager, 
            IConnectionMultiplexer multiplexer)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _redisDb = multiplexer.GetDatabase();
        }

        public async Task<IActionResult> Index(HomeIndexViewModel model)
        {
            model.ItemsCounter = 0;
            model.DarkTheme = false;

            if (_signInManager.IsSignedIn(User))
            {
                string username = _userManager.GetUserName(User);
                string key = username + "dm";

                int darkMode = (int)await _redisDb.StringGetAsync(key);

                model.DarkTheme = (darkMode == 1) ? true : false;
                model.ItemsCounter = (int) await _redisDb.ListLengthAsync(username);
            }

            return View(model);
        }

        public async Task<IActionResult> Settings(SettingsViewModel model)
        {
            string username = _userManager.GetUserName(User);
            string key = username + "dm";

            int darkMode = (int) await _redisDb.StringGetAsync(key);

            model.DarkTheme = (darkMode == 1) ? true : false;
            model.ItemsCounter = (int) await _redisDb.ListLengthAsync(username);

            return View(model);
        }

        public IActionResult Registration()
        {
            return View();
        }

        public IActionResult Login(int counter)
        {
            HomeLoginViewModel model = new HomeLoginViewModel()
            {
                FailedAttempts = 0
            };

            if (counter != 0)
            {
                model.FailedAttempts = counter;
            }

            return View(model);
        }

        public async Task<IActionResult> Logout(HomeLogoutViewModel model)
        {
            model.DarkTheme = false;
            model.ItemsCounter = 0;

            if (_signInManager.IsSignedIn(User))
            {
                string username = _userManager.GetUserName(User);
                string key = username + "dm";

                int darkMode = (int)await _redisDb.StringGetAsync(key);

                model.DarkTheme = (darkMode == 1) ? true : false;
                model.ItemsCounter = (int)await _redisDb.ListLengthAsync(username);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser(HomeLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    ResetFailedAttempts(model.Username);

                    if (returnUrl == null || returnUrl == "")
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction(returnUrl);
                    }
                } else
                {
                    int counter = await IncrementFailedAttempts(model.Username);

                    return RedirectToAction("Login", new { counter = counter });
                }
            } 

            return View("./Login");
        }
        
        [HttpPost]
        public async Task<IActionResult> RegisterUser(HomeRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser()
                {
                    UserName = model.Username,
                    Email = model.Email,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ModelState.AddModelError("", "Username or Password is incorrect.");
            }

            return View("Registration");
        }

        [HttpPost]
        public async Task<IActionResult> LogoutUser()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult DontLogoutUser()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SaveSettings(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                string username = _userManager.GetUserName(User);
                string key = username + "dm";

                int val = model.DarkTheme ? 1 : 0;

                await _redisDb.StringSetAsync(key, val);
            }

            return View("Settings", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<int> IncrementFailedAttempts(string username)
        {
            string key = username + "fa"; 

            return (int) await _redisDb.StringIncrementAsync(key);
        }
            
        public async void ResetFailedAttempts(string username)
        {
            string key = username + "fa";

            await _redisDb.KeyDeleteAsync(key);
        }
    }
}
