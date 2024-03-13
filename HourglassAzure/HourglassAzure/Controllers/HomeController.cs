using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HourglassAzure.Data;
using HourglassAzure.Views;
using HourglassAzure.Models;

namespace HourglassAzure.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var email = User.Identity.Name;
            return View(_context.Tramite.Where(m => m.Email == email).ToList());
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Sua página de descrição do aplicativo.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Sua página de contato.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
