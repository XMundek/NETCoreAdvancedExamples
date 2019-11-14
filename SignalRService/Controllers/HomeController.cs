using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SignalRService.Models;

namespace SignalRService.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult TestCache([FromServices] IMemoryCache _cache)
        {
            List<string> products;

            if (!_cache.TryGetValue("Items", out products))
            {
                products = new List<string> {"P1" + DateTime.Now.ToString(), "P2" + DateTime.Now.ToString(), };
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                options.SetPriority(CacheItemPriority.Low);
                options.SetSlidingExpiration(TimeSpan.FromSeconds(10));
                _cache.Set("Items", products, options);
            }

            return Content(string.Join("<br/>", products),"text/html");
        }

        public IActionResult Index()
        {
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
