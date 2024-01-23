using FunShare_Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace FunShare_Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FUNShareContext _context;

        public HomeController(ILogger<HomeController> logger, FUNShareContext c)
        {
            _logger = logger;
            _context = c;
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